using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BASeCamp.BASeBlock
{
    public partial class ImageImport : Form
    {
        public struct ImageImportOptions
        {
            public Image ImageData;
            public SizeF BlockSize;
            public SizeF PreScale;
            public bool UseClipBlock;
            public Size ClipSize;
            public int SessionID;

        }
        //callback function is used so that the form that creates this one can be "updated" to show applied changes, without closing the form.
        public delegate void ImageImportCallback(ImageImportOptions currentoptions);
        private ImageImportCallback callbackfunc;
        private SizeF EditorSize;
        private Image currentImage=null;
        public int SessionID; //each time the Form is shown, the session ID will be different.

        //this can be used by the import callback to determine whether this is a "new" import operation or not.
        //specifically, the editor form in BASeBlock uses this to determine whether it can delete any previously created image from the CreatorProperties
        //data of the level.
        public static ImageImportOptions DoImport(IWin32Window uparent,SizeF editsize,ImageImportCallback importcallback)
        {
            ImageImport useform = new ImageImport(editsize, importcallback);
            useform.ShowDialog(uparent);
            return useform.GetCurrentOptions();
            
        }

        public ImageImport(SizeF Editsize, ImageImportCallback importcallback)
            : this()
        {
            callbackfunc = importcallback;
            EditorSize=Editsize;
            

        }

        private ImageImport()
        {
            SessionID = BCBlockGameState.rgen.Next(int.MinValue, int.MaxValue);
            InitializeComponent();
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd= new OpenFileDialog();
            ofd.Filter = BCBlockGameState.Imageman.GetFilter();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //populate the textbox. The textbox should validate which will populate the other properties as well.
                txtFilename.Text=ofd.FileName;


                validatefile();

            }
        }
        /// <summary>
        /// changes the width and height to a "best fit" value, which is the largest square that would allow all the pixels of the image to be shown.
        /// </summary>
        private void BestFit()
        {
            if(currentImage==null) return;
            SizeF PixelSize = new SizeF(EditorSize.Width / currentImage.Width, EditorSize.Height / currentImage.Height);
            int minuse = (int)Math.Min(PixelSize.Width, PixelSize.Height);
            
            UDWidth.Text = minuse.ToString();
            UDHeight.Text = minuse.ToString();


        }
        private void validatefile()
        {
            String usefilename = txtFilename.Text;
            if (File.Exists(usefilename))
            {
                try
                {
                    FileStream readstream = new FileStream(usefilename, FileMode.Open);
                    Image readImage = Image.FromStream(readstream);
                    currentImage = readImage;
                    readstream.Close();

                    BestFit();


                }
                catch (IOException IOErr)
                {
                    MessageBox.Show("Error accessing \"" + usefilename + IOErr.Message + "\"");


                }
            }


        }

        private void txtFilename_Validating(object sender, CancelEventArgs e)
        {
         
        }
        private ImageImportOptions GetCurrentOptions()
        {
            return new ImageImportOptions() {BlockSize=new SizeF(Single.Parse(UDWidth.Text),Single.Parse(UDHeight.Text)),
                ImageData=currentImage,
                PreScale=new SizeF(Single.Parse(udcXPrescale.Text),Single.Parse(UDCYPreScale.Text)),
            ClipSize=new Size(Int32.Parse(UDCClipX.Text),Int32.Parse(UDCClipY.Text)),
            UseClipBlock=chkImageClip.Checked,
            SessionID=this.SessionID};


        }

        private void DoApply()
        {
            callbackfunc(GetCurrentOptions());


        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            DoApply();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkImageClip_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control setenabled in new Control[] { UDCClipX, UDCClipY, fraClipBlock })
            {
                setenabled.Enabled = chkImageClip.Checked;


            }
        }

        private void txtFilename_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
