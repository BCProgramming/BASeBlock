using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BASeBlock
{
    //newerr implementation: attempts to use BASS directly, rather then through the BASS.NET API.

    

    



    public class BASSDirect
    {
        //declarations:
        [DllImport("bass.dll", EntryPoint = "BASS_MusicLoad")]
        static extern IntPtr BASS_LoadMusic(int mem, IntPtr file, int offset, int length, int flags);//OK, bool mem, return point to HMUSIC

        [DllImport("bass.dll", EntryPoint = "BASS_StreamCreateFile")]
        static extern IntPtr _CreateStreamFile(int mem, IntPtr file, int offset,
            int length, int flags);//OK, mem is bool




    }
}
