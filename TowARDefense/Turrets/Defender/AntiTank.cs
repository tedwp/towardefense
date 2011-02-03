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

namespace TowARDefense.Turrets.Defender
{
    class AntiTank : Turret
    {
        public AntiTank(DestroyableObject bearer, Vector3 pos, TowARDefense parent_f)
            : base(bearer, pos, parent_f)
        {
            tNode.Translation = pos;
        }

        protected override void Init()
        {
            range = 40;
            reloadTime = 1.3;
            weaponType = GameWeapons.AntiTank;

            gNode.Model = parent.graSys.antiTankTurretModel;

            Material enMat = new Material();
            enMat.Ambient = Color.AliceBlue.ToVector4();
            enMat.Diffuse = Color.AliceBlue.ToVector4();
            enMat.SpecularPower = 0;
            gNode.Material = enMat;

            tNode.AddChild(gNode);
            ((DefenseTower)bearer).tNode.AddChild(tNode);
        }

        protected override void checkForEnemies()
        {
            List<DestroyableObject> targets = new List<DestroyableObject>();
            foreach (DestroyableObject e in parent.logSys.enemies)
            {
                if (
                    (e.position2d - bearer.position2d).Length() < range
                    && bearer.state != ObjectState.Destroyed
                    && bearer.state != ObjectState.Construction
                    && bearer.state != ObjectState.WaitingForSpawn
                    && bearer.state != ObjectState.Cleanup
                    && e.state != ObjectState.Destroyed
                    && e.state != ObjectState.Cleanup
                    && e.state != ObjectState.WaitingForSpawn
                    )
                {
                    targets.Add(e);
                }
            }
            if (targets.Count > 0)
            {
                target = targets[RandomHelper.GetRandomInt(targets.Count)];
                state = TurretStates.Firing;
            }
        }

        protected override void UpdateShotPos()
        {
            shotPos = new Vector3(0, 0, 0);
        }
    }
}
