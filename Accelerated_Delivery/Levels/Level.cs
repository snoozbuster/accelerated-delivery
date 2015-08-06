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
    public delegate Effect EffectDelegate();

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

        public SongOptions LevelSong { get { return levelTheme.Song; } }

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

        protected Goal normalCatcher;
        protected readonly Goal coloredCatcher;

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

        //protected static Effect effect { get { return effectDelegate(); } }
        protected static Texture2D[] billboardList;
        protected static Texture2D[] activeBillboardList;
        protected static ModelDelegate Dispenser;
        protected static EffectDelegate effectDelegate;

        protected static event Action gdmReset;

        public static void Initialize(EffectDelegate eff, Texture2D[] bblist, Texture2D[] activebblist, ModelDelegate disp)
        {
            effectDelegate = eff;
            billboardList = bblist;
            activeBillboardList = activebblist;
            Dispenser = disp;
        }

        public static void OnGDMReset()
        {
            gdmReset();
        }

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
            Theme levelTheme, BaseModel levelModel, BaseModel[] glassModels, Goal normalCatcher, Goal coloredCatcher,
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
            Board = new Billboard(effectDelegate);

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
                textureList[i] = billboardList[i];
                activeList[i] = activeBillboardList[i];
            }

            dispenser = new BaseModel(Dispenser, false, null, BoxSpawnPoint);
            dispenser.Ent.BecomeKinematic();

            //if(levelNumber != 0)
            //    Program.Game.Manager.CurrentSave.LevelData.SetCompletionData(levelNumber, data);

            gdmReset += onGDMReset;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(Input.KeyboardState.IsKeyDown(Keys.LeftControl) && Input.CheckKeyboardJustPressed(Keys.F1))
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
#if DEMO
                            if(levelNumber != 4)
#endif
                            MediaSystem.PlayVoiceActing(levelNumber);
                    }
                    else if(overlay.Completed && badEnding)
                    {
                        GameManager.State = GameState.GameOver;
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
                    if(boxesOnscreen[i].NeedsRemoval && boxesOnscreen[i].Kill)
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
                        Input.CheckKeyboardJustPressed(Input.WindowsOptions.QuickBoxKey) ||
                        Input.CheckXboxJustPressed(Input.XboxOptions.QuickBoxKey)) && BoxesRemaining > 0)
                        spawnBox();

                    if(BoxesDestroyed * 1.3 >= boxesMax - boxesNeeded && !MediaSystem.SirenPlaying && levelNumber != 0 && levelNumber != 11)
                        MediaSystem.PlaySoundEffect(SFXOptions.Siren);
                    else if(BoxesDestroyed > boxesMax - boxesNeeded && !Input.WindowsOptions.HighScoreMode)
                    {
                        badEnding = true;
                        overlay = new OpeningOverlay(false);
                        MediaSystem.PlaySoundEffect(SFXOptions.Fail);
                        MediaSystem.StopSiren();
                    }
                    else if(boxesSaved >= boxesNeeded && !Input.WindowsOptions.HighScoreMode)
                        endLevel();
                    else if(BoxesRemaining == 0 && boxesOnscreen.Count == 0 && Input.WindowsOptions.HighScoreMode)
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
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                RenderingDevice.SpriteBatch.Draw(Resources.EmptyTex, screenSpace, color);
                RenderingDevice.SpriteBatch.End();
            }
        }

        #region add/remove from game
        public virtual void AddToGame(Space s)
        {
            TemporarilyMuteVoice = false;

            MediaSystem.PlayTrack(levelTheme.Song);
            overlay = new OpeningOverlay(levelNumber < 12 ? levelNumber : levelNumber - 11, levelNumber > 11, levelName);
            time = new TimeSpan();
            RebuildTiming();

            foreach(OperationalMachine m in MachineList.Keys)
                s.Add(m);
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
            levelTheme.SetUpLighting();
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

            normalCatcher.AddToGame(GameManager.Space);
            if(coloredCatcher != null)
                coloredCatcher.AddToGame(GameManager.Space);
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
            if(results == null)
                MediaSystem.StopVoiceActing();
            MediaSystem.LevelReset();
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
            //switch(Input.WindowsOptions.DifficultyLevel)
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
                MediaSystem.PlaySoundEffect(SFXOptions.Box_Death);
            }

            boxesOnscreen.Remove(b);
            RenderingDevice.RemovePermanent(b);
            GameManager.Space.Remove(b);
        }

        public void CatchBox(Box b)
        {
            if(b is BlackBox)
            {
                (b as BlackBox).Remove();
                if(!Ending)
                {
                    MediaSystem.PlaySoundEffect(SFXOptions.Box_Death);
                    boxesSaved--;
                    score -= baseBoxValue;
                    multiplier = multiplierStages[0];
                    index = 0;
                    BoxesDestroyed++;
                }
            }
            if(!Ending)
            {
                MediaSystem.PlaySoundEffect(SFXOptions.Box_Success);
                boxesSaved++;
                score += (int)(baseBoxValue * multiplier + b.ExtraPoints);
                if(index + 1 == multiplierStages.Length)
                    multiplier = multiplierStages[index];
                else
                    multiplier = multiplierStages[++index];
            }
            
            boxesOnscreen.Remove(b);
            RenderingDevice.RemovePermanent(b);
            GameManager.Space.Remove(b);
        }

        public void VanishBox(BlackBox b, bool destroyed)
        {
            if(b == null)
            {
                GameManager.Cutter.WriteToLog(this, "Warning: VanishBox() called with a null parameter.");
                return;
            }

            boxesOnscreen.Remove(b);
            RenderingDevice.RemovePermanent(b);
            b.Ent.Space.Remove(b);
            b.Remove();
            if(destroyed)
            {
                MediaSystem.PlaySoundEffect(SFXOptions.Box_Success);
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
                if(counter < deltaBox && random.Next(0, 2) == 0)
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
            MediaSystem.PlaySoundEffect(SFXOptions.Win);
            overlay = new OpeningOverlay(true);
            results = new ResultsScreen(time, BoxesDestroyed, score, levelNumber, CompletionData);
            ending = true;
            color = new Color(0, 0, 0, 0);
            timer = new Timer(2500, onTimerFired, false);
            timer.Start();
            TemporarilyMuteVoice = false;
            screenSpace = new Rectangle(0, 0, (int)RenderingDevice.Width, (int)RenderingDevice.Height);
        }

        protected void releaseBoxes()
        {
            foreach(Box b in boxesOnscreen)
            {
                if(b is BlackBox)
                    (b as BlackBox).Remove();
                RenderingDevice.RemovePermanent(b);
            }
            for(int i = 0; i < GameManager.Space.Entities.Count; i++)
                if(GameManager.Space.Entities[i].Tag is Box)
                    GameManager.Space.Remove(GameManager.Space.Entities[i--]);
            boxesOnscreen.Clear();
        }

        protected virtual void onTimerFired()
        {
            RemoveFromGame(GameManager.Space);
            drawResults = true;
            results.Ready();

            foreach(OperationalMachine m in MachineList.Keys)
                m.ResetMachine();

            MediaSystem.EndingLevel();
        }

        protected virtual void loadNextLevel()
        {
            results.RemoveFromGame();
            MediaSystem.StopVoiceActing();
            if(GameManager.LevelEnteredFrom == GameState.Running && GameManager.LevelNumber < 10 && GameManager.LevelNumber != 0)
            {
                GameManager.State = GameState.Running;
                GameManager.LevelNumber++;
                GameManager.Manager.CurrentSave.LevelData[GameManager.LevelNumber].Unlocked = true;
            }
            else if(GameManager.LevelNumber == 10)
            {
                GameManager.LevelNumber = -1;
                GameManager.State = GameState.Ending;
                MediaSystem.PlayTrack(SongOptions.Credits);
                MediaSystem.StopSFX();
            }
            else
            {
                if(levelNumber != 10 && levelNumber != 11 && levelNumber != 14)
                    GameManager.Manager.CurrentSave.LevelData[GameManager.LevelNumber + 1].Unlocked = true;
                GameManager.LevelNumber = -1;
                GameManager.State = GameManager.LevelEnteredFrom == GameState.Menuing_Lev ? GameState.Menuing_Lev : GameState.MainMenu;
                MediaSystem.PlayTrack(SongOptions.Menu);
            }
            drawResults = false;
            GameManager.Manager.Save(true);
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
            if(pressedKeys.Length > 1 && pressedKeys[0] == Keys.None) // then something weird is going on and it needs to be fixed
            {
                Keys[] temp = new Keys[pressedKeys.Length - 1];
                for(int i = 1; i < pressedKeys.Length; i++)
                    temp[i - 1] = pressedKeys[i];
                pressedKeys = temp;
            }

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
                GameManager.Cutter.WriteToLog(this, "Box unnaturally spawned at: " + location.ToString());
            }
        }

        protected void drawDebugJunk()
        {
            if(spawnlevel > 0 && spawnlevel < 4)
            {
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Enter a location: ", new Vector2(RenderingDevice.Height - 40, 60), Color.GhostWhite);
                RenderingDevice.SpriteBatch.DrawString(Resources.Font, "X: " + (isNegative && spawnlevel == 1 ? "-" : "") + Math.Abs(location.X), new Vector2(RenderingDevice.Height - 40, 80), spawnlevel == 1 ? Color.BlueViolet : Color.GhostWhite);
                RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Y: " + (isNegative && spawnlevel == 2 ? "-" : "") + Math.Abs(location.Y), new Vector2(RenderingDevice.Height - 40, 100), spawnlevel == 2 ? Color.BlueViolet : Color.GhostWhite);
                RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Z: " + (isNegative && spawnlevel == 3 ? "-" : "") + Math.Abs(location.Z), new Vector2(RenderingDevice.Height - 40, 120), spawnlevel == 3 ? Color.BlueViolet : Color.GhostWhite);
                RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Tab to cancel.", new Vector2(RenderingDevice.Height - 40, 140), Color.GhostWhite);
                RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Backspace to clear.", new Vector2(RenderingDevice.Height - 40, 160), Color.GhostWhite);
                RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Right Shift to use last location.", new Vector2(RenderingDevice.Height - 40, 180), Color.GhostWhite);
                RenderingDevice.SpriteBatch.End();
            }
        }
#endif
        #endregion

        #region helper classes
        protected class OpeningOverlay
        {
            protected Sprite levelText;
            protected Sprite numberWord;
            protected Sprite demoText;
            protected int stage = 1;
            protected bool failure = false;
            protected bool ending;
            protected string levelName;
            protected SpriteFont font { get { return Resources.BiggerFont; } }

            protected Vector2 stringPos;
            protected Vector2 stringSpeed;

            public bool Completed { get; protected set; }

            public OpeningOverlay(int level, bool demo, string name)
            {
                if(level > 11 || level < 0)
                    throw new ArgumentException(level + " is not a valid level number.");
                Completed = false;
                if(demo)
                    demoText = Resources.OverlayWords[12];
                levelText = Resources.LevelOverlay;
                numberWord = Resources.OverlayWords[level];

                levelName = "\"" + name + "\'\'";

                stringPos = new Vector2(numberWord.Center.X + 20 * RenderingDevice.TextureScaleFactor.X, numberWord.LowerRight.Y + 10 * RenderingDevice.TextureScaleFactor.Y);

                levelText.Reset();
                numberWord.Reset();
                if(demoText != null)
                    demoText.Reset();

                levelText.MoveTo(new Vector2(RenderingDevice.Width * 0.45f, levelText.Center.Y), 1.1f);
                stringSpeed = (new Vector2(RenderingDevice.Width * 0.55f, stringPos.Y) - stringPos) * GameManager.Space.TimeStepSettings.TimeStepDuration / 1.1f;
                if(demoText != null)
                    demoText.MoveTo(new Vector2(RenderingDevice.Width * 0.45f, demoText.Center.Y), 1.1f);
                numberWord.MoveTo(new Vector2(RenderingDevice.Width * 0.55f, numberWord.Center.Y), 1.1f);
            }

            /// <summary>
            /// Creates an overlay that handles flying text. True displays "completed" whereas false displays "failed."
            /// </summary>
            /// <param name="completed"></param>
            public OpeningOverlay(bool completed)
            {
                Completed = false;
                ending = true;
                levelText = Resources.LevelOverlay;
                if(completed)
                    numberWord = Resources.OverlayWords[14];
                else
                    numberWord = Resources.OverlayWords[13];
                levelText.Reset();
                numberWord.Reset();
                failure = !completed;
                levelText.MoveTo(new Vector2(RenderingDevice.Width * 0.45f, levelText.Center.Y), 1.1f);
                numberWord.MoveTo(new Vector2(RenderingDevice.Width * 0.65f, numberWord.Center.Y), 1.1f);
            }

            public void Update(GameTime gameTime)
            {
                stringPos += stringSpeed;
                switch(stage)
                {
                    case 1:
                        if(!levelText.Moving)
                        {
                            levelText.MoveTo(new Vector2(RenderingDevice.Width * 0.49f, levelText.Center.Y), 3);
                            if(demoText != null)
                                demoText.MoveTo(new Vector2(RenderingDevice.Width * 0.49f, demoText.Center.Y), 3);
                            if(!ending || (ending && failure))
                            {
                                numberWord.MoveTo(new Vector2(RenderingDevice.Width * 0.51f, numberWord.Center.Y), 3);
                                stringSpeed = (new Vector2(RenderingDevice.Width * 0.51f, stringPos.Y) - stringPos) * GameManager.Space.TimeStepSettings.TimeStepDuration / 2.6f;
                            }
                            else if(ending && !failure)
                            {
                                numberWord.MoveTo(new Vector2(RenderingDevice.Width * 0.58f, numberWord.Center.Y), 3);
                                stringSpeed = (new Vector2(RenderingDevice.Width * 0.58f, stringPos.Y) - stringPos) * GameManager.Space.TimeStepSettings.TimeStepDuration / 2.6f;
                            }
                            stage++;
                        }
                        break;
                    case 2:
                        if(!levelText.Moving && !failure)
                        {
                            levelText.MoveTo(new Vector2(RenderingDevice.Width + levelText.Width * 0.5f, levelText.Center.Y), 1.1f);
                            if(demoText != null)
                                demoText.MoveTo(new Vector2(RenderingDevice.Width + demoText.Width * 0.5f, demoText.Center.Y), 1.1f);
                            numberWord.MoveTo(new Vector2(-numberWord.Center.X, numberWord.Center.Y), 1.1f);
                            if(!ending)
                                stringSpeed = (new Vector2(-font.MeasureString(levelName).X, stringPos.Y) - stringPos) * GameManager.Space.TimeStepSettings.TimeStepDuration / 0.8f;
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
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                if(GameManager.State == GameState.GameOver)
                    RenderingDevice.SpriteBatch.Draw(Resources.halfBlack, new Rectangle(0, 0, (int)RenderingDevice.Width, (int)RenderingDevice.Height), Color.White);
                if(font != null && levelName != "\"\'\'" && levelName != null)
                    RenderingDevice.SpriteBatch.DrawString(font, levelName, stringPos, Color.Gainsboro, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                levelText.Draw();
                numberWord.Draw();
                if(demoText != null)
                    demoText.Draw();
                RenderingDevice.SpriteBatch.End();
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
                //plane = Resources.resultsPlane;
                timer = new Timer(3000, OnFirstFire, false);
                entity = new BEPUphysics.Entities.Prefabs.Box(Vector3.Zero, 20, 20, 1);
                //transforms = new Matrix[plane.Bones.Count];
                screenSpace = new Rectangle(0, 0, (int)RenderingDevice.Width, (int)RenderingDevice.Height);
                color = new Color(0, 0, 0, 255);
            }
            
            public void Draw()
            {
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                if(level == 0)
                    RenderingDevice.SpriteBatch.Draw(Resources.EmptyTex, screenSpace, color);
                if(level >= 1)
                    RenderingDevice.SpriteBatch.DrawString(Resources.BiggerFont, "Results: Level " + (levelNumber > 11 ? "D" + (levelNumber - 11) : levelNumber.ToString()), new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height * 0.15f), Color.White, 0, Resources.BiggerFont.MeasureString("Results: Level " + (levelNumber > 11 ? "D" + (levelNumber - 11) : levelNumber.ToString())) * 0.5f, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                if(level >= 2)
                {
                    if(levelNumber != 11)
                    {
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Time Taken:", new Vector2(RenderingDevice.Width * 0.25f, RenderingDevice.Height * 0.4f), Color.White, 0, new Vector2(Resources.Font.MeasureString("Time Taken:").X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Boxes Lost:", new Vector2(RenderingDevice.Width * 0.25f, RenderingDevice.Height * 0.55f), Color.White, 0, new Vector2(Resources.Font.MeasureString("Boxes Lost:").X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Score:", new Vector2(RenderingDevice.Width * 0.25f, RenderingDevice.Height * 0.7f), Color.White, 0, new Vector2(Resources.Font.MeasureString("Score:").X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                    else
                    {
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Completed:", new Vector2(RenderingDevice.Width * 0.25f, RenderingDevice.Height * 0.5f), Color.White, 0, new Vector2(Resources.Font.MeasureString("Completed:").X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Time Taken:", new Vector2(RenderingDevice.Width * 0.25f, RenderingDevice.Height * 0.65f), Color.White, 0, new Vector2(Resources.Font.MeasureString("Time Taken:").X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
                if(level >= 3)
                {
                    string t = convertTimeSpan(time);
                    if(levelNumber != 11)
                    {
                        float constant = Input.WindowsOptions.HighScoreMode ? 0.65f : 0.4f;
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, t, new Vector2(RenderingDevice.Width * constant, RenderingDevice.Height * 0.4f), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, lost.ToString(), new Vector2(RenderingDevice.Width * constant, RenderingDevice.Height * 0.55f), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, score.ToString(), new Vector2(RenderingDevice.Width * constant, RenderingDevice.Height * 0.7f), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        if(!Input.WindowsOptions.HighScoreMode)
                        {
                            RenderingDevice.SpriteBatch.DrawString(Resources.Font, "(target: " + convertTimeSpan(completionData.ThreeStarTime) + ")", new Vector2(RenderingDevice.Width * 0.45f, RenderingDevice.Height * 0.4f), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                            RenderingDevice.SpriteBatch.DrawString(Resources.Font, "(max allowed: " + completionData.ThreeStarBoxes + ")", new Vector2(RenderingDevice.Width * 0.45f, RenderingDevice.Height * 0.55f), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                            RenderingDevice.SpriteBatch.DrawString(Resources.Font, "(target: " + completionData.ThreeStarScore + ")", new Vector2(RenderingDevice.Width * 0.45f, RenderingDevice.Height * 0.7f), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
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
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Yes", new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height * 0.5f), Color.White, 0, new Vector2(Resources.Font.MeasureString("Yes").X * 0.5f * RenderingDevice.TextureScaleFactor.X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        RenderingDevice.SpriteBatch.DrawString(Resources.Font, t, new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height * 0.65f), Color.White, 0, new Vector2(Resources.Font.MeasureString(t).X * 0.5f * RenderingDevice.TextureScaleFactor.X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
                if(level >= 4 && (!Input.WindowsOptions.HighScoreMode || levelNumber == 11))
                {
                    Rectangle r = new Rectangle(39, 36, 39, 36);
                    if(levelNumber != 11)
                    {
                        if(time <= completionData.ThreeStarTime)
                        {
                            RenderingDevice.SpriteBatch.Draw(Resources.starTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.4f), r, Color.White, 0, new Vector2(r.Width * 0.5f, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                            if(GameManager.Manager.CurrentSave.LevelData[levelNumber].TimeStarNumber != LevelSelectData.Stars.Three)
                                RenderingDevice.SpriteBatch.Draw(Resources.Plus1, new Vector2(RenderingDevice.Width * 0.75f + r.Width * 0.5f * RenderingDevice.TextureScaleFactor.X, RenderingDevice.Height * 0.4f), null, Color.White, 0, new Vector2(Resources.Plus1.Width, Resources.Plus1.Height) * 0.5f, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                            RenderingDevice.SpriteBatch.Draw(Resources.starTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.4f), new Rectangle(0, 0, r.Width, r.Height), Color.White, 0, new Vector2(r.Width * 0.5f, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);

                        if(lost <= completionData.ThreeStarBoxes)
                        {
                            RenderingDevice.SpriteBatch.Draw(Resources.starTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.55f), r, Color.White, 0, new Vector2(r.Width * 0.5f, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                            if(GameManager.Manager.CurrentSave.LevelData[levelNumber].BoxStarNumber != LevelSelectData.Stars.Three)
                                RenderingDevice.SpriteBatch.Draw(Resources.Plus1, new Vector2(RenderingDevice.Width * 0.75f + r.Width * 0.5f * RenderingDevice.TextureScaleFactor.X, RenderingDevice.Height * 0.55f), null, Color.White, 0, new Vector2(Resources.Plus1.Width, Resources.Plus1.Height) * 0.5f, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                            RenderingDevice.SpriteBatch.Draw(Resources.starTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.55f), new Rectangle(0, 0, r.Width, r.Height), Color.White, 0, new Vector2(r.Width * 0.5f, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);

                        if(score >= completionData.ThreeStarScore)
                        {
                            RenderingDevice.SpriteBatch.Draw(Resources.starTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.7f), r, Color.White, 0, new Vector2(r.Width * 0.5f, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                            if(GameManager.Manager.CurrentSave.LevelData[levelNumber].ScoreStarNumber != LevelSelectData.Stars.Three)
                                RenderingDevice.SpriteBatch.Draw(Resources.Plus1, new Vector2(RenderingDevice.Width * 0.75f + r.Width * 0.5f * RenderingDevice.TextureScaleFactor.X, RenderingDevice.Height * 0.7f), null, Color.White, 0, new Vector2(Resources.Plus1.Width, Resources.Plus1.Height) * 0.5f, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                            RenderingDevice.SpriteBatch.Draw(Resources.starTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.7f), new Rectangle(0, 0, r.Width, r.Height), Color.White, 0, new Vector2(r.Width * 0.5f, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                    else
                    {
                        RenderingDevice.SpriteBatch.Draw(Resources.starTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.5f), r, Color.White, 0, new Vector2(r.Width * 0.5f, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                        if(!GameManager.Manager.CurrentSave.LevelData[levelNumber].Completed)
                            RenderingDevice.SpriteBatch.Draw(Resources.Plus1, new Vector2(RenderingDevice.Width * 0.75f + r.Width * 0.5f * RenderingDevice.TextureScaleFactor.X, RenderingDevice.Height * 0.5f), null, Color.White, 0, new Vector2(Resources.Plus1.Width, Resources.Plus1.Height) * 0.5f, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
                if(level >= 5)
                {
                    Vector2 screenSpot = new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height * 0.9f);
                    Vector2 textLength = Resources.Font.MeasureString("Press       to continue");
                    RenderingDevice.SpriteBatch.DrawString(Resources.Font, "Press       to continue", screenSpot, Color.LightGray, 0, textLength * 0.5f, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    if(Input.ControlScheme == ControlScheme.Keyboard)
                        SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.SelectionKey, screenSpot, new Vector2((textLength.X * 0.5f + SymbolWriter.IconCenter.X * 1.5f - Resources.Font.MeasureString("Press ").X), SymbolWriter.IconCenter.Y), true);
                    else
                        SymbolWriter.WriteXboxIcon(Input.XboxOptions.SelectionKey, screenSpot, new Vector2((textLength.X * 0.5f + SymbolWriter.IconCenter.X * 1.5f - Resources.Font.MeasureString("Press ").X), SymbolWriter.IconCenter.Y), true);
                }
                RenderingDevice.SpriteBatch.End();
            }

            public bool Update(GameTime gameTime)
            {
                timer.Update(gameTime);
                if(color.A - 2 <= 0)
                    color.A = 0;
                else
                    color.A -= 2;
                if(level == 5 && (Input.CheckKeyboardJustPressed(Input.WindowsOptions.SelectionKey) ||
                    Input.CheckXboxJustPressed(Input.XboxOptions.SelectionKey)))
                {
                    RenderingDevice.Remove(box);
                    //if(MediaSystem.PlayingVoiceActing)
                    //    MediaSystem.StopVoiceActing();
                    return true;
                }
                return false;
            }

            private void OnFirstFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(SFXOptions.Result_Da);
                timer = new Timer(1500, OnSecondFire, false);
                timer.Start();
            }
            private void OnSecondFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(SFXOptions.Result_Da);
                timer = new Timer(500, OnThirdFire, false);
                timer.Start();
            }
            private void OnThirdFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(SFXOptions.Result_Da);
                timer = new Timer(500, OnFourthFire, false);
                timer.Start();
            }
            private void OnFourthFire()
            {
                level++;
                MediaSystem.PlaySoundEffect(SFXOptions.Result_Da);
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
                RenderingDevice.Camera.SetForResultsScreen();
                GameManager.Space.Add(entity);
                GameManager.State = GameState.Results;
                RenderingDevice.Add(box);

                ADVertexFormat[] verts = new ADVertexFormat[4];
                float halfWidth = 80;
                verts[1] = new ADVertexFormat(new Vector3(-halfWidth, -halfWidth, 0), new Vector2(0, 0), new Vector3(0, 0, 1));
                verts[0] = new ADVertexFormat(new Vector3(-halfWidth, halfWidth, 0), new Vector2(0, 1), new Vector3(0, 0, 1));
                verts[2] = new ADVertexFormat(new Vector3(halfWidth, halfWidth, 0), new Vector2(1, 0), new Vector3(0, 0, 1));
                verts[3] = new ADVertexFormat(new Vector3(halfWidth, -halfWidth, 0), new Vector2(1, 1), new Vector3(0, 0, 1));

                buff = new VertexBuffer(RenderingDevice.GraphicsDevice, ADVertexFormat.VertexDeclaration, 4, BufferUsage.WriteOnly);
                buff.SetData(verts);

                RenderingDevice.Add(buff, Resources.MetalTex);
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
                GameManager.Space.Remove(box.Ent);
                GameManager.Space.Remove(entity);
                if(levelNumber != 0)
                {
                    if(!Input.WindowsOptions.HighScoreMode)
                        GameManager.Manager.CurrentSave.LevelData.Replace(levelNumber, new LevelSelectData(score, lost, time, true, true, completionData));
                    else
                        GameManager.Manager.CurrentSave.SideBLevelData.Replace(levelNumber, new LevelSelectData(score, lost, time, true, true, completionData));
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
            private Texture2D texture { get { return Resources.LCDNumbers; } }
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

            protected Texture2D barTex { get { return Resources.BarTexture; } }

            protected const int texOffset = 3;
            protected int outerWidth { get { return (int)((barTex.Width - texOffset * 1.5) * RenderingDevice.TextureScaleFactor.X); } }
            protected int outerHeight { get { return (int)((barTex.Height - texOffset * 1.5) * RenderingDevice.TextureScaleFactor.Y); } }

            protected bool OneBox { get { return remainingBoxesMax == 1; } }

            public Scoreboard(Color textColor, int survivingMax, int remainingMax, int spawnTime)
            {
                fadedRed = new Color(255, 64, 64, 255);
                textRenderColor = textColor;
                survivingBoxesMax = survivingMax;
                remainingBoxesMax = remainingMax;
                destroyedBoxesMax = remainingBoxesMax - survivingMax + 1;
                remaining = remainingMax;
                this.spawnTime = spawnTime;

                outerSpawnBar = new Rectangle(0, 0, outerWidth, outerHeight);
                innerSpawnBar = outerSpawnBar;
                innerSpawnBar.Width = 0;

                RenderingDevice.GDM.DeviceReset += new EventHandler<EventArgs>(GDM_DeviceReset);

                renderPlaces = new Dictionary<string, Vector2>(5);
                UINumbers = Resources.UINumbers;
                renderPlaces.Add("Surviving", new Vector2(Resources.SurvivingBoxesBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.SurvivingBoxesBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Remaining", new Vector2(Resources.RemainingBoxesBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.RemainingBoxesBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Destroyed", new Vector2(Resources.DestroyedBoxesBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.DestroyedBoxesBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Score", new Vector2(Resources.ScoreboardBase.Center.X - 17 * RenderingDevice.TextureScaleFactor.X, Resources.ScoreboardText.UpperLeft.Y + 13 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Multiplier", new Vector2(renderPlaces["Score"].X + 19 * 5 * RenderingDevice.TextureScaleFactor.X + 8 * RenderingDevice.TextureScaleFactor.X, Resources.ScoreboardText.UpperLeft.Y + 13 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Time", new Vector2(Resources.TimeElapsedBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.TimeElapsedBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
            }

            void GDM_DeviceReset(object sender, EventArgs e)
            {
                Resources.SurvivingBoxesBase.ForceResize();
                Resources.RemainingBoxesBase.ForceResize();
                Resources.DestroyedBoxesBase.ForceResize();
                Resources.ScoreboardBase.ForceResize();
                Resources.TimeElapsedBase.ForceResize();
                renderPlaces.Clear();
                renderPlaces.Add("Surviving", new Vector2(Resources.SurvivingBoxesBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.SurvivingBoxesBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Remaining", new Vector2(Resources.RemainingBoxesBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.RemainingBoxesBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Destroyed", new Vector2(Resources.DestroyedBoxesBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.DestroyedBoxesBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Score", new Vector2(Resources.ScoreboardBase.Center.X - 17 * RenderingDevice.TextureScaleFactor.X, Resources.ScoreboardText.UpperLeft.Y + 13 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Multiplier", new Vector2(renderPlaces["Score"].X + 19 * 5 * RenderingDevice.TextureScaleFactor.X + 8 * RenderingDevice.TextureScaleFactor.X, Resources.ScoreboardText.UpperLeft.Y + 13 * RenderingDevice.TextureScaleFactor.Y));
                renderPlaces.Add("Time", new Vector2(Resources.TimeElapsedBase.LowerRight.X - 80 * RenderingDevice.TextureScaleFactor.X, Resources.TimeElapsedBase.UpperLeft.Y + 15 * RenderingDevice.TextureScaleFactor.Y));
            }
                
            public void Draw()
            {
                Color col = MediaSystem.SirenPlaying ? tintColor : textRenderColor;
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                drawSpawnBar(GameManager.CurrentLevel.BoxSpawnPoint, RenderingDevice.Camera.Zoom, RenderingDevice.GraphicsDevice.Viewport); // needs to be under everything

                // Draw basic elements
                if(!OneBox)
                {
                    Resources.ScoreboardBase.Draw();
                    Resources.ScoreboardText.Draw(textRenderColor);
                }
                Resources.SurvivingBoxesBase.Draw();
                Resources.SurvivingBoxesText.Draw(textRenderColor);
                Resources.RemainingBoxesBase.Draw();
                Resources.RemainingBoxesText.Draw(textRenderColor);
                Resources.TimeElapsedBase.Draw();
                Resources.TimeElapsedText.Draw(textRenderColor);
                Resources.DestroyedBoxesBase.Draw(MediaSystem.SirenPlaying ? col : Color.White);
                Resources.DestroyedBoxesText.Draw(Color.Lerp(textRenderColor, col, 0.5f));

                // Draw scoreboard numbers
                drawScoreboardUI(remaining, remainingBoxesMax, renderPlaces["Remaining"], false, false);

                drawScoreboardUI(destroyed, destroyedBoxesMax, renderPlaces["Destroyed"], true, false);

                drawScoreboardUI(surviving, survivingBoxesMax, renderPlaces["Surviving"], false, false);

                drawScoreboardUI(elapsedTime, renderPlaces["Time"]);

                if(!OneBox)
                {
                    drawScoreboardUI(score, multiplier);
                    foreach(ScorePopup p in popups)
                        p.Draw();
                }

                RenderingDevice.SpriteBatch.End();
            }

            private void drawSpawnBar(Vector3 spawnPoint, float zoom, Viewport viewport)
            {
                Vector3 projection = viewport.Project(spawnPoint, RenderingDevice.Camera.Projection,
                    RenderingDevice.Camera.View, RenderingDevice.Camera.World);
                float baseZoom = 100;
                float scale = baseZoom / zoom; // baseZoom / zoom gets us a scale that increases when zoom is low; that is; zoomed in.
                float offset = 15;
                Vector2 screenCoords = new Vector2(projection.X, projection.Y);

                screenCoords.X += offset * RenderingDevice.TextureScaleFactor.X;
                screenCoords.Y -= offset * RenderingDevice.TextureScaleFactor.Y;

                outerSpawnBar.X = innerSpawnBar.X = (int)(screenCoords.X + texOffset * RenderingDevice.TextureScaleFactor.X * scale);
                outerSpawnBar.Y = innerSpawnBar.Y = (int)(screenCoords.Y + texOffset * RenderingDevice.TextureScaleFactor.Y * scale);

                RenderingDevice.SpriteBatch.Draw(Resources.EmptyTex, outerSpawnBar, null, outerBarColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                if(remaining > 0)
                    RenderingDevice.SpriteBatch.Draw(Resources.EmptyTex, innerSpawnBar, null, innerBarColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                RenderingDevice.SpriteBatch.Draw(barTex, screenCoords, null, Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor * scale, SpriteEffects.None, 0);
            }

            /// <summary>
            /// Updates the scoreboard with new values. In all technicality, you wouldn't have to do this unless the values changed.
            /// </summary>
            /// <param name="surviving">The number of currently Delivered Boxes.</param>
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
                    popups.Add(new ScorePopup(renderPlaces["Score"], this.score - scoreLastFrame, RenderingDevice.SpriteBatch));

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
                    RenderingDevice.SpriteBatch.Draw(texture, position, UINumbers[10], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(position.X + 13 * RenderingDevice.TextureScaleFactor.X, position.Y), UINumbers[number1], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                }
                else if(number1 > 9)
                {
                    int firstDigit, secondDigit;
                    firstDigit = number1 / 10;
                    secondDigit = number1 % 10;
                    RenderingDevice.SpriteBatch.Draw(texture, position, UINumbers[firstDigit], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(position.X + 13 * RenderingDevice.TextureScaleFactor.X, position.Y), UINumbers[secondDigit], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                }

                RenderingDevice.SpriteBatch.Draw(texture, new Vector2(position.X + 26 * RenderingDevice.TextureScaleFactor.X, position.Y), UINumbers[useColon ? 14 : 11], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);

                if(number2 < 10)
                {
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(position.X + 39 * RenderingDevice.TextureScaleFactor.X, position.Y), UINumbers[useColon ? 0 : 10], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(position.X + 52 * RenderingDevice.TextureScaleFactor.X, position.Y), UINumbers[number2], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                }
                else if(number2 > 9)
                {
                    int firstDigit, secondDigit;
                    firstDigit = number2 / 10;
                    secondDigit = number2 % 10;
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(position.X + 39 * RenderingDevice.TextureScaleFactor.X, position.Y), UINumbers[firstDigit], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(position.X + 52 * RenderingDevice.TextureScaleFactor.X, position.Y), UINumbers[secondDigit], col, 0.0f, Vector2.Zero, 0.9f * RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
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
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Score"].X + (i * 15 * RenderingDevice.TextureScaleFactor.X), renderPlaces["Score"].Y), UINumbers[digits[i]], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);

                // draw multiplier
                float offset = 0;
                if(multiplier == 10)
                {
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[1], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * RenderingDevice.TextureScaleFactor.X;
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[0], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * RenderingDevice.TextureScaleFactor.X;
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[13], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 8 * RenderingDevice.TextureScaleFactor.X;
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[0], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * RenderingDevice.TextureScaleFactor.X;
                }
                else
                {
                    int whole = (int)multiplier;
                    int part = (int)((multiplier - whole) * 10);
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[10], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * RenderingDevice.TextureScaleFactor.X;
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[whole], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * RenderingDevice.TextureScaleFactor.X;
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[13], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 8 * RenderingDevice.TextureScaleFactor.X;
                    RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[part], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                    offset += 15 * RenderingDevice.TextureScaleFactor.X;
                }
                RenderingDevice.SpriteBatch.Draw(texture, new Vector2(renderPlaces["Multiplier"].X + offset, renderPlaces["Multiplier"].Y), UINumbers[12], Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
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
                private readonly float targetY = Resources.ScoreboardBase.LowerRight.Y + 22 * RenderingDevice.TextureScaleFactor.X;
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
                    font = Resources.LCDFont;
                    this.pos = initialPos;
                    int i = 0;
                    int temp = value;
                    while(temp > 0)
                    {
                        i++;
                        temp /= 10;
                    }
                    digits = i;
                    GameManager.Space.DuringForcesUpdateables.Starting += updateVelocities;
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
                        GameManager.Space.DuringForcesUpdateables.Starting -= updateVelocities;
                        IsDone = true;
                    }
                }

                public void Draw()
                {
                    float offset = 5 * 15 * RenderingDevice.TextureScaleFactor.X;
                    string s = (value > 0 ? "+" : "") + value.ToString();
                    batch.DrawString(font, s, new Vector2(pos.X + offset, pos.Y), new Color(value > 0 ? 0 : 255, 0, value > 0 ? 255 : 0) * (alpha / 255f), 0, new Vector2(font.MeasureString(s).X, 0), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
                }
            }
        }
        #endregion

        internal void AddModels()
        {
            addModelsToRenderer();
            foreach(Box b in boxesOnscreen)
                RenderingDevice.Add(b);
        }

        protected void onGDMReset()
        {
            for(int i = 0; i < textureList.Length; i++)
            {
                textureList[i] = billboardList[i];
                activeList[i] = activeBillboardList[i];
            }
        }
    }
}
