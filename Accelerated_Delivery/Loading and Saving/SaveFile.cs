using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    [Serializable]
    public class SaveFile
    {
        public SaveData save1;
        public SaveData save2;
        public SaveData save3;
        public int currentSaveGame;
        public bool Fullscreen;
    }
}
