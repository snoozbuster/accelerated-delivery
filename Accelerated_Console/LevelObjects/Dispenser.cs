using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    class Dispenser : LevelObject
    {
        private const int spawnRate = 4;
        private int timer = 3;

        public Dispenser()
            : base(0, "Dispenser")
        { }

        public override void AddBox()
        {
            throw new InvalidOperationException("This operation makes no sense.");
        }

        public override void PromoteBox()
        {
            timer = 0;
            next.AddBox();
            containingLevel.AddBox();
        }

        public override void Update()
        {
            timer++;
            if(timer == spawnRate)
                PromoteBox();
        }
    }
}
