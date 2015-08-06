using System.Collections.Generic;
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    class Level6 : IceLevel
    {
        protected BaseModel invisiblePlane;
        protected new const int deltaBox = 2;

        /// <summary>
        /// Creates a new Level 6.
        /// </summary>
        /// <param name="boxesMax">Max boxes allowed.</param>
        /// <param name="boxNeed">Boxes needed.</param>
        /// <param name="spawnPoint">Spawn point.</param>
        /// <param name="billboardsThisLevel">List of billboards.</param>
        /// <param name="levelTheme">Theme of the level.</param>
        /// <param name="levelModel">Base terrain of the level.</param>
        /// <param name="glassModels">Array of glass models.</param>
        /// <param name="normalCatcher">Goal.</param>
        /// <param name="machines">List of machines.</param>
        /// <param name="tubes">List of tubes.</param>
        /// <param name="data">LevelCompletionData.</param>
        /// <param name="name">Name of the level.</param>
        /// <param name="plane">Tacos.</param>
        public Level6(int boxesMax, int boxNeed, Vector3 spawnPoint, List<Vector3> billboardsThisLevel,
            BaseModel levelModel, BaseModel extras, BaseModel[] glassModels, Goal normalCatcher,
            Dictionary<OperationalMachine, bool> machines, List<Tube> tubes, LevelCompletionData data, string name, BaseModel plane)
            : base(6, boxesMax, boxNeed, spawnPoint, billboardsThisLevel, levelModel, extras, glassModels, normalCatcher,
            machines, tubes, data, name)
        {
            invisiblePlane = plane;
            //spawnBlackBox = true;
        }

        public override void AddToGame(Space s)
        {
            //s.Add(invisiblePlane);
            base.AddToGame(s);
        }

        public override void RemoveFromGame(Space s)
        {
            //if(invisiblePlane.Ent.Space != null)
            //    invisiblePlane.Ent.Space.Remove(invisiblePlane);
            base.RemoveFromGame(s);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            RenderingDevice.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            RenderingDevice.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            foreach(Box b in boxesOnscreen)
                if(b is BlackBox)
                    (b as BlackBox).Draw(); // draws billboards

            RenderingDevice.GraphicsDevice.BlendState = BlendState.Opaque;
            RenderingDevice.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        protected override void removeModelsFromRenderer()
        {
            foreach(Box b in boxesOnscreen)
                if(b is BlackBox)
                {
                    RenderingDevice.RemovePermanent(b.Model);
                    (b as BlackBox).Remove();
                }
            base.removeModelsFromRenderer();
        }

        protected override void spawnBox()
        {
            int r = random.Next(0, 3); // one-third chance of spawning a black box
            if(counter > deltaBox || r != 0 || BoxesRemaining == boxesMax)
            {
                base.spawnBox();
                counter = 0;
            }
            else // r == 0 and counter < deltaBox
            {
                counter++;
                spawnTimer = 0;
                BlackBox box = new BlackBox();

                if(!(spawnTimer >= spawnTime))
                    box.ExtraPoints = (spawnTime - spawnTimer) / 60;
                boxesOnscreen.Add(box);

                //BoxesRemaining--;
                RenderingDevice.Add(box);
            }
        }
    }
}
