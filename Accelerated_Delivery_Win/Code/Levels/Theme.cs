using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysics.UpdateableSystems;

namespace Accelerated_Delivery_Win
{
    public abstract class Theme
    {
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
        public abstract MediaSystem.SongOptions Song { get; }
        public abstract string Name { get; }

        protected float time;

        protected const float basinWidth = 120, basinLength = 120, waterHeight = -14.5f;
        
        protected readonly static Texture2D noiseTex;
        static Theme()
        {
            noiseTex = Program.Game.Content.Load<Texture2D>("textures/noise");
        }

        protected Theme(Effect shader, Model planeModel, Color textColor, Color titleColor, Texture2D shaderTex, BaseModel outer, BaseModel skybox, FluidVolume fluid)
        {
            RemovalHeight = -10.479f;
            this.planeModel = planeModel;
            internalFluid = fluid;
            shaderTexture = shaderTex;
            internalTextColor = textColor;
            LevelTitleColor = titleColor;
            internalShader = shader;
            if(internalShader != null)
                foreach(ModelMesh mesh in planeModel.Meshes)
                    foreach(ModelMeshPart part in mesh.MeshParts)
                        part.Effect = internalShader;
            else if(planeModel != null)
                foreach(ModelMesh mesh in planeModel.Meshes)
                    foreach(ModelMeshPart part in mesh.MeshParts)
                        part.Effect = RenderingDevice.Shader;
            OuterModel = outer;
            SkyBox = skybox;
        }

        public abstract void SetUpLighting();
        public abstract void InitializeShader();
        /// <summary>
        /// This updates the shader, and may include drawing.
        /// </summary>
        public abstract void UpdateShader(GameTime gameTime);

        protected class LavaTheme : Theme
        {
            public override string Name { get { return "Lava"; } }
            public override MediaSystem.SongOptions Song { get { return MediaSystem.SongOptions.Lava; } }

            public LavaTheme() :
                base(Program.Game.Content.Load<Effect>("Shaders/lava"), Program.Game.Content.Load<Model>("All Levels/plane"), Color.Moccasin, Color.Black,
                    Program.Game.Content.Load<Texture2D>("textures/lava"),
                    Program.Game.LoadingScreen == null ? Program.Game.Loader.lavaOuterModel : Program.Game.LoadingScreen.loader.lavaOuterModel,
                    Program.Game.LoadingScreen == null ? Program.Game.Loader.lavaSkyboxModel : Program.Game.LoadingScreen.loader.lavaSkyboxModel,
                    new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]    
                         {
                             new Vector3(-basinWidth / 2, -basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                             new Vector3(-basinWidth / 2, basinLength / 2, waterHeight)
                         }, new[]
                         {
                             new Vector3(-basinWidth / 2, basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                             new Vector3(basinWidth / 2, basinLength / 2, waterHeight)
                         } }, -waterHeight, .1f, 0.98f, 0.98f, Program.Game.Space.BroadPhase.QueryAccelerator, Program.Game.Space.ThreadManager))
            { }

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
                if(Program.Game.State == BaseGame.GameState.Running && Program.Game.IsActive)
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
            public override MediaSystem.SongOptions Song
            {
                get { return MediaSystem.SongOptions.Ice; }
            }

            public IceTheme()
                : base(Program.Game.Content.Load<Effect>("Shaders/ice"), Program.Game.Content.Load<Model>("All Levels/ice_plane"), 
                Color.LightSkyBlue, Color.GhostWhite, null, Program.Game.LoadingScreen.loader.iceOuterModel, Program.Game.LoadingScreen.loader.iceSkyboxModel,
                null)
            { }

            public override void InitializeShader()
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                temp.Add("xWorld", Matrix.Identity);
                temp.Add("xView", RenderingDevice.Camera.View);
                temp.Add("xProjection", RenderingDevice.Camera.Projection);
                temp.Add("xReflectionView", RenderingDevice.CreateReflectionMatrix(-13 - .5f));
                temp.Add("xReflectionMap", Program.Game.Loader.halfBlack);
                temp.Add("xIceBumpMap", Program.Game.Loader.WaterBumpMap);
                temp.Add("xWaveLength", 0.15f);
                temp.Add("xCamPos", RenderingDevice.Camera.Position);
                temp.Add("xLightDirection", RenderingDevice.Lights.LightDirection);
                temp.Add("xTexture", Program.Game.Loader.IceTexture);
                temp.Add("xEnableReflections", Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics);

                RenderingDevice.Add(PlaneModel, Shader, temp);
            }

            public override void UpdateShader(GameTime gameTime)
            {
                float waterheight = -13;
                Plane reflectionPlane = RenderingDevice.CreatePlane(waterheight - 0.5f, -Vector3.UnitZ, true);
                Matrix reflectionMatrix = RenderingDevice.CreateReflectionMatrix(waterheight);

                Texture2D reflectionTex = Program.Game.Loader.halfBlack;
                if(Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics)
                    reflectionTex = RenderingDevice.CreateReflectionMap(reflectionMatrix, reflectionPlane);

                RenderingDevice.SetShaderValues(PlaneModel,
                    new[]{ "xWorld", "xView", "xReflectionView", "xProjection", "xReflectionMap",  "xCamPos", "xLightDirection", "xEnableReflections" },
                    new object[]{ RenderingDevice.Camera.World, RenderingDevice.Camera.View, reflectionMatrix, RenderingDevice.Camera.Projection,
                        reflectionTex, RenderingDevice.Camera.Position, RenderingDevice.Lights.LightDirection, Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics });
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
            public override MediaSystem.SongOptions Song
            {
                get { return MediaSystem.SongOptions.Generic; }
            }

            public GenericTheme()
                : base(null, null,
                    Color.Gray, Color.Red, Program.Game.Content.Load<Texture2D>("textures/blackTex"), Program.Game.LoadingScreen.loader.genericOuterModel,
                    Program.Game.LoadingScreen.loader.genericSkyboxModel, new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]
                         {
                             new Vector3(-basinWidth / 2, waterHeight, -basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                             new Vector3(-basinWidth / 2, waterHeight, basinLength / 2)
                         }, new[]
                         {
                             new Vector3(-basinWidth / 2, waterHeight, basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                             new Vector3(basinWidth / 2, waterHeight, basinLength / 2)
                         }}, -waterHeight, .3f, 0.7f, 0.95f, Program.Game.Space.BroadPhase.QueryAccelerator, Program.Game.Space.ThreadManager))
            {
                RemovalHeight = -13;
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
            public override MediaSystem.SongOptions Song
            {
                get { return MediaSystem.SongOptions.Space; }
            }

            public SpaceTheme()
                : base(null, null,
                    Color.Gray, Color.GhostWhite, Program.Game.Content.Load<Texture2D>("textures/blackTex"), Program.Game.LoadingScreen.loader.spaceOuterModel,
                    Program.Game.LoadingScreen.loader.spaceSkyboxModel, new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]
                         {
                             new Vector3(-basinWidth / 2, waterHeight, -basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                             new Vector3(-basinWidth / 2, waterHeight, basinLength / 2)
                         }, new[]
                         {
                             new Vector3(-basinWidth / 2, waterHeight, basinLength / 2), new Vector3(basinWidth / 2, waterHeight, -basinLength / 2),
                             new Vector3(basinWidth / 2, waterHeight, basinLength / 2)
                         }}, -waterHeight, .3f, 0.7f, 0.95f, Program.Game.Space.BroadPhase.QueryAccelerator, Program.Game.Space.ThreadManager))
            { }

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
            public override MediaSystem.SongOptions Song
            {
                get { return MediaSystem.SongOptions.Sky; }
            }

            public SkyTheme()
                : base(Program.Game.Content.Load<Effect>("Shaders/sky"), Program.Game.Content.Load<Model>("All Levels/sky_plane"),
                    Color.DodgerBlue, Color.Black, Program.Game.Content.Load<Texture2D>("textures/cloudstest2"), Program.Game.LoadingScreen.loader.skyOuterModel,
                    Program.Game.LoadingScreen.loader.skySkyboxModel, null)
            {
                RemovalHeight = -20f;
            }

            public override void SetUpLighting()
            {
                RenderingDevice.SetUpLighting(RenderingDevice.LightingData.Sky);
            }

            public override void InitializeShader()
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                temp.Add("mWorldViewProj", RenderingDevice.Camera.WorldViewProj);
                temp.Add("time", 0);
                temp.Add("pos", new Vector3(-7, 0, 0));
                RenderingDevice.Add(PlaneModel, Shader, temp, ShaderTexture);
                //UpdateShader();
            }

            public override void UpdateShader(GameTime gameTime)
            {
                time += 0.0075f;
                RenderingDevice.SetShaderValues(PlaneModel,
                    new string[] { "mWorldViewProj", "time" }, new object[] { RenderingDevice.Camera.WorldViewProj, time });
            }
        }

        protected class BeachTheme : Theme
        {
            public override string Name { get { return "Beach"; } }
            public override MediaSystem.SongOptions Song { get { return MediaSystem.SongOptions.Beach; } }

            public BeachTheme() :
                base(Program.Game.Content.Load<Effect>("shaders/water"), Program.Game.Content.Load<Model>("All Levels/beach_plane"),
                    Color.DeepSkyBlue, Color.Black, Program.Game.Content.Load<Texture2D>("textures/beach"), Program.Game.LoadingScreen.loader.beachOuterModel,
                    Program.Game.LoadingScreen.loader.beachSkyboxModel,
                new FluidVolume(Vector3.UnitZ, -9.81f, new List<Vector3[]>() { new[]    
                         {
                             new Vector3(-basinWidth / 2, -basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                             new Vector3(-basinWidth / 2, basinLength / 2, waterHeight)
                         }, new[]
                         {
                             new Vector3(-basinWidth / 2, basinLength / 2, waterHeight), new Vector3(basinWidth / 2, -basinLength / 2, waterHeight),
                             new Vector3(basinWidth / 2, basinLength / 2, waterHeight)
                         } }, -waterHeight, .1f, 0.98f, 0.98f, Program.Game.Space.BroadPhase.QueryAccelerator, Program.Game.Space.ThreadManager))
            {
                RemovalHeight = -12f;
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
                temp.Add("xRefractionMap", Program.Game.Loader.halfBlack);
                temp.Add("xReflectionMap", Program.Game.Loader.halfBlack);
                temp.Add("xWaterBumpMap", Program.Game.Loader.WaterBumpMap);
                temp.Add("xWaveHeight", 0.4f);
                temp.Add("xWaveLength", 0.1f);
                temp.Add("xCamPos", RenderingDevice.Camera.Position);
                temp.Add("xTime", 0);
                temp.Add("xWindForce", 0.01f);
                temp.Add("xWindDirection", -Vector3.UnitY);
                temp.Add("xLightDirection", RenderingDevice.Lights.LightDirection);
                temp.Add("xEnableReflections", Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics);
                temp.Add("xColorMap", Program.Game.Loader.BeachTexture);
                RenderingDevice.Add(PlaneModel, Shader, temp);
            }

            public override void UpdateShader(GameTime gameTime)
            {
                if(Program.Game.State == BaseGame.GameState.Running && Program.Game.IsActive)
                    time += 0.0005f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                float waterheight = -15;
                Plane refractionPlane = RenderingDevice.CreatePlane(waterheight + 1.5f, -Vector3.UnitZ, false);
                Plane reflectionPlane = RenderingDevice.CreatePlane(waterheight - 0.5f, -Vector3.UnitZ, true);
                Matrix reflectionMatrix = RenderingDevice.CreateReflectionMatrix(waterheight);

                Texture2D refractionTex, reflectionTex;
                refractionTex = reflectionTex = Program.Game.Loader.halfBlack;
                if(Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics)
                {
                    refractionTex = RenderingDevice.CreateRefractionMap(refractionPlane);
                    reflectionTex = RenderingDevice.CreateReflectionMap(reflectionMatrix, reflectionPlane);
                }

                RenderingDevice.SetShaderValues(PlaneModel,
                    new[]{ "xWorld", "xView", "xReflectionView", "xProjection", "xReflectionMap", "xRefractionMap", "xCamPos", "xTime",
                            "xLightDirection", "xEnableReflections" },
                    new object[]{ RenderingDevice.Camera.World, RenderingDevice.Camera.View, reflectionMatrix, RenderingDevice.Camera.Projection,
                        reflectionTex, refractionTex, RenderingDevice.Camera.Position, time, RenderingDevice.Lights.LightDirection, Program.Game.Manager.CurrentSaveWindowsOptions.FancyGraphics });
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
