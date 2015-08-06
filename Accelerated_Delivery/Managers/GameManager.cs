using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Accelerated_Delivery_Win
{
    public static class GameManager
    {
        #region Current Level
        public static Level CurrentLevel { get; private set; }
        private static int internalCurrentLevel = -1;
        /// <summary>
        /// This is the current level. When you set this, it updates CurrentLevel and the state.
        /// If you set it to an invalid value, it defaults to the menu.
        /// </summary>
        public static int LevelNumber
        {
            get { return internalCurrentLevel; }
            set
            {
                internalCurrentLevel = value;
                if(value < 0 || value > 14)
                {
                    CurrentLevel = null;
                    dock.Reset();
                    State = GameState.MainMenu;
                }
                else
                {
                    CurrentLevel = levelArray[value];
                    CurrentLevel.AddToGame(Space);
                    CurrentLevel.ResetLevel();
                    State = GameState.Running;
                }
            }
        }
        #endregion

        public static Space Space { get; private set; }

        #region Game State
        /// <summary>
        /// To keep track of the game state. 
        /// </summary>
        public static GameState State
        {
            get { return state; }
            set
            {
                if(value != state) // if we're setting the state to something it already is, don't update previous state.
                    PreviousState = state;
                state = value;
            }
        }

        private static GameState state = GameState.MainMenu;

        public static GameState PreviousState { get; private set; }

        private static GameState enteredFrom;
        /// <summary>
        /// Specify this when a level is loaded.
        /// Valid values are:
        /// Running - Goes to the next level when this one is over.
        /// Level Select - Returns to the level select menu when this one is over.
        /// </summary>
        public static GameState LevelEnteredFrom
        {
            get { return enteredFrom; }
            set
            {
                if(value != GameState.Menuing_Lev && value != GameState.Running && value != GameState.MainMenu)
                    throw new InvalidOperationException("LevelEnteredFrom was set to an invalid state.");
                enteredFrom = value;
            }
        }
        #endregion

        private static Level[] levelArray;

        public static IInputManager Manager { get; private set; }
        public static Game Game { get; private set; }
        public static BoxCutter Cutter { get; private set; }

        public static void FirstStageInitialization(Game g, BoxCutter c)
        {
            Game = g;
            Cutter = c;
            Space = new Space();
            Space.ForceUpdater.Gravity = new Vector3(0, 0, -9.81f);
        }

        public static void Initialize(Level[] levels, SpriteFont f, Dock d, IInputManager man)
        {
            Manager = man;
            levelArray = levels;
            PreviousState = GameState.MainMenu;
            State = GameState.MainMenu;
            font = f;
            dock = d;
        }

        private static SpriteFont font;
        private static Dock dock;

        public static void DrawLevel(GameTime gameTime)
        {
            RenderingDevice.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RenderingDevice.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            RenderingDevice.GraphicsDevice.BlendState = BlendState.Opaque;

            if(CurrentLevel != null)
                CurrentLevel.Draw(gameTime);

#if DEBUG
            if(RenderingDevice.Camera.debugCamera)
            {
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                RenderingDevice.SpriteBatch.DrawString(font, "Debug Camera: On", new Vector2(RenderingDevice.GraphicsDevice.Viewport.TitleSafeArea.Width * 0.7f, RenderingDevice.GraphicsDevice.Viewport.TitleSafeArea.Height * 0.87f), Color.GhostWhite);
                RenderingDevice.SpriteBatch.End();
            }
            if(Input.KeyboardState.IsKeyDown(Keys.Q))
            {
                RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                RenderingDevice.SpriteBatch.DrawString(font, "Double speed", new Vector2(RenderingDevice.GraphicsDevice.Viewport.TitleSafeArea.Width * 0.7f, RenderingDevice.GraphicsDevice.Viewport.TitleSafeArea.Height * 0.9f), Color.GhostWhite);
                RenderingDevice.SpriteBatch.End();
            }
#endif
        }

        public static void ReInitialize(Level[] levels, SpriteFont f)
        {
            levelArray = levels;
            font = f;

            if(internalCurrentLevel >= 0)
                CurrentLevel = levelArray[internalCurrentLevel];
        }
    }
}
