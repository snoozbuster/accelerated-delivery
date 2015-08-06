using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BEPUphysics;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;
using Microsoft.Xna.Framework.Audio;
using BEPUphysics.Materials;
using BEPUphysics.CollisionRuleManagement;

namespace Accelerated_Delivery_Win
{
    public enum ActivationType
    {
        JustActivated,
        IsActive,
        JustDeactivated
    }

    public abstract class OperationalMachine : IMachineInput, ISpaceObject, IRenderableObject
    {
        public bool IsManual { get { return inputs.Count == 1 && inputs[0] is KeyInput; } }

        public int MachineNumber { get; private set; }
        protected List<Tube> tubeList;
        protected List<BaseModel> modelList;
        protected GameTime savedTime;
        public abstract bool IsActive { get; }
        protected bool wasActive;

        protected readonly List<OperationalMachine> linkedMachines = new List<OperationalMachine>();
        protected bool internalIsDone;

        protected MachineSoundInstance machineNoise;

        protected AudioEmitter emitter;

        protected int soundIndex;
        protected readonly List<IMachineInput> inputs = new List<IMachineInput>();
        protected ActivationType activation = ActivationType.JustActivated;

        //protected Vector3 positionLastFrame;

        //protected CollisionGroup group = new CollisionGroup();

        /// <summary>
        /// Sets a custom friction for all the models in this machine. Be careful; may not work as intended.
        /// </summary>
        public Material Friction { set { foreach(BaseModel b in modelList) b.Ent.Material = value; } }

        /// <summary>
        /// Creates a basic, functionless Machine.
        /// </summary>
        /// <param name="machineNo">The machine number. For any numbers outside of 1-10, it is treated as the number of
        /// milliseconds until this machine automatically activates.</param>
        /// <param name="machines">An array of machines this machine holds. Null is acceptable.</param>
        /// <param name="models">A list of BaseModels and Tubes that the Machine contains.</param>
        protected OperationalMachine(int machineNo, int soundIndex, params BaseModel[] models)
        {
            tubeList = new List<Tube>();
            modelList = new List<BaseModel>();
            this.soundIndex = soundIndex;
            //emitter = new AudioEmitter();
            //emitter.DopplerScale = 2;
            //emitter.Up = Vector3.UnitZ;
            //emitter.Forward = Vector3.UnitY;

            //CollisionGroup.DefineCollisionRule(group, group, CollisionRule.NoBroadPhase);
            //CollisionGroup.DefineCollisionRule(group, BaseModel.KinematicGroup, CollisionRule.NoBroadPhase);
            //CollisionGroup.DefineCollisionRule(group, BaseModel.TubeGroup, CollisionRule.NoBroadPhase);

            foreach(BaseModel m in models)
            {
                if(m is Tube)
                    tubeList.Add((Tube)m);
                else if(m is BaseModel)
                {
                    modelList.Add(m);
                    //m.Ent.CollisionInformation.CollisionRules.Group = group;
                }
                else
                    throw new ArgumentException("A machine can't be made out of " + m.ToString() + ".");
            }

            MachineNumber = machineNo;
            if(MachineNumber != 0)
                inputs.Add(new KeyInput(MachineNumber));

            //if(modelList.Count > 0)
            //    emitter.Position = modelList[0].ModelPosition;
            //else if(tubeList.Count > 0)
            //    emitter.Position = tubeList[0].ModelPosition;
        }

        /// <summary>
        /// Sets the input for the machine. This removes all previous inputs.
        /// </summary>
        /// <param name="inputs">Inputs to set.</param>
        public void SetInputs(params IMachineInput[] inputs)
        {
            this.inputs.Clear();
            this.inputs.AddRange(inputs);
        }

        public void SetActivationType(ActivationType type)
        {
            activation = type;
        }

        public virtual void Update(GameTime gameTime)
        {
            //if(modelList.Count > 0)
            //    positionLastFrame = modelList[0].ModelPosition;

            savedTime = gameTime;
            foreach(BaseModel m in modelList)
                m.Update(gameTime);
            foreach(Tube t in tubeList)
                t.Update(gameTime);

            //if(modelList.Count > 0)
            //{
            //    emitter.Position = modelList[0].ModelPosition;
            //    emitter.Velocity = emitter.Position - positionLastFrame;
            //}
            //else
            //    emitter.Position = Vector3.Zero;
            //if(machineNoise != null && machineNoise.IsPlaying)
            //    machineNoise.Apply3D(RenderingDevice.Camera.Ears, emitter);

            if(checkInputs() && !wasActive)
            {
                fetchMachineNoise();
                playMachineNoise();
            }
            else if(!IsActive && wasActive)
                stopMachineNoise();
            wasActive = IsActive;
        }

#if DEBUG
        public bool inputPaused = false;
#endif

        public List<BaseModel> GetGlassModels()
        {
            List<BaseModel> output = new List<BaseModel>();

            foreach(BaseModel m in modelList)
                if(m.RenderAsGlass) output.Add(m);

            return output;
        }

        protected virtual void OnTimerFired(object sender, EventArgs e)
        { }

        /// <summary>
        /// Updates machine inputs.
        /// </summary>
        protected virtual void updateVelocities()
        {
            foreach(IMachineInput i in inputs)
                i.UpdateInput();
        }

        /// <summary>
        /// Checks to see if all the inputs are active. Can be overridden to provide alternate behavior.
        /// </summary>
        /// <returns></returns>
        protected virtual bool checkInputs()
        {
            if(GameManager.CurrentLevel != null && GameManager.CurrentLevel.Ending)
                return false;

#if DEBUG
            if(inputPaused)
                return false;
#endif

            foreach(IMachineInput i in inputs)
            {
                if(activation == ActivationType.JustActivated)
                    if(!i.JustActivated)
                        return false;
                if(activation == ActivationType.JustDeactivated)
                    if(!i.JustDeactivated)
                        return false;
                if(activation == ActivationType.IsActive)
                    if(!i.IsActive)
                        return false;
            }
            return true;
        }

        // It is wise to override this.
        /// <summary>
        /// Resets the machine for restarting a level. Also resets all MachineTimers.
        /// </summary>
        public virtual void ResetMachine()
        {
            //positionLastFrame = Vector3.Zero;
            internalIsDone = true;
            foreach(BaseModel m in modelList)
                m.Reset();
            foreach(Tube t in tubeList)
                t.Reset();
            foreach(IMachineInput t in inputs)
                t.Reset();
            stopMachineNoise();
        }

        public virtual void OnAdditionToSpace(ISpace newSpace)
        {
            foreach(BaseModel m in modelList)
                if(m.Ent.Space == null)
                    newSpace.Add(m);
            foreach(Tube t in tubeList)
                newSpace.Add(t);
            foreach(Button b in inputs.FindAll(v => { return v is Button; }))
                if(b.Model.Ent.Space == null)
                    newSpace.Add(b);
            (newSpace as Space).DuringForcesUpdateables.Starting += updateVelocities;
        }

        public virtual void OnRemovalFromSpace(ISpace s)
        {
            foreach(BaseModel m in modelList)
                if(m.Ent.Space == s)
                    s.Remove(m);
            foreach(Tube t in tubeList)
                if(t.Ent.Space == s)
                    s.Remove(t);
            foreach(Button b in inputs.FindAll(v => { return v is Button; }))
            {
                RenderingDevice.Remove(b.Model);
                //if(b.Model.Ent.Space == null)
                //    s.Remove(b);
            }
            (s as Space).DuringForcesUpdateables.Starting -= updateVelocities;
        }

        public virtual void AddToRenderer()
        {
            foreach(BaseModel m in modelList)
                if(!m.RenderAsGlass)
                    RenderingDevice.Add(m);
            foreach(Tube t in tubeList)
                RenderingDevice.Add(t);
            foreach(Button b in inputs.FindAll(v => { return v is Button; }))
                RenderingDevice.Add(b.Model);
        }

        public virtual void RemoveFromRenderer()
        {
            foreach(BaseModel m in modelList)
                if(!m.RenderAsGlass)
                    RenderingDevice.Remove(m);
            foreach(Tube t in tubeList)
                RenderingDevice.Remove(t);
        }

        /// <summary>
        /// Links the machines so that they become done at the same time.
        /// </summary>
        /// <param name="machines"></param>
        public static void LinkMachines(params OperationalMachine[] machines)
        {
            foreach(OperationalMachine m in machines)
                foreach(OperationalMachine ma in machines)
                    if(m != ma)
                        m.linkedMachines.Add(ma);
        }

        protected bool checkLinkedMachines()
        {
            if(linkedMachines.Count == 0)
                return true;
            foreach(OperationalMachine m in linkedMachines)
                if(!m.internalIsDone)
                    return false;
            return internalIsDone; // we have to check ourselves too
        }

        protected virtual void playMachineNoise()
        {
            if(machineNoise != null)// && machineNoise.IsPrepared)
                machineNoise.Play();
        }

        protected void stopMachineNoise()
        {
            if(machineNoise != null && !machineNoise.IsDisposed && machineNoise.State == SoundState.Playing)
                machineNoise.Stop(false);
        }

        protected virtual void fetchMachineNoise()
        {
            stopMachineNoise();
            machineNoise = MediaSystem.GetMachineNoise(soundIndex, MachineNumber);
            //if(machineNoise != null)
            //    machineNoise.Apply3D(RenderingDevice.Camera.Ears, emitter);
        }

        public BaseModel GetBase()
        {
            if(modelList.Count > 0 && modelList[0] != null)
                return modelList[0];
            if(tubeList.Count > 0 && tubeList[0] != null)
                return tubeList[0];
            return null;
        }

        /// <summary>
        /// True when the machine stops moving.
        /// </summary>
        public bool JustActivated
        {
            get { return wasActive && !IsActive; }
        }

        /// <summary>
        /// True when the machine starts moving.
        /// </summary>
        public bool JustDeactivated
        {
            get { return IsActive && !wasActive; }
        }

        public void UpdateInput() { } // don't need to do any input-related updating
        public void Reset() { } // don't need to do any input-related resetting

        /// <summary>
        /// Rotates the machine by the given Quaternion.
        /// </summary>
        /// <param name="rotation">Identity is original rotation. Rotations are not cumulative.</param>
        public abstract void Rotate(Quaternion rotation);

        public ISpace Space { get; set; }

        public object Tag { get; set; }
    }
}
