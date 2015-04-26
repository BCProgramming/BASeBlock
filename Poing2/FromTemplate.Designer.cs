namespace BASeBlock.Templates
{
    partial class frmFromTemplate
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
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpCategory = new System.Windows.Forms.GroupBox();
            this.lvwCategories = new System.Windows.Forms.ListView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvwTemplates = new System.Windows.Forms.ListView();
            this.grpCategory.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Enabled = false;
            this.cmdOK.Location = new System.Drawing.Point(475, 255);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(394, 255);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // grpCategory
            // 
            this.grpCategory.Controls.Add(this.lvwCategories);
            this.grpCategory.Location = new System.Drawing.Point(1, 3);
            this.grpCategory.Name = "grpCategory";
            this.grpCategory.Size = new System.Drawing.Size(167, 246);
            this.grpCategory.TabIndex = 4;
            this.grpCategory.TabStop = false;
            this.grpCategory.Text = "Category";
            // 
            // lvwCategories
            // 
            this.lvwCategories.HideSelection = false;
            this.lvwCategories.Location = new System.Drawing.Point(11, 19);
            this.lvwCategories.Name = "lvwCategories";
            this.lvwCategories.Size = new System.Drawing.Size(150, 221);
            this.lvwCategories.TabIndex = 0;
            this.lvwCategories.UseCompatibleStateImageBehavior = false;
            this.lvwCategories.SelectedIndexChanged += new System.EventHandler(this.lvwCategories_SelectedIndexChanged);
            this.lvwCategories.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwCategories_ItemSelectionChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lvwTemplates);
            this.groupBox2.Location = new System.Drawing.Point(168, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(382, 246);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "&Templates";
            // 
            // lvwTemplates
            // 
            this.lvwTemplates.HideSelection = false;
            this.lvwTemplates.Location = new System.Drawing.Point(6, 19);
            this.lvwTemplates.Name = "lvwTemplates";
            this.lvwTemplates.Size = new System.Drawing.Size(370, 221);
            this.lvwTemplates.TabIndex = 1;
            this.lvwTemplates.UseCompatibleStateImageBehavior = false;
            this.lvwTemplates.SelectedIndexChanged += new System.EventHandler(this.lvwTemplates_SelectedIndexChanged);
            this.lvwTemplates.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwTemplates_ItemSelectionChanged);
            // 
            // frmFromTemplate
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(562, 290);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grpCategory);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFromTemplate";
            this.ShowInTaskbar = false;
            this.Text = "Load From Template";
            this.Load += new System.EventHandler(this.frmFromTemplate_Load);
            this.grpCategory.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox grpCategory;
        private System.Windows.Forms.ListView lvwCategories;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView lvwTemplates;
    }
}