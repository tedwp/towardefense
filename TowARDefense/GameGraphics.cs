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
    public class GameGraphics
    {
        private TowARDefense parent;

        public TransformNode groundNode;
        public TransformNode towerNode;
        public TransformNode resNode;
        public TransformNode enemyNode;
        public TransformNode projNode;

        public GeometryNode groundModelNode;
        public TransformNode cameraTestNode;

        public TTerrain terrain;

        private TransformNode cameraSphereTransNode;

        public LightNode cameraLightNode;
        public TransformNode cameraLightTransNode;

        public Texture2D greenRectangleTexture;
        public Texture2D blackTransparentTexture;
        public Texture2D mainMenuTexture;
        public Texture2D resourcesTexture;
        public Texture2D boxTexture;
        public Texture2D stageChangeTexture;

        public Model mainBModel;
        public Model resBModel;
        public Model resRModel;

        public Model map1Model;
        public Model map2Model;

        // Models for Shots
        public Model infShotModel;
        public Model tankShotModel;
        public Model siegeShotModel;
        public Model antiInfShotModel;
        public Model antiTankShotModel;
        public Model antiSiegeShotModel;

        // Models for Towers
        public Model antiInfTowerModel;
        public Model antiTankTowerModel;
        public Model antiSiegeTowerModel;

        // Models for Turrets
        public Model infTurretModel;
        public Model tankTurretModel;
        public Model siegeTurretModel;
        public Model antiInfTurretModel;
        public Model antiTankTurretModel;
        public Model antiSiegeTurretModel;

        // Models for Enemiez
        public Model infModel;
        public Model tankModel;
        public Model siegeModel;

        public Model boss1Model;
        public Model boss2Model;
        public Model boss3Model;
        public Model boss4Model;

        public Model boss1TurretModel;
        public Model boss2TurretModel;
        public Model boss3TurretModel;
        public Model boss4TurretModel;

        // Materials for Kugelnode
        public Material kugelWhite;
        public Material kugelRed;

        public GameGraphics(TowARDefense parent_f)
        {
            parent = parent_f;

            groundNode = new TransformNode("groundNode");
            enemyNode = new TransformNode("enemyNode");
            towerNode = new TransformNode("towerNode");
            resNode = new TransformNode("resNode");
            projNode = new TransformNode("projectileNode");

            cameraLightTransNode = new TransformNode("CameraLightTransNode");

            groundNode.AddChild(enemyNode);
            groundNode.AddChild(towerNode);
            groundNode.AddChild(resNode);
            groundNode.AddChild(projNode);
        }

        public void Init()
        {
            parent.gameScene.RootNode.AddChild(groundNode);

            cameraSphereTransNode = new TransformNode("CameraSphere");
            groundNode.AddChild(cameraSphereTransNode);
            parent.gameScene.RootNode.AddChild(cameraLightTransNode);

            terrain = new TTerrain("terrain", groundNode, groundModelNode, parent);

            Load();
        }

        public void Update(double timePassed)
        {
            terrain.Update(timePassed);
        }

        public void Draw()
        {
            foreach (Enemy e in parent.logSys.enemies)
            {
                e.Draw();
            }
            foreach (DefenseTower d in parent.logSys.towers)
            {
                d.Draw();
            }
            foreach (ResourceBuilding r in parent.logSys.resBuildings)
            {
                r.Draw();
            }
            parent.logSys.mainBuilding.Draw();
        }

        public void loadMap()
        {
            if (parent.chosenMap == 0)
            {
                groundModelNode.Model = map1Model;
                terrain = new TTerrain("terrain", groundNode, groundModelNode, parent);
            }
            else
            {
                groundModelNode.Model = map2Model;
                terrain = new TTerrain("terrain-map2", groundNode, groundModelNode, parent);
            }
        }

        public void Load()
        {
            LoadMaterials();
            LoadModels();
            LoadLights();
        }

        private void LoadLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(-1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();

            LightSource lightSource2 = new LightSource();
            lightSource2.Direction = new Vector3(1, 0, 0);
            lightSource2.Diffuse = Color.White.ToVector4();

            LightSource lightSource3 = new LightSource();
            lightSource3.Direction = new Vector3(-0.5f, 0, 1);
            lightSource3.Diffuse = Color.White.ToVector4();

            // Create a light node to hold the light source
            LightNode lightNode1 = new LightNode();
            lightNode1.LightSources.Add(lightSource);

            LightNode lightNode2 = new LightNode();
            lightNode2.LightSources.Add(lightSource2);

            LightNode lightNode3 = new LightNode();
            lightNode3.LightSources.Add(lightSource3);

            parent.graSys.groundNode.AddChild(lightNode1);
            parent.graSys.groundNode.AddChild(lightNode2);
            parent.graSys.groundNode.AddChild(lightNode3);

            /*LightSource cameraLightSource = new LightSource();
            cameraLightSource.Type = LightType.SpotLight;
            cameraLightSource.Diffuse = Color.Yellow.ToVector4();
            cameraLightSource.Specular = Color.Yellow.ToVector4();
            cameraLightSource.Range = 10000;

            cameraLightNode = new LightNode("cameraLight");
            cameraLightNode.LightSources.Add(cameraLightSource);

            cameraLightTransNode.AddChild(cameraLightNode);*/
        }

        private void LoadMaterials()
        {
            ModelLoader loader = new ModelLoader();

            greenRectangleTexture = new Texture2D(parent.graphics.GraphicsDevice, 1, 1);
            greenRectangleTexture.SetData(new Color[] { Color.White });

            blackTransparentTexture = Texture2D.FromFile(parent.graphics.GraphicsDevice, "../../../Content/black-transparent.png");

            mainMenuTexture = Texture2D.FromFile(parent.graphics.GraphicsDevice, "../../../Content/mainmenu.png");

            resourcesTexture = Texture2D.FromFile(parent.graphics.GraphicsDevice, "../../../Content/resources.png");

            boxTexture = Texture2D.FromFile(parent.graphics.GraphicsDevice, "../../../Content/box.png");

            stageChangeTexture = Texture2D.FromFile(parent.graphics.GraphicsDevice, "../../../Content/stageChange.png");

            kugelWhite = new Material();
            kugelWhite.Diffuse = ColorHelper.ApplyAlphaToColor(Color.White, 0.15f).ToVector4();
            kugelWhite.SpecularPower = 0;
        }

        private void LoadModels()
        {
            ModelLoader loader = new ModelLoader();

            // mainBuildingModel
            mainBModel = (Model)loader.Load("Models", "mainBuild");
            mainBModel.UseInternalMaterials = true;
            mainBModel.CastShadows = true;
            mainBModel.UseLighting = true;

            //res BuildingModel
            resBModel = (Model)loader.Load("Models", "resource-body");
            resBModel.UseInternalMaterials = true;
            resBModel.CastShadows = true;
            resBModel.UseLighting = true;

            resRModel = (Model)loader.Load("Models", "resource-wings");
            resRModel.UseInternalMaterials = true;
            resRModel.CastShadows = true;
            resRModel.UseLighting = true;

            // Models for shots
            infShotModel = new Box(0.6f, 0.3f, 0.3f);
            tankShotModel = (Model)loader.Load("Models", "tankbullet");
            tankShotModel.UseInternalMaterials = true;
            tankShotModel.UseLighting = true;
            siegeShotModel = (Model)loader.Load("Models", "catapult-bullet");
            siegeShotModel.UseInternalMaterials = true;
            siegeShotModel.UseLighting = true;

            antiInfShotModel = new Box(0.9f, 0.3f, 0.3f);
            antiInfShotModel.UseLighting = true;

            antiTankShotModel = new Box(0.6f, 0.3f, 0.3f);
            antiTankShotModel.UseLighting = true;
            antiSiegeShotModel = new Box(0.9f, 0.3f, 0.3f);
            antiSiegeShotModel.UseLighting = true;

            // Models for Towers
            antiInfTowerModel = (Model)loader.Load("Models", "tower2-Body");
            antiInfTowerModel.UseInternalMaterials = true;
            antiInfTowerModel.UseLighting = true;
            antiTankTowerModel = (Model)loader.Load("Models", "tanktower-body");
            antiTankTowerModel.UseInternalMaterials = true;
            antiTankTowerModel.UseLighting = true;
            antiSiegeTowerModel = (Model)loader.Load("Models", "Laser-Body");
            antiSiegeTowerModel.UseInternalMaterials = true;
            antiSiegeTowerModel.UseLighting = true;

            // Models for Turrets
            infTurretModel = (Model)loader.Load("Models", "Robot-weapon");
            infTurretModel.UseLighting = true;
            infTurretModel.UseInternalMaterials = true;
            tankTurretModel = (Model)loader.Load("Models", "tank-weapon");
            tankTurretModel.UseInternalMaterials = true;
            tankTurretModel.UseLighting = true;
            siegeTurretModel = (Model)loader.Load("Models", "Catapult-weapon");
            siegeTurretModel.UseInternalMaterials = true;
            siegeTurretModel.UseLighting = true;

            antiInfTurretModel = (Model)loader.Load("Models", "tower2-weapon");
            antiInfTurretModel.UseInternalMaterials = true;
            antiInfTurretModel.UseLighting = true;
            antiTankTurretModel = (Model)loader.Load("Models", "tanktower-weapon");
            antiTankTurretModel.UseInternalMaterials = true;
            antiTankTurretModel.UseLighting = true;
            antiSiegeTurretModel = (Model)loader.Load("Models", "Laser-Weapon");
            antiSiegeTurretModel.UseInternalMaterials = true;
            antiSiegeTurretModel.UseLighting = true;

            // Models for Enemiez
            infModel = (Model)loader.Load("Models", "Robot-Body"); ;
            infModel.UseLighting = true;
            infModel.UseInternalMaterials = true;
            tankModel = (Model)loader.Load("Models", "tank-body");
            tankModel.UseLighting = true;
            tankModel.UseInternalMaterials = true;
            siegeModel = (Model)loader.Load("Models", "Catapult-body");
            siegeModel.UseLighting = true;
            siegeModel.UseInternalMaterials = true;

            boss1Model = (Model)loader.Load("Models", "tankboss-body");
            boss1Model.UseLighting = true;
            boss1Model.UseInternalMaterials = true;
            boss1TurretModel = (Model)loader.Load("Models", "tankboss-weapon");
            boss1TurretModel.UseLighting = true;
            boss1TurretModel.UseInternalMaterials = true;

            boss2Model = (Model)loader.Load("Models", "Boss2-Body");
            boss2Model.UseLighting = true;
            boss2Model.UseInternalMaterials = true;
            boss2TurretModel = (Model)loader.Load("Models", "Boss2-weapon");
            boss2TurretModel.UseLighting = true;
            boss2TurretModel.UseInternalMaterials = true;

            boss3Model = (Model)loader.Load("Models", "fighter");
            boss3Model.UseLighting = true;
            boss3Model.UseInternalMaterials = true;
            boss3TurretModel = new Box(0.1f, 0.1f, 0.1f);
            boss3TurretModel.UseLighting = true;
            boss3TurretModel.UseInternalMaterials = true;

            boss4Model = (Model)loader.Load("Models", "tankboss-body");
            boss4Model.UseLighting = true;
            boss4Model.UseInternalMaterials = true;
            boss4TurretModel = (Model)loader.Load("Models", "tankboss-weapon");
            boss4TurretModel.UseLighting = true;
            boss4TurretModel.UseInternalMaterials = true;

            groundModelNode = new GeometryNode("groundPlane");

            map1Model = (Model)loader.Load("", "newmap");
            map1Model.UseInternalMaterials = true;
            map1Model.UseLighting = false;
            map1Model.ReceiveShadows = false;

            map2Model = (Model)loader.Load("", "map2");
            map2Model.UseInternalMaterials = true;
            map2Model.UseLighting = false;
            map2Model.ReceiveShadows = false;

            groundModelNode.Model = map1Model;
            groundModelNode.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            groundModelNode.Physics.Pickable = true;
            groundModelNode.AddToPhysicsEngine = true;

            groundNode.AddChild(groundModelNode);


            // Nodes for the Camera Stuff
            Material cameraTestNodeMat = new Material();
            cameraTestNodeMat.Ambient = new Vector4(255, 255, 255, 20);
            cameraTestNodeMat.Diffuse = new Vector4(255, 255, 255, 20);
            cameraTestNodeMat.SpecularPower = 0;
            cameraTestNode = new TransformNode();
            parent.inpSys.cameraModelNode = new GeometryNode("CameraTest");
            parent.inpSys.cameraModelNode.Model = new Sphere(3.0f, 10, 10);
            parent.inpSys.cameraModelNode.Model.ReceiveShadows = false;
            parent.inpSys.cameraModelNode.Model.UseLighting = false;
            parent.inpSys.cameraModelNode.Material = kugelWhite;
            parent.inpSys.cameraModelNode.Physics.Shape = ShapeType.Sphere;

            cameraTestNode.AddChild(parent.inpSys.cameraModelNode);
            cameraSphereTransNode.AddChild(cameraTestNode);
        }

        public void setCameraLightPosition(Vector3 Position, Vector3 Direction)
        {
            Vector3 normal;
            float height;

            if (terrain.getHeightMapInfo(Position, out height, out normal))
            {
                Position.Z = height;
                cameraSphereTransNode.Translation = Position;
            }
        }
    }
}