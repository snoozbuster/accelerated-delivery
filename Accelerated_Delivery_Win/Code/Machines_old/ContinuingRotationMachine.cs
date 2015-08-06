using System;
using System.Collections.Generic;
using BEPUphysics.Paths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using BEPUphysics.Constraints.SolverGroups;

namespace Accelerated_Delivery_Win
{
    public class ContinuingRotationMachine : OperationalMachine
    {
        //protected readonly QuaternionSlerpCurve curve;
        //protected readonly List<RotateMotor> motorList;
        protected readonly bool automatic;
        protected float pathTime;
        protected bool rotating;
        public override bool IsActive { get { return rotating; } }

        protected readonly RevoluteJoint baseJoint;
        protected readonly List<WeldJoint> joints = new List<WeldJoint>();
        protected readonly float angle;
        protected readonly float inactiveTime;
        protected readonly int rotationsToOrigin;
        protected readonly float rotationTime;

        protected Vector3 originalAxis;

        protected int rotationNumber = 1;

        public float DampingMultiplier { set { baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant *= value; } }

        /// <summary>
        /// Creates a machine that rotates, pauses, and rotates again.
        /// </summary>
        /// <param name="machineNo">Machine number, zero for auto.</param>
        /// <param name="center">Center of the machine.</param>
        /// <param name="radiansPerRotation">Angle to rotate on each rotation.</param>
        /// <param name="rotationAxis">Axis to rotate on.</param>
        /// <param name="rotationsToOrigin">Number of rotations until the machine is back to normal.</param>
        /// <param name="deactivationTime">Time to stay still for (if automatic).</param>
        /// <param name="rotateTime">Time to rotate for.</param>
        /// <param name="models">Models.</param>
        public ContinuingRotationMachine(int machineNo, int soundIndex, Vector3 rotationAxis, Vector3 zeroAxis, float radiansPerRotation, 
            int rotationsToOrigin, float deactivationTime, float rotateTime, Vector3 center, params BaseModel[] models)
            : base(machineNo, soundIndex, models)
        {
            if(machineNo == 0)
                automatic = true;

            //Quaternion radiansToRotateQ = Quaternion.CreateFromAxisAngle(rotationAxis, radiansPerRotation);
            this.rotationsToOrigin = rotationsToOrigin;
            rotationTime = rotateTime;
            angle = radiansPerRotation;
            inactiveTime = deactivationTime;
            originalAxis = rotationAxis;

            baseJoint = new RevoluteJoint(null, modelList[0].Ent, center, rotationAxis);
            baseJoint.Motor.IsActive = true;
            baseJoint.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            baseJoint.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant /= 12;
            baseJoint.Motor.Settings.Servo.Goal = 0;
            baseJoint.Motor.Settings.Servo.MaxCorrectiveVelocity = radiansPerRotation / rotateTime;
            baseJoint.Motor.Settings.Servo.BaseCorrectiveSpeed = radiansPerRotation / rotateTime;
            baseJoint.Motor.Basis.SetWorldAxes(rotationAxis, zeroAxis);
            baseJoint.Motor.TestAxis = zeroAxis;

            if(modelList.Count > 1)
                foreach(BaseModel m in modelList)
                {
                    if(m == modelList[0])
                        continue;
                    WeldJoint j = new WeldJoint(modelList[0].Ent, m.Ent);
                    joints.Add(j);
                }
            foreach(Tube t in tubeList)
                t.SetParent(modelList[0].Ent);
        }

        public override void Update(GameTime gameTime)
        {
            if((checkInputs() || automatic) && !rotating)
            {
                rotating = true;
                baseJoint.Motor.Settings.Servo.Goal = angle * rotationNumber;
            }
            if(rotating)
            {
                if(pathTime >= (inactiveTime + rotationTime) * rotationsToOrigin)
                {
                    pathTime = 0;
                    if(!automatic)
                        rotating = false;
                    baseJoint.Motor.Settings.Servo.Goal = 0;
                    rotationNumber = 1;
                }
                else if(pathTime > inactiveTime * rotationNumber + rotationTime * (rotationNumber - 1) && automatic)
                {
                    baseJoint.Motor.Settings.Servo.Goal = angle * rotationNumber;
                    rotationNumber++;
                }
            }
            else
                stopMachineNoise();
            base.Update(gameTime);
        }

        protected override void updateVelocities()
        {
            //Matrix newOrientation;
            //Vector3 scale, translation;
            //Quaternion rotation;
            base.updateVelocities();
            if(rotating)
            {
                pathTime += Program.Game.Space.TimeStepSettings.TimeStepDuration;
                //else if(pathTime > rotationTime * rotationNumber && !automatic)
                //{
                //    rotationNumber++;
                //    rotating = false;
                //}
            }
        }

        public override void OnAdditionToSpace(BEPUphysics.ISpace s)
        {
            s.Add(baseJoint);
            foreach(WeldJoint j in joints)
                s.Add(j);
            base.OnAdditionToSpace(s);
        }

        public override void ResetMachine()
        {
            pathTime = 0;
            rotationNumber = 1;
            baseJoint.Motor.Settings.Servo.Goal = 0;
            base.ResetMachine();
        }

        public override void Rotate(Quaternion rotation)
        {
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);
            baseJoint.Motor.SetupJointTransforms(Vector3.Transform(originalAxis, rotation));
            baseJoint.AngularJoint.WorldFreeAxisA = baseJoint.AngularJoint.WorldFreeAxisB = Vector3.Transform(originalAxis, rotation);
        }
    }
}
