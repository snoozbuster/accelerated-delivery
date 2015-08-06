using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Delivery_Win
{
    public interface IInputManager
    {
        event Action OnSaveChanged;
        event Action<int> OnSaveDeleted;

        int CurrentSaveNumber { get; }
        SaveData CurrentSave { get; }
        bool FullScreen { get; set; }

        void Save(bool uploadAsync);
    }
}
