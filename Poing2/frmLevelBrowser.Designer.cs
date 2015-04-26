namespace BASeBlock
{
    partial class frmLevelBrowser
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
            this.tvwLevelSets = new System.Windows.Forms.TreeView();
            this.btnOK = new System.Windows.Forms.Button();
            this.PicLevel = new System.Windows.Forms.PictureBox();
            this.lblViewLabel = new System.Windows.Forms.Label();
            this.cboViewStyle = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.PicLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // tvwLevelSets
            // 
            this.tvwLevelSets.Location = new System.Drawing.Point(4, 3);
            this.tvwLevelSets.Name = "tvwLevelSets";
            this.tvwLevelSets.Size = new System.Drawing.Size(161, 363);
            this.tvwLevelSets.TabIndex = 0;
            this.tvwLevelSets.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvwLevelSets_BeforeExpand);
            this.tvwLevelSets.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwLevelSets_AfterSelect);
            this.tvwLevelSets.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvwLevelSets_NodeMouseClick);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(455, 339);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 27);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PicLevel
            // 
            this.PicLevel.Location = new System.Drawing.Point(171, 5);
            this.PicLevel.Name = "PicLevel";
            this.PicLevel.Size = new System.Drawing.Size(357, 329);
            this.PicLevel.TabIndex = 1;
            this.PicLevel.TabStop = false;
            this.PicLevel.Paint += new System.Windows.Forms.PaintEventHandler(this.PicLevel_Paint);
            // 
            // lblViewLabel
            // 
            this.lblViewLabel.AutoSize = true;
            this.lblViewLabel.Location = new System.Drawing.Point(182, 346);
            this.lblViewLabel.Name = "lblViewLabel";
            this.lblViewLabel.Size = new System.Drawing.Size(33, 13);
            this.lblViewLabel.TabIndex = 3;
            this.lblViewLabel.Text = "View:";
            this.lblViewLabel.Visible = false;
            // 
            // cboViewStyle
            // 
            this.cboViewStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboViewStyle.FormattingEnabled = true;
            this.cboViewStyle.Location = new System.Drawing.Point(221, 343);
            this.cboViewStyle.Name = "cboViewStyle";
            this.cboViewStyle.Size = new System.Drawing.Size(179, 21);
            this.cboViewStyle.TabIndex = 4;
            this.cboViewStyle.Visible = false;
            // 
            // frmLevelBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 371);
            this.Controls.Add(this.cboViewStyle);
            this.Controls.Add(this.lblViewLabel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.PicLevel);
            this.Controls.Add(this.tvwLevelSets);
            this.Name = "frmLevelBrowser";
            this.Text = "frmLevelBrowser";
            this.Load += new System.EventHandler(this.frmLevelBrowser_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PicLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvwLevelSets;
        private System.Windows.Forms.PictureBox PicLevel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblViewLabel;
        private System.Windows.Forms.ComboBox cboViewStyle;
    }
}