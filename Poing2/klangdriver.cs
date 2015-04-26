using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrKlang;

namespace BASeBlock
{

    //some quick n' dirty implementations of the Sound "Driver" concept (cNewSoundManager), for the irrklang lib...
    public class irrklangSound : iActiveSoundObject, ISoundStopEventReceiver
    {
        private bool mfinished = false;
        private ISound msoundobj;
        private ISoundEngine mEngine;
        private irrklangDriver ourdriver;
        private irrklangSSource klSource = null;

        public iSoundSourceObject Source
        {
            get {
            return klSource;
            
            }
        }

        public irrklangSound(irrklangDriver pourdriver, ISoundEngine engineobj,irrklangSSource sourceobj, ISound soundobj)
        {
            ourdriver = pourdriver;
            mEngine = engineobj;
            msoundobj = soundobj;
            klSource = sourceobj;
            msoundobj.setSoundStopEventReceiver(this);
            msoundobj.Paused = false;

        }

        #region iActiveSoundObject Members

        public bool Finished
        {
            get
            {

                return mfinished;


            }
        }
        public void Pause()
        {
            msoundobj.Paused = true;

        }
        public void UnPause()
        {
            msoundobj.Paused = false;

        }
        public bool Paused
        {
            get
            {
                return msoundobj.Paused;


            }
            set { msoundobj.Paused = value; }



        }

        public void setVolume(float volumeset)
        {
            msoundobj.Volume=volumeset;
        }

        public void Stop()
        {
            msoundobj.Stop();
        }

        #endregion

        #region ISoundStopEventReceiver Members

        public void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
    public class irrklangSSource : iSoundSourceObject
    {

        private ISoundSource mSource;
        private ISoundEngine mEngine;
        private irrklangDriver ourdriver;


        public irrklangSSource(irrklangDriver pourdriver, ISoundEngine engineobj, ISoundSource usesource)
        {
            mEngine = engineobj;
            mSource = usesource;
            ourdriver = pourdriver;
        }

        #region iSoundSourceObject Members
        public float getLength()
        {
            return ((float)mSource.PlayLength) / 1000; //value is in ms, return with seconds...


        }

        public iActiveSoundObject Play(bool playlooped)
        {

            ISound gotsound = mEngine.Play2D(mSource, playlooped, true, false);
            irrklangSound newsound = new irrklangSound(ourdriver, mEngine,this, gotsound);
            ourdriver.RaiseOnSoundPlay(newsound);
            if (gotsound.Finished)
                ourdriver.RaiseOnSoundStop(newsound);
            return newsound;
        }
        public iActiveSoundObject Play(bool playlooped,float volume)
        {

            ISound gotsound = mEngine.Play2D(mSource, playlooped, true, false);
            gotsound.Volume=volume;
            irrklangSound newsound = new irrklangSound(ourdriver, mEngine,this, gotsound);
            ourdriver.RaiseOnSoundPlay(newsound);
            if (gotsound.Finished)
                ourdriver.RaiseOnSoundStop(newsound);
            return newsound;
        }

        #endregion
    }
    public class irrklangDriver : iSoundEngineDriver
    {

        private ISoundEngine mEngine;
        public static string DrvName= "IRRKLANG";
        public override string ToString()
        {
            return Name;
        }
        public String Name { get { return "IRRKLANG"; } }
            public irrklangDriver()
        {

            mEngine = new ISoundEngine();

        }

        public irrklangDriver(String DefaultPath)
            : this()
        {

            mEngine.LoadPlugins(DefaultPath);


        }

        #region iSoundEngineDriver Members

        public iSoundSourceObject loadSound(byte[] data, String sName,string fileextension)
        {
            //
            //return new irrklangSSource(
            ISoundSource gotsource = mEngine.AddSoundSourceFromMemory(data, sName);
            return new irrklangSSource(this, mEngine, gotsource);
            
        }

        public iSoundSourceObject loadSound(string filename)
        {
            ISoundSource gotsource = mEngine.AddSoundSourceFromFile(filename);
            return new irrklangSSource(this, mEngine, gotsource);
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            return new string[] { ".MP3", ".WAV", ".OGG", ".FLAC" };
        }

        public void RaiseOnSoundStop(iActiveSoundObject soundstopped)
        {
            OnSoundStopDelegate tempholder = OnSoundStop;
            if (tempholder != null)
                tempholder(soundstopped);



        }

        public void RaiseOnSoundPlay(iActiveSoundObject soundPlayed)
        {
            OnSoundPlayDelegate tempholder = OnSoundPlay;
            if (tempholder != null)
                tempholder(soundPlayed);



        }

        public event OnSoundStopDelegate OnSoundStop;



        public event OnSoundPlayDelegate OnSoundPlay;
        #endregion

        public void Dispose()
        {
            mEngine=null;
        }
    }
}
