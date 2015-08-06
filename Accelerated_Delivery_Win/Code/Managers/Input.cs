using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Accelerated_Delivery_Win
{
    public enum ControlScheme
    {
        Keyboard,
        XboxController,
        /// <summary>
        /// This is only valid on the Press Start screen and the disconnect screens.
        /// </summary>
        None
    }

    public static class Input
    {
        private static bool xboxPluggedInWhenKeyboard;

        static Input()
        {
            ControlScheme = ControlScheme.None;
        }

        public static void Update(GameTime gameTime)
        {
            KeyboardLastFrame = KeyboardState;
            KeyboardState = Keyboard.GetState();

#if WINDOWS
            MouseLastFrame = MouseState;
            MouseState = Mouse.GetState();
#endif

            #region Gamepad Updates
            if(MessagePad == null)
            {
                // If primary index not selected, update all.
                playerOneLastFrame = playerOneState;
                playerOneState = GamePad.GetState(PlayerIndex.One);

                playerTwoLastFrame = playerTwoState;
                playerTwoState = GamePad.GetState(PlayerIndex.Two);

                playerThreeLastFrame = playerThreeState;
                playerThreeState = GamePad.GetState(PlayerIndex.Three);

                playerFourLastFrame = playerFourState;
                playerFourState = GamePad.GetState(PlayerIndex.Four);
            }
            else
            {
                // Otherwise, just update the one that is primary.
                CurrentPadLastFrame = CurrentPad;
                CurrentPad = GamePad.GetState((PlayerIndex)MessagePad);
                if(!CurrentPad.IsConnected)
                    xboxPluggedInWhenKeyboard = false;
#if XBOX360
                if(!CurrentPad.IsConnected)
                    Program.Game.State = BaseGame.GameState.Paused_DC;
#else
                if(!CurrentPad.IsConnected && WasConnected && Program.Game.State != BaseGame.GameState.Exiting && 
                    !MenuHandler.IsSelectingSave && Program.Game.State != BaseGame.GameState.Paused_DC)
                {
                    if(Program.Game.State == BaseGame.GameState.Paused_SelectingMedia || Program.Game.State == BaseGame.GameState.Exiting)
                        Program.Game.State = Program.Game.PreviousState;
                    Program.Game.State = BaseGame.GameState.Paused_DC;
                    ControlScheme = ControlScheme.None;
                    MediaSystem.PauseAuxilary();
                }
                else if(!xboxPluggedInWhenKeyboard && !WasConnected && CurrentPad.IsConnected && 
                    Program.Game.State != BaseGame.GameState.Exiting && !MenuHandler.IsSelectingSave && 
                    Program.Game.State != BaseGame.GameState.Paused_PadQuery)
                {
                    if(Program.Game.State == BaseGame.GameState.Paused_SelectingMedia || Program.Game.State == BaseGame.GameState.Exiting)
                        Program.Game.State = Program.Game.PreviousState;
                    Program.Game.State = BaseGame.GameState.Paused_PadQuery;
                    ControlScheme = ControlScheme.None;
                    MediaSystem.PauseAuxilary();
                }
#endif
            }
            #endregion
        }

        /// <summary>
        /// Updates things for the main menu.
        /// </summary>
        public static void PlayerSelect()
        {
            if(CheckXboxJustPressed(PlayerIndex.One, Buttons.Start) ||
                CheckXboxJustPressed(PlayerIndex.One, Buttons.A))
            {
                WasConnected = true;
                ControlScheme = ControlScheme.XboxController;
                MessagePad = PlayerIndex.One;
            }
            else if(CheckXboxJustPressed(PlayerIndex.Two, Buttons.Start) ||
                CheckXboxJustPressed(PlayerIndex.Two, Buttons.A))
            {
                WasConnected = true;
                ControlScheme = ControlScheme.XboxController;
                MessagePad = PlayerIndex.Two;
            }
            else if(CheckXboxJustPressed(PlayerIndex.Three, Buttons.Start) ||
                CheckXboxJustPressed(PlayerIndex.Three, Buttons.A))
            {
                WasConnected = true;
                ControlScheme = ControlScheme.XboxController;
                MessagePad = PlayerIndex.Three;
            }
            else if(CheckXboxJustPressed(PlayerIndex.Four, Buttons.Start) ||
                CheckXboxJustPressed(PlayerIndex.Four, Buttons.A))
            {
                WasConnected = true;
                ControlScheme = ControlScheme.XboxController;
                MessagePad = PlayerIndex.Four;
            }
            else if(CheckKeyboardJustPressed(Keys.Enter) || CheckMouseJustClicked())
            {
                MessagePad = PlayerIndex.One;
                WasConnected = false;
                ControlScheme = ControlScheme.Keyboard;
                xboxPluggedInWhenKeyboard = playerOneState.IsConnected;
#if XBOX360
                SimpleMessageBox.ShowMessageBox("Notice", "Keyboard is enabled", new string[] { "Okay" }, 0, MessageBoxIcon.Alert);
#endif
            }
        }

        public static bool CheckMouseJustClicked()
        {
#if WINDOWS
            return MouseLastFrame.LeftButton == ButtonState.Released && MouseState.LeftButton == ButtonState.Pressed && Program.Game.IsActive;
#else
            return false;
#endif
        }

        /// <summary>
        /// 1 for LMB, 2 for RMB, 3 for MMB.
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool CheckMouseJustClicked(int mouseButton)
        {
#if WINDOWS
            switch(mouseButton)
            {
                case 1: return MouseLastFrame.LeftButton == ButtonState.Released && MouseState.LeftButton == ButtonState.Pressed;
                case 2: return MouseLastFrame.RightButton == ButtonState.Released && MouseState.RightButton == ButtonState.Pressed;
                case 3: return MouseLastFrame.MiddleButton == ButtonState.Released && MouseState.MiddleButton == ButtonState.Pressed;
                default: throw new ArgumentException("No such mouse button.");
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// The pad that pressed start. This pad has messages sent to it.
        /// </summary>
        public static PlayerIndex? MessagePad { get; private set; }
        /// <summary>
        /// The state of the above pad.
        /// </summary>
        public static GamePadState CurrentPad { get; private set; }
        public static GamePadState CurrentPadLastFrame { get; private set; }

        /// <summary>
        /// A bool specifying if a gamepad was ever connected.
        /// </summary>
        public static bool WasConnected { get; private set; }

        private static GamePadState playerOneState;
        private static GamePadState playerOneLastFrame;

        private static GamePadState playerTwoState;
        private static GamePadState playerTwoLastFrame;

        private static GamePadState playerThreeState;
        private static GamePadState playerThreeLastFrame;

        private static GamePadState playerFourState;
        private static GamePadState playerFourLastFrame;

        /// <summary>
        /// Current keyboard state.
        /// </summary>
        public static KeyboardState KeyboardState { get; private set; }
        /// <summary>
        /// Keyboard state last frame. For disallowing repeating input.
        /// </summary>
        public static KeyboardState KeyboardLastFrame { get; private set; }

#if WINDOWS
        /// <summary>
        /// Current mouse state.
        /// </summary>
        public static MouseState MouseState { get; private set; }
        /// <summary>
        /// Mouse state last frame. For disallowing repeating input.
        /// </summary>
        public static MouseState MouseLastFrame { get; private set; }
#endif

        public static ControlScheme ControlScheme { get; private set; }

        /// <summary>
        /// Checks to see if the left mouse button has just been clicked.
        /// </summary>
        /// <returns>True if the LMB is pressed this frame and wasn't last frame.</returns>
        public static bool CheckForMouseDown()
        {
#if WINDOWS
            return (MouseState.LeftButton == ButtonState.Pressed &&
                MouseLastFrame.LeftButton == ButtonState.Released);
#else
            return false;
#endif
        }

        public static bool CheckForMouseJustReleased()
        {
#if WINDOWS
            return MouseLastFrame.LeftButton == ButtonState.Pressed && MouseState.LeftButton == ButtonState.Released;
#else
            return false;
#endif
        }

        public static bool CheckForMouseJustReleased(int mouseButton)
        {
#if WINDOWS
            switch(mouseButton)
            {
                case 1: return MouseLastFrame.LeftButton == ButtonState.Pressed && MouseState.LeftButton == ButtonState.Released;
                case 2: return MouseLastFrame.RightButton == ButtonState.Pressed && MouseState.RightButton == ButtonState.Released;
                case 3: return MouseLastFrame.MiddleButton == ButtonState.Pressed && MouseState.MiddleButton == ButtonState.Released;
                default: throw new ArgumentException("No such mouse button.");
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// Checks to see if the mouse is within the given coordinates.
        /// </summary>
        /// <param name="upperLeft">Upper left corner of the area to check.</param>
        /// <param name="lowerRight">Lower right corner of the area to check.</param>
        /// <returns>True if the mouse is within the rectangle formed by the vectors.</returns>
        public static bool CheckMouseWithinCoords(Vector2 upperLeft, Vector2 lowerRight)
        {
#if WINDOWS
            return ((MouseState.X > upperLeft.X && MouseState.X < lowerRight.X) &&
                    (MouseState.Y > upperLeft.Y && MouseState.Y < lowerRight.Y));
#else
            return false;
#endif
        }
        /// <summary>
        /// Checks to see if the mouse was within the given coordinates last frame.
        /// </summary>
        /// <param name="upperLeft">Upper left corner of the area to check.</param>
        /// <param name="lowerRight">Lower right corner of the area to check.</param>
        /// <returns>True if the mouse is within the rectangle formed by the vectors.</returns>
        public static bool CheckMouseLastFrameWithinCoords(Vector2 upperLeft, Vector2 lowerRight)
        {
#if WINDOWS
            return ((MouseLastFrame.X > upperLeft.X && MouseLastFrame.X < lowerRight.X) &&
                    (MouseLastFrame.Y > upperLeft.Y && MouseLastFrame.Y < lowerRight.Y));
#else
            return false;
#endif
        }
        /// <summary>
        /// Checks to see if the mouse is within the given screen-relative texture.
        /// </summary>
        /// <param name="theTex">The SuperTextor to check if the mouse is within.</param>
        /// <returns>True if the mouse is within the texture.</returns>
        public static bool CheckMouseWithinCoords(Sprite theTex)
        {
#if WINDOWS
            return ((MouseState.X > theTex.UpperLeft.X && MouseState.X < theTex.LowerRight.X) &&
                    (MouseState.Y > theTex.UpperLeft.Y && MouseState.Y < theTex.LowerRight.Y));
#else
            return false;
#endif
        }

        /// <summary>
        /// Checks if a keyboard press is down last frame and up this frame. 
        /// </summary>
        /// <param name="theKey">The key to check.</param>
        /// <returns>True if the key was just released, false if it is not or if it is held.</returns>
        public static bool CheckKeyboardPress(Keys theKey)
        {
            if(WasConnected || ControlScheme == ControlScheme.XboxController)
                return false;
            return (KeyboardLastFrame.IsKeyDown(theKey) && KeyboardState.IsKeyUp(theKey));
        }
        public static bool CheckKeyboardJustPressed(Keys theKey)
        {
            if((WasConnected && CurrentPad.IsConnected) || ControlScheme == ControlScheme.XboxController)
                return false;
            return (KeyboardState.IsKeyDown(theKey) && KeyboardLastFrame.IsKeyUp(theKey));
        }
        public static bool CheckXboxPress(Buttons theButton)
        {
            if((CurrentPad == null || !WasConnected) || ControlScheme == ControlScheme.Keyboard)
                return false;
            return (CurrentPadLastFrame.IsButtonDown(theButton) && CurrentPad.IsButtonUp(theButton));
        }
        public static bool CheckXboxJustPressed(Buttons theButton)
        {
            if((CurrentPad == null || !CurrentPad.IsConnected) || ControlScheme == ControlScheme.Keyboard)
                return false;
            return (CurrentPad.IsButtonDown(theButton) && CurrentPadLastFrame.IsButtonUp(theButton));
        }
        public static bool CheckXboxJustPressed(PlayerIndex index, Buttons buttons)
        {
            switch(index)
            {
                case PlayerIndex.One: return (playerOneState.IsButtonDown(buttons) && playerOneLastFrame.IsButtonUp(buttons));
                case PlayerIndex.Two: return (playerTwoState.IsButtonDown(buttons) && playerTwoLastFrame.IsButtonUp(buttons));
                case PlayerIndex.Three: return (playerThreeState.IsButtonDown(buttons) && playerThreeLastFrame.IsButtonUp(buttons));
                case PlayerIndex.Four: return (playerFourState.IsButtonDown(buttons) && playerFourLastFrame.IsButtonUp(buttons));
                default: throw new ArgumentException("A player index outside the normal range was specified.");
            }
        }

        public static bool CheckMachineJustPressed(int machineNumber)
        {
            switch(machineNumber)
            {
                case 1: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key == Keys.D1)
                        return CheckKeyboardJustPressed(Keys.D1) || CheckKeyboardJustPressed(Keys.NumPad1) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key);
                case 2: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key == Keys.D2)
                        return CheckKeyboardJustPressed(Keys.D2) || CheckKeyboardJustPressed(Keys.NumPad2) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key);
                case 3: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key == Keys.D3)
                        return CheckKeyboardJustPressed(Keys.D3) || CheckKeyboardJustPressed(Keys.NumPad3) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key);
                case 4: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key == Keys.D4)
                        return CheckKeyboardJustPressed(Keys.D4) || CheckKeyboardJustPressed(Keys.NumPad4) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key);
                case 5: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key == Keys.D5)
                        return CheckKeyboardJustPressed(Keys.D5) || CheckKeyboardJustPressed(Keys.NumPad5) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key);
                case 6: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key == Keys.D6)
                        return CheckKeyboardJustPressed(Keys.D6) || CheckKeyboardJustPressed(Keys.NumPad6) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key);
                case 7: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key == Keys.D7)
                        return CheckKeyboardJustPressed(Keys.D7) || CheckKeyboardJustPressed(Keys.NumPad7) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key);
                case 8: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key == Keys.D8)
                        return CheckKeyboardJustPressed(Keys.D8) || CheckKeyboardJustPressed(Keys.NumPad8) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key);
                case 9: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key == Keys.D9)
                        return CheckKeyboardJustPressed(Keys.D9) || CheckKeyboardJustPressed(Keys.NumPad9) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key);
                case 10: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key == Keys.D0)
                        return CheckKeyboardJustPressed(Keys.D0) || CheckKeyboardJustPressed(Keys.NumPad0) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key);
                    else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key);
                default: return false;
            }
        }

        public static bool CheckMachineIsHeld(int machineNumber)
        {
            switch(machineNumber)
            {
                case 1: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key == Keys.D1)
                        return KeyboardState.IsKeyDown(Keys.D1) || KeyboardState.IsKeyDown(Keys.NumPad1) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key);
                case 2: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key == Keys.D2)
                        return KeyboardState.IsKeyDown(Keys.D2) || KeyboardState.IsKeyDown(Keys.NumPad2) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key);
                case 3: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key == Keys.D3)
                        return KeyboardState.IsKeyDown(Keys.D3) || KeyboardState.IsKeyDown(Keys.NumPad3) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key);
                case 4: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key == Keys.D4)
                        return KeyboardState.IsKeyDown(Keys.D4) || KeyboardState.IsKeyDown(Keys.NumPad4) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key);
                case 5: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key == Keys.D5)
                        return KeyboardState.IsKeyDown(Keys.D5) || KeyboardState.IsKeyDown(Keys.NumPad5) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key);
                case 6: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key == Keys.D6)
                        return KeyboardState.IsKeyDown(Keys.D6) || KeyboardState.IsKeyDown(Keys.NumPad6) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key);
                case 7: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key == Keys.D7)
                        return KeyboardState.IsKeyDown(Keys.D7) || KeyboardState.IsKeyDown(Keys.NumPad7) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key);
                case 8: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key == Keys.D8)
                        return KeyboardState.IsKeyDown(Keys.D8) || KeyboardState.IsKeyDown(Keys.NumPad8) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key);
                case 9: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key == Keys.D9)
                        return KeyboardState.IsKeyDown(Keys.D9) || KeyboardState.IsKeyDown(Keys.NumPad9) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key);
                case 10: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key == Keys.D0)
                        return KeyboardState.IsKeyDown(Keys.D0) || KeyboardState.IsKeyDown(Keys.NumPad0) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key);
                    else return KeyboardState.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key) || CurrentPad.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key);
                default: return false;
            }
        }

        internal static void NullMessagePad()
        {
            MessagePad = null;
        }

        internal static void ContinueWithKeyboard()
        {
            WasConnected = false;
            MessagePad = PlayerIndex.One;
            ControlScheme = ControlScheme.Keyboard;
            Program.Game.State = Program.Game.PreviousState;
        }

        internal static void ContinueWithPad()
        {
            WasConnected = true;
            MessagePad = PlayerIndex.One;
            ControlScheme = ControlScheme.XboxController;
            Program.Game.State = Program.Game.PreviousState;
        }

        //internal static bool CheckMachineJustReleased(int machineNumber)
        //{
        //    switch(machineNumber)
        //    {
        //        case 1: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key == Keys.D1)
        //                return (KeyboardLastFrame.IsKeyDown(Keys.D1) && KeyboardState.IsKeyUp(Keys.D1)) || (KeyboardLastFrame.IsKeyDown(Keys.NumPad1) && KeyboardState.IsKeyUp(Keys.NumPad1)) || (CurrentPadLastFrame.IsButtonDown(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key) && CurrentPad.IsButtonUp(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key));
        //            else return (KeyboardLastFrame.IsKeyDown(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key) && KeyboardState.) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key);
        //        case 2: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key == Keys.D2)
        //                return CheckKeyboardJustPressed(Keys.D2) || CheckKeyboardJustPressed(Keys.NumPad2) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key);
        //        case 3: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key == Keys.D3)
        //                return CheckKeyboardJustPressed(Keys.D3) || CheckKeyboardJustPressed(Keys.NumPad3) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key);
        //        case 4: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key == Keys.D4)
        //                return CheckKeyboardJustPressed(Keys.D4) || CheckKeyboardJustPressed(Keys.NumPad4) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key);
        //        case 5: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key == Keys.D5)
        //                return CheckKeyboardJustPressed(Keys.D5) || CheckKeyboardJustPressed(Keys.NumPad5) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key);
        //        case 6: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key == Keys.D6)
        //                return CheckKeyboardJustPressed(Keys.D6) || CheckKeyboardJustPressed(Keys.NumPad6) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key);
        //        case 7: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key == Keys.D7)
        //                return CheckKeyboardJustPressed(Keys.D7) || CheckKeyboardJustPressed(Keys.NumPad7) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key);
        //        case 8: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key == Keys.D8)
        //                return CheckKeyboardJustPressed(Keys.D8) || CheckKeyboardJustPressed(Keys.NumPad8) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key);
        //        case 9: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key == Keys.D9)
        //                return CheckKeyboardJustPressed(Keys.D9) || CheckKeyboardJustPressed(Keys.NumPad9) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key);
        //        case 10: if(Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key == Keys.D0)
        //                return CheckKeyboardJustPressed(Keys.D0) || CheckKeyboardJustPressed(Keys.NumPad0) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key);
        //            else return CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key) || CheckXboxJustPressed(Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key);
        //        default: return false;
        //    }
        //}
    }
}
