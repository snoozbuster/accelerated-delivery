using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using BEPUphysics;
using BEPUphysics.Entities;
using Microsoft.Win32;
using Accelerated_Delivery_Win;

#if INDIECITY
using ICEBridgeLib;
using ICECoreLib;
#endif

namespace Accelerated_Delivery_Win
{
    public class BaseGame : Game
    {
        #region Graphics
            /// <summary>
            /// Gets the primary GraphicsDeviceManager.
            /// </summary>
            public GraphicsDeviceManager GDM { get; private set; }
            
            /// <summary>
            /// Gets the primary SpriteBatch.
            /// </summary>
            //public SpriteBatch SpriteBatch { get; private set; }

//#if WINDOWS
//            /// <summary>
//            /// A short form of graphics.GraphicsDevice.Viewport.Height.
//            /// </summary>
//            public float Height { get { return GraphicsDevice.Viewport.Height; } }
//            /// <summary>
//            /// A short form of graphics.GraphicsDevice.Viewport.Width.
//            /// </summary>
//            public float Width { get { return GraphicsDevice.Viewport.Width; } }
//#elif XBOX
//            /// <summary>
//            /// A short form of graphics.GraphicsDevice.Viewport.Height.
//            /// </summary>
//            public float Height { get { return GraphicsDevice.Viewport.Height * 0.9f; } }
//            /// <summary>
//            /// A short form of graphics.GraphicsDevice.Viewport.Width.
//            /// </summary>
//            public float Width { get { return GraphicsDevice.Viewport.Width * 0.9f; } }

//#endif

//            public int ScreenWidth { get { return GraphicsDevice.DisplayMode.Width; } }
//            public int ScreenHeight { get { return GraphicsDevice.DisplayMode.Height; } }

              public const int PreferredScreenHeight = 720;
              public const int PreferredScreenWidth = 1280;

//            public float RawHeight { get { return GraphicsDevice.Viewport.Height; } }
//            public float RawWidth { get { return GraphicsDevice.Viewport.Width; } }

//            public Vector2 TextureScaleFactor { get { return new Vector2(RawWidth / PreferredScreenWidth, RawHeight / PreferredScreenHeight); } }

//            /// <summary>
//            /// Gets the screen's aspect ratio. Same as graphics.GraphicsDevice.Viewport.AspectRatio.
//            /// </summary>
//            public float AspectRatio { get { return GraphicsDevice.Viewport.AspectRatio; } }

//            /// <summary>
//            /// Gets the primary camera.
//            /// </summary>
//            public MyCamera Camera { get { return RenderingDevice.Camera; } }

            private bool renderDock = false;
            private Dock Dock;

//#if DEBUG
//            public ModelDrawer drawer { get; private set; }
//#endif
        #endregion

        #region I/O
        public IOManager Manager { get; private set; }
        #endregion

        #region Loading
        public LoadingScreen LoadingScreen { get; private set; }
        public Loader Loader { get; private set; }
        public bool Loading { get; private set; }
            
        private Texture2D backgroundTex;
        private int alpha = 0;
        private int subFactor = 255;

        private bool readyToLoad = false;
        private bool beenDrawn = false;
        #endregion

        protected bool successfulRender;
        protected bool locked = false;
        protected GameState stateLastFrame;

#if INDIECITY
        #region IndieCity
        /// <summary>
        /// Bridge for IC.
        /// </summary>
        public CoBridge2 Bridge { get; private set; }
        /// <summary>
        /// Service ID for IC.
        /// </summary>
        public ServiceId ServiceID { get { return serviceID; } }
        private ServiceId serviceID;
        /// <summary>
        /// User Store for IC.
        /// </summary>
        public CoUserStore UserStore { get; private set; }
        /// <summary>
        /// User Info for IC.
        /// </summary>
        public CoUserInfo UserInfo { get; private set; }
        /// <summary>
        /// Session for IC.
        /// </summary>
        public CoGameSession Session { get; private set; }
        /// <summary>
        /// User ID for current user for IC.
        /// </summary>
        public int UserID { get; private set; }
        private bool request;
        private bool error;
        #endregion
#endif

        public BaseGame()
        {
            IsMouseVisible = true;
#if XBOX
            Components.Add(new GamerServicesComponent(this));
#endif
            Content = new ContentManager(Services);
            GDM = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
#if !INTERNAL && !INDIECITY
            if(!Program.Apprehensive)
            {
                ShowMissingRequirementMessage(new Exception("An invalid operation was performed."));
                Exit();
            }

            try
            {
#if !DEBUG
                var k = Registry.CurrentUser.OpenSubKey("inf_data_AD");
                if((int)k.GetValue("potato", 0) != 1)
                    throw new Exception();
#endif
            }
            catch
            {
                ShowMissingRequirementMessage(new Exception("An invalid operation was performed."));
                Registry.CurrentUser.DeleteSubKeyTree("inf_data_AD", false);
                Exit();
            }
            Registry.CurrentUser.DeleteSubKeyTree("inf_data_AD", false);
#elif INDIECITY
            //create bridge
            Bridge = new CoBridge2();

            //Set your game ids here as given from your game project page
            string myGameId = "50ff0fa6-7628-419f-bfc7-50ead82077bc";
            serviceID = new ServiceId();
            serviceID.Data1 = 0x7710ab33;
            serviceID.Data2 = 0xf822;
            serviceID.Data3 = 0x4297;
            serviceID.Data4 = new byte[] { 0xb9, 0x74, 0x05, 0x64, 0x4f, 0x2a, 0x1b, 0xd8 };
            string myICELibSecret = "bad7922a-0d9e-42c8-8445-32f99e90d323";

            //Initialise the bridge with the gameId and secret.
            //This allows the bridge to access users tokens registered on the computer 
            Bridge.Initialise(myGameId);
            Bridge.SetServiceCredentials(GameService.GameService_IndieCityLeaderboardsAndAchievements, serviceID, myICELibSecret);

            UserID = Bridge.DefaultUserId;
            UserStore = Bridge.UserStore;
            UserInfo = UserStore.GetUserFromId(UserID);
            //Program.Cutter.WriteToLog(this, "here");
            string userName = UserInfo.Name;
            //Program.Cutter.WriteToLog(this, "username is: " + userName);

            //RegistryKey k1 = null, k2 = null;
            //using(k1 = Registry.CurrentUser.OpenSubKey("Software", true))
            //    using(k2 = k1.CreateSubKey("Accelerated Delivery"))
            //    {
            //        Program.Cutter.WriteToLog(this, "username is: " + userName);
            //        k2.SetValue("username", userName);
            //    }

            //create a game session for the user playing the game              
            Session = Bridge.CreateDefaultGameSession();

            //Start the session 
            Session.RequestStartSession();
            
            //block until session has started
            //This is just for simplicity. Real game would react to session start and end events. Session 
            //may end for different reasons or may never start.

            bool started = false;
            int sleptFor = 0;
            do 
            {
                Session.UpdateSession();
                started = Session.IsSessionStarted();
                System.Threading.Thread.Sleep(100);
                sleptFor += 100;
                if(sleptFor > 20000)
                {
                    error = true;
                    SuppressDraw();
                    new Error().Show();
                    return;
                }
            } while(!started);
#endif

            GDM.PreferredBackBufferWidth = PreferredScreenWidth;
            GDM.PreferredBackBufferHeight = PreferredScreenHeight;

            // supported resolutions are 800x600, 1280x720, 1366x768, 1980x1020, and 1024x768
            int width = GraphicsDevice.DisplayMode.Width;
            int height = GraphicsDevice.DisplayMode.Height;
            if(width > 800) // this setup doesn't account for aspect ratios, which may be a problem
            {
                if(width > 1024)
                {
                    if(width > 1280)
                    {
                        if(width > 1366)
                        {
                            GDM.PreferredBackBufferWidth = 1366;
                            GDM.PreferredBackBufferHeight = 768;
                        }
                        else
                        {
                            GDM.PreferredBackBufferWidth = 1280;
                            GDM.PreferredBackBufferHeight = 720;
                        }
                    }
                    else
                    {
                        GDM.PreferredBackBufferWidth = 1024;
                        GDM.PreferredBackBufferHeight = 768;
                    }
                }
            }
            else if(width < 800)
            {
                ShowMissingRequirementMessage(new Exception("The game requires at least an 800x600 screen resolution."));
                Exit();
            }
            else
            {
                GDM.PreferredBackBufferWidth = 800;
                GDM.PreferredBackBufferHeight = 600;
            }

            GameManager.FirstStageInitialization(this, Program.Cutter);

            AccomplishmentManager.InitAchievements();

            try { Manager = new IOManager(); }
            catch { }

            if(Manager != null)
                if(Manager.SuccessfulLoad)
                {
                    GDM.IsFullScreen = Manager.FullScreen;
                    GDM.PreferredBackBufferHeight = Manager.CurrentSaveWindowsOptions.ScreenHeight;
                    GDM.PreferredBackBufferWidth = Manager.CurrentSaveWindowsOptions.ScreenWidth;
                    MenuHandler.SaveLoaded();
                }

            Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics = false;
            if(GDM.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.HiDef))
            {
                GDM.GraphicsProfile = GraphicsProfile.HiDef;
                Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics = true;
            }

            GDM.ApplyChanges();

//#if DEBUG
//            if(HiDef)
//                drawer = new InstancedModelDrawer(this);
//            else
//                drawer = new BruteModelDrawer(this);    
//#endif

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
#if INDIECITY
            if(error)
            {
                SuppressDraw();
                return;
            }

            Session.UpdateSession();
            if(!Session.IsSessionStarted() && request)
            {
                request = false;
                Session.RequestStartSession();
            }
            else if(Session.IsSessionStarted() && !request)
                request = true;
#endif
            if(LoadingScreen == null && Loader == null) // do once and only once (or when we need to reload)
            {
                Box.Initialize(this);
                RenderingDevice.Initialize(GDM, Program.Cutter, GameManager.Space, Content.Load<Effect>("Shaders/shadowmap"), Content.Load<Texture2D>("textures/lightMap"));

                LoadingScreen = new LoadingScreen(Content, GraphicsDevice);

                backgroundTex = Content.Load<Texture2D>("2D/Splashes and Overlays/Logo");

                Extensions.Initialize(GraphicsDevice);
                Resources.Initialize(Content);
#if WINDOWS
                Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
                GDM.DeviceCreated += onGDMCreation;
#endif
            }

            if(stateLastFrame == GameState.Running && GameManager.State == GameState.Paused)
                MediaSystem.PlaySoundEffect(SFXOptions.Pause);

            stateLastFrame = GameManager.State;

            if((!IsActive && Loader != null) || locked)
            {
                base.Update(gameTime);
                return;
            }

            Input.Update(gameTime, MenuHandler.IsSelectingSave);
            MediaSystem.Update(gameTime, Program.Game.IsActive);

            if(Manager.SaveLoaded)
                AccomplishmentManager.Update(gameTime);

            if(LoadingScreen != null)
            {
                if(alpha + 3 >= 255)
                {
                    alpha = 255;
                    readyToLoad = true;
                    if(subFactor - 3 < 0)
                        subFactor = 0;
                    else
                        subFactor -= 3;
                }
                else
                    alpha += 3;

                IsMouseVisible = false;

                if(readyToLoad)
                {
                    Loading = true;
                    IsFixedTimeStep = false;

                    Loader = LoadingScreen.Update(gameTime);
                    if(Loader != null)
                    {
                        // Content loaded.  Use loader members to get at the loaded content.
                        LoadingScreen = null;
                        IsFixedTimeStep = true; // Back to the default -- change if you want.
                        Loading = false;
                        Dock = new Dock(delegate { return Loader.Dock; }, delegate { return Loader.Font; });
                        MenuHandler.Create(Loader);
                        GameManager.Initialize(Loader.levelArray, Loader.Font, Dock, Manager);
#if INDIECITY
                        AccomplishmentManager.Ready(Session, UserInfo.GetId());
#else
                        AccomplishmentManager.Ready();
#endif
                        MediaSystem.PlayVoiceActing(12);
                        BlackBox.Initialize(Content.Load<Effect>("Shaders/bbeffect"), Loader.blackBoxBillboardList);
                    }
                }
            }
            else
            {
                //if(State != GameState.Ending)
                //{
                //    MediaSystem.PlayTrack(SongOptions.Credits);
                //    State = GameState.Ending;
                //}

                if(GameManager.State == GameState.MainMenu && Loading)
                    this.IsMouseVisible = false;
                else
                    this.IsMouseVisible = true;

                GameState statePrior = GameManager.State;
                MenuHandler.Update(gameTime);
                bool stateChanged = GameManager.State != statePrior; 

                #region Running
                if(GameManager.State == GameState.Running)
                {
                    if(((Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.PauseKey) ||
                        Input.CheckXboxJustPressed(Manager.CurrentSaveXboxOptions.PauseKey)) && !GameManager.CurrentLevel.ShowingOverlay &&
                        !GameManager.CurrentLevel.Ending) && !stateChanged)
                    {
                        //MediaSystem.PlaySoundEffect(SFXOptions.Pause);
                        GameManager.State = GameState.Paused;
                    }
#if DEBUG
                    else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.HelpKey) ||
                        Input.CheckXboxJustPressed(Manager.CurrentSaveXboxOptions.HelpKey) && GameManager.CurrentLevel.spawnlevel == 0)
#else
                    else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.HelpKey) ||
                        Input.CheckXboxJustPressed(Manager.CurrentSaveXboxOptions.HelpKey))
#endif
                    {
                        renderDock = !renderDock;
                        if(renderDock)
                            Dock.Open();
                        else
                            Dock.Close();
                    }
                    else
                    {
                        Manager.CurrentSave.Playtime += gameTime.ElapsedGameTime;
                        GameManager.Space.Update((float)(gameTime.ElapsedGameTime.TotalSeconds));
#if DEBUG
                        if(Input.KeyboardState.IsKeyDown(Keys.Q))
                            GameManager.Space.Update((float)(gameTime.ElapsedGameTime.TotalSeconds));
#endif
                        RenderingDevice.Update(gameTime);
                        if(GameManager.CurrentLevel != null)
                            GameManager.CurrentLevel.Update(gameTime);
                    }
                }
                #endregion

                if(GameManager.State == GameState.Results)
                {
                    GameManager.Space.Update((float)(gameTime.ElapsedGameTime.TotalSeconds));
                    GameManager.CurrentLevel.Update(gameTime);
                    RenderingDevice.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if(!beenDrawn)
            {
                MediaSystem.Ready(Content);
                beenDrawn = true;
            }

            if(Loading || !readyToLoad)
            {
                GraphicsDevice.Clear(Color.Black);
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                RenderingDevice.SpriteBatch.Draw(backgroundTex, new Vector2(RenderingDevice.Width * 0.5f, RenderingDevice.Height * 0.5f), null, new Color(255, 255, 255) * (alpha / 255f), 0, new Vector2(backgroundTex.Width * 0.5f, backgroundTex.Height * 0.5f), RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
#if !INDIECITY
                string s = "Accelerated Delivery uses an autosave function. When you see a spinning box, your progress is being saved.";
                SpriteFont f = Content.Load<SpriteFont>("Font/Ad-Font");
                Vector2 length = f.MeasureString(s);
                RenderingDevice.SpriteBatch.DrawString(f, s, new Vector2(GraphicsDevice.Viewport.Width * 0.5f, GraphicsDevice.Viewport.Height * 0.9f), new Color(255, 255, 255) * ((alpha - subFactor) / 255f), 0, length * 0.5f, new Vector2(GraphicsDevice.Viewport.Width / 1280, GraphicsDevice.Viewport.Height / 720), SpriteEffects.None, 0);
#endif
                RenderingDevice.SpriteBatch.End();

                if(readyToLoad)
                    LoadingScreen.Draw();
                return;
            }

            GraphicsDevice.Clear(Color.Black);

            if(GameManager.State == GameState.Running || GameManager.State == GameState.Results)
            {
                GameManager.DrawLevel(gameTime);
                if(GameManager.State != GameState.Results && GameManager.LevelNumber != 0)
                    Dock.Draw();
            }

            MenuHandler.Draw(gameTime);
            AccomplishmentManager.Draw();

            if(Manager.IsSaving)
                RenderingDevice.DrawSpinningBox();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Forces the tab-helper to be drawn.
        /// </summary>
        public void DrawDock()
        {
            Dock.Draw();
        }

        #region Miscellaneous
        public void RestartLoad()
        {
            LoadingScreen = new LoadingScreen(Content, GraphicsDevice);

            backgroundTex = Content.Load<Texture2D>("2D/Splashes and Overlays/Logo");

            Extensions.Initialize(GraphicsDevice);
            Resources.Initialize(Content);
        }

        protected void onGDMCreation(object sender, EventArgs e)
        {
            Loader.ReloadALLtheThings();
            Goal.GDMReset();
            Theme.OnGDMReset();
            Level.OnGDMReset();
            RenderingDevice.OnGDMCreation(Content.Load<Effect>("Shaders/shadowmap"), Content.Load<Texture2D>("textures/lightMap"));
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            try 
            { 
                Manager.Save(false);
            }
            // Ignore errors, it's too late to do anything about them.
            catch { }

#if INDIECITY
            //Make call to end the session.
            Session.EndSession();

            do
            {
                Session.UpdateSession();
                System.Threading.Thread.Sleep(100);
            } while(Session.IsSessionStarted());
#endif

            base.OnExiting(sender, args);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
#if INDIECITY
            if(error)
                return;
#endif

            if(GameManager.PreviousState == GameState.Running && Manager.CurrentSaveWindowsOptions.ResumeOnFocus)
                GameManager.State = GameState.Running;
            if(GameManager.State == GameState.Running)
                MediaSystem.PlayAll();
            else
                MediaSystem.ResumeBGM();
            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            if(GameManager.State == GameState.Running)
                if(GameManager.CurrentLevel == null)
                    GameManager.State = GameState.Paused;
                else if(!GameManager.CurrentLevel.ShowingOverlay)
                    GameManager.State = GameState.Paused;

            MediaSystem.PauseAll();
            base.OnDeactivated(sender, args);
        }

#if WINDOWS
        protected void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if(e.Reason == SessionSwitchReason.SessionLock)
            {
                OnDeactivated(sender, e);
                locked = true;
            }
            else if(e.Reason == SessionSwitchReason.SessionUnlock)
            {
                OnActivated(sender, e);
                locked = false;
            }
        }
#endif
        #endregion
    }
}
