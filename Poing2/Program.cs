/*
 * BASeCamp BASeBlock
Copyright (c) 2011, Michael Burgwin
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of BASeCamp Corporation nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
 * BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO 
 * EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BASeCamp.Licensing;
using BASeCamp.Updating;
using Microsoft.WindowsAPICodePack.ApplicationServices;


namespace BASeCamp.BASeBlock
{
    public class FileLogCallback : iManagerCallback
    {
        private StreamWriter logwriter;

        public FileLogCallback(String filename)
        {
            FileStream fstr =null;
            try
            {
                fstr = new FileStream(filename, FileMode.Create, FileAccess.Write);

            }
            catch (Exception ep)
            {
                Debug.Print("Exception in FileLogCallback constructor:" + ep.Message);

            }

            logwriter = new StreamWriter(fstr);
            logwriter.WriteLine("\n Begin Log:" + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
        }

        public FileLogCallback(Stream usestream):this(new StreamWriter(usestream))
        {
            
            

        }
        public FileLogCallback(StreamWriter usewriter)
        {
            logwriter = usewriter;

        }

        #region iManagerCallback Members
        public void UpdateProgress(float ProgressPercentage)
        {
            if(logwriter!=null)
                try { logwriter.WriteLine("[Progress:" + ProgressPercentage + "]"); }
                catch (ObjectDisposedException exx) { }

        }
        public void FlagError(String ErrorDescription, Exception AttachedException)
        {
            try
            {
                logwriter.WriteLine("[Error:" + ErrorDescription + AttachedException != null ? AttachedException.ToString() : "");

            }
            catch (ObjectDisposedException exx) { }


        }
        public void ShowMessage(string message)
        {
            //throw new NotImplementedException();
            if (logwriter != null)
                try
                {
                    logwriter.WriteLine(message);
                    logwriter.Flush();
                }
                catch (ObjectDisposedException eex) { }
        }

        #endregion
    }


    public static class Program
    {
        public static bool isLicensed = false;
        //Enumerate for the run state as we start the application- used for Crash handling purposes during startup.
        public enum CurrentLoadStateConstants
        {
            LoadState_None,
            LoadState_Splash,
            LoadState_Updates,
            LoadState_Editor,
            LoadState_Game



        }


        /// <summary>
        /// quick class for redirecting Debug output to a file.
        /// </summary>
        private class FileLogListener : TraceListener
        {
            private StreamWriter logstream;

            public FileLogListener(StreamWriter filestream)
            {
                logstream=filestream;
                logstream.AutoFlush = true;

            }
            public FileLogListener(String filename):this(new FileInfo(filename))
            {


            }
            public FileLogListener(FileInfo fileinfo):this(new StreamWriter(fileinfo.Open(FileMode.Create)))
            {



            }
            ~FileLogListener()
            {
                
            
            
            }


            public override void Write(string message)
            {
                if (logstream != null)
                {
                    try
                    {
                        logstream.Write(message);
                        logstream.Flush();
                    }
                    catch { }
                }
            }

            public override void WriteLine(string message)
            {
                if (logstream != null)
                {

                    try { logstream.WriteLine(message); }catch{}
                }
            }
        }
        /// <summary>
        /// Quick 'n dirty class to output Debug info to the console.
        /// </summary>
        private class ConsoleListener : TraceListener
        {




            public override void Write(string message)
            {
                //throw new NotImplementedException();
                Console.Write(message);
            }

            public override void WriteLine(string message)
            {
                Console.WriteLine(message);
            }
        }
        private static String getdebugfilename()
        {
            
            //String appfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //appfolder=Path.Combine(appfolder,"BASeBlock");
            String appfolder = BCBlockGameState.AppDataFolder;
            if (!Directory.Exists(appfolder)) Directory.CreateDirectory(appfolder);
            String filename = Path.Combine(appfolder,"debug" + (DateTime.Now.Year.ToString() +
                              DateTime.Now.Month.ToString() +
                              DateTime.Now.Day.ToString() +
                                DateTime.Now.Millisecond.ToString()) + ".log");


            return filename;



        }
        private static bool isWin7()
        {

            var osverinfo = Environment.OSVersion;
            //Win7 is version 6.1; anything larger supports the API codepack, presumably.
            return (osverinfo.Version > new Version(6, 1)) && Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported;


        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DebugLogger.EnableLogging = true;
            //added Feb 03 2012: licensing.
         
            
            //added Dec 08 2010: logging

            String cmdline = System.Environment.CommandLine.ToUpper();


           

            try
            {


                
                

                if (System.Environment.CommandLine.ToUpper().Contains("/FILELOG"))
                {
                    var createdlistener = new FileLogListener(getdebugfilename());

                    //Debug.Listeners.Add(createdlistener);
                    Trace.Listeners.Add(createdlistener);

                }
                if (System.Environment.CommandLine.ToUpper().Contains("/VERBOSE"))
                {
                    BCBlockGameState.Verbose = true;

                    Debug.Listeners.Add(new ConsoleListener());
                }
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Debug.Print("Setting ThreadExit handler...");
                Application.ThreadExit += new EventHandler(Application_ThreadExit);
                Debug.Print("Setting Exception Handler...");
                Application.ThreadException +=
                    new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //register this app...

                //TODO: fix the Update library so that it provides a better UX.
                //cannot work on that currently since I have no connection (sadface).
                if (NetworkInterface.GetIsNetworkAvailable() &&
                    !System.Environment.CommandLine.ToUpper().Contains("/SKIPUPDATE"))
                {
                    try
                    {
                        String updatetext =
                            "A new Version of {0} is available. \n Current Version: {1}\nNew Version: {2}\n Would you like to update?";
                        String Forceupdatetext =
                            "An Update of {0} is being forced. Installed Version is {1}; update version is {2}. Continue?";
                        String useupdatetext = updatetext;

                        BCUpdate.RegisterApplication("BASeBlock", BCBlockGameState.GameID,
                                                                 Assembly.GetExecutingAssembly().GetName().Version.
                                                                     ToString(),
                                                                 BCBlockGameState.GetAssemblyFile());



                    }
                    catch (Exception ave)
                    {
                        Debug.Print("Failed to Register BASeBlock with the BASeCamp Updating Library-" + ave.Message);



                    }
                }

                CurrentLoadStateConstants currentstep = CurrentLoadStateConstants.LoadState_None;
                try
                {

                    if (System.Environment.CommandLine.ToUpper().Contains("/SHOWUPDATES"))
                    {
                        currentstep = CurrentLoadStateConstants.LoadState_Updates;
                        Application.Run(new frmUpdates());


                    }
                    else
                    {

                        BCBlockGameState.ProductRegistration = ProductKey.GetProductInformation(
                            Assembly.GetExecutingAssembly().GetName().Name, ProductKey.Products.BASeBlock, true, null,null);
                        isLicensed = BCBlockGameState.ProductRegistration != null;
                    
                        if (!System.Environment.CommandLine.Contains("/NOSPLASH"))
                        {
                            currentstep = CurrentLoadStateConstants.LoadState_Splash;
                            var usesplash = new SplashScreen();
                            Application.Run(usesplash);
                            if (usesplash.DoQuit)
                            {
                                BCBlockGameState.Cleanup();
                                return;

                            }
                            //hopefully, this will allow the "tempdeleter" instances to delete the files created during the run...
                        }

                        if(isWin7())
                        {
                        String exepath = Assembly.GetExecutingAssembly().Location;
                        Microsoft.WindowsAPICodePack.ApplicationServices.RestartSettings rs = new RestartSettings(exepath + " /RECOVER", RestartRestrictions.NotOnReboot);
                        ApplicationRestartRecoveryManager.RegisterForApplicationRestart(rs);
                        }

                        if (System.Environment.CommandLine.ToUpper().Contains("/DEBUG"))
                        {
                            Application.Run(new Debuggerform());
                            return;

                        }
                        else if (System.Environment.CommandLine.ToUpper().Contains("/EDITOR"))
                        {
                            currentstep = CurrentLoadStateConstants.LoadState_Editor;
                            Application.Run(new frmEditor());
                        }
                        else
                        {
                            currentstep = CurrentLoadStateConstants.LoadState_Game;
                            Application.Run(new frmBaseBlock());
                        }
                        BCBlockGameState.Soundman.Dispose();
                    }
                }
                catch (Exception fatalerr)
                {
                    MessageBox.Show("Error During init. Stack Trace:\n" + fatalerr.StackTrace + "\nMessage:" +
                                    fatalerr.Message + "\n Occured at step " +
                                    Enum.GetName(typeof(CurrentLoadStateConstants), currentstep));

                }
                finally
                {
                    BCBlockGameState.Cleanup();


                }
            }
            catch (Exception ee)
            {
                //cleanup the mutex...


            }


        }



        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            String writehere = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BASeBlock//crash.log");
            using(FileStream writeit = new FileStream(writehere,FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(writeit);
                sw.WriteLine(e.ExceptionObject.ToString());
                sw.Flush();
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Debug.Print("ThreadException:" + e.Exception.ToString());
        }

        static void Application_ThreadExit(object sender, EventArgs e)
        {
            Debug.Print("Thread Exit" + e.ToString());
        }
    }
}
