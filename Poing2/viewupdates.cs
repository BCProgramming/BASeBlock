using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GlacialComponents.Controls;
namespace BASeBlock
{
    public partial class frmUpdates : Form
    {

        private class DrawItemData
        {
            public String StringDraw;
            public Exception DownloadError;
            public long ByteSpeed;

            public DrawItemData(String pStringDraw, Exception pDownloadError)
            {
                StringDraw=pStringDraw;
                DownloadError=pDownloadError;

            }


        }


        private int musegameID=-1;
        public frmUpdates(int gameID)
        {
            musegameID = gameID;


        }

        public frmUpdates()
        {
            InitializeComponent();
        }

        private void frmUpdates_Resize(object sender, EventArgs e)
        {
            
            fraavailupdates.Location = new Point(ClientRectangle.Left, ClientRectangle.Top);
        
            
                panLower.Location = new Point(0, ClientSize.Height - panLower.Height);
                fraavailupdates.Size = new Size(ClientSize.Width, panLower.Top-fraavailupdates.Top);
                
            
            

        }
        BCUpdate updateobj;
        Dictionary<BCUpdate.UpdateInfo, ListViewItem> lookupinfo = new Dictionary<BCUpdate.UpdateInfo, ListViewItem>();
        private void frmUpdates_Load(object sender, EventArgs e)
        {
            //load all updates.
            tryagain:
            updateobj = new BCUpdate();
            lvwUpdates.Items.Clear();
            lvwUpdates.Columns.Clear();
            lvwUpdates.Columns.Add("NAME", "Name");
            lvwUpdates.Columns.Add("VERSION", "Version");
            lvwUpdates.Columns.Add("PROGRESS", "Download Progress",256);
            lookupinfo.Clear();
            try
            {
                foreach (BCUpdate.UpdateInfo loopupdate in updateobj.LoadedUpdates)
                {
                    loopupdate.Tag = new DrawItemData("", null);
                    string[] createdstrings = new string[] { loopupdate.DlName, loopupdate.UpdateVersion, "0" };

                    ListViewItem newitem = new ListViewItem(createdstrings);

                    newitem.Tag = loopupdate;

                    lookupinfo.Add(loopupdate, newitem);
                    lvwUpdates.Items.Add(newitem);
                }
            }
            catch(Exception except)
            {
                switch (MessageBox.Show("The Following Exception occured trying to retrieve update information:\n" + except.Message, "Unexpected Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error))
                {
                    case DialogResult.Retry:
                        goto tryagain;
                        break;
                    case DialogResult.Cancel:
                        Close();
                        break;

                }

            }
            

            
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private long lastBytesreceived = 0;
        private void updateprogressforitem(ListViewItem UpdateItem, DownloadProgressChangedEventArgs args)
        {
            int percentage = args.ProgressPercentage;
            //long bytespeed = args.BytesReceived - lastBytesreceived;
            //UpdateItem.ProgressEvent = args;
            UpdateItem.SubItems[2].Text = percentage.ToString();
            DrawItemData itemdraw=((DrawItemData)((BCUpdate.UpdateInfo)UpdateItem.Tag).Tag);
            long currspeed = args.BytesReceived - lastBytesreceived;
            if(currspeed>0)
                itemdraw.ByteSpeed=currspeed;
            lastBytesreceived = args.BytesReceived;
            if(0<args.ProgressPercentage& args.ProgressPercentage<100)
                itemdraw.StringDraw = String.Format("{0}% ({1}KB/s)", args.ProgressPercentage,itemdraw.ByteSpeed);
            else if (args.ProgressPercentage == 100)
            {
                //complete
                if (itemdraw.DownloadError == null)
                {
                    itemdraw.StringDraw = "Ready to Install";



                }
                else
                {
                    itemdraw.StringDraw = "Error";
                    UpdateItem.ToolTipText = "Error:" + itemdraw.DownloadError.Message;
                }


            }
            //((BCUpdate.UpdateInfo)UpdateItem.Tag).Tag = new DrawItemData(
            



        }
        
        private void progressroutine(BCUpdate.UpdateInfo updateobject, DownloadProgressChangedEventArgs args)
        {
            ListViewItem upobj = lookupinfo[updateobject];
            
            updateprogressforitem(upobj, args);

        }
        List<BCUpdate.UpdateInfo> completeddownloads = new List<BCUpdate.UpdateInfo>();
        private void completionroutine(BCUpdate.UpdateInfo updateobject, AsyncCompletedEventArgs args)
        {
            ListViewItem upobj = lookupinfo[updateobject];
            Debug.Print("Completed download of " + updateobject.DlName);
            if (args.Error != null)
                ((DrawItemData)updateobject.Tag).DownloadError = args.Error;


            {
                completeddownloads.Add(updateobject);
            }
            alldownloads--;
            upobj.Checked = false;
            



            if (alldownloads == 0)
            {
                Debug.Print("All Downloads Finished.");

                InstallDownloaded_Thread();


            }

        }
        private void ZipInstall(String installfile)
        {
         


        }
        
        /// <summary>
        /// installs all downloaded updates.
        /// EXE and MSI files will be run directly; .ZIP files will be handled specially (via ZipInstall() which starts out as a stub)
        /// </summary>
        private void InstallDownloaded()
        {
            foreach (BCUpdate.UpdateInfo loopdownloaded in completeddownloads)
            {

                String installfile = loopdownloaded.DownloadedFilename;
                String grabextension = Path.GetExtension(installfile).ToUpper();
                DrawItemData updatedrawdata = (DrawItemData)loopdownloaded.Tag;
                if (grabextension == ".EXE" || grabextension == ".MSI")
                {
                    Debug.Print("Running external Program:" + installfile);
                    try
                    {

                    
                    Process executableProgram = Process.Start(installfile);
                    //wait for it to complete.

                    
                    updatedrawdata.StringDraw = "Installing...";
                    while (!executableProgram.HasExited)
                    {
                        Thread.Sleep(0);
                    }
                    if (executableProgram.ExitCode != 0)
                    {
                        updatedrawdata.StringDraw="Install Error" + executableProgram.ExitCode.ToString();


                    }
                    }
                    catch (Exception ex)
                    {

                        updatedrawdata.StringDraw = "Exec Error:" + ex.Message;
                    }
                }
                else if (grabextension == ".ZIP")
                {
                    ZipInstall(installfile);



                }









            }    



        }
        Thread AsyncInstallthread;
        private void InstallDownloaded_Thread()
        {
            AsyncInstallthread = new Thread(InstallDownloaded);
            AsyncInstallthread.Start();



        }

        int alldownloads = 0;
       

        private void btnDownload_Click(object sender, EventArgs e)
        {

            if (!lvwUpdates.Columns.ContainsKey("PROGRESS"))
            {
                lvwUpdates.Columns.Add("PROGRESS", "Progress");


            }
            foreach (ListViewItem loopitem in lvwUpdates.Items)
            {
                if (loopitem.Checked) alldownloads++;


            }
            completeddownloads = new List<BCUpdate.UpdateInfo>();
            foreach (ListViewItem loopitem in lvwUpdates.Items)
            {
                if (loopitem.Checked)
                {
                    
                    BCUpdate.UpdateInfo upinfo = (BCUpdate.UpdateInfo) loopitem.Tag;
                    upinfo.Tag = new DrawItemData("0%", null);
                    upinfo.DownloadUpdate(progressroutine, completionroutine);
                }

            }
            

        }

        private void lvwUpdates_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            //
        }

        private void lvwUpdates_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //e.DrawDefault = true;
            //
        }
        
        Brush progressbrush;
        private void lvwUpdates_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            //
            ListViewItem thisitem = e.Item;
            BCUpdate.UpdateInfo upinfo = (BCUpdate.UpdateInfo)thisitem.Tag;
            DrawItemData usedrawdata = (DrawItemData)upinfo.Tag;
            //upinfo.DrawSubItem(sender, e);
            DrawItemData drawdata = (DrawItemData)upinfo.Tag;
            if (e.ColumnIndex == 2)
            {

                StringFormat centeralign = new StringFormat();
                centeralign.Alignment = StringAlignment.Center;
                e.DrawDefault = false;
                e.DrawBackground();
                int usepercent;
                int.TryParse(e.SubItem.Text, out usepercent);
                double percentfraction = (double)usepercent / 100;
                e.Graphics.DrawRectangle(new Pen(Color.Black), e.Bounds.Left+2, e.Bounds.Top+2, e.Bounds.Width - 4, e.Bounds.Height - 4);





                if (percentfraction > 0)
                {
                    Rectangle userectangle = new Rectangle(2, 2, (int)(e.Bounds.Width * ((float)percentfraction)),
                                                           e.Bounds.Height-4);





                    if (userectangle.Width >= 1)
                    {
                        userectangle.Offset(e.Bounds.Left, e.Bounds.Top);

                        progressbrush = new LinearGradientBrush(userectangle, Color.Yellow,
                                                                Color.DimGray, 0f);
                        /*if (upinfo.downloadinprogress)
                        {
                            if (usepercent < 100)
                                usestring = String.Format("{0}% ({1} KB/sec)", usepercent, bytespeed / 1024);
                            else
                            {
                                usestring = "Complete.";
                                progressbrush = new LinearGradientBrush(userectangle, Color.ForestGreen,
                                                                        Color.Yellow, 0f);
                            }
                        }*/



                        e.Graphics.FillRectangle(progressbrush, userectangle);
                    }
                }

                e.Graphics.DrawString(drawdata.StringDraw, new Font("Consolas", 10), Brushes.Black,
                                      e.Bounds, centeralign);
            }
            else
            {
                e.DrawDefault = true;
            }



        }
        
       
    }
}
