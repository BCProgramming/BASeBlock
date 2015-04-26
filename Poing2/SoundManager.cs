using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using irrklang;
using BASeCamp.Configuration;
using IrrKlang;
using FMODNet;
namespace BASeBlock
{

    public class cActiveSoundObject
    {
        private ISound mSoundWrapping;

        public cActiveSoundObject(ISound wrappingthis)
        {
            mSoundWrapping=wrappingthis;
            
        }
        



    }
    public class cSoundSource
    {

        private ISoundSource mSound;
        private ISoundEngine mEngine;
        public cSoundSource(ISoundEngine engineuse,ISoundSource objsound)
        {
            mEngine=engineuse;
            mSound = objsound;


        }

        /// <summary>
        /// start playback of this sound at the given position.
        /// </summary>
        /// <param name="StartPosition">Position to start playback.</param>
        cActiveSoundObject Play(int StartPosition,bool playloop)
        {
            return new cActiveSoundObject(mEngine.Play2D(mSound, playloop, false, false));

        }

        cActiveSoundObject Play()
        {
            return new cActiveSoundObject(mEngine.Play2D(mSound, false, false, false));
        }

        



    }



    /// <summary>
    /// Generic Sound manager for handling the automatic loading,cache, and access of sounds.
    /// requires: cINIFile.cs, irrklang sound library
    /// </summary>
    public class SoundManager
    {

        /// <summary>
        /// represents All available sounds, indexed into a dictionary by their base filename.
        /// </summary>
        /// 
        
        //added this class so that derived classed won't automatically implement the irrklang "ISoundStopEventReceiver" interface:
        private class cSoundStopEvent:ISoundStopEventReceiver
        {
            public delegate void OnSoundStop(ISound sound, StopEventCause reason, object userData);
            private OnSoundStop mstopFunction;
            public cSoundStopEvent(OnSoundStop stopfunction)
            {
                mstopFunction = stopfunction;

            }

            
            #region ISoundStopEventReceiver Members

            public void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
            {
                mstopFunction(sound, reason, userData);
            }

            #endregion
        }

        private Dictionary<String, IrrKlang.ISoundSource> SoundSources = new Dictionary<String, ISoundSource>();
        private List<ISound> PlayingSounds = new List<ISound>();
        private ISoundEngine mSoundEngine = new ISoundEngine();
        private ISound mPlayingMusic = null;
        private ISoundSource mPlayingMusicSource = null;



        public SoundManager(String[] pathsuse)
        {

            
            
            LoadSounds(pathsuse);

        }

        public SoundManager(String pathuse)
            : this(new String[] { pathuse })
        {

            LoadSounds(pathuse);

        }
        public SoundManager()
        {
            LoadSounds();

        }
        public void LoadSounds(String pathuse)
        {
            LoadSounds(pathuse.Split(';'));


        }
        public void LoadSounds()
        {
            List<String> soundfolders = new List<string>();


            soundfolders.Add(AppDomain.CurrentDomain.BaseDirectory);


            LoadSounds(soundfolders.ToArray());

        }
        public cSoundSource GetSound(String key)
        {
            return new cSoundSource(mSoundEngine,SoundSources[key]);


        }


        public String AddSound(String soundfile)
        {
            FileInfo gotfile = new FileInfo(soundfile);
            if (IsSupportedSoundfile(gotfile.FullName))
            {
                if (!SoundSources.ContainsKey(Path.GetFileNameWithoutExtension(gotfile.FullName).ToUpper()))
                {
                    String usekey = Path.GetFileNameWithoutExtension(gotfile.FullName).ToUpper();
                    ISoundSource addsource = mSoundEngine.AddSoundSourceFromFile(gotfile.FullName);
                    SoundSources.Add(usekey, addsource);
                    return usekey;
                }



            }

            return "";


        }

        public bool HasSound(String key)
        {
            if (key == null) return false;
            return SoundSources.ContainsKey(key.ToUpper());

        }

        public cActiveSoundObject PlaySound(String key, float volume)
        {

            ISound startsound = mSoundEngine.Play2D(SoundSources[key.ToUpper()], false, true, false);

            if (startsound != null)
            {
                startsound.Volume = volume;
                startsound.setSoundStopEventReceiver(new cSoundStopEvent(OnSoundStopped));
                PlayingSounds.Add(startsound);
                startsound.Paused = false;
            }

            return new cActiveSoundObject(startsound);


        }

        public void PauseMusic()
        {
            if (mPlayingMusic != null)
                mPlayingMusic.Paused = true;


        }
        public void StopMusic()
        {
            if (mPlayingMusic != null)
                mPlayingMusic.Stop();

            mPlayingMusic = null;
        }

        public void ResumeMusic()
        {
            if (mPlayingMusic != null)
                mPlayingMusic.Paused = false;



        }
        public void PlayMusic(String key)
        {
            PlayMusic(key, 1.0f);


        }

        public void PlayMusic(String key, float volume)
        {
            if (mPlayingMusic != null)
            {
                mPlayingMusic.Stop();


            }
            if (key.Equals(String.Empty)) return; // null;
            mPlayingMusicSource = SoundSources[key.ToUpper()];
            mPlayingMusic = mSoundEngine.Play2D(mPlayingMusicSource, true, true, false);
            mPlayingMusic.Volume = volume;
            mPlayingMusic.Paused = false;
            //return mPlayingMusic;



        }
        public void PlayMusic(String key, float volume, bool repeat)
        {

            if (mPlayingMusic != null)
            {
                mPlayingMusic.Stop();


            }
            if (key.Equals(String.Empty)) return; // null;
            mPlayingMusicSource = SoundSources[key.ToUpper()];
            mPlayingMusic = mSoundEngine.Play2D(mPlayingMusicSource, repeat, true, false);
            mPlayingMusic.Volume = volume;
            mPlayingMusic.Paused = false;
           // return mPlayingMusic;


        }

        private bool IsSupportedSoundfile(String filename)
        {

            String[] supportedtypes = new string[] { ".MP3", ".WAV", ".OGG", ".FLAC" };

            //return (supportedtypes.ToList().Exists((p)=>(p.ToUpper()==Path.GetFileNameWithoutExtension(filename).ToUpper())));
            return supportedtypes.Contains(Path.GetExtension(filename).ToUpper());











        }

        public void LoadSounds(String[] pathsuse)
        {
            foreach (String loopfolder in pathsuse)
            {
                if (Directory.Exists(loopfolder))
                {
                    DirectoryInfo getdirinfo = new DirectoryInfo(loopfolder);
                    foreach (FileInfo loopfile in getdirinfo.GetFiles())
                    {
                        if (IsSupportedSoundfile(loopfile.FullName))
                        {
                            if (!SoundSources.ContainsKey(Path.GetFileNameWithoutExtension(loopfile.FullName).ToUpper()))
                            {
                                AddSoundFromFile(loopfile);
                            }



                        }




                    }


                }

            }


        }

        private void AddSoundFromFile(FileInfo loopfile)
        {
            ISoundSource addsource = mSoundEngine.AddSoundSourceFromFile(loopfile.FullName);
            SoundSources.Add(Path.GetFileNameWithoutExtension(loopfile.FullName).ToUpper(), addsource);
        }


        private void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
        {

            PlayingSounds.Remove(sound);
        }

    
    }
}
