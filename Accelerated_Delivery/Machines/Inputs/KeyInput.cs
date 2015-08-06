using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Accelerated_Delivery_Win
{
    public class KeyInput : IMachineInput
    {
        private int machineNo;
        private bool wasActive;

        /// <summary>
        /// Creates a new key input. This input fires when the designated button has been pressed.
        /// </summary>
        /// <param name="machineNumber"></param>
        public KeyInput(int machineNumber)
        {
            machineNo = machineNumber;
        }

        public bool JustActivated
        {
            get { return Input.CheckMachineJustPressed(machineNo); }
        }

        public bool JustDeactivated
        {
            get { return wasActive && !IsActive; }
        }

        public bool IsActive
        {
            get { return Input.CheckMachineJustPressed(machineNo) || Input.CheckMachineIsHeld(machineNo); }
        }

        public void UpdateInput()
        {
            wasActive = IsActive;
        }

        public void Reset() { }
    }
}
