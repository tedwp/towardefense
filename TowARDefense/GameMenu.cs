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
    public class GameMenu
    {
        private TowARDefense parent;

        private Menus.MainMenu mainMenu;
        private Menus.PauseMenu pauseMenu;
        private Menus.Menu optionsMenu;
        private Menus.MarkerNotFoundScreen markerNotFoundScreen;
        private Menus.HowToPlay howToPlay;
        private Menus.HighScoreScreen highScoreScreen;
        public Menus.LostScreen lostScreen;
        private Menus.choseMapScreen choseMap;

        public bool downKeyDown;
        public bool upKeyDown;
        public bool enterKeyDown;

        private Cue mainMenuCue;

        public MenuState state;

        public GameMenu(TowARDefense parent_f)
        {
            parent = parent_f;

            downKeyDown = false;
            upKeyDown = false;
            enterKeyDown = false;

            mainMenu = new Menus.MainMenu(parent);
            pauseMenu = new Menus.PauseMenu(parent);
            optionsMenu = new Menus.OptionMenu(parent);
            howToPlay = new Menus.HowToPlay(parent);
            markerNotFoundScreen = new Menus.MarkerNotFoundScreen(parent);
            highScoreScreen = new Menus.HighScoreScreen(parent);
            lostScreen = new Menus.LostScreen(parent);
            choseMap = new Menus.choseMapScreen(parent);

        }

        public void Init()
        {
            // Einträge Main Menu
            mainMenu.Init();

            // Einträge pause Menu
            pauseMenu.Init();

            // Einträge options Menu
            optionsMenu.Init();

            howToPlay.Init();

            markerNotFoundScreen.Init();

            highScoreScreen.Init();

            lostScreen.Init();

            choseMap.Init();

            mainMenuCue = Sound.Play("main_menu");

            state = MenuState.MainMenu;
        }

        public void Update(GameTime gameTime)
        {

            double timePassed = gameTime.ElapsedGameTime.TotalSeconds;

            switch (state)
            {
                case MenuState.OptionsMenu:
                    optionsMenu.Update(timePassed);
                    break;
                case MenuState.MainMenu:
                    mainMenu.Update(timePassed);
                    break;
                case MenuState.PauseMenu:
                    pauseMenu.Update(timePassed);
                    break;
                case MenuState.MarkerNotFound:
                    markerNotFoundScreen.Update(timePassed);
                    break;
                case MenuState.HowToPlay:
                    howToPlay.Update(timePassed);
                    break;
                case MenuState.HighScore:
                    highScoreScreen.Update(timePassed);
                    break;
                case MenuState.LostScreen:
                    lostScreen.Update(timePassed);
                    break;
                case MenuState.ChoseMap:
                    choseMap.Update(timePassed);
                    break;
            }
        }

        public void Draw()
        {
            switch (state)
            {
                case MenuState.OptionsMenu:
                    optionsMenu.Draw();
                    break;
                case MenuState.MainMenu:
                    mainMenu.Draw();
                    break;
                case MenuState.PauseMenu:
                    pauseMenu.Draw();
                    break;
                case MenuState.MarkerNotFound:
                    markerNotFoundScreen.Draw();
                    break;
                case MenuState.HowToPlay:
                    howToPlay.Draw();
                    break;
                case MenuState.HighScore:
                    highScoreScreen.Draw();
                    break;
                case MenuState.LostScreen:
                    lostScreen.Draw();
                    break;
                case MenuState.ChoseMap:
                    choseMap.Draw();
                    break;
            }
        }

        public void startGame(object sender, EventArgs e)
        {
            parent.graSys.loadMap();
            parent.logSys.kiSys.pathGrid.setupGrid();
            parent.logSys.mainBuilding.posForLevel();

            parent.state = GameState.GameRunning;
            parent.ResetElapsedTime();
            if (mainMenuCue.IsPlaying)
                mainMenuCue.Stop(AudioStopOptions.AsAuthored);
            parent.music = Sound.Play("game-sound");
        }

        public void pauseGame(object sender, EventArgs e)
        {
            state = MenuState.PauseMenu;
            parent.state = GameState.PauseMenu;           
        }

        public void markerNotFound(object sender, EventArgs e)
        {
            state = MenuState.MarkerNotFound;
            parent.state = GameState.MarkerNotFound;
        }

        public void markerFoundAgain(object sender, EventArgs e)
        {
            state = MenuState.PauseMenu;
            parent.state = GameState.GameRunning;
        }

        public void quitGame(object sender, EventArgs e)
        {
            parent.Exit();
        }
    }
}
