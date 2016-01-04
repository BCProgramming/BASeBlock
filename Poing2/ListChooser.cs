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
    public partial class frmChooser<T> : Form
    {
        private List<T> ItemsChoose;
        private String mCaption,mDescription;
        public T SelectedItem;
        public bool SaveCheckbox{get { return chkSaveSelection.Checked; }set { chkSaveSelection.Checked = value; }}
            public static T DoChoose(List<T> ChooseItems, String pCaption, String pDescription, out bool SaveValue)
        {
            frmChooser<T> FormCreate = new frmChooser<T>(ChooseItems, pCaption, pDescription);
            FormCreate.ShowDialog();
            SaveValue = FormCreate.SaveCheckbox;
            return FormCreate.SelectedItem;


        }

        public frmChooser(List<T> ChooseItems, String pCaption, String pDescription):this()
        {
            
            ItemsChoose = ChooseItems;
            mCaption = pCaption;
            mDescription = pDescription;



        }

        public frmChooser()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmChooser_Load(object sender, EventArgs e)
        {
            lvwChooseItems.View = View.Details;
            lvwChooseItems.Columns.Clear();
            lvwChooseItems.Columns.Add("NAME", "Name", lvwChooseItems.ClientSize.Width);
            Text = mCaption;
            lbldescription.Text = mDescription;
            foreach (T loopitem in ItemsChoose)
            {
                ListViewItem addeditem = new ListViewItem(new string[] { loopitem.ToString() });
                addeditem.Tag=loopitem;
                lvwChooseItems.Items.Add(addeditem);
            }
        }

        private void lvwChooseItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
        }

        private void lvwChooseItems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            SelectedItem = (T)e.Item.Tag;
            cmdOK.Enabled = true;

        }

        private void lvwChooseItems_DoubleClick(object sender, EventArgs e)
        {

        }

        private void lvwChooseItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView chooseview = (ListView)lvwChooseItems;
            //use hittest to detect selected item...
            ListViewHitTestInfo htinfo = chooseview.HitTest(e.X, e.Y);
            if (htinfo == null) return;
            ListViewItem useitem = htinfo.Item;
            SelectedItem = (T)useitem.Tag;
            cmdOK_Click(cmdOK, null);
            


        }
    }
}
