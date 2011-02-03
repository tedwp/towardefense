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
    abstract public class Turret
    {
        protected TowARDefense parent;
        protected DestroyableObject bearer;

        public TransformNode tNode;
        protected GeometryNode gNode;

        public Vector2 position2d;

        public DestroyableObject target;

        public float range;
        protected double reloadTime;
        private double timeExpired;
        public GameWeapons weaponType;

        public bool stop;

        private double checkForTargetsInterval;
        private double checkForTargetsPassed;

        protected int damOverride;

        protected Vector3 shotPos;

        protected TurretStates state;

        public Turret(DestroyableObject bearer_f, Vector3 pos, TowARDefense parent_f)
        {
            parent = parent_f;
            bearer = bearer_f;

            checkForTargetsInterval = 1.0 + RandomHelper.GetRandomFloat(-0.5f, 0.5f);
            checkForTargetsPassed = 0.0;

            timeExpired = 0.0;

            damOverride = 0;

            target = null;
            stop = false;

            tNode = new TransformNode();
            gNode = new GeometryNode();
            tNode.Translation = pos;

            Init();

            UpdateShotPos();
        }

        abstract protected void Init();

        public void face(DestroyableObject o)
        {
            Vector2 dir = target.position2d - bearer.position2d;

            float bearerRot = (float)Math.Atan2(bearer.dir.Y, bearer.dir.X);

            tNode.Rotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(new Vector3(0,0,1), 0), (Quaternion.CreateFromYawPitchRoll(0, 0, (float)Math.Atan2(dir.Y, dir.X)-bearerRot)));
        }

        public virtual void Update(double timePassed)
        {
            timeExpired += timePassed;
            if (state == TurretStates.Firing)
            {
                UpdateFiring(timePassed);
            }
            if (state == TurretStates.Idle)
            {
                UpdateIdle(timePassed);
            }
            if (state == TurretStates.Reloading)
            {
                UpdateReloading(timePassed);
            }
        }

        private void UpdateIdle(double timePassed)
        {
            checkForTargetsPassed += timePassed;
            if (checkForTargetsPassed >= checkForTargetsInterval)
            {
                checkForEnemies();
                checkForTargetsPassed = 0.0;
            }
        }

        // This needs to be moved to the SubClass -> Different for attacker and defender turrets
        abstract protected void checkForEnemies(); 

        private void UpdateFiring(double timePassed)
        {
            stop = true;

            if (target == null)
            {
                state = TurretStates.Idle;
                target = null;
                stop = false;
                return;
            }

            if ((target.position2d - bearer.position2d).Length() > range)
            {
                state = TurretStates.Idle;
                target = null;
                stop = false;
                return;
            }

            if  (target.state != ObjectState.Destroyed
                && target.state != ObjectState.Cleanup
                && target.state != ObjectState.WaitingForSpawn
                && bearer.state != ObjectState.Destroyed
                && bearer.state != ObjectState.Cleanup
                && bearer.state != ObjectState.WaitingForSpawn
                )
            {
                face(target);

                Projectile p;
                if (bearer.GetType().BaseType == typeof(DefenseTower))
                {          
                    p = new Projectile(Vector3.Transform(Vector3.Transform((tNode.Translation+shotPos), ((DefenseTower)bearer).tNode.WorldTransformation), bearer.gtNode.WorldTransformation), parent);
                }
                else
                {
                    p = new Projectile(Vector3.Transform((tNode.Translation+shotPos), bearer.gtNode.WorldTransformation), parent);
                }
                if (damOverride != 0)
                    p.setupWeaponType(weaponType, damOverride);
                else
                    p.setupWeaponType(weaponType);
                p.target = target;
                parent.logSys.projectiles.Add(p);
                state = TurretStates.Reloading;
                timeExpired = 0.0;
            }
            else
            {
                state = TurretStates.Idle;
                target = null;
                stop = false;
            }
        }

        private void UpdateReloading(double timePassed)
        {
            stop = true;
            timeExpired += timePassed;
            if (timeExpired >= reloadTime)
            {
                UpdateShotPos();
                state = TurretStates.Firing;
                timeExpired = 0.0;
            }
        }

        virtual public void Cleanup()
        {
            if (tNode == null)
                return;

            tNode.RemoveChild(gNode);
            ((TransformNode)(tNode.Parent)).RemoveChild(tNode);

            bearer = null;
            target = null;

            tNode = null;
            gNode = null;
        }

        abstract protected void UpdateShotPos();
    }
}
