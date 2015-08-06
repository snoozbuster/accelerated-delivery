using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Constraints.TwoEntity.Joints;
using EntityBox = BEPUphysics.Entities.Prefabs.Box;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Constraints.TwoEntity;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    public class Level10 : Level
    {
        protected bool moved = false;

        protected Vector3 secondCameraPoint = new Vector3(243.289f, -4.69f, 2.05f);
        protected Vector3 currentCameraPoint;
        protected Vector3 speed = Vector3.Zero;
        protected Vector3 acceleration = Vector3.UnitX * 0.1f;

        protected int boxesMaxOne, boxesMaxTwo;
        protected int boxesNeedOne, boxesNeedTwo;

        protected Vector3 spawnTheFirst, spawnTheSecond;

        protected Goal catcherTheFirst, catcherTheSecond;
        protected BaseModel baseTheSecond;

        protected int scoreAtCheckpoint;
        protected TimeSpan timeAtCheckpoint;

        protected VertexBuffer buff;
        protected Texture2D blackTex;
        protected int lostAtCheckpoint;

        public override int Score { get { return base.Score + scoreAtCheckpoint; } }

        public Level10(int boxesMax, int boxNeed, Vector3 spawnPoint, List<Vector3> billboardList,
            Theme levelTheme, BaseModel levelModel, BaseModel levelModelTwo, BaseModel[] glassModels, Goal catcher,
            Dictionary<OperationalMachine, bool> machines, List<Tube> tubes, LevelCompletionData data, string name)
            : base(10, boxesMax, boxNeed, spawnPoint, billboardList, levelTheme, levelModel, glassModels,
            catcher, null, machines, tubes, data, name)
        {
            currentCameraPoint = RenderingDevice.Camera.Position;

            boxesMaxOne = boxesMax;
            boxesNeedOne = boxNeed;
            boxesMaxTwo = 20;
            boxesNeedTwo = 10;

            spawnTheFirst = spawnPoint;
            spawnTheSecond = new Vector3(197.153f, -4.134f, 1.5f);

            catcherTheFirst = normalCatcher;
            catcherTheSecond = new Goal(new Vector3(313.454f, -3.801f, 5.5f), true);

            baseTheSecond = levelModelTwo;

            Vector3 pos = new Vector3(198.273f, -4.468f, 6.037f);
            ADVertexFormat[] verts = new ADVertexFormat[4];
            verts[0] = new ADVertexFormat(new Vector3(0, 13.686f / 2, -11.359f / 2) + pos, new Vector2(0, 0), Vector3.UnitX);
            verts[1] = new ADVertexFormat(new Vector3(0, -13.686f / 2, -11.359f / 2) + pos, new Vector2(0, 1), Vector3.UnitX);
            verts[2] = new ADVertexFormat(new Vector3(0, 13.686f / 2, 11.359f / 2) + pos, new Vector2(1, 1), Vector3.UnitX);
            verts[3] = new ADVertexFormat(new Vector3(0, -13.686f / 2, 11.359f / 2) + pos, new Vector2(1, 0), Vector3.UnitX);

            buff = new VertexBuffer(RenderingDevice.GraphicsDevice, ADVertexFormat.VertexDeclaration, 4, BufferUsage.WriteOnly);
            buff.SetData(verts);

            blackTex = new Texture2D(RenderingDevice.GraphicsDevice, 1, 1);
            blackTex.SetData(new Color[] { new Color(0, 0, 0, 255) });
        }

        public override void AddToGame(BEPUphysics.Space s)
        {
            levelTheme.SetUpLighting();
            TemporarilyMuteVoice = false;

            MediaSystem.PlayTrack(levelTheme.Song);
            overlay = new OpeningOverlay(levelNumber, false, levelName);
            time = new TimeSpan();
            RebuildTiming();

            int i = 0;
            foreach(OperationalMachine m in MachineList.Keys)
                if(i < 11)
                {
                    s.Add(m);
                    i++;
                }
                else
                    break;
            s.Add(levelModel);
            i = 0;
            foreach(Tube t in tubeList)
            {
                if(i < 110)
                    s.Add(t);
                else
                    break;
                i++;
            }
            i = 0;
            foreach(BaseModel m in glassModels)
                if(i < 6)
                {
                    if(m.Ent.Space == null)
                        s.Add(m);
                    i++;
                }
                else
                    break;
            s.Add(dispenser);
            if(levelTheme.Fluid != null)
                s.Add(levelTheme.Fluid);

            addModelsToRenderer();
            results = null;
            ending = badEnding = false;
        }

        protected override void addModelsToRenderer()
        {
            RenderingDevice.Add(buff, blackTex);

            int i = 0;
            foreach(OperationalMachine m in MachineList.Keys)
                if(i < 11)
                {
                    RenderingDevice.Add(m);
                    i++;
                }
                else
                    break;
            i = 0;
            foreach(Tube t in tubeList)
            {
                if(i < 110)
                    RenderingDevice.Add(t);
                else
                    break;
                i++;
            }
            i = 0;
            foreach(BaseModel m in glassModels)
                if(i < 6)
                {
                    RenderingDevice.Add(m);
                    i++;
                }
                else 
                    break;

            RenderingDevice.Add(levelTheme.OuterModel);
            RenderingDevice.Add(levelTheme.SkyBox);
            RenderingDevice.Add(levelModel);
            RenderingDevice.Add(dispenser);

            levelTheme.InitializeShader();

            normalCatcher.AddToGame(GameManager.Space);
        }

        public override void RemoveFromGame(BEPUphysics.Space s)
        {
            RenderingDevice.Remove(buff);
            normalCatcher = catcherTheFirst;
            if(moved)
            {
                RenderingDevice.Remove(baseTheSecond);
                catcherTheSecond.RemoveFromGame();
            }
            moved = false;

            base.RemoveFromGame(s);
        }

        protected override void updateMachines(GameTime gameTime)
        {
            if(!moved)
                for(int i = 0; i < 11; i++)
                    MachineList.ElementAt(i).Key.Update(gameTime);
            else
                for(int i = 11; i < MachineList.Count; i++)
                    MachineList.ElementAt(i).Key.Update(gameTime);
        }

        protected override void removeModelsFromRenderer()
        {
            base.removeModelsFromRenderer();
            RenderingDevice.Remove(baseTheSecond);
        }

        protected override void endLevel()
        {
            if(!moved)
            {
                moved = true;
                MediaSystem.StopSiren();
                BoxSpawnPoint = spawnTheSecond;
                GameManager.Space.DuringForcesUpdateables.Starting += updateCamera;
                MediaSystem.LevelReset();
                MediaSystem.PlayVoiceActing(11);
                boxesMax = boxesMaxTwo;
                boxesNeeded = boxesNeedTwo;
                scoreboard = new Scoreboard(levelTheme.TextColor, boxesNeedTwo, boxesMaxTwo, spawnTime);
                scoreAtCheckpoint = score;
                timeAtCheckpoint = time;
                lostAtCheckpoint = BoxesDestroyed;
                BoxesDestroyed = 0;
                boxesSaved = 0;
                BoxesRemaining = boxesMaxTwo;
                RenderingDevice.Add(baseTheSecond);
                GameManager.Space.Add(baseTheSecond);
                while(boxesOnscreen.Count > 0)
                {
                    RenderingDevice.RemovePermanent(boxesOnscreen[0]);
                    GameManager.Space.Remove(boxesOnscreen[0]);
                    boxesOnscreen.RemoveAt(0);
                }

                TemporarilyMuteVoice = false;

                catcherTheSecond.AddToGame(GameManager.Space);
                int i = 0;
                foreach(OperationalMachine m in MachineList.Keys)
                {
                    i++;
                    if(i < 12)
                        continue;
                    else
                    {
                        RenderingDevice.Add(m);
                        GameManager.Space.Add(m);
                    }
                }
                i = 0;
                foreach(Tube m in tubeList)
                {
                    i++;
                    if(i < 111)
                        continue;
                    else
                    {
                        RenderingDevice.Add(m);
                        GameManager.Space.Add(m);
                    }
                }
                i = 0;
                foreach(BaseModel m in glassModels)
                {
                    i++;
                    if(i < 6)
                        continue;
                    else
                    {
                        RenderingDevice.Add(m);
                        //GameManager.Space.Add(m);
                    }
                }
            }
            else
            {
                base.endLevel();
                results = new ResultsScreen(time, BoxesDestroyed + lostAtCheckpoint, score, levelNumber, CompletionData);
            }
        }

        //public override void Update(GameTime gameTime)
        //{
        //    if(Input.CheckKeyboardJustPressed(Microsoft.Xna.Framework.Input.Keys.LeftShift))
        //        endLevel();
            
        //    base.Update(gameTime);
        //}

        protected void updateCamera()
        {
            speed += acceleration;
            if(speed.X > 20)
                speed = Vector3.UnitX * 20;

            RenderingDevice.Camera.TargetPosition += speed * GameManager.Space.TimeStepSettings.TimeStepDuration;
            
            RenderingDevice.Camera.LookAt(RenderingDevice.Camera.TargetPosition);

            if(RenderingDevice.Camera.TargetPosition.X > secondCameraPoint.X)
            {
                GameManager.Space.DuringForcesUpdateables.Starting -= updateCamera;
                speed = Vector3.Zero;
                //RenderingDevice.Camera.TargetPosition = secondCameraPoint;
            }
        }

        public override void ResetLevel()
        {
            speed = Vector3.Zero;
            GameManager.Space.DuringForcesUpdateables.Starting -= updateCamera;

            if(moved)
            {
                base.ResetLevel();
                if(catcherTheSecond.Ent.Space == null)
                    catcherTheSecond.AddToGame(GameManager.Space);
                score = scoreAtCheckpoint;
                overlay = null;
                if(!TemporarilyMuteVoice)
                    MediaSystem.PlayVoiceActing(11);
                RenderingDevice.Camera.TargetPosition = secondCameraPoint;
                return;

                //int i = 0;
                //foreach(Machine m in MachineList.Keys)
                //{
                //    i++;
                //    if(i < 12)
                //        continue;
                //    else
                //    {
                //        m.RemoveModelsFromRenderer();
                //        m.RemoveFromSpace(GameManager.Space);
                //    }
                //}
                //i = 0;
                //foreach(Tube m in tubeList)
                //{
                //    i++;
                //    if(i < 251)
                //        continue;
                //    else
                //    {
                //        RenderingDevice.Remove(m);
                //        GameManager.Space.Remove(m);
                //    }
                //}
                //i = 0;
                //foreach(BaseModel m in glassModels)
                //{
                //    i++;
                //    if(i < 8)
                //        continue;
                //    else
                //    {
                //        RenderingDevice.Remove(m);
                //        GameManager.Space.Remove(m);
                //    }
                //}
            }

            RenderingDevice.Camera.SetForResultsScreen();

            BoxSpawnPoint = spawnTheFirst;
            boxesMax = boxesMaxOne;
            scoreboard = new Scoreboard(levelTheme.TextColor, boxesNeedOne, boxesMaxOne, spawnTime);
            boxesNeeded = boxesNeedOne;
            scoreAtCheckpoint = lostAtCheckpoint = 0;
            timeAtCheckpoint = TimeSpan.Zero;

            base.ResetLevel();
        }

#if INTERNAL
        public override void Update(GameTime gameTime)
        {
            if(Input.CheckKeyboardJustPressed(Microsoft.Xna.Framework.Input.Keys.LeftShift) && !moved)
                endLevel();

            base.Update(gameTime);
        }
#endif
    }
}
