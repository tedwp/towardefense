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
    abstract public class Placeable : GameObject
    {
        public Placeable(TowARDefense parent_f) : base(parent_f)
        {    
        }

        protected void setPosition(Vector2 pos2d)
        {
            Vector3 pos = new Vector3(pos2d.X, pos2d.Y, 1);
            Vector3 normal;
            float height = 0.0f;

            if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                pos.Z = height;
                base.setPosition(pos, normal);
            }
            else
            {
                throw new Exception();
            }
        }

        protected void setPositionNN(Vector2 pos2d)
        {
            Vector3 pos = new Vector3(pos2d.X, pos2d.Y, 1);
            Vector3 normal;
            float height = 0.0f;

            if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                pos.Z = height;
                base.setPosition(pos, new Vector3(0,0,1));
            }
            else
            {
                throw new Exception();
            }
        }

        virtual protected void setPosition(Vector2 pos2d, Vector2 dir)
        {
            Vector3 pos = new Vector3(pos2d.X, pos2d.Y, 1);
            Vector3 normal;
            float height = 0.0f;

            if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                pos.Z = height;
                base.setPosition(pos, normal, dir);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}