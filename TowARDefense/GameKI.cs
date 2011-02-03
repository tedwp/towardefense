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
using GoblinXNA.UI.UI2D;
using Sound = GoblinXNA.Sounds.Sound;

namespace TowARDefense
{
    public class GameKI
    {
        private TowARDefense parent;

        public PathGrid pathGrid;
        public EnemyWave wave;
        public int stage;

        private double timeExpired;
        private double timeBetweenWaves;

        private double transitionTime;
        private TransitionState transitionState;
        private Rectangle transRec;

        public GameKI(TowARDefense parent_f)
        {
            parent = parent_f;

            timeBetweenWaves = 15.0;

            transitionState = TransitionState.TransitionDone;

            pathGrid = new PathGrid(parent);

            transRec = new Rectangle(450, 544, 50, 50);

            stage = 0;
        }

        public void Update(double timePassed)
        {
            pathGrid.Update(timePassed);

            timeExpired += timePassed;

            if (wave.done)
            {
                timeExpired += timePassed;
                if (timeExpired >= timeBetweenWaves)
                {
                    stage++;
                    wave = new EnemyWave(stage, parent);
                    Sound.Play("newwave");
                    transitionTime = 0.0;
                    transitionState = TransitionState.TransitionIn;
                    timeExpired = 0.0;
                }
            }
            else
            {
                wave.Update(timePassed);
            }

            updateStageTransition(timePassed);
        }

        public void Init()
        {
            pathGrid.Init();

            wave = new EnemyWave(0, parent);
            wave.done = true;
        }

        public void Draw()
        {
            drawStageTransition();
            UI2DRenderer.WriteText(Vector2.Zero, "Stage " + stage.ToString(), Color.White,
                    parent.bigFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Bottom);
        }

        private void drawStageTransition()
        {
            if (transitionState == TransitionState.TransitionIn)
            {
                UI2DRenderer.FillRectangle(transRec, parent.graSys.stageChangeTexture, ColorHelper.ApplyAlphaToColor(Color.White, (float)(transitionTime/2.0)));
            }
            else if (transitionState == TransitionState.Transition)
            {
                UI2DRenderer.FillRectangle(transRec, parent.graSys.stageChangeTexture, ColorHelper.ApplyAlphaToColor(Color.White, 0.5f));
            }
            else if (transitionState == TransitionState.TransitionOut)
            {
                UI2DRenderer.FillRectangle(transRec, parent.graSys.stageChangeTexture, ColorHelper.ApplyAlphaToColor(Color.White, 0.5f-(float)(transitionTime / 2.0)));
            }
        }

        private void updateStageTransition(double timePassed)
        {
            transitionTime += timePassed;
            if (transitionState == TransitionState.TransitionIn && transitionTime >= 1)
            {
                transitionTime = 0.0;
                transitionState = TransitionState.Transition;
            }
            else if (transitionState == TransitionState.Transition && transitionTime >= 2.0)
            {
                transitionTime = 0.0;
                transitionState = TransitionState.TransitionOut;
            }
            else if (transitionState == TransitionState.TransitionOut && transitionTime >= 1)
            {
                transitionTime = 0.0;
                transitionState = TransitionState.TransitionDone;
            }
        }

        public void reset()
        {
            stage = 0;
            wave = new EnemyWave(stage, parent);
            wave.done = true;

            timeExpired = 0.0;
        }
    }
}
