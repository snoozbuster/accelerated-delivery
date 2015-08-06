﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Accelerated_Delivery_Win
{
    public class MyCamera
    {
        private Matrix rotation = Matrix.Identity;
        public Matrix Rotation { get { return rotation; } }
        public Vector3 Position { get; private set; }

        // Simply feed this camera the position of whatever you want its target to be
        public Vector3 TargetPosition = Vector3.Zero;
        protected Vector3 posLastFrame;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        private float zoom = 100.0f;
        public float Zoom { get { return zoom; } set { zoom = MathHelper.Clamp(value, zoomMin, zoomMax); } }

        private float horizontalAngle = MathHelper.PiOver2;
        public float HorizontalAngle
        {
            get { return horizontalAngle; }
            set
            {
                if(value >= MathHelper.Pi || value <= -MathHelper.Pi)
                {
                    horizontalAngle = -horizontalAngle + (value % MathHelper.Pi);
                    return;
                }
                horizontalAngle = value % MathHelper.Pi; 
            }
        }

        private float verticalAngle = (float)Math.Sqrt(3) / 2;
        public float VerticalAngle{ get { return verticalAngle; } set { verticalAngle = MathHelper.Clamp(value, verticalAngleMin, verticalAngleMax); } }

#if DEBUG || INTERNAL
        private float verticalAngleMin = 0;
        private float verticalAngleMax = MathHelper.TwoPi;
        private float zoomMin = 0;
        private float zoomMax = 10000;
        public bool debugCamera { get; private set; }
#else
        private const float verticalAngleMin = 0.7f;
        private const float verticalAngleMax = 1.25f;
        private const float zoomMin = 75.0f;
        private const float zoomMax = 125.0f;
#endif

        public Matrix WorldViewProj { get { return World * ViewProj; } }
        public Matrix InverseView { get { return Matrix.Invert(View); } }
        public Matrix ViewProj { get { return View * Projection; } }
        public Matrix World { get { return Matrix.Identity; } }
        public AudioListener Ears { get; private set; }

        /// <summary>
        /// Creates a camera.
        /// </summary>
        /// <param name="fieldOfView">The field of view in radians.</param>
        /// <param name="aspectRatio">The aspect ratio of the game.</param>
        /// <param name="nearPlane">The near plane.</param>
        /// <param name="farPlane">The far plane.</param>
        public MyCamera(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            if(nearPlane < 0.1f)
                throw new ArgumentException("nearPlane must be greater than 0.1.");

            Position = new Vector3(20, 20, 20);

            HorizontalAngle = MathHelper.PiOver4;
            VerticalAngle = (float)Math.Sqrt(3) / 2;

            this.Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio,
                                                                        nearPlane, farPlane);
            this.LookAt(TargetPosition);
            this.View = Matrix.CreateLookAt(this.Position,
                                            this.Position + this.rotation.Forward,
                                            this.rotation.Up);
            
            Ears = new AudioListener();
            Ears.Up = Vector3.UnitZ;
            Ears.Position = Position;
            Ears.Forward = rotation.Forward;
            Update(new GameTime()); // call a quick update to get everything in order
        }

        public void Update(GameTime gameTime)
        {
            posLastFrame = Position;
            Vector3 cameraPosition = new Vector3(0.0f, 0.0f, zoom);

            HandleInput(gameTime);

            // Rotate vertically
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationX(verticalAngle));

            // Rotate horizontally
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationZ(horizontalAngle));
            
            Position = cameraPosition + TargetPosition;

            this.LookAt(TargetPosition);

            // Compute view matrix
            this.View = Matrix.CreateLookAt(this.Position,
                                            this.Position + this.rotation.Forward,
                                            this.rotation.Up);

            Ears.Position = Position;
            Ears.Forward = rotation.Forward;
            Ears.Velocity = (Position - cameraPosition) / gameTime.ElapsedGameTime.Milliseconds;
        }

        /// <summary>
        /// Points camera in direction of any position.
        /// </summary>
        /// <param name="targetPos">Target position for camera to face.</param>
        public void LookAt(Vector3 targetPos)
        {
            Vector3 newForward = targetPos - this.Position;
            newForward.Normalize();
            this.rotation.Forward = newForward;

            Vector3 referenceVector = Vector3.UnitZ;

            this.rotation.Right = Vector3.Cross(this.rotation.Forward, referenceVector);
            this.rotation.Up = Vector3.Cross(this.rotation.Right, this.rotation.Forward);
        }

        private void HandleInput(GameTime gameTime)
        {
            if(GameManager.State == GameState.Results)
                return;

#if DEBUG || INTERNAL
            if(Input.CheckKeyboardJustPressed(Keys.Space))
                debugCamera = !debugCamera;

            if(!debugCamera)
            {
                verticalAngleMin = 0.7f;
                verticalAngleMax = 1.25f;
                zoomMin = 75.0f;
                zoomMax = 125.0f;
            }
            else
            {
                verticalAngleMin = 0;
                verticalAngleMax = MathHelper.TwoPi;
                zoomMin = 0;
                zoomMax = 10000;
            }

#endif
            if(!Input.WindowsOptions.SwapCamera)
            {
                if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraDownKey) ||
                    Input.CurrentPad.IsButtonDown(Buttons.LeftThumbstickDown) || Input.CurrentPad.IsButtonDown(Buttons.RightThumbstickDown))
                    VerticalAngle += gameTime.ElapsedGameTime.Milliseconds * MathHelper.ToRadians(0.1f);
                if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraUpKey) ||
                    Input.CurrentPad.IsButtonDown(Buttons.LeftThumbstickUp) || Input.CurrentPad.IsButtonDown(Buttons.RightThumbstickUp))
                    this.VerticalAngle -= gameTime.ElapsedGameTime.Milliseconds * MathHelper.ToRadians(0.1f);
            }
            else
            {
                if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraDownKey) ||
                    Input.CurrentPad.IsButtonDown(Buttons.LeftThumbstickDown) || Input.CurrentPad.IsButtonDown(Buttons.RightThumbstickDown))
                    this.VerticalAngle -= gameTime.ElapsedGameTime.Milliseconds * MathHelper.ToRadians(0.1f);
                if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraUpKey) ||
                    Input.CurrentPad.IsButtonDown(Buttons.LeftThumbstickUp) || Input.CurrentPad.IsButtonDown(Buttons.RightThumbstickUp))
                    this.VerticalAngle += gameTime.ElapsedGameTime.Milliseconds * MathHelper.ToRadians(0.1f);
            }
            if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraLeftKey) ||
                Input.CurrentPad.IsButtonDown(Buttons.LeftThumbstickLeft) || Input.CurrentPad.IsButtonDown(Buttons.RightThumbstickLeft))
                this.HorizontalAngle -= gameTime.ElapsedGameTime.Milliseconds * MathHelper.ToRadians(0.1f);
            if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraRightKey) ||
                Input.CurrentPad.IsButtonDown(Buttons.LeftThumbstickRight) || Input.CurrentPad.IsButtonDown(Buttons.RightThumbstickRight))
                this.HorizontalAngle += gameTime.ElapsedGameTime.Milliseconds * MathHelper.ToRadians(0.1f);

            if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraZoomPlusKey) ||
                (Input.WindowsOptions.CameraZoomPlusKey == Keys.Add && Input.KeyboardState.IsKeyDown(Keys.OemPlus)) ||
                Input.CurrentPad.IsButtonDown(Input.XboxOptions.CameraZoomPlusKey))
                this.Zoom -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50f;

#if DEBUG
            if(GameManager.CurrentLevel != null && GameManager.CurrentLevel.spawnlevel == 0)
#endif
            if(Input.KeyboardState.IsKeyDown(Input.WindowsOptions.CameraZoomMinusKey) ||
                (Input.WindowsOptions.CameraZoomMinusKey == Keys.Subtract && Input.KeyboardState.IsKeyDown(Keys.OemMinus)) ||
                Input.CurrentPad.IsButtonDown(Input.XboxOptions.CameraZoomMinusKey))
                this.Zoom += (float)gameTime.ElapsedGameTime.TotalSeconds * 50f;
#if WINDOWS
            Zoom -= MathHelper.ToRadians(Input.MouseState.ScrollWheelValue - Input.MouseLastFrame.ScrollWheelValue);
            if(Input.MouseState.RightButton == ButtonState.Pressed)
            {
                HorizontalAngle += MathHelper.ToRadians(Input.MouseState.X - Input.MouseLastFrame.X);
                VerticalAngle += MathHelper.ToRadians((Input.MouseState.Y - Input.MouseLastFrame.Y) * 0.5f);
            }
#endif
        }

        public void SetForResultsScreen()
        {
            zoom = 40f;
            horizontalAngle = MathHelper.ToRadians(45);
            verticalAngle = (float)Math.Sqrt(3) / 2;
            TargetPosition = Vector3.Zero;
        }

        public void Reset()
        {
            zoom = 100f;
            verticalAngle = (float)Math.Sqrt(3) / 2;
            horizontalAngle = MathHelper.PiOver2;
            TargetPosition = Vector3.Zero;
        }
    }
}