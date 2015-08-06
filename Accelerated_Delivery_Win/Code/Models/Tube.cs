using BEPUphysics;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BEPUphysics.Constraints;
using BEPUphysics.Materials;

namespace Accelerated_Delivery_Win
{
    public class Tube : BaseModel, ISpaceObject
    {
        protected Keys key;
        protected Buttons button;
        protected int machineNo;

        public bool Forward { get; private set; }
        public bool Rotating { get; private set; }
        //public bool RotationHandled { private get; set; }
        //public double Time { get; private set; }
        //new public Vector3 LocalOffset;
        public Quaternion Orientation
        {
            set
            {
                Ent.Orientation = OriginalOrientation = value;
                rotationDirection = Vector3.Transform(Vector3.UnitY, value);
                if(motor != null)
                {
                    motor.Motor.SetupJointTransforms(rotationDirection);
                    motor.AngularJoint.WorldFreeAxisA = motor.AngularJoint.WorldFreeAxisB = rotationDirection;
                }
            }
        }
        //protected readonly SingleEntityAngularMotor m1;
        //protected readonly SingleEntityLinearMotor m2;

        //protected QuaternionSlerpCurve curve;
        //protected float pathTime;
        //protected readonly bool x;

        protected RevoluteJoint motor;
        protected readonly SpringSettings spring;
        protected Vector3 rotationDirection;
        protected float velocity;

        /// <summary>
        /// This makes a tube. You're gonna have lots of these in a list in a level. 
        /// </summary>
        /// <param name="pos">Position to start with.</param>
        /// <param name="x">True if facing the x axis, false if facing y axis.</param>
        /// <param name="forward">This is actually the opposite of what it should be.</param>
        public Tube(Vector3 pos, bool x, bool forward)
            : base(Program.Game.LoadingScreen.loader.tubeModelX, false, null, pos)
        {
            Ent = new Cylinder(pos, 4, 0.5f);
            Ent.Material = tubeMaterial;
            Ent.Tag = this;
            Forward = forward;
            //this.x = x;
            //TargetTime = 1;
            if(!x)
                Ent.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2);
            OriginalOrientation = Ent.Orientation;

            //Ent.IsAffectedByGravity = false;
            Ent.CollisionInformation.Events.PairTouching += OnCollision;
            Ent.CollisionInformation.Events.CollisionEnded += OffCollision;
            Ent.CollisionInformation.CollisionRules.Group = tubeGroup;

            rotationDirection = x ? Vector3.UnitY : -Vector3.UnitX;
            //motor = new RevoluteJoint(null, Ent, ModelPosition, rotationDirection);
            //motor.Motor.Settings.Mode = MotorMode.VelocityMotor;
            //motor.Motor.Settings.VelocityMotor.Softness *= 9;
            //motor.Motor.IsActive = true;
            //motor.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            //motor.Motor.Settings.Servo.SpringSettings.DampingConstant /= 10;
            //motor.Motor.Settings.Servo.BaseCorrectiveSpeed = MathHelper.TwoPi * Program.Game.Space.TimeStepSettings.TimeStepDuration;

            //m1 = new SingleEntityAngularMotor(Ent);
            //m1.Settings.Mode = MotorMode.Servomechanism;
            //m1.Settings.Servo.SpringSettings.StiffnessConstant = 0;

            //AdditionalRotation = Quaternion.Identity;
            //rebuildCurve();

            //m2 = new SingleEntityLinearMotor(Ent, ModelPosition);
            //m2.Settings.Mode = MotorMode.Servomechanism;
            //m2.Settings.Servo.Goal = ModelPosition;
            //m2.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            //m2.Settings.Servo.SpringSettings.DampingConstant /= 4;
        }

        //private void rebuildCurve()
        //{
        //    curve = new QuaternionSlerpCurve();
        //    Vector3 axis;
        //    float angle;
        //    Quaternion q = OriginalOrientation;
        //    Toolbox.GetAxisAngleFromQuaternion(ref q, out axis, out angle); // test this plz
        //    curve.ControlPoints.Add(0.25f, x ? Quaternion.CreateFromAxisAngle(axis, angle + (Forward ? -MathHelper.PiOver2 : MathHelper.PiOver2)) : Quaternion.Multiply(OriginalOrientation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, Forward ? -MathHelper.PiOver2 : MathHelper.PiOver2)));
        //    curve.ControlPoints.Add(0.5f, x ? Quaternion.CreateFromAxisAngle(axis, angle + (Forward ? -MathHelper.Pi : MathHelper.Pi)) : Quaternion.Multiply(OriginalOrientation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, Forward ? -MathHelper.Pi : MathHelper.Pi)));
        //    curve.ControlPoints.Add(0.75f, x ? Quaternion.CreateFromAxisAngle(axis, angle + (Forward ? -MathHelper.PiOver2 + MathHelper.Pi : MathHelper.PiOver2 + MathHelper.Pi)) : Quaternion.Multiply(OriginalOrientation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, Forward ? -MathHelper.PiOver2 + MathHelper.Pi : MathHelper.PiOver2 + MathHelper.Pi)));
        //    curve.ControlPoints.Add(1, x ? Quaternion.CreateFromAxisAngle(axis, angle + (Forward ? -MathHelper.TwoPi : MathHelper.TwoPi)) : Quaternion.Multiply(OriginalOrientation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, Forward ? -MathHelper.TwoPi : MathHelper.TwoPi)));
        //}

        public void BecomeKeybasedTube(int machineNo)
        {
            //motor.Motor.Settings.Mode = MotorMode.VelocityMotor;
            //motor.Motor.Settings.VelocityMotor.GoalVelocity = 0;
            this.machineNo = machineNo;
            Program.Game.Manager.CurrentSaveWindowsOptions.GetMachineInputs(machineNo, out key, out button);
        }

        public void SetParent(Entity e)
        {
            Ent.BecomeDynamic(25);
            bool temp = false;
            if(motor != null && motor.Solver != null)
            {
                Program.Game.Space.Remove(motor);
                temp = true;
            }
            motor = new RevoluteJoint(e, Ent, Ent.Position, rotationDirection);
            motor.Motor.IsActive = true;
            motor.Motor.Settings.Mode = MotorMode.VelocityMotor;
            if(temp)
                Program.Game.Space.Add(motor);
        }

        public void SetKinematicParent(Entity e)
        {
            Ent.BecomeKinematic();
            if(motor != null && motor.Solver != null)
                Program.Game.Space.Remove(motor);
            motor = null;
        }

        public override void Update(GameTime gameTime)
        {
            if(key != Keys.None)
            {
                if(Input.KeyboardState.IsKeyDown(key) || Input.CurrentPad.IsButtonDown(button))
                    Rotating = true;
                else
                    Rotating = false;
            }

            base.Update(gameTime);
        }

        protected void updateVelocities()
        {
            if(Rotating)
            {
                if(Ent.IsDynamic && motor != null)
                    motor.Motor.Settings.VelocityMotor.GoalVelocity = MathHelper.TwoPi * (Forward ? -1 : 1);
                else
                    Ent.AngularVelocity = MathHelper.TwoPi * (Forward ? 1 : -1) * Ent.OrientationMatrix.Down;
            }
            else
            {
                if(Ent.IsDynamic && motor != null)
                    motor.Motor.Settings.VelocityMotor.GoalVelocity = 0;
                else
                    Ent.AngularVelocity = Vector3.Zero;
            }
            //if(Rotating && !RotationHandled)
            //    pathTime += Program.Game.Space.TimeStepSettings.TimeStepDuration;
            //else if(Rotating && RotationHandled)
            //    Time += Program.Game.Space.TimeStepSettings.TimeStepDuration;

            //m1.Settings.Servo.Goal = AdditionalRotation != Quaternion.Identity ? AdditionalRotation : curve.Evaluate(pathTime);
            //m2.Settings.Servo.Goal = TargetPosition != Vector3.Zero ? TargetPosition : Origin;
        }

        private void OnCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if(key == Keys.None)
                Rotating = true;
        }

        private void OffCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if(key == Keys.None)
                Rotating = false;
        }

        void ISpaceObject.OnAdditionToSpace(ISpace newSpace)
        {
            //newSpace.Add(m1);
            //newSpace.Add(m2);
            if(motor != null)
                newSpace.Add(motor);
            newSpace.Add(Ent);
            Program.Game.Space.DuringForcesUpdateables.Starting += updateVelocities;
        }

        void ISpaceObject.OnRemovalFromSpace(ISpace oldSpace)
        {
            //oldSpace.Remove(m1);
            //oldSpace.Remove(m2);
            if(motor != null)
                oldSpace.Remove(motor);
            oldSpace.Remove(Ent);
            Program.Game.Space.DuringForcesUpdateables.Starting -= updateVelocities;
        }

        public void Rotate(Quaternion rotation)
        {
            this.Orientation = rotation;
        }
    }
}
