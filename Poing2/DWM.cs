using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BASeCamp.BASeBlock
{
    static class DWM
    {

        //dwmapi.dll
        //HRESULT DwmIsCompositionEnabled(          BOOL *pfEnabled
//);
        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool fEnabled);



        public static bool IsDWMEnabled()
        {
            try
            {
                bool returnvalue = false;
                DwmIsCompositionEnabled(out returnvalue);
                return returnvalue;

            }
            catch
            {
                return false;

            }

        }

    }
}
