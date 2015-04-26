using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
//using csvorbis;
using OpenTK.Audio;

using BASeBlock;
namespace DragonAdapter
{


    //Implementation of BASeBlock "Audio Driver" classes, using OpenTK and OpenAL.
    //current major weakness being it won't load ogg files... grrr.

    public class TKAudioPoller
    {

        private class TKSourceTrackData
        {
            
            public ALSourceState PreviousState;
            public TKSourceTrackData(TKAudioSource tksource)
            {
                //initialize PreviousState to the current state of the source.
                PreviousState = OpenTK.Audio.AL.GetSourceState(tksource._source);
                

                

            }


        }

        private Dictionary<TKAudioSource, TKSourceTrackData> sources = new Dictionary<TKAudioSource, TKSourceTrackData>();
        private Thread PollThread = null;
        public TKAudioPoller(TKDriver driverobj)
        {
            //Listens to the source objects to fire  stop events.
            //will be instantiated by the Driver object, which hooks each AudioSource and will
            //add it to this object when they are played.
            


        }
        private void PollRoutine(Object parameter)
        {
            try
            {
                for (; ; )
                {
                    if (sources.Count == 0)
                    {
                        return; //no work to do...
                        PollThread = null; //set thread var to null so Play can see it and restart it...
                    }
                    foreach (var kvp in sources)
                    {
                        //compare the current state to the previous one....
                        ALSourceState checkstate = OpenTK.Audio.AL.GetSourceState(kvp.Key._source);
                        if (checkstate != kvp.Value.PreviousState)
                        {
                            //state has changed!
                            switch (checkstate)
                            {
                                case ALSourceState.Initial:
                                    break;
                                    case ALSourceState.Paused:
                                    //hmmm....
                                    break;
                                    case ALSourceState.Playing:
                                    kvp.Key.FireSoundPlay();
                                    break;
                                    case ALSourceState.Stopped:
                                    kvp.Key.FireSoundStop(); //Driver object has this hooked and should remove us ('untrack').
                                    break;

                            }

                        }

                        kvp.Value.PreviousState = checkstate;



                    }


                    Thread.Sleep(250);
                }




            }
            catch (ThreadAbortException ex)
            {
                //meh
            }
        }



        public void TrackSource(TKAudioSource trackit)
        {
            if (!sources.ContainsKey(trackit)) sources.Add(trackit,new TKSourceTrackData(trackit));

            if (PollThread == null)
            {

                PollThread = new Thread(PollRoutine);
                PollThread.Start();

            }

        }
        public void UntrackSource(TKAudioSource untrack)
        {
            if (sources.ContainsKey(untrack)) sources.Remove(untrack);

        }



    }

    
    public class TKAudioSource : iSoundSourceObject, iActiveSoundObject
    {
        internal uint _buffer;
        internal uint _source;
        internal bool willLoop = false;
        public TimeSpan Length = new TimeSpan();
        public event OnSoundStopDelegate OnSoundStop;
        public event OnSoundPlayDelegate OnSoundPlay;

        internal void FireSoundStop()
        {
            var copied = OnSoundStop;
            copied(this);

        }
        internal void FireSoundPlay()
        {
            var copied = OnSoundPlay;
            copied(this);

        }

        internal TKAudioSource(uint buffer,TimeSpan audiolength)
        {
            _buffer=buffer;
            AL.GenSource(out _source);
            
            

        }

        #region iSoundSourceObject Members

        public iActiveSoundObject Play(bool playlooped)
        {
            willLoop = playlooped;
            //OpenTK.Audio.OpenAL.AL.BindBufferToSource
            OpenTK.Audio.AL.SourcePlay(_source);
            FireSoundPlay();
            return this;
        }

        public iActiveSoundObject Play(bool playlooped, float volume)
        {
            //throw new NotImplementedException();
            return Play(playlooped);
        }

        public float getLength()
        {
            return (float)Length.TotalSeconds;
        }

        #endregion

        #region iActiveSoundObject Members

        public bool Finished
        {
            get { return OpenTK.Audio.AL.GetSourceState(_source) == ALSourceState.Stopped; }
        }

        public void Stop()
        {
            OpenTK.Audio.AL.SourceStop(_source);
        }

        public void Pause()
        {
            if (OpenTK.Audio.AL.GetSourceState(_source) != ALSourceState.Paused)
                OpenTK.Audio.AL.SourcePause(_source);
        }

        public void UnPause()
        {
            if (OpenTK.Audio.AL.GetSourceState(_source) == ALSourceState.Paused)
                OpenTK.Audio.AL.SourcePlay(_source);
        }
        public float Progress { get { return 0; } }
        public bool Paused
        {
            get
            {
                return OpenTK.Audio.AL.GetSourceState(_source) == ALSourceState.Paused;
            }
            set
            {
                if (value) Pause(); else UnPause();
            }
        }

        void iActiveSoundObject.setVolume(float volumeset)
        {
            //No code...
        }

        iSoundSourceObject iActiveSoundObject.Source
        {
            get { return this; }
        }

        #endregion
    }


    //TKDriver: implements sound Driver using OpenAL via OpenTK.
    public class TKDriver : iSoundEngineDriver
    {
        public static String DrvName = "OpenTK";
        private TKAudioPoller PollingObject = null;
        public static AudioContext ac;
         public static XRamExtension xram;
         internal void FireSoundStop(iActiveSoundObject parameter)
         {
             var copied = OnSoundStop;
             copied(parameter);

         }
         internal void FireSoundPlay(iActiveSoundObject parameter)
         {
             var copied = OnSoundPlay;
             copied(parameter);

         }

         private void EnsureDLLLocation()
         {
             String appfolder = BCBlockGameState.AppDataFolder;
             String x86Folder = Path.Combine(appfolder, "lib\\x86");
             String x64Folder = Path.Combine(appfolder, "lib\\x64");
             string executablefolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
             string targetlocation = Path.Combine(executablefolder, "OpenAL32.dll");
             String sourcefilename;
             if (BCBlockGameState.Isx64())
             {

                 sourcefilename = Path.Combine(x64Folder, "OpenAL64.dll");

             }
             else
             {

                 sourcefilename = Path.Combine(x86Folder, "OpenAL32.dll");
             }

             if (File.Exists(targetlocation))
                 File.Delete(targetlocation);

             File.Copy(sourcefilename, targetlocation, true);




         }
         public TKDriver(String pluginfolder):this()
         {


         }
        public TKDriver()
        {
            //make sure proper OpenAL file is in application folder.
            //OpenAL32.dll in lib//x86, OpenAL64.dll in lib//x64. Copy to
            //Application folder the proper one (and rename it to 32 as well if necessary)
            EnsureDLLLocation();

            if (ac==null) ac = new AudioContext();
            if (xram == null) xram = new XRamExtension();
            PollingObject = new TKAudioPoller(this);
        }



        #region iSoundEngineDriver Members

        public event OnSoundStopDelegate OnSoundStop;

        public event OnSoundPlayDelegate OnSoundPlay;

        public iSoundSourceObject loadSound(byte[] data, string sName, string fileextension)
        {
            uint gotBuffer=0;
            MemoryStream ms = new MemoryStream(data);
            AudioReader ar = new AudioReader(ms);

            
            

            AL.GenBuffer(out gotBuffer);

            return null;
            

        }

        public iSoundSourceObject loadSound(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".wav")
            {
                uint gotbuffer = 0;
                AL.GenBuffer(out gotbuffer);
                //
                //if (xram.IsInitialized) xram.SetBufferMode(ac.  ref gotbuffer, XRamExtension.XRamStorage.Automatic);
                AudioReader ar = new AudioReader(filename);

                SoundData sd = ar.ReadToEnd();

                int numsamples = sd.Data.Length;

                if (sd.SoundFormat.SampleFormat == SampleFormat.Mono16 || sd.SoundFormat.SampleFormat == SampleFormat.Stereo16)
                {
                    //divide by two (16-bits/2 bytes per sample)
                    numsamples /= 2;

                }
                //sd.SoundFormat.SampleRate
                //number of samples divided by sample rate should give length in seconds...
                var lengthinseconds = (double)numsamples / (double)sd.SoundFormat.SampleRate;
                //create a timespan...

                TimeSpan tsbuild = new TimeSpan(0, 0, 0, 0, (int)(lengthinseconds * 1000));


                AL.BufferData(gotbuffer, sd);

                TKAudioSource buildsource = new TKAudioSource(gotbuffer, tsbuild);
                buildsource.OnSoundPlay += new OnSoundPlayDelegate(buildsource_OnSoundPlay);
                buildsource.OnSoundStop += new OnSoundStopDelegate(buildsource_OnSoundStop);
                return buildsource;
            }
            return null;
        }

        void buildsource_OnSoundStop(iActiveSoundObject objstop)
        {
            PollingObject.UntrackSource(objstop as TKAudioSource); //Polling Object's work is done :P
            FireSoundStop(objstop);
        }

        void buildsource_OnSoundPlay(iActiveSoundObject objplay)
        {
            PollingObject.TrackSource(objplay as TKAudioSource); 
            FireSoundPlay(objplay);
        }

        public string Name
        {
            get { return "OpenAL"; }
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            yield return ".WAV";
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            ac.Dispose();
        }

        #endregion
    }
     
}
