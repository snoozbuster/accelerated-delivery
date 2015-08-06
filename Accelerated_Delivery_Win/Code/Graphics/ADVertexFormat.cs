using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Accelerated_Delivery_Win
{
    public struct ADVertexFormat
    {
        private Vector3 position;
        private Vector2 texCoord;
        private Vector3 normal;

        public ADVertexFormat(Vector3 position, Vector2 texCoord, Vector3 normal)
        {
            this.position = position;
            this.texCoord = texCoord;
            this.normal = normal;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * (3 + 2), VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }
}
