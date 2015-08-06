using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    class Machine : LevelObject
    {
        private readonly int turnsToActivate;
        private int timer = 0;
        public int MachineNumber { get; private set; }
        public bool Moving { get { return timer != 0; } }
        public bool AtOrigin { get; private set; }

        public int TurnsRequired { get { return turnsToActivate; } }

        public Machine(int machineNumber, int turnsToActivate, int capacity)
            : base(capacity, "Machine " + machineNumber)
        {
            this.MachineNumber = machineNumber;
            this.turnsToActivate = turnsToActivate;
            AtOrigin = true;
        }

        public override void AddBox()
        {
            if(!AtOrigin || Moving)
            {
                Console.WriteLine(name + " is not in the correct position to receive a box, and has lost a box as a result!");
                containingLevel.DestroyBox();
            }
            else
                base.AddBox();
        }

        public void Activate()
        {
            timer++;
        }

        public override void Update()
        {
            if(timer == turnsToActivate)
            {
                if(AtOrigin)
                    moveAllBoxes();
                AtOrigin = !AtOrigin;
            }
            if(timer != 0)
                timer++;
        }

        private void moveAllBoxes()
        {
            for(int i = 0; i < currentNumber; i++)
                next.AddBox();
            currentNumber = 0;
        }
    }
}
