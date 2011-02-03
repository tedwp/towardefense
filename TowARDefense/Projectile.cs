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
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;
using Sound = GoblinXNA.Sounds.Sound;

namespace TowARDefense
{
    public class Projectile
    {
        TowARDefense parent;

        private TransformNode tNode;
        private GeometryNode gNode;

        public DestroyableObject target;

        public GameWeapons weaponType;

        public bool cleanup;

        private float speed;
        private int damage;

        public Projectile(Vector3 source, TowARDefense parent_f)
        {
            parent = parent_f;

            tNode = new TransformNode("projectile_tnode");
            tNode.Translation = source;
            gNode = new GeometryNode("projectile_gnode");

            cleanup = false;
        }

        public void setupWeaponType(GameWeapons weapon, int damage_f)
        {
            setupWeaponType(weapon);
            damage = damage_f;
        }

        public void setupWeaponType(GameWeapons weapon)
        {
            if (weapon == GameWeapons.Infantery)
            {
                Sound.Play("inf_shot");
                weaponType = weapon;

                gNode.Model = parent.graSys.infShotModel;

                damage = 2;
                speed = 70.0f;
            }
            else if (weapon == GameWeapons.Tank)
            {
                Sound.Play("tank_shot");
                weaponType = weapon;

                gNode.Model = parent.graSys.tankShotModel;

                damage = 10;
                speed = 60.0f;
            }
            else if (weapon == GameWeapons.Siege)
            {
                Sound.Play("siege_shot");
                weaponType = weapon;

                gNode.Model = parent.graSys.siegeShotModel;

                damage = 25;
                speed = 30.0f;
            }
            else if (weapon == GameWeapons.AntiInfantery)
            {
                Sound.Play("antiinf_shot");
                weaponType = weapon;

                gNode.Model = parent.graSys.antiInfShotModel;

                damage = 7;
                speed = 50.0f;
            }
            else if (weapon == GameWeapons.AntiTank)
            {
                Sound.Play("antitank_shot");
                weaponType = weapon;

                gNode.Model = parent.graSys.antiTankShotModel;

                damage = 9;
                speed = 60.0f;
            }
            else
            {
                Sound.Play("antisiege_shot");
                weaponType = weapon;

                gNode.Model = parent.graSys.antiSiegeShotModel;

                gNode.Material = new Material();
                gNode.Material.Diffuse = Color.Gold.ToVector4();
                gNode.Material.SpecularPower = 0;

                damage = 7;
                speed = 50.0f;
            }
            tNode.AddChild(gNode);
            parent.graSys.projNode.AddChild(tNode);
        }

        public void Update(double timePassed)
        {
            if (target != null)
            {
                UpdatePath(timePassed);
            }
            else
            {
                cleanup = true;
            }
        }

        private void UpdatePath(double timePassed)
        {
            if (target.state == ObjectState.Cleanup)
                return;
            Vector3 targetVector;
            if (target.GetType().BaseType == typeof(Enemy))
            {
                Enemy e = (Enemy)target;
                if (e.state != ObjectState.Destroyed)
                {
                    targetVector = Vector3.Transform(e.turrets[0].tNode.Translation, e.gtNode.WorldTransformation);
                }
                else
                {
                    targetVector = e.gtNode.Translation;
                }
            }
            else if (target.GetType().BaseType == typeof(DefenseTower))
            {
                DefenseTower d = (DefenseTower)target;
                if (d.state != ObjectState.Destroyed)
                {
                    targetVector = Vector3.Transform(Vector3.Transform(d.turrets[0].tNode.Translation, d.tNode.WorldTransformation), d.gtNode.WorldTransformation);
                }
                else
                {
                    targetVector = Vector3.Transform(d.tNode.Translation, d.gtNode.WorldTransformation);
                }
            }
            else
            {
                targetVector = target.gtNode.Translation;
            }
            Vector3 dir = targetVector  - tNode.Translation;
            float step = (float)timePassed * speed;
            if (dir.Length() < step)
            {
                collides();
            }
            else
            {
                dir.Normalize();
                tNode.Translation = tNode.Translation + dir * step;
                tNode.Rotation = Quaternion.CreateFromAxisAngle(dir, 0);
            }
            if (target.state == ObjectState.Cleanup)
            {
                cleanup = true;
            }
        }

        private void collides()
        {
            if (target != null && target.state != ObjectState.Destroyed && target.state != ObjectState.Cleanup)
            {
                if (weaponType == GameWeapons.Siege)
                    Sound.Play("siege-hitneu");
                target.dealDamage(damage, weaponType);
            }
            cleanup = true;
        }
        
        public void Cleanup()
        {
            target = null;

            tNode.RemoveChild(gNode);
            parent.graSys.projNode.RemoveChild(tNode);

            tNode = null;
            gNode = null;
        }
    }
}
