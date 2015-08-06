using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;
using BEPUphysics.Paths;
using Microsoft.Xna.Framework.Audio;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Accelerated_Delivery_Win
{
    public class RailMachine : OperationalMachine
    {
        public override bool IsActive
        {
            get { return rotating; }
        }

        protected bool rotating;
        protected bool atOriginForTranslation = true;
        protected bool atOriginForRotation = true;

        //protected QuaternionSlerpCurve curve;
        protected double pathTime = 0;
        protected float translationTime = 0;
        protected readonly float timeStep;
        protected Quaternion radiansToRotate;

        protected readonly Vector3 unitsToTranslate; // units to move
        protected readonly double lengthToPause; // length of time to pause at each end
        protected float slideTime = 0;
        protected float rotateTime;

        protected Cue railSlideSound;

        protected readonly List<WeldJoint> welds = new List<WeldJoint>();
        protected readonly RevoluteJoint rotateJoint;
        protected readonly PrismaticJoint baseJoint;
        protected readonly bool negativeAngle;
        protected readonly float angle;

        protected Vector3 originalAxis;

        protected const float epsilon = 0.01f;

        /// <summary>
        /// Makes a machine that automatically translates and user-rotates.
        /// </summary>
        /// <param name="machineNo">Machine number of the machine.</param>
        /// <param name="unitsToTranslate">The distance to translate from the origin.</param>
        /// <param name="pauseTime">The time in seconds to pause at each end.</param>
        /// <param name="centerOfRotation">The center of rotation based on the starting position.</param>
        /// <param name="rotateTime">Time to take to rotate.</param>
        /// <param name="degreesToRotate">The degrees on each axis to rotate.</param>
        /// <param name="models">The models that rotate.</param>
        /// <param name="angle">The angle to rotate.</param>
        /// <param name="rotationAxis">The axis to rotate on.</param>
        /// <param name="soundIndex">The sound to use for the machine.</param>
        /// <param name="stableModels">The models that don't rotate.</param>
        /// <param name="translationTime">The time to take to translate.</param>
        public RailMachine(int machineNo, int soundIndex, Vector3 unitsToTranslate, float pauseTime, float translationTime,
            Vector3 centerOfRotation, float rotateTime, Vector3 rotationAxis, float angle, Vector3 zeroAxis, BaseModel[] stableModels,
            params BaseModel[] models)
            : base(machineNo, soundIndex, models)
        {
            if(stableModels.Length == 0)
                throw new ArgumentException("There must be at least one stable model.");
            if(models.Length == 0)
                throw new ArgumentException("There must be a least rotational model.");

            this.unitsToTranslate = unitsToTranslate;
            this.translationTime = translationTime;
            this.rotateTime = rotateTime;
            this.angle = angle;
            lengthToPause = pauseTime;
            originalAxis = rotationAxis;

            if(angle < 0)
            {
                negativeAngle = true;
                angle = -angle;
            }

            baseJoint = new PrismaticJoint(null, stableModels[0].Ent, stableModels[0].Ent.Position, Vector3.Normalize(unitsToTranslate), stableModels[0].Ent.Position);
            baseJoint.Motor.IsActive = true;
            baseJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
            baseJoint.Motor.Settings.Servo.Goal = 0;
            baseJoint.Limit.Maximum = unitsToTranslate.Length();
            baseJoint.Limit.Minimum = 0;
            baseJoint.Limit.IsActive = true;
            baseJoint.Motor.Settings.Servo.BaseCorrectiveSpeed = unitsToTranslate.Length() / translationTime;
            baseJoint.Motor.Settings.Servo.MaxCorrectiveVelocity = unitsToTranslate.Length() / translationTime;
            baseJoint.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant /= 2;

            rotateJoint = new RevoluteJoint(stableModels[0].Ent, models[0].Ent, centerOfRotation, rotationAxis);
            rotateJoint.Motor.IsActive = true;
            rotateJoint.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            rotateJoint.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            rotateJoint.Motor.Settings.Servo.SpringSettings.DampingConstant /= 4;
            rotateJoint.Motor.Settings.Servo.Goal = negativeAngle ? angle : 0;
            rotateJoint.Motor.Settings.Servo.MaxCorrectiveVelocity = angle / rotateTime;
            rotateJoint.Motor.Settings.Servo.BaseCorrectiveSpeed = angle / rotateTime;
            rotateJoint.Motor.Basis.SetWorldAxes(rotationAxis, zeroAxis);
            rotateJoint.Motor.TestAxis = zeroAxis;
            rotateJoint.Limit.IsActive = true;
            rotateJoint.Limit.MinimumAngle = 0;
            rotateJoint.Limit.MaximumAngle = angle;
            rotateJoint.Limit.Basis.SetWorldAxes(rotationAxis, zeroAxis);
            rotateJoint.Limit.TestAxis = zeroAxis;

            for(int i = 0; i < stableModels.Length - 1; i++)
            {
                WeldJoint j = new WeldJoint(stableModels[0].Ent, stableModels[i + 1].Ent);
                j.IsActive = true;
                welds.Add(j);
            }
            for(int i = 0; i < models.Length - 1; i++)
            {
                WeldJoint j = new WeldJoint(models[0].Ent, models[i + 1].Ent);
                j.IsActive = true;
                welds.Add(j);
            }
            foreach(Tube t in tubeList)
                t.SetParent(models[0].Ent);

            modelList.Clear();
            modelList.AddRange(stableModels);
            modelList.AddRange(models);

            //Vector3 radians = new Vector3(MathHelper.ToRadians(degreesToRotate.X),
            //    MathHelper.ToRadians(degreesToRotate.Y), MathHelper.ToRadians(degreesToRotate.Z));
            //radiansToRotate = Quaternion.CreateFromYawPitchRoll(radians.Y,
            //    radians.X, radians.Z);

            //for(int i = 1; i < modelList.Count; i++)
            //{
            //    BaseModel m = modelList[i];
            //    m.Ent.Orientation = Quaternion.Identity;
            //    m.Ent.CollisionInformation.LocalPosition = m.Ent.Position - centerOfRotation;
            //    m.Ent.Position = centerOfRotation;
            //    m.Ent.Orientation = m.OriginalOrientation; // Resets orientation
            //}
            //curve = new QuaternionSlerpCurve();
            //curve.ControlPoints.Add(0, Quaternion.Identity);
            //curve.ControlPoints.Add(rotateTime, radiansToRotate);
            //curve.PostLoop = CurveEndpointBehavior.Mirror;
            //curve.PreLoop = CurveEndpointBehavior.Mirror;
            timeStep = rotateTime * 1000;
        }
        public override void ResetMachine()
        {
            if(railSlideSound != null && railSlideSound.IsPlaying)
                railSlideSound.Stop(AudioStopOptions.Immediate);
            atOriginForTranslation = atOriginForRotation = true;
            pathTime = slideTime = 0;
            rotating = false;
            base.ResetMachine();
        }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            if(!inputPaused)
#endif
                if(checkInputs() && !rotating)
                {
                    rotating = true;
                    rotateJoint.Motor.Settings.Servo.Goal = (negativeAngle ? !atOriginForRotation : atOriginForRotation) ? angle : 0;
                }

            if(rotating)
            {
                if(pathTime > rotateTime && Math.Abs(rotateJoint.Motor.RelativeVelocity) < epsilon)
                {
                    rotating = false;
                    atOriginForRotation = !atOriginForRotation;
                    pathTime = 0;
                }
            }

            if(slideTime > translationTime + lengthToPause || slideTime < lengthToPause)
            {
                if(slideTime > translationTime + lengthToPause * 2)
                {
                    atOriginForTranslation = !atOriginForTranslation;
                    slideTime = 0;
                }
                //SetTranslationToZero();
                if(railSlideSound != null && !railSlideSound.IsDisposed)
                {
                    railSlideSound.Stop(AudioStopOptions.AsAuthored);
                    railSlideSound = null;
                }
            }
            if((slideTime < translationTime + lengthToPause && slideTime > lengthToPause))
            {
                if(railSlideSound == null)
                    railSlideSound = MediaSystem.GetMachineNoise(soundIndex, MachineNumber);

                baseJoint.Motor.Settings.Servo.Goal = atOriginForTranslation ? unitsToTranslate.Length() : 0;
                //foreach(BaseModel m in modelList)
                //    CalculateVelocity(m);
                //foreach(Tube t in tubeList)
                //    CalculateVelocity(t);
            }
            base.Update(gameTime);
        }

        protected override void updateVelocities()
        {
            if(rotating)
                pathTime += Program.Game.Space.TimeStepSettings.TimeStepDuration;

            slideTime += Program.Game.Space.TimeStepSettings.TimeStepDuration;
        }

        public override void OnAdditionToSpace(ISpace s)
        {
            s.Add(baseJoint);
            s.Add(rotateJoint);
            foreach(WeldJoint j in welds)
                s.Add(j);
            base.OnAdditionToSpace(s);
        }

        public override void Rotate(Quaternion rotation)
        {
            // todo
            // rotate bucket, glass, motors, tubes
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);

            rotateJoint.Motor.SetupJointTransforms(Vector3.Transform(originalAxis, rotation));
            rotateJoint.AngularJoint.WorldFreeAxisA = rotateJoint.AngularJoint.WorldFreeAxisB = Vector3.Transform(originalAxis, rotation);
            
            baseJoint.Motor.Axis = Vector3.Transform(unitsToTranslate, rotation);
        }

        //private void SetTranslationToZero()
        //{
        //    foreach(BaseModel m in modelList)
        //        m.Ent.LinearVelocity = Vector3.Zero;
        //    foreach(Tube m in tubeList)
        //        m.Ent.LinearVelocity = Vector3.Zero;
        //}

        //protected void CalculateVelocity(BaseModel m)
        //{
        //    Vector3 distance;

        //    if(atOrigin)
        //        distance = unitsToTranslate;
        //    else
        //        distance = -unitsToTranslate;

        //    m.Ent.LinearVelocity = distance / translationTime;
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

        //protected void SetVelocityToZero()
        //{
        //    foreach(BaseModel m in modelList)
        //        m.Ent.AngularVelocity = Vector3.Zero;
        //    foreach(Tube m in tubeList)
        //        m.Ent.AngularVelocity = Vector3.Zero;
        //}
    }
}
