using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    class Level : Menu
    {
        protected const int actionsMax = 3;

        protected List<LevelObject> levelObjects;
        protected int levelNumber;
        protected int boxesSupplied;
        protected int boxesNeed;
        protected int boxesDestroyed = 0;
        protected int boxesActive = 0;
        protected int actionsRemaining = actionsMax;

        /// <summary>
        /// no touchy.
        /// </summary>
        internal bool done = false;
        internal int ActionsRemaining { get { return actionsRemaining; } }

        private bool finishedPrePrint = false;

        public Level(int levelNumber, int boxesSupplied, int boxesNeed, params LevelObject[] objects)
            : base(Program.State.Running)
        {
            entries = new Dictionary<string, Action>()
            {
                { "Spawn Box", () => { (levelObjects[0] as Dispenser).PromoteBox(); } },
                { "Activate Machine", activateMachines },
                { "Examine Level", examineLevel },
                { "Do Nothing", () => { } }
            };

            levelObjects = new List<LevelObject>();
            levelObjects.Add(new Dispenser());
            levelObjects.AddRange(objects);
            levelObjects.Add(new Goal(boxesNeed));

            int i = 0;
            foreach(LevelObject o in levelObjects)
            {
                if(i == 0)
                    o.Initialize(null, levelObjects[i + 1], this);
                else if(i + 1 == levelObjects.Count)
                    o.Initialize(levelObjects[i - 1], null, this);
                else
                    o.Initialize(levelObjects[i - 1], levelObjects[i + 1], this);
            }

            this.levelNumber = levelNumber;
            this.boxesSupplied = boxesSupplied;
            this.boxesNeed = boxesNeed;
        }

        public void DestroyBox()
        {
            boxesDestroyed++;
            if(boxesDestroyed > boxesSupplied - boxesNeed)
                failLevel();
        }

        private void failLevel()
        {
            Console.WriteLine("You've failed the level. Game over.");
            Console.ReadKey(false);
            done = true;
        }

        private void examineLevel()
        {
            Dictionary<string, Action> newEntries = new Dictionary<string, Action>();
            for(int i = 1; i < levelObjects.Count - 1; i++)
            {
                if(levelObjects[i] is Belt)
                {
                    Belt b = levelObjects[i] as Belt;
                    string text = "This is a belt of length " + b.Length + ". It contains " + b.CurrentNumber + " boxes, in the format " + b.GetDistribution() + ".";
                    newEntries.Add(b.Name, () => { (new Menu(new Dictionary<string,Action>(), Program.State.ChildMenu, text)).EnterMenuLoop(Program.State.Running); });
                }
                if(levelObjects[i] is Machine)
                {
                    Machine m = levelObjects[i] as Machine;
                    string text = "This is a machine that contains " + m.CurrentNumber + " boxes, requires " + m.TurnsRequired + " turn(s) to complete, " + (m.AtOrigin ? "can receive boxes" : "cannot receive boxes") + ", and " + (m.Moving ? "is moving" : "is not moving") + ".";
                    newEntries.Add(m.Name, () => { (new Menu(new Dictionary<string,Action>(), Program.State.ChildMenu, text)).EnterMenuLoop(Program.State.Running); });
                }
            }
            (new Menu(newEntries, Program.State.ChildMenu, "Actions remaining: " + actionsRemaining)).EnterMenuLoop(State);
        }

        private void activateMachines()
        {
            Dictionary<string, Action> newEntries = new Dictionary<string, Action>();
            for(int i = 1; i < levelObjects.Count; i++)
            {
                if(levelObjects[i] is Machine)
                {
                    Machine m = levelObjects[i] as Machine;
                    newEntries.Add(m.Name + " (contains " + m.CurrentNumber + " boxes, requires " + m.TurnsRequired + " turn(s) to complete, " + (m.AtOrigin && !m.Moving ? "can receive boxes" : "cannot receive boxes") + ", " + (m.Moving ? "is moving" : "is not moving") + ")",
                        () => { if(!m.Moving) { m.Activate(); actionsRemaining--; } });
                }
            }
            (new Menu(newEntries, Program.State.ChildMenu, "Actions remaining: " + actionsRemaining, true)).EnterMenuLoop(State);
        }

        protected override void doPrePrintProcessing()
        {
            Console.Clear();
            introText =
                "Level " + levelNumber + ":\n" +
                "Boxes Remaining: " + (boxesSupplied - boxesDestroyed - boxesActive) + "\tBoxes In Level: " + boxesActive + "\nBoxes Destroyed: " + boxesDestroyed + "\tBoxes Needed: " + boxesNeed + "  \tActions remaining: " + actionsRemaining;
            if(boxesSupplied - boxesDestroyed - boxesActive > 0)
                levelObjects[0].Update(); // update dispenser

            finishedPrePrint = true;
        }

        protected override void doPostInputProcessing()
        {
            if(ActionsRemaining != 0)
                return;

            for(int i = 1; i < levelObjects.Count; i++)
                levelObjects[i].Update();
            if((levelObjects[levelObjects.Count - 1] as Goal).BoxesSaved == boxesNeed)
                winLevel();
            Console.Write("Press any key to continue. ");
            Console.ReadKey(false);
            actionsRemaining = actionsMax;
            finishedPrePrint = false;
        }

        public void AddBox()
        {
            if(!finishedPrePrint)
                introText += "\nA box has been spawned!";
            else
            {
                Console.WriteLine("A box has been spawned!");
                actionsRemaining--;
            }
            boxesActive++;
        }

        private void winLevel()
        {
            Console.WriteLine("You've won the game! Hurray!");
            Console.ReadKey(false);
            done = true;
        }
    }
}
