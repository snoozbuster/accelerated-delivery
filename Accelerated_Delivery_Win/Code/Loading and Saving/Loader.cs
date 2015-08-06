using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Collidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.CollisionShapes.ConvexShapes;
using Microsoft.Xna.Framework.Media;
using BEPUphysics.Materials;
using System.Security;
using System.Net;
using Accelerated_Delivery_Win;
using BEPUphysics.Paths;
using System.IO;

namespace Accelerated_Delivery_Win
{
    // Loader is responsible for incrementally loading the content.
    public sealed class Loader : IEnumerable<float>
    {
        #region Textures

        #region Main Menu
        /// <summary>
        /// Opening splash screen! Yaaaaay!
        /// </summary>
        public Texture2D tbcSplash;
        /// <summary>
        /// Background for the main menu.
        /// </summary>
        public Texture2D mainMenuBackground;
        /// <summary>
        /// Logo for the main menu.
        /// </summary>
        public Texture2D mainMenuLogo;

        public Texture2D pressStart;

        //public Texture2D IceBumpMap;
        public Texture2D IceTexture;

        #region Buttons
        /// <summary>
        /// This is a texture for a button to start the game with.
        /// </summary>
        public Sprite startButton;
        /// <summary>
        /// This is a texture for a button to quit the game with.
        /// </summary>
        public Sprite quitButton;
        /// <summary>
        /// This is a texture for a button to open a menu to select menus with.
        /// </summary>
        public Sprite levelSelectButton;
        /// <summary>
        /// This is a texture for a button to open a screen with instructions on it.
        /// </summary>
        public Sprite instructionsButton;
        /// <summary>
        /// This is a texture for a button to open a screen with options on it.
        /// </summary>
        public Sprite optionsButton;
        /// <summary>
        /// This is a texture for a button to continue a level.
        /// </summary>
        public Sprite continueButton;
        #endregion

        #endregion

        #region Other Buttons
        /// <summary>
        /// This is a texture for a button used to resume the game.
        /// </summary>
        public Sprite resumeButton;
        /// <summary>
        /// This is a texture for a button used to restart the level.
        /// </summary>
        public Sprite restartButton;
        /// <summary>
        /// This is a texture for a button used to return to the main menu.
        /// </summary>
        public Sprite mainMenuButton;
        /// <summary>
        /// This is a texture for a button used to quit the game from the pause menu 
        /// (and Game Over menu).
        /// </summary>
        public Sprite pauseQuitButton;
        /// <summary>
        /// This is a texture for a button used to return to the main menu from the options.
        /// </summary>
        public Sprite backButton;
        /// <summary>
        /// This is a texture for a button used to say yes.
        /// </summary>
        public Sprite yesButton;
        /// <summary>
        /// This is a texture for a button used to say no.
        /// </summary>
        public Sprite noButton;
        /// <summary>
        /// This is a texture for a button used to go to the level select from the Game Over menu.
        /// </summary>
        public Sprite gameOverLevSelButton;
        /// <summary>
        /// This is a texture for a button used to restart the game from the Game Over menu.
        /// </summary>
        public Sprite gameOverRestartButton;
        /// <summary>
        /// This is a texture for a button used to open the extras menu.
        /// </summary>
        public Sprite extrasButton;
        /// <summary>
        /// This is a texture for a button used to open the high scores menu.
        /// </summary>
        public Sprite highScoreButton;
        /// <summary>
        /// This is a texture for a button used to open the objectives menu.
        /// </summary>
        public Sprite objectiveButton;
        /// <summary>
        /// This is a texture for a button used to open the saves menu.
        /// </summary>
        public Sprite savesButton;
        /// <summary>
        /// Save selection texture.
        /// </summary>
        public Texture2D SaveSelectorTex;
        #endregion

        #region Billboards
        public Texture2D[] billboardList;
        public Texture2D[] activeBillboardList;
        public Texture2D[] blackBoxBillboardList;
        #endregion

        #region Options
        public Texture2D buttonsTex;
        public Texture2D optionsTex;

        public Texture2D borders;

        public Sprite lowerOptionsBox;
        public Sprite higherOptionsBox;

        public Sprite difficultyBorderLight;
        public Texture2D difficultySlider;
        public Vector2 difficultyVector;

        public Texture2D optionsUI;
        public Sprite leftLightArrow;
        public Sprite rightLightArrow;
        public Sprite secondLeftLightArrow;
        public Sprite secondRightLightArrow;
        //SuperTextor checkmark;
        #endregion

        #region UI
        private Texture2D UIBase;
        private Texture2D scoreboardBase;
        public Sprite RemainingBoxesBase { get; private set; }
        public Sprite RemainingBoxesText { get; private set; }
        public Sprite SurvivingBoxesBase { get; private set; }
        public Sprite SurvivingBoxesText { get; private set; }
        public Sprite DestroyedBoxesBase { get; private set; }
        public Sprite DestroyedBoxesText { get; private set; }
        public Sprite ScoreboardBase { get; private set; }
        public Sprite ScoreboardText { get; private set; }
        public Sprite TimeElapsedBase { get; private set; }
        public Sprite TimeElapsedText { get; private set; }
        public Texture2D LCDNumbers { get; private set; }
        public Texture2D BarTexture { get; private set; }
        /// <summary>
        /// Use the index access to get a certain number.
        /// </summary>
        public Dictionary<int, Rectangle> UINumbers { get; private set; }
        public Sprite LevelOverlay { get; private set; }
        public Sprite[] OverlayWords { get; private set; }
        #endregion

        #region Level Select
        /// <summary>
        /// 37.5 wide by 34 tall, 2x2.
        /// </summary>
        public Texture2D starTex;
        /// <summary>
        /// i + 1 is level number. 11-13 is demos.
        /// </summary>
        public Sprite[] lockOverlays;
        /// <summary>
        /// i + 1 is level number. 11-13 is demos.
        /// </summary>
        public Sprite[] selectionGlows;
        /// <summary>
        /// i + 1 is level number. 11-13 is demos.
        /// </summary>
        public Sprite[] iconArray;
        /// <summary>
        /// i + 1 is level number. 11-13 is demos.
        /// </summary>
        public Sprite[] activeIconArray;
        public Sprite InfoBox { get; private set; }
        #endregion

        public Texture2D halfBlack;
        public Color FadeColor;
        public Texture2D EmptyTex { get; private set; }
        public Texture2D LaserTex;
        public Effect LaserShader;
        public Texture2D cursor;
        public Texture2D Plus1 { get; private set; }
        public Texture2D Dock;
        public Texture2D MediaTexture;
        public Texture2D AchievementToastTexture;
        public Texture2D AchievementMenuTexture;
        public Texture2D AchievementsCompTex;
        public Texture2D AchievementLockedTexture;
        public Texture2D BeachTexture;

        #endregion

        #region Common Models
        /// <summary>
        /// This is a model of le box. It's much easier to be able to call Box() 
        /// without parameters, so the constructor pulls it from here.
        /// </summary>
        public Model boxModel;

        /// <summary>
        /// Model of a tube. Also faster to load a Tube with one less parameter, 
        /// so it's here.
        /// </summary>
        public Model tubeModelX;

        public Model resultsPlane;

        public BaseModel lavaSkyboxModel { get; private set; }

        public BaseModel lavaOuterModel { get; private set; }

        public BaseModel skySkyboxModel { get; private set; }

        public BaseModel skyOuterModel { get; private set; }

        public BaseModel iceSkyboxModel { get; private set; }

        public BaseModel iceOuterModel { get; private set; }

        public BaseModel beachSkyboxModel { get; private set; }

        public BaseModel beachOuterModel { get; private set; }

        public BaseModel genericSkyboxModel { get; private set; }

        public BaseModel genericOuterModel { get; private set; }

        public BaseModel spaceSkyboxModel { get; private set; }

        public BaseModel spaceOuterModel { get; private set; }

        public Model Dispenser { get; private set; }

        public Model BlueBoxModel { get; private set; }

        public Model BlackBoxModel { get; private set; }
        #endregion

        #region Font
        /// <summary>
        /// A small version of the font we use. For fatscreen only.
        /// </summary>
        public SpriteFont SmallerFont { get; private set; }
        /// <summary>
        /// A default size of the font we use.
        /// </summary>
        public SpriteFont Font { get; private set; }
        /// <summary>
        /// A big version of the font we use. For widescreen only.
        /// </summary>
        public SpriteFont BiggerFont { get; private set; }
        /// <summary>
        /// The LCD font.
        /// </summary>
        public SpriteFont LCDFont { get; set; }
        #endregion

        #region Levels
        public Level level00 { get; private set; }
        public Level level01 { get; private set; }
        public Level level02 { get; private set; }
        public Level level03 { get; private set; }
        public Level level04 { get; private set; }
        public Level level05 { get; private set; }
        public Level level06 { get; private set; }
        public Level level07 { get; private set; }
        public Level level08 { get; private set; }
        public Level level09 { get; private set; }
        public Level level10 { get; private set; }
        public Level level11 { get; private set; }
        public Level levelD1 { get; private set; }
        public Level levelD2 { get; private set; }
        public Level levelD3 { get; private set; }
        public Level[] levelArray { get; private set; }
        #endregion

        public Sprite[] Credits = new Sprite[3];

        ContentManager content;
        int loadedItems = 0;
        int totalItems = 0;
        private GameTime savedTime;

        int sectionTime = 0;
        int totalTime = 0;
        public Texture2D WaterBumpMap;
        public Texture2D MetalTex { get; private set; }

        public Video LevelSelectVideo { get; private set; }
        internal Video end_0 { get; private set; }
        internal Video end_1 { get; private set; }

        private Texture2D creditsTex1, creditsTex2, creditsTex3;
        private Texture2D levelOverlay, overlay, overlay2;
        private Texture2D selectionGlow, qMark, icons, activeIcons, lockOverlay, boxTex;

        public Loader(ContentManager content, GraphicsDeviceManager gdm)
        {
            this.content = content;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        List<Dictionary<string, Model>> modelList;
        Effect bbEffect;

        ContentManager newContent;

        public IEnumerator<float> GetEnumerator()
        {
            // This function loads all the items, yielding the progress after each one.
            Program.Cutter.WriteToLog(this, "Beginning loading; performing variable initialization.");

            #region Init
            // we have to load any lists of items first, so we know how many steps will be in the loading bar

            string[][] levelList = { loadModelNames(0), loadModelNames(1), loadModelNames(2), 
                                  loadModelNames(3), loadModelNames(4), loadModelNames(5), 
                                  loadModelNames(6), loadModelNames(7), loadModelNames(8), 
                                  loadModelNames(9), loadModelNames(10), loadModelNames(11),
                                  loadModelNames(12), loadModelNames(13), loadModelNames(14) };

            modelList = new List<Dictionary<string, Model>>();
            for(int i = 0; i < 15; i++)
                modelList.Add(new Dictionary<string, Model>());

            // Set this to match how many times we will yield:
            for(int i = 0; i < levelList.Length; i++)
                totalItems += levelList[i].Length;
            // 67 = texture count, 1 = music, 5 = font count, 17 = all levels stuff, 1 = this initialization, 15 = level creation, 1 = dispenser, 3 = videos
            totalItems += 67 + 1 + 5 + 17 + 1 + 15 + 1 + 3;
            
            yield return progress(); // first progress bar update.
#endregion

            Program.Cutter.WriteToLog(this, "Loading video");
            LevelSelectVideo = content.Load<Video>("Video/level select");
            yield return progress();

            newContent = new ContentManager(Program.Game.Services);
            newContent.RootDirectory = Program.SavePath;

            File.Copy("Content\\textures\\edn_1.xnb", Program.SavePath + "edn_0.wmv", true);
            File.Copy("Content\\textures\\edn_0.xnb", Program.SavePath + "edn_0.xnb", true);
            end_0 = newContent.Load<Video>("edn_0");
            File.Delete(Program.SavePath + "edn_0.wmv");
            File.Delete(Program.SavePath + "edn_0.xnb");
            yield return progress();

            File.Copy("Content\\textures\\enr_1.xnb", Program.SavePath + "enr_0.wmv", true);
            File.Copy("Content\\textures\\enr_0.xnb", Program.SavePath + "enr_0.xnb", true);
            end_1 = newContent.Load<Video>("enr_0");
            File.Delete(Program.SavePath + "enr_0.wmv");
            File.Delete(Program.SavePath + "enr_0.xnb");
            yield return progress();

            //File.Move("Content\\textures\\edn_1.xnb", "Content\\textures\\edn_0.wmv");
            //end_0 = content.Load<Video>("textures/edn_0");
            //File.Move("Content\\textures\\edn_0.wmv", "Content\\textures\\edn_1.xnb");
            //yield return progress();

            //File.Move("Content\\textures\\enr_1.xnb", "Content\\textures\\enr_0.wmv");
            //end_1 = content.Load<Video>("textures/enr_0");
            //File.Move("Content\\textures\\enr_0.wmv", "Content\\textures\\enr_1.xnb");
            //yield return progress();
            #region Models
            // Where possible, load the big things first -- an accelerating progress bar feels quicker.

            Program.Cutter.WriteToLog(this, "Loading models");
            for(int j = 0; j < levelList.Length; j++)
            {
                string[] strings = levelList[j];
                for(int i = 0; i < strings.Length; i++)
                {
                    modelList[j].Add(strings[i].Replace("Level" + (j < 12 ? j.ToString() : "D" + (j - 11)) + "/", ""), content.Load<Model>(strings[i]));
                    yield return progress();
                }
            }
            Resources.boxModel = boxModel = content.Load<Model>("All Levels/box");
            Box box = new Box(Vector3.Zero); // necessary because otherwise it looks for the current level's
            RenderingDevice.Add(box); RenderingDevice.RemovePermanent(box); // this gets the box model in the masterDict for the spinning saving box
            yield return progress();
            Resources.tubeModel = tubeModelX = content.Load<Model>("All Levels/tube_x");
            yield return progress();
            Resources.MetalTex = MetalTex = content.Load<Texture2D>("textures/metal");
            yield return progress();
            Resources.WaterBumpMap = WaterBumpMap = content.Load<Texture2D>("textures/waterbump");
            yield return progress();
            //IceBumpMap = content.Load<Texture2D>("textures/icenor_01");
            //yield return progress();
            Resources.IceTexture = IceTexture = content.Load<Texture2D>("textures/icetex_01");
            yield return progress();
            Resources.BeachTexture = BeachTexture = content.Load<Texture2D>("textures/beach");
            yield return progress();
            Resources.lavaSkyboxModel = lavaSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_lava"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.lavaOuterModel = lavaOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_lava"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.iceSkyboxModel = iceSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_ice"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.iceOuterModel = iceOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_ice"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.beachSkyboxModel = beachSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_beach"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.beachOuterModel = beachOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_beach"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.skySkyboxModel = skySkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_sky"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.skyOuterModel = skyOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_sky"); }, false, false, Vector3.Zero);
            yield return progress();
            //Resources.genericSkyboxModel = genericSkyboxModel = new BaseModel(content.Load<Model>("All Levels/skybox_generic"), false, false, Vector3.Zero);
            //yield return progress();
            Resources.genericOuterModel = genericOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/generic_outer"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.spaceSkyboxModel = spaceSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_space"); }, false, false, Vector3.Zero);
            yield return progress();
            Resources.spaceOuterModel = spaceOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_space"); }, false, false, Vector3.Zero);
            yield return progress();
            Dispenser = content.Load<Model>("All Levels/dispenser");
            yield return progress();
            Resources.blueBoxModel = BlueBoxModel = content.Load<Model>("All Levels/blue_box");
            yield return progress();
            Resources.blackBoxModel = BlackBoxModel = content.Load<Model>("All Levels/black_box");
            yield return progress();

            Theme.Initialize(content);

            Program.Cutter.WriteToLog(this, "Last section took " + sectionTime + "ms.");
            sectionTime = 0;
            #endregion

            Program.Cutter.WriteToLog(this, "Loading textures");

            halfBlack = new Texture2D(RenderingDevice.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
            color[0] = new Color(0, 0, 0, 178);
            halfBlack.SetData(color); //set the color data on the texture
            Resources.halfBlack = halfBlack;

            tbcSplash = content.Load<Texture2D>("2D/Splashes and Overlays/Logo");
            yield return progress();
            pressStart = content.Load<Texture2D>("Font/press start");
            yield return progress();

            #region credits
            creditsTex1 = content.Load<Texture2D>("2D/Credits/credits_01");
            Credits[0] = new Sprite(delegate { return creditsTex1; }, new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height + 30) + new Vector2(0, (2048 * 0.5f) * RenderingDevice.TextureScaleFactor.Y), null, Sprite.RenderPoint.Center);
            yield return progress();
            creditsTex2 = content.Load<Texture2D>("2D/Credits/credits_02");
            Credits[1] = new Sprite(delegate { return creditsTex2; }, new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height + 30) + new Vector2(0, (2048 + 936 * 0.5f - 4) * RenderingDevice.TextureScaleFactor.Y), null, Sprite.RenderPoint.Center);
            yield return progress();
            creditsTex3 = content.Load<Texture2D>("2D/Credits/credits_03");
            Credits[Credits.Length - 1] = new Sprite(delegate { return creditsTex3; }, new Vector2(RenderingDevice.Width, RenderingDevice.Height) * 0.5f, null, Sprite.RenderPoint.Center);
            Credits[0].DrawUnscaled = Credits[1].DrawUnscaled = Credits[2].DrawUnscaled = true;
            yield return progress();
            #endregion

            #region Numbers
            blackBoxBillboardList = new Texture2D[10];
            this.billboardList = new Texture2D[10];
            activeBillboardList = new Texture2D[10];
            for(int i = 0; i < 10; i++)
            {
                string number = (i + 1 < 10 ? "0" : "") + (i + 1);
                blackBoxBillboardList[i] = content.Load<Texture2D>("2D/Other/num_" + number + "_blackbox");
                yield return progress();
                activeBillboardList[i] = content.Load<Texture2D>("2D/Other/num_" + number + "_active");
                yield return progress();
                this.billboardList[i] = content.Load<Texture2D>("2D/Other/num_" + number);
                yield return progress();
            }
            #endregion

            //LaserTex = content.Load<Texture2D>("textures/Lazar");
            //yield return progress();

            borders = content.Load<Texture2D>("2D/Options Menu/borders");
            yield return progress();
            Dock = content.Load<Texture2D>("2D/Splashes and Overlays/box_720x400_light");
            yield return progress();

            mainMenuBackground = content.Load<Texture2D>("2D/Splashes and Overlays/Background");
            yield return progress();
            mainMenuLogo = content.Load<Texture2D>("2D/Splashes and Overlays/BurningBoxesLogo01");
            yield return progress();
            MediaTexture = content.Load<Texture2D>("2D/Music Player/music");
            yield return progress();
            Resources.LaserTexture = LaserTex = content.Load<Texture2D>("textures/Lazar");
            yield return progress();
            Resources.LaserShader = LaserShader = content.Load<Effect>("Shaders/laser");
            yield return progress();

            Resources.AchievementToastTexture = AchievementToastTexture = content.Load<Texture2D>("2D/Splashes and Overlays/achievement");
            yield return progress();
            AchievementMenuTexture = content.Load<Texture2D>("2D/Objectives/ach_comp");
            yield return progress();
            AchievementsCompTex = content.Load<Texture2D>("2D/Objectives/ach_compilation");
            Accomplishment.AchievementTexture = AchievementsCompTex;
            yield return progress();
            AchievementLockedTexture = content.Load<Texture2D>("2D/Objectives/ach_locked");
            yield return progress();

            #region Buttons
            //if(Program.Game.AspectRatio < 1.4)
            //{
            //    buttonsTex = content.Load<Texture2D>("2D/Buttons/buttons_old");
            //    yield return progress();

            //    Rectangle buttonRect = new Rectangle(0, 0, 168, 41);

            //    resumeButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 5, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    restartButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    mainMenuButton = new SuperTextor(ref buttonsTex, new Vector2((RenderingDevice.Width * 0.53f), (RenderingDevice.Height * 0.75f)), new Rectangle(buttonRect.Width, buttonRect.Height, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    pauseQuitButton = new SuperTextor(ref buttonsTex, new Vector2((RenderingDevice.Width * 0.76f), (RenderingDevice.Height * 0.75f)), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    gameOverLevSelButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    gameOverRestartButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    instructionsButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.23f, RenderingDevice.Height * 0.84f), new Rectangle(buttonRect.Width, 0, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    quitButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.71f, RenderingDevice.Height * 0.755f), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    levelSelectButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.39f, RenderingDevice.Height * 0.755f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    startButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.07f, RenderingDevice.Height * 0.755f), buttonRect, SuperTextor.RenderPoint.UpLeft);
            //    optionsButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.55f, RenderingDevice.Height * 0.84f), new Rectangle(0, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    backButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.025f), new Rectangle(buttonRect.Width, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    yesButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.315f, RenderingDevice.Height * 0.65f), new Rectangle(0, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    noButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.515f, RenderingDevice.Height * 0.65f), new Rectangle(buttonRect.Width, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    optionsTex = content.Load<Texture2D>("2D/Options Menu/options_fat");
            //    yield return progress();
            //    tabRect = new Rectangle(0, 0, 200, 66);

            //    higherOptionsBox = new SuperTextor(ref optionsTex, new Vector2(RenderingDevice.Width * 0.015f, RenderingDevice.Height * 0.1f), new Rectangle(0, tabRect.Height, 774, 334), SuperTextor.RenderPoint.UpLeft);
            //    lowerOptionsBox = new SuperTextor(ref optionsTex, new Vector2(RenderingDevice.Width * 0.015f, higherOptionsBox.LowerRight.Y + 30), new Rectangle(0, tabRect.Height + 334, 774, 160), SuperTextor.RenderPoint.UpLeft);
            //}
            //else
            //{
            buttonsTex = content.Load<Texture2D>("2D/Buttons/buttons");
            yield return progress();

            Rectangle buttonRect = new Rectangle(0, 0, 210, 51);

            resumeButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 5, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            restartButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            mainMenuButton = new Sprite(delegate { return buttonsTex; }, new Vector2((RenderingDevice.Width * 0.53f), (RenderingDevice.Height * 0.75f)), new Rectangle(buttonRect.Width, buttonRect.Height, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            pauseQuitButton = new Sprite(delegate { return buttonsTex; }, new Vector2((RenderingDevice.Width * 0.76f), (RenderingDevice.Height * 0.75f)), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            gameOverLevSelButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            gameOverRestartButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            instructionsButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.23f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, 0, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            quitButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.8f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            levelSelectButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.42f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            startButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.04f, RenderingDevice.Height * 0.75f), buttonRect, Sprite.RenderPoint.UpLeft);
            backButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.025f), new Rectangle(buttonRect.Width, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            yesButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.315f, RenderingDevice.Height * 0.65f), new Rectangle(0, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            noButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.515f, RenderingDevice.Height * 0.65f), new Rectangle(buttonRect.Width, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            continueButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.04f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 6, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            optionsButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            extrasButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 6, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            highScoreButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 7, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            savesButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 7, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            objectiveButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 5, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //}

            //windowsTabLight = new SuperTextor(ref optionsTex, new Vector2(higherOptionsBox.UpperLeft.X + tabRect.Width * 0.16f, higherOptionsBox.UpperLeft.Y - tabRect.Height), tabRect, SuperTextor.RenderPoint.UpLeft);
            //xboxTabLight = new SuperTextor(ref optionsTex, new Vector2(higherOptionsBox.UpperLeft.X + tabRect.Width * 1.03f, higherOptionsBox.UpperLeft.Y - tabRect.Height), new Rectangle(tabRect.Width * 2, 0, tabRect.Width, tabRect.Height), SuperTextor.RenderPoint.UpLeft);

            SaveSelectorTex = content.Load<Texture2D>("2D/Buttons/save_selection");
            yield return progress();
            #endregion

            #region Options
            optionsTex = content.Load<Texture2D>("2D/Options Menu/options_widescr");
            yield return progress();
            Rectangle tabRect = new Rectangle(0, 0, 240, 80);

            higherOptionsBox = new Sprite(delegate { return optionsTex; }, new Vector2(RenderingDevice.Width * 0.013f, RenderingDevice.Height * 0.12f), new Rectangle(0, tabRect.Height, 1248, 400), Sprite.RenderPoint.UpLeft);
            lowerOptionsBox = new Sprite(delegate { return optionsTex; }, new Vector2(RenderingDevice.Width * 0.013f, higherOptionsBox.LowerRight.Y + 30), new Rectangle(0, tabRect.Height + 400, 1248, 192), Sprite.RenderPoint.UpLeft);

            float xOffsetBorder = RenderingDevice.Width * 0.048f;
            float yOffset = higherOptionsBox.UpperLeft.Y + RenderingDevice.Height * 0.087f;

            difficultyBorderLight = new Sprite(delegate { return borders; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.728f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Width * 0.03f)), new Rectangle(0, 90, 175, 45), Sprite.RenderPoint.UpLeft);
            difficultyVector = new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.728f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Width * 0.03f));
            difficultySlider = content.Load<Texture2D>("2D/Options Menu/DifficultySlider");
            yield return progress();

            Rectangle arrowBox = new Rectangle(0, 0, 50, 40);
            optionsUI = content.Load<Texture2D>("2D/Options Menu/arrows");
            yield return progress();

            rightLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.87f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Height * 0.055f)), arrowBox, Sprite.RenderPoint.UpLeft);
            leftLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.68f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Height * 0.055f)), new Rectangle(arrowBox.Width, 0, arrowBox.Width, arrowBox.Height), Sprite.RenderPoint.UpLeft);

            secondRightLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.783f), yOffset + (RenderingDevice.Height * 0.17f)), arrowBox, Sprite.RenderPoint.UpLeft);
            secondLeftLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.692f), yOffset + (RenderingDevice.Height * 0.17f)), new Rectangle(arrowBox.Width, 0, arrowBox.Width, arrowBox.Height), Sprite.RenderPoint.UpLeft);
            #endregion

            #region Level Select
            lockOverlay = content.Load<Texture2D>("2D/Level Select/lock_overlay");
            yield return progress();
            selectionGlow = content.Load<Texture2D>("2D/Level Select/selection-glow");
            yield return progress();
            Resources.starTex = starTex = content.Load<Texture2D>("2D/Level Select/stars");
            yield return progress();
            icons = content.Load<Texture2D>("2D/Level Select/icons");
            yield return progress();
            activeIcons = content.Load<Texture2D>("2D/Level Select/icons_active");
            yield return progress();
            qMark = content.Load<Texture2D>("2D/Level Select/question_mark");
            yield return progress();
            boxTex = content.Load<Texture2D>("2D/Level Select/box_440x260_light");
            yield return progress();

            iconArray = new Sprite[14];
            activeIconArray = new Sprite[14];
            lockOverlays = new Sprite[14];
            selectionGlows = new Sprite[14];
            float x = 0, y = 0;
            int index;

            Rectangle iconRect = new Rectangle(0, 0, icons.Width / 7, icons.Height / 2 + 1);

            for(int j = 0; j < 4; j++)
            {
                if(j < 2)
                    for(int i = 0; i < 5; i++)
                    {
                        index = j * 5 + i;
                        x = (RenderingDevice.Width * 0.11f) + (RenderingDevice.Width * 0.195f * i);
                        y = (RenderingDevice.Height * 0.245f) + (RenderingDevice.Height * 0.2f * j);
                        iconArray[index] = new Sprite(delegate { return icons; }, new Vector2(x, y), new Rectangle(iconRect.Width * i, iconRect.Height * j, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
                        lockOverlays[index] = new Sprite(delegate { return lockOverlay; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
                        selectionGlows[index] = new Sprite(delegate { return selectionGlow; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
                        activeIconArray[index] = new Sprite(delegate { return activeIcons; }, new Vector2(x, y), new Rectangle(iconRect.Width * i, iconRect.Height * j, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
                    }
                else if(j == 2)
                {
                    x = (RenderingDevice.Width * 0.11f);
                    y = (RenderingDevice.Height * 0.245f) + (RenderingDevice.Height * 0.2f * j);
                    iconArray[10] = new Sprite(delegate { return icons; }, new Vector2(x, y), new Rectangle(iconRect.Width * 5, 0, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
                    lockOverlays[10] = new Sprite(delegate { return qMark; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
                    selectionGlows[10] = new Sprite(delegate { return selectionGlow; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
                    activeIconArray[10] = new Sprite(delegate { return activeIcons; }, new Vector2(x, y), new Rectangle(iconRect.Width * 5, 0, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
                }
                else
                    for(int i = 0; i < 3; i++)
                    {
                        index = 11 + i; // remember level 11 is index 10
                        x = (RenderingDevice.Width * 0.11f) + (RenderingDevice.Width * 0.195f * i);
                        y = (RenderingDevice.Height * 0.245f) + (RenderingDevice.Height * 0.2f * j);
                        iconArray[index] = new Sprite(delegate { return icons; }, new Vector2(x, y), new Rectangle(iconRect.Width * (i == 0 ? 5 : 6), iconRect.Height * ((i + 1) % 2), iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
                        lockOverlays[index] = new Sprite(delegate { return lockOverlay; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
                        selectionGlows[index] = new Sprite(delegate { return selectionGlow; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
                        activeIconArray[index] = new Sprite(delegate { return activeIcons; }, new Vector2(x, y), new Rectangle(iconRect.Width * (i == 0 ? 5 : 6), iconRect.Height * ((i + 1) % 2), iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
                    }
            }

            InfoBox = new Sprite(delegate { return boxTex; }, new Vector2(iconArray[9].LowerRight.X - boxTex.Width, iconArray[13].LowerRight.Y - boxTex.Height), null, Sprite.RenderPoint.UpLeft);
            #endregion

            EmptyTex = new Texture2D(RenderingDevice.GraphicsDevice, 1, 1);
            EmptyTex.SetData(new[] { Color.White });
            Resources.EmptyTex = EmptyTex;
            FadeColor = new Color(0, 0, 0, 0);
            yield return progress();

            Resources.Plus1 = Plus1 = content.Load<Texture2D>("2D/Other/plus1");
            yield return progress();
            Resources.BarTexture = BarTexture = content.Load<Texture2D>("2D/Splashes and Overlays/spawnBar");
            yield return progress();

            Rectangle r = new Rectangle(0, 0, 370, 123);
            levelOverlay = content.Load<Texture2D>("2D/Special Text/level");
            Resources.LevelOverlay = LevelOverlay = new Sprite(delegate { return levelOverlay; }, new Vector2(-levelOverlay.Width * 0.5f, RenderingDevice.Height * 0.45f), null, Sprite.RenderPoint.Center);
            yield return progress();
            overlay = content.Load<Texture2D>("2D/Special Text/words");
            overlay2 = content.Load<Texture2D>("2D/Special Text/words2");
            OverlayWords = new Sprite[15];
            for(int i = 0; i < OverlayWords.Length - 3; i++)
                OverlayWords[i] = new Sprite(delegate { return overlay; }, new Vector2(RenderingDevice.Width + r.Width * 0.5f, (LevelOverlay.Center.Y + LevelOverlay.LowerRight.Y) * 0.52f), new Rectangle(r.Width * (i % 2), r.Height * (i / 2), r.Width, r.Height), Sprite.RenderPoint.Center);
            OverlayWords[12] = new Sprite(delegate { return overlay2; }, new Vector2(LevelOverlay.UpperLeft.X - r.Width * 0.75f, LevelOverlay.UpperLeft.Y - r.Height * 0.45f), new Rectangle(0, 0, r.Width, r.Height), Sprite.RenderPoint.Center);
            OverlayWords[13] = new Sprite(delegate { return overlay2; }, new Vector2(RenderingDevice.Width + r.Width * 0.5f, (LevelOverlay.Center.Y + LevelOverlay.LowerRight.Y) * 0.525f), new Rectangle(r.Width, 0, r.Width, r.Height - 2), Sprite.RenderPoint.Center);
            OverlayWords[14] = new Sprite(delegate { return overlay2; }, new Vector2(RenderingDevice.Width + r.Width * 0.5f, (LevelOverlay.Center.Y + LevelOverlay.LowerRight.Y) * 0.525f), new Rectangle(0, r.Height, 550, r.Height + 2), Sprite.RenderPoint.Center);
            Resources.OverlayWords = OverlayWords;
            yield return progress();

            UIBase = content.Load<Texture2D>("Font/UI base");
            Resources.SurvivingBoxesBase = SurvivingBoxesBase = new Sprite(delegate { return UIBase; }, new Vector2(908.8f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            Resources.SurvivingBoxesText = SurvivingBoxesText = new Sprite(delegate { return UIBase; }, new Vector2(908.8f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 100, 357, 50), Sprite.RenderPoint.UpLeft);
            Resources.RemainingBoxesBase = RemainingBoxesBase = new Sprite(delegate { return UIBase; }, new Vector2(16.64f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            Resources.RemainingBoxesText = RemainingBoxesText = new Sprite(delegate { return UIBase; }, new Vector2(16.64f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 50, 357, 50), Sprite.RenderPoint.UpLeft);
            Resources.DestroyedBoxesBase = DestroyedBoxesBase = new Sprite(delegate { return UIBase; }, new Vector2(16.64f * RenderingDevice.TextureScaleFactor.X, RemainingBoxesBase.LowerRight.Y), new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            Resources.DestroyedBoxesText = DestroyedBoxesText = new Sprite(delegate { return UIBase; }, new Vector2(16.64f * RenderingDevice.TextureScaleFactor.X, RemainingBoxesBase.LowerRight.Y), new Rectangle(0, 150, 357, 50), Sprite.RenderPoint.UpLeft);
            Resources.TimeElapsedBase = TimeElapsedBase = new Sprite(delegate { return UIBase; }, new Vector2(908.8f, SurvivingBoxesBase.LowerRight.Y) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            Resources.TimeElapsedText = TimeElapsedText = new Sprite(delegate { return UIBase; }, new Vector2(908.8f, SurvivingBoxesBase.LowerRight.Y) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 200, 357, 50), Sprite.RenderPoint.UpLeft);
            yield return progress();

            scoreboardBase = content.Load<Texture2D>("Font/scoreboard");
            Resources.ScoreboardBase = ScoreboardBase = new Sprite(delegate { return scoreboardBase; }, new Vector2(640, 39.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 320, 50), Sprite.RenderPoint.Center);
            Resources.ScoreboardText = ScoreboardText = new Sprite(delegate { return scoreboardBase; }, new Vector2(ScoreboardBase.UpperLeft.X + 65 * RenderingDevice.TextureScaleFactor.X, ScoreboardBase.Center.Y), new Rectangle(0, 50, 122, 50), Sprite.RenderPoint.Center);
            yield return progress();

            Program.Cutter.WriteToLog(this, "Last section took " + sectionTime + "ms.");
            sectionTime = 0;

            #region Music
            Program.Cutter.WriteToLog(this, "Loading music");

            //MediaSystem.LoadAmbience(content);
            //yield return progress();
            //MediaSystem.LoadSoundEffects(content);
            //yield return progress();
            MediaSystem.LoadVoiceActing(content);
            yield return progress();

            Program.Cutter.WriteToLog(this, "Last section took " + sectionTime + "ms.");
            sectionTime = 0;
            #endregion

            #region Font
            Program.Cutter.WriteToLog(this, "Loading font");
            Resources.LCDFont = LCDFont = content.Load<SpriteFont>("Font/LCD");
            yield return progress();
            Resources.Font = Font = content.Load<SpriteFont>("Font/Ad-Font");
            yield return progress();
            SmallerFont = content.Load<SpriteFont>("Font/AD-Font_small");
            yield return progress();
            Resources.BiggerFont = BiggerFont = content.Load<SpriteFont>("Font/AD-Font_big");
            yield return progress();
            Resources.LCDNumbers = LCDNumbers = content.Load<Texture2D>("Font/LCD numbers");
            UINumbers = new Dictionary<int, Rectangle>(LCDNumbers.Width / 19);
            for(int i = 0; i < LCDNumbers.Width / 19; i++)
                UINumbers[i] = new Rectangle(i * 19, 0, 19, 22);
            Resources.UINumbers = UINumbers;
            yield return progress();

            Program.Cutter.WriteToLog(this, "Last section took " + sectionTime + "ms.");
            sectionTime = 0;
            #endregion

            #region Levels
            Program.Cutter.WriteToLog(this, "Creating levels");
            List<Vector3> billboardList = new List<Vector3>();
            List<Tube> tubeList = new List<Tube>();
            Dictionary<OperationalMachine, bool> machs = new Dictionary<OperationalMachine, bool>();
            List<Keyframe> keyframeList = new List<Keyframe>();
            Vector3 offset;

            bbEffect = content.Load<Effect>("Shaders/bbEffect");
            Level.Initialize(delegate { return bbEffect; }, this.billboardList, activeBillboardList, delegate { return Dispenser; });

            #region Level 0
            Program.Cutter.WriteToLog(this, "Creating level 0");
            for(int i = 0; i < 21; i++)
                tubeList.Add(new Tube(new Vector3(-23.182f + i, 0.241f, 7), true, false));

            billboardList.Add(new Vector3(2.689f, 0.241f, 14));

            machs.Add(new ClampedRotationMachine(1, 0, 2.25f, Vector3.UnitY, MathHelper.ToRadians(179), new Vector3(7.329f, 0.239f, 3.073f), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[0]["machine1"]; }, false, null, new Vector3(2.716f, 0.239f, 0.749f)),
                new BaseModel(delegate { return modelList[0]["machine1_glass"]; }, true, null, new Vector3(0.918f, 0.234f, 2.441f))), true);

            level00 = new InstructionsLevel(1, 1, new Vector3(-20.182f, 0.241f, 13), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[0]["base"]; },
                //level00 = new Level(0, 20, 10, new Vector3(-20.182f, 0.241f, 13), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[0]["base"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[0]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[0]["glass2"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(13.942f, 0, -4.61f)), null, machs, tubeList, new LevelCompletionData(new TimeSpan(0, 0, 30), 100, 0), "Training");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            yield return progress();
            #endregion

            #region Level 1
            Program.Cutter.WriteToLog(this, "Creating level 1");
            for(int i = 0; i < 20; i++)
                tubeList.Add(new Tube(new Vector3(-15 + i, 0, 1), true, false));

            billboardList.Add(new Vector3(-5.55f, -0.05f, 7));

            machs.Add(new TranslateMachine(1, 9, new Vector3(0, 0, -6.5f), 1f, false,
                new BaseModel(delegate { return modelList[1]["machine1_glass"]; }, true, null, new Vector3(-5.55f, -0.05f, 3.85f))) , true);

            //keyframeList.Add(new Keyframe(new Vector3(0, -3, 0), Quaternion.Identity, 1.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 0.75f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 3, 0), Quaternion.Identity, 1.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 0.75f));

            //machs.Add(new KeyframeMachine(0, keyframeList, true,
            //    new BaseModel(delegate { return modelList[1]["machine1_auto"]; }, false, null, new Vector3(-0.7f, 1.5f, 4.495f)) { LocalOffset = new Vector3(0, 0, -1.1f) }), false);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Vector3.UnitY * -3, 1.5f));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            keyframeList.Add(new Keyframe(Vector3.UnitY * 3, 1.5f));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[1]["machine1_auto"]; }, false, null, new Vector3(-0.7f, 1.5f, 4f - 0.833333f)) { LocalOffset = new Vector3(0, 0, -1.1f) }), false);
            keyframeList.Clear();

            level01 = new Level(1, 10, 5, new Vector3(-13, 0, 10), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[1]["base"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[1]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[1]["glass2"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(8.264f, 0.241f, -5.041f)), null, machs, tubeList, 
                new LevelCompletionData(new TimeSpan(0, 0, 30), 2000, 0), "Clock-in");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 2
            Program.Cutter.WriteToLog(this, "Creating level 2");

            for(int i = 0; i < 16; i++)
                tubeList.Add(new Tube(new Vector3(-48 + i, 4.414f, 7.2f), true, false));
            for(int i = 0; i < 27; i++)
                tubeList.Add(new Tube(new Vector3(-23 + i, 4.414f + (i * 0.45f), 7.2f), true, false));

            billboardList.Add(new Vector3(-27.5f, 4.506f, 13.5f));
            billboardList.Add(new Vector3(10.438f, 12.571f, 13.5f));

            machs.Add(new TranslateMachine(1, 7, Vector3.UnitZ * 5, 1.25f, false,
                new BaseModel(delegate { return modelList[2]["machine1"]; }, false, null, new Vector3(-27.5f, 4.506f, 5.867f)),
                new BaseModel(delegate { return modelList[2]["machine1_stripes"]; }, false, null, new Vector3(-27.5f, 4.414f, 5.796f))), true);

            machs.Add(new TranslateMachine(2, 9, Vector3.UnitZ * 5, 1.4f, false,
                new BaseModel(delegate { return modelList[2]["machine2"]; }, false, null, new Vector3(11.038f, 13.171f, 4.935f)),
                new BaseModel(delegate { return modelList[2]["machine2_stripes"]; }, false, null, new Vector3(10.654f, 12.763f, 5.004f))), true);

            level02 = new Level(2, 20, 5, new Vector3(-46, 4.414f, 15f), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[2]["base"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[2]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[2]["glass2"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(14.864f, -5.925f, -4.844f)), null, machs, tubeList, 
                new LevelCompletionData(new TimeSpan(0, 1, 45), 2250, 1), "Is There A Help\n  Button?");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            yield return progress();
            #endregion

            #region Level 3
            Program.Cutter.WriteToLog(this, "Creating level 3");

            for(int i = 0; i < 18; i++)
            {
                if(i < 15)
                    tubeList.Add(new Tube(new Vector3(-1.349f + i, -10.073f, 3.2f), true, false));
                if(i < 17)
                    tubeList.Add(new Tube(new Vector3(-34.849f + i, -4.017f, 8.2f), true, false));
                tubeList.Add(new Tube(new Vector3(6.151f, -6.573f + i, 3.2f), false, false));
            }
            for(int i = 0; i < 23; i++)
            {
                if(i < 21)
                    tubeList.Add(new Tube(new Vector3(-17.849f + i, -4.073f, 5.7f), true, false));
                tubeList.Add(new Tube(new Vector3(21.151f, -6.573f + i, 1.2f), false, false));
            }
            for(int i = 0; i < 31; i++)
                tubeList.Add(new Tube(new Vector3(23.651f - i, 18.503f, -1.8f), true, true));

            billboardList.Add(new Vector3(-6.849f, -4.073f, 10f));
            billboardList.Add(new Vector3(16.151f, -10.073f, 6f));
            billboardList.Add(new Vector3(6.151f, 13.027f, 10f));

            ClampedRotationMachine c1, c2, c3;

            c1 = new ClampedRotationMachine(1, 2, 0.75f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-12.849f, -5.853f, 6.7f), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[3]["machine1_part1"]; }, false, null, new Vector3(-12.849f, -5.853f, 6.7f)));

            c2 = new ClampedRotationMachine(1, -1, 0.75f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-4.849f, -2.303f, 6.7f), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[3]["machine1_part2"]; }, false, null, new Vector3(-4.849f, -2.303f, 6.7f)));

            c3 = new ClampedRotationMachine(1, -1, 0.75f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(2.651f, -5.853f, 6.7f), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[3]["machine1_part3"]; }, false, null, new Vector3(2.651f, -5.853f, 6.7f)));

            OperationalMachine.LinkMachines(c1, c2, c3);
            machs.Add(c1, true);
            machs.Add(c2, false);
            machs.Add(c3, false);

            float tubeY = -12.573f;

            machs.Add(new TranslateMachine(2, 5, new Vector3(5, 0, 0), 1.5f, false,
                new BaseModel(delegate { return modelList[3]["machine2"]; }, false, null, new Vector3(16.151f, -9.40585f, 1.367f)),
                new BaseModel(delegate { return modelList[3]["machine2_stripes"]; }, false, null, new Vector3(16.185f, -10.073f, 1.4f)),
                new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false), new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false),
                new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false), new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false),
                new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false), new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false)), true);

            machs.Add(new Cannon(3, 8, new Vector3(0, 0, 6), Vector3.UnitX, new CannonPathFinder(new Vector3(0, -11, 11), new Vector3(6.15f, 14, 4.2f), new Vector3(6.15f, -45.45f, 4.2f)), new Vector3(6.15f, 14, 4.2f), //195,
                new BaseModel(delegate { return modelList[3]["machine3"]; }, false, null, new Vector3(6.151f, 13.027f, 0f))), true);

            level03 = new Level(3, 30, 10, new Vector3(-31.849f, -4.073f, 15), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[3]["base"]; },
                new[] { new BaseModel(delegate { return modelList[3]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[3]["flags"]; }, false, false, Vector3.Zero) }, 
                new Goal(new Vector3(-10.099f, 18.62f, -6.5f)), new Goal(new Vector3(6.222f, -52.45f, -7f), Color.Blue), machs, tubeList,
                new LevelCompletionData(new TimeSpan(0, 3, 50), 3700, 2), "One Box Two Box\n      Red Box Blue Box");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 4
            Program.Cutter.WriteToLog(this, "Creating level 4");

            for(int i = 0; i < 17; i++)
            {
                tubeList.Add(new Tube(new Vector3(-47 + i, 4.9f, 7.2f), true, false));
                tubeList.Add(new Tube(new Vector3(3.995f, -14.598f + i, -0.4f), false, false));
                tubeList.Add(new Tube(new Vector3(-1.005f, -1.598f + i, -0.4f), false, false));
                tubeList.Add(new Tube(new Vector3(-13.505f, 6.902f - i, -0.8f), false, true));
            }

            for(int i = 0; i < 19; i++)
                tubeList.Add(new Tube(new Vector3(20.995f, 14.902f - i, -3.3f), false, true));

            for(int i = 0; i < 11; i++)
                tubeList.Add(new Tube(new Vector3(19.495f + i, -5.598f, -3.3f), true, false));

            machs.Add(new ClampedRotationMachine(1, 1, 1.7f, Vector3.UnitY, MathHelper.PiOver2, new Vector3(-20.65f, 4.974f, 6.362f), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[4]["machine1"]; }, false, null, new Vector3(-23.305f, 4.902f, 6.87f)), new Tube(new Vector3(-29.505f, 4.902f, 6.204f), true, false),
                new Tube(new Vector3(-28.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-27.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-26.505f, 4.902f, 6.204f), true, false),
                new Tube(new Vector3(-25.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-24.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-23.505f, 4.902f, 6.204f), true, false)) { DampingMultiplier = 6 }, true);

            machs.Add(new TranslateMachine(2, 7, new Vector3(19.7f, 0, 10f), 4.25f, false,
                new BaseModel(delegate { return modelList[4]["machine2"]; }, false, null, new Vector3(-18.885f, -12.536f, -6.854f))), true);

            TranslateMachine m31, m32;

            m31 = new TranslateMachine(3, 9, new Vector3(-5.9f, 0, 0), 1.75f, false,
                new BaseModel(delegate { return modelList[4]["machine3_part1"]; }, false, null, new Vector3(8.529f, -0.536f, 0.209f)));

            m32 = new TranslateMachine(3, -1, new Vector3(5.9f, 0, 0), 1.75f, false,
                new BaseModel(delegate { return modelList[4]["machine3_part2"]; }, false, null, new Vector3(-5.961f, 12.464f, 0.209f)));

            OperationalMachine.LinkMachines(m31, m32);
            machs.Add(m31, true);
            machs.Add(m32, false);
            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 5f));

            KeyframeMachine k1, k2;

            k1 = new KeyframeMachine(4, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[4]["machine4_part1"]; }, false, null, new Vector3(10.895f, 12.402f, 0.773f)));

            keyframeList.Clear();

            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            keyframeList.Add(new Keyframe(new Vector3(5, 0, 0), Quaternion.Identity, 2.3f));
            keyframeList.Add(new Keyframe(0.4f));
            keyframeList.Add(new Keyframe(new Vector3(-5, 0, 0), Quaternion.Identity, 2.3f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            keyframeList.Add(new Keyframe(new Vector3(5, 0, 0), Quaternion.Identity, 2.3f));
            keyframeList.Add(new Keyframe(0.4f));
            keyframeList.Add(new Keyframe(new Vector3(-5, 0, 0), Quaternion.Identity, 2.3f));

            k2 = new KeyframeMachine(4, 8, keyframeList, true, 
                new BaseModel(delegate { return modelList[4]["machine4_part2"]; }, false, null, new Vector3(10.8f, 12.45f, 0f)));

            //OperationalMachine.LinkMachines(k1, k2);
            machs.Add(k1, true);
            machs.Add(k2, false);

            billboardList.Add(new Vector3(-17.305f, 4.902f, 14.87f));
            billboardList.Add(new Vector3(-18.885f, -12.536f, 1f));
            billboardList.Add(new Vector3(0, 0, 8.1f));
            billboardList.Add(new Vector3(10.895f, 12.402f, 10.5f));

            level04 = new IceLevel(4, 40, 10, new Vector3(-44.917f, 4.94f, 12.207f), billboardList, (BaseModel)delegate { return modelList[4]["base"]; },
                (BaseModel)delegate { return modelList[4]["extras"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[4]["glass1"]; }, true, false, Vector3.Zero), 
                new BaseModel(delegate { return modelList[4]["glass2"]; }, true, false, Vector3.Zero), 
                new BaseModel(delegate { return modelList[4]["glass3"]; }, true, false, Vector3.Zero), 
                new BaseModel(delegate { return modelList[4]["glass4"]; }, true, false, Vector3.Zero), 
                new BaseModel(delegate { return modelList[4]["glass5"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(31.654f, -5.558f, -7.572f)), machs, tubeList, 
                new LevelCompletionData(new TimeSpan(0, 4, 10), 3550, 3), "Cold Shoulder");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 5
            Program.Cutter.WriteToLog(this, "Creating level 5");

            for(int i = 0; i < 20; i++)
            {
                tubeList.Add(new Tube(new Vector3(-46.658f + i, 17.528f, 14), true, false));
                tubeList.Add(new Tube(new Vector3(35.642f - i, -35.272f, 19.4f), true, true));
                tubeList.Add(new Tube(new Vector3(-15.158f - i, -12.472f, 6.4f), true, true));
            }
            for(int i = 0; i < 5; i++)
                tubeList.Add(new Tube(new Vector3(32.342f, -11.972f - i, 6), false, true));
            for(int i = 0; i < 21; i++)
            {
                Tube t = new Tube(new Vector3(16.006f - ((16.006f + 0.377f) / 21) * i, -35.73f - ((-35.73f + 24.259f) / 21) * i, 18.2f), true, true);
                t.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-30));
                tubeList.Add(t);
            }
            for(int i = 0; i < 22; i++)
                tubeList.Add(new Tube(new Vector3(32.342f, 20.048f - i, 6), false, true));
            for(int i = 0; i < 11; i++)
                tubeList.Add(new Tube(new Vector3(0.644f - i, -23.879f, 17), true, true));

            billboardList.Add(new Vector3(-27.261f, 17.528f, 23f));
            billboardList.Add(new Vector3(24.382f, 17.528f, 21f));
            billboardList.Add(new Vector3(32.342f, -3.972f, 14));
            billboardList.Add(new Vector3(32.342f, -23.972f, 13));
            billboardList.Add(new Vector3(-7.858f, -23.872f, 26));

            machs.Add(new TranslateMachine(1, 10, new Vector3(0, 0, -5.5f), 1.5f, false,
                new BaseModel(delegate { return modelList[5]["machine1"]; }, true, null, new Vector3(-27.261f, 17.528f, 16.5f))) { DampingMultiplier = 2.5f }, true);

            machs.Add(new RailMachine(2, 1, false, new Vector3(-42.5f, 0, 0), 6, 10, new Vector3(25.878f, 17.528f, 10), 2.5f, Vector3.UnitY, MathHelper.ToRadians(179), -Vector3.UnitX,
                new[] { new BaseModel(delegate { return modelList[5]["machine2_slidepart"]; }, false, null, new Vector3(24.418f, 17.528f, 6.861f)) },
                new BaseModel(delegate { return modelList[5]["machine2_rotatepart"]; }, false, null, new Vector3(21.249f, 17.528f, 7.676f)),
                new BaseModel(delegate { return modelList[5]["machine2_rotatepart_glass"]; }, true, null, new Vector3(19.392f, 17.528f, 9)),
                new BaseModel(delegate { return modelList[5]["machine2_ice"]; }, false, null, new Vector3(19.361f, 17.528f, 4.846f)) { Mass = 0.05f }), true);

            #region machine 3
            tubeY = -25.972f;

            TranslateMachine m1, m2, m3, m4;

            m1 = new TranslateMachine(3, 9, Vector3.UnitZ * -12f, 2f, false, 1.5f, 0.5f,
                new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -23.972f, 3.686f)),
                new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
                new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -23.892f, 6.246f)));

            m4 = new TranslateMachine(3, 9, Vector3.UnitZ * -11.9f, 2f, false, 2f, 0f,
                new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -18.972f, 3.606f)),
                new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
                new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -18.892f, 6.3f)));

            tubeY += 5;

            m3 = new TranslateMachine(3, 9, Vector3.UnitZ * 11.9f, 2f, false, 0.5f, 1.5f,
                new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -8.792f, -8.206f)),
                new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
                new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -8.892f, -5.646f)));

            m2 = new TranslateMachine(3, 9, Vector3.UnitZ * 12f, 2f, false, 0, 2,
                new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -3.792f, -8.286f)),
                new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
                new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
                new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -3.892f, -5.7f)));

            OperationalMachine.LinkMachines(m1, m2, m3, m4);

            machs.Add(m1, true);
            machs.Add(m2, false);
            machs.Add(m3, false);
            machs.Add(m4, false);

            //keyframeList.Add(Keyframe.Zero);
            //keyframeList.Add(Keyframe.Zero);
            //// dummy machine to get the middle set of stripes in
            //machs.Add(new KeyframeMachine(0, keyframeList, false,
            //    new BaseModel(delegate { return modelList[5]["base_stripes"]; }, false, null, Vector3.Zero)), false);
            #endregion

            OperationalMachine m5, m6;

            keyframeList.Clear();

            keyframeList.Add(new Keyframe(new Vector3(0, 0, 15.5f), Quaternion.Identity, 4f));
            keyframeList.Add(new Keyframe(new Vector3(0, 0, 4.65f), Quaternion.Identity, 2f));

            BaseModel rotors = new BaseModel(delegate { return modelList[5]["machine4_rotors"]; }, false, null, new Vector3(32.242f, -29.272f, 0.529f));
            rotors.Ent.AngularVelocity = new Vector3(0, 0, 7);
            rotors.CommitInitialVelocities();

            m5 = new KeyframeMachine(4, 6, keyframeList, false, 
                rotors,
                new BaseModel(delegate { return modelList[5]["machine4_base"]; }, false, null, new Vector3(32.242f, -29.272f, 1.133f)));

            keyframeList.Clear();

            keyframeList.Add(new Keyframe(new Vector3(0, 0, 14.2f), Quaternion.Identity, 3.8f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.2f));

            m6 = new KeyframeMachine(4, -1, keyframeList, false,
                new BaseModel(delegate { return modelList[5]["machine4_glass"]; }, true, null, new Vector3(32.342f, -29.372f, 5.479f)));

            keyframeList.Clear();
            //Machine.LinkMachines(m5, m6);
            machs.Add(m5, true);
            machs.Add(m6, false);

            machs.Add(new TranslateMachine(5, 5, new Vector3(0, 0, -6.5f), 1.5f, false,
                new BaseModel(delegate { return modelList[5]["machine5_glass"]; }, true, null, new Vector3(-7.858f, -23.872f, 20.232f))), true);

            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(new Vector3(0, 12.5f, 0), Quaternion.Identity, 3f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(new Vector3(0, -12.5f, 0), Quaternion.Identity, 3f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));

            m5 = new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[5]["machine5_part2"]; }, false, null, new Vector3(-15.325f, -23.972f, 16)),
                new BaseModel(delegate { return modelList[5]["machine5_part2_glass2"]; }, true, null, new Vector3(-14.41629f - .445f, -23.98160f, 17.49686f - 0.1229568f)));

            keyframeList.Clear();

            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(new Vector3(0, 12.5f, 0), Quaternion.Identity, 3f));
            keyframeList.Add(new Keyframe(new Vector3(7, 0, 0), Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 0.5f));
            keyframeList.Add(new Keyframe(new Vector3(-7, 0, 0), Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(new Vector3(0, -12.5f, 0), Quaternion.Identity, 3f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));

            m6 = new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[5]["machine5_part2_glass"]; }, true, null, new Vector3(-14.208f, -23.972f, 16f)));

            OperationalMachine.LinkMachines(m5, m6);
            machs.Add(m5, false);
            machs.Add(m6, false);

            level05 = new IceLevel(5, 50, 20, new Vector3(-45.658f, 17.528f, 26), billboardList, (BaseModel)delegate { return modelList[5]["base"]; }, (BaseModel)delegate { return modelList[5]["effects"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[5]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[5]["glass2"]; }, true, false, Vector3.Zero), 
                new BaseModel(delegate { return modelList[5]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[5]["glass4"]; }, true, false, Vector3.Zero),
                new BaseModel(delegate { return modelList[5]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[5]["glass6"]; }, true, false, Vector3.Zero),
                new BaseModel(delegate { return modelList[5]["glass7"]; }, true, false, Vector3.Zero) }, 
                new Goal(new Vector3(-37.658f, -12.472f, 2.775f)), machs, tubeList, new LevelCompletionData(new TimeSpan(0, 7, 55), 10100, 7), "Water Cooler Talk");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 6
            Program.Cutter.WriteToLog(this, "Creating level 6");

            billboardList.Add(new Vector3(-30.927f, 9.851f, 18));
            billboardList.Add(new Vector3(-28.427f, -1.399f, 5));
            billboardList.Add(new Vector3(-10.927f, -22.649f, 15));
            billboardList.Add(new Vector3(6.323f, -37.199f, 0));
            billboardList.Add(new Vector3(6.323f, 0.851f, 15));
            billboardList.Add(new Vector3(6.323f, 16.331f, 14));

            for(int i = 0; i < 20; i++)
                tubeList.Add(new Tube(new Vector3(-49.927f + i, 9.851f, 9), true, false));
            for(int i = 0; i < 5; i++)
                tubeList.Add(new Tube(new Vector3(-28.427f, -16.149f - i, -3), false, true));
            for(int i = 0; i < 14; i++)
                tubeList.Add(new Tube(new Vector3(-29.927f + i, -22.649f, -3), true, false));
            for(int i = 0; i < 10; i++)
            {
                tubeList.Add(new Tube(new Vector3(-5.927f + i, -22.649f, 7), true, false));
                tubeList.Add(new Tube(new Vector3(6.323f, 24.871f + i, 6), false, false));
            }
            for(int i = 0; i < 19; i++)
                tubeList.Add(new Tube(new Vector3(6.323f, -10.269f + i, 6), false, false));
            for(int i = 0; i < 17; i++)
                tubeList.Add(new Tube(new Vector3(4.823f + i, 36.371f, 6), true, false));

            machs.Add(new TranslateMachine(1, 5, new Vector3(0, 0, -6.2f), 1.25f, false,
                new BaseModel(delegate { return modelList[6]["machine1_glass"]; }, true, null, new Vector3(-30.927f, 9.851f, 12.07f))) , true);

            BaseModel ww = new BaseModel(delegate { return modelList[6]["waterwheel"]; }, false, null, new Vector3(-28.927f, 9.851f, 3));
            ww.Ent.Material = new BEPUphysics.Materials.Material(0.25f, 0.25f, 0);
            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.Pi), 10));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.Pi), 10));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true, ww), false);

            tubeY = 4.351f;
            machs.Add(new TranslateMachine(2, 2, new Vector3(0, -10.5f, 0), 2.5f, false,
                new BaseModel(delegate { return modelList[6]["machine2_base"]; }, false, null, new Vector3(-28.427f, 0.85108f, -2.7f)),
                new BaseModel(delegate { return modelList[6]["machine2_glass"]; }, true, null, new Vector3(-28.427f, 0f, 0.6f)),
                new BaseModel(delegate { return modelList[6]["machine2_stripes"]; }, false, null, new Vector3(-28.35680f, -0.19892f, -2.65f)),
                new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
                new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
                new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
                new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
                new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true)), true);

            tubeY = -15.927f;
            machs.Add(new TranslateMachine(3, 7, new Vector3(0, 0, 10), 1.5f, false,
                new BaseModel(delegate { return modelList[6]["machine3"]; }, false, null, new Vector3(-11.196f, -22.649f, -8.103f)),
                new BaseModel(delegate { return modelList[6]["machine3_stripes"]; }, false, null, new Vector3(-11.4268f, -22.64892f, -2.9f)),
                new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
                new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
                new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
                new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
                new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false)), true);

            machs.Add(new TranslateMachine(4, 10, new Vector3(0, 25.461f, 14.7f), 4.5f, false,
                new BaseModel(delegate { return modelList[6]["machine4"]; }, false, null, new Vector3(6.323f, -37.199f, -4.65f))), true);

            machs.Add(new TranslateMachine(5, 7, new Vector3(0, 0, -6.2f), 1.25f, false,
                new BaseModel(delegate { return modelList[6]["machine6_glass"]; }, true, null, new Vector3(6.323f, 0.851f, 9.6f))), true);

            tubeY = 8.831f;
            m1 = new TranslateMachine(6, 5, new Vector3(0, 0, 6), 1.25f, false,
                new BaseModel(delegate { return modelList[6]["machine5_part1_base"]; }, false, null, new Vector3(6.323f, 12.351f, 3.167f)),
                new BaseModel(delegate { return modelList[6]["machine5_part1_glass"]; }, true, null, new Vector3(6.323f, 12.351f, 6.6f)),
                new BaseModel(delegate { return modelList[6]["machine5_part1_stripes"]; }, true, null, new Vector3(6.323f, 12.351f, 3.1f)),
                new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false),
                new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false),
                new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false),
                new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false));
            m2 = new TranslateMachine(6, -1, new Vector3(0, 0, -6), 1.25f, false,
                new BaseModel(delegate { return modelList[6]["machine5_part2_base"]; }, false, null, new Vector3(6.323f, 20.351f, 9.167f)),
                new BaseModel(delegate { return modelList[6]["machine5_part2_glass"]; }, true, null, new Vector3(6.323f, 20.351f, 12.6f)),
                new BaseModel(delegate { return modelList[6]["machine5_part2_stripes"]; }, true, null, new Vector3(6.323f, 20.351f, 9.1f)),
                new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false),
                new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false),
                new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false),
                new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false));

            OperationalMachine.LinkMachines(m1, m2);
            machs.Add(m1, true);
            machs.Add(m2, false);

            level06 = new Level6(50, 10, new Vector3(-46.927f, 9.851f, 17), billboardList, (BaseModel)delegate { return modelList[6]["base"]; }, (BaseModel)delegate { return modelList[6]["extras"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[6]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[6]["glass2"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(24.573f, 36.351f, 1.862f)), machs, tubeList,
                new LevelCompletionData(new TimeSpan(0, 8, 45), 6300, 8), "Operation: Pretzel Cup", null);

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 7
            // Level 7 offset is: (-10.102, -16.072, -1.026)
            // offset is: level before offset - level after offset
            Program.Cutter.WriteToLog(this, "Creating level 7");

            billboardList.Add(new Vector3(-36.5f, -13, 18));
            billboardList.Add(new Vector3(-42, -17, 10));
            billboardList.Add(new Vector3(-19, -17, 12));
            billboardList.Add(new Vector3(-10, 5, 3));
            billboardList.Add(new Vector3(2, 35, 5));
            billboardList.Add(new Vector3(4, -11, 11));
            billboardList.Add(new Vector3(32.5f, -4, 10));

            offset = new Vector3(-10.102f, -16.072f, -1.026f);
            Vector3 tubeTemp = new Vector3(-46.75f, -10.75f, 7);
            for(Vector3 i = Vector3.Zero; i.Y < 19.53f; i.Y += 0.93f)
                tubeList.Add(new Tube(tubeTemp - i - offset, false, true));
            tubeTemp = new Vector3(-20.065f, -35.55f, -4);
            for(Vector3 i = Vector3.Zero; i.Y < 20; i.Y += 1)
                tubeList.Add(new Tube(tubeTemp + i - offset, false, false));
            tubeTemp = new Vector3(-20.565f, 9.05f, 5);
            for(Vector3 i = Vector3.Zero; i.X < 10; i.X += 1)
                tubeList.Add(new Tube(tubeTemp + i - offset, true, false));
            tubeTemp = new Vector3(10, -20, -1);
            for(Vector3 i = Vector3.Zero; i.X < 20; i.X += 1)
                tubeList.Add(new Tube(tubeTemp + i - offset, true, false));

            //machs.Add(new TranslateMachine(1, new Vector3(0, 0, 7), 1f, false,
            //    new BaseModel(level07Dict["machine1_glass"]; }, true, null, new Vector3(-36.697f, -13.1f, 10.1f), true)), true);
            //machs.Add(new RotateMachine(1, 1.5f, new Vector3(-90, 0, 0), RotateMachine.RotationType.Clamped, new Vector3(-36.697f, -13.1f, 13.1f),
            //    new BaseModel(level07Dict["machine1_glass"]; }, true, null, new Vector3(-36.697f, -13.1f, 16.1f), true)), true);
            machs.Add(new TranslateMachine(1, 10, new Vector3(0, 0, -5.5f), 1f, false,
                new BaseModel(delegate { return modelList[7]["machine1_glass"]; }, true, null, new Vector3(-36.697f, -13.1f, 10.1f))) , true);

            Tube[] keyTubes = new Tube[7];
            for(int i = 0; i < 7; i++)
            {
                keyTubes[i] = new Tube(new Vector3(-38.259f + i, -17.211f, 5.521f), true, false);
                keyTubes[i].BecomeKeybasedTube(3);
            }

            machs.Add(new TranslateMachine(2, 9, new Vector3(13.4f, 0, 0), 3.5f, false,
                new BaseModel(delegate { return modelList[7]["machine2_glass"]; }, true, null, new Vector3(-38.64f, -17.211f, 7.721f)), keyTubes[0], keyTubes[1],
                keyTubes[2], keyTubes[3], keyTubes[4], keyTubes[5], keyTubes[6]), true);

            // dummy machine to work with the above machine
            machs.Add(new HoldRotationMachine(3, 3, Vector3.Zero), true);

            List<Vector3> vectorList = new List<Vector3>();
            for(int i = 0; i < 5; i++)
                vectorList.Add(new Vector3(-12.15f - i, -33.25f, 4) - offset);
            for(int i = 0; i < 5; i++)
                vectorList.Add(new Vector3(-20.05f, -25.65f - i, 4) - offset);
            for(int i = 0; i < 5; i++)
                vectorList.Add(new Vector3(-27.65f + i, -33.25f, 4) - offset);
            for(int i = 0; i < 5; i++)
                vectorList.Add(new Vector3(-20.05f, -40.85f + i, 4) - offset);
            machs.Add(new ContinuingRotationMachine(0, -1, -Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 13.5f, 4, new Vector3(-20.05f, -33.25f, 4.0f) - offset,
                new BaseModel(delegate { return modelList[7]["machine3"]; }, false, null, new Vector3(-20.05f, -33.25f, 4.0f) - offset),
                new Tube(vectorList[0], true, true), new Tube(vectorList[1], true, true), new Tube(vectorList[2], true, true), new Tube(vectorList[3], true, true),
                new Tube(vectorList[4], true, true), new Tube(vectorList[5], false, true), new Tube(vectorList[6], false, true), new Tube(vectorList[7], false, true),
                new Tube(vectorList[8], false, true), new Tube(vectorList[9], false, true), new Tube(vectorList[10], true, false), new Tube(vectorList[11], true, false),
                new Tube(vectorList[12], true, false), new Tube(vectorList[13], true, false), new Tube(vectorList[14], true, false), new Tube(vectorList[15], false, false),
                new Tube(vectorList[16], false, false), new Tube(vectorList[17], false, false), new Tube(vectorList[18], false, false), new Tube(vectorList[19], false, false)) { DampingMultiplier = 10 }, false);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(new Vector3(0, 16.5f, 16.5f), Quaternion.Identity, 5));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), 1.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, .75f));
            k1 = new KeyframeMachine(4, -1, keyframeList, false,
                new BaseModel(delegate { return modelList[7]["machine4"]; }, false, null, new Vector3(-9.985f, 3.5f, -5f)));

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(new Vector3(0, 16.5f, 16.5f), Quaternion.Identity, 5));
            //keyframeList.Add(new Keyframe(new Vector3(0, 3.15f, 0), Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), 3));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), 1.5f));
            keyframeList.Add(new Keyframe(new Vector3(0, 3.15f, 0), Quaternion.Identity, .75f));
            k2 = new KeyframeMachine(4, 2, keyframeList, false,
                new BaseModel(delegate { return modelList[7]["machine4_part2"]; }, false, null, new Vector3(-9.987f, 3.5f, -6.8f)) { LocalOffset = new Vector3(0, 0, -1.6f) });

            OperationalMachine.LinkMachines(k1, k2);
            machs.Add(k1, true);
            machs.Add(k2, false);

            machs.Add(new TranslateMachine(5, 1, new Vector3(0, -35.532f, 14.497f), 5, false,
                new BaseModel(delegate { return modelList[7]["machine5"]; }, false, null, new Vector3(2.419f, 36.908f, -2.39f))), true);

            machs.Add(new ClampedRotationMachine(6, 0, 4, Vector3.UnitZ, MathHelper.ToRadians(179), new Vector3(12.048f, -6.85f, 4.247f), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[7]["machine6"]; }, false, null, new Vector3(12.048f - 10, -6.85f, 4.247f))), true);

            //machs.Add(new TranslateMachine(7, new Vector3(0, 0, 6.2f), 1f, false,
            //    new BaseModel(level07Dict["machine7_part1_glass"]; }, true, null, new Vector3(32.478f, -3.789f, 3.305f), true)), true);
            //machs.Add(new RotateMachine(7, 1.5f, new Vector3(0, -90, 0), RotateMachine.RotationType.Clamped, new Vector3(32.478f, -3.789f, 6.15f),
            //    new BaseModel(level07Dict["machine7_part1_glass"]; }, true, null, new Vector3(32.478f, -3.789f, 9.15f), true)), true);
            machs.Add(new TranslateMachine(7, 10, new Vector3(0, 0, -6.2f), 1f, false,
                new BaseModel(delegate { return modelList[7]["machine7_part1_glass"]; }, true, null, new Vector3(32.478f, -3.789f, 3.305f))) , true);

            BaseModel bm = new BaseModel(delegate { return modelList[7]["machine7_part2"]; }, true, null, new Vector3(35.191f, -3.998f, 1.189f));
            bm.Remover = true;
            //bm.SetRenderOptions(content.Load<Effect>("Shaders/lava"), LaserTex, content.Load<Texture2D>("textures/noise"));
            machs.Add(new TranslateMachine(7, -1, new Vector3(0, 0, 4.8f), 5, true, bm), false);

            level07 = new Level(7, 30, 10, new Vector3(-36.5f, 3, 20), billboardList, Theme.Beach, (BaseModel)delegate { return modelList[7]["base"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[7]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass2"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[7]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass4"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[7]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass6"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[7]["glass7"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass8"]; }, true, false, Vector3.Zero),
                    new BaseModel(delegate { return modelList[7]["glass9"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(43.102f, -3.928f, -9f)), null, machs, tubeList, 
                new LevelCompletionData(new TimeSpan(0, 6, 5), 4500, 7), "Sun, Surf, and\n    Snackie-Snacks");

            machs.Clear();
            keyframeList.Clear();
            tubeList.Clear();
            billboardList.Clear();
            #endregion

            #region Level 8
            Program.Cutter.WriteToLog(this, "Creating level 8");

            billboardList.Add(new Vector3(6.079f, -3.424f, 16));
            billboardList.Add(new Vector3(37.24f, -29.924f, 5));
            billboardList.Add(new Vector3(-6.024f, -29.924f, 1));
            billboardList.Add(new Vector3(-31.2f, 0.861f, 8));
            billboardList.Add(new Vector3(-29.421f, 17.576f, 1));
            billboardList.Add(new Vector3(-11.321f, 35.076f, 17));
            billboardList.Add(new Vector3(17.408f, 35.151f, 31));
            billboardList.Add(new Vector3(39.079f, 15.076f, 6.5f));

            for(int i = 0; i < 20; i++)
            {
                tubeList.Add(new Tube(new Vector3(-24.421f + i, -3.424f, 7), true, false)); // first set
                tubeList.Add(new Tube(new Vector3(12.979f - i, -29.924f, -4), true, true)); // fifth set
                tubeList.Add(new Tube(new Vector3(-20.021f - i, -29.924f, 10), true, true)); // sixth set
                tubeList.Add(new Tube(new Vector3(-42.771f, -32.174f + i, 6), false, false)); // seventh set
                tubeList.Add(new Tube(new Vector3(-29.421f, -1.424f + i, -4), false, false)); // eighth set
                tubeList.Add(new Tube(new Vector3(-31.321f + i, 35.076f, 10), true, false)); // ninth set
            }
            for(int i = 0; i < 28; i++)
                tubeList.Add(new Tube(new Vector3(10.707f + i, -3.424f, -2), true, false)); // second set
            for(int i = 0; i < 27; i++)
                tubeList.Add(new Tube(new Vector3(39.579f, -1.924f - i, -2), false, true)); // third set
            for(int i = 0; i < 5; i++)
                tubeList.Add(new Tube(new Vector3(41.24f - i, -29.924f, -2), true, true)); // fourth set
            for(int i = 0; i < 22; i++)
                tubeList.Add(new Tube(new Vector3(16.079f + i, 35.076f, -1), true, false)); // tenth set
            for(int i = 0; i < 19; i++)
                tubeList.Add(new Tube(new Vector3(39.079f, 36.576f - i, -1), false, true)); // last (eleventh) set

            machs.Add(new ClampedRotationMachine(1, 1, 2.5f, Vector3.UnitY, MathHelper.ToRadians(179), new Vector3(6.079f, -3.424f, 3), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[8]["machine1_base"]; }, false, null, new Vector3(6.079f, -3.424f, 3) + new Vector3(-4.629f, 0, -2.324f)),
                new BaseModel(delegate { return modelList[8]["machine1_glass"]; }, true, null, new Vector3(-0.421f, -3.424f, 0))) { DampingMultiplier = 5 }, true);

            machs.Add(new TranslateMachine(2, 7, new Vector3(-13.78359f, 0, 15.6f), 2.75f, false,
                new BaseModel(delegate { return modelList[8]["machine2"]; }, false, null, new Vector3(37.57499f, -29.425f, -6.65f))), true);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 6));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 6));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[8]["spiked_roller"]; }, false, null, new Vector3(12.1f, -29.824f, 1.3f))), false);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.Pi), 6));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.Pi), 6));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[8]["spiked_roller"]; }, false, null, new Vector3(6.9f, -29.824f, 1.3f))), false);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(new Vector3(0, 0, 18.5f), Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            keyframeList.Add(new Keyframe(new Vector3(0, 0, 2f), Quaternion.Identity, 0.5f));
            BaseModel glassTemp = new BaseModel(delegate { return modelList[8]["machine3_glass"]; }, true, null, new Vector3(-9.872f, -29.424f, -6.633f));
            glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-16.321f, -29.424f, -6.024f);
            glassTemp.Ent.Position = new Vector3(-16.321f, -24.924f, -6.024f);
            machs.Add(new KeyframeMachine(3, -1, keyframeList, false, glassTemp), false);
            
            keyframeList.Clear();
            keyframeList.Add(new Keyframe(new Vector3(0, 0, 18.5f), Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 0.5f));
            glassTemp = new BaseModel(delegate { return modelList[8]["machine3_base"]; }, false, null, new Vector3(-11.7f, -29.424f, -8.296f));
            glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-16.321f, -29.424f, -6.024f);
            glassTemp.Ent.Position = new Vector3(-16.321f, -29.424f, -6.024f);
            machs.Add(new KeyframeMachine(3, 9, keyframeList, false, glassTemp), true);
            keyframeList.Clear();

            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 8));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 8));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[8]["machine4_dome"]; }, false, null, new Vector3(-35.424f, -7.424f, 3.5f))), false);
            keyframeList.Clear();
            
            keyframeList.Add(new Keyframe(-Vector3.UnitZ, Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(new Vector3(-2, -3, 0), Quaternion.Identity, 1f));
            machs.Add(new KeyframeMachine(4, 8, keyframeList, false,
                new BaseModel(delegate { return modelList[8]["machine4_slide"]; }, false, null, new Vector3(-31.2f, 0.861f, 0.5f))), true);
            keyframeList.Clear();

            keyframeList.Add(new Keyframe(new Vector3(0, 0, 17.5f), Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 2.25f));
            keyframeList.Add(new Keyframe(new Vector3(0, 0, 2f), Quaternion.Identity, 0.5f));
            glassTemp = new BaseModel(delegate { return modelList[8]["machine5_glass"]; }, true, null, new Vector3(-29.231f, 21.626f, -5.774f));
            glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-29.321f, 28.076f, -5.131f);
            glassTemp.Ent.Position = new Vector3(-29.321f, 28.076f, -5.131f);
            machs.Add(new KeyframeMachine(5, -1, keyframeList, false, glassTemp), false);
            
            keyframeList.Clear();
            keyframeList.Add(new Keyframe(new Vector3(0, 0, 17.5f), Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 2.25f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 0.5f));
            glassTemp = new BaseModel(delegate { return modelList[8]["machine5_base"]; }, false, null, new Vector3(-29.321f, 23.821f, -7.803f));
            glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-29.321f, 28.076f, -5.131f);
            glassTemp.Ent.Position = new Vector3(-29.321f, 28.076f, -5.131f);
            machs.Add(new KeyframeMachine(5, 9, keyframeList, false, glassTemp), true);
            keyframeList.Clear();

            machs.Add(new TranslateMachine(6, 7, new Vector3(27.95f, 0, 17.735f), 4, false,
                new BaseModel(delegate { return modelList[8]["machine6"]; }, false, null, new Vector3(-18.503f, 35.07602f, 5.1f)) { LocalOffset = new Vector3(-2.616649f, 0, -1.590488f) }), true);

            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.75f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.75f));
            machs.Add(new KeyframeMachine(7, 9, keyframeList, true,
                new BaseModel(delegate { return modelList[8]["machine7_rim"]; }, false, null, new Vector3(17.408f, 35.147f, 15.9f)),
                new BaseModel(delegate { return modelList[8]["machine7_glass"]; }, true, null, new Vector3(17.408f, 35.147f, 15.9f))), true);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(new Vector3(-5.6f, 0, 0), Quaternion.Identity, 0.3f));
            keyframeList.Add(new Keyframe(new Vector3(5.5f, 0, 0), Quaternion.Identity, 0.6f));
            machs.Add(new KeyframeMachine(8, 8, keyframeList, true,
                new BaseModel(delegate { return modelList[8]["machine8"]; }, false, null, new Vector3(41.779f, 15.076f, -1.5f))) { NoStop = true }, true);

            //machs.Add(new ContinuingRotationMachine(0, -1, Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 0, 0.5f,
            //    new Vector3(27.579f, -3.424f, 2), new BaseModel(delegate { return modelList[8]["fan"]; }, false, null, new Vector3(27.579f, -3.424f, 2))), false);
            //machs.Add(new ContinuingRotationMachine(0, -1, Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 0, 0.5f,
            //    new Vector3(31.079f, -3.424f, 3), new BaseModel(delegate { return modelList[8]["fan"]; }, false, null, new Vector3(31.079f, -3.424f, 3))), false);
            //machs.Add(new ContinuingRotationMachine(0, -1, Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 0, 0.5f,
            //    new Vector3(34.579f, -3.424f, 2), new BaseModel(delegate { return modelList[8]["fan"]; }, false, null, new Vector3(34.579f, -3.424f, 2))), false);

            level08 = new Level(8, 50, 10, new Vector3(-22.421f, -3.424f, 16), billboardList, Theme.Beach, (BaseModel)delegate { return modelList[8]["base"]; },
                new[] { new BaseModel(delegate { return modelList[8]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass2"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[8]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass4"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[8]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass6"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[8]["glass7"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass8"]; }, true, false, Vector3.Zero),
                    new BaseModel(delegate { return modelList[8]["glass9"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass10"]; }, true, false, Vector3.Zero),
                    new BaseModel(delegate { return modelList[8]["glass11"]; }, true, false, Vector3.Zero) },
                    new Goal(new Vector3(31.279f, 15.076f, -9.4f)), null, machs, tubeList,
                    new LevelCompletionData(new TimeSpan(0, 8, 10), 3300, 7), "Pressure Gauge");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 9
            Program.Cutter.WriteToLog(this, "Creating level 9");

            // elongated set (thirteenth)
            tubeList.Add(new Tube(new Vector3(-27.092f, 29.201f, -3.3f), false, false));
            for(int i = 0; i < 4; i++)
            {
                tubeList.Add(new Tube(new Vector3(-25.092f, 30.021f + i, -3.3f), false, false));
                tubeList.Add(new Tube(new Vector3(-29.092f, 30.021f + i, -3.3f), false, false));
            }
            for(int i = 0; i < 5; i++)
            {
                tubeList.Add(new Tube(new Vector3(-24.592f, 34.201f + i, -3.3f), false, false)); // fourteenth set (inside box, shorter, before turn)
                tubeList.Add(new Tube(new Vector3(18.408f, 47.201f - i, -5.3f), false, true)); // twenty-second set
                tubeList.Add(new Tube(new Vector3(18.408f, 42.201f - i, -0.3f), false, true)); // twenty-third set
            }
            for(int i = 0; i < 6; i++)
            {
                tubeList.Add(new Tube(new Vector3(-33.592f + i, 8.701f, 4.2f), true, false)); // second set
                tubeList.Add(new Tube(new Vector3(-23.592f + i, 3.701f, 4.2f), true, false)); // third set
                tubeList.Add(new Tube(new Vector3(-26.072f + i, 40.701f, -3.3f), true, false)); // fifteenth set (inside box, shorter, after turn)
                tubeList.Add(new Tube(new Vector3(-14.092f + i, 40.701f, -5.3f), true, false)); // eighteenth set
            }
            for(int i = 0; i < 23; i++)
                tubeList.Add(new Tube(new Vector3(-2.592f - i, 21.701f, -3.3f), true, true)); // eleventh set
            for(int i = 0; i < 24; i++)
                tubeList.Add(new Tube(new Vector3(-13.592f + i, 3.701f, 4.2f), true, false)); // fourth set
            for(int i = 0; i < 7; i++)
            {
                tubeList.Add(new Tube(new Vector3(34.408f + i, 3.701f, 1.2f), true, false)); // fifth set
                tubeList.Add(new Tube(new Vector3(9.908f + i, 45.701f, -5.3f), true, false)); // twenty-first set
            }
            for(int i = 0; i < 18; i++)
            {
                tubeList.Add(new Tube(new Vector3(42.908f, 5.201f - i, 1.2f), false, true)); // sixth set
                tubeList.Add(new Tube(new Vector3(-14.092f + i, 45.701f, -5.3f), true, false)); // ninteenth set
            }
            for(int i = 0; i < 51; i++)
                tubeList.Add(new Tube(new Vector3(44.408f - i, -14.299f, 1.2f), true, true)); // seventh set
            for(int i = 0; i < 21; i++)
            {
                Tube t = new Tube(new Vector3(-8.921f + (0.245211f * i), -9.444f + (0.910518f * i), -1.391f - (0.095684f * i)), false, true); 
                t.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(255));
                tubeList.Add(t); // ninth set (slanted)
            }
            for(int i = 0; i < 10; i++)
                tubeList.Add(new Tube(new Vector3(-29.592f, 34.201f + i, -3.3f), false, false)); // sixteenth set (inside box, longer, before turn)
            for(int i = 0; i < 9; i++)
            {
                tubeList.Add(new Tube(new Vector3(-9.092f, -17.799f + i, -1.3f), false, false)); // eighth set
                tubeList.Add(new Tube(new Vector3(-27.092f, 20.201f + i, -3.3f), false, false)); // twelveth set
            }
            for(int i = 0; i < 11; i++)
                tubeList.Add(new Tube(new Vector3(-31.092f + i, 45.701f, -3.3f), true, false)); // seventeenth set (inside box, longer, after turn)
            for(int i = 0; i < 17; i++)
                tubeList.Add(new Tube(new Vector3(-49.592f + i, 8.701f, 7.2f), true, false)); // first set
            for(int i = 0; i < 11; i++)
                tubeList.Add(new Tube(new Vector3(-4.092f, 9.201f + i, -3.3f), false, false)); // tenth set
            for(int i = 0; i < 12; i++)
                tubeList.Add(new Tube(new Vector3(-2.092f + i, 40.701f, -0.3f), true, false)); // twentieth set
            for(int i = 0; i < 25; i++)
                tubeList.Add(new Tube(new Vector3(18.408f, 42.247f - i, -5.255f), false, true)); // last set
            tubeList.Add(new Tube(new Vector3(15.908f, 40.701f, -0.3f), true, false));
            //tubeList.Add(new Tube(new Vector3(-2.555f, 8.332f, -3.31f), false, false) { Height = 2.182f, Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-100)) }); // first tilted tube
            //tubeList.Add(new Tube(new Vector3(-10.519f, -9.931f, -1.29f), false, false) { Height = 2.182f, Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-100)) }); // second tilted tube

            billboardList.Add(new Vector3(-33.592f, 8.701f, 16f));
            billboardList.Add(new Vector3(-21.092f, 8.701f, 8));
            billboardList.Add(new Vector3(2.408f, 0, 8.5f));
            billboardList.Add(new Vector3(6.994f, 3.701f, 11));
            billboardList.Add(new Vector3(30.408f, 3.701f, 7));
            billboardList.Add(new Vector3(10.908f, -13.5f, 5.5f));
            billboardList.Add(new Vector3(-17.592f, 45.701f, -1));
            billboardList.Add(new Vector3(-5.592f, 40.701f, 12));
            billboardList.Add(new Vector3(6.408f, 45.701f, -1));

            machs.Add(new TranslateMachine(1, 5, new Vector3(0, 0, -6.5f), 1.25f, false,
                new BaseModel(delegate { return modelList[9]["machine1_glass"]; }, true, null, new Vector3(-33.592f, 8.701f, 10.4f))) , true);

            tubeY = -27.592f;
            machs.Add(new TranslateMachine(2, 7, new Vector3(0, -5, 0), 1f, false,
                new BaseModel(delegate { return modelList[9]["machine2"]; }, false, null, new Vector3(-21.092f, 8.701f, 4.36f)),
                new BaseModel(delegate { return modelList[9]["machine2_stripes"]; }, false, null, new Vector3(-21.092f, 8.701f, 4.4f)),
                new BaseModel(delegate { return modelList[9]["machine2_glass"]; }, true, null, new Vector3(-21.01690f, 8.62007f, 7.43633f)),
                new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false),
                new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false),
                new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false), 
                new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false)), true);

            keyframeList.Add(new Keyframe(new Vector3(0, 5, 0), Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(new Vector3(0, KeyframeMachine.DelayConstant, 0), Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 6f));
            keyframeList.Add(new Keyframe(new Vector3(0, -KeyframeMachine.DelayConstant, 0), Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(new Vector3(0, -5, 0), Quaternion.Identity, 1f));
            k1 = new KeyframeMachine(3, 10, keyframeList, true,
                new BaseModel(delegate { return modelList[9]["machine3"]; }, false, null, new Vector3(2.408f, -2.243f, 4.359f))) { NoStop = true };
            Cannon c = new Cannon(3, 8, new Vector3(0, 0, 6), Vector3.UnitX, new CannonPathFinder(new Vector3(0, -9, 9), new Vector3(2.408f, 10.667f, 5.25f), new Vector3(2.408f, -44.524f, 5.25f)), new Vector3(2.408f, 10.667f, 5.25f), //220,
                new BaseModel(delegate { return modelList[9]["machine3_cannon"]; }, false, null, new Vector3(2.408f, 9.327f, 0.7f)));
            c.SetInputs(k1);
            c.SetActivationType(ActivationType.JustDeactivated);
            machs.Add(c, false);
            machs.Add(k1, true);
            keyframeList.Clear();

            machs.Add(new TranslateMachine(4, 0, new Vector3(19.5f, 0, 13.5f), 3f, false,
                new BaseModel(delegate { return modelList[9]["machine4"]; }, false, null, new Vector3(6.994f, 3.701f, -2.209f))), true);

            machs.Add(new TranslateMachine(5, 2, new Vector3(0, 0, 11f), 3.5f, false,
                new BaseModel(delegate { return modelList[9]["machine5"]; }, false, null, new Vector3(30.408f, 3.701f, -7.85f))), true);

            machs.Add(new ClampedRotationMachine(6, 2, 0.5f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(22.908f, -16.799f, 2.8f), Vector3.UnitY,
                new BaseModel(delegate { return modelList[9]["machine6_slant"]; }, false, null, new Vector3(22.908f, -16.799f, 2.8f))), true);
            machs.Add(new ClampedRotationMachine(6, -1, 0.5f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(10.908f, -11.799f, 2.8f), Vector3.UnitY,
                new BaseModel(delegate { return modelList[9]["machine6_flat"]; }, false, null, new Vector3(10.908f, -11.799f, 2.8f))), false);
            machs.Add(new ClampedRotationMachine(6, -1, 0.5f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-1.902f, -16.799f, 2.8f), Vector3.UnitY,
                new BaseModel(delegate { return modelList[9]["machine6_slant"]; }, false, null, new Vector3(-1.902f, -16.799f, 2.8f))), false);

            keyframeList.Add(new Keyframe(new Vector3(0, -14.5f, 2.7f), Quaternion.Identity, 3.5f));
            keyframeList.Add(new Keyframe(new Vector3(0, 0, -2.5f), Quaternion.Identity, 1));
            keyframeList.Add(new Keyframe(new Vector3(0, 14.5f, -2.7f), Quaternion.Identity, 3.5f));
            keyframeList.Add(new Keyframe(new Vector3(0, 0, 2.5f), Quaternion.Identity, 1));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[9]["auto_belt"]; }, false, null, new Vector3(2.408f, -17.544f, 0.66f))), false);
            keyframeList.Clear();

            machs.Add(new ClampedRotationMachine(0, -1, 1.5f, Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-27.092f, 33.201f, -1.7f), -Vector3.UnitX,
                new BaseModel(delegate { return modelList[9]["auto_switcher"]; }, false, null, new Vector3(-27.092f, 33.201f, -1.7f))), false);

            tubeY = -20.092f;
            machs.Add(new TranslateMachine(7, 9, new Vector3(0, -5, 0), 1.5f, false,
                new BaseModel(delegate { return modelList[9]["machine7"]; }, false, null, new Vector3(-17.592f, 45.701f, -4.733f)),
                new BaseModel(delegate { return modelList[9]["machine7_stripes"]; }, false, null, new Vector3(-17.592f, 45.701f, -5.1f)),
                new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
                new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
                new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false)), true);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Vector3.UnitZ * 14, Quaternion.Identity, 2.5f));
            keyframeList.Add(new Keyframe(Vector3.UnitX * 2, Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.PiOver2), 1f));
            machs.Add(new KeyframeMachine(8, 4, keyframeList, false,
                new BaseModel(delegate { return modelList[9]["machine8"]; }, false, null, new Vector3(-5.592f, 40.701f, -5.298f))), true);
            keyframeList.Clear();

            tubeY = 3.908f;
            keyframeList.Add(new Keyframe(Vector3.UnitY * -5, Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(Vector3.UnitX * 6, Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(Vector3.UnitZ * 5, Quaternion.Identity, 1f));
            machs.Add(new KeyframeMachine(9, 10, keyframeList, false,
                new BaseModel(delegate { return modelList[9]["machine9"]; }, false, null, new Vector3(6.408f, 45.701f, -4.733f)),
                new BaseModel(delegate { return modelList[9]["machine9_stripes"]; }, false, null, new Vector3(6.408f, 45.701f, -5.1f)),
                new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
                new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
                new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false)), true);

            level09 = new Level9(40, 10, new Vector3(-46.592f, 8.701f, 13), billboardList, (BaseModel)delegate { return modelList[9]["base"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[9]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[9]["glass2"]; }, true, false, Vector3.Zero),
                (BaseModel)delegate { return modelList[9]["flags"]; } }, new Goal(new Vector3(18.414f, 14.703f, -9.4f)),
                new Goal(new Vector3(2.312f, -44.524f, -7), Color.Blue), machs, tubeList,
                new LevelCompletionData(new TimeSpan(0, 7, 10), 3200, 11), "No. No there is not.");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 10
            Program.Cutter.WriteToLog(this, "Creating level 10");

            for(int i = 0; i < 20; i++)
                tubeList.Add(new Tube(new Vector3(-47.047f + i, 16.124f, 21), true, false)); // first set
            for(int i = 0; i < 4; i++)
                tubeList.Add(new Tube(new Vector3(-22.04736f - i, -12.876f, 18), true, true)); // second set
            for(int i = 0; i < 13; i++)
                tubeList.Add(new Tube(new Vector3(-27.54736f, -11.376f - i, 18), false, true)); // third set
            for(int i = 0; i < 8; i++)
                tubeList.Add(new Tube(new Vector3(-29.04736f + i, -25.876f, 18), true, false)); // fourth set
            for(int i = 0; i < 32; i++)
                tubeList.Add(new Tube(new Vector3(-9.04736f + i, -25.87586f, 1), true, false)); // inserted set 1
            for(int i = 0; i < 8; i++)
                tubeList.Add(new Tube(new Vector3(24.45264f, -27.37586f + i, 1), false, false)); // inserted set 2
            for(int i = 0; i < 25; i++)
                tubeList.Add(new Tube(new Vector3(18.953f + i, -4.134f, 1.5f), true, false)); // long set part 1

            for(int i = 0; i < 20; i++)
                tubeList.Add(new Tube(new Vector3(189.953f + i, -4.134f, 1.5f), true, false)); // long set part 2
            for(int i = 0; i < 27; i++)
                tubeList.Add(new Tube(new Vector3(243.007f, -19.33f - i, 0), false, true)); // sixth set
            for(int i = 0; i < 38; i++)
                tubeList.Add(new Tube(new Vector3(242.953f, 0.624f + i, 5), false, false)); // seventh set
            for(int i = 0; i < 2; i++)
            {
                tubeList.Add(new Tube(new Vector3(254.703f + i, -3.884f, -0.5f), true, false)); // eighth set
                tubeList.Add(new Tube(new Vector3(274.703f + i, -3.884f, -0.5f), true, false)); // ninth set
                tubeList.Add(new Tube(new Vector3(294.703f + i, -3.884f, -0.5f), true, false)); // tenth set
            }

            billboardList.Add(new Vector3(-44.979f, 16.116f, 39));
            billboardList.Add(new Vector3(-6.279f, 16.116f, 22));
            billboardList.Add(new Vector3(0.203f, 9.616f - 14, 18));
            billboardList.Add(new Vector3(-19.64830f, -25.884f, 22f));
            billboardList.Add(new Vector3(24.45264f, -13.52587f, 16));
            billboardList.Add(new Vector3(208.453f, -4.184f, 9.5f));
            billboardList.Add(new Vector3(243.005f, -4.184f, 13));
            billboardList.Add(new Vector3(242.999f, -9.08f, 9));
            billboardList.Add(new Vector3(242.408f, 21.116f, 12.5f));
            billboardList.Add(new Vector3(242.953f, 27.116f, 12));

            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            machs.Add(new KeyframeMachine(1, 2, keyframeList, true,
                new BaseModel(delegate { return modelList[10]["machine1"]; }, false, null, new Vector3(-44.797f, 16.116f, 32))), true);
            keyframeList.Clear();

            machs.Add(new RailMachine(2, 6, true, new Vector3(-17, 0, 0), 7.5f, 3.5f, new Vector3(-7.247f, 16.116f, 17.45f), 2.5f, Vector3.UnitY, MathHelper.ToRadians(110),
                -Vector3.UnitX,
                new[] { new BaseModel(delegate { return modelList[10]["machine2_base"]; }, false, null, new Vector3(-7.247f, 16.116f, 13.788f)) },
                new BaseModel(delegate { return modelList[10]["machine2_bucket"]; }, false, null, new Vector3(-7.247f, 16.116f, 16.713f)),
                new BaseModel(delegate { return modelList[10]["machine2_glass"]; }, true, null, new Vector3(-7.247f, 16.116f, 18.45f))), true);

            machs.Add(new RailMachine(3, 4, true, new Vector3(0, -23.75f, 0), 7.5f, 3.5f, new Vector3(-1.797f, 16.116f, 10.5f), 2.5f, Vector3.UnitX, MathHelper.ToRadians(110),
                Vector3.UnitY,
                new[] { new BaseModel(delegate { return modelList[10]["machine3_base"]; }, false, null, new Vector3(-1.797f, 16.116f, 8.014f)) },
                new BaseModel(delegate { return modelList[10]["machine3_bucket"]; }, false, null, new Vector3(-1.797f, 16.116f, 9.958f)),
                new BaseModel(delegate { return modelList[10]["machine3_glass"]; }, true, null, new Vector3(-1.797f, 16.116f, 11.45f))), true);

            machs.Add(new TranslateMachine(2500, -1, new Vector3(21.185f, 0, -21.392f), 4.5f, true, 1f, 1f,
                new BaseModel(delegate { return modelList[10]["slab1"]; }, false, null, new Vector3(-7.882f, -12.821f, 9.935f))), false);
            //machs.Add(new TranslateMachine(2500, -1, new Vector3(21.067f, 0, 21.259f), 4.5f, true, 1f, 1f,
            //    new BaseModel(delegate { return modelList[10]["slab2"]; }, false, null, new Vector3(-18.055f, -25.843f, 13.924f) - new Vector3(21.067f, 0, 21.259f))), false);

            Material newFriction = new Material(0.5f, 0.5f, 0);
            machs.Add(new ClampedRotationMachine(4, 5, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-19.64830f, -25.884f, 16.036f),
                Vector3.UnitZ,
                new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-19.64830f, -25.884f, 16.036f))) { Friction = newFriction }, true);
            machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-17.35016f, -25.884f, 14.107f),
                Vector3.UnitZ,
                new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-17.35016f, -25.884f, 14.107f))) { Friction = newFriction }, false);
            machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-15.05203f, -25.884f, 12.179f),
                Vector3.UnitZ,
                new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-15.05203f, -25.884f, 12.179f))) { Friction = newFriction }, false);
            machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-12.75390f, -25.884f, 10.251f),
                Vector3.UnitZ,
                new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-12.75390f, -25.884f, 10.251f))) { Friction = newFriction }, false);
            machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-10.45576f, -25.884f, 8.332f),
                Vector3.UnitZ,
                new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-10.45576f, -25.884f, 8.332f))) { Friction = newFriction }, false);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 8));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 8));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[10]["machine4_auto"]; }, false, null, new Vector3(-5, -26, 0.87438f)) { LocalOffset = new Vector3(0, 0, -0.95271f) }), false);
            keyframeList.Clear();

            //machs.Add(new TruckMachine(5, 6, 6f, 0.75f, MathHelper.PiOver2, Vector3.UnitX * 22.5f, Vector3.UnitX, Vector3.UnitY,
            //    -Vector3.UnitZ, new Vector3(0.203f, -30.384f, 17.5f), new Vector3(0.203f, -21.384f, 17.5f), new Vector3(-3.773f, -25.884f, 17.524f),
            //    new Vector3(4.227f, -25.884f, 17.524f),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_1_wheels"]; }, false, null, new Vector3(-3.773f, -25.884f, 17.524f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_1_wheels"]; }, false, null, new Vector3(4.227f, -25.884f, 17.524f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_1_moving_glass"]; }, true, null, new Vector3(0.203f, -30.384f, 15.25f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_1_moving_glass"]; }, true, null, new Vector3(0.203f, -21.384f, 15.25f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_1_base"]; }, false, null, new Vector3(0.203f, -25.884f, 17)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_1_glass"]; }, true, null, new Vector3(0.203f, -25.884f, 19))), true);

            //machs.Add(new TruckMachine(5, 8, 6f, 0.75f, MathHelper.PiOver2, Vector3.UnitY * -20, -Vector3.UnitY, Vector3.UnitX,
            //    Vector3.UnitX, new Vector3(27.664f, -3.863f, 8.5f), new Vector3(18.664f, -3.863f, 8.5f), new Vector3(23.164f, -7.887f, 8.524f),
            //    new Vector3(23.164f, 0.113f, 8.524f),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_2_wheels"]; }, false, null, new Vector3(23.164f, -7.887f, 8.524f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_2_wheels"]; }, false, null, new Vector3(23.164f, 0.113f, 8.524f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_2_moving_glass"]; }, true, null, new Vector3(25.414f, -3.863f, 8.5f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_2_moving_glass"]; }, true, null, new Vector3(20.914f, -3.863f, 8.5f)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_2_base"]; }, false, null, new Vector3(23.164f, -4.376f, 10)),
            //    new BaseModel(delegate { return modelList[10]["machine5_auto_2_glass"]; }, true, null, new Vector3(23.164f, -4.363f, 10))), false);

            machs.Add(new TranslateMachine(5, 8, Vector3.UnitZ * 10, 1.25f, false,
                new BaseModel(delegate { return modelList[10]["machine5"]; }, false, null, new Vector3(24.45264f, -13.52587f, -2.20833f))), true);

            machs.Add(new TranslateMachine(6, 7, Vector3.UnitZ * -6.5f, 1.25f, false,
                new BaseModel(delegate { return modelList[10]["machine6_glass"]; }, true, null, new Vector3(208.453f, -4.184f, 3.887f))) , true);

            machs.Add(new TranslateMachine(2000, -1, new Vector3(-25.656f, 0, -14.978f), 5.5f, true, 1.5f, 1.5f,
                new BaseModel(delegate { return modelList[10]["slab3"]; }, false, null, new Vector3(222.444f, -4.187f, 5.017f))), false);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));

            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[10]["machine7_rotatepart"]; }, false, null, new Vector3(243.005f, -4.184f, 7.65f))) { Friction = new Material(1.2f, 1.2f, 0) }, false);
            keyframeList.Clear();

            machs.Add(new TranslateMachine(7, 10, Vector3.UnitX * 2.5f, 1f, false,
                new BaseModel(delegate { return modelList[10]["machine7_plusX"]; }, false, null, new Vector3(243.187f, -4.184f, 9.5f))), true);
            machs.Add(new TranslateMachine(7, -1, Vector3.UnitY * 2.5f, 1f, false,
                new BaseModel(delegate { return modelList[10]["machine7_plusY"]; }, false, null, new Vector3(243.003f, -4.124f, 9.5f))), false);
            machs.Add(new TranslateMachine(7, -1, Vector3.UnitY * -2.5f, 1f, false,
                new BaseModel(delegate { return modelList[10]["machine7_minusY"]; }, false, null, new Vector3(243.003f, -4.424f, 9.5f))), false);
            machs.Add(new TranslateMachine(7, -1, Vector3.UnitX * -2.5f, 1f, false,
                new BaseModel(delegate { return modelList[10]["machine7_minusX"]; }, false, null, new Vector3(242.867f, -4.184f, 9.5f))), false);

            machs.Add(new ClampedRotationMachine(8, 0, 1f, Vector3.UnitX, MathHelper.ToRadians(55), new Vector3(243.003f, -20.917f, 0.15f), Vector3.UnitZ,
                new BaseModel(delegate { return modelList[10]["machine8"]; }, false, null, new Vector3(243.003f, -20.917f, 0.15f))) { DampingMultiplier = 2 }, true); 

            keyframeList.Add(new Keyframe(new Vector3(-5, 0, 0), Quaternion.Identity, 1f));
            keyframeList.Add(new Keyframe(new Vector3(-0.000000001f, 0, 0), 1.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 5.25f));
            keyframeList.Add(new Keyframe(new Vector3(0.000000001f, 0, 0), 1.5f));
            keyframeList.Add(new Keyframe(new Vector3(5, 0, 0), Quaternion.Identity, 1f));
            k1 = new KeyframeMachine(9, 0, keyframeList, true,
                new BaseModel(delegate { return modelList[10]["machine9_pusher"]; }, false, null, new Vector3(249.408f, 21.116f, 5.2f)));
            k1.NoStop = true;

            CardinalSpline3D path2 = new CardinalSpline3D();
            path2.ControlPoints.Add(-1, new Vector3(226.356f - .942f, 21.096f, 0));
            path2.ControlPoints.Add(0, new Vector3(236.356f, 21.096f, 6.5f));
            path2.ControlPoints.Add(2, new Vector3(258.42f, 21.316f, 19.5f));
            path2.ControlPoints.Add(3, new Vector3(258.42f + 10.942f, 21.316f, 25));
            c = new Cannon(9, 8, new Vector3(0, 0, 6), Vector3.UnitY, new CannonPathFinder(path2, 22.064f, 2, true), new Vector3(236.356f, 21.096f, 4.5f), //140,
                new BaseModel(delegate { return modelList[10]["machine9_cannon"]; }, false, null, new Vector3(237.856f, 21.016f, 0.706f)));
            c.SetInputs(k1);
            c.SetActivationType(ActivationType.JustDeactivated);
            machs.Add(k1, true);
            machs.Add(c, false);
            keyframeList.Clear();

            machs.Add(new TranslateMachine(10, 10, Vector3.UnitZ * -6.5f, 1.25f, false,
                new BaseModel(delegate { return modelList[10]["machine10_glass"]; }, true, null, new Vector3(242.953f, 27.116f, 7.777f))), true);

            keyframeList.Add(new Keyframe(Vector3.UnitZ * -6, Quaternion.Identity, 0.5f));
            keyframeList.Add(new Keyframe(Vector3.UnitZ * 6, Quaternion.Identity, 6f));
            BaseModel pillar = new BaseModel(delegate { return modelList[10]["auto_pillar"]; }, false, null, new Vector3(242.953f, 31.116f, 17));
            BaseModel plane = new BaseModel(new BEPUphysics.Entities.Prefabs.Box(new Vector3(242.95779f, 31.13339f, 11.83277f), 3.934f, 3.934f, 0.01f), new Vector3(242.95779f, 31.13339f, 11.83277f));
            plane.Remover = true;
            plane.UsesLaserSound = false;
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true, pillar, plane) { NoStop = true }, false);

            keyframeList.Clear();
            Button[] buttons = new Button[5];
            buttons[0] = new Button(-Vector3.UnitY, new BaseModel(delegate { return modelList[10]["button1"]; }, false, null, new Vector3(242.803f, 39.4f, 8)));
            buttons[1] = new Button(-Vector3.UnitX, new BaseModel(delegate { return modelList[10]["button2"]; }, false, null, new Vector3(258.42f, 21.316f, 19.5f)));
            buttons[2] = new Button(Vector3.UnitY, new BaseModel(delegate { return modelList[10]["button4"]; }, false, null, new Vector3(243.053f, -19.635f, -4.5f)));
            buttons[3] = new Button(Vector3.UnitZ, new BaseModel(delegate { return modelList[10]["button3"]; }, false, null, new Vector3(242.999f, -48.68f, -2.052f)));
            buttons[4] = new Button(Vector3.UnitZ, new BaseModel(delegate { return modelList[10]["button3"]; }, false, null, new Vector3(235.803f, -4.184f, 4.697f)));
            keyframeList.Add(new Keyframe(new Vector3(0, 3.5f, 0), Quaternion.Identity, 1.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, float.PositiveInfinity));
            KeyframeMachine machine = new KeyframeMachine(0, -1, keyframeList, true, 
                new BaseModel(delegate { return modelList[10]["door2"]; }, false, null, new Vector3(309.403f, -2.17f, 4.35f))//,
                /*new BaseModel(delegate { return modelList[10]["door1_stripes"]; }, false, null, new Vector3(309.403f, -3.301f, 4.35f))*/);
            machine.SetInputs(buttons);
            machine.SetActivationType(ActivationType.IsActive);
            machs.Add(machine, false);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(new Vector3(0, -3.5f, 0), Quaternion.Identity, 1.5f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, float.PositiveInfinity));
            machine = new KeyframeMachine(0, 4, keyframeList, true, 
                new BaseModel(delegate { return modelList[10]["door1"]; }, false, null, new Vector3(309.403f, -5.269f, 4.35f))//,
                /* new BaseModel(delegate { return modelList[10]["door2_stripes"]; }, false, null, new Vector3(309.403f, -3.801f, 4.35f))*/);
            machine.SetInputs(buttons);
            machine.SetActivationType(ActivationType.IsActive);
            machs.Add(machine, false);

            tubeY = 310.703f;
            TranslateMachine machine2 = new TranslateMachine(0, -1, Vector3.UnitX * -62, 20, false,
                new BaseModel(delegate { return modelList[10]["long_belt"]; }, false, null, new Vector3(364.203f, -3.886f, 1.083f)),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
                new Tube(new Vector3(tubeY++, -3.886f, 1), true, false)) { Kinetic = true };
            machine2.SetInputs(machine);
            machine2.SetActivationType(ActivationType.JustDeactivated);
            machs.Add(machine2, false);

            level10 = new Level10(40, 20, new Vector3(-44.979f, 16.116f, 43f), billboardList, Theme.Sky, (BaseModel)delegate { return modelList[10]["base"]; }, (BaseModel)delegate { return modelList[10]["base_two"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[10]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[10]["glass2"]; }, true, false, Vector3.Zero),
                new BaseModel(delegate { return modelList[10]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[10]["glass4"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(42.953f, -4.134f, 5.5f), true), machs, tubeList,
                new LevelCompletionData(new TimeSpan(0, 15, 0), 7200, 14), "One Day, Three Cents,\n    and 9000 Miles Into\n    the Sky Later");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level 11
            Program.Cutter.WriteToLog(this, "Creating level 11");

            for(int i = 0; i < 17; i++)
            {
                tubeList.Add(new Tube(new Vector3(-47.21f + i, 24.649f, 12.2f), true, false)); // first set
                tubeList.Add(new Tube(new Vector3(27.79f, 1.149f - i, 0.2f), false, true)); // sixth set
            }
            for(int i = 0; i < 13; i++)
                tubeList.Add(new Tube(new Vector3(1.79f + i, 24.649f, 12.2f), true, false)); // second set
            for(int i = 0; i < 14; i++)
                tubeList.Add(new Tube(new Vector3(8.97f + i, 13.649f, 12.2f), true, false)); // third set
            for(int i = 0; i < 12; i++)
                tubeList.Add(new Tube(new Vector3(27.29f - i, 13.649f, -0.8f), true, true)); // fourth set
            for(int i = 0; i < 8; i++)
            {
                tubeList.Add(new Tube(new Vector3(13.79f, 15.149f - i, -0.8f), false, true)); // fifth set
                tubeList.Add(new Tube(new Vector3(-27.70972f, -29.35069f + i, -6.8f), false, false)); // thirteenth set
                tubeList.Add(new Tube(new Vector3(-27.70972f, -13.35069f + i, -1.8f), false, false)); // fourteenth set
                tubeList.Add(new Tube(new Vector3(-26.20972f - i, 8.14931f, 3.2f), true, true)); // sixteenth (last) set
            }
            for(int i = 0; i < 15; i++)
            {
                Tube t = new Tube(new Vector3(27.79f, -15.851f - i, -0.8f), false, true);
                t.BecomeKeybasedTube(6);
                tubeList.Add(t); // seventh set
            }
            for(int i = 0; i < 25; i++)
                tubeList.Add(new Tube(new Vector3(30.29f - i, -33.351f, -2.8f), true, true)); // eighth set
            for(int i = 0; i < 12; i++)
            {
                tubeList.Add(new Tube(new Vector3(-17.70972f, -1.85069f + i, 1.2f), false, false)); // ninth set
                tubeList.Add(new Tube(new Vector3(-19.20972f + i, 11.64931f, 1.2f), true, false)); // tenth set
                tubeList.Add(new Tube(new Vector3(-5.70972f, 13.14931f - i, 1.2f), false, true)); // eleventh set
            }
            for(int i = 0; i < 4; i++)
            {
                tubeList.Add(new Tube(new Vector3(-22.20972f - i, -27.85069f, -6.8f), true, true)); // twelvth set
                tubeList.Add(new Tube(new Vector3(-27.70972f, 2.64931f + i, 3.2f), false, false)); // fifteenth set
            }

            billboardList.Add(new Vector3(-14.7f, 24.649f, 15.5f));
            billboardList.Add(new Vector3(8.29f, 24.649f, 20f));
            billboardList.Add(new Vector3(32.09f, 17.149f, 12f));
            billboardList.Add(new Vector3(14.29f, 4.649f, 8.5f));
            billboardList.Add(new Vector3(27.79f, -7.531f, 5));
            billboardList.Add(new Vector3(27.79f, -16.851f, 3.5f));
            billboardList.Add(new Vector3(1.29f, -33.351f, 8f));
            billboardList.Add(new Vector3(-17.7f, -33.35f, 15));
            billboardList.Add(new Vector3(-5.71f, -2.85f, 15));
            billboardList.Add(new Vector3(-27.70972f, -9.85069f, 6));

            tubeY = -30.21f;
            TranslateMachine t1, t2;
            t1 = new TranslateMachine(1, 9, new Vector3(4, 0, 0), 1.75f, false,
                new BaseModel(delegate { return modelList[11]["machine1_part1"]; }, false, null, new Vector3(-20.71f, 24.649f, 12.287f)),
                new BaseModel(delegate { return modelList[11]["machine1_stripes"]; }, false, null, new Vector3(-20.7f, 24.65f, 12.4f)),
                new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
                new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
                new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false),
                new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false));
            tubeY = -18.21f;
            t2 = new TranslateMachine(1, 9, new Vector3(-4, 0, 0), 1.75f, false,
                new BaseModel(delegate { return modelList[11]["machine1_part2"]; }, false, null, new Vector3(-8.71f, 24.649f, 12.287f)),
                new BaseModel(delegate { return modelList[11]["machine1_stripes"]; }, false, null, new Vector3(-8.7f, 24.65f, 12.4f)),
                new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
                new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
                new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false),
                new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false));
            OperationalMachine.LinkMachines(t1, t2);

            machs.Add(t1, true);
            machs.Add(t2, false);

            machs.Add(new TranslateMachine(2, 5, new Vector3(0, 0, -6.5f), 1.5f, false,
                new BaseModel(delegate { return modelList[11]["machine2_glass"]; }, true, null, new Vector3(8.29f, 24.649f, 14.9f))), true);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.PiOver2), 1.5f));
            keyframeList.Add(new Keyframe(2.5f));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2), 1.5f));
            keyframeList.Add(new Keyframe(2.5f));
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true, 
                new BaseModel(delegate { return modelList[11]["auto_hammer"]; }, false, null, new Vector3(12.29f, 29.713f, 18.251f)) { LocalOffset = -new Vector3(0, 24.649f - 29.713f, 22.5f - 18.251f) }), false);
            keyframeList.Clear();

            machs.Add(new TranslateMachine(3, 8, new Vector3(0, 5, 0), 0.75f, false,
                new BaseModel(delegate { return modelList[11]["machine3_part1"]; }, false, null, new Vector3(32.09f, 11.149f, 8.9f))//,
                //new BaseModel(delegate { return modelList[11]["machine3_part1_stripes"]; }, false, null, new Vector3(32.26129f, 11.14931f, 9.37985f))
                ), true);
            machs.Add(new TranslateMachine(3, 8, new Vector3(0, -5, 0), 0.75f, false,
                new BaseModel(delegate { return modelList[11]["machine3_part2"]; }, false, null, new Vector3(26.09f, 22.149f, 2.6f))//,
                //new BaseModel(delegate { return modelList[11]["machine3_part2_stripes"]; }, false, null, new Vector3(25.91927f, 22.14931f, 3.07985f))
                ), false);

            tubeY = 11.29f;
            machs.Add(new ClampedRotationMachine(4, 6, 3.5f, Vector3.UnitY, MathHelper.PiOver2, new Vector3(27.79f, 3.649f, 1.284f), Vector3.UnitX,
                new BaseModel(delegate { return modelList[11]["machine4_base"]; }, false, null, new Vector3(22.679f, 3.649f, 1.284f)), 
                new BaseModel(delegate { return modelList[11]["machine4_glass"]; }, true, null, new Vector3(15.24f, 4.649f, 3.8f)),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
                new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(27.79f, 6.149f, 1.2f), false, true),
                new Tube(new Vector3(27.79f, 5.149f, 1.2f), false, true), new Tube(new Vector3(27.79f, 4.149f, 1.2f), false, true),
                new Tube(new Vector3(27.79f, 3.149f, 1.2f), false, true), new Tube(new Vector3(27.79f, 2.149f, 1.2f), false, true)) { DampingMultiplier = 40 }, true);

            CardinalSpline3D path = new CardinalSpline3D();
            path.ControlPoints.Add(-1, new Vector3(13.62f, 4.649f, -17.7f));
            path.ControlPoints.Add(0, new Vector3(13.62f, 4.649f, -9.7f));
            path.ControlPoints.Add(5, new Vector3(13.62f, 4.649f, 30.3f));
            path.ControlPoints.Add(10, new Vector3(13.62f, 4.649f, -9.7f));
            path.ControlPoints.Add(11, new Vector3(13.62f, 4.649f, -17.7f));
            machs.Add(new StationaryCollisionCannon(new CannonPathFinder(path, 0, 10, false), new Vector3(13.62f, 4.649f, -9.7f),
                new BaseModel(delegate { return modelList[11]["machine4_cannon"]; }, false, null, new Vector3(13.89f, 4.749f, -5.338f))), false);

            BaseModel laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -2.351f, 1.8f));
            laser.Remover = true;
            BaseModel laser2 = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -12.351f, 1.8f));
            laser2.Remover = true;
            machs.Add(new DisappearenceMachine(5, true, laser, laser2), true);
            laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -7.351f, 1.8f));

            laser.Remover = true;
            machs.Add(new DisappearenceMachine(5, false, laser), false);

            laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -19.351f, 0.8f));
            laser.Remover = true;
            laser2 = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -29.351f, 0.8f));
            laser2.Remover = true;
            machs.Add(new DisappearenceMachine(5000, true, laser, laser2), false);

            laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -24.351f, 0.8f));
            laser.Remover = true;
            machs.Add(new DisappearenceMachine(5000, false, laser), false);

            machs.Add(new HoldRotationMachine(6, 2, Vector3.Zero), true);              

            machs.Add(new TranslateMachine(7, 8, Vector3.UnitZ * 8, 1, false,
                new BaseModel(delegate { return modelList[11]["machine7"]; }, false, null, new Vector3(1.29f, -33.351f, -4.633f)),
                new BaseModel(delegate { return modelList[11]["machine7_stripes"]; }, false, null, new Vector3(1.3f, -33.35f, -4.6f))), true);

            BaseModel b = new BaseModel(delegate { return modelList[11]["machine8_part1"]; }, false, null, new Vector3(-17.7f, -33.35f, 1));
            BaseModel b2 = new BaseModel(delegate { return modelList[11]["machine8_glass"]; }, true, null, new Vector3(-7.05f, -33.15013f, 2.14f));
            b.Ent.CollisionInformation.LocalPosition = new Vector3(-2.89114f, -0.005958138f, -0.1427545f);
            b2.Ent.CollisionInformation.LocalPosition = new Vector3(10.57148f - 0.7564434f, 0f, 1.03349f);

            keyframeList.Add(new Keyframe(Vector3.UnitZ * 5, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2), 3f));
            keyframeList.Add(new Keyframe(5f));
            k1 = new KeyframeMachine(8, 7, keyframeList, false,
                b, b2);
            keyframeList.Clear();

            b = new BaseModel(delegate { return modelList[11]["machine8_part2"]; }, false, null, new Vector3(-17.7f, -9.85f, 5.5f));
            b.Ent.CollisionInformation.LocalPosition = new Vector3(0.000284043f, 0.5598117f, 0.4155622f);

            keyframeList.Add(new Keyframe(3f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.PiOver4), 1.25f));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 3.75f));
            k2 = new KeyframeMachine(8, 7, keyframeList, false,
                b);
            keyframeList.Clear();

            machs.Add(k1, true);
            machs.Add(k2, false);
            //machs.Add(k3, false);

            machs.Add(new TranslateMachine(9, 9, Vector3.UnitZ * 7, 1f, false,
                new BaseModel(delegate { return modelList[11]["machine9"]; }, false, null, new Vector3(-5.71f, -3.05f, -0.134f)),
                new BaseModel(delegate { return modelList[11]["machine9_stripes"]; }, false, null, new Vector3(-5.7f, -3.3f, -0.6f))), true);

            tubeY = -21.26369f;

            machs.Add(new TranslateMachine(10, 7, Vector3.UnitZ * 5, 1f, false,
                new BaseModel(delegate { return modelList[11]["machine10"]; }, false, null, new Vector3(-27.70972f, -9.85069f, -4.21666f)),
                new BaseModel(delegate { return modelList[11]["machine10_stripes"]; }, false, null, new Vector3(-27.7f, -9.85f, -4.1f)),
                new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
                new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
                new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
                new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
                new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false),
                new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false),
                new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false),
                new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false)), true);

            level11 = new Level(11, 1, 1, new Vector3(-44.21f, 24.649f, 18.5f), billboardList, Theme.Space, (BaseModel)delegate { return modelList[11]["base"]; },
                new BaseModel[] { new BaseModel(delegate { return modelList[11]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[11]["glass2"]; }, true, false, Vector3.Zero),
                new BaseModel(delegate { return modelList[11]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[11]["glass4"]; }, true, false, Vector3.Zero),
                new BaseModel(delegate { return modelList[11]["glass6"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[11]["glass7"]; }, true, false, Vector3.Zero) },
                new Goal(new Vector3(-37.13897f, 8.33103f, -3.90719f - 1.5f)), null, 
                machs, tubeList, new LevelCompletionData(), "");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();  
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level D1
            Program.Cutter.WriteToLog(this, "Creating level D1");

            for(int i = 0; i < 19; i++)
                tubeList.Add(new Tube(new Vector3(-35.033f + i, 0.395f, 5), true, false));
            for(int i = 0; i < 33; i++)
                tubeList.Add(new Tube(new Vector3(-4.533f + i, 0.595f, -1), true, false));

            TranslateMachine t3, t4;
            t1 = new TranslateMachine(1, 10, Vector3.UnitX * 3f, 1.15f, false,
                new BaseModel(delegate { return modelList[12]["machine1_plus_x"]; }, false, null, new Vector3(-10.353f, 0.345f, 3)));
            t2 = new TranslateMachine(1, 10, Vector3.UnitX * -3f, 1.15f, false,
                 new BaseModel(delegate { return modelList[12]["machine1_minus_x"]; }, false, null, new Vector3(-10.662f, 0.345f, 3)));
            t3 = new TranslateMachine(1, 10, Vector3.UnitY * 3f, 1.15f, false,
                new BaseModel(delegate { return modelList[12]["machine1_plus_y"]; }, false, null, new Vector3(-10.483f, 0.505f, 3)));
            t4 = new TranslateMachine(1, 10, Vector3.UnitY * -3f, 1.15f, false,
                new BaseModel(delegate { return modelList[12]["machine1_minus_y"]; }, false, null, new Vector3(-10.483f, 0.215f, 3)));

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));
            keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));

            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[12]["machine1_rotatebase"]; }, false, null, new Vector3(-10.483f, 0.345f, 1.15f))) { Friction = new Material(1.2f, 1.2f, 0) }, false);
            keyframeList.Clear();

            OperationalMachine.LinkMachines(t1, t2, t3, t4);
            machs.Add(t1, true);
            machs.Add(t2, false);
            machs.Add(t3, false);
            machs.Add(t4, false);

            machs.Add(new TranslateMachine(2, 7, Vector3.UnitZ * -6.5f, 1.5f, false,
                new BaseModel(delegate { return modelList[12]["machine2_glass"]; }, true, null, new Vector3(14f, 0.595f, 1.25f))) , true);

            keyframeList.Clear();
            keyframeList.Add(new Keyframe(Vector3.UnitY * -2.7f, 1.5f));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            keyframeList.Add(new Keyframe(Vector3.UnitY * 2.7f, 1.5f));
            keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            //machs.Add(new BeltMachine(0, new Vector3(16.967f, 2.145f - 3, 0.967f - 1.033f), new Vector3(16.967f, 2.145f, 0.967f - 1.033f), Vector3.UnitZ * 1.033f, 2.7f, Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[12]["machine2_auto"]; }, false, null, new Vector3(16.967f, 2.145f, 0.967f + 1.033f))), false);
            machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
                new BaseModel(delegate { return modelList[12]["machine2_auto"]; }, false, null, new Vector3(16.967f, 2.145f, 0.967f + 1.1f)) { LocalOffset = new Vector3(0, 0, -1.1f) }), false);
            keyframeList.Clear();

            billboardList.Add(new Vector3(-10.483f, 0.215f, 8.5f));
            billboardList.Add(new Vector3(13.697f, 0.595f, 8.5f));

            levelD1 = new Level(12, 20, 5, new Vector3(-33.033f, 0.395f, 15), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[12]["base"]; },
                new[] { new BaseModel(delegate { return modelList[12]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[12]["glass2"]; }, true, false, Vector3.Zero) }, 
                new Goal(new Vector3(31.717f, 0.645f, -5.344f)), null, machs, tubeList, 
                new LevelCompletionData(new TimeSpan(0, 1, 50), 2200, 1), "Pilot");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level D2
            Program.Cutter.WriteToLog(this, "Creating level D2");

            for(int i = 0; i < 20; i++)
            {
                tubeList.Add(new Tube(new Vector3(-31.608f + i, -32.067f, 21), true, false));
                tubeList.Add(new Tube(new Vector3(-25.608f + i, 47.693f, 3), true, false));
            }
            for(int i = 0; i < 24; i++)
                tubeList.Add(new Tube(new Vector3(-17.608f + i, 4.963f, 3), true, false));
            for(int i = 0; i < 7; i++)
            {
                tubeList.Add(new Tube(new Vector3(6.392f + i, 6.767f, 3), true, false));
                tubeList.Add(new Tube(new Vector3(6.392f + i, 2.756f, 3), true, false));
            }

            tubeList.Add(new Tube(new Vector3(13.392f, 7.091f, 3), true, false));
            tubeList.Add(new Tube(new Vector3(14.392f, 7.237f, 3), true, false));
            tubeList.Add(new Tube(new Vector3(15.392f, 7.518f, 3), true, false));
            tubeList.Add(new Tube(new Vector3(16.392f, 7.747f, 3), true, false));
            tubeList.Add(new Tube(new Vector3(13.392f, 2.756f, 3), true, false));
            tubeList.Add(new Tube(new Vector3(14.392f, 2.583f, 3), true, false));
            tubeList.Add(new Tube(new Vector3(15.392f, 2.416f, 3), true, false));
            tubeList.Add(new Tube(new Vector3(16.392f, 2.148f, 3), true, false));

            for(int i = 0; i < 6; i++)
            {
                tubeList.Add(new Tube(new Vector3(17.392f + i, 1.963f, 3), true, false));
                tubeList.Add(new Tube(new Vector3(17.392f + i, 7.963f, 3), true, false));
            }
            for(int i = 0; i < 13; i++)
                tubeList.Add(new Tube(new Vector3(24.982f, 6.463f + i, 3), false, false));
            for(int i = 0; i < 15; i++)
                tubeList.Add(new Tube(new Vector3(25.292f, 3.463f - i, 3), false, true));
            for(int i = 0; i < 14; i++)
                tubeList.Add(new Tube(new Vector3(-24.108f, -13.537f + i, 3), false, false));
            for(int i = 0; i < 27; i++)
                tubeList.Add(new Tube(new Vector3(26.392f - i, 20.963f, 3), true, true));
            for(int i = 0; i < 12; i++)
            {
                tubeList.Add(new Tube(new Vector3(-2.108f, 19.463f + i, 3), false, false));
                tubeList.Add(new Tube(new Vector3(-24.108f, 12.463f + i, 3), false, false));
            }
            for(int i = 0; i < 49; i++)
                tubeList.Add(new Tube(new Vector3(26.392f - i, -12.437f, 3), true, true));
            for(int i = 0; i < 10; i++)
                tubeList.Add(new Tube(new Vector3(-24.108f, 36.463f + i, 3), false, false));

            billboardList.Add(new Vector3(-12.108f, -32.037f, 31.818f));
            billboardList.Add(new Vector3(-7.608f, -54.524f + 60.5f, 19.5f));
            billboardList.Add(new Vector3(-2.108f, 35.734f, 15));
            billboardList.Add(new Vector3(-24.108f, 5.963f, 9f));

            machs.Add(new TranslateMachine(1, 9, Vector3.UnitZ * -6.5f, 1.5f, false,
                new BaseModel(delegate { return modelList[13]["machine1_glass"]; }, true, null, new Vector3(-12.108f, -32.037f, 23.818f))) , true);

            machs.Add(new TruckMachine(2, 5, 13.5f, 1.25f, MathHelper.PiOver2, Vector3.UnitY * 55.5f, -Vector3.UnitY, -Vector3.UnitX, Vector3.UnitX,
                new Vector3(-3.108f, -50.037f, 11.5f), new Vector3(-12.108f, -50.037f, 11.5f), new Vector3(-7.608f, -46.013f, 11.524f), new Vector3(-7.608f, -54.013f, 11.524f),
                new BaseModel(delegate { return modelList[13]["machine2_wheels"]; }, false, null, new Vector3(-7.608f, -46.013f, 11.524f)),
                new BaseModel(delegate { return modelList[13]["machine2_wheels"]; }, false, null, new Vector3(-7.608f, -54.013f, 11.524f)),
                new BaseModel(delegate { return modelList[13]["machine2_door"]; }, true, null, new Vector3(-5.358f, -50.037f, 11.5f)),
                new BaseModel(delegate { return modelList[13]["machine2_door"]; }, true, null, new Vector3(-9.858f, -50.037f, 11.5f)),
                new BaseModel(delegate { return modelList[13]["machine2_base"]; }, false, null, new Vector3(-7.608f, -49.524f, 13)),
                new BaseModel(delegate { return modelList[13]["machine2_glass"]; }, true, null, new Vector3(-7.608f, -49.537f, 13))), true);

            machs.Add(new ClampedRotationMachine(0, -1, 1.5f, Vector3.UnitZ, MathHelper.PiOver4, new Vector3(13.392f, 4.963f, 3.5f), Vector3.UnitY,
                new BaseModel(delegate { return modelList[13]["auto_switcher"]; }, false, null, new Vector3(12.252f, 5.476f, 3.9f))), false);

            machs.Add(new ClampedRotationMachine(3, 1, 2.5f, -Vector3.UnitX, MathHelper.ToRadians(179), new Vector3(-2.108f, 40.363f, 0.589f), -Vector3.UnitY,
                new BaseModel(delegate { return modelList[13]["machine3_bucket"]; }, false, null, new Vector3(-2.108f, 35.734f, -1.787f)),
                new BaseModel(delegate { return modelList[13]["machine3_glass"]; }, true, null, new Vector3(-2.782f, 34.717f, -0.11f))), true);

            keyframeList.Add(new Keyframe(Vector3.UnitZ * -2.5f, Quaternion.Identity, 0.75f));
            keyframeList.Add(new Keyframe(Vector3.UnitY * 24f, Quaternion.Identity, 2f));
            keyframeList.Add(new Keyframe(Vector3.UnitZ * 2.5f, Quaternion.Identity, 0.75f));

            tubeY = 0.463f;
            machs.Add(new KeyframeMachine(4, 8, keyframeList, false,
                new BaseModel(delegate { return modelList[13]["machine4_base"]; }, false, null, new Vector3(-24.108f, 5.963f, 3.084f)),
                new BaseModel(delegate { return modelList[13]["machine4_glass"]; }, true, null, new Vector3(-24.108f, 5.963f, 5.963f)),
                new BaseModel(delegate { return modelList[13]["machine4_stripes"]; }, false, null, new Vector3(-24.092f, 5.963f, 3.15f)),
                new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
                new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
                new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
                new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
                new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
                new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false)), true);

            levelD2 = new Level(13, 30, 5, new Vector3(-28.608f, -32.037f, 31.5f), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[13]["base"]; },
                new[] { new BaseModel(delegate { return modelList[13]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[13]["glass2"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[13]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[13]["glass4"]; }, true, false, Vector3.Zero), 
                    new BaseModel(delegate { return modelList[13]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[13]["glass6"]; }, true, false, Vector3.Zero) }, new Goal(new Vector3(-2.108f, 47.963f, -5.066f)),
                    null, machs, tubeList, new LevelCompletionData(new TimeSpan(0, 2, 45), 2100, 2), "Accomplishment");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            #region Level D3
            Program.Cutter.WriteToLog(this, "Creating level D3");

            for(int i = 0; i < 14; i++)
                tubeList.Add(new Tube(new Vector3(-52.886f + i, -14.187f, -0.5f), true, false));
            for(int i = 0; i < 19; i++)
                tubeList.Add(new Tube(new Vector3(-37.366f, -15.687f + i, -0.5f), false, false));
            for(int i = 0; i < 3; i++)
                tubeList.Add(new Tube(new Vector3(-33.886f + i, -3.187f, -0.5f), true, false));
            for(int i = 0; i < 7; i++)
                tubeList.Add(new Tube(new Vector3(-30.466f + i, 14.813f, 2.5f), true, false));
            tubeList.Add(new Tube(new Vector3(-3.466f, 14.813f, 2.5f), true, false));
            for(int i = 0; i < 14; i++)
                tubeList.Add(new Tube(new Vector3(-0.466f - i, 4.813f, 0.4f), true, true));
            for(int i = 0; i < 11; i++)
                tubeList.Add(new Tube(new Vector3(-15.966f, 6.313f - i, 0.4f), false, true));
            for(int i = 0; i < 5; i++)
                tubeList.Add(new Tube(new Vector3(-15.966f, -8.687f - i, 0.4f), false, true));
            for(int i = 0; i < 23; i++)
                tubeList.Add(new Tube(new Vector3(-17.466f + i, -15.187f, 0.4f), true, false));
            for(int i = 0; i < 17; i++)
                tubeList.Add(new Tube(new Vector3(7.034f, -16.687f + i, 0.4f), false, false));
            for(int i = 0; i < 12; i++)
                tubeList.Add(new Tube(new Vector3(10.534f + i, -7.187f, -1), true, false));
            for(int i = 0; i < 22; i++)
                tubeList.Add(new Tube(new Vector3(24.034f, -8.867f + i, -1), false, false));

            machs.Add(new TranslateMachine(1, 9, Vector3.UnitX * 5, 1.25f, false,
                new BaseModel(delegate { return modelList[14]["machine1"]; }, false, null, new Vector3(-41.79f, -3.187f, -.12f)),
                new BaseModel(delegate { return modelList[14]["machine1_box"]; }, false, null, new Vector3(-43.355f, -3.215f, 0)) { IsInvisible = true }), true);

            machs.Add(new TranslateMachine(2, 7, new Vector3(0, 20.732f, 12.28f), 5.75f, false,
                new BaseModel(delegate { return modelList[14]["machine2"]; }, false, null, new Vector3(-28.866f, -9.904f, -6.351f))), true);

            tubeY = -23.466f;
            t1 = new TranslateMachine(3, 5, Vector3.UnitX * 5, 1, false,
                new BaseModel(delegate { return modelList[14]["machine3"]; }, false, null, new Vector3(-21.466f, 14.813f, 2.584f)),
                new BaseModel(delegate { return modelList[14]["machine3_glass1"]; }, true, null, new Vector3(-21.45f, 14.813f, 5.7f)),
                new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), 
                new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), 
                new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false));

            tubeY = -8.466f;
            t2 = new TranslateMachine(3, 5, Vector3.UnitX * -5, 1, false,
                new BaseModel(delegate { return modelList[14]["machine3"]; }, false, null, new Vector3(-6.466f, 14.813f, 2.584f)),
                new BaseModel(delegate { return modelList[14]["machine3_glass2"]; }, true, null, new Vector3(-6.45f, 14.813f, 5.7f)),
                new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false),
                new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false),
                new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false));

            OperationalMachine.LinkMachines(t1, t2);
            machs.Add(t1, true);
            machs.Add(t2, false);

            machs.Add(new TranslateMachine(4, 0, Vector3.UnitY * -9, 2.75f, false,
                new BaseModel(delegate { return modelList[14]["machine4"]; }, false, null, new Vector3(-1.5f, 17.313f, 3))), true);

            machs.Add(new TranslateMachine(5, 1, Vector3.UnitZ * 2.75f, 0.75f, false,
                new BaseModel(delegate { return modelList[14]["machine5"]; }, false, null, new Vector3(-16, -6.187f, -0.125f)),
                new BaseModel(delegate { return modelList[14]["machine5_glass"]; }, true, null, new Vector3(-16, -6.187f, 3.6f))), true);

            machs.Add(new ClampedRotationMachine(6, 4, 1.25f, -Vector3.UnitY, MathHelper.PiOver2, new Vector3(7, -7.187f, 10.5f), Vector3.UnitX,
                new BaseModel(delegate { return modelList[14]["machine6"]; }, false, null, new Vector3(2.324f, -7.187f, 5.824f))), true);

            billboardList.Add(new Vector3(-41.79f, -3.187f, 4.5f));
            billboardList.Add(new Vector3(-28.866f, -9.904f, 0));
            billboardList.Add(new Vector3(-15.466f, 14.813f, 7));
            billboardList.Add(new Vector3(-1.5f, 17.313f, 7.5f));
            billboardList.Add(new Vector3(-16, -6.187f, 7f));
            billboardList.Add(new Vector3(7, -7.187f, 13.5f));

            levelD3 = new Level(14, 50, 5, new Vector3(-50.866f, -14.187f, 7.5f), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[14]["base"]; },
                new[] { new BaseModel(delegate { return modelList[14]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[14]["glass2"]; }, true, false, Vector3.Zero),
                    new BaseModel(delegate { return modelList[14]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[14]["glass4"]; }, true, false, Vector3.Zero),
                    new BaseModel(delegate { return modelList[14]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[14]["glass6"]; }, true, false, Vector3.Zero),
                    new BaseModel(delegate { return modelList[14]["glass7"]; }, true, false, Vector3.Zero) },
                    new Goal(new Vector3(24.395f, 15.153f, -5.75f)), null, machs, tubeList, 
                    new LevelCompletionData(new TimeSpan(0, 3, 15), 2000, 3), "Legend");

            tubeList.Clear();
            machs.Clear();
            billboardList.Clear();
            keyframeList.Clear();
            yield return progress();
            #endregion

            levelArray = new Level[15];
            levelArray[0] = level00;
            levelArray[1] = level01;
            levelArray[2] = level02;
            levelArray[3] = level03;
            levelArray[4] = level04;
            levelArray[5] = level05;
            levelArray[6] = level06;
            levelArray[7] = level07;
            levelArray[8] = level08;
            levelArray[9] = level09;
            levelArray[10] = level10;
            levelArray[11] = level11;
            levelArray[12] = levelD1;
            levelArray[13] = levelD2;
            levelArray[14] = levelD3;
            yield return progress();

            Program.Cutter.WriteToLog(this, "Last section took " + sectionTime + "ms.");
            sectionTime = 0;
            #endregion

            string loadedCheckMessage = String.Format("Loaded {0} items. Expected {1} items.", loadedItems, totalItems);
            Program.Cutter.WriteToLog(this, loadedCheckMessage);
            Program.Cutter.WriteToLog(this, "Total loading time was " + totalTime + "ms.");
            Debug.Assert(loadedItems == totalItems, "totalItems needs adjusting.", loadedCheckMessage);
            yield return 1;
        }

        float progress()
        {
            ++loadedItems;
            Program.Cutter.WriteToLog(this, "Loading Time for last loaded object was: " + savedTime.ElapsedGameTime.Milliseconds.ToString() + "ms");
            return (float)loadedItems / totalItems;
        }

        string[] loadModelNames(int levelNo)
        {
            string[] modelStrings;
            switch(levelNo)
            {
                case 0: modelStrings = new string[] { "base", "glass1", "glass2", "machine1", "machine1_glass" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level0/" + modelStrings[i];
                    return modelStrings;
                case 1: modelStrings = new string[] { "base", "glass1", "glass2", "machine1_glass", "machine1_auto" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level1/" + modelStrings[i];
                    return modelStrings;
                case 2: modelStrings = new string[] { "base", "glass1", "glass2", "machine1", "machine2", "machine1_stripes", 
                    "machine2_stripes" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level2/" + modelStrings[i];
                    return modelStrings;
                case 3: modelStrings = new string[] { "base", "glass1", "machine1_part1", "machine1_part2", "machine1_part3", 
                    "machine2", "machine2_stripes", "machine3", "flags" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level3/" + modelStrings[i];
                    return modelStrings;
                case 4: modelStrings = new string[] { "base", "glass1", "glass2", "glass3", "glass4", "extras",
                    "glass5", "machine1", "machine2", "machine3_part1", "machine3_part2", "machine4_part1", "machine4_part2" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level4/" + modelStrings[i];
                    return modelStrings;
                case 5: modelStrings = new string[] { 
                    "base", "glass1", "glass2", "glass3", "glass4", "glass5", "glass6", "glass7", "machine1",
                    "machine2_rotatepart", "machine2_rotatepart_glass", "machine2_slidepart", "machine3", "machine4_base",
                    "machine4_glass", "machine4_rotors", "machine5_glass", "machine5_part2", "machine5_part2_glass", "machine3_stripes",
                    "machine2_ice", "effects", "machine5_part2_glass2" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level5/" + modelStrings[i];
                    return modelStrings;
                case 6: modelStrings = new string[] { "base", "glass1", "glass2", "machine1_glass", "extras",
                    "machine2_base", "machine2_glass", "machine2_stripes", "machine3", "machine4", "machine5_part1_base",
                    "machine5_part1_glass", "machine5_part1_stripes", "machine5_part2_base", "machine5_part2_glass", "machine5_part2_stripes",
                    "waterwheel", "machine3_stripes", "machine6_glass" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level6/" + modelStrings[i];
                    return modelStrings;
                case 7: modelStrings = new string[] { 
                        "base", "machine1_glass", "machine2_glass", "machine3", "machine4", "machine4_part2", "machine5", "machine6", 
                        "machine7_part1_glass", "machine7_part2", "glass1", "glass2", "glass3", "glass4", "glass5", "glass6",
                        "glass7", "glass8", "glass9" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level7/" + modelStrings[i];
                    return modelStrings;
                case 8: modelStrings = new string[] { 
                    "base", "fan", "glass1", "glass2", "glass3", "glass4", "glass5", "glass6", "glass7", "glass8", "glass9", "glass10", "glass11", "machine1_base",
                    "machine1_glass", "machine2", "machine3_base", "machine3_glass", "machine4_dome", "machine4_slide", "machine5_base",
                    "machine5_glass", "machine6", "machine7_glass", "machine7_rim", "machine8", "spiked_roller" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level8/" + modelStrings[i];
                    return modelStrings;
                case 9: modelStrings = new string[] { "auto_belt", "auto_switcher", "base", "glass1", "glass2", "machine1_glass",
                    "machine2", "machine2_stripes", "machine2_glass", "machine3", "machine3_cannon", "machine4", "machine5", "machine6_flat",
                    "machine6_slant", "machine7", "machine7_stripes", "machine8", "machine9", "machine9_stripes", "flags" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level9/" + modelStrings[i];
                    return modelStrings;
                case 10: modelStrings = new string[] { "auto_pillar", "base", "button1", "button2", "button3", "button4", "glass1", "glass2", 
                    "glass3", "glass4",
                    "machine1", "machine10_glass", "machine2_base", "machine2_bucket", "machine2_glass",
                    "machine3_glass", "machine3_base", "machine3_bucket", "machine4", "machine4_auto", "machine5", //"machine5_auto_1_base", 
                    //"machine5_auto_1_glass", "machine5_auto_1_moving_glass", "machine5_auto_1_wheels",
                    //"machine5_auto_2_base", "machine5_auto_2_glass", "machine5_auto_2_wheels", "machine5_auto_2_moving_glass",
                    "machine6_glass", "machine7_rotatepart", "machine8", "machine9_cannon", "machine9_pusher",
                    "slab1", /*"slab2",*/ "slab3", "long_belt", "door1", /*"door1_stripes",*/ "door2", //"door2_stripes",
                    "machine7_plusY", "machine7_plusX", "machine7_minusX", "machine7_minusY", "base_two" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level10/" + modelStrings[i];
                    return modelStrings;
                case 11: modelStrings = new string[] { "auto_hammer", "base", "glass1", "glass2", "glass3",
                    "glass4", "glass6", "glass7", "laser", "machine1_part1", "machine1_part2", "machine8_glass",
                    "machine2_glass", "machine3_part1", "machine3_part2", "machine4_base", "machine4_glass",  "machine4_cannon",
                    "machine7", "machine8_part1", "machine8_part2", "machine9", "machine10", "machine10_stripes", "straight_tube",
                    "machine1_stripes", "machine7_stripes", "machine9_stripes", "machine3_part1_stripes", "machine3_part2_stripes" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "Level11/" + modelStrings[i];
                    return modelStrings;
                case 12: modelStrings = new string[] { "base", "glass1", "glass2", "machine1_minus_y", "machine1_minus_x",
                    "machine1_plus_y", "machine1_plus_x", "machine1_rotatebase", "machine2_auto", "machine2_glass" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "LevelD1/" + modelStrings[i];
                    return modelStrings;
                case 13: modelStrings = new string[] { "base", "glass1", "glass2", "glass3", "glass4", "glass5", 
                    "glass6", "auto_switcher", "machine1_glass", "machine2_base", "machine2_door", "machine2_wheels", "machine2_glass",
                    "machine3_bucket", "machine3_glass", "machine4_glass", "machine4_base", "machine4_stripes" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "LevelD2/" + modelStrings[i];
                    return modelStrings;
                case 14: modelStrings = new string[] { "base", "glass1", "glass2", "glass3", "glass4", "glass5", 
                    "glass6", "glass7", "machine1", "machine2", "machine3", "machine3_glass1", "machine3_glass2", "machine4", "machine5",
                    "machine5_glass", "machine6", "machine1_box" };
                    for(int i = 0; i < modelStrings.Length; i++)
                        modelStrings[i] = "LevelD3/" + modelStrings[i];
                    return modelStrings;
                default: throw new ArgumentException("An attempt was made to load a level that doesn't exist.");
            }
        }

        public void Update(GameTime gameTime)
        {
            savedTime = gameTime;
            sectionTime += savedTime.ElapsedGameTime.Milliseconds;
            totalTime += savedTime.ElapsedGameTime.Milliseconds;
        }

        /// <summary>
        /// Be careful when you call this.
        /// </summary>
        public void ReloadALLtheThings()
        {
            if(Program.Game.Loading)
            {
                Program.Game.RestartLoad();
                return;
            }

            #region Init
            // we have to load any lists of items first, so we know how many steps will be in the loading bar

            string[][] levelList = { loadModelNames(0), loadModelNames(1), loadModelNames(2), 
                                  loadModelNames(3), loadModelNames(4), loadModelNames(5), 
                                  loadModelNames(6), loadModelNames(7), loadModelNames(8), 
                                  loadModelNames(9), loadModelNames(10), loadModelNames(11),
                                  loadModelNames(12), loadModelNames(13), loadModelNames(14) };

            modelList = new List<Dictionary<string, Model>>();
            for(int i = 0; i < 15; i++)
                modelList.Add(new Dictionary<string, Model>());

             // first progress bar update.
            #endregion

            Program.Cutter.WriteToLog(this, "Loading video");
            LevelSelectVideo = content.Load<Video>("Video/level select");
            MenuHandler.SetVideo(LevelSelectVideo);

            #region Models
            // Where possible, load the big things first -- an accelerating progress bar feels quicker.

            Program.Cutter.WriteToLog(this, "Loading models");
            for(int j = 0; j < levelList.Length; j++)
            {
                string[] strings = levelList[j];
                for(int i = 0; i < strings.Length; i++)
                {
                    modelList[j].Add(strings[i].Replace("Level" + (j < 12 ? j.ToString() : "D" + (j - 11)) + "/", ""), content.Load<Model>(strings[i]));
                }
            }
            Resources.boxModel = boxModel = content.Load<Model>("All Levels/box");
            
            Resources.tubeModel = tubeModelX = content.Load<Model>("All Levels/tube_x");
            
            Resources.MetalTex = MetalTex = content.Load<Texture2D>("textures/metal");
            
            Resources.WaterBumpMap = WaterBumpMap = content.Load<Texture2D>("textures/waterbump");
            
            //IceBumpMap = content.Load<Texture2D>("textures/icenor_01");
            //
            Resources.IceTexture = IceTexture = content.Load<Texture2D>("textures/icetex_01");
            
            Resources.BeachTexture = BeachTexture = content.Load<Texture2D>("textures/beach");

            //Resources.lavaSkyboxModel = lavaSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_lava"); }, false, false, Vector3.Zero);
            
            //Resources.lavaOuterModel = lavaOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_lava"); }, false, false, Vector3.Zero);
            
            //Resources.iceSkyboxModel = iceSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_ice"); }, false, false, Vector3.Zero);
            
            //Resources.iceOuterModel = iceOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_ice"); }, false, false, Vector3.Zero);
            
            //Resources.beachSkyboxModel = beachSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_beach"); }, false, false, Vector3.Zero);
            
            //Resources.beachOuterModel = beachOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_beach"); }, false, false, Vector3.Zero);
            
            //Resources.skySkyboxModel = skySkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_sky"); }, false, false, Vector3.Zero);
            
            //Resources.skyOuterModel = skyOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_sky"); }, false, false, Vector3.Zero);
            
            ////Resources.genericSkyboxModel = genericSkyboxModel = new BaseModel(content.Load<Model>("All Levels/skybox_generic"), false, false, Vector3.Zero);
            ////
            //Resources.genericOuterModel = genericOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/generic_outer"); }, false, false, Vector3.Zero);
            
            //Resources.spaceSkyboxModel = spaceSkyboxModel = new BaseModel(delegate { return content.Load<Model>("All Levels/skybox_space"); }, false, false, Vector3.Zero);
            
            //Resources.spaceOuterModel = spaceOuterModel = new BaseModel(delegate { return content.Load<Model>("All Levels/outer_space"); }, false, false, Vector3.Zero);
            
            Dispenser = content.Load<Model>("All Levels/dispenser");
            
            Resources.blueBoxModel = BlueBoxModel = content.Load<Model>("All Levels/blue_box");
            
            Resources.blackBoxModel = BlackBoxModel = content.Load<Model>("All Levels/black_box");
            

            Theme.Initialize(content);

            #endregion

            Program.Cutter.WriteToLog(this, "Loading textures");

            halfBlack = new Texture2D(RenderingDevice.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
            color[0] = new Color(0, 0, 0, 178);
            halfBlack.SetData(color); //set the color data on the texture
            Resources.halfBlack = halfBlack;

            tbcSplash = content.Load<Texture2D>("2D/Splashes and Overlays/Logo");
            
            pressStart = content.Load<Texture2D>("Font/press start");
            
            #region credits
            Texture2D tex = content.Load<Texture2D>("2D/Credits/credits_01");
            //Credits[0] = new Sprite(delegate { return tex; }, new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height + 30) + new Vector2(0, (2048 * 0.5f) * RenderingDevice.TextureScaleFactor.Y), null, Sprite.RenderPoint.Center);
            
            Texture2D tex2 = content.Load<Texture2D>("2D/Credits/credits_02");
            //Credits[1] = new Sprite(delegate { return tex2; }, new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height + 30) + new Vector2(0, (2048 + 936 * 0.5f - 7) * RenderingDevice.TextureScaleFactor.Y), null, Sprite.RenderPoint.Center);
            
            Texture2D tex3 = content.Load<Texture2D>("2D/Credits/credits_03");
            //Credits[Credits.Length - 1] = new Sprite(delegate { return tex3; }, new Vector2(RenderingDevice.Width, RenderingDevice.Height) * 0.5f, null, Sprite.RenderPoint.Center);
            //Credits[0].DrawUnscaled = Credits[1].DrawUnscaled = Credits[2].DrawUnscaled = true;
            
            #endregion

            #region Numbers
            //blackBoxBillboardList = new Texture2D[10];
            //this.billboardList = new Texture2D[10];
            //activeBillboardList = new Texture2D[10];
            for(int i = 0; i < 10; i++)
            {
                string number = (i + 1 < 10 ? "0" : "") + (i + 1);
                blackBoxBillboardList[i] = content.Load<Texture2D>("2D/Other/num_" + number + "_blackbox");
                activeBillboardList[i] = content.Load<Texture2D>("2D/Other/num_" + number + "_active");
                this.billboardList[i] = content.Load<Texture2D>("2D/Other/num_" + number);
            }
            #endregion

            //LaserTex = content.Load<Texture2D>("textures/Lazar");
            //

            borders = content.Load<Texture2D>("2D/Options Menu/borders");
            
            Dock = content.Load<Texture2D>("2D/Splashes and Overlays/box_720x400_light");
            

            mainMenuBackground = content.Load<Texture2D>("2D/Splashes and Overlays/Background");
            
            mainMenuLogo = content.Load<Texture2D>("2D/Splashes and Overlays/BurningBoxesLogo01");
            
            MediaTexture = content.Load<Texture2D>("2D/Music Player/music");
            
            Resources.LaserTexture = LaserTex = content.Load<Texture2D>("textures/Lazar");
            
            Resources.LaserShader = LaserShader = content.Load<Effect>("Shaders/laser");
            
            Resources.AchievementToastTexture = AchievementToastTexture = content.Load<Texture2D>("2D/Splashes and Overlays/achievement");
            
            AchievementMenuTexture = content.Load<Texture2D>("2D/Objectives/ach_comp");

            Accomplishment.AchievementTexture = AchievementsCompTex = content.Load<Texture2D>("2D/Objectives/ach_compilation");
            
            AchievementLockedTexture = content.Load<Texture2D>("2D/Objectives/ach_locked");
            

            #region Buttons
            //if(Program.Game.AspectRatio < 1.4)
            //{
            //    buttonsTex = content.Load<Texture2D>("2D/Buttons/buttons_old");
            //    

            //    Rectangle buttonRect = new Rectangle(0, 0, 168, 41);

            //    resumeButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 5, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    restartButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    mainMenuButton = new SuperTextor(ref buttonsTex, new Vector2((RenderingDevice.Width * 0.53f), (RenderingDevice.Height * 0.75f)), new Rectangle(buttonRect.Width, buttonRect.Height, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    pauseQuitButton = new SuperTextor(ref buttonsTex, new Vector2((RenderingDevice.Width * 0.76f), (RenderingDevice.Height * 0.75f)), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    gameOverLevSelButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    gameOverRestartButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    instructionsButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.23f, RenderingDevice.Height * 0.84f), new Rectangle(buttonRect.Width, 0, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    quitButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.71f, RenderingDevice.Height * 0.755f), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    levelSelectButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.39f, RenderingDevice.Height * 0.755f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    startButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.07f, RenderingDevice.Height * 0.755f), buttonRect, SuperTextor.RenderPoint.UpLeft);
            //    optionsButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.55f, RenderingDevice.Height * 0.84f), new Rectangle(0, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    backButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.025f), new Rectangle(buttonRect.Width, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    yesButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.315f, RenderingDevice.Height * 0.65f), new Rectangle(0, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);
            //    noButton = new SuperTextor(ref buttonsTex, new Vector2(RenderingDevice.Width * 0.515f, RenderingDevice.Height * 0.65f), new Rectangle(buttonRect.Width, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), SuperTextor.RenderPoint.UpLeft);

            //    optionsTex = content.Load<Texture2D>("2D/Options Menu/options_fat");
            //    
            //    tabRect = new Rectangle(0, 0, 200, 66);

            //    higherOptionsBox = new SuperTextor(ref optionsTex, new Vector2(RenderingDevice.Width * 0.015f, RenderingDevice.Height * 0.1f), new Rectangle(0, tabRect.Height, 774, 334), SuperTextor.RenderPoint.UpLeft);
            //    lowerOptionsBox = new SuperTextor(ref optionsTex, new Vector2(RenderingDevice.Width * 0.015f, higherOptionsBox.LowerRight.Y + 30), new Rectangle(0, tabRect.Height + 334, 774, 160), SuperTextor.RenderPoint.UpLeft);
            //}
            //else
            //{
            buttonsTex = content.Load<Texture2D>("2D/Buttons/buttons");
            

            //Rectangle buttonRect = new Rectangle(0, 0, 210, 51);

            //resumeButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 5, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //restartButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //mainMenuButton = new Sprite(delegate { return buttonsTex; }, new Vector2((RenderingDevice.Width * 0.53f), (RenderingDevice.Height * 0.75f)), new Rectangle(buttonRect.Width, buttonRect.Height, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //pauseQuitButton = new Sprite(delegate { return buttonsTex; }, new Vector2((RenderingDevice.Width * 0.76f), (RenderingDevice.Height * 0.75f)), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            //gameOverLevSelButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.29f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //gameOverRestartButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.065f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            //instructionsButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.23f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, 0, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //quitButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.8f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //levelSelectButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.42f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //startButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.04f, RenderingDevice.Height * 0.75f), buttonRect, Sprite.RenderPoint.UpLeft);
            //backButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.75f, RenderingDevice.Height * 0.025f), new Rectangle(buttonRect.Width, buttonRect.Height * 3, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            //yesButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.315f, RenderingDevice.Height * 0.65f), new Rectangle(0, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //noButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.515f, RenderingDevice.Height * 0.65f), new Rectangle(buttonRect.Width, buttonRect.Height * 4, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            //continueButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.04f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 6, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

            //optionsButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 2, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //extrasButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 6, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //highScoreButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 7, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //savesButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(0, buttonRect.Height * 7, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //objectiveButton = new Sprite(delegate { return buttonsTex; }, new Vector2(RenderingDevice.Width * 0.61f, RenderingDevice.Height * 0.75f), new Rectangle(buttonRect.Width, buttonRect.Height * 5, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
            //}

            //windowsTabLight = new SuperTextor(ref optionsTex, new Vector2(higherOptionsBox.UpperLeft.X + tabRect.Width * 0.16f, higherOptionsBox.UpperLeft.Y - tabRect.Height), tabRect, SuperTextor.RenderPoint.UpLeft);
            //xboxTabLight = new SuperTextor(ref optionsTex, new Vector2(higherOptionsBox.UpperLeft.X + tabRect.Width * 1.03f, higherOptionsBox.UpperLeft.Y - tabRect.Height), new Rectangle(tabRect.Width * 2, 0, tabRect.Width, tabRect.Height), SuperTextor.RenderPoint.UpLeft);

            SaveSelectorTex = content.Load<Texture2D>("2D/Buttons/save_selection");
            
            #endregion

            #region Options
            optionsTex = content.Load<Texture2D>("2D/Options Menu/options_widescr");
            
            //Rectangle tabRect = new Rectangle(0, 0, 240, 80);

            //higherOptionsBox = new Sprite(delegate { return optionsTex; }, new Vector2(RenderingDevice.Width * 0.013f, RenderingDevice.Height * 0.12f), new Rectangle(0, tabRect.Height, 1248, 400), Sprite.RenderPoint.UpLeft);
            //lowerOptionsBox = new Sprite(delegate { return optionsTex; }, new Vector2(RenderingDevice.Width * 0.013f, higherOptionsBox.LowerRight.Y + 30), new Rectangle(0, tabRect.Height + 400, 1248, 192), Sprite.RenderPoint.UpLeft);

            //float xOffsetBorder = RenderingDevice.Width * 0.048f;
            //float yOffset = higherOptionsBox.UpperLeft.Y + RenderingDevice.Height * 0.087f;

            //difficultyBorderLight = new Sprite(delegate { return borders; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.728f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Width * 0.03f)), new Rectangle(0, 90, 175, 45), Sprite.RenderPoint.UpLeft);
            //difficultyVector = new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.728f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Width * 0.03f));
            difficultySlider = content.Load<Texture2D>("2D/Options Menu/DifficultySlider");
            

            //Rectangle arrowBox = new Rectangle(0, 0, 50, 40);
            optionsUI = content.Load<Texture2D>("2D/Options Menu/arrows");
            

            //rightLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.87f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Height * 0.055f)), arrowBox, Sprite.RenderPoint.UpLeft);
            //leftLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.68f), lowerOptionsBox.UpperLeft.Y + (RenderingDevice.Height * 0.055f)), new Rectangle(arrowBox.Width, 0, arrowBox.Width, arrowBox.Height), Sprite.RenderPoint.UpLeft);

            //secondRightLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.783f), yOffset + (RenderingDevice.Height * 0.17f)), arrowBox, Sprite.RenderPoint.UpLeft);
            //secondLeftLightArrow = new Sprite(delegate { return optionsUI; }, new Vector2(xOffsetBorder + (RenderingDevice.Width * 0.692f), yOffset + (RenderingDevice.Height * 0.17f)), new Rectangle(arrowBox.Width, 0, arrowBox.Width, arrowBox.Height), Sprite.RenderPoint.UpLeft);
            #endregion

            #region Level Select
            lockOverlay = content.Load<Texture2D>("2D/Level Select/lock_overlay");
            
            selectionGlow = content.Load<Texture2D>("2D/Level Select/selection-glow");
            
            Resources.starTex = starTex = content.Load<Texture2D>("2D/Level Select/stars");
            
            icons = content.Load<Texture2D>("2D/Level Select/icons");
            
            activeIcons = content.Load<Texture2D>("2D/Level Select/icons_active");
            
            qMark = content.Load<Texture2D>("2D/Level Select/question_mark");
            
            boxTex = content.Load<Texture2D>("2D/Level Select/box_440x260_light");
            
            //iconArray = new Sprite[14];
            //activeIconArray = new Sprite[14];
            //lockOverlays = new Sprite[14];
            //selectionGlows = new Sprite[14];
            //float x = 0, y = 0;
            //int index;

            //Rectangle iconRect = new Rectangle(0, 0, icons.Width / 7, icons.Height / 2 + 1);

            //for(int j = 0; j < 4; j++)
            //{
            //    if(j < 2)
            //        for(int i = 0; i < 5; i++)
            //        {
            //            index = j * 5 + i;
            //            x = (RenderingDevice.Width * 0.11f) + (RenderingDevice.Width * 0.195f * i);
            //            y = (RenderingDevice.Height * 0.245f) + (RenderingDevice.Height * 0.2f * j);
            //            iconArray[index] = new Sprite(delegate { return icons; }, new Vector2(x, y), new Rectangle(iconRect.Width * i, iconRect.Height * j, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
            //            lockOverlays[index] = new Sprite(delegate { return lockOverlay; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
            //            selectionGlows[index] = new Sprite(delegate { return selectionGlow; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
            //            activeIconArray[index] = new Sprite(delegate { return activeIcons; }, new Vector2(x, y), new Rectangle(iconRect.Width * i, iconRect.Height * j, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
            //        }
            //    else if(j == 2)
            //    {
            //        x = (RenderingDevice.Width * 0.11f);
            //        y = (RenderingDevice.Height * 0.245f) + (RenderingDevice.Height * 0.2f * j);
            //        iconArray[10] = new Sprite(delegate { return icons; }, new Vector2(x, y), new Rectangle(iconRect.Width * 5, 0, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
            //        lockOverlays[10] = new Sprite(delegate { return qMark; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
            //        selectionGlows[10] = new Sprite(delegate { return selectionGlow; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
            //        activeIconArray[10] = new Sprite(delegate { return activeIcons; }, new Vector2(x, y), new Rectangle(iconRect.Width * 5, 0, iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
            //    }
            //    else
            //        for(int i = 0; i < 3; i++)
            //        {
            //            index = 11 + i; // remember level 11 is index 10
            //            x = (RenderingDevice.Width * 0.11f) + (RenderingDevice.Width * 0.195f * i);
            //            y = (RenderingDevice.Height * 0.245f) + (RenderingDevice.Height * 0.2f * j);
            //            iconArray[index] = new Sprite(delegate { return icons; }, new Vector2(x, y), new Rectangle(iconRect.Width * (i == 0 ? 5 : 6), iconRect.Height * ((i + 1) % 2), iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
            //            lockOverlays[index] = new Sprite(delegate { return lockOverlay; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
            //            selectionGlows[index] = new Sprite(delegate { return selectionGlow; }, new Vector2(x, y), null, Sprite.RenderPoint.Center);
            //            activeIconArray[index] = new Sprite(delegate { return activeIcons; }, new Vector2(x, y), new Rectangle(iconRect.Width * (i == 0 ? 5 : 6), iconRect.Height * ((i + 1) % 2), iconRect.Width, iconRect.Height), Sprite.RenderPoint.Center);
            //        }
            //}

            //InfoBox = new Sprite(delegate { return boxTex; }, new Vector2(iconArray[9].LowerRight.X - boxTex.Width, iconArray[13].LowerRight.Y - boxTex.Height), null, Sprite.RenderPoint.UpLeft);
            #endregion

            EmptyTex = new Texture2D(RenderingDevice.GraphicsDevice, 1, 1);
            EmptyTex.SetData(new[] { Color.White });
            Resources.EmptyTex = EmptyTex;
            //FadeColor = new Color(0, 0, 0, 0);
            

            Resources.Plus1 = Plus1 = content.Load<Texture2D>("2D/Other/plus1");
            
            Resources.BarTexture = BarTexture = content.Load<Texture2D>("2D/Splashes and Overlays/spawnBar");
            

            //Rectangle r = new Rectangle(0, 0, 370, 123);
            levelOverlay = content.Load<Texture2D>("2D/Special Text/level");
            //Resources.LevelOverlay = LevelOverlay = new Sprite(delegate { return levelOverlay; }, new Vector2(-levelOverlay.Width * 0.5f, RenderingDevice.Height * 0.45f), null, Sprite.RenderPoint.Center);
            
            overlay = content.Load<Texture2D>("2D/Special Text/words");
            overlay2 = content.Load<Texture2D>("2D/Special Text/words2");
            //OverlayWords = new Sprite[15];
            //for(int i = 0; i < OverlayWords.Length - 3; i++)
            //    OverlayWords[i] = new Sprite(delegate { return overlay; }, new Vector2(RenderingDevice.Width + r.Width * 0.5f, (LevelOverlay.Center.Y + LevelOverlay.LowerRight.Y) * 0.52f), new Rectangle(r.Width * (i % 2), r.Height * (i / 2), r.Width, r.Height), Sprite.RenderPoint.Center);
            //OverlayWords[12] = new Sprite(delegate { return overlay2; }, new Vector2(LevelOverlay.UpperLeft.X - r.Width * 0.75f, LevelOverlay.UpperLeft.Y - r.Height * 0.45f), new Rectangle(0, 0, r.Width, r.Height), Sprite.RenderPoint.Center);
            //OverlayWords[13] = new Sprite(delegate { return overlay2; }, new Vector2(RenderingDevice.Width + r.Width * 0.5f, (LevelOverlay.Center.Y + LevelOverlay.LowerRight.Y) * 0.525f), new Rectangle(r.Width, 0, r.Width, r.Height - 2), Sprite.RenderPoint.Center);
            //OverlayWords[14] = new Sprite(delegate { return overlay2; }, new Vector2(RenderingDevice.Width + r.Width * 0.5f, (LevelOverlay.Center.Y + LevelOverlay.LowerRight.Y) * 0.525f), new Rectangle(0, r.Height, 550, r.Height + 2), Sprite.RenderPoint.Center);
            //Resources.OverlayWords = OverlayWords;
            

            UIBase = content.Load<Texture2D>("Font/UI base");
            //Resources.SurvivingBoxesBase = SurvivingBoxesBase = new Sprite(delegate { return UIBase; }, new Vector2(908.8f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            //Resources.SurvivingBoxesText = SurvivingBoxesText = new Sprite(delegate { return UIBase; }, new Vector2(908.8f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 100, 357, 50), Sprite.RenderPoint.UpLeft);
            //Resources.RemainingBoxesBase = RemainingBoxesBase = new Sprite(delegate { return UIBase; }, new Vector2(16.64f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            //Resources.RemainingBoxesText = RemainingBoxesText = new Sprite(delegate { return UIBase; }, new Vector2(16.64f, 14.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 50, 357, 50), Sprite.RenderPoint.UpLeft);
            //Resources.DestroyedBoxesBase = DestroyedBoxesBase = new Sprite(delegate { return UIBase; }, new Vector2(16.64f * RenderingDevice.TextureScaleFactor.X, RemainingBoxesBase.LowerRight.Y), new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            //Resources.DestroyedBoxesText = DestroyedBoxesText = new Sprite(delegate { return UIBase; }, new Vector2(16.64f * RenderingDevice.TextureScaleFactor.X, RemainingBoxesBase.LowerRight.Y), new Rectangle(0, 150, 357, 50), Sprite.RenderPoint.UpLeft);
            //Resources.TimeElapsedBase = TimeElapsedBase = new Sprite(delegate { return UIBase; }, new Vector2(908.8f * RenderingDevice.TextureScaleFactor.X, SurvivingBoxesBase.LowerRight.Y) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 357, 50), Sprite.RenderPoint.UpLeft);
            //Resources.TimeElapsedText = TimeElapsedText = new Sprite(delegate { return UIBase; }, new Vector2(908.8f * RenderingDevice.TextureScaleFactor.X, SurvivingBoxesBase.LowerRight.Y) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 200, 357, 50), Sprite.RenderPoint.UpLeft);
            

            scoreboardBase = content.Load<Texture2D>("Font/scoreboard");
            //Resources.ScoreboardBase = ScoreboardBase = new Sprite(delegate { return scoreboardBase; }, new Vector2(640, 39.4f) * RenderingDevice.TextureScaleFactor, new Rectangle(0, 0, 320, 50), Sprite.RenderPoint.Center);
            //Resources.ScoreboardText = ScoreboardText = new Sprite(delegate { return scoreboardBase; }, new Vector2(ScoreboardBase.UpperLeft.X + 65 * RenderingDevice.TextureScaleFactor.X, ScoreboardBase.Center.Y), new Rectangle(0, 50, 122, 50), Sprite.RenderPoint.Center);
            

            #region Music
            //Program.Cutter.WriteToLog(this, "Loading music");

            //MediaSystem.LoadAmbience(content);
            //
            MediaSystem.LoadSoundEffects(content);
            //
            MediaSystem.LoadVoiceActing(content);

            MediaSystem.OnGDMReset();
            #endregion

            #region Font
            Program.Cutter.WriteToLog(this, "Loading font");
            Resources.LCDFont = LCDFont = content.Load<SpriteFont>("Font/LCD");
            
            Resources.Font = Font = content.Load<SpriteFont>("Font/Ad-Font");
            
            SmallerFont = content.Load<SpriteFont>("Font/AD-Font_small");
            
            Resources.BiggerFont = BiggerFont = content.Load<SpriteFont>("Font/AD-Font_big");
            
            Resources.LCDNumbers = LCDNumbers = content.Load<Texture2D>("Font/LCD numbers");
            //UINumbers = new Dictionary<int, Rectangle>(LCDNumbers.Width / 19);
            //for(int i = 0; i < LCDNumbers.Width / 19; i++)
            //    UINumbers[i] = new Rectangle(i * 19, 0, 19, 22);
            //Resources.UINumbers = UINumbers;
            #endregion

            #region Levels
            bbEffect = content.Load<Effect>("Shaders/bbEffect");
            Level.Initialize(delegate { return bbEffect; }, this.billboardList, activeBillboardList, delegate { return Dispenser; });

            //Program.Cutter.WriteToLog(this, "Creating levels");
            //List<Vector3> billboardList = new List<Vector3>();
            //List<Tube> tubeList = new List<Tube>();
            //Dictionary<OperationalMachine, bool> machs = new Dictionary<OperationalMachine, bool>();
            //List<Keyframe> keyframeList = new List<Keyframe>();
            //Vector3 offset;

            //Level.Initialize(content.Load<Effect>("Shaders/bbEffect"), this.billboardList, activeBillboardList, delegate { return Dispenser; });

            //#region Level 0
            //Program.Cutter.WriteToLog(this, "Creating level 0");
            //for(int i = 0; i < 21; i++)
            //    tubeList.Add(new Tube(new Vector3(-23.182f + i, 0.241f, 7), true, false));

            //billboardList.Add(new Vector3(2.689f, 0.241f, 14));

            //machs.Add(new ClampedRotationMachine(1, 0, 2.25f, Vector3.UnitY, MathHelper.ToRadians(179), new Vector3(7.329f, 0.239f, 3.073f), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[0]["machine1"]; }, false, null, new Vector3(2.716f, 0.239f, 0.749f)),
            //    new BaseModel(delegate { return modelList[0]["machine1_glass"]; }, true, null, new Vector3(0.918f, 0.234f, 2.441f))), true);

            //level00 = new InstructionsLevel(1, 1, new Vector3(-20.182f, 0.241f, 13), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[0]["base"]; },
            //    //level00 = new Level(0, 20, 10, new Vector3(-20.182f, 0.241f, 13), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[0]["base"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[0]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[0]["glass2"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(13.942f, 0, -4.61f)), null, machs, tubeList, new LevelCompletionData(new TimeSpan(0, 0, 30), 100, 0), "Training");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            
            //#endregion

            //#region Level 1
            //Program.Cutter.WriteToLog(this, "Creating level 1");
            //for(int i = 0; i < 20; i++)
            //    tubeList.Add(new Tube(new Vector3(-15 + i, 0, 1), true, false));

            //billboardList.Add(new Vector3(-5.55f, -0.05f, 7));

            //machs.Add(new TranslateMachine(1, 9, new Vector3(0, 0, -6.5f), 1f, false,
            //    new BaseModel(delegate { return modelList[1]["machine1_glass"]; }, true, null, new Vector3(-5.55f, -0.05f, 3.85f))), true);

            ////keyframeList.Add(new Keyframe(new Vector3(0, -3, 0), Quaternion.Identity, 1.5f));
            ////keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 0.75f));
            ////keyframeList.Add(new Keyframe(new Vector3(0, 3, 0), Quaternion.Identity, 1.5f));
            ////keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 0.75f));

            ////machs.Add(new KeyframeMachine(0, keyframeList, true,
            ////    new BaseModel(delegate { return modelList[1]["machine1_auto"]; }, false, null, new Vector3(-0.7f, 1.5f, 4.495f)) { LocalOffset = new Vector3(0, 0, -1.1f) }), false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Vector3.UnitY * -3, 1.5f));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            //keyframeList.Add(new Keyframe(Vector3.UnitY * 3, 1.5f));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[1]["machine1_auto"]; }, false, null, new Vector3(-0.7f, 1.5f, 4f - 0.833333f)) { LocalOffset = new Vector3(0, 0, -1.1f) }), false);
            //keyframeList.Clear();

            //level01 = new Level(1, 10, 5, new Vector3(-13, 0, 10), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[1]["base"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[1]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[1]["glass2"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(8.264f, 0.241f, -5.041f)), null, machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 0, 30), 2000, 0), "Clock-in");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 2
            //Program.Cutter.WriteToLog(this, "Creating level 2");

            //for(int i = 0; i < 16; i++)
            //    tubeList.Add(new Tube(new Vector3(-48 + i, 4.414f, 7.2f), true, false));
            //for(int i = 0; i < 27; i++)
            //    tubeList.Add(new Tube(new Vector3(-23 + i, 4.414f + (i * 0.45f), 7.2f), true, false));

            //billboardList.Add(new Vector3(-27.5f, 4.506f, 13.5f));
            //billboardList.Add(new Vector3(10.438f, 12.571f, 13.5f));

            //machs.Add(new TranslateMachine(1, 7, Vector3.UnitZ * 5, 1.25f, false,
            //    new BaseModel(delegate { return modelList[2]["machine1"]; }, false, null, new Vector3(-27.5f, 4.506f, 5.867f)),
            //    new BaseModel(delegate { return modelList[2]["machine1_stripes"]; }, false, null, new Vector3(-27.5f, 4.414f, 5.796f))), true);

            //machs.Add(new TranslateMachine(2, 9, Vector3.UnitZ * 5, 1.25f, false,
            //    new BaseModel(delegate { return modelList[2]["machine2"]; }, false, null, new Vector3(10.438f, 12.571f, 4.835f)),
            //    new BaseModel(delegate { return modelList[2]["machine2_stripes"]; }, false, null, new Vector3(10.654f, 12.663f, 4.904f))), true);

            //level02 = new Level(2, 20, 5, new Vector3(-46, 4.414f, 15f), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[2]["base"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[2]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[2]["glass2"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(14.864f, -5.925f, -4.844f)), null, machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 1, 45), 2250, 1), "Is There A Help\n  Button?");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            
            //#endregion

            //#region Level 3
            //Program.Cutter.WriteToLog(this, "Creating level 3");

            //for(int i = 0; i < 18; i++)
            //{
            //    if(i < 15)
            //        tubeList.Add(new Tube(new Vector3(-1.349f + i, -10.073f, 3.2f), true, false));
            //    if(i < 17)
            //        tubeList.Add(new Tube(new Vector3(-34.849f + i, -4.017f, 8.2f), true, false));
            //    tubeList.Add(new Tube(new Vector3(6.151f, -6.573f + i, 3.2f), false, false));
            //}
            //for(int i = 0; i < 23; i++)
            //{
            //    if(i < 21)
            //        tubeList.Add(new Tube(new Vector3(-17.849f + i, -4.073f, 5.7f), true, false));
            //    tubeList.Add(new Tube(new Vector3(21.151f, -6.573f + i, 1.2f), false, false));
            //}
            //for(int i = 0; i < 31; i++)
            //    tubeList.Add(new Tube(new Vector3(23.651f - i, 18.503f, -1.8f), true, true));

            //billboardList.Add(new Vector3(-6.849f, -4.073f, 10f));
            //billboardList.Add(new Vector3(16.151f, -10.073f, 6f));
            //billboardList.Add(new Vector3(6.151f, 13.027f, 10f));

            //ClampedRotationMachine c1, c2, c3;

            //c1 = new ClampedRotationMachine(1, 2, 0.75f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-12.849f, -5.853f, 6.7f), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[3]["machine1_part1"]; }, false, null, new Vector3(-12.849f, -5.853f, 6.7f)));

            //c2 = new ClampedRotationMachine(1, -1, 0.75f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-4.849f, -2.303f, 6.7f), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[3]["machine1_part2"]; }, false, null, new Vector3(-4.849f, -2.303f, 6.7f)));

            //c3 = new ClampedRotationMachine(1, -1, 0.75f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(2.651f, -5.853f, 6.7f), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[3]["machine1_part3"]; }, false, null, new Vector3(2.651f, -5.853f, 6.7f)));

            //OperationalMachine.LinkMachines(c1, c2, c3);
            //machs.Add(c1, true);
            //machs.Add(c2, false);
            //machs.Add(c3, false);

            //float tubeY = -12.573f;

            //machs.Add(new TranslateMachine(2, 5, new Vector3(5, 0, 0), 1.5f, false,
            //    new BaseModel(delegate { return modelList[3]["machine2"]; }, false, null, new Vector3(16.151f, -9.40585f, 1.367f)),
            //    new BaseModel(delegate { return modelList[3]["machine2_stripes"]; }, false, null, new Vector3(16.185f, -10.073f, 1.4f)),
            //    new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false), new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false),
            //    new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false), new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false),
            //    new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false), new Tube(new Vector3(16.151f, tubeY++, 1.2f), false, false)), true);

            //machs.Add(new Cannon(3, 8, new Vector3(0, 0, 6), Vector3.UnitX, new CannonPathFinder(new Vector3(0, -11, 11), new Vector3(6.15f, 14, 4.2f), new Vector3(6.15f, -45.45f, 4.2f)), new Vector3(6.15f, 14, 4.2f), //195,
            //    new BaseModel(delegate { return modelList[3]["machine3"]; }, false, null, new Vector3(6.151f, 13.027f, 0f))), true);

            //level03 = new Level(3, 30, 10, new Vector3(-31.849f, -4.073f, 15), billboardList, Theme.Lava, (BaseModel)delegate { return modelList[3]["base"]; },
            //    new[] { new BaseModel(delegate { return modelList[3]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[3]["flags"]; }, false, false, Vector3.Zero) },
            //    new Goal(new Vector3(-10.099f, 18.62f, -6.5f)), new Goal(new Vector3(6.222f, -52.45f, -7f), Color.Blue), machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 3, 50), 3700, 2), "One Box Two Box\n      Red Box Blue Box");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 4
            //Program.Cutter.WriteToLog(this, "Creating level 4");

            //for(int i = 0; i < 17; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-47 + i, 4.9f, 7.2f), true, false));
            //    tubeList.Add(new Tube(new Vector3(3.995f, -14.598f + i, -0.4f), false, false));
            //    tubeList.Add(new Tube(new Vector3(-1.005f, -1.598f + i, -0.4f), false, false));
            //    tubeList.Add(new Tube(new Vector3(-13.505f, 6.902f - i, -0.8f), false, true));
            //}

            //for(int i = 0; i < 19; i++)
            //    tubeList.Add(new Tube(new Vector3(20.995f, 14.902f - i, -3.3f), false, true));

            //for(int i = 0; i < 11; i++)
            //    tubeList.Add(new Tube(new Vector3(19.495f + i, -5.598f, -3.3f), true, false));

            //machs.Add(new ClampedRotationMachine(1, 1, 1.7f, Vector3.UnitY, MathHelper.PiOver2, new Vector3(-20.65f, 4.974f, 6.362f), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[4]["machine1"]; }, false, null, new Vector3(-23.305f, 4.902f, 6.87f)), new Tube(new Vector3(-29.505f, 4.902f, 6.204f), true, false),
            //    new Tube(new Vector3(-28.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-27.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-26.505f, 4.902f, 6.204f), true, false),
            //    new Tube(new Vector3(-25.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-24.505f, 4.902f, 6.204f), true, false), new Tube(new Vector3(-23.505f, 4.902f, 6.204f), true, false)) { DampingMultiplier = 6 }, true);

            //machs.Add(new TranslateMachine(2, 7, new Vector3(19.7f, 0, 10f), 4.25f, false,
            //    new BaseModel(delegate { return modelList[4]["machine2"]; }, false, null, new Vector3(-18.885f, -12.536f, -6.854f))), true);

            //TranslateMachine m31, m32;

            //m31 = new TranslateMachine(3, 9, new Vector3(-5.9f, 0, 0), 1.75f, false,
            //    new BaseModel(delegate { return modelList[4]["machine3_part1"]; }, false, null, new Vector3(8.529f, -0.536f, 0.209f)));

            //m32 = new TranslateMachine(3, -1, new Vector3(5.9f, 0, 0), 1.75f, false,
            //    new BaseModel(delegate { return modelList[4]["machine3_part2"]; }, false, null, new Vector3(-5.961f, 12.464f, 0.209f)));

            //OperationalMachine.LinkMachines(m31, m32);
            //machs.Add(m31, true);
            //machs.Add(m32, false);
            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 5f));

            //KeyframeMachine k1, k2;

            //k1 = new KeyframeMachine(4, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[4]["machine4_part1"]; }, false, null, new Vector3(10.895f, 12.402f, 0.773f)));

            //keyframeList.Clear();

            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            //keyframeList.Add(new Keyframe(new Vector3(5, 0, 0), Quaternion.Identity, 2.3f));
            //keyframeList.Add(new Keyframe(0.4f));
            //keyframeList.Add(new Keyframe(new Vector3(-5, 0, 0), Quaternion.Identity, 2.3f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.Pi), 6.5f));
            //keyframeList.Add(new Keyframe(new Vector3(5, 0, 0), Quaternion.Identity, 2.3f));
            //keyframeList.Add(new Keyframe(0.4f));
            //keyframeList.Add(new Keyframe(new Vector3(-5, 0, 0), Quaternion.Identity, 2.3f));

            //k2 = new KeyframeMachine(4, 8, keyframeList, true,
            //    new BaseModel(delegate { return modelList[4]["machine4_part2"]; }, false, null, new Vector3(10.8f, 12.45f, 0f)));

            ////OperationalMachine.LinkMachines(k1, k2);
            //machs.Add(k1, true);
            //machs.Add(k2, false);

            //billboardList.Add(new Vector3(-17.305f, 4.902f, 14.87f));
            //billboardList.Add(new Vector3(-18.885f, -12.536f, 1f));
            //billboardList.Add(new Vector3(0, 0, 8.1f));
            //billboardList.Add(new Vector3(10.895f, 12.402f, 10.5f));

            //level04 = new IceLevel(4, 40, 10, new Vector3(-44.917f, 4.94f, 12.207f), billboardList, (BaseModel)delegate { return modelList[4]["base"]; },
            //    (BaseModel)delegate { return modelList[4]["extras"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[4]["glass1"]; }, true, false, Vector3.Zero), 
            //    new BaseModel(delegate { return modelList[4]["glass2"]; }, true, false, Vector3.Zero), 
            //    new BaseModel(delegate { return modelList[4]["glass3"]; }, true, false, Vector3.Zero), 
            //    new BaseModel(delegate { return modelList[4]["glass4"]; }, true, false, Vector3.Zero), 
            //    new BaseModel(delegate { return modelList[4]["glass5"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(31.654f, -5.558f, -7.572f)), machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 4, 10), 3550, 3), "Cold Shoulder");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 5
            //Program.Cutter.WriteToLog(this, "Creating level 5");

            //for(int i = 0; i < 20; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-46.658f + i, 17.528f, 14), true, false));
            //    tubeList.Add(new Tube(new Vector3(35.642f - i, -35.272f, 19.4f), true, true));
            //    tubeList.Add(new Tube(new Vector3(-15.158f - i, -12.472f, 6.4f), true, true));
            //}
            //for(int i = 0; i < 5; i++)
            //    tubeList.Add(new Tube(new Vector3(32.342f, -11.972f - i, 6), false, true));
            //for(int i = 0; i < 21; i++)
            //{
            //    Tube t = new Tube(new Vector3(16.006f - ((16.006f + 0.377f) / 21) * i, -35.73f - ((-35.73f + 24.259f) / 21) * i, 18.2f), true, true);
            //    t.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-30));
            //    tubeList.Add(t);
            //}
            //for(int i = 0; i < 22; i++)
            //    tubeList.Add(new Tube(new Vector3(32.342f, 20.048f - i, 6), false, true));
            //for(int i = 0; i < 11; i++)
            //    tubeList.Add(new Tube(new Vector3(0.644f - i, -23.879f, 17), true, true));

            //billboardList.Add(new Vector3(-27.261f, 17.528f, 23f));
            //billboardList.Add(new Vector3(24.382f, 17.528f, 21f));
            //billboardList.Add(new Vector3(32.342f, -3.972f, 14));
            //billboardList.Add(new Vector3(32.342f, -23.972f, 13));
            //billboardList.Add(new Vector3(-7.858f, -23.872f, 26));

            //machs.Add(new TranslateMachine(1, 10, new Vector3(0, 0, -5.5f), 1.5f, false,
            //    new BaseModel(delegate { return modelList[5]["machine1"]; }, true, null, new Vector3(-27.261f, 17.528f, 16.5f))) { DampingMultiplier = 2.5f }, true);

            //machs.Add(new RailMachine(2, 4, false, new Vector3(-42.5f, 0, 0), 6, 10, new Vector3(25.878f, 17.528f, 10), 2.5f, Vector3.UnitY, MathHelper.ToRadians(179), -Vector3.UnitX,
            //    new[] { new BaseModel(delegate { return modelList[5]["machine2_slidepart"]; }, false, null, new Vector3(24.418f, 17.528f, 6.861f)) },
            //    new BaseModel(delegate { return modelList[5]["machine2_rotatepart"]; }, false, null, new Vector3(21.249f, 17.528f, 7.676f)),
            //    new BaseModel(delegate { return modelList[5]["machine2_rotatepart_glass"]; }, true, null, new Vector3(19.392f, 17.528f, 9)),
            //    new BaseModel(delegate { return modelList[5]["machine2_ice"]; }, false, null, new Vector3(19.361f, 17.528f, 4.846f)) { Mass = 0.05f }), true);

            //#region machine 3
            //tubeY = -25.972f;

            //TranslateMachine m1, m2, m3, m4;

            //m1 = new TranslateMachine(3, 9, Vector3.UnitZ * -12f, 2f, false, 1.5f, 0.5f,
            //    new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -23.972f, 3.686f)),
            //    new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
            //    new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -23.892f, 6.246f)));

            //m4 = new TranslateMachine(3, 9, Vector3.UnitZ * -11.9f, 2f, false, 2f, 0f,
            //    new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -18.972f, 3.606f)),
            //    new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, 5.946f), false, true),
            //    new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -18.892f, 6.3f)));

            //tubeY += 5;

            //m3 = new TranslateMachine(3, 9, Vector3.UnitZ * 11.9f, 2f, false, 0.5f, 1.5f,
            //    new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -8.792f, -8.206f)),
            //    new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
            //    new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -8.892f, -5.646f)));

            //m2 = new TranslateMachine(3, 9, Vector3.UnitZ * 12f, 2f, false, 0, 2,
            //    new BaseModel(delegate { return modelList[5]["machine3"]; }, false, null, new Vector3(32.342f, -3.792f, -8.286f)),
            //    new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true), new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
            //    new Tube(new Vector3(32.342f, tubeY++, -5.946f), false, true),
            //    new BaseModel(delegate { return modelList[5]["machine3_stripes"]; }, false, null, new Vector3(32.342f, -3.892f, -5.7f)));

            //OperationalMachine.LinkMachines(m1, m2, m3, m4);

            //machs.Add(m1, true);
            //machs.Add(m2, false);
            //machs.Add(m3, false);
            //machs.Add(m4, false);

            ////keyframeList.Add(Keyframe.Zero);
            ////keyframeList.Add(Keyframe.Zero);
            ////// dummy machine to get the middle set of stripes in
            ////machs.Add(new KeyframeMachine(0, keyframeList, false,
            ////    new BaseModel(delegate { return modelList[5]["base_stripes"]; }, false, null, Vector3.Zero)), false);
            //#endregion

            //OperationalMachine m5, m6;

            //keyframeList.Clear();

            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 15.5f), Quaternion.Identity, 4f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 4.65f), Quaternion.Identity, 2f));

            //BaseModel rotors = new BaseModel(delegate { return modelList[5]["machine4_rotors"]; }, false, null, new Vector3(32.242f, -29.272f, 0.529f));
            //rotors.Ent.AngularVelocity = new Vector3(0, 0, 7);
            //rotors.CommitInitialVelocities();

            //m5 = new KeyframeMachine(4, 6, keyframeList, false,
            //    rotors,
            //    new BaseModel(delegate { return modelList[5]["machine4_base"]; }, false, null, new Vector3(32.242f, -29.272f, 1.133f)));

            //keyframeList.Clear();

            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 14.2f), Quaternion.Identity, 3.8f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.2f));

            //m6 = new KeyframeMachine(4, -1, keyframeList, false,
            //    new BaseModel(delegate { return modelList[5]["machine4_glass"]; }, true, null, new Vector3(32.342f, -29.372f, 5.479f)));

            //keyframeList.Clear();
            ////Machine.LinkMachines(m5, m6);
            //machs.Add(m5, true);
            //machs.Add(m6, false);

            //machs.Add(new TranslateMachine(5, 5, new Vector3(0, 0, -6.5f), 1.5f, false,
            //    new BaseModel(delegate { return modelList[5]["machine5_glass"]; }, true, null, new Vector3(-7.858f, -23.872f, 20.232f))), true);

            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 12.5f, 0), Quaternion.Identity, 3f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(new Vector3(0, -12.5f, 0), Quaternion.Identity, 3f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));

            //m5 = new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[5]["machine5_part2"]; }, false, null, new Vector3(-15.325f, -23.972f, 16)),
            //    new BaseModel(delegate { return modelList[5]["machine5_part2_glass2"]; }, true, null, new Vector3(-14.41629f - .445f, -23.98160f, 17.49686f - 0.1229568f)));

            //keyframeList.Clear();

            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 12.5f, 0), Quaternion.Identity, 3f));
            //keyframeList.Add(new Keyframe(new Vector3(7, 0, 0), Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 0.5f));
            //keyframeList.Add(new Keyframe(new Vector3(-7, 0, 0), Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(new Vector3(0, -12.5f, 0), Quaternion.Identity, 3f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 2.5f));

            //m6 = new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[5]["machine5_part2_glass"]; }, true, null, new Vector3(-14.208f, -23.972f, 16f)));

            //OperationalMachine.LinkMachines(m5, m6);
            //machs.Add(m5, false);
            //machs.Add(m6, false);

            //level05 = new IceLevel(5, 50, 20, new Vector3(-45.658f, 17.528f, 26), billboardList, (BaseModel)delegate { return modelList[5]["base"]; }, (BaseModel)delegate { return modelList[5]["effects"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[5]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[5]["glass2"]; }, true, false, Vector3.Zero), 
            //    new BaseModel(delegate { return modelList[5]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[5]["glass4"]; }, true, false, Vector3.Zero),
            //    new BaseModel(delegate { return modelList[5]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[5]["glass6"]; }, true, false, Vector3.Zero),
            //    new BaseModel(delegate { return modelList[5]["glass7"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(-37.658f, -12.472f, 2.775f)), machs, tubeList, new LevelCompletionData(new TimeSpan(0, 7, 55), 10100, 7), "Water Cooler Talk");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 6
            //Program.Cutter.WriteToLog(this, "Creating level 6");

            //billboardList.Add(new Vector3(-30.927f, 9.851f, 18));
            //billboardList.Add(new Vector3(-28.427f, -1.399f, 5));
            //billboardList.Add(new Vector3(-10.927f, -22.649f, 15));
            //billboardList.Add(new Vector3(6.323f, -37.199f, 0));
            //billboardList.Add(new Vector3(6.323f, 0.851f, 15));
            //billboardList.Add(new Vector3(6.323f, 16.331f, 14));

            //for(int i = 0; i < 20; i++)
            //    tubeList.Add(new Tube(new Vector3(-49.927f + i, 9.851f, 9), true, false));
            //for(int i = 0; i < 5; i++)
            //    tubeList.Add(new Tube(new Vector3(-28.427f, -16.149f - i, -3), false, true));
            //for(int i = 0; i < 14; i++)
            //    tubeList.Add(new Tube(new Vector3(-29.927f + i, -22.649f, -3), true, false));
            //for(int i = 0; i < 10; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-5.927f + i, -22.649f, 7), true, false));
            //    tubeList.Add(new Tube(new Vector3(6.323f, 24.871f + i, 6), false, false));
            //}
            //for(int i = 0; i < 19; i++)
            //    tubeList.Add(new Tube(new Vector3(6.323f, -10.269f + i, 6), false, false));
            //for(int i = 0; i < 17; i++)
            //    tubeList.Add(new Tube(new Vector3(4.823f + i, 36.371f, 6), true, false));

            //machs.Add(new TranslateMachine(1, 5, new Vector3(0, 0, -6.2f), 1.25f, false,
            //    new BaseModel(delegate { return modelList[6]["machine1_glass"]; }, true, null, new Vector3(-30.927f, 9.851f, 12.07f))), true);

            //BaseModel ww = new BaseModel(delegate { return modelList[6]["waterwheel"]; }, false, null, new Vector3(-28.927f, 9.851f, 3));
            //ww.Ent.Material = new BEPUphysics.Materials.Material(0.25f, 0.25f, 0);
            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.Pi), 10));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.Pi), 10));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true, ww), false);

            //tubeY = 4.351f;
            //machs.Add(new TranslateMachine(2, 2, new Vector3(0, -10.5f, 0), 2.5f, false,
            //    new BaseModel(delegate { return modelList[6]["machine2_base"]; }, false, null, new Vector3(-28.427f, 0.85108f, -2.7f)),
            //    new BaseModel(delegate { return modelList[6]["machine2_glass"]; }, true, null, new Vector3(-28.427f, 0f, 0.6f)),
            //    new BaseModel(delegate { return modelList[6]["machine2_stripes"]; }, false, null, new Vector3(-28.35680f, -0.19892f, -2.65f)),
            //    new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
            //    new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
            //    new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
            //    new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true),
            //    new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true), new Tube(new Vector3(-28.427f, tubeY--, -3.033f), false, true)), true);

            //tubeY = -15.927f;
            //machs.Add(new TranslateMachine(3, 7, new Vector3(0, 0, 10), 1.5f, false,
            //    new BaseModel(delegate { return modelList[6]["machine3"]; }, false, null, new Vector3(-11.196f, -22.649f, -8.103f)),
            //    new BaseModel(delegate { return modelList[6]["machine3_stripes"]; }, false, null, new Vector3(-11.4268f, -22.64892f, -2.9f)),
            //    new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
            //    new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
            //    new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
            //    new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false),
            //    new Tube(new Vector3(tubeY++, -22.649f, -3), true, false), new Tube(new Vector3(tubeY++, -22.649f, -3), true, false)), true);

            //machs.Add(new TranslateMachine(4, 10, new Vector3(0, 25.461f, 14.7f), 4.5f, false,
            //    new BaseModel(delegate { return modelList[6]["machine4"]; }, false, null, new Vector3(6.323f, -37.199f, -4.65f))), true);

            //machs.Add(new TranslateMachine(5, 7, new Vector3(0, 0, -6.2f), 1.25f, false,
            //    new BaseModel(delegate { return modelList[6]["machine6_glass"]; }, true, null, new Vector3(6.323f, 0.851f, 9.6f))), true);

            //tubeY = 8.831f;
            //m1 = new TranslateMachine(6, 5, new Vector3(0, 0, 6), 1.25f, false,
            //    new BaseModel(delegate { return modelList[6]["machine5_part1_base"]; }, false, null, new Vector3(6.323f, 12.351f, 3.167f)),
            //    new BaseModel(delegate { return modelList[6]["machine5_part1_glass"]; }, true, null, new Vector3(6.323f, 12.351f, 6.6f)),
            //    new BaseModel(delegate { return modelList[6]["machine5_part1_stripes"]; }, true, null, new Vector3(6.323f, 12.351f, 3.1f)),
            //    new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(6.323f, tubeY++, 3), false, false), new Tube(new Vector3(6.323f, tubeY++, 3), false, false));
            //m2 = new TranslateMachine(6, -1, new Vector3(0, 0, -6), 1.25f, false,
            //    new BaseModel(delegate { return modelList[6]["machine5_part2_base"]; }, false, null, new Vector3(6.323f, 20.351f, 9.167f)),
            //    new BaseModel(delegate { return modelList[6]["machine5_part2_glass"]; }, true, null, new Vector3(6.323f, 20.351f, 12.6f)),
            //    new BaseModel(delegate { return modelList[6]["machine5_part2_stripes"]; }, true, null, new Vector3(6.323f, 20.351f, 9.1f)),
            //    new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false),
            //    new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false),
            //    new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false),
            //    new Tube(new Vector3(6.323f, tubeY++, 9), false, false), new Tube(new Vector3(6.323f, tubeY++, 9), false, false));

            //OperationalMachine.LinkMachines(m1, m2);
            //machs.Add(m1, true);
            //machs.Add(m2, false);

            //level06 = new Level6(50, 10, new Vector3(-46.927f, 9.851f, 17), billboardList, (BaseModel)delegate { return modelList[6]["base"]; }, (BaseModel)delegate { return modelList[6]["extras"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[6]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[6]["glass2"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(24.573f, 36.351f, 1.862f)), machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 8, 45), 6300, 8), "Operation: Pretzel Cup", null);

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 7
            //// Level 7 offset is: (-10.102, -16.072, -1.026)
            //// offset is: level before offset - level after offset
            //Program.Cutter.WriteToLog(this, "Creating level 7");

            //billboardList.Add(new Vector3(-36.5f, -13, 18));
            //billboardList.Add(new Vector3(-42, -17, 10));
            //billboardList.Add(new Vector3(-19, -17, 12));
            //billboardList.Add(new Vector3(-10, 5, 3));
            //billboardList.Add(new Vector3(2, 35, 5));
            //billboardList.Add(new Vector3(4, -11, 11));
            //billboardList.Add(new Vector3(32.5f, -4, 10));

            //offset = new Vector3(-10.102f, -16.072f, -1.026f);
            //Vector3 tubeTemp = new Vector3(-46.75f, -10.75f, 7);
            //for(Vector3 i = Vector3.Zero; i.Y < 19.53f; i.Y += 0.93f)
            //    tubeList.Add(new Tube(tubeTemp - i - offset, false, true));
            //tubeTemp = new Vector3(-20.065f, -35.55f, -4);
            //for(Vector3 i = Vector3.Zero; i.Y < 20; i.Y += 1)
            //    tubeList.Add(new Tube(tubeTemp + i - offset, false, false));
            //tubeTemp = new Vector3(-20.565f, 9.05f, 5);
            //for(Vector3 i = Vector3.Zero; i.X < 10; i.X += 1)
            //    tubeList.Add(new Tube(tubeTemp + i - offset, true, false));
            //tubeTemp = new Vector3(10, -20, -1);
            //for(Vector3 i = Vector3.Zero; i.X < 20; i.X += 1)
            //    tubeList.Add(new Tube(tubeTemp + i - offset, true, false));

            ////machs.Add(new TranslateMachine(1, new Vector3(0, 0, 7), 1f, false,
            ////    new BaseModel(level07Dict["machine1_glass"]; }, true, null, new Vector3(-36.697f, -13.1f, 10.1f), true)), true);
            ////machs.Add(new RotateMachine(1, 1.5f, new Vector3(-90, 0, 0), RotateMachine.RotationType.Clamped, new Vector3(-36.697f, -13.1f, 13.1f),
            ////    new BaseModel(level07Dict["machine1_glass"]; }, true, null, new Vector3(-36.697f, -13.1f, 16.1f), true)), true);
            //machs.Add(new TranslateMachine(1, 10, new Vector3(0, 0, -5.5f), 1f, false,
            //    new BaseModel(delegate { return modelList[7]["machine1_glass"]; }, true, null, new Vector3(-36.697f, -13.1f, 10.1f))), true);

            //Tube[] keyTubes = new Tube[5];
            //for(int i = 0; i < 5; i++)
            //{
            //    keyTubes[i] = new Tube(new Vector3(-38.259f + i, -17.211f, 4.921f), true, false);
            //    keyTubes[i].BecomeKeybasedTube(3);
            //}

            //machs.Add(new TranslateMachine(2, 9, new Vector3(15.4f, 0, 0), 3.5f, false,
            //    new BaseModel(delegate { return modelList[7]["machine2_glass"]; }, true, null, new Vector3(-38.64f, -17.211f, 7.121f)), keyTubes[0], keyTubes[1],
            //    keyTubes[2], keyTubes[3], keyTubes[4]), true);

            //// dummy machine to work with the above machine
            //machs.Add(new HoldRotationMachine(3, 3, Vector3.Zero), true);

            //List<Vector3> vectorList = new List<Vector3>();
            //for(int i = 0; i < 5; i++)
            //    vectorList.Add(new Vector3(-12.15f - i, -33.25f, 4) - offset);
            //for(int i = 0; i < 5; i++)
            //    vectorList.Add(new Vector3(-20.05f, -25.65f - i, 4) - offset);
            //for(int i = 0; i < 5; i++)
            //    vectorList.Add(new Vector3(-27.65f + i, -33.25f, 4) - offset);
            //for(int i = 0; i < 5; i++)
            //    vectorList.Add(new Vector3(-20.05f, -40.85f + i, 4) - offset);
            //machs.Add(new ContinuingRotationMachine(0, -1, -Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 13.5f, 4, new Vector3(-20.05f, -33.25f, 3.397f) - offset,
            //    new BaseModel(delegate { return modelList[7]["machine3"]; }, false, null, new Vector3(-20.05f, -33.25f, 3.397f) - offset),
            //    new Tube(vectorList[0], true, true), new Tube(vectorList[1], true, true), new Tube(vectorList[2], true, true), new Tube(vectorList[3], true, true),
            //    new Tube(vectorList[4], true, true), new Tube(vectorList[5], false, true), new Tube(vectorList[6], false, true), new Tube(vectorList[7], false, true),
            //    new Tube(vectorList[8], false, true), new Tube(vectorList[9], false, true), new Tube(vectorList[10], true, false), new Tube(vectorList[11], true, false),
            //    new Tube(vectorList[12], true, false), new Tube(vectorList[13], true, false), new Tube(vectorList[14], true, false), new Tube(vectorList[15], false, false),
            //    new Tube(vectorList[16], false, false), new Tube(vectorList[17], false, false), new Tube(vectorList[18], false, false), new Tube(vectorList[19], false, false)) { DampingMultiplier = 10 }, false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(new Vector3(0, 16.5f, 16.5f), Quaternion.Identity, 5));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), 1.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, .75f));
            //k1 = new KeyframeMachine(4, -1, keyframeList, false,
            //    new BaseModel(delegate { return modelList[7]["machine4"]; }, false, null, new Vector3(-9.985f, 3.5f, -5f)));

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(new Vector3(0, 16.5f, 16.5f), Quaternion.Identity, 5));
            ////keyframeList.Add(new Keyframe(new Vector3(0, 3.15f, 0), Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), 3));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), 1.5f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 3.15f, 0), Quaternion.Identity, .75f));
            //k2 = new KeyframeMachine(4, 2, keyframeList, false,
            //    new BaseModel(delegate { return modelList[7]["machine4_part2"]; }, false, null, new Vector3(-9.987f, 3.5f, -6.8f)) { LocalOffset = new Vector3(0, 0, -1.6f) });

            //OperationalMachine.LinkMachines(k1, k2);
            //machs.Add(k1, true);
            //machs.Add(k2, false);

            //machs.Add(new TranslateMachine(5, 1, new Vector3(0, -35.532f, 14.497f), 5, false,
            //    new BaseModel(delegate { return modelList[7]["machine5"]; }, false, null, new Vector3(2.419f, 36.908f, -2.39f))), true);

            //machs.Add(new ClampedRotationMachine(6, 0, 4, Vector3.UnitZ, MathHelper.ToRadians(179), new Vector3(12.048f, -6.85f, 4.247f), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[7]["machine6"]; }, false, null, new Vector3(12.048f - 10, -6.85f, 4.247f))), true);

            ////machs.Add(new TranslateMachine(7, new Vector3(0, 0, 6.2f), 1f, false,
            ////    new BaseModel(level07Dict["machine7_part1_glass"]; }, true, null, new Vector3(32.478f, -3.789f, 3.305f), true)), true);
            ////machs.Add(new RotateMachine(7, 1.5f, new Vector3(0, -90, 0), RotateMachine.RotationType.Clamped, new Vector3(32.478f, -3.789f, 6.15f),
            ////    new BaseModel(level07Dict["machine7_part1_glass"]; }, true, null, new Vector3(32.478f, -3.789f, 9.15f), true)), true);
            //machs.Add(new TranslateMachine(7, 10, new Vector3(0, 0, -6.2f), 1f, false,
            //    new BaseModel(delegate { return modelList[7]["machine7_part1_glass"]; }, true, null, new Vector3(32.478f, -3.789f, 3.305f))), true);

            //BaseModel bm = new BaseModel(delegate { return modelList[7]["machine7_part2"]; }, true, null, new Vector3(35.191f, -3.998f, 1.189f));
            //bm.Remover = true;
            ////bm.SetRenderOptions(content.Load<Effect>("Shaders/lava"), LaserTex, content.Load<Texture2D>("textures/noise"));
            //machs.Add(new TranslateMachine(7, -1, new Vector3(0, 0, 4.8f), 5, true, bm), false);

            //level07 = new Level(7, 30, 10, new Vector3(-36.5f, 3, 20), billboardList, Theme.Beach, (BaseModel)delegate { return modelList[7]["base"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[7]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass2"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[7]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass4"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[7]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass6"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[7]["glass7"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[7]["glass8"]; }, true, false, Vector3.Zero),
            //        new BaseModel(delegate { return modelList[7]["glass9"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(43.102f, -3.928f, -9f)), null, machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 6, 5), 4500, 7), "Sun, Surf, and\n    Snackie-Snacks");

            //machs.Clear();
            //keyframeList.Clear();
            //tubeList.Clear();
            //billboardList.Clear();
            //#endregion

            //#region Level 8
            //Program.Cutter.WriteToLog(this, "Creating level 8");

            //billboardList.Add(new Vector3(6.079f, -3.424f, 16));
            //billboardList.Add(new Vector3(37.24f, -29.924f, 5));
            //billboardList.Add(new Vector3(-6.024f, -29.924f, 1));
            //billboardList.Add(new Vector3(-31.2f, 0.861f, 8));
            //billboardList.Add(new Vector3(-29.421f, 17.576f, 1));
            //billboardList.Add(new Vector3(-11.321f, 35.076f, 17));
            //billboardList.Add(new Vector3(17.408f, 35.151f, 31));
            //billboardList.Add(new Vector3(39.079f, 15.076f, 6.5f));

            //for(int i = 0; i < 20; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-24.421f + i, -3.424f, 7), true, false)); // first set
            //    tubeList.Add(new Tube(new Vector3(12.979f - i, -29.924f, -4), true, true)); // fifth set
            //    tubeList.Add(new Tube(new Vector3(-20.021f - i, -29.924f, 10), true, true)); // sixth set
            //    tubeList.Add(new Tube(new Vector3(-42.771f, -32.174f + i, 6), false, false)); // seventh set
            //    tubeList.Add(new Tube(new Vector3(-29.421f, -1.424f + i, -4), false, false)); // eighth set
            //    tubeList.Add(new Tube(new Vector3(-31.321f + i, 35.076f, 10), true, false)); // ninth set
            //}
            //for(int i = 0; i < 28; i++)
            //    tubeList.Add(new Tube(new Vector3(10.707f + i, -3.424f, -2), true, false)); // second set
            //for(int i = 0; i < 27; i++)
            //    tubeList.Add(new Tube(new Vector3(39.579f, -1.924f - i, -2), false, true)); // third set
            //for(int i = 0; i < 5; i++)
            //    tubeList.Add(new Tube(new Vector3(41.24f - i, -29.924f, -2), true, true)); // fourth set
            //for(int i = 0; i < 22; i++)
            //    tubeList.Add(new Tube(new Vector3(16.079f + i, 35.076f, -1), true, false)); // tenth set
            //for(int i = 0; i < 19; i++)
            //    tubeList.Add(new Tube(new Vector3(39.079f, 36.576f - i, -1), false, true)); // last (eleventh) set

            //machs.Add(new ClampedRotationMachine(1, 1, 2.5f, Vector3.UnitY, MathHelper.ToRadians(179), new Vector3(6.079f, -3.424f, 3), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[8]["machine1_base"]; }, false, null, new Vector3(6.079f, -3.424f, 3) + new Vector3(-4.629f, 0, -2.324f)),
            //    new BaseModel(delegate { return modelList[8]["machine1_glass"]; }, true, null, new Vector3(-0.421f, -3.424f, 0))) { DampingMultiplier = 5 }, true);

            //machs.Add(new TranslateMachine(2, 7, new Vector3(-13.78359f, 0, 15.6f), 2.75f, false,
            //    new BaseModel(delegate { return modelList[8]["machine2"]; }, false, null, new Vector3(37.57499f, -29.425f, -6.65f))), true);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 6));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 6));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[8]["spiked_roller"]; }, false, null, new Vector3(12.1f, -29.824f, 1.3f))), false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.Pi), 6));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathHelper.Pi), 6));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[8]["spiked_roller"]; }, false, null, new Vector3(6.9f, -29.824f, 1.3f))), false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 18.5f), Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 2f), Quaternion.Identity, 0.5f));
            //BaseModel glassTemp = new BaseModel(delegate { return modelList[8]["machine3_glass"]; }, true, null, new Vector3(-9.872f, -29.424f, -6.633f));
            //glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-16.321f, -29.424f, -6.024f);
            //glassTemp.Ent.Position = new Vector3(-16.321f, -24.924f, -6.024f);
            //machs.Add(new KeyframeMachine(3, -1, keyframeList, false, glassTemp), false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 18.5f), Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 0.5f));
            //glassTemp = new BaseModel(delegate { return modelList[8]["machine3_base"]; }, false, null, new Vector3(-11.7f, -29.424f, -8.296f));
            //glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-16.321f, -29.424f, -6.024f);
            //glassTemp.Ent.Position = new Vector3(-16.321f, -29.424f, -6.024f);
            //machs.Add(new KeyframeMachine(3, 9, keyframeList, false, glassTemp), true);
            //keyframeList.Clear();

            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 8));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 8));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[8]["machine4_dome"]; }, false, null, new Vector3(-35.424f, -7.424f, 3.5f))), false);
            //keyframeList.Clear();

            //keyframeList.Add(new Keyframe(-Vector3.UnitZ, Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(new Vector3(-2, -3, 0), Quaternion.Identity, 1f));
            //machs.Add(new KeyframeMachine(4, 8, keyframeList, false,
            //    new BaseModel(delegate { return modelList[8]["machine4_slide"]; }, false, null, new Vector3(-31.2f, 0.861f, 0.5f))), true);
            //keyframeList.Clear();

            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 17.5f), Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 2.25f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 2f), Quaternion.Identity, 0.5f));
            //glassTemp = new BaseModel(delegate { return modelList[8]["machine5_glass"]; }, true, null, new Vector3(-29.231f, 21.626f, -5.774f));
            //glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-29.321f, 28.076f, -5.131f);
            //glassTemp.Ent.Position = new Vector3(-29.321f, 28.076f, -5.131f);
            //machs.Add(new KeyframeMachine(5, -1, keyframeList, false, glassTemp), false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 17.5f), Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 2.25f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 0.5f));
            //glassTemp = new BaseModel(delegate { return modelList[8]["machine5_base"]; }, false, null, new Vector3(-29.321f, 23.821f, -7.803f));
            //glassTemp.Ent.CollisionInformation.LocalPosition = glassTemp.Ent.Position - new Vector3(-29.321f, 28.076f, -5.131f);
            //glassTemp.Ent.Position = new Vector3(-29.321f, 28.076f, -5.131f);
            //machs.Add(new KeyframeMachine(5, 9, keyframeList, false, glassTemp), true);
            //keyframeList.Clear();

            //machs.Add(new TranslateMachine(6, 7, new Vector3(27.95f, 0, 17.735f), 4, false,
            //    new BaseModel(delegate { return modelList[8]["machine6"]; }, false, null, new Vector3(-18.503f, 35.07602f, 5.1f)) { LocalOffset = new Vector3(-2.616649f, 0, -1.590488f) }), true);

            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.75f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.75f));
            //machs.Add(new KeyframeMachine(7, 9, keyframeList, true,
            //    new BaseModel(delegate { return modelList[8]["machine7_rim"]; }, false, null, new Vector3(17.408f, 35.147f, 15.9f)),
            //    new BaseModel(delegate { return modelList[8]["machine7_glass"]; }, true, null, new Vector3(17.408f, 35.147f, 15.9f))), true);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(new Vector3(-5.6f, 0, 0), Quaternion.Identity, 0.3f));
            //keyframeList.Add(new Keyframe(new Vector3(5.5f, 0, 0), Quaternion.Identity, 0.6f));
            //machs.Add(new KeyframeMachine(8, 8, keyframeList, true,
            //    new BaseModel(delegate { return modelList[8]["machine8"]; }, false, null, new Vector3(41.779f, 15.076f, -1.5f))) { NoStop = true }, true);

            ////machs.Add(new ContinuingRotationMachine(0, -1, Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 0, 0.5f,
            ////    new Vector3(27.579f, -3.424f, 2), new BaseModel(delegate { return modelList[8]["fan"]; }, false, null, new Vector3(27.579f, -3.424f, 2))), false);
            ////machs.Add(new ContinuingRotationMachine(0, -1, Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 0, 0.5f,
            ////    new Vector3(31.079f, -3.424f, 3), new BaseModel(delegate { return modelList[8]["fan"]; }, false, null, new Vector3(31.079f, -3.424f, 3))), false);
            ////machs.Add(new ContinuingRotationMachine(0, -1, Vector3.UnitZ, Vector3.UnitX, MathHelper.PiOver2, 4, 0, 0.5f,
            ////    new Vector3(34.579f, -3.424f, 2), new BaseModel(delegate { return modelList[8]["fan"]; }, false, null, new Vector3(34.579f, -3.424f, 2))), false);

            //level08 = new Level(8, 50, 10, new Vector3(-22.421f, -3.424f, 16), billboardList, Theme.Beach, (BaseModel)delegate { return modelList[8]["base"]; },
            //    new[] { new BaseModel(delegate { return modelList[8]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass2"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[8]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass4"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[8]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass6"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[8]["glass7"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass8"]; }, true, false, Vector3.Zero),
            //        new BaseModel(delegate { return modelList[8]["glass9"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[8]["glass10"]; }, true, false, Vector3.Zero),
            //        new BaseModel(delegate { return modelList[8]["glass11"]; }, true, false, Vector3.Zero) },
            //        new Goal(new Vector3(31.279f, 15.076f, -9.4f)), null, machs, tubeList,
            //        new LevelCompletionData(new TimeSpan(0, 8, 10), 3300, 7), "Pressure Gauge");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 9
            //Program.Cutter.WriteToLog(this, "Creating level 9");

            //// elongated set (thirteenth)
            //tubeList.Add(new Tube(new Vector3(-27.092f, 29.201f, -3.3f), false, false));
            //for(int i = 0; i < 4; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-25.092f, 30.021f + i, -3.3f), false, false));
            //    tubeList.Add(new Tube(new Vector3(-29.092f, 30.021f + i, -3.3f), false, false));
            //}
            //for(int i = 0; i < 5; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-24.592f, 34.201f + i, -3.3f), false, false)); // fourteenth set (inside box, shorter, before turn)
            //    tubeList.Add(new Tube(new Vector3(18.408f, 47.201f - i, -5.3f), false, true)); // twenty-second set
            //    tubeList.Add(new Tube(new Vector3(18.408f, 42.201f - i, -0.3f), false, true)); // twenty-third set
            //}
            //for(int i = 0; i < 6; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-33.592f + i, 8.701f, 4.2f), true, false)); // second set
            //    tubeList.Add(new Tube(new Vector3(-23.592f + i, 3.701f, 4.2f), true, false)); // third set
            //    tubeList.Add(new Tube(new Vector3(-26.072f + i, 40.701f, -3.3f), true, false)); // fifteenth set (inside box, shorter, after turn)
            //    tubeList.Add(new Tube(new Vector3(-14.092f + i, 40.701f, -5.3f), true, false)); // eighteenth set
            //}
            //for(int i = 0; i < 23; i++)
            //    tubeList.Add(new Tube(new Vector3(-2.592f - i, 21.701f, -3.3f), true, true)); // eleventh set
            //for(int i = 0; i < 24; i++)
            //    tubeList.Add(new Tube(new Vector3(-13.592f + i, 3.701f, 4.2f), true, false)); // fourth set
            //for(int i = 0; i < 7; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(34.408f + i, 3.701f, 1.2f), true, false)); // fifth set
            //    tubeList.Add(new Tube(new Vector3(9.908f + i, 45.701f, -5.3f), true, false)); // twenty-first set
            //}
            //for(int i = 0; i < 18; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(42.908f, 5.201f - i, 1.2f), false, true)); // sixth set
            //    tubeList.Add(new Tube(new Vector3(-14.092f + i, 45.701f, -5.3f), true, false)); // ninteenth set
            //}
            //for(int i = 0; i < 51; i++)
            //    tubeList.Add(new Tube(new Vector3(44.408f - i, -14.299f, 1.2f), true, true)); // seventh set
            //for(int i = 0; i < 21; i++)
            //{
            //    Tube t = new Tube(new Vector3(-8.921f + (0.245211f * i), -9.444f + (0.910518f * i), -1.391f - (0.095684f * i)), false, true);
            //    t.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(255));
            //    tubeList.Add(t); // ninth set (slanted)
            //}
            //for(int i = 0; i < 10; i++)
            //    tubeList.Add(new Tube(new Vector3(-29.592f, 34.201f + i, -3.3f), false, false)); // sixteenth set (inside box, longer, before turn)
            //for(int i = 0; i < 9; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-9.092f, -17.799f + i, -1.3f), false, false)); // eighth set
            //    tubeList.Add(new Tube(new Vector3(-27.092f, 20.201f + i, -3.3f), false, false)); // twelveth set
            //}
            //for(int i = 0; i < 11; i++)
            //    tubeList.Add(new Tube(new Vector3(-31.092f + i, 45.701f, -3.3f), true, false)); // seventeenth set (inside box, longer, after turn)
            //for(int i = 0; i < 17; i++)
            //    tubeList.Add(new Tube(new Vector3(-49.592f + i, 8.701f, 7.2f), true, false)); // first set
            //for(int i = 0; i < 11; i++)
            //    tubeList.Add(new Tube(new Vector3(-4.092f, 9.201f + i, -3.3f), false, false)); // tenth set
            //for(int i = 0; i < 12; i++)
            //    tubeList.Add(new Tube(new Vector3(-2.092f + i, 40.701f, -0.3f), true, false)); // twentieth set
            //for(int i = 0; i < 25; i++)
            //    tubeList.Add(new Tube(new Vector3(18.408f, 42.247f - i, -5.255f), false, true)); // last set
            //tubeList.Add(new Tube(new Vector3(15.908f, 40.701f, -0.3f), true, false));
            ////tubeList.Add(new Tube(new Vector3(-2.555f, 8.332f, -3.31f), false, false) { Height = 2.182f, Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-100)) }); // first tilted tube
            ////tubeList.Add(new Tube(new Vector3(-10.519f, -9.931f, -1.29f), false, false) { Height = 2.182f, Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-100)) }); // second tilted tube

            //billboardList.Add(new Vector3(-33.592f, 8.701f, 16f));
            //billboardList.Add(new Vector3(-21.092f, 8.701f, 8));
            //billboardList.Add(new Vector3(2.408f, 0, 8.5f));
            //billboardList.Add(new Vector3(6.994f, 3.701f, 11));
            //billboardList.Add(new Vector3(30.408f, 3.701f, 7));
            //billboardList.Add(new Vector3(10.908f, -13.5f, 5.5f));
            //billboardList.Add(new Vector3(-17.592f, 45.701f, -1));
            //billboardList.Add(new Vector3(-5.592f, 40.701f, 12));
            //billboardList.Add(new Vector3(6.408f, 45.701f, -1));

            //machs.Add(new TranslateMachine(1, 5, new Vector3(0, 0, -6.5f), 1.25f, false,
            //    new BaseModel(delegate { return modelList[9]["machine1_glass"]; }, true, null, new Vector3(-33.592f, 8.701f, 10.4f))), true);

            //tubeY = -27.592f;
            //machs.Add(new TranslateMachine(2, 7, new Vector3(0, -5, 0), 1f, false,
            //    new BaseModel(delegate { return modelList[9]["machine2"]; }, false, null, new Vector3(-21.092f, 8.701f, 4.36f)),
            //    new BaseModel(delegate { return modelList[9]["machine2_stripes"]; }, false, null, new Vector3(-21.092f, 8.701f, 4.4f)),
            //    new BaseModel(delegate { return modelList[9]["machine2_glass"]; }, true, null, new Vector3(-21.01690f, 8.62007f, 7.43633f)),
            //    new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++, 8.701f, 4.2f), true, false),
            //    new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false),
            //    new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false), new Tube(new Vector3(tubeY++ + 6, 8.701f, 4.2f), true, false)), true);

            //keyframeList.Add(new Keyframe(new Vector3(0, 5, 0), Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(new Vector3(0, KeyframeMachine.DelayConstant, 0), Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 6f));
            //keyframeList.Add(new Keyframe(new Vector3(0, -KeyframeMachine.DelayConstant, 0), Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(new Vector3(0, -5, 0), Quaternion.Identity, 1f));
            //k1 = new KeyframeMachine(3, 10, keyframeList, true,
            //    new BaseModel(delegate { return modelList[9]["machine3"]; }, false, null, new Vector3(2.408f, -2.243f, 4.359f))) { NoStop = true };
            //Cannon c = new Cannon(3, 8, new Vector3(0, 0, 6), Vector3.UnitX, new CannonPathFinder(new Vector3(0, -9, 9), new Vector3(2.408f, 10.667f, 5.25f), new Vector3(2.408f, -44.524f, 5.25f)), new Vector3(2.408f, 10.667f, 5.25f), //220,
            //    new BaseModel(delegate { return modelList[9]["machine3_cannon"]; }, false, null, new Vector3(2.408f, 9.327f, 0.7f)));
            //c.SetInputs(k1);
            //c.SetActivationType(ActivationType.JustDeactivated);
            //machs.Add(c, false);
            //machs.Add(k1, true);
            //keyframeList.Clear();

            //machs.Add(new TranslateMachine(4, 0, new Vector3(19.5f, 0, 13.5f), 5f, false,
            //    new BaseModel(delegate { return modelList[9]["machine4"]; }, false, null, new Vector3(6.994f, 3.701f, -2.209f))), true);

            //machs.Add(new TranslateMachine(5, 2, new Vector3(0, 0, 11f), 3.5f, false,
            //    new BaseModel(delegate { return modelList[9]["machine5"]; }, false, null, new Vector3(30.408f, 3.701f, -7.85f))), true);

            //machs.Add(new ClampedRotationMachine(6, 2, 0.5f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(22.908f, -16.799f, 2.8f), Vector3.UnitY,
            //    new BaseModel(delegate { return modelList[9]["machine6_slant"]; }, false, null, new Vector3(22.908f, -16.799f, 2.8f))), true);
            //machs.Add(new ClampedRotationMachine(6, -1, 0.5f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(10.908f, -11.799f, 2.8f), Vector3.UnitY,
            //    new BaseModel(delegate { return modelList[9]["machine6_flat"]; }, false, null, new Vector3(10.908f, -11.799f, 2.8f))), false);
            //machs.Add(new ClampedRotationMachine(6, -1, 0.5f, -Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-1.902f, -16.799f, 2.8f), Vector3.UnitY,
            //    new BaseModel(delegate { return modelList[9]["machine6_slant"]; }, false, null, new Vector3(-1.902f, -16.799f, 2.8f))), false);

            //keyframeList.Add(new Keyframe(new Vector3(0, -14.5f, 2.7f), Quaternion.Identity, 3.5f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, -2.5f), Quaternion.Identity, 1));
            //keyframeList.Add(new Keyframe(new Vector3(0, 14.5f, -2.7f), Quaternion.Identity, 3.5f));
            //keyframeList.Add(new Keyframe(new Vector3(0, 0, 2.5f), Quaternion.Identity, 1));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[9]["auto_belt"]; }, false, null, new Vector3(2.408f, -17.544f, 0.66f))), false);
            //keyframeList.Clear();

            //machs.Add(new ClampedRotationMachine(0, -1, 1.5f, Vector3.UnitZ, -MathHelper.PiOver4, new Vector3(-27.092f, 33.201f, -1.7f), -Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[9]["auto_switcher"]; }, false, null, new Vector3(-27.092f, 33.201f, -1.7f))), false);

            //tubeY = -20.092f;
            //machs.Add(new TranslateMachine(7, 9, new Vector3(0, -5, 0), 1.5f, false,
            //    new BaseModel(delegate { return modelList[9]["machine7"]; }, false, null, new Vector3(-17.592f, 45.701f, -4.733f)),
            //    new BaseModel(delegate { return modelList[9]["machine7_stripes"]; }, false, null, new Vector3(-17.592f, 45.701f, -5.1f)),
            //    new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
            //    new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
            //    new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false)), true);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Vector3.UnitZ * 14, Quaternion.Identity, 2.5f));
            //keyframeList.Add(new Keyframe(Vector3.UnitX * 2, Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.PiOver2), 1f));
            //machs.Add(new KeyframeMachine(8, 4, keyframeList, false,
            //    new BaseModel(delegate { return modelList[9]["machine8"]; }, false, null, new Vector3(-5.592f, 40.701f, -5.298f))), true);
            //keyframeList.Clear();

            //tubeY = 3.908f;
            //keyframeList.Add(new Keyframe(Vector3.UnitY * -5, Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(Vector3.UnitX * 6, Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(Vector3.UnitZ * 5, Quaternion.Identity, 1f));
            //machs.Add(new KeyframeMachine(9, 10, keyframeList, false,
            //    new BaseModel(delegate { return modelList[9]["machine9"]; }, false, null, new Vector3(6.408f, 45.701f, -4.733f)),
            //    new BaseModel(delegate { return modelList[9]["machine9_stripes"]; }, false, null, new Vector3(6.408f, 45.701f, -5.1f)),
            //    new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
            //    new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false),
            //    new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false), new Tube(new Vector3(tubeY++, 45.701f, -5.3f), true, false)), true);

            //level09 = new Level9(40, 10, new Vector3(-46.592f, 8.701f, 13), billboardList, (BaseModel)delegate { return modelList[9]["base"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[9]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[9]["glass2"]; }, true, false, Vector3.Zero),
            //    (BaseModel)delegate { return modelList[9]["flags"]; } }, new Goal(new Vector3(18.414f, 14.703f, -9.4f)),
            //    new Goal(new Vector3(2.312f, -44.524f, -7), Color.Blue), machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 7, 10), 3200, 11), "No. No there is not.");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 10
            //Program.Cutter.WriteToLog(this, "Creating level 10");

            //for(int i = 0; i < 20; i++)
            //    tubeList.Add(new Tube(new Vector3(-47.047f + i, 16.124f, 21), true, false)); // first set
            //for(int i = 0; i < 4; i++)
            //    tubeList.Add(new Tube(new Vector3(-22.04736f - i, -12.876f, 18), true, true)); // second set
            //for(int i = 0; i < 13; i++)
            //    tubeList.Add(new Tube(new Vector3(-27.54736f, -11.376f - i, 18), false, true)); // third set
            //for(int i = 0; i < 8; i++)
            //    tubeList.Add(new Tube(new Vector3(-29.04736f + i, -25.876f, 18), true, false)); // fourth set
            //for(int i = 0; i < 32; i++)
            //    tubeList.Add(new Tube(new Vector3(-9.04736f + i, -25.87586f, 1), true, false)); // inserted set 1
            //for(int i = 0; i < 8; i++)
            //    tubeList.Add(new Tube(new Vector3(24.45264f, -27.37586f + i, 1), false, false)); // inserted set 2
            //for(int i = 0; i < 25; i++)
            //    tubeList.Add(new Tube(new Vector3(18.953f + i, -4.134f, 1.5f), true, false)); // long set part 1

            //for(int i = 0; i < 20; i++)
            //    tubeList.Add(new Tube(new Vector3(189.953f + i, -4.134f, 1.5f), true, false)); // long set part 2
            //for(int i = 0; i < 27; i++)
            //    tubeList.Add(new Tube(new Vector3(243.007f, -19.33f - i, 0), false, true)); // sixth set
            //for(int i = 0; i < 38; i++)
            //    tubeList.Add(new Tube(new Vector3(242.953f, 0.624f + i, 5), false, false)); // seventh set
            //for(int i = 0; i < 2; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(254.703f + i, -3.884f, -0.5f), true, false)); // eighth set
            //    tubeList.Add(new Tube(new Vector3(274.703f + i, -3.884f, -0.5f), true, false)); // ninth set
            //    tubeList.Add(new Tube(new Vector3(294.703f + i, -3.884f, -0.5f), true, false)); // tenth set
            //}

            //billboardList.Add(new Vector3(-44.979f, 16.116f, 39));
            //billboardList.Add(new Vector3(-6.279f, 16.116f, 22));
            //billboardList.Add(new Vector3(0.203f, 9.616f - 14, 18));
            //billboardList.Add(new Vector3(-19.64830f, -25.884f, 22f));
            //billboardList.Add(new Vector3(24.45264f, -13.52587f, 16));
            //billboardList.Add(new Vector3(208.453f, -4.184f, 9.5f));
            //billboardList.Add(new Vector3(243.005f, -4.184f, 13));
            //billboardList.Add(new Vector3(242.999f, -9.08f, 9));
            //billboardList.Add(new Vector3(242.408f, 21.116f, 12.5f));
            //billboardList.Add(new Vector3(242.953f, 27.116f, 12));

            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), 2.25f));
            //machs.Add(new KeyframeMachine(1, 2, keyframeList, true,
            //    new BaseModel(delegate { return modelList[10]["machine1"]; }, false, null, new Vector3(-44.797f, 16.116f, 32))), true);
            //keyframeList.Clear();

            //machs.Add(new RailMachine(2, 6, true, new Vector3(-17, 0, 0), 7.5f, 3.5f, new Vector3(-7.247f, 16.116f, 17.45f), 2.5f, Vector3.UnitY, MathHelper.ToRadians(110),
            //    -Vector3.UnitX,
            //    new[] { new BaseModel(delegate { return modelList[10]["machine2_base"]; }, false, null, new Vector3(-7.247f, 16.116f, 13.788f)) },
            //    new BaseModel(delegate { return modelList[10]["machine2_bucket"]; }, false, null, new Vector3(-7.247f, 16.116f, 16.713f)),
            //    new BaseModel(delegate { return modelList[10]["machine2_glass"]; }, true, null, new Vector3(-7.247f, 16.116f, 18.45f))), true);

            //machs.Add(new RailMachine(3, 4, true, new Vector3(0, -23.75f, 0), 7.5f, 3.5f, new Vector3(-1.797f, 16.116f, 10.5f), 2.5f, Vector3.UnitX, MathHelper.ToRadians(110),
            //    Vector3.UnitY,
            //    new[] { new BaseModel(delegate { return modelList[10]["machine3_base"]; }, false, null, new Vector3(-1.797f, 16.116f, 8.014f)) },
            //    new BaseModel(delegate { return modelList[10]["machine3_bucket"]; }, false, null, new Vector3(-1.797f, 16.116f, 9.958f)),
            //    new BaseModel(delegate { return modelList[10]["machine3_glass"]; }, true, null, new Vector3(-1.797f, 16.116f, 11.45f))), true);

            //machs.Add(new TranslateMachine(2500, -1, new Vector3(21.185f, 0, -21.392f), 4.5f, true, 1f, 1f,
            //    new BaseModel(delegate { return modelList[10]["slab1"]; }, false, null, new Vector3(-7.882f, -12.821f, 9.935f))), false);
            ////machs.Add(new TranslateMachine(2500, -1, new Vector3(21.067f, 0, 21.259f), 4.5f, true, 1f, 1f,
            ////    new BaseModel(delegate { return modelList[10]["slab2"]; }, false, null, new Vector3(-18.055f, -25.843f, 13.924f) - new Vector3(21.067f, 0, 21.259f))), false);

            //Material newFriction = new Material(0.5f, 0.5f, 0);
            //machs.Add(new ClampedRotationMachine(4, 5, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-19.64830f, -25.884f, 16.036f),
            //    Vector3.UnitZ,
            //    new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-19.64830f, -25.884f, 16.036f))) { Friction = newFriction }, true);
            //machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-17.35016f, -25.884f, 14.107f),
            //    Vector3.UnitZ,
            //    new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-17.35016f, -25.884f, 14.107f))) { Friction = newFriction }, false);
            //machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-15.05203f, -25.884f, 12.179f),
            //    Vector3.UnitZ,
            //    new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-15.05203f, -25.884f, 12.179f))) { Friction = newFriction }, false);
            //machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-12.75390f, -25.884f, 10.251f),
            //    Vector3.UnitZ,
            //    new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-12.75390f, -25.884f, 10.251f))) { Friction = newFriction }, false);
            //machs.Add(new ClampedRotationMachine(4, -1, 0.5f, Vector3.UnitY, MathHelper.PiOver4, new Vector3(-10.45576f, -25.884f, 8.332f),
            //    Vector3.UnitZ,
            //    new BaseModel(delegate { return modelList[10]["machine4"]; }, false, null, new Vector3(-10.45576f, -25.884f, 8.332f))) { Friction = newFriction }, false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 8));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 8));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[10]["machine4_auto"]; }, false, null, new Vector3(-5, -26, 0.87438f)) { LocalOffset = new Vector3(0, 0, -0.95271f) }), false);
            //keyframeList.Clear();

            ////machs.Add(new TruckMachine(5, 6, 6f, 0.75f, MathHelper.PiOver2, Vector3.UnitX * 22.5f, Vector3.UnitX, Vector3.UnitY,
            ////    -Vector3.UnitZ, new Vector3(0.203f, -30.384f, 17.5f), new Vector3(0.203f, -21.384f, 17.5f), new Vector3(-3.773f, -25.884f, 17.524f),
            ////    new Vector3(4.227f, -25.884f, 17.524f),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_1_wheels"]; }, false, null, new Vector3(-3.773f, -25.884f, 17.524f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_1_wheels"]; }, false, null, new Vector3(4.227f, -25.884f, 17.524f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_1_moving_glass"]; }, true, null, new Vector3(0.203f, -30.384f, 15.25f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_1_moving_glass"]; }, true, null, new Vector3(0.203f, -21.384f, 15.25f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_1_base"]; }, false, null, new Vector3(0.203f, -25.884f, 17)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_1_glass"]; }, true, null, new Vector3(0.203f, -25.884f, 19))), true);

            ////machs.Add(new TruckMachine(5, 8, 6f, 0.75f, MathHelper.PiOver2, Vector3.UnitY * -20, -Vector3.UnitY, Vector3.UnitX,
            ////    Vector3.UnitX, new Vector3(27.664f, -3.863f, 8.5f), new Vector3(18.664f, -3.863f, 8.5f), new Vector3(23.164f, -7.887f, 8.524f),
            ////    new Vector3(23.164f, 0.113f, 8.524f),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_2_wheels"]; }, false, null, new Vector3(23.164f, -7.887f, 8.524f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_2_wheels"]; }, false, null, new Vector3(23.164f, 0.113f, 8.524f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_2_moving_glass"]; }, true, null, new Vector3(25.414f, -3.863f, 8.5f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_2_moving_glass"]; }, true, null, new Vector3(20.914f, -3.863f, 8.5f)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_2_base"]; }, false, null, new Vector3(23.164f, -4.376f, 10)),
            ////    new BaseModel(delegate { return modelList[10]["machine5_auto_2_glass"]; }, true, null, new Vector3(23.164f, -4.363f, 10))), false);

            //machs.Add(new TranslateMachine(5, 8, Vector3.UnitZ * 10, 1.25f, false,
            //    new BaseModel(delegate { return modelList[10]["machine5"]; }, false, null, new Vector3(24.45264f, -13.52587f, -2.20833f))), true);

            //machs.Add(new TranslateMachine(6, 7, Vector3.UnitZ * -6.5f, 1.25f, false,
            //    new BaseModel(delegate { return modelList[10]["machine6_glass"]; }, true, null, new Vector3(208.453f, -4.184f, 3.887f))), true);

            //machs.Add(new TranslateMachine(2000, -1, new Vector3(-25.656f, 0, -14.978f), 5.5f, true, 1.5f, 1.5f,
            //    new BaseModel(delegate { return modelList[10]["slab3"]; }, false, null, new Vector3(222.444f, -4.187f, 5.017f))), false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));

            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[10]["machine7_rotatepart"]; }, false, null, new Vector3(243.005f, -4.184f, 7.65f))) { Friction = new Material(1.2f, 1.2f, 0) }, false);
            //keyframeList.Clear();

            //machs.Add(new TranslateMachine(7, 6, Vector3.UnitX * 2.5f, 1f, false,
            //    new BaseModel(delegate { return modelList[10]["machine7_plusX"]; }, false, null, new Vector3(243.187f, -4.184f, 9.5f))), true);
            //machs.Add(new TranslateMachine(7, -1, Vector3.UnitY * 2.5f, 1f, false,
            //    new BaseModel(delegate { return modelList[10]["machine7_plusY"]; }, false, null, new Vector3(243.003f, -4.124f, 9.5f))), false);
            //machs.Add(new TranslateMachine(7, -1, Vector3.UnitY * -2.5f, 1f, false,
            //    new BaseModel(delegate { return modelList[10]["machine7_minusY"]; }, false, null, new Vector3(243.003f, -4.424f, 9.5f))), false);
            //machs.Add(new TranslateMachine(7, -1, Vector3.UnitX * -2.5f, 1f, false,
            //    new BaseModel(delegate { return modelList[10]["machine7_minusX"]; }, false, null, new Vector3(242.867f, -4.184f, 9.5f))), false);

            //machs.Add(new ClampedRotationMachine(8, 0, 1f, Vector3.UnitX, MathHelper.ToRadians(55), new Vector3(243.003f, -20.917f, 0.15f), Vector3.UnitZ,
            //    new BaseModel(delegate { return modelList[10]["machine8"]; }, false, null, new Vector3(243.003f, -20.917f, 0.15f))) { DampingMultiplier = 2 }, true);

            //keyframeList.Add(new Keyframe(new Vector3(-5, 0, 0), Quaternion.Identity, 1f));
            //keyframeList.Add(new Keyframe(new Vector3(-0.000000001f, 0, 0), 1.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, 5.25f));
            //keyframeList.Add(new Keyframe(new Vector3(0.000000001f, 0, 0), 1.5f));
            //keyframeList.Add(new Keyframe(new Vector3(5, 0, 0), Quaternion.Identity, 1f));
            //k1 = new KeyframeMachine(9, 0, keyframeList, true,
            //    new BaseModel(delegate { return modelList[10]["machine9_pusher"]; }, false, null, new Vector3(249.408f, 21.116f, 5.2f)));
            //k1.NoStop = true;

            //CardinalSpline3D path2 = new CardinalSpline3D();
            //path2.ControlPoints.Add(-1, new Vector3(226.356f - .942f, 21.096f, 0));
            //path2.ControlPoints.Add(0, new Vector3(236.356f, 21.096f, 6.5f));
            //path2.ControlPoints.Add(2, new Vector3(258.42f, 21.316f, 19.5f));
            //path2.ControlPoints.Add(3, new Vector3(258.42f + 10.942f, 21.316f, 25));
            //c = new Cannon(9, 8, new Vector3(0, 0, 6), Vector3.UnitY, new CannonPathFinder(path2, 22.064f, 2, true), new Vector3(236.356f, 21.096f, 4.5f), //140,
            //    new BaseModel(delegate { return modelList[10]["machine9_cannon"]; }, false, null, new Vector3(237.856f, 21.016f, 0.706f)));
            //c.SetInputs(k1);
            //c.SetActivationType(ActivationType.JustDeactivated);
            //machs.Add(k1, true);
            //machs.Add(c, false);
            //keyframeList.Clear();

            //machs.Add(new TranslateMachine(10, 10, Vector3.UnitZ * -6.5f, 1.25f, false,
            //    new BaseModel(delegate { return modelList[10]["machine10_glass"]; }, true, null, new Vector3(242.953f, 27.116f, 7.777f))), true);

            //keyframeList.Add(new Keyframe(Vector3.UnitZ * -6, Quaternion.Identity, 0.5f));
            //keyframeList.Add(new Keyframe(Vector3.UnitZ * 6, Quaternion.Identity, 6f));
            //BaseModel pillar = new BaseModel(delegate { return modelList[10]["auto_pillar"]; }, false, null, new Vector3(242.953f, 31.116f, 17));
            //BaseModel plane = new BaseModel(new BEPUphysics.Entities.Prefabs.Box(new Vector3(242.95779f, 31.13339f, 11.83277f), 3.934f, 3.934f, 0.01f), new Vector3(242.95779f, 31.13339f, 11.83277f));
            //plane.Remover = true;
            //plane.UsesLaserSound = false;
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true, pillar, plane) { NoStop = true }, false);

            //keyframeList.Clear();
            //Button[] buttons = new Button[5];
            //buttons[0] = new Button(-Vector3.UnitY, new BaseModel(delegate { return modelList[10]["button1"]; }, false, null, new Vector3(242.803f, 39.4f, 8)));
            //buttons[1] = new Button(-Vector3.UnitX, new BaseModel(delegate { return modelList[10]["button2"]; }, false, null, new Vector3(258.42f, 21.316f, 19.5f)));
            //buttons[2] = new Button(Vector3.UnitY, new BaseModel(delegate { return modelList[10]["button4"]; }, false, null, new Vector3(243.053f, -19.635f, -4.5f)));
            //buttons[3] = new Button(Vector3.UnitZ, new BaseModel(delegate { return modelList[10]["button3"]; }, false, null, new Vector3(242.999f, -48.68f, -2.052f)));
            //buttons[4] = new Button(Vector3.UnitZ, new BaseModel(delegate { return modelList[10]["button3"]; }, false, null, new Vector3(235.803f, -4.184f, 4.697f)));
            //keyframeList.Add(new Keyframe(new Vector3(0, 3.5f, 0), Quaternion.Identity, 1.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, float.PositiveInfinity));
            //KeyframeMachine machine = new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[10]["door2"]; }, false, null, new Vector3(309.403f, -2.17f, 4.35f))//,
            //    /*new BaseModel(delegate { return modelList[10]["door1_stripes"]; }, false, null, new Vector3(309.403f, -3.301f, 4.35f))*/);
            //machine.SetInputs(buttons);
            //machine.SetActivationType(ActivationType.IsActive);
            //machs.Add(machine, false);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(new Vector3(0, -3.5f, 0), Quaternion.Identity, 1.5f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.Identity, float.PositiveInfinity));
            //machine = new KeyframeMachine(0, 4, keyframeList, true,
            //    new BaseModel(delegate { return modelList[10]["door1"]; }, false, null, new Vector3(309.403f, -5.269f, 4.35f))//,
            //    /* new BaseModel(delegate { return modelList[10]["door2_stripes"]; }, false, null, new Vector3(309.403f, -3.801f, 4.35f))*/);
            //machine.SetInputs(buttons);
            //machine.SetActivationType(ActivationType.IsActive);
            //machs.Add(machine, false);

            //tubeY = 310.703f;
            //TranslateMachine machine2 = new TranslateMachine(0, -1, Vector3.UnitX * -62, 20, false,
            //    new BaseModel(delegate { return modelList[10]["long_belt"]; }, false, null, new Vector3(364.203f, -3.886f, 1.083f)),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false), new Tube(new Vector3(tubeY++, -3.886f, 1), true, false),
            //    new Tube(new Vector3(tubeY++, -3.886f, 1), true, false)) { Kinetic = true };
            //machine2.SetInputs(machine);
            //machine2.SetActivationType(ActivationType.JustDeactivated);
            //machs.Add(machine2, false);

            //level10 = new Level10(40, 20, new Vector3(-44.979f, 16.116f, 43f), billboardList, Theme.Sky, (BaseModel)delegate { return modelList[10]["base"]; }, (BaseModel)delegate { return modelList[10]["base_two"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[10]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[10]["glass2"]; }, true, false, Vector3.Zero),
            //    new BaseModel(delegate { return modelList[10]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[10]["glass4"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(42.953f, -4.134f, 5.5f), true), machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 15, 0), 7200, 14), "One Day, Three Cents,\n    and 9000 Miles Into\n    the Sky Later");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level 11
            //Program.Cutter.WriteToLog(this, "Creating level 11");

            //for(int i = 0; i < 17; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-47.21f + i, 24.649f, 12.2f), true, false)); // first set
            //    tubeList.Add(new Tube(new Vector3(27.79f, 1.149f - i, 0.2f), false, true)); // sixth set
            //}
            //for(int i = 0; i < 13; i++)
            //    tubeList.Add(new Tube(new Vector3(1.79f + i, 24.649f, 12.2f), true, false)); // second set
            //for(int i = 0; i < 14; i++)
            //    tubeList.Add(new Tube(new Vector3(8.97f + i, 13.649f, 12.2f), true, false)); // third set
            //for(int i = 0; i < 12; i++)
            //    tubeList.Add(new Tube(new Vector3(27.29f - i, 13.649f, -0.8f), true, true)); // fourth set
            //for(int i = 0; i < 8; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(13.79f, 15.149f - i, -0.8f), false, true)); // fifth set
            //    tubeList.Add(new Tube(new Vector3(-27.70972f, -29.35069f + i, -6.8f), false, false)); // thirteenth set
            //    tubeList.Add(new Tube(new Vector3(-27.70972f, -13.35069f + i, -1.8f), false, false)); // fourteenth set
            //    tubeList.Add(new Tube(new Vector3(-26.20972f - i, 8.14931f, 3.2f), true, true)); // sixteenth (last) set
            //}
            //for(int i = 0; i < 15; i++)
            //{
            //    Tube t = new Tube(new Vector3(27.79f, -15.851f - i, -0.8f), false, true);
            //    t.BecomeKeybasedTube(6);
            //    tubeList.Add(t); // seventh set
            //}
            //for(int i = 0; i < 25; i++)
            //    tubeList.Add(new Tube(new Vector3(30.29f - i, -33.351f, -2.8f), true, true)); // eighth set
            //for(int i = 0; i < 12; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-17.70972f, -1.85069f + i, 1.2f), false, false)); // ninth set
            //    tubeList.Add(new Tube(new Vector3(-19.20972f + i, 11.64931f, 1.2f), true, false)); // tenth set
            //    tubeList.Add(new Tube(new Vector3(-5.70972f, 13.14931f - i, 1.2f), false, true)); // eleventh set
            //}
            //for(int i = 0; i < 4; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-22.20972f - i, -27.85069f, -6.8f), true, true)); // twelvth set
            //    tubeList.Add(new Tube(new Vector3(-27.70972f, 2.64931f + i, 3.2f), false, false)); // fifteenth set
            //}

            //billboardList.Add(new Vector3(-14.7f, 24.649f, 15.5f));
            //billboardList.Add(new Vector3(8.29f, 24.649f, 20f));
            //billboardList.Add(new Vector3(32.09f, 17.149f, 12f));
            //billboardList.Add(new Vector3(14.29f, 4.649f, 8.5f));
            //billboardList.Add(new Vector3(27.79f, -7.531f, 5));
            //billboardList.Add(new Vector3(27.79f, -16.851f, 3.5f));
            //billboardList.Add(new Vector3(1.29f, -33.351f, 8f));
            //billboardList.Add(new Vector3(-17.7f, -33.35f, 15));
            //billboardList.Add(new Vector3(-5.71f, -2.85f, 15));
            //billboardList.Add(new Vector3(-27.70972f, -9.85069f, 6));

            //tubeY = -30.21f;
            //TranslateMachine t1, t2;
            //t1 = new TranslateMachine(1, 9, new Vector3(4, 0, 0), 1.75f, false,
            //    new BaseModel(delegate { return modelList[11]["machine1_part1"]; }, false, null, new Vector3(-20.71f, 24.649f, 12.287f)),
            //    new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
            //    new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false),
            //    new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false));
            //tubeY = -18.21f;
            //t2 = new TranslateMachine(1, 9, new Vector3(-4, 0, 0), 1.75f, false,
            //    new BaseModel(delegate { return modelList[11]["machine1_part2"]; }, false, null, new Vector3(-8.71f, 24.649f, 12.287f)),
            //    new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++, 24.649f, 12.2f), true, false),
            //    new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false),
            //    new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false), new Tube(new Vector3(tubeY++ + 12, 24.649f, 12.2f), true, false));
            //OperationalMachine.LinkMachines(t1, t2);

            //machs.Add(t1, true);
            //machs.Add(t2, false);

            //machs.Add(new TranslateMachine(2, 5, new Vector3(0, 0, -6.5f), 1.5f, false,
            //    new BaseModel(delegate { return modelList[11]["machine2_glass"]; }, true, null, new Vector3(8.29f, 24.649f, 14.9f))), true);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.PiOver2), 1.5f));
            //keyframeList.Add(new Keyframe(2.5f));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2), 1.5f));
            //keyframeList.Add(new Keyframe(2.5f));
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[11]["auto_hammer"]; }, false, null, new Vector3(12.29f, 29.713f, 18.251f)) { LocalOffset = -new Vector3(0, 24.649f - 29.713f, 22.5f - 18.251f) }), false);
            //keyframeList.Clear();

            //machs.Add(new TranslateMachine(3, 8, new Vector3(0, 5, 0), 0.75f, false,
            //    new BaseModel(delegate { return modelList[11]["machine3_part1"]; }, false, null, new Vector3(32.09f, 11.149f, 8.9f))), true);
            //machs.Add(new TranslateMachine(3, 8, new Vector3(0, -5, 0), 0.75f, false,
            //    new BaseModel(delegate { return modelList[11]["machine3_part2"]; }, false, null, new Vector3(26.09f, 22.149f, 2.6f))), false);

            //tubeY = 11.29f;
            //machs.Add(new ClampedRotationMachine(4, 6, 3.5f, Vector3.UnitY, MathHelper.PiOver2, new Vector3(27.79f, 3.649f, 1.284f), Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[11]["machine4_base"]; }, false, null, new Vector3(22.679f, 3.649f, 1.284f)),
            //    new BaseModel(delegate { return modelList[11]["machine4_glass"]; }, true, null, new Vector3(15.24f, 4.649f, 3.8f)),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false),
            //    new Tube(new Vector3(tubeY++, 4.649f, 1.2f), true, false), new Tube(new Vector3(27.79f, 6.149f, 1.2f), false, true),
            //    new Tube(new Vector3(27.79f, 5.149f, 1.2f), false, true), new Tube(new Vector3(27.79f, 4.149f, 1.2f), false, true),
            //    new Tube(new Vector3(27.79f, 3.149f, 1.2f), false, true), new Tube(new Vector3(27.79f, 2.149f, 1.2f), false, true)) { DampingMultiplier = 40 }, true);

            //CardinalSpline3D path = new CardinalSpline3D();
            //path.ControlPoints.Add(-1, new Vector3(13.62f, 4.649f, -17.7f));
            //path.ControlPoints.Add(0, new Vector3(13.62f, 4.649f, -9.7f));
            //path.ControlPoints.Add(5, new Vector3(13.62f, 4.649f, 30.3f));
            //path.ControlPoints.Add(10, new Vector3(13.62f, 4.649f, -9.7f));
            //path.ControlPoints.Add(11, new Vector3(13.62f, 4.649f, -17.7f));
            //machs.Add(new StationaryCollisionCannon(new CannonPathFinder(path, 0, 10, false), new Vector3(13.62f, 4.649f, -9.7f),
            //    new BaseModel(delegate { return modelList[11]["machine4_cannon"]; }, false, null, new Vector3(13.89f, 4.749f, -5.338f))), false);

            //BaseModel laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -2.351f, 1.8f));
            //laser.Remover = true;
            //BaseModel laser2 = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -12.351f, 1.8f));
            //laser2.Remover = true;
            //machs.Add(new DisappearenceMachine(5, true, laser, laser2), true);
            //laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -7.351f, 1.8f));

            //laser.Remover = true;
            //machs.Add(new DisappearenceMachine(5, false, laser), false);

            //laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -19.351f, 0.8f));
            //laser.Remover = true;
            //laser2 = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -29.351f, 0.8f));
            //laser2.Remover = true;
            //machs.Add(new DisappearenceMachine(5000, true, laser, laser2), false);

            //laser = new BaseModel(delegate { return modelList[11]["laser"]; }, false, null, new Vector3(27.79f, -24.351f, 0.8f));
            //laser.Remover = true;
            //machs.Add(new DisappearenceMachine(5000, false, laser), false);

            //machs.Add(new HoldRotationMachine(6, 2, Vector3.Zero), true);

            //machs.Add(new TranslateMachine(7, 8, Vector3.UnitZ * 8, 1, false,
            //    new BaseModel(delegate { return modelList[11]["machine7"]; }, false, null, new Vector3(1.29f, -33.351f, -4.633f))), true);

            //BaseModel b = new BaseModel(delegate { return modelList[11]["machine8_part1"]; }, false, null, new Vector3(-17.7f, -33.35f, 1));
            //b.Ent.CollisionInformation.LocalPosition = new Vector3(-2.89114f, -0.005958138f, -0.1427545f);

            //keyframeList.Add(new Keyframe(Vector3.UnitZ * 5, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2), 3f));
            //keyframeList.Add(new Keyframe(5f));
            //k1 = new KeyframeMachine(8, 7, keyframeList, false,
            //    b);
            //keyframeList.Clear();

            //b = new BaseModel(delegate { return modelList[11]["machine8_part2"]; }, false, null, new Vector3(-17.7f, -9.85f, 5.5f));
            //b.Ent.CollisionInformation.LocalPosition = new Vector3(0.000284043f, 0.5598117f, 0.4155622f);

            //keyframeList.Add(new Keyframe(3f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathHelper.PiOver4), 1.25f));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 3.75f));
            //k2 = new KeyframeMachine(8, 7, keyframeList, false,
            //    b);
            //keyframeList.Clear();

            //machs.Add(k1, true);
            //machs.Add(k2, false);

            //machs.Add(new TranslateMachine(9, 9, Vector3.UnitZ * 7, 1f, false,
            //    new BaseModel(delegate { return modelList[11]["machine9"]; }, false, null, new Vector3(-5.71f, -2.85f, 0.366f))), true);

            //tubeY = -21.26369f;

            //machs.Add(new TranslateMachine(10, 7, Vector3.UnitZ * 5, 1f, false,
            //    new BaseModel(delegate { return modelList[11]["machine10"]; }, false, null, new Vector3(-27.70972f, -9.85069f, -4.21666f)),
            //    new BaseModel(delegate { return modelList[11]["machine10_stripes"]; }, false, null, new Vector3(-27.7f, -9.85f, -6.6f)),
            //    new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
            //    new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
            //    new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
            //    new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false), new Tube(new Vector3(-27.70972f, tubeY++, -6.80241f), false, false),
            //    new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false),
            //    new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false),
            //    new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false),
            //    new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false), new Tube(new Vector3(-27.70972f, tubeY++ + 7.913f, -1.8f), false, false)), true);

            //level11 = new Level(11, 1, 1, new Vector3(-44.21f, 24.649f, 18.5f), billboardList, Theme.Space, (BaseModel)delegate { return modelList[11]["base"]; },
            //    new BaseModel[] { new BaseModel(delegate { return modelList[11]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[11]["glass2"]; }, true, false, Vector3.Zero),
            //    new BaseModel(delegate { return modelList[11]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[11]["glass4"]; }, true, false, Vector3.Zero),
            //    new BaseModel(delegate { return modelList[11]["glass6"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[11]["glass7"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(-37.13897f, 8.33103f, -3.90719f - 1.5f)), null,
            //    machs, tubeList, new LevelCompletionData(), "");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level D1
            //Program.Cutter.WriteToLog(this, "Creating level D1");

            //for(int i = 0; i < 19; i++)
            //    tubeList.Add(new Tube(new Vector3(-35.033f + i, 0.395f, 5), true, false));
            //for(int i = 0; i < 33; i++)
            //    tubeList.Add(new Tube(new Vector3(-4.533f + i, 0.595f, -1), true, false));

            //TranslateMachine t3, t4;
            //t1 = new TranslateMachine(1, 10, Vector3.UnitX * 3f, 1.15f, false,
            //    new BaseModel(delegate { return modelList[12]["machine1_plus_x"]; }, false, null, new Vector3(-10.353f, 0.345f, 3)));
            //t2 = new TranslateMachine(1, 10, Vector3.UnitX * -3f, 1.15f, false,
            //     new BaseModel(delegate { return modelList[12]["machine1_minus_x"]; }, false, null, new Vector3(-10.662f, 0.345f, 3)));
            //t3 = new TranslateMachine(1, 10, Vector3.UnitY * 3f, 1.15f, false,
            //    new BaseModel(delegate { return modelList[12]["machine1_plus_y"]; }, false, null, new Vector3(-10.483f, 0.505f, 3)));
            //t4 = new TranslateMachine(1, 10, Vector3.UnitY * -3f, 1.15f, false,
            //    new BaseModel(delegate { return modelList[12]["machine1_minus_y"]; }, false, null, new Vector3(-10.483f, 0.215f, 3)));

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));
            //keyframeList.Add(new Keyframe(Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi), 6));

            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[12]["machine1_rotatebase"]; }, false, null, new Vector3(-10.483f, 0.345f, 1.15f))) { Friction = new Material(1.2f, 1.2f, 0) }, false);
            //keyframeList.Clear();

            //OperationalMachine.LinkMachines(t1, t2, t3, t4);
            //machs.Add(t1, true);
            //machs.Add(t2, false);
            //machs.Add(t3, false);
            //machs.Add(t4, false);

            //machs.Add(new TranslateMachine(2, 7, Vector3.UnitZ * -6.5f, 1.5f, false,
            //    new BaseModel(delegate { return modelList[12]["machine2_glass"]; }, true, null, new Vector3(14f, 0.595f, 1.25f))), true);

            //keyframeList.Clear();
            //keyframeList.Add(new Keyframe(Vector3.UnitY * -2.7f, 1.5f));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            //keyframeList.Add(new Keyframe(Vector3.UnitY * 2.7f, 1.5f));
            //keyframeList.Add(new Keyframe(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi), 1f));
            ////machs.Add(new BeltMachine(0, new Vector3(16.967f, 2.145f - 3, 0.967f - 1.033f), new Vector3(16.967f, 2.145f, 0.967f - 1.033f), Vector3.UnitZ * 1.033f, 2.7f, Vector3.UnitX,
            ////    new BaseModel(delegate { return modelList[12]["machine2_auto"]; }, false, null, new Vector3(16.967f, 2.145f, 0.967f + 1.033f))), false);
            //machs.Add(new KeyframeMachine(0, -1, keyframeList, true,
            //    new BaseModel(delegate { return modelList[12]["machine2_auto"]; }, false, null, new Vector3(16.967f, 2.145f, 0.967f + 1.1f)) { LocalOffset = new Vector3(0, 0, -1.1f) }), false);
            //keyframeList.Clear();

            //billboardList.Add(new Vector3(-10.483f, 0.215f, 8.5f));
            //billboardList.Add(new Vector3(13.697f, 0.595f, 8.5f));

            //levelD1 = new Level(12, 20, 5, new Vector3(-33.033f, 0.395f, 15), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[12]["base"]; },
            //    new[] { new BaseModel(delegate { return modelList[12]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[12]["glass2"]; }, true, false, Vector3.Zero) },
            //    new Goal(new Vector3(31.717f, 0.645f, -5.344f)), null, machs, tubeList,
            //    new LevelCompletionData(new TimeSpan(0, 1, 50), 2200, 1), "Pilot");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level D2
            //Program.Cutter.WriteToLog(this, "Creating level D2");

            //for(int i = 0; i < 20; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-31.608f + i, -32.067f, 21), true, false));
            //    tubeList.Add(new Tube(new Vector3(-25.608f + i, 47.693f, 3), true, false));
            //}
            //for(int i = 0; i < 24; i++)
            //    tubeList.Add(new Tube(new Vector3(-17.608f + i, 4.963f, 3), true, false));
            //for(int i = 0; i < 7; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(6.392f + i, 6.767f, 3), true, false));
            //    tubeList.Add(new Tube(new Vector3(6.392f + i, 2.756f, 3), true, false));
            //}

            //tubeList.Add(new Tube(new Vector3(13.392f, 7.091f, 3), true, false));
            //tubeList.Add(new Tube(new Vector3(14.392f, 7.237f, 3), true, false));
            //tubeList.Add(new Tube(new Vector3(15.392f, 7.518f, 3), true, false));
            //tubeList.Add(new Tube(new Vector3(16.392f, 7.747f, 3), true, false));
            //tubeList.Add(new Tube(new Vector3(13.392f, 2.756f, 3), true, false));
            //tubeList.Add(new Tube(new Vector3(14.392f, 2.583f, 3), true, false));
            //tubeList.Add(new Tube(new Vector3(15.392f, 2.416f, 3), true, false));
            //tubeList.Add(new Tube(new Vector3(16.392f, 2.148f, 3), true, false));

            //for(int i = 0; i < 6; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(17.392f + i, 1.963f, 3), true, false));
            //    tubeList.Add(new Tube(new Vector3(17.392f + i, 7.963f, 3), true, false));
            //}
            //for(int i = 0; i < 13; i++)
            //    tubeList.Add(new Tube(new Vector3(24.982f, 6.463f + i, 3), false, false));
            //for(int i = 0; i < 15; i++)
            //    tubeList.Add(new Tube(new Vector3(25.292f, 3.463f - i, 3), false, true));
            //for(int i = 0; i < 14; i++)
            //    tubeList.Add(new Tube(new Vector3(-24.108f, -13.537f + i, 3), false, false));
            //for(int i = 0; i < 27; i++)
            //    tubeList.Add(new Tube(new Vector3(26.392f - i, 20.963f, 3), true, true));
            //for(int i = 0; i < 12; i++)
            //{
            //    tubeList.Add(new Tube(new Vector3(-2.108f, 19.463f + i, 3), false, false));
            //    tubeList.Add(new Tube(new Vector3(-24.108f, 12.463f + i, 3), false, false));
            //}
            //for(int i = 0; i < 49; i++)
            //    tubeList.Add(new Tube(new Vector3(26.392f - i, -12.437f, 3), true, true));
            //for(int i = 0; i < 10; i++)
            //    tubeList.Add(new Tube(new Vector3(-24.108f, 36.463f + i, 3), false, false));

            //billboardList.Add(new Vector3(-12.108f, -32.037f, 31.818f));
            //billboardList.Add(new Vector3(-7.608f, -54.524f + 60.5f, 19.5f));
            //billboardList.Add(new Vector3(-2.108f, 35.734f, 15));
            //billboardList.Add(new Vector3(-24.108f, 5.963f, 9f));

            //machs.Add(new TranslateMachine(1, 9, Vector3.UnitZ * -6.5f, 1.5f, false,
            //    new BaseModel(delegate { return modelList[13]["machine1_glass"]; }, true, null, new Vector3(-12.108f, -32.037f, 23.818f))), true);

            //machs.Add(new TruckMachine(2, 5, 13.5f, 1.25f, MathHelper.PiOver2, Vector3.UnitY * 55.5f, -Vector3.UnitY, -Vector3.UnitX, Vector3.UnitX,
            //    new Vector3(-3.108f, -50.037f, 11.5f), new Vector3(-12.108f, -50.037f, 11.5f), new Vector3(-7.608f, -46.013f, 11.524f), new Vector3(-7.608f, -54.013f, 11.524f),
            //    new BaseModel(delegate { return modelList[13]["machine2_wheels"]; }, false, null, new Vector3(-7.608f, -46.013f, 11.524f)),
            //    new BaseModel(delegate { return modelList[13]["machine2_wheels"]; }, false, null, new Vector3(-7.608f, -54.013f, 11.524f)),
            //    new BaseModel(delegate { return modelList[13]["machine2_door"]; }, true, null, new Vector3(-5.358f, -50.037f, 11.5f)),
            //    new BaseModel(delegate { return modelList[13]["machine2_door"]; }, true, null, new Vector3(-9.858f, -50.037f, 11.5f)),
            //    new BaseModel(delegate { return modelList[13]["machine2_base"]; }, false, null, new Vector3(-7.608f, -49.524f, 13)),
            //    new BaseModel(delegate { return modelList[13]["machine2_glass"]; }, true, null, new Vector3(-7.608f, -49.537f, 13))), true);

            //machs.Add(new ClampedRotationMachine(0, -1, 1.5f, Vector3.UnitZ, MathHelper.PiOver4, new Vector3(13.392f, 4.963f, 3.5f), Vector3.UnitY,
            //    new BaseModel(delegate { return modelList[13]["auto_switcher"]; }, false, null, new Vector3(12.252f, 5.476f, 3.9f))), false);

            //machs.Add(new ClampedRotationMachine(3, 1, 2.5f, -Vector3.UnitX, MathHelper.ToRadians(179), new Vector3(-2.108f, 40.363f, 0.589f), -Vector3.UnitY,
            //    new BaseModel(delegate { return modelList[13]["machine3_bucket"]; }, false, null, new Vector3(-2.108f, 35.734f, -1.787f)),
            //    new BaseModel(delegate { return modelList[13]["machine3_glass"]; }, true, null, new Vector3(-2.782f, 34.717f, -0.11f))), true);

            //keyframeList.Add(new Keyframe(Vector3.UnitZ * -2.5f, Quaternion.Identity, 0.75f));
            //keyframeList.Add(new Keyframe(Vector3.UnitY * 24f, Quaternion.Identity, 2f));
            //keyframeList.Add(new Keyframe(Vector3.UnitZ * 2.5f, Quaternion.Identity, 0.75f));

            //tubeY = 0.463f;
            //machs.Add(new KeyframeMachine(4, 8, keyframeList, false,
            //    new BaseModel(delegate { return modelList[13]["machine4_base"]; }, false, null, new Vector3(-24.108f, 5.963f, 3.084f)),
            //    new BaseModel(delegate { return modelList[13]["machine4_glass"]; }, true, null, new Vector3(-24.108f, 5.963f, 5.963f)),
            //    new BaseModel(delegate { return modelList[13]["machine4_stripes"]; }, false, null, new Vector3(-24.092f, 5.963f, 3.15f)),
            //    new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false),
            //    new Tube(new Vector3(-24.108f, tubeY++, 3), false, false), new Tube(new Vector3(-24.108f, tubeY++, 3), false, false)), true);

            //levelD2 = new Level(13, 30, 5, new Vector3(-28.608f, -32.037f, 31.5f), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[13]["base"]; },
            //    new[] { new BaseModel(delegate { return modelList[13]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[13]["glass2"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[13]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[13]["glass4"]; }, true, false, Vector3.Zero), 
            //        new BaseModel(delegate { return modelList[13]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[13]["glass6"]; }, true, false, Vector3.Zero) }, new Goal(new Vector3(-2.108f, 47.963f, -5.066f)),
            //        null, machs, tubeList, new LevelCompletionData(new TimeSpan(0, 2, 45), 2100, 2), "Accomplishment");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //#region Level D3
            //Program.Cutter.WriteToLog(this, "Creating level D3");

            //for(int i = 0; i < 14; i++)
            //    tubeList.Add(new Tube(new Vector3(-52.886f + i, -14.187f, -0.5f), true, false));
            //for(int i = 0; i < 19; i++)
            //    tubeList.Add(new Tube(new Vector3(-37.366f, -15.687f + i, -0.5f), false, false));
            //for(int i = 0; i < 3; i++)
            //    tubeList.Add(new Tube(new Vector3(-33.886f + i, -3.187f, -0.5f), true, false));
            //for(int i = 0; i < 7; i++)
            //    tubeList.Add(new Tube(new Vector3(-30.466f + i, 14.813f, 2.5f), true, false));
            //tubeList.Add(new Tube(new Vector3(-3.466f, 14.813f, 2.5f), true, false));
            //for(int i = 0; i < 14; i++)
            //    tubeList.Add(new Tube(new Vector3(-0.466f - i, 4.813f, 0.4f), true, true));
            //for(int i = 0; i < 11; i++)
            //    tubeList.Add(new Tube(new Vector3(-15.966f, 6.313f - i, 0.4f), false, true));
            //for(int i = 0; i < 5; i++)
            //    tubeList.Add(new Tube(new Vector3(-15.966f, -8.687f - i, 0.4f), false, true));
            //for(int i = 0; i < 23; i++)
            //    tubeList.Add(new Tube(new Vector3(-17.466f + i, -15.187f, 0.4f), true, false));
            //for(int i = 0; i < 17; i++)
            //    tubeList.Add(new Tube(new Vector3(7.034f, -16.687f + i, 0.4f), false, false));
            //for(int i = 0; i < 12; i++)
            //    tubeList.Add(new Tube(new Vector3(10.534f + i, -7.187f, -1), true, false));
            //for(int i = 0; i < 22; i++)
            //    tubeList.Add(new Tube(new Vector3(24.034f, -8.867f + i, -1), false, false));

            //machs.Add(new TranslateMachine(1, 9, Vector3.UnitX * 5, 1.25f, false,
            //    new BaseModel(delegate { return modelList[14]["machine1"]; }, false, null, new Vector3(-41.79f, -3.187f, -.12f)),
            //    new BaseModel(delegate { return modelList[14]["machine1_box"]; }, false, null, new Vector3(-43.355f, -3.215f, 0)) { IsInvisible = true }), true);

            //machs.Add(new TranslateMachine(2, 7, new Vector3(0, 20.732f, 12.28f), 5.75f, false,
            //    new BaseModel(delegate { return modelList[14]["machine2"]; }, false, null, new Vector3(-28.866f, -9.904f, -6.351f))), true);

            //tubeY = -23.466f;
            //t1 = new TranslateMachine(3, 5, Vector3.UnitX * 5, 1, false,
            //    new BaseModel(delegate { return modelList[14]["machine3"]; }, false, null, new Vector3(-21.466f, 14.813f, 2.584f)),
            //    new BaseModel(delegate { return modelList[14]["machine3_glass1"]; }, true, null, new Vector3(-21.45f, 14.813f, 5.7f)),
            //    new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false),
            //    new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false),
            //    new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false));

            //tubeY = -8.466f;
            //t2 = new TranslateMachine(3, 5, Vector3.UnitX * -5, 1, false,
            //    new BaseModel(delegate { return modelList[14]["machine3"]; }, false, null, new Vector3(-6.466f, 14.813f, 2.584f)),
            //    new BaseModel(delegate { return modelList[14]["machine3_glass2"]; }, true, null, new Vector3(-6.45f, 14.813f, 5.7f)),
            //    new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false),
            //    new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false), new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false),
            //    new Tube(new Vector3(tubeY++, 14.813f, 2.5f), true, false));

            //OperationalMachine.LinkMachines(t1, t2);
            //machs.Add(t1, true);
            //machs.Add(t2, false);

            //machs.Add(new TranslateMachine(4, 0, Vector3.UnitY * -9, 2.75f, false,
            //    new BaseModel(delegate { return modelList[14]["machine4"]; }, false, null, new Vector3(-1.5f, 17.313f, 3))), true);

            //machs.Add(new TranslateMachine(5, 1, Vector3.UnitZ * 2.75f, 0.75f, false,
            //    new BaseModel(delegate { return modelList[14]["machine5"]; }, false, null, new Vector3(-16, -6.187f, -0.125f)),
            //    new BaseModel(delegate { return modelList[14]["machine5_glass"]; }, true, null, new Vector3(-16, -6.187f, 3.6f))), true);

            //machs.Add(new ClampedRotationMachine(6, 4, 1.25f, -Vector3.UnitY, MathHelper.PiOver2, new Vector3(7, -7.187f, 10.5f), Vector3.UnitX,
            //    new BaseModel(delegate { return modelList[14]["machine6"]; }, false, null, new Vector3(2.324f, -7.187f, 5.824f))), true);

            //billboardList.Add(new Vector3(-41.79f, -3.187f, 4.5f));
            //billboardList.Add(new Vector3(-28.866f, -9.904f, 0));
            //billboardList.Add(new Vector3(-15.466f, 14.813f, 7));
            //billboardList.Add(new Vector3(-1.5f, 17.313f, 7.5f));
            //billboardList.Add(new Vector3(-16, -6.187f, 7f));
            //billboardList.Add(new Vector3(7, -7.187f, 13.5f));

            //levelD3 = new Level(14, 50, 5, new Vector3(-50.866f, -14.187f, 7.5f), billboardList, Theme.Generic, (BaseModel)delegate { return modelList[14]["base"]; },
            //    new[] { new BaseModel(delegate { return modelList[14]["glass1"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[14]["glass2"]; }, true, false, Vector3.Zero),
            //        new BaseModel(delegate { return modelList[14]["glass3"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[14]["glass4"]; }, true, false, Vector3.Zero),
            //        new BaseModel(delegate { return modelList[14]["glass5"]; }, true, false, Vector3.Zero), new BaseModel(delegate { return modelList[14]["glass6"]; }, true, false, Vector3.Zero),
            //        new BaseModel(delegate { return modelList[14]["glass7"]; }, true, false, Vector3.Zero) },
            //        new Goal(new Vector3(24.395f, 15.153f, -5.75f)), null, machs, tubeList,
            //        new LevelCompletionData(new TimeSpan(0, 3, 15), 2000, 3), "Legend");

            //tubeList.Clear();
            //machs.Clear();
            //billboardList.Clear();
            //keyframeList.Clear();
            
            //#endregion

            //levelArray = new Level[15];
            //levelArray[0] = level00;
            //levelArray[1] = level01;
            //levelArray[2] = level02;
            //levelArray[3] = level03;
            //levelArray[4] = level04;
            //levelArray[5] = level05;
            //levelArray[6] = level06;
            //levelArray[7] = level07;
            //levelArray[8] = level08;
            //levelArray[9] = level09;
            //levelArray[10] = level10;
            //levelArray[11] = level11;
            //levelArray[12] = levelD1;
            //levelArray[13] = levelD2;
            //levelArray[14] = levelD3;
            

            
            
            #endregion

            GameManager.ReInitialize(levelArray, Font);
        }
    }
}
