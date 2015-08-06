using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Collidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics;

namespace Accelerated_Delivery_Win
{
    public class Goal
    {
        private static event Action OnGDMReset;

        public Color CatcherColor { get; private set; }
        public Entity Ent { get; private set; }
        public Entity BlackSquare { get; private set; }
        private VertexBuffer buff;
        private Texture2D blackTex;

        protected Vector3 pos;
        protected Color col;

        protected int size;
        protected bool sideways;

        public Goal(Vector3 pos) 
            : this(pos, Color.White)
        { }

        public Goal(Vector3 pos, bool sideways)
        {
            Ent = new BEPUphysics.Entities.Prefabs.Box(pos, 0.1f, 9, 9f);
            BlackSquare = new BEPUphysics.Entities.Prefabs.Box(pos - new Vector3(2f, 0, 0), 0.1f, 9, 9f);
            Ent.CollisionInformation.Events.InitialCollisionDetected += OnCollision;
            BlackSquare.CollisionInformation.CollisionRules.Group = BaseModel.noSolverGroupL;
            BlackSquare.CollisionInformation.Events.InitialCollisionDetected += onBlackCollision;
            CatcherColor = col = Color.White;

            size = 6;
            this.sideways = sideways;
            this.pos = pos;

            onGDMCreation();
            OnGDMReset += onGDMCreation;
        }

        public Goal(Vector3 pos, Color col)
        {
            Ent = new BEPUphysics.Entities.Prefabs.Box(pos, 7, 7, 0.1f);
            BlackSquare = new BEPUphysics.Entities.Prefabs.Box(pos + new Vector3(0, 0, 2f), 7, 7, 0.1f);
            BlackSquare.CollisionInformation.Events.InitialCollisionDetected += onBlackCollision;
            BlackSquare.CollisionInformation.CollisionRules.Group = BaseModel.noSolverGroupL;
            Ent.CollisionInformation.Events.InitialCollisionDetected += OnCollision;
            CatcherColor = col;

            this.pos = pos;
            this.col = col;
            
            size = 3;
            if(col != Color.White)
                size++;

            onGDMCreation();
            OnGDMReset += onGDMCreation;
        }

        public static void GDMReset()
        {
            OnGDMReset();
        }

        public void AddToGame(Space s)
        {
            RenderingDevice.Add(buff, blackTex);
            if(Ent.Space == null)
                s.Add(Ent);
            if(BlackSquare.Space == null)
                s.Add(BlackSquare);
        }
        public void RemoveFromGame()
        {
            RenderingDevice.Remove(buff);
            if(Ent.Space != null)
                Ent.Space.Remove(Ent);
        }

        private void OnCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            var otherEntity = other as EntityCollidable;
            if(otherEntity != null && otherEntity.Tag is Box)
            {
                if((otherEntity.Tag as Box).BoxColor != CatcherColor)
                    GameManager.CurrentLevel.DestroyBox((Box)otherEntity.Tag);
                else
                    GameManager.CurrentLevel.CatchBox((Box)otherEntity.Tag);
            }
        }

        private void onBlackCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if(other.Tag is Box)
                (other.Tag as Box).DoFade(false);
        }

        private void onGDMCreation()
        {
            ADVertexFormat[] verts = new ADVertexFormat[4];
            if(sideways)
            {
                verts[0] = new ADVertexFormat(Vector3.Transform(new Vector3(-size, size, 1.5f), Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.PiOver2)) + pos, new Vector2(0, 0), new Vector3(0, 0, 1));
                verts[1] = new ADVertexFormat(Vector3.Transform(new Vector3(-size, -size, 1.5f), Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.PiOver2)) + pos, new Vector2(0, 1), new Vector3(0, 0, 1));
                verts[2] = new ADVertexFormat(Vector3.Transform(new Vector3(size, size, 1.5f), Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.PiOver2)) + pos, new Vector2(1, 1), new Vector3(0, 0, 1));
                verts[3] = new ADVertexFormat(Vector3.Transform(new Vector3(size, -size, 1.5f), Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.PiOver2)) + pos, new Vector2(1, 0), new Vector3(0, 0, 1));
            }
            else
            {
                verts[0] = new ADVertexFormat(new Vector3(-size, size, 1.5f) + pos, new Vector2(0, 0), new Vector3(0, 0, 1));
                verts[1] = new ADVertexFormat(new Vector3(-size, -size, 1.5f) + pos, new Vector2(0, 1), new Vector3(0, 0, 1));
                verts[2] = new ADVertexFormat(new Vector3(size, size, 1.5f) + pos, new Vector2(1, 1), new Vector3(0, 0, 1));
                verts[3] = new ADVertexFormat(new Vector3(size, -size, 1.5f) + pos, new Vector2(1, 0), new Vector3(0, 0, 1));
            }

            buff = new VertexBuffer(RenderingDevice.GraphicsDevice, ADVertexFormat.VertexDeclaration, 4, BufferUsage.WriteOnly);
            buff.SetData(verts);

            blackTex = new Texture2D(RenderingDevice.GraphicsDevice, 1, 1);
            blackTex.SetData(new Color[] { new Color(0, 0, 0, 255) });
        }
    }
}
