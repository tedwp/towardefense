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
    class HowToPlay : Menu
    {
        Rectangle r;

        String text;

        public HowToPlay(TowARDefense parent_f)
            : base(parent_f)
        {
        }

        public override void Init()
        {
            r = new Rectangle(0, 0, parent.graphics.GraphicsDevice.Viewport.Width, parent.graphics.GraphicsDevice.Viewport.Height);

            menuEntrys.Add(new TextEntry("Zurueck", true));
            menuEntrys[menuEntrys.Count - 1].handler += new EventHandler(goBack);

            text =  string.Format (@"Wenn Sie das Spiel starten haben sie einige Zeit um Tuerme
zu bauen und sich auf den ersten Angriff vorzubereiten.

Sollten sie noch keinen Marker besitzen, finden Sie ihn
im Content/marker Verzeichnis als PDF-Datei.

Tuerme bauen Sie indem Sie die mit der Kamera auf die
Stelle zeigen und den zugehoerigen Marker auf dem
Spielfeld verdecken, oder halten Sie eine der folgenden
Tasten gedrueckt:

A fuer Anti-Infanterie : Kosten {0}
S fuer Anti-Tank : Kosten {1}
D fuer Anti-Belagerung : Kosten {2}
W fuer eine Muehle (generiert Resourcen) : Kosten {3}

Die Gegner kommen in grossen Wellen auf Sie zu. Versuchen
Sie so lange wie moeglich zu ueberleben.", Towers.AntiInfTower.getPrice(), Towers.AntiTankTower.getPrice(), Towers.AntiSiegeTower.getPrice(), ResourceBuilding.getPrice());
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

            UI2DRenderer.WriteText(new Vector2(120, 120), text, Color.Gray, parent.hudFont);

            Vector2 pos = new Vector2(120, 480);
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
                        UI2DRenderer.WriteText(new Vector2(pos.X + 400, pos.Y), o.getValue(), Color.Red, parent.textFont);
                    }
                    else
                    {
                        UI2DRenderer.WriteText(pos, o.text, Color.White, parent.textFont);
                        UI2DRenderer.WriteText(new Vector2(pos.X + 400, pos.Y), o.getValue(), Color.White, parent.textFont);
                    }
                }
                pos.Y += 30;
            }
        }

        public void goBack(object sender, EventArgs e)
        {
            parent.menSys.state = MenuState.MainMenu;
        }
    }
}