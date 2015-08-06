using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Paths;
using BEPUphysics;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.BroadPhaseEntries;

namespace Accelerated_Delivery_Win
{
    public class Cannon : OperationalMachine
    {
        protected readonly Vector3 unitsToTranslate;
        protected readonly Vector3 rotationAxis;

        protected readonly PerpendicularLineJoint baseJoint;
        protected readonly Explosion explosion;
        protected readonly List<WeldJoint> joints = new List<WeldJoint>();
        //protected readonly float delay = 0;

        protected const float angle = MathHelper.PiOver4;
        protected const float epsilon = 0.15f;

        protected bool doingSomething;
        //protected readonly QuaternionSlerpCurve curve;
        protected float pathTime;
        protected bool exploded;

        protected FocusedBurstSystem particles;
        protected readonly Quaternion initialParticleOrientation;

        protected List<BroadPhaseEntry> boxes = new List<BroadPhaseEntry>();

        protected CannonPathFinder path;

        //public Cannon(int machineNo, int soundIndex, float delayTime, Vector3 unitsToTranslate, Vector3 rotationAxis, Vector3 explosionPos, float explosionMag, params BaseModel[] models)
        //    : this(machineNo, soundIndex, unitsToTranslate, rotationAxis, explosionPos, explosionMag, models)
        //{
        //    delay = delayTime;
        //}

        public Cannon(int machineNo, int soundIndex, Vector3 unitsToTranslate, Vector3 rotationAxis, CannonPathFinder path, Vector3 particlePos, /*float explosionMag,*/ params BaseModel[] models)
            :base(machineNo, soundIndex, models)
        {
            this.unitsToTranslate = unitsToTranslate;
            this.rotationAxis = rotationAxis;
            //explosion = new Explosion(explosionPos, explosionMag, 5, GameManager.Space);
            this.path = path;
            Vector3 explosionDirection = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, Quaternion.CreateFromAxisAngle(rotationAxis, angle)));
            if(float.IsInfinity(explosionDirection.X) || float.IsInfinity(explosionDirection.X) || float.IsInfinity(explosionDirection.Z))
                explosionDirection = Vector3.UnitZ;

            particles = new FocusedBurstSystem(Program.Game, explosionDirection, particlePos + 3 * explosionDirection);
            particles.AutoInitialize(RenderingDevice.GraphicsDevice, Program.Game.Content, RenderingDevice.SpriteBatch);
            RenderingDevice.GDM.DeviceCreated += delegate { particles.AutoInitialize(RenderingDevice.GraphicsDevice, Program.Game.Content, RenderingDevice.SpriteBatch); };
            RenderingDevice.Add(particles);

            initialParticleOrientation = particles.Emitter.OrientationData.Orientation;

            baseJoint = new PerpendicularLineJoint(null, modelList[0].Ent, modelList[0].Ent.Position, Vector3.Normalize(unitsToTranslate), 
                modelList[0].Ent.Position, rotationAxis);
            baseJoint.LinearMotor.IsActive = true;
            baseJoint.LinearMotor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            baseJoint.LinearMotor.Settings.Servo.Goal = 0;
            baseJoint.LinearMotor.Settings.Servo.BaseCorrectiveSpeed = baseJoint.LinearMotor.Settings.Servo.MaxCorrectiveVelocity = unitsToTranslate.Length() / 2;
            baseJoint.LinearMotor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            baseJoint.LinearMotor.Settings.Servo.SpringSettings.DampingConstant /= 15;
            baseJoint.LinearLimit.Maximum = unitsToTranslate.Length();
            baseJoint.LinearLimit.Minimum = 0;
            baseJoint.LinearLimit.IsActive = true;

            baseJoint.AngularMotor.IsActive = true;
            baseJoint.AngularMotor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            baseJoint.AngularMotor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            baseJoint.AngularMotor.Settings.Servo.SpringSettings.DampingConstant /= 12;
            baseJoint.AngularMotor.Settings.Servo.Goal = 0;
            baseJoint.AngularMotor.Settings.Servo.MaxCorrectiveVelocity = angle;
            baseJoint.AngularMotor.Settings.Servo.BaseCorrectiveSpeed = angle;
            baseJoint.AngularMotor.Basis.SetWorldAxes(rotationAxis, Vector3.UnitZ);
            baseJoint.AngularMotor.TestAxis = Vector3.UnitZ;
            baseJoint.AngularLimit.IsActive = true;
            baseJoint.AngularLimit.MinimumAngle = 0;
            baseJoint.AngularLimit.MaximumAngle = angle;
            baseJoint.AngularLimit.Basis.SetWorldAxes(rotationAxis, Vector3.UnitZ);
            baseJoint.AngularLimit.TestAxis = Vector3.UnitZ;

            if(modelList.Count > 1)
                foreach(BaseModel m in modelList)
                {
                    if(m == modelList[0])
                        continue;
                    WeldJoint j = new WeldJoint(modelList[0].Ent, m.Ent);
                    joints.Add(j);
                }

            //curve = new QuaternionSlerpCurve();
            //curve.ControlPoints.Add(0, Quaternion.Identity);
            //curve.ControlPoints.Add(2, Quaternion.Identity);
            //curve.ControlPoints.Add(3, Quaternion.CreateFromAxisAngle(rotationAxis, MathHelper.ToRadians(45)));
            //curve.ControlPoints.Add(4, Quaternion.CreateFromAxisAngle(rotationAxis, MathHelper.ToRadians(45)));
            //curve.PostLoop = CurveEndpointBehavior.Mirror;
            //curve.PreLoop = CurveEndpointBehavior.Mirror;
        }

        public override bool IsActive
        {
            get { return doingSomething; }
        }

        protected override void updateVelocities()
        {
            base.updateVelocities();
            if(doingSomething)
            {
                double pathTimeLastFrame = pathTime;
                pathTime += GameManager.Space.TimeStepSettings.TimeStepDuration;
                if(pathTimeLastFrame < 2 && pathTime > 2)
                {
                    baseJoint.AngularMotor.Settings.Servo.Goal = angle;
                    //ResetVelocity();
                }
                else if(pathTime > 3 && !exploded && Math.Abs(baseJoint.AngularMotor.RelativeVelocity) < epsilon)
                {
                    //if(TeleportPoint != Vector3.Zero)
                    //{
                    //    // This block of code does nothing except teleport boxes inside the cannon to a specific point if and only if there is one box inside.
                    //    (Space as Space).BroadPhase.QueryAccelerator.GetEntries(baseJoint.AngularJoint.ConnectionB.CollisionInformation.BoundingBox, boxes);
                    //    boxes = boxes.FindAll(v => { return v.Tag is Box; });
                    //    if(boxes.Count == 1)
                    //    {
                    //        (boxes[0].Tag as Box).Ent.Position = TeleportPoint;
                    //        (boxes[0].Tag as Box).Ent.Orientation = new Quaternion(0.09443433f, -0.9264939f, -0.3610983f, -0.04794991f);
                    //    }
                    //}
                    (Space as Space).BroadPhase.QueryAccelerator.GetEntries(baseJoint.AngularJoint.ConnectionB.CollisionInformation.BoundingBox, boxes);
                    boxes = boxes.FindAll(v => { return v.Tag is Box; });
                    if(boxes.Count == 1)
                        path.Add(boxes[0].Tag as Box);
                    else if(boxes.Count > 1)
                    {
                        Box[] lololol = new Box[boxes.Count];
                        int i = 0;
                        foreach(BroadPhaseEntry b in boxes)
                            lololol[i++] = b.Tag as Box;
                        path.Add(lololol);
                    }
                    boxes.Clear();
                    //explosion.Explode();
                    exploded = true;
                    particles.Activate();
                    stopMachineNoise();
                    // add a little kickback
                    //modelList[0].Ent.AngularMomentum = Vector3.Transform(Vector3.Cross(Vector3.Normalize(unitsToTranslate), rotationAxis), Quaternion.CreateFromAxisAngle(rotationAxis, angle)) * 15;
                    MediaSystem.PlaySoundEffect(SFXOptions.Explosion);//, modelList[0].ModelPosition);
                }
                else if(pathTimeLastFrame < 4 && pathTime > 4)
                {
                    baseJoint.AngularMotor.Settings.Servo.Goal = 0;
                    fetchMachineNoise();
                    if(machineNoise != null)
                        machineNoise.Play();
                }
                else if(pathTimeLastFrame < 5 && pathTime > 5)
                    baseJoint.LinearMotor.Settings.Servo.Goal = 0;
                else if(pathTime > 5 && Math.Abs(baseJoint.LinearMotor.RelativeVelocity) < epsilon)
                {
                    stopMachineNoise();
                    doingSomething = false;
                    pathTime = 0;
                    exploded = false;
                }
            }

            //if(doingSomething)
            //    foreach(BaseModel m in modelList)
            //        m.Ent.AngularVelocity = GetAngularVelocity(m.Ent.Orientation, curve.Evaluate(pathTime), 
            //            GameManager.Space.TimeStepSettings.TimeStepDuration);
            //else
            //    ResetVelocity();
        }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            if(!inputPaused)
#endif
                if(checkInputs() && !doingSomething)
                {
                    doingSomething = true;
                    baseJoint.LinearMotor.Settings.Servo.Goal = unitsToTranslate.Length();
                    fetchMachineNoise();
                    if(machineNoise != null)
                        machineNoise.Play();
                    //foreach(BaseModel m in modelList)
                    //    CalculateVelocity(m, false);
                    //foreach(Tube t in tubeList)
                    //    CalculateVelocity(t, false);
                }
            base.Update(gameTime); // if we have delay, the base will fetch a sound for us.
        }

        public override void OnAdditionToSpace(ISpace s)
        {
            s.Add(baseJoint);
            foreach(WeldJoint j in joints)
                s.Add(j);
            base.OnAdditionToSpace(s);
        }

        //protected void CalculateVelocity(BaseModel m, bool goBackward)
        //{
        //    Vector3 distance;

        //    if(!goBackward)
        //        distance = unitsToTranslate;
        //    else
        //        distance = -unitsToTranslate;

        //    m.Ent.LinearVelocity = distance / 2;
        //}

        //protected Vector3 GetAngularVelocity(Quaternion start, Quaternion end, float dt)
        //{
        //    //Compute the relative orientation R' between R and the target relative orientation.
        //    Quaternion errorOrientation;
        //    Quaternion.Conjugate(ref start, out errorOrientation);
        //    Quaternion.Multiply(ref end, ref errorOrientation, out errorOrientation);

        //    Vector3 axis;
        //    float angle;
        //    //Turn this into an axis-angle representation.
        //    Toolbox.GetAxisAngleFromQuaternion(ref errorOrientation, out axis, out angle);
        //    Vector3.Multiply(ref axis, angle / dt, out axis);
        //    return axis;
        //}

        //protected void ResetVelocity()
        //{
        //    foreach(BaseModel m in modelList)
        //        m.ReturnToInitialVelocities();
        //    foreach(Tube m in tubeList)
        //        m.ReturnToInitialVelocities();
        //}

        public override void ResetMachine()
        {
            pathTime = 0;
            baseJoint.LinearMotor.Settings.Servo.Goal = 0;
            baseJoint.AngularMotor.Settings.Servo.Goal = 0;
            doingSomething = false;
            exploded = false;
            particles.Reset();
            base.ResetMachine();
        }

        public override void Rotate(Quaternion rotation)
        {
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation; 
            particles.Emitter.OrientationData.Orientation = initialParticleOrientation * rotation;

            baseJoint.AngularMotor.SetupJointTransforms(Vector3.Transform(rotationAxis, rotation));
            baseJoint.AngularJoint.WorldFreeAxisA = baseJoint.AngularJoint.WorldFreeAxisB = Vector3.Transform(rotationAxis, rotation);

            baseJoint.LinearMotor.Axis = Vector3.Transform(unitsToTranslate, rotation);
        }
    }
}
