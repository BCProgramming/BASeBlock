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
    /// <summary>
    /// FolderListView: given a set of arrays, displays those various arrays in lists and allows the user to remove and add new folders to each.
    /// </summary>
    public partial class FolderListView : Form
    {


       



        private String[] mnameLabels;
        public DirectoryInfo[][] currFolderLists;
        private int mCurrentSelIndex=-1;
        public void DoShow()
        {
            ShowDialog();


        }
        
        public static void EditValues(ref String[] NameLabels,ref DirectoryInfo[][] dirinfos)
        {
            FolderListView Formcreate = new FolderListView(NameLabels, dirinfos);
            switch (Formcreate.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.Cancel:
                    break;
                default:

                    break;


            }



        }

        public FolderListView(String[] NameLabels,DirectoryInfo[][] dirinfos)
        {
            if (NameLabels.Length != dirinfos.Length)
            {
                throw new ArgumentException("FolderListView NameLabels array must have same length as dirinfos array");


            }
            //accepts a array of directoryinfos, used during load.
            currFolderLists = dirinfos;
            mnameLabels=NameLabels;
            InitializeComponent();
        }

        private void FolderListView_Load(object sender, EventArgs e)
        {
            //first, add each name label to the combobox...
            cboFolderTypes.Items.Clear();
            foreach (String labeladd in mnameLabels)
            {
                cboFolderTypes.Items.Add(labeladd);

            }

        }
        private DirectoryInfo[] ListBoxToDirList(ListBox usebox)
        {
            List<DirectoryInfo> buildlist = new List<DirectoryInfo>();

            String sitem;

            foreach (Object item in usebox.Items)
            {
                sitem = (String)item;
                if (Directory.Exists(sitem))
                {
                    buildlist.Add(new DirectoryInfo(sitem));
                }


            }


            return buildlist.ToArray();

        }

        private void cboFolderTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //when the selection changes:
            //"save" the current state of the list shown
            if (mCurrentSelIndex > 0) //will be zero on the first change.
            {
                //save the "state" of the list...
                currFolderLists[mCurrentSelIndex] = ListBoxToDirList(lstFolders);


            }
            //"select" the new list...
            grpFolders.Text = "Folders:" + mnameLabels[cboFolderTypes.SelectedIndex];
            //clear the list...
            lstFolders.Items.Clear();
            //add each directory...
            foreach (DirectoryInfo loopinfo in currFolderLists[cboFolderTypes.SelectedIndex])
            {
                lstFolders.Items.Add(loopinfo.FullName);



            }
            cmdRemove.Enabled=false;



            mCurrentSelIndex = cboFolderTypes.SelectedIndex;
        }

        private void lstFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdRemove.Enabled=true;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Close();
        }
        
    }
}
