namespace BASeBlock
{
    partial class frmImageDataListEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImageDataListEditor));
            this.lvwImageEntries = new System.Windows.Forms.ListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ToolStripAddItem = new System.Windows.Forms.ToolStripButton();
            this.toolstripRemove = new System.Windows.Forms.ToolStripButton();
            this.tstripshowhidden = new System.Windows.Forms.ToolStripButton();
            this.fraDetails = new System.Windows.Forms.GroupBox();
            this.chkHidden = new System.Windows.Forms.CheckBox();
            this.picImagePreview = new System.Windows.Forms.PictureBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.txtProp_Key = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.fraDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImagePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // lvwImageEntries
            // 
            this.lvwImageEntries.HideSelection = false;
            this.lvwImageEntries.Location = new System.Drawing.Point(0, 28);
            this.lvwImageEntries.Name = "lvwImageEntries";
            this.lvwImageEntries.Size = new System.Drawing.Size(314, 169);
            this.lvwImageEntries.TabIndex = 0;
            this.lvwImageEntries.UseCompatibleStateImageBehavior = false;
            this.lvwImageEntries.View = System.Windows.Forms.View.Details;
            this.lvwImageEntries.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwImageEntries_ItemSelectionChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripAddItem,
            this.toolstripRemove,
            this.tstripshowhidden});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(323, 25);
            this.toolStrip1.TabIndex = 3;
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
            // tstripshowhidden
            // 
            this.tstripshowhidden.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tstripshowhidden.Image = ((System.Drawing.Image)(resources.GetObject("tstripshowhidden.Image")));
            this.tstripshowhidden.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tstripshowhidden.Name = "tstripshowhidden";
            this.tstripshowhidden.Size = new System.Drawing.Size(82, 22);
            this.tstripshowhidden.Text = "Show Hidden";
            this.tstripshowhidden.ToolTipText = "Show Hidden items";
            this.tstripshowhidden.Click += new System.EventHandler(this.tstripshowhidden_Click);
            // 
            // fraDetails
            // 
            this.fraDetails.Controls.Add(this.chkHidden);
            this.fraDetails.Controls.Add(this.picImagePreview);
            this.fraDetails.Controls.Add(this.cmdBrowse);
            this.fraDetails.Controls.Add(this.txtProp_Key);
            this.fraDetails.Controls.Add(this.lblName);
            this.fraDetails.Location = new System.Drawing.Point(3, 201);
            this.fraDetails.Name = "fraDetails";
            this.fraDetails.Size = new System.Drawing.Size(311, 135);
            this.fraDetails.TabIndex = 4;
            this.fraDetails.TabStop = false;
            this.fraDetails.Text = "Details";
            // 
            // chkHidden
            // 
            this.chkHidden.AutoSize = true;
            this.chkHidden.Location = new System.Drawing.Point(23, 60);
            this.chkHidden.Name = "chkHidden";
            this.chkHidden.Size = new System.Drawing.Size(60, 17);
            this.chkHidden.TabIndex = 4;
            this.chkHidden.Text = "&Hidden";
            this.chkHidden.UseVisualStyleBackColor = true;
            // 
            // picImagePreview
            // 
            this.picImagePreview.Location = new System.Drawing.Point(204, 16);
            this.picImagePreview.Name = "picImagePreview";
            this.picImagePreview.Size = new System.Drawing.Size(97, 89);
            this.picImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picImagePreview.TabIndex = 3;
            this.picImagePreview.TabStop = false;
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(23, 92);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(74, 27);
            this.cmdBrowse.TabIndex = 2;
            this.cmdBrowse.Text = "Browse...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // txtProp_Key
            // 
            this.txtProp_Key.Location = new System.Drawing.Point(54, 24);
            this.txtProp_Key.Name = "txtProp_Key";
            this.txtProp_Key.Size = new System.Drawing.Size(143, 20);
            this.txtProp_Key.TabIndex = 1;
            this.txtProp_Key.Validated += new System.EventHandler(this.txtProp_Key_Validated);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 27);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(28, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Key:";
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(227, 342);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(83, 29);
            this.cmdOK.TabIndex = 5;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmImageDataListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 382);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.fraDetails);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lvwImageEntries);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImageDataListEditor";
            this.Text = "Image Data List Editor";
            this.Load += new System.EventHandler(this.frmImageDataListEditor_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.fraDetails.ResumeLayout(false);
            this.fraDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImagePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwImageEntries;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ToolStripAddItem;
        private System.Windows.Forms.ToolStripButton toolstripRemove;
        private System.Windows.Forms.GroupBox fraDetails;
        private System.Windows.Forms.TextBox txtProp_Key;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.PictureBox picImagePreview;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.ToolStripButton tstripshowhidden;
        private System.Windows.Forms.CheckBox chkHidden;
    }
}