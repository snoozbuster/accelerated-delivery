using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    abstract class LevelObject
    {
        protected int capacity;
        protected int currentNumber = 0;
        protected Level containingLevel;
        protected string name;

        protected LevelObject previous;
        protected LevelObject next;

        public string Name { get { return name; } }
        public int CurrentNumber { get { return currentNumber; } }

        public LevelObject(int capacity, string name)
        {
            this.capacity = capacity;
            this.name = name;
        }

        public void Initialize(LevelObject previous, LevelObject next, Level containingLevel)
        {
            this.previous = previous;
            this.next = next;
            this.containingLevel = containingLevel;
        }

        public virtual void AddBox()
        {
            if(currentNumber == capacity)
            {
                Console.WriteLine(name + " is over capacity, and has lost a box as a result!");
                containingLevel.DestroyBox();
            }
            else
                currentNumber++;
        }

        public virtual void PromoteBox()
        {
            currentNumber--;
            next.AddBox();
        }

        /// <summary>
        /// This could be called at various times, depending on the type of the LevelObject.
        /// For example, machines and belts are called after input has been selected.
        /// The Dispenser is called before input.
        /// </summary>
        public abstract void Update();
    }
}
