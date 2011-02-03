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
    class MainMenu : Menu
    {
        Rectangle r;

        public MainMenu(TowARDefense parent_f)
            : base(parent_f)
        {
        }

        public override void Init()
        {
            r = new Rectangle(0, 0, parent.graphics.GraphicsDevice.Viewport.Width, parent.graphics.GraphicsDevice.Viewport.Height);

            menuEntrys.Add(new TextEntry("Spiel starten", true));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(gotoChoseMap);
            menuEntrys.Add(new TextEntry("Wie spielt man?", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(gotoHowToPlay);
            menuEntrys.Add(new TextEntry("Highscores", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(gotoHighScores);
            menuEntrys.Add(new TextEntry("Optionen", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(gotoOptions);
            menuEntrys.Add(new TextEntry("Beenden", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(parent.menSys.quitGame);
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

        public void gotoChoseMap(object sender, EventArgs e)
        {
            parent.menSys.state = MenuState.ChoseMap;
        }

        public void gotoHowToPlay(object sender, EventArgs e)
        {
            parent.menSys.state = MenuState.HowToPlay;
        }

        public void gotoHighScores(object sender, EventArgs e)
        {
            parent.menSys.state = MenuState.HighScore;
        }

        public void gotoOptions(object sender, EventArgs e)
        {
            parent.menSys.state = MenuState.OptionsMenu;
        }
    }
}
