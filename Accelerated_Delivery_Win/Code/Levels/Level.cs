using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using BEPUphysics;

namespace Accelerated_Delivery_Win
{
    public class Level
    {
        #region box junk
        /// <summary>
        /// The number of boxes you have safely saved.
        /// </summary>
        protected int boxesSaved;
        /// <summary>
        /// Maximum number of boxes that will be spawned.
        /// </summary>
        protected int boxesMax;// { get; protected set; }
        /// <summary>
        /// Remaining number of boxes to be spawned.
        /// </summary>
        public int BoxesRemaining { get; protected set; }
        /// <summary>
        /// Boxes required to finish the level.
        /// </summrary>
        protected int boxesNeeded;// { get; protected set; }
        /// <summary>
        /// Number of boxes destroyed.
        /// </summary>
        public int BoxesDestroyed { get; protected set; }
        /// <summary>
        /// Maximum number of boxes to be supplied this level.
        /// </summary>
        public int BoxesSupplied { get { return boxesMax; } }
        /// <summary>
        /// Number of boxes currently saved.
        /// </summary>
        public int BoxesSaved { get { return boxesSaved; } }
        protected readonly List<Box> boxesOnscreen;
        /// <summary>
        /// The point at which boxes spawn.
        /// </summary>
        public Vector3 BoxSpawnPoint { get; protected set; }

        /// <summary>
        /// Number of boxes current active.
        /// </summary>
        public int BoxesInLevel { get { return boxesOnscreen.Count; } }
        #endregion

        #region level info
        protected readonly Theme levelTheme;// { get; protected set; }

        protected readonly int levelNumber;// { get; protected set; }

        protected string levelName;// { get; protected set; }

        public string LevelName { get { return levelName; } }

        public MediaSystem.SongOptions LevelSong { get { return levelTheme.Song; } }

        public readonly LevelCompletionData CompletionData;

        /// <summary>
        /// Height at which boxes should start to fade.
        /// </summary>
        public float RemovalHeight { get { return levelTheme.RemovalHeight; } }
        #endregion

        #region machines and tubes and models
        /// <summary>
        /// The tubes in the level.
        /// </summary>
        protected readonly List<Tube> tubeList;
        /// <summary>
        /// The machines in the level.
        /// </summary>
        public Dictionary<OperationalMachine, bool> MachineList { get; protected set; }

        protected readonly BaseModel levelModel;// { get; protected set; }
        protected readonly List<BaseModel> glassModels;// { get; protected set; }
        protected BaseModel dispenser;

        protected Catcher normalCatcher;
        protected readonly Catcher coloredCatcher;

        protected Scoreboard scoreboard;
        #endregion

        #region graphics
        protected readonly Billboard Board;

        protected Texture2D[] textureList;
        protected Texture2D[] activeList;

        protected float lavaTimer = 0;
        #endregion

        #region results and overlay
        protected Timer timer;
        protected Rectangle screenSpace;
        protected Color color;
        protected bool drawResults = false;
        protected ResultsScreen results;// { get; protected set; }
        protected bool ending;// { get; protected set; }
        protected bool badEnding;

        public bool Ending { get { return ending || badEnding; } }

        public bool GoodEnding { get { return ending; } }
        public virtual int Score { get { return score; } }
        public float Multiplier { get { return multiplier; } }

        protected OpeningOverlay overlay;
        public bool ShowingOverlay { get { return overlay != null || Ending; } }
        #endregion

        #region spawning
        protected int spawnTimer = 0;
        protected int spawnTime;
        protected Random random = new Random();
        protected const int deltaBox = 3;
        protected int counter = 0;
        #endregion

        #region score and time
        protected int score = 0;
        protected float multiplier = 1;
        protected readonly float[] multiplierStages = new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 6, 7, 8, 9, 10 };
        protected int index = 0;
        protected TimeSpan time;
        protected const int baseBoxValue = 100;
        #endregion

        public bool TemporarilyMuteVoice { get; set; }

        private bool scoreboardOn = true;


        /// <summary>
        /// Create a new level.
        /// </summary>
        /// <param name="levelNo">The level number.</param>
        /// <param name="boxesMax">The number of boxes the level will give you.</param>
        /// <param name="boxNeed">The amount of boxes you need to win.</param>
        /// <param name="spawnPoint">The Vector3 to spawn boxes at.</param>
        /// <param name="levelTheme">The theme to use.</param>
        /// <param name="glassModels">The array of glass models to use.</param>
        /// <param name="billboardsThisLevel">A list of Vector3's that determine the location
        /// of this level's machine's billboards.</param>
        /// <param name="levelModel">A model of the level's static parts (machine bases and such).</param>
        /// <param name="machines">A list of machines in this level.</param>
        /// <param name="coloredCatcher">If the level has a colored catcher, this is it.</param>
        /// <param name="normalCatcher">The level's Catcher.</param>
        /// <param name="tubes">All the tubes. Lots... and lots... of tubes.</param>
        /// <param name="data">Data detailing the elite requirements of complete completion.</param>
        /// <param name="name">Level's name.</param>
        public Level(int levelNo, int boxesMax, int boxNeed, Vector3 spawnPoint, List<Vector3> billboardsThisLevel,
            Theme levelTheme, BaseModel levelModel, BaseModel[] glassModels, Catcher normalCatcher, Catcher coloredCatcher,
            Dictionary<OperationalMachine, bool> machines, List<Tube> tubes, LevelCompletionData data, string name)
        {
            levelNumber = levelNo;
            this.levelTheme = levelTheme;
            scoreboard = new Scoreboard(levelTheme.TextColor, boxNeed, boxesMax, 16333);
            this.boxesMax = boxesMax;
            boxesSaved = 0;
            BoxesDestroyed = 0;
            BoxesRemaining = boxesMax;
            boxesNeeded = boxNeed;
            BoxSpawnPoint = spawnPoint;
            levelName = name;
            boxesOnscreen = new List<Box>();
            tubeList = new List<Tube>(tubes);
            MachineList = new Dictionary<OperationalMachine, bool>(machines);
            Board = new Billboard();

            this.levelModel = levelModel;
            //LevelModel.Lighting = levelTheme.Lighting;
            this.glassModels = new List<BaseModel>();
            textureList = new Texture2D[billboardsThisLevel.Count];
            activeList = new Texture2D[billboardsThisLevel.Count];

            this.normalCatcher = normalCatcher;
            this.coloredCatcher = coloredCatcher;

            CompletionData = data;

            foreach(BaseModel m in glassModels)
                this.glassModels.Add(m);
            foreach(OperationalMachine mach in machines.Keys)
                foreach(BaseModel m in mach.GetGlassModels())
                    this.glassModels.Add(m);

            //foreach(Machine m in machines.Keys)
            //    m.ApplyLighting(levelTheme.Lighting);

            if(billboardsThisLevel.Count != 0)
                Board.CreateBillboardVerticesFromList(billboardsThisLevel);

            for(int i = 0; i < textureList.Length; i++)
            {
                textureList[i] = Program.Game.LoadingScreen.loader.billboardList[i];
                activeList[i] = Program.Game.LoadingScreen.loader.activeBillboardList[i];
            }

            dispenser = new BaseModel(Program.Game.LoadingScreen.loader.Dispenser, false, null, BoxSpawnPoint);
            dispenser.Ent.BecomeKinematic();

            //if(levelNumber != 0)
            //    Program.Game.Manager.CurrentSave.LevelData.SetCompletionData(levelNumber, data);
        }

        public virtual void Update(GameTime gameTime)
        {
            if(Input.KeyboardState.IsKeyDown(Keys.LeftControl) && Input.KeyboardState.IsKeyDown(Keys.F1))
                scoreboardOn = !scoreboardOn;
            
            updateMachines(gameTime);

            if(!ending)
            {
                if(overlay != null)
                {
                    overlay.Update(gameTime);
                    if(overlay.Completed && !badEnding)
                    {
                        overlay = null;
                        if(!TemporarilyMuteVoice)
                            MediaSystem.PlayVoiceActing(levelNumber);
                    }
                    else if(overlay.Completed && badEnding)
                    {
                        Program.Game.State = BaseGame.GameState.GameOver;
                        return;
                    }
                }
#if DEBUG
                if(!badEnding && !ending)
                {
                    spawnDebugBox();
                    if(Input.CheckKeyboardJustPressed(Keys.LeftShift))
                    {
                        endLevel();
                        return;
                    }
                }
#endif
                if(overlay == null)
                    time += gameTime.ElapsedGameTime;
                for(int i = 0; i < boxesOnscreen.Count; i++)
                {
                    boxesOnscreen[i].Update(gameTime);
                    if(boxesOnscreen[i].NeedsRemoval)
                    {
                        if(boxesOnscreen[i].BoxColor != Color.Black)
                            DestroyBox(boxesOnscreen[i]);
                        else
                            VanishBox(boxesOnscreen[i] as BlackBox, true);
                        i--; // Need to redo this number so that we don't miss some.
                    }
                }

                foreach(Tube t in tubeList)
                    t.Update(gameTime);

                if(overlay == null)
                    spawnTimer += gameTime.ElapsedGameTime.Milliseconds * (levelNumber != 0 ? (boxesMax == BoxesRemaining ? 11 : 1) : 0);

                if(!badEnding && overlay == null)
                {
                    if((spawnTimer >= spawnTime ||
                        Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.QuickBoxKey) ||
                        Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.QuickBoxKey)) && BoxesRemaining > 0)
                        spawnBox();

                    if(BoxesDestroyed * 1.3 >= boxesMax - boxesNeeded && !MediaSystem.SirenPlaying && levelNumber != 0 && levelNumber != 11)
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Siren);
                    else if(BoxesDestroyed > boxesMax - boxesNeeded)
                    {
                        badEnding = true;
                        overlay = new OpeningOverlay(false);
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Fail);
                        MediaSystem.StopSiren();
                    }
                    else if(boxesSaved >= boxesNeeded)
                        endLevel();
                }
            }
            else // is ending
            {
                if(overlay != null)
                {
                    overlay.Update(gameTime);
                    scoreboard.Update(boxesSaved, BoxesDestroyed, BoxesRemaining, score, multiplier, spawnTimer, time); // so that popups can still get removed
                    if(overlay.Completed)
                        overlay = null;
                    else
                        return;
                }
                timer.Update(gameTime);
                if(!drawResults)
                {
                    if(color.A + 3 >= 255)
                        color.A = 255;
                    else
                        color.A += 3;
                }
                else
                {
                    if(results.Update(gameTime))
                        loadNextLevel();
                }
            }
            scoreboard.Update(boxesSaved, BoxesDestroyed, BoxesRemaining, score, multiplier, spawnTimer, time);
        }

        protected virtual void updateMachines(GameTime gameTime)
        {
            foreach(OperationalMachine m in MachineList.Keys)
#if DEBUG
            {
                if(spawnlevel == 0)
                    m.inputPaused = false;
                else
                    m.inputPaused = true;
#endif
                m.Update(gameTime);
#if DEBUG
            }
#endif
        }

        public virtual void Draw(GameTime gameTime)
        {
            levelTheme.UpdateShader(gameTime);

            RenderingDevice.Draw();

            if(drawResults)
            {
                results.Draw();
                return;
            }

            Board.DrawBillboards(textureList, activeList);
            if(scoreboardOn)
                scoreboard.Draw();
            if(overlay != null)
                overlay.Draw();
#if DEBUG
            drawDebugJunk();
#endif
            if(ending)
            {
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                Program.Game.SpriteBatch.Draw(Program.Game.Loader.EmptyTex, screenSpace, color);
                Program.Game.SpriteBatch.End();
            }
        }

        #region add/remove from game
        public virtual void AddToGame(Space s)
        {
            levelTheme.SetUpLighting();
            TemporarilyMuteVoice = false;

            MediaSystem.PlayTrack(levelTheme.Song);
            overlay = new OpeningOverlay(levelNumber < 12 ? levelNumber : levelNumber - 11, levelNumber > 11, levelName);
            time = new TimeSpan();
            RebuildTiming();

            foreach(OperationalMachine m in MachineList.Keys)
                Program.Game.Space.Add(m);
            s.Add(levelModel);
            foreach(Tube t in tubeList)
                s.Add(t);
            foreach(BaseModel m in glassModels)
                if(m.Ent.Space == null)
                    s.Add(m);
            s.Add(dispenser);
            if(levelTheme.Fluid != null)
                s.Add(levelTheme.Fluid);
            addModelsToRenderer();
            results = null;
            ending = badEnding = false;
        }

        public virtual void RemoveFromGame(Space s)
        {
            TemporarilyMuteVoice = false;
            removeModelsFromRenderer();
            s.Clear();
            MediaSystem.StopSiren();
            MediaSystem.StopSFX();
            if(overlay != null)
                overlay.RemoveThineself();
            overlay = null;
        }

        protected virtual void addModelsToRenderer()
        {
            foreach(OperationalMachine m in MachineList.Keys)
                RenderingDevice.Add(m);
            foreach(Tube t in tubeList)
                RenderingDevice.Add(t);
            foreach(BaseModel m in glassModels)
                RenderingDevice.Add(m);

            RenderingDevice.Add(levelTheme.OuterModel);
            RenderingDevice.Add(levelTheme.SkyBox);
            RenderingDevice.Add(levelModel);
            RenderingDevice.Add(dispenser);

            levelTheme.InitializeShader();

            normalCatcher.AddToGame(Program.Game.Space);
            if(coloredCatcher != null)
                coloredCatcher.AddToGame(Program.Game.Space);
        }

        protected virtual void removeModelsFromRenderer()
        {
            foreach(OperationalMachine m in MachineList.Keys)
                RenderingDevice.Remove(m);
            foreach(Tube t in tubeList)
                RenderingDevice.Remove(t);
            foreach(BaseModel m in glassModels)
                RenderingDevice.Remove(m);

            foreach(Box b in boxesOnscreen)
                RenderingDevice.RemovePermanent(b);

            RenderingDevice.Remove(levelTheme.OuterModel);
            RenderingDevice.Remove(levelTheme.SkyBox);
            RenderingDevice.Remove(levelModel);
            RenderingDevice.Remove(dispenser);
            RenderingDevice.Remove(levelTheme.PlaneModel);

            normalCatcher.RemoveFromGame();
            if(coloredCatcher != null)
                coloredCatcher.RemoveFromGame();
        }
        #endregion

        #region resetting methods
        public virtual void ResetLevel()
        {
            releaseBoxes();

            foreach(OperationalMachine m in MachineList.Keys)
                m.ResetMachine();

            RenderingDevice.Camera.Reset();
            boxesSaved = 0;
            BoxesDestroyed = 0;
            spawnTimer = 0;
            score = 0;
            counter = 0;
            time = TimeSpan.Zero;
            BoxesRemaining = boxesMax;
            badEnding = false;
            scoreboard.Reset();
            ending = false;
            MediaSystem.StopSiren();
            MediaSystem.StopVoiceActing();
            if(overlay != null)
                overlay.RemoveThineself();
            overlay = new OpeningOverlay(levelNumber < 12 ? levelNumber : levelNumber - 11, levelNumber > 11, levelName);
        }

        /// <summary>
        /// Resets the timing for a difficulty change.
        /// </summary>
        public void RebuildTiming()
        {
            spawnTime = 16333;
            //switch(Program.Game.Manager.CurrentSaveWindowsOptions.DifficultyLevel)
            //{
            //    case WindowsOptions.Difficulty.Easy: spawnTime = 15000;
            //        break;
            //    case WindowsOptions.Difficulty.Hard: spawnTime = 5000;
            //        break;
            //    case WindowsOptions.Difficulty.Medium: spawnTime = 10000;
            //        break;
            //}
        }
        #endregion

        #region box handling
        public void DestroyBox(Box b)
        {
            if(!Ending)
            {
                BoxesDestroyed++;
                multiplier = multiplierStages[0];
                index = 0;
                if(score < baseBoxValue)
                    score = 0;
                else
                    score -= baseBoxValue;
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Box_Death);
            }

            boxesOnscreen.Remove(b);
            RenderingDevice.RemovePermanent(b);
            Program.Game.Space.Remove(b);
        }

        public void CatchBox(Box b)
        {
            if(b is BlackBox)
            {
                (b as BlackBox).Remove();
                if(!Ending)
                {
                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Box_Death);
                    boxesSaved--;
                    score -= baseBoxValue;
                    multiplier = multiplierStages[0];
                    index = 0;
                    BoxesDestroyed++;
                }
            }
            if(!Ending)
            {
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Box_Success);
                boxesSaved++;
                score += (int)(baseBoxValue * multiplier + b.ExtraPoints);
                if(index + 1 == multiplierStages.Length)
                    multiplier = multiplierStages[index];
                else
                    multiplier = multiplierStages[++index];
            }
            
            boxesOnscreen.Remove(b);
            RenderingDevice.RemovePermanent(b);
            Program.Game.Space.Remove(b);
        }

        public void VanishBox(BlackBox b, bool destroyed)
        {
            if(b == null)
            {
                Program.Cutter.WriteToLog(this, "Warning: VanishBox() called with a null parameter.");
                return;
            }

            boxesOnscreen.Remove(b);
            RenderingDevice.RemovePermanent(b);
            b.Ent.Space.Remove(b);
            b.Remove();
            if(destroyed)
            {
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Box_Success);
                score += (int)(baseBoxValue * multiplier);
                if(index + 1 == multiplierStages.Length)
                    multiplier = multiplierStages[index];
                else
                    multiplier = multiplierStages[++index];
            }
        }

        protected virtual void spawnBox()
        {
            spawnTimer = 0;
            Box box;
            if(coloredCatcher != null)
            {
                bool blue = random.Next(0, 1) == 0;
                if(blue && counter < deltaBox)
                {
                    box = new Box(Color.Blue);
                    counter++;
                }
                else
                {
                    box = new Box();
                    counter = 0;
                }
            }
            else
                box = new Box();

            if(!(spawnTimer >= spawnTime))
                box.ExtraPoints = (spawnTime - spawnTimer) / 60;
            boxesOnscreen.Add(box);
            RenderingDevice.Add(box);
            BoxesRemaining--;
        }
        #endregion

        #region protected methods
        protected virtual void endLevel()
        {
            MediaSystem.StopSiren();
            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Win);
            overlay = new OpeningOverlay(true);
            results = new ResultsScreen(time, BoxesDestroyed, score, levelNumber, CompletionData);
            ending = true;
            color = new Color(0, 0, 0, 0);
            timer = new Timer(2500, onTimerFired, false);
            timer.Start();
            TemporarilyMuteVoice = false;
            screenSpace = new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height);
        }

        protected void releaseBoxes()
        {
            foreach(Box b in boxesOnscreen)
            {
                if(b is BlackBox)
                    (b as BlackBox).Remove();
                RenderingDevice.RemovePermanent(b);
            }
            for(int i = 0; i < Program.Game.Space.Entities.Count; i++)
                if(Program.Game.Space.Entities[i].Tag is Box)
                    Program.Game.Space.Remove(Program.Game.Space.Entities[i--]);
            boxesOnscreen.Clear();
        }

        protected virtual void onTimerFired()
        {
            RemoveFromGame(Program.Game.Space);
            drawResults = true;
            results.Ready();

            foreach(OperationalMachine m in MachineList.Keys)
                m.ResetMachine();

            MediaSystem.EndingLevel();
        }

        protected virtual void loadNextLevel()
        {
            results.RemoveFromGame();
            if(Program.Game.LevelEnteredFrom == BaseGame.GameState.Running && Program.Game.LevelNumber < 10 && Program.Game.LevelNumber != 0)
            {
                Program.Game.State = BaseGame.GameState.Running;
                Program.Game.LevelNumber++;
                Program.Game.Manager.CurrentSave.LevelData[Program.Game.LevelNumber].Unlocked = true;
            }
            else if(Program.Game.LevelNumber == 10)
            {
                Program.Game.LevelNumber = -1;
                Program.Game.State = BaseGame.GameState.Ending;
                MediaSystem.PlayTrack(MediaSystem.SongOptions.Credits);
                MediaSystem.StopSFX();
            }
            else
            {
                if(levelNumber != 10 && levelNumber != 11 && levelNumber != 14)
                    Program.Game.Manager.CurrentSave.LevelData[Program.Game.LevelNumber + 1].Unlocked = true;
                Program.Game.LevelNumber = -1;
                Program.Game.State = Program.Game.LevelEnteredFrom == BaseGame.GameState.Menuing_Lev ? BaseGame.GameState.Menuing_Lev : BaseGame.GameState.MainMenu;
                MediaSystem.PlayTrack(MediaSystem.SongOptions.Menu);
            }
            drawResults = false;
        }
        #endregion

        #region Spawning - Debug
#if DEBUG
        public int spawnlevel { get; protected set; }
        protected int place = 1;
        protected Vector3 location;
        protected List<int> intList;
        protected bool isNegative = false;
        protected Vector3 lastLocation;
        protected bool spawnBlackBox = false;

        protected void spawnDebugBox()
        {
            if(intList == null)
                intList = new List<int>();
            if(Input.CheckKeyboardJustPressed(Keys.Enter))
            {
                spawnlevel++;
                place = 1;
                isNegative = false;
                intList.Clear();
            }
            else if(Input.CheckKeyboardJustPressed(Keys.Tab))
            {
                spawnlevel = 0;
                place = 1;
                isNegative = false;
                location = Vector3.Zero;
                intList.Clear();
                return;
            }
            else if(Input.CheckKeyboardJustPressed(Keys.RightShift))
            {
                location = lastLocation;
                spawnlevel = 4;
            }
            if(spawnlevel == 0)
                return;

            Keys[] pressedKeys = Input.KeyboardState.GetPressedKeys();
            if(pressedKeys.Length > 0)
                if(pressedKeys[0] != Keys.None && pressedKeys[0] != Keys.Enter)
                {
                    if(Input.CheckKeyboardJustPressed(pressedKeys[0]))
                    {
                        int numberKey = ((int)pressedKeys[0] - 48);
                        if((numberKey < 0 || numberKey > 9) && numberKey != 141 && numberKey != -40 && numberKey != 113)
                            return;
                        if(numberKey == 141)
                        {
                            isNegative = !isNegative;
                            return;
                        }
                        switch(spawnlevel)
                        {
                            case 1:
                                if(numberKey == -40)
                                {
                                    location.X = 0;
                                    intList.Clear();
                                    return;
                                }
                                intList.Add(numberKey);
                                location.X = 0;
                                place = 1;
                                for(int i = intList.Count; i > 0; i--)
                                {
                                    location.X += intList[i - 1] * place;
                                    place *= 10;
                                }
                                if(isNegative)
                                    location.X = -location.X;
                                break;
                            case 2:
                                if(numberKey == -40)
                                {
                                    location.Y = 0;
                                    intList.Clear();
                                    return;
                                }
                                intList.Add(numberKey);
                                location.Y = 0;
                                place = 1;
                                for(int i = intList.Count; i > 0; i--)
                                {
                                    location.Y += intList[i - 1] * place;
                                    place *= 10;
                                }
                                if(isNegative)
                                    location.Y = -location.Y;
                                break;
                            case 3:
                                if(numberKey == -40)
                                {
                                    location.Z = 0;
                                    intList.Clear();
                                    return;
                                }
                                intList.Add(numberKey);
                                location.Z = 0;
                                place = 1;
                                for(int i = intList.Count; i > 0; i--)
                                {
                                    location.Z += intList[i - 1] * place;
                                    place *= 10;
                                }
                                if(isNegative)
                                    location.Z = -location.Z;
                                break;
                        }
                    }
                }
            if(spawnlevel == 4)
            {
                place = 1;
                Box b = new Box(location);
                boxesOnscreen.Add(b);
                RenderingDevice.Add(b);
                if(BoxesRemaining > 0)
                    BoxesRemaining--;
                lastLocation = location;
                location = Vector3.Zero;
                spawnlevel = 0;
                Program.Cutter.WriteToLog(this, "Box unnaturally spawned at: " + location.ToString());
            }
        }

        protected void drawDebugJunk()
        {
            if(spawnlevel > 0 && spawnlevel < 4)
            {
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Enter a location: ", new Vector2(Program.Game.Height - 40, 60), Color.GhostWhite);
                Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "X: " + (isNegative && spawnlevel == 1 ? "-" : "") + Math.Abs(location.X), new Vector2(Program.Game.Height - 40, 80), spawnlevel == 1 ? Color.BlueViolet : Color.GhostWhite);
                Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Y: " + (isNegative && spawnlevel == 2 ? "-" : "") + Math.Abs(location.Y), new Vector2(Program.Game.Height - 40, 100), spawnlevel == 2 ? Color.BlueViolet : Color.GhostWhite);
                Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Z: " + (isNegative && spawnlevel == 3 ? "-" : "") + Math.Abs(location.Z), new Vector2(Program.Game.Height - 40, 120), spawnlevel == 3 ? Color.BlueViolet : Color.GhostWhite);
                Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Tab to cancel.", new Vector2(Program.Game.Height - 40, 140), Color.GhostWhite);
                Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Backspace to clear.", new Vector2(Program.Game.Height - 40, 160), Color.GhostWhite);
                Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Right Shift to use last location.", new Vector2(Program.Game.Height - 40, 180), Color.GhostWhite);
                Program.Game.SpriteBatch.End();
            }
        }
#endif
        #endregion

        #region helper classes
        protected class OpeningOverlay
        {
            protected SuperTextor levelText;
            protected SuperTextor numberWord;
            protected SuperTextor demoText;
            protected int stage = 1;
            protected bool failure = false;
            protected bool ending;
            protected string levelName;
            protected SpriteFont font;

            protected Vector2 stringPos;
            protected Vector2 stringSpeed;

            public bool Completed { get; protected set; }

            public OpeningOverlay(int level, bool demo, string name)
            {
                if(level > 11 || level < 0)
                    throw new ArgumentException(level + " is not a valid level number.");
                Completed = false;
                if(demo)
                    demoText = Program.Game.Loader.OverlayWords[12];
                levelText = Program.Game.Loader.LevelOverlay;
                numberWord = Program.Game.Loader.OverlayWords[level];

                levelName = "\"" + name + "\'\'";

                font = Program.Game.Loader.BiggerFont;
                stringPos = new Vector2(numberWord.Center.X + 20 * Program.Game.TextureScaleFactor.X, numberWord.LowerRight.Y + 10 * Program.Game.TextureScaleFactor.Y);

                levelText.Reset();
                numberWord.Reset();
                if(demoText != null)
                    demoText.Reset();

                levelText.MoveTo(new Vector2(Program.Game.Width * 0.45f, levelText.Center.Y), 1.1f);
                stringSpeed = (new Vector2(Program.Game.Width * 0.55f, stringPos.Y) - stringPos) * Program.Game.Space.TimeStepSettings.TimeStepDuration / 1.1f;
                if(demoText != null)
                    demoText.MoveTo(new Vector2(Program.Game.Width * 0.45f, demoText.Center.Y), 1.1f);
                numberWord.MoveTo(new Vector2(Program.Game.Width * 0.55f, numberWord.Center.Y), 1.1f);
            }

            /// <summary>
            /// Creates an overlay that handles flying text. True displays "completed" whereas false displays "failed."
            /// </summary>
            /// <param name="completed"></param>
            public OpeningOverlay(bool completed)
            {
                Completed = false;
                ending = true;
                levelText = Program.Game.Loader.LevelOverlay;
                if(completed)
                    numberWord = Program.Game.Loader.OverlayWords[14];
                else
                    numberWord = Program.Game.Loader.OverlayWords[13];
                levelText.Reset();
                numberWord.Reset();
                failure = !completed;
                levelText.MoveTo(new Vector2(Program.Game.Width * 0.45f, levelText.Center.Y), 1.1f);
                numberWord.MoveTo(new Vector2(Program.Game.Width * 0.65f, numberWord.Center.Y), 1.1f);
            }

            public void Update(GameTime gameTime)
            {
                stringPos += stringSpeed;
                switch(stage)
                {
                    case 1:
                        if(!levelText.Moving)
                        {
                            levelText.MoveTo(new Vector2(Program.Game.Width * 0.49f, levelText.Center.Y), 3);
                            if(demoText != null)
                                demoText.MoveTo(new Vector2(Program.Game.Width * 0.49f, demoText.Center.Y), 3);
                            if(!ending || (ending && failure))
                            {
                                numberWord.MoveTo(new Vector2(Program.Game.Width * 0.51f, numberWord.Center.Y), 3);
                                stringSpeed = (new Vector2(Program.Game.Width * 0.51f, stringPos.Y) - stringPos) * Program.Game.Space.TimeStepSettings.TimeStepDuration / 2.6f;
                            }
                            else if(ending && !failure)
                            {
                                numberWord.MoveTo(new Vector2(Program.Game.Width * 0.58f, numberWord.Center.Y), 3);
                                stringSpeed = (new Vector2(Program.Game.Width * 0.58f, stringPos.Y) - stringPos) * Program.Game.Space.TimeStepSettings.TimeStepDuration / 2.6f;
                            }
                            stage++;
                        }
                        break;
                    case 2:
                        if(!levelText.Moving && !failure)
                        {
                            levelText.MoveTo(new Vector2(Program.Game.Width + levelText.Width * 0.5f, levelText.Center.Y), 1.1f);
                            if(demoText != null)
                                demoText.MoveTo(new Vector2(Program.Game.Width + demoText.Width * 0.5f, demoText.Center.Y), 1.1f);
                            numberWord.MoveTo(new Vector2(-numberWord.Center.X, numberWord.Center.Y), 1.1f);
                            if(font != null)
                                stringSpeed = (new Vector2(-font.MeasureString(levelName).X, stringPos.Y) - stringPos) * Program.Game.Space.TimeStepSettings.TimeStepDuration / 0.8f;
                            stage++;
                        }
                        else if(!levelText.Moving && failure)
                        {
                            levelText.MoveTo(levelText.Center, 2f);
                            numberWord.MoveTo(numberWord.Center, 2f);
                            stage++;
                        }
                        break;
                    case 3:
                        if(!levelText.Moving)
                        {
                            if(!failure)
                            {
                                levelText.Reset();
                                if(demoText != null)
                                    demoText.Reset();
                                numberWord.Reset();
                            }
                            Completed = true;
                            stage = 1;
                        }
                        break;
                    default: throw new Exception("A problem in OpeningOverlay.Update().");
                }
            }

            public void Draw()
            {
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                if(Program.Game.State == BaseGame.GameState.GameOver)
                    Program.Game.SpriteBatch.Draw(Program.Game.Loader.halfBlack, new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), Color.White);
                if(font != null && levelName != "\"\'\'" && levelName != null)
                    Program.Game.SpriteBatch.DrawString(font, levelName, stringPos, Color.Gainsboro, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                levelText.Draw();
                numberWord.Draw();
                if(demoText != null)
                    demoText.Draw();
                Program.Game.SpriteBatch.End();
            }

            public void RemoveThineself()
            {
                levelText.Reset();
                numberWord.Reset();
                if(demoText != null)
                    demoText.Reset();
            }
        }

        protected class ResultsScreen
        {
            private TimeSpan time;
            private int lost, score;
            //private Model plane;
            private Entity entity;
            private Box box;
            private Timer timer;
            private int level = 0;
            //private Matrix[] transforms;
            private Rectangle screenSpace;
            private Color color;
            private VertexBuffer buff;

            private int levelNumber;
            private LevelCompletionData completionData;

            public ResultsScreen(TimeSpan timeTaken, int boxesLost, int score, int level, LevelCompletionData data)
            {
                this.levelNumber = level;
                completionData = data;
                time = new TimeSpan(timeTaken.Ticks);
                lost = boxesLost;
                this.score = score;
                //plane = Program.Game.loader.resultsPlane;
                timer = new Timer(3000, OnFirstFire, false);
                entity = new BEPUphysics.Entities.Prefabs.Box(Vector3.Zero, 20, 20, 1);
                //transforms = new Matrix[plane.Bones.Count];
                screenSpace = new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height);
                color = new Color(0, 0, 0, 255);
            }

            public void Draw()
            {
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                if(level == 0)
                    Program.Game.SpriteBatch.Draw(Program.Game.Loader.EmptyTex, screenSpace, color);
                if(level >= 1)
                    Program.Game.SpriteBatch.DrawString(Program.Game.Loader.BiggerFont, "Results", new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.15f), Color.White, 0, Program.Game.Loader.BiggerFont.MeasureString("Results") * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                if(level >= 2)
                {
                    if(levelNumber != 11)
                    {
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Time Taken:", new Vector2(Program.Game.Width * 0.25f, Program.Game.Height * 0.4f), Color.White, 0, new Vector2(Program.Game.Loader.Font.MeasureString("Time Taken:").X, 0), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Boxes Lost:", new Vector2(Program.Game.Width * 0.25f, Program.Game.Height * 0.55f), Color.White, 0, new Vector2(Program.Game.Loader.Font.MeasureString("Boxes Lost:").X, 0), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Score:", new Vector2(Program.Game.Width * 0.25f, Program.Game.Height * 0.7f), Color.White, 0, new Vector2(Program.Game.Loader.Font.MeasureString("Score:").X, 0), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                    else
                    {
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Completed:", new Vector2(Program.Game.Width * 0.25f, Program.Game.Height * 0.5f), Color.White, 0, new Vector2(Program.Game.Loader.Font.MeasureString("Completed:").X, 0), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Time Taken:", new Vector2(Program.Game.Width * 0.25f, Program.Game.Height * 0.65f), Color.White, 0, new Vector2(Program.Game.Loader.Font.MeasureString("Time Taken:").X, 0), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
                if(level >= 3)
                {
                    string t = convertTimeSpan(time);
                    if(levelNumber != 11)
                    {
                        float constant = Program.Game.Manager.CurrentSaveWindowsOptions.HighScoreMode ? 0.65f : 0.4f;
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, t, new Vector2(Program.Game.Width * constant, Program.Game.Height * 0.4f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, lost.ToString(), new Vector2(Program.Game.Width * constant, Program.Game.Height * 0.55f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, score.ToString(), new Vector2(Program.Game.Width * constant, Program.Game.Height * 0.7f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        if(!Program.Game.Manager.CurrentSaveWindowsOptions.HighScoreMode)
                        {
                            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "(target: " + convertTimeSpan(completionData.ThreeStarTime) + ")", new Vector2(Program.Game.Width * 0.45f, Program.Game.Height * 0.4f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "(max allowed: " + completionData.ThreeStarBoxes + ")", new Vector2(Program.Game.Width * 0.45f, Program.Game.Height * 0.55f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "(target: " + completionData.ThreeStarScore + ")", new Vector2(Program.Game.Width * 0.45f, Program.Game.Height * 0.7f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else if(level == 3)
                        {
                            level++;
                            timer = new Timer(500, OnFifthFire, false);
                            timer.Start();
                        }
                    }
                    else
                    {
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Yes", new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.5f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, t, new Vector2(Program.Game.Width * 0.65f, Program.Game.Height * 0.65f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
                if(level >= 4 && (!Program.Game.Manager.CurrentSaveWindowsOptions.HighScoreMode || levelNumber == 11))
                {
                    Rectangle r = new Rectangle(39, 36, 39, 36);
                    if(levelNumber != 11)
                    {
                        if(time <= completionData.ThreeStarTime)
                        {
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Program.Game.Width * 0.75f, Program.Game.Height * 0.4f), r, Color.White, 0, new Vector2(r.Width * 0.5f, r.Height * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            if(Program.Game.Manager.CurrentSave.LevelData[levelNumber].TimeStarNumber != LevelSelectData.Stars.Three)
                                Program.Game.SpriteBatch.Draw(Program.Game.Loader.Plus1, new Vector2(Program.Game.Width * 0.75f + r.Width * 0.5f * Program.Game.TextureScaleFactor.X, Program.Game.Height * 0.4f - r.Height * 0.5f * Program.Game.TextureScaleFactor.Y), null, Color.White, 0, new Vector2(Program.Game.Loader.Plus1.Width, Program.Game.Loader.Plus1.Height) * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Program.Game.Width * 0.75f, Program.Game.Height * 0.4f), new Rectangle(0, 0, r.Width, r.Height), Color.White, 0, new Vector2(r.Width * 0.5f, r.Height * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        if(lost <= completionData.ThreeStarBoxes)
                        {
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Program.Game.Width * 0.75f, Program.Game.Height * 0.55f), r, Color.White, 0, new Vector2(r.Width * 0.5f, r.Height * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            if(Program.Game.Manager.CurrentSave.LevelData[levelNumber].BoxStarNumber != LevelSelectData.Stars.Three)
                                Program.Game.SpriteBatch.Draw(Program.Game.Loader.Plus1, new Vector2(Program.Game.Width * 0.75f + r.Width * 0.5f * Program.Game.TextureScaleFactor.X, Program.Game.Height * 0.55f - r.Height * 0.5f * Program.Game.TextureScaleFactor.Y), null, Color.White, 0, new Vector2(Program.Game.Loader.Plus1.Width, Program.Game.Loader.Plus1.Height) * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Program.Game.Width * 0.75f, Program.Game.Height * 0.55f), new Rectangle(0, 0, r.Width, r.Height), Color.White, 0, new Vector2(r.Width * 0.5f, r.Height * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        if(score >= completionData.ThreeStarScore)
                        {
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Program.Game.Width * 0.75f, Program.Game.Height * 0.7f), r, Color.White, 0, new Vector2(r.Width * 0.5f, r.Height * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            if(Program.Game.Manager.CurrentSave.LevelData[levelNumber].ScoreStarNumber != LevelSelectData.Stars.Three)
                                Program.Game.SpriteBatch.Draw(Program.Game.Loader.Plus1, new Vector2(Program.Game.Width * 0.75f + r.Width * 0.5f * Program.Game.TextureScaleFactor.X, Program.Game.Height * 0.7f - r.Height * 0.5f * Program.Game.TextureScaleFactor.Y), null, Color.White, 0, new Vector2(Program.Game.Loader.Plus1.Width, Program.Game.Loader.Plus1.Height) * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Program.Game.Width * 0.75f, Program.Game.Height * 0.7f), new Rectangle(0, 0, r.Width, r.Height), Color.White, 0, new Vector2(r.Width * 0.5f, r.Height * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                    else
                    {
                        Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Program.Game.Width * 0.75f, Program.Game.Height * 0.5f), r, Color.White, 0, new Vector2(r.Width * 0.5f, r.Height * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        if(!Program.Game.Manager.CurrentSave.LevelData[levelNumber].Completed)
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.Plus1, new Vector2(Program.Game.Width * 0.75f + r.Width * 0.5f * Program.Game.TextureScaleFactor.X, Program.Game.Height * 0.5f - r.Height * 0.5f * Program.Game.TextureScaleFactor.Y), null, Color.White, 0, new Vector2(Program.Game.Loader.Plus1.Width, Program.Game.Loader.Plus1.Height) * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
                if(level >= 5)
                {
                    Vector2 screenSpot = new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.9f);
                    Vector2 textLength = Program.Game.Loader.Font.MeasureString("Press       to continue");
                    Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Press       to continue", screenSpot, Color.LightGray, 0, textLength * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    if(Input.ControlScheme == ControlScheme.Keyboard)
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey, screenSpot, new Vector2((textLength.X * 0.5f + SymbolWriter.IconCenter.X * 1.5f - Program.Game.Loader.Font.MeasureString("Press ").X), SymbolWriter.IconCenter.Y), true);
                    else
                        SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey, screenSpot, new Vector2((textLength.X * 0.5f + SymbolWriter.IconCenter.X * 1.5f - Program.Game.Loader.Font.MeasureString("Press ").X), SymbolWriter.IconCenter.Y), true);
                }
                Program.Game.SpriteBatch.End();
            }

            public bool Update(GameTime gameTime)
            {
                timer.Update(gameTime);
                if(color.A - 2 <= 0)
                    color.A = 0;
                else
                    color.A -= 2;
                if(level == 5 && (Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey) ||
                    Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey)))
                {
                    RenderingDevice.Remove(box);
                    if(MediaSystem.PlayingVoiceActing)
                        MediaSystem.StopVoiceActing();
                    return true;
                }
                return false;
            }

            private void OnFirstFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Result_Da);
                timer = new Timer(1500, OnSecondFire, false);
                timer.Start();
            }
            private void OnSecondFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Result_Da);
                timer = new Timer(500, OnThirdFire, false);
                timer.Start();
            }
            private void OnThirdFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Result_Da);
                timer = new Timer(500, OnFourthFire, false);
                timer.Start();
            }
            private void OnFourthFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Result_Da);
                timer = new Timer(500, OnFifthFire, false);
                timer.Start();
            }
            private void OnFifthFire()
            {
                level++;
            }

            public void Ready()
            {
                box = new Box(new Vector3(0, 0, 20));
                Random r = new Random();
                box.Ent.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(r.Next(-25, 25))) * 
                    Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(r.Next(-25, 25))) * 
                    Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(r.Next(-25, 25)));
                Program.Game.Camera.SetForResultsScreen();
                Program.Game.Space.Add(entity);
                Program.Game.State = BaseGame.GameState.Results;
                RenderingDevice.Add(box);

                ADVertexFormat[] verts = new ADVertexFormat[4];
                verts[1] = new ADVertexFormat(new Vector3(-20, -20, 0), new Vector2(0, 1), new Vector3(0, 0, 1));
                verts[0] = new ADVertexFormat(new Vector3(-20, 20, 0), new Vector2(0, 0), new Vector3(0, 0, 1));
                verts[2] = new ADVertexFormat(new Vector3(20, 20, 0), new Vector2(1, 1), new Vector3(0, 0, 1));
                verts[3] = new ADVertexFormat(new Vector3(20, -20, 0), new Vector2(1, 0), new Vector3(0, 0, 1));

                buff = new VertexBuffer(Program.Game.GraphicsDevice, ADVertexFormat.VertexDeclaration, 4, BufferUsage.WriteOnly);
                buff.SetData(verts);

                RenderingDevice.Add(buff, Program.Game.Loader.MetalTex);
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Results);

                timer.Start();
            }

            private string convertTimeSpan(TimeSpan t)
            {
                string output = "";
                if(t.Hours > 0)
                    output += t.Hours * 60 + t.Minutes;
                else
                    output += (t.Minutes < 10 ? "0" : "") + t.Minutes;
                output += ':';
                output += (t.Seconds < 10 ? "0" : "") + t.Seconds;

                return output;
            }

            public void RemoveFromGame()
            {
                RenderingDevice.Remove(buff);
                RenderingDevice.RemovePermanent(box);
                Program.Game.Space.Remove(box.Ent);
                Program.Game.Space.Remove(entity);
                if(levelNumber != 0)
                {
                    if(!Program.Game.Manager.CurrentSaveWindowsOptions.HighScoreMode)
                        Program.Game.Manager.CurrentSave.LevelData.Replace(levelNumber, new LevelSelectData(score, lost, time, true, true, completionData));
                    else
                        Program.Game.Manager.CurrentSave.SideBLevelData.Replace(levelNumber, new LevelSelectData(score, lost, time, true, true, completionData));
                }
            }
        }

        protected class Scoreboard
        {
            private readonly Color textRenderColor;
            private readonly int survivingBoxesMax;
            private readonly int remainingBoxesMax;
            private readonly int destroyedBoxesMax;
            private readonly Color fadedRed;
            private readonly Dictionary<string, Vector2> renderPlaces;
            private readonly Dictionary<int, Rectangle> UINumbers;
            private readonly Texture2D texture;
            private readonly List<ScorePopup> popups = new List<ScorePopup>();
            protected readonly int spawnTime;
            protected Rectangle outerSpawnBar;
            protected Rectangle innerSpawnBar;
            protected TimeSpan elapsedTime;

            private int remaining;
            private int surviving = 0, destroyed = 0;
            private int score;
            private Color tintColor = Color.White;
            private int deltaR = -3;
            private float multiplier = 1;

            protected readonly Color outerBarColor = Color.Gray;
            protected readonly Color innerBarColor = new Color(0, 176, 80);

            protected readonly Texture2D barTex;

            protected const int texOffset = 3;
            protected int outerWidth { get { return (int)((barTex.Width - texOffset * 1.5) * Program.Game.TextureScaleFactor.X); } }
            protected int outerHeight { get { return (int)((barTex.Height - texOffset * 1.5) * Program.Game.TextureScaleFactor.Y); } }

            public Scoreboard(Color textColor, int survivingMax, int remainingMax, int spawnTime)
            {
                fadedRed = new Color(255, 64, 64, 255);
                textRenderColor = textColor;
                survivingBoxesMax = survivingMax;
                remainingBoxesMax = remainingMax;
                destroyedBoxesMax = remainingBoxesMax - survivingMax + 1;
                remaining = remainingMax;
                this.spawnTime = spawnTime;

                if(Program.Game.LoadingScreen != null)
                    barTex = Program.Game.LoadingScreen.loader.BarTexture;
                else
                    barTex = Program.Game.Loader.BarTexture;

                outerSpawnBar = new Rectangle(0, 0, outerWidth, outerHeight);
                innerSpawnBar = outerSpawnBar;
                innerSpawnBar.Width = 0;

                Program.Game.GDM.DeviceReset += new EventHandler<EventArgs>(GDM_DeviceReset);

                renderPlaces = new Dictionary<string, Vector2>(5);
                if(Program.Game.Loading)
                {
                    UINumbers = Program.Game.LoadingScreen.loader.UINumbers;
                    texture = Program.Game.LoadingScreen.loader.LCDNumbers;
                    renderPlaces.Add("Surviving", new Vector2(Program.Game.LoadingScreen.loader.SurvivingBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.LoadingScreen.loader.SurvivingBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Remaining", new Vector2(Program.Game.LoadingScreen.loader.RemainingBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.LoadingScreen.loader.RemainingBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Destroyed", new Vector2(Program.Game.LoadingScreen.loader.DestroyedBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.LoadingScreen.loader.DestroyedBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Score", new Vector2(Program.Game.LoadingScreen.loader.ScoreboardBase.Center.X - 17 * Program.Game.TextureScaleFactor.X, Program.Game.LoadingScreen.loader.ScoreboardText.UpperLeft.Y + 13 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Multiplier", new Vector2(renderPlaces["Score"].X + 19 * 5 * Program.Game.TextureScaleFactor.X + 8 * Program.Game.TextureScaleFactor.X, Program.Game.LoadingScreen.loader.ScoreboardText.UpperLeft.Y + 13 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Time", new Vector2(Program.Game.LoadingScreen.loader.TimeElapsedBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.LoadingScreen.loader.TimeElapsedBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                }
                else
                {
                    UINumbers = Program.Game.Loader.UINumbers;
                    texture = Program.Game.Loader.LCDNumbers;
                    renderPlaces.Add("Surviving", new Vector2(Program.Game.Loader.SurvivingBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.SurvivingBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Remaining", new Vector2(Program.Game.Loader.RemainingBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.RemainingBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Destroyed", new Vector2(Program.Game.Loader.DestroyedBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.DestroyedBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Score", new Vector2(Program.Game.Loader.ScoreboardBase.Center.X - 17 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.ScoreboardText.UpperLeft.Y + 13 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Multiplier", new Vector2(renderPlaces["Score"].X + 19 * 5 * Program.Game.TextureScaleFactor.X + 8 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.ScoreboardText.UpperLeft.Y + 13 * Program.Game.TextureScaleFactor.Y));
                    renderPlaces.Add("Time", new Vector2(Program.Game.Loader.TimeElapsedBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.TimeElapsedBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                }
            }

            void GDM_DeviceReset(object sender, EventArgs e)
            {
                Program.Game.Loader.SurvivingBoxesBase.ForceResize();
                Program.Game.Loader.RemainingBoxesBase.ForceResize();
                Program.Game.Loader.DestroyedBoxesBase.ForceResize();
                Program.Game.Loader.ScoreboardBase.ForceResize();
                Program.Game.Loader.TimeElapsedBase.ForceResize();
                renderPlaces.Clear();
                renderPlaces.Add("Surviving", new Vector2(Program.Game.Loader.SurvivingBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.SurvivingBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                renderPlaces.Add("Remaining", new Vector2(Program.Game.Loader.RemainingBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.RemainingBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                renderPlaces.Add("Destroyed", new Vector2(Program.Game.Loader.DestroyedBoxesBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.DestroyedBoxesBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
                renderPlaces.Add("Score", new Vector2(Program.Game.Loader.ScoreboardBase.Center.X - 17 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.ScoreboardText.UpperLeft.Y + 13 * Program.Game.TextureScaleFactor.Y));
                renderPlaces.Add("Multiplier", new Vector2(renderPlaces["Score"].X + 19 * 5 * Program.Game.TextureScaleFactor.X + 8 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.ScoreboardText.UpperLeft.Y + 13 * Program.Game.TextureScaleFactor.Y));
                renderPlaces.Add("Time", new Vector2(Program.Game.Loader.TimeElapsedBase.LowerRight.X - 80 * Program.Game.TextureScaleFactor.X, Program.Game.Loader.TimeElapsedBase.UpperLeft.Y + 15 * Program.Game.TextureScaleFactor.Y));
            }
                
            public void Draw()
            {
                Color col = MediaSystem.SirenPlaying ? tintColor : textRenderColor;
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                drawSpawnBar(Program.Game.CurrentLevel.BoxSpawnPoint, RenderingDevice.Camera.Zoom, Program.Game.GraphicsDevice.Viewport); // needs to be under everything

                // Draw basic elements
                Program.Game.Loader.ScoreboardBase.Draw();
                Program.Game.Loader.ScoreboardText.Draw(textRenderColor);
                Program.Game.Loader.SurvivingBoxesBase.Draw();
                Program.Game.Loader.SurvivingBoxesText.Draw(textRenderColor);
                Program.Game.Loader.RemainingBoxesBase.Draw();
                Program.Game.Loader.RemainingBoxesText.Draw(textRenderColor);
                Program.Game.Loader.TimeElapsedBase.Draw();
                Program.Game.Loader.TimeElapsedText.Draw(textRenderColor);
                Program.Game.Loader.DestroyedBoxesBase.Draw(MediaSystem.SirenPlaying ? col : Color.White);
                Program.Game.Loader.DestroyedBoxesText.Draw(Color.Lerp(textRenderColor, col, 0.5f));

                // Draw scoreboard numbers
                drawScoreboardUI(remaining, remainingBoxesMax, renderPlaces["Remaining"], false, false);

                drawScoreboardUI(destroyed, destroyedBoxesMax, renderPlaces["Destroyed"], true, false);

                drawScoreboardUI(surviving, survivingBoxesMax, renderPlaces["Surviving"], false, false);

                drawScoreboardUI(elapsedTime, renderPlaces["Time"]);

                drawScoreboardUI(score, multiplier);

                foreach(ScorePopup p in popups)
                    p.Draw();

                Program.Game.SpriteBatch.End();
            }

            private void drawSpawnBar(Vector3 spawnPoint, float zoom, Viewport viewport)
            {
                Vector3 projection = viewport.Project(spawnPoint, RenderingDevice.Camera.Projection,
                    RenderingDevice.Camera.View, RenderingDevice.Camera.World);
                float baseZoom = 100;
                float scale = baseZoom / zoom; // baseZoom / zoom gets us a scale that increases when zoom is low; that is; zoomed in.
                float offset = 15;
                Vector2 screenCoords = new Vector2(projection.X, projection.Y);

                screenCoords.X += offset * Program.Game.TextureScaleFactor.X;
                screenCoords.Y -= offset * Program.Game.TextureScaleFactor.Y;

                outerSpawnBar.X = innerSpawnBar.X = (int)(screenCoords.X + texOffset * Program.Game.TextureScaleFactor.X * scale);
                outerSpawnBar.Y = innerSpawnBar.Y = (int)(screenCoords.Y + texOffset * Program.Game.TextureScaleFactor.Y * scale);

                Program.Game.SpriteBatch.Draw(Program.Game.Loader.EmptyTex, outerSpawnBar, null, outerBarColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                if(remaining > 0)
                    Program.Game.SpriteBatch.Draw(Program.Game.Loader.EmptyTex, innerSpawnBar, null, innerBarColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                Program.Game.SpriteBatch.Draw(barTex, screenCoords, null, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor * scale, SpriteEffects.None, 0);
            }

            /// <summary>
            /// Updates the scoreboard with new values. In all technicality, you wouldn't have to do this unless the values changed.
            /// </summary>
            /// <param name="surviving">The number of currently surviving boxes.</param>
            /// <param name="destroyed">The number of boxes currently lost.</param>
            /// <param name="remaining">The number of boxes currently remaining.</param>
            /// <param name="score">The player's score.</param>
            public void Update(int surviving, int destroyed, int remaining, int score, float multiplier,
                int spawnTimer, TimeSpan elapsedTime)
            {
                this.surviving = surviving;
                this.destroyed = destroyed;
                this.remaining = remaining;
                int scoreLastFrame = this.score;
                this.score = score;
                this.multiplier = multiplier;
                this.elapsedTime = elapsedTime;

                float scale = (float)spawnTimer / spawnTime;
                if(scale > 1)
                    scale = 1;
                outerSpawnBar.Width = (int)(outerWidth * (100 / RenderingDevice.Camera.Zoom));
                innerSpawnBar.Width = (int)(outerSpawnBar.Width * scale);
                outerSpawnBar.Height = innerSpawnBar.Height = (int)(outerHeight * (100 / RenderingDevice.Camera.Zoom));

                if(scoreLastFrame != this.score)
                    popups.Add(new ScorePopup(renderPlaces["Score"], this.score - scoreLastFrame, Program.Game.SpriteBatch));

                for(int i = 0; i < popups.Count; i++)
                    if(popups[i].IsDone)
                    {
                        popups.RemoveAt(i);
                        i--;
                    }

                if(MediaSystem.SirenPlaying)
                {
                    if((tintColor.B + deltaR > 210 && !(tintColor.B > 210)) || tintColor.B + deltaR < 0)
                        deltaR = -deltaR;
                    if(deltaR < 0)
                    {
                        tintColor.B -= (byte)-deltaR;
                        tintColor.G -= (byte)-deltaR;
                    }
                    else
                    {
                        tintColor.B += (byte)deltaR;
                        tintColor.G += (byte)deltaR;
                    }
                }
                else
                    tintColor = Color.White;
            }

            public void Reset()
            {
                popups.Clear();
                score = 0;
            }

            /// <summary>
            /// A function to draw a scoreboard string at a given location. Start SpriteBatch first!
            /// </summary>
            /// <param name="number1">The number to draw on the left side of the slash.</param>
            /// <param name="number2">The number to draw on the right side of the slash.</param>
            /// <param name="position">The position to draw at (upper-left based).</param>
            private void drawScoreboardUI(int number1, int number2, Vector2 position, bool withTint, bool useColon)
            {
                Color col = withTint && MediaSystem.SirenPlaying ? tintColor : Color.White;
                if(number1 < 10)
                {
                    Program.Game.SpriteBatch.Draw(texture, position, UINumbers[10], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(position.X + 13 * Program.Game.TextureScaleFactor.X, position.Y), UINumbers[number1], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }
                else if(number1 > 9)
                {
                    int firstDigit, secondDigit;
                    firstDigit = number1 / 10;
                    secondDigit = number1 % 10;
                    Program.Game.SpriteBatch.Draw(texture, position, UINumbers[firstDigit], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(position.X + 13 * Program.Game.TextureScaleFactor.X, position.Y), UINumbers[secondDigit], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }

                Program.Game.SpriteBatch.Draw(texture, new Vector2(position.X + 26 * Program.Game.TextureScaleFactor.X, position.Y), UINumbers[useColon ? 14 : 11], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                if(number2 < 10)
                {
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(position.X + 39 * Program.Game.TextureScaleFactor.X, position.Y), UINumbers[useColon ? 0 : 10], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(position.X + 52 * Program.Game.TextureScaleFactor.X, position.Y), UINumbers[number2], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }
                else if(number2 > 9)
                {
                    int firstDigit, secondDigit;
                    firstDigit = number2 / 10;
                    secondDigit = number2 % 10;
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(position.X + 39 * Program.Game.TextureScaleFactor.X, position.Y), UINumbers[firstDigit], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(position.X + 52 * Program.Game.TextureScaleFactor.X, position.Y), UINumbers[secondDigit], col, 0.0f, Vector2.Zero, 0.9f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }
            }

            /// <summary>
            /// A function to draw the Score element of the scoreboard. Start SpriteBatch first!
            /// </summary>
            /// <param name="score">The score to draw the scoreboard with.</param>
            private void drawScoreboardUI(int score, float multiplier)
            {
                int i = 0;
                int[] digits = new int[5];
                int temp = score;
                while(temp > 0) // extracting digits
                {
                    digits[i++] = temp % 10;
                    temp /= 10;
                }
                i = 0;
                int j = digits.Length - 1;
                while(i < j) // reversing digits
                {
                    temp = digits[i];
                    digits[i] = digits[j];
                    digits[j] = temp;
                    i++; j--;
                }
                for(i = 0; i < 4; i++) // replacing leading zeros with empties
                    if(digits[i] == 0)
                        digits[i] = 10;
                    else
                        break;
                for(i = 0; i < 5; i++) // drawing score
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Score"].X + (i * 15 * Program.Game.TextureScaleFactor.X), renderPlaces["Score"].Y), UINumbers[digits[i]], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                // draw multiplier
                float offset = 0;
                if(multiplier == 10)
                {
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[1], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * Program.Game.TextureScaleFactor.X;
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[0], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * Program.Game.TextureScaleFactor.X;
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[13], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 8 * Program.Game.TextureScaleFactor.X;
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[0], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * Program.Game.TextureScaleFactor.X;
                }
                else
                {
                    int whole = (int)multiplier;
                    int part = (int)((multiplier - whole) * 10);
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[10], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * Program.Game.TextureScaleFactor.X;
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[whole], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * Program.Game.TextureScaleFactor.X;
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[13], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 8 * Program.Game.TextureScaleFactor.X;
                    Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[part], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * Program.Game.TextureScaleFactor.X;
                }
                Program.Game.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[12], Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            }

            private void drawScoreboardUI(TimeSpan timeElapsed, Vector2 pos)
            {
                int minutes = timeElapsed.Minutes + timeElapsed.Hours * 60;
                int seconds = timeElapsed.Seconds;
                if(minutes > 99)
                {
                    minutes = 99;
                    seconds = 59;
                }

                drawScoreboardUI(minutes, seconds, pos, true, true);
            }

            private sealed class ScorePopup
            {
                private readonly int value;
                private readonly int digits;
                private readonly SpriteBatch batch;
                private readonly SpriteFont font;

                private int alpha = 255;
                private Vector2 pos;

                private readonly Vector2 speed = new Vector2(0, -1f);
                private readonly float targetY = Program.Game.Loader.ScoreboardBase.LowerRight.Y + 22 * Program.Game.TextureScaleFactor.X;
                private const byte deltaA = 3;

                public bool IsDone { get; private set; }

                /// <summary>
                /// Creates a score thing that looks nice.
                /// </summary>
                /// <param name="initialPos">Pass renderPlaces["Score"] into this.</param>
                /// <param name="value">The value to add.</param>
                /// <param name="b">A SpriteBatch that will be started and ended outside this class's Draw().</param>
                public ScorePopup(Vector2 initialPos, int value, SpriteBatch b)
                {
                    batch = b;
                    this.value = value;
                    font = Program.Game.Loader.LCDFont;
                    this.pos = initialPos;
                    int i = 0;
                    int temp = value;
                    while(temp > 0)
                    {
                        i++;
                        temp /= 10;
                    }
                    digits = i;
                    Program.Game.Space.DuringForcesUpdateables.Starting += updateVelocities;
                }

                private void updateVelocities()
                {
                    pos -= speed;
                    if(alpha - deltaA < 0)
                        alpha = 0;
                    else
                        alpha -= deltaA;

                    if(pos.Y >= targetY)
                    {
                        Program.Game.Space.DuringForcesUpdateables.Starting -= updateVelocities;
                        IsDone = true;
                    }
                }

                public void Draw()
                {
                    float offset = 5 * 15 * Program.Game.TextureScaleFactor.X;
                    string s = (value > 0 ? "+" : "") + value.ToString();
                    batch.DrawString(font, s, new Vector2(pos.X + offset, pos.Y), new Color(value > 0 ? 0 : 255, 0, value > 0 ? 255 : 0) * (alpha / 255f), 0, new Vector2(font.MeasureString(s).X, 0), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }
            }
        }
        #endregion
    }
}
