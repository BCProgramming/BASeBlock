using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace BASeBlock
{
    /// <summary>
    /// Debug logging output class. Can automatically transform Debug output (Debug.Print) and save it to a properly dated and organized
    /// file in %APPDATA%\Mainframe\DebugLogs\AppName. Just make sure it get's initialized by calling it in some fashion- setting EnableDebugging to True is usually good enough.
    /// Note that Debug output will not appear for Release builds, if we ever use them.
    /// </summary>
    public class DebugLogger : TraceListener
    {
        public static bool EnableLogging = true;
        public static bool FullExceptionLogging = false;
        public static DebugLogger Log = new DebugLogger(Application.ProductName);
        private String _LoggerName;
        private StreamWriter LogStream = null;
        private String _ActiveLogFile;
        public String ActiveLogFile
        {
            get { return _ActiveLogFile; }
        }
        object logStreamLock = new object();

        private void InitLog()
        {
            InitLog(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BASeCamp\\DebugLogs"));
        }

        private void InitLog(String sLogFolder)
        {
            PurgeOldLogs();
            String BasePath = Path.Combine(sLogFolder, _LoggerName);
            Directory.CreateDirectory(BasePath);
            String LogFileUse = Path.Combine(BasePath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffffff") + "." +
                (new Random().Next()).ToString("x8") + ".log");
            FileStream fs = new FileStream(LogFileUse, FileMode.CreateNew);
            try
            {
                LogStream = new StreamWriter(fs);
                _ActiveLogFile = LogFileUse;
            }
            catch
            {
                fs.Dispose();
                throw;
            }

            lock (logStreamLock)
            {
                LogStream.WriteLine("--Log Initialized--");
                WriteLogHeader();
            }
            System.Windows.Forms.Application.ThreadException += Application_ThreadException;
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            //for logging we'll trace unhandled exceptions.
            //catch-all exception in case anything goes wrong. 
            try
            {
                WriteLine(e.Exception.ToString());

            }
            catch (Exception exx)
            {

            }
        }

        private void WriteLogHeader()
        {
            LogStream.WriteLine("--" + DateTime.Now.ToString() + "--");
            LogStream.WriteLine(Application.ProductName);
            LogStream.WriteLine(Application.CompanyName);
            LogStream.WriteLine("Main executable file: " + Assembly.GetExecutingAssembly().Location);
            LogStream.WriteLine("=== System Information ===");
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM " + "win32_operatingsystem"))
                {
                    foreach (ManagementObject wmi_HD in searcher.Get())
                    {
                        PropertyDataCollection searcherProperties = wmi_HD.Properties;
                        foreach (PropertyData sp in searcherProperties)
                        {
                            LogStream.WriteLine(sp.Name + " = " + sp.Value);
                        }
                    }
                }
                LogStream.WriteLine("=== End System Information ===");
            }
            catch (Exception exx)
            {
                LogStream.WriteLine("Exception retrieving System Info:" + exx.ToString());
            }
        }

        private static TimeSpan AgeCutoff = new TimeSpan(7, 0, 0, 0); //two weeks.
        private void PurgeOldLogs()
        {
            String BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BASeCamp\\DebugLogs", _LoggerName);
            if (!Directory.Exists(BasePath)) return;
            var getdir = new DirectoryInfo(BasePath);
            foreach (var iterate in getdir.GetFiles("*.log"))
            {
                if ((DateTime.Now - iterate.LastWriteTime) > new TimeSpan(64, 0, 0, 0))
                {
                    try
                    {
                        iterate.Delete();
                    }
                    catch (IOException exx)
                    {
                        //
                    }
                }
            }

        }

        public override void Write(String LogMessage)
        {
            lock (logStreamLock)
            {
                LogStream.Write(DateTime.Now.ToString("hh:mm:ss") + ">>" + LogMessage);
                LogStream.Flush();
            }
        }

        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }

        public DebugLogger(String sName, String sLogFolder)
        {
            _LoggerName = sName;
            InitLog(sLogFolder);
            if (EnableLogging)
                Debug.Listeners.Add(this);
            //Trace.Listeners.Add(this);
        }

        public DebugLogger(String sName)
        {
            _LoggerName = sName;
            InitLog();
            if (EnableLogging)
            {
                Debug.Listeners.Add(this);
            }
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        }

        void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (!FullExceptionLogging) return;
            try
            {
                this.WriteLine(e.Exception.ToString());
            }
            catch (Exception exx)
            {
                //Application must be in a very bad state- or, the error was actually caused by DebugLogger.
            }

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Log the Exception. Note that we don't want ANOTHER exception to occur, so we wrap it all in a try-catch.
            try
            {
                this.WriteLine(e.ExceptionObject.ToString());
            }
            catch (Exception exx)
            {
                //Application must be in a very bad state- or, the error was actually caused by DebugLogger.
            }

        }
    }

}
