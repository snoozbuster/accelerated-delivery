using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;

#if INDIECITY
using ICELandaLib;
using System.Threading;
#endif

namespace Accelerated_Delivery_Win
{
    public static class AccomplishmentManager
    {
        private static List<Accomplishment> accomplishmentList;
        private static bool drawAccomplishmentGotten;
        private static Accomplishment achToDraw;

        private static Color textColor;
        private static Texture2D achievementTex;
        private static Vector2 scale = new Vector2(0, 0);
        private static Color tint = new Color(255, 255, 255, 0);
        private static Color titleColor { get { return new Color(194, 193, 97, tint.A); } }

        private static HelpfulTextBox text;

        private static Vector2 achievementPos;
        private static readonly Vector2 basePos;
        private static readonly Vector2 relativeScreenSpace;

        private static int height { get { return achievementTex.Height; } }
        private static int width { get { return achievementTex.Width; } }

        private static Timer timer;

        private const float deltaY = 0.1f;
        private const float deltaX = 0.05f;
        private const int deltaA = 13;

#if INDIECITY
        private static CoAchievementManager manager;
#endif

        //private static Texture2D achTex;

        static AccomplishmentManager()
        {
            timer = new Timer(7000, OnEvent, false);
            textColor = Color.Black;
            basePos = new Vector2(604, 620);
            relativeScreenSpace = new Vector2(basePos.X / 1280, basePos.Y / 720);

#if INDIECITY
            manager = new CoAchievementManager();
            manager.SetGameSession(Program.Game.Session);
            manager.InitialiseAchievements(null);
#endif
        }

        /// <summary>
        /// Performs necessary initialization.
        /// </summary>
        public static void Ready()
        {
            Program.Game.Manager.OnSaveChanged += updateAchievementList;
            Program.Game.Manager.OnSaveDeleted += initializeList;

            Program.Game.GDM.DeviceReset += onGDMReset;
            achievementTex = Program.Game.Loader.AchievementToastTexture;
            if(Program.Game.Manager.CurrentSaveNumber != 0)
                accomplishmentList = Accomplishment.GetAccomplishmentList(Program.Game.Manager.CurrentSaveNumber);
            achievementPos = new Vector2(Program.Game.Width, Program.Game.Height) * relativeScreenSpace;
            text = new HelpfulTextBox(new Rectangle((int)(achievementPos.X + (143 - achievementTex.Width * 0.5f) * Program.Game.TextureScaleFactor.X), (int)(achievementPos.Y + (50 - achievementTex.Height * 0.5f) * Program.Game.TextureScaleFactor.Y), 
                (int)(332 * Program.Game.TextureScaleFactor.X), (int)(50 * Program.Game.TextureScaleFactor.Y)), Program.Game.Loader.Font);
            text.SetScaling(new Vector2(.8f, .8f));
        }

        private static void initAchievements()
        {
            //if(Program.Game.achTex == null)
            //    Program.Game.achTex = Program.Game.Content.Load<Texture2D>("2D/Objectives/ach_compilation");

            for(int i = 1; i < 4; i++)
                initializeList(i);
        }

        private static void initializeList(int listNumber)
        {
            Accomplishment.ClearList(listNumber);
            Accomplishment.SaveFile = listNumber;

            new n00b();
            new Operator();
            new LikeABoss();
            new SecretGovernmentShipment();
            new PriorExperience();
            new BlastFromThePast();
            new ReleaseThatBreath();
            new SoundOfSpeed();
            new TacticalDelivery();
            new LevelCleared();
            new Mailman();
            new TwitchyFingers();
            new HighScoreHero();
            new HighlyTrainedProfessional();
            new Impossibru();
            new Multiplicity();
            new NotAProblem();
            new DeathWish();
            new HeavyMachinery();
            new PretzelCup();
            new Showdown();
            new TickTock();
            new ReadyForARaise();
            new EnjoyingYourSpoils();
            new CantGetEnough();
        }

        public static void Update(GameTime gameTime)
        {
#if INDIECITY
            manager.Update();
#endif

            if(accomplishmentList == null && Program.Game.Manager.CurrentSaveNumber != 0)
                updateAchievementList();

            if(!drawAccomplishmentGotten)
                foreach(Accomplishment ach in accomplishmentList)
                {
                    if(ach.Completed) // no use updating achievements already completed
                        continue;

                    int current, max;
                    ach.TimeLastFrame = gameTime;
                    bool completed = ach.CheckProgress(out current, out max);
                    if(completed)
                    {
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Achievement);
                        drawAccomplishmentGotten = true;
                        achToDraw = ach;
                        timer.Reset();
                        timer.Start();

#if INDIECITY
                        manager.UnlockAchievement(accomplishmentList.IndexOf(ach) + 2433);
#endif
                    }
                }

            if(drawAccomplishmentGotten)
            {
                timer.Update(gameTime);

                if(scale.X + deltaX >= 1)
                    scale.X = 1;
                else
                    scale.X += deltaX;
                if(scale.Y + deltaY >= 1)
                    scale.Y = 1;
                else 
                    scale.Y += deltaY;
                if(tint.A + deltaA >= 255)
                    tint.A = 255;
                else
                    tint.A += deltaA;
            }
            else if(scale.X > 0)
            {
                if(scale.X - deltaX < 0)
                    scale.X = 0;
                else
                    scale.X -= deltaX;
                if(deltaX < 0.3f)
                {
                    if(scale.Y - deltaY < 0)
                        scale.Y = 0;
                    else
                        scale.Y -= deltaY;
                }
                if(tint.A - deltaA < 0)
                {
                    tint.A = 0;
                    achToDraw = null;
                }
                else
                    tint.A -= deltaA;
            }
        }

        public static void Draw()
        {
            if(!drawAccomplishmentGotten && tint.A == 0)
                return;

            Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

            Program.Game.SpriteBatch.Draw(achievementTex, achievementPos, null, tint * (tint.A / 255f), 0, new Vector2(achievementTex.Width, achievementTex.Height) * 0.5f * Program.Game.TextureScaleFactor * scale, scale * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            
            Program.Game.SpriteBatch.Draw(achToDraw.Icon, (achievementPos - new Vector2(30, -6) * Program.Game.TextureScaleFactor), achToDraw.RenderRectangle, tint * (tint.A / 255f), 0, (new Vector2(achievementTex.Width, achievementTex.Height) * 0.5f) * Program.Game.TextureScaleFactor * scale, 0.78125f * scale * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

            Vector2 dim = Program.Game.Loader.BiggerFont.MeasureString(achToDraw.Title) * 0.85f;
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.BiggerFont, achToDraw.Title, achievementPos + new Vector2(-150 + achievementTex.Width * 0.5f, -2) * Program.Game.TextureScaleFactor, titleColor * (tint.A / 255f), 0, new Vector2(achievementTex.Width, achievementTex.Height) * 0.5f * Program.Game.TextureScaleFactor * scale / (dim.X > achievementTex.Width - 170 ? (achievementTex.Width - 180) / dim.X : 1), (dim.X > achievementTex.Width - 170 ? (achievementTex.Width - 180) / dim.X : 1) * 0.85f * scale * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            
            text.SetTextColor(textColor * (tint.A / 255f));
            text.Draw(achToDraw.Text);

            Vector2 length = Program.Game.Loader.Font.MeasureString("Progress: Completed!") * Program.Game.TextureScaleFactor;
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Progress: Completed!", achievementPos + new Vector2(achievementTex.Width * 0.5f - 42, achievementTex.Height * 0.5f - 10) * Program.Game.TextureScaleFactor, textColor * (tint.A / 255f), 0, length, Program.Game.TextureScaleFactor * scale, SpriteEffects.None, 0);

            Program.Game.SpriteBatch.End();
        }

        /// <summary>
        /// Updates loaded achievement list after a save change.
        /// </summary>
        private static void updateAchievementList()
        {
            accomplishmentList = Accomplishment.GetAccomplishmentList(Program.Game.Manager.CurrentSaveNumber);
        }

        private static void OnEvent()
        {
            drawAccomplishmentGotten = false;
        }

        private static void onGDMReset(object sender, EventArgs e)
        {
            achievementPos = new Vector2(Program.Game.Width, Program.Game.Height) * relativeScreenSpace;
            text.SetSpace(new Rectangle((int)(143 * Program.Game.TextureScaleFactor.X + achievementPos.X), (int)(50 * Program.Game.TextureScaleFactor.Y + achievementPos.Y), (int)(332 * Program.Game.TextureScaleFactor.X), (int)(50 * Program.Game.TextureScaleFactor.Y)));
        }

        internal static void InitAchievements()
        {
            initAchievements();
            
#if INDIECITY
            int test = Program.Game.UserInfo.GetId();
            CoUserAchievementList list = manager.GetUserAchievementList(Program.Game.UserInfo.GetId());
            
            Thread.Sleep(2000); // hopefully this is long enough

            // unlock all "missed" achievements
            for(int i = 0; i < list.NumberAchievements; i++)
            {
                CoAchievement ach = list.GetAchievementFromId(i + 2433);
                if(!list.IsAchievementUnlocked(i + 2433) && accomplishmentList[i].Completed)
                    ach.Unlock();
            }
#endif
        }

        #region Achievements
        private class n00b : Accomplishment
        {
            public n00b()
                : base(0, "n00b", "Fail on the instructions level", "Did you really just do that?", "lol", new Rectangle(0, 0, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.LevelNumber == 0 && Program.Game.CurrentLevel.BoxesDestroyed == 1)
                    currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class Operator : Accomplishment
        {
            public Operator()
                : base(5, "Operator", "Complete level 5.", "Rise to the occasion and do your job.", "Operator", new Rectangle(128, 0, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.LevelNumber == 5 && Program.Game.CurrentLevel.GoodEnding)
                    currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class LikeABoss : Accomplishment
        {
            public LikeABoss()
                : base(10, "Like A Boss", "Complete level 10.", "Successfully get promoted.", "LikeABoss", new Rectangle(128 * 2, 0, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.LevelNumber == 10 && Program.Game.CurrentLevel.GoodEnding)
                    currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class SecretGovernmentShipment : Accomplishment
        {
            public SecretGovernmentShipment()
                : base(11, "Secret Government Shipment", "Complete level 11.", "Operate machinery... out of this world.", "SecretGovernmentShipment", new Rectangle(128 * 3, 0, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.LevelNumber == 11 && Program.Game.CurrentLevel.GoodEnding)
                    currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class PriorExperience : Accomplishment
        {
            public PriorExperience()
                : base(12, "Prior Experience", "Complete a demo level.", "Demonstrate your knack for operating outdated machinery.", "PriorExperience", new Rectangle(128 * 4, 0, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.LevelNumber >= 12 && Program.Game.CurrentLevel.GoodEnding)
                    currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class BlastFromThePast : Accomplishment
        {
            public BlastFromThePast()
                : base(14, "Blast From The Past", "Complete all demo levels.", "Prove that you were once a legend of machines.", "BlastFromThePast", new Rectangle(0, 128, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.LevelNumber == 14 && Program.Game.CurrentLevel.GoodEnding)
                    currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class ReleaseThatBreath : Accomplishment
        {
            public ReleaseThatBreath()
                : base(5, "You Can Release That Breath Now", "Finish a level past level 5 with a star in Boxes Destroyed.", "Even under stress, a seasoned machinist mustn't drop boxes. Do you have what it takes?", "ReleaseThatBreath", new Rectangle(128, 128, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                for(int i = 6; i <= 10; i++)
                    if(Program.Game.Manager.CurrentSave.LevelData[i].BoxStarNumber == LevelSelectData.Stars.Three)
                        currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class SoundOfSpeed : Accomplishment
        {
            public SoundOfSpeed()
                : base(5, "Sound of Speed", "Finish a level past level 5 with a star in Time.", "You get paid by the hour; it's in your best interest to slow down.", "SoundOfSpeed", new Rectangle(128 * 2, 128, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                for(int i = 6; i <= 10; i++)
                    if(Program.Game.Manager.CurrentSave.LevelData[i].TimeStarNumber == LevelSelectData.Stars.Three)
                        currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class TacticalDelivery : Accomplishment
        {
            public TacticalDelivery()
                : base(5, "Tactical Delivery", "Finish a level past level 5 with a star in Score.", "Distribute boxes carefully so as to achieve an impressive score on a difficult operation.", "TacticalDelivery", new Rectangle(128 * 3, 128, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                for(int i = 6; i <= 10; i++)
                    if(Program.Game.Manager.CurrentSave.LevelData[i].ScoreStarNumber == LevelSelectData.Stars.Three)
                        currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class LevelCleared : Accomplishment
        {
            public LevelCleared()
                : base(5, "Level Cleared!", "Finish a level past level 5 with a star in Score, Boxes Lost, and Time.", "Perform splendidly under pressure.", "LevelCleared", new Rectangle(128 * 4, 128, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                for(int i = 6; i <= 10; i++)
                    if(Program.Game.Manager.CurrentSave.LevelData[i].ScoreStarNumber == LevelSelectData.Stars.Three &&
                        Program.Game.Manager.CurrentSave.LevelData[i].TimeStarNumber == LevelSelectData.Stars.Three &&
                        Program.Game.Manager.CurrentSave.LevelData[i].BoxStarNumber == LevelSelectData.Stars.Three)
                        currentProgress = 1;
                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class Mailman : Accomplishment
        {
            public Mailman()
                : base(0, "Rain, Shine, Sleet, Fog, and Lava", "Finish all levels with a star in Boxes Destroyed.", "Demonstrate your incredible cautiousness and ability to deliver boxes safely, regardless of circumstance.", "Mailman", new Rectangle(0, 128 * 2, 128, 128))
            {
                maxProgress = 13;
                NotifyEvery = 5;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                int temp = 0;
                for(int i = 1; i <= 14; i++)
                    if(i == 11)
                        continue;
                    else if(Program.Game.Manager.CurrentSave.LevelData[i].BoxStarNumber == LevelSelectData.Stars.Three)
                        temp++;
                if(currentProgress < temp)
                    currentProgress = temp;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class TwitchyFingers : Accomplishment
        {
            public TwitchyFingers()
                : base(0, "Twitchy Fingers", "Finish all levels with a star in Time.", "You REALLY should slow down.", "TwitchyFingers", new Rectangle(128, 128 * 2, 128, 128))
            {
                maxProgress = 13;
                NotifyEvery = 5;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                int temp = 0;
                for(int i = 1; i <= 14; i++)
                    if(i == 11)
                        continue;
                    else if(Program.Game.Manager.CurrentSave.LevelData[i].TimeStarNumber == LevelSelectData.Stars.Three)
                        temp++;
                if(currentProgress < temp)
                    currentProgress = temp;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class HighScoreHero : Accomplishment
        {
            public HighScoreHero()
                : base(0, "High Score Hero", "Finish all levels with a star in Score.", "Demonstrate that you played a lot of old-school games by getting lots of impressive scores.", "HighScoreHero", new Rectangle(128 * 2, 128 * 2, 128, 128))
            {
                maxProgress = 13;
                NotifyEvery = 5;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                int temp = 0;
                for(int i = 1; i <= 14; i++)
                    if(i == 11)
                        continue;
                    else if(Program.Game.Manager.CurrentSave.LevelData[i].ScoreStarNumber == LevelSelectData.Stars.Three)
                        temp++;
                if(currentProgress < temp)
                    currentProgress = temp;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class HighlyTrainedProfessional : Accomplishment
        {
            public HighlyTrainedProfessional()
                : base(0, "Highly Trained Professional", "Finish all levels with a star in Score, Time, and Boxes Lost.", "\"The player doesn't need to hear this, they're a highly trained professional.\"", "HighlyTrainedProfessional", new Rectangle(128 * 3, 128 * 2, 128, 128))
            {
                maxProgress = 13;
                NotifyEvery = 5;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                int temp = 0;
                for(int i = 1; i <= 14; i++)
                    if(i == 11)
                        continue;
                    else if(Program.Game.Manager.CurrentSave.LevelData[i].ScoreStarNumber == LevelSelectData.Stars.Three &&
                        Program.Game.Manager.CurrentSave.LevelData[i].TimeStarNumber == LevelSelectData.Stars.Three &&
                        Program.Game.Manager.CurrentSave.LevelData[i].BoxStarNumber == LevelSelectData.Stars.Three)
                        temp++;
                if(currentProgress < temp)
                    currentProgress = temp;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class Impossibru : Accomplishment
        {
            public Impossibru()
                : base(0, "\"That's Impossible!\"", "Score over 9,000 points and finish the level.", "What does the scouter say about your score?", "Impossibru", new Rectangle(128 * 4, 128 * 2, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.CurrentLevel != null && Program.Game.CurrentLevel.GoodEnding && Program.Game.CurrentLevel.Score >= 9000)
                    currentProgress = 1;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class Multiplicity : Accomplishment
        {
            public Multiplicity()
                : base(0, "Multiplicity", "Reach a multiplier of 10x.", "Save a lot of boxes in a row and max out the multiplier.", "Multiplicity", new Rectangle(0, 128 * 3, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.CurrentLevel != null && Program.Game.CurrentLevel.Multiplier == 10)
                    currentProgress = 1;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class NotAProblem : Accomplishment
        {
            public NotAProblem()
                : base(5, "Not A Problem", "Finish any level past level 5 without losing a box.", "These packages are very important. You're not going to drop ANY... right?", "NotAProblem", new Rectangle(128, 128 * 3, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.CurrentLevel != null && Program.Game.CurrentLevel.GoodEnding && Program.Game.CurrentLevel.BoxesDestroyed == 0)
                    currentProgress = 1;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress && Program.Game.LevelNumber > LevelRequired;
            }
        }

        private class DeathWish : Accomplishment
        {
            public DeathWish()
                : base(5, "Death Wish", "Finish any level past level 5 with victory depending on the last box.", "Come very, very, VERY close to failing a difficult level, but snatch victory from the jaws of defeat.", "DeathWish", new Rectangle(128 * 2, 128 * 3, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.CurrentLevel != null && Program.Game.CurrentLevel.GoodEnding &&
                    (Program.Game.CurrentLevel.BoxesSupplied - Program.Game.CurrentLevel.BoxesSaved == Program.Game.CurrentLevel.BoxesDestroyed))
                    currentProgress = 1;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress && Program.Game.LevelNumber > LevelRequired;
            }
        }

        private class HeavyMachinery : Accomplishment
        {
            protected float timer = 0;
            protected const float timerGoal = 1f;

            public HeavyMachinery()
                : base(4, "Heavy Machinery", "Operate four machines at the same time, each with at least one box in them.", "Show that you have a natural talent for efficiency and multitasking.", "HeavyMachinery", new Rectangle(128 * 3, 128 * 3, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(TimeLastFrame == null)
                    throw new InvalidOperationException("Set TimeLastFrame before calling this.");
                if(Program.Game.LevelNumber > 4)
                {
                    if(timer != 0)
                    {
                        timer += (float)TimeLastFrame.ElapsedGameTime.TotalSeconds;
                        if(timer < timerGoal)
                        {
                            int activeMachinesWithBoxes = getMachinesWithBoxes();
                            if(activeMachinesWithBoxes >= 4)
                                currentProgress = 1;
                        }
                        else
                            timer = 0;
                    }
                    else
                    {
                        int activeMachinesWithBoxes = getMachinesWithBoxes();
                        if(activeMachinesWithBoxes != 0)
                            if(activeMachinesWithBoxes >= 4)
                                currentProgress = 1;
                            else
                                timer += (float)TimeLastFrame.ElapsedGameTime.TotalSeconds;
                    }
                }

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress && Program.Game.LevelNumber >= LevelRequired;
            }

            protected int getMachinesWithBoxes()
            {
                Dictionary<OperationalMachine, bool>.KeyCollection machines = Program.Game.CurrentLevel.MachineList.Keys;
                List<OperationalMachine> activeMachines = new List<OperationalMachine>();
                foreach(OperationalMachine m in machines)
                    if(m.IsManual && m.IsActive)
                        activeMachines.Add(m);

                int machinesWithBoxes = 0;
                int lastKey = -1;
                foreach(OperationalMachine m in activeMachines)
                {
                    if(m.MachineNumber == lastKey || m is HoldRotationMachine) // machines with the same input only count once per part
                        continue;

                    List<BroadPhaseEntry> pairs = new List<BroadPhaseEntry>();
                    BaseModel bm = m.GetBase();
                    if(bm == null || bm.Ent == null)
                        continue;

                    BoundingBox bb = bm.Ent.CollisionInformation.BoundingBox;
                    bb = new BoundingBox(bb.Min - 4 * Vector3.One, bb.Max + 4 * Vector3.One);
                    Program.Game.Space.BroadPhase.QueryAccelerator.GetEntries(bm.Ent.CollisionInformation.BoundingBox, pairs);

                    foreach(BroadPhaseEntry b in pairs)
                        if(b.Tag is Box)
                        {
                            lastKey = m.MachineNumber;
                            machinesWithBoxes++;
                            break;
                        }
                }
                return machinesWithBoxes;
            }
        }

        private class PretzelCup : Accomplishment
        {
            protected float timer;
            protected const float timerGoal = 10;

            public PretzelCup()
                : base(7, "Pretzel Cup", "Touch no controls for 10 seconds with at least five boxes on screen.", "Take your hands off ALL the controls and eat those pretzels. Those boxes can wait.", "NummyNummyPretzels", new Rectangle(128 * 4, 128 * 3, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(TimeLastFrame == null)
                    throw new InvalidOperationException("This accomplishment needs to have TimeLastFrame set.");

                if(Program.Game.LevelNumber == 7 && Program.Game.CurrentLevel.BoxesInLevel >= 5)
                {
                    timer += (float)TimeLastFrame.ElapsedGameTime.TotalSeconds;
                    if(timer > timerGoal && Input.MouseLastFrame == Input.MouseState && Input.KeyboardLastFrame == Input.KeyboardState &&
                        Input.CurrentPad == Input.CurrentPadLastFrame)
                        currentProgress = 1;
                }
                else
                    timer = 0;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress && Program.Game.LevelNumber == LevelRequired;
            }
        }

        private class Showdown : Accomplishment
        {
            protected float timer;
            protected const float timerGoal = 7.5f;
            protected int level;

            protected bool boxesSent;

            public Showdown()
                : base(5, "Ultimate Showdown", "In a level past 5, send all boxes at the beginning and then finish.", "What if, on an already difficult level, all the boxes dispensed at once? Prove you can handle such a scenario.", "Showdown", new Rectangle(0, 128 * 4, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(TimeLastFrame == null)
                    throw new InvalidOperationException("TimeLastFrame needs to be set.");

                if(level == Program.Game.LevelNumber && Program.Game.LevelNumber > 5 && Program.Game.LevelNumber < 11)
                {
                    if(timer < timerGoal)
                    {
                        timer += (float)TimeLastFrame.ElapsedGameTime.TotalSeconds;
                        if(Program.Game.CurrentLevel.BoxesRemaining == 0)
                            boxesSent = true;
                    }

                    if(Program.Game.CurrentLevel.GoodEnding && boxesSent)
                        currentProgress = 1;
                }
                else
                {
                    timer = 0;
                    boxesSent = false;
                }

                level = Program.Game.LevelNumber;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress && Program.Game.LevelNumber > LevelRequired;
            }
        }

        private class TickTock : Accomplishment
        {
            private float timer;
            private const float timerGoal = 2f;
            private Entity ent;

            public TickTock()
                : base(8, "Tick Tock Goes The Clock", "Collect half of level 8's boxes in machine 7.", "There's an hourglass somewhere... it just needs sand.", "TickTock", new Rectangle(128, 128 * 4, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(TimeLastFrame == null)
                    throw new InvalidOperationException("This accomplishment needs to have TimeLastFrame set.");

                if(Program.Game.LevelNumber == 8)
                {
                    List<BroadPhaseEntry> affectedEntries = new List<BroadPhaseEntry>();
                    if(ent == null)
                        ent = Program.Game.CurrentLevel.MachineList.Keys.ElementAt(11).GetBase().Ent;

                    BoundingBox bb = ent.CollisionInformation.BoundingBox;
                    bb = new BoundingBox(bb.Min - 3 * Vector3.One, bb.Max + 3 * Vector3.One);
                    Program.Game.Space.BroadPhase.QueryAccelerator.GetEntries(bb, affectedEntries);

                    affectedEntries = affectedEntries.FindAll(v => { return v.Tag is Box; });
                    if(affectedEntries.Count >= Program.Game.CurrentLevel.BoxesSupplied / 2)
                        timer += (float)TimeLastFrame.ElapsedGameTime.TotalSeconds;
                    else
                        timer = 0;
                    if(timer > timerGoal)
                        currentProgress = 1;
                }
                else
                    timer = 0;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class ReadyForARaise : Accomplishment
        {
            public ReadyForARaise()
                : base(0, "Ready For A Raise", "Get the maximum number of stars.", "Demonstrate your commendable talent for operating machinery very well in all environments.", "ReadyForARaise", new Rectangle(128 * 2, 128 * 4, 128, 128))
            {
                maxProgress = SaveData.MaxStars;
                NotifyEvery = 10;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                if(Program.Game.Manager.CurrentSave.StarNumber > currentProgress)
                    currentProgress = Program.Game.Manager.CurrentSave.StarNumber;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class EnjoyingYourSpoils : Accomplishment
        {
            public EnjoyingYourSpoils()
                : base(0, "Enjoying Your Spoils", "Finish any level in high score mode.", "We won't lie to you; there's a hidden game mode. Find it and play it.", "EnjoyingYourSpoils", new Rectangle(128 * 3, 128 * 4, 128, 128))
            {
                maxProgress = 1;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                for(int i = 1; i <= 14; i++)
                    if(Program.Game.Manager.CurrentSave.SideBLevelData[i].Score != 0)
                    {
                        currentProgress = 1;
                        break;
                    }

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }

        private class CantGetEnough : Accomplishment
        {
            public CantGetEnough()
                : base(0, "Can't Get Enough", "Finish all levels in high score mode.", "Prove yourself to be a master of that which is hidden.", "CantGetEnough", new Rectangle(128 * 4, 128 * 4, 128, 128))
            {
                maxProgress = 14;
                NotifyEvery = 7;
            }

            public override bool CheckProgress(out int current, out int max)
            {
                int temp = 0;
                for(int i = 1; i <= 14; i++)
                    if(Program.Game.Manager.CurrentSave.SideBLevelData[i].Score != 0)
                        temp++;

                if(temp > currentProgress)
                    currentProgress = temp;

                current = currentProgress;
                max = maxProgress;
                return current == maxProgress;
            }
        }
        #endregion
    }

    #region base
    /// <summary>
    /// Base Accomplishment class. Derived classes are expected to instantiate themselves and set maxProgress.
    /// </summary>
    public abstract class Accomplishment : IXmlSerializable
    {
        /// <summary>
        /// This is where you set the texture for the achiemvements to read from.
        /// </summary>
        public static Texture2D AchievementTexture { protected get; set; }
        /// <summary>
        /// If the accomplishment needs a time, set it here.
        /// </summary>
        public GameTime TimeLastFrame { protected get; set; }
        /// <summary>
        /// Gets the text of the achievement. This is exactly what needs to be done to get it, and should
        /// only be displayed after getting the achievement.
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// Gets the hint of the achievement. This is what should be displayed until the achievement is gotten.
        /// </summary>
        public string Hint { get; private set; }
        /// <summary>
        /// Gets the name of the achievement.
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// The current level must be greater than this to actually get the achievement.
        /// </summary>
        public int LevelRequired { get; private set; }
        /// <summary>
        /// Indicates if the achievement has been completed. Does not update progress.
        /// </summary>
        public bool Completed { get { return currentProgress == maxProgress; } }
        /// <summary>
        /// Gets the achievement's icon.
        /// </summary>
        public Texture2D Icon { get { return AchievementTexture; } }
        /// <summary>
        /// Indicates how often the player should be notified of achievement progress, if applicable.
        /// The player is always notified the first time.
        /// </summary>
        public int NotifyEvery { get; protected set; }
        /// <summary>
        /// The Rectangle indicating the portion of Icon to draw.
        /// </summary>
        public Rectangle RenderRectangle { get; private set; }
        /// <summary>
        /// The max progress of the achievement.
        /// </summary>
        public int Max { get { return maxProgress; } }
        /// <summary>
        /// The current progress of the achievement.
        /// </summary>
        public int Current { get { return currentProgress; } }

        private string ID;
        protected int currentProgress = 0;
        protected int maxProgress;
        
        private static List<Accomplishment> accomplishmentList1 = new List<Accomplishment>();
        private static List<Accomplishment> accomplishmentList2 = new List<Accomplishment>();
        private static List<Accomplishment> accomplishmentList3 = new List<Accomplishment>();

        /// <summary>
        /// Lets achievements know what save file they should be reading/writing from. Only relevant for creation of new achievements,
        /// which should be done for all save files.
        /// </summary>
        public static int SaveFile { get; set; }

        protected Accomplishment(int levelReq, string title, string achText, string hint, string ID, Rectangle renderSpace)
        {
            LevelRequired = levelReq;
            Text = achText;
            Title = title;
            NotifyEvery = 1;
            Hint = hint;

            if(renderSpace == null)
                RenderRectangle = new Rectangle(0, 0, Icon.Width, Icon.Height);
            else
                RenderRectangle = renderSpace;

            if(ID.Contains(' '))
                throw new ArgumentException("ID cannot contain whitespace.");
            this.ID = ID;

            if(GetAccomplishmentByID(this.ID, SaveFile) == null)
                GetAccomplishmentList(SaveFile).Add(this);
            else
                throw new InvalidOperationException("You can't instantiate an instance of the same accomplishment twice (or that accomplishment ID has already been used).");
        }

        /// <summary>
        /// Checks the progress of an achievement.
        /// </summary>
        /// <param name="current">Current progress.</param>
        /// <param name="max">Maximum progress.</param>
        /// <returns>If the achievement is gotten, ie if current == max and the level is such that the achievement can be gotten.</returns>
        public abstract bool CheckProgress(out int current, out int max);

        /// <summary>
        /// Gets a list of accomplishements.
        /// </summary>
        /// <param name="saveSlot">The slot to get. Valid values are 1-3.</param>
        /// <returns>The list for the specified slot.</returns>
        public static List<Accomplishment> GetAccomplishmentList(int saveSlot)
        {
            switch(saveSlot)
            {
                case 1: return accomplishmentList1;
                case 2: return accomplishmentList2;
                case 3: return accomplishmentList3;
                default: throw new ArgumentOutOfRangeException("There are only three save files.");
            }
        }

        /// <summary>
        /// Gets an accomplishment from the list based on its ID.
        /// </summary>
        /// <param name="ID">The unique ID to get an achievement by.</param>
        /// <returns>The accomplishment attached to that ID, or null if that ID is not in use. Searches the current list.</returns>
        public static Accomplishment GetAccomplishmentByID(string ID, int saveSlot)
        {
            foreach(Accomplishment a in GetAccomplishmentList(saveSlot))
                if(a.ID == ID)
                    return a;
            return null;
        }

        #region junk
        public sealed override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public sealed override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public sealed override string ToString()
        {
            return "Objective";
        }
        #endregion

        /// <summary>
        /// Reads XML from a file into the accomplishment.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            //reader.ReadStartElement();
            currentProgress = reader.ReadElementContentAsInt();
            //reader.ReadEndElement();
        }

        /// <summary>
        /// Writes data from the accomplishment into XML.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(ID);
            writer.WriteString(currentProgress.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Clears a list completely. Do NOT use unless you know what you're doing.
        /// </summary>
        /// <param name="listNumber">List to clear.</param>
        internal static void ClearList(int listNumber)
        {
            switch(listNumber)
            {
                case 1: accomplishmentList1.Clear();
                    break;
                case 2: accomplishmentList2.Clear();
                    break;
                case 3: accomplishmentList3.Clear();
                    break;
                default: throw new ArgumentOutOfRangeException("You clearly do not know what you are doing.");
            }
            Program.Cutter.WriteToLog("Accomplishment", "Warning: list number " + listNumber + " cleared.");
        }
    }
    #endregion
}
