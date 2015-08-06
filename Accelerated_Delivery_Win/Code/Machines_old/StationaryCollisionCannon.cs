using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Collidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics;

namespace Accelerated_Delivery_Win
{
    public class StationaryCollisionCannon : OperationalMachine
    {
        public override bool IsActive { get { return true; } }

        protected readonly Explosion explosion;
        protected readonly List<WeldJoint> joints = new List<WeldJoint>();
        protected float time = 0;

        protected FocusedBurstSystem particles;
        protected readonly Quaternion initialParticleOrientation;

        public StationaryCollisionCannon(Vector3 explosionPos, float explosionMag, params BaseModel[] models)
            : base(0, -1, models)
        {
            explosion = new Explosion(explosionPos, explosionMag, 5, Program.Game.Space);

            particles = new FocusedBurstSystem(Program.Game, Vector3.UnitZ, explosionPos + 2 * Vector3.UnitZ);
            particles.AutoInitialize(Program.Game.GraphicsDevice, Program.Game.Content, Program.Game.SpriteBatch);
            initialParticleOrientation = particles.Emitter.OrientationData.Orientation;

            foreach(BaseModel m in modelList)
            {
                WeldJoint j = new WeldJoint(null, m.Ent);
                j.IsActive = true;
                joints.Add(j);
            }

            modelList[0].Ent.CollisionInformation.Events.PairTouching += onCollision;
        }

        protected override void updateVelocities() { }

        public override void OnAdditionToSpace(ISpace newSpace)
        {
            foreach(WeldJoint j in joints)
                newSpace.Add(j);
            base.OnAdditionToSpace(newSpace);
        }

        protected void onCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            modelList[0].Ent.CollisionInformation.Events.PairTouching -= onCollision;
            Program.Game.Space.DuringForcesUpdateables.Starting += update;
        }

        protected void update()
        {
            double timeLastFrame = time;
            time += Program.Game.Space.TimeStepSettings.TimeStepDuration;
            if(time > 3.5f && timeLastFrame < 3.5f)
                explode();
            else if(time >= 5.5f)
                reset();
        }

        protected void explode()
        {
            explosion.Explode();
            particles.Activate();
            MediaSystem.PlaySoundEffect(MediaSystem.SFXOptions.Explosion);
        }

        protected void reset()
        {
            time = 0;
            modelList[0].Ent.CollisionInformation.Events.PairTouching += onCollision;
            Program.Game.Space.DuringForcesUpdateables.Starting -= update;
        }

        public override void ResetMachine()
        {
            modelList[0].Ent.CollisionInformation.Events.PairTouching -= onCollision;
            Program.Game.Space.DuringForcesUpdateables.Starting -= update;
            modelList[0].Ent.CollisionInformation.Events.PairTouching += onCollision;
            time = 0;
            particles.Reset();
            base.ResetMachine();
        }

        public override void Rotate(Quaternion rotation)
        {
            foreach(BaseModel m in modelList)
                m.Ent.Orientation = m.OriginalOrientation * rotation; // cannon
            particles.Emitter.OrientationData.Orientation = initialParticleOrientation * rotation;
        }
    }
}
