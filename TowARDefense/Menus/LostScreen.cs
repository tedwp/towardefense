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
    public class LostScreen : Menu
    {
        Rectangle r;

        List<HighScore> highScores;

        bool waitForEntry;

        G2DPanel frame;
        G2DLabel label;
        G2DTextField textField;
        G2DButton button;

        private int highScoreCount;

        public LostScreen(TowARDefense parent_f)
            : base(parent_f)
        {
            highScores = new List<HighScore>();
        }

        public override void Init()
        {
            r = new Rectangle(0, 0, parent.graphics.GraphicsDevice.Viewport.Width, parent.graphics.GraphicsDevice.Viewport.Height);

            highScoreCount = 5;

            menuEntrys.Add(new TextEntry("Spiel neu starten", true));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(parent.resetGame);
            menuEntrys.Add(new TextEntry("Beenden", false));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(parent.menSys.quitGame);

            readHighScores();

            frame = new G2DPanel();
            frame.Bounds = new Rectangle(120, 170, 350, 118);
            frame.Border = GoblinEnums.BorderFactory.LineBorder;
            frame.BackgroundColor = Color.Black;
            frame.BorderColor = Color.DarkRed;
            frame.Transparency = 0.5f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)

            label = new G2DLabel("Bitte geben Sie ihren Namen ein");
            label.Bounds = new Rectangle(10, 10, 330, 20);
            label.TextColor = Color.White;
            label.Visible = true;
            label.Enabled = true;

            textField = new G2DTextField("Ihr Name", 20); ;
            textField.Bounds = new Rectangle(10, 40, 330, 28);
            textField.FocusedColor = Color.DarkRed;
            textField.HighlightColor = Color.DarkRed;
            textField.TextColor = Color.White;
            textField.Enabled = true;
            textField.Editable = true;
            textField.KeyReleasedEvent += new GoblinXNA.UI.KeyReleased(keyDown);

            button = new G2DButton("OK");
            button.Bounds = new Rectangle(230, 80, 100, 28);
            button.TextColor = Color.White;
            button.ActionPerformedEvent += clickButton;

            frame.AddChild(button);
            frame.AddChild(label);
            frame.AddChild(textField);
        }

        public void showed()
        {
            readHighScores();
            if (highScores.Count < 5 || parent.logSys.timeLasted >= highScores[4].time)
            {
                waitForEntry = true;
                label.TextFont = parent.hudFont;
                textField.TextFont = parent.hudFont;
                button.TextFont = parent.hudFont;
                parent.IsMouseVisible = true;
                parent.gameScene.UIRenderer.Add2DComponent(frame);
            }
            else
            {
                waitForEntry = false;
            }
        }

        public void writeHighScores()
        {
            int diff = highScores.Count - highScoreCount;
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    highScores.RemoveAt(highScores.Count - 1);
                }
            }

            // TestWrite Highscore
            StreamWriter writer = new StreamWriter("highscore.txt");
            try
            {
                foreach (HighScore h in highScores)
                {

                    writer.WriteLine(h.name + "~~" + h.time);
                }
            }
            finally
            {
                writer.Close();
            }
        }

        public void readHighScores()
        {
            highScores.Clear();

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
            if (!waitForEntry)
            {
                base.Update(timePassed);
            }
        }

        public override void Draw()
        {
            if (!waitForEntry)
            {
                UI2DRenderer.FillRectangle(r, parent.graSys.blackTransparentTexture, Color.Black);
                UI2DRenderer.FillRectangle(r, parent.graSys.mainMenuTexture, Color.White);
            }

            UI2DRenderer.WriteText(new Vector2(120, 90), "Sie wurden vernichtet!", Color.White, parent.bigFont);
            UI2DRenderer.WriteText(new Vector2(120, 140), "Sie haben " + String.Format("{0:00}", parent.logSys.min) + ":" + String.Format("{0:00}", parent.logSys.sec) + " durchgehalten", Color.White, parent.hudFont);

            if (!waitForEntry)
            {
                Vector2 pos = new Vector2(120, 170);
                Color c = Color.White;
                for (int i = 0; (i < highScoreCount) && (i < highScores.Count); i++)
                {
                    if (highScores[i].justMade)
                        c = Color.Red;
                    else if (i == 0)
                        c = Color.DarkRed;
                    else
                        c = Color.White;
                    UI2DRenderer.WriteText(pos, (i + 1).ToString() + ". " + highScores[i].name, c, parent.textFont);
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
        }

        private static int sortHighScores(HighScore x, HighScore y)
        {
            if (x.time > y.time)
                return -1;
            else
                return 1;
        }

        private void keyDown(Keys k, KeyModifier t)
        {
            if (k == Keys.Enter)
            {
                clickButton(null);
            }
        }

        private void clickButton(object source)
        {
            parent.gameScene.UIRenderer.Remove2DComponent(frame);

            HighScore h = new HighScore();
            h.name = textField.Text;
            h.time = parent.logSys.timeLasted;
            h.min = parent.logSys.min;
            h.sec = parent.logSys.sec;
            h.justMade = true;

            highScores.Add(h);
            highScores.Sort(sortHighScores);

            writeHighScores();


            parent.IsMouseVisible = false;
            waitForEntry = false;
        }
    }
}
