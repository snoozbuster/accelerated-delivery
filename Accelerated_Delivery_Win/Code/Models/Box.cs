using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Collidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics;
using BEPUphysics.Materials;

namespace Accelerated_Delivery_Win
{
    public class Box : BaseModel
    {
        public Color BoxColor { get; private set; }
        public bool NeedsRemoval { get; private set; }

        public int ExtraPoints { get; set; }
        protected BurstParticleSystem particles;

        protected int alpha = 255;
        protected byte deltaA = 10;

        protected bool fading;

        /// <summary>
        /// Constructor for boxes.
        /// </summary>
        /// <param name="color">If the box has a color, specify it here.</param>
        public Box(Color color) 
            : this(Program.Game.CurrentLevel.BoxSpawnPoint, color)
        { }

        /// <summary>
        /// Constructor for boxes.
        /// </summary>
        public Box()
            : this(Program.Game.CurrentLevel.BoxSpawnPoint, Color.White)
        { }

        public Box(Vector3 location)
            : this(location, Color.White)
        { }

        protected Box(Vector3 location, Color color)
            : base(color == Color.White ? Program.Game.Loader.boxModel : color == Color.Blue ? Program.Game.Loader.BlueBoxModel : Program.Game.Loader.BlackBoxModel,
            false, true, location)
        {
            particles = new BurstParticleSystem(Program.Game);
            particles.AutoInitialize(Program.Game.GraphicsDevice, Program.Game.Content, Program.Game.SpriteBatch);

            ModelPosition = location;
            Ent.Tag = this;
            Ent.CollisionInformation.Tag = this;
            Ent.CollisionInformation.Entity.Tag = this;
            BoxColor = color;
            Program.Game.Space.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if(fading || (ModelPosition.Z < Program.Game.CurrentLevel.RemovalHeight && !NeedsRemoval))
                DoFade();

            base.Update(gameTime);
        }

        public void PlayParticles()
        {
            particles.Emitter.PositionData.Position = ModelPosition;
            particles.Activate();
            particles.Emitter.BurstComplete += removeParticleSystemOnCompletion;
            RenderingDevice.Add(particles);
        }

        private void removeParticleSystemOnCompletion(object caller, EventArgs e)
        {
            RenderingDevice.Remove(particles);
        }

        public void DoFade()
        {
            fading = true;
            if(alpha - deltaA > 0)
                alpha -= deltaA;
            else
                alpha = 0;

            UseCustomAlpha = true;
            CustomAlpha = alpha / 255.0f;

            if(alpha == 0)
                NeedsRemoval = true;
        }
    }
}
