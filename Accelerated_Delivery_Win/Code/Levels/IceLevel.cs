using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    class IceLevel : Level
    {
        BaseModel extras;

        /// <summary>
        /// Creates a new Ice level.
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
        public IceLevel(int level, int boxesMax, int boxNeed, Vector3 spawnPoint, List<Vector3> billboardsThisLevel,
            BaseModel levelModel, BaseModel extras, BaseModel[] glassModels, Goal normalCatcher,
            Dictionary<OperationalMachine, bool> machines, List<Tube> tubes, LevelCompletionData data, string name)
            : base(level, boxesMax, boxNeed, spawnPoint, billboardsThisLevel, Theme.Ice, levelModel, glassModels, normalCatcher,
            null, machines, tubes, data, name)
        {
            this.extras = extras;
        }

        protected override void addModelsToRenderer()
        {
            base.addModelsToRenderer();
            RenderingDevice.Add(extras);
        }

        public override void AddToGame(BEPUphysics.Space s)
        {
            base.AddToGame(s);
            //s.Add(extras); // no real reason to give these collision
        }

        protected override void removeModelsFromRenderer()
        {
            base.removeModelsFromRenderer();
            RenderingDevice.Remove(extras);
        }
    }
}
