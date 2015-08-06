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

namespace Accelerated_Delivery_Win
{
    public class Billboard
    {
        private Effect bbEffect;
        public List<VertexBuffer> boardVertexBuffer;
        public VertexDeclaration boardVertexDeclaration;

        public Billboard()
        {
            bbEffect = Program.Game.Content.Load<Effect>("Shaders/bbEffect");
            boardVertexBuffer = new List<VertexBuffer>();
        }

        public void CreateBillboardVerticesFromList(List<Vector3> boardList)
        {
            List<VertexPositionTexture[]> billboardVertices = new List<VertexPositionTexture[]>(boardList.Count);
            int i = 0;
            foreach(Vector3 currentV3 in boardList)
            {
                VertexPositionTexture[] temp = new VertexPositionTexture[6];
                temp[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 0));
                temp[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 0));
                temp[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 1));

                temp[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 0));
                temp[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 1));
                temp[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 1));
                billboardVertices.Add(temp);
                i = 0;
            }

            foreach(VertexPositionTexture[] verts in billboardVertices)
            {
                boardVertexBuffer.Add(new VertexBuffer(Program.Game.GraphicsDevice, VertexPositionTexture.VertexDeclaration, verts.Length, BufferUsage.None));
                boardVertexBuffer[i].SetData(billboardVertices[i]);
                i++;
            }
        }

        public void DrawBillboards(Texture2D[] textureList, Texture2D[] activeList)
        {
            Program.Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            bbEffect.CurrentTechnique = bbEffect.Techniques["CylBillboard"];
            bbEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            bbEffect.Parameters["xView"].SetValue(Program.Game.Camera.View);
            bbEffect.Parameters["xProjection"].SetValue(Program.Game.Camera.Projection);
            bbEffect.Parameters["xCamPos"].SetValue(Program.Game.Camera.Position);
            bbEffect.Parameters["xAllowedRotDir"].SetValue(Vector3.UnitZ);

            int moar = 0;
            for(int i = 0; i < textureList.Length + moar; i++)
            {
                if(Program.Game.CurrentLevel.MachineList.Count >= i + 1 && Program.Game.CurrentLevel.MachineList.ElementAt(i).Value)
                {
                    foreach(EffectPass pass in bbEffect.CurrentTechnique.Passes)
                    {
                        if(Program.Game.CurrentLevel.MachineList.ElementAt(i).Key.IsActive)
                            bbEffect.Parameters["xBillboardTexture"].SetValue(activeList[i - moar]);
                        else
                            bbEffect.Parameters["xBillboardTexture"].SetValue(textureList[i - moar]);
                        Program.Game.GraphicsDevice.SetVertexBuffer(boardVertexBuffer[i - moar]);
                        pass.Apply();
                        if((Program.Game.LevelNumber == 10 && ((Program.Game.CurrentLevel.BoxSpawnPoint.X < 100 && i - moar < 5) || Program.Game.CurrentLevel.BoxSpawnPoint.X >= 100)) ||
                            Program.Game.LevelNumber != 10)
                            Program.Game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, boardVertexBuffer[i - moar].VertexCount / 3);
                    }
                }
                else if(Program.Game.CurrentLevel.MachineList.Count >= i + 1)
                    moar++;
            }

            Program.Game.GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
