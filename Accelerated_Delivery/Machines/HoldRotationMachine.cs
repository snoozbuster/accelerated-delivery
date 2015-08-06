using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using Microsoft.Xna.Framework.Audio;

namespace Accelerated_Delivery_Win
{
    public class HoldRotationMachine : OperationalMachine
    {
        //protected readonly List<RotateMotor> motorList;
        protected bool rotating;
        public override bool IsActive { get { return rotating; } }

        /// <summary>
        /// Creates a machine that rotates while the button is held.
        /// </summary>
        /// <param name="machineNo">Machine number.</param>
        /// <param name="degreesToRotate">Value means rotate, negative value means rotate backward.</param>
        /// <param name="models">Models.</param>
        public HoldRotationMachine(int machineNo, int soundIndex, Vector3 degreesToRotate, params Tube[] models)
            : base(machineNo, soundIndex, models)
        {
            //motorList = new List<RotateMotor>();
            //Vector3 temp = Vector3.Zero;
            //if(degreesToRotate.X != 0)
            //    temp += Vector3.UnitX * (degreesToRotate.X < 0 ? -6 : 6);
            //if(degreesToRotate.Y != 0)
            //    temp += Vector3.UnitY * (degreesToRotate.Y < 0 ? -6 : 6);
            //if(degreesToRotate.Z != 0)
            //    temp += Vector3.UnitZ * (degreesToRotate.Z < 0 ? -6 : 6);
            //foreach(BaseModel m in modelList)
            //    motorList.Add(new RotateMotor(m, temp));
            //foreach(Tube t in tubeList)
            //    motorList.Add(new RotateMotor(t, temp));
            foreach(Tube t in tubeList)
                t.BecomeKeybasedTube(machineNo);
        }

        protected override void updateVelocities()
        { }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            if(!inputPaused)
#endif
            if(checkInputs())
            {
                rotating = true;
                //foreach(RotateMotor m in motorList)
                //    m.IsActive = true;
            }
            else if(!Input.CheckMachineIsHeld(MachineNumber) && rotating)
            {
                rotating = false;
                //foreach(RotateMotor m in motorList)
                //    m.IsActive = false;
                //resetVelocities();
                stopMachineNoise();
            }

            base.Update(gameTime);
        }

        protected override bool checkInputs()
        {
            return base.checkInputs();
        }

        protected override void fetchMachineNoise()
        { }

        protected override void playMachineNoise()
        { }

        //protected void resetVelocities()
        //{
            //foreach(BaseModel m in modelList)
            //    m.ReturnToInitialVelocities();
        //    foreach(Tube m in tubeList)
        //        m.ReturnToInitialVelocities();
        //}

        public override void ResetMachine()
        {
            base.ResetMachine();
        }

        public override void Rotate(Quaternion rotation)
        {
            // just rotate pieces and maybe motors
            // this machine isn't actually used anywhere that I know of except as a dummy
            // not to mention it literally does nothing
            // not even play sounds
            // what is it even doing here
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);
        }
    }
}
