using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Accelerated_Delivery_Win
{
    /// <summary>
    /// Represents a portion of a level defined by a "track" piece and any number of associated tubes.
    /// </summary>
    public struct TrackPiece
    {
        /// <summary>
        /// Base model of the piece.
        /// </summary>
        public readonly BaseModel Model;
        /// <summary>
        /// Tubes in the piece.
        /// </summary>
        public readonly Tube[] Tubes;

        /// <summary>
        /// Creates a new TrackPiece.
        /// </summary>
        /// <param name="model">The model that contains the specified tubes.</param>
        /// <param name="tubes">The tubes contained by the specified model.</param>
        public TrackPiece(BaseModel model, params Tube[] tubes)
        {
            Model = model;
            Tubes = tubes;
        }

        /// <summary>
        /// Updates the model and the tubes.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Model.Update(gameTime);
            foreach(Tube t in Tubes)
                t.Update(gameTime);
        }

        /// <summary>
        /// Rotates the piece around the Z axis. 
        /// </summary>
        /// <param name="degrees">Degrees to rotate. 0 is the original rotation. Rotations are not cumulative.</param>
        public void RotatePieceOnZ(float degrees)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(degrees));
            Model.Ent.Orientation = Model.OriginalOrientation * q;
            foreach(Tube t in Tubes)
                t.Rotate(q);
        }

        /// <summary>
        /// Moves the piece to a new location.
        /// </summary>
        /// <param name="newPos">This is relative to the center of the model, tubes are restructured around it.</param>
        public void SetPosition(Vector3 newPos)
        {
            Vector3 oldPos = Model.ModelPosition;
            Model.Ent.Position = newPos;
            foreach(Tube t in Tubes)
                t.ModelPosition = newPos - t.Origin;
        }
    }
}
