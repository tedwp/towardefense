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
    class choseMapScreen : Menu
    {
        Rectangle r;

        public choseMapScreen(TowARDefense parent_f)
            : base(parent_f)
        {
        }

        public override void Init()
        {
            r = new Rectangle(0, 0, parent.graphics.GraphicsDevice.Viewport.Width, parent.graphics.GraphicsDevice.Viewport.Height);

            menuEntrys.Add(new TextEntry("Karte 1", true));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(startL1);
            menuEntrys.Add(new TextEntry("Karte 2", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(startL2);
            menuEntrys.Add(new TextEntry("Zurueck", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(goBack);
        }

        public override void Update(double timePassed)
        {
            base.Update(timePassed);
        }

        public override void Draw()
        {
            UI2DRenderer.FillRectangle(r, parent.graSys.blackTransparentTexture, Color.Black);
            UI2DRenderer.FillRectangle(r, parent.graSys.mainMenuTexture, Color.White);

            Vector2 pos = new Vector2(120, 120);
            UI2DRenderer.WriteText(pos, "Waehlen Sie die Karte", Color.White, parent.textFont);
            pos.Y += 30;
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
                pos.Y += 30;
            }
        }

        public void startL1(object sender, EventArgs e)
        {
            parent.chosenMap = 0;
            parent.menSys.startGame(null, null);
        }

        public void startL2(object sender, EventArgs e)
        {
            parent.chosenMap = 1;
            parent.optionDifficultyMultiplier = parent.optionDifficultyMultiplier * 2;
            parent.menSys.startGame(null, null);
        }

        public void goBack(object sender, EventArgs e)
        {
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