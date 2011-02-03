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

namespace TowARDefense {
    public class GameLogic
    {
        private TowARDefense parent;
        public GameKI kiSys;

        private int resourcesPerSec;
        public int resourceCount;

        public List<DefenseTower> towers;
        public List<ResourceBuilding> resBuildings;
        public List<Enemy> enemies;
        public List<Projectile> projectiles;
        public MainBuilding mainBuilding;

        private float[,] damageAttMatrix;
        private float[,] damageDefMatrix;

        private double resourceTimeExpired;

        public int timeLasted;
        public double timeHelper;
        public int min;
        public int sec;

        public float mapWidth;
        public float mapHeight;
        public float buildRadius;

        private TransitionState messTransState;
        private string messageText;
        private double messTransTime;

        private Rectangle r1;
        private Vector2 t;

        private Rectangle r2;
        private Vector2 t2;

        public GameLogic(TowARDefense parent_f)
	    {
            parent = parent_f;
            kiSys = new GameKI(parent);

            towers = new List<DefenseTower>();
            resBuildings = new List<ResourceBuilding>();
            enemies = new List<Enemy>();
            projectiles = new List<Projectile>();

            /* Arrangement:
             * Erste Ebene: AntiInfTower, AntiTankTower, AntiSiegeTower
             * Zweite Ebene: Inf, Tank, Siege */
            damageDefMatrix = new float[3, 3];
            damageDefMatrix[0, 0] = 1.0f;
            damageDefMatrix[0, 1] = 0.4f;
            damageDefMatrix[0, 2] = 0.8f;
            damageDefMatrix[1, 0] = 0.5f;
            damageDefMatrix[1, 1] = 1.0f;
            damageDefMatrix[1, 2] = 0.8f;
            damageDefMatrix[2, 0] = 0.2f;
            damageDefMatrix[2, 1] = 0.2f;
            damageDefMatrix[2, 2] = 1.0f;
            /* Arrangement:
             * Erste Ebene: Inf, Tank, Siege
             * Zweite Ebene: AntiInfTower, AntiTankTower, AntiSiegeTower */
            damageAttMatrix = new float[3, 3];
            damageAttMatrix[0, 0] = 0.5f;
            damageAttMatrix[0, 1] = 0.8f;
            damageAttMatrix[0, 2] = 1.0f;
            damageAttMatrix[1, 0] = 0.9f;
            damageAttMatrix[1, 1] = 0.5f;
            damageAttMatrix[1, 2] = 1.0f;
            damageAttMatrix[2, 0] = 1.0f;
            damageAttMatrix[2, 1] = 1.0f;
            damageAttMatrix[2, 1] = 0.2f;

            resourcesPerSec = 3;
            resourceCount = 100;

            resourceTimeExpired = 0.0;

            timeLasted = 0;
            timeHelper = 0.0;

            messTransState = TransitionState.TransitionDone;
	    }

        public void Init()
        {
            mainBuilding = new MainBuilding(new Vector3(0, 0, 0), parent);

            r1 = new Rectangle(parent.GraphicsDevice.Viewport.Width - 100, 20, 25, 25);
            t = new Vector2(parent.GraphicsDevice.Viewport.Width - 72, 23);

            r2 = new Rectangle(parent.GraphicsDevice.Viewport.Width/2 - 60, 20, 120, 25);
            t2 = new Vector2(parent.GraphicsDevice.Viewport.Width/2 - 26, 23);

            kiSys.Init();
        }

        public void Update(GameTime gt)
        {
            double timePassed = gt.ElapsedGameTime.TotalSeconds;
            resourceTimeExpired += timePassed;

            if (!(parent.state == GameState.Lost))
            {
                timeHelper += gt.ElapsedGameTime.TotalSeconds;
                if (timeHelper > 1.0)
                {
                    timeLasted++;
                    timeHelper = 0.0;
                }

                /*if (timeLasted > 10)
                    mainBuilding.dealDamage(100000, GameWeapons.AntiInfantery);*/

                min = (int)Math.Floor((double)timeLasted / 60.0);
                sec = timeLasted - min*60;
                kiSys.Update(timePassed);

                mainBuilding.Update(timePassed);

                for (int i = towers.Count - 1; i >= 0; i--)
                {
                    towers[i].Update(timePassed);
                    if (towers[i].state == ObjectState.Cleanup)
                    {
                        for (int k = projectiles.Count - 1; k >= 0; k--)
                        {
                            if (projectiles[k].target == towers[i])
                            {
                                projectiles[k].cleanup = true;
                            }
                        }
                        towers[i].Cleanup();
                        towers.Remove(towers[i]);
                    }
                }
                for (int i = resBuildings.Count - 1; i >= 0; i--)
                {
                    resBuildings[i].Update(timePassed);
                    if (resBuildings[i].state == ObjectState.Cleanup)
                    {
                        for (int k = projectiles.Count - 1; k >= 0; k--)
                        {
                            if (projectiles[k].target == resBuildings[i])
                            {
                                projectiles[k].cleanup = true;
                            }
                        }
                        resBuildings[i].Cleanup();
                        resBuildings.Remove(resBuildings[i]);
                    }
                }
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies[i].Update(timePassed);
                    if (enemies[i].state == ObjectState.Cleanup)
                    {
                        for (int k = projectiles.Count - 1; k >= 0; k--)
                        {
                            if (projectiles[k].target == enemies[i])
                            {
                                projectiles[k].cleanup = true;
                            }
                        }
                        enemies[i].Cleanup();
                        enemies.Remove(enemies[i]);
                    }

                }
                for (int i = projectiles.Count - 1; i >= 0; i--)
                {
                    projectiles[i].Update(timePassed);
                    if (projectiles[i].cleanup)
                    {
                        projectiles[i].Cleanup();
                        projectiles.Remove(projectiles[i]);
                    }
                }

                if (resourceTimeExpired >= 1.0)
                {
                    resourceCount += resourcesPerSec;
                    resourceTimeExpired = 0.0;
                }

            }

            updateMessage(timePassed);
        }

        public void Draw()
        {
            kiSys.Draw();
            if (!(parent.state == GameState.Lost))
            {
                UI2DRenderer.FillRectangle(r1, parent.graSys.resourcesTexture, Color.White);
                UI2DRenderer.WriteText(t, resourceCount.ToString(), Color.White, parent.hudFont);

                UI2DRenderer.FillRectangle(r2, parent.graSys.boxTexture, Color.White);
                UI2DRenderer.WriteText(t2, String.Format("{0:00}", min) + ":" + String.Format("{0:00}", sec), Color.White, parent.hudFont);
            }
            drawMessage();
        }

        public void BuildAntiInfTower(Vector3 pos)
        {
            Vector3 normal;
            float height;
            if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                if (height > parent.graSys.terrain.towerThreshold  && nothingNear(pos))
                {
                    if (resourceCount < Towers.AntiInfTower.getPrice())
                    {
                        showMessage("Zu wenig Ressourcen");
                        return;
                    }
                    if (!nothingNear(pos))
                    {
                        showMessage("Zu nah an anderem Gebaeude");
                        return;
                    }
                    if (toFarFromMainBuilding(pos))
                    {
                        showMessage("Zu weit weg vom Hauptgebaeude");
                        return;
                    }
                    Sound.Play("buildtowerneu");
                    Towers.AntiInfTower tower = new Towers.AntiInfTower(pos, parent);
                    resourceCount -= Towers.AntiInfTower.getPrice();
                    towers.Add(tower);
                    kiSys.pathGrid.pathUpdateTow(tower);
                }
            }
        }

        public void BuildAntiTankTower(Vector3 pos)
        {
            Vector3 normal;
            float height;
            if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                if (height > parent.graSys.terrain.towerThreshold)
                {
                    if (resourceCount < Towers.AntiTankTower.getPrice())
                    {
                        showMessage("Zu wenig Ressourcen");
                        return;
                    }
                    if (!nothingNear(pos))
                    {
                        showMessage("Zu nah an anderem Gebaeude");
                        return;
                    }
                    if (toFarFromMainBuilding(pos))
                    {
                        showMessage("Zu weit weg vom Hauptgebaeude");
                        return;
                    }
                    Sound.Play("buildtowerneu");
                    Towers.AntiTankTower tower = new Towers.AntiTankTower(pos, parent);
                    resourceCount -= Towers.AntiTankTower.getPrice();
                    towers.Add(tower);
                    kiSys.pathGrid.pathUpdateTow(tower);
                }
            }
        }

        public void BuildAntiSiegeTower(Vector3 pos)
        {
            Vector3 normal;
            float height;
            if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                if (height > parent.graSys.terrain.towerThreshold)
                {
                    if (resourceCount < Towers.AntiSiegeTower.getPrice())
                    {
                        showMessage("Zu wenig Ressourcen");
                        return;
                    }
                    if (!nothingNear(pos))
                    {
                        showMessage("Zu nah an anderem Gebaeude");
                        return;
                    }
                    if (toFarFromMainBuilding(pos))
                    {
                        showMessage("Zu weit weg vom Hauptgebaeude");
                        return;
                    }
                    Sound.Play("buildtowerneu");
                    Towers.AntiSiegeTower tower = new Towers.AntiSiegeTower(pos, parent);
                    resourceCount -= Towers.AntiSiegeTower.getPrice();
                    towers.Add(tower);
                    kiSys.pathGrid.pathUpdateTow(tower);
                }
            }
        }

        public void BuildResource(Vector3 pos)
        {
            Vector3 normal;
            float height;
            if (parent.graSys.terrain.getHeightMapInfo(pos, out height, out normal))
            {
                if (height < parent.graSys.terrain.pathThreshold)
                {
                    if (resourceCount < ResourceBuilding.getPrice())
                    {
                        showMessage("Zu wenig Ressourcen");
                        return;
                    }
                    if (!nothingNear(pos))
                    {
                        showMessage("Zu nah an anderem Gebaeude");
                        return;
                    }
                    if (toFarFromMainBuilding(pos))
                    {
                        showMessage("Zu weit weg vom Hauptgebaeude");
                        return;
                    }
                    Sound.Play("buildtowerneu");
                    resourceCount -= ResourceBuilding.getPrice();
                    ResourceBuilding res = new ResourceBuilding(pos, parent);
                    resBuildings.Add(res);
                    kiSys.pathGrid.pathUpdateRes(res);
                }
            }
        }

        private bool nothingNear(Vector3 pos)
        {
            Vector2 pos2d = new Vector2(pos.X, pos.Y);

            bool canBuild = true;
            foreach (DefenseTower d in towers)
            {
                canBuild = canBuild && ((d.position2d - pos2d).Length() >= parent.inpSys.minDistance);
            }
            foreach (ResourceBuilding r in resBuildings)
            {
                canBuild = canBuild && ((r.position2d - pos2d).Length() >= parent.inpSys.minDistance);
            }
            canBuild = canBuild && ((mainBuilding.position2d - pos2d).Length() >= parent.inpSys.minDistance);
            return canBuild;
        }

        public void showMessage(string text)
        {
            messageText = text;
            messTransState = TransitionState.TransitionIn;
        }

        private void drawMessage()
        {
            if (messTransState == TransitionState.TransitionIn)
            {
                UI2DRenderer.WriteText(Vector2.Zero, messageText, ColorHelper.ApplyAlphaToColor(Color.White, ((float)messTransTime / 1.0f)), parent.textFont, Vector2.One, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Center);
            }
            else if (messTransState == TransitionState.Transition)
            {
                UI2DRenderer.WriteText(Vector2.Zero, messageText, Color.White, parent.textFont, Vector2.One, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Center);
            }
            else if (messTransState == TransitionState.TransitionOut)
            {
                UI2DRenderer.WriteText(Vector2.Zero, messageText, ColorHelper.ApplyAlphaToColor(Color.White, 1.0f - ((float)messTransTime / 1.0f)), parent.textFont, Vector2.One, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Center);
            }
        }

        private void updateMessage(double timePassed)
        {
            messTransTime += timePassed;
            if (messTransState == TransitionState.TransitionIn && messTransTime >= 1.0)
            {
                messTransTime = 0.0;
                messTransState = TransitionState.Transition;
            }
            else if (messTransState == TransitionState.Transition && messTransTime >= 0.5)
            {
                messTransTime = 0.0;
                messTransState = TransitionState.TransitionOut;
            }
            else if (messTransState == TransitionState.TransitionOut && messTransTime >= 1.0)
            {
                messTransTime = 0.0;
                messTransState = TransitionState.TransitionDone;
            }
        }

        private bool toFarFromMainBuilding(Vector3 position)
        {
            Vector2 pos2d = new Vector2(position.X, position.Y);
            pos2d = mainBuilding.position2d - pos2d;

            //Console.WriteLine("Radius: {0}; Pos: {1};", buildRadius, pos2d.Length());

            if (pos2d.Length() <= buildRadius)
                return false;
            return true;
        }

    }
}