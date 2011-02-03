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
    abstract public class DefenseTower : DestroyableObject
    {
        public TransformNode tNode;
        protected GeometryNode gNode;

        public List<Turret> turrets;

        public int[] weights;
        public float avRange;

        public double[] damageInfluence;

        public int towerCosts;

        protected float initHeight;
        protected float finalHeight;

        protected float buildTime;
        protected double timeExpired;

        public DefenseTower(Vector3 position, TowARDefense parent_f) : base(parent_f)
        {
            turrets = new List<Turret>();

            tNode = new TransformNode();
            gNode = new GeometryNode();

            weights = new int[7];
            for (int i = 0; i <= 6; i++)
            {
                weights[i] = 0;
            }

            state = ObjectState.Construction;
            timeExpired = 0.0;

            Vector2 position2d = new Vector2(position.X, position.Y);

            setPositionNN(position2d);

            gNode.AddToPhysicsEngine = false;

            Init();

            tNode.Translation = new Vector3(0, 0, initHeight); 

            tNode.AddChild(gNode);
            gtNode.AddChild(tNode);

            parent.graSys.towerNode.AddChild(gtNode);


            calcWeights();
        }

        abstract protected void Init();

        private void calcWeights()
        {
            int i = 0;
            foreach (Turret t in turrets)
            {
                i++;
                avRange += t.range;
                weights[(int)t.weaponType] += 10;
            }
            avRange = avRange / i;
        }

        public void Update(double timePassed)
        {
            timeExpired += timePassed;
            if (state == ObjectState.Construction && timeExpired < buildTime)
            {
                float z = ((float)timeExpired / buildTime) * finalHeight + initHeight;
                tNode.Translation = new Vector3(0, 0, z);  
            }
            if (state == ObjectState.Construction && timeExpired >= buildTime)
            {
                state = ObjectState.Idle;
                timeExpired = 0.0;
            }
            if (state == ObjectState.Idle)
            {
                foreach (Turret t in turrets)
                {
                    t.Update(timePassed);
                }
            }
            if (state == ObjectState.Destroyed)
            {
                timeExpired += timePassed;
                if (timeExpired >= 8.0)
                {
                    state = ObjectState.Cleanup;
                }
            }
        }

        public void Cleanup()
        {
            if (gtNode == null)
                return;

            foreach (Turret t in turrets)
            {
                t.Cleanup();
            }

            tNode.RemoveChild(gNode);
            gtNode.RemoveChild(tNode);
            parent.graSys.towerNode.RemoveChild(gtNode);

            gNode = null;
            tNode = null;
            gtNode = null;
        }
    }
}
