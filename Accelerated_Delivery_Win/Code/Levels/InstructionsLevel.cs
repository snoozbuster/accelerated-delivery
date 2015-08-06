using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    public class InstructionsLevel : Level
    {
        protected static readonly Color lightBlue = new Color(150, 190, 205, 210);
        protected static readonly Color darkBlue = new Color(0, 0, 139, 200);
        protected readonly Texture2D cursorTex;

        protected float instructionTime = 0;

        protected InstructionalBox box;
        protected InstructionalBox goalBox;

        protected Stage stage = Stage.Hello;
        protected BaseModel currentModel;

        protected Timer stageTimer;

        protected enum Stage
        {
            Hello,
            Camera,
            Dispenser,
            Machines
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
        public InstructionsLevel(int boxesMax, int boxNeed, Vector3 spawnPoint, List<Vector3> billboardsThisLevel,
            Theme levelTheme, BaseModel levelModel, BaseModel[] glassModels, Goal normalCatcher, Goal coloredCatcher,
            Dictionary<OperationalMachine, bool> machines, List<Tube> tubes, LevelCompletionData data, string name)
            : base(0, boxesMax, boxNeed, spawnPoint, billboardsThisLevel, levelTheme, levelModel, glassModels, normalCatcher,
            coloredCatcher, machines, tubes, data, name)
        {
            spawnTime = int.MaxValue;
            if(Program.Game.LoadingScreen != null)
                cursorTex = Program.Game.LoadingScreen.loader.cursor;
            else
                cursorTex = Program.Game.Loader.cursor;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Program.Game.DrawDock(); // forces the tab-helper to draw under the instructions

            RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);

            if(currentModel != null && box != null)
            {
                drawLine(findCenterOfModel(currentModel.Ent), box.LowerLeft);
                //drawCursors(currentModel.Ent);
            }
            if(stage == Stage.Dispenser)
            {
                drawLine(findCenterOfModel(normalCatcher.Ent), goalBox.UpperRight);
                Vector3 temp = RenderingDevice.GraphicsDevice.Viewport.Project(BoxSpawnPoint, RenderingDevice.Camera.Projection, RenderingDevice.Camera.View,
                    RenderingDevice.Camera.World);
                drawLine(new Vector2(temp.X, temp.Y), box.LowerLeft);
                //drawCursors(normalCatcher.Ent);
                goalBox.Draw();
            }
            if(box != null)
                box.Draw();
            RenderingDevice.SpriteBatch.End();
        }

        private Vector2 findCenterOfModel(Entity entity)
        {
            Vector3 v = RenderingDevice.GraphicsDevice.Viewport.Project(entity.Position - entity.CollisionInformation.LocalPosition, RenderingDevice.Camera.Projection,
                RenderingDevice.Camera.View, Matrix.Identity);
            return new Vector2(v.X, v.Y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(box == null && stage == Stage.Hello && overlay == null)
            {
                string text = "Welcome to the Accelerated Delivery Corporation's Automated Instructional System. This program will teach the operator the basics of machine operation. Please stand by for further instruction.";
                Vector2 dim = new Vector2(426.4f, 195);
                box = new InstructionalBox(text, new Rectangle((int)(Program.Game.Loader.DestroyedBoxesBase.UpperLeft.X + 5 * RenderingDevice.TextureScaleFactor.X), (int)(RenderingDevice.Height * 0.01f + Program.Game.Loader.DestroyedBoxesBase.LowerRight.Y),
                    (int)dim.X, (int)dim.Y), advanceToCamera);
                stageTimer = new Timer(10000, box.Close, false);
                stageTimer.Start();
            }
            if(stageTimer != null)
                stageTimer.Update(gameTime);
            if(stage == Stage.Dispenser && BoxesRemaining == 0)
            {
                box.Close();
                goalBox.Close();
            }
            if(goalBox != null)
                goalBox.Update();
            if(Ending)
            {
                if(box != null)
                    box.Close();
                if(goalBox != null)
                    goalBox.Close();
            }
            if(box != null)
                box.Update();
        }

        private void advanceToMachines()
        {
            stage = Stage.Machines;
            goalBox = null;
            string text = "This is a machine. Activate this one with the %1% key. These machines are instrumental in your delivery's success. Learn their function and time their operation well, and you will succeed. You can access a list of machines with the %h% button.";
            Vector2 dim = new Vector2(536, 190);
            currentModel = MachineList.Keys.ElementAt(0).GetBase();
            box = new InstructionalBox(text, new Rectangle((int)(Program.Game.Loader.SurvivingBoxesBase.LowerRight.X - dim.X * RenderingDevice.TextureScaleFactor.X), (int)(RenderingDevice.Height * 0.01f + Program.Game.Loader.SurvivingBoxesBase.LowerRight.Y),
                (int)(dim.X), (int)(dim.Y)), delegate { box = null; currentModel = null; });
        }

        protected void advanceToCamera()
        {
            stage = Stage.Camera;
            string text = "Camera control is important. %c% moves the camera around the level, and %z% controls the zoom." +
#if WINDOWS
 " Alternatively, clicking and holding the right mouse button and moving the mouse will move the camera, and the middle mouse wheel can be used to zoom."
#else
                ""
#endif
;
            Vector2 dim = new Vector2(467.666656f, 220);
            box = new InstructionalBox(text, new Rectangle((int)(Program.Game.Loader.DestroyedBoxesBase.UpperLeft.X), (int)(RenderingDevice.Height * 0.65f),
                (int)(dim.X), (int)(dim.Y)), advanceToDispenser);
            stageTimer = new Timer(12000, box.Close, false);
            stageTimer.Start();
        }

        protected void advanceToDispenser()
        {
            stage = Stage.Dispenser;
            stageTimer = null;
            string text = "Boxes fall from this dispenser, here. Normally, they are on a timed-release system, but the operator can dispense them faster by pressing %~% .";
            Vector2 dim = new Vector2(320.6f, 195);
            box = new InstructionalBox(text, new Rectangle((int)(Program.Game.Loader.SurvivingBoxesBase.UpperLeft.X), (int)(RenderingDevice.Height * 0.05f + Program.Game.Loader.SurvivingBoxesBase.LowerRight.Y),
                (int)(dim.X), (int)(dim.Y)), advanceToMachines);
            text = "This is the goal. The operator's primary mission is to deposit boxes here, without letting them fall into the hazards below. The number of boxes required to move on to the next delivery is displayed on-screen, with the number of boxes remaining and lost. Please dispense a box to continue.";
            goalBox = new InstructionalBox(text, new Rectangle((int)(Program.Game.Loader.DestroyedBoxesBase.UpperLeft.X), (int)(RenderingDevice.Height * 0.65f),
                512, 216), null);
        }

        protected override void endLevel()
        {
            base.endLevel();
            results = null;
        }

        protected override void onTimerFired()
        {
            RemoveFromGame(GameManager.Space);
            loadNextLevel();
        }

        protected override void loadNextLevel()
        {
            GameManager.LevelNumber = -1;
            MediaSystem.PlayTrack(SongOptions.Menu);
        }

        protected override void spawnBox()
        {
            if(stage >= Stage.Dispenser && box.Open)
                base.spawnBox();    
        }

        public override void ResetLevel()
        {
            stage = Stage.Hello;
            box = null;
            goalBox = null;
            currentModel = null;
            if(stageTimer != null)
                stageTimer.Stop();
            stageTimer = null;
            base.ResetLevel();
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        protected void drawLine(Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            RenderingDevice.SpriteBatch.Draw(Program.Game.Loader.EmptyTex, point1, null, lightBlue,
                       angle, Vector2.Zero, new Vector2(length, 1.5f), SpriteEffects.None, 0);
        }

        protected class InstructionalBox
        {
            protected readonly int maxHeight;
            protected readonly int maxWidth;
            protected const byte delta = 6;

            protected readonly Rectangle initialUnscaled;

            protected Rectangle internalBox;
            protected Rectangle externalBox;

            protected Color textColor = Color.White;
            protected int alpha;
            protected HelpfulTextBox box;
            protected readonly string text;

            protected bool closing;
            protected bool opened;

            public Vector2 LowerLeft { get { return new Vector2(externalBox.X, externalBox.Y + externalBox.Height); } }
            public Vector2 UpperRight { get { return new Vector2(externalBox.X + externalBox.Width, externalBox.Y); } }

            public bool Closed { get; private set; }
            public bool Open { get { return opened; } }
            protected Action onClose { get { if(dontUseThis == null) return delegate { }; return dontUseThis; } set { dontUseThis = value; } }
            private Action dontUseThis;

            /// <summary>
            /// Creates a new box.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="initialPos">Should not be scaled by the texture scale factor.</param>
            /// <param name="function">Will be called when closing is finished. Null for nothing.</param>
            public InstructionalBox(string text, Rectangle initialPos, Action function)
            {
                onClose = function;
                this.text = text;
                initialUnscaled = initialPos;
                maxHeight = initialUnscaled.Height;
                maxWidth = initialUnscaled.Width;
                internalBox = new Rectangle(initialUnscaled.X, initialUnscaled.Y, 0, 0);
                externalBox = internalBox;
                externalBox.Inflate(2, 2);
                box = new HelpfulTextBox(new Rectangle(initialUnscaled.X + 2, initialUnscaled.Y + 2, (int)((initialUnscaled.Width - 4) * RenderingDevice.TextureScaleFactor.X), (int)((initialUnscaled.Height - 4) * RenderingDevice.TextureScaleFactor.Y)), delegate { return Program.Game.Loader.Font; });
                textColor.A = 0;
                box.SetTextColor(textColor);
            }

            public void Close()
            {
                closing = true;
            }

            public void Update()
            {
                if(!closing)
                {
                    if(internalBox.Height < (int)(maxHeight * RenderingDevice.TextureScaleFactor.Y))
                    {
                        internalBox.Height += delta;
                        if(internalBox.Height > maxHeight * RenderingDevice.TextureScaleFactor.Y)
                            internalBox.Height = (int)(maxHeight * RenderingDevice.TextureScaleFactor.Y);
                    }
                    else if(internalBox.Width < (int)(maxWidth * RenderingDevice.TextureScaleFactor.X))
                    {
                        internalBox.Width += delta;
                        if(internalBox.Width > maxWidth * RenderingDevice.TextureScaleFactor.X)
                            internalBox.Width = (int)(maxWidth * RenderingDevice.TextureScaleFactor.X);
                    }
                    if(internalBox.Width == (int)(maxWidth * RenderingDevice.TextureScaleFactor.X) && internalBox.Height == (int)(maxHeight * RenderingDevice.TextureScaleFactor.Y))
                        opened = true;
                    if(textColor.A < 255 && opened)
                        if(textColor.A + delta > 255)
                            textColor.A = 255;
                        else
                            textColor.A += delta;
                }
                else
                {
                    if(textColor.A > 0)
                        if(textColor.A - delta < 0)
                            textColor.A = 0;
                        else
                            textColor.A -= delta;
                    else if(internalBox.Height > 2)
                        internalBox.Height -= delta;
                    else if(internalBox.Width >= 2)
                    {
                        internalBox.Width -= delta;
                        if(internalBox.Width <= 0)
                        {
                            onClose();
                            Closed = true;
                        }
                    }
                }

                box.SetTextColor(textColor);
                externalBox = internalBox;
                externalBox.Inflate(2, 2);
            }

            public void Draw()
            {
                RenderingDevice.SpriteBatch.Draw(Program.Game.Loader.EmptyTex, externalBox, darkBlue);
                RenderingDevice.SpriteBatch.Draw(Program.Game.Loader.EmptyTex, internalBox, lightBlue);
                box.Draw(text);
            }
        }
    }
}
