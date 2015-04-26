namespace BASeBlock
{
    partial class frmSoundDataListEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSoundDataListEditor));
            this.lvwSoundEntries = new System.Windows.Forms.ListView();
            this.grpProperties = new System.Windows.Forms.GroupBox();
            this.tbarSoundPosition = new System.Windows.Forms.TrackBar();
            this.cmdPlayStop = new System.Windows.Forms.Button();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.txtFileType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDatainfo = new System.Windows.Forms.Label();
            this.txtProp_Key = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ToolStripAddItem = new System.Windows.Forms.ToolStripButton();
            this.toolstripRemove = new System.Windows.Forms.ToolStripButton();
            this.tsPlaysound = new System.Windows.Forms.ToolStripDropDownButton();
            this.gHOSTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.grpProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbarSoundPosition)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwSoundEntries
            // 
            this.lvwSoundEntries.Location = new System.Drawing.Point(3, 28);
            this.lvwSoundEntries.MultiSelect = false;
            this.lvwSoundEntries.Name = "lvwSoundEntries";
            this.lvwSoundEntries.Size = new System.Drawing.Size(360, 177);
            this.lvwSoundEntries.TabIndex = 0;
            this.lvwSoundEntries.UseCompatibleStateImageBehavior = false;
            this.lvwSoundEntries.View = System.Windows.Forms.View.Details;
            this.lvwSoundEntries.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwSoundEntries_ItemSelectionChanged);
            // 
            // grpProperties
            // 
            this.grpProperties.Controls.Add(this.tbarSoundPosition);
            this.grpProperties.Controls.Add(this.cmdPlayStop);
            this.grpProperties.Controls.Add(this.cmdBrowse);
            this.grpProperties.Controls.Add(this.txtFileType);
            this.grpProperties.Controls.Add(this.label1);
            this.grpProperties.Controls.Add(this.lblDatainfo);
            this.grpProperties.Controls.Add(this.txtProp_Key);
            this.grpProperties.Controls.Add(this.lblName);
            this.grpProperties.Location = new System.Drawing.Point(4, 211);
            this.grpProperties.Name = "grpProperties";
            this.grpProperties.Size = new System.Drawing.Size(359, 120);
            this.grpProperties.TabIndex = 1;
            this.grpProperties.TabStop = false;
            this.grpProperties.Text = "Properties";
            // 
            // tbarSoundPosition
            // 
            this.tbarSoundPosition.Enabled = false;
            this.tbarSoundPosition.Location = new System.Drawing.Point(196, 69);
            this.tbarSoundPosition.Name = "tbarSoundPosition";
            this.tbarSoundPosition.Size = new System.Drawing.Size(157, 45);
            this.tbarSoundPosition.TabIndex = 5;
            // 
            // cmdPlayStop
            // 
            this.cmdPlayStop.Location = new System.Drawing.Point(109, 83);
            this.cmdPlayStop.Name = "cmdPlayStop";
            this.cmdPlayStop.Size = new System.Drawing.Size(81, 23);
            this.cmdPlayStop.TabIndex = 4;
            this.cmdPlayStop.Text = "&Play";
            this.cmdPlayStop.UseVisualStyleBackColor = true;
            this.cmdPlayStop.Click += new System.EventHandler(this.cmdPlayStop_Click);
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(14, 82);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(80, 25);
            this.cmdBrowse.TabIndex = 3;
            this.cmdBrowse.Text = "Browse...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // txtFileType
            // 
            this.txtFileType.Location = new System.Drawing.Point(67, 49);
            this.txtFileType.Name = "txtFileType";
            this.txtFileType.Size = new System.Drawing.Size(145, 20);
            this.txtFileType.TabIndex = 2;
            this.txtFileType.TextChanged += new System.EventHandler(this.txtFileType_TextChanged);
            this.txtFileType.Validated += new System.EventHandler(this.txtFileType_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Type:";
            // 
            // lblDatainfo
            // 
            this.lblDatainfo.AutoSize = true;
            this.lblDatainfo.Location = new System.Drawing.Point(219, 50);
            this.lblDatainfo.Name = "lblDatainfo";
            this.lblDatainfo.Size = new System.Drawing.Size(64, 13);
            this.lblDatainfo.TabIndex = 2;
            this.lblDatainfo.Text = "Size:0 bytes";
            // 
            // txtProp_Key
            // 
            this.txtProp_Key.Location = new System.Drawing.Point(67, 16);
            this.txtProp_Key.Name = "txtProp_Key";
            this.txtProp_Key.Size = new System.Drawing.Size(216, 20);
            this.txtProp_Key.TabIndex = 1;
            this.txtProp_Key.TextChanged += new System.EventHandler(this.txtProp_Key_TextChanged);
            this.txtProp_Key.Validating += new System.ComponentModel.CancelEventHandler(this.txtProp_Key_Validating);
            this.txtProp_Key.Validated += new System.EventHandler(this.txtProp_Key_Validated);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(9, 19);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(62, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name(Key):";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripAddItem,
            this.toolstripRemove,
            this.tsPlaysound});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(375, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ToolStripAddItem
            // 
            this.ToolStripAddItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripAddItem.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripAddItem.Image")));
            this.ToolStripAddItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripAddItem.Name = "ToolStripAddItem";
            this.ToolStripAddItem.Size = new System.Drawing.Size(33, 22);
            this.ToolStripAddItem.Text = "Add";
            this.ToolStripAddItem.Click += new System.EventHandler(this.ToolStripAddItem_Click);
            // 
            // toolstripRemove
            // 
            this.toolstripRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolstripRemove.Image = ((System.Drawing.Image)(resources.GetObject("toolstripRemove.Image")));
            this.toolstripRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripRemove.Name = "toolstripRemove";
            this.toolstripRemove.Size = new System.Drawing.Size(54, 22);
            this.toolstripRemove.Text = "Remove";
            this.toolstripRemove.Click += new System.EventHandler(this.toolstripRemove_Click);
            // 
            // tsPlaysound
            // 
            this.tsPlaysound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsPlaysound.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gHOSTToolStripMenuItem});
            this.tsPlaysound.Image = ((System.Drawing.Image)(resources.GetObject("tsPlaysound.Image")));
            this.tsPlaysound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPlaysound.Name = "tsPlaysound";
            this.tsPlaysound.Size = new System.Drawing.Size(42, 22);
            this.tsPlaysound.Text = "Play";
            this.tsPlaysound.DropDownOpening += new System.EventHandler(this.tsPlaysound_DropDownOpening);
            // 
            // gHOSTToolStripMenuItem
            // 
            this.gHOSTToolStripMenuItem.Name = "gHOSTToolStripMenuItem";
            this.gHOSTToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.gHOSTToolStripMenuItem.Text = "GHOST";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(272, 337);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 30);
            this.button1.TabIndex = 6;
            this.button1.Text = "&OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmSoundDataListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 371);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.grpProperties);
            this.Controls.Add(this.lvwSoundEntries);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSoundDataListEditor";
            this.ShowInTaskbar = false;
            this.Text = "Edit Sound Entries";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSoundDataListEditor_FormClosing);
            this.Load += new System.EventHandler(this.frmSoundDataListEditor_Load);
            this.grpProperties.ResumeLayout(false);
            this.grpProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbarSoundPosition)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwSoundEntries;
        private System.Windows.Forms.GroupBox grpProperties;
        private System.Windows.Forms.Label lblDatainfo;
        private System.Windows.Forms.TextBox txtProp_Key;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtFileType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ToolStripAddItem;
        private System.Windows.Forms.ToolStripButton toolstripRemove;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button cmdPlayStop;
        private System.Windows.Forms.TrackBar tbarSoundPosition;
        private System.Windows.Forms.ToolStripDropDownButton tsPlaysound;
        private System.Windows.Forms.ToolStripMenuItem gHOSTToolStripMenuItem;
    }
}