using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysics.UpdateableSystems;
using Microsoft.Xna.Framework.Content;

namespace Accelerated_Delivery_Win
{
    public abstract class Theme
    {
        private static event Action onGDMReset;

        public static Theme Lava { get { return new LavaTheme(); } }

        public static Theme Beach { get { return new BeachTheme(); } }

        public static Theme Ice { get { return new IceTheme(); } }

        public static Theme Sky { get { return new SkyTheme(); } }

        public static Theme Generic { get { return new GenericTheme(); } }

        public static Theme Space { get { return new SpaceTheme(); } }

        public float RemovalHeight { get; protected set; }
        protected Color internalTextColor;
        public Color TextColor { get { return internalTextColor; } }
        protected Effect internalShader;
        public Effect Shader { get { return internalShader; } }
        protected Texture2D shaderTexture;
        public Texture2D ShaderTexture { get { return shaderTexture; } }

        private Model planeModel;
        public Model PlaneModel { get { return planeModel; } }
        public BaseModel SkyBox { get; protected set; }
        public BaseModel OuterModel { get; protected set; }

        protected FluidVolume internalFluid;
        public FluidVolume Fluid { get { return internalFluid; } }

        public Color LevelTitleColor { get; protected set; }
        public abstract SongOptions Song { get; }
        public abstract string Name { get; }

        protected float time;

        protected const float basinWidth = 120, basinLength = 120, waterHeight = -14.5f;
        
        protected static Texture2D noiseTex;
        protected static ContentManager content;

        public static void Initialize(ContentManager c)
        {
            content = c;
            noiseTex = content.Load<Texture2D>("textures/noise");
            RenderingDevice.GDM.DeviceCreated += delegate { noiseTex = content.Load<Texture2D>("textures/noise"); };
        }

        protected Theme(Effect shader, Model planeModel, Color textColor, Color titleColor, Texture2D shaderTex, BaseModel outer, BaseModel skybox)//, FluidVolume fluid)
        {
            RemovalHeight = -9f;
            this.planeModel = planeModel;
            //internalFluid = fluid;
            shaderTexture = shaderTex;
            internalTextColor = textColor;
            LevelTitleColor = titleColor;
            internalShader = shader;
            OuterModel = outer;
            SkyBox = skybox;

            setUpModel();
        }

        protected void setUpModel()
        {
            if(internalShader != null)
                foreach(ModelMesh mesh in planeModel.Meshes)
                    foreach(ModelMeshPart part in mesh.MeshParts)
                        part.Effect = internalShader;
            else if(planeModel != null)
                foreach(ModelMesh mesh in planeModel.Meshes)
                    foreach(ModelMeshPart part in mesh.MeshParts)
                        part.Effect = RenderingDevice.Shader;
        }

        public abstract void SetUpLighting();
        public abstract void InitializeShader();
        /// <summary>
        /// This updates the shader, and may include drawing.
        /// </summary>
        public abstract void UpdateShader(GameTime gameTime);

        public static void OnGDMReset()
        {
            onGDMReset();
        }

        protected class LavaTheme : Theme
        {
            public override string Name { get { return "Lava"; } }
            public override SongOptions Song { get { return SongOptions.Lava; } }

            public LavaTheme() :
                base(content.Load<Effect>("Shaders/lava"), content.Load<Model>("All Levels/plane"), Color.Moccasin, Color.Black,
                    content.Load<Texture2D>("textures/lava"),
                    Resources.lavaOuterModel,
                    Resources.lavaSkyboxModel)//,
                    //new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]    
                    //     {
                    //         new Vector3(-basinWidth / 2, -basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                    //         new Vector3(-basinWidth / 2, basinLength / 2, waterHeight)
                    //     }, new[]
                    //     {
                    //         new Vector3(-basinWidth / 2, basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                    //         new Vector3(basinWidth / 2, basinLength / 2, waterHeight)
                    //     } }, -waterHeight, .1f, 0.98f, 0.98f, GameManager.Space.BroadPhase.QueryAccelerator, GameManager.Space.ThreadManager))
            {
                onGDMReset += delegate { shaderTexture = content.Load<Texture2D>("textures/lava"); internalShader = content.Load<Effect>("Shaders/lava"); planeModel = content.Load<Model>("All Levels/plane"); setUpModel(); };
            }

            public override void SetUpLighting()
            {
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Lava);
            }

            public override void InitializeShader()
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                temp.Add("mWorldViewProj", RenderingDevice.Camera.WorldViewProj);
                temp.Add("time", 0);
                temp.Add("pos", new Vector3(-7, 0, 0));
                RenderingDevice.Add(PlaneModel, Shader, temp, ShaderTexture, noiseTex);
                //UpdateShader();
            }

            public override void UpdateShader(GameTime gameTime)
            {
                if(GameManager.State == GameState.Running && GameManager.Game.IsActive)
                    time += 0.0075f;
                RenderingDevice.SetShaderValues(PlaneModel,
                    new string[] { "mWorldViewProj", "time" }, new object[] { RenderingDevice.Camera.WorldViewProj, time });
            }
        }

        protected class IceTheme : Theme
        {
            public override string Name
            {
                get { return "Ice"; }
            }
            public override SongOptions Song
            {
                get { return SongOptions.Ice; }
            }

            public IceTheme()
                : base(content.Load<Effect>("Shaders/ice"), content.Load<Model>("All Levels/ice_plane"), 
                Color.LightSkyBlue, Color.GhostWhite, null, Resources.iceOuterModel, Resources.iceSkyboxModel)//,
                //null)
            {
                onGDMReset += delegate { internalShader = content.Load<Effect>("Shaders/ice"); planeModel = content.Load<Model>("All Levels/ice_plane"); setUpModel(); };
            }

            public override void InitializeShader()
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                temp.Add("xWorld", Matrix.Identity);
                temp.Add("xView", RenderingDevice.Camera.View);
                temp.Add("xProjection", RenderingDevice.Camera.Projection);
                temp.Add("xReflectionView", RenderingDevice.CreateReflectionMatrix(-13 - .5f));
                temp.Add("xReflectionMap", Resources.halfBlack);
                temp.Add("xIceBumpMap", Resources.WaterBumpMap);
                temp.Add("xWaveLength", 0.15f);
                temp.Add("xCamPos", RenderingDevice.Camera.Position);
                temp.Add("xLightDirection", RenderingDevice.Lights.LightDirection);
                temp.Add("xTexture", Resources.IceTexture);
                temp.Add("xEnableReflections", Input.WindowsOptions.FancyGraphics);

                RenderingDevice.Add(PlaneModel, Shader, temp);
            }

            public override void UpdateShader(GameTime gameTime)
            {
                float waterheight = -13;
                Plane reflectionPlane = RenderingDevice.CreatePlane(waterheight - 0.5f, -Vector3.UnitZ, true);
                Matrix reflectionMatrix = RenderingDevice.CreateReflectionMatrix(waterheight);

                Texture2D reflectionTex = Resources.halfBlack;
                if(Input.WindowsOptions.FancyGraphics)
                    reflectionTex = RenderingDevice.CreateReflectionMap(reflectionMatrix, reflectionPlane);

                RenderingDevice.SetShaderValues(PlaneModel,
                    new[]{ "xWorld", "xView", "xReflectionView", "xProjection", "xReflectionMap",  "xCamPos", "xLightDirection", "xEnableReflections" },
                    new object[]{ RenderingDevice.Camera.World, RenderingDevice.Camera.View, reflectionMatrix, RenderingDevice.Camera.Projection,
                        reflectionTex, RenderingDevice.Camera.Position, RenderingDevice.Lights.LightDirection, Input.WindowsOptions.FancyGraphics });
            }

            public override void SetUpLighting()
            {
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Ice);
            }
        }

        protected class GenericTheme : Theme
        {
            public override string Name
            {
                get { return "Generic"; }
            }
            public override SongOptions Song
            {
                get { return SongOptions.Generic; }
            }

            public GenericTheme()
                : base(null, null,
                    Color.Gray, Color.Red, null, Resources.genericOuterModel,
                    null)//, new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]
                         //{
                         //    new Vector3(-basinWidth / 2, waterHeight, -basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                         //    new Vector3(-basinWidth / 2, waterHeight, basinLength / 2)
                         //}, new[]
                         //{
                         //    new Vector3(-basinWidth / 2, waterHeight, basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                         //    new Vector3(basinWidth / 2, waterHeight, basinLength / 2)
                         //}}, -waterHeight, .3f, 0.7f, 0.95f, GameManager.Space.BroadPhase.QueryAccelerator, GameManager.Space.ThreadManager))
            {
                RemovalHeight = -8.5f;
                //RenderingDevice.GDM.DeviceCreated += delegate { shaderTexture = content.Load<Texture2D>("textures/blackTex"); };
            }

            public override void SetUpLighting()
            {
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Generic);
            }

            public override void InitializeShader() { }

            public override void UpdateShader(GameTime gameTime) { }
        }

        protected class SpaceTheme : Theme
        {
            public override string Name
            {
                get { return "Space"; }
            }
            public override SongOptions Song
            {
                get { return SongOptions.Space; }
            }

            public SpaceTheme()
                : base(null, null,
                    Color.Gray, Color.GhostWhite, null, Resources.spaceOuterModel,
                    Resources.spaceSkyboxModel)//, new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]
                         //{
                         //    new Vector3(-basinWidth / 2, waterHeight, -basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                         //    new Vector3(-basinWidth / 2, waterHeight, basinLength / 2)
                         //}, new[]
                         //{
                         //    new Vector3(-basinWidth / 2, waterHeight, basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                         //    new Vector3(basinWidth / 2, waterHeight, basinLength / 2)
                         //}}, -waterHeight, .3f, 0.7f, 0.95f, GameManager.Space.BroadPhase.QueryAccelerator, GameManager.Space.ThreadManager))
            {
                //RenderingDevice.GDM.DeviceCreated += delegate { shaderTexture = content.Load<Texture2D>("textures/blackTex"); };
            }

            public override void SetUpLighting()
            {
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Space);
            }

            public override void InitializeShader() { }

            public override void UpdateShader(GameTime gameTime) { }
        }

        protected class SkyTheme : Theme
        {
            public override string Name
            {
                get { return "Sky"; }
            }
            public override SongOptions Song
            {
                get { return SongOptions.Sky; }
            }

            public SkyTheme()
                : base(null, null, Color.DodgerBlue, Color.Black, null, Resources.skyOuterModel,
                    Resources.skySkyboxModel)//, null)
            {
                RemovalHeight = -30f;
            }

            public override void SetUpLighting()
            {
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Sky);
            }

            public override void InitializeShader()
            {
                //Dictionary<string, object> temp = new Dictionary<string, object>();
                //temp.Add("mWorldViewProj", RenderingDevice.Camera.WorldViewProj);
                //temp.Add("time", 0);
                //temp.Add("pos", new Vector3(-7, 0, 0));
                //RenderingDevice.Add(PlaneModel, Shader, temp, ShaderTexture);
                //UpdateShader();
            }

            public override void UpdateShader(GameTime gameTime)
            {
                //time += 0.0075f;
                //RenderingDevice.SetShaderValues(PlaneModel,
                //    new string[] { "mWorldViewProj", "time" }, new object[] { RenderingDevice.Camera.WorldViewProj, time });
            }
        }

        protected class BeachTheme : Theme
        {
            public override string Name { get { return "Beach"; } }
            public override SongOptions Song { get { return SongOptions.Beach; } }

            public BeachTheme() :
                base(content.Load<Effect>("shaders/water"), content.Load<Model>("All Levels/beach_plane"),
                    Color.DeepSkyBlue, Color.Black, content.Load<Texture2D>("textures/beach"), Resources.beachOuterModel,
                    Resources.beachSkyboxModel)//,
                //new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]    
                //         {
                //             new Vector3(-basinWidth / 2, -basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                //             new Vector3(-basinWidth / 2, basinLength / 2, waterHeight)
                //         }, new[]
                //         {
                //             new Vector3(-basinWidth / 2, basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                //             new Vector3(basinWidth / 2, basinLength / 2, waterHeight)
                //         } }, -waterHeight, .1f, 0.98f, 0.98f, GameManager.Space.BroadPhase.QueryAccelerator, GameManager.Space.ThreadManager))
            {
                RemovalHeight = -12f;
                onGDMReset += delegate { internalShader = content.Load<Effect>("Shaders/water"); planeModel = content.Load<Model>("All Levels/beach_plane"); shaderTexture = content.Load<Texture2D>("textures/beach"); setUpModel(); };
            }

            public override void SetUpLighting()
            {
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Beach);
            }

            public override void InitializeShader()
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                temp.Add("xWorld", Matrix.Identity);
                temp.Add("xView", RenderingDevice.Camera.View);
                temp.Add("xProjection", RenderingDevice.Camera.Projection);
                temp.Add("xReflectionView", RenderingDevice.CreateReflectionMatrix(-15 - .5f));
                temp.Add("xRefractionMap", Resources.halfBlack);
                temp.Add("xReflectionMap", Resources.halfBlack);
                temp.Add("xWaterBumpMap", Resources.WaterBumpMap);
                temp.Add("xWaveHeight", 0.4f);
                temp.Add("xWaveLength", 0.1f);
                temp.Add("xCamPos", RenderingDevice.Camera.Position);
                temp.Add("xTime", 0);
                temp.Add("xWindForce", 0.01f);
                temp.Add("xWindDirection", -Vector3.UnitY);
                temp.Add("xLightDirection", RenderingDevice.Lights.LightDirection);
                temp.Add("xEnableReflections", Input.WindowsOptions.FancyGraphics);
                temp.Add("xColorMap", Resources.BeachTexture);
                RenderingDevice.Add(PlaneModel, Shader, temp);
            }

            public override void UpdateShader(GameTime gameTime)
            {
                if(GameManager.State == GameState.Running && GameManager.Game.IsActive)
                    time += 0.0005f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                float waterheight = -15;
                Plane refractionPlane = RenderingDevice.CreatePlane(waterheight + 1.5f, -Vector3.UnitZ, false);
                Plane reflectionPlane = RenderingDevice.CreatePlane(waterheight - 0.5f, -Vector3.UnitZ, true);
                Matrix reflectionMatrix = RenderingDevice.CreateReflectionMatrix(waterheight);

                Texture2D refractionTex, reflectionTex;
                refractionTex = reflectionTex = Resources.halfBlack;
                if(Input.WindowsOptions.FancyGraphics)
                {
                    reflectionTex = RenderingDevice.CreateReflectionMap(reflectionMatrix, reflectionPlane);
                    refractionTex = RenderingDevice.CreateRefractionMap(refractionPlane);
                    //System.IO.Stream s = System.IO.File.Open(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Accelerated Delivery/reflect.png", System.IO.FileMode.Create);
                    //reflectionTex.SaveAsPng(s, reflectionTex.Width, reflectionTex.Height);
                    //s.Close();
                }

                RenderingDevice.SetShaderValues(PlaneModel,
                    new[]{ "xWorld", "xView", "xReflectionView", "xProjection", "xReflectionMap", "xRefractionMap", "xCamPos", "xTime",
                            "xLightDirection", "xEnableReflections" },
                    new object[]{ RenderingDevice.Camera.World, RenderingDevice.Camera.View, reflectionMatrix, RenderingDevice.Camera.Projection,
                        reflectionTex, refractionTex, RenderingDevice.Camera.Position, time, RenderingDevice.Lights.LightDirection, Input.WindowsOptions.FancyGraphics });
            }
        }

        #region junk
        public static bool operator==(Theme lhs, Theme rhs)
        {
            return lhs.Name == rhs.Name;
        }
        public static bool operator !=(Theme lhs, Theme rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
