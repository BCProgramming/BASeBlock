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
    public partial class frmLevelBrowser : Form
    {
        private class LevelViewInfo
        {
            public enum LevelViewKind
            {
                [Description("Image Preview")]
                LevelView_ImagePreview,
                [Description("Statistics")]
                LevelView_Statistics

            }
            public static readonly LevelViewInfo ImagePreview = new LevelViewInfo(LevelViewKind.LevelView_ImagePreview);
            public static readonly LevelViewInfo Statistics = new LevelViewInfo(LevelViewKind.LevelView_Statistics);

            private LevelViewKind _ViewKind = LevelViewKind.LevelView_ImagePreview;
            public LevelViewKind ViewKind { get { return _ViewKind; } set { _ViewKind = value; } }
            public LevelViewInfo(LevelViewKind pViewKind)
            {
                _ViewKind = pViewKind;

            }
            public override string ToString()
            {
                return _ViewKind.ToString();
            }

        }
        /// <summary>
        /// Holds LevelSetFileData until necessary.
        /// </summary>
        private class LevelSetFileData
        {
            public String SetFileName;
            private EditorSet _LSetObject = null;

            public LevelSetFileData(String pFilename)
            {
                SetFileName = pFilename;

            }
            public EditorSet LevelSetObj
            {
                get
                {
                    if (_LSetObject == null)
                        _LSetObject = EditorSet.FromFile(SetFileName);

                    return _LSetObject;
                }
            }
        }

        public frmLevelBrowser()
        {
            InitializeComponent();
        }
        List<LevelSetFileData> LevelFileData = new List<LevelSetFileData>();
        private void frmLevelBrowser_Load(object sender, EventArgs e)
        {
            //load:
            
            //what do we do? Look for all BLF files in all the folders specified in BCBlockGameState.LevelFolders, and create a LevelSetFileData
            //and add it to the treeview.

            cboViewStyle.Items.AddRange(new object[] { LevelViewInfo.ImagePreview,LevelViewInfo.Statistics});
            //step one: enumerate all BLF files.
            List<FileInfo> BLFFiles = new List<FileInfo>();

            foreach (String looppath in BCBlockGameState.LevelFolders)
            {
                DirectoryInfo thisdir = new DirectoryInfo(looppath);


                BLFFiles.AddRange(thisdir.GetFiles("*.blf"));
             


            }

            foreach (FileInfo BLFFile in BLFFiles)
            {
                LevelSetFileData lsfd = new LevelSetFileData(BLFFile.FullName);
                LevelFileData.Add(lsfd);
                //possible change: a image to show for levelsets.
                TreeNode newnode = tvwLevelSets.Nodes.Add(BLFFile.FullName, Path.GetFileNameWithoutExtension(BLFFile.FullName));
                //add a ghost child...
                newnode.Tag = lsfd;
                newnode.Nodes.Add("GHOST");

            }





        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tvwLevelSets_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandnode = e.Node;
            bool isghost=true;

            foreach (TreeNode loopnode in expandnode.Nodes)
            {
                if (loopnode.Text != "GHOST")
                {
                    isghost=false;
                    break;
                    
                }
            }
            if (isghost)
            {

                //clear all child nodes.
                expandnode.Nodes.Clear();
                
                //and add one for each level in that levelset.
                //the levelsetfiledata was stored in the tag,
                //and will load  the file on-demand the first time the editorset data is accessed.
                
                LevelSetFileData lsfd = (LevelSetFileData)expandnode.Tag;
                EditorSet theeditorset = lsfd.LevelSetObj;

                //iterate through each Level.
                foreach (Level looplevel in theeditorset.LevelData.Levels)
                {

                    TreeNode levelnode = expandnode.Nodes.Add(looplevel.MD5Hash(), looplevel.LevelName);
                    levelnode.Tag = looplevel;


                }


            }

        }
        Dictionary<Level, Image> LevelImageDictionary = new Dictionary<Level, Image>();
         Image DrawImage=null;
        private void tvwLevelSets_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode clickednode = e.Node;

            if (clickednode.Tag is Level)
            {

                Level gotlevel = (Level)clickednode.Tag;

                if(LevelImageDictionary.ContainsKey(gotlevel))
                {
                    //we already have the image, so just plonk it down.
                    DrawImage = LevelImageDictionary[gotlevel];
                    PicLevel.Invalidate();
                    PicLevel.Update();
                    return;

                }
                //after this, we know it doesn't contain the key.

                if (!String.IsNullOrEmpty(gotlevel.PreviewImageKey))
                {
                    DrawImage = BCBlockGameState.Imageman.getLoadedImage(gotlevel.PreviewImageKey);
                    //add the key to the dictionary...
                    LevelImageDictionary.Add(gotlevel, DrawImage);
                    PicLevel.Invalidate();
                    PicLevel.Update();
                }
                else
                {
                    if (clickednode.Parent.Tag is LevelSetFileData)
                    {
                        //if the image key is null- add it, and set the value, then call this routine again.
                        Image gotimage = BCBlockGameState.DrawLevelToImage(gotlevel);
                        LevelImageDictionary.Add(gotlevel, gotimage);
                        DrawImage = gotimage;
                        PicLevel.Invalidate();
                        PicLevel.Update();
                    }




                   
                   


                }


            }

        }

        private void PicLevel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (DrawImage == null)
            {
                StringFormat centerformat = new StringFormat();
                centerformat.Alignment = StringAlignment.Center;
                g.DrawString("Image Not Available.", new Font("Arial", 48), new SolidBrush(Color.Red), new RectangleF(0, 0, PicLevel.Width, PicLevel.Height), centerformat);
                

            }
            else
            {
                
                g.DrawImage(DrawImage, 0, 0, PicLevel.ClientSize.Width, PicLevel.ClientSize.Height);
            }
        }

        private void tvwLevelSets_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }
    }
}
