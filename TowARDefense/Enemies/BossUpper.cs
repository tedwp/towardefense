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
    class BossUpper : Enemy
    {
        public BossUpper(TowARDefense parent_f, int stage)
            : base(parent_f)
        {
            Init(stage);
        }

        public void Init(int stage)
        {
            speed = 6;

            turnSpeed = 0.08f;

            setHealth(250 + (100 * stage));
            setHealthBarSize(16, 2);

            stopsWhenFiring = true;
            turnsWhenFiring = false;

            damageInfluence = new double[] { 0.0, 0.0, 0.0, 0.0, 0.3, 1.0, 0.5 };

            gNode.Model = parent.graSys.boss4Model;
            gtNode.Scale = new Vector3(1.3f, 1.1f, 1.0f);

            Console.WriteLine("turrets Level {0}", stage);

            float height = 2.0f;
            for (int i = 0; i <= stage; i++)
            {
                turrets.Add(new Turrets.Attacker.Boss1(this, new Vector3(0, -1.5f, height), parent));
                turrets.Add(new Turrets.Attacker.Boss1(this, new Vector3(0, +1.5f, height), parent));
                height += 1.4f;
            }
        }

        protected override void Init()
        {
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
