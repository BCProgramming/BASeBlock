namespace BASeBlock
{
    partial class FolderListView
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
            this.cboFolderTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpFolders = new System.Windows.Forms.GroupBox();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.lstFolders = new System.Windows.Forms.ListBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.grpFolders.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboFolderTypes
            // 
            this.cboFolderTypes.FormattingEnabled = true;
            this.cboFolderTypes.Location = new System.Drawing.Point(52, 12);
            this.cboFolderTypes.Name = "cboFolderTypes";
            this.cboFolderTypes.Size = new System.Drawing.Size(259, 21);
            this.cboFolderTypes.TabIndex = 0;
            this.cboFolderTypes.SelectedIndexChanged += new System.EventHandler(this.cboFolderTypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type:";
            // 
            // grpFolders
            // 
            this.grpFolders.Controls.Add(this.cmdRemove);
            this.grpFolders.Controls.Add(this.cmdAdd);
            this.grpFolders.Controls.Add(this.lstFolders);
            this.grpFolders.Location = new System.Drawing.Point(12, 44);
            this.grpFolders.Name = "grpFolders";
            this.grpFolders.Size = new System.Drawing.Size(298, 252);
            this.grpFolders.TabIndex = 2;
            this.grpFolders.TabStop = false;
            this.grpFolders.Text = "Folders";
            // 
            // cmdRemove
            // 
            this.cmdRemove.Location = new System.Drawing.Point(132, 220);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(75, 26);
            this.cmdRemove.TabIndex = 2;
            this.cmdRemove.Text = "&Remove";
            this.cmdRemove.UseVisualStyleBackColor = true;
            // 
            // cmdAdd
            // 
            this.cmdAdd.Location = new System.Drawing.Point(211, 220);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(75, 26);
            this.cmdAdd.TabIndex = 1;
            this.cmdAdd.Text = "&Add...";
            this.cmdAdd.UseVisualStyleBackColor = true;
            // 
            // lstFolders
            // 
            this.lstFolders.FormattingEnabled = true;
            this.lstFolders.Location = new System.Drawing.Point(7, 20);
            this.lstFolders.Name = "lstFolders";
            this.lstFolders.Size = new System.Drawing.Size(279, 186);
            this.lstFolders.TabIndex = 0;
            this.lstFolders.SelectedIndexChanged += new System.EventHandler(this.lstFolders_SelectedIndexChanged);
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(235, 302);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(74, 25);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // FolderListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 338);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.grpFolders);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboFolderTypes);
            this.Name = "FolderListView";
            this.Text = "FolderListView";
            this.Load += new System.EventHandler(this.FolderListView_Load);
            this.grpFolders.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboFolderTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpFolders;
        private System.Windows.Forms.Button cmdRemove;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.ListBox lstFolders;
        private System.Windows.Forms.Button cmdOK;

    }
}