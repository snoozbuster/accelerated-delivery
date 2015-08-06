using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using DPSF;

namespace Accelerated_Delivery_Win
{
    public static class RenderingDevice
    {
        private static readonly List<ModelData> texturedModels = new List<ModelData>();
        private static readonly Dictionary<BaseModel, bool> untexturedModels = new Dictionary<BaseModel, bool>();
        private static readonly List<ModelData> glassModels = new List<ModelData>();
        private static readonly List<PrimData> primitives = new List<PrimData>();
        private static readonly List<ShaderModel> shaderModels = new List<ShaderModel>();
        public static MyCamera Camera { get; private set; }
        private static LightingSystem lights;
        public static LightingSystem Lights { get { return lights; } }
        private static RenderTarget2D renderTarget;
        //private static Texture2D shadowMap;

        private static Dictionary<Model, List<Texture2D>> masterDict = new Dictionary<Model, List<Texture2D>>();

        //private static List<Texture2D> normalBoxTextures;
        //private static List<Texture2D> blueBoxTextures;
        //private static List<Texture2D> blackBoxTextures;
        //private static List<Texture2D> dispenserTextures;
        //private static List<Texture2D> stripesTextures;
        //private static List<Texture2D> laserTextures;
        //private static List<Texture2D> glassTextures;

        public static Effect Shader { get { return lights.Shader; } }

        private static readonly ParticleSystemManager manager = new ParticleSystemManager();

        private class Device : BoxCutter.INameable { string BoxCutter.INameable.Name { get { return "RenderingDevice"; } } }
        private static Device d = new Device();

        static RenderingDevice()
        {
            onGDMReset(new object(), EventArgs.Empty);
            Program.Game.GDM.DeviceReset += onGDMReset;
        }

        public static void Ready(Effect shader, Texture2D texture)
        {
            lights = new LightingSystem(shader, texture);
            Camera = new MyCamera(MathHelper.PiOver4, Program.Game.AspectRatio, 1f, 10000f);
        }

        #region add
        public static void Add(BaseModel m)
        {
            try
            {
                if(m == null || m.Model == null)
                {
                    Program.Cutter.WriteToLog(d, "Warning: Add(m) was called with a null parameter. Ensure this is correct behavior.");
                    return;
                }

                if(!m.RenderAsGlass)
                {
                    foreach(ModelData d in texturedModels)
                        if(d == m)
                        {
                            d.MakeActive();
                            return;
                        }
                    if(untexturedModels.ContainsKey(m))
                    {
                        untexturedModels[m] = true;
                        return;
                    }
                }
                else
                    foreach(ModelData d in glassModels)
                        if(d == m)
                        {
                            d.MakeActive();
                            return;
                        }

                List<Texture2D> list;
                BasicEffect currentEffect;

                //if(m is Box && (m as Box).BoxColor == Color.White && normalBoxTextures != null)
                //    list = normalBoxTextures;
                //else if(m is Box && (m as Box).BoxColor == Color.Blue && blueBoxTextures != null)
                //    list = blueBoxTextures;
                //else if(m is BlackBox && blackBoxTextures != null)
                //    list = blackBoxTextures;
                //else if(m.Model == Program.Game.Loader.Dispenser && dispenserTextures != null)
                //    list = dispenserTextures;
                //else if(m.Model.Meshes[0].Name == "Plane_424" && stripesTextures != null)
                //    list = stripesTextures;
                //else if(m.Model.Meshes[0].Name == "Circle_024" && laserTextures != null)
                //    list = laserTextures;
                //else if(((m.Model.Meshes[0].Name == "Plane_589" || m.Model.Meshes[0].Name == "Plane_027" || (m.Model.Meshes[0].Name == "Plane_591")) && !(m.Model.Meshes[0].Effects[0] is BasicEffect)) && glassTextures != null)
                //    list = glassTextures;
                //else
                //{
                list = new List<Texture2D>();
                foreach(ModelMesh mesh in m.Model.Meshes)
                {
                    bool alreadyConverted = false;
                    for(int i = 0; i < mesh.Effects.Count; i++)
                    {
                        currentEffect = mesh.Effects[i] as BasicEffect;
                        if(currentEffect != null)
                            list.Add(currentEffect.Texture);
                        else
                        {
                            list.AddRange(masterDict[m.Model]);
                            alreadyConverted = true;
                            break;
                        }
                    }

                    if(!alreadyConverted)
                    {
                        if(!masterDict.ContainsKey(m.Model))
                            masterDict.Add(m.Model, list);
                        foreach(ModelMeshPart meshPart in mesh.MeshParts)
                            meshPart.Effect = lights.Shader.Clone();
                    }
                    //if(m is Box && !(m is BlackBox))
                    //{
                    //    if((m as Box).BoxColor == Color.White)
                    //        normalBoxTextures = list;
                    //    else if((m as Box).BoxColor == Color.Blue)
                    //        blueBoxTextures = list;
                    //}
                    //else if(m is BlackBox)
                    //    blackBoxTextures = list;
                    //else if(m.Model == Program.Game.Loader.Dispenser)
                    //    dispenserTextures = list;
                    //else if(m.Model.Meshes[0].Name == "Plane_424")
                    //    stripesTextures = list;
                    //else if(m.Model.Meshes[0].Name == "Circle_024")
                    //    laserTextures = list;
                    //else if(m.Model.Meshes[0].Name == "Plane_589" || m.Model.Meshes[0].Name == "Plane_027" || m.Model.Meshes[0].Name == "Plane_591")
                    //    if(glassTextures == null)
                    //        glassTextures = list;
                }
                //}

                if((list.Count == 0) || (list.Count > 0 && list[0] == null))
                    untexturedModels.Add(m, true);
                else if(m.RenderAsGlass)
                    glassModels.Add(new ModelData(m, list));
                else
                    texturedModels.Add(new ModelData(m, list));
            }
            catch(OutOfMemoryException)
            {
                foreach(BaseModel b in untexturedModels.Keys)
                    if(!untexturedModels[b])
                        untexturedModels.Remove(b);
                for(int i = 0; i < texturedModels.Count; i++)
                    if(!texturedModels[i].IsActive)
                        texturedModels.RemoveAt(i);
                for(int i = 0; i < primitives.Count; i++)
                    if(!primitives[i].IsActive)
                        primitives.RemoveAt(i);
                for(int i = 0; i < glassModels.Count; i++)
                    if(!glassModels[i].IsActive)
                        glassModels.RemoveAt(i);
                for(int i = 0; i < shaderModels.Count; i++)
                    if(!shaderModels[i].IsActive)
                        shaderModels.RemoveAt(i);
            }
        }

        public static void Add(BaseModel m, List<Texture2D> textures)
        {
            try
            {
                if(m == null || textures == null || m.Model == null)// || textures.Count == 0)
                {
                    Program.Cutter.WriteToLog(d, "Warning: Add(m, textures) was called with a null parameter. Ensure this is correct behavior.");
                    return;
                }

                foreach(ModelData d in texturedModels)
                    if(d == m)
                    {
                        d.MakeActive();
                        return;
                    }
                texturedModels.Add(new ModelData(m, textures));
            }
            catch(OutOfMemoryException)
            {
                foreach(BaseModel b in untexturedModels.Keys)
                    if(!untexturedModels[b])
                        untexturedModels.Remove(b);
                for(int i = 0; i < texturedModels.Count; i++)
                    if(!texturedModels[i].IsActive)
                        texturedModels.RemoveAt(i);
                for(int i = 0; i < primitives.Count; i++)
                    if(!primitives[i].IsActive)
                        primitives.RemoveAt(i);
                for(int i = 0; i < glassModels.Count; i++)
                    if(!glassModels[i].IsActive)
                        glassModels.RemoveAt(i);
                for(int i = 0; i < shaderModels.Count; i++)
                    if(!shaderModels[i].IsActive)
                        shaderModels.RemoveAt(i);
            }
        }

        public static void Add(Model m, Effect shader, Dictionary<string, object> objects, params Texture2D[] samplers)
        {
            try
            {
                if(m == null || shader == null || objects == null)
                {
                    Program.Cutter.WriteToLog(d, "Warning: Add(m, shader, objects, samplers) was called with a null parameter. Ensure this is correct behavior.");
                    return;
                }

                foreach(ShaderModel s in shaderModels)
                    if(s == m)
                    {
                        s.MakeActive();
                        return;
                    }
                shaderModels.Add(new ShaderModel(m, shader, objects, samplers));
            }
            catch(OutOfMemoryException)
            {
                foreach(BaseModel b in untexturedModels.Keys)
                    if(!untexturedModels[b])
                        untexturedModels.Remove(b);
                for(int i = 0; i < texturedModels.Count; i++)
                    if(!texturedModels[i].IsActive)
                        texturedModels.RemoveAt(i);
                for(int i = 0; i < primitives.Count; i++)
                    if(!primitives[i].IsActive)
                        primitives.RemoveAt(i);
                for(int i = 0; i < glassModels.Count; i++)
                    if(!glassModels[i].IsActive)
                        glassModels.RemoveAt(i);
                for(int i = 0; i < shaderModels.Count; i++)
                    if(!shaderModels[i].IsActive)
                        shaderModels.RemoveAt(i);
            }
        }

        public static void Add(VertexBuffer vertBuffer, Texture2D texture)
        {
            try
            {
                if(vertBuffer == null || texture == null)
                {
                    Program.Cutter.WriteToLog(d, "Warning: Add(vertBuffer, texture) was called with a null parameter. Ensure this is correct behavior.");
                    return;
                }

                foreach(PrimData d in primitives)
                    if(d == vertBuffer)
                    {
                        d.MakeActive();
                        return;
                    }

                primitives.Add(new PrimData(vertBuffer, texture));
            }
            catch(OutOfMemoryException)
            {
                foreach(BaseModel b in untexturedModels.Keys)
                    if(!untexturedModels[b])
                        untexturedModels.Remove(b);
                for(int i = 0; i < texturedModels.Count; i++)
                    if(!texturedModels[i].IsActive)
                        texturedModels.RemoveAt(i);
                for(int i = 0; i < primitives.Count; i++)
                    if(!primitives[i].IsActive)
                        primitives.RemoveAt(i);
                for(int i = 0; i < glassModels.Count; i++)
                    if(!glassModels[i].IsActive)
                        glassModels.RemoveAt(i);
                for(int i = 0; i < shaderModels.Count; i++)
                    if(!shaderModels[i].IsActive)
                        shaderModels.RemoveAt(i);
            }
        }

        public static void Add(IDPSFParticleSystem system)
        {
            if(system == null)
            {
                Program.Cutter.WriteToLog(d, "Warning: Add(system) was called with a null parameter. Ensure this is correct behavior.");
                return;
            }

            if(!manager.ContainsParticleSystem(system))
            {
                manager.AddParticleSystem(system);
            }
        }

        public static void Add(IRenderableObject obj)
        {
            obj.AddToRenderer();
        }
        #endregion

        #region remove
        public static void Remove(Model m)
        {
            if(m == null)
                return;

            foreach(ShaderModel s in shaderModels)
                if(s == m)
                {
                    s.MakeInactive();
                    return;
                }
        }

        public static void Remove(BaseModel m)
        {
            if(m == null || m.Model == null)
                return;

            if(untexturedModels.ContainsKey(m))
            {
                untexturedModels[m] = false;
                return;
            }
            foreach(ModelData d in texturedModels)
                if(d == m)
                {
                    d.MakeInactive();
                    return;
                }
            foreach(ModelData d in glassModels)
                if(d == m)
                {
                    d.MakeInactive();
                    return;
                }
        }

        public static void Remove(VertexBuffer vertBuffer)
        {
            if(vertBuffer == null)
                return;

            foreach(PrimData d in primitives)
                if(d == vertBuffer)
                {
                    d.MakeInactive();
                    return;
                }
        }

        public static void RemovePermanent(BaseModel m)
        {
            if(m == null || m.Model == null)
                return;

            if(untexturedModels.ContainsKey(m))
            {
                untexturedModels.Remove(m);
                return;
            }
            foreach(ModelData d in texturedModels)
                if(d == m)
                {
                    texturedModels.Remove(d);
                    return;
                }
            foreach(ModelData d in glassModels)
                if(d == m)
                {
                    glassModels.Remove(d);
                    return;
                }
        }

        public static void RemovePermanent(Model m)
        {
            if(m == null)
                return;

            foreach(ShaderModel s in shaderModels)
                if(s == m)
                {
                    shaderModels.Remove(s);
                    return;
                }
        }

        public static void Remove(IDPSFParticleSystem system)
        {
            if(system == null)
                return;

            if(manager.ContainsParticleSystem(system))
            {
                system.RemoveAllParticles();
                manager.RemoveParticleSystem(system);
            }
        }

        public static void Remove(IRenderableObject obj)
        {
            obj.RemoveFromRenderer();
        }
        #endregion

        public static bool Contains(BaseModel m)
        {
            if(m == null || m.Model == null)
                return false;

            foreach(ModelData model in texturedModels)
                if(model.Model == m)
                    return true;
            foreach(KeyValuePair<BaseModel, bool> k in untexturedModels)
                if(k.Key == m)
                    return true;
            return false;
        }

        public static void SetShaderValues(Model m, string[] keys, object[] values)
        {
            if(m == null || keys == null || values == null)
            {
                Program.Cutter.WriteToLog(d, "Warning: SetShaderValues() was called with a null parameter. Ensure this is correct behavior.");
                return;
            }

            foreach(ShaderModel s in shaderModels)
                if(s == m)
                    s.SetValues(keys, values);
        }

        public static void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);
            manager.SetWorldViewProjectionMatricesForAllParticleSystems(Camera.World, Camera.View, Camera.Projection);
            manager.SetCameraPositionForAllParticleSystems(Camera.Position);
            manager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public static void UnloadContent()
        {
            manager.DestroyAndRemoveAllParticleSystems();
        }

        public static void SetUpLighting(LightingData l)
        {
            lights.SetUpLighting(l.LightPower, l.AmbientPower, l.Position, l.Target, l.LightColor);
        }
        public static void SetUpLighting(LightingData l, Texture2D tex)
        {
            lights.SetUpLighting(l.LightPower, l.AmbientPower, l.Position, l.Target, tex, l.LightColor);
        }

        public static void Draw()
        {
            Program.Game.GraphicsDevice.Clear(Color.Black);
            draw(null, Camera.View);
        }

        private static void onGDMReset(object sender, EventArgs e)
        {
            PresentationParameters pp = Program.Game.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(Program.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, Program.Game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
        }

        private static void draw(Plane? clipPlane, Matrix view)
        {
            Program.Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Program.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Program.Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Program.Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            Program.Game.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

            foreach(ModelData m in texturedModels)
                if(m.IsActive && !m.Model.UseCustomAlpha)
                    drawSingleModel(m.Model, m.Textures, "ShadowedScene", clipPlane, view, null);
            foreach(KeyValuePair<BaseModel, bool> key in untexturedModels)
                if(key.Value && !key.Key.UseCustomAlpha)
                    drawSingleModel(key.Key, null, "ShadowedSceneColor", clipPlane, view, null);
            foreach(PrimData p in primitives)
            {
                if(p.IsActive)
                {
                    lights.Shader.CurrentTechnique = lights.Shader.Techniques["ShadowedScene"];
                    lights.Shader.Parameters["xCamerasViewProjection"].SetValue(view * Camera.Projection);
                    lights.Shader.Parameters["xLightsViewProjection"].SetValue(lights.ViewProjection);
                    lights.Shader.Parameters["xTexture"].SetValue(p.Texture);
                    lights.Shader.Parameters["xWorld"].SetValue(Matrix.Identity);
                    lights.Shader.Parameters["xLightPos"].SetValue(lights.LightPosition);
                    lights.Shader.Parameters["xLightPower"].SetValue(lights.LightPower);
                    lights.Shader.Parameters["xAmbient"].SetValue(lights.AmbientPower);
                    //lights.Shader.Parameters["xCarLightTexture"].SetValue(lights.LightMap);
                    lights.Shader.Parameters["xColor"].SetValue(lights.LightColor);
                    if(clipPlane.HasValue)
                    {
                        lights.Shader.Parameters["xEnableClipping"].SetValue(true);
                        lights.Shader.Parameters["xClipPlane"].SetValue(new Vector4(clipPlane.Value.Normal, clipPlane.Value.D));
                    }
                    else
                        lights.Shader.Parameters["xEnableClipping"].SetValue(false);
                    lights.Shader.Parameters["xEnableCustomAlpha"].SetValue(false);
                    //lights.Shader.Parameters["xModelPos"].SetValue(Vector3.Zero); // generally we don't want prims to be transparent
                    //lights.Shader.Parameters["xFarPlane"].SetValue(CameraFarPlane);
                    //lights.Shader.Parameters["xCamPos"].SetValue(Camera.Position);

                    foreach(EffectPass pass in lights.Shader.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Program.Game.GraphicsDevice.SetVertexBuffer(p.Buffer);
                        Program.Game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    }
                }
            }
#if DEBUG
            drawAxes();
            if(Camera.debugCamera)
                foreach(Entity e in Program.Game.Space.Entities)
                    e.CollisionInformation.BoundingBox.Draw();
#endif
            Program.Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            if(!clipPlane.HasValue)
                foreach(ShaderModel s in shaderModels)
                    if(s.IsActive)
                    {
                        for(int i = 0; i < s.Samplers.Length; i++)
                            Program.Game.GraphicsDevice.Textures[i] = s.Samplers[i];
                        foreach(ModelMesh mesh in s.Model.Meshes)
                            foreach(Effect e in mesh.Effects)
                                mesh.Draw();
                    }

            Program.Game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            foreach(ModelData m in glassModels)
                if(m.IsActive)
                    drawSingleModel(m.Model, m.Textures, "ShadowedScene", clipPlane, view, m.Model.UseCustomAlpha ? m.Model.CustomAlpha : (float?)null);
            foreach(ModelData m in texturedModels)
                if(m.IsActive && m.Model.UseCustomAlpha)
                    drawSingleModel(m.Model, m.Textures, "ShadowedScene", clipPlane, view, m.Model.CustomAlpha);
            foreach(KeyValuePair<BaseModel, bool> key in untexturedModels)
                if(key.Value && key.Key.UseCustomAlpha)
                    drawSingleModel(key.Key, null, "ShadowedSceneColor", clipPlane, view, key.Key.CustomAlpha);

            manager.DrawAllParticleSystems();
        }

        private static void drawSingleModel(BaseModel model, List<Texture2D> textures, string tech, Plane? clipPlane, Matrix view, float? customAlpha)
        {
            if(model.IsInvisible)
                return;
            Matrix[] transforms = new Matrix[model.Model.Bones.Count];
            Matrix entityWorld = Matrix.Identity;
            if(!model.IsTerrain)
                entityWorld = model.Ent.CollisionInformation.WorldTransform.Matrix;
            model.Model.CopyAbsoluteBoneTransformsTo(transforms);
            int i = 0;
            foreach(ModelMesh mesh in model.Model.Meshes)
            {
                foreach(Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques[tech];

                    if(textures != null)
                        currentEffect.Parameters["xTexture"].SetValue(textures[i++]);

                    currentEffect.Parameters["xCamerasViewProjection"].SetValue(view * Camera.Projection);
                    currentEffect.Parameters["xLightsViewProjection"].SetValue(lights.ViewProjection);
                    currentEffect.Parameters["xWorld"].SetValue(transforms[mesh.ParentBone.Index] * model.Transform * entityWorld * Camera.World);
                    currentEffect.Parameters["xLightPos"].SetValue(lights.LightPosition);
                    currentEffect.Parameters["xLightPower"].SetValue(lights.LightPower);
                    currentEffect.Parameters["xAmbient"].SetValue(lights.AmbientPower);

                    if(clipPlane.HasValue)
                    {
                        currentEffect.Parameters["xEnableClipping"].SetValue(true);
                        currentEffect.Parameters["xClipPlane"].SetValue(new Vector4(clipPlane.Value.Normal, clipPlane.Value.D));
                    }
                    else
                        currentEffect.Parameters["xEnableClipping"].SetValue(false);

                    if(customAlpha.HasValue)
                    {
                        currentEffect.Parameters["xEnableCustomAlpha"].SetValue(true);
                        currentEffect.Parameters["xCustomAlpha"].SetValue(customAlpha.Value);
                    }
                    else
                        currentEffect.Parameters["xEnableCustomAlpha"].SetValue(false);
                }
                mesh.Draw();
            }
        }

        #region Pre-draw operations
        public static Texture2D CreateRefractionMap(Plane refractionPlane)
        {
            Program.Game.GraphicsDevice.SetRenderTarget(renderTarget);
            Program.Game.GraphicsDevice.Clear(Color.Black);
            draw(refractionPlane, Camera.View);

            Program.Game.GraphicsDevice.SetRenderTarget(null);
            Texture2D output = (Texture2D)renderTarget;
            //renderTarget = null;
            return output;
        }

        public static Texture2D CreateReflectionMap(Matrix reflectedView, Plane reflectionPlane)
        {
            Program.Game.GraphicsDevice.SetRenderTarget(renderTarget);
            Program.Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 1);
            draw(reflectionPlane, reflectedView);
            
            Program.Game.GraphicsDevice.SetRenderTarget(null);
            Texture2D output = (Texture2D)renderTarget;
            //try
            //{
            //    System.IO.Stream s = System.IO.File.Open(Program.SavePath + "reflection.png", System.IO.FileMode.CreateNew);
            //    renderTarget.SaveAsPng(s, output.Width, output.Height);
            //    s.Close();
            //}
            //catch { }
            //renderTarget = null;
            return output;
        }

        /// <summary>
        /// Creates a reflection at a certain Z height.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Matrix CreateReflectionMatrix(float height)
        {
            Vector3 reflectedCameraPos = Camera.Position;
            reflectedCameraPos.Z = -Camera.Position.Z + height * 2;
            Vector3 reflectedCameraTarget = Camera.TargetPosition;
            reflectedCameraTarget.Z = -Camera.TargetPosition.Z + height * 2;
            Vector3 cameraRight = Camera.Rotation.Right;
            Vector3 inverseUp = Vector3.Cross(cameraRight, reflectedCameraTarget - reflectedCameraPos);

            return Matrix.CreateLookAt(reflectedCameraPos, reflectedCameraTarget, inverseUp);
        }

        /// <summary>
        /// Creates a clip plane.
        /// </summary>
        /// <param name="height">Height of clip plane.</param>
        /// <param name="planeNormal">Normal of plane.</param>
        /// <param name="clipSide">False = below, True = above</param>
        /// <returns></returns>
        public static Plane CreatePlane(float height, Vector3 planeNormal, bool clipSide)
        {
            planeNormal.Normalize();
            Vector4 planeCoeffs = new Vector4(planeNormal, height);
            if(clipSide)
                planeCoeffs *= -1;

            //Matrix inverseWorldViewProj = Matrix.Transpose(Matrix.Invert(Camera.WorldViewProj));
            //planeCoeffs = Vector4.Transform(planeCoeffs, inverseWorldViewProj);
            Plane finalPlane = new Plane(planeCoeffs);

            return finalPlane;
        }
        #endregion

        #region Debug - Axes
#if DEBUG
        private static void drawAxes()
        {
            Program.Game.GraphicsDevice.SetVertexBuffer(Program.Game.Loader.vertexBuff);
            Program.Game.Loader.xyz.VertexColorEnabled = true;
            Program.Game.Loader.xyz.World = Matrix.Identity;
            Program.Game.Loader.xyz.View = Camera.View;
            Program.Game.Loader.xyz.Projection = Camera.Projection;
            Program.Game.Loader.xyz.TextureEnabled = false;
            Program.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            foreach(EffectPass pass in Program.Game.Loader.xyz.CurrentTechnique.Passes)
            {
                pass.Apply();
                Program.Game.GDM.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList, Program.Game.Loader.vertices, 0, 3);
            }
        }
#endif
        #endregion

        public struct LightingData
        {
            public float LightPower { get; private set; }
            public float AmbientPower { get; private set; }
            public Vector3 Position { get; private set; }
            public Vector3 Target { get; private set; }
            public Color LightColor { get; private set; }

            private LightingData(float lightPower, float ambientPower, Vector3 pos, Vector3 targetPos, Color color)
                :this()
            {
                LightPower = lightPower;
                AmbientPower = ambientPower;
                Position = pos;
                Target = targetPos;
                LightColor = color;
            }

            public static LightingData Lava { get { return new LightingData(0.3f, 0.2f, new Vector3(-.1f, 0, -15), new Vector3(0, 0, 10), Color.White); } }
            public static LightingData Results { get { return new LightingData(0.35f, 0.2f, new Vector3(0, 1, 25), Vector3.Zero, Color.White); } }
            public static LightingData Beach { get { return new LightingData(0.31f, 0.2f, new Vector3(1, 2, 40), Vector3.Zero, Color.White); } }
            public static LightingData Ice { get { return new LightingData(0.3f, 0.3f, new Vector3(1, 2, 30), Vector3.Zero, Color.White); } }
            public static LightingData Space { get { return new LightingData(0.15f, 0.2f, new Vector3(.1f, 0, 40), Vector3.Zero, Color.White); } }
            public static LightingData Generic { get { return new LightingData(0.01f, 0.1f, new Vector3(.1f, 0, 60), Vector3.Zero, Color.White); } }
            public static LightingData Sky { get { return new LightingData(0.4f, 0.3f, new Vector3(-4, 0, 80), Vector3.Zero, Color.White); } }
        }

        public struct LightingSystem
        {
            public Effect Shader { get; private set; }
            public Texture2D LightMap { get; private set; }
            public float LightPower { get; private set; }
            public float AmbientPower { get; private set; }
            public Vector3 LightPosition { get; private set; }
            private Matrix view;
            private Matrix projection;
            public Matrix ViewProjection { get { return view * projection; } }
            public Vector3 LightColor { get; private set; }
            public Vector3 LightDirection { get; private set; }

            public LightingSystem(Effect shader, Texture2D lightmap)
                :this()
            {
                Shader = shader;
                LightMap = lightmap;
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, Program.Game.AspectRatio, 1f, 10000f);
            }

            public void SetUpLighting(float lightPower, float ambientPower, Vector3 pos, Vector3 targetPos, Color color)
            {
                this.LightPower = lightPower;
                this.AmbientPower = ambientPower;
                this.LightPosition = pos;
                this.LightColor = color.ToVector3();
                this.LightDirection = pos - targetPos;

                view = Matrix.CreateLookAt(pos, targetPos, Vector3.UnitZ);
            }
            public void SetUpLighting(float lightPower, float ambientPower, Vector3 pos, Vector3 targetPos, Texture2D lightTex, Color color)
            {
                SetUpLighting(lightPower, ambientPower, pos, targetPos, color);
                LightMap = lightTex;
            }
        }

        private class ShaderModel
        {
            public Model Model { get; private set; }
            public Dictionary<string, object> Parameters { get; private set; }
            public bool IsActive { get; private set; }
            public Effect Shader { get; private set; }
            public Texture2D[] Samplers { get; private set; }

            public ShaderModel(Model m, Effect shader, Dictionary<string, object> objects, params Texture2D[] samplerTextures)
            {
                Model = m;
                Parameters = objects;
                Shader = shader;
                IsActive = true;
                Samplers = samplerTextures;
            }

            public void SetValue(string name, object value)
            {
                if(Parameters.ContainsKey(name))
                    Parameters[name] = value;
                commitChanges();
            }

            public void SetValues(string[] names, object[] values)
            {
                if(names.Length != values.Length)
                    throw new ArgumentException("SetValues requires arrays of identical lengths.");
                for(int i = 0; i < names.Length; i++)
                    if(Parameters.ContainsKey(names[i]))
                        Parameters[names[i]] = values[i];
                commitChanges();
            }

            private object GetValue(string name)
            {
                if(Parameters.ContainsKey(name))
                    return Parameters[name];
                else throw new ArgumentException("The parameter name given to GetValue does not exist!");
            }

            private void commitChanges()
            {
                for(int i = 0; i < Parameters.Count; i++)
                {
                    KeyValuePair<string, object> key = Parameters.ElementAt(i);
                    if(key.Value is Matrix)
                        Shader.Parameters[key.Key].SetValue((Matrix)key.Value);
                    else if(key.Value is bool)
                        Shader.Parameters[key.Key].SetValue((bool)key.Value);
                    else if(key.Value is Vector2)
                        Shader.Parameters[key.Key].SetValue((Vector2)key.Value);
                    else if(key.Value is Vector3)
                        Shader.Parameters[key.Key].SetValue((Vector3)key.Value);
                    else if(key.Value is float)
                        Shader.Parameters[key.Key].SetValue((float)key.Value);
                    else if(key.Value is Quaternion)
                        Shader.Parameters[key.Key].SetValue((Quaternion)key.Value);
                    else if(key.Value is Vector4)
                        Shader.Parameters[key.Key].SetValue((Vector4)key.Value);
                    else if(key.Value is int)
                        Shader.Parameters[key.Key].SetValue((int)key.Value);
                    else if(key.Value is string)
                        Shader.Parameters[key.Key].SetValue((string)key.Value);
                    else if(key.Value is Texture)
                        Shader.Parameters[key.Key].SetValue((Texture)key.Value);
                    else if(key.Value is Matrix[])
                        Shader.Parameters[key.Key].SetValue((Matrix[])key.Value);
                    else if(key.Value is bool[])
                        Shader.Parameters[key.Key].SetValue((bool[])key.Value);
                    else if(key.Value is Vector2[])
                        Shader.Parameters[key.Key].SetValue((Vector2[])key.Value);
                    else if(key.Value is Vector3[])
                        Shader.Parameters[key.Key].SetValue((Vector3[])key.Value);
                    else if(key.Value is float[])
                        Shader.Parameters[key.Key].SetValue((float[])key.Value);
                    else if(key.Value is Quaternion[])
                        Shader.Parameters[key.Key].SetValue((Quaternion[])key.Value);
                    else if(key.Value is Vector4[])
                        Shader.Parameters[key.Key].SetValue((Vector4[])key.Value);
                    else if(key.Value is int[])
                        Shader.Parameters[key.Key].SetValue((int[])key.Value);
                    else if(key.Value is Color)
                    {
                        Color c = (Color)key.Value;
                        Shader.Parameters[key.Key].SetValue(c.ToVector4());
                    }
                    else
                        throw new ArgumentException("There was a value in Parameters that didn't make sense. Value was " + key.Value.GetType().Name);
                }
            }

            public void MakeActive()
            {
                IsActive = true;
            }
            public void MakeInactive()
            {
                IsActive = false;
            }

            public static bool operator==(ShaderModel lhs, Model rhs)
            {
                return lhs.Model == rhs;
            }
            public static bool operator !=(ShaderModel lhs, Model rhs)
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
        }

        private class ModelData
        {
            public List<Texture2D> Textures { get { return textures; } }
            private List<Texture2D> textures;
            public bool IsActive { get { return isActive; } }
            private bool isActive;
            public BaseModel Model { get; private set; }

            public ModelData(BaseModel m, List<Texture2D> textures)
            {
                if(textures.Count == 0)
                    throw new ArgumentException("Can't create a ModelData without any textures.");
                isActive = true;
                this.textures = new List<Texture2D>(textures);
                Model = m;
            }

            public void MakeActive()
            {
                isActive = true;
            }

            public void MakeInactive()
            {
                isActive = false;
            }

            public static bool operator==(ModelData lhs, BaseModel rhs)
            {
                return lhs.Model == rhs;
            }
            public static bool operator!=(ModelData lhs, BaseModel rhs)
            {
                return !(lhs.Model == rhs);
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private class PrimData
        {
            public Texture2D Texture { get { return texture; } }
            private Texture2D texture;
            public bool IsActive { get { return isActive; } }
            private bool isActive;
            public VertexBuffer Buffer { get; private set; }

            public PrimData(VertexBuffer b, Texture2D texture)
            {
                isActive = true;
                this.texture = texture;
                Buffer = b;
            }

            public void MakeActive()
            {
                isActive = true;
            }

            public void MakeInactive()
            {
                isActive = false;
            }

            public static bool operator ==(PrimData lhs, VertexBuffer rhs)
            {
                return lhs.Buffer == rhs;
            }
            public static bool operator !=(PrimData lhs, VertexBuffer rhs)
            {
                return !(lhs.Buffer == rhs);
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
