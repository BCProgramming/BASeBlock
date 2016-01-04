using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeCamp.BASeBlock.HighScores;
using BASeCamp.Updating;
namespace BASeCamp.BASeBlock
{
    public partial class frmhighscores : Form
    {

        private class HighScoreComboData
        {
            public String Name;
            public KeyValuePair<String, LocalHighScores> Data;

            public HighScoreComboData(String pName, KeyValuePair<String, LocalHighScores> pData)
            {
                Name = pName;
                Data = pData;


                

            }

            public override string ToString()
            {
                return Name + "(" + Data.Value.Scores.Count.ToString() + " scores)";
            }


        }


        private IHighScoreList mHighScoreList=null;
        private String mproduct = "";
        GenericListViewSorter glvw;
        public frmhighscores(IHighScoreList scorelist,String ProductName)
        {
            if (scorelist == null) throw new ArgumentNullException("scorelist",@"""scorelist"" passed into frmhighscores constructor cannot be null.");
            mHighScoreList=scorelist;
            mproduct = ProductName;
            InitializeComponent();
            
        }
        public frmhighscores(String ProductName)
        {
            mproduct = ProductName;
            InitializeComponent();


        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void PopulateCombo()
        {
            //populate cboHighScoreSets with the names of all high score sets.




        }

        private void RefreshList()
        {
            //refresh the listview with the scores from the highscore list.
            
            mHighScoreList.Reload();
            lvwHighScores.Items.Clear();
            lvwHighScores.Columns.Clear();
            lvwHighScores.View = View.Details;

            //add columns...
            lvwHighScores.Columns.Add("NAME", "Name");
            lvwHighScores.Columns.Add("SCORE", "SCORE");
            lvwHighScores.Columns.Add("DATE", "Date");
            //and now, add the items...

            foreach (HighScoreEntry loopentry in mHighScoreList.GetScores())
            {
                ListViewItem itemadd = new ListViewItem(new string[] { loopentry.Name, loopentry.Score.ToString(), loopentry.DateSet.ToString() });
                //set the item's tag to the highscore entry...
                itemadd.Tag=loopentry;
                lvwHighScores.Items.Add(itemadd);



            }




        }
        private void frmhighscores_Load(object sender, EventArgs e)
        {
            this.Size = new Size(cmdClose.Right + 5,cmdClose.Bottom+5);
            if (mproduct != "")

                Text = "High Scores:" + mproduct;
            else
                Text = "High Scores";


            glvw = new GenericListViewSorter(lvwHighScores);
            cboHighScoreSets.Items.Clear();
            object itemsel=null; 
            //get all the score lists...
            foreach (var loopentry in BCBlockGameState.Scoreman.GetScores())
            {
                //cboHighScoreSets.Items.Add(
                //create a new item in the combo box for each.
                var additem = new HighScoreComboData(loopentry.Key, loopentry);
                cboHighScoreSets.Items.Add(additem);

                //if this is the mHighScoreList we were constructed with, select it.
                if (loopentry.Value == mHighScoreList)
                {
                    itemsel = additem;

                }

            }
            cboHighScoreSets.SelectedItem = itemsel;
            
            if (mHighScoreList != null)
            {
                RefreshList();


            }
        }
      
        private void frmhighscores_Resize(object sender, EventArgs e)
        {
        
            cmdClose.Location = new Point(ClientSize.Width-cmdClose.Width-5,ClientSize.Height-cmdClose.Height-5);
            lvwHighScores.Location = new Point(0,0);
            lvwHighScores.Size =new Size(ClientSize.Width,cmdClose.Top-5);
            cboHighScoreSets.Top = ClientSize.Height - cboHighScoreSets.Height - 5;
        }

        private void cboHighScoreSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            mHighScoreList = (cboHighScoreSets.SelectedItem as HighScoreComboData).Data.Value;
            RefreshList();
        }
    }
}
