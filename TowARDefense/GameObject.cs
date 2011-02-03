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
    abstract public class GameObject
    {
        protected TowARDefense parent;

        public TransformNode gtNode;

        public Vector3 normal;

        public Vector2 position2d;
        public Vector2 dir;

        public GameObject(TowARDefense parent_f)
        {
            parent = parent_f;

            gtNode = new TransformNode();
        }

        protected void setPosition(Vector3 pos, Vector3 normal)
        {
            gtNode.Translation = pos;
            gtNode.Rotation = Quaternion.CreateFromAxisAngle(normal, 0);

            this.normal = normal;

            position2d = new Vector2(pos.X, pos.Y);
            dir = new Vector2(0, 0);
        }

        protected void setPosition(Vector3 pos, Vector3 normal, Vector2 dir)
        {
            gtNode.Translation = pos;

            this.normal = normal;

            gtNode.Rotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(normal, 0) ,(Quaternion.CreateFromYawPitchRoll(0,0,(float)Math.Atan2(dir.Y, dir.X))));
            position2d = new Vector2(pos.X, pos.Y);
            this.dir = dir;
        }
    }
}
