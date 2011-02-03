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

using SceneGraphDisplay;

namespace TowARDefense
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TowARDefense : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public SpriteFont textFont;
        public SpriteFont bigFont;
        public SpriteFont hudFont;

        public Scene gameScene;
        SGForm fs;

        public GameInput inpSys;
        public GameLogic logSys;
        public GameGraphics graSys;
        public GameMenu menSys;

        public GameState state;

        public Cue music;

        public int chosenMap;

        public int optionDifficultyMultiplier;
        public int optionPathNodesPerEnemy;
        // Weitere Optionen die nur toggle Besitzen: ShowFPS and Fullscreen

        public TowARDefense()
        {
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 34);
            IsFixedTimeStep = true;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            inpSys = new GameInput(this);
            logSys = new GameLogic(this);
            graSys = new GameGraphics(this);
            menSys = new GameMenu(this);

            optionDifficultyMultiplier = 1;
            optionPathNodesPerEnemy = 0;
        }

        protected override void Initialize()
        {

            State.InitGoblin(graphics, Content, "");
            State.ThreadOption = (ushort)ThreadOptions.MarkerTracking;
            State.ShowFPS = true;
            State.ShowTriangleCount = true;
            State.ShowNotifications = true;
            //graphics.ToggleFullScreen();

            gameScene = new Scene(this);
            gameScene.PhysicsEngine = new NewtonPhysics();
            ((NewtonPhysics)gameScene.PhysicsEngine).MaxSimulationSubSteps = 1;

            fs = null;
            /*fs = new SGForm(gameScene);
            MouseInput.Instance.MouseClickEvent += new
            HandleMouseClick(fs.SG_MouseClickHandler);
            fs.RunTool();
            fs.Show();*/

            GoblinXNA.Sounds.Sound.Initialize("towardefsound");

            inpSys.Init();
            graSys.Init();
            logSys.Init();
            menSys.Init();

            base.Initialize();

            state = GameState.MainMenu;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            textFont = Content.Load<SpriteFont>("textFont");
            bigFont = Content.Load<SpriteFont>("bigFont");
            hudFont = Content.Load<SpriteFont>("HUD");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /// 
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (state == GameState.GameRunning)
            {
                //Console.WriteLine(gameTime.ElapsedRealTime.TotalSeconds.ToString());
                logSys.Update(gameTime);
                graSys.Update(gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                menSys.Update(gameTime);
            }
            inpSys.Update(gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)

        {

            GraphicsDevice device = graphics.GraphicsDevice;
            device.Clear(Color.Black);

            if (fs != null)
                fs.UpdatePickedObjectDrawing();
            if (state == GameState.GameRunning)
            {
                inpSys.Draw();
                logSys.Draw();
                graSys.Draw();
            }
            else
            {
                menSys.Draw();
            }

            base.Draw(gameTime);
        }

        public void resetGame(object sender, EventArgs ev)
        {
            // Cleanup and remove all Game Objects
            foreach (Enemy e in logSys.enemies)
            {
                e.state = ObjectState.Cleanup;
                e.Cleanup();
            }
            foreach (DefenseTower t in logSys.towers)
            {
                t.state = ObjectState.Cleanup;
                t.Cleanup();
            }
            foreach (ResourceBuilding r in logSys.resBuildings)
            {
                r.state = ObjectState.Cleanup;
                r.Cleanup();
            }
            foreach (Projectile p in logSys.projectiles)
            {
                p.cleanup = true;
                p.Cleanup();
            }

            while (logSys.enemies.Count > 0)
                logSys.enemies.RemoveAt(0);
            while (logSys.towers.Count > 0)
                logSys.towers.RemoveAt(0);
            while (logSys.resBuildings.Count > 0)
                logSys.resBuildings.RemoveAt(0);
            while (logSys.projectiles.Count > 0)
                logSys.projectiles.RemoveAt(0);

            // Reset Grid
            logSys.kiSys.pathGrid.setupGrid();

            logSys.min = 0;
            logSys.sec = 0;
            logSys.timeLasted = 0;

            logSys.mainBuilding.setToMaxHealth();

            logSys.kiSys.reset();

            logSys.resourceCount = 100;

            state = GameState.GameRunning;
        }
    }
}
