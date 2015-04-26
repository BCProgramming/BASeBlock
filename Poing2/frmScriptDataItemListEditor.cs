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
    public partial class frmScriptDataItemListEditor : Form
    {
        private ListViewItem CurrentlySelected =null;
        private Dictionary<String, CreatorProperties.ScriptDataItem> ScriptDataList=null;
        public frmScriptDataItemListEditor()
        {
            InitializeComponent();
        }
        public frmScriptDataItemListEditor(Dictionary<String, CreatorProperties.ScriptDataItem> pinitlist)
            : this()
        {
            ScriptDataList = pinitlist;

        }

        private void ToolStripAddItem_Click(object sender, EventArgs e)
        {
            AddScriptItem();
        }
        private void RefreshView()
        {
            lvwScripts.Items.Clear();
            lvwScripts.Columns.Clear();

            lvwScripts.Columns.Add("NAME", "Name");
            lvwScripts.Columns.Add("LANGUAGE", "Language");


            foreach (var loopvalue in ScriptDataList)
            {

                ListViewItem createitem = new ListViewItem(new string[] { loopvalue.Value.Name, loopvalue.Value.Language });
                createitem.Tag = loopvalue;
                lvwScripts.Items.Add(createitem);



            }




        }
        private void ApplyItem()
        {
            if (CurrentlySelected != null && txtName.Enabled == true)
            {
                var casted = (KeyValuePair<String, CreatorProperties.ScriptDataItem>)CurrentlySelected.Tag;
                casted.Value.Name = txtName.Text;
                



                lvwScripts.Items.Remove(CurrentlySelected);
                CurrentlySelected = new ListViewItem(new string[] { casted.Value.Name, casted.Value.Language });
                lvwScripts.Items.Add(CurrentlySelected);

                var replacekvp = (from n in ScriptDataList where n.Value == casted.Value select n).First();
                //update the kvp by replacing it
                ScriptDataList.Remove(replacekvp.Key);
                ScriptDataList.Add(casted.Value.Name, casted.Value);
                casted = (from n in ScriptDataList where n.Key == casted.Value.Name select n).First();

                CurrentlySelected.Tag = casted;




            }


        }
        private ListViewItem AddScriptItem()
        {
            CreatorProperties.ScriptDataItem newitem;
            newitem = new CreatorProperties.ScriptDataItem("Script" + BCBlockGameState.rgen.Next(0, 32768), "Python","#insert code here");


            ListViewItem newlitem = new ListViewItem(new string[] { newitem.Name, newitem.Language });
            ScriptDataList.Add(newitem.Name, newitem);
            newlitem.Tag = (from h in ScriptDataList where h.Key == newitem.Name select h).First();

            lvwScripts.Items.Add(newlitem);
            newlitem.Selected = true;
            return newlitem;
        }

        private void toolstripRemove_Click(object sender, EventArgs e)
        {
            if (lvwScripts.SelectedItems.Count > 0)
            {
                var casteditem = ((KeyValuePair<String, CreatorProperties.ImageDataItem>)CurrentlySelected.Tag);
                //remove from listview, and the "connected" list.
                ScriptDataList.Remove(casteditem.Value.Name);
                lvwScripts.Items.Remove(CurrentlySelected);





            }
        }
        private void SelectItem(ListViewItem itemselect)
        {
            if (CurrentlySelected != itemselect)
            {
                ApplyItem(); ///apply any currently selected item changes.
                ///
            }
            var casted = (KeyValuePair<String, CreatorProperties.ScriptDataItem>)itemselect.Tag;
            txtName.Text = casted.Value.Name;
            txtLanguage.Text = casted.Value.Language;
          
            txtName.Enabled = true;


            txtLanguage.Enabled = true;

        }
        private void lvwScripts_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {

                SelectItem(e.Item);
                CurrentlySelected = e.Item;




            }
            else
            {
                CurrentlySelected = null;
                txtName.Enabled = false;
                txtLanguage.Text = "";
                txtName.Text = "";

                txtLanguage.Enabled=false;



            }
        }

        private void frmScriptDataItemListEditor_Load(object sender, EventArgs e)
        {
            RefreshView();
            if(lvwScripts.Items.Count > 0)
                lvwScripts.Items[0].Selected=true;
        }

        private void cmdEditCode_Click(object sender, EventArgs e)
        {
            if(CurrentlySelected !=null)
            {
                var kvp = (KeyValuePair<String,CreatorProperties.ScriptDataItem>)CurrentlySelected.Tag;



                SimpleTextEdit ste = new SimpleTextEdit(kvp.Value.Code);
                if (ste.ShowDialog(this) == DialogResult.OK)
                {

                    kvp.Value.Code = ste.EditText;
                    
                }
            }
        }
    }
}
