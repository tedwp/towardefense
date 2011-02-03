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
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;
using Sound = GoblinXNA.Sounds.Sound;

namespace TowARDefense.Enemies
{
    class Boss3 : Enemy
    {

        public Boss3(TowARDefense parent_f)
            : base(parent_f)
        {
        }

        protected override void Init()
        {
            speed = 12;
            turnSpeed = 0.001f;

            setHealth(3000);
            setHealthBarSize(16, 2);

            stopsWhenFiring = false;
            turnsWhenFiring = false;
            stopsForMainBuilding = false;

            damageInfluence = new double[] { 0.0, 0.0, 0.0, 0.0, 0.5, 0.5, 1.0 };

            gNode.Model = parent.graSys.boss3Model;

            turrets.Add(new Turrets.Attacker.Boss3(this, new Vector3(0, 0, 0), parent));
            turrets.Add(new Turrets.Attacker.Boss3(this, new Vector3(0, 0, 0), parent));
            turrets.Add(new Turrets.Attacker.Boss3(this, new Vector3(0, 0, 0), parent));
        }

        protected override void setPosition(Vector2 pos2d, Vector2 dir)
        {
            Console.WriteLine("BlaBlaBla");
            Vector3 pos = new Vector3(pos2d.X, pos2d.Y, 1);

            base.setPosition(new Vector3(pos2d.X, pos2d.Y, 20.0f), Vector3.UnitY, dir);
        }

        protected override List<global::TowARDefense.Pathfinding.Node> solvePathfinding(global::TowARDefense.Pathfinding.Node pos, double[] damageInfluence)
        {
            List<Pathfinding.Node> l = new List<global::TowARDefense.Pathfinding.Node>();
            if (pos == parent.logSys.kiSys.pathGrid.targetNodes[0])
            {
                l.Add(parent.logSys.kiSys.pathGrid.spawnNodes[RandomHelper.GetRandomInt(parent.logSys.kiSys.pathGrid.spawnNodes.Count)]);
            }
            else
            {
                l.Add(parent.logSys.kiSys.pathGrid.targetNodes[0]);
            }
            return l;
        }

        public override void dealDamage(int damage, GameWeapons weaponType)
        {
            health -= (int)(damage * damageInfluence[(int)weaponType]);
            if (health <= 0)
            {
                Sound.Play("explosion");
                explode();

                state = ObjectState.Destroyed;
                timeExpired = 0.0;

                foreach (Turret t in turrets)
                {
                    t.Cleanup();
                }
            }
        }
    }
}

