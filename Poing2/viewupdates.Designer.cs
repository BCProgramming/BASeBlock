namespace BASeBlock
{
    partial class frmUpdates
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
            this.components = new System.ComponentModel.Container();
            this.fraavailupdates = new System.Windows.Forms.GroupBox();
            this.lvwUpdates = new System.Windows.Forms.ListView();
            this.panLower = new System.Windows.Forms.Panel();
            this.btnDownload = new System.Windows.Forms.Button();
            this.cmdClose = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fraavailupdates.SuspendLayout();
            this.panLower.SuspendLayout();
            this.SuspendLayout();
            // 
            // fraavailupdates
            // 
            this.fraavailupdates.Controls.Add(this.lvwUpdates);
            this.fraavailupdates.Location = new System.Drawing.Point(2, 1);
            this.fraavailupdates.Name = "fraavailupdates";
            this.fraavailupdates.Size = new System.Drawing.Size(403, 232);
            this.fraavailupdates.TabIndex = 2;
            this.fraavailupdates.TabStop = false;
            this.fraavailupdates.Text = "Available Updates";
            // 
            // lvwUpdates
            // 
            this.lvwUpdates.CheckBoxes = true;
            this.lvwUpdates.FullRowSelect = true;
            this.lvwUpdates.GridLines = true;
            this.lvwUpdates.HideSelection = false;
            this.lvwUpdates.Location = new System.Drawing.Point(0, 19);
            this.lvwUpdates.Name = "lvwUpdates";
            this.lvwUpdates.OwnerDraw = true;
            this.lvwUpdates.Size = new System.Drawing.Size(397, 207);
            this.lvwUpdates.TabIndex = 1;
            this.lvwUpdates.UseCompatibleStateImageBehavior = false;
            this.lvwUpdates.View = System.Windows.Forms.View.Details;
            this.lvwUpdates.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvwUpdates_DrawColumnHeader);
            this.lvwUpdates.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvwUpdates_DrawItem);
            this.lvwUpdates.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvwUpdates_DrawSubItem);
            // 
            // panLower
            // 
            this.panLower.Controls.Add(this.btnDownload);
            this.panLower.Controls.Add(this.cmdClose);
            this.panLower.Location = new System.Drawing.Point(2, 233);
            this.panLower.Name = "panLower";
            this.panLower.Size = new System.Drawing.Size(403, 37);
            this.panLower.TabIndex = 3;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(3, 3);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(92, 25);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "&Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(310, 3);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(90, 26);
            this.cmdClose.TabIndex = 2;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // frmUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 271);
            this.Controls.Add(this.panLower);
            this.Controls.Add(this.fraavailupdates);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdates";
            this.Text = "BASeCamp Updater";
            this.Load += new System.EventHandler(this.frmUpdates_Load);
            this.Resize += new System.EventHandler(this.frmUpdates_Resize);
            this.fraavailupdates.ResumeLayout(false);
            this.panLower.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox fraavailupdates;
        private System.Windows.Forms.ListView lvwUpdates;
        private System.Windows.Forms.Panel panLower;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}