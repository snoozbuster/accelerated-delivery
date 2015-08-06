using System;
using Microsoft.Xna.Framework;
using BEPUphysics.Paths;
using BEPUphysics;
using System.Collections.Generic;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework.Audio;

namespace Accelerated_Delivery_Win
{
    public class ClampedRotationMachine : OperationalMachine
    {
        public float DampingMultiplier { set { baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant *= value; } }

        protected readonly float timeStep;
        //protected readonly List<RotateMotor> motorList;
        protected readonly List<WeldJoint> joints;
        protected readonly RevoluteJoint baseJoint;
        //protected readonly Quaternion radiansToRotate;
        protected readonly float angle;
        protected readonly bool negativeAngle;

        protected bool rotating = false;
        protected float pathTime = 0;
        protected bool atOrigin = true;
        protected const float epsilon = 0.01f;
        public override bool IsActive { get { return rotating; } }

        protected Vector3 originalAxis;

        /// <summary>
        /// Creates a machine that rotates between two orientations, with or without an offset.
        /// </summary>
        /// <param name="machineNo">The machine number.</param>
        /// <param name="objects">A list of BaseModels and Tubes that the Machine contains. It will be assumed that the first is the base of the
        /// machine and all others are contained inside the machine.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <param name="rotationAxis">The axis of rotation.</param>
        /// <param name="center">The center of rotation.</param>
        /// <param name="rotateTime">The time to complete one rotation in seconds.</param>
        public ClampedRotationMachine(int machineNo, int soundIndex, float rotateTime, Vector3 rotationAxis, float angle,
            Vector3 center, Vector3 zeroAxis, params BaseModel[] objects)
            : base(machineNo, soundIndex, objects)
        {
            //if(angle < 0 || angle > MathHelper.Pi)
            //    throw new ArgumentOutOfRangeException("Angle must be between 0 and pi. Use a negative zeroAxis to simulate negative angles.");
            if(rotateTime <= 0)
                throw new ArgumentOutOfRangeException("rotateTime cannot be equal to or less than zero.");

            if(angle < 0)
            {
                negativeAngle = true;
                angle = -angle;
                atOrigin = false;
            }

            originalAxis = rotationAxis;
            
            //motorList = new List<RotateMotor>();
            joints = new List<WeldJoint>();
            baseJoint = new RevoluteJoint(null, modelList[0].Ent, center, rotationAxis);
            baseJoint.Motor.IsActive = true;
            baseJoint.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            baseJoint.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant /= 12;
            baseJoint.Motor.Settings.Servo.Goal = negativeAngle ? angle : 0;
            baseJoint.Motor.Settings.Servo.MaxCorrectiveVelocity = angle / rotateTime;
            baseJoint.Motor.Settings.Servo.BaseCorrectiveSpeed = angle / rotateTime;
            baseJoint.Motor.Basis.SetWorldAxes(rotationAxis, zeroAxis);
            baseJoint.Motor.TestAxis = zeroAxis;
            baseJoint.Limit.IsActive = true;
            baseJoint.Limit.MinimumAngle = 0;
            baseJoint.Limit.MaximumAngle = angle;
            baseJoint.Limit.Basis.SetWorldAxes(rotationAxis, zeroAxis);
            baseJoint.Limit.TestAxis = zeroAxis;

            timeStep = rotateTime;
            this.angle = angle;

            //foreach(Tube t in tubeList)
            //    t.LocalOffset = t.Ent.Position - center;

            if(modelList.Count > 1)
                foreach(BaseModel m in modelList)
                {
                    if(m == modelList[0]) // skip first element
                        continue;
                    //m.Ent.Orientation = Quaternion.Identity;
                    //m.Ent.CollisionInformation.LocalPosition = m.Ent.Position - center;
                    //m.Ent.Position = center;
                    //m.Ent.Orientation = m.OriginalOrientation; // Resets orientation
                    //motorList.Add(new RotateMotor(m, radiansToRotate) { Time = GameManager.Space.TimeStepSettings.TimeStepDuration });
                    WeldJoint j = new WeldJoint(baseJoint.Motor.ConnectionB, m.Ent);
                    j.IsActive = true;
                    //j.Limit.IsActive = true;
                    //j.Limit.MinimumAngle = 0;
                    joints.Add(j);
                }
            foreach(Tube t in tubeList)
                t.SetParent(baseJoint.Motor.ConnectionB);
                //motorList.Add(new RotateMotor(t, radiansToRotate, t.LocalOffset) { Time = GameManager.Space.TimeStepSettings.TimeStepDuration });
        }

        protected override void updateVelocities()
        {
            //Quaternion rotation;
            //Matrix newOrientation;
            //Vector3 scale, translation;
            base.updateVelocities();
            if(rotating)
            {
                //double pathTimeLastFrame = pathTime;
                pathTime += GameManager.Space.TimeStepSettings.TimeStepDuration;
                //foreach(RevoluteJoint j in joints)
                //    if(atOrigin)
                //        j.Motor.Settings.Servo.Goal += Math.Min(angle / timeStep * GameManager.Space.TimeStepSettings.TimeStepDuration, j.Limit.MaximumAngle);
                //    else
                //        j.Motor.Settings.Servo.Goal += Math.Max(-angle / timeStep * GameManager.Space.TimeStepSettings.TimeStepDuration, j.Limit.MinimumAngle);
                //if(atOrigin)
                //    baseJoint.Motor.Settings.Servo.Goal += Math.Min(angle / timeStep * GameManager.Space.TimeStepSettings.TimeStepDuration, baseJoint.Limit.MaximumAngle);
                //else
                //    baseJoint.Motor.Settings.Servo.Goal += Math.Max(-angle / timeStep * GameManager.Space.TimeStepSettings.TimeStepDuration, baseJoint.Limit.MinimumAngle);
            }
            //else
            //    resetVelocities();

            //foreach(Tube t in tubeList)
            //{
                //newOrientation = Matrix.Identity;

                //if(t.Forward)
                //    newOrientation *= Matrix.CreateRotationY(MathHelper.TwoPi * (float)t.Time);
                //else
                //    newOrientation *= Matrix.CreateRotationY(-MathHelper.TwoPi * (float)t.Time);

                //newOrientation *= Matrix.CreateFromQuaternion(t.OriginalOrientation);

                //newOrientation *= Matrix.CreateTranslation(t.LocalOffset);

                //newOrientation *= Matrix.CreateFromQuaternion(Quaternion.Slerp(atOrigin ? t.OriginalOrientation : radiansToRotate, atOrigin ? radiansToRotate : t.OriginalOrientation, pathTime / timeStep));

                //newOrientation *= Matrix.CreateTranslation(t.Origin - t.LocalOffset);
                //newOrientation.Decompose(out scale, out rotation, out translation);

                //t.Ent.AngularVelocity = GetAngularVelocity(t.Ent.Orientation, rotation, GameManager.Space.TimeStepSettings.TimeStepDuration);

                //t.Ent.LinearVelocity = (translation - t.Ent.Position) / (float)GameManager.Space.TimeStepSettings.TimeStepDuration;
            //}
        }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            if(!inputPaused)
#endif
            if((checkInputs() || MachineNumber == 0 || (pathTime > MachineNumber && MachineNumber > 10)) && !rotating)
            {
                rotating = true;
                internalIsDone = false;
                baseJoint.Motor.Settings.Servo.Goal = (negativeAngle ? !atOrigin : atOrigin) ? angle : 0;
                //foreach(RevoluteJoint j in joints)
                //    j.Motor.Settings.Servo.Goal = atOrigin? angle : 0;
                //foreach(RotateMotor r in motorList)
                //    r.SetDirection(atOrigin);
            }
            if(rotating)
            {
                if(pathTime > timeStep && Math.Abs(baseJoint.Motor.RelativeVelocity) <= epsilon)
                {
                    if(MachineNumber != 0)
                        internalIsDone = true;
                    if(checkLinkedMachines())
                    {
                        rotating = MachineNumber == 0;
                        atOrigin = !atOrigin;
                        pathTime = 0;
                        if(MachineNumber == 0)
                            baseJoint.Motor.Settings.Servo.Goal = (negativeAngle ? !atOrigin : atOrigin) ? angle : 0;
                    }
                }
            }
            base.Update(gameTime);
        }

        //protected void resetVelocities()
        //{
        //    foreach(BaseModel m in modelList)
        //        m.ReturnToInitialVelocities();
        //    foreach(Tube m in tubeList)
        //        m.ReturnToInitialVelocities();
        //}

        public override void OnAdditionToSpace(ISpace s)
        {
            //foreach(RotateMotor m in motorList)
            //    s.Add(m);
            foreach(WeldJoint j in joints)
                s.Add(j);
            s.Add(baseJoint);
            base.OnAdditionToSpace(s);
        }

        public override void ResetMachine()
        {
            pathTime = 0;
            rotating = false;
            //foreach(RevoluteJoint j in joints)
            //    j.Motor.Settings.Servo.Goal = 0;
            baseJoint.Motor.Settings.Servo.Goal = 0;
            //foreach(RotateMotor m in motorList)
            //    m.SetDirection(false);
            atOrigin = !negativeAngle;
            base.ResetMachine();
        }

        public override void Rotate(Quaternion rotation)
        {
            // todo
            // rotate pieces, motors
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);
            baseJoint.Motor.SetupJointTransforms(Vector3.Transform(originalAxis, rotation));
            baseJoint.AngularJoint.WorldFreeAxisA = baseJoint.AngularJoint.WorldFreeAxisB = Vector3.Transform(originalAxis, rotation);
        }
    }
}
