using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BASeCamp.BASeBlock
{
    public static class Win32File
    {

        private static String[] stdbyteprefixes = new String[]
    {
        " Bytes",
        "KB",
        "MB",
        "GB",
        "TB"
    };
        private static int getbyteprefixindex(long bytevalue)
        {
            int currindex = 0;
            long reduceit = bytevalue;
            while (reduceit > 1024)
            {
                reduceit /= 1024;
                currindex++;

            }
            return currindex;


        }
        public static String FormatSize(long amount)
        {
            int gotindex = getbyteprefixindex(amount);
            double calcamount = amount;
            for (int i = 0; i < gotindex; i++)
            {
                calcamount /= 1024;

            }
            return calcamount.ToString("F2", CultureInfo.InvariantCulture) + " " + stdbyteprefixes[gotindex];

        }
        
        
        //called immediately before a item is added for a dir or file.
        public delegate bool BeforeAddItemCallback(FileSystemInfo itemadd);
        public delegate void AfterAddItemCallback(ToolStripMenuItem addeditem);
        public delegate void ItemClicked(ToolStripMenuItem clickeditem,FileSystemInfo fsinfo);
        public static void PopulateDropDownWithDir(ToolStripDropDown tsdropdown,DirectoryInfo dirInfo,bool ExpandFolders,
            BeforeAddItemCallback beforeaddcallback,
            AfterAddItemCallback afteraddcallback,
            ItemClicked clickedcallback)
        {
            if (beforeaddcallback == null) beforeaddcallback = (a) => true;
            if (afteraddcallback == null) afteraddcallback = (a) => { };
            if (clickedcallback == null) clickedcallback = (citem, fs) => { MessageBox.Show(fs.FullName); };
           
                foreach (Win32Find.WIN32_FIND_DATA founddata in
                    from p in Win32Find.EnumerateFindData(dirInfo.FullName)
                    where !(new String[] { "..", "." }).Contains(p.cFileName.Trim())
                    select p)
                {



                    //add a new item.
                    if (((FileAttributes)founddata.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                        
                    {
                        //is a folder.
                        DirectoryInfo subfolder = new DirectoryInfo(Path.Combine(dirInfo.FullName, founddata.cFileName));
                        {
                            if (beforeaddcallback(subfolder))
                            {
                                Image folderimage = Win32Find.GetIcon(subfolder.FullName, true).ToBitmap();
                                ToolStripMenuItem createitem = new ToolStripMenuItem(subfolder.Name, folderimage);
                                tsdropdown.Items.Add(createitem);
                                createitem.Tag = new Object[] { beforeaddcallback, afteraddcallback, clickedcallback, subfolder };
                                //use a DropDownOpening handler if we are supposed to expand the folder menu.
                                //otherwise, use standard click event.
                                if (ExpandFolders)
                                {
                                    createitem.DropDownOpening += createitem_DropDownOpening;
                                    //add a ghost item.
                                    createitem.DropDown.Items.Add("GHOST");
                                }
                                else
                                    createitem.Click += createitem_Click;



                            }


                        }
                    }
                    else
                    {
                        //a file.
                        FileInfo grabfile = new FileInfo(Path.Combine(dirInfo.FullName, founddata.cFileName));

                        if (beforeaddcallback(grabfile))
                        {
                            Image fileimage = Win32Find.GetIcon(grabfile.FullName, true).ToBitmap();
                            ToolStripMenuItem createitem = new ToolStripMenuItem(grabfile.Name, fileimage);
                            tsdropdown.Items.Add(createitem);
                            createitem.Tag = new Object[] { beforeaddcallback, afteraddcallback, clickedcallback, grabfile };

                            createitem.Click += createitem_Click;

                        }



                    }
                }


            



        }

        static void createitem_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Object[] tags = (Object[])(((ToolStripMenuItem)sender).Tag);
            //beforeaddcallback,afteraddcallback,clickedcallback,filesysteminfo item.
            BeforeAddItemCallback beforecallback = (BeforeAddItemCallback)tags[0];
            AfterAddItemCallback aftercallback = (AfterAddItemCallback)tags[1];
            ItemClicked clickedcallback = (ItemClicked)tags[2];
            FileSystemInfo fsi = (FileSystemInfo)tags[3];
            clickedcallback(sender as ToolStripMenuItem, fsi);
        }

        static void createitem_DropDownOpening(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            var source = (ToolStripMenuItem)sender;
            Object[] tags = (Object[])(source.Tag);

            //beforeaddcallback,afteraddcallback,clickedcallback,filesysteminfo item.
            BeforeAddItemCallback beforecallback = (BeforeAddItemCallback)tags[0];
            AfterAddItemCallback aftercallback = (AfterAddItemCallback)tags[1];
            ItemClicked clickedcallback = (ItemClicked)tags[2];
            FileSystemInfo fsi = (FileSystemInfo)tags[3];
            source.DropDown.Items.Clear();
            PopulateDropDownWithDir(source.DropDown, (DirectoryInfo)fsi, true, beforecallback, aftercallback, clickedcallback);

        }

    }


    public static class Win32Find
    {
        private struct SHFILEINFO
        {
            /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
            private const int MAX_PATH = 260;
            /// <summary>Maximal Length of unmanaged Typename</summary>
            private const int MAX_TYPE = 80;
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
            public string szTypeName;
        };
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);


        [Flags]
        enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }
        [DllImport("user32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);

        private static Dictionary<String, Icon> Extensionicons = new Dictionary<string, Icon>(StringComparer.OrdinalIgnoreCase);
        
        public static void PurgeIconCache()
        {
            Extensionicons.Clear();
            

        }
        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon.  If the strPath is invalid or there is no idonc the default icon is returned
        /// </summary>
        /// <param name="strPath">full path to the file</param>
        /// <param name="bSmall">if true, the 16x16 icon is returned otherwise the 32x32</param>
        /// <returns></returns>
        public static Icon GetIcon(string strPath, bool bSmall)
        {
           
            String gotext = Path.GetExtension(strPath);
            if (!Extensionicons.ContainsKey(gotext))
            {


                SHFILEINFO info = new SHFILEINFO(true);
                int cbFileInfo = Marshal.SizeOf(info);
                SHGFI flags;
                if (bSmall)
                    flags = SHGFI.Icon | SHGFI.SmallIcon;
                else
                    flags = SHGFI.Icon | SHGFI.LargeIcon ;

                SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, (uint)flags);
                Icon returnthis = Icon.FromHandle(info.hIcon);
                //DestroyIcon(info.hIcon);
                Extensionicons.Add(gotext, returnthis);
            }
            return Extensionicons[gotext];
        }



        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        // The CharSet must match the CharSet of the corresponding PInvoke signature
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
            public ulong FileSize() { return (ulong)nFileSizeLow + (ulong)nFileSizeHigh * 4294967296; }
            public string FileName() { return cFileName.Replace('\0',' ').Trim();}
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        public static IEnumerable<Win32Find.WIN32_FIND_DATA> EnumerateFindData(String directory)
        {
            Win32Find.WIN32_FIND_DATA w32find;
            Trace.WriteLine("enumerating Directory " + directory);
            IntPtr findhandle = Win32Find.FindFirstFile(@"\\?\" + directory + @"\*", out w32find);
            //if no items found, break the enumeration.
            if (findhandle == Win32Find.INVALID_HANDLE_VALUE)
            {
                int lasterror = GetLastError();
                Debug.Print("LastError is " + lasterror.ToString());
                yield break;

            }
            do
            {
                if(!(w32find.cFileName =="." || w32find.cFileName==".."))
                    yield return w32find;

            }
            while (FindNextFile(findhandle, out w32find));
            FindClose(findhandle);


        }






    }
}
