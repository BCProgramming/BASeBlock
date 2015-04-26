namespace BASeBlock
{
    partial class frmhighscores
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
            this.cmdClose = new System.Windows.Forms.Button();
            this.lvwHighScores = new System.Windows.Forms.ListView();
            this.cboHighScoreSets = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(244, 320);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(74, 27);
            this.cmdClose.TabIndex = 0;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // lvwHighScores
            // 
            this.lvwHighScores.Location = new System.Drawing.Point(3, 12);
            this.lvwHighScores.Name = "lvwHighScores";
            this.lvwHighScores.Size = new System.Drawing.Size(314, 298);
            this.lvwHighScores.TabIndex = 1;
            this.lvwHighScores.UseCompatibleStateImageBehavior = false;
            // 
            // cboHighScoreSets
            // 
            this.cboHighScoreSets.FormattingEnabled = true;
            this.cboHighScoreSets.Location = new System.Drawing.Point(6, 317);
            this.cboHighScoreSets.Name = "cboHighScoreSets";
            this.cboHighScoreSets.Size = new System.Drawing.Size(215, 21);
            this.cboHighScoreSets.TabIndex = 2;
            this.cboHighScoreSets.SelectedIndexChanged += new System.EventHandler(this.cboHighScoreSets_SelectedIndexChanged);
            // 
            // frmhighscores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 352);
            this.Controls.Add(this.cboHighScoreSets);
            this.Controls.Add(this.lvwHighScores);
            this.Controls.Add(this.cmdClose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmhighscores";
            this.Text = "frmhighscores";
            this.Load += new System.EventHandler(this.frmhighscores_Load);
            this.Resize += new System.EventHandler(this.frmhighscores_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ListView lvwHighScores;
        private System.Windows.Forms.ComboBox cboHighScoreSets;
    }
}