using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Accelerated_Delivery_Win
{
    public class BurstParticleSystem : DefaultTexturedQuadParticleSystem
    {
        public BurstParticleSystem(Game game)
            : base(game) { }

        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 50, 100,
                UpdateVertexProperties, "textures/FlowerBurst");

            loadParticleSystem();
        }

        protected void loadParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            InitialProperties.AccelerationMax = new Vector3(3f, 3f, 2f);
            InitialProperties.AccelerationMin = new Vector3(-3f, -3f, -2f);
            InitialProperties.EndColorMax = InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndSizeMax = 12;
            InitialProperties.EndSizeMin = 7;
            //InitialProperties.InterpolateBetweenMinAndMaxAcceleration = true;
            InitialProperties.InterpolateBetweenMinAndMaxColors = true;
            InitialProperties.LifetimeMax = 0.75f;
            InitialProperties.LifetimeMin = 0.5f;
            InitialProperties.PositionMax = new Vector3(0, 0, 0.5f);
            InitialProperties.PositionMin = new Vector3(0, 0, -0.5f);
            //InitialProperties.InterpolateBetweenMinAndMaxPosition = true;
            InitialProperties.StartColorMax = InitialProperties.StartColorMin = Color.Gray;
            InitialProperties.StartSizeMax = 4;
            InitialProperties.StartSizeMin = 3;
            //InitialProperties.VelocityIsAffectedByEmittersOrientation = true;
            InitialProperties.VelocityMax = new Vector3(9f, 9f, 7f);
            InitialProperties.VelocityMin = new Vector3(-9f, -9f, -7f);
            //InitialProperties.
            //InitialProperties.InterpolateBetweenMinAndMaxVelocity = true;

            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp);

            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 100);
            Emitter.EmitParticlesAutomatically = false;
            Emitter.ParticlesPerSecond = 200;
            Emitter.PositionData.Position = Vector3.Zero;
        }

        public void Activate()
        {
            Emitter.BurstParticles = 50;
        }
    }
}
