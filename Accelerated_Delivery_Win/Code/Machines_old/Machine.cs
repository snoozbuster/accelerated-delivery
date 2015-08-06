using System;
using BEPUphysics;
using Microsoft.Xna.Framework;

namespace Accelerated_Delivery_Win
{
    /// <summary>
    /// Represents a collection of immobile and mobile parts that would be considered a single "machine."
    /// </summary>
    public class Machine : ISpaceObject, IRenderableObject
    {
        /// <summary>
        /// List of models in the machine.
        /// </summary>
        protected readonly BaseModel[] models;
        /// <summary>
        /// List of mobile OperationalMachines.
        /// </summary>
        protected readonly OperationalMachine[] machines;
        
        /// <summary>
        /// Creates a new Machine. Machines contain any number of supporting immobile pieces and any number of machines that
        /// would be considered associated with one "machine."
        /// </summary>
        /// <param name="models">All of these are expected to be kinetic. This is enforced.</param>
        /// <param name="machines">All of these are expected to have the same MachineNumber. This is enforced.</param>
        public Machine(BaseModel[] models, params OperationalMachine[] machines)
        {
            this.models = models;
            this.machines = machines;

            foreach(BaseModel m in models)
                if(m.Ent.IsDynamic)
                    throw new ArgumentException("Can't have dynamic BaseModels in a Machine.");

            if(machines.Length == 0)
                throw new ArgumentException("Can't have no machines, use a BeltPiece for that.");

            int machineNumber = -1;
            foreach(OperationalMachine o in machines)
                if(machineNumber == -1)
                    machineNumber = o.MachineNumber;
                else if(machineNumber != o.MachineNumber)
                    throw new ArgumentException("Can't have OperationalMachines with different numbers in a Machine.");
        }

        /// <summary>
        /// Updates all models and OperationalMachines.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            foreach(BaseModel m in models)
                m.Update(gameTime);

            foreach(OperationalMachine m in machines)
                m.Update(gameTime);
        }

        /// <summary>
        /// Rotates a machine by the given Quaternion.
        /// </summary>
        /// <param name="rotation">Quaternion.Identity is the original orientation. Rotations are not cumulative.</param>
        public void Rotate(Quaternion rotation)
        {
            foreach(BaseModel m in models)
                m.Ent.Orientation = m.OriginalOrientation * rotation;

            foreach(OperationalMachine o in machines)
                o.Rotate(rotation);
        }

        public void OnAdditionToSpace(ISpace newSpace)
        {
            foreach(OperationalMachine o in machines)
                newSpace.Add(o);
            foreach(BaseModel m in models)
                newSpace.Add(m);
        }

        public void OnRemovalFromSpace(ISpace oldSpace)
        {
            foreach(OperationalMachine o in machines)
                oldSpace.Remove(o);
            foreach(BaseModel m in models)
                oldSpace.Remove(m);
        }

        public ISpace Space { get; set; }

        public object Tag { get; set; }

        public void AddToRenderer()
        {
            foreach(OperationalMachine o in machines)
                RenderingDevice.Add(o);
            foreach(BaseModel m in models)
                RenderingDevice.Add(m);
        }

        public void RemoveFromRenderer()
        {
            foreach(OperationalMachine o in machines)
                RenderingDevice.Remove(o);
            foreach(BaseModel m in models)
                RenderingDevice.Remove(m);
        }
    }
}
