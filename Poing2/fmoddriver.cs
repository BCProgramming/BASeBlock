using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
/*
using FMODNet;
namespace BASeCamp.BASeBlock
{
    public class fmodSoundSource : iSoundSourceObject,iActiveSoundObject
    {
        private FMODNet.Sound fsound;
        private fmoddriver driverobj;
        private bool playlooped=false;
        private bool mfinished=false;
        private int mPausedPosition;
        private bool islooping;
        private float playvolume;
        public void SCallback(FMODNet.Sound soundobj,uint position)
        {
            Debug.Print("SCallback routine called; position=" + position);
            
            if (playlooped)
            {
                fsound.Play();
            }
            else
            {
                mfinished=true;


            }

        }

        public fmodSoundSource(fmoddriver pdriverclass, FMODNet.Sound psoundobject)
        {

            driverobj = pdriverclass;
            fsound = psoundobject;
            

        }



        #region iSoundSourceObject Members

        public iActiveSoundObject Play(bool playlooped)
        {
            try
            {
                fsound.AddCallback((uint)fsound.Length, SCallback);
            }
            catch
            {
            }
            islooping=true;
            fsound.Play();
            mfinished = false;
            return this;
        }
        public iActiveSoundObject Play(bool playlooped,float volume)
        {
            try
            {
                fsound.AddCallback((uint)fsound.Length, SCallback);
            }
            catch
            {
            }
            islooping=true;
            playvolume=volume;
            fsound.Channel.Volume = volume;
            fsound.Channel.Position = mPausedPosition;
            fsound.Play();
            mfinished = false;
            return this;
        }
                
        #endregion

        #region iActiveSoundObject Members

        public bool Finished
        {
            get { return mfinished; }
        }

        public void Stop()
        {
            fsound.Stop();
        }

        public void Pause()
        {
            mPausedPosition=fsound.Channel.Position;
            //force to the end...
            fsound.Channel.Position = fsound.Length;
            //fsound.Stop();
            
        }

        public void UnPause()
        {
            Play(islooping, playvolume);
        }

        public bool Paused
        {
            get
            {
                return false;
            }
            set
            {
                if (value) Pause(); else UnPause();
            }
        }

        #endregion
    }
    public class fmoddriver : iSoundEngineDriver
    {

        public FMODNet.SoundDevice sdevice = new SoundDevice();
        public String Name { get { return "FMOD"; } }
            public fmoddriver()
        {

            sdevice.Initialize();
        }

        #region iSoundEngineDriver Members

        public event OnSoundStopDelegate OnSoundStop;

        public event OnSoundPlayDelegate OnSoundPlay;

        public iSoundSourceObject loadSound(string filename)
        {
            return new fmodSoundSource(this, sdevice.CreateSound(filename));
            
        }

        public string[] GetSupportedExtensions()
        {
            return new string[] { ".MP3", ".WAV", ".OGG", ".FLAC" };
        }

        #endregion
    }
}
*/