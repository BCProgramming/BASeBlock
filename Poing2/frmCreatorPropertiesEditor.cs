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
    public partial class frmCreatorPropertiesEditor : Form
    {
        private CreatorProperties editthis;
        
        public frmCreatorPropertiesEditor(CreatorProperties createprops)
            : this()
        {
            editthis = createprops;
            //editthis.Author 
            //editthis.Comment 
            //editthis.SavedImages 
            //editthis.SavedSounds 
            //editthis.Version 

        }

        public frmCreatorPropertiesEditor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            Close();
        }
        private void ApplyChanges()
        {
            editthis.Description=txtdescription.Text;
            editthis.isTemplate = chkTemplate.Checked;

        }

        private void frmCreatorPropertiesEditor_Load(object sender, EventArgs e)
        {
            txtdescription.Text = editthis.Description;
            chkTemplate.Checked = editthis.isTemplate;
        }

        private void cmdSounds_Click(object sender, EventArgs e)
        {
            if (editthis.SavedSounds == null) editthis.SavedSounds = new Dictionary<string, CreatorProperties.SoundDataItem>();
            frmSoundDataListEditor SoundEdit = new frmSoundDataListEditor(editthis.SavedSounds);
            SoundEdit.ShowDialog(this);
            //load all sounds.
            foreach (var loopitem in editthis.SavedSounds)
            {
                //BCBlockGameState.Soundman.AddSound(loopitem.Value.
                BCBlockGameState.Soundman.AddSound(loopitem.Value.SoundData, loopitem.Value.Name.ToUpper(), loopitem.Value.FileExtension);


            }
        }

        private void cmdImages_Click(object sender, EventArgs e)
        {
            if (editthis.SavedImages == null) editthis.SavedImages = new Dictionary<string, CreatorProperties.ImageDataItem>();
            frmImageDataListEditor imageedit = new frmImageDataListEditor(editthis.SavedImages);
            imageedit.ShowDialog(this);
            //force all images into the current state.
            foreach (var loopimage in editthis.SavedImages)
            {
                BCBlockGameState.Imageman.AddImage(loopimage.Value.Name.ToUpper(), loopimage.Value.ImageData);

            }

        }

        private void cmdEditScripts_Click(object sender, EventArgs e)
        {
            if (editthis.SavedScripts == null) editthis.SavedScripts = new Dictionary<string, CreatorProperties.ScriptDataItem>();
            frmScriptDataItemListEditor scriptedit = new frmScriptDataItemListEditor(editthis.SavedScripts);
            scriptedit.ShowDialog(this);
            //complete....
            
        }
    }
}
