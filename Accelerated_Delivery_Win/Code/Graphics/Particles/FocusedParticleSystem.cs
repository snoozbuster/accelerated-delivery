using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DPSF;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Accelerated_Delivery_Win
{
    public class FocusedBurstSystem : DefaultTexturedQuadParticleSystem
    {
        protected Vector3 target;
        protected Vector3 position;

        public FocusedBurstSystem(Game game, Vector3 targetDirection, Vector3 pos)
            : base(game) 
        { 
            target = targetDirection;
            position = pos;
        }

        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 50, 100,
                UpdateVertexProperties, "textures/FlowerBurst");

            loadParticleSystem();
        }

        protected void loadParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            InitialProperties.AccelerationMax = target * 5f;
            InitialProperties.AccelerationMin = target * 2.7f;
            InitialProperties.EndColorMax = new Color(128, 128, 128, 200);
            InitialProperties.EndColorMin = new Color(70, 70, 70, 200);
            InitialProperties.EndSizeMax = 15;
            InitialProperties.EndSizeMin = 9;
            //InitialProperties.InterpolateBetweenMinAndMaxAcceleration = true;
            InitialProperties.InterpolateBetweenMinAndMaxColors = true;
            InitialProperties.LifetimeMax = 0.75f;
            InitialProperties.LifetimeMin = 0.5f;
            InitialProperties.PositionMax = new Vector3(0.2f);
            InitialProperties.PositionMin = new Vector3(-0.2f);
            //InitialProperties.InterpolateBetweenMinAndMaxPosition = true;
            InitialProperties.StartColorMax = Color.Gray;
            InitialProperties.StartColorMin = new Color(128, 128, 128, 200);
            InitialProperties.StartSizeMax = 7;
            InitialProperties.StartSizeMin = 5;
            InitialProperties.VelocityIsAffectedByEmittersOrientation = false;
            InitialProperties.VelocityMax = target * 9f;
            InitialProperties.VelocityMin = target * 2.4f;
            //InitialProperties.InterpolateBetweenMinAndMaxVelocity = true;

            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 50);

            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 100);
            Emitter.EmitParticlesAutomatically = false;
            Emitter.ParticlesPerSecond = 100;
            Emitter.PositionData.Position = position;
        }

        public void Activate()
        {
            Emitter.BurstParticles = 30;
        }

        public void Reset()
        {
            Emitter.BurstParticles = 0;
            this.RemoveAllParticles();
        }
    }
}
