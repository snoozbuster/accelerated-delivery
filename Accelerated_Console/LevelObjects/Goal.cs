using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    class Goal : LevelObject
    {
        public int BoxesSaved { get; private set; }

        public Goal(int boxesNeeded)
            : base(boxesNeeded, "Goal") 
        { }

        public override void AddBox()
        {
            BoxesSaved++;
            Console.WriteLine("Saved a box!");
        }

        public override void PromoteBox()
        {
            throw new InvalidOperationException("This makes no sense.");
        }

        public override void Update() { }
    }
}
