using BEPUphysics;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Materials;

namespace Accelerated_Delivery_Win
{
    public delegate Model ModelDelegate();

    public class BaseModel : ISpaceObject
    {
        //public MobileMeshSolidity Solidity { set { if(Ent != null) (Ent.CollisionInformation.Shape as MobileMeshShape).Solidity = value; } }

        public readonly bool RenderAsGlass;

        protected Model internalModel { get { return modelDelegate(); } }
        public Model Model { get { return modelDelegate(); } }

        /// <summary>
        /// The current model position.
        /// </summary>
        public Vector3 ModelPosition 
        { 
            get { return Ent.Position; } 
            set { Ent.Position = value; } 
        }
        public Vector3 Origin { get; protected set; }

        public Entity Ent { get; protected set; }

        public Matrix Transform { get; protected set; }

        public Quaternion OriginalOrientation { get; protected set; }
        public Vector3 LocalOffset { set { if(Ent != null) { Ent.CollisionInformation.LocalPosition = value; } } }

        //public Effect Shader { get; protected set; }
        //public LightingRig Lighting;
        private float time = 0;
        //public Texture2D ShaderTex { get; protected set; }
        //public Texture2D NoiseTex { get; protected set; }
        //private bool ignoreLight;

        protected Vector3 initialAngVel;
        protected Vector3 initialLinVel;

        public Vector3 InitialAngularVelocity { get { return initialAngVel; } }

        public bool UseCustomAlpha { get; protected set; }
        public float CustomAlpha { get; protected set; }

        public bool Remover
        {
            set
            {
                if(value && Ent != null)
                {
                    Ent.CollisionInformation.Events.InitialCollisionDetected += OnCollision;
                    Ent.IsAffectedByGravity = false;
                    Ent.CollisionInformation.CollisionRules.Group = noSolverGroupL;
                    UsesLaserSound = true;

                    //if(Model != null)
                    //    foreach(ModelMesh mesh in Model.Meshes)
                    //        foreach(ModelMeshPart meshPart in mesh.MeshParts)
                    //            meshPart.Effect = Resources.LaserShader;
                }
                else if(Ent != null)
                {
                    Ent.CollisionInformation.Events.InitialCollisionDetected -= OnCollision;
                    Ent.CollisionInformation.CollisionRules.Group = null;
                }
            }
        }
        public bool UsesLaserSound { get; set; }
        public float Mass { get { if(Ent != null) return Ent.Mass; return float.PositiveInfinity; } set { if(Ent != null) Ent.Mass = value; } }
        public bool IsInvisible { get; set; }

        public bool IsTerrain { get; private set; }

        protected readonly static CollisionGroup kinematicGroup = new CollisionGroup();
        protected readonly static CollisionGroup machineGroup = new CollisionGroup();
        protected readonly static CollisionGroup tubeGroup = new CollisionGroup();
        public readonly static CollisionGroup noSolverGroupL = new CollisionGroup();
        protected readonly static CollisionGroup noSolverGroupB = new CollisionGroup();

        protected readonly static Material machineMaterial = new Material(0.05f, 0.05f, 0);
        protected readonly static Material boxMaterial = new Material(1, 1, 0);
        protected readonly static Material tubeMaterial = new Material(0.33f, 0.33f, 0);

        protected ModelDelegate modelDelegate;

        static BaseModel()
        {
            CollisionGroup.DefineCollisionRule(kinematicGroup, machineGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(kinematicGroup, tubeGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(machineGroup, tubeGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(kinematicGroup, kinematicGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(machineGroup, machineGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(tubeGroup, tubeGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(noSolverGroupL, noSolverGroupB, CollisionRule.NoSolver);
            CollisionGroup.DefineCollisionRule(noSolverGroupL, tubeGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(noSolverGroupL, kinematicGroup, CollisionRule.NoBroadPhase);
            CollisionGroup.DefineCollisionRule(noSolverGroupL, machineGroup, CollisionRule.NoBroadPhase);

            MaterialManager.MaterialInteractions.Add(new MaterialPair { MaterialA = tubeMaterial, MaterialB = boxMaterial },
                                                     delegate(Material a, Material b, out InteractionProperties properties)
                                                     {
                                                         properties = new InteractionProperties
                                                             {
                                                                 StaticFriction = 0.62f,
                                                                 KineticFriction = 0.62f,
                                                                 Bounciness = 0
                                                             };
                                                     });
            MaterialManager.MaterialInteractions.Add(new MaterialPair { MaterialA = boxMaterial, MaterialB = machineMaterial },
                                                     delegate(Material a, Material b, out InteractionProperties properties)
                                                     {
                                                         properties = new InteractionProperties
                                                         {
                                                             StaticFriction = 0.001f,
                                                             KineticFriction = 0.001f,
                                                             Bounciness = 0
                                                         };
                                                     });
        }

        /// <summary>
        /// Creates a BaseModel without a model.
        /// </summary>
        /// <param name="e">The entity to use.</param>
        /// <param name="origin">The position of the model.</param>
        public BaseModel(Entity e, Vector3 origin)
        {
            UseCustomAlpha = false;
            RenderAsGlass = false;
            modelDelegate = delegate { return null; };
            Origin = origin;

            Ent = e;
            e.Tag = this;
            OriginalOrientation = Ent.Orientation;
        }

        /// <summary>
        /// Creates a BaseModel.
        /// </summary>
        /// <param name="model">The Model to use. Cannot be null.</param>
        /// <param name="glass">If true, the model is rendered as glass.</param>
        /// <param name="mobile">True if the model is dynamic, false if kinetic. Null means 
        /// kinetic but moves via velocity.</param>
        /// <param name="origin">The position of the model. Use Vector3.Zero if mobile is false.</param>
        public BaseModel(ModelDelegate modelDelegate, bool glass, bool? mobile, Vector3 origin)
        {
            UseCustomAlpha = false;
            this.modelDelegate = modelDelegate;

            Transform = Matrix.Identity;
            RenderAsGlass = glass;
            Origin = origin;
            OriginalOrientation = Quaternion.Identity;
            //this.ignoreLight = ignoreLight;

            Vector3[] verts;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(Model, out verts, out indices);
            Model.Tag = this;

            if(mobile.HasValue)
            {
                if(mobile.Value)
                {
                    // If it's mobile, it's a box!
                    if(Model == Resources.boxModel || Model == Resources.blueBoxModel || Model == Resources.blackBoxModel)
                    {
                        Ent = new BEPUphysics.Entities.Prefabs.Box(Origin, 1.45f, 2.05f, 1.25f, 7); // originally 1.4x2x1.2
                        Ent.ActivityInformation.IsAlwaysActive = true;
                        Ent.Material = boxMaterial;
                        //Ent.CollisionInformation.CollisionRules.Group = dynamicGroup;
                        Ent.CollisionInformation.CollisionRules.Group = noSolverGroupB;
                    }
                    else
                    {
                        // unless it's not a box.
                        Ent = new BEPUphysics.Entities.Prefabs.Box(Origin, 0.592f, 1.193f, 2f, 0.05f);
                        Ent.Material = machineMaterial;
                        Ent.CollisionInformation.CollisionRules.Group = machineGroup;
                    }
                }
                else
                {
                    // If false, it never moves.
                    Ent = new MobileMesh(verts, indices, AffineTransform.Identity, MobileMeshSolidity.DoubleSided);
                    //Transform = mesh.WorldTransform.Matrix;
                    Ent.Material = machineMaterial; // Make it slippery.
                    Ent.CollisionInformation.CollisionRules.Group = kinematicGroup;
                    //Transform = Ent.WorldTransform;
                    //Ent.Position = Origin;
                    IsTerrain = true;
                }
            }
            else
            {
                // If null, it is kinematic, but does move. (no longer true)
                //Ent = new MobileMesh(verts, indices, AffineTransform.Identity, solid ? MobileMeshSolidity.Solid : MobileMeshSolidity.Counterclockwise);
                Ent = new MobileMesh(verts, indices, AffineTransform.Identity, MobileMeshSolidity.DoubleSided, 30);
                Ent.CollisionInformation.CollisionRules.Group = machineGroup;
                //Ent.IsAffectedByGravity = false;
                Transform = Matrix.CreateTranslation(-Ent.Position);
                Ent.Position += Origin;
                Ent.Material = machineMaterial; // Make it slippery.
                Ent.CollisionInformation.Tag = this;
            }

            //foreach(ModelMesh mesh in internalModel.Meshes)
            //    foreach(BasicEffect effect in mesh.Effects)
            //        effect.EnableDefaultLighting();

            //if(Ent != null)
            //{
            OriginalOrientation = Ent.Orientation;
            Ent.Tag = this;
            //}
            //if(Mesh != null)
            //    Mesh.Tag = this;
        }

        protected BaseModel(ModelDelegate m, bool glass, Vector3 origin)
        {
            //internalModel = m;
            modelDelegate = m;
            RenderAsGlass = glass;
            Origin = origin;
        }

        public virtual void Update(GameTime gameTime) 
        {
            if(UsesLaserSound) // then we are a laser
            {
                time += 0.01f;
                RenderingDevice.SetShaderValues(Model, new[] { "time", "WorldViewProj", "position" }, new object[] { time, RenderingDevice.Camera.WorldViewProj, Ent.Position });
            }
        }

        private void OnCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            var otherEntity = other as EntityCollidable;
            if(otherEntity != null)
            {
                if(otherEntity.Tag is Box)
                {
                    if(UsesLaserSound)
                    {
                        (otherEntity.Tag as Box).PlayParticles();
                        MediaSystem.PlaySoundEffect(SFXOptions.Laser);
                    }
                    GameManager.CurrentLevel.DestroyBox((Box)otherEntity.Tag);
                }
            }
        }

        /// <summary>
        /// Casts a Model to an immobile BaseModel.
        /// </summary>
        /// <param name="m">The Model to cast.</param>
        /// <returns>A BaseModel created from the original Model.</returns>
        public static explicit operator BaseModel(ModelDelegate m)
        {
            if(m == null)
                return null;

            return new BaseModel(m, false, false, Vector3.Zero);
        }

        public void CommitInitialVelocities()
        {
            if(Ent == null)
                return;
            initialAngVel = Ent.AngularVelocity;
            initialLinVel = Ent.LinearVelocity;
        }

        public void ReturnToInitialVelocities()
        {
            if(Ent == null)
                return;
            Ent.AngularVelocity = initialAngVel;
            Ent.LinearVelocity = initialLinVel;
        }        

        public void Reset()
        {
            ModelPosition = Origin - Ent.CollisionInformation.LocalPosition - Transform.Translation;
            Ent.Orientation = OriginalOrientation;
            Ent.LinearVelocity = initialLinVel;
            Ent.AngularVelocity = initialAngVel;
        }

        void ISpaceObject.OnAdditionToSpace(ISpace newSpace)
        {
            //if(Ent != null)
            newSpace.Add(Ent);
            //else
            //    newSpace.Add(mesh);
        }

        void ISpaceObject.OnRemovalFromSpace(ISpace oldSpace)
        {
            if(/*Ent != null &&*/ Ent.Space != null)
            oldSpace.Remove(Ent);
            //else if(mesh != null)
            //    oldSpace.Remove(mesh);
        }

        ISpace ISpaceObject.Space { get; set; }

        object ISpaceObject.Tag { get; set; }

        public void AddToRenderer()
        {
            //if(UsesLaserSound)
            //    RenderingDevice.Add(Model, Resources.LaserShader, new System.Collections.Generic.Dictionary<string, object>() { { "WorldViewProj", RenderingDevice.Camera.WorldViewProj }, { "tex", Resources.LaserTexture }, { "time", 0 }, { "position", Ent.Position } });
        }

        public void RemoveFromRenderer()
        {
            //if(UsesLaserSound)
            //{
            //    RenderingDevice.Remove(Model);
            //    time = 0;
            //}
        }
    } // end class
}
