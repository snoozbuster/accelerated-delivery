using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerated_Console
{
    class Menu
    {
        protected Dictionary<string, Action> entries;
        private Program.State state;
        public Program.State State { get { return state; } }

        protected string introText = string.Empty;
        private Menu childMenu;
        private bool singleChoice;
        protected bool done;
        protected bool isChild = false;
        protected Menu parent;

        public Menu(Dictionary<string, Action> entries, Program.State state)
        {
            this.state = state;

            if(entries.Count > 9)
            {
                Dictionary<string, Action> newEntries = new Dictionary<string, Action>();
                Dictionary<string, Action> childEntries = new Dictionary<string, Action>();
                int i = 0;
                foreach(KeyValuePair<string, Action> pair in entries)
                {
                    if(i < 9)
                    {
                        i++;
                        newEntries.Add(pair.Key, pair.Value);
                    }
                    else
                    {
                        i++;
                        childEntries.Add(pair.Key, pair.Value);
                    }
                }
                childMenu = new Menu(childEntries, Program.State.ChildMenu);
                childMenu.isChild = true;
                childMenu.parent = this;
                this.entries = newEntries;
            }
            else
                this.entries = entries;
        }

        public Menu(Dictionary<string, Action> entries, Program.State state, string introText)
            : this(entries, state)
        {
            this.introText = introText;
        }

        public Menu(Dictionary<string, Action> entries, Program.State state, string introText, bool singleChoice)
            : this(entries, state, introText)
        {
            this.singleChoice = singleChoice;
        }

        protected Menu(Program.State state) { this.state = state; }

        public virtual void EnterMenuLoop(Program.State previousState)
        {
            Program.CurrentState = state;
            done = false;

            while(!done)
            {
                doPrePrintProcessing();

                printEntries(previousState);
                int key = Console.Read();
                Console.ReadLine();
                key -= 48;
                if((key >= 1 || (key == 0 && ((isChild && entries.Count == 8) || entries.Count == 9))) && ((key - 1 <= entries.Count || (key - 2 <= entries.Count && isChild)) || entries.Count == 0))
                {
                    if((key - 1 == entries.Count || (isChild && key - 2 == entries.Count)) || entries.Count == 0 || key == 0)
                    {
                        if((isChild && (key == 0) || key - 1 == entries.Count) || !isChild)
                        {
                            if(childMenu == null)
                            {
                                done = true;

                                Program.CurrentState = previousState;
                            }
                            else
                                childMenu.EnterMenuLoop(previousState);
                        }
                        else if(isChild && key - 2 == entries.Count)
                        {
                            done = true;
                            parent.done = true;
                            Menu originalParent = parent;
                            while(parent.parent != null)
                            {
                                parent = parent.parent;
                                parent.done = true;
                            }
                            parent = originalParent;
                        }
                    }
                    else
                    {
                        entries.ElementAt(key - 1).Value();
                            
                        doPostInputProcessing();
                        
                        if(singleChoice)
                            done = true;
                    }
                }
                else
                    Console.WriteLine("Invalid input.");
            }
            done = false;
        }

        protected virtual void printEntries(Program.State previousState)
        {
            if(introText != string.Empty)
                Console.WriteLine(introText);

            int i = 1;
            foreach(string s in entries.Keys)
                Console.WriteLine(i++ + ") " + s);
            if(isChild)
                Console.WriteLine(i++ + ") " + "Return to previous choices");
            if(childMenu == null)
                Console.WriteLine(i++ + ") " + (previousState == Program.State.None || this is Level ? "Quit Level" : "Return to previous menu"));
            else
                Console.WriteLine("0) More...");
            Console.Write("Enter your selection: ");
        }

        protected virtual void doPrePrintProcessing() { }
        protected virtual void doPostInputProcessing() { }

        //public void ForceLeaveLoop()
        //{
        //    done = true;
        //}
    }
}
