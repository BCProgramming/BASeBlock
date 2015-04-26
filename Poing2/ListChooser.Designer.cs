namespace BASeBlock
{
    partial class frmChooser<T>
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
            this.cmdOK = new System.Windows.Forms.Button();
            this.chkSaveSelection = new System.Windows.Forms.CheckBox();
            this.lbldescription = new System.Windows.Forms.Label();
            this.lvwChooseItems = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Enabled = false;
            this.cmdOK.Location = new System.Drawing.Point(133, 268);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(83, 26);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // chkSaveSelection
            // 
            this.chkSaveSelection.AutoSize = true;
            this.chkSaveSelection.Location = new System.Drawing.Point(9, 270);
            this.chkSaveSelection.Name = "chkSaveSelection";
            this.chkSaveSelection.Size = new System.Drawing.Size(98, 17);
            this.chkSaveSelection.TabIndex = 2;
            this.chkSaveSelection.Text = "&Save Selection";
            this.chkSaveSelection.UseVisualStyleBackColor = true;
            // 
            // lbldescription
            // 
            this.lbldescription.AutoSize = true;
            this.lbldescription.Location = new System.Drawing.Point(3, 0);
            this.lbldescription.Name = "lbldescription";
            this.lbldescription.Size = new System.Drawing.Size(43, 13);
            this.lbldescription.TabIndex = 3;
            this.lbldescription.Text = "Choose";
            // 
            // lvwChooseItems
            // 
            this.lvwChooseItems.FullRowSelect = true;
            this.lvwChooseItems.Location = new System.Drawing.Point(6, 40);
            this.lvwChooseItems.MultiSelect = false;
            this.lvwChooseItems.Name = "lvwChooseItems";
            this.lvwChooseItems.Size = new System.Drawing.Size(209, 216);
            this.lvwChooseItems.TabIndex = 4;
            this.lvwChooseItems.UseCompatibleStateImageBehavior = false;
            this.lvwChooseItems.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwChooseItems_MouseDoubleClick);
            this.lvwChooseItems.SelectedIndexChanged += new System.EventHandler(this.lvwChooseItems_SelectedIndexChanged);
            this.lvwChooseItems.DoubleClick += new System.EventHandler(this.lvwChooseItems_DoubleClick);
            this.lvwChooseItems.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwChooseItems_ItemSelectionChanged);
            // 
            // frmChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 302);
            this.ControlBox = false;
            this.Controls.Add(this.lvwChooseItems);
            this.Controls.Add(this.lbldescription);
            this.Controls.Add(this.chkSaveSelection);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmChooser";
            this.Text = "Chooser";
            this.Load += new System.EventHandler(this.frmChooser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.CheckBox chkSaveSelection;
        private System.Windows.Forms.Label lbldescription;
        private System.Windows.Forms.ListView lvwChooseItems;
    }
}