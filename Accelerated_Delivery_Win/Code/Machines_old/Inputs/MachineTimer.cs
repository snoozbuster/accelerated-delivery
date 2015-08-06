using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Delivery_Win
{
    public class MachineTimer : IMachineInput
    {
        private readonly float fireTime;
        private float currentTime;
        private float timeLastFrame;

        /// <summary>
        /// Creates a new machine timer. This timer ticks for as long as you are updating it.
        /// </summary>
        /// <param name="seconds"></param>
        public MachineTimer(float seconds)
        {
            fireTime = seconds;
        }

        public bool JustActivated
        {
            get { return timeLastFrame < fireTime && currentTime > fireTime; }
        }

        public bool JustDeactivated
        {
            get { return false; }
        }

        public bool IsActive
        {
            get { return currentTime > fireTime; }
        }

        /// <summary>
        /// This should be called in a method hooked to the Space's timekeeping.
        /// </summary>
        public void UpdateInput()
        {
            timeLastFrame = currentTime;
            currentTime += Program.Game.Space.TimeStepSettings.TimeStepDuration;
        }

        /// <summary>
        /// Resets the timer to zero.
        /// </summary>
        public void Reset() { currentTime = 0; }
    }
}
