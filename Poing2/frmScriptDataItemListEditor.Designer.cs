namespace BASeCamp.BASeBlock
{
    partial class frmScriptDataItemListEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScriptDataItemListEditor));
            this.lvwScripts = new System.Windows.Forms.ListView();
            this.cmdClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.txtLanguage = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.cmdEditCode = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ToolStripAddItem = new System.Windows.Forms.ToolStripButton();
            this.toolstripRemove = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwScripts
            // 
            this.lvwScripts.Location = new System.Drawing.Point(10, 33);
            this.lvwScripts.Name = "lvwScripts";
            this.lvwScripts.Size = new System.Drawing.Size(263, 85);
            this.lvwScripts.TabIndex = 0;
            this.lvwScripts.UseCompatibleStateImageBehavior = false;
            this.lvwScripts.View = System.Windows.Forms.View.Details;
            this.lvwScripts.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwScripts_ItemSelectionChanged);
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(199, 234);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(74, 25);
            this.cmdClose.TabIndex = 1;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(46, 124);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(189, 20);
            this.txtName.TabIndex = 3;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(12, 152);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(58, 13);
            this.lblLanguage.TabIndex = 4;
            this.lblLanguage.Text = "Language:";
            // 
            // txtLanguage
            // 
            this.txtLanguage.Location = new System.Drawing.Point(76, 149);
            this.txtLanguage.Name = "txtLanguage";
            this.txtLanguage.Size = new System.Drawing.Size(159, 20);
            this.txtLanguage.TabIndex = 5;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(16, 196);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(35, 13);
            this.lblCode.TabIndex = 6;
            this.lblCode.Text = "Code:";
            // 
            // cmdEditCode
            // 
            this.cmdEditCode.Location = new System.Drawing.Point(57, 189);
            this.cmdEditCode.Name = "cmdEditCode";
            this.cmdEditCode.Size = new System.Drawing.Size(61, 27);
            this.cmdEditCode.TabIndex = 7;
            this.cmdEditCode.Text = "&Edit...";
            this.cmdEditCode.UseVisualStyleBackColor = true;
            this.cmdEditCode.Click += new System.EventHandler(this.cmdEditCode_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripAddItem,
            this.toolstripRemove});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(285, 25);
            this.toolStrip1.TabIndex = 8;
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
            // frmScriptDataItemListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 272);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cmdEditCode);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.txtLanguage);
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.lvwScripts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmScriptDataItemListEditor";
            this.Text = "frmScriptDataItemListEditor";
            this.Load += new System.EventHandler(this.frmScriptDataItemListEditor_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwScripts;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.TextBox txtLanguage;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.Button cmdEditCode;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ToolStripAddItem;
        private System.Windows.Forms.ToolStripButton toolstripRemove;
    }
}