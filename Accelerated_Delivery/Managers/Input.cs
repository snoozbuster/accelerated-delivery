using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Accelerated_Delivery_Win
{
    public static class Input
    {
        private static bool xboxPluggedInWhenKeyboard;

        static Input()
        {
            ControlScheme = ControlScheme.None;
        }

        public static void Update(GameTime gameTime, bool isSelectingSave)
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
                    GameManager.State = GameState.Paused_DC;
#else
                if(!CurrentPad.IsConnected && WasConnected && GameManager.State != GameState.Exiting && 
                    !isSelectingSave && GameManager.State != GameState.Paused_DC)
                {
                    if(GameManager.State == GameState.Paused_SelectingMedia || GameManager.State == GameState.Exiting)
                        GameManager.State = GameManager.PreviousState;
                    GameManager.State = GameState.Paused_DC;
                    ControlScheme = ControlScheme.None;
                    MediaSystem.PauseAuxilary();
                }
                else if(!xboxPluggedInWhenKeyboard && !WasConnected && CurrentPad.IsConnected && 
                    GameManager.State != GameState.Exiting && !isSelectingSave && 
                    GameManager.State != GameState.Paused_PadQuery)
                {
                    if(GameManager.State == GameState.Paused_SelectingMedia || GameManager.State == GameState.Exiting)
                        GameManager.State = GameManager.PreviousState;
                    GameManager.State = GameState.Paused_PadQuery;
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
        public static void PlayerSelect(bool isActive)
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
            else if(CheckKeyboardJustPressed(Keys.Enter) || CheckMouseJustClicked(isActive))
            {
                MessagePad = PlayerIndex.One;
                WasConnected = false;
                ControlScheme = ControlScheme.Keyboard;
                xboxPluggedInWhenKeyboard = playerOneState.IsConnected;
#if XBOX360
                SimpleMessageBox.ShowMessageBox("Notice", "Keyboard is enabled.", new string[] { "Okay" }, 0, MessageBoxIcon.Alert);
#endif
            }
        }

        public static bool CheckMouseJustClicked(bool isActive)
        {
#if WINDOWS
            return MouseLastFrame.LeftButton == ButtonState.Released && MouseState.LeftButton == ButtonState.Pressed && isActive;
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
                case 1: if(WindowsOptions.Machine1Key == Keys.D1)
                        return CheckKeyboardJustPressed(Keys.D1) || CheckKeyboardJustPressed(Keys.NumPad1) || CheckXboxJustPressed(XboxOptions.Machine1Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine1Key) || CheckXboxJustPressed(XboxOptions.Machine1Key);
                case 2: if(WindowsOptions.Machine2Key == Keys.D2)
                        return CheckKeyboardJustPressed(Keys.D2) || CheckKeyboardJustPressed(Keys.NumPad2) || CheckXboxJustPressed(XboxOptions.Machine2Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine2Key) || CheckXboxJustPressed(XboxOptions.Machine2Key);
                case 3: if(WindowsOptions.Machine3Key == Keys.D3)
                        return CheckKeyboardJustPressed(Keys.D3) || CheckKeyboardJustPressed(Keys.NumPad3) || CheckXboxJustPressed(XboxOptions.Machine3Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine3Key) || CheckXboxJustPressed(XboxOptions.Machine3Key);
                case 4: if(WindowsOptions.Machine4Key == Keys.D4)
                        return CheckKeyboardJustPressed(Keys.D4) || CheckKeyboardJustPressed(Keys.NumPad4) || CheckXboxJustPressed(XboxOptions.Machine4Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine4Key) || CheckXboxJustPressed(XboxOptions.Machine4Key);
                case 5: if(WindowsOptions.Machine5Key == Keys.D5)
                        return CheckKeyboardJustPressed(Keys.D5) || CheckKeyboardJustPressed(Keys.NumPad5) || CheckXboxJustPressed(XboxOptions.Machine5Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine5Key) || CheckXboxJustPressed(XboxOptions.Machine5Key);
                case 6: if(WindowsOptions.Machine6Key == Keys.D6)
                        return CheckKeyboardJustPressed(Keys.D6) || CheckKeyboardJustPressed(Keys.NumPad6) || CheckXboxJustPressed(XboxOptions.Machine6Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine6Key) || CheckXboxJustPressed(XboxOptions.Machine6Key);
                case 7: if(WindowsOptions.Machine7Key == Keys.D7)
                        return CheckKeyboardJustPressed(Keys.D7) || CheckKeyboardJustPressed(Keys.NumPad7) || CheckXboxJustPressed(XboxOptions.Machine7Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine7Key) || CheckXboxJustPressed(XboxOptions.Machine7Key);
                case 8: if(WindowsOptions.Machine8Key == Keys.D8)
                        return CheckKeyboardJustPressed(Keys.D8) || CheckKeyboardJustPressed(Keys.NumPad8) || CheckXboxJustPressed(XboxOptions.Machine8Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine8Key) || CheckXboxJustPressed(XboxOptions.Machine8Key);
                case 9: if(WindowsOptions.Machine9Key == Keys.D9)
                        return CheckKeyboardJustPressed(Keys.D9) || CheckKeyboardJustPressed(Keys.NumPad9) || CheckXboxJustPressed(XboxOptions.Machine9Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine9Key) || CheckXboxJustPressed(XboxOptions.Machine9Key);
                case 10: if(WindowsOptions.Machine0Key == Keys.D0)
                        return CheckKeyboardJustPressed(Keys.D0) || CheckKeyboardJustPressed(Keys.NumPad0) || CheckXboxJustPressed(XboxOptions.Machine0Key);
                    else return CheckKeyboardJustPressed(WindowsOptions.Machine0Key) || CheckXboxJustPressed(XboxOptions.Machine0Key);
                default: return false;
            }
        }

        public static bool CheckMachineIsHeld(int machineNumber)
        {
            switch(machineNumber)
            {
                case 1: if(WindowsOptions.Machine1Key == Keys.D1)
                        return KeyboardState.IsKeyDown(Keys.D1) || KeyboardState.IsKeyDown(Keys.NumPad1) || CurrentPad.IsButtonDown(XboxOptions.Machine1Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine1Key) || CurrentPad.IsButtonDown(XboxOptions.Machine1Key);
                case 2: if(WindowsOptions.Machine2Key == Keys.D2)
                        return KeyboardState.IsKeyDown(Keys.D2) || KeyboardState.IsKeyDown(Keys.NumPad2) || CurrentPad.IsButtonDown(XboxOptions.Machine2Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine2Key) || CurrentPad.IsButtonDown(XboxOptions.Machine2Key);
                case 3: if(WindowsOptions.Machine3Key == Keys.D3)
                        return KeyboardState.IsKeyDown(Keys.D3) || KeyboardState.IsKeyDown(Keys.NumPad3) || CurrentPad.IsButtonDown(XboxOptions.Machine3Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine3Key) || CurrentPad.IsButtonDown(XboxOptions.Machine3Key);
                case 4: if(WindowsOptions.Machine4Key == Keys.D4)
                        return KeyboardState.IsKeyDown(Keys.D4) || KeyboardState.IsKeyDown(Keys.NumPad4) || CurrentPad.IsButtonDown(XboxOptions.Machine4Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine4Key) || CurrentPad.IsButtonDown(XboxOptions.Machine4Key);
                case 5: if(WindowsOptions.Machine5Key == Keys.D5)
                        return KeyboardState.IsKeyDown(Keys.D5) || KeyboardState.IsKeyDown(Keys.NumPad5) || CurrentPad.IsButtonDown(XboxOptions.Machine5Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine5Key) || CurrentPad.IsButtonDown(XboxOptions.Machine5Key);
                case 6: if(WindowsOptions.Machine6Key == Keys.D6)
                        return KeyboardState.IsKeyDown(Keys.D6) || KeyboardState.IsKeyDown(Keys.NumPad6) || CurrentPad.IsButtonDown(XboxOptions.Machine6Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine6Key) || CurrentPad.IsButtonDown(XboxOptions.Machine6Key);
                case 7: if(WindowsOptions.Machine7Key == Keys.D7)
                        return KeyboardState.IsKeyDown(Keys.D7) || KeyboardState.IsKeyDown(Keys.NumPad7) || CurrentPad.IsButtonDown(XboxOptions.Machine7Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine7Key) || CurrentPad.IsButtonDown(XboxOptions.Machine7Key);
                case 8: if(WindowsOptions.Machine8Key == Keys.D8)
                        return KeyboardState.IsKeyDown(Keys.D8) || KeyboardState.IsKeyDown(Keys.NumPad8) || CurrentPad.IsButtonDown(XboxOptions.Machine8Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine8Key) || CurrentPad.IsButtonDown(XboxOptions.Machine8Key);
                case 9: if(WindowsOptions.Machine9Key == Keys.D9)
                        return KeyboardState.IsKeyDown(Keys.D9) || KeyboardState.IsKeyDown(Keys.NumPad9) || CurrentPad.IsButtonDown(XboxOptions.Machine9Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine9Key) || CurrentPad.IsButtonDown(XboxOptions.Machine9Key);
                case 10: if(WindowsOptions.Machine0Key == Keys.D0)
                        return KeyboardState.IsKeyDown(Keys.D0) || KeyboardState.IsKeyDown(Keys.NumPad0) || CurrentPad.IsButtonDown(XboxOptions.Machine0Key);
                    else return KeyboardState.IsKeyDown(WindowsOptions.Machine0Key) || CurrentPad.IsButtonDown(XboxOptions.Machine0Key);
                default: return false;
            }
        }

        public static void NullMessagePad()
        {
            MessagePad = null;
        }

        public static void ContinueWithKeyboard()
        {
            WasConnected = false;
            MessagePad = PlayerIndex.One;
            ControlScheme = ControlScheme.Keyboard;
            GameManager.State = GameManager.PreviousState;
        }

        public static void ContinueWithPad()
        {
            WasConnected = true;
            MessagePad = PlayerIndex.One;
            ControlScheme = ControlScheme.XboxController;
            GameManager.State = GameManager.PreviousState;
        }

        //internal static bool CheckMachineJustReleased(int machineNumber)
        //{
        //    switch(machineNumber)
        //    {
        //        case 1: if(WindowsOptions.Machine1Key == Keys.D1)
        //                return (KeyboardLastFrame.IsKeyDown(Keys.D1) && KeyboardState.IsKeyUp(Keys.D1)) || (KeyboardLastFrame.IsKeyDown(Keys.NumPad1) && KeyboardState.IsKeyUp(Keys.NumPad1)) || (CurrentPadLastFrame.IsButtonDown(XboxOptions.Machine1Key) && CurrentPad.IsButtonUp(XboxOptions.Machine1Key));
        //            else return (KeyboardLastFrame.IsKeyDown(WindowsOptions.Machine1Key) && KeyboardState.) || CheckXboxJustPressed(XboxOptions.Machine1Key);
        //        case 2: if(WindowsOptions.Machine2Key == Keys.D2)
        //                return CheckKeyboardJustPressed(Keys.D2) || CheckKeyboardJustPressed(Keys.NumPad2) || CheckXboxJustPressed(XboxOptions.Machine2Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine2Key) || CheckXboxJustPressed(XboxOptions.Machine2Key);
        //        case 3: if(WindowsOptions.Machine3Key == Keys.D3)
        //                return CheckKeyboardJustPressed(Keys.D3) || CheckKeyboardJustPressed(Keys.NumPad3) || CheckXboxJustPressed(XboxOptions.Machine3Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine3Key) || CheckXboxJustPressed(XboxOptions.Machine3Key);
        //        case 4: if(WindowsOptions.Machine4Key == Keys.D4)
        //                return CheckKeyboardJustPressed(Keys.D4) || CheckKeyboardJustPressed(Keys.NumPad4) || CheckXboxJustPressed(XboxOptions.Machine4Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine4Key) || CheckXboxJustPressed(XboxOptions.Machine4Key);
        //        case 5: if(WindowsOptions.Machine5Key == Keys.D5)
        //                return CheckKeyboardJustPressed(Keys.D5) || CheckKeyboardJustPressed(Keys.NumPad5) || CheckXboxJustPressed(XboxOptions.Machine5Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine5Key) || CheckXboxJustPressed(XboxOptions.Machine5Key);
        //        case 6: if(WindowsOptions.Machine6Key == Keys.D6)
        //                return CheckKeyboardJustPressed(Keys.D6) || CheckKeyboardJustPressed(Keys.NumPad6) || CheckXboxJustPressed(XboxOptions.Machine6Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine6Key) || CheckXboxJustPressed(XboxOptions.Machine6Key);
        //        case 7: if(WindowsOptions.Machine7Key == Keys.D7)
        //                return CheckKeyboardJustPressed(Keys.D7) || CheckKeyboardJustPressed(Keys.NumPad7) || CheckXboxJustPressed(XboxOptions.Machine7Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine7Key) || CheckXboxJustPressed(XboxOptions.Machine7Key);
        //        case 8: if(WindowsOptions.Machine8Key == Keys.D8)
        //                return CheckKeyboardJustPressed(Keys.D8) || CheckKeyboardJustPressed(Keys.NumPad8) || CheckXboxJustPressed(XboxOptions.Machine8Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine8Key) || CheckXboxJustPressed(XboxOptions.Machine8Key);
        //        case 9: if(WindowsOptions.Machine9Key == Keys.D9)
        //                return CheckKeyboardJustPressed(Keys.D9) || CheckKeyboardJustPressed(Keys.NumPad9) || CheckXboxJustPressed(XboxOptions.Machine9Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine9Key) || CheckXboxJustPressed(XboxOptions.Machine9Key);
        //        case 10: if(WindowsOptions.Machine0Key == Keys.D0)
        //                return CheckKeyboardJustPressed(Keys.D0) || CheckKeyboardJustPressed(Keys.NumPad0) || CheckXboxJustPressed(XboxOptions.Machine0Key);
        //            else return CheckKeyboardJustPressed(WindowsOptions.Machine0Key) || CheckXboxJustPressed(XboxOptions.Machine0Key);
        //        default: return false;
        //    }
        //}

        public static WindowsOptions WindowsOptions { get; private set; }
        public static XboxOptions XboxOptions { get; private set; }

        public static void SetOptions(WindowsOptions winop, XboxOptions xop)
        {
            WindowsOptions = winop;
            XboxOptions = xop;
        }
    }
}
