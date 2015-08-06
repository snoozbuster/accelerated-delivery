using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    class Belt : LevelObject
    {
        // first number is "segment", segments = length
        // second number is number of boxes in segment
        private Dictionary<int, int> boxes;
        private int length;

        public int Length { get { return length; } }

        public Belt(int length, string name)
            : base((int)(length * 1.5), name)
        {
            boxes = new Dictionary<int, int>();
            for(int i = 0; i < length; i++)
                boxes.Add(i, 0);

            this.length = length;
        }

        public override void AddBox()
        {
            if(currentNumber != capacity)
                boxes[0]++;

            base.AddBox();
        }

        public override void PromoteBox()
        {
            if(boxes[length - 1] != 0)
            {
                for(int i = 0; i < boxes[length - 1]; i++)
                    base.PromoteBox();
                boxes[length - 1] = 0;
            }
        }

        public override void Update()
        {
            PromoteBox();
            for(int i = length - 2; i >= 0; i--)
            {
                boxes[i + 1] += boxes[i];
                boxes[i] = 0;
            }
        }

        public string GetDistribution()
        {
            string output = "[ ";
            for(int i = 0; i < boxes.Keys.Count; i++)
                output += boxes[i] + " ";
            return output + "]";
        }
    }
}
