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

namespace TowARDefense.Turrets.Attacker
{
    class Tank : Turret
    {
        public Tank(DestroyableObject bearer, Vector3 pos, TowARDefense parent_f)
            : base(bearer, pos, parent_f)
        {
            tNode.Translation = pos;
        }

        protected override void Init()
        {
            range = 35;
            reloadTime = 2.0;
            weaponType = GameWeapons.Tank;

            gNode.Model = parent.graSys.tankTurretModel;

            tNode.AddChild(gNode);
            bearer.gtNode.AddChild(tNode);
        }

        protected override void checkForEnemies()
        {
            if (bearer == null)
                return;

            List<DestroyableObject> targets = new List<DestroyableObject>();
            foreach (DestroyableObject tower in parent.logSys.towers)
            {
                if (tower == null)
                    continue;
                if (
                    (tower.position2d - bearer.position2d).Length() < range
                    && bearer.state != ObjectState.Destroyed
                    && bearer.state != ObjectState.Construction
                    && bearer.state != ObjectState.WaitingForSpawn
                    && bearer.state != ObjectState.Cleanup
                    && tower.state != ObjectState.Destroyed
                    && tower.state != ObjectState.Cleanup
                    && tower.state != ObjectState.WaitingForSpawn
                    )
                {
                    targets.Add(tower);
                }
            }
            foreach (ResourceBuilding b in parent.logSys.resBuildings)
            {
                if (b == null)
                    continue;
                if (
                    (b.position2d - bearer.position2d).Length() < range
                    && bearer.state != ObjectState.Destroyed
                    && bearer.state != ObjectState.Construction
                    && bearer.state != ObjectState.WaitingForSpawn
                    && bearer.state != ObjectState.Cleanup
                    && b.state != ObjectState.Destroyed
                    && b.state != ObjectState.Cleanup
                    && b.state != ObjectState.WaitingForSpawn
                    )
                {
                    targets.Add(b);
                }
            }
            if (
                    (parent.logSys.mainBuilding.position2d - bearer.position2d).Length() < range
                    && bearer.state != ObjectState.Destroyed
                    && bearer.state != ObjectState.Construction
                    && bearer.state != ObjectState.WaitingForSpawn
                    && bearer.state != ObjectState.Cleanup
                    && parent.logSys.mainBuilding.state != ObjectState.Destroyed
                    && parent.logSys.mainBuilding.state != ObjectState.Cleanup
                    && parent.logSys.mainBuilding.state != ObjectState.WaitingForSpawn
                    )
            {
                targets.Add(parent.logSys.mainBuilding);
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
