using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Constraints.SolverGroups;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Accelerated_Delivery_Win
{
    public class TruckMachine : OperationalMachine
    {
        public override bool IsActive { get { return rotating; } }

        protected readonly RevoluteJoint glassJoint1, glassJoint2;
        protected readonly RevoluteJoint wheelsJoint1, wheelsJoint2;
        protected readonly PrismaticJoint baseJoint;
        protected readonly List<WeldJoint> joints = new List<WeldJoint>();

        protected readonly Vector3 translation;
        protected readonly float translationTime;
        protected readonly float rotationTime;

        protected float translationTimeStep;
        protected float rotationTimeStep;

        protected bool translating;
        protected bool rotating;
        protected bool atOriginRotation = true;
        protected bool atOriginTranslation = true;

        protected float delayTime = 5f;
        protected const float epsilon = 0.01f;

        protected Vector3 originalGlassAxis;
        protected Vector3 originalWheelAxis;
        protected Vector3 unitsToTranslate;

        public float DelayTime { set { delayTime = value; } }

        public TruckMachine(int machineNo, int soundIndex, float translationTime, float rotationTime, float angle,
            Vector3 translationAxis, Vector3 glassRotationAxis, Vector3 wheelRotationAxis,
            Vector3 glassZeroAxis, Vector3 glassCenter1, Vector3 glassCenter2, Vector3 wheelsCenter1,
            Vector3 wheelsCenter2, BaseModel wheels1, BaseModel wheels2, BaseModel glass1,
            BaseModel glass2, params BaseModel[] models)
            : base(machineNo, soundIndex, models.Concat(new BaseModel[] { wheels1, wheels2, glass1, glass2 }).ToArray<BaseModel>())
        {
            translation = translationAxis;
            this.translationTime = translationTime;
            this.rotationTime = rotationTime;

            originalGlassAxis = glassRotationAxis;
            originalWheelAxis = wheelRotationAxis;
            unitsToTranslate = translationAxis;

            baseJoint = new PrismaticJoint(null, models[0].Ent, models[0].Ent.Position, translationAxis, models[0].Ent.Position);
            baseJoint.Motor.IsActive = true;
            baseJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
            baseJoint.Motor.Settings.Servo.Goal = 0;
            baseJoint.Limit.Maximum = translationAxis.Length();
            baseJoint.Limit.Minimum = 0;
            baseJoint.Limit.IsActive = true;
            baseJoint.Motor.Settings.Servo.BaseCorrectiveSpeed = translationAxis.Length() / translationTime;
            baseJoint.Motor.Settings.Servo.MaxCorrectiveVelocity = translationAxis.Length() / translationTime;
            baseJoint.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant /= 11;

            glassJoint1 = new RevoluteJoint(models[0].Ent, glass1.Ent, glassCenter1, glassRotationAxis);
            glassJoint1.Motor.IsActive = true;
            glassJoint1.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            glassJoint1.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            glassJoint1.Motor.Settings.Servo.SpringSettings.DampingConstant /= 12;
            glassJoint1.Motor.Settings.Servo.Goal = 0;
            glassJoint1.Motor.Settings.Servo.MaxCorrectiveVelocity = angle / rotationTime;
            glassJoint1.Motor.Settings.Servo.BaseCorrectiveSpeed = angle / rotationTime;
            glassJoint1.Motor.Basis.SetWorldAxes(glassRotationAxis, glassZeroAxis);
            glassJoint1.Motor.TestAxis = glassZeroAxis;
            glassJoint1.Limit.IsActive = true;
            glassJoint1.Limit.MinimumAngle = 0;
            glassJoint1.Limit.MaximumAngle = angle;
            glassJoint1.Limit.Basis.SetWorldAxes(glassRotationAxis, glassZeroAxis);
            glassJoint1.Limit.TestAxis = glassZeroAxis;

            glassJoint2 = new RevoluteJoint(models[0].Ent, glass2.Ent, glassCenter2, -glassRotationAxis);
            glassJoint2.Motor.IsActive = true;
            glassJoint2.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            glassJoint2.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            glassJoint2.Motor.Settings.Servo.SpringSettings.DampingConstant /= 12;
            glassJoint2.Motor.Settings.Servo.Goal = 0;
            glassJoint2.Motor.Settings.Servo.MaxCorrectiveVelocity = angle / rotationTime;
            glassJoint2.Motor.Settings.Servo.BaseCorrectiveSpeed = angle / rotationTime;
            glassJoint2.Motor.Basis.SetWorldAxes(-glassRotationAxis, glassZeroAxis);
            glassJoint2.Motor.TestAxis = glassZeroAxis;
            glassJoint2.Limit.IsActive = true;
            glassJoint2.Limit.MinimumAngle = 0;
            glassJoint2.Limit.MaximumAngle = angle;
            glassJoint2.Limit.Basis.SetWorldAxes(-glassRotationAxis, glassZeroAxis);
            glassJoint2.Limit.TestAxis = glassZeroAxis;

            wheelsJoint1 = new RevoluteJoint(models[0].Ent, wheels1.Ent, wheelsCenter1, wheelRotationAxis);
            wheelsJoint1.Motor.Settings.Mode = MotorMode.VelocityMotor;
            wheelsJoint1.Motor.Settings.VelocityMotor.GoalVelocity = 0f;
            wheelsJoint1.Motor.Settings.VelocityMotor.Softness = 1 / glassJoint1.Motor.Settings.Servo.SpringSettings.DampingConstant;
            wheelsJoint2 = new RevoluteJoint(models[0].Ent, wheels2.Ent, wheelsCenter2, wheelRotationAxis);
            wheelsJoint2.Motor.Settings.Mode = MotorMode.VelocityMotor;
            wheelsJoint2.Motor.Settings.VelocityMotor.GoalVelocity = 0f;
            wheelsJoint2.Motor.Settings.VelocityMotor.Softness = 1 / glassJoint2.Motor.Settings.Servo.SpringSettings.DampingConstant;

            if(models.Length > 1)
                foreach(BaseModel m in models)
                {
                    if(m == models[0])
                        continue;
                    WeldJoint j = new WeldJoint(models[0].Ent, m.Ent);
                    j.IsActive = true;
                    joints.Add(j);
                }
        }

        protected override void updateVelocities()
        {
            translationTimeStep += GameManager.Space.TimeStepSettings.TimeStepDuration;
            if(translationTimeStep > delayTime)
            {
                translating = true;
                baseJoint.Motor.Settings.Servo.Goal = atOriginTranslation ? baseJoint.Limit.Maximum : baseJoint.Limit.Minimum;
            }

            if(translating)
            {
                wheelsJoint1.Motor.IsActive = wheelsJoint2.Motor.IsActive = true;
                wheelsJoint1.Motor.Settings.VelocityMotor.GoalVelocity = wheelsJoint2.Motor.Settings.VelocityMotor.GoalVelocity =
                    atOriginTranslation ? 3 : -3;
                if(translationTimeStep >= delayTime + translationTime && Math.Abs(baseJoint.Motor.RelativeVelocity) < epsilon)
                {
                    translating = false;
                    atOriginTranslation = !atOriginTranslation;
                    translationTimeStep = 0;
                }
            }
            else
                wheelsJoint1.Motor.Settings.VelocityMotor.GoalVelocity = wheelsJoint2.Motor.Settings.VelocityMotor.GoalVelocity = 0;

            if(rotating)
            {
                rotationTimeStep += GameManager.Space.TimeStepSettings.TimeStepDuration;
                if(rotationTimeStep >= rotationTime && Math.Abs(glassJoint1.Motor.RelativeVelocity) <= epsilon)
                {
                    internalIsDone = true;
                    if(checkLinkedMachines())
                    {
                        rotationTimeStep = 0;
                        atOriginRotation = !atOriginRotation;
                        rotating = false;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(checkInputs() && !rotating)
            {
                rotating = true;
                internalIsDone = false;
                glassJoint1.Motor.Settings.Servo.Goal = glassJoint2.Motor.Settings.Servo.Goal = atOriginRotation ? glassJoint1.Limit.MaximumAngle : glassJoint1.Limit.MinimumAngle;
            }
            base.Update(gameTime);
        }

        public override void ResetMachine()
        {
            atOriginRotation = atOriginTranslation = true;
            rotationTimeStep = translationTimeStep = 0;
            glassJoint1.Motor.Settings.Servo.Goal = glassJoint2.Motor.Settings.Servo.Goal = 0;
            baseJoint.Motor.Settings.Servo.Goal = 0;
            wheelsJoint1.Motor.Settings.VelocityMotor.GoalVelocity = wheelsJoint2.Motor.Settings.VelocityMotor.GoalVelocity = 0;
            wheelsJoint1.Motor.IsActive = wheelsJoint2.Motor.IsActive = false;
            base.ResetMachine();
        }

        public override void OnAdditionToSpace(BEPUphysics.ISpace s)
        {
            foreach(WeldJoint j in joints)
                s.Add(j);
            s.Add(glassJoint1);
            s.Add(glassJoint2);
            s.Add(baseJoint);
            s.Add(wheelsJoint1);
            s.Add(wheelsJoint2);
            base.OnAdditionToSpace(s);
        }

        public override void Rotate(Quaternion rotation)
        {
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);

            glassJoint1.Motor.SetupJointTransforms(Vector3.Transform(originalGlassAxis, rotation));
            glassJoint1.AngularJoint.WorldFreeAxisA = glassJoint1.AngularJoint.WorldFreeAxisB = Vector3.Transform(originalGlassAxis, rotation);

            glassJoint2.Motor.SetupJointTransforms(Vector3.Transform(originalGlassAxis, rotation));
            glassJoint2.AngularJoint.WorldFreeAxisA = glassJoint2.AngularJoint.WorldFreeAxisB = Vector3.Transform(originalGlassAxis, rotation);

            wheelsJoint1.Motor.SetupJointTransforms(Vector3.Transform(originalWheelAxis, rotation));
            wheelsJoint1.AngularJoint.WorldFreeAxisA = wheelsJoint1.AngularJoint.WorldFreeAxisB = Vector3.Transform(originalWheelAxis, rotation);

            wheelsJoint2.Motor.SetupJointTransforms(Vector3.Transform(originalWheelAxis, rotation));
            wheelsJoint2.AngularJoint.WorldFreeAxisA = wheelsJoint2.AngularJoint.WorldFreeAxisB = Vector3.Transform(originalWheelAxis, rotation);

            baseJoint.Motor.Axis = Vector3.Transform(unitsToTranslate, rotation);
        }
    }
}
