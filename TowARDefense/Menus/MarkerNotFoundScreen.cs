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
    class MarkerNotFoundScreen
    {
        TowARDefense parent;

        Rectangle r;

        public MarkerNotFoundScreen(TowARDefense parent_f)
        {
            parent = parent_f;
        }

        public void Init()
        {
           r = new Rectangle(0, 0, parent.graphics.GraphicsDevice.Viewport.Width, parent.graphics.GraphicsDevice.Viewport.Height);
        }

        public void Update(double timePassed)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                parent.menSys.pauseGame(null, null);
            }
        }

        public void Draw()
        {
            UI2DRenderer.FillRectangle(r, parent.graSys.blackTransparentTexture, Color.Black);
            UI2DRenderer.FillRectangle(r, parent.graSys.mainMenuTexture, Color.White);
            UI2DRenderer.WriteText(new Vector2(120,180), "Marker wird nicht gefunden", Color.White, parent.textFont);
        }
    }
}
