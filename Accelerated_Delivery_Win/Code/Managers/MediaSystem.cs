using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Accelerated_Delivery_Win
{
    public static class MediaSystem
    {
        /// <summary>
        /// True if there is playing voice acting. False if there is not.
        /// </summary>
        public static bool PlayingVoiceActing { get { if(playingVoiceActing != null) return !(playingVoiceActing.IsStopped || playingVoiceActing.IsStopping || playingVoiceActing.IsPaused) && !Program.Game.Manager.CurrentSaveWindowsOptions.Muted; return false; } }
        /// <summary>
        /// True if the siren for nearly losing is going.
        /// </summary>
        public static bool SirenPlaying { get { if(siren != null) return siren.IsPlaying; return false; } }

        /// <summary>
        /// True if the BGM is currently playing.
        /// </summary>
        public static bool PlayingBGM { get { if(playingBGM == null) return false; return playingBGM.IsPlaying; } }

        #region Private Members
        private static MediaLibrary library;
        private static AudioEngine engine;
        private static WaveBank bgmWaveBank;
        private static SoundBank bgmSoundBank;
        private static Cue playingBGM;
        private static Cue playingAmbience;
        private static Cue siren;
        private static /*Dictionary<Cue, AudioEmitter>*/List<Cue> playingSFX;
        private static WaveBank sfxWaveBank;
        private static SoundBank sfxSoundBank;
        private static Cue playingVoiceActing;
        private static int level;
        private static WaveBank voiceWaveBank;
        private static SoundBank voiceSoundBank;
        private static List<Cue> autonomousList = new List<Cue>();

        private static Dictionary<int, Cue> machineDict;

        private static float volToReturnTo;
        private static float volToReturnToSFX;
        private static bool shouldQuietMusic = false;
        //private static bool initallyQuiet = false;

        private static SongOptions playingSong = SongOptions.Credits;

#if WINDOWS
        private static readonly string path = "Content\\Music\\";
#elif XBOX
        private static readonly string path = "Content\\Music\\";
#endif

        private static GameState stateLastFrame;
        private static bool playingCustomMusic;
        private static bool playingAlbum;
        #endregion

        static MediaSystem()
        {
            engine = new AudioEngine(path + "music.xgs");
            bgmSoundBank = new SoundBank(engine, path + "Background_Music.xsb");
            sfxSoundBank = new SoundBank(engine, path + "Sound_Effects.xsb");
            voiceWaveBank = new WaveBank(engine, path + "Voice_Acting.xwb");
            bgmWaveBank = new WaveBank(engine, path + "Background_Music.xwb");
            sfxWaveBank = new WaveBank(engine, path + "Sound_Effects.xwb");
            voiceSoundBank = new SoundBank(engine, path + "Voice_Acting.xsb");

            machineDict = new Dictionary<int, Cue>();
            for(int i = 1; i <= 10; i++)
                machineDict.Add(i, null);

            playingSFX = new List<Cue>();//new Dictionary<Cue, AudioEmitter>();

            volToReturnToSFX = engine.GetGlobalVariable("SFXVolume");
            volToReturnTo = engine.GetGlobalVariable("BGMVolume");
            //if(!Program.Game.IsActive)
            //{
            //    initallyQuiet = true;
            //    engine.SetGlobalVariable("BGMVolume", 0);
            //    engine.SetGlobalVariable("SFXVolume", 0);
            //}

            library = new MediaLibrary();
        }

        #region Update
        /// <summary>
        /// Updates the media.
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            if(playingBGM != null && Program.Game.IsActive && !playingBGM.IsPlaying && !Program.Game.Manager.CurrentSaveWindowsOptions.Muted)
            {
                if(playingBGM.IsPrepared || playingBGM.IsPreparing)
                    playingBGM.Play();
                else if(playingBGM.IsPaused)
                    playingBGM.Resume();
                else if(playingBGM.IsStopped)
                {
                    if(GameManager.CurrentLevel != null)
                        PlayTrack(GameManager.CurrentLevel.LevelSong);
                    else
                        PlayTrack(SongOptions.Menu);
                }
            }
            //if(initallyQuiet && Program.Game.IsActive)
            //{
            //    engine.SetGlobalVariable("SFXVolume", volToReturnToSFX);
            //    engine.SetGlobalVariable("BGMVolume", volToReturnTo);
            //    initallyQuiet = false;
            //}

            if(Program.Game.Manager != null && !Program.Game.Manager.CurrentSaveWindowsOptions.Muted)
            {
                volToReturnTo = Program.Game.Manager.CurrentSaveWindowsOptions.BGMVolume;
                volToReturnToSFX = Program.Game.Manager.CurrentSaveWindowsOptions.SFXVolume;
                engine.SetGlobalVariable("BGMVolume", volToReturnTo);
                engine.SetGlobalVariable("SFXVolume", volToReturnToSFX);
                engine.SetGlobalVariable("VoiceVolume", Program.Game.Manager.CurrentSaveWindowsOptions.VoiceVolume);
            }

            for(int i = 0; i < playingSFX.Count; i++)
            {
                Cue c = playingSFX[i];//.ElementAt(i).Key;
                if(c.IsStopped)
                {
                    c.Dispose();
                    playingSFX.Remove(c);
                    i--;
                }
            }
            for(int i = 1; i <= 10; i++)
                if(machineDict[i] != null)
                {
                    if(machineDict[i].IsPreparing)
                        continue;
                    else if(machineDict[i].IsPrepared)
                        machineDict[i].Play();
                    else if(machineDict[i].IsStopped)
                    {
                        machineDict[i].Dispose();
                        machineDict[i] = null;
                    }
                }
            for(int i = 0; i < autonomousList.Count; i++)
                if(autonomousList[i].IsStopped)
                {
                    autonomousList.RemoveAt(i);
                    i--;
                }
            if(level != GameManager.LevelNumber)
            {
                if(playingVoiceActing != null)
                {
                    playingVoiceActing.Stop(AudioStopOptions.Immediate);
                    playingVoiceActing = null;
                    engine.SetGlobalVariable("BGMVolume", volToReturnTo);
                }
                while(autonomousList.Count > 0)
                {
                    if(autonomousList[0].IsPlaying)
                        autonomousList[0].Stop(AudioStopOptions.Immediate);
                    autonomousList.RemoveAt(0);
                }
                for(int i = 1; i <= 10; i++)
                {
                    if(machineDict[i] != null && machineDict[i].IsPlaying)
                        machineDict[i].Stop(AudioStopOptions.Immediate);
                    machineDict[i] = null;
                }
            }
            if(playingVoiceActing != null)
                if(playingVoiceActing.IsStopped)
                {
                    playingVoiceActing = null;
                    engine.SetGlobalVariable("BGMVolume", volToReturnTo);
                }
            //foreach(Cue c in playingSFX.Keys)
            //    if(c.IsPlaying)
            //        c.Apply3D(RenderingDevice.Camera.Ears, playingSFX[c]);
#if WINDOWS
            if(Input.CheckKeyboardJustPressed(Program.Game.Manager.CurrentSaveWindowsOptions.MuteKey))
            {
                ToggleMute();
            }
#endif
            if(GameManager.State == GameState.Paused && stateLastFrame == GameState.Running)
            {
                PauseAll();
                
                ResumeBGM();
            }
            else if(GameManager.State == GameState.Running && stateLastFrame == GameState.Paused)
            {
                PlayAll();
            }
            stateLastFrame = GameManager.State;

            engine.Update();
        }
        #endregion

        #region Play BGM/SFX/Voice
        /// <summary>
        /// Plays a BGM and ambience, if any.
        /// </summary>
        /// <param name="song">The BGM to play.</param>
        public static void PlayTrack(SongOptions song)
        {
            level = GameManager.LevelNumber;
            if(song == playingSong)
                return;
            playingSong = song;
            switch(song)
            {
                case SongOptions.Credits:
                    playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Credits_BGM");
                    playingBGM.Play();
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = null;
                    break;
                case SongOptions.Menu:
                    if(playingBGM != null)
                        playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Menu_BGM");
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = null;
                    if(!playingBGM.IsPlaying)
                    {
                        if(playingBGM.IsPaused)
                            playingBGM.Resume();
                        playingBGM.Play();
                    }
                    break;
                case SongOptions.Generic:
                    playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Generic_BGM");
                    playingBGM.Play();
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = bgmSoundBank.GetCue("Ambience_Generic");
                    break;
                case SongOptions.Lava:
                    playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Lava_BGM");
                    playingBGM.Play();
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = bgmSoundBank.GetCue("Ambience_Lava");
                    break;
                case SongOptions.Ice:
                    playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Ice_BGM");
                    playingBGM.Play();
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = bgmSoundBank.GetCue("Ambience_Ice");
                    break;
                case SongOptions.Beach:
                    playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Beach_BGM");
                    playingBGM.Play();
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = bgmSoundBank.GetCue("Ambience_Beach");
                    break;
                case SongOptions.Sky:
                    playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Sky_BGM");
                    playingBGM.Play();
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = bgmSoundBank.GetCue("Ambience_Sky");
                    break;
                case SongOptions.Space:
                    playingBGM.Stop(AudioStopOptions.AsAuthored);
                    playingBGM = bgmSoundBank.GetCue("Space_BGM");
                    playingBGM.Play();
                    if(playingAmbience != null)
                        playingAmbience.Stop(AudioStopOptions.Immediate);
                    playingAmbience = bgmSoundBank.GetCue("Ambience_Space");
                    break;
            }
            if(playingAmbience != null)
                playingAmbience.Play();
            if(playingCustomMusic)
                playingBGM.Pause();
        }

        /// <summary>
        /// Plays a sound effect at the center of the 3D space.
        /// </summary>
        /// <param name="s">Sound effect to play.</param>
        //public static void PlaySoundEffect(SFXOptions s)
        //{
        //    PlaySoundEffect(s, Vector3.Zero);
        //}

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="s">The sound effect to play.</param>
        /// <param name="position">The 3D position to play it at.</param>
        public static void PlaySoundEffect(SFXOptions s)//, Vector3 position)
        {
            Cue c = null;
            switch(s)
            {
                case SFXOptions.Button_Press:
                    c = sfxSoundBank.GetCue("Button_Depress");
                    break;
                case SFXOptions.Button_Release:
                    c = sfxSoundBank.GetCue("Button_Release");
                    break;
                case SFXOptions.Button_Rollover:
                    c = sfxSoundBank.GetCue("Button_Rollover");
                    break;
                case SFXOptions.Achievement:
                    c = sfxSoundBank.GetCue("Achievement");
                    break;
                case SFXOptions.Box_Death:
                    c = sfxSoundBank.GetCue("Box_Death");
                    break;
                case SFXOptions.Box_Success:
                    c = sfxSoundBank.GetCue("Box_Success");
                    break;
                case SFXOptions.Fail:
                    c = sfxSoundBank.GetCue("Fail");
                    break;
                case SFXOptions.Result_Da:
                    c = sfxSoundBank.GetCue("Result_Da");
                    break;
                case SFXOptions.Siren:
                    if(siren != null)
                        siren.Stop(AudioStopOptions.Immediate);
                    siren = sfxSoundBank.GetCue("Siren");
                    siren.Play();
                    break;
                case SFXOptions.Win:
                    c = sfxSoundBank.GetCue("Level_Win");
                    break;
                case SFXOptions.Press_Start:
                    c = sfxSoundBank.GetCue("Press_Start");
                    break;
                case SFXOptions.Pause:
                    c = sfxSoundBank.GetCue("Pause_Jingle");
                    break;
                case SFXOptions.Explosion:
                    c = sfxSoundBank.GetCue("Explosion");
                    break;
                case SFXOptions.Machine_Button_Press:
                    c = sfxSoundBank.GetCue("Machine_Button_Depress");
                    break;
                case SFXOptions.Laser:
                    c = sfxSoundBank.GetCue("Laser");
                    break;
                default: throw new ArgumentException("You forgot to add the effect to PlaySoundEffect()!");
            }
            if(c != null)
            {
                playingSFX.Add(c);//, new AudioEmitter() { Position = position, DopplerScale = 2, Up = Vector3.UnitZ });
                //c.Apply3D(RenderingDevice.Camera.Ears, playingSFX[c]);
                c.Play();
            }
        }

        /// <summary>
        /// Pretty much trackNo = level.
        /// 10.5 is 11.
        /// Handle With Care is 12.
        /// </summary>
        public static void PlayVoiceActing(int trackNo)
        {
            if(GameManager.LevelNumber > 10)
                return;

            if(playingVoiceActing != null)
                playingVoiceActing.Stop(AudioStopOptions.Immediate);

            if(!Program.Game.Manager.CurrentSaveWindowsOptions.VoiceClips)
                return;

            if(trackNo != 12)
                playingVoiceActing = voiceSoundBank.GetCue("Level" + trackNo);
            else
                playingVoiceActing = voiceSoundBank.GetCue("Handle With Care");

            playingVoiceActing.Play();
            if(!Program.Game.IsActive)
                playingVoiceActing.Pause();
            shouldQuietMusic = trackNo != 12;
            volToReturnTo = engine.GetGlobalVariable("BGMVolume");
            level = GameManager.LevelNumber;
        }
        #endregion

        #region Machine SFX
        /// <summary>
        /// This gets a machine noise. Returns null if the machine number already has a noise playing.
        /// Does not play the noise.
        /// </summary>
        /// <param name="soundIndex">Values from 0 to 10 inclusive return a sound. -1 returns null. All other values
        /// throw an exception.</param>
        /// <param name="machineNo"></param>
        /// <returns></returns>
        public static Cue GetMachineNoise(int soundIndex, int machineNo)
        {
            if(soundIndex == -1)
                return null;

            if(soundIndex < 0 || soundIndex > 10)
                throw new ArgumentException("soundIndex must be between 0 and 10 inclusive.");
            if(machineNo == 0)
            {
                Cue c = sfxSoundBank.GetCue("Machine_Sound_" + soundIndex);
                autonomousList.Add(c);
                return c;
            }

            if(machineDict[machineNo] == null || machineDict[machineNo].IsStopped || machineDict[machineNo].IsStopping)
                machineDict[machineNo] = sfxSoundBank.GetCue("Machine_Sound_" + soundIndex);
            else
                return null;
            
            return machineDict[machineNo];
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Stops the siren.
        /// </summary>
        public static void StopSiren()
        {
            if(siren != null)
                siren.Stop(AudioStopOptions.Immediate);
            siren = null;
        }

        /// <summary>
        /// Call when a level ends.
        /// </summary>
        public static void EndingLevel()
        {
            foreach(Cue c in autonomousList)
                if(c.IsPlaying)
                    c.Stop(AudioStopOptions.AsAuthored);
            autonomousList.Clear();
        }

        /// <summary>
        /// Call to play the startup noise.
        /// </summary>
        public static void Ready()
        {
            Cue c = sfxSoundBank.GetCue("Startup");
            playingSFX.Add(c);//, new AudioEmitter() { Position = Vector3.Zero });
            //c.Apply3D(RenderingDevice.Camera.Ears, playingSFX[c]);
            c.Play();
        }

        /// <summary>
        /// Mutes or unmutes the game.
        /// </summary>
        public static void ToggleMute()
        {
            Program.Game.Manager.CurrentSaveWindowsOptions.Muted = !Program.Game.Manager.CurrentSaveWindowsOptions.Muted;
            if(Program.Game.Manager.CurrentSaveWindowsOptions.Muted)
            {
                engine.SetGlobalVariable("BGMVolume", 0);
                engine.SetGlobalVariable("SFXVolume", 0);
                engine.SetGlobalVariable("VoiceVolume", 0);
            }
            else
            {
                if(Program.Game.Manager != null)
                {
                    engine.SetGlobalVariable("BGMVolume", Program.Game.Manager.CurrentSaveWindowsOptions.BGMVolume);
                    engine.SetGlobalVariable("SFXVolume", Program.Game.Manager.CurrentSaveWindowsOptions.SFXVolume);
                    engine.SetGlobalVariable("VoiceVolume", Program.Game.Manager.CurrentSaveWindowsOptions.VoiceVolume);
                }
                else
                {
                    engine.SetGlobalVariable("BGMVolume", 100);
                    engine.SetGlobalVariable("SFXVolume", 100);
                    engine.SetGlobalVariable("VoiceVolume", 100);
                }
            }
        }
        #endregion

        #region Play/Pause All
        /// <summary>
        /// Pauses everything for when the game gets paused.
        /// </summary>
        public static void PauseAuxilary()
        {
            if(GameManager.State != GameState.MainMenu)
            {
                foreach(Cue c in playingSFX)//.Keys)
                    if(c.IsPlaying)
                        c.Pause();
            }
            else
                foreach(Cue c in playingSFX)//.Keys)
                    if(c.IsPlaying)
                        c.Stop(AudioStopOptions.Immediate);
            if(playingVoiceActing != null && playingVoiceActing.IsPlaying)
                playingVoiceActing.Pause();
            if(siren != null && siren.IsPlaying)
                siren.Pause();
            foreach(Cue c in machineDict.Values)
                if(c != null && c.IsPlaying)
                    c.Pause();
            foreach(Cue c in autonomousList)
                if(c != null && c.IsPlaying)
                    c.Pause();
        }

        /// <summary>
        /// Pauses everything for when the game goes out of focus.
        /// </summary>
        public static void PauseAll()
        {
            if(playingBGM != null && playingBGM.IsPlaying)
                playingBGM.Pause();
            if(GameManager.State != GameState.MainMenu)
            {
                int i = 0;
                foreach(Cue c in playingSFX)//.Keys)
                {
                    if(c.IsPlaying && i != playingSFX.Count - 1)//.Keys.Count - 1)
                        c.Pause();
                    i++;
                }
            }
            else
                foreach(Cue c in playingSFX)//.Keys)
                    if(c.IsPlaying)
                        c.Stop(AudioStopOptions.Immediate);
            if(playingVoiceActing != null && playingVoiceActing.IsPlaying)
                playingVoiceActing.Pause();
            if(siren != null && siren.IsPlaying)
                siren.Pause();
            if(playingAmbience != null && playingAmbience.IsPlaying)
                playingAmbience.Pause();
            foreach(Cue c in machineDict.Values)
                if(c != null && c.IsPlaying)
                    c.Pause();
            foreach(Cue c in autonomousList)
                if(c != null && c.IsPlaying)
                    c.Pause();
            volToReturnToSFX = engine.GetGlobalVariable("SFXVolume");
            engine.SetGlobalVariable("SFXVolume", 0);
            pauseCustomMusic();
        }

        /// <summary>
        /// Resumes the BGM.
        /// </summary>
        internal static void ResumeBGM()
        {
            if(playingBGM != null && playingBGM.IsPaused && !playingCustomMusic)
                playingBGM.Resume();
            if(playingAmbience != null && playingAmbience.IsPaused)
                playingAmbience.Resume();
            if(playingCustomMusic)
                resumeCustomMusic();
        }

        /// <summary>
        /// Resumes all music for when the game receives focus.
        /// </summary>
        public static void PlayAll()
        {
            if(playingBGM != null && playingBGM.IsPaused && !playingCustomMusic)
                playingBGM.Resume();
            foreach(Cue c in playingSFX)//.Keys)
                if(c != null && c.IsPaused)
                    c.Resume();
            if(playingVoiceActing != null && playingVoiceActing.IsPaused)
                playingVoiceActing.Resume();
            if(siren != null && siren.IsPaused)
                siren.Resume();
            if(playingAmbience != null)
                playingAmbience.Resume();
            foreach(Cue c in machineDict.Values)
                if(c != null && c.IsPaused)
                    c.Resume();
            engine.SetGlobalVariable("SFXVolume", volToReturnToSFX);
            resumeCustomMusic();
        }
        #endregion

        #region Stopping functions
        /// <summary>
        /// Stops all SFX.
        /// </summary>
        public static void StopSFX()
        {
            foreach(Cue c in playingSFX)//.Keys)
                if(c != null && c.IsPlaying)
                    c.Stop(AudioStopOptions.Immediate);
            if(playingAmbience != null)
                playingAmbience.Stop(AudioStopOptions.Immediate);
            playingAmbience = null;
        }

        /// <summary>
        /// Stops the voice acting.
        /// </summary>
        public static void StopVoiceActing()
        {
            if(playingVoiceActing != null && !playingVoiceActing.IsStopped)
                playingVoiceActing.Stop(AudioStopOptions.Immediate);
            playingVoiceActing = null;
        }
#endregion

        #region Custom Music
        /// <summary>
        /// Begins playing custom music on the Shuffle All setting.
        /// </summary>
        public static void StartShuffleCustomMusic()
        {
            if(MediaPlayer.IsShuffled && MediaPlayer.State != MediaState.Stopped)
                return;

            MediaPlayer.IsShuffled = true;
            MediaPlayer.IsRepeating = true;
            try
            {
                if(MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();
                MediaPlayer.Play(library.Songs);
                if(playingBGM.IsPlaying)
                    playingBGM.Pause();
                playingCustomMusic = true;
            }
            catch
            {
                MenuHandler.ErrorString = "Could not shuffle all music. There may not be any songs to shuffle; run Windows Media Player to create a library.";
            }
        }

        /// <summary>
        /// Begins playing custom music on the Albums setting.
        /// </summary>
        /// <param name="album">Album to play.</param>
        public static void StartAlbumCustomMusic(Album album)
        {
            if(playingAlbum && MediaPlayer.State != MediaState.Stopped)
                return;
            
            playingAlbum = true;
            MediaPlayer.IsShuffled = false;
            MediaPlayer.IsRepeating = true;
            try
            {
                if(MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();
                MediaPlayer.Play(album.Songs);
                if(playingBGM.IsPlaying)
                    playingBGM.Pause();
                playingCustomMusic = true;
            }
            catch
            {
                MenuHandler.ErrorString = "Could not play the album.";
            }
        }

        /// <summary>
        /// Begins playing custom music on the Artists setting.
        /// </summary>
        /// <param name="artist">Artist to play.</param>
        public static void StartArtistsCustomMusic(Artist artist)
        {
            if(!playingAlbum && MediaPlayer.State != MediaState.Stopped)
                return;

            playingAlbum = false;
            MediaPlayer.IsShuffled = false;
            MediaPlayer.IsRepeating = true;
            try
            {
                if(MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();
                MediaPlayer.Play(artist.Songs);
                if(playingBGM.IsPlaying)
                    playingBGM.Pause();
                playingCustomMusic = true;
            }
            catch
            {
                MenuHandler.ErrorString = "Could not play the artist.";
            }
        }

        /// <summary>
        /// Gets a list of the artists in the library.
        /// </summary>
        /// <returns></returns>
        public static ArtistCollection GetArtistsInLibrary()
        {
            return library.Artists;
        }

        /// <summary>
        /// Gets a list of the albums in the library.
        /// </summary>
        /// <returns></returns>
        public static AlbumCollection GetAlbumsInLibrary()
        {
            return library.Albums;
        }

        /// <summary>
        /// Stops playing custom music and restarts the normal BGM.
        /// </summary>
        public static void StopCustomMusic()
        {
            MediaPlayer.Stop();
            if(playingBGM.IsPaused)
                playingBGM.Resume();
            else if(playingBGM.IsStopped)
                playingBGM.Play();
            playingCustomMusic = false;
        }

        /// <summary>
        /// Goes to the previous track.
        /// </summary>
        public static void MovePrevious()
        {
            int attempts = 5;
            for(int i = 0; i < attempts; i++)
            {
                Thread moveNext = null;
                if(i == 0)
                    moveNext = new Thread(MediaPlayer.MovePrevious);
                else
                    moveNext = new Thread(delegate() { MediaPlayer.Queue.ActiveSongIndex -= i + 1; });
                moveNext.Start();
                bool success = moveNext.Join(1000);

                if(success)
                {
                    MenuHandler.ErrorString = "Skipped " + i + " unplayable songs.";
                    return;
                }
            }
            MenuHandler.ErrorString = "Could not play next track. Please try again.";
        }
        
        /// <summary>
        /// Goes to the next track.
        /// </summary>
        public static void MoveNext()
        {
            int attempts = 5;
            for(int i = 0; i < attempts; i++)
            {
                Thread moveNext = null;
                if(i == 0)
                    moveNext = new Thread(MediaPlayer.MoveNext);
                else
                    moveNext = new Thread(delegate() { MediaPlayer.Queue.ActiveSongIndex += i + 1; });
                moveNext.Start();
                bool success = moveNext.Join(2000);

                if(success)
                {
                    if(i != 0)
                        MenuHandler.ErrorString = "Playing: " + GetPlayingSong() + ". Skipped " + i + " unplayable song" + (i == 1 ? "" : "s") + " (likely due to DRM).";
                    return;
                }
            }
            MenuHandler.ErrorString = "Could not play next track. Please try again.";
        }

        /// <summary>
        /// Gets a string in the format of (song name) by (artist name) on (album name). Returns (nothing) if no song is playing.
        /// </summary>
        /// <returns></returns>
        public static string GetPlayingSong()
        {
            if(MediaPlayer.State == MediaState.Stopped)
                return "(nothing)";

            Song playingSong = MediaPlayer.Queue.ActiveSong;
            string artist = playingSong.Artist.Name;
            string album = playingSong.Album.Name;
            return playingSong.Name + " by " + (artist == string.Empty ? "Unknown Artist" : artist) + " on " + (album == string.Empty ? "Unknown Album" : album);
        }

        private static void pauseCustomMusic()
        {
            if(MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        private static void resumeCustomMusic()
        {
            if(MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }
        #endregion
    }
}