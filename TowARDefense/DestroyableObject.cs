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
using GoblinXNA.Graphics.ParticleEffects;

namespace TowARDefense
{
    abstract public class DestroyableObject : Placeable
    {
        Random random = new Random();//add

        private Rectangle healthRecBorder;
        private Rectangle healthRec;

        protected int health;
        protected int maxHealth;

        protected bool explosion;

        private int explosionDur;
        private int explosionEl;

        abstract public void dealDamage(int damage, GameWeapons weaponType);

        public ObjectState state;

        public DestroyableObject(TowARDefense parent_f)
            : base(parent_f)
        {
            healthRecBorder = new Rectangle();
            healthRec = new Rectangle();

            explosion = false;

            explosionDur = 10;
            explosionEl = 0;

            createParticles();
        }

        protected void setHealth(int max)
        {
            maxHealth = max;
            health = max;
        }

        protected void setHealthBarSize(int healthBarWidth, int healthBarHeight)
        {
            healthRecBorder.Height = healthBarHeight + 2;
            healthRecBorder.Width = healthBarWidth + 2;
            healthRec.Height = healthBarHeight;
        }

        public void createParticles()
        {
            SmokePlumeParticleEffect smokeParticles = new SmokePlumeParticleEffect();
            FireParticleEffect fireParticles = new FireParticleEffect();
            // The order defines which particle effect to render first. Since we want
            // to show the fire particles in front of the smoke particles, we make
            // the smoke particles to be rendered first, and then fire particles
            smokeParticles.DrawOrder = 200;
            fireParticles.DrawOrder = 300;

            // Create a particle node to hold these two particle effects
            ParticleNode EffectNode = new ParticleNode();
            EffectNode.ParticleEffects.Add(smokeParticles);
            EffectNode.ParticleEffects.Add(fireParticles);

            smokeParticles.Duration = new TimeSpan(0, 0, 0, 0, 1500);
            fireParticles.Duration = new TimeSpan(0, 0, 0, 0, 750);

            EffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateEffects);

            gtNode.AddChild(EffectNode);

        }

        private void UpdateEffects(Matrix worldTransform, List<ParticleEffect> particleEffects)//add
        {
            if (explosion)
            {
                explosionEl++;
                foreach (ParticleEffect particle in particleEffects)
                {
                    particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), Vector3.Zero);
                }
                if (explosionEl > explosionDur)
                {
                    explosion = false;
                    explosionEl = 0;
                }
            }
        }


        // Get a random point on a circle

        private Vector3 RandomPointOnCircle(Vector3 pos)//add
        {
            const float radius = 0.1f;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector3(x * radius + pos.X, y * radius + pos.Y, pos.Z);
        }

        public void Draw()
        {
            if (health == maxHealth)
                return;
            if (state == ObjectState.Destroyed ||
                state == ObjectState.Cleanup ||
                state == ObjectState.WaitingForSpawn)
                return;
            Vector3 pos = Vector3.Transform(gtNode.Translation, parent.graSys.groundNode.WorldTransformation);
            pos = parent.graphics.GraphicsDevice.Viewport.Project(pos, State.ProjectionMatrix, State.ViewMatrix, Matrix.Identity);

            if (health > 0)
            {
                if (pos.X <= parent.graphics.GraphicsDevice.Viewport.Width && pos.X <= parent.graphics.GraphicsDevice.Viewport.Height)
                {
                    float ratio = (float)health / (float)maxHealth;
                    healthRecBorder.X = (int)(pos.X - healthRecBorder.Width / 2);
                    healthRecBorder.Y = (int)pos.Y;
                    healthRec.X = (int)(pos.X - healthRecBorder.Width / 2) + 1;
                    healthRec.Y = (int)(pos.Y + 1);
                    healthRec.Width = (int)(ratio * (healthRecBorder.Width - 2));
                    UI2DRenderer.DrawRectangle(healthRecBorder, Color.Black, 1);
                    if (ratio > 0.6f)
                    {
                        UI2DRenderer.FillRectangle(healthRec, parent.graSys.greenRectangleTexture, Color.DarkGreen);
                    }
                    else if (ratio > 0.3f)
                    {
                        UI2DRenderer.FillRectangle(healthRec, parent.graSys.greenRectangleTexture, Color.Orange);
                    }
                    else
                    {
                        UI2DRenderer.FillRectangle(healthRec, parent.graSys.greenRectangleTexture, Color.DarkRed);
                    }
                }
            }
        }

        public void setToMaxHealth()
        {
            health = maxHealth;
        }

        protected void explode()
        {
            explosion = true;
        }
    }
}
