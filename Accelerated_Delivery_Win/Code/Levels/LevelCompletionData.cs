using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Delivery_Win
{
    public struct LevelCompletionData
    {
        public readonly TimeSpan ThreeStarTime;
        public readonly int ThreeStarScore;
        public readonly int ThreeStarBoxes;

        /// <summary>
        /// Creates a struct outlining ideal level completion.
        /// Three stars: meets or exceeds given value.
        /// Zero stars: Is less than given value.
        /// </summary>
        /// <param name="threeStarTime">If the player's time is equal to or less than this value, three stars are given.</param>
        /// <param name="threeStarScore">If the player's score is equal to or greater than this value, three stars are given.</param>
        /// <param name="threeStarBoxes">If the player's number of boxes lost is less than or equal to this value, three stars are given.</param>
        public LevelCompletionData(TimeSpan threeStarTime, int threeStarScore, int threeStarBoxes)
        {
            ThreeStarTime = threeStarTime;
            ThreeStarScore = threeStarScore;
            ThreeStarBoxes = threeStarBoxes;
        }
    }
}
