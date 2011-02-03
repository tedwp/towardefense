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


namespace TowARDefense
{
    public enum GameState { MainMenu, PauseMenu, GameRunning, MarkerNotFound, Lost }
    public enum MenuState { MainMenu, OptionsMenu, PauseMenu, MarkerNotFound, HowToPlay, HighScore, LostScreen, ChoseMap }
    public enum TransitionState { TransitionIn, TransitionOut, Transition, TransitionDone }

    public enum ObjectState { Construction, WaitingForSpawn, Moving, Idle, Destroyed, Cleanup }
    public enum TurretStates { Idle, Firing, Reloading, Destroyed, Cleanup }

    public enum GameWeapons { Infantery, Tank, Siege, Special, AntiInfantery, AntiTank, AntiSiege }
}