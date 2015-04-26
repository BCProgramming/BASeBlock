using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BASeBlock
{

    
    static class FileExtensions
    {
        static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);


        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo dirinfo, String sFileMask, Func<FileInfo, bool> AdditionalFilter)
        {


            foreach (FileSystemInfo fsi in EnumerateDirectory(dirinfo.FullName, sFileMask, (finf) =>finf is FileInfo))
            {
                bool doyield = true;
                if (AdditionalFilter != null)
                    doyield = AdditionalFilter((FileInfo)fsi);

                if (doyield) yield return (FileInfo)fsi;

                
            }



        }

        public static IEnumerable<FileSystemInfo> EnumerateDirectory(String sPath,String sFileMask,Func<FileSystemInfo,bool> AdditionalFilter)
        {
            
            //init to Zero...
            IntPtr findhandle = IntPtr.Zero;
            Win32Find.WIN32_FIND_DATA fdata = new Win32Find.WIN32_FIND_DATA();
            try
            {

                //start a search.
                
                
                
                findhandle = Win32Find.FindFirstFile(Path.Combine(sPath, sFileMask), out fdata);
                if (findhandle != INVALID_HANDLE_VALUE)
                {
                    //get the full path of the acquired name.
                    String sfullpath = Path.Combine(sPath, fdata.cFileName.Trim());
                    bool doyield = false;

                    FileSystemInfo dotest = ((((FileAttributes)fdata.dwFileAttributes) & FileAttributes.Directory) 
                        == FileAttributes.Directory) ?
                        (FileSystemInfo)(new DirectoryInfo(sfullpath)) : 
                        (FileSystemInfo)(new FileInfo(sfullpath));

                    if (AdditionalFilter != null)
                    {
                        doyield = AdditionalFilter(dotest);

                    }
                    else
                    {
                        doyield = !(new String[] { ".", ".." }.Contains(fdata.cFileName.Trim()));
                    }

                    if (doyield) yield return dotest;

                }
                else { yield break; }
                fdata = new Win32Find.WIN32_FIND_DATA();
                Win32Find.FindNextFile(findhandle, out fdata);


            }
            finally
            {
                if (findhandle != IntPtr.Zero) Win32Find.FindClose(findhandle);

            }


            

        }
        


    }
}
