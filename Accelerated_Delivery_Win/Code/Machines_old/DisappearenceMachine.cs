using BEPUphysics;
using Microsoft.Xna.Framework;

namespace Accelerated_Delivery_Win
{
    public class DisappearenceMachine : OperationalMachine
    {
        public override bool IsActive { get { return visibilityLastFrame != visible; } }

        protected bool visible;
        protected bool visibilityLastFrame;
        protected readonly bool initalVisibility;
        //protected readonly List<WeldJoint> joints = new List<WeldJoint>();

        /// <summary>
        /// Creates a machine that becomes visible/invisible when you press button/when timer fires.
        /// </summary>
        /// <param name="machineNo">Machine number. Values outside 1-10 will be treated as ms until toggle.</param>
        /// <param name="initiallyVisible">If the machine is initially visible.</param>
        /// <param name="models">You should know by now.</param>
        public DisappearenceMachine(int machineNo, bool initiallyVisible, params BaseModel[] models)
            : base(machineNo, -1, models)
        {
            visible = initalVisibility = initiallyVisible;

            //foreach(BaseModel m in modelList)
            //{
            //    WeldJoint j = new WeldJoint(null, m.Ent);
            //    j.IsActive = true;
            //    j.BallSocketJoint.LocalOffsetB = Vector3.Zero;
            //    joints.Add(j);
            //}

            if(machineNo > 10)
            {
                inputs.Clear();
                inputs.Add(new MachineTimer(machineNo / 1000f));
            }
        }

        public override void OnAdditionToSpace(ISpace newSpace)
        {
            if(visible)
                changeVisibility();
            Program.Game.Space.DuringForcesUpdateables.Starting += updateVelocities;
        }

        public override void AddToRenderer() { } // this is overridden to control inital state of machine
        public override void Update(GameTime gameTime) { } // stops game from trying to play sounds for one frame

        protected override void updateVelocities()
        {
            base.updateVelocities();
            visibilityLastFrame = visible;
            if(checkInputs())
            {
                visible = !visible;
                changeVisibility();
            }
        }

        public override void RemoveFromRenderer()
        {
            if(visible)
            {
                visible = !visible;
                changeVisibility();
            }
            Program.Game.Space.DuringForcesUpdateables.Starting -= updateVelocities;
        }

        protected void changeVisibility()
        {
            if(visible)
            {
                foreach(BaseModel m in modelList)
                {
                    RenderingDevice.Add(m);
                    Program.Game.Space.Add(m);
                }
                //foreach(WeldJoint j in joints)
                //    Program.Game.Space.Add(j);
            }
            else
            {
                foreach(BaseModel m in modelList)
                {
                    RenderingDevice.Remove(m);
                    Program.Game.Space.Remove(m);
                }
                //foreach(WeldJoint j in joints)
                //    Program.Game.Space.Remove(j);
            }
        }

        public override void ResetMachine()
        {
            if(visible != initalVisibility)
            {
                if(!visible)
                {
                    foreach(BaseModel m in modelList)
                    {
                        RenderingDevice.Add(m);
                        Program.Game.Space.Add(m);
                    }
                    //foreach(WeldJoint j in joints)
                    //    Program.Game.Space.Add(j);
                }
                else
                {
                    foreach(BaseModel m in modelList)
                    {
                        RenderingDevice.Remove(m);
                        Program.Game.Space.Remove(m);
                    }
                    //foreach(WeldJoint j in joints)
                    //    Program.Game.Space.Remove(j);
                }
            }
            visible = initalVisibility;

            base.ResetMachine();
        }

        public override void Rotate(Quaternion rotation)
        {
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation;
            foreach(Tube t in tubeList)
                t.Rotate(rotation);
        }
    }
}
