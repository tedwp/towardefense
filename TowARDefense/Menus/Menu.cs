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


namespace TowARDefense.Menus
{
    public abstract class Menu
    {
        protected TowARDefense parent;

        protected List<Entry> menuEntrys;
        protected int selected;

        public Menu(TowARDefense parent_f)
        {
            parent = parent_f;

            menuEntrys = new List<Entry>();
        }

        abstract public void Init();


        public virtual void Update(double timePassed)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !parent.menSys.downKeyDown)
            {
                parent.menSys.downKeyDown = true;
                if (selected != menuEntrys.Count - 1)
                {
                    selected++;
                    Sound.Play("click");
                }
                for (int i = 0; i < menuEntrys.Count; i++)
                {
                    menuEntrys[i].selected = (i == selected);
                }
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                parent.menSys.downKeyDown = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !parent.menSys.upKeyDown)
            {
                parent.menSys.upKeyDown = true;
                if (selected != 0)
                {
                    selected--;
                    Sound.Play("click");
                }
                for (int i = 0; i < menuEntrys.Count; i++)
                {
                    menuEntrys[i].selected = (i == selected);
                }
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                parent.menSys.upKeyDown = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !parent.menSys.enterKeyDown)
            {
                parent.menSys.enterKeyDown = true;
                Sound.Play("toggle");
                menuEntrys[selected].pressed();
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                parent.menSys.enterKeyDown = false;
            }
        }

        abstract public void Draw();
    }
}
