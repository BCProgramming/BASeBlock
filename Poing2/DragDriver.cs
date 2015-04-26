using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
/*
using DragonOgg.MediaPlayer;

namespace BASeBlock
{
    public class DragSound : iActiveSoundObject, iSoundSourceObject
    {
        private OggFile FileObject;
        private OggPlayer playerobj;
        private bool doloop = false;

        internal event OnSoundPlayDelegate OnSoundPlay;
        internal event OnSoundStopDelegate OnSoundStop;

        internal void FireSoundPlay()
        {
            var copied = OnSoundPlay;

            if(copied!=null) copied(this);


        }
        internal void FireSoundStop()
        {
            var copied = OnSoundStop;
            if (copied != null) copied(this);


        }

        public DragSound(OggFile FilePlayer)
        {
            playerobj = new OggPlayerVBN();
            playerobj.SetCurrentFile(FilePlayer);
            
            playerobj.StateChanged += new OggPlayerStateChangedHandler(playerobj_StateChanged);

        }

        void playerobj_StateChanged(object sender, OggPlayerStateChangedArgs e)
        {
            if(e.NewState == OggPlayerStatus.Stopped)
            {
                if(e.OldState==OggPlayerStatus.Playing|| e.OldState==OggPlayerStatus.Paused)
                    FireSoundStop();
                if (doloop) Play(true);

            }


            if (e.NewState == OggPlayerStatus.Playing)
            {

                if (e.OldState == OggPlayerStatus.Stopped || e.OldState == OggPlayerStatus.Paused)
                    FireSoundPlay();


            }

            
        }

        public bool Finished
        {
            get { return playerobj.PlayerState == OggPlayerStatus.Stopped; }
        }

        public void Stop()
        {
            playerobj.Stop();
        }

        public void Pause()
        {
            playerobj.Pause();
        }

        public void UnPause()
        {
            playerobj.Unpause();
        }

        public bool Paused
        {
            get { return playerobj.PlayerState == OggPlayerStatus.Paused; }
            set { if (value) Pause(); else UnPause(); }
        }

        public void setVolume(float volumeset)
        {
            ///
            
        }

        public iSoundSourceObject Source
        {
            get { return this; }
        }

        public iActiveSoundObject Play(bool playlooped)
        {
            doloop = playlooped;
            playerobj.Play();
            return this;
        }

        public iActiveSoundObject Play(bool playlooped, float volume)
        {
            doloop = playlooped;
            playerobj.Play();
            return this;
        }

        public float getLength()
        {
            return playerobj.FileLengthTime;
        }
    }


    public class DragDriver:iSoundEngineDriver
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //nothing...
        }

        public event OnSoundStopDelegate OnSoundStop;
        public event OnSoundPlayDelegate OnSoundPlay;
        private void InvokeStop(iActiveSoundObject active)
        {
            var copied = OnSoundStop;
            if (copied != null) copied(active);
            

        }
        private void InvokePlay(iActiveSoundObject active)
        {

            var copied = OnSoundPlay;
            if (copied != null) copied(active);

        }
        public DragDriver(String pluginfolder)
        {


        }
        public DragDriver()
        {


        }
        public static string DrvName = "Dragon";
        public iSoundSourceObject loadSound(byte[] data, string sName, string fileextension)
        {
            //create a temporary Ogg file.
            String tempfilename = Path.GetTempFileName();
            tempfilename = Path.GetFileNameWithoutExtension(tempfilename) + ".ogg";
            

            using (FileStream writeout = new FileStream(tempfilename, FileMode.Create)) 
            {
                writeout.Write(data, 0, data.Length);
                writeout.Close();
                BCBlockGameState.QueueDelete(tempfilename); //queue that file for deletion later.
            }
            return loadSound(tempfilename);

        }

        public iSoundSourceObject loadSound(string filename)
        {
            OggFile ogfile = new OggFile(filename);
            DragSound ds = new DragSound(ogfile);
            ds.OnSoundPlay += new OnSoundPlayDelegate(ds_OnSoundPlay);
            ds.OnSoundStop += new OnSoundStopDelegate(ds_OnSoundStop);
            return ds;
        }

        void ds_OnSoundStop(iActiveSoundObject objstop)
        {
            InvokeStop(objstop);
        }

        void ds_OnSoundPlay(iActiveSoundObject objplay)
        {
            InvokePlay(objplay);
        }

        public string Name
        {
            get { return "Dragon"; }
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            yield return ".OGG";
        }
    }
}
*/