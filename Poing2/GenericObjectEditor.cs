using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BASeBlock
{
    public partial class GenericObjectEditor : Form
    {
        public GenericObjectEditor()
        {
            InitializeComponent();
        }

        private void GenericObjectEditor_Resize(object sender, EventArgs e)
        {
            //move CmdClose and CmdOK.
            cmdOK.Location = new Point(ClientRectangle.Right - cmdOK.Width - 12,
                ClientRectangle.Top - cmdOK.Height - 12);

            
        }
    }
}
