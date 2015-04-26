using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ProductKeyGenerator
{
    

    public static class TextBoxExtensions
    {
        private const int ECM_FIRST = 0x1500;
        [DllImport("user32.dll", CharSet=CharSet.Unicode, EntryPoint = "SendMessageW")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam,String lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg,out String wParam,ref String lParam);

        //#define EM_SETCUEBANNER     (ECM_FIRST + 1)     // Set the cue banner with the lParm = LPCWSTR
        private const int EM_SETCUEBANNER = (ECM_FIRST + 1);
        private const int EM_GETCUEBANNER = (ECM_FIRST+2);


        public static void SetCueBanner(this TextBox tx, String banner)
        {

            SendMessage(tx.Handle, EM_SETCUEBANNER, 0,banner);


        }
        public static String GetCueBanner(this TextBox tx)
        {
            String value = "";
            String result = new string(Enumerable.Repeat(' ', 32767).ToArray());
            SendMessage(tx.Handle, EM_GETCUEBANNER, out value,ref result);
            
            return result.Replace('\0', ' ').Trim();
        }

    }
}
