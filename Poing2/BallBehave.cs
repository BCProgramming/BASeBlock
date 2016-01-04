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
    public partial class BallBehave : Form
    {
        private readonly cBall _ballEdit;
        public BallBehave(cBall ballBehavioursEdit)
        {
            _ballEdit = ballBehavioursEdit;
            InitializeComponent();
        }

        private void BallBehave_Load(object sender, EventArgs e)
        {
            
            //on form load, initialize both the available behaviours (found in the manager class in BCBlockGameState) as well
            //as loaded behaviours (corresponding to the behaviours currently present in ballEdit).
            cboAvailableBehaviours.Items.Clear();
            
            foreach (Type looptype in BCBlockGameState.BallBehaviourManager.ManagedTypes)
            {
                cboAvailableBehaviours.Items.Add(looptype);



            }
            RefreshApplied();
        }

        private void RefreshApplied()
        {
            lvwApplied.Items.Clear();
            foreach (iBallBehaviour loopbehaviour in _ballEdit.Behaviours)
            {
                //lvwApplied.Items.Add(loopbehaviour);
                ListViewItem newitem = new ListViewItem(loopbehaviour.GetType().Name);
                newitem.Tag = loopbehaviour;
                lvwApplied.Items.Add(newitem);



            }
        }

        private void lvwApplied_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private ListViewItem _lastselitem;
        private void lvwApplied_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                PropertyApplied.SelectedObject = e.Item.Tag;

                cmdRemove.Enabled=true;
                _lastselitem = e.Item;

            }
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            //add a new instance of the selected item in the combo to the balls behaviours list...

            Type typeuse = (Type)cboAvailableBehaviours.SelectedItem;



            iBallBehaviour addbehaviour = (iBallBehaviour)Activator.CreateInstance(typeuse);
            _ballEdit.Behaviours.Add(addbehaviour);
            RefreshApplied();



        }

        private void cboAvailableBehaviours_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAvailableBehaviours_SelectionChangeCommitted(object sender, EventArgs e)
        {
            cmdAdd.Enabled=true;
            
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            if (_lastselitem != null)
            {
                lvwApplied.Items.Remove(_lastselitem);
                //remove behaviour from ball
                _ballEdit.Behaviours.Remove((iBallBehaviour)_lastselitem.Tag);



            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
