using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.IModel;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;
using GoblinXNA.UI.UI2D;
using Sound = GoblinXNA.Sounds.Sound;

namespace TowARDefense
{
    abstract public class Enemy : DestroyableObject
    {
        protected GeometryNode gNode;

        private List<Pathfinding.Node> pNode;
        public Vector2 wantedDir;
        protected float turnSpeed;

        public List<Turret> turrets;

        // Hat immer die Länge der enum GameWeapons
        public double[] damageInfluence;

        public bool stopsWhenFiring;
        public bool stopsForMainBuilding;
        public bool stop;
        public bool turnsWhenFiring;

        protected float speed;
        protected double timeExpired;

        public Enemy(TowARDefense parent_f) : base(parent_f)
        {
            turrets = new List<Turret>();

            pNode = new List<Pathfinding.Node>();
            state = ObjectState.WaitingForSpawn;

            gNode = new GeometryNode("en_gn_" + parent.logSys.enemies.Count.ToString());
            gtNode.AddChild(gNode);

            stop = false;
            stopsForMainBuilding = true;

            turnSpeed = 0.05f;

            Init();
        }

        abstract protected void Init();

        public void Spawn(Pathfinding.Node spawnNode)
        {
            pNode = solvePathfinding(spawnNode, damageInfluence);

            setPosition(spawnNode.position2d);

            Vector2 wantedDir = pNode[0].position2d - position2d;
            Vector2 dir = new Vector2(0, 1);

            parent.graSys.enemyNode.AddChild(gtNode);

            parent.logSys.enemies.Add(this);

            state = ObjectState.Moving;
        }

        public void Update(double timePassed)
        {
            if (state == ObjectState.Moving)
            {
                UpdateMoving(timePassed);
            }
            if (state == ObjectState.Destroyed)
            {
                timeExpired += timePassed;
                if (timeExpired >= 8.0)
                {
                    state = ObjectState.Cleanup;
                }
            }
            stop = false;
            foreach (Turret t in turrets)
            {
                if (t.target == parent.logSys.mainBuilding && stopsForMainBuilding)
                {
                    stop = true;
                }
                else
                {
                    stop = stopsWhenFiring && t.stop;
                }
                t.Update(timePassed);
            }
        }

        virtual protected void UpdateMoving(double timePassed)
        {
            if (!stop)
            {
                float stepSize = speed * (float)timePassed;

                wantedDir = pNode[0].position2d - position2d;

                dir = Vector2.Lerp(dir, wantedDir, turnSpeed);

                dir.Normalize();

                if ((position2d - pNode[0].position2d).Length() < stepSize)
                {
                    position2d = pNode[0].position2d;
                    if (pNode.Count <= 1)
                    {
                        pNode = solvePathfinding(pNode[0], damageInfluence);
                    }
                    else
                    {
                        pNode.RemoveAt(0);
                    }
                }
                else
                {

                    position2d = position2d + stepSize * dir;
                    setPosition(position2d, dir);
                }
            }
        }

        virtual protected List<Pathfinding.Node> solvePathfinding(Pathfinding.Node pos, double[] damageInfluence)
        {
            List<Pathfinding.Node> n = parent.logSys.kiSys.pathGrid.solvePathfinding(pos, damageInfluence);
            dir = n[0].position2d - pos.position2d;
            dir.Normalize();
            return n;
        }

        public void Cleanup()
        {
            if (gtNode == null)
                return;

            foreach (Turret t in turrets)
            {
                t.Cleanup();
            }

            gtNode.RemoveChild(gNode);
            parent.graSys.enemyNode.RemoveChild(gtNode);

            gNode = null;
            gtNode = null;
        }
    }
}