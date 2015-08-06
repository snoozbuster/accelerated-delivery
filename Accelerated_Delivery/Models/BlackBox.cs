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

        public static void Initialize(Effect eff, Texture2D[] bbblist) { effect = eff; blackBoxBillboardList = bbblist; }
        protected static Effect effect;
        protected static Texture2D[] blackBoxBillboardList;

        public BlackBox()
            : base(Color.Black)
        {
            boom = new Explosion(ModelPosition, 100, 10, GameManager.Space);
            explosionTime = random.Next(15, 60);
            GameManager.Space.DuringForcesUpdateables.Starting += updateVelocities;
            bbEffect = effect;

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

            buffer = new VertexBuffer(RenderingDevice.GraphicsDevice, VertexPositionTexture.VertexDeclaration, verts.Length, BufferUsage.None);
            buffer.SetData(verts);
        }

        protected void updateVelocities()
        {
            time += GameManager.Space.TimeStepSettings.TimeStepDuration;

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
            bbEffect.Parameters["xView"].SetValue(RenderingDevice.Camera.View);
            bbEffect.Parameters["xProjection"].SetValue(RenderingDevice.Camera.Projection);
            bbEffect.Parameters["xCamPos"].SetValue(RenderingDevice.Camera.Position);
            bbEffect.Parameters["xAllowedRotDir"].SetValue(Vector3.UnitZ);

            foreach(EffectPass pass in bbEffect.CurrentTechnique.Passes)
            {
                //bbEffect.Parameters["xBillboardTexture"].SetValue(activeList[i - moar]);
                bbEffect.Parameters["xBillboardTexture"].SetValue(blackBoxBillboardList[(int)(explosionTime - time)]);
                RenderingDevice.GraphicsDevice.SetVertexBuffer(buffer);
                pass.Apply();
                RenderingDevice.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, buffer.VertexCount / 3);
            }
        }

        protected void explode()
        {
            boom.Position = ModelPosition - Vector3.UnitZ * 5f;
            boom.Explode();
            MediaSystem.PlaySoundEffect(SFXOptions.Explosion);
            GameManager.Space.DuringForcesUpdateables.Starting -= updateVelocities;
            drawingBillboard = false;
            GameManager.CurrentLevel.VanishBox(this, false);
            PlayParticles();
        }

        public void Remove()
        {
            GameManager.Space.DuringForcesUpdateables.Starting -= updateVelocities;
        }
    }
}
