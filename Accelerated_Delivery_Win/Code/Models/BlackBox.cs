using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;

namespace Accelerated_Delivery_Win
{
    public class BlackBox : Box, ISpaceObject
    {
        protected Explosion boom;
        protected float time = 0;
        protected readonly float explosionTime;

        protected readonly static Random random = new Random();

        private Effect bbEffect;
        private VertexBuffer buffer;
        private VertexPositionTexture[] verts;
        private bool drawingBillboard;
        
        public BlackBox()
            : base(Color.Black)
        {
            boom = new Explosion(ModelPosition, 100, 10, Program.Game.Space);
            explosionTime = random.Next(20, 125);
            Program.Game.Space.DuringForcesUpdateables.Starting += updateVelocities;
            bbEffect = Program.Game.Content.Load<Effect>("Shaders/bbEffect");

            int i = 0;
            Vector3 currentV3 = Vector3.UnitZ * 5;
            //currentV3.Z += 5;
            verts = new VertexPositionTexture[6];
            verts[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 0));
            verts[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 0));
            verts[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 1));

            verts[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 0));
            verts[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 1));
            verts[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 1));

            buffer = new VertexBuffer(Program.Game.GraphicsDevice, VertexPositionTexture.VertexDeclaration, verts.Length, BufferUsage.None);
            buffer.SetData(verts);
        }

        protected void updateVelocities()
        {
            time += Program.Game.Space.TimeStepSettings.TimeStepDuration;

            if(time >= explosionTime)
            {
                explode();
                return; // prevents division by zero
            }

            if(explosionTime - time < 10)
                drawingBillboard = true;

            if(drawingBillboard)
            {
                for(int i = 0; i < verts.Length; i++)
                    verts[i].Position = this.Ent.Position + new Vector3(Vector2.Zero, 5);
                buffer.SetData(verts);
            }
        }

        /// <summary>
        /// Draws the billboard if necessary.
        /// </summary>
        public void Draw()
        {
            if(!drawingBillboard)
                return;

            bbEffect.CurrentTechnique = bbEffect.Techniques["CylBillboard"];

            bbEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            bbEffect.Parameters["xView"].SetValue(Program.Game.Camera.View);
            bbEffect.Parameters["xProjection"].SetValue(Program.Game.Camera.Projection);
            bbEffect.Parameters["xCamPos"].SetValue(Program.Game.Camera.Position);
            bbEffect.Parameters["xAllowedRotDir"].SetValue(Vector3.UnitZ);

            foreach(EffectPass pass in bbEffect.CurrentTechnique.Passes)
            {
                //bbEffect.Parameters["xBillboardTexture"].SetValue(activeList[i - moar]);
                bbEffect.Parameters["xBillboardTexture"].SetValue(Program.Game.Loader.blackBoxBillboardList[(int)(explosionTime - time)]);
                Program.Game.GraphicsDevice.SetVertexBuffer(buffer);
                pass.Apply();
                Program.Game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, buffer.VertexCount / 3);
            }
        }

        protected void explode()
        {
            boom.Position = ModelPosition - Vector3.UnitZ * 5f;
            boom.Explode();
            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Explosion);
            Program.Game.Space.DuringForcesUpdateables.Starting -= updateVelocities;
            drawingBillboard = false;
            Program.Game.CurrentLevel.VanishBox(this, false);
            PlayParticles();
        }

        internal void Remove()
        {
            Program.Game.Space.DuringForcesUpdateables.Starting -= updateVelocities;
        }
    }
}
