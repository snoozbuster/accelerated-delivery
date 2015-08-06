using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    class Program
    {
        public enum State
        {
            Main, LevelSelect, Extras, Options, Achievements, Instructions, ChildMenu, // menus
            Running, None                                                              // other stuff
        }

        public static State CurrentState = State.None;

        private static Menu mainMenu;
        private static Menu levelSelect;
        private static Menu extras;
        private static Menu options;
        private static Menu achievements;
        private static Menu instructions;

        //private static Menu[] menus;

        static void Main(string[] args)
        {
            instructions = new Menu(new Dictionary<string, Action>(), State.Instructions,
                "Welcome to the console version of Accelerated Delivery. This game functions on a turn-based system. Each turn, you will get three actions to use to activate machines, spawn boxes, or simply wait. Machines may move instantly, or they make take multiple turns to move their distance. Every machine and belt has a capacity, which, if exceeded, will cause it to lose boxes. Belts have multiple \"segments\", through which boxes will advance each turn. You'll pick it up if you just start playing.");
            
            extras = new Menu(new Dictionary<string, Action>(), State.Extras, "This doesn't work yet.");
            options = new Menu(new Dictionary<string, Action>(), State.Options, "This doesn't work yet.");
            levelSelect = new Menu(new Dictionary<string, Action>(), State.LevelSelect, "This doesn't work yet.");
            achievements = new Menu(new Dictionary<string, Action>(), State.Achievements, "This doesn't work yet.");
            
            mainMenu = new Menu(new Dictionary<string, Action> { 
                { "Start Game", () => { loadLevel(1, State.Main); } },
                { "Instructions", () => { instructions.EnterMenuLoop(State.Main); } },
                { "Level Select", () => { levelSelect.EnterMenuLoop(State.Main); } },
                { "Extras", () => { extras.EnterMenuLoop(State.Main); } }
                    }, State.Main);

            //menus = new Menu[5];
            //menus[0] = mainMenu;
            //menus[1] = levelSelect;
            //menus[2] = extras;
            //menus[3] = options;
            //menus[4] = achievements;

            Console.WriteLine("                             WELCOME TO ACCELERATED DELIVERY");
            Console.WriteLine("Please select an option:");
            mainMenu.EnterMenuLoop(State.None); // and so it begins
        }

        private static void loadLevel(int levelNumber, State previousState)
        {
            Level level = new Level(1, 20, 5,
                new Belt(3, "Belt between dispenser and machine 1"),
                new Machine(1, 1, 4),
                new Belt(5, "Belt between machines 1 and 2"),
                new Machine(2, 3, 5),
                new Machine(3, 2, 5),
                new Belt(3, "Belt between machine 3 and machine 4"),
                new Machine(4, 2, 12),
                new Machine(5, 1, 6),
                new Belt(9, "Belt between machines 5 and 6"),
                new Machine(6, 4, 20),
                new Belt(10, "Belt between machine 6 and another belt"),
                new Belt(4, "Belt between previous belt and goal"));
            level.EnterMenuLoop(previousState);
        }
    }
}
