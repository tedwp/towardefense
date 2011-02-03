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
    public class ResourceBuilding : DestroyableObject
    {
        private TransformNode tNode;

        private GeometryNode gNode;

        private TransformNode wingsTNode;
        private GeometryNode wingsGNode;
        private float wingsVel;
        private float wingsAngle;


        private float buildTime;

        private float initHeight;
        private float finalHeight;

        private double timeExpired;
        private int resourcesPerSec;
        private double resTime;

        public ResourceBuilding(Vector3 position, TowARDefense parent_f) : base(parent_f)
        {
            gNode = new GeometryNode();
            tNode = new TransformNode();

            wingsTNode = new TransformNode();
            wingsGNode = new GeometryNode();

            timeExpired = 0.0;

            state = ObjectState.Construction;

            position2d = new Vector2(position.X, position.Y);

            setPosition(position2d);

            Init();

            gNode.AddToPhysicsEngine = false;
        }

        public void Init()
        {
            buildTime = 5.0f;
            resourcesPerSec = 1;

            setHealthBarSize(16, 2);
            setHealth(40);

            initHeight = -10.0f;
            finalHeight = 10f;

            wingsVel = (float)((2 * Math.PI) / 3);
            wingsAngle = 0.0f;

            gNode.Model = parent.graSys.resBModel;

            tNode.Translation = new Vector3(0.0f, 0.0f, initHeight);

            tNode.AddChild(gNode);

            wingsGNode.Model = parent.graSys.resRModel;

            wingsTNode.Translation = new Vector3(0, -1.48f, 6.1f);

            wingsTNode.AddChild(wingsGNode);

            tNode.AddChild(wingsTNode);

            gtNode.AddChild(tNode);
            parent.graSys.resNode.AddChild(gtNode);
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
                resTime += timePassed;
                if (resTime > 20.0)
                {
                    if (resourcesPerSec < 2)
                        resourcesPerSec++;
                    resTime = 0.0;
                }

                //Mühle Drehen
                wingsAngle += (float)timePassed * wingsVel;
                if (wingsAngle >= (float)(2*Math.PI))
                    wingsAngle -= (float)(2*Math.PI);
                wingsTNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, wingsAngle);
            }
            if (state == ObjectState.Idle && timeExpired >= 1.0)
            {
                parent.logSys.resourceCount += resourcesPerSec;
                timeExpired = 0.0;
            }
            if (state == ObjectState.Destroyed && timeExpired >= 8.0)
            {
                state = ObjectState.Cleanup;
            }
        }

        override public void dealDamage(int damage, GameWeapons weaponType)
        {
            health -= damage;
            if (health <= 0)
            {
                Sound.Play("explosion");
                explode();

                tNode.RemoveChild(wingsTNode);

                state = ObjectState.Destroyed;
                timeExpired = 0.0;

                parent.logSys.kiSys.pathGrid.pathUpdateRes(this);
            }
        }

        public void Cleanup()
        {
            if (gtNode == null)
                return;

            if (tNode.Children.Contains(wingsTNode))
                tNode.RemoveChild(wingsTNode);
            wingsTNode.RemoveChild(wingsGNode);

            wingsTNode = null;
            wingsGNode = null;

            tNode.RemoveChild(gNode);
            gtNode.RemoveChild(tNode);

            parent.graSys.resNode.RemoveChild(gtNode);

            gNode = null;
            tNode = null;
            gtNode = null;
        }

        public static int getPrice()
        {
            return 40;
        }
    }
}
