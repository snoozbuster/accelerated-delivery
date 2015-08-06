using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Paths;
using Microsoft.Xna.Framework;

namespace Accelerated_Delivery_Win
{
    public class StationaryCollisionCannon : OperationalMachine
    {
        public override bool IsActive { get { return true; } }

        //protected readonly Explosion explosion;
        protected readonly List<WeldJoint> joints = new List<WeldJoint>();
        protected float time = 0;

        protected FocusedBurstSystem particles;
        protected readonly Quaternion initialParticleOrientation;
        protected CannonPathFinder pathFinder;
        protected List<BroadPhaseEntry> boxes = new List<BroadPhaseEntry>();

        public StationaryCollisionCannon(CannonPathFinder path, Vector3 particlePos, params BaseModel[] models)
            : base(0, -1, models)
        {
            //explosion = new Explosion(explosionPos, explosionMag, 5, GameManager.Space);
            pathFinder = path;

            particles = new FocusedBurstSystem(Program.Game, Vector3.UnitZ, particlePos + 2 * Vector3.UnitZ);
            particles.AutoInitialize(RenderingDevice.GraphicsDevice, Program.Game.Content, RenderingDevice.SpriteBatch);
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
            GameManager.Space.DuringForcesUpdateables.Starting += update;
        }

        protected void update()
        {
            double timeLastFrame = time;
            time += GameManager.Space.TimeStepSettings.TimeStepDuration;
            if(time > 3.5f && timeLastFrame < 3.5f)
                explode();
            else if(time >= 5.5f)
                reset();
        }

        protected void explode()
        {
            boxes.Clear();
            (Space as Space).BroadPhase.QueryAccelerator.GetEntries(joints[0].BallSocketJoint.ConnectionB.CollisionInformation.BoundingBox, boxes);
            boxes = boxes.FindAll(v => { return v.Tag is Box; });
            if(boxes.Count == 1)
                pathFinder.Add(boxes[0].Tag as Box);
            else if(boxes.Count > 1)
            {
                Box[] lololol = new Box[boxes.Count];
                int i = 0;
                foreach(BroadPhaseEntry b in boxes)
                    lololol[i++] = b.Tag as Box;
                pathFinder.Add(lololol);
            }
            //explosion.Explode();
            particles.Activate();
            MediaSystem.PlaySoundEffect(SFXOptions.Explosion);
        }

        protected void reset()
        {
            time = 0;
            modelList[0].Ent.CollisionInformation.Events.PairTouching += onCollision;
            GameManager.Space.DuringForcesUpdateables.Starting -= update;
        }

        public override void ResetMachine()
        {
            modelList[0].Ent.CollisionInformation.Events.PairTouching -= onCollision;
            GameManager.Space.DuringForcesUpdateables.Starting -= update;
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
