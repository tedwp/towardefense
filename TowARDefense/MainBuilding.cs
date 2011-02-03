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
    public class MainBuilding : DestroyableObject
    {
        private GeometryNode gNode;

        private List<Turret> turrets;

        public MainBuilding(Vector3 pos, TowARDefense parent_f) : base(parent_f)
        {
            state = ObjectState.Idle;

            position2d = new Vector2(pos.X, pos.Y);

            turrets = new List<Turret>();

            setPosition(position2d);

            Init();
        }

        public void Init()
        {
            setHealth(1000);
            setHealthBarSize(32, 3);

            gNode = new GeometryNode();
            gNode.Model = parent.graSys.mainBModel;

            gtNode.AddChild(gNode);
            parent.graSys.groundNode.AddChild(gtNode);

            turrets.Add(new Turrets.Defender.MainBuilding(this, new Vector3(4, 4, 6), parent));
            turrets.Add(new Turrets.Defender.MainBuilding(this, new Vector3(4, -4, 6), parent));
            turrets.Add(new Turrets.Defender.MainBuilding(this, new Vector3(-4, 4, 6), parent));
            turrets.Add(new Turrets.Defender.MainBuilding(this, new Vector3(-4, -4, 6), parent));
        }

        public void Update(double timePassed)
        {
            foreach (Turret t in turrets)
            {
                t.Update(timePassed);
            }
        }

        override public void dealDamage(int damage, GameWeapons weaponType)
        {
            health -= damage;
            if (health <= 0)
            {
                parent.state = GameState.Lost;
                parent.menSys.state = MenuState.LostScreen;
                parent.menSys.lostScreen.showed();
            }
        }

        public void posForLevel()
        {
            this.setPositionNN(parent.logSys.kiSys.pathGrid.targetNodes[0].position2d);

            parent.graSys.terrain.kugelNode.Translation = gtNode.Translation;
        }
    }
}
