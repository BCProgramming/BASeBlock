using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BASeCamp.BASeBlock
{
    public partial class SoundPluginSelector : Form
    {
        Type SelectedPlugin=null;
        public static Type ChooseSoundPlugin()
        {
            return ChooseSoundPlugin(null);

        }

        public static Type ChooseSoundPlugin(IWin32Window useparent)
        {

            SoundPluginSelector createform = new SoundPluginSelector();
            return createform.ChoosePlugin(useparent);



        }
        public Type ChoosePlugin()
        {
            return ChoosePlugin(null);

        }

        public Type ChoosePlugin(IWin32Window useparent)
        {
            if (useparent != null)
                this.ShowDialog(useparent);
            else
                this.ShowDialog();
                
            
            return SelectedPlugin;

        }

        public SoundPluginSelector()
        {
            InitializeComponent();
        }

        private void cboSoundPlugins_SelectedValueChanged(object sender, EventArgs e)
        {
            cmdOK.Enabled=true;
            SelectedPlugin = ((iSoundEngineDriver)(cboSoundPlugins.SelectedItem)).GetType();
        }

        private void SoundPluginSelector_Load(object sender, EventArgs e)
        {
            //populate combobox with Sound plugins.
            foreach (Type looptype in BCBlockGameState.SoundDriverManager.ManagedTypes)
            {
                //for each type, instantiate it...
                try
                {
                    iSoundEngineDriver ised = (iSoundEngineDriver)Activator.CreateInstance(looptype);

                    cboSoundPlugins.Items.Add(ised);
                }
                catch (Exception exx)
                {
                    //ignore...

                }



            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
