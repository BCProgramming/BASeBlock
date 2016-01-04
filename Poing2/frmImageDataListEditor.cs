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
    public partial class frmImageDataListEditor : Form
    {
        private Dictionary<String, CreatorProperties.ImageDataItem> ImageDataList=null;
        private ListViewItem CurrentlySelected=null;
        public frmImageDataListEditor(Dictionary<String, CreatorProperties.ImageDataItem> pinitlist) : this()
        {
            ImageDataList=pinitlist;

        }

        public frmImageDataListEditor()
        {
            InitializeComponent();
        }

        private void frmImageDataListEditor_Load(object sender, EventArgs e)
        {
            RefreshView();
        }
        private void SelectItem(ListViewItem itemselect)
        {
            if (CurrentlySelected != itemselect)
            {
                ApplyItem(); ///apply any currently selected item changes.
                ///
            }
            var casted = (KeyValuePair<String, CreatorProperties.ImageDataItem>)itemselect.Tag;
            txtProp_Key.Text = casted.Key;
            chkHidden.Checked = casted.Value.Flag == CreatorProperties.ImageDataItem.ImageDataItemFlagConstants.Hidden;
            if (casted.Value.ImageData != null)
            {
                picImagePreview.Image = casted.Value.ImageData;
            }
            txtProp_Key.Enabled = true;
           
           
            cmdBrowse.Enabled=true;

        }

        private void ApplyItem()
        {
            if (CurrentlySelected != null && txtProp_Key.Enabled==true)
            {
                var casted = (KeyValuePair<String, CreatorProperties.ImageDataItem>)CurrentlySelected.Tag;
                casted.Value.Name = txtProp_Key.Text;
                casted.Value.Flag=chkHidden.Checked?CreatorProperties.ImageDataItem.ImageDataItemFlagConstants.Hidden:CreatorProperties.ImageDataItem.ImageDataItemFlagConstants.Normal;
                
                

                lvwImageEntries.Items.Remove(CurrentlySelected);
                CurrentlySelected = new ListViewItem(new string[] { casted.Value.Name, casted.Value.ImageData.Size.ToString()});
                lvwImageEntries.Items.Add(CurrentlySelected);

                var replacekvp = (from n in ImageDataList where n.Value == casted.Value select n).First();
                //update the kvp by replacing it
                ImageDataList.Remove(replacekvp.Key);
                ImageDataList.Add(casted.Value.Name, casted.Value);
                casted = (from n in ImageDataList where n.Key == casted.Value.Name select n).First();

                CurrentlySelected.Tag = casted;




            }


        }



        private void RefreshView()
        {
            lvwImageEntries.SuspendLayout();
            lvwImageEntries.Items.Clear();
            lvwImageEntries.Columns.Clear();
            //first, add columns; Key, Type, and Length
            lvwImageEntries.Columns.Add("KEY", "Key");
            lvwImageEntries.Columns.Add("SIZE", "Size");
            



            //iterate through all elements of EditList, and add the items to the listview.
            foreach (KeyValuePair<String, CreatorProperties.ImageDataItem> kvp in ImageDataList)
            {
                if (tstripshowhidden.Checked || !(kvp.Value.Flag == CreatorProperties.ImageDataItem.ImageDataItemFlagConstants.Hidden))
                {
                    //add a new item for each; the keyvaluepair will be used as the tag 
                    ListViewItem newitem = new ListViewItem(new string[] {kvp.Key, kvp.Value.ImageData.Size.ToString()});
                    lvwImageEntries.Items.Add(newitem);
                    newitem.Tag = kvp;
                }


            }
            //don't select any of them...



            lvwImageEntries.ResumeLayout();
            lvwImageEntries.Update();
        }

        private void lvwImageEntries_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {

                SelectItem(e.Item);
                CurrentlySelected = e.Item;




            }
            else
            {
                CurrentlySelected = null;
                txtProp_Key.Enabled = false;
               
               
                txtProp_Key.Text = "";
               
               
               
            }
        }

        private void toolstripRemove_Click(object sender, EventArgs e)
        {
            if (lvwImageEntries.SelectedItems.Count > 0)
            {
                var casteditem = ((KeyValuePair<String, CreatorProperties.ImageDataItem>)CurrentlySelected.Tag);
                //remove from listview, and the "connected" list.
                ImageDataList.Remove(casteditem.Value.Name);
                lvwImageEntries.Items.Remove(CurrentlySelected);





            }
        }
        
        private void ToolStripAddItem_Click(object sender, EventArgs e)
        {
            AddImageItem();
        }
        private ListViewItem AddImageItem()
        {
            CreatorProperties.ImageDataItem newitem;
            newitem = new CreatorProperties.ImageDataItem("Image" + BCBlockGameState.rgen.Next(0, 32768), new Bitmap(1,1));


            ListViewItem newlitem = new ListViewItem(new string[] { newitem.Name, newitem.ImageData.Size.ToString() });
            ImageDataList.Add(newitem.Name, newitem);
            newlitem.Tag = (from h in ImageDataList where h.Key == newitem.Name select h).First();

            lvwImageEntries.Items.Add(newlitem);
            newlitem.Selected = true;
            return newlitem;
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            String imagefilename = "";
            if (CurrentlySelected == null) AddImageItem();

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "All Supported Formats(*.bmp;*.png;*.jpg;*.jpeg;*.jpe;*.gif)|*.bmp;*.png;*.jpg;*.gif|" +
                "Windows or OS/2 Bitmap(*.bmp)|*.bmp|" +
                "Portable Network Graphics(*.png)|*.png|" +
                "JPEG(*.jpg;*.jpeg;*.jpe)|*.jpg;*.jpeg;*.jpe|" +
                "GIF(*.gif)|*.gif|" +
                "all files(*.*)|*.*";
            ofd.DefaultExt = "*.png";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                imagefilename = ofd.FileName;
                Stream readimage = new FileStream(imagefilename, FileMode.Open);
                Image loadimage = Image.FromStream(readimage);
                readimage.Close();
                ((KeyValuePair<String,CreatorProperties.ImageDataItem>)CurrentlySelected.Tag ).Value.ImageData=loadimage;
                picImagePreview.Image = loadimage;
                ApplyItem();





            }

        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            ApplyItem();
            Close();
        }

        private void txtProp_Key_Validated(object sender, EventArgs e)
        {
            ApplyItem();
        }

        private void tstripshowhidden_Click(object sender, EventArgs e)
        {
            tstripshowhidden.Checked=!tstripshowhidden.Checked;
            RefreshView();
        }
    }
}
