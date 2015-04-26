namespace BASeBlock
{
    partial class frmCreatorPropertiesEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreatorPropertiesEditor));
            this.cmdOK = new System.Windows.Forms.Button();
            this.fraDataCache = new System.Windows.Forms.GroupBox();
            this.cmdEditScripts = new System.Windows.Forms.Button();
            this.cmdImages = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdSounds = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtdescription = new System.Windows.Forms.TextBox();
            this.chkTemplate = new System.Windows.Forms.CheckBox();
            this.tTips = new System.Windows.Forms.ToolTip(this.components);
            this.fraDataCache.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(238, 244);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(78, 27);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // fraDataCache
            // 
            this.fraDataCache.Controls.Add(this.cmdEditScripts);
            this.fraDataCache.Controls.Add(this.cmdImages);
            this.fraDataCache.Controls.Add(this.label1);
            this.fraDataCache.Controls.Add(this.cmdSounds);
            this.fraDataCache.Location = new System.Drawing.Point(8, 94);
            this.fraDataCache.Name = "fraDataCache";
            this.fraDataCache.Size = new System.Drawing.Size(308, 144);
            this.fraDataCache.TabIndex = 2;
            this.fraDataCache.TabStop = false;
            this.fraDataCache.Text = "Data Cache";
            // 
            // cmdEditScripts
            // 
            this.cmdEditScripts.Location = new System.Drawing.Point(67, 109);
            this.cmdEditScripts.Name = "cmdEditScripts";
            this.cmdEditScripts.Size = new System.Drawing.Size(74, 23);
            this.cmdEditScripts.TabIndex = 5;
            this.cmdEditScripts.Text = "&Scripts";
            this.cmdEditScripts.UseVisualStyleBackColor = true;
            this.cmdEditScripts.Visible = false;
            this.cmdEditScripts.Click += new System.EventHandler(this.cmdEditScripts_Click);
            // 
            // cmdImages
            // 
            this.cmdImages.Location = new System.Drawing.Point(147, 109);
            this.cmdImages.Name = "cmdImages";
            this.cmdImages.Size = new System.Drawing.Size(74, 23);
            this.cmdImages.TabIndex = 4;
            this.cmdImages.Text = "&Edit Images";
            this.tTips.SetToolTip(this.cmdImages, "Edit the Images in this LevelSet\'s DataCache.");
            this.cmdImages.UseVisualStyleBackColor = true;
            this.cmdImages.Click += new System.EventHandler(this.cmdImages_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 90);
            this.label1.TabIndex = 3;
            this.label1.Text = "Use the LevelSet Data Cache to store Images and Sounds that your LevelSet uses.";
            // 
            // cmdSounds
            // 
            this.cmdSounds.Location = new System.Drawing.Point(227, 109);
            this.cmdSounds.Name = "cmdSounds";
            this.cmdSounds.Size = new System.Drawing.Size(75, 23);
            this.cmdSounds.TabIndex = 2;
            this.cmdSounds.Text = "Edit S&ounds";
            this.tTips.SetToolTip(this.cmdSounds, "Edit the sounds in this LevelSet\'s Data Cache");
            this.cmdSounds.UseVisualStyleBackColor = true;
            this.cmdSounds.Click += new System.EventHandler(this.cmdSounds_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtdescription);
            this.groupBox1.Location = new System.Drawing.Point(10, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 89);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Description";
            // 
            // txtdescription
            // 
            this.txtdescription.Location = new System.Drawing.Point(5, 16);
            this.txtdescription.Multiline = true;
            this.txtdescription.Name = "txtdescription";
            this.txtdescription.Size = new System.Drawing.Size(294, 66);
            this.txtdescription.TabIndex = 0;
            this.tTips.SetToolTip(this.txtdescription, "Enter an appropriate description for this LevelSet");
            // 
            // chkTemplate
            // 
            this.chkTemplate.AutoSize = true;
            this.chkTemplate.Location = new System.Drawing.Point(12, 247);
            this.chkTemplate.Name = "chkTemplate";
            this.chkTemplate.Size = new System.Drawing.Size(128, 17);
            this.chkTemplate.TabIndex = 4;
            this.chkTemplate.Text = "Allow use as template";
            this.tTips.SetToolTip(this.chkTemplate, resources.GetString("chkTemplate.ToolTip"));
            this.chkTemplate.UseVisualStyleBackColor = true;
            // 
            // frmCreatorPropertiesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 276);
            this.Controls.Add(this.chkTemplate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fraDataCache);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCreatorPropertiesEditor";
            this.ShowInTaskbar = false;
            this.Text = "Set Properties";
            this.Load += new System.EventHandler(this.frmCreatorPropertiesEditor_Load);
            this.fraDataCache.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox fraDataCache;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdSounds;
        private System.Windows.Forms.Button cmdImages;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtdescription;
        private System.Windows.Forms.Button cmdEditScripts;
        private System.Windows.Forms.CheckBox chkTemplate;
        private System.Windows.Forms.ToolTip tTips;
    }
}