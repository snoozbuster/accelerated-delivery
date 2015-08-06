using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;

namespace Accelerated_Delivery_Win
{
    public class BeltMachine : OperationalMachine
    {
        public override bool IsActive { get { return true; } }

        protected readonly Vector3 firstPoint, secondPoint;
        protected readonly List<PerpendicularLineJoint> joints = new List<PerpendicularLineJoint>();
        //protected readonly float angle;
        protected readonly Vector3 offset;

        protected float pathTime;

        protected const float epsilon = 0.03f;

        /// <summary>
        /// Creates a belt-like machine.
        /// </summary>
        /// <param name="machineNo">Machine number. Zero is automatic, and currently the only supported number.</param>
        /// <param name="firstTurnPoint">The point at which each model makes its first turn around the belt.</param>
        /// <param name="secondTurnPoint">The point at which each model makes the second turn around the belt.</param>
        /// <param name="offset">The offset that should be used (without this, models won't actually rotate 
        /// around the belt, they'll just rotate).</param>
        /// <param name="speed">The speed the belt will move at.</param>
        /// <param name="rotationAxis">The axis around which rotation is performed.</param>
        /// <param name="angle">The angle to rotate around rotationAxis.</param>
        /// <param name="zeroAxis">The direction of an angle of 0.</param>
        /// <param name="models">Models. Tubes are disallowed here.</param>
        public BeltMachine(int machineNo, int soundIndex, Vector3 firstTurnPoint, Vector3 secondTurnPoint, Vector3 offset, 
            float speed, Vector3 rotationAxis, params BaseModel[] models)
            : base(machineNo, soundIndex, models)
        {
            if(tubeList.Count > 0)
                throw new ArgumentException("BeltMachines can't contain tubes!");

            firstPoint = firstTurnPoint;
            secondPoint = secondTurnPoint;
            this.offset = offset;
            //this.angle = angle;

            foreach(BaseModel m in modelList)
            {
                //m.Ent.Position -= offset;
                //m.Ent.CollisionInformation.LocalPosition += offset;
                PerpendicularLineJoint j = new PerpendicularLineJoint(null, m.Ent, m.Ent.Position + offset, Vector3.Normalize(firstTurnPoint - secondTurnPoint),
                    m.Ent.Position + offset, rotationAxis);
                j.AngularMotor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
                j.AngularMotor.Settings.Servo.MaxCorrectiveVelocity = j.AngularMotor.Settings.Servo.BaseCorrectiveSpeed = speed / 1.1f;
                j.AngularMotor.IsActive = true;
                j.AngularMotor.Basis.SetWorldAxes(rotationAxis, Vector3.UnitZ);
                j.AngularMotor.TestAxis = Vector3.UnitZ;
                j.AngularMotor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
                j.AngularMotor.Settings.Servo.SpringSettings.DampingConstant /= 8;
                j.AngularMotor.Settings.Servo.Goal = MathHelper.ToRadians(-179.9999f);
                //j.AngularLimit.IsActive = true;
                //j.AngularLimit.Basis.SetWorldAxes(rotationAxis, Vector3.UnitZ);
                //j.AngularLimit.MaximumAngle = 0;
                //j.AngularLimit.MinimumAngle = -MathHelper.Pi;
                //j.AngularLimit.TestAxis = Vector3.UnitZ;

                j.LinearMotor.IsActive = true;
                j.LinearMotor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
                j.LinearMotor.Settings.Servo.BaseCorrectiveSpeed = j.LinearMotor.Settings.Servo.MaxCorrectiveVelocity = speed;
                j.LinearMotor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
                j.LinearMotor.Settings.Servo.SpringSettings.DampingConstant /= 9;
                j.LinearMotor.Settings.Servo.Goal = Vector3.Distance(firstTurnPoint, m.Ent.Position);
                j.LinearLimit.IsActive = true;
                j.LinearLimit.Maximum = Vector3.Distance(firstTurnPoint, m.Ent.Position);
                j.LinearLimit.Minimum = -Vector3.Distance(secondTurnPoint, m.Ent.Position);
                joints.Add(j);
            }
        }
        
        protected override void updateVelocities()
        {
            foreach(PerpendicularLineJoint j in joints)
            {
                if(Vector3.Distance(j.LinearMotor.ConnectionB.Position, firstPoint) < epsilon)
                    j.AngularMotor.Settings.Servo.Goal = MathHelper.ToRadians(-179.9999f);
                else if(Vector3.Distance(j.AngularMotor.TestAxis, -Vector3.UnitZ) < epsilon && Vector3.Distance(j.LinearMotor.ConnectionB.Position - offset * 2, firstPoint) < epsilon)
                    j.LinearMotor.Settings.Servo.Goal = j.LinearLimit.Minimum;
                else if(Vector3.Distance(j.LinearMotor.ConnectionB.Position - offset * 2, secondPoint) < epsilon)
                    j.AngularMotor.Settings.Servo.Goal = 0.001f;
                else if(Vector3.Distance(j.AngularMotor.TestAxis, Vector3.UnitZ) < epsilon && Vector3.Distance(j.LinearMotor.ConnectionB.Position, secondPoint) < epsilon)
                {
                    j.AngularMotor.Settings.Servo.Goal = 0;
                    j.LinearMotor.Settings.Servo.Goal = j.LinearLimit.Maximum;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(machineNoise == null)
            {
                fetchMachineNoise();
                playMachineNoise();
            }
            base.Update(gameTime);
        }

        public override void OnAdditionToSpace(ISpace s)
        {
            foreach(PerpendicularLineJoint j in joints)
                s.Add(j);
            base.OnAdditionToSpace(s);
        }

        public override void ResetMachine()
        {
            pathTime = 0;
            base.ResetMachine();
            foreach(PerpendicularLineJoint j in joints)
            {
                j.LinearMotor.Settings.Servo.Goal = j.LinearLimit.Maximum;
                j.AngularMotor.Settings.Servo.Goal = j.AngularLimit.MinimumAngle;
            }
        }

        public override void Rotate(Quaternion rotation)
        {
            // todo
            // motors, pieces, tubes, points
            throw new NotImplementedException("BeltMachines can't be rotated right now.");
        }
    }
}
