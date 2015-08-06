using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Accelerated_Delivery_Win
{
    [Serializable]
    public class LevelDataDictionary
    {
        public List<int> keyList;
        public List<LevelSelectData> typeList;

        public LevelDataDictionary()
        {
            keyList = new List<int>();
            typeList = new List<LevelSelectData>();
        }

        /// <summary>
        /// Adds a key to the dictionary.
        /// </summary>
        /// <param name="key">Key to add. Throws an ArgumentException if the key already exists.</param>
        /// <param name="type">The object to link to the key.</param>
        public void Add(int key, LevelSelectData type)
        {
            if(keyList.Contains(key))
                throw new ArgumentException("This key already exists.");

            keyList.Add(key);
            typeList.Add(type);
        }

        /// <summary>
        /// Replaces the object at the specified key with a new object.
        /// </summary>
        /// <param name="key">The key to be relinked.</param>
        /// <param name="newData">The new object.</param>
        public void Replace(int key, LevelSelectData newData)
        {
            int oldScore, oldLost;
            TimeSpan oldTime;
            oldScore = this[key].Score;
            oldLost = this[key].BoxesLost;
            oldTime = this[key].Time;
            bool completed = this[key].Completed;
            // current score <= new score
            if(oldScore <= newData.Score || oldScore == 0)
            {
                this[key].Score = newData.Score;
                this[key].ScoreStarNumber = newData.ScoreStarNumber;
            }
            // current number lost >= new number lost
            if(oldLost >= newData.BoxesLost || !completed)
            {
                this[key].BoxesLost = newData.BoxesLost;
                this[key].BoxStarNumber = newData.BoxStarNumber;
            }
            // current time >= new time
            if(oldTime >= newData.Time || oldTime == TimeSpan.Zero)
            {
                this[key].Time = newData.Time;
                this[key].TimeStarNumber = newData.TimeStarNumber;
            }
            this[key].Completed = this[key].Completed || newData.Completed;
            this[key].Unlocked = this[key].Unlocked || newData.Unlocked;
        }

        [System.Xml.Serialization.XmlIgnore]
        public LevelSelectData this[int key]
        {
            get
            {
                for(int i = 0; i < keyList.Count; i++)
                    if(key == keyList[i])
                        return typeList[i];
                throw new Exception("The key doesn't exist!");
            }
        }

        public static bool operator ==(LevelDataDictionary lhs, LevelDataDictionary rhs)
        {
            return lhs.typeList == rhs.typeList;
        }

        public static bool operator !=(LevelDataDictionary lhs, LevelDataDictionary rhs)
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
    
    /// <summary>
    /// This class is entirely a wrapper for serialization of accomplishments. It does nothing except serialize and deserialize.
    /// </summary>
    [Serializable]
    public class AccomplishmentDictionary : IXmlSerializable
    {
        private int saveSlot;

        public AccomplishmentDictionary(int saveSlot)
        {
            if(saveSlot < 1 || saveSlot > 3)
                throw new ArgumentOutOfRangeException("lolno.");
            
            this.saveSlot = saveSlot;
        }

        /// <summary>
        /// For serializer ONLY.
        /// </summary>
        public AccomplishmentDictionary()
        { }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            saveSlot = reader.ReadElementContentAsInt();
            //reader.ReadEndElement();

            while(reader.LocalName != "AchievementData")
            {
                string ID = reader.Name;
                Accomplishment a = Accomplishment.GetAccomplishmentByID(ID, saveSlot);
                a.ReadXml(reader); // throw null exceptions on purpose
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Slot");
            writer.WriteString(saveSlot.ToString());
            writer.WriteEndElement();

            foreach(Accomplishment ach in Accomplishment.GetAccomplishmentList(saveSlot))
                ach.WriteXml(writer);
        }
    }
}
