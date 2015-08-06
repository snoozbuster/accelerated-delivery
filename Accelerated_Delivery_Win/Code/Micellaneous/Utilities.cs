using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;
using BEPUphysics.Entities;
using Accelerated_Delivery_Win;
using BEPUphysics.MathExtensions;

namespace Accelerated_Delivery_Win
{
    public static class Extensions
    {
        static Extensions()
        {
#if DEBUG
            effect = new BasicEffect(Program.Game.GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;
#endif
        }

        #region BoundingBox extension - Draw()
#if DEBUG
        static VertexPositionColor[] verts = new VertexPositionColor[8];
        static BasicEffect effect;

        static int[] indices = new int[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4,
        };

        public static void Draw(this BoundingBox box)
        {
            if(Program.Game.HiDef)
            {
                effect.View = Program.Game.Camera.View;
                effect.Projection = Program.Game.Camera.Projection;

                Vector3[] corners = box.GetCorners();
                for(int i = 0; i < 8; i++)
                {
                    verts[i].Position = corners[i];
                    verts[i].Color = Color.Goldenrod;
                }

                foreach(EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Program.Game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, verts, 0, 8, indices, 0, indices.Length / 2);
                }
            }
        }
#endif
        #endregion

        #region Vector3 extension - ToRadians
        /// <summary>
        /// Converts each member of a Vector3 into a radian representation.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector3 ToRadians(this Vector3 degrees)
        {
            return new Vector3(MathHelper.ToRadians(degrees.X), MathHelper.ToRadians(degrees.Y), MathHelper.ToRadians(degrees.Z));
        }
        #endregion

        #region Vector3 extension - AngleBetween()
        public static float AngleBetween(this Vector3 v1, Vector3 v2)
        {
            float dot = Vector3.Dot(v1, v2);
            float mag1 = Math.Abs(v1.Length());
            float mag2 = Math.Abs(v2.Length());
            return (float)Math.Acos(dot / (mag1 * mag2));
        }
        #endregion
    }
}
