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
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;

using Sound = GoblinXNA.Sounds.Sound;

using System.Threading;

using Edge = TowARDefense.Pathfinding.Edge;
using Node = TowARDefense.Pathfinding.Node;

namespace TowARDefense
{
    public class PathGrid
    {
        private TowARDefense parent;

        private TransformNode tNode;

        List<List<Node>> nodes;
        public List<Node> spawnNodes;
        public List<Node> targetNodes;

        private double timeExpired;
        private int gridSize;

        private float[,] towerWeight;
        private float resWeight;

        private DefenseTower newTower;
        private ResourceBuilding newRes;
        private Thread calcThread;

        private GoblinXNA.Graphics.Model pfModel;

        public PathGrid(TowARDefense parent_f)
        {
            parent = parent_f;

            gridSize = 40;

            timeExpired = 0.0;

            resWeight = -1.5f;

            /* Arrangement:
             * Erste Ebene: AntiInfTower, AntiTankTower, AntiSiegeTower
             * Zweite Ebene: Inf, Tank, Siege */

            towerWeight = new float[3,3];
            towerWeight[0,0] = 4.0f;
            towerWeight[0,1] = -1.5f;
            towerWeight[0,2] = -1.5f;
            towerWeight[1,0] = -1.5f;
            towerWeight[1,1] = 4.0f;
            towerWeight[1,2] = -0.5f;
            towerWeight[2,0] = -1.5f;
            towerWeight[2,1] = -3.0f;
            towerWeight[2,2] = 6.0f;

            tNode = new TransformNode("Pathfinding Grid Node");

            nodes = new List<List<Node>>();
            spawnNodes = new List<Node>();
            targetNodes = new List<Node>();
        }

        public void Init()
        {
            parent.graSys.groundNode.AddChild(tNode);

            pfModel = new Box(2);
        }

        public void Update(double timePassed)
        {
            timeExpired += timePassed;
        }

        public void setupGrid()
        {
            nodes.Clear();

            float stepSize;

            float height;
            Vector3 normal;

            Vector3 pos = new Vector3(0, 0, 0);
            while (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                pos.X = pos.X + 0.5f;
            }
            stepSize = (pos.X / gridSize) * 2;

            Console.WriteLine("Path Grid Step Size: " + stepSize.ToString());
            Console.WriteLine("Path Grid Step Count: " + gridSize.ToString());

            parent.logSys.mapWidth = stepSize * gridSize;
            parent.logSys.mapHeight = stepSize * gridSize;
            parent.logSys.buildRadius = ((float)((parent.logSys.mapHeight + parent.logSys.mapWidth) / 2)) * 0.4f;

            Console.WriteLine("Build Radius: " + parent.logSys.buildRadius);

            int gsHalf = (int)(gridSize / 2);

            // Nodes bauen
            // X-Richtung
            for (int i = 1-gsHalf; i < gsHalf; i++)
            {
                nodes.Add(new List<Node>());
                //Y-Richtung
                for (int k = 1-gsHalf; k < gsHalf; k++)
                {
                    nodes[i + gsHalf - 1].Add(null);
                    pos = new Vector3(i * stepSize, k * stepSize, 1);
                    if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
                    {
                        //Console.WriteLine("X: {0}; Y: {1}; Height: {2}; Threshold: {3};", i, k, height, parent.graSys.terrain.pathThreshold);
                        if (height < parent.graSys.terrain.pathThreshold)
                        {
                            nodes[i + gsHalf - 1][k + gsHalf - 1] = new Node(new Vector3(pos.X, pos.Y, height), normal, parent, pfModel);
                        }
                    }
                }
            }

            // For Map2 wen dont need a lot of Nodes
            if (parent.chosenMap != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < gridSize-1; k++)
                    {
                        if (nodes[k][i] != null)
                        {
                            /*if (nodes[k][i].tNode != null)
                            {
                                nodes[k][i].tNode.Children.Clear();
                                parent.graSys.groundNode.RemoveChild(nodes[k][i].tNode);
                            }*/
                            nodes[k][i] = null;
                        }
                    }
                }
                for (int i = 0; i < gridSize-3; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (nodes[k][i] != null)
                        {
                            /*if (nodes[k][i].tNode != null)
                            {
                                nodes[k][i].tNode.Children.Clear();
                                parent.graSys.groundNode.RemoveChild(nodes[k][i].tNode);
                            }*/
                            nodes[k][i] = null;
                        }
                    }
                }
                for (int i = 0; i < gridSize - 3; i++)
                {
                    for (int k = gridSize-2; k > gridSize-4; k--)
                    {
                        if (nodes[k][i] != null)
                        {
                            /*if (nodes[k][i].tNode != null)
                            {
                                nodes[k][i].tNode.Children.Clear();
                                parent.graSys.groundNode.RemoveChild(nodes[k][i].tNode);
                            }*/
                            nodes[k][i] = null;
                        }
                    }
                }
            }

            // Edges bauen
            for (int i = 1 - gsHalf; i < gsHalf; i++)
            {
                for (int k = 1 - gsHalf; k < gsHalf; k++)
                {
                    if (nodes[i + gsHalf - 1][k + gsHalf - 1] != null)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            for (int y = -1; y <= 1; y++)
                            {
                                    if ((x == 0  && y == 0) || (i + gsHalf - 1 - x) < 0 || (k + gsHalf - 1 - y) < 0 || (i + gsHalf - 1 - x) >= nodes.Count)
                                    {
                                        continue;
                                    }
                                    if ((k + gsHalf - 1 - y) >= nodes[(i + gsHalf - 1 - x)].Count)
                                    {
                                        continue;
                                    }
                                    Node n = nodes[i + gsHalf - 1 - x][k + gsHalf - 1 - y];
                                    if (n != null)
                                    {
                                        Edge e = new Edge(nodes[i + gsHalf - 1][k + gsHalf - 1], n);
                                        nodes[i + gsHalf - 1][k + gsHalf - 1].edges.Add(e);
                                    }
                            }
                        }
                    }
                }
            }

            spawnNodes.Clear();
            targetNodes.Clear();

            if (parent.chosenMap == 0)
            {

                // Spawn und Target Nodes setzen
                if (nodes[nodes.Count - 1][nodes[nodes.Count - 1].Count - 1] != null)
                    spawnNodes.Add(nodes[nodes.Count - 1][nodes[nodes.Count - 1].Count - 1]);
                if (nodes[nodes.Count - 1][0] != null)
                    spawnNodes.Add(nodes[nodes.Count - 1][0]);
                if (nodes[0][nodes[0].Count - 1] != null)
                    spawnNodes.Add(nodes[0][nodes[0].Count - 1]);
                if (nodes[0][0] != null)
                    spawnNodes.Add(nodes[0][0]);

                if (nodes[(int)((gridSize - 2) / 2)][(int)((gridSize - 2) / 2)] != null)
                    targetNodes.Add(nodes[(int)((gridSize - 2) / 2)][(int)((gridSize - 2) / 2)]);
            }
            else
            {
                if (nodes[0][nodes[0].Count - 1] != null)
                    spawnNodes.Add(nodes[0][nodes[0].Count - 1]);
                if (nodes[nodes.Count - 1][nodes[nodes.Count - 1].Count - 1] != null)
                    spawnNodes.Add(nodes[nodes.Count - 1][nodes[nodes.Count - 1].Count - 1]);

                if (nodes[20][10] != null)
                    targetNodes.Add(nodes[20][10]);
            }
        }

        public List<Node> solvePathfinding(Node pos, double[] damageInfluence)
        {
            List<Node> r = solveA(pos, targetNodes[0], damageInfluence);
            r.Reverse();

            /*Material ma = new Material();
            ma.Ambient = Color.DarkRed.ToVector4();
            ma.Diffuse = Color.DarkRed.ToVector4();
            ma.SpecularPower = 0;

            foreach (Node n in r)
            {
                ((GeometryNode)(n.tNode.Children[0])).Material = ma;
            }*/
            if (r.Count == 0)
            {
                return targetNodes;
            }
            if (parent.optionPathNodesPerEnemy > 0)
            {
                // Shorten the path to parent.optionPathNodesPerEnemy nodes +- some random
                int diff = r.Count - parent.optionPathNodesPerEnemy;
                diff += RandomHelper.GetRandomInt(4) - 1;
                for (int i = 0; i < diff; i++)
                {
                    r.RemoveAt(r.Count - 1);
                }
            }
            return r;
        }

        private Node getLowestWeightNode(List<Node> list)
        {
            Node low = list[0];
            foreach (Node n in list)
            {
                if (n.fScore < low.fScore)
                {
                    low = n;
                }
            }
            return low;
        }

        private float estimatedDistance(Node pos, Node target)
        {
            return (target.position2d - pos.position2d).Length();
        }

        private List<Node> reconstructPath(Node last)
        {
            List<Node> ln = new List<Node>();
            if (last != null)
            {
                while (last.cameFrom != null)
                {
                    ln.Add(last);
                    last = last.cameFrom;
                }
            }
            return ln;
        }

        public List<Node> solveA(Node pos, Node target, double[] damageInfluence)
        {
            pos.cameFrom = null;

            List<Node> closedSet = new List<Node>();
            List<Node> openSet = new List<Node>();
            openSet.Add(pos);

            pos.gScore = 0;
            pos.hScore = estimatedDistance(pos, target);
            pos.fScore = pos.hScore;

            while (openSet.Count > 0)
            {
                Node x = getLowestWeightNode(openSet);
                if (x == target)
                {
                    return reconstructPath(x.cameFrom);
                }
                openSet.Remove(x);
                closedSet.Add(x);
                foreach (Edge e in x.edges)
                {
                    Node y = e.target;
                    if (closedSet.Contains(y))
                    {
                        continue;
                    }
                    bool tentativeIsBetter;
                    float tentativeG = x.gScore;
                    for (int i = 0; i < 7; i++)
                    {
                        tentativeG += (float)(e.weight[i] * damageInfluence[i]);
                        if (x.blocked)
                            tentativeG += 10000000;
                    }
                    if (!openSet.Contains(y))
                    {
                        openSet.Add(y);
                        tentativeIsBetter = true;
                    }
                    else if (tentativeG < y.gScore)
                    {
                        tentativeIsBetter = true;
                    }
                    else
                    {
                        tentativeIsBetter = false;
                    }
                    if (tentativeIsBetter)
                    {
                        y.cameFrom = x;

                        y.gScore = tentativeG;
                        y.hScore = estimatedDistance(y, target);
                        y.fScore = y.gScore + y.hScore;
                    }
                }
            }
            Console.WriteLine("Target Not Reachable");
            return null;
        }

        public void pathUpdateTow(DefenseTower tower)
        {
            newTower = tower;
            calcThread = new Thread(recalculateWeightsDef);
            calcThread.Start();
        }

        public void pathUpdateRes(ResourceBuilding res)
        {
            newRes = res;
            calcThread = new Thread(recalculateWeightsRes);
            calcThread.Start();
        }

        private void recalculateWeightsDef()
        {
            int vz = 1;
            if (newTower.state == ObjectState.Destroyed)
            {
                vz = -1;
            }

            Vector2 v;
            float r = newTower.avRange;
            foreach (List<Node> l in nodes)
            {
                foreach (Node n in l)
                {
                    if (n != null)
                    {
                        v = newTower.position2d - n.position2d;
                        if (v.Length() < r)
                        {
                            foreach (Edge e in n.edges)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    e.weight[i] += vz * newTower.weights[i];
                                }
                            }
                        }
                    }
                }
            }
        }

        private void recalculateWeightsRes()
        {
            int vz = 1;
            if (newRes.state == ObjectState.Destroyed)
            {
                vz = -1;
            }

            Vector2 v;
            foreach (List<Node> l in nodes)
            {
                foreach (Node n in l)
                {
                    if (n != null)
                    {
                        v = newRes.position2d - n.position2d;
                        if (v.Length() < 5.0)
                        {
                            foreach (Edge e in n.edges)
                            {
                                e.weight[0] += vz * resWeight;
                                e.weight[1] += vz * resWeight;
                                e.weight[2] += vz * resWeight;
                            }
                        }
                    }
                }
            }
        }

    }
}