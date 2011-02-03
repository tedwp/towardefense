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
using Model = GoblinXNA.Graphics.Model;
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

namespace TowARDefense.Pathfinding
{
    public class Node
    {
        private TowARDefense parent;

        // blocked flag -> keine 2 gegner auf einem fleck
        public bool blocked;

        /*private GeometryNode gNode;
        public TransformNode tNode;*/

        public List<Edge> edges;
        public Vector2 position2d;

        public Node cameFrom;
        public float fScore;
        public float hScore;
        public float gScore;

        public Node(Vector3 position, Vector3 normal, TowARDefense parent_f, Model model)
        {
            parent = parent_f;

            position2d = new Vector2(position.X, position.Y);

            edges = new List<Edge>();

            /*tNode = new TransformNode();
            tNode.Translation = position;
            tNode.Rotation = Quaternion.CreateFromAxisAngle(normal , 0);

            gNode = new GeometryNode();
            gNode.Model = model;

            tNode.AddChild(gNode);
            
            parent.graSys.groundNode.AddChild(tNode);*/
        }
    }
}
