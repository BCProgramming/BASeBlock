using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using BASeCamp.Updating;
using Timer = System.Threading.Timer;

namespace BASeBlock
{
    public partial class frmSoundDataListEditor : Form
    {
        ListViewItem CurrentlySelected = null;
        private byte[] LoadedSoundData;
        public Dictionary<String, CreatorProperties.SoundDataItem> EditList { get; set; }
        public frmSoundDataListEditor()
        {
            InitializeComponent();
        }
        public frmSoundDataListEditor(Dictionary<String, CreatorProperties.SoundDataItem> pEditList)
            : this()
        {
            EditList = pEditList;


        }
        private void RefreshView()
        {
            lvwSoundEntries.SuspendLayout();
            lvwSoundEntries.Items.Clear();
            lvwSoundEntries.Columns.Clear();
            //first, add columns; Key, Type, and Length
            lvwSoundEntries.Columns.Add("KEY", "Key");
            lvwSoundEntries.Columns.Add("TYPE", "Type");
            lvwSoundEntries.Columns.Add("LENGTH", "Length");




            //iterate through all elements of EditList, and add the items to the listview.
            foreach (KeyValuePair<String, CreatorProperties.SoundDataItem> kvp in EditList)
            {
                //add a new item for each; the keyvaluepair will be used as the tag 
                ListViewItem newitem = new ListViewItem(new string[] { kvp.Key, kvp.Value.FileExtension, kvp.Value.SoundData.Length.ToString() });
                lvwSoundEntries.Items.Add(newitem);
                newitem.Tag = kvp;



            }
            //don't select any of them...



            lvwSoundEntries.ResumeLayout();
            lvwSoundEntries.Update();
        }

        private void frmSoundDataListEditor_Load(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }
        private void ApplyItem()
        {
            if (ignoreItemChanges) return;
            if (CurrentlySelected != null)
            {
                var casted = (KeyValuePair<String, CreatorProperties.SoundDataItem>)CurrentlySelected.Tag;
                casted.Value.Name = txtProp_Key.Text;

                casted.Value.FileExtension = txtFileType.Text;
                casted.Value.SoundData = LoadedSoundData;

                lvwSoundEntries.Items.Remove(CurrentlySelected);
                var replacekvp = (from n in EditList where n.Value == casted.Value select n).First();
                //update the kvp by replacing it
                
                
                CurrentlySelected = new ListViewItem(new string[] { casted.Value.Name, casted.Value.FileExtension, casted.Value.SoundData.Length.ToString() });
                EditList.Remove(replacekvp.Key);
                lvwSoundEntries.Items.Add(CurrentlySelected);

                
                EditList.Add(casted.Value.Name, casted.Value);
                casted = (from n in EditList where n.Key == casted.Value.Name select n).First();

                CurrentlySelected.Tag = casted;
                CurrentlySelected.Selected = true;
                
                //lvwSoundEntries_ItemSelectionChanged(lvwSoundEntries, new ListViewItemSelectionChangedEventArgs(CurrentlySelected, CurrentlySelected.Index, true));


            }


        }

        private void SelectItem(ListViewItem itemselect)
        {
            if (CurrentlySelected != itemselect)
            {
                ApplyItem(); ///apply any currently selected item changes.
                ///
            }
            var casted = (KeyValuePair<String, CreatorProperties.SoundDataItem>)itemselect.Tag;
            txtProp_Key.Text = casted.Key;
            txtFileType.Text = casted.Value.FileExtension;

            LoadedSoundData = casted.Value.SoundData;
            txtProp_Key.Enabled = true;
            txtFileType.Enabled = true;
            lblDatainfo.Enabled = true;
            lblDatainfo.Text = "Data Length:" + LoadedSoundData.Length.ToString() + " bytes";

        }
        private bool ignoreItemChanges = false; 
        private void lvwSoundEntries_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (ignoreItemChanges) return;
            ListViewItem currentselection = lvwSoundEntries.getSelectedItem();
            if (currentselection!=null)
            {
                if (CurrentlySelected == currentselection) return;
                SelectItem(e.Item);
                CurrentlySelected = e.Item;




            }
            else
            {
                CurrentlySelected = null;
                txtProp_Key.Enabled = false;
                txtFileType.Enabled = false;
                lblDatainfo.Enabled = false;
                txtProp_Key.Text = "";
                txtFileType.Text = "";
                lblDatainfo.Text = "";
                LoadedSoundData = new byte[0];
            }
        }

        private void ToolStripAddItem_Click(object sender, EventArgs e)
        {
            //add a new SoundEntry item...
            AddSoundItem();
            
            
                cmdBrowse_Click(sender, e);
                if(CurrentlySelected!=null) CurrentlySelected.Selected = true;
            
            
            //lvwSoundEntries_ItemSelectionChanged(lvwSoundEntries,new ListViewItemSelectionChangedEventArgs(CurrentlySelected,CurrentlySelected.Index,true));
        }

        private ListViewItem AddSoundItem()
        {
            CreatorProperties.SoundDataItem newitem;
            newitem = new CreatorProperties.SoundDataItem("Sound" + BCBlockGameState.rgen.Next(0, 32768), "NONE", new byte[0]);


            ListViewItem newlitem = new ListViewItem(new string[] { newitem.Name, newitem.FileExtension, "0" });
            EditList.Add(newitem.Name, newitem);
            newlitem.Tag = (from h in EditList where h.Key == newitem.Name select h).First();

            lvwSoundEntries.Items.Add(newlitem);
            newlitem.Selected = true;
            return newlitem;
        }

        private void toolstripRemove_Click(object sender, EventArgs e)
        {
            if (lvwSoundEntries.SelectedItems.Count > 0)
            {
                var casteditem = ((KeyValuePair<String, CreatorProperties.SoundDataItem>)CurrentlySelected.Tag);
                //remove from listview, and the "connected" list.
                EditList.Remove(casteditem.Value.Name);
                lvwSoundEntries.Items.Remove(CurrentlySelected);





            }
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        { 
            const string Soundfilefilters = "All Supported Files(*.wav,*.ogg,*.mp3)|*.wav;*.ogg;*.mp3|" +
                "Wave Audio (*.wav)|*.wav|" +
                "Ogg Vorbis(*.ogg)|*.ogg|" +
                "MPEG Layer 3 (*.MP3)|*.MP3|" +
                "All Files(*.*)|*.*";


                //task: browse for and select a file, and populate the textboxes and LoadedSoundData arrays 
                OpenFileDialog ofd = new OpenFileDialog();
                //filters? hmm. all the standard ones.

                ofd.Filter = Soundfilefilters;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {

                    //check that we have write access to the file.

                    var permission = new FileIOPermission(FileIOPermissionAccess.Read, ofd.FileName);
                    var permissionSet = new PermissionSet(PermissionState.None);
                    permissionSet.AddPermission(permission);
                    if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                    {


                        if (lvwSoundEntries.Items.Count == 0) AddSoundItem();
                        //pressed OK.
                        String usefilename = ofd.FileName;
                        //cheat...
                        try
                        {
                            CreatorProperties.SoundDataItem sdi = new CreatorProperties.SoundDataItem(usefilename);


                            //txtProp_Key.Text = Path.GetFileNameWithoutExtension(usefilename);
                            //txtFileType.Text = Path.GetExtension(usefilename);
                            ignoreItemChanges = true;
                            txtProp_Key.Text = sdi.Name;
                            txtFileType.Text = sdi.FileExtension;
                            LoadedSoundData = sdi.SoundData;
                            lblDatainfo.Text = "Data Length:" + sdi.SoundData.Length.ToString() + " bytes";
                            ignoreItemChanges = false;
                            ApplyItem();
                        }
                        catch (IOException iox)
                        {
                            MessageBox.Show("Error Accessing \"" + usefilename + "\". " + iox.Message);


                        }

                    }

                }
                
                
                


            




        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApplyItem();
            Close();
        }

        //some variables used  for the "preview" feature
        private String mTempFilename;
        private iSoundSourceObject playingsound = null;
        private iActiveSoundObject playedsound = null;
        private iSoundEngineDriver usedriver = null;
        private System.Threading.Timer PositionUpdateTimer = null;
        private void UpdateSoundPos()
        {

            tbarSoundPosition.Minimum = 0;
            tbarSoundPosition.Maximum = 500;
            tbarSoundPosition.Value = (int)(playedsound.Progress * 500);
            tbarSoundPosition.Invalidate();
            tbarSoundPosition.Update();
        }
        private void cmdPlayStop_Click(object sender, EventArgs e)
        {
            String useext = txtFileType.Text;
            //If currently playing; stop. Otherwise, play the sound data in LoadedSoundData.
            if (playingsound == null && LoadedSoundData !=null)
            {
                
                usedriver = BCBlockGameState.Soundman.Driver;
                mTempFilename = BCBlockGameState.GetTempFile(useext);
                //write the sounddata to that file.
                FileStream writesound = new FileStream(mTempFilename, FileMode.Create);
                writesound.Write(LoadedSoundData, 0, LoadedSoundData.Length);
                writesound.Close();
                //load a sound object from the current Driver...
                usedriver.OnSoundStop += new OnSoundStopDelegate(usedriver_OnSoundStop);
                
                playingsound = usedriver.LoadSound(mTempFilename);
                
                playedsound = playingsound.Play(false);

                cmdPlayStop.Text = "&Stop";
                //start the PositionUpdate Timer.
                PositionUpdateTimer = new Timer((w) => BeginInvoke((MethodInvoker)(() => { UpdateSoundPos(); })),null,0,250);
                

            }
            else
            {
                PositionUpdateTimer.Dispose();
                PositionUpdateTimer = null;
                playedsound.Stop();
                // usedriver_OnSoundStop(playedsound);
                cmdPlayStop.Text = "&Play";
                SetStopState();
            }

        }
        private void SetStopState()
        {
            playedsound.Stop();
            cmdPlayStop.Invoke((MethodInvoker)(() => { cmdPlayStop.Text = "&Play"; }));
            if (PositionUpdateTimer != null)
            {
                PositionUpdateTimer.Dispose();
                PositionUpdateTimer = null;
            }
            playedsound = null;
            playingsound = null;
            //File.Delete(mTempFilename);
            BCBlockGameState.QueueDelete(mTempFilename);
            mTempFilename = "";

        }

        void usedriver_OnSoundStop(iActiveSoundObject objstop)
        {
            if (objstop == playedsound)
            {
                SetStopState();
                usedriver.OnSoundStop -= usedriver_OnSoundStop;


            }
        }

        private void frmSoundDataListEditor_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (playingsound != null)
            {
                //stop music if playing.
                SetStopState();
            }

        }

        private void txtProp_Key_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtFileType_TextChanged(object sender, EventArgs e)
        {
            
        }
        private bool soundlistInit = false;
        private iActiveSoundObject menuplayingsound = null;
        private void tsPlaysound_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripDropDownItem source = sender as ToolStripDropDownItem;
            if (source == null) return;
            if (!soundlistInit || KeyboardInfo.IsPressed(Keys.ShiftKey))
            {
                soundlistInit = true;
                source.DropDown.Items.Clear();

                var stopall = new ToolStripMenuItem("Stop", null, (o, ev) => { if (menuplayingsound != null) menuplayingsound.Stop(); });
                source.DropDown.Items.Add(stopall);
                source.DropDown.Items.Add(new ToolStripSeparator());

                foreach (var iterate in BCBlockGameState.Soundman.SoundSources.Keys)
                {
                    var newitem = new ToolStripMenuItem(iterate, null, (obj, earg) =>
        {
            var sourceobject = (((ToolStripDropDownItem)obj).Tag as iSoundSourceObject);
            
            menuplayingsound = sourceobject.Play(false, 1.0f);



        });
                    newitem.Tag = BCBlockGameState.Soundman.SoundSources[iterate];

                    source.DropDown.Items.Add(newitem);

                }

            }
        }

        private void txtProp_Key_Validating(object sender, CancelEventArgs e)
        {
            if (txtProp_Key.Text.Length > 0)
            {
                //if key is not unique, disallow.
                if (EditList.ContainsKey(txtProp_Key.Text))
                {
                    e.Cancel = true;
                    return;

                }


                
            }
        }

        private void txtProp_Key_Validated(object sender, EventArgs e)
        {
            ApplyItem();
            SelectItem(CurrentlySelected);
        }

        private void txtFileType_Validated(object sender, EventArgs e)
        {
            ApplyItem();
            SelectItem(CurrentlySelected);
        }
    }
}
