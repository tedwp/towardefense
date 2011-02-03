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
    class OptionMenu : Menu
    {
        Rectangle r;

        public OptionMenu(TowARDefense parent_f)
            : base(parent_f)
        {
        }

        public override void Init()
        {
            r = new Rectangle(0, 0, parent.graphics.GraphicsDevice.Viewport.Width, parent.graphics.GraphicsDevice.Viewport.Height);

            menuEntrys.Add(new TextEntry("Vollbild umschalten", true));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(toggleFullScreen);
            menuEntrys.Add(new TextEntry("FPS anzeigen/verstecken", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(toggleFPS);

            menuEntrys.Add(new OptionEntry("Schwierigkeit", false));
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("Schwer", 1);
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("Bockschwer", 2);
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("Bombe", 3);

            menuEntrys.Add(new OptionEntry("Wegfindungsneuberechnung nach", false));
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("Nie", 0);
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("5 Nodes", 5);
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("4 Nodes", 4);
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("3 Nodes", 3);
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("2 Nodes", 2);
            ((OptionEntry)menuEntrys[menuEntrys.Count - 1]).addEntry("jedem Node", 1);

            menuEntrys.Add(new TextEntry("Zurueck", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(goBack);
        }

        public override void Update(double timePassed)
        {
            base.Update(timePassed);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                goBack(null, null);
            }
        }

        public override void Draw()
        {
            UI2DRenderer.FillRectangle(r, parent.graSys.blackTransparentTexture, Color.Black);
            UI2DRenderer.FillRectangle(r, parent.graSys.mainMenuTexture, Color.White);

            Vector2 pos = new Vector2(120, 120);
            foreach (Entry e in menuEntrys)
            {
                if (e.GetType() == typeof(TextEntry))
                {
                    TextEntry t = (TextEntry)e;
                    if (t.selected)
                        UI2DRenderer.WriteText(pos, t.text, Color.Red, parent.textFont);
                    else
                        UI2DRenderer.WriteText(pos, t.text, Color.White, parent.textFont);
                }
                if (e.GetType() == typeof(OptionEntry))
                {
                    OptionEntry o = (OptionEntry)e;
                    if (o.selected)
                    {
                        UI2DRenderer.WriteText(pos, o.text, Color.Red, parent.textFont);
                        UI2DRenderer.WriteText(new Vector2(pos.X + 430, pos.Y), o.getValue(), Color.Red, parent.textFont);
                    }
                    else
                    {
                        UI2DRenderer.WriteText(pos, o.text, Color.White, parent.textFont);
                        UI2DRenderer.WriteText(new Vector2(pos.X + 430, pos.Y), o.getValue(), Color.White, parent.textFont);
                    }
                }
                pos.Y += 30;
            }
        }

        public void toggleFullScreen(object sender, EventArgs e)
        {
            //Console.WriteLine("ToggleFS");
            parent.graphics.ToggleFullScreen();
        }

        public void toggleFPS(object sender, EventArgs e)
        {
            State.ShowFPS = !State.ShowFPS;
            State.ShowTriangleCount = !State.ShowTriangleCount;
            State.ShowNotifications = !State.ShowNotifications;
        }

        public void goBack(object sender, EventArgs e)
        {
            parent.optionDifficultyMultiplier = (int)((OptionEntry)menuEntrys[2]).getInternalValue();
            parent.optionPathNodesPerEnemy = (int)((OptionEntry)menuEntrys[3]).getInternalValue();

            foreach (Entry en in menuEntrys)
            {
                en.selected = false;
            }
            menuEntrys[0].selected = true;
            selected = 0;

            parent.menSys.state = MenuState.MainMenu;
        }
    }
}
