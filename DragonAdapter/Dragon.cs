using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BASeBlock;
using DragonOgg.MediaPlayer;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

//DragonAdapter; implements Sound Driver for use with DragonOgg.



namespace DragonAdapter
{

    //each DragonSound get's it's own playlist...


    /*
             static void Main(string[] args)
        {
            OggPlaylist playlist = new OggPlaylist();

            // Add the guitar sound
            OggFile guitarFile = new OggFile("GuitarSample.ogg");
            OggPlaylistFile guitarPlayList = new OggPlaylistFile(guitarFile, 0);
            playlist.Add(guitarPlayList);

            // Add the boing sound after the guitar
            OggFile boingFile = new OggFile("BoingSample.ogg");
            OggPlaylistFile boingPlayList = new OggPlaylistFile(boingFile, -1);
            playlist.Add(boingPlayList);

            OggPlayerFBN player = new OggPlayerFBN();

            // Keep playing until our playlist is empty
            while(true)
            {
                while (player.PlayerState != OggPlayerStatus.Waiting
                    && player.PlayerState != OggPlayerStatus.Stopped)
                {
                    Thread.Sleep(5);
                }

                if (playlist.GetNextFile() != null)
                {
                    playlist.CurrentFile.File.ResetFile();
                    player.SetCurrentFile(playlist.CurrentFile.File);
                    player.Play();
                }
                else
                {
                    break;
                }
            }
        }
      
      
     * */

    public class DragonSound : iSoundSourceObject, iActiveSoundObject
    {
        
        private DragonOgg.MediaPlayer.OggPlayer oplayer = null;
        private OggPlaylistFile oggplf;
        private OggPlaylist oggplst;

        public event OnSoundStopDelegate OnSoundStop;

        public event OnSoundPlayDelegate OnSoundPlay;
        public float Progress { get { return oplayer.AmountPlayed; } }
        public void FireSoundPlay(iActiveSoundObject Soundplayed)
        {
            OnSoundPlayDelegate temp = OnSoundPlay;
            if (temp != null)
                temp(Soundplayed);



        }
        public void FireSoundStop(iActiveSoundObject SoundStopped)
        {
            OnSoundStopDelegate temp = OnSoundStop;
            if (temp != null)
                temp(SoundStopped);



        }
        bool flFinished = false;

        [STAThread]
        public void doinit(String snd)
        {
            oplayer = new OggPlayerVBN();
            OggFile oof = new OggFile(snd);
            oggplf = new OggPlaylistFile(oof, 0);
            oggplst = new OggPlaylist();
            oggplst.Add(oggplf);

            oplayer.SetCurrentFile(oof);
            oplayer.StateChanged += new OggPlayerStateChangedHandler(oplayer_StateChanged);
            //oplayer.Play();

        }
        public DragonSound(String SoundFile)
        {
            doinit(SoundFile);
            



        }
        
        bool doloop = false;
        void oplayer_StateChanged(object sender, OggPlayerStateChangedArgs e)
        {
            Debug.Print("State Changed to " + e.NewState + " From " + e.OldState);

            if (e.NewState == OggPlayerStatus.Stopped)
            {
                flFinished = true;
                FireSoundStop(this);

            }
            

            

        }
        [STAThread]
        public iActiveSoundObject Play(bool playlooped)
        {
            return Play(playlooped, 1.0f);
        }
        [STAThread]
        public iActiveSoundObject Play(bool playlooped, float volume)
        {
            doloop = playlooped;
            oplayer.CurrentFile.ResetFile();
            var outputvalue = oplayer.Play();
            Trace.WriteLine("DragonSound::Play result:" + outputvalue);
            flFinished = false;
            FireSoundPlay(this);
            return this;
        }

        public float getLength()
        {
            return oggplf.File.GetTime();
        }

        public bool Finished
        {
            get { return flFinished; }
        }

        public void Stop()
        {
            oplayer.Stop();
        }

        public void Pause()
        {
            oplayer.Pause();
        }

        public void UnPause()
        {
            oplayer.Unpause();
        }

        public bool Paused
        {
            get { return oplayer.PlayerState == OggPlayerStatus.Paused; }
            set { if (value) Pause(); else UnPause(); }
        }

        public void setVolume(float volumeset)
        {
            //
        }

        public iSoundSourceObject Source
        {
            get { return this; }
        }
    }
    
    public class DragonDriver : iSoundEngineDriver
    {
        public static string DrvName = "DragonOgg";
        #region iSoundEngineDriver Members

        public event OnSoundStopDelegate OnSoundStop;

        public event OnSoundPlayDelegate OnSoundPlay;

        public iSoundSourceObject loadSound(byte[] data, string sName, string fileextension)
        {
            throw new NotImplementedException();
        }
        
        public DragonDriver(String pluginfolder)
        {


        }

        [STAThread]
        public iSoundSourceObject loadSound(string filename)
        {
            return new DragonSound(filename);
        }

        public string Name
        {
            get { return DrvName; }
        }
        [STAThread]
        public IEnumerable<string> GetSupportedExtensions()
        {
            yield return ".OGG";
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //meh
        }

        #endregion
    }
}
