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
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;
using Sound = GoblinXNA.Sounds.Sound;

namespace TowARDefense.Pathfinding
{
    public class Edge
    {
        public Node target;

        public float[] weight;
        public float calc;

        public Edge(Node parent_f, Node target_f)
        {
            target = target_f;

            weight = new float[7];
            weight[0] = (parent_f.position2d - target_f.position2d).Length();
            weight[1] = (parent_f.position2d - target_f.position2d).Length();
            weight[2] = (parent_f.position2d - target_f.position2d).Length();
            weight[3] = (parent_f.position2d - target_f.position2d).Length();
            weight[4] = (parent_f.position2d - target_f.position2d).Length();
            weight[5] = (parent_f.position2d - target_f.position2d).Length();
            weight[6] = (parent_f.position2d - target_f.position2d).Length();

            //Console.WriteLine(weight[0]);
        }
    }
}
