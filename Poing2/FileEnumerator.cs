using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BASeCamp.BASeBlock
{
    class FileEnumerator
    {

        //dllimports, from P/Invoke.net
        // The CharSet must match the CharSet of the corresponding PInvoke signature
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
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
        }
        [DllImport("kernel32.dll")]
        static extern bool FindClose(IntPtr hfindFile);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);
        public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static String unicodeprefix = "\\\\?\\";
        private static String unicodeuncprefix = "\\\\?\\UNC\\";
        private static String CreateUnicodePath(String path)
        {
            if (path.StartsWith(unicodeprefix))
            {
                return path;

            }
            else if (path.StartsWith("\\"))
            {
                return unicodeuncprefix + path.Substring(2);
                //UNC path needs to be in the form \\?\UNC\server\share but is normally passed as \\server\share
                //assume if it starts with "\\"

            }
            else if (!path.StartsWith(unicodeprefix))
            {
                return "\\\\?\\" + path;

            }
            
            return path;

        }
        private static String Denull(String denullthis)
        {

            return denullthis.Replace("\0", "").Trim(); 

        }

        public static IEnumerable<String> EnumDirectories_String(String pPath, String mask)
        {
            String usepath = CreateUnicodePath(pPath);

            WIN32_FIND_DATA wfind;
            IntPtr findhandle;
            findhandle = FindFirstFile(mask, out wfind);


            if (findhandle != INVALID_HANDLE_VALUE)
            {
                do
                {
                    //return the full path of this directory
                    yield return Path.Combine(pPath, Denull(wfind.cFileName));

                } while (FindNextFile(findhandle, out wfind));

                FindClose(findhandle);

            }






        }
        public static IEnumerable<DirectoryInfo> EnumDirectories(String pPath, String mask)
        {
            foreach (String loopfolder in EnumDirectories_String(pPath, mask))
            {
                yield return new DirectoryInfo(loopfolder);

            }


        }
        

    }
}
