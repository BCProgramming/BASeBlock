using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BASeBlock
{
    //class used to watch a given stream to update a progressbar.
    public class StreamProgress : IDisposable 
    {
        public delegate void ProgressFunction(float Percentage, long BytesRead, long TotalBytes);
        private Stream _WatchStream = null;
        private ProgressBar setProgress = null;
        private Thread WatcherThread = null;
        private bool _Cancel = false;
        private ProgressFunction useProgressFunction = null;

        //disposable stuff.
        private bool _Disposed;
        public void Dispose()
        {
            if (_Disposed) return;
            _Disposed = true;
            _Cancel = true;


        }
        ~StreamProgress()
        {
            Dispose();

        }


        public StreamProgress(Stream watchStream, ProgressFunction Callback)
        {
            if (watchStream == null) throw new ArgumentException("watchStream");
            //if (Callback == null) throw new ArgumentException("Callback");

            useProgressFunction = Callback;
            _WatchStream=watchStream;
            WatcherThread = new Thread(ThreadFunction);
            WatcherThread.Start();



        }
        public StreamProgress(Stream watchStream, ProgressBar Target)
        {
            if(watchStream==null) throw new ArgumentNullException("watchStream");
            if (Target == null) throw new ArgumentNullException("Target");

            useProgressFunction = defaultProgress;
            _WatchStream = watchStream;
            setProgress = Target;
            WatcherThread = new Thread(ThreadFunction);
            
            WatcherThread.Start();



        }
        private void defaultProgress(float Percentage, long BytesRead, long TotalBytes)
        {
            Debug.Print("Default Progress.");
            setProgress.Invoke
                ((MethodInvoker)(()=>{ setProgress.Value = 
                    (int)((float)(setProgress.Maximum - setProgress.Minimum) * Percentage) + setProgress.Minimum; }));


        }
        /// <summary>
        /// cancels the progress checking logic.
        /// </summary>
        public void Cancel()
        {
            _Cancel = true;


        }
        long LastPosition = 0;
        float lastpercentage = 0;
        private void ThreadFunction(Object parameter)
        {
            
            Debug.Print("Position:" + _WatchStream.Position);
            LastPosition = 0;
            try
            {
                while (!_Cancel)
                {
                    //sleep for 50 ms.
                    
                    
                    
                    //now update progress bar with current progress... get current progress first, I suppose.
                    long currentpos = _WatchStream.Position;
                    long len = _WatchStream.Length;

                    //percentage...
                    float progresspercent = (float)currentpos / (float)len;
                    //"max out" at 70 percent.
                    progresspercent *= 0.7f;
                    if ((Math.Abs(progresspercent) - 0.7f) < 0.05)
                    {
                        progresspercent = lastpercentage + 0.05f;

                    }
                    progresspercent = Math.Min(progresspercent, 1.0f);
                    lastpercentage = progresspercent;
                    Debug.Print("StreamProgress::ThreadFunction percentage(currentpos=" + currentpos.ToString() + ",len=" + len.ToString() + ":" + progresspercent.ToString());
                    if(currentpos != LastPosition)
                        if(useProgressFunction!=null) useProgressFunction(progresspercent, currentpos, len);




                    Thread.Sleep(50);
                }

            }
            catch (ThreadAbortException ex)
            {
                return;

            }
            catch (Exception exx)
            {
                Debug.Print("Unexpected Exception in Streamprogress class:" + exx.ToString());

            }
        }

    }
}
