using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrrKlang;

namespace BASeBlock
{
    /// <summary>
    /// represents and manipulates a sound object that is in use/playing.
    /// </summary>
    public interface iActiveSoundObject
    {

        bool Finished { get; }
        void Stop();
        void Pause();
        void UnPause();
        bool Paused { get; set; }

    }
    public interface iSoundSourceObject
    {
        iActiveSoundObject Play(bool playlooped);

        iActiveSoundObject Play(bool playlooped, float volume);

    }
    public delegate void OnSoundStopDelegate(iActiveSoundObject objstop);
    public delegate void OnSoundPlayDelegate(iActiveSoundObject objplay);
    public interface iSoundEngineDriver
    {
        event OnSoundStopDelegate OnSoundStop;
        event OnSoundPlayDelegate OnSoundPlay;

        iSoundSourceObject loadSound(String filename);
        String Name { get; }
        String[] GetSupportedExtensions();


    }







    public class cNewSoundManager
    {
        private iSoundEngineDriver mDriver;
        private Dictionary<String, iSoundSourceObject> mSoundSources = new Dictionary<string, iSoundSourceObject>();
        private List<iActiveSoundObject> PlayingSounds = new List<iActiveSoundObject>();
        private iActiveSoundObject mPlayingMusic;
        private iSoundSourceObject mPlayingMusicSource;
        protected cNewSoundManager(iSoundEngineDriver sounddriver)
        {
            mDriver = sounddriver;
            mDriver.OnSoundStop +=new OnSoundStopDelegate(mDriver_OnSoundStop);
            mDriver.OnSoundPlay += new OnSoundPlayDelegate(mDriver_OnSoundPlay);
        }

        void mDriver_OnSoundPlay(iActiveSoundObject objplay)
        {
            PlayingSounds.Add(objplay);
        }

void  mDriver_OnSoundStop(iActiveSoundObject objstop)
{
 //	throw new NotImplementedException();
    if(objstop!=null)
        PlayingSounds.Remove(objstop);
}

        public cNewSoundManager(iSoundEngineDriver sounddriver, String[] SoundFilePaths):this(sounddriver)
        {
            LoadSounds(SoundFilePaths);


        }
        public cNewSoundManager(iSoundEngineDriver sounddriver, DirectoryInfo[] DirsUse)
            : this(sounddriver)
        {
            LoadSounds(DirsUse);


        }
        public cNewSoundManager(iSoundEngineDriver sounddriver, String SoundFilePath)
            : this(sounddriver)
        {
            LoadSounds(SoundFilePath);


        }
        public bool IsSupportedSound(String testfile)
        {
            //return return supportedtypes.Contains(Path.GetExtension(filename).ToUpper());
            return mDriver.GetSupportedExtensions().Contains(Path.GetExtension(testfile).ToUpper());

        }

        public void LoadSounds(DirectoryInfo[] loadfromdirs)
        {



            foreach (DirectoryInfo loopdir in loadfromdirs)
            {
                LoadSounds(loopdir);


            }







        }
        public bool HasSound(String key)
        {
            return mSoundSources.Keys.Contains(key.ToUpper());

        }

        public iSoundSourceObject GetSound(String key)
        {

            return mSoundSources[key.ToUpper()];
            //return GetSoundRnd(key);


        }
        public String getRandomSound(String keyprefix)
        {
            String[] gotkeys = (from w in mSoundSources.Keys
                                where w.ToUpper().StartsWith(keyprefix.ToUpper())
                                select w).ToArray();

            int randomindex = BCBlockGameState.rgen.Next(0, gotkeys.Length);
            return gotkeys[randomindex];

        }

        public iSoundSourceObject GetSoundRnd(String key)
        {
            //gets a random sound; for example, if key="TALLYMUSIC", and we have the following:
            //"TALLYMUSIC"
            //"TALLYMUSIC1"
            //"TALLYMUSIC2"
            //etc, it will choose a random one of these.

            //so.... iterate through all of our keys, get those keys that start with the passed key, and choose a random one from that set.



            return mSoundSources[getRandomSound(key)];



        }

        public String AddSound(String filename)
        {
            iSoundSourceObject ss = mDriver.loadSound(filename);
            string usekey = Path.GetFileNameWithoutExtension(filename).ToUpper();
            mSoundSources.Add(usekey, ss);
            return usekey;

        }

        public iActiveSoundObject PlaySound(String key,bool playlooped)
        {
            iSoundSourceObject grabbed = GetSound(key);
            return grabbed.Play(playlooped);
            


        }
        public iActiveSoundObject PlaySound(String key)
        {
            iSoundSourceObject grabbed = GetSound(key);
            return grabbed.Play(false);

        }
        public iActiveSoundObject PlaySound(String key, float volume)
        {
            iSoundSourceObject grabbed = GetSound(key);
            return grabbed.Play(false,volume);

        }

        public iActiveSoundObject PlayMusic(String key)
        {
            return PlayMusic(key,1.0f,true);

        }


        public iActiveSoundObject PlayMusic(String key,float volume,bool loop)
        {
            iSoundSourceObject getsource = GetSoundRnd(key);
            if (mPlayingMusic != null)
            {
                if (getsource == mPlayingMusicSource && mPlayingMusic.Paused)
                    mPlayingMusic.UnPause();
                else 
                    mPlayingMusic.Stop();
            }

            iActiveSoundObject soundobj = getsource.Play(loop,volume);
            mPlayingMusicSource = getsource;
            mPlayingMusic = soundobj;
            return soundobj;

        }
        public iActiveSoundObject PlayMusic()
        {
            if (mPlayingMusic != null)
                mPlayingMusic.UnPause();

            return mPlayingMusic;

        }

        public void PauseMusic()
        {
            mPlayingMusic.Pause();


        }

        public void StopMusic()
        {
            if (mPlayingMusic != null)
                mPlayingMusic.Stop();

            mPlayingMusic=null;
            
        }

        public void LoadSounds(String[] loadfromfolders)
        {
            DirectoryInfo[] makeinfo = new DirectoryInfo[loadfromfolders.Length];
            for (int i = 0; i < makeinfo.Length; i++)
            {
                try
                {
                    makeinfo[i] = new DirectoryInfo(loadfromfolders[i]);


                }
                catch
                {
                    //ignore exceptions...
                }



            }

            LoadSounds(makeinfo);

        }
        public void LoadSounds(DirectoryInfo loadfolder)
        {
            foreach(FileInfo loopfile in loadfolder.GetFiles())
            {

                if(IsSupportedSound(loopfile.FullName))
                {
                    iSoundSourceObject ss = mDriver.loadSound(loopfile.FullName);
                    mSoundSources.Add(Path.GetFileNameWithoutExtension(loopfile.FullName).ToUpper(), ss);






                }




            }


        }

        public void LoadSounds(String loadfolder)
        {
            
            LoadSounds(new DirectoryInfo (loadfolder));

        }



    }
}
