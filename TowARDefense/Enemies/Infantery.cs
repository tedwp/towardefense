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
    class Infantery : Enemy
    {

        public Infantery(TowARDefense parent_f)
            : base(parent_f)
        {
        }

        protected override void Init()
        {
            speed = 5;
            turnSpeed = 0.06f;

            setHealth(30);
            setHealthBarSize(16, 2);

            stopsWhenFiring = true;
            turnsWhenFiring = false;

            damageInfluence = new double[] { 0.0, 0.0, 0.0, 0.0, 0.5, 0.5, 1.0 };

            gNode.Model = parent.graSys.infModel;

            turrets.Add(new Turrets.Attacker.Infantery(this, new Vector3(0, 0, 3), parent));
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
