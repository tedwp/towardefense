﻿using System;
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

namespace TowARDefense.Towers
{
    class AntiSiegeTower : DefenseTower
    {
        public AntiSiegeTower(Vector3 position, TowARDefense parent_f)
            : base(position, parent_f)
        {
        }

        protected override void Init()
        {
            setHealth(60);
            setHealthBarSize(16, 2);

            buildTime = 3;

            initHeight = -5.0f;
            finalHeight = 5.0f;

            damageInfluence = new double[] { 1.0, 1.0, 0.2, 1.0, 0.0, 0.0, 0.0 };

            gNode.Model = parent.graSys.antiSiegeTowerModel;

            turrets.Add(new Turrets.Defender.AntiSiege(this, new Vector3(0, 0, 6), parent));
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

                parent.logSys.kiSys.pathGrid.pathUpdateTow(this);

                foreach (Turret t in turrets)
                {
                    t.Cleanup();
                }
            }
        }

        public static int getPrice()
        {
            return 30;
        }
    }
}
