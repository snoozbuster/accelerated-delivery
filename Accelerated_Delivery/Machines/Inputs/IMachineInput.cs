using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Delivery_Win
{
    public interface IMachineInput
    {
        /// <summary>
        /// Indicates whether or not the input has just been activated.
        /// </summary>
        bool JustActivated { get; }

        /// <summary>
        /// Indicates whether or not the input has just been deactivated.
        /// </summary>
        bool JustDeactivated { get; }

        /// <summary>
        /// Indicates whether or not the input is currently active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Updates the machine input.
        /// </summary>
        void UpdateInput();

        /// <summary>
        /// Resets the input.
        /// </summary>
        void Reset();
    }
}
