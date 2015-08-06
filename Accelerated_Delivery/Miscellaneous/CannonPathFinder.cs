using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Paths.PathFollowing;
using BEPUphysics.Paths;
using BEPUphysics;
using Microsoft.Xna.Framework;
using BEPUphysics.MathExtensions;

namespace Accelerated_Delivery_Win
{
    public class CannonPathFinder
    {
        protected Path<Vector3> positionPath;
        protected Dictionary<EntityMover, float> movers = new Dictionary<EntityMover, float>();
        protected float maxTime;
        protected float delta;
        protected bool x;
        protected Random r = new Random();

        /// <summary>
        /// Makes a CannonPathFinder with a custom path.
        /// </summary>
        /// <param name="path"></param>
        public CannonPathFinder(Path<Vector3> path, float delta, float maxTime, bool x)
        {
            this.delta = delta;
            this.maxTime = maxTime;
            this.x = x;
            positionPath = path;
            GameManager.Space.DuringForcesUpdateables.Starting += update; // we only have like two of these it'll be okay
        }

        /// <summary>
        /// Creates a helper class for pathfinding.
        /// </summary>
        /// <param name="velocity">Velocity with which the box is launched. This should only really have a x OR y value and a Z value.</param>
        /// <param name="initialPoint">Initial point of launch.</param>
        /// <param name="finalPoint">Target point. Should have similar Z-value to initialPoint.</param>
        public CannonPathFinder(Vector3 velocity, Vector3 initialPoint, Vector3 finalPoint)
        {
            if(velocity.X != 0 && velocity.Y != 0)
                throw new ArgumentException("You're going to have a bad time.");
            if(velocity.X == 0 && initialPoint.X != finalPoint.X)
                throw new ArgumentException("You're going to have a bad time.");
            if(velocity.Y == 0 && initialPoint.Y != finalPoint.Y)
                throw new ArgumentException("You're going to have a bad time.");


            CardinalSpline3D path = new CardinalSpline3D();
            Matrix3X3 m1i, m2;
            if(velocity.X != 0)
            {
                Matrix3X3 m1 = new Matrix3X3(initialPoint.X * initialPoint.X, initialPoint.X, 1, (initialPoint.X + finalPoint.X) * (initialPoint.X + finalPoint.X) / 4,
                    (initialPoint.X + finalPoint.X) / 2, 1, finalPoint.X * finalPoint.X, finalPoint.X, 1);
                Matrix3X3.Invert(ref m1, out m1i);
                m2 = new Matrix3X3(initialPoint.Z, 0, 0,
                                   initialPoint.Z + (velocity.Z / velocity.X) * (finalPoint.X - initialPoint.X) / 2, 0, 0,
                                   finalPoint.Z, 0, 0);
            }
            else
            {
                Matrix3X3 m1 = new Matrix3X3(initialPoint.Y * initialPoint.Y, initialPoint.Y, 1, (initialPoint.Y + finalPoint.Y) * (initialPoint.Y + finalPoint.Y) / 4,
                    (initialPoint.Y + finalPoint.Y) / 2, 1, finalPoint.Y * finalPoint.Y, finalPoint.Y, 1);
                Matrix3X3.Invert(ref m1, out m1i);
                m2 = new Matrix3X3(initialPoint.Z, 0, 0,
                                   initialPoint.Z + (velocity.Z / velocity.Y) * (finalPoint.Y - initialPoint.Y) / 2, 0, 0,
                                   finalPoint.Z, 0, 0);

            }
            Matrix3X3 r;
            Matrix3X3.Multiply(ref m1i, ref m2, out r);
            float a = r.M11;
            float b = r.M21;
            float c = r.M31;
            x = velocity.X != 0;
            delta = (x ? finalPoint.X - initialPoint.X : finalPoint.Y - initialPoint.Y);
            maxTime = delta / (x ? velocity.X : velocity.Y);
            if(maxTime < 0)
                throw new ArgumentException("Something is not right, friend.");

            path.ControlPoints.Add(-Math.Abs(1 / (x ? velocity.X : velocity.Y)), new Vector3(x ? initialPoint.X - 1 : initialPoint.X, x ? initialPoint.Y : initialPoint.Y - 1, a - b + c));
            path.ControlPoints.Add(0, initialPoint);
            path.ControlPoints.Add(maxTime / 2, new Vector3(x ? initialPoint.X + delta / 2 : initialPoint.X, x ? initialPoint.Y : initialPoint.Y + delta / 2, initialPoint.Z + delta * velocity.Z / (x ? velocity.X : velocity.Y) / 2));
            path.ControlPoints.Add(maxTime, new Vector3(x ? finalPoint.X - 2 : finalPoint.X, x ? finalPoint.Y : finalPoint.Y - 2, finalPoint.Z));
            path.ControlPoints.Add(maxTime + Math.Abs(1 / (x ? velocity.X : velocity.Y)), new Vector3(x ? finalPoint.X - 2 : finalPoint.X, x ? finalPoint.Y : finalPoint.Y - 2, a * (maxTime + 1) * (maxTime + 1) + b * (maxTime + 1) + c));

            positionPath = path;

            GameManager.Space.DuringForcesUpdateables.Starting += update; // we only have like two of these it'll be okay
        }

        /// <summary>
        /// Add a box and forget about it!
        /// </summary>
        /// <param name="b"></param>
        public void Add(Box b)
        {
            EntityMover m = new EntityMover(b.Ent);
            m.LinearMotor.Settings.MaximumForce = m.LinearMotor.Settings.Servo.MaxCorrectiveVelocity = 500;
            movers.Add(m, 0.5f);
            GameManager.Space.Add(m);
            b.Ent.AngularMomentum += new Vector3((float)r.NextDouble() * 3, (float)r.NextDouble() * 3, (float)r.NextDouble() * 3);
        }

        /// <summary>
        /// Add lots of boxes and forget about them!
        /// </summary>
        /// <param name="boxes"></param>
        public void Add(params Box[] boxes)
        {
            int i = 0;
            foreach(Box b in boxes)
            {
                EntityMover m = new EntityMover(b.Ent);
                m.LinearMotor.Settings.MaximumForce = m.LinearMotor.Settings.Servo.MaxCorrectiveVelocity = 500;
                movers.Add(m, 0.5f + i * 0.25f);
                GameManager.Space.Add(m);
                b.Ent.AngularMomentum += new Vector3((float)r.NextDouble() * 3, (float)r.NextDouble() * 3, (float)r.NextDouble() * 3);
                i++;
            }
        }

        protected void update()
        {
            for(int i = 0; i < movers.Count; i++)
            {
                KeyValuePair<EntityMover, float> k = movers.ElementAt(i);
                movers[k.Key] += GameManager.Space.TimeStepSettings.TimeStepDuration;
                if(movers[k.Key] >= 29 * maxTime / 64)
                {
                    movers.Remove(k.Key);
                    GameManager.Space.Remove(k.Key);
                    i--;
                    k.Key.Entity.LinearMomentum += new Vector3(x ? delta * 0.25f / maxTime : 0, x ? 0 : delta * 0.25f / maxTime, 0) * k.Key.Entity.Mass;
                    continue;
                }
                k.Key.TargetPosition = positionPath.Evaluate(k.Value);
            }
        }
    }
}
