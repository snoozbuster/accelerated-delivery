using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Paths;
using BEPUphysics;
using Microsoft.Xna.Framework.Audio;
using BEPUphysics.Constraints.SolverGroups;

namespace Accelerated_Delivery_Win
{
    public class KeyframeMachine : OperationalMachine, IMachineInput
    {
        /// <summary>
        /// Use this in a translation field if you want to add a delay.
        /// </summary>
        public const float DelayConstant = 0.001f;

        /// <summary>
        /// Contains all the keyframes for both forward and backward. 
        /// </summary>
        private KeyframeList keyframes;
        //private readonly List<PerpendicularLineJoint> joints = new List<PerpendicularLineJoint>();
        //private readonly List<WeldJoint> welds = new List<WeldJoint>();
        //private int index = 0;
        //private bool halfway = false;
        private QuaternionSlerpCurve orientationCurve;
        private double pathTime;
        private bool doingSomething;
        public override bool IsActive
        {
            get { return doingSomething; }
        }

        private const float epsilon = 0.15f;

        public bool NoStop { get; set; }

        public new bool JustDeactivated { get { if(!justActivatedSet) return base.JustDeactivated; justActivatedSet = false; return justActivated; } private set { justActivatedSet = true; justActivated = value; } }
        private bool justActivatedSet;
        private bool justActivated;

        /// <summary>
        /// Creates a KeyframeMachine that uses keyframes and other fun stuff to move everything.
        /// </summary>
        /// <param name="machineNo">The machine number.</param>
        /// <param name="keyframes">The keyframes to use. </param>
        /// <param name="models">The models to use. Warning: If doing translation OR rotation, multiple models are fine. Probably.
        /// However, if you're doing both, I'd recommend only using one, and just making more KeyframeMachines.
        /// If you take this approach, both KeyframeMachines NEED to have the same number of Keyframes and all the times added up
        /// need to be equal.</param>
        public KeyframeMachine(int machineNo, int soundIndex, List<Keyframe> keyframes, bool completeList, params BaseModel[] models)
            : base(machineNo, soundIndex, models)
        {
            if(keyframes.Count == 0)
                throw new ArgumentException("Parameter \"keyframes\" must contain some keyframes!");

            this.keyframes = new KeyframeList(keyframes, completeList);
            NoStop = false;

            orientationCurve = new QuaternionSlerpCurve();
            if(machineNo != 0)
            {
                orientationCurve.PostLoop = CurveEndpointBehavior.Mirror;
                orientationCurve.PreLoop = CurveEndpointBehavior.Mirror;
            }
            else
            {
                orientationCurve.PostLoop = CurveEndpointBehavior.Wrap;
                orientationCurve.PreLoop = CurveEndpointBehavior.Wrap;
            }
            orientationCurve.ControlPoints.Add(0, Quaternion.Identity);

            Quaternion lastQ = keyframes[0].Rotation;
            float time = keyframes[0].Time;
            orientationCurve.ControlPoints.Add(time, lastQ);

            for(int i = 1; i < keyframes.Count; i++)
            {
                lastQ = Quaternion.Concatenate(lastQ, keyframes[i].Rotation);
                time += keyframes[i].Time;
                orientationCurve.ControlPoints.Add(time, lastQ);
            }

            foreach(BaseModel m in modelList)
                m.Ent.BecomeKinematic();
            foreach(Tube t in tubeList)
                t.SetKinematicParent(modelList[0].Ent);
        }

        protected override void updateVelocities()
        {
            if(internalIsDone && checkLinkedMachines())
                Deactivate();
            if(doingSomething)
                pathTime += GameManager.Space.TimeStepSettings.TimeStepDuration;
            if(pathTime > keyframes.TimeToDate)// && (Math.Abs(joints[index].AngularMotor.RelativeVelocity) <= epsilon && Math.Abs(joints[index].LinearMotor.RelativeVelocity) <= epsilon))
            {
                foreach(BaseModel m in modelList)
                    m.ReturnToInitialVelocities();
                foreach(Tube m in tubeList)
                    m.ReturnToInitialVelocities();

                bool done = keyframes.Next();
                float length = keyframes.CurrentKeyframe.Translation.Length();
                if(length == DelayConstant || length == DelayConstant * Math.Sqrt(2) || length == DelayConstant * Math.Sqrt(3))
                    stopMachineNoise();
                if(keyframes.CurrentKeyframe.Rotation == Quaternion.Identity && keyframes.CurrentKeyframe.Translation == Vector3.Zero)
                {
                    JustDeactivated = true;
                    stopMachineNoise();
                }

                if(done && (!NoStop || (NoStop && keyframes.JustReset)) && MachineNumber != 0) // Then we're out of keyframes.
                {
                    internalIsDone = true;
                    if(checkLinkedMachines())
                        Deactivate();
                }
                else if(done && NoStop || !done || MachineNumber == 0)
                {
                    //if(index + 1 == joints.Count)
                    //    halfway = true;

                    //joints[index].AngularMotor.IsActive = joints[index].LinearMotor.IsActive = false;
                    //index += halfway ? -1 : 1;

                    //joints[index].AngularMotor.IsActive = joints[index].LinearMotor.IsActive = true;
                    //if(halfway)
                    ////{
                    //    joints[index].LinearMotor.Settings.Servo.Goal = joints[index].AngularMotor.Settings.Servo.Goal = 0;
                    ////}
                    //else
                    //{
                    //    joints[index].LinearMotor.Settings.Servo.Goal = joints[index].LinearLimit.Maximum;
                    //    joints[index].AngularMotor.Settings.Servo.Goal = joints[index].AngularLimit.MaximumAngle;
                    //}

                    foreach(BaseModel m in modelList)
                        CalculateLinearVelocity(m);
                    foreach(Tube t in tubeList)
                        CalculateLinearVelocity(t);

                    if(MachineNumber == 0 && keyframes.JustReset)
                        pathTime = 0;
                    if(machineNoise == null || machineNoise.IsDisposed || machineNoise.State == SoundState.Stopped)
                    {
                        fetchMachineNoise();
                        playMachineNoise();
                    }
                }
            }
            if(keyframes.CurrentKeyframe.Rotation != Quaternion.Identity && doingSomething)
            {
                foreach(BaseModel m in modelList)
                    m.Ent.AngularVelocity = GetAngularVelocity(m.Ent.Orientation, orientationCurve.Evaluate(pathTime),
                        GameManager.Space.TimeStepSettings.TimeStepDuration, m) + m.InitialAngularVelocity;
                foreach(Tube t in tubeList)
                    t.Ent.AngularVelocity = GetAngularVelocity(t.Ent.Orientation, orientationCurve.Evaluate(pathTime),
                            GameManager.Space.TimeStepSettings.TimeStepDuration, t);
            }
        }

        private void Deactivate()
        {
            doingSomething = false;
            if(keyframes.JustReset)
            {
                //joints[index].AngularMotor.IsActive = joints[index].LinearMotor.IsActive = false;
                //joints[0].AngularMotor.IsActive = joints[0].LinearMotor.IsActive = true;
                pathTime = 0;
                //index = 0;
                //halfway = false;
            }
            if(NoStop)
                modelList[0].ModelPosition = modelList[0].Origin;
            else
                stopMachineNoise();
        }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            if(!inputPaused)
#endif
            if((checkInputs() || (inputs.Count == 0 && MachineNumber == 0)) && !doingSomething)
            {
                doingSomething = true;
                internalIsDone = false;
                //joints[index].LinearMotor.Settings.Servo.Goal = joints[index].LinearLimit.Maximum;
                //joints[index].AngularMotor.Settings.Servo.Goal = joints[index].AngularLimit.MaximumAngle;
                foreach(BaseModel m in modelList)
                    CalculateLinearVelocity(m);
                foreach(Tube t in tubeList)
                    CalculateLinearVelocity(t);
            }
            base.Update(gameTime);
        }

        protected Vector3 GetAngularVelocity(Quaternion start, Quaternion end, float dt, BaseModel m)
        {
            //Compute the relative orientation R' between R and the target relative orientation.
            Quaternion errorOrientation;
            Quaternion.Conjugate(ref start, out errorOrientation);
            Quaternion.Multiply(ref end, ref errorOrientation, out errorOrientation);

            Vector3 axis;
            float angle;
            //Turn this into an axis-angle representation.
            Toolbox.GetAxisAngleFromQuaternion(ref errorOrientation, out axis, out angle);
            Vector3.Multiply(ref axis, angle / dt, out axis);
            return axis;
        }

        protected void CalculateLinearVelocity(BaseModel m)
        {
            Keyframe k = keyframes.CurrentKeyframe;

            m.Ent.LinearVelocity = k.Translation / k.Time;
        }

        public override void ResetMachine()
        {
            //joints[index].AngularMotor.IsActive = joints[index].LinearMotor.IsActive = false;
            pathTime = 0;
            //index = 0;
            //joints[0].LinearMotor.IsActive = joints[0].AngularMotor.IsActive = true;
            //halfway = false;
            keyframes.Reset();
            doingSomething = false;
            base.ResetMachine();
        }

        public override void Rotate(Quaternion rotation)
        {
            keyframes.Rotate(rotation);
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);
        }

        private class KeyframeList
        {
            private List<Keyframe> internalList;
            private List<float> consecutiveTimes;
            private int currentIndex = 0;
            /// <summary>
            /// The current keyframe.
            /// </summary>
            public Keyframe CurrentKeyframe { get { return internalList[currentIndex]; } }
            /// <summary>
            /// The last keyframe to have been used. This may refer to the last element in the list when the current element is the first.
            /// </summary>
            public Keyframe LastKeyframe { get; private set; }
            /// <summary>
            /// The time this keyframe is done.
            /// </summary>
            public float TimeToDate { get { return consecutiveTimes[currentIndex]; } }
            /// <summary>
            /// Returns true if we are going forward, false if backward.
            /// </summary>
            public bool AtOrigin { get { return currentIndex < internalList.Count / 2; } }
            /// <summary>
            ///  Indicates if the last call to Next() wrapped the list to the first element.
            /// </summary>
            public bool JustReset { get; private set; }

            /// <summary>
            /// Creates a new KeyframeList.
            /// </summary>
            /// <param name="list">The list of keyframes from forward to backward. The keyframes back to the origin will be generated
            /// automatically if wrap is false.</param>
            /// <param name="wrap">If true, the keyframe list will be treated complete as it is. If false, keyframes from the ending
            /// point back to the origin will be generated automatically.</param>
            public KeyframeList(List<Keyframe> list, bool wrap)
            {
                internalList = new List<Keyframe>();
                consecutiveTimes = new List<float>();
                float consecutiveTime = 0;

                foreach(Keyframe k in list)
                {
                    internalList.Add(k);
                    consecutiveTime += k.Time;
                    consecutiveTimes.Add(consecutiveTime);
                }

                if(!wrap)
                    for(int i = internalList.Count - 1; i >= 0; i--)
                    {
                        internalList.Add(new Keyframe(-internalList[i].Translation, Quaternion.Conjugate(internalList[i].Rotation),
                            internalList[i].Time));
                        consecutiveTime += internalList[i].Time;
                        consecutiveTimes.Add(consecutiveTime);
                    }

                LastKeyframe = Keyframe.Zero;
            }

            /// <summary>
            /// Moves the list to the next Keyframe.
            /// </summary>
            /// <returns>True if the Keyframe advanced to is either at the origin or at the limit.</returns>
            public bool Next()
            {
                JustReset = false;
                LastKeyframe = CurrentKeyframe;
                currentIndex++;
                if(currentIndex == internalList.Count)
                {
                    currentIndex = 0;
                    JustReset = true;
                    return true;
                }
                else if(currentIndex == internalList.Count / 2)
                    return true;

                return false;
            }

            /// <summary>
            /// Resets the list.
            /// </summary>
            public void Reset()
            {
                currentIndex = 0;
                LastKeyframe = Keyframe.Zero;
                JustReset = false;
            }

            /// <summary>
            /// Rotates all the keyframes by the specified value. Not consecutive.
            /// </summary>
            /// <param name="rotation">The amount to rotate by.</param>
            public void Rotate(Quaternion rotation)
            {
                foreach(Keyframe k in internalList)
                    k.Rotate(rotation);
            }

            public static explicit operator KeyframeList(List<Keyframe> lhs)
            {
                return new KeyframeList(lhs, false);
            }
        }
    }

    public class Keyframe
    {
        public static Keyframe Zero { get { return new Keyframe(Vector3.Zero, Quaternion.Identity, 0); } }

        public Vector3 Translation { get; private set; }
        public Quaternion Rotation { get; private set; }
        public float Time { get; private set; }

        private Quaternion originalOrientation;

        /// <summary>
        /// Creates a Keyframe for use in a KeyframeMachine.
        /// </summary>
        /// <param name="translation">The distance to translate during the time in timeInSeconds.</param>
        /// <param name="rotation">The amount to rotate during the time in timeInSeconds.</param>
        /// <param name="timeInSeconds">The amount of time to use during rotation and translation.</param>
        public Keyframe(Vector3 translation, Quaternion rotation, float timeInSeconds)
        {
            Translation = translation;
            Rotation = rotation;
            Time = timeInSeconds;
            originalOrientation = rotation;
        }

        public Keyframe(Vector3 translation, float timeInSeconds)
            : this(translation, Quaternion.Identity, timeInSeconds)
        { }

        public Keyframe(Quaternion rotation, float timeInSeconds) 
            : this(Vector3.Zero, rotation, timeInSeconds) 
        { }

        public Keyframe(float timeInSeconds)
            : this(Vector3.Zero, Quaternion.Identity, timeInSeconds)
        { }

        /// <summary>
        /// Rotates a Keyframe. All rotations use original configuration as a base.
        /// </summary>
        /// <param name="rotation">Amount to rotate by.</param>
        public void Rotate(Quaternion rotation)
        {
            Rotation = originalOrientation * rotation;
        }
    }
}
