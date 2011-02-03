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
    class Boss2 : Enemy
    {
        public Boss2(TowARDefense parent_f)
            : base(parent_f)
        {
        }

        protected override void Init()
        {
            speed = 5;

            turnSpeed = 0.08f;

            setHealth(300);
            setHealthBarSize(16, 2);

            stopsWhenFiring = false;
            turnsWhenFiring = false;

            damageInfluence = new double[] { 0.0, 0.0, 0.0, 0.0, 0.3, 1.0, 0.5 };

            gNode.Model = parent.graSys.boss2Model;

            gNode.Material = new Material();
            gNode.Material.Diffuse = Color.DarkRed.ToVector4();

            gtNode.Scale = new Vector3(1.5f, 1.5f, 1.5f);

            turrets.Add(new Turrets.Attacker.Boss2(this, new Vector3(0, 0, 2.25f), parent));
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
