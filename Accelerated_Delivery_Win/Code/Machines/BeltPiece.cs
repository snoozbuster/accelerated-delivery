using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;

namespace Accelerated_Delivery_Win
{
    public class BeltPiece : ISpaceObject, IRenderableObject
    {
        protected BaseModel[] models;
        protected Tube[] tubes;

        /// <summary>
        /// A piece of a belt that holds tubes and whatnot.
        /// </summary>
        /// <param name="numberOfTubes">Number of tubes in the piece. Includes first tube.</param>
        /// <param name="direction">Direction that tubes propagate.</param>
        /// <param name="models">Kinetic models in the piece.</param>
        /// <param name="initialTubeLocation">Location of the first tube.</param>
        /// <param name="forward">If the tubes go "forward" (push boxes toward positive axis) or "backward" (push boxes toward negative axis).</param>
        /// <param name="inXDirection">True if the tube is aligned on the x-axis, false otherwise.</param>
        public BeltPiece(Vector3 initialTubeLocation, int numberOfTubes, Vector3 direction, bool inXDirection, bool forward, params BaseModel[] models)
        {
            tubes = new Tube[numberOfTubes];
            this.models = models;
            foreach(BaseModel m in models)
                if(m.Ent.IsDynamic)
                    throw new NotSupportedException("Dynamic models not supported in BeltPieces.");

            for(int i = 0; i < numberOfTubes; i++)
                tubes[i] = new Tube(initialTubeLocation + direction * i, inXDirection, !forward);
        }

        /// <summary>
        /// A piece of a belt that holds tubes and whatnot.
        /// </summary>
        /// <param name="numberOfTubes">Number of tubes in the piece. Includes first tube.</param>
        /// <param name="direction">Direction that tubes propagate.</param>
        /// <param name="models">Kinetic models in the piece.</param>
        /// <param name="initialTubeLocation">Location of the first tube.</param>
        /// <param name="forward">If the tubes go "forward" (push boxes toward positive axis) or "backward" (push boxes toward negative axis).</param>
        /// <param name="inXDirection">True if the tube is aligned on the x-axis, false otherwise.</param>
        /// <param name="tubeRotation">Custom rotation of tubes around Z-axis in degrees (ignores inXDirection).</param>
        public BeltPiece(Vector3 initialTubeLocation, int numberOfTubes, Vector3 direction, bool inXDirection, bool forward, float tubeRotation, params BaseModel[] models)
            : this(initialTubeLocation, numberOfTubes, direction, inXDirection, forward, models)
        {
            foreach(Tube t in tubes)
                t.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(tubeRotation));
        }

        /// <summary>
        /// Updates the Tubes contained in the BeltPiece.
        /// </summary>
        /// <param name="gameTime">The GameTime.</param>
        public void Update(GameTime gameTime)
        {
            foreach(Tube t in tubes)
                t.Update(gameTime);
        }

        /// <summary>
        /// Adds the BeltPiece to the RenderingDevice.
        /// </summary>
        public void AddToRenderer()
        {
            foreach(BaseModel m in models.Union(tubes))
                RenderingDevice.Add(m);
        }

        /// <summary>
        /// Removes the BeltPiece from the RenderingDevice.
        /// </summary>
        public void RemoveFromRenderer()
        {
            foreach(BaseModel m in models.Union(tubes))
                RenderingDevice.Remove(m);
        }

        /// <summary>
        /// Adds the BeltPiece to an ISpace.
        /// </summary>
        /// <param name="newSpace"></param>
        public void OnAdditionToSpace(ISpace newSpace)
        {
            foreach(BaseModel m in models.Union(tubes))
                newSpace.Add(m);
        }

        /// <summary>
        /// Adds the BeltPiece to an ISpace.
        /// </summary>
        /// <param name="oldSpace"></param>
        public void OnRemovalFromSpace(ISpace oldSpace)
        {
            foreach(BaseModel m in models.Union(tubes))
                oldSpace.Remove(m);
        }

        public ISpace Space { get; set; }

        public object Tag { get; set; }
    }
}
