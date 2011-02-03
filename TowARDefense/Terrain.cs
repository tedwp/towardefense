using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
    public class TTerrain
    {
        public TransformNode groundNode;
        public GeometryNode groundModelNode;
        public HeightMapInfo heightMapInfo;
        private TowARDefense parent;

        public TransformNode kugelNode;
        private GeometryNode kugelModelNode;
        private Vector3 lastMove;

        private float heightMultiplier;
        private float heightOffset;
        public float towerThreshold;
        public float pathThreshold;

        public TTerrain(String model, TransformNode groundNode_f, GeometryNode groundModelNode_f, TowARDefense parent_f)
        {
            groundNode = groundNode_f;
            groundModelNode = groundModelNode_f;
            parent = parent_f;
            Init(model);

            kugelNode = new TransformNode();
            kugelModelNode = new GeometryNode();
            kugelModelNode.Model = new Box(3);
            kugelNode.AddChild(kugelModelNode);
            groundNode.AddChild(kugelNode);

            heightMultiplier = 2.0f;
            heightOffset = 6.0f;
            towerThreshold = 3.0f;
            pathThreshold = 0.2f;
        }

        private void Init(String model)
        {
            Microsoft.Xna.Framework.Graphics.Model terrain;
            terrain = parent.Content.Load<Microsoft.Xna.Framework.Graphics.Model>(model);

            // The terrain processor attached a HeightMapInfo to the terrain model's
            // Tag. We'll save that to a member variable now, and use it to
            // calculate the terrain's heights later.

            heightMapInfo = terrain.Tag as HeightMapInfo;
            if (heightMapInfo == null)
            {
                string message = "The terrain model did not have a HeightMapInfo " +
                    "object attached. Are you sure you are using the " +
                    "TerrainProcessor?";
                throw new InvalidOperationException(message);
            }
        }

        public bool getHeightMapInfo(Vector3 position, out float height, out Vector3 normal)
        {
            if (heightMapInfo.IsOnHeightmap(position))
            {
                heightMapInfo.GetHeightAndNormal(position, out height, out normal);
                height = (height * heightMultiplier) + heightOffset;
                return true;
            } else
            {
                height = 0.0f;
                normal = new Vector3(0,0,0);
                return false;
            }
        }

        public void Update(double timePassed)
        {
            Vector3 t = kugelNode.Translation;
            Vector3 m = t;

            if (heightMapInfo.IsOnHeightmap(m))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    kugelNode.Translation = kugelNode.Translation + new Vector3(1.0f, 0, 0);
                    lastMove = new Vector3(1, 0, 0);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    kugelNode.Translation = kugelNode.Translation + new Vector3(-1.0f, 0, 0);
                    lastMove = new Vector3(-1, 0, 0);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    kugelNode.Translation = kugelNode.Translation + new Vector3(0, 1.0f, 0);
                    lastMove = new Vector3(0, 1, 0);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    kugelNode.Translation = kugelNode.Translation + new Vector3(0, -1.0f, 0);
                    lastMove = new Vector3(0, -1, 0);
                }

                float height;
                Vector3 normal;
                Vector3 translation;
                getHeightMapInfo(m, out height, out normal);
                translation = kugelNode.Translation;
                translation.Z = height;
                kugelNode.Translation = translation;
                Quaternion dir = new Quaternion(normal, 0);
                kugelNode.Rotation = dir;
            }
            else
            {
                Vector3 translation;
                translation = kugelNode.Translation - lastMove;
                kugelNode.Translation = translation;
            }
        }


    }
}
