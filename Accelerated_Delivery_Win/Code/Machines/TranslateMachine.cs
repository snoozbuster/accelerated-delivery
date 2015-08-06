using System;
using System.Collections.Generic;
using BEPUphysics.Constraints.SingleEntity;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics;
using BEPUphysics.Constraints.SolverGroups;

namespace Accelerated_Delivery_Win
{
    public class TranslateMachine : OperationalMachine
    {
        public float DampingMultiplier { set { baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant *= value; } }

        public bool Kinetic
        {
            set
            {
                kinetic = true;
                foreach(BaseModel m in modelList)
                    m.Ent.BecomeKinematic();
                foreach(Tube t in tubeList)
                    t.SetKinematicParent(modelList[0].Ent);
                foreach(WeldJoint j in joints)
                    if(j.Solver != null)
                        GameManager.Space.Remove(j);
                if(baseJoint.Solver != null)
                    GameManager.Space.Remove(baseJoint);
            }
        }
        protected bool kinetic;

        protected const float epsilon = 0.16f;

        protected Vector3 targetPosition;
        protected bool translating = false;
        protected float timeStep;
        protected bool atOrigin = true;
        protected bool automatic = false;
        protected float pathTime = 0;
        protected float preDelayTime;
        protected float postDelayTime;

        protected readonly PrismaticJoint baseJoint;
        protected readonly List<WeldJoint> joints;

        public override bool IsActive
        {
            get { return translating; }
        }

        public TranslateMachine(int machineNo, int soundIndex, Vector3 targetPos, float timestep, bool automatic,
            float preDelay, float postDelay, params BaseModel[] models)
            : this(machineNo, soundIndex, targetPos, timestep, automatic, models)
        {
            preDelayTime = preDelay;
            postDelayTime = postDelay;
        }

        /// <summary>
        /// Creates a new, cool, advanced Model. For Accelerated Delivery. Otherwise I get
        /// lots of silly repetative variables. Note that all rendering options are hardcoded.
        /// </summary>
        /// <param name="machineNo">The machine number. For any numbers outside of 1-10, it is treated as the number of
        /// milliseconds until this machine automatically activates.</param>
        /// <param name="machines">An array of machines this machine holds. Null is acceptable.</param>
        /// <param name="models">A list of BaseModels and Tubes that the Machine contains.</param>
        /// <param name="targetPos">The amount of X, Y, and Z to translate.</param>
        /// <param name="timestep">The amount of time to use to translate.</param>
        public TranslateMachine(int machineNo, int soundIndex, Vector3 targetPos, float timestep, bool automatic, params BaseModel[] models)
            : base(machineNo, soundIndex, models)
        {
            joints = new List<WeldJoint>();
            timeStep = timestep;
            targetPosition = targetPos;
            this.automatic = automatic;
            baseJoint = new PrismaticJoint(null, modelList[0].Ent, modelList[0].Ent.Position, Vector3.Normalize(targetPos), modelList[0].Ent.Position);
            baseJoint.Motor.IsActive = true;
            baseJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
            baseJoint.Motor.Settings.Servo.Goal = 0;
            baseJoint.Limit.Maximum = targetPos.Length();
            baseJoint.Limit.Minimum = 0;
            baseJoint.Limit.IsActive = true;
            baseJoint.Motor.Settings.Servo.BaseCorrectiveSpeed = targetPos.Length() / timestep;
            baseJoint.Motor.Settings.Servo.MaxCorrectiveVelocity = targetPos.Length() / timestep;
            baseJoint.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            baseJoint.Motor.Settings.Servo.SpringSettings.DampingConstant /= 15;

            foreach(BaseModel m in modelList)
            {
                if(m == modelList[0])
                    continue;
                WeldJoint j = new WeldJoint(modelList[0].Ent, m.Ent);
                joints.Add(j);
            }
            foreach(Tube t in tubeList)
                t.SetParent(modelList[0].Ent);

            if(automatic)
            {
                inputs.Clear();
                inputs.Add(new MachineTimer(machineNo / 1000f));
            }
        }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            if(!inputPaused)
#endif
                if((checkInputs() || automatic) && !translating)
                {
                    translating = true;
                    internalIsDone = false;
                    if(atOrigin ? preDelayTime == 0 : postDelayTime == 0)
                    {
                        if(!kinetic)
                            baseJoint.Motor.Settings.Servo.Goal = atOrigin ? targetPosition.Length() : 0;
                        else
                        {
                            foreach(BaseModel m in modelList)
                                CalculateVelocity(m);
                            foreach(BaseModel m in tubeList)
                                CalculateVelocity(m);
                        }
                    }
                }
            if(translating)
            {
                if(!kinetic)
                {
                    if(atOrigin)
                        if(pathTime >= preDelayTime && preDelayTime != 0 && pathTime <= preDelayTime + GameManager.Space.TimeStepSettings.TimeStepDuration)
                            baseJoint.Motor.Settings.Servo.Goal = targetPosition.Length();
                    if(!atOrigin)
                        if(pathTime >= postDelayTime && postDelayTime != 0 && pathTime <= postDelayTime + GameManager.Space.TimeStepSettings.TimeStepDuration)
                            baseJoint.Motor.Settings.Servo.Goal = 0;
                }
                if((pathTime >= timeStep + preDelayTime + postDelayTime && Math.Abs(baseJoint.Motor.RelativeVelocity) <= epsilon) ||
                    (pathTime >= timeStep && kinetic))
                {
                    internalIsDone = true;
                    if(checkLinkedMachines())
                        stop();
                }
            }
            base.Update(gameTime);
        }

        protected override void updateVelocities()
        {
            base.updateVelocities();
            //if(translating)
            //    pathTime += GameManager.Space.TimeStepSettings.TimeStepDuration;
            //if(pathTime > timeStep)
            if(translating)
            {
                pathTime += GameManager.Space.TimeStepSettings.TimeStepDuration;
            }
        }

        protected void CalculateVelocity(BaseModel m)
        {
            Vector3 distance;

            if(atOrigin)
                distance = targetPosition;
            else
                distance = -targetPosition;

            m.Ent.LinearVelocity = distance / timeStep;
        }

        protected void stop()
        {
            atOrigin = !atOrigin;
            pathTime = 0;
            if(!automatic)
            {
                translating = false;
                if(kinetic)
                {
                    foreach(BaseModel m in modelList)
                        m.ReturnToInitialVelocities();
                    foreach(Tube t in tubeList)
                        t.ReturnToInitialVelocities();
                }
            }
            else
            {
                if(!kinetic)
                    baseJoint.Motor.Settings.Servo.Goal = atOrigin ? targetPosition.Length() : 0;
                else
                {
                    foreach(BaseModel m in modelList)
                        m.Ent.LinearVelocity = -m.Ent.LinearVelocity;
                    foreach(Tube t in tubeList)
                        t.Ent.LinearVelocity = -t.Ent.LinearVelocity;
                }

            }
        }

        public override void ResetMachine()
        {
            translating = false;
            atOrigin = true;
            pathTime = 0;
            if(!kinetic)
                baseJoint.Motor.Settings.Servo.Goal = 0;
            base.ResetMachine();
        }

        public override void OnAdditionToSpace(ISpace s)
        {
            foreach(WeldJoint j in joints)
                s.Add(j);
            s.Add(baseJoint);
            base.OnAdditionToSpace(s);
        }

        public override void Rotate(Quaternion rotation)
        {
            baseJoint.PointOnLineJoint.LineDirection = Vector3.Transform(targetPosition, rotation);
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);
        }
    }
}
