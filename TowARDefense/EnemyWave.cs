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
    public class EnemyWave
    {
        private TowARDefense parent;

        List<Enemy> spawns;
        Enemy boss;

        private double spawnInterval;
        private int rot;
        private double rotSpawn;
        private double rotSpawnExp;
        private double timeExpired;

        private float infSpawnProb;
        private float tankSpawnProb;

        public bool done;

        public EnemyWave(int stage, TowARDefense parent_f)
        {
            parent = parent_f;

            spawns = new List<Enemy>();

            if (stage != 0)
            {
                setupEnemiesForWave(stage);
            }
        }

        public void Update(double timePassed)
        {
            timeExpired += timePassed;
            if (timeExpired >= spawnInterval && !done)
            {
                rotSpawnExp += timePassed;
                if (spawns.Count > 0 && rotSpawnExp >= rotSpawn)
                {
                    spawns[0].Spawn(parent.logSys.kiSys.pathGrid.spawnNodes[rot]);
                    spawns.RemoveAt(0);
                    rot++;
                    rotSpawnExp = 0.0;
                }
                if (rot == parent.logSys.kiSys.pathGrid.spawnNodes.Count)
                {
                    timeExpired = 0.0;
                    rotSpawnExp = 0.0;
                    rot = 0;
                }

                if ((rotSpawnExp >= rotSpawn) && (parent.logSys.enemies.Count <= 2) && (boss.state == ObjectState.WaitingForSpawn))
                {
                    Console.WriteLine("Boss Spawn");
                    int r = RandomHelper.GetRandomInt(parent.logSys.kiSys.pathGrid.spawnNodes.Count);
                    boss.Spawn(parent.logSys.kiSys.pathGrid.spawnNodes[r]);
                }
                if (parent.logSys.enemies.Count <= 2 && (boss.state == ObjectState.Destroyed || boss.state == ObjectState.Cleanup || boss == null))
                {
                    done = true;
                }
            }
        }

        private void setupEnemiesForWave(int stage)
        {
            //Sound.Play("newwave");

            double mul = 1;

            if (stage > 6)
                mul = Math.Pow(0.5, stage - 6) * mul;


            int enemyCount =(int)(mul * parent.optionDifficultyMultiplier * Math.Exp(0.5 * (stage + 1))) + 1;
            Console.WriteLine("EnemyCount of stage {0}: {1}", stage, enemyCount);

            spawnInterval = 1.5 + 0.3 * parent.logSys.kiSys.pathGrid.spawnNodes.Count;
            rot = 0;
            rotSpawn = 0.3;

            for (int k = 0; k < enemyCount; k++)
            {
                for (int i = 0; i < parent.logSys.kiSys.pathGrid.spawnNodes.Count; i++)
                {
                   spawns.Add(getRandomEnemyForStage(stage));
                }
            }

            boss = getBoss(stage);
        }

        private Enemy getRandomEnemyForStage(int stage)
        {
            if (stage == 1)
            {
                infSpawnProb = 0.75f;
                tankSpawnProb = 0.25f;
            }
            else if (stage == 2)
            {
                infSpawnProb = 0.6f;
                tankSpawnProb = 0.4f;
            }
            else if (stage == 3)
            {
                infSpawnProb = 0.65f;
                tankSpawnProb = 0.25f;
            }
            else if (stage == 4)
            {
                infSpawnProb = 0.25f;
                tankSpawnProb = 0.5f;
            }
            else
            {
                infSpawnProb = 0.25f;
                tankSpawnProb = 0.5f;
            }
            float r = RandomHelper.GetRandomFloat(0.0f, 1.0f);
            if (r > infSpawnProb + tankSpawnProb)
            {
                return new Enemies.Siege(parent);
            }
            if (r > infSpawnProb)
            {
                return new Enemies.Tank(parent);
            }
            return new Enemies.Infantery(parent);
        }

        private Enemy getBoss(int stage)
        {
            switch (stage)
            {
                case 1:
                    return new Enemies.Boss1(parent);
                case 2:
                    return new Enemies.Boss2(parent);
                case 3:
                    return new Enemies.Boss3(parent);
                default:
                    return new Enemies.BossUpper(parent, stage-3);
            }
        }
    }
}
