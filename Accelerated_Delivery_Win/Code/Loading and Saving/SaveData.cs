using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    [Serializable]
    public class SaveData
    {
        public bool BeenCreated = false;

        public static int MaxStars { get { return 13 * 3 + 1; } } // level 11 only has one star

        public LevelDataDictionary LevelData;

        public LevelDataDictionary SideBLevelData;

        public AccomplishmentDictionary AchievementData;

        public bool GameCompleted;
        public bool SideBUnlocked;
        public bool SideBCompleted;

        public WindowsOptions Options;
        public XboxOptions Xoptions;

        [XmlIgnore]
        public TimeSpan Playtime;
        /// <summary>
        /// Devised as a workaround for XmlSerializer. Don't use this directly.
        /// </summary>
        public string SerializationPlaytime
        {
            get { return Playtime.ToString(); }
            set { Playtime = TimeSpan.Parse(value); }
        }

        public int CurrentLevel
        {
            get
            {
                for(int i = 1; i < 11; i++) // levels 1 - 10 (don't want level 11 or the demo levels in here)
                    if(!LevelData[i].Completed)
                        return i; // first level that isn't completed
                return 10; // I guess we'll give them level 10 if they click continue after they've beaten the game
            }
        }

        public int StarNumber
        {
            get
            {
                int have = 0;
                for(int i = 1; i < LevelData.typeList.Count; i++)
                    if(i == 11) // level 11
                        continue;
                    else
                        have += LevelData[i].TotalStars;
                if(LevelData[11].Completed) // level 11 has only one star
                    have++;
                return have;
            }
        }

        /// <summary>
        /// Do not use null. Just don't.
        /// </summary>
        /// <param name="levelDict"></param>
        /// <param name="sideBDict"></param>
        /// <param name="achDict"></param>
        /// <param name="complete"></param>
        /// <param name="sideBUnlocked"></param>
        /// <param name="sideBComplete"></param>
        public SaveData(LevelDataDictionary levelDict, LevelDataDictionary sideBDict, 
            AccomplishmentDictionary achList, bool complete, bool sideBUnlocked, bool sideBComplete,
            WindowsOptions winOpt, XboxOptions xOpt)
        {
            LevelData = levelDict;
            SideBLevelData = sideBDict;
            AchievementData = achList;
            GameCompleted = complete;
            SideBUnlocked = sideBUnlocked;
            SideBCompleted = sideBComplete;
            Options = winOpt;
            Xoptions = xOpt;
        }

        /// <summary>
        /// Only the serializer should call this.
        /// </summary>
        public SaveData()
        {
            SaveData EmptyData = SaveData.GetEmptyData(1);

            this.AchievementData = EmptyData.AchievementData;
            this.GameCompleted = false;
            this.LevelData = EmptyData.LevelData;
            this.SideBCompleted = false;
            this.SideBUnlocked = false;
            this.SideBLevelData = EmptyData.SideBLevelData;

            Options = new WindowsOptions();
            Xoptions = new XboxOptions();
        }

        public static SaveData GetEmptyData(int saveSlot)
        {
            return new SaveData(emptyNormalData, emptySideBData, getEmptyAchData(saveSlot), false, false, false,
                    new WindowsOptions(), new XboxOptions());
        }

        private static LevelDataDictionary emptyNormalData
        {
            get
            {
                LevelDataDictionary temp = new LevelDataDictionary();
                for(int i = 1; i < 15; i++)
                    temp.Add(i, LevelSelectData.Uncomplete);
                temp[1].Unlocked = true;
                temp[12].Unlocked = true;
                return temp;
            }
        }

        private static LevelDataDictionary emptySideBData
        {
            get
            {
                return emptyNormalData;
            }
        }

        private static AccomplishmentDictionary getEmptyAchData(int saveSlot)
        {
            return new AccomplishmentDictionary(saveSlot);
        }

        public static bool operator ==(SaveData lhs, SaveData rhs)
        {
            return lhs.AchievementData == rhs.AchievementData && lhs.GameCompleted == rhs.GameCompleted &&
                lhs.LevelData == rhs.LevelData && lhs.Options == rhs.Options && lhs.Playtime == rhs.Playtime &&
                lhs.SideBCompleted == rhs.SideBCompleted && lhs.SideBLevelData == rhs.SideBLevelData && lhs.SideBUnlocked == rhs.SideBUnlocked &&
                lhs.Xoptions == rhs.Xoptions;
        }

        public static bool operator !=(SaveData lhs, SaveData rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
