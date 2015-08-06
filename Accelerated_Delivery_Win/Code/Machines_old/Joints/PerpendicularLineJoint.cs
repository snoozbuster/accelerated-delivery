using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Constraints.TwoEntity.JointLimits;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.TwoEntity;

namespace Accelerated_Delivery_Win
{
    public class PerpendicularLineJoint : SolverGroup
    {
        public LinearAxisMotor LinearMotor { get; private set; }
        public LinearAxisLimit LinearLimit { get; private set; }
        public RevoluteMotor AngularMotor { get; private set; }
        public RevoluteLimit AngularLimit { get; private set; }
        public RevoluteAngularJoint AngularJoint { get; private set; }
        public PointOnLineJoint LinearJoint { get; private set; }

        /// <summary>
        /// Constructs a new constraint which restricts two degrees of linear freedom and two degrees of angular freedom between two entities.
        /// </summary>
        /// <param name="connectionA">First entity of the constraint pair.</param>
        /// <param name="connectionB">Second entity of the constraint pair.</param>
        /// <param name="lineAnchor">Location of the anchor for the line to be attached to connectionA in world space.</param>
        /// <param name="lineDirection">Axis in world space to be attached to connectionA along which connectionB can move and rotate.</param>
        /// <param name="pointAnchor">Location of the anchor for the point to be attached to connectionB in world space.</param>
        /// <param name="rotationAxis">Axis to rotate around.</param>
        public PerpendicularLineJoint(Entity connectionA, Entity connectionB,
            Vector3 lineAnchor, Vector3 lineDirection, Vector3 pointAnchor, Vector3 rotationAxis)
        {
            if(connectionA == null)
                connectionA = TwoEntityConstraint.WorldEntity;
            if(connectionB == null)
                connectionB = TwoEntityConstraint.WorldEntity;

            LinearJoint = new PointOnLineJoint(connectionA, connectionB, lineAnchor, lineDirection, pointAnchor);
            LinearMotor = new LinearAxisMotor(connectionA, connectionB, lineAnchor, pointAnchor, lineDirection);
            LinearLimit = new LinearAxisLimit(connectionA, connectionB, lineAnchor, pointAnchor, lineDirection, 0, 0);
            AngularJoint = new RevoluteAngularJoint(connectionA, connectionB, rotationAxis);
            AngularMotor = new RevoluteMotor(connectionA, connectionB, rotationAxis);
            AngularLimit = new RevoluteLimit(connectionA, connectionB);
            LinearLimit.IsActive = LinearMotor.IsActive = AngularMotor.IsActive = AngularLimit.IsActive = false;

            Add(LinearJoint);
            Add(LinearLimit);
            Add(LinearMotor);
            Add(AngularJoint);
            Add(AngularLimit);
            Add(AngularMotor);
        }
    }
}
