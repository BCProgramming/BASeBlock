using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BASeBlock
{
    //URL to retrieve info on all updates...
    //http://bc-programming.com/update.php?action=getupdate
    //returns a list, delimited by $$$; each item is a set of items further delimited by "&&&":
    // DlID = splitvalue(0)
   // Version = splitvalue(1)
   // fullURL = splitvalue(2)
   // DlSummary = splitvalue(3)
   // DocURL = splitvalue(4)
   // FileSize = splitvalue(5)
   // dateuse = splitvalue(6)
   /// DlName = splitvalue(7)
    public class BCUpdate
    {

        public class UpdateInfo
        {
            public int dlID { get; set; }
            public String UpdateVersion { get; set; }
            public String fullURL { get; set; }
            public String DlSummary { get; set; }
            public String DocURL { get; set; }
            public int FileSize { get; set; }
            public String dateuse { get; set; }
            public String DlName { get; set; }
            public Object Tag { get; set; }
            /*Private Enum VersionChangeAmountEnum
    Version_Major = 1
    Version_Minor = 2
    Version_Revision = 4
    Version_Build = 8
End Enum*/
            [Flags]
            public enum compareVersionEnum
            {
                EVersion_Major=1,
                EVersion_Minor=2,
                EVersion_Revision=3,
                EVersion_Build=4



            }
            public static compareVersionEnum VersionsChanged(String InstalledVersion, String NewVersion)
            {
                String[] SplitInstalled = InstalledVersion.Split('.');
                String[] SplitNew = NewVersion.Split('.');


            }
            /*
             
             Private Function VersionsChanged(InstalledVersion As String, NewVersion As String) As VersionChangeAmountEnum

Dim splitInstalled() As String
Dim SplitNew() As String
Dim loopTo As Long, runchange As Long
Dim i As Long
splitInstalled = Split(InstalledVersion, ".")
SplitNew = Split(NewVersion, ".")
If UBound(splitInstalled) < UBound(SplitNew) Then loopTo = UBound(splitInstalled) Else loopTo = UBound(SplitNew)
For i = 0 To loopTo
    'if it is NOT numeric, take the ASCII sum...
    If Not IsNumeric(splitInstalled(i)) Then
        splitInstalled(i) = AscSum(splitInstalled(i))
    End If
    If Not IsNumeric(SplitNew(i)) Then
        SplitNew(i) = AscSum(SplitNew(i))
    End If
    
    If Val(splitInstalled(i)) < Val(SplitNew(i)) Then
        runchange = runchange + (2 ^ i)
    
    End If

Next i

VersionsChanged = runchange






End Function 
              
             * */


          

            public String DownloadedFilename { get; set; }
            public DownloadProgressChangedEventArgs ProgressEvent { get; set; }
           private WebClient usecli;
            
            [DllImport("kernel32.dll", EntryPoint="GetTempPathA")]
private static extern int GetTempPath (int nBufferLength, string lpBuffer);
            private Brush progressbrush;

  

            private static String GetTempFile(String useextension)
            {
                String tpath = new string(' ',1024);
                GetTempPath(1023,tpath);
                tpath = tpath.Replace('\0',' ').Trim();
                String destfilename = Guid.NewGuid().ToString() + "." + useextension;
                return Path.Combine(tpath,destfilename);





            }


            private string GetURL(String urlload)
            {
                StreamReader sreader = new StreamReader(usecli.OpenRead(urlload));
                String returnthis = sreader.ReadToEnd();
                sreader.Close();
                return returnthis;
            }

            public UpdateInfo(int GameID)
            {
                //updates with a specific gameID
                String useurl = String.Format(updateURLwithID,GameID);
                String resulttext = GetURL(useurl);
                SetParameters(resulttext);

            }

            internal UpdateInfo(string updatestring)
            {
                //split at "&&&"
                SetParameters(updatestring);
            }

            /// <summary>
            /// Downloads the update file to a temporary location.
            /// </summary>
            /// <returns>the downloaded data</returns>
            public String DownloadUpdate(DownloadUpdateProgressChangedFunction pprogressroutine,DownloadUpdateCompletedFunction pcompletionroutine)
            {
                //set class level variable to be the two specified update routines
                if (!downloadinprogress)
                {
                    lastbytesreceived = 0; bytespeed = 0;
                    downloadinprogress=true;
                    downloadinprogress = true;
                    completionRoutine = pcompletionroutine;
                    ProgressRoutine = pprogressroutine;
                    string useextension = fullURL.Substring(fullURL.LastIndexOf('.') + 1);
                    String usetempfile = GetTempFile(useextension);
                    Debug.Print("using temp file:" + usetempfile);
                    Uri makeuri = new Uri(fullURL);
                    DownloadedFilename = usetempfile;
                    usecli.DownloadFileAsync(makeuri, usetempfile);
                    return usetempfile;
                }
                return "";
            }
            public bool downloadinprogress=false;
            private DownloadUpdateCompletedFunction completionRoutine;
            private DownloadUpdateProgressChangedFunction ProgressRoutine;
            public delegate void DownloadUpdateCompletedFunction(UpdateInfo updatecompleted,System.ComponentModel.AsyncCompletedEventArgs e);
            public delegate void DownloadUpdateProgressChangedFunction(UpdateInfo updateobject, DownloadProgressChangedEventArgs e);
            private void SetParameters(string updatestring)
            {
                String[] splitvalue = updatestring.Split(new string[] { "&&&" }, StringSplitOptions.None);
                int outdlid;
                int.TryParse(splitvalue[0],out outdlid);
                UpdateVersion = splitvalue[1];
                fullURL = splitvalue[2];
                DlSummary = splitvalue[3];
                DocURL = splitvalue[4];
                int outsize;

                try
                {
                    int.TryParse(splitvalue[5], out outsize);
                    FileSize = outsize;
                }
                catch (FormatException fe)
                {
                    FileSize = 0;
                }
                dateuse = splitvalue[6];
                DlName = splitvalue[7];
                usecli=new WebClient();
                usecli.DownloadProgressChanged+=new DownloadProgressChangedEventHandler(usecli_DownloadProgressChanged);
                usecli.DownloadFileCompleted+=new System.ComponentModel.AsyncCompletedEventHandler(usecli_DownloadFileCompleted);
            }
            
void  usecli_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
{
    downloadinprogress=false;
    if (e.Error != null)
    {
        progressbrush = new SolidBrush(Color.Red);


    }
    if(completionRoutine!=null)
        completionRoutine(this,e);
    lastbytesreceived = 0; bytespeed = 0;
}

private long lastbytesreceived = 0, bytespeed = 0;
void  usecli_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
{
    bytespeed = e.BytesReceived - lastbytesreceived;
    lastbytesreceived = e.BytesReceived;
    if(ProgressRoutine!=null)
        ProgressRoutine(this,e);

}
        }
        const string updateURL = @"http://bc-programming.com/update.php?action=getupdate";
        const string updateURLwithID = @"http://bc-programming.com/update.php?action=getupdate&dlid={0}";
        private WebClient usecli = new WebClient();
        public List<UpdateInfo> LoadedUpdates=null;
        public BCUpdate()
        {
            //loads update information for all items.
            String urlData = GetURL(updateURL);

            //split at "$$$"

            String[] updatelines = urlData.Split(new string[] {"$$$"},StringSplitOptions.RemoveEmptyEntries);
            //loop through each, create a new Update object for each one, and add it to our list.
            LoadedUpdates=new List<UpdateInfo>();
            foreach(String loopupdate in updatelines)
            {
                UpdateInfo addthis = new UpdateInfo(loopupdate);
                LoadedUpdates.Add(addthis);





            }




        }
        
       private string GetURL(String urlload)
        {
            StreamReader sreader = new StreamReader(usecli.OpenRead(urlload));
            String returnthis = sreader.ReadToEnd();
            sreader.Close();
            return returnthis;
        }
    }
}
