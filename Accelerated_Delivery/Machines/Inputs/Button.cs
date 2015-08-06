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
    public class Button : ISpaceObject, IMachineInput
    {
        protected bool pressed;
        protected bool wasPressed;
        public readonly BaseModel Model;
        protected readonly Vector3 translationAxis;
        protected readonly PrismaticJoint joint;

        public Button(Vector3 translationAxis, BaseModel button)
        {
            Model = button;
            this.translationAxis = translationAxis;

            joint = new PrismaticJoint(null, button.Ent, button.Ent.Position, translationAxis, button.Ent.Position);
            joint.Motor.IsActive = true;
            joint.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
            joint.Motor.Settings.Servo.BaseCorrectiveSpeed = joint.Motor.Settings.Servo.MaxCorrectiveVelocity = 1.5f;
            joint.Motor.Settings.Servo.SpringSettings.StiffnessConstant = 0;
            joint.Motor.Settings.Servo.SpringSettings.DampingConstant /= 12;
            joint.Limit.Minimum = -0.7f;
            joint.Limit.Maximum = 0;
        }

        protected void onCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if(!(other.Tag is Box))
                return;

            joint.Motor.Settings.Servo.Goal = -0.7f;
            if(!pressed)
                MediaSystem.PlaySoundEffect(SFXOptions.Machine_Button_Press);
            pressed = true;
        }

        public void Reset()
        {
            joint.Motor.Settings.Servo.Goal = 0;
            pressed = wasPressed = false;
        }

        public void OnAdditionToSpace(ISpace newSpace)
        {
            newSpace.Add(Model);
            newSpace.Add(joint);
            Model.Ent.CollisionInformation.Events.PairTouching += onCollision;
        }

        public void OnRemovalFromSpace(ISpace oldSpace)
        {
            oldSpace.Remove(Model);
            oldSpace.Remove(joint);
            Model.Ent.CollisionInformation.Events.PairTouching -= onCollision;
        }

        public ISpace Space { get; set; }

        public object Tag { get; set; }

        public bool JustActivated
        {
            get { return pressed && !wasPressed; }
        }

        public bool JustDeactivated
        {
            get { return wasPressed && !pressed; }
        }

        public bool IsActive
        {
            get { return pressed; }
        }

        public void UpdateInput()
        {
            wasPressed = pressed;
        }
    }
}
