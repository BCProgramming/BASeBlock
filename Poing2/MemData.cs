using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BASeCamp.BASeBlock
{
    public struct MemData
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        [DllImport("kernel32.dll")]
        private static extern int GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
        public static MEMORYSTATUSEX getStatus()
        {
            MEMORYSTATUSEX ex = new MEMORYSTATUSEX();
            GlobalMemoryStatusEx(ref ex);
            return ex;
        }

    }
}
