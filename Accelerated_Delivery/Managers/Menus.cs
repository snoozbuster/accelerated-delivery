using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    public static class MenuHandler
    {
        /// <summary>
        /// This is for if you're navigating with the mouse and ditch it for the keyboard. 
        /// Upon pressing a keyboard button, when appropriate, this will toggle and the
        /// game will ignore mouse input until the mouse state changes.
        /// </summary>
        public static bool MouseTempDisabled { get; private set; }

        public static string ErrorString { set { mediaMenu.ErrorString = value; } }

        private static PauseMenu pauseMenu;
        private static MainMenu mainMenu;
        private static GamePadDCMenu disconnectMenu;
        private static GameOverMenu gameOverMenu;
        private static OptionsMenu optionsMenu;
        private static LevelSelectMenu levelSelectMenu;
        private static ExitMenu exitMenu;
        private static GamePadQueryMenu queryMenu;
        private static MediaMenu mediaMenu;
        private static EndingMenu endingMenu;
        private static AchievementMenu achMenu;

        public static bool IsSelectingSave { get { return mainMenu.SaveSelecting; } } 

        private static Loader loader;

        public static void Create(Loader l)
        {
            loader = l;
            pauseMenu = new PauseMenu();
            mainMenu = new MainMenu();
            gameOverMenu = new GameOverMenu();
            levelSelectMenu = new LevelSelectMenu();
            disconnectMenu = new GamePadDCMenu();
            optionsMenu = new OptionsMenu();
            exitMenu = new ExitMenu();
            queryMenu = new GamePadQueryMenu();
            mediaMenu = new MediaMenu();
            endingMenu = new EndingMenu();
            achMenu = new AchievementMenu();

            levelSelectMenu.UpdateLockedLevels();
        }

        public static void Draw(GameTime gameTime)
        {
            switch(Program.Game.State)
            {
                case BaseGame.GameState.GameOver: gameOverMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.MainMenu: mainMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Menuing_Lev: levelSelectMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Menuing_Opt: optionsMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Paused: pauseMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Ending: endingMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Paused_DC:
                    switch(Program.Game.PreviousState)
                    {
                        case BaseGame.GameState.GameOver: gameOverMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.MainMenu: mainMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Menuing_Lev: levelSelectMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Menuing_Opt: optionsMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Paused: pauseMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Exiting: exitMenu.Draw(gameTime);
                            break;
                    }
                    disconnectMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Exiting: exitMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Paused_PadQuery:
                    switch(Program.Game.PreviousState)
                    {
                        case BaseGame.GameState.GameOver: gameOverMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.MainMenu: mainMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Menuing_Lev: levelSelectMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Menuing_Opt: optionsMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Paused: pauseMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Exiting: exitMenu.Draw(gameTime);
                            break;
                    }
                    queryMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Paused_SelectingMedia: 
                    switch(Program.Game.PreviousState)
                    {
                        case BaseGame.GameState.GameOver: gameOverMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.MainMenu: mainMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Menuing_Lev: levelSelectMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Menuing_Opt: optionsMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Paused: pauseMenu.Draw(gameTime);
                            break;
                        case BaseGame.GameState.Exiting: exitMenu.Draw(gameTime);
                            break;
                    }
                    mediaMenu.Draw(gameTime);
                    break;
                case BaseGame.GameState.Menuing_Obj:
                    achMenu.Draw(gameTime);
                    break;
                default: return;
            }
        }

        public static void Update(GameTime gameTime)
        {
            CheckForMouseMove();

            switch(Program.Game.State)
            {
                case BaseGame.GameState.Exiting:
                    exitMenu.Update(gameTime);
                    break;
                case BaseGame.GameState.GameOver:
                    gameOverMenu.Update(gameTime);
                    break;
                case BaseGame.GameState.MainMenu:
                    mainMenu.Update(gameTime);
                    break;
                case BaseGame.GameState.Menuing_Lev:
                    levelSelectMenu.Update(gameTime);
                    break;
                case BaseGame.GameState.Menuing_Opt:
                    optionsMenu.Update(gameTime);
                    return;
                case BaseGame.GameState.Paused:
                    pauseMenu.Update(gameTime);
                    break;
                case BaseGame.GameState.Paused_DC:
                    disconnectMenu.Update(gameTime);
                    break;
                case BaseGame.GameState.Paused_PadQuery:
                    queryMenu.Update(gameTime);
                    break;
                case BaseGame.GameState.Paused_SelectingMedia:
                    mediaMenu.Update(gameTime);
                    return;
                case BaseGame.GameState.Menuing_Obj:
                    achMenu.Update(gameTime);
                    return;
                case BaseGame.GameState.Ending: endingMenu.Update(gameTime);
                    return;
            }

            if((Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.MusicKey) || Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MusicKey)) &&
               (Program.Game.State != BaseGame.GameState.Paused_DC && Program.Game.State != BaseGame.GameState.Paused_PadQuery && Program.Game.State != BaseGame.GameState.Exiting &&
               Program.Game.State != BaseGame.GameState.Ending && Program.Game.State != BaseGame.GameState.Paused_SelectingMedia && !MenuHandler.IsSelectingSave))
            {
                Program.Game.State = BaseGame.GameState.Paused_SelectingMedia;
                MediaSystem.PauseAuxilary();
            }
        }

        #region Helper methods
        /// <summary>
        /// If the mouse has moved, it becomes enabled.
        /// </summary>
        public static void CheckForMouseMove()
        {
            if(Input.MouseState != Input.MouseLastFrame)
                MouseTempDisabled = false;
        }

        internal static void SaveLoaded()
        {
        }
        #endregion

        #region Menu base
        private abstract class Menu
        {
            /// <summary>
            /// A dictionary of all the controls. The key is the control, and the value is false if not selected, null if selected but
            /// no buttons are down, and true if there's a button down and it's selected.
            /// </summary>
            protected List<MenuControl> controlArray;

            /// <summary>
            /// The currently selected control.
            /// </summary>
            protected MenuControl selectedControl;

            protected bool enterLetGo = false;
            protected bool holdingSelection = false;

            private bool playedClickSound = false;

            /// <summary>
            /// You NEED to set selectedControl in your constructor. Absolutely need. Just set it to
            /// controlArray.ElementAt(0).Key. That's all you have to remember to do. If you don't,
            /// things WILL crash. Boom. Also, set controlArray.IsSelected to null.
            /// </summary>
            protected Menu()
            {
                controlArray = new List<MenuControl>();
            }

            /// <summary>
            /// Calls DetectKeyboardInput and DetectMouseInput and if either is true invokes the selected control.
            /// </summary>
            /// <param name="gameTime">Snapshot of timing values.</param>
            public virtual void Update(GameTime gameTime)
            {
                if(detectKeyboardInput() || detectMouseInput())
                    selectedControl.OnSelect();
            }

            /// <summary>
            /// Draws each of the controls in controlArray.
            /// </summary>
            public virtual void Draw(GameTime gameTime)
            {
                foreach(MenuControl m in controlArray)
                    m.Draw(selectedControl);
            }

            /// <summary>
            /// True denotes do something. False denotes don't.
            /// </summary>
            /// <returns></returns>
            protected virtual bool detectKeyboardInput()
            {
                if((Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey) ||
                    Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.MenuLeftKey)) && selectedControl.OnLeft != null)
                {
                    MouseTempDisabled = true;
                    selectedControl.IsSelected = false;

                    MenuControl initial = selectedControl;
                    do
                    {
                        selectedControl = selectedControl.OnLeft;
                        if(selectedControl == null)
                        {
                            selectedControl = initial;
                            break;
                        }
                    } while(selectedControl.IsDisabled);
                    //} while(loader.levelArray[controlArray.IndexOf(selectedControl)] == null);

                    if(initial != selectedControl)
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);

                    selectedControl.IsSelected = null;
                    return false;
                }
                else if((Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey) ||
                         Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.MenuRightKey)) && selectedControl.OnRight != null)
                {
                    MouseTempDisabled = true;
                    selectedControl.IsSelected = false;

                    MenuControl initial = selectedControl;

                    do
                    {
                        selectedControl = selectedControl.OnRight;
                        if(selectedControl == null)
                        {
                            selectedControl = initial;
                            break;
                        }
                    } while(selectedControl.IsDisabled);
                    //} while(loader.levelArray[controlArray.IndexOf(selectedControl)] == null);

                    if(initial != selectedControl)
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);

                    selectedControl.IsSelected = null;
                    return false;
                }
                else if((Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuUpKey) ||
                         Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.MenuUpKey)) && selectedControl.OnUp != null)
                {
                    MouseTempDisabled = true;
                    selectedControl.IsSelected = false;

                    MenuControl initial = selectedControl;
                    do
                    {
                        if(selectedControl.OnUp == null)
                        {
                            selectedControl = initial;
                            break;
                        }
                        selectedControl = selectedControl.OnUp;
                        if(selectedControl.IsDisabled)
                        {
                            MenuControl initialLeft = selectedControl;
                            do
                            {
                                if(selectedControl.OnLeft == null && selectedControl.OnUp == null)
                                {
                                    selectedControl = initial;
                                    break;
                                }
                                if(selectedControl.OnLeft != null)
                                    selectedControl = selectedControl.OnLeft;
                                else
                                    break; // this'll get us out of all the loops
                                if(initialLeft == selectedControl) // we've looped, time to move on
                                    break;
                            } while(selectedControl.IsDisabled);
                        }
                    } while(selectedControl.IsDisabled);
                    //} while(loader.levelArray[controlArray.IndexOf(selectedControl)] == null);

                    if(initial != selectedControl)
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);

                    selectedControl.IsSelected = null;
                    return false;
                }
                else if((Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuDownKey) ||
                         Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.MenuDownKey)) && selectedControl.OnDown != null)
                {
                    MouseTempDisabled = true;
                    selectedControl.IsSelected = false;
                    MenuControl initial = selectedControl;
                    do
                    {
                        if(selectedControl.OnDown == null)
                        {
                            selectedControl = initial;
                            break;
                        }
                        selectedControl = selectedControl.OnDown;
                        if(selectedControl.IsDisabled)
                        {
                            MenuControl initialLeft = selectedControl;
                            do
                            {
                                if(selectedControl.OnLeft == null && selectedControl.OnDown == null)
                                {
                                    selectedControl = initial;
                                    break;
                                }
                                if(selectedControl.OnLeft != null)
                                    selectedControl = selectedControl.OnLeft;
                                else
                                    break;
                                if(initialLeft == selectedControl) // we've looped, time to move on
                                    break;
                            } while(selectedControl.IsDisabled);
                        }
                    } while(selectedControl.IsDisabled);
                    //} while(loader.levelArray[controlArray.IndexOf(selectedControl)] == null);

                    if(initial != selectedControl)
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);

                    selectedControl.IsSelected = null;
                    return false;
                }

                bool? old = selectedControl.IsSelected;

                if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey) ||
                    Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey))
                {
                    holdingSelection = true;
                    MouseTempDisabled = true;
                }
                else if(Input.KeyboardState.IsKeyUp(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey) &&
                    Input.CurrentPad.IsButtonUp(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey) && holdingSelection)
                    holdingSelection = false;

                bool buttonDown = holdingSelection && !selectedControl.IsDisabled;
                //loader.levelArray[controlArray.IndexOf(selectedControl)] != null;
                if(buttonDown)
                    MouseTempDisabled = true;

                if(!old.HasValue && buttonDown && MouseTempDisabled)
                {
                    if(!(selectedControl is DropUpMenuControl))
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                    selectedControl.IsSelected = true;
                    return false;
                }
                else if(old.HasValue && old.Value && !buttonDown && MouseTempDisabled)
                {
                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                    selectedControl.IsSelected = null;
                    holdingSelection = false;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// True denotes do something. False denotes don't.
            /// </summary>
            /// <returns></returns>
            protected virtual bool detectMouseInput()
            {
#if WINDOWS
                if(!MouseTempDisabled)
                {
                    if(Input.MouseState.LeftButton == ButtonState.Released)
                        playedClickSound = false;

                    foreach(MenuControl m in controlArray)
                    {
                        bool? old = m.IsSelected;
                        bool? current = m.CheckMouseInput(selectedControl);

                        if(old.HasValue && !old.Value && !current.HasValue)
                        {
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                            selectedControl = m;
                            return false;
                        }
                        else if(!old.HasValue && current.HasValue && current.Value)
                        {
                            if(!playedClickSound)
                                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                            playedClickSound = true;
                            selectedControl = m;
                            return false;
                        }
                        else if(old.HasValue && old.Value && !current.HasValue && Input.MouseState.LeftButton == ButtonState.Released)
                        {
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                            selectedControl = m;
                            return true;
                        }
                    }
                }
#endif
                return false;
            }
        }
        #endregion

        #region Options
        private class OptionsMenu : Menu
        {
            //private TabControl currentTab;

            private Sprite lowerBox, upperBox;

            private HelpfulTextBox textBox;

            private List<MenuControl> windowsControls = new List<MenuControl>();
            private List<MenuControl> xboxControls = new List<MenuControl>();

            private List<MenuControl> currentList;

            public Vector2[,] optionsVectorArray;
            public Sprite[,] optionsLightBorderArray;
            public Sprite[] optionsCheckmarkArray;

            DropUpMenuControl res;
            MenuButton b1, b2, b3, b4, b5;

            private ToggleControl highScore, voice;

            public OptionsMenu()
            {
                Rectangle buttonRect = new Rectangle(0, 0, 210, 51);
                Rectangle borderRect = new Rectangle(45, 0, 45, 45);
                Vector2 textDimensions = loader.Font.MeasureString("Resume On Focus") * Program.Game.TextureScaleFactor;
                Vector2 initialOffset = new Vector2(45) * Program.Game.TextureScaleFactor;

                lowerBox = loader.lowerOptionsBox;
                upperBox = loader.higherOptionsBox;

                optionsCheckmarkArray = new Sprite[6];
                optionsVectorArray = new Vector2[6, 5];
                optionsLightBorderArray = new Sprite[6, 5];

                float xSpacing = (upperBox.Width - borderRect.Width * 5 * Program.Game.TextureScaleFactor.X - textDimensions.X * 5 - initialOffset.X * 2) / 9;
                float ySpacing = (upperBox.Height - borderRect.Height * 5 * Program.Game.TextureScaleFactor.Y - initialOffset.Y * 2) / 4;

                float cumY = 0;
                float cumX = 0;

                for(int x = 0; x < optionsLightBorderArray.Length / 5; x++)
                {
                    for(int y = 0; y < optionsLightBorderArray.Length / 6; y++)
                    {
                        if(x == 5 && y >= 3)
                            continue;
                        if(x == 4 && y == 3)
                        {
                            optionsLightBorderArray[5, 3] = new Sprite(ref loader.borders, initialOffset + upperBox.UpperLeft + new Vector2(xSpacing * (y + (y % 2)) + cumX, ySpacing * x + cumY),
                                borderRect, Sprite.RenderPoint.UpLeft);
                            optionsVectorArray[5, 3] = optionsLightBorderArray[5, 3].Center + new Vector2(xSpacing + borderRect.Width * Program.Game.TextureScaleFactor.X, -textDimensions.Y * 0.5f);
                        }
                        if(x == 4 && y == 2)
                        {
                            optionsLightBorderArray[5, 4] = new Sprite(ref loader.borders, initialOffset + upperBox.UpperLeft + new Vector2(xSpacing * (y + (y % 2)) + cumX, ySpacing * x + cumY),
                                borderRect, Sprite.RenderPoint.UpLeft);
                            optionsVectorArray[5, 4] = optionsLightBorderArray[5, 4].Center + new Vector2(xSpacing + borderRect.Width * Program.Game.TextureScaleFactor.X, -textDimensions.Y * 0.5f);
                        }

                        optionsLightBorderArray[x, y] = new Sprite(ref loader.borders, initialOffset + upperBox.UpperLeft + new Vector2(xSpacing * (y + (y % 2)) + cumX, ySpacing * x + cumY),
                            borderRect, Sprite.RenderPoint.UpLeft);
                        cumX += textDimensions.X + borderRect.Width * Program.Game.TextureScaleFactor.X;
                        optionsVectorArray[x, y] = optionsLightBorderArray[x, y].Center + new Vector2(xSpacing + borderRect.Width * Program.Game.TextureScaleFactor.X, -textDimensions.Y * 0.5f);
                    }
                    cumY += borderRect.Height * Program.Game.TextureScaleFactor.Y;
                    cumX = 0;
                }

                cumX = cumY = 0;
                ySpacing = (lowerBox.Height - borderRect.Height * 2 * Program.Game.TextureScaleFactor.Y - initialOffset.Y * 2) / 1;
                // use same x spacing

                for(int x = 4; x < optionsLightBorderArray.Length / 5; x++)
                {
                    for(int y = 0; y < optionsLightBorderArray.Length / 6; y++)
                    {
                        if((x == 4 && y < 2) || (x == 5 && y >= 3)) // skip the stuff we still need
                            continue;
                        if(x == 4)
                            optionsLightBorderArray[x, y] = new Sprite(ref loader.borders, initialOffset + lowerBox.UpperLeft + new Vector2(xSpacing * (y - 2 + ((y - 2) % 2)) + cumX, ySpacing * (x - 4) + cumY),
                                borderRect, Sprite.RenderPoint.UpLeft); // simulate y=2 is y=0
                        else 
                            optionsLightBorderArray[x, y] = new Sprite(ref loader.borders, initialOffset + lowerBox.UpperLeft + new Vector2(xSpacing * (y + (y % 2)) + cumX, ySpacing * (x - 4) + cumY),
                                borderRect, Sprite.RenderPoint.UpLeft);
                        cumX += textDimensions.X + borderRect.Width * Program.Game.TextureScaleFactor.X;
                        optionsVectorArray[x, y] = optionsLightBorderArray[x, y].Center + new Vector2(xSpacing + borderRect.Width * Program.Game.TextureScaleFactor.X, -textDimensions.Y * 0.5f);
                    }
                    cumY += borderRect.Height * Program.Game.TextureScaleFactor.Y;
                    cumX = 0;
                }

                optionsCheckmarkArray[0] = new Sprite(ref loader.optionsUI, optionsLightBorderArray[4, 2].UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);
                optionsCheckmarkArray[1] = new Sprite(ref loader.optionsUI, optionsLightBorderArray[4, 3].UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);
                optionsCheckmarkArray[2] = new Sprite(ref loader.optionsUI, optionsLightBorderArray[4, 4].UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);
                optionsCheckmarkArray[3] = new Sprite(ref loader.optionsUI, optionsLightBorderArray[5, 0].UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);
                optionsCheckmarkArray[4] = new Sprite(ref loader.optionsUI, optionsLightBorderArray[5, 1].UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);
                optionsCheckmarkArray[5] = new Sprite(ref loader.optionsUI, optionsLightBorderArray[5, 2].UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);

                textBox = new HelpfulTextBox(new Rectangle((int)(upperBox.UpperLeft.X + 10 * Program.Game.TextureScaleFactor.X), (int)(loader.higherOptionsBox.UpperLeft.X - 10 * Program.Game.TextureScaleFactor.X), (int)(upperBox.Width - loader.backButton.Width - 10 * Program.Game.TextureScaleFactor.X - (upperBox.LowerRight.X - loader.backButton.LowerRight.X)), (int)(upperBox.UpperLeft.Y - 20 * Program.Game.TextureScaleFactor.Y)), loader.SmallerFont);

                VariableButton back = new VariableButton(loader.backButton, delegate { Program.Game.State = BaseGame.GameState.MainMenu; }, String.Empty);

                KeyControl m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, box, mL, mR, mU, mD, cU, cL, cD, cR, cZ, cM, pause, kmute, kmusic;
                m1 = new KeyControl(optionsLightBorderArray[0, 0], false, optionsVectorArray[0, 0], "Machine 1", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key = v; }));
                m2 = new KeyControl(optionsLightBorderArray[0, 1], false, optionsVectorArray[0, 1], "Machine 2", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key = v; }));
                m3 = new KeyControl(optionsLightBorderArray[0, 2], false, optionsVectorArray[0, 2], "Machine 3", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key = v; }));
                m4 = new KeyControl(optionsLightBorderArray[0, 3], false, optionsVectorArray[0, 3], "Machine 4", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key = v; }));
                m5 = new KeyControl(optionsLightBorderArray[0, 4], false, optionsVectorArray[0, 4], "Machine 5", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key = v; }));
                m6 = new KeyControl(optionsLightBorderArray[1, 0], false, optionsVectorArray[1, 0], "Machine 6", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key = v; }));
                m7 = new KeyControl(optionsLightBorderArray[1, 1], false, optionsVectorArray[1, 1], "Machine 7", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key = v; }));
                m8 = new KeyControl(optionsLightBorderArray[1, 2], false, optionsVectorArray[1, 2], "Machine 8", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key = v; }));
                m9 = new KeyControl(optionsLightBorderArray[1, 3], false, optionsVectorArray[1, 3], "Machine 9", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key = v; }));
                m10 = new KeyControl(optionsLightBorderArray[1, 4], false, optionsVectorArray[1, 4], "Machine 10", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key = v; }));
                box = new KeyControl(optionsLightBorderArray[2, 0], false, optionsVectorArray[2, 0], "Send Box", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.QuickBoxKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.QuickBoxKey = v; }));
                cL = new KeyControl(optionsLightBorderArray[2, 1], false, optionsVectorArray[2, 1], "Camera Left", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.CameraLeftKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.CameraLeftKey = v; }));
                cR = new KeyControl(optionsLightBorderArray[2, 2], false, optionsVectorArray[2, 2], "Camera Right", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.CameraRightKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.CameraRightKey = v; }));
                cU = new KeyControl(optionsLightBorderArray[2, 3], false, optionsVectorArray[2, 3], "Camera Up", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.CameraUpKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.CameraUpKey = v; }));
                cD = new KeyControl(optionsLightBorderArray[2, 4], false, optionsVectorArray[2, 4], "Camera Down", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.CameraDownKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.CameraDownKey = v; }));
                cZ = new KeyControl(optionsLightBorderArray[3, 0], false, optionsVectorArray[3, 0], "Zoom In", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.CameraZoomPlusKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.CameraZoomPlusKey = v; }));
                cM = new KeyControl(optionsLightBorderArray[3, 1], false, optionsVectorArray[3, 1], "Zoom Out", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.CameraZoomMinusKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.CameraZoomMinusKey = v; }));
                pause = new KeyControl(optionsLightBorderArray[3, 2], false, optionsVectorArray[3, 2], "Pause", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.PauseKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.PauseKey = v; }));
                mL = new KeyControl(optionsLightBorderArray[3, 3], false, optionsVectorArray[3, 3], "Menu Left", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey = v; }));
                mR = new KeyControl(optionsLightBorderArray[3, 4], false, optionsVectorArray[3, 4], "Menu Right", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey = v; }));
                mU = new KeyControl(optionsLightBorderArray[4, 0], false, optionsVectorArray[4, 0], "Menu Up", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.MenuUpKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.MenuUpKey = v; }));
                mD = new KeyControl(optionsLightBorderArray[4, 1], false, optionsVectorArray[4, 1], "Menu Down", loader.SmallerFont, new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.MenuDownKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.MenuDownKey = v; }));
                kmute = new KeyControl(optionsLightBorderArray[5, 4], false, optionsVectorArray[5, 4], "Mute Key", loader.SmallerFont,
                    new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.MuteKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.MuteKey = v; }));
                kmusic = new KeyControl(optionsLightBorderArray[5, 3], false, optionsVectorArray[5, 3], "Music Player", loader.SmallerFont,
                    new Pointer<Keys>(() => Program.Game.Manager.CurrentSaveWindowsOptions.MusicKey, v => { Program.Game.Manager.CurrentSaveWindowsOptions.MusicKey = v; }));

                ButtonControl bm1, bm2, bm3, bm4, bm5, bm6, bm7, bm8, bm9, bm10, bbox, bcZ, bcM, bpause, bmusic;
                bm1 = new ButtonControl(optionsLightBorderArray[0, 0], false, optionsVectorArray[0, 0], "Machine 1", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key = v; }));
                bm2 = new ButtonControl(optionsLightBorderArray[0, 1], false, optionsVectorArray[0, 1], "Machine 2", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key = v; }));
                bm3 = new ButtonControl(optionsLightBorderArray[0, 2], false, optionsVectorArray[0, 2], "Machine 3", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key = v; }));
                bm4 = new ButtonControl(optionsLightBorderArray[0, 3], false, optionsVectorArray[0, 3], "Machine 4", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key = v; }));
                bm5 = new ButtonControl(optionsLightBorderArray[0, 4], false, optionsVectorArray[0, 4], "Machine 5", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key = v; }));
                bm6 = new ButtonControl(optionsLightBorderArray[1, 0], false, optionsVectorArray[1, 0], "Machine 6", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key = v; }));
                bm7 = new ButtonControl(optionsLightBorderArray[1, 1], false, optionsVectorArray[1, 1], "Machine 7", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key = v; }));
                bm8 = new ButtonControl(optionsLightBorderArray[1, 2], false, optionsVectorArray[1, 2], "Machine 8", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key = v; }));
                bm9 = new ButtonControl(optionsLightBorderArray[1, 3], false, optionsVectorArray[1, 3], "Machine 9", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key = v; }));
                bm10 = new ButtonControl(optionsLightBorderArray[1, 4], false, optionsVectorArray[1, 4], "Machine 10", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key, v => { Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key = v; }));
                bbox = new ButtonControl(optionsLightBorderArray[2, 0], false, optionsVectorArray[2, 0], "Send Box", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.QuickBoxKey, v => { Program.Game.Manager.CurrentSaveXboxOptions.QuickBoxKey = v; }));
                bcZ = new ButtonControl(optionsLightBorderArray[2, 1], false, optionsVectorArray[2, 1], "Zoom In", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.CameraZoomPlusKey, v => { Program.Game.Manager.CurrentSaveXboxOptions.CameraZoomPlusKey = v; }));
                bcM = new ButtonControl(optionsLightBorderArray[2, 2], false, optionsVectorArray[2, 2], "Zoom Out", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.CameraZoomMinusKey, v => { Program.Game.Manager.CurrentSaveXboxOptions.CameraZoomMinusKey = v; }));
                bpause = new ButtonControl(optionsLightBorderArray[2, 3], false, optionsVectorArray[2, 3], "Pause", loader.SmallerFont, new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.PauseKey, v => { Program.Game.Manager.CurrentSaveXboxOptions.PauseKey = v; }));
                bmusic = new ButtonControl(optionsLightBorderArray[2, 4], false, optionsVectorArray[2, 4], "Music Player", loader.SmallerFont,
                    new Pointer<Buttons>(() => Program.Game.Manager.CurrentSaveXboxOptions.MusicKey, v => { Program.Game.Manager.CurrentSaveXboxOptions.MusicKey = v; }));

                back.SetPointerDirectionals(null, null, null, new Pointer<MenuControl>(()=>currentList[3],v=>{}));

                //WindowBox difficulty = new WindowBox(difficultyBorderLight, loader.leftLightArrow, loader.rightLightArrow, loader.difficultySlider,
                //    loader.difficultyVector, new Rectangle((int)Program.Game.Manager.CurrentSaveWindowsOptions.DifficultyLevel, 0, loader.difficultySlider.Width / 3, loader.difficultySlider.Height),
                //    optionsVectorArray[5, 4], "Difficulty", loader.Font, new Pointer<Enum>(() => Program.Game.Manager.CurrentSaveWindowsOptions.DifficultyLevel, v => { Program.Game.Manager.CurrentSaveWindowsOptions.DifficultyLevel=(WindowsOptions.Difficulty)v; }), 3, Tooltips.DifficultyString);

                ToggleControl resumeOnFocus, fullscreen, swap, mute, fancy;

                swap = new ToggleControl(optionsCheckmarkArray[0], optionsLightBorderArray[4, 2], optionsVectorArray[4, 2], "Invert Camera", loader.SmallerFont,
                    new Pointer<bool>(() => Program.Game.Manager.CurrentSaveWindowsOptions.SwapCamera, v => { Program.Game.Manager.CurrentSaveWindowsOptions.SwapCamera = v; }), 
                    "Press %s% or click the box to invert the vertical axis of the camera.");

                mute = new ToggleControl(optionsCheckmarkArray[1], optionsLightBorderArray[4, 3], optionsVectorArray[4, 3], "Mute Sounds", loader.SmallerFont,
                    new Pointer<bool>(() => Program.Game.Manager.CurrentSaveWindowsOptions.Muted, v => { Program.Game.Manager.CurrentSaveWindowsOptions.Muted = v; }), 
                    "Press %s% or click the box to mute all music and sounds.");

#if WINDOWS
                Sprite resButton, res1, res2, res3, res4, res5;

                resButton = new Sprite(ref loader.buttonsTex, optionsLightBorderArray[5, 2].UpperLeft - new Vector2(0, 5 * Program.Game.TextureScaleFactor.Y), new Rectangle(0, buttonRect.Height * 8, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
                res1 = new Sprite(ref loader.buttonsTex, optionsLightBorderArray[5, 2].UpperLeft - new Vector2(0, 5 * Program.Game.TextureScaleFactor.Y), new Rectangle(buttonRect.Width, buttonRect.Height * 8, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
                res2 = new Sprite(ref loader.buttonsTex, optionsLightBorderArray[5, 2].UpperLeft - new Vector2(0, 5 * Program.Game.TextureScaleFactor.Y), new Rectangle(0, buttonRect.Height * 9, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
                res3 = new Sprite(ref loader.buttonsTex, optionsLightBorderArray[5, 2].UpperLeft - new Vector2(0, 5 * Program.Game.TextureScaleFactor.Y), new Rectangle(buttonRect.Width, buttonRect.Height * 9, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
                res4 = new Sprite(ref loader.buttonsTex, optionsLightBorderArray[5, 2].UpperLeft - new Vector2(0, 5 * Program.Game.TextureScaleFactor.Y), new Rectangle(0, buttonRect.Height * 10, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);
                res5 = new Sprite(ref loader.buttonsTex, optionsLightBorderArray[5, 2].UpperLeft - new Vector2(0, 5 * Program.Game.TextureScaleFactor.Y), new Rectangle(buttonRect.Width, buttonRect.Height * 10, buttonRect.Width, buttonRect.Height), Sprite.RenderPoint.UpLeft);

                string resString = "Use this menu to select the resolution of the game. Changes take effect immediately. Current resolution is: %r%";

                b1 = new MenuButton(res1, delegate { Program.Game.Manager.CurrentSaveWindowsOptions.ScreenWidth = Program.Game.GDM.PreferredBackBufferWidth = 1024; Program.Game.Manager.CurrentSaveWindowsOptions.ScreenHeight = Program.Game.GDM.PreferredBackBufferHeight = 768; Program.Game.GDM.ApplyChanges(); }, resString);
                b2 = new MenuButton(res2, delegate { Program.Game.Manager.CurrentSaveWindowsOptions.ScreenWidth = Program.Game.GDM.PreferredBackBufferWidth = 1280; Program.Game.Manager.CurrentSaveWindowsOptions.ScreenHeight = Program.Game.GDM.PreferredBackBufferHeight = 720; Program.Game.GDM.ApplyChanges(); }, resString);
                b3 = new MenuButton(res3, delegate { Program.Game.Manager.CurrentSaveWindowsOptions.ScreenWidth = Program.Game.GDM.PreferredBackBufferWidth = 1366; Program.Game.Manager.CurrentSaveWindowsOptions.ScreenHeight = Program.Game.GDM.PreferredBackBufferHeight = 768; Program.Game.GDM.ApplyChanges(); }, resString);
                b4 = new MenuButton(res4, delegate { Program.Game.Manager.CurrentSaveWindowsOptions.ScreenWidth = Program.Game.GDM.PreferredBackBufferWidth = 1980; Program.Game.Manager.CurrentSaveWindowsOptions.ScreenHeight = Program.Game.GDM.PreferredBackBufferHeight = 1080; Program.Game.GDM.ApplyChanges(); }, resString);
                b5 = new MenuButton(res5, delegate { Program.Game.Manager.CurrentSaveWindowsOptions.ScreenWidth = Program.Game.GDM.PreferredBackBufferWidth = 800; Program.Game.Manager.CurrentSaveWindowsOptions.ScreenHeight = Program.Game.GDM.PreferredBackBufferHeight = 600; Program.Game.GDM.ApplyChanges(); }, resString);

                resumeOnFocus = new ToggleControl(optionsCheckmarkArray[2], optionsLightBorderArray[4,4], optionsVectorArray[4,4], "Resume On Focus", loader.SmallerFont,
                    new Pointer<bool>(() => Program.Game.Manager.CurrentSaveWindowsOptions.ResumeOnFocus, v => { Program.Game.Manager.CurrentSaveWindowsOptions.ResumeOnFocus = v; }), 
                    "Press %s% or click the box to toggle whether the game will automatically resume from a paused state once it has gained focus.");

                voice = new ToggleControl(optionsCheckmarkArray[3], optionsLightBorderArray[5,0], optionsVectorArray[5,0], "Voice Clips", loader.SmallerFont,
                    new Pointer<bool>(() => Program.Game.Manager.CurrentSaveWindowsOptions.VoiceClips, v => { Program.Game.Manager.CurrentSaveWindowsOptions.VoiceClips = v; }),
                    "Press %s% or click the box to mute the voice acting only.");

                highScore = new ToggleControl(optionsCheckmarkArray[4], optionsLightBorderArray[5,1], optionsVectorArray[5,1], "High Score Mode", loader.SmallerFont,
                    new Pointer<bool>(() => Program.Game.Manager.CurrentSaveWindowsOptions.HighScoreMode, v => { Program.Game.Manager.CurrentSaveWindowsOptions.HighScoreMode = v; }),
                    "Press %s% or click the box to toggle High Score Mode. In High Score Mode, you will play until you run out of boxes with the intention to get the highest score possible.");
                
                Sprite highScoreTex = new Sprite(ref loader.borders, initialOffset + lowerBox.UpperLeft + new Vector2(xSpacing * (5 - 2 + ((5 - 2) % 2)) + (textDimensions.X + borderRect.Width * Program.Game.TextureScaleFactor.X) * 3, ySpacing * (4 - 4)),
                    borderRect, Sprite.RenderPoint.UpLeft);
                Sprite checkmark = new Sprite(ref loader.optionsUI, highScoreTex.UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);
                fullscreen = new ToggleControl(checkmark, highScoreTex, highScoreTex.Center + new Vector2(xSpacing + borderRect.Width * Program.Game.TextureScaleFactor.X, -textDimensions.Y * 0.5f), "Fullscreen", loader.SmallerFont,
                    new Pointer<bool>(() => Program.Game.Manager.CurrentSaveWindowsOptions.FullScreen, v => { Program.Game.Manager.CurrentSaveWindowsOptions.FullScreen = v; }),
                    "Press %s% or click the box to toggle between fullscreen and windowed mode. Changes will take effect immediately.");

                Sprite fancyTex = new Sprite(ref loader.borders, new Vector2(highScoreTex.UpperLeft.X, voice.Texture.UpperLeft.Y),
                    borderRect, Sprite.RenderPoint.UpLeft);
                Sprite checkmark2 = new Sprite(ref loader.optionsUI, fancyTex.UpperLeft - new Vector2(0, 10 * Program.Game.TextureScaleFactor.Y), new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.UpLeft);
                fancy = new ToggleControl(checkmark2, fancyTex, fancyTex.Center + new Vector2(xSpacing + borderRect.Width * Program.Game.TextureScaleFactor.X, -textDimensions.Y * 0.5f), "Fancy Graphics", loader.SmallerFont,
                    new Pointer<bool>(() => Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics, v => { Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics = v; }),
                    "Press %s% or click the box to toggle reflections in the environment.");
                
                if(!Program.Game.HiDef)
                    fancy.IsDisabled = true;

                res = new DropUpMenuControl(resButton, resString);
                res.SetPointerDirectionals(new Pointer<MenuControl>(() => { if(controlArray.Contains(highScore)) return highScore; else if(controlArray.Contains(voice)) return voice; else return controlArray[2]; }, v => { }), 
                    new Pointer<MenuControl>(() => fancy, v => { }), new Pointer<MenuControl>(() => resumeOnFocus, v => { }), null);
                res.SetInternalMenu(new List<MenuButton>() { b5, b1, b2, b3, b4 });
#endif
                bm1.SetDirectionals(bm5, bm2, back, bm6);
                bm2.SetDirectionals(bm1, bm3, back, bm7);
                bm3.SetDirectionals(bm2, bm4, back, bm8);
                bm4.SetDirectionals(bm3, bm5, back, bm9);
                bm5.SetDirectionals(bm4, bm1, back, bm10);
                bm6.SetDirectionals(bm10, bm7, bm1, bbox);
                bm7.SetDirectionals(bm6, bm8, bm2, bcZ);
                bm8.SetDirectionals(bm7, bm9, bm3, bcM);
                bm9.SetDirectionals(bm8, bm10, bm4, bpause);
                bm10.SetDirectionals(bm9, bm6, bm5, bmusic);
                bbox.SetDirectionals(bmusic, bcZ, bm6, swap);
                bcZ.SetDirectionals(bbox, bcM, bm7, mute);
                bcM.SetDirectionals(bcZ, bpause, bm8, resumeOnFocus);
                bpause.SetDirectionals(bcM, bmusic, bm9, fullscreen);
                bmusic.SetDirectionals(bpause, bbox, bm10, fullscreen);

                m1.SetDirectionals(m5, m2, back, m6);
                m2.SetDirectionals(m1, m3, back, m7);
                m3.SetDirectionals(m2, m4, back, m8);
                m4.SetDirectionals(m3, m5, back, m9);
                m5.SetDirectionals(m4, m1, back, m10);
                m6.SetDirectionals(m10, m7, m1, box);
                m7.SetDirectionals(m6, m8, m2, cL);
                m8.SetDirectionals(m7, m9, m3, cR);
                m9.SetDirectionals(m8, m10, m4, cU);
                m10.SetDirectionals(m9, m6, m5, cD);
                box.SetDirectionals(cD, cL, m6, cZ);
                cL.SetDirectionals(box, cR, m7, cM);
                cR.SetDirectionals(cL, cU, m8, pause);
                cU.SetDirectionals(cR, cD, m9, mL);
                cD.SetDirectionals(cU, box, m10, mR);
                cZ.SetDirectionals(mR, cM, box, mU);
                cM.SetDirectionals(cZ, pause, cL, mD);
                pause.SetDirectionals(cM, mL, cR, kmute);
                mL.SetDirectionals(pause, mR, cU, kmusic);
                mU.SetDirectionals(kmusic, mD, cZ, swap);
                mD.SetDirectionals(mU, kmute, cM, mute);
                mR.SetDirectionals(mL, cZ, cD, resumeOnFocus);
                kmute.SetDirectionals(mD, kmusic, pause, resumeOnFocus);
                kmusic.SetDirectionals(kmute, mU, mL, fullscreen);

#if WINDOWS
                fullscreen.SetAction(delegate { Program.Game.GDM.IsFullScreen = Program.Game.Manager.CurrentSaveWindowsOptions.FullScreen; Program.Game.GDM.ApplyChanges(); });
                mute.SetAction(delegate { MediaSystem.ToggleMute(); });

                resumeOnFocus.SetPointerDirectionals(new Pointer<MenuControl>(() => mute, v => { }), new Pointer<MenuControl>(() => fullscreen, v => { }), new Pointer<MenuControl>(() => currentList[12], v => { }), new Pointer<MenuControl>(() => res, v => { }));
                voice.SetPointerDirectionals(new Pointer<MenuControl>(() => mute, v => { }), new Pointer<MenuControl>(() => { if(controlArray.Contains(highScore)) return highScore; return null; }, v => { }), new Pointer<MenuControl>(() => swap, v => { }), new Pointer<MenuControl>(() => res, v => { }));
                mute.SetPointerDirectionals(new Pointer<MenuControl>(() => swap, v => { }), new Pointer<MenuControl>(() => resumeOnFocus, v => { }), new Pointer<MenuControl>(() => currentList[13], v => { }), new Pointer<MenuControl>(() => { if(controlArray.Contains(highScore)) return highScore; return null; }, v => { }));
                swap.SetPointerDirectionals(null, new Pointer<MenuControl>(() => mute, v => { }), new Pointer<MenuControl>(() => currentList[10], v => { }), new Pointer<MenuControl>(() => { if(controlArray.Contains(voice)) return voice; return null; }, v => { }));
                fullscreen.SetPointerDirectionals(new Pointer<MenuControl>(() => resumeOnFocus, v => { }), null, new Pointer<MenuControl>(() => currentList[14], v => { }), new Pointer<MenuControl>(() => fancy, v => { }));
                highScore.SetPointerDirectionals(new Pointer<MenuControl>(() => { if(controlArray.Contains(voice)) return voice; return null; }, v => { }), new Pointer<MenuControl>(() => res, v => { }), new Pointer<MenuControl>(() => mute, v => { }), null);
                fancy.SetPointerDirectionals(new Pointer<MenuControl>(() => res, v => { }), null, new Pointer<MenuControl>(() => fullscreen, v => { }), null);
                controlArray.AddRange(new MenuControl[] { back, resumeOnFocus, mute, fullscreen, swap, res, fancy });//voice, highScore });
#else
                voice.SetPointerDirectionals(new Pointer<MenuControl>(() => mute, v => { }), new Pointer<MenuControl>(() => difficulty, v => { }), new Pointer<MenuControl>(() => currentTab.Controls[12], v => { }), null);
                mute.SetPointerDirectionals(new Pointer<MenuControl>(() => swap, v => { }), new Pointer<MenuControl>(() => voice, v => { }), new Pointer<MenuControl>(() => currentTab.Controls[13], v => { }), null);
                swap.SetPointerDirectionals(null, new Pointer<MenuControl>(() => mute, v => { }), new Pointer<MenuControl>(() => currentTab.Controls[10], v => { }), null);
                controlArray.AddRange(new MenuControl[] { back, difficulty, voice, mute, swap });
#endif

                back.IsSelected = null;
                selectedControl = back;

                xboxControls.AddRange(new MenuControl[] { bm1, bm2, bm3, bm4, bm5, bm6, bm7, bm8, bm9, bm10, bbox, bmusic, bcM, bcZ, bpause });
                windowsControls.AddRange(new MenuControl[] { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, mU, mR, kmute, mD, kmusic, cU, cD, cZ, cM, mL, box, cL, pause, cR });

                if(Input.ControlScheme == ControlScheme.Keyboard)
                    currentList = windowsControls;
                else
                    currentList = xboxControls;

                Program.Game.GDM.DeviceReset += rebuildSpacing;
            }

            public override void Draw(GameTime gameTime)
            {
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                lowerBox.Draw();
                upperBox.Draw();

                if(Input.ControlScheme == ControlScheme.Keyboard)
                    currentList = windowsControls;
                else
                    currentList = xboxControls;

                foreach(MenuControl m in currentList)
                    m.Draw(selectedControl);

                base.Draw(gameTime); // draw non-tab controls

                textBox.Draw(selectedControl.HelpfulText);

                Program.Game.SpriteBatch.End();
            }

            public override void Update(GameTime gameTime)
            {
                if((xboxControls.Contains(selectedControl) && Input.ControlScheme == ControlScheme.Keyboard) ||
                    (windowsControls.Contains(selectedControl) && Input.ControlScheme == ControlScheme.XboxController))
                {
                    selectedControl.IsSelected = false;
                    selectedControl = controlArray[0];
                    selectedControl.IsSelected = null;
                }

#if !DEBUG && !INTERNAL
                if(Program.Game.Manager.CurrentSave.LevelData[10].Completed && !controlArray.Contains(voice))
#else
                if(!controlArray.Contains(voice))
#endif
                    controlArray.Add(voice);
#if !DEBUG && !INTERNAL
                if(Program.Game.Manager.CurrentSave.LevelData[11].Completed && !controlArray.Contains(highScore))
#else
                if(!controlArray.Contains(highScore))
#endif
                    controlArray.Add(highScore);

                // b1 is 1024x768, b2 is 1280x720, b3 is 1366x768, b4 is 1980x1080, b5 is 800x600
                int width = Program.Game.ScreenWidth;
                if(width < 1980)
                    b4.IsDisabled = true;
                if(width < 1366)
                    b3.IsDisabled = true;
                if(width < 1280)
                    b2.IsDisabled = true;
                if(width < 1024)
                    b1.IsDisabled = true;
                int height = Program.Game.ScreenHeight;
                if(height < 1080)
                    b4.IsDisabled = true;
                if(height < 768)
                    b1.IsDisabled = b3.IsDisabled = true;
                if(height < 720)
                    b2.IsDisabled = true;

                foreach(GreedyControl c in currentList.OfType<GreedyControl>())
                {
                    if(c.IsActive)
                    {
                        c.OnSelect();
                        return;
                    }
                }
                foreach(GreedyControl c in controlArray.OfType<GreedyControl>())
                {
                    if(c.IsActive)
                    {
                        c.OnSelect();
                        return;
                    }
                }

                MenuControl t = res.Update(selectedControl, gameTime);
                if(t != null)
                    selectedControl = t;

                if(detectKeyboardInput() || detectMouseInput())
                    selectedControl.OnSelect();
            }

            protected override bool detectMouseInput()
            {
                bool? old, current;

                if(res.IsActive)
                {
                    old = res.IsSelected;
                    current = res.CheckMouseInput(selectedControl);

                    if(old.HasValue && !old.Value && !current.HasValue)
                    {
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                        selectedControl = res;
                        return false;
                    }
                    else if(!old.HasValue && current.HasValue && current.Value)
                    {
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                        selectedControl = res;
                        return false;
                    }
                    else if(old.HasValue && old.Value && !current.HasValue)
                    {
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                        selectedControl = res;
                        return true;
                    }
                }

                foreach(MenuControl m in currentList)
                {
                    old = m.IsSelected;
                    current = m.CheckMouseInput(selectedControl);

                    if(old.HasValue && !old.Value && !current.HasValue)
                    {
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                        selectedControl = m;
                        return false;
                    }
                    else if(!old.HasValue && current.HasValue && current.Value)
                    {
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                        selectedControl = m;
                        return false;
                    }
                    else if(old.HasValue && old.Value && !current.HasValue)
                    {
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                        selectedControl = m;
                        return true;
                    }
                }

                return base.detectMouseInput();
            }

            private void rebuildSpacing(object caller, EventArgs e)
            {
                upperBox.ForceResize();
                loader.backButton.ForceResize();

                textBox.SetSpace(new Rectangle((int)(upperBox.UpperLeft.X + 10 * Program.Game.TextureScaleFactor.X), (int)(10 * Program.Game.TextureScaleFactor.Y), (int)(loader.backButton.UpperLeft.X - upperBox.UpperLeft.X), (int)(upperBox.UpperLeft.Y - 20 * Program.Game.TextureScaleFactor.Y)));
            }
        }
        #endregion

        #region Game Over
        private class GameOverMenu : Menu
        {
            public GameOverMenu()
            {
                MenuControl restart, levSel, mainMenu, quit;
                restart = new MenuButton(loader.gameOverRestartButton, delegate { Program.Game.State = BaseGame.GameState.Running; Program.Game.CurrentLevel.ResetLevel(); });
                levSel = new MenuButton(loader.gameOverLevSelButton, delegate { Program.Game.CurrentLevel.RemoveFromGame(Program.Game.Space); Program.Game.LevelNumber = -1; Program.Game.State = BaseGame.GameState.Menuing_Lev; MediaSystem.PlayTrack(MediaSystem.SongOptions.Menu); });
                mainMenu = new MenuButton(loader.mainMenuButton, delegate { Program.Game.CurrentLevel.RemoveFromGame(Program.Game.Space); Program.Game.LevelNumber = -1; MediaSystem.PlayTrack(MediaSystem.SongOptions.Menu); });
                quit = new MenuButton(loader.pauseQuitButton, delegate { Program.Game.State = BaseGame.GameState.Exiting; });
                restart.SetDirectionals(null, levSel, null, null);
                levSel.SetDirectionals(restart, mainMenu, null, null);
                mainMenu.SetDirectionals(levSel, quit, null, null);
                quit.SetDirectionals(mainMenu, null, null, null);
                
                controlArray.Add(restart);
                controlArray.Add(levSel);
                controlArray.Add(mainMenu);
                controlArray.Add(quit);
                selectedControl = restart;
                selectedControl.IsSelected = null;
            }

            public override void Draw(GameTime gameTime)
            {
                Program.Game.DrawLevel(gameTime);

                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                Program.Game.SpriteBatch.DrawString(loader.Font, "You have lost too many boxes.", new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.19f), Color.White, 0.0f, loader.Font.MeasureString("You have lost too many boxes") * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                base.Draw(gameTime);

                Program.Game.SpriteBatch.End();
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
            }
        }
        #endregion

        #region Game Pad Disconnected
        private class GamePadDCMenu : Menu
        {
            public GamePadDCMenu()
            { }

            public override void Draw(GameTime gameTime)
            {
#if XBOX
                SimpleMessageBox.ShowMessageBox("Game Pad Disconnected!", "The primary game pad has become disconnected. Please reconnect it to continue.",
                    new string[] { "Okay" }, 0, MessageBoxIcon.Error);
#else
                if(Program.Game.PreviousState == BaseGame.GameState.Running)
                    Program.Game.DrawLevel(gameTime);

                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                Program.Game.SpriteBatch.Draw(loader.halfBlack, new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), Color.White);
                Program.Game.SpriteBatch.DrawString(loader.BiggerFont, "The gamepad has become disconnected. Please\nreconnect it, or press the Spacebar to\ncontinue using the keyboard.", new Vector2(Program.Game.Width, Program.Game.Height) * 0.5f, Color.White, 0, loader.BiggerFont.MeasureString("The gamepad has become disconnected. Please\nreconnect it, or press the Spacebar to\ncontinue using the keyboard.") * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                Program.Game.SpriteBatch.End();
#endif
            }

            public override void Update(GameTime gameTime)
            {
                if(Input.CurrentPad.IsConnected)
                {
                    Program.Game.State = Program.Game.PreviousState;
                    MediaSystem.PlayAll();
                }
#if WINDOWS
                if(Input.CheckKeyboardJustPressed(Keys.Space))
                {
                    Input.ContinueWithKeyboard();
                    MediaSystem.PlayAll();
                }
#endif
            }
        }
        #endregion

        #region Game Pad Query
        private class GamePadQueryMenu : Menu
        {
            public GamePadQueryMenu()
            { }

            public override void Draw(GameTime gameTime)
            {
                if(Program.Game.PreviousState == BaseGame.GameState.Running)
                    Program.Game.DrawLevel(gameTime);

                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                Program.Game.SpriteBatch.Draw(loader.halfBlack, new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), Color.White);
                string text = "An Xbox controller has been plugged in. Press\nStart to continue the game using the\ncontroller, or disconnect it to close\nthis prompt.";
                Program.Game.SpriteBatch.DrawString(loader.BiggerFont, text, new Vector2(Program.Game.Width, Program.Game.Height) * 0.5f, Color.White, 0, loader.BiggerFont.MeasureString(text) * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                Program.Game.SpriteBatch.End();
            }

            public override void Update(GameTime gameTime)
            {
                if(!Input.CurrentPad.IsConnected)
                {
                    Program.Game.State = Program.Game.PreviousState;
                    MediaSystem.PlayAll();
                }
                if(Input.CheckXboxJustPressed(Buttons.Start) || Input.CheckXboxJustPressed(Buttons.A))
                {
                    Input.ContinueWithPad();
                    MediaSystem.PlayAll();
                }
            }
        }
        #endregion

        #region Pause
        private class PauseMenu : Menu
        {
            private readonly ToggleControl voice;
            private readonly MenuButton resume;
            private bool confirming = false;
            private bool confirmRestart = false;
            private ConfirmationMenu menu;
            private ConfirmationMenu restartMenu;

            public PauseMenu()
            {
                MenuButton restart, mainMenu, quit;

                resume = new MenuButton(loader.resumeButton, delegate { MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Pause); Program.Game.State = BaseGame.GameState.Running; });
                restart = new MenuButton(loader.restartButton, delegate { restartMenu.Reset(); confirmRestart = true; });
                mainMenu = new MenuButton(loader.mainMenuButton, delegate { menu.Reset(); confirming = true; });
                quit = new MenuButton(loader.pauseQuitButton, delegate { Program.Game.State = BaseGame.GameState.Exiting; });
                
                Vector2 pos = new Vector2(Program.Game.Width * 0.1f, Program.Game.Height * 0.9f);

                Sprite checkmark = new Sprite(ref loader.optionsUI, pos, new Rectangle(200, 0, 40, 50), Sprite.RenderPoint.Center);
                Sprite border = new Sprite(ref loader.borders, pos, new Rectangle(45, 0, 45, 45), Sprite.RenderPoint.Center);
                voice = new ToggleControl(checkmark, border, pos + new Vector2(border.Width + 5 * Program.Game.TextureScaleFactor.X, -Program.Game.Loader.Font.MeasureString("Temporarily Mute Voice").Y * 0.5f),
                    "Temporarily Mute Voice", Program.Game.Loader.Font, new Pointer<bool>(() => Program.Game.CurrentLevel.TemporarilyMuteVoice, v => { Program.Game.CurrentLevel.TemporarilyMuteVoice = v; MediaSystem.StopVoiceActing(); }), String.Empty);

                resume.SetDirectionals(null, restart, null, voice);
                restart.SetDirectionals(resume, mainMenu, null, voice);
                mainMenu.SetDirectionals(restart, quit, null, voice);
                quit.SetDirectionals(mainMenu, null, null, voice);
                voice.SetPointerDirectionals(null, new Pointer<MenuControl>(() => resume, v => { }), new Pointer<MenuControl>(() => resume, v => { }), null);

                controlArray.AddRange(new MenuControl[] { resume, restart, mainMenu, quit, voice });
                selectedControl = resume;
                selectedControl.IsSelected = null;

                menu = new ConfirmationMenu("Are you sure you want to return to the main menu?\n             All progress in this level will be lost.",
                    delegate { Program.Game.CurrentLevel.RemoveFromGame(Program.Game.Space); Program.Game.LevelNumber = -1; MediaSystem.PlayTrack(MediaSystem.SongOptions.Menu); if(Program.Game.LevelEnteredFrom == BaseGame.GameState.Menuing_Lev) Program.Game.State = BaseGame.GameState.Menuing_Lev; });
                restartMenu = new ConfirmationMenu("Are you sure you want to restart the level?", delegate { Program.Game.CurrentLevel.ResetLevel(); Program.Game.State = BaseGame.GameState.Running; MenuHandler.mainMenu.ResetTimer(); });
            }

            public override void Draw(GameTime gameTime)
            {
                Program.Game.DrawLevel(gameTime);

                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                Program.Game.SpriteBatch.Draw(loader.halfBlack, new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), Color.White);
                Program.Game.SpriteBatch.Draw(loader.mainMenuLogo, new Vector2(Program.Game.Width * 0.97f - loader.mainMenuLogo.Width / 2, Program.Game.Height * 0.03f - loader.mainMenuLogo.Height / 2), null, Color.White, 0.0f, new Vector2(loader.mainMenuLogo.Width / 2, loader.mainMenuLogo.Height / 2), 0.3f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                Program.Game.SpriteBatch.DrawString(loader.BiggerFont, "Paused", new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.3f), Color.OrangeRed, 0, loader.BiggerFont.MeasureString("Paused") * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                
                Vector2 stringLength = loader.BiggerFont.MeasureString("Press       to resume");
                Vector2 screenSpot = new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.5f);
                Program.Game.SpriteBatch.DrawString(loader.BiggerFont, "Press       to resume", screenSpot, Color.White, 0, stringLength * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                if(Input.ControlScheme == ControlScheme.Keyboard)
                    SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.PauseKey, screenSpot, new Vector2((stringLength.X * 0.5f + SymbolWriter.IconCenter.X * 0.5f - loader.BiggerFont.MeasureString("Press ").X), SymbolWriter.IconCenter.Y), false);
                else
                    SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.PauseKey, screenSpot, new Vector2((stringLength.X * 0.5f + SymbolWriter.IconCenter.X * 0.5f - loader.BiggerFont.MeasureString("Press ").X), SymbolWriter.IconCenter.Y), false);

                base.Draw(gameTime);

                if(confirming)
                    menu.Draw(gameTime);
                else if(confirmRestart)
                    restartMenu.Draw(gameTime);

                Program.Game.SpriteBatch.End();
            }

            public override void Update(GameTime gameTime)
            {
                if(confirming)
                {
                    menu.Update(gameTime);
                    if(menu.Finished)
                    {
                        confirming = false;
                        menu.Reset();
                    }
                    return;
                }
                else if(confirmRestart)
                {
                    restartMenu.Update(gameTime);
                    if(restartMenu.Finished)
                    {
                        confirmRestart = false;
                        restartMenu.Reset();
                    }
                    return;
                }

                if(Program.Game.LevelNumber > 10 || Program.Game.Manager.CurrentSaveWindowsOptions.Muted)
                    voice.IsDisabled = true;
                else
                    voice.IsDisabled = false;

                if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.PauseKey) ||
                    Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.PauseKey))
                {
                    Program.Game.State = BaseGame.GameState.Running;
                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Pause);
                    selectedControl.IsSelected = false;
                    resume.IsSelected = null;
                    return;
                }
                base.Update(gameTime);
            }
        }
        #endregion

        #region Main Menu
        private class MainMenu : Menu
        {
            private float timerInMilliseconds = 0;

            private bool startBeenPressed;
            private int timer = 1200;
            private const int smallerTime = 300;
            private const int largerTime = 750;
            private readonly SaveSelectMenu save;

            private readonly MenuControl cont, start, instructions, levSel;
            private readonly DropUpMenuControl extras;

            public bool SaveSelecting { get { return !SaveSelectMenu.Finished; } }

            public MainMenu()
            {
                save = new SaveSelectMenu(loader);

                MenuButton options, quit, saves, objectives;//, highScores;
                start = new MenuButton(loader.startButton, delegate { if(MediaSystem.PlayingVoiceActing) return; Program.Game.LevelNumber = 1; timerInMilliseconds = 0; Program.Game.LevelEnteredFrom = BaseGame.GameState.Running; });
                instructions = new MenuButton(loader.instructionsButton, delegate { Program.Game.LevelNumber = 0; Program.Game.LevelEnteredFrom = BaseGame.GameState.MainMenu; });
                levSel = new MenuButton(loader.levelSelectButton, delegate { Program.Game.State = BaseGame.GameState.Menuing_Lev; });
                quit = new MenuButton(loader.quitButton, delegate { Program.Game.State = BaseGame.GameState.Exiting; });
                cont = new MenuButton(loader.continueButton, delegate { if(MediaSystem.PlayingVoiceActing) return; Program.Game.LevelNumber = Program.Game.Manager.CurrentSave.CurrentLevel; timerInMilliseconds = 0; Program.Game.LevelEnteredFrom = BaseGame.GameState.Running; });
                objectives = new MenuButton(loader.objectiveButton, delegate { Program.Game.State = BaseGame.GameState.Menuing_Obj; });

                options = new MenuButton(loader.optionsButton, delegate { Program.Game.State = BaseGame.GameState.Menuing_Opt; });
                extras = new DropUpMenuControl(loader.extrasButton);
                saves = new MenuButton(loader.savesButton, save.Reset);
                //saves.IsDisabled = true;
                //objectives.IsDisabled = true;
                //highScores = new MenuButton(loader.highScoreButton, delegate { Program.Game.State = BaseGame.GameState.Menuing_HiS; });
                //highScores.IsDisabled = true;

                start.SetDirectionals(null, instructions, null, null);
                cont.SetDirectionals(null, instructions, null, null);
                instructions.SetDirectionals(start, levSel, null, null);
                levSel.SetDirectionals(instructions, extras, null, null);
                extras.SetDirectionals(levSel, quit, null, null);
                extras.SetInternalMenu(new List<MenuButton>() { options, objectives, /*highScores,*/ saves });
                quit.SetDirectionals(extras, null, null, null);
                //levSel.SetDirectionals(instructions, quit, null, null);
                //quit.SetDirectionals(levSel, null, null, null);
                //controlArray.AddRange(new MenuControl[] { instructions, levSel, options, quit });
                controlArray.AddRange(new MenuControl[] { instructions, levSel, extras, quit });
                if(Program.Game.Manager.CurrentSave.CurrentLevel == 1)
                {
                    selectedControl = start;
                    selectedControl.IsSelected = null;
                }
                else
                {
                    selectedControl = cont;
                    cont.IsSelected = null;
                }
            }

            public override void Draw(GameTime gameTime)
            {
                Program.Game.GraphicsDevice.Clear(Color.White);
                if(Program.Game.Loading)
                {
                    if(Program.Game.LoadingScreen != null)
                        Program.Game.LoadingScreen.Draw();
                    else
                    { 
                        MediaSystem.PlayTrack(MediaSystem.SongOptions.Menu);
                        Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                        Program.Game.SpriteBatch.Draw(loader.tbcSplash, new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.5f), null, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.End();
                    }
                }
                else if(Input.MessagePad == null || (startBeenPressed && timer > 0))
                {
                    int time = startBeenPressed ? smallerTime : largerTime;
 
                    Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                    Program.Game.GraphicsDevice.Clear(Color.White);
                    Program.Game.SpriteBatch.Draw(loader.mainMenuBackground, new Vector2(0, 0), null, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    Program.Game.SpriteBatch.Draw(loader.mainMenuLogo, new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.15f), null, Color.White, 0.0f, new Vector2(loader.mainMenuLogo.Width, loader.mainMenuLogo.Height) * 0.5f * Program.Game.TextureScaleFactor, 0.75f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    if(timerInMilliseconds % time <= time * 0.5f)
                    {
                        Vector2 screenSpot = new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.8f);
                        Program.Game.SpriteBatch.Draw(Program.Game.Loader.pressStart, screenSpot, null, Color.White, 0, new Vector2(Program.Game.Loader.pressStart.Width, Program.Game.Loader.pressStart.Height) * 0.5f * Program.Game.TextureScaleFactor,
                            Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                    Program.Game.SpriteBatch.End();
                }
                else
                {
                    if(SaveSelectMenu.Finished)
                    {
                        startBeenPressed = false;
                        Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null);
                        Program.Game.SpriteBatch.Draw(loader.mainMenuBackground, new Vector2(0, 0), null, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.Draw(loader.mainMenuLogo, new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.15f), null, Color.White, 0.0f, new Vector2(loader.mainMenuLogo.Width / 2, loader.mainMenuLogo.Height / 2), 0.75f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        base.Draw(gameTime);
                        Program.Game.SpriteBatch.End();
                    }
                    else
                    {
                        Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                        Program.Game.SpriteBatch.Draw(loader.mainMenuBackground, new Vector2(0, 0), null, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        save.Draw(gameTime);
                        Program.Game.SpriteBatch.End();
                    }
                }
            }

            public override void Update(GameTime gameTime)
            {
                timerInMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                if(startBeenPressed)
                    timer -= gameTime.ElapsedGameTime.Milliseconds;

                if(!Program.Game.Loading)
                {
                    if(Input.MessagePad == null)
                    {
                        #region Player Select and Loading
                        Input.PlayerSelect();
                        if(Input.MessagePad != null)
                        {
                            if(Program.Game.Manager.FullScreen)
                            {
                                Program.Game.GDM.IsFullScreen = true;
                                Program.Game.GDM.ApplyChanges();
                            }
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Box_Success);
                            MouseTempDisabled = true;
                            startBeenPressed = true;
                            timer = 1200;
                            timerInMilliseconds %= smallerTime;
                        }
                        #endregion
                    }
                    else if(Input.MessagePad != null && timer <= 0)
                    {
                        if(SaveSelectMenu.Finished)
                        {
                            if(Program.Game.Manager.CurrentSave.CurrentLevel > 1)
                            {
                                if(!controlArray.Contains(cont))
                                {
                                    if(controlArray.Contains(start))
                                    {
                                        controlArray.Remove(start);
                                        if(!start.IsSelected.HasValue)
                                            cont.IsSelected = null;
                                    }
                                    instructions.SetDirectionals(cont, levSel, null, null);
                                    controlArray.Add(cont);
                                }
                                if(!start.IsSelected.HasValue)
                                {
                                    start.IsSelected = false;
                                    cont.IsSelected = null;
                                    selectedControl = cont;
                                }
                            }
                            else
                            {
                                if(!controlArray.Contains(start))
                                {
                                    if(controlArray.Contains(cont))
                                    {
                                        controlArray.Remove(cont);
                                        if(!cont.IsSelected.HasValue)
                                            start.IsSelected = null;
                                    }
                                    instructions.SetDirectionals(start, levSel, null, null);
                                    controlArray.Add(start);
                                }
                                if(!cont.IsSelected.HasValue)
                                {
                                    cont.IsSelected = false;
                                    start.IsSelected = null;
                                    selectedControl = start;
                                }
                            }
                            if(Input.CheckKeyboardJustPressed(Keys.Escape) || Input.CheckXboxJustPressed(Buttons.B))
                            {
                                ReturnToPressStart();
                                return;
                            }
                            base.Update(gameTime);
                            MenuControl t = extras.Update(selectedControl, gameTime);
                            if(t != null)
                                selectedControl = t;
                        }
                        else
                            save.Update(gameTime);
                    }
                }
            }

            private void ReturnToPressStart()
            {
                Input.NullMessagePad();
                startBeenPressed = false;
                ResetTimer();
                save.Reset();
                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Box_Death);
                Program.Game.Manager.Unload();
            }

            internal void ResetTimer()
            {
                timerInMilliseconds = 0;
            }

            private class SaveSelectMenu : Menu
            {
                protected readonly int rectHeight = 200;
                protected readonly int rectWidth = 450;
                protected bool deleteMenu = false;
                private ConfirmationMenu confirmMenu;
                public static bool Finished { get; protected set; }

                public SaveSelectMenu(Loader l)
                {
                    float x = (640 - (rectWidth / 2f)) * Program.Game.TextureScaleFactor.X;
                    float y = 145 * Program.Game.TextureScaleFactor.Y - (rectHeight * Program.Game.TextureScaleFactor.Y) / 2f;
                    int i = 0;
                    SaveSlotButton b1 = new SaveSlotButton(Program.Game.Manager, 1, l.Font, new Sprite(ref l.SaveSelectorTex, new Vector2(x, y + i++ * ((rectHeight + 5) * Program.Game.TextureScaleFactor.Y)), null, Sprite.RenderPoint.UpLeft));
                    SaveSlotButton b2 = new SaveSlotButton(Program.Game.Manager, 2, l.Font, new Sprite(ref l.SaveSelectorTex, new Vector2(x, y + i++ * ((rectHeight + 5) * Program.Game.TextureScaleFactor.Y)), null, Sprite.RenderPoint.UpLeft));
                    SaveSlotButton b3 = new SaveSlotButton(Program.Game.Manager, 3, l.Font, new Sprite(ref l.SaveSelectorTex, new Vector2(x, y + i++ * ((rectHeight + 5) * Program.Game.TextureScaleFactor.Y)), null, Sprite.RenderPoint.UpLeft));
                    b1.SetDirectionals(null, null, b3, b2);
                    b2.SetDirectionals(null, null, b1, b3);
                    b3.SetDirectionals(null, null, b2, b1);

                    controlArray.AddRange(new[] { b1, b2, b3 });
                    selectedControl = b1;
                    b1.IsSelected = null;

                    confirmMenu = new ConfirmationMenu("Are you sure you want to delete this file?\n          This can't be undone, ever.",
                        delegate
                        {
                            for(i = 0; i < controlArray.Count; i++)
                                if(controlArray[i].IsSelected == null || (controlArray[i].IsSelected.HasValue && controlArray[i].IsSelected.Value))
                                    Program.Game.Manager.Delete(i + 1);
                        });
                }

                public void Reset()
                {
                    Finished = false;
                    controlArray[0].IsSelected = null;
                    controlArray[1].IsSelected = controlArray[2].IsSelected = false;
                    selectedControl = controlArray[0];
                    deleteMenu = false;
                    confirmMenu.Reset();
                    Program.Game.Manager.Unload();
                }

                public override void Draw(GameTime gameTime)
                {
                    base.Draw(gameTime);
                    for(int i = 0; i < controlArray.Count; i++)
                        if((controlArray[i].IsSelected == null || (controlArray[i].IsSelected.HasValue && controlArray[i].IsSelected.Value)) &&
                            Program.Game.Manager.GetSaveSlot(i+1).BeenCreated)
                        {
                            if(Input.ControlScheme == ControlScheme.Keyboard)
                                SymbolWriter.WriteKeyboardIcon(Keys.Back, new Vector2(controlArray[controlArray.Count - 1].Texture.UpperLeft.X, controlArray[controlArray.Count - 1].Texture.LowerRight.Y) +
                                    new Vector2(40, 10) * Program.Game.TextureScaleFactor, true);
                            else
                                SymbolWriter.WriteXboxIcon(Buttons.Back, new Vector2(controlArray[controlArray.Count - 1].Texture.UpperLeft.X, controlArray[controlArray.Count - 1].Texture.LowerRight.Y) +
                                    new Vector2(40, 10) * Program.Game.TextureScaleFactor, true);
                            Program.Game.SpriteBatch.DrawString(loader.SmallerFont, "Delete Save", new Vector2(controlArray[controlArray.Count - 1].Texture.UpperLeft.X, controlArray[controlArray.Count - 1].Texture.LowerRight.Y) +
                                    new Vector2(60, 0) * Program.Game.TextureScaleFactor, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                    if(deleteMenu)
                        confirmMenu.Draw(gameTime);
                }

                public override void Update(GameTime gameTime)
                {
                    if(deleteMenu)
                    {
                        confirmMenu.Update(gameTime);
                        if(confirmMenu.Finished)
                        {
                            confirmMenu.Reset();
                            deleteMenu = false;
                        }
                        return;
                    }
                    if(Input.CheckKeyboardJustPressed(Keys.Back) || Input.CheckXboxJustPressed(Buttons.Back))
                        for(int i = 0; i < controlArray.Count; i++)
                            if((controlArray[i].IsSelected == null || (controlArray[i].IsSelected.HasValue && controlArray[i].IsSelected.Value)) &&
                                    Program.Game.Manager.GetSaveSlot(i + 1).BeenCreated)
                            {
                                deleteMenu = true;
                                confirmMenu.Reset();
                            }

                    base.Update(gameTime);
                }

                private class SaveSlotButton : MenuControl
                {
                    protected new readonly Color DownTint = Color.LawnGreen;
                    protected new readonly Color SelectionTint = new Color(255, 128, 128);
                    protected readonly int saveSlot;
                    protected readonly SpriteFont font;
                    protected readonly IOManager manager;
                    protected readonly Color textColor = Color.LightSlateGray;
                    protected readonly string iString;
                    protected string starString;

                    public SaveSlotButton(IOManager m, int slot, SpriteFont font, Sprite tex)
                        : base(tex, String.Empty, delegate { m.SwitchCurrentSave(slot); Finished = true; })
                    {
                        manager = m;
                        saveSlot = slot;
                        this.font = font;
                        for(int i = 0; i < slot; i++)
                            iString += "I";
                    }

                    public override void Draw(MenuControl selected)
                    {
                        if(IsSelected.HasValue && IsSelected.Value)
                            Texture.Draw(DownTint);
                        else if(!IsSelected.HasValue)
                            Texture.Draw(SelectionTint);
                        else
                            Texture.Draw();
                        float offset = 30;
                        starString = manager.GetSaveSlot(saveSlot).StarNumber.ToString();
                        Rectangle r = new Rectangle((int)(39), (int)(36), (int)(39), (int)(36));
                        Program.Game.SpriteBatch.DrawString(font, "Save Slot " + iString, new Vector2(Texture.UpperLeft.X + offset * Program.Game.TextureScaleFactor.X, Texture.UpperLeft.Y + offset * Program.Game.TextureScaleFactor.Y), textColor, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        if(manager.GetSaveSlot(saveSlot).BeenCreated)
                        {
                            TimeSpan p = manager.GetSaveSlot(saveSlot).Playtime;
                            Program.Game.SpriteBatch.DrawString(font, " : " + starString, new Vector2(Texture.UpperLeft.X + (37.5f + offset) * Program.Game.TextureScaleFactor.X, Texture.LowerRight.Y - offset * Program.Game.TextureScaleFactor.Y), textColor, 0, new Vector2(0, font.MeasureString(" : " + starString).Y * Program.Game.TextureScaleFactor.Y), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            Program.Game.SpriteBatch.Draw(Program.Game.Loader.starTex, new Vector2(Texture.UpperLeft.X + offset * Program.Game.TextureScaleFactor.X, Texture.LowerRight.Y - offset * Program.Game.TextureScaleFactor.Y), r, Color.White, 0, new Vector2(0, r.Width) * Program.Game.TextureScaleFactor, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            Program.Game.SpriteBatch.DrawString(font, "Playtime: " + p.ToString(@"hh\:mm\:ss"), Texture.LowerRight - new Vector2(offset) * Program.Game.TextureScaleFactor, textColor, 0, font.MeasureString("Playtime: " + p.ToString(@"hh\:mm\:ss")), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                            Program.Game.SpriteBatch.DrawString(font, "Current Level: " + manager.GetSaveSlot(saveSlot).CurrentLevel, new Vector2(Texture.LowerRight.X - offset * Program.Game.TextureScaleFactor.X, Texture.UpperLeft.Y + offset * Program.Game.TextureScaleFactor.Y), textColor, 0, new Vector2(font.MeasureString("Current Level: " + manager.CurrentSave.CurrentLevel).X, 0), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        }
                        else
                            Program.Game.SpriteBatch.DrawString(font, "Create New File", Texture.Center, textColor, 0, font.MeasureString("Create New File") * 0.5f * Program.Game.TextureScaleFactor, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
            }
        }
        #endregion

        #region Level Select
        private class LevelSelectMenu : Menu
        {
            Sprite[] glowArray;
            Sprite[] overlayArray;
            Sprite[] activeArray;
            Rectangle[] stars;
            Sprite[] tempStarArray;

            MenuControl level11control;

            VideoPlayer player;
            Video video;

            Sprite box;

            protected bool rebuildStars = false;

            public LevelSelectMenu()
            {
                player = new VideoPlayer();
                video = loader.LevelSelectVideo;
                player.IsLooped = true;
                player.IsMuted = true;
                player.Play(video);

                box = loader.InfoBox;

                MenuButton back, l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, ld1, ld2, ld3;
                back = new MenuButton(loader.backButton, delegate { Program.Game.State = BaseGame.GameState.MainMenu; });
                
                l1 = new MenuButton(loader.iconArray[0], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; Program.Game.LevelNumber = 1; });
                l2 = new MenuButton(loader.iconArray[1], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 2; });
                l3 = new MenuButton(loader.iconArray[2], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 3; });
                l4 = new MenuButton(loader.iconArray[3], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 4; });
                l5 = new MenuButton(loader.iconArray[4], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 5; });
                l6 = new MenuButton(loader.iconArray[5], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 6; });
                l7 = new MenuButton(loader.iconArray[6], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 7; });
                l8 = new MenuButton(loader.iconArray[7], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 8; });
                l9 = new MenuButton(loader.iconArray[8], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 9; });
                l10 = new MenuButton(loader.iconArray[9], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 10; });
                l11 = new MenuButton(loader.iconArray[10], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 11; });
                ld1 = new MenuButton(loader.iconArray[11], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 12; });
                ld2 = new MenuButton(loader.iconArray[12], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 13; });
                ld3 = new MenuButton(loader.iconArray[13], delegate { Program.Game.LevelEnteredFrom = BaseGame.GameState.Menuing_Lev; ; Program.Game.LevelNumber = 14; });

                back.SetDirectionals(null, null, null, l4);
                l1.SetDirectionals(l5, l2, back, l6);
                l2.SetDirectionals(l1, l3, back, l7);
                l3.SetDirectionals(l2, l4, back, l8);
                l4.SetDirectionals(l3, l5, back, l9);
                l5.SetDirectionals(l4, l1, back, l10);
                l6.SetDirectionals(l10, l7, l1, l11);
                l7.SetDirectionals(l6, l8, l2, ld2);
                l8.SetDirectionals(l7, l9, l3, ld3);
                l9.SetDirectionals(l8, l10, l4, ld3);
                l10.SetDirectionals(l9, l6, l5, ld3);
                l11.SetDirectionals(null, null, l6, ld1);
                ld1.SetDirectionals(ld3, ld2, l11, null);
                ld2.SetDirectionals(ld1, ld3, l7, null);
                ld3.SetDirectionals(ld2, ld1, l8, null);
                controlArray.AddRange(new MenuControl[] { back, l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, ld1, ld2, ld3 });
                selectedControl = back;
                selectedControl.IsSelected = null;

                level11control = l11;

                glowArray = loader.selectionGlows;
                overlayArray = loader.lockOverlays;
                activeArray = loader.activeIconArray;
                stars = new Rectangle[2];
                stars[0] = new Rectangle(0, 0, 39, 36);
                stars[1] = new Rectangle(39, 36, 39, 36);
                //stars[1] = new Rectangle(39, 0, 39, 36);
                //stars[2] = new Rectangle(0, 36, 39, 36);
            }

            public override void Update(GameTime gameTime)
            {
                UpdateLockedLevels();
                if(detectKeyboardInput() || detectMouseInput())
                {
                    rebuildStars = true;
                    selectedControl.OnSelect();
                }
            }

            public override void Draw(GameTime gameTime)
            {
                bool done = false;
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                Program.Game.SpriteBatch.Draw(player.GetTexture(), new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), Color.White);

                box.Draw();
                controlArray[0].Draw(selectedControl);

                for(int i = 1; i < controlArray.Count; i++)
                {
                    // The question here is this: Is LevelData[1] level 1 or level 2?

                    done = !controlArray[i].IsDisabled;

                    if(selectedControl == controlArray[i] && done) // if this button is selected...
                    {
                        activeArray[i - 1].Draw();
                        DrawStars(activeArray[i - 1], Program.Game.Manager.CurrentSave.LevelData[i], i);
                        drawInfoBox(i);
                    }
                    else // if it is not...
                    {
                        if(i == 11 & !done)
                            continue;
                        controlArray[i].Texture.Draw(); // Intentionally invoke a non-auto-tinted draw.
                    }
                    if(!done) // and if it's not completed...
                    {
                        if(i == 11)
                            continue; // level 11 is invisible
                        
                        overlayArray[i - 1].Draw();
                    }
                }
                for(int i = 1; i < controlArray.Count; i++) // Has to be done last so the glow draws over everything.
                    if(!controlArray[i].IsDisabled && selectedControl == controlArray[i])
                    {
                        glowArray[i - 1].Draw();
                        break;
                    }
                Program.Game.SpriteBatch.End();
            }

            public void UpdateLockedLevels()
            {
                for(int i = 1; i < controlArray.Count; i++)
                {
#if !DEBUG && !INTERNAL
                    controlArray[i].IsDisabled = !Program.Game.Manager.CurrentSave.LevelData[i].Unlocked;
#elif DEBUG || INTERNAL
                    controlArray[i].IsDisabled = Program.Game.Loader.levelArray[i] == null;
#endif
                }
            }

            private void drawInfoBox(int level)
            {
                string levelText = "\"" + loader.levelArray[level].LevelName.Replace('\n', ' ').Replace("  ", "") + "\'\'";
                if(levelText == "\"\'\'")
                    levelText = "Level " + level;
                Vector2 levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                Vector2 offset = new Vector2(20) * Program.Game.TextureScaleFactor;
                float ySpacing = (box.Height - levelTextSize.Y * 5 - offset.Y * 2) / 4;
                float boxesLength = loader.Font.MeasureString("Least Boxes: ").X * Program.Game.TextureScaleFactor.X;

                Program.Game.SpriteBatch.DrawString(loader.Font, levelText, box.UpperLeft + new Vector2(box.Width * 0.5f, offset.Y),
                    Color.White, 0, new Vector2(levelTextSize.X * 0.5f, 0), (levelTextSize.X > box.Width - offset.X - 3 ? (box.Width - offset.X - 3) / levelTextSize.X : 1) * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                if(level != 11)
                {
                    if(!Program.Game.Manager.CurrentSaveWindowsOptions.HighScoreMode)
                    {
                        Program.Game.SpriteBatch.DrawString(loader.Font, "/", offset + box.UpperLeft + new Vector2(box.Width * 0.55f, ySpacing + levelTextSize.Y), Color.White,
                            0, new Vector2(0, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(loader.Font, "/", offset + box.UpperLeft + new Vector2(box.Width * 0.55f, (ySpacing + levelTextSize.Y) * 2), Color.White,
                            0, new Vector2(0, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(loader.Font, "/", offset + box.UpperLeft + new Vector2(box.Width * 0.55f, (ySpacing + levelTextSize.Y) * 3), Color.White,
                            0, new Vector2(0, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = "Best Time: ";
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, Program.Game.Manager.CurrentSave.LevelData[level].Time.ToString(@"mm\:ss"),
                            offset + box.UpperLeft + new Vector2(boxesLength, ySpacing + levelTextSize.Y * 0.5f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, ySpacing + levelTextSize.Y),
                            Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.Draw(loader.starTex, new Vector2(box.LowerRight.X, box.UpperLeft.Y + ySpacing + levelTextSize.Y * 0.5f) + new Vector2(-offset.X, offset.Y) + new Vector2(-stars[1].Width * Program.Game.TextureScaleFactor.X, ySpacing), Program.Game.Manager.CurrentSave.LevelData[level].TimeStarNumber == LevelSelectData.Stars.Three ? stars[1] : stars[0], Color.White, 0, new Vector2(0, stars[1].Height * 0.5f * Program.Game.TextureScaleFactor.Y), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = loader.levelArray[level].CompletionData.ThreeStarTime.ToString(@"mm\:ss");
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, new Vector2(box.LowerRight.X, box.UpperLeft.Y + ySpacing + levelTextSize.Y * 0.5f) + new Vector2(-offset.X, offset.Y) + new Vector2(-stars[1].Width * Program.Game.TextureScaleFactor.X, ySpacing) - new Vector2(stars[1].Width * 0.5f * Program.Game.TextureScaleFactor.X, 0), Color.White,
                            0, new Vector2(levelTextSize.X, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = "Least Boxes: " + Program.Game.Manager.CurrentSave.LevelData[level].BoxesLost;
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, (ySpacing + levelTextSize.Y) * 2),
                            Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f) * Program.Game.TextureScaleFactor, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.Draw(loader.starTex, new Vector2(box.LowerRight.X, box.UpperLeft.Y + (ySpacing + levelTextSize.Y) * 2 - levelTextSize.Y * 0.5f) + new Vector2(-offset.X, offset.Y) + new Vector2(-stars[1].Width * Program.Game.TextureScaleFactor.X, ySpacing), Program.Game.Manager.CurrentSave.LevelData[level].BoxStarNumber == LevelSelectData.Stars.Three ? stars[1] : stars[0], Color.White, 0, new Vector2(0, stars[1].Height * 0.5f * Program.Game.TextureScaleFactor.Y), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = loader.levelArray[level].CompletionData.ThreeStarBoxes.ToString();
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, new Vector2(box.LowerRight.X, box.UpperLeft.Y + (ySpacing + levelTextSize.Y) * 2 - levelTextSize.Y * 0.5f) + new Vector2(-offset.X, offset.Y) + new Vector2(-stars[1].Width * Program.Game.TextureScaleFactor.X, ySpacing) - new Vector2(stars[1].Width * 0.5f * Program.Game.TextureScaleFactor.X, 0), Color.White,
                            0, new Vector2(levelTextSize.X, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = "Best Score: ";
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, Program.Game.Manager.CurrentSave.LevelData[level].Score.ToString(),
                            offset + box.UpperLeft + new Vector2(boxesLength, (ySpacing + levelTextSize.Y) * 3 - levelTextSize.Y * 0.5f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, (ySpacing + levelTextSize.Y) * 3),
                            Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f) * Program.Game.TextureScaleFactor, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.Draw(loader.starTex, new Vector2(box.LowerRight.X, box.UpperLeft.Y + (ySpacing + levelTextSize.Y) * 3 - levelTextSize.Y * 0.5f) + new Vector2(-offset.X, offset.Y) + new Vector2(-stars[1].Width * Program.Game.TextureScaleFactor.X, ySpacing), Program.Game.Manager.CurrentSave.LevelData[level].ScoreStarNumber == LevelSelectData.Stars.Three ? stars[1] : stars[0], Color.White, 0, new Vector2(0, stars[1].Height * 0.5f * Program.Game.TextureScaleFactor.Y), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = loader.levelArray[level].CompletionData.ThreeStarScore.ToString();
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, new Vector2(box.LowerRight.X, box.UpperLeft.Y + (ySpacing + levelTextSize.Y) * 3 - levelTextSize.Y * 0.5f) + new Vector2(-offset.X, offset.Y) + new Vector2(-stars[1].Width * Program.Game.TextureScaleFactor.X, ySpacing) - new Vector2(stars[1].Width * 0.5f * Program.Game.TextureScaleFactor.X, 0), Color.White,
                            0, new Vector2(levelTextSize.X, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = "Completed:    " + (Program.Game.Manager.CurrentSave.LevelData[level].Completed ? "Yes" : "No");
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, (ySpacing + levelTextSize.Y) * 4),
                            Color.White, 0, new Vector2(0, loader.Font.MeasureString(levelText).Y * 0.5f) * Program.Game.TextureScaleFactor, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                    else
                    {
                        levelText = "Time Taken: ";
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, Program.Game.Manager.CurrentSave.SideBLevelData[level].Time.ToString(@"mm\:ss"),
                            offset + box.UpperLeft + new Vector2(boxesLength, ySpacing + levelTextSize.Y * 0.5f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, ySpacing + levelTextSize.Y),
                            Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = "Boxes Lost:   " + Program.Game.Manager.CurrentSave.SideBLevelData[level].BoxesLost;
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, (ySpacing + levelTextSize.Y) * 2),
                            Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f) * Program.Game.TextureScaleFactor, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                        levelText = "Best Score: ";
                        levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                        Program.Game.SpriteBatch.DrawString(loader.Font, Program.Game.Manager.CurrentSave.SideBLevelData[level].Score.ToString(),
                            offset + box.UpperLeft + new Vector2(boxesLength, (ySpacing + levelTextSize.Y) * 3 - levelTextSize.Y * 0.5f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                        Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, (ySpacing + levelTextSize.Y) * 3),
                            Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f) * Program.Game.TextureScaleFactor, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    }
                }
                else
                {
                    levelText = "Time Taken: ";
                    levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                    Program.Game.SpriteBatch.DrawString(loader.Font, Program.Game.Manager.CurrentSave.SideBLevelData[level].Time.ToString(@"mm\:ss"),
                        offset + box.UpperLeft + new Vector2(boxesLength, ySpacing + levelTextSize.Y * 0.5f), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, ySpacing + levelTextSize.Y),
                        Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                    levelText = "Completed:    " + (Program.Game.Manager.CurrentSave.LevelData[level].Completed ? "Yes" : "No");
                    levelTextSize = loader.Font.MeasureString(levelText) * Program.Game.TextureScaleFactor;
                    Program.Game.SpriteBatch.DrawString(loader.Font, levelText, offset + box.UpperLeft + new Vector2(0, (ySpacing + levelTextSize.Y) * 2),
                        Color.White, 0, new Vector2(0, levelTextSize.Y * 0.5f), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    Program.Game.SpriteBatch.Draw(loader.starTex, new Vector2(box.LowerRight.X, box.UpperLeft.Y + (ySpacing + levelTextSize.Y) * 2 - levelTextSize.Y * 0.5f) + new Vector2(-offset.X, offset.Y) + new Vector2(-stars[1].Width * Program.Game.TextureScaleFactor.X, ySpacing), Program.Game.Manager.CurrentSave.LevelData[11].Completed ? stars[1] : stars[0], Color.White, 0, new Vector2(0, stars[1].Height * 0.5f * Program.Game.TextureScaleFactor.Y), Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }
            }

            private void DrawStars(Sprite centerTex, LevelSelectData starData, int level)
            {
                // time
                // boxes
                // score
                if(level != 11)
                {
                    if(tempStarArray != null)
                        if(centerTex != tempStarArray[3])
                            tempStarArray = null;
                    if(tempStarArray == null || rebuildStars)
                    {
                        tempStarArray = new Sprite[4];
                        float x = centerTex.LowerRight.X + 22.5f * Program.Game.TextureScaleFactor.X;
                        float y = centerTex.Center.Y;
                        tempStarArray[0] = new Sprite(ref loader.starTex, new Vector2(x, y - 36), stars[(int)starData.TimeStarNumber], Sprite.RenderPoint.Center);
                        tempStarArray[1] = new Sprite(ref loader.starTex, new Vector2(x, y), stars[(int)starData.BoxStarNumber], Sprite.RenderPoint.Center);
                        tempStarArray[2] = new Sprite(ref loader.starTex, new Vector2(x, y + 36), stars[(int)starData.ScoreStarNumber], Sprite.RenderPoint.Center);
                        tempStarArray[3] = centerTex;
                        rebuildStars = false;
                    }
                    for(int i = 0; i < tempStarArray.Length; i++)
                        tempStarArray[i].Draw();
                }
                else
                {
                    if(tempStarArray != null)
                        if(centerTex != tempStarArray[3])
                            tempStarArray = null;
                    if(tempStarArray == null || rebuildStars)
                    {
                        tempStarArray = new Sprite[4];
                        float x = centerTex.LowerRight.X + 22.5f * Program.Game.TextureScaleFactor.X;
                        float y = centerTex.Center.Y;
                        tempStarArray[0] = new Sprite(ref loader.starTex, new Vector2(x, y), stars[starData.Completed ? 1 : 0], Sprite.RenderPoint.Center);
                        tempStarArray[3] = centerTex;
                        rebuildStars = false;
                    }
                    tempStarArray[0].Draw();
                }
            }
        }
        #endregion

        #region Exiting
        private class ExitMenu : Menu
        {
            private readonly string exitString = "Are you sure you want to exit?";
            private Vector2 exitStringCenter;
            private readonly SpriteFont font;

            public ExitMenu()
            {
                MenuButton yes, no;
                yes = new MenuButton(loader.yesButton, delegate { Program.Game.Exit(); });
                no = new MenuButton(loader.noButton, delegate { Program.Game.State = Program.Game.PreviousState; });
                yes.SetDirectionals(null, no, null, null);
                no.SetDirectionals(yes, null, null, null);
                selectedControl = yes;
                yes.IsSelected = null;

                controlArray.AddRange(new MenuControl[] { yes, no });

                font = loader.BiggerFont;
                exitStringCenter = font.MeasureString(exitString) * 0.5f;
            }

            public override void Draw(GameTime gameTime)
            {
                if(Program.Game.PreviousState == BaseGame.GameState.MainMenu)
                    mainMenu.Draw(gameTime);
                else
                    Program.Game.DrawLevel(gameTime);

                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                Program.Game.SpriteBatch.Draw(loader.halfBlack, new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), Color.White);
                Program.Game.SpriteBatch.Draw(loader.mainMenuLogo, new Vector2(Program.Game.Width * 0.97f - loader.mainMenuLogo.Width / 2, Program.Game.Height * 0.03f - loader.mainMenuLogo.Height / 2), null, Color.White, 0.0f, new Vector2(loader.mainMenuLogo.Width / 2, loader.mainMenuLogo.Height / 2), 0.3f * Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                Program.Game.SpriteBatch.DrawString(font, exitString, new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.3f), Color.White, 0.0f, exitStringCenter, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                base.Draw(gameTime);

                Program.Game.SpriteBatch.End();
            }
        }
        #endregion

        #region Media
        private class MediaMenu : Menu
        {
            private readonly MenuButton play, stop;
            private Color fill = Color.Transparent;
            private readonly Sprite background;
            private readonly Sprite scissorBackground;
            private Rectangle scissorRectangle;
            private readonly RasterizerState rs = new RasterizerState();
            private readonly SpriteFont font;
            private const byte deltaA = 3;
            private const byte fadeBoundary = 185;
            private string scrollingText = string.Empty;
            private string scrollingTextLastFrame = string.Empty;
            private MenuButton playPause;

            private enum FadeDirection { In, Out, None }
            private FadeDirection fade;
            private bool scrolling = false;
            private Vector2 textPos;
            private int timer;
            private const int waitTime = 1200;

            public string ErrorString { private get; set; }

            private enum MenuState { Normal, Artists, Albums }
            private MenuState state = MenuState.Normal;
            private AlbumCollection albums;
            private ArtistCollection artists;
            private string[] names;
            private float sliderStepDistance;
            private int startingIndex = -3;
            private int selectedIndex = 0;
            private Color backgroundTint = new Color(255, 255, 255, 0);
            private bool holdingEnter = false;
            private bool sliding = false;

            private readonly Texture2D musicTex;
            private readonly Rectangle mainRenderRect = new Rectangle(188, 235, 630, 249);
            private readonly Rectangle sliderRenderRect = new Rectangle(803, 0, 16, 39);
            private const int sliderSlideDistance = 140;
            private readonly Vector2 mainRenderPos;
            private readonly Vector2 sliderBaseRenderPos;
            private readonly Vector2[] textPositions;
            private Vector2 sliderPos;
            private const float epsilon = 0.001f;

            public MediaMenu()
            {
                Texture2D tex = musicTex = loader.MediaTexture;
                font = loader.SmallerFont;
                MenuButton artists, shuffle, albums;
                VariableButton prev, next;
                rs.CullMode = CullMode.None;
                rs.ScissorTestEnable = true;

                Sprite playTex, stopTex, artistsTex, shuffleTex, albumsTex, prevTex, nextTex;
                Texture2D black = new Texture2D(Program.Game.GraphicsDevice, 172, 28);
                Color[] col = new Color[172 * 28];
                for(int i = 0; i < col.Length; i++)
                    col[i] = Color.Black;
                black.SetData(col);

                background = new Sprite(ref tex, new Vector2(Program.Game.Width, Program.Game.Height) * 0.5f, new Rectangle(213, 0, 447, 235), Sprite.RenderPoint.Center);
                scissorBackground = new Sprite(ref black, new Vector2(background.UpperLeft.X + 17 * Program.Game.TextureScaleFactor.X, background.LowerRight.Y - 55 * Program.Game.TextureScaleFactor.Y), null, Sprite.RenderPoint.UpLeft);
                albumsTex = new Sprite(ref tex, background.UpperLeft + new Vector2(205, 24) * Program.Game.TextureScaleFactor, new Rectangle(0, 0, 210, 51), Sprite.RenderPoint.UpLeft);
                artistsTex = new Sprite(ref tex, new Vector2(albumsTex.UpperLeft.X, albumsTex.LowerRight.Y) + new Vector2(0, 11) * Program.Game.TextureScaleFactor, new Rectangle(0, 51, 210, 51), Sprite.RenderPoint.UpLeft);
                shuffleTex = new Sprite(ref tex, new Vector2(artistsTex.UpperLeft.X, artistsTex.LowerRight.Y) + new Vector2(0, 11) * Program.Game.TextureScaleFactor, new Rectangle(0, 102, 210, 51), Sprite.RenderPoint.UpLeft);
                playTex = new Sprite(ref tex, background.UpperLeft + new Vector2(45, 36) * Program.Game.TextureScaleFactor, new Rectangle(0, 154, 112, 112), Sprite.RenderPoint.UpLeft);
                stopTex = new Sprite(ref tex, background.UpperLeft + new Vector2(45, 36) * Program.Game.TextureScaleFactor, new Rectangle(0, 266, 112, 112), Sprite.RenderPoint.UpLeft);
                prevTex = new Sprite(ref tex, background.UpperLeft + new Vector2(17, 10) * Program.Game.TextureScaleFactor, new Rectangle(112, 154, 76, 162), Sprite.RenderPoint.UpLeft);
                nextTex = new Sprite(ref tex, new Vector2(prevTex.LowerRight.X, prevTex.UpperLeft.Y) + new Vector2(12, 0) * Program.Game.TextureScaleFactor, new Rectangle(112, 317, 76, 162), Sprite.RenderPoint.UpLeft);

                play = new MenuButton(playTex, delegate { swapPlayPause(); MediaSystem.StartShuffleCustomMusic(); });
                stop = new MenuButton(stopTex, delegate { swapPlayPause(); MediaSystem.StopCustomMusic(); });
                artists = new MenuButton(artistsTex, doArtistsClick);
                shuffle = new MenuButton(shuffleTex, delegate { MediaSystem.StartShuffleCustomMusic(); });
                albums = new MenuButton(albumsTex, doAlbumsClick);
                prev = new VariableButton(prevTex, delegate { MediaSystem.MovePrevious(); }, string.Empty);
                next = new VariableButton(nextTex, delegate { MediaSystem.MoveNext(); }, string.Empty);
                play.MakeTransparencySensitive();
                stop.MakeTransparencySensitive();
                prev.MakeTransparencySensitive();
                next.MakeTransparencySensitive();

                prev.SetPointerDirectionals(null, new Pointer<MenuControl>(() => playPause, v => { }), null, null);
                next.SetPointerDirectionals(new Pointer<MenuControl>(() => playPause, v => { }), new Pointer<MenuControl>(() => artists, v => { }), null, null);
                play.SetDirectionals(prev, next, null, null);
                stop.SetDirectionals(prev, next, null, null);
                albums.SetDirectionals(next, prev, shuffle, artists);
                artists.SetDirectionals(next, prev, albums, shuffle);
                shuffle.SetDirectionals(next, prev, artists, albums);

                mainRenderPos = new Vector2(BaseGame.PreferredScreenWidth - mainRenderRect.Width, BaseGame.PreferredScreenHeight - mainRenderRect.Height) * 0.5f;
                sliderPos = sliderBaseRenderPos = mainRenderPos + new Vector2(578, 36);
                textPositions = new Vector2[7];
                textPositions[0] = mainRenderPos + new Vector2(46, 40);
                for(int i = 1; i < textPositions.Length; i++)
                    textPositions[i] = textPositions[i - 1] + new Vector2(0, 23);

                Program.Game.GraphicsDevice.DeviceReset += onGDMReset;
                startDarkFade();
                scissorRectangle = new Rectangle((int)scissorBackground.UpperLeft.X, (int)scissorBackground.UpperLeft.Y, (int)scissorBackground.Width, (int)scissorBackground.Height);

                selectedControl = playPause = play;
                selectedControl.IsSelected = null;
                controlArray.AddRange(new[] { play, prev, next, artists, shuffle, albums });
                scrollingText = "Playing: (nothing)";
                textPos = scissorBackground.UpperLeft + new Vector2(0, (scissorBackground.Height - font.MeasureString(scrollingText).Y) / 2);
                ErrorString = "";
                MediaPlayer.ActiveSongChanged += onSongChanged;
            }

            private void startDarkFade()
            {
                fade = FadeDirection.In;
            }

            private void startFadeBack()
            {
                fade = FadeDirection.Out;
            }

            private void swapPlayPause()
            {
                if(controlArray.Contains(play))
                {
                    selectedControl = playPause = stop;
                    if(play.IsSelected == null)
                        stop.IsSelected = null;
                    play.IsSelected = false;
                    controlArray.Remove(play);
                    controlArray.Add(stop);
                }
                else if(controlArray.Contains(stop))
                {
                    selectedControl = playPause = play;
                    if(stop.IsSelected == null)
                        play.IsSelected = null; 
                    stop.IsSelected = false;
                    controlArray.Remove(stop); 
                    controlArray.Add(play);
                }
            }

            private void onGDMReset(object sender, EventArgs e)
            {
                scissorBackground.ForceResize();
                scissorRectangle = new Rectangle((int)scissorBackground.UpperLeft.X, (int)scissorBackground.UpperLeft.Y, (int)scissorBackground.Width, (int)scissorBackground.Height);
            }

            public override void Draw(GameTime gameTime)
            {
                if(Program.Game.PreviousState == BaseGame.GameState.Running)
                    Program.Game.DrawLevel(gameTime);

                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
                Program.Game.SpriteBatch.Draw(loader.EmptyTex, new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), fill);
                if(fade != FadeDirection.None)
                {
                    Program.Game.SpriteBatch.End();
                    return;
                }

                scissorBackground.Draw();
                if(scrolling)
                {
                    RasterizerState prev = Program.Game.GraphicsDevice.RasterizerState;
                    Rectangle r = Program.Game.GraphicsDevice.ScissorRectangle;
                    Program.Game.GraphicsDevice.RasterizerState = rs;
                    Program.Game.GraphicsDevice.ScissorRectangle = scissorRectangle;
                    Program.Game.SpriteBatch.DrawString(font, scrollingText, textPos, Color.SlateGray);
                    Program.Game.GraphicsDevice.RasterizerState = prev;
                    Program.Game.GraphicsDevice.ScissorRectangle = r;
                }
                else // just draw normally
                    Program.Game.SpriteBatch.DrawString(font, scrollingText, textPos, Color.SlateGray);

                background.Draw();
                base.Draw(gameTime);

                if(state != MenuState.Normal || backgroundTint.A > 0)
                {
                    Color selectionTint = Color.Red;
                    selectionTint.A = backgroundTint.A;
                    Color downTint = new Color(180, 0, 0, backgroundTint.A);

                    Program.Game.SpriteBatch.Draw(musicTex, new Vector2(Program.Game.Width, Program.Game.Height) * 0.5f, mainRenderRect, backgroundTint,
                        0, new Vector2(mainRenderRect.Width, mainRenderRect.Height) * 0.5f, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                    if(sliding)
                        Program.Game.SpriteBatch.Draw(musicTex, sliderPos, sliderRenderRect, downTint,
                            0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    else if(Input.CheckMouseWithinCoords(sliderPos, sliderPos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height) * Program.Game.TextureScaleFactor))
                        Program.Game.SpriteBatch.Draw(musicTex, sliderPos, sliderRenderRect, selectionTint,
                            0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                    else
                        Program.Game.SpriteBatch.Draw(musicTex, sliderPos, sliderRenderRect, backgroundTint,
                            0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                    int step = 0;
                    if(startingIndex < 0)
                        step = -startingIndex; // step is now positive
                    else if(startingIndex > names.Length - 7)
                        step = -(names.Length - startingIndex);
                    for(int i = startingIndex + (step < 0 ? 0 : step); i < (startingIndex + (step < 0 ? -step : 7)); i++)
                        Program.Game.SpriteBatch.DrawString(loader.Font, names[i], textPositions[i - startingIndex] * Program.Game.TextureScaleFactor,
                            i == selectedIndex ? (holdingEnter ? downTint : selectionTint) : backgroundTint, 0,
                            Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }

                Program.Game.SpriteBatch.End();
            }

            public override void Update(GameTime gameTime)
            {
                if(state == MenuState.Normal)
                {
                    if(backgroundTint.A > 0)
                    {
                        if(backgroundTint.A - deltaA * 2 < 0)
                        {
                            backgroundTint.A = 0;
                            albums = null;
                            artists = null;
                            names = null;
                            sliding = false;
                            holdingEnter = false;
                            startingIndex = -3;
                            selectedIndex = 0;
                        }
                        else
                            backgroundTint.A -= deltaA * 2;
                    }

                    if(ErrorString != string.Empty)
                    {
                        scrollingText = ErrorString;
                        ErrorString = string.Empty;
                    }

                    if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MusicKey) || Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.MusicKey))
                        startFadeBack();

                    #region Fading
                    if(fade == FadeDirection.In)
                    {
                        fill.A += deltaA;
                        if(fill.A >= fadeBoundary)
                            fade = FadeDirection.None;
                    }
                    else if(fade == FadeDirection.Out)
                    {
                        if(fill.A - deltaA > 0)
                            fill.A -= deltaA;
                        else
                            fill.A = 0;
                        if(fill.A == 0)
                        {
                            fade = FadeDirection.In;
                            MediaSystem.PlayAll();
                            Program.Game.State = Program.Game.PreviousState;
                        }
                        else
                            return;
                    }
                    #endregion
                }
                else
                {
                    if(backgroundTint.A < 255)
                    {
                        if(backgroundTint.A + deltaA * 2 > 255)
                            backgroundTint.A = 255;
                        else
                            backgroundTint.A += deltaA * 2;
                    }
                    else
                    {
                        #region keyboard input
                        if(Input.CheckXboxPress(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey) ||
                            Input.CheckKeyboardPress(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey))
                        {
                            if(names[selectedIndex] != "(Empty)")
                            {
                                switch(state)
                                {
                                    case MenuState.Albums:
                                        playAlbum(albums[selectedIndex]); // this should work
                                        break;
                                    case MenuState.Artists:
                                        playArtist(artists[selectedIndex]);
                                        break;
                                }
                                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                                holdingEnter = false;
                            }
                        }
                        if(!holdingEnter || sliding)
                        {
                            if(Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey) ||
                                Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey))
                            {
                                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                                holdingEnter = true;
                                MouseTempDisabled = true;
                            }

                            else if(Input.CheckXboxJustPressed(Buttons.Back) || Input.CheckKeyboardJustPressed(Keys.Escape))
                            {
                                state = MenuState.Normal;
                                MouseTempDisabled = true;
                            }
                            else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuUpKey) ||
                                Input.CheckXboxJustPressed(Buttons.LeftThumbstickUp))
                            {
                                if(selectedIndex > 0)
                                {
                                    selectedIndex--;
                                    startingIndex--;
                                    if(names[selectedIndex] == "(Empty)")
                                    {
                                        selectedIndex++;
                                        startingIndex++;
                                    }
                                    else
                                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                                }
                                MouseTempDisabled = true;
                            }
                            else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuDownKey) ||
                                Input.CheckXboxJustPressed(Buttons.LeftThumbstickDown))
                            {
                                if(selectedIndex < names.Length - 1)
                                {
                                    startingIndex++;
                                    selectedIndex++;
                                    if(names[selectedIndex] == "(Empty)")
                                    {
                                        selectedIndex--;
                                        startingIndex--;
                                    }
                                    else
                                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                                }
                                MouseTempDisabled = true;
                            }
                            else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey) ||
                                Input.CheckXboxJustPressed(Buttons.LeftThumbstickLeft))
                            {
                                if(selectedIndex > 0)
                                {
                                    startingIndex -= 7;
                                    selectedIndex -= 7;
                                    if(selectedIndex < 0)
                                        selectedIndex = 0;
                                    if(startingIndex < -3)
                                        startingIndex = -3;
                                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                                }
                                MouseTempDisabled = true;
                            }
                            else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey) ||
                                Input.CheckXboxJustPressed(Buttons.LeftThumbstickRight))
                            {
                                if(selectedIndex < names.Length - 1)
                                {
                                    startingIndex += 7;
                                    selectedIndex += 7;
                                    if(selectedIndex > names.Length - 1)
                                        selectedIndex = names.Length - 1;
                                    if(startingIndex > names.Length - 4)
                                        startingIndex = names.Length - 4;
                                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                                }
                                MouseTempDisabled = true;
                            }
                        }
                        #endregion

                        if(sliding && MouseTempDisabled)
                            MouseTempDisabled = false;

                        #region slider
                        if(Input.CheckMouseWithinCoords(sliderPos * Program.Game.TextureScaleFactor, (sliderPos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height)) * Program.Game.TextureScaleFactor) &&
                            !Input.CheckMouseLastFrameWithinCoords(sliderPos * Program.Game.TextureScaleFactor, (sliderPos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height)) * Program.Game.TextureScaleFactor))
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                        if(!sliding && Input.CheckMouseWithinCoords(sliderPos, sliderPos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height)) &&
                            Input.MouseState.LeftButton == ButtonState.Pressed)
                        {
                            sliding = true;
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                        }
                        if(Input.MouseState.LeftButton != ButtonState.Pressed && sliding)
                        {
                            sliding = false;
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                        }
                        if(!sliding)
                        {
                            sliderPos.Y = (sliderBaseRenderPos.Y + selectedIndex * sliderStepDistance) * Program.Game.TextureScaleFactor.Y;
                            if(sliderPos.Y < sliderBaseRenderPos.Y)
                                sliderPos.Y = sliderBaseRenderPos.Y * Program.Game.TextureScaleFactor.Y;
                            else if(sliderPos.Y > sliderBaseRenderPos.Y + sliderSlideDistance)
                                sliderPos.Y = (sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y;
                        }
                        else
                        {
                            if(Input.MouseState.Y > sliderBaseRenderPos.Y * Program.Game.TextureScaleFactor.Y &&
                                Input.MouseState.Y < (sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y)
                                sliderPos.Y = Input.MouseState.Y;
                            else if(Input.MouseState.Y < sliderBaseRenderPos.Y * Program.Game.TextureScaleFactor.Y)
                                sliderPos.Y = sliderBaseRenderPos.Y * Program.Game.TextureScaleFactor.Y;
                            else
                                sliderPos.Y = (sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y;

                            if(Math.Abs(sliderPos.Y - sliderBaseRenderPos.Y * Program.Game.TextureScaleFactor.Y) < epsilon)
                                startingIndex = -3;
                            else if(Math.Abs(sliderPos.Y - ((sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y)) < epsilon)
                                startingIndex = names.Length - 4;
                            else
                            {
                                int i;
                                float step = sliderStepDistance * Program.Game.TextureScaleFactor.Y;
                                float top = sliderBaseRenderPos.Y * Program.Game.TextureScaleFactor.Y;
                                for(i = 0; i < names.Length - 2; i++)
                                {
                                    if(i * step + top < Input.MouseState.Y && (i + 1) * step + top > Input.MouseState.Y)
                                    {
                                        float upper = (i * step + top) - Input.MouseState.Y;
                                        float lower = Input.MouseState.Y - ((i + 1) * step + top);
                                        if(lower > upper)
                                            i = i + 1;
                                        break;
                                    }
                                }
                                startingIndex = i - 3;
                            }

                            selectedIndex = startingIndex + 3;
                        }
                        #endregion

                        #region mouse
                        if(!MouseTempDisabled)
                        {
                            if(!sliding && !holdingEnter)
                            {
                                Vector2 upperLeftBlack = (mainRenderPos + new Vector2(44, 110)) * Program.Game.TextureScaleFactor;
                                Vector2 lowerRightBlack = (mainRenderPos + new Vector2(572, 130)) * Program.Game.TextureScaleFactor;
                                if(Input.CheckMouseWithinCoords(upperLeftBlack, lowerRightBlack) && Input.CheckMouseJustClicked())
                                {
                                    holdingEnter = true;
                                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                                }

                                int step = Input.MouseState.ScrollWheelValue - Input.MouseLastFrame.ScrollWheelValue;
                                if(step != 0)
                                {
                                    step = Math.Sign(step);
                                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                                }
                                startingIndex -= step;
                                if(startingIndex < -3)
                                    startingIndex = -3;
                                if(startingIndex > names.Length - 4)
                                    startingIndex = names.Length - 4;
                                selectedIndex = startingIndex + 3;
                            }
                            if(holdingEnter && Input.MouseState.LeftButton == ButtonState.Released && Input.MouseLastFrame.LeftButton == ButtonState.Pressed)
                            {
                                if(names[selectedIndex] != "(Empty)")
                                {
                                    switch(state)
                                    {
                                        case MenuState.Albums:
                                            playAlbum(albums[selectedIndex]); // this should work
                                            break;
                                        case MenuState.Artists:
                                            playArtist(artists[selectedIndex]);
                                            break;
                                    }
                                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                                    holdingEnter = false;
                                }
                            }
                        }
                        #endregion
                    }
                }

                // update scrolling text
                scrolling = scrollingText != null && font.MeasureString(scrollingText).X > scissorBackground.Width;
                if(scrolling)
                {
                    if(timer < waitTime)
                        timer += gameTime.ElapsedGameTime.Milliseconds;
                    else
                        textPos.X -= 1f;
                }
                if(font.MeasureString(scrollingText).X + textPos.X <= scissorBackground.UpperLeft.X)
                    textPos.X = scissorBackground.LowerRight.X;

                if(fade == FadeDirection.None && state == MenuState.Normal && backgroundTint.A == 0) // stop updating the buttons when we're selecting other stuff
                    base.Update(gameTime);
            }

            private void onSongChanged(object sender, EventArgs e)
            {
                timer = 0;
                scrollingText = "Playing: " + MediaSystem.GetPlayingSong();
                textPos = scissorBackground.UpperLeft + new Vector2(0, (scissorBackground.Height - font.MeasureString(scrollingText).Y) / 2);
            }

            private void doArtistsClick()
            {
                ArtistCollection artists = MediaSystem.GetArtistsInLibrary();
                if(artists.Count == 0)
                {
                    scrollingText = "No artists found. Try running Windows Media Player to create the required library.";
                    textPos = scissorBackground.UpperLeft + new Vector2(0, (scissorBackground.Height - font.MeasureString(scrollingText).Y) / 2);
                    return;
                }

                this.artists = artists;
                names = new string[artists.Count];
                int i = 0;
                foreach(Artist a in artists)
                    names[i++] = a.Name;
                if(names.Length < 7)
                    for(i = 6 - names.Length; i < 6; i++)
                        names[i] = "(Empty)";

                float maxWidth = 523 * Program.Game.TextureScaleFactor.X;
                for(i = 0; i < names.Length; i++)
                {
                    string temp = names[i];
                    while(loader.Font.MeasureString(temp).X * Program.Game.TextureScaleFactor.X > maxWidth)
                        temp = temp.Substring(0, temp.Length - 5) + "..."; // chop off last 4 characters and add "..."
                    names[i] = temp;
                }

                sliderStepDistance = ((float)sliderSlideDistance / (names.Length - 4)) * Program.Game.TextureScaleFactor.Y;
                sliderPos = sliderBaseRenderPos * Program.Game.TextureScaleFactor;

                state = MenuState.Artists;
            }

            private void doAlbumsClick()
            {
                AlbumCollection albums = MediaSystem.GetAlbumsInLibrary();
                if(albums.Count == 0)
                {
                    scrollingText = "No albums found. Try running Windows Media Player to create the required library.";
                    textPos = scissorBackground.UpperLeft + new Vector2(0, (scissorBackground.Height - font.MeasureString(scrollingText).Y) / 2);
                    return;
                }

                this.albums = albums;
                names = new string[albums.Count];
                int i = 0;
                foreach(Album a in albums)
                    names[i++] = a.Name;
                if(names.Length < 7)
                    for(i = 6 - names.Length; i < 6; i++)
                        names[i] = "(Empty)";

                float maxWidth = 523 * Program.Game.TextureScaleFactor.X;
                for(i = 0; i < names.Length; i++)
                {
                    string temp = names[i];
                    while(loader.Font.MeasureString(temp).X * Program.Game.TextureScaleFactor.X > maxWidth)
                        temp = temp.Substring(0, temp.Length - 5) + "..."; // chop off last 4 characters and add "..."
                    names[i] = temp;
                }

                sliderStepDistance = ((float)sliderSlideDistance / (names.Length - 4)) * Program.Game.TextureScaleFactor.Y;
                sliderPos = sliderBaseRenderPos * Program.Game.TextureScaleFactor;

                state = MenuState.Albums;
            }

            private void playArtist(Artist a)
            {
                MediaSystem.StartArtistsCustomMusic(a);
                state = MenuState.Normal;
                swapPlayPause();
            }

            private void playAlbum(Album a)
            {
                MediaSystem.StartAlbumCustomMusic(a);
                state = MenuState.Normal;
                swapPlayPause();
            }
        }
        #endregion

        #region Ending
        private class EndingMenu : Menu
        {
            protected Sprite[] credits;
            protected float alpha;
            protected float time;
            protected bool moved;

            protected const float deltaA = 3;
            protected const float secondsToPause = 3;

            protected readonly Texture2D gradient;

            protected Rectangle topR;
            protected Rectangle bottomR;

            public EndingMenu()
            {
                credits = loader.Credits;
                gradient = new Texture2D(Program.Game.GraphicsDevice, 1, 20);
                Color[] colors = new Color[20];
                for(int i = 0; i < 5; i++)
                    colors[i] = new Color(0, 0, 0, 255);
                for(int i = 5; i < 20; i++)
                    colors[i] = new Color(0, 0, 0, 255 - ((255 / 15) * (i - 4)));
                gradient.SetData(colors);
            }

            public override void Update(GameTime gameTime)
            {
                if(!credits[0].Moving && !moved)
                {
                    moved = true;
                    credits[0].Move(new Vector2(0, -0.55f), 115);
                    credits[1].Move(new Vector2(0, -0.55f), 115);
                    topR = new Rectangle(0, 0, (int)Program.Game.Width, 20);
                    bottomR = new Rectangle(0, (int)Program.Game.Height - 20, (int)Program.Game.Width, 20);
                }

                for(int i = 0; i < credits.Length - 1; i++)
                    credits[i].ForceMoveUpdate(gameTime);

                if(!credits[0].Moving)
                {
                    if(alpha + deltaA > 255)
                    {
                        alpha = 255;
                        time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(time >= secondsToPause)
                            reset();
                    }
                    else
                        alpha += deltaA;
                }
            }

            public override void Draw(GameTime gameTime)
            {
                Program.Game.SpriteBatch.Begin();
                for(int i = 0; i < credits.Length - 1; i++)
                    credits[i].Draw();
                credits[credits.Length - 1].Draw(new Color(255, 255, 255, alpha) * (alpha / 255f));

                Program.Game.SpriteBatch.Draw(gradient, topR, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                Program.Game.SpriteBatch.Draw(gradient, bottomR, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                

                Program.Game.SpriteBatch.End();
            }

            protected void reset()
            {
                time = alpha = 0;
                moved = false;
                foreach(Sprite s in credits)
                    s.Reset();
            }
        }
        #endregion

        #region Confirmation Menu
        private class ConfirmationMenu : Menu
        {
            /// <summary>
            /// This will be true if the user selects no.
            /// </summary>
            public bool Finished { get; private set; }

            private readonly Vector2 confirmStringCenter;
            private readonly SpriteFont font;
            private readonly string confirmString;

            /// <summary>
            /// Creates a confirmation menu.
            /// </summary>
            /// <param name="confirmationString">The string to prompt the user with.</param>
            /// <param name="onYes">The delegate to perform if the user selects yes.</param>
            public ConfirmationMenu(string confirmationString, Action onYes)
            {
                MenuButton yes, no;
                yes = new MenuButton(loader.yesButton, onYes + delegate { Finished = true; });
                no = new MenuButton(loader.noButton, delegate { Finished = true; });
                yes.SetDirectionals(null, no, null, null);
                no.SetDirectionals(yes, null, null, null);
                selectedControl = no;
                no.IsSelected = null;

                controlArray.AddRange(new MenuControl[] { yes, no });

                font = loader.BiggerFont;
                confirmString = confirmationString;
                confirmStringCenter = font.MeasureString(confirmString) * 0.5f;
            }

            public override void Draw(GameTime gameTime)
            {
                Program.Game.SpriteBatch.Draw(loader.halfBlack, new Rectangle(0, 0, (int)Program.Game.Width, (int)Program.Game.Height), Color.White);
                Program.Game.SpriteBatch.DrawString(font, confirmString, new Vector2(Program.Game.Width * 0.5f, Program.Game.Height * 0.3f), Color.White, 0.0f, confirmStringCenter, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                base.Draw(gameTime);
            }

            public void Reset()
            {
                Finished = false;
                selectedControl = controlArray[1];
                controlArray[1].IsSelected = null;
                controlArray[0].IsSelected = false;
            }
        }
        #endregion

        #region Achievements
        private class AchievementMenu : Menu
        {
            private Color backgroundTint = new Color(255, 255, 255, 255);
            private const float epsilon = 0.001f;

            private bool sliding = false;
            private Texture2D background;
            private Vector2 sliderPos; // relative to basePos, scaled, set in code
            private readonly Rectangle sliderRenderRect = new Rectangle(1258, 0, 22, 60);
            private readonly Rectangle mainRenderRect = new Rectangle(0, 0, 1248, 502);

            private int startingIndex = -2;
            private int selectedIndex = 0;
            //private bool holdingEnter = false;
            private List<Accomplishment> achs;

            private Vector2 sliderBaseRenderPos = new Vector2(1176, 43); // relative to basePos, unscaled
            private float sliderStepDistance; // scaled, set in code
            private const float sliderSlideDistance = 356; // constant, unscaled

            private readonly Vector2 basePos = new Vector2(21, 110); // constant, unscaled

            private HelpfulTextBox[] boxes; // positions set in deviceReset()

            public AchievementMenu()
            {
                background = loader.AchievementMenuTexture;
                boxes = new HelpfulTextBox[5];
                Program.Game.Manager.OnSaveChanged += rebuildList;
                Program.Game.GDM.DeviceReset += GDM_DeviceReset;
                rebuildList();
                GDM_DeviceReset(this, EventArgs.Empty);
            }

            private void GDM_DeviceReset(object sender, EventArgs e)
            {
                if(boxes[0] == null)
                    for(int i = 0; i < boxes.Length; i++)
                        boxes[i] = new HelpfulTextBox(new Rectangle(), loader.Font);

                for(int i = 0; i < boxes.Length; i++)
                {
                    boxes[i].SetSpace(new Rectangle((int)((basePos.X + 130) * Program.Game.TextureScaleFactor.X), (int)((basePos.Y + (52 + 81 * i)) * Program.Game.TextureScaleFactor.Y), 
                        (int)(1020 * Program.Game.TextureScaleFactor.X), (int)(61 * Program.Game.TextureScaleFactor.Y)));
                }

                sliderPos = sliderBaseRenderPos * Program.Game.TextureScaleFactor;
            }

            public override void Draw(GameTime gameTime)
            {
                Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

                Color selectionTint = Color.Red;
                selectionTint.A = backgroundTint.A;
                Color downTint = new Color(180, 0, 0, backgroundTint.A);
                Color greyTint = Color.DarkGray;
                greyTint.A = backgroundTint.A;
                
                Program.Game.SpriteBatch.Draw(background, basePos * Program.Game.TextureScaleFactor, mainRenderRect, backgroundTint,
                        0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                if(sliding)
                    Program.Game.SpriteBatch.Draw(background, sliderPos + basePos * Program.Game.TextureScaleFactor, sliderRenderRect, downTint,
                        0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                else if(Input.CheckMouseWithinCoords(sliderPos + basePos * Program.Game.TextureScaleFactor, sliderPos + (basePos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height)) * Program.Game.TextureScaleFactor))
                    Program.Game.SpriteBatch.Draw(background, sliderPos + basePos * Program.Game.TextureScaleFactor, sliderRenderRect, selectionTint,
                        0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                else
                    Program.Game.SpriteBatch.Draw(background, sliderPos + basePos * Program.Game.TextureScaleFactor, sliderRenderRect, backgroundTint,
                        0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);

                int step = 0;
                if(startingIndex < 0)
                    step = -startingIndex; // step is now positive
                else if(startingIndex > achs.Count - 5)
                    step = -(achs.Count - startingIndex);
                int simple = 0;
                if(startingIndex < 0)
                    simple -= startingIndex;
                for(int i = startingIndex + (step < 0 ? 0 : step); i < (startingIndex + (step < 0 ? -step : 5)); i++, simple++)
                {
                    if(achs[i].Icon != null)
                        Program.Game.SpriteBatch.Draw(achs[i].Completed? achs[i].Icon : Program.Game.Loader.AchievementLockedTexture, (basePos + new Vector2(45, 38 + 85 * simple)) * Program.Game.TextureScaleFactor, achs[i].Completed ? (Rectangle?)achs[i].RenderRectangle : null,
                            achs[i].Completed ? backgroundTint : greyTint, 0, Vector2.Zero, Program.Game.TextureScaleFactor * 0.78125f * 0.75f, SpriteEffects.None, 0);
                    
                    boxes[simple].SetTextColor(i == selectedIndex ? selectionTint : backgroundTint);
                    boxes[simple].Draw(achs[i].Title + " (Progress: " + (achs[i].Completed ? "Complete!) " : (achs[i].Max == 1 ? "Incomplete" : achs[i].Current + "/" + achs[i].Max) + ") ") + (achs[i].Completed ? achs[i].Text : achs[i].Hint));
                    
                    //Program.Game.SpriteBatch.DrawString(loader.Font, achs[i], textPositions[i - startingIndex] * Program.Game.TextureScaleFactor,
                    //    i == selectedIndex ? selectionTint : backgroundTint, 0,
                    //    Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                }

                string s = "To return, press       "
#if WINDOWS
                    + " or click the RMB.";
#else
                    + ".";
#endif
                Vector2 dim = loader.Font.MeasureString(s) * Program.Game.TextureScaleFactor;
                Vector2 firstPart = loader.Font.MeasureString("To return, press ") * Program.Game.TextureScaleFactor;

                Program.Game.SpriteBatch.DrawString(loader.Font, s, (basePos + new Vector2(mainRenderRect.Width, mainRenderRect.Height + 20)) * Program.Game.TextureScaleFactor - new Vector2(dim.X, 0),
                    Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                if(Input.ControlScheme == ControlScheme.Keyboard)
                    SymbolWriter.WriteKeyboardIcon(Keys.Escape, 
                        (basePos + new Vector2(mainRenderRect.Width + 20, mainRenderRect.Height + 35)) * Program.Game.TextureScaleFactor - dim + firstPart, true);
                else
                    SymbolWriter.WriteXboxIcon(Buttons.Back, 
                        (basePos + new Vector2(mainRenderRect.Width + 20, mainRenderRect.Height + 35)) * Program.Game.TextureScaleFactor - dim + firstPart, true);

                Program.Game.SpriteBatch.End();
            }

            public override void Update(GameTime gameTime)
            {
                #region keyboard input
                //if(Input.CheckXboxPress(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey) ||
                //    Input.CheckKeyboardPress(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey))
                //{
                //    if(achs[selectedIndex] != "(Empty)")
                //    {
                //        switch(state)
                //        {
                //            case MenuState.Albums:
                //                playAlbum(albums[selectedIndex]); // this should work
                //                break;
                //            case MenuState.Artists:
                //                playArtist(artists[selectedIndex]);
                //                break;
                //        }
                //        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                //        holdingEnter = false;
                //    }
                //}
                //if(!holdingEnter || sliding)
                if(Input.CheckXboxJustPressed(Buttons.Back) || Input.CheckKeyboardJustPressed(Keys.Escape))
                {
                    Program.Game.State = BaseGame.GameState.MainMenu;
                    MouseTempDisabled = true;
                }
                    //if(Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey) ||
                    //    Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey))
                    //{
                    //    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                    //    holdingEnter = true;
                    //    MouseTempDisabled = true;
                    //}

                if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuUpKey) ||
                    Input.CheckXboxJustPressed(Buttons.LeftThumbstickUp))
                {
                    if(selectedIndex > 0)
                    {
                        selectedIndex--;
                        startingIndex--;
                        //if(names[selectedIndex] == "(Empty)")
                        //{
                        //    selectedIndex++;
                        //    startingIndex++;
                        //}
                        //else
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                    }
                    MouseTempDisabled = true;
                }
                else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuDownKey) ||
                    Input.CheckXboxJustPressed(Buttons.LeftThumbstickDown))
                {
                    if(selectedIndex < achs.Count - 1)
                    {
                        startingIndex++;
                        selectedIndex++;
                        //if(names[selectedIndex] == "(Empty)")
                        //{
                        //    selectedIndex--;
                        //    startingIndex--;
                        //}
                        //else
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                    }
                    MouseTempDisabled = true;
                }
                else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey) ||
                    Input.CheckXboxJustPressed(Buttons.LeftThumbstickLeft))
                {
                    if(selectedIndex > 0)
                    {
                        startingIndex -= 5;
                        selectedIndex -= 5;
                        if(selectedIndex < 0)
                            selectedIndex = 0;
                        if(startingIndex < -2)
                            startingIndex = -2;
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                    }
                    MouseTempDisabled = true;
                }
                else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey) ||
                    Input.CheckXboxJustPressed(Buttons.LeftThumbstickRight))
                {
                    if(selectedIndex < achs.Count - 1)
                    {
                        startingIndex += 5;
                        selectedIndex += 5;
                        if(selectedIndex > achs.Count - 1)
                            selectedIndex = achs.Count - 1;
                        if(startingIndex > achs.Count - 3)
                            startingIndex = achs.Count - 3;
                        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                    }
                    MouseTempDisabled = true;
                }
                #endregion

                if(sliding && MouseTempDisabled)
                    MouseTempDisabled = false;

                #region slider
                if(!sliding && Input.CheckMouseWithinCoords(sliderPos + basePos * Program.Game.TextureScaleFactor, sliderPos + (basePos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height)) * Program.Game.TextureScaleFactor) &&
                    !Input.CheckMouseLastFrameWithinCoords(sliderPos + basePos * Program.Game.TextureScaleFactor, sliderPos + (basePos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height)) * Program.Game.TextureScaleFactor))
                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                if(!sliding && Input.CheckMouseWithinCoords(sliderPos + basePos * Program.Game.TextureScaleFactor, sliderPos + (basePos + new Vector2(sliderRenderRect.Width, sliderRenderRect.Height)) * Program.Game.TextureScaleFactor) &&
                    Input.MouseState.LeftButton == ButtonState.Pressed)
                {
                    sliding = true;
                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                }
                if(Input.MouseState.LeftButton != ButtonState.Pressed && sliding)
                {
                    sliding = false;
                    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                }
                if(!sliding)
                {
                    sliderPos.Y = (sliderBaseRenderPos.Y + selectedIndex * sliderStepDistance) * Program.Game.TextureScaleFactor.Y - sliderRenderRect.Height * 0.5f * Program.Game.TextureScaleFactor.Y;
                    if(sliderPos.Y < (sliderBaseRenderPos.Y) * Program.Game.TextureScaleFactor.Y)
                        sliderPos.Y = (sliderBaseRenderPos.Y) * Program.Game.TextureScaleFactor.Y;
                    else if(sliderPos.Y > (sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y)
                        sliderPos.Y = (sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y;
                }
                else
                {
                    if(Input.MouseState.Y > (sliderBaseRenderPos.Y + basePos.Y) * Program.Game.TextureScaleFactor.Y &&
                        Input.MouseState.Y < (sliderBaseRenderPos.Y + basePos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y)
                        sliderPos.Y = Input.MouseState.Y - basePos.Y * Program.Game.TextureScaleFactor.Y - sliderRenderRect.Height * 0.5f * Program.Game.TextureScaleFactor.Y;
                    else if(Input.MouseState.Y < (sliderBaseRenderPos.Y + basePos.Y) * Program.Game.TextureScaleFactor.Y)
                        sliderPos.Y = (sliderBaseRenderPos.Y) * Program.Game.TextureScaleFactor.Y;
                    else
                        sliderPos.Y = (sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y;

                    if(Math.Abs(sliderPos.Y - (sliderBaseRenderPos.Y) * Program.Game.TextureScaleFactor.Y) < epsilon)
                        startingIndex = -2;
                    else if(Math.Abs(sliderPos.Y - ((sliderBaseRenderPos.Y + sliderSlideDistance) * Program.Game.TextureScaleFactor.Y)) < epsilon)
                        startingIndex = achs.Count - 3;
                    else
                    {
                        int i;
                        float step = sliderStepDistance * Program.Game.TextureScaleFactor.Y;
                        float top = (sliderBaseRenderPos.Y + basePos.Y) * Program.Game.TextureScaleFactor.Y;
                        for(i = 0; i < achs.Count - 2; i++)
                        {
                            if(i * step + top < Input.MouseState.Y && (i + 1) * step + top > Input.MouseState.Y)
                            {
                                float upper = (i * step + top) - Input.MouseState.Y;
                                float lower = Input.MouseState.Y - ((i + 1) * step + top);
                                if(lower > upper)
                                    i = i + 1;
                                break;
                            }
                        }
                        startingIndex = i - 2;
                    }

                    selectedIndex = startingIndex + 2;
                }
                #endregion

                #region mouse
                if(!MouseTempDisabled)
                {
                    if(!sliding)// && !holdingEnter)
                    {
                        //Vector2 upperLeftBlack = (mainRenderPos + new Vector2(44, 110)) * Program.Game.TextureScaleFactor;
                        //Vector2 lowerRightBlack = (mainRenderPos + new Vector2(572, 130)) * Program.Game.TextureScaleFactor;
                        //if(Input.CheckMouseWithinCoords(upperLeftBlack, lowerRightBlack) && Input.CheckMouseJustClicked())
                        //{
                        //    holdingEnter = true;
                        //    MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                        //}

                        if(Input.CheckMouseJustClicked(2))
                        {
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                        }
                        if(Input.CheckForMouseJustReleased(2))
                        {
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                            Program.Game.State = BaseGame.GameState.MainMenu;
                            return;
                        }

                        int step = Input.MouseState.ScrollWheelValue - Input.MouseLastFrame.ScrollWheelValue;
                        if(step != 0)
                        {
                            step = Math.Sign(step);
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                        }
                        startingIndex -= step;
                        if(startingIndex < -2)
                            startingIndex = -2;
                        if(startingIndex > achs.Count - 3)
                            startingIndex = achs.Count - 3;
                        selectedIndex = startingIndex + 2;
                    }
                    //if(holdingEnter && Input.MouseState.LeftButton == ButtonState.Released && Input.MouseLastFrame.LeftButton == ButtonState.Pressed)
                    //{
                    //    if(achs[selectedIndex] != "(Empty)")
                    //    {
                    //        switch(state)
                    //        {
                    //            case MenuState.Albums:
                    //                playAlbum(albums[selectedIndex]); // this should work
                    //                break;
                    //            case MenuState.Artists:
                    //                playArtist(artists[selectedIndex]);
                    //                break;
                    //        }
                    //        MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                    //        holdingEnter = false;
                    //    }
                    //}
                }
                #endregion
            }

            private void rebuildList()
            {
                achs = Accomplishment.GetAccomplishmentList(Program.Game.Manager.CurrentSaveNumber); 

                sliderStepDistance = ((float)sliderSlideDistance / (achs.Count - 3)) * Program.Game.TextureScaleFactor.Y;
            }
        }
        #endregion

        #region Controls
        private class DropUpMenuControl : MenuControl
        {
            protected readonly List<MenuButton> controls = new List<MenuButton>();
            protected const float dropTime = 1.1f;
            protected const int deltaA = 4;

            public bool IsActive { get; private set; }
            protected bool isFading;
            protected int alpha;

            protected Vector2 lowerRight { get { return Texture.LowerRight; } }
            protected Vector2 upperLeft { get { if(controls.Count > 0) return controls[controls.Count - 1].Texture.UpperLeft; return Texture.UpperLeft; } }

            protected Pointer<MenuControl> onLeft, onUp, onRight, onDown;
            protected MenuControl selectedLastFrame;
            protected MenuControl selectedLastFrameMouse;
            protected bool playedClickSound = false;

            public override bool? IsSelected
            {
                get { return base.IsSelected; } 
                set 
                {
                    if((value == null && IsActive && MouseTempDisabled) || // we need to close
                        (!MouseTempDisabled && IsActive && value.HasValue && value == false))
                    {
                        moveTextures(false);
                        IsActive = false;
                    }
                    else if(value == null && !IsActive)
                        this.invoke();
                    else if(value.HasValue && value.Value)
                        value = null;
                    base.IsSelected = value; 
                }
            }

            public override MenuControl OnUp
            {
                get
                {
                    MouseTempDisabled = true;
                    if(IsActive && !isFading)
                        return controls[0];
                    else if(!IsActive)
                        this.invoke();
                    return null;
                }
                protected set { onUp.Value = value; }
            }

            public override MenuControl OnDown
            {
                get
                {
                    if(IsActive)
                    {
                        moveTextures(false);
                        IsActive = false;
                    }
                    if(onDown != null)
                        return onDown.Value;
                    return null;
                }
                protected set { onDown.Value = value; }
            }
            public override MenuControl OnLeft
            {
                get
                {
                    if(IsActive)
                    {
                        moveTextures(false);
                        IsActive = false;
                    }
                    if(onLeft != null)
                        return onLeft.Value;
                    return null;
                }
                protected set { onLeft.Value = value; }
            }
            public override MenuControl OnRight
            {
                get
                {
                    if(IsActive)
                    {
                        moveTextures(false);
                        IsActive = false;
                    }
                    if(onRight != null)
                        return onRight.Value;
                    return null;
                }
                protected set { onRight.Value = value; }
            }

            public override Action OnSelect 
            { 
                get 
                {
                    if(!IsActive)
                        return this.invoke;
                    else return delegate
                    {
                        IsActive = false;
                        moveTextures(false);
                    };
                } 
                protected set { } 
            }

            public DropUpMenuControl(Sprite texture)
            {
                Texture = texture;
                this.HelpfulText = String.Empty;
                IsSelected = false;
            }

            public DropUpMenuControl(Sprite texture, string helpfulText)
                :this(texture)
            {
                HelpfulText = helpfulText;
            }

            /// <summary>
            /// The first will be the closest to the parent, and so on.
            /// </summary>
            /// <param name="controls">They don't need to have their directionals set. They should also have the same
            /// original position and HelpfulText (if applicable) as the parent.</param>
            public void SetInternalMenu(IList<MenuButton> controls)
            {
                this.controls.Clear();
                this.controls.AddRange(controls);
                for(int i = 0; i < controls.Count - 1; i++)
                    this.controls[i].SetDirectionals(null, null, this.controls[i + 1], (i == 0 ? this as MenuControl : this.controls[i - 1]));
                this.controls[this.controls.Count - 1].SetDirectionals(null, null, null, this.controls[this.controls.Count - 2]);
            }

            /// <summary>
            /// Updates the control and its children. Automatically performs necessary mouse input for its children
            /// (the controls' own CheckMouseInput must still be called for it to update itself).
            /// </summary>
            /// <param name="selected">The currently selected control.</param>
            /// <param name="gameTime">The gameTime.</param>
            /// <returns>A reference to one of its children if one is selected and null if it is selected.</returns>
            public MenuControl Update(MenuControl selected, GameTime gameTime)
            {
                if(isFading)
                {
                    if(!IsActive)
                    {
                        alpha -= deltaA;
                        if(alpha < 0)
                        {
                            alpha = 0;
                            isFading = false;
                        }
                    }
                    else
                    {
                        alpha += deltaA;
                        if(alpha > 255)
                        {
                            alpha = 255;
                            isFading = false;
                        }
                    }
                }

                bool selectedWasChanged = false;

                if(IsActive || isFading)
                    foreach(MenuControl c in controls)
                        c.Texture.ForceMoveUpdate(gameTime);
                if(IsActive && alpha >= 215)
                {
                    foreach(MenuButton m in controls)
                    {
                        if(MouseTempDisabled)
                            break;

                        bool? old = m.IsSelected;
                        bool? current = m.CheckMouseInput(selected);
                        if((current == null || (current.HasValue && current.Value)) && m != selectedLastFrameMouse)
                        {
                            selected = m;
                            selectedLastFrameMouse = m;
                            selectedWasChanged = true;
                            current = m.CheckMouseInput(selected);
                        }

                        if(old.HasValue && !old.Value && !current.HasValue)
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Rollover);
                        else if(!old.HasValue && current.HasValue && current.Value)
                        {
                            if(!playedClickSound)
                                MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Press);
                            playedClickSound = true;
                        }
                        else if(old.HasValue && old.Value && !current.HasValue && Input.MouseState.LeftButton == ButtonState.Released)
                        {
                            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Button_Release);
                            m.OnSelect();
                            playedClickSound = false;
                            return null; // necessary?
                        }
                        else if(Input.MouseState.LeftButton == ButtonState.Released)
                            playedClickSound = false;
                    }
                }
                else if(!IsActive)
                {
                    playedClickSound = false;
                    foreach(MenuButton b in controls)
                        b.IsSelected = false;
                    if(selected == this || controls.Contains(selected))
                    {
                        base.IsSelected = null;
                        return this;
                    }
                }

                if(selectedWasChanged)
                    return selected;

                foreach(MenuControl c in controls)
                    if(!c.IsSelected.HasValue || (c.IsSelected.HasValue && c.IsSelected.Value))
                    {
                        selectedLastFrameMouse = c;
                        return c;
                    }
                return null;
            }

            public override void Draw(MenuControl selected)
            {
                if(IsActive || isFading)
                    foreach(MenuControl c in controls)
                        c.Draw(selected, new Color(255, 255, 255, alpha));
                base.Draw(selected);
            }

            public void SetPointerDirectionals(Pointer<MenuControl> left, Pointer<MenuControl> right,
                Pointer<MenuControl> up, Pointer<MenuControl> down)
            {
                onLeft = left;
                onRight = right;
                onUp = up;
                onDown = down;
            }

            public override void SetDirectionals(MenuControl left, MenuControl right, MenuControl up, MenuControl down)
            {
                if(left != null)
                    onLeft = new Pointer<MenuControl>(() => left, v => { });
                else
                    onLeft = null;

                if(right != null)
                    onRight = new Pointer<MenuControl>(() => right, v => { });
                else
                    onRight = null;

                if(up != null)
                    onUp = new Pointer<MenuControl>(() => up, v => { });
                else
                    onUp = null;

                if(down != null)
                    onDown = new Pointer<MenuControl>(() => down, v => { });
                else
                    onDown = null;
            }

            protected void invoke()
            {
                IsActive = true;
                moveTextures(true);
            }

            protected void moveTextures(bool forward)
            {
                isFading = true;
                if(forward)
                {
                    float offset = Texture.Height * Program.Game.TextureScaleFactor.Y * 0.1f; // tenth of texture height
                    Vector2 temp = (controls[0].Texture.Point == Sprite.RenderPoint.Center ? Texture.Center : Texture.UpperLeft) - new Vector2(0, offset + controls[0].Texture.Height);
                    controls[0].Texture.MoveTo(temp, dropTime);
                    for(int i = 1; i < controls.Count; i++)
                    {
                        temp = (controls[i].Texture.Point == Sprite.RenderPoint.Center ? Texture.Center : Texture.UpperLeft) - new Vector2(0, (offset + controls[i - 1].Texture.Height) * (i + 1));
                        controls[i].Texture.MoveTo(temp, dropTime);
                    }
                }
                else
                    foreach(MenuControl c in controls)
                        c.Texture.MoveTo(c.Texture.Point == Sprite.RenderPoint.Center ? Texture.Center : Texture.UpperLeft,
                           dropTime);
            }

            public override bool? CheckMouseInput(MenuControl selected)
            {
                bool eligible = Program.Game.IsActive && !MouseTempDisabled && !IsDisabled;
                if(!eligible)
                    return IsSelected;

                if(!Input.CheckMouseWithinCoords(upperLeft, lowerRight) && IsActive)
                {
                    IsActive = false;
                    moveTextures(false);
                }
                else if(Input.CheckMouseWithinCoords(upperLeft, lowerRight) && !IsActive && alpha > 0)
                {
                    IsActive = true;
                    moveTextures(true);
                }

                bool withinCoords = Input.CheckMouseWithinCoords(Texture);
                if(withinCoords && eligible && !IsActive)// && this != selectedLastFrame)
                {
                    selectedLastFrame.IsSelected = false;
                    base.IsSelected = null;
                    invoke();
                }
                else if((withinCoords && IsActive) || this == selected)
                    base.IsSelected = null;
                else if(!withinCoords && IsActive)
                    base.IsSelected = false;
                else
                    base.IsSelected = false;

                selectedLastFrame = selected;
                return IsSelected;
            }

            public void SetNewPosition(Vector2 pos)
            {
                Texture.TeleportTo(pos);
                foreach(MenuControl c in controls)
                    c.Texture.TeleportTo(pos);
            }
        }

        private class DualValueControl<T> : GreedyControl<T>
        {
            protected T value1, value2;

            protected Action drawV1, drawV2;

            /// <summary>
            /// Creates a control that can only hold two pre-provided values.
            /// </summary>
            /// <param name="t">The texture to use.</param>
            /// <param name="text"></param>
            /// <param name="textV"></param>
            /// <param name="font"></param>
            /// <param name="variable"></param>
            /// <param name="value1"></param>
            /// <param name="value2"></param>
            /// <param name="drawValue1">The function to execute if the value of the variable is value1.</param>
            /// <param name="drawValue2">The function to execute if the value of the variable is value2.</param>
            public DualValueControl(Sprite t, string text, Vector2 textV, SpriteFont font, Pointer<T> variable,
                T value1, T value2, Action drawValue1, Action drawValue2)
                :base(variable, t, text, textV, font)
            {
                this.value1 = value1;
                this.value2 = value2;
                drawV1 = drawValue1;
                drawV2 = drawValue2;
                HelpfulText = "Press %s% or click the box to toggle between values.";
            }

            public override void Draw(MenuControl selectedControl)
            {
                if(variable.Value.Equals(value1))
                    drawV1();
                else 
                    drawV2();
                base.Draw(selectedControl);
            }

            protected override void invoke()
            {
                if(IsSelected.HasValue && IsSelected.Value)
                    return; // cause we're still holding the button down.

                if(variable.Value.Equals(value1))
                    variable.Value = value2;
                else 
                    variable.Value = value1;
                IsActive = false;
            }
        }

        #region TabControl
        //private class TabControl : MenuControl
        //{
        //    private readonly Color darkenFactor = new Color(158, 158, 158);

        //    public MenuControl[] Controls { get; private set; }
        //    private bool darken = true;

        //    public TabControl(SuperTextor tabTexture, string tooltip)
        //        : base(tabTexture, tooltip, delegate { })
        //    {
        //        HelpfulText = "Use %lr% or rollover the desired tab to toggle between them.";
        //    }

        //    public void SetData(MenuControl[] controls)
        //    {
        //        Controls = controls;
        //    }

        //    public bool? CheckMouseInput(MenuControl selected, TabControl currentTab)
        //    {
        //        darken = !(this == currentTab);
        //        return base.CheckMouseInput(selected);
        //    }

        //    public override bool? CheckMouseInput(MenuControl selected)
        //    {
        //        throw new NotImplementedException("This method is not supported by TabControl.");
        //    }

        //    public override void Draw(MenuControl selected)
        //    {
        //        if(!IsSelected.HasValue || (IsSelected.HasValue && IsSelected.Value))
        //            Texture.Draw(darken ? Color.Lerp(SelectionTint, darkenFactor, 1) : SelectionTint);
        //        else
        //            Texture.Draw(darken ? darkenFactor : Color.White);
        //    }
        //}
#endregion

        private class ButtonControl : GreedyControl<Buttons>
        {
            protected bool allowDuplicates;

            public ButtonControl(Sprite borderTex, bool allowDupes, Vector2 stringV, string text, 
                SpriteFont font, Pointer<Buttons> variable)
                : base(variable, borderTex, text, stringV, font)
            {
                allowDuplicates = allowDupes;
                HelpfulText = "Press %s% or click the box and then press the desired Xbox button. Press %b% to cancel.";
            }

            protected override void invoke()
            {
                if(IsSelected.HasValue && IsSelected.Value)
                    return; // cause we're still holding the button down.

                IsActive = true;

                if(Input.CheckKeyboardJustPressed(Keys.Escape) || Input.CheckXboxJustPressed(Buttons.Back))
                {
                    IsActive = false;
                    return;
                }

                Buttons? pressedKey = getFirstButton(Input.CurrentPad);
                if(pressedKey.HasValue)
                {
                    Program.Game.Manager.CurrentSaveXboxOptions.Swap(variable, pressedKey.Value);
                    IsActive = false;
                }
            }

            protected Buttons? getFirstButton(GamePadState thisFrame)
            {
                if(thisFrame.Buttons.A == ButtonState.Pressed)
                    return Buttons.A;
                if(thisFrame.Buttons.B == ButtonState.Pressed)
                    return Buttons.B;
                if(thisFrame.Buttons.LeftShoulder == ButtonState.Pressed)
                    return Buttons.LeftShoulder;
                if(thisFrame.Buttons.LeftStick == ButtonState.Pressed)
                    return Buttons.LeftStick;
                if(thisFrame.Buttons.RightShoulder == ButtonState.Pressed)
                    return Buttons.RightShoulder;
                if(thisFrame.Buttons.RightStick == ButtonState.Pressed)
                    return Buttons.RightStick;
                if(thisFrame.Buttons.Start == ButtonState.Pressed)
                    return Buttons.Start;
                if(thisFrame.Buttons.X == ButtonState.Pressed)
                    return Buttons.X;
                if(thisFrame.Buttons.Y == ButtonState.Pressed)
                    return Buttons.Y;
                if(thisFrame.DPad.Down == ButtonState.Pressed)
                    return Buttons.DPadDown;
                if(thisFrame.DPad.Up == ButtonState.Pressed)
                    return Buttons.DPadUp;
                if(thisFrame.DPad.Left == ButtonState.Pressed)
                    return Buttons.DPadLeft;
                if(thisFrame.DPad.Right == ButtonState.Pressed)
                    return Buttons.DPadRight;
                if(thisFrame.Triggers.Left > 0.75f)
                    return Buttons.LeftTrigger;
                if(thisFrame.Triggers.Right > 0.75f)
                    return Buttons.RightTrigger;
                if(thisFrame.ThumbSticks.Left.X > 0.75f)
                    return Buttons.LeftThumbstickRight;
                if(thisFrame.ThumbSticks.Left.Y > 0.75f)
                    return Buttons.LeftThumbstickUp;
                if(thisFrame.ThumbSticks.Left.X < -0.75f)
                    return Buttons.LeftThumbstickLeft;
                if(thisFrame.ThumbSticks.Left.Y < -0.75f)
                    return Buttons.LeftThumbstickDown;
                if(thisFrame.ThumbSticks.Right.X > 0.75f)
                    return Buttons.RightThumbstickRight;
                if(thisFrame.ThumbSticks.Right.Y > 0.75f)
                    return Buttons.RightThumbstickUp;
                if(thisFrame.ThumbSticks.Right.X < -0.75f)
                    return Buttons.RightThumbstickLeft;
                if(thisFrame.ThumbSticks.Right.Y < -0.75f)
                    return Buttons.RightThumbstickDown;
                return null;
            }

            public override void Draw(MenuControl selectedControl)
            {
                SymbolWriter.WriteXboxIcon(variable.Value, Texture.Center, true);
                base.Draw(selectedControl);
            }
        }

        private class KeyControl : GreedyControl<Keys>
        {
            private readonly bool allowDuplicates;

            public KeyControl(Sprite borderTex, bool allowDuplicates, Vector2 stringV, string text, 
                SpriteFont font, Pointer<Keys> variable)
                : base(variable, borderTex, text, stringV, font)
            {
                this.allowDuplicates = allowDuplicates;
                HelpfulText = "Press %s% or click the box and then press the desired keyboard key. Press %b% to cancel selection of a new key.";
            }

            protected override void invoke()
            {
                if(IsSelected.HasValue && IsSelected.Value)
                    return; // cause we're still holding the button down.

                IsActive = true;

                if(Input.CheckKeyboardJustPressed(Keys.Escape) || Input.CheckXboxJustPressed(Buttons.Back))
                {
                    IsActive = false;
                    return;
                }

                Keys[] pressedKeys = Input.KeyboardState.GetPressedKeys();
                if(pressedKeys.Length > 0)
                {
                    Keys firstPressedKey = pressedKeys[0];
                    if(Program.Game.Manager.CurrentSaveWindowsOptions.DetermineIfSupported(firstPressedKey) &&
                        !Program.Game.Manager.CurrentSaveWindowsOptions.FindDuplicateKeys(firstPressedKey, allowDuplicates))
                        variable.Value = firstPressedKey;

                    IsActive = false;
                }
            }

            public override void Draw(MenuControl selected)
            {
                base.Draw(selected);
                SymbolWriter.WriteKeyboardIcon(variable.Value, Texture.Center, true);
            }
        }

        /// <summary>
        /// This technically isn't a GreedyControl because it doesn't command attention, but oh well.
        /// </summary>
        private class ToggleControl : GreedyControl<bool>
        {
            private readonly Sprite checkmark;
            private Action optionalAction;

            public ToggleControl(Sprite checkmark, Sprite border, Vector2 textV, string text, SpriteFont font,
                Pointer<bool> variable, string helpfulText)
                : base(variable, border, text, textV, font)
            {
                this.checkmark = checkmark;
                HelpfulText = helpfulText;
            }

            protected override void invoke()
            {
                if((IsSelected.HasValue && IsSelected.Value) || IsDisabled)
                    return; // cause we're still holding the button down.

                IsActive = false;
                variable.Value = !variable.Value;

                if(optionalAction != null)
                    optionalAction();
            }

            public void SetAction(Action a)
            {
                optionalAction = a;
            }

            public override void Draw(MenuControl selected)
            {
                base.Draw(selected);
                if(variable.Value)
                    checkmark.Draw();
            }

            public override MenuControl OnDown { get { if(onDown != null) return onDown.Value; return null; } protected set { if(onDown != null) onDown.Value = value; onDown = new Pointer<MenuControl>(() => value, v => { value = v; }); OnDown = value; } }
            protected Pointer<MenuControl> onDown;
            public override MenuControl OnUp { get { if(onUp != null) return onUp.Value; return null; } protected set { if(onUp != null) onUp.Value = value; onUp = new Pointer<MenuControl>(() => value, v => { value = v; }); OnUp = value; } }
            protected Pointer<MenuControl> onUp;
            public override MenuControl OnLeft { get { if(onLeft != null) return onLeft.Value; return null; } protected set { if(onLeft != null) onLeft.Value = value; onLeft = new Pointer<MenuControl>(() => value, v => { value = v; }); OnLeft = value; } }
            protected Pointer<MenuControl> onLeft;
            public override MenuControl OnRight { get { if(onRight != null) return onRight.Value; return null; } protected set { if(onRight != null) onRight.Value = value; onRight = new Pointer<MenuControl>(() => value, v => { value = v; }); OnRight = value; } }
            protected Pointer<MenuControl> onRight;
            /// <summary>
            /// Call this instead of the other one.
            /// </summary>
            /// <param name="onL"></param>
            /// <param name="onR"></param>
            /// <param name="onU"></param>
            /// <param name="onD"></param>
            public void SetPointerDirectionals(Pointer<MenuControl> onL, Pointer<MenuControl> onR, Pointer<MenuControl> onU, Pointer<MenuControl> onD)
            {
                onLeft = onL;
                onRight = onR;
                onUp = onU;
                onDown = onD;
            }

            public override void SetDirectionals(MenuControl left, MenuControl right, MenuControl up, MenuControl down)
            {
                throw new NotSupportedException("Cannot be called on this object.");
            }
        }

        #region WindowBox
        //private class WindowBox : GreedyControl<Enum>
        //{
        //    private readonly Color darkenFactor = new Color(158, 158, 158);

        //    private readonly Texture2D internalTexture;
        //    private readonly SuperTextor leftArrow, rightArrow;
        //    private Rectangle frameWindow;
        //    private readonly int numberOfFrames;
        //    private readonly Vector2 framePos;
        //    private readonly int baseWidth;

        //    private const int deltaX = 5;

        //    private bool waiting;
        //    private bool drawRightDown;
        //    private bool drawLeftDown;
        //    private bool drawArrowLighter;

        //    private bool justInvoked = true;

        //    private bool goingLeft, goingRight;

        //    private readonly List<Enum> values;

        //    /// <summary>
        //    /// If this is true, you need to feed this control invocations.
        //    /// </summary>
        //    public override bool IsActive { get { return goingLeft || goingRight || waiting; } protected set { waiting = false; goingRight = false; goingLeft = false; justInvoked = true; } }

        //    public WindowBox(SuperTextor frameTex, SuperTextor leftArrow, SuperTextor rightArrow, Texture2D internalTex, Vector2 windowVector, Rectangle window, Vector2 textVector, string text, SpriteFont font,
        //        Pointer<Enum> variable, int numValues, string tooltip)
        //        : base(variable, frameTex, text, textVector, font, tooltip)
        //    {
        //        internalTexture = internalTex;
        //        frameWindow = window;
        //        framePos = windowVector;
        //        this.leftArrow = leftArrow;
        //        this.rightArrow = rightArrow;
        //        baseWidth = frameWindow.Width;
        //        numberOfFrames = numValues;

        //        if(leftArrow.UpperLeft.X > upperLeft.X)
        //            upperLeft.X = leftArrow.UpperLeft.X;
        //        if(rightArrow.UpperLeft.X > upperLeft.X)
        //            upperLeft.X = leftArrow.UpperLeft.X;
        //        if(leftArrow.UpperLeft.Y > upperLeft.Y)
        //            upperLeft.Y = leftArrow.UpperLeft.Y;
        //        if(rightArrow.UpperLeft.Y > upperLeft.Y)
        //            upperLeft.Y = rightArrow.UpperLeft.Y;

        //        values = new List<Enum>();

        //        foreach(object o in Enum.GetValues(variable.Value.GetType()))
        //            values.Add((Enum)o);

        //        HelpfulText = "Press %s% and use %lr% to adjust, then press %s% again to confirm, or click the arrows with the mouse.";
        //    }

        //    protected override void invoke()
        //    {
        //        if(justInvoked)
        //        {
        //            waiting = true;
        //            justInvoked = false;
        //        }

        //        bool rightDown, leftDown;
        //        rightDown = (Input.KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey) || Input.CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.MenuRightKey | Program.Game.Manager.CurrentSaveXboxOptions.CameraRightKey));
        //        leftDown = (Input.KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey) || Input.CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.MenuLeftKey | Program.Game.Manager.CurrentSaveXboxOptions.CameraLeftKey));

        //        if(leftDown && !goingRight)
        //            goingLeft = true;
        //        else if(rightDown && !goingLeft)
        //            goingRight = true;
        //        else if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey) || Input.CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey)
        //            && !goingRight && !goingLeft)
        //        {
        //            IsActive = false;
        //            return;
        //        }

        //        if(rightDown && waiting && !(IsSelected.HasValue && !IsSelected.Value))
        //            drawRightDown = drawArrowLighter = true;
        //        else if(!rightDown && waiting && !(IsSelected.HasValue && !IsSelected.Value))
        //        {
        //            drawRightDown = true;
        //            drawArrowLighter = false;
        //        }
        //        else
        //            drawRightDown = drawArrowLighter = false;

        //        if(leftDown && waiting && !(IsSelected.HasValue && !IsSelected.Value))
        //            drawLeftDown = drawArrowLighter = true;
        //        else if(!leftDown && waiting && !(IsSelected.HasValue && !IsSelected.Value))
        //        {
        //            drawLeftDown = true;
        //            drawArrowLighter = false;
        //        }
        //        else
        //            drawLeftDown = drawArrowLighter = false;

        //        if(goingLeft)
        //            frameWindow.X -= deltaX;
        //        else if(goingRight)
        //            frameWindow.X += deltaX;

        //        if(frameWindow.X < 0)
        //        {
        //            frameWindow.X = 0;
        //            IsActive = false;
        //        }
        //        else if(frameWindow.X > baseWidth * (numberOfFrames - 1))
        //        {
        //            frameWindow.X = baseWidth * numberOfFrames;
        //            IsActive = false;
        //        }
        //        if(frameWindow.X % baseWidth == 0 && (goingLeft || goingRight))
        //        {
        //            variable.Value = values[frameWindow.X / baseWidth];
        //            goingLeft = goingRight = false;
        //            loader.RebuildLevelTiming();
        //        }
        //    }

        //    public override void Draw(MenuControl selected)
        //    {
        //        Program.Game.SpriteBatch.Draw(internalTexture, framePos, frameWindow, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
        //        Program.Game.SpriteBatch.DrawString(font, controlText, textVector, Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
        //        if(IsSelected.HasValue && !IsSelected.Value)
        //            Texture.Draw(darkenFactor);
        //        else if(!IsSelected.HasValue)
        //            Texture.Draw();
        //        else if(IsActive)
        //            Texture.Draw(SelectionTint);
        //        else
        //            Texture.Draw(darkenFactor);

        //        if(drawRightDown && drawArrowLighter)
        //            rightArrow.Draw(SelectionTint);
        //        else if(drawRightDown && !drawArrowLighter)
        //            rightArrow.Draw(Color.Lerp(SelectionTint, darkenFactor, 1));
        //        else if(!drawRightDown && drawArrowLighter)
        //            rightArrow.Draw();
        //        else
        //            rightArrow.Draw(darkenFactor);

        //        if(drawLeftDown && drawArrowLighter)
        //            leftArrow.Draw(SelectionTint);
        //        else if(drawLeftDown && !drawArrowLighter)
        //            leftArrow.Draw(Color.Lerp(SelectionTint, darkenFactor, 1));
        //        else if(!drawLeftDown && drawArrowLighter)
        //            leftArrow.Draw();
        //        else
        //            leftArrow.Draw(darkenFactor);
        //    }

        //    public override MenuControl OnDown { get { if(onDown != null) return onDown.Value; return null; } protected set { if(onDown != null) onDown.Value = value; onDown = new Pointer<MenuControl>(() => value, v => { value = v; }); OnDown = value; } }
        //    protected Pointer<MenuControl> onDown;
        //    public override MenuControl OnUp { get { if(onUp != null) return onUp.Value; return null; } protected set { if(onUp != null) onUp.Value = value; onUp = new Pointer<MenuControl>(() => value, v => { value = v; }); OnUp = value; } }
        //    protected Pointer<MenuControl> onUp;
        //    public override MenuControl OnLeft { get { if(onLeft != null) return onLeft.Value; return null; } protected set { if(onLeft != null) onLeft.Value = value; onLeft = new Pointer<MenuControl>(() => value, v => { value = v; }); OnLeft = value; } }
        //    protected Pointer<MenuControl> onLeft;
        //    public override MenuControl OnRight { get { if(onRight != null) return onRight.Value; return null; } protected set { if(onRight != null) onRight.Value = value; onRight = new Pointer<MenuControl>(() => value, v => { value = v; }); OnRight = value; } }
        //    protected Pointer<MenuControl> onRight;
        //    /// <summary>
        //    /// Call this instead of the other one.
        //    /// </summary>
        //    /// <param name="onL"></param>
        //    /// <param name="onR"></param>
        //    /// <param name="onU"></param>
        //    /// <param name="onD"></param>
        //    public void SetPointerDirectionals(Pointer<MenuControl> onL, Pointer<MenuControl> onR, Pointer<MenuControl> onU, Pointer<MenuControl> onD)
        //    {
        //        onLeft = onL;
        //        onRight = onR;
        //        onUp = onU;
        //        onDown = onD;
        //    }

        //    public override void SetDirectionals(MenuControl left, MenuControl right, MenuControl up, MenuControl down)
        //    {
        //        throw new NotSupportedException("Cannot be called on this object.");
        //    }

        //    public override bool? CheckMouseInput(MenuControl selected)
        //    {
        //        if(Input.CheckMouseWithinCoords(leftArrow))
        //        {
        //            if(Input.MouseState.LeftButton == ButtonState.Pressed && !goingLeft)
        //            {
        //                waiting = false;
        //                goingLeft = true;
        //                drawLeftDown = true;
        //                drawArrowLighter = true;
        //                IsSelected = true; 
        //                justInvoked = true;
        //            }
        //            else
        //            {
        //                drawLeftDown = false;
        //                drawArrowLighter = true;
        //                IsSelected = null; 
        //                justInvoked = true;
        //            }
        //        }
        //        else if(Input.CheckMouseWithinCoords(rightArrow))
        //        {
        //            if(Input.MouseState.LeftButton == ButtonState.Pressed && !goingRight)
        //            {
        //                waiting = false;
        //                justInvoked = true;
        //                goingRight = true;
        //                drawRightDown = true;
        //                drawArrowLighter = true;
        //                IsSelected = true;
        //            }
        //            else
        //            {
        //                drawRightDown = false;
        //                justInvoked = true;
        //                drawArrowLighter = true;
        //                IsSelected = null;
        //            }
        //        }
        //        return IsSelected;
        //    }
        //}
#endregion

        private class MenuSlider : GreedyControl<float>
        {
            /// <summary>
            /// Background (immobile) part of the slider.
            /// </summary>
            public Sprite BackgroundTexture { get; private set; }
            
            /// <summary>
            /// If this is true, you should hold everything and call OnSelect(), because the user wants to send input to this slider.
            /// </summary>
            public override bool IsActive { get { return isActive; } protected set { isActive = value; if(!isActive) justInvoked = false; } }

            private readonly float minValue;
            private readonly float maxValue;

            private readonly float distance; // rhsbound - lhsbound
            private readonly float lhsBound; // in screenspace
            private readonly float rhsBound; // in screenspace

            private const int frameLapse = 20; // number of frames to wait until accepting input again
            private const int delta = 5; // amount to advance or devance when using keyboard

            /// <summary>
            /// This is to be set only by the property. PERIOD.
            /// </summary>
            private bool isActive = false;

            private int framesHeld = 0; // number of frames the button has been held down

            // true if using mouse, false if using keyboard/xbox, null if we don't know yet.
            private bool? usingMouse;
            private bool justInvoked;

            private Direction direction = Direction.None;

            private enum Direction
            {
                Left,
                Right,
                None
            }

            /// <summary>
            /// Creates a slider that can be used to modify a floating-point value.
            /// </summary>
            /// <param name="backgroundTexture">The background of the slider (immobile part).</param>
            /// <param name="foreGroundTexture">The foreground of the slider (mobile part). Should be created with the center
            /// as the render point.</param>
            /// <param name="min">The minimum value of the variable.</param>
            /// <param name="max">The maximum value of the variable.</param>
            /// <param name="distance">The distance (in pixels) the slider can travel.</param>
            /// <param name="offset">Offset of distance (in pixels) from backgroundTexture.UpperLeft.X.</param>
            /// <param name="variable">The variable to get and set.</param>
            public MenuSlider(Sprite backgroundTexture, Sprite foreGroundTexture,
                float min, float max, float distance, float offset, string text, Vector2 textVector, SpriteFont font, 
                Pointer<float> variable)
                : base(variable, foreGroundTexture, text, textVector, font)
            {
                minValue = min;
                maxValue = max;
                BackgroundTexture = backgroundTexture;
                this.distance = distance;

                lhsBound = Texture.UpperLeft.X + offset;
                rhsBound = lhsBound + distance;

                HelpfulText = "Press %s% and use %lr% to adjust then press %s% again to confirm, or drag with the mouse.";
            }

            protected override void invoke()
            {
                if(IsSelected.HasValue && IsSelected.Value)
                    return; // cause we're still holding the button down.

                IsActive = true;

                if(!usingMouse.HasValue)
                {
                    justInvoked = true;
                    if(Input.CheckKeyboardPress(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey) ||
                        Input.CheckXboxPress(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey))
                        usingMouse = false;
                    else // we got in here with the mouse. Probably.
                        usingMouse = true;
                }
                else
                    justInvoked = false;

                if((Input.MouseState.LeftButton == ButtonState.Released && usingMouse.GetValueOrDefault()) || !Program.Game.IsActive)
                {
                    IsActive = false;
                    return;
                }
                else if((Input.CheckKeyboardPress(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey) ||
                        Input.CheckXboxPress(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey)) && !justInvoked &&
                        !usingMouse.GetValueOrDefault())
                {
                    IsActive = false;
                    return;
                }        

                #region variable updates
                if(usingMouse.GetValueOrDefault())
                {
                    float x = Input.MouseState.X;
                    if(x < lhsBound)
                        x = lhsBound;
                    if(x > rhsBound)
                        x = rhsBound;
                    Texture.TeleportTo(new Vector2(x, Texture.Center.Y));
                    variable.Value = ((x - lhsBound) / distance) * (maxValue - minValue);
                }
                else // using keyboard
                {
                    if(direction == Direction.Left)
                    {
                        if(Input.KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey) ||
                           Input.CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.MenuLeftKey))
                        {
                            if(framesHeld < frameLapse) // if the button hasn't been held down long enough, increase frames by one and return.
                                framesHeld++;
                            else
                            {
                                framesHeld = 0;
                                variable.Value -= delta;
                                if(variable.Value < minValue)
                                    variable.Value = minValue;
                                Texture.TeleportTo(new Vector2(Texture.Center.Y - delta, Texture.Center.Y));
                            }
                        }
                        else // left is no longer down
                            direction = Direction.None;
                    }
                    else if(direction == Direction.Right)
                    {
                        if(Input.KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey) ||
                              Input.CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.MenuRightKey))
                        {
                            if(framesHeld < frameLapse) // if the button hasn't been held down long enough, increase frames by one and return.
                                framesHeld++;
                            else
                            {
                                framesHeld = 0;
                                variable.Value += delta;
                                if(variable.Value > maxValue)
                                    variable.Value = maxValue;
                                Texture.TeleportTo(new Vector2(Texture.Center.Y + delta, Texture.Center.Y));
                            }
                        }
                        else // left is no longer down
                            direction = Direction.None;
                    }
                    else // direction is none
                        framesHeld = 0; // so this can be reset
                }
                #endregion
            }

            public override MenuControl OnDown { get { if(onDown != null) return onDown.Value; return null; } protected set { if(onDown != null) onDown.Value = value; onDown = new Pointer<MenuControl>(() => value, v => { value = v; }); OnDown = value; } }
            protected Pointer<MenuControl> onDown;
            public override MenuControl OnUp { get { if(onUp != null) return onUp.Value; return null; } protected set { if(onUp != null) onUp.Value = value; onUp = new Pointer<MenuControl>(() => value, v => { value = v; }); OnUp = value; } }
            protected Pointer<MenuControl> onUp;
            public override MenuControl OnLeft { get { if(onLeft != null) return onLeft.Value; return null; } protected set { if(onLeft != null) onLeft.Value = value; onLeft = new Pointer<MenuControl>(() => value, v => { value = v; }); OnLeft = value; } }
            protected Pointer<MenuControl> onLeft;
            public override MenuControl OnRight { get { if(onRight != null) return onRight.Value; return null; } protected set { if(onRight != null) onRight.Value = value; onRight = new Pointer<MenuControl>(() => value, v => { value = v; }); OnRight = value; } }
            protected Pointer<MenuControl> onRight;
            /// <summary>
            /// Call this instead of the other one.
            /// </summary>
            /// <param name="onL"></param>
            /// <param name="onR"></param>
            /// <param name="onU"></param>
            /// <param name="onD"></param>
            public void SetPointerDirectionals(Pointer<MenuControl> onL, Pointer<MenuControl> onR, Pointer<MenuControl> onU, Pointer<MenuControl> onD)
            {
                onLeft = onL;
                onRight = onR;
                onUp = onU;
                onDown = onD;
            }

            public override void SetDirectionals(MenuControl left, MenuControl right, MenuControl up, MenuControl down)
            {
                throw new NotSupportedException("Cannot be called on this object.");
            }

            public override void Draw(MenuControl selected)
            {
                base.Draw(selected);
                BackgroundTexture.Draw();
            }
        }

        private class VariableButton : MenuButton
        {
            public override MenuControl OnDown { get { if(onDown != null) return onDown.Value; return null; } protected set { if(onDown != null) onDown.Value = value; onDown = new Pointer<MenuControl>(() => value, v => { value = v; }); OnDown = value; } }
            protected Pointer<MenuControl> onDown;
            public override MenuControl OnUp { get { if(onUp != null) return onUp.Value; return null; } protected set { if(onUp != null) onUp.Value = value; onUp = new Pointer<MenuControl>(() => value, v => { value = v; }); OnUp = value; } }
            protected Pointer<MenuControl> onUp;
            public override MenuControl OnLeft { get { if(onLeft != null) return onLeft.Value; return null; } protected set { if(onLeft != null) onLeft.Value = value; onLeft = new Pointer<MenuControl>(() => value, v => { value = v; }); OnLeft = value; } }
            protected Pointer<MenuControl> onLeft;
            public override MenuControl OnRight { get { if(onRight != null) return onRight.Value; return null; } protected set { if(onRight != null) onRight.Value = value; onRight = new Pointer<MenuControl>(() => value, v => { value = v; }); OnRight = value; } }
            protected Pointer<MenuControl> onRight;

            public VariableButton(Sprite tex, Action a, string tooltip)
                : base(tex, a, tooltip)
            { }

            /// <summary>
            /// Call this instead of the other one.
            /// </summary>
            /// <param name="onL"></param>
            /// <param name="onR"></param>
            /// <param name="onU"></param>
            /// <param name="onD"></param>
            public void SetPointerDirectionals(Pointer<MenuControl> onL, Pointer<MenuControl> onR, Pointer<MenuControl> onU, Pointer<MenuControl> onD)
            {
                onLeft = onL;
                onRight = onR;
                onUp = onU;
                onDown = onD;
            }

            public override void SetDirectionals(MenuControl left, MenuControl right, MenuControl up, MenuControl down)
            {
                throw new InvalidOperationException("Can't call the normal SetDirectionals() on a VariableControl!");
            }
        }

        private class MenuButton : MenuControl
        {
            public MenuButton(Sprite t, Action action)
                :this(t, action, String.Empty)
            { }

            public MenuButton(Sprite t, Action action, string helpfulText)
                : base(t, helpfulText, action)
            { }
        }

        private abstract class GreedyControl<T> : GreedyControl
        {
            protected Pointer<T> variable { get; private set; }
            protected readonly string controlText;
            protected Vector2 textVector;
            protected readonly SpriteFont font;
            protected Vector2 stringLength;
            protected Vector2 relativeScreenSpace;

            public override Action OnSelect { get { return this.invoke; } protected set{ } }

            /// <summary>
            /// Creates a generic GreedyControl: a control that requires invocations when it is selected.
            /// </summary>
            /// <param name="variable">The variable to get/set.</param>
            /// <param name="backgroundTex">The background texture of the control.</param>
            /// <param name="text">The control's display text.</param>
            /// <param name="textV">The upper-left corner of the text.</param>
            /// <param name="f">The font to use.</param>
            protected GreedyControl(Pointer<T> variable, Sprite backgroundTex,
                string text, Vector2 textV, SpriteFont f)
            {
                this.variable = variable;
                Texture = backgroundTex;
                font = f;
                controlText = text;
                textVector = textV;
                upperLeft = new Vector2();
                lowerRight = new Vector2();
                upperLeft.X = backgroundTex.UpperLeft.X < textVector.X ? backgroundTex.UpperLeft.X : textVector.X;
                upperLeft.Y = backgroundTex.UpperLeft.Y < textVector.Y ? backgroundTex.UpperLeft.Y : textVector.Y;
                stringLength = font.MeasureString(controlText) * Program.Game.TextureScaleFactor;
                lowerRight.X = backgroundTex.LowerRight.X > textVector.X + stringLength.X ? backgroundTex.LowerRight.X : textVector.X + stringLength.X;
                lowerRight.Y = backgroundTex.LowerRight.Y > textVector.Y + stringLength.Y ? backgroundTex.LowerRight.Y : textVector.Y + stringLength.Y;
                IsSelected = false;

                relativeScreenSpace = textVector / new Vector2(Program.Game.Width, Program.Game.Height);
                Program.Game.GDM.DeviceReset += GDM_device_reset;
            }

            public override void Draw(MenuControl selected)
            {
                Program.Game.SpriteBatch.DrawString(font, controlText, textVector, IsDisabled ? DisabledTint : textColor, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
                base.Draw(selected);
            }

            protected virtual void GDM_device_reset(object caller, EventArgs e)
            {
                textVector = new Vector2(Program.Game.Width, Program.Game.Height) * relativeScreenSpace;

                Texture.ForceResize();
                Sprite backgroundTex = Texture;
                stringLength = font.MeasureString(controlText) * Program.Game.TextureScaleFactor;

                upperLeft.X = backgroundTex.UpperLeft.X < textVector.X ? backgroundTex.UpperLeft.X : textVector.X;
                upperLeft.Y = backgroundTex.UpperLeft.Y < textVector.Y ? backgroundTex.UpperLeft.Y : textVector.Y;
                lowerRight.X = backgroundTex.LowerRight.X > textVector.X + stringLength.X ? backgroundTex.LowerRight.X : textVector.X + stringLength.X;
                lowerRight.Y = backgroundTex.LowerRight.Y > textVector.Y + stringLength.Y ? backgroundTex.LowerRight.Y : textVector.Y + stringLength.Y;
            }
        }

        /// <summary>
        /// The only purpose for this class is to provide a solution for iterating through GreedyControls
        /// when the generic part doesn't matter.
        /// </summary>
        private abstract class GreedyControl : MenuControl
        {
            protected readonly Color textColor = Color.White;
            protected readonly Color invocationColor = Color.Green;

            /// <summary>
            /// This will be the upper-left of the text's vector and the background texture. Test it against any other applicable textures
            /// in the control.
            /// </summary>
            protected Vector2 upperLeft;
            /// <summary>
            /// This will be the lower-right of the text's vector and the background texture. Test it against any other applicable textures
            /// in the control.
            /// </summary>
            protected Vector2 lowerRight;

            public virtual bool IsActive { get; protected set; }

            protected abstract void invoke();

            /// <summary>
            /// Draws texture differently from MenuControl.Draw(). Does not call MenuControl.Draw(). Draws text.
            /// </summary>
            public override void Draw(MenuControl selected)
            {
                if(IsDisabled)
                {
                    Texture.Draw(DisabledTint);
                    return;
                }
                if(IsSelected.HasValue && IsSelected.Value && !IsActive)
                    Texture.Draw(DownTint);
                else if(!IsSelected.HasValue && !IsActive)
                    Texture.Draw(SelectionTint);
                else if(IsActive && !IsSelected.HasValue)
                    Texture.Draw(invocationColor);
                else
                    Texture.Draw();
            }

            /// <summary>
            /// Checks the mouse within the upper-left and lower-right of the sum of all drawn controls.
            /// </summary>
            /// <param name="selected">The currently selected control.</param>
            /// <returns>True if mouse is held down, null if rolled over or the mouse hasn't rolled over anything else, false if
            /// not selected. Also sets the return value to IsSelected.</returns>
            public override bool? CheckMouseInput(MenuControl selected)
            {
                bool transparent = false;
                if(hasTransparency)
                {
                    Vector2 antiscale = new Vector2(1 / Texture.Scale.X, 1 / Texture.Scale.Y);
                    Color[] pixel = new Color[] { new Color(1, 1, 1, 1) };
                    int relativeX = (int)((Input.MouseState.X - Texture.UpperLeft.X) * antiscale.X) + (Texture.TargetArea == Texture.Texture.Bounds ? 0 : Texture.TargetArea.X);
                    int relativeY = (int)((Input.MouseState.Y - Texture.UpperLeft.Y) * antiscale.Y) + (Texture.TargetArea == Texture.Texture.Bounds ? 0 : Texture.TargetArea.Y);
                    if(relativeX > 0 && relativeY > 0 && relativeX < Texture.Texture.Width && relativeY < Texture.Texture.Height)
                        Texture.Texture.GetData(0, new Rectangle(relativeX, relativeY, 1, 1), pixel, 0, 1);
                    if(pixel[0].A == 0)
                        transparent = true;
                }

                bool withinCoords = Input.CheckMouseWithinCoords(upperLeft, lowerRight) && !transparent;
                bool eligible = Program.Game.IsActive && !MouseTempDisabled && !IsDisabled;
                if(!eligible)
                    return IsSelected;
                if(withinCoords && eligible && Input.MouseState.LeftButton == ButtonState.Pressed)
                    IsSelected = true;
                else if(withinCoords && eligible && Input.MouseState.LeftButton != ButtonState.Pressed)
                    IsSelected = null;
                else if(!withinCoords && this == selected)
                    IsSelected = null;
                else
                    IsSelected = false;
                return IsSelected;
            }
        }

        private abstract class MenuControl
        {
            public static readonly Color SelectionTint = Color.Red;
            public static readonly Color DownTint = new Color(180, 0, 0);
            public static readonly Color DisabledTint = new Color(128, 128, 128, 128);
            /// <summary>
            /// The MenuControl that should be selected when Left is pressed and this is the selected MenuControl.
            /// </summary>
            public virtual MenuControl OnLeft { get; protected set; }
            /// <summary>
            /// The MenuControl that should be selected when Right is pressed and this is the selected MenuControl.
            /// </summary>
            public virtual MenuControl OnRight { get; protected set; }
            /// <summary>
            /// The MenuControl that should be selected when Up is pressed and this is the selected MenuControl.
            /// </summary>
            public virtual MenuControl OnUp { get; protected set; }
            /// <summary>
            /// The MenuControl that should be selected when Down is pressed and this is the selected MenuControl.
            /// </summary>
            public virtual MenuControl OnDown { get; protected set; }
            /// <summary>
            /// An Action defining what should be done when this control is clicked on.
            /// </summary>
            public virtual Action OnSelect { get; protected set; }
            public Sprite Texture { get; protected set; }

            public virtual bool? IsSelected { get; set; }
            public virtual bool IsDisabled { get; set; }

            /// <summary>
            /// This string displays a helpful bit of text about what the control does. %s% means print selection keys, %b% means 
            /// print Escape and Back, %lr% means print left and right icons.
            /// </summary>
            public string HelpfulText { get; protected set; }

            protected bool hasTransparency = false;

            /// <summary>
            /// Creates a new MenuControl.
            /// </summary>
            /// <param name="t">The texture to use.</param>
            /// <param name="onSelect">The action to invoke on selection.</param>
            protected MenuControl(Sprite t, string tooltip, Action onSelect)
            {
                Texture = t;
                OnSelect = onSelect;
                IsSelected = false;
                HelpfulText = tooltip;
            }

            /// <summary>
            /// Only GreedyControl is allowed to call this.
            /// </summary>
            protected MenuControl()
            { }

            /// <summary>
            /// Sets the directionals of the control. I'd recommend calling this.
            /// </summary>
            /// <param name="directionals">
            /// [0] - OnLeft
            /// [1] - OnRight
            /// [2] - OnUp
            /// [3] - OnDown</param>
            public virtual void SetDirectionals(MenuControl left, MenuControl right, MenuControl up, MenuControl down)
            {
                OnLeft = left;
                OnRight = right;
                OnUp = up;
                OnDown = down;
            }

            public void MakeTransparencySensitive()
            {
                hasTransparency = true;
            }

            /// <summary>
            /// Checks for mouse input.
            /// </summary>
            /// <returns>Returns false if nothing, null if rolled over, true if down. The value it returns will
            /// be the same as the one in IsSelected.</returns>
            public virtual bool? CheckMouseInput(MenuControl selected)
            {
                bool transparent = false;
                if(hasTransparency)
                {
                    Vector2 antiscale = new Vector2(1 / Texture.Scale.X, 1 / Texture.Scale.Y);
                    Color[] pixel = new Color[]{ new Color(1, 1, 1, 1) };
                    int relativeX = (int)((Input.MouseState.X - Texture.UpperLeft.X) * antiscale.X) + (Texture.TargetArea == Texture.Texture.Bounds ? 0 : Texture.TargetArea.X);
                    int relativeY = (int)((Input.MouseState.Y - Texture.UpperLeft.Y) * antiscale.Y) + (Texture.TargetArea == Texture.Texture.Bounds ? 0 : Texture.TargetArea.Y);
                    if(relativeX > 0 && relativeY > 0 && relativeX < Texture.Texture.Width && relativeY < Texture.Texture.Height)
                        Texture.Texture.GetData(0, new Rectangle(relativeX, relativeY, 1, 1), pixel, 0, 1);
                    if(pixel[0].A == 0)
                        transparent = true;
                }
                
                bool withinCoords = Input.CheckMouseWithinCoords(Texture) && !transparent;
                bool eligible = Program.Game.IsActive && !MouseTempDisabled && !IsDisabled;
                if(!eligible)
                    return IsSelected;

                if(withinCoords && eligible && Input.MouseState.LeftButton == ButtonState.Pressed && this == selected)
                    IsSelected = true;
                else if(withinCoords && eligible && Input.MouseState.LeftButton != ButtonState.Pressed)
                    IsSelected = null;
                else if(!withinCoords && this == selected)
                    IsSelected = null;
                else
                    IsSelected = false;
                return IsSelected;
            }

            /// <summary>
            /// Draws the control.
            /// </summary>
            public virtual void Draw(MenuControl selected)
            {
                this.Draw(selected, Color.White);
            }

            internal virtual void Draw(MenuControl selected, Color tint)
            {
                if(IsDisabled)
                    Texture.Draw(new Color(new Vector4(DisabledTint.ToVector3(), tint.A / 255f)));
                else if(IsSelected.HasValue && IsSelected.Value)
                    Texture.Draw(new Color(new Vector4(DownTint.ToVector3(), tint.A / 255f)));
                else if(!IsSelected.HasValue)
                    Texture.Draw(new Color(new Vector4(SelectionTint.ToVector3(), tint.A / 255f)));
                else
                    Texture.Draw(tint);
            }
        }
        #endregion
    }

    public sealed class Pointer<T>
    {
        private Func<T> getter;
        private Action<T> setter;

        public Pointer(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
        public T Value
        {
            get { return getter(); }
            set { setter(value); }
        }
    }
}
