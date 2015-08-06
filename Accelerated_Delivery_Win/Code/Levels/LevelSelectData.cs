using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Accelerated_Delivery_Win
{
    [Serializable]
    public class LevelSelectData
    {
        public int TotalStars { get { return (int)TimeStarNumber + (int)BoxStarNumber + (int)ScoreStarNumber; } }

        public enum Stars
        {
            Zero = 0,
            //One = 1,
            //Two = 2,
            Three = 1
        }
        public Stars TimeStarNumber;
        public Stars BoxStarNumber;
        public Stars ScoreStarNumber;

        public int Score;
        public int BoxesLost;
        public bool Completed;
        public bool Unlocked;
        [XmlIgnore]
        public TimeSpan Time;
        /// <summary>
        /// A workaround for XmlSerializer. Don't use this directly.
        /// </summary>
        public string TimeSerialization
        {
            get { return Time.ToString(); }
            set { Time = TimeSpan.Parse(value); }
        }

        public LevelSelectData(int score, int lost, TimeSpan time, bool p_3, bool unlocked, LevelCompletionData data)
        {
            if(score >= data.ThreeStarScore)
                ScoreStarNumber = Stars.Three;
            //else if(score >= data.TwoStarScore)
            //    ScoreStarNumber = Stars.Two;
            //else if(score >= data.OneStarScore)
            //    ScoreStarNumber = Stars.One;
            else
                ScoreStarNumber = Stars.Zero;

            if(time <= data.ThreeStarTime)
                TimeStarNumber = Stars.Three;
            //else if(time <= data.TwoStarTime)
            //    TimeStarNumber = Stars.Two;
            //else if(time <= data.OneStarTime)
            //    TimeStarNumber = Stars.One;
            else
                TimeStarNumber = Stars.Zero;

            if(lost <= data.ThreeStarBoxes)
                BoxStarNumber = Stars.Three;
            //else if(lost <= data.TwoStarBoxes)
            //    BoxStarNumber = Stars.Two;
            //else if(lost <= data.OneStarBoxes)
            //    BoxStarNumber = Stars.One;
            else
                BoxStarNumber = Stars.Zero;

            Score = score;
            BoxesLost = lost;
            Time = time;
            Unlocked = unlocked;
            Completed = p_3;
        }

        public LevelSelectData(LevelSelectData copy)
        {
            Time = copy.Time;
            Score = copy.Score;
            BoxStarNumber = copy.BoxStarNumber;
            BoxesLost = copy.BoxesLost;
            ScoreStarNumber = copy.ScoreStarNumber;
            Completed = copy.Completed;
            Unlocked = copy.Unlocked;
            TimeStarNumber = copy.TimeStarNumber;
        }

        public LevelSelectData()
        {
            TimeStarNumber = Stars.Zero;
            ScoreStarNumber = Stars.Zero;
            BoxStarNumber = Stars.Zero;
            Score = 0;
            BoxesLost = 0;
            Time = TimeSpan.Zero;
            Completed = false;
            Unlocked = false;
        }

        [XmlIgnore]
        public static LevelSelectData Uncomplete 
        { 
            get 
            {
                return new LevelSelectData();
            } 
        }

    }
}
