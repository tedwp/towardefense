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

using TowARDefense;

namespace TowARDefense
{
    public class GameInput
    {
        private TowARDefense parent;

        private MarkerNode groundMarkerNode;
        public GeometryNode cameraModelNode;

        private MarkerNode infanteryMarkerNode;
        private MarkerNode tankMarkerNode;
        private MarkerNode siegeMarkerNode;
        private MarkerNode resMarkerNode;

        // Nodes um zu schauen ib die Marker wirklich im Viewport sind
        private TransformNode infNode;
        private TransformNode tankNode;
        private TransformNode siegeNode;
        private TransformNode resNode;

        private bool infDone;
        private bool tankDone;
        private bool siegeDone;
        private bool resDone;

        private double infExp;
        private double tankExp;
        private double siegeExp;
        private double resExp;

        private double markerTH;
        private float markerBorder;

        private double keyThreshold;
        private double timeExpired;
        private bool markerUsed;

        public float minDistance;

        private double markerNotFoundTE;
        private double markerNotFoundThreshold;
        public bool markerNotFound;

        private Vector3 currentRelMousePosition;

        public GameInput(TowARDefense parent_f)
        {
            parent = parent_f;

            markerNotFound = false;
            markerNotFoundTE = 0.0;
            markerNotFoundThreshold = 0.5;

            minDistance = 6.0f;

            keyThreshold = 0.2;
            markerTH = 0.5;
            markerBorder = 30.0f;

            timeExpired = 0.0;

            markerUsed = false;
            infDone = false;
            tankDone = false;
            siegeDone = false;
            resDone = false;
            infExp = 0.0;
            tankExp = 0.0;
            siegeExp = 0.0;
            resExp = 0.0;
        }

        public void Init()
        {
            setupMarkerTracking();
            setupMarkerBuilding();
        }

        public void Update(double timePassed)
        {
            UpdateMarkerPosition(timePassed);
            if (parent.state == GameState.GameRunning)
            {
                UpdateMousePosition();
                handleMarkerInput(timePassed);
                handleBuildInput(timePassed);
                handleKeyboardInput();
            }
        }

        public void Draw()
        {
        }

        private void setupMarkerTracking()
        {
            // Create our video capture device that uses DirectShow library. Note that 
            // the combinations of resolution and frame rate that are allowed depend on 
            // the particular video capture device. Thus, setting incorrect resolution 
            // and frame rate values may cause exceptions or simply be ignored, depending 
            // on the device driver.  The values set here will work for a Microsoft VX 6000, 
            // and many other webcams.
            IVideoCapture captureDevice = new DirectShowCapture();

            try
            {
                captureDevice.InitVideoCapture(0, FrameRate._50Hz, Resolution._640x480, ImageFormat.R8G8B8_24, false);
            }
            catch (GoblinException e)
            {
                Console.WriteLine("Problems initializing the Camera {0}", e.Message);
                parent.Exit();
                return;
            }

            // Add this video capture device to the scene so that it can be used for
            // the marker tracker
            parent.gameScene.AddVideoCaptureDevice(captureDevice);

            IMarkerTracker tracker = null;

#if USE_ARTAG
        // Create an optical marker tracker that uses ARTag library
        tracker = new ARTagTracker();
        // Set the configuration file to look for the marker specifications
        tracker.InitTracker(638.052f, 633.673f, captureDevice.Width,
        captureDevice.Height, false, "ARTag.cf");
#else
            // Create an optical marker tracker that uses ALVAR library
            tracker = new ALVARMarkerTracker();
            ((ALVARMarkerTracker)tracker).MaxMarkerError = 0.02f;
            tracker.InitTracker(captureDevice.Width, captureDevice.Height, "calib.xml", 9.0);
#endif

            // Set the marker tracker to use for our scene
            parent.gameScene.MarkerTracker = tracker;

            // Display the camera image in the background. Note that this parameter should
            // be set after adding at least one video capture device to the Scene class.
            parent.gameScene.ShowCameraImage = true;

            int[] ids = new int[28];
            for (int i = 0; i <= 27; i++)
            {
                ids[i] = i;
            }
            groundMarkerNode = new MarkerNode(parent.gameScene.MarkerTracker, "new2GroundArray.txt", ids);

            groundMarkerNode.Smoother = new DESSmoother();

            infanteryMarkerNode = new MarkerNode(parent.gameScene.MarkerTracker, 15, 18.0);
            tankMarkerNode = new MarkerNode(parent.gameScene.MarkerTracker, 0, 18.0);
            siegeMarkerNode = new MarkerNode(parent.gameScene.MarkerTracker, 25, 18.0);
            resMarkerNode = new MarkerNode(parent.gameScene.MarkerTracker, 20, 18.0); 

            parent.gameScene.RootNode.AddChild(groundMarkerNode);
            parent.gameScene.RootNode.AddChild(infanteryMarkerNode);
            parent.gameScene.RootNode.AddChild(tankMarkerNode);
            parent.gameScene.RootNode.AddChild(siegeMarkerNode);
            parent.gameScene.RootNode.AddChild(resMarkerNode);
        }

        private void UpdateMarkerPosition(double timePassed)
        {
            if (groundMarkerNode.MarkerFound)
            {
                if (parent.state == GameState.MarkerNotFound)
                {
                    parent.menSys.markerFoundAgain(null, null);
                }
                markerNotFound = false;
                markerNotFoundTE = 0.0;
                parent.graSys.groundNode.WorldTransformation = groundMarkerNode.WorldTransformation;
            }
            else if (parent.state == GameState.GameRunning)
            {
                markerNotFoundTE += timePassed;
            }
            if (markerNotFoundTE >= markerNotFoundThreshold && parent.state == GameState.GameRunning)
            {
                markerNotFound = true;
                parent.menSys.markerNotFound(null, null);
            }
        }

        private void UpdateMousePosition()
        {
            Vector3 nearSource = new Vector3(parent.graphics.GraphicsDevice.Viewport.Width / 2, parent.graphics.GraphicsDevice.Viewport.Height / 2, 0);
            Vector3 farSource = new Vector3(parent.graphics.GraphicsDevice.Viewport.Width / 2, parent.graphics.GraphicsDevice.Viewport.Height / 2, 1);

            Vector3 nearPoint = parent.graphics.GraphicsDevice.Viewport.Unproject(nearSource, State.ProjectionMatrix, State.ViewMatrix, Matrix.Identity);
            Vector3 farPoint = parent.graphics.GraphicsDevice.Viewport.Unproject(farSource, State.ProjectionMatrix, State.ViewMatrix, Matrix.Identity);

            Vector3 normRay = (farPoint - nearPoint);

            List<PickedObject> pickedObjects = ((NewtonPhysics)parent.gameScene.PhysicsEngine).PickRayCast(nearPoint, farPoint);

            if (pickedObjects.Count > 0)
            {
                pickedObjects.Sort();

                Vector3 worldPoint = nearPoint + normRay * pickedObjects[0].IntersectParam;

                Matrix m = Matrix.Invert(parent.graSys.groundNode.WorldTransformation);
                worldPoint = Vector3.Transform(worldPoint, m);

                parent.graSys.setCameraLightPosition(worldPoint, normRay);

                currentRelMousePosition = worldPoint;
            }
        }

        private void handleKeyboardInput()
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                parent.menSys.pauseGame(null,null);
            }
        }

        private void handleBuildInput(double timePassed)
        {
            bool infDown = Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A);
            bool tankDown = Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S);
            bool sieDown = Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D);
            bool resDown = Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W);
            if (infDown && !markerUsed)
            {
                timeExpired += timePassed;
                if (timeExpired >= keyThreshold)
                {
                    markerUsed = true;
                    parent.logSys.BuildAntiInfTower(currentRelMousePosition);
                }
            }
            if (tankDown && !markerUsed)
            {
                timeExpired += timePassed;
                if (timeExpired >= keyThreshold)
                {
                    markerUsed = true;
                    parent.logSys.BuildAntiTankTower(currentRelMousePosition);
                }
            }
            if (sieDown && !markerUsed)
            {
                timeExpired += timePassed;
                if (timeExpired >= keyThreshold)
                {
                    markerUsed = true;
                    parent.logSys.BuildAntiSiegeTower(currentRelMousePosition);
                }
            }
            if (resDown && !markerUsed)
            {
                timeExpired += timePassed;
                if (timeExpired >= keyThreshold)
                {
                    markerUsed = true;
                    parent.logSys.BuildResource(currentRelMousePosition);
                }
            }
            if (!infDown && !tankDown && !sieDown && !resDown)
            {
                markerUsed = false;
                timeExpired = 0.0;
            }
        }

        private bool inViewport(TransformNode pa)
        {
            bool inScreen = true;
            foreach (Node n in pa.Children)
            {
                if (n.GetType() != typeof(TransformNode))
                    continue;
                TransformNode node = (TransformNode)n;
                Vector3 pos = Vector3.Transform(Vector3.Transform(node.Translation, pa.WorldTransformation), parent.graSys.groundNode.WorldTransformation);
                Vector3 screenPos = parent.GraphicsDevice.Viewport.Project(pos, State.ProjectionMatrix, State.ViewMatrix, Matrix.Identity);
                if (screenPos.X < markerBorder)
                    inScreen = false;
                if (screenPos.Y < markerBorder)
                    inScreen = false;
                if (screenPos.X > parent.GraphicsDevice.Viewport.Width - markerBorder)
                    inScreen = false;
                if (screenPos.Y > parent.GraphicsDevice.Viewport.Height - markerBorder)
                    inScreen = false;
            }
            return inScreen;
        }

        private void handleMarkerInput(double timePassed)
        {
            if (!groundMarkerNode.MarkerFound)
            {
                infExp = 0.0;
                tankExp = 0.0;
                siegeExp = 0.0;
                resExp = 0.0;
                return;
            }
            //Console.WriteLine("InScreen: {0}", inViewport(tankNode));
            if (!infanteryMarkerNode.MarkerFound && !infDone && inViewport(infNode))
            {
                infExp += timePassed;
                if (infExp > markerTH)
                {
                    infDone = true;
                    parent.logSys.BuildAntiInfTower(currentRelMousePosition);
                }
            }
            else
            {
                infDone = !infanteryMarkerNode.MarkerFound;
                infExp = 0.0;
            }
            if (!tankMarkerNode.MarkerFound && !tankDone && inViewport(tankNode))
            {
                tankExp += timePassed;
                if (tankExp > markerTH)
                {
                    tankDone = true;
                    parent.logSys.BuildAntiTankTower(currentRelMousePosition);
                }
            }
            else
            {
                tankDone = !tankMarkerNode.MarkerFound;
                tankExp = 0.0;
            }
            if (!siegeMarkerNode.MarkerFound && !siegeDone && inViewport(siegeNode))
            {
                siegeExp += timePassed;
                if (siegeExp > markerTH)
                {
                    siegeDone = true;
                    parent.logSys.BuildAntiSiegeTower(currentRelMousePosition);
                }
            }
            else
            {
                siegeDone = !siegeMarkerNode.MarkerFound;
                siegeExp = 0.0;
            }
            if (!resMarkerNode.MarkerFound && !resDone && inViewport(resNode))
            {
                resExp += timePassed;
                if (resExp > markerTH)
                {
                    resDone = true;
                    parent.logSys.BuildResource(currentRelMousePosition);
                }
            }
            else
            {
                resDone = !resMarkerNode.MarkerFound;
                resExp = 0.0;
            }
        }

        private void setupMarkerBuilding()
        {
            infNode = new TransformNode("infNode");
            infNode.Translation = new Vector3(-36f, 13.0f, 0);
            for (int i = 0; i < 4; i++)
            {
                TransformNode t = new TransformNode();
                switch (i)
                {
                    case 0:
                        t.Translation = new Vector3(8.5f, 8.5f, 0);
                        break;
                    case 1:
                        t.Translation = new Vector3(-8.5f, 8.5f, 0);
                        break;
                    case 2:
                        t.Translation = new Vector3(8.5f, -8.5f, 0);
                        break;
                    case 3:
                        t.Translation = new Vector3(-8.5f, -8.5f, 0);
                        break;
                }

                GeometryNode g = new GeometryNode();
                g.Model = new Box(1, 1, 1);
                g.Material = new Material();
                g.Material.Diffuse = Color.Red.ToVector4();


                //t.AddChild(g);
                infNode.AddChild(t);
            }

            tankNode = new TransformNode("tankNode");
            tankNode.Translation = new Vector3(-13.4f, -14.0f, 0);
            for (int i = 0; i < 4; i++)
            {
                TransformNode t = new TransformNode();
                switch (i)
                {
                    case 0:
                        t.Translation = new Vector3(8.5f, 8.5f, 0);
                        break;
                    case 1:
                        t.Translation = new Vector3(-8.5f, 8.5f, 0);
                        break;
                    case 2:
                        t.Translation = new Vector3(8.5f, -8.5f, 0);
                        break;
                    case 3:
                        t.Translation = new Vector3(-8.5f, -8.5f, 0);
                        break;
                }

                GeometryNode g = new GeometryNode();
                g.Model = new Box(1, 1, 1);
                g.Material = new Material();
                g.Material.Diffuse = Color.Blue.ToVector4();


                //t.AddChild(g);
                tankNode.AddChild(t);
            }

            siegeNode = new TransformNode("siegeNode");
            siegeNode.Translation = new Vector3(-13.4f, 36.5f, 0);
            for (int i = 0; i < 4; i++)
            {
                TransformNode t = new TransformNode();
                switch (i)
                {
                    case 0:
                        t.Translation = new Vector3(8.5f, 8.5f, 0);
                        break;
                    case 1:
                        t.Translation = new Vector3(-8.5f, 8.5f, 0);
                        break;
                    case 2:
                        t.Translation = new Vector3(8.5f, -8.5f, 0);
                        break;
                    case 3:
                        t.Translation = new Vector3(-8.5f, -8.5f, 0);
                        break;
                }

                GeometryNode g = new GeometryNode();
                g.Model = new Box(1, 1, 1);
                g.Material = new Material();
                g.Material.Diffuse = Color.Green.ToVector4();


                //t.AddChild(g);
                siegeNode.AddChild(t);
            }

            resNode = new TransformNode("resNode");
            resNode.Translation = new Vector3(-36.0f, -36.0f, 0);
            for (int i = 0; i < 4; i++)
            {
                TransformNode t = new TransformNode();
                switch (i)
                {
                    case 0:
                        t.Translation = new Vector3(8.5f, 8.5f, 0);
                        break;
                    case 1:
                        t.Translation = new Vector3(-8.5f, 8.5f, 0);
                        break;
                    case 2:
                        t.Translation = new Vector3(8.5f, -8.5f, 0);
                        break;
                    case 3:
                        t.Translation = new Vector3(-8.5f, -8.5f, 0);
                        break;
                }

                GeometryNode g = new GeometryNode();
                g.Model = new Box(1, 1, 1);
                g.Material = new Material();
                g.Material.Diffuse = Color.Yellow.ToVector4();


                //t.AddChild(g);
                resNode.AddChild(t);
            }

            parent.graSys.groundNode.AddChild(infNode);
            parent.graSys.groundNode.AddChild(tankNode);
            parent.graSys.groundNode.AddChild(siegeNode);
            parent.graSys.groundNode.AddChild(resNode);
        }
    }
}