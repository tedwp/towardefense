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

using System.IO;

namespace TowARDefense.Menus
{
    class HighScoreScreen : Menu
    {
        Rectangle r;

        List<HighScore> highScores;

        private int highScoreCount;

        public HighScoreScreen(TowARDefense parent_f) : base(parent_f)
        {
            highScores = new List<HighScore>();
        }

        public override void Init()
        {
            r = new Rectangle(0, 0, parent.graphics.GraphicsDevice.Viewport.Width, parent.graphics.GraphicsDevice.Viewport.Height);

            highScoreCount = 5;

            menuEntrys.Add(new TextEntry("Zurueck", true));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(goBack);

            readHighScores();
        }

        public void readHighScores()
        {
            FileStream f;
            if (!File.Exists("highscore.txt"))
            {
                f = File.Create("highscore.txt");
                f.Close();
            }
            // Load HighScrore
            StreamReader reader = new StreamReader("highscore.txt");
            try
            {
                while (!reader.EndOfStream)
                {
                    String s = reader.ReadLine();
                    string[] arr = s.Split(("~~").ToCharArray());

                    HighScore h = new HighScore();

                    //Console.WriteLine("String: {2} Arr0: {0}, Arr1: {1}", arr[0], arr[2], s);

                    h.name = arr[0];
                    h.time = int.Parse(arr[2]);

                    h.min = (int)Math.Floor(((double)h.time / 60.0));
                    h.sec = h.time - h.min * 60;

                    h.justMade = false;

                    highScores.Add(h);
                }
            }
            finally
            {
                reader.Close();
            }

            highScores.Sort(sortHighScores);
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

            

            UI2DRenderer.WriteText(new Vector2(120, 90), "Highscores", Color.White, parent.bigFont);

            Vector2 pos = new Vector2(120, 140);
            Color c = Color.White;
            for (int i = 0; (i < highScoreCount) && (i < highScores.Count); i++)
            {
                if (i == 0)
                    c = Color.Red;
                else
                    c = Color.White;
                UI2DRenderer.WriteText(pos, (i + 1).ToString() + ". " + highScores[i].name,c, parent.textFont);
                UI2DRenderer.WriteText(new Vector2(pos.X + 400, pos.Y), String.Format("{0:00}", highScores[i].min) + ":" + String.Format("{0:00}", highScores[i].sec), c, parent.textFont);
                pos.Y += 30;
            }

            pos = new Vector2(120, 480);
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

        private static int sortHighScores(HighScore x, HighScore y)
        {
            if (x.time > y.time)
                return -1;
            else
                return 1;
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
