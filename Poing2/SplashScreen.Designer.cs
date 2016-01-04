namespace BASeCamp.BASeBlock
{
    partial class SplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.panProgress = new System.Windows.Forms.Panel();
            this.cmdCopy = new System.Windows.Forms.Button();
            this.txtprogress = new System.Windows.Forms.TextBox();
            this.panWin31 = new System.Windows.Forms.Panel();
            this.PanelAbout = new System.Windows.Forms.PictureBox();
            this.chkShowLoaded = new System.Windows.Forms.CheckBox();
            this.tmrFade = new System.Windows.Forms.Timer(this.components);
            this.panStandard = new System.Windows.Forms.Panel();
            this.panImage = new System.Windows.Forms.PictureBox();
            this.rdash = new System.Windows.Forms.Panel();
            this.ttoaster = new System.Windows.Forms.Panel();
            this.panStandard2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panStandard3 = new System.Windows.Forms.Panel();
            this.panstandard4 = new System.Windows.Forms.Panel();
            this.panstandard5 = new System.Windows.Forms.Panel();
            this.pandetails = new System.Windows.Forms.Panel();
            this.tabDetails = new System.Windows.Forms.TabControl();
            this.tabAssemblies = new System.Windows.Forms.TabPage();
            this.tabPlugins = new System.Windows.Forms.TabPage();
            this.lvwAssemblies = new System.Windows.Forms.ListView();
            this.panProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PanelAbout)).BeginInit();
            this.PanelAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panImage)).BeginInit();
            this.pandetails.SuspendLayout();
            this.tabDetails.SuspendLayout();
            this.tabAssemblies.SuspendLayout();
            this.SuspendLayout();
            // 
            // panProgress
            // 
            this.panProgress.Controls.Add(this.cmdCopy);
            this.panProgress.Controls.Add(this.txtprogress);
            this.panProgress.Controls.Add(this.panWin31);
            this.panProgress.Location = new System.Drawing.Point(1, 256);
            this.panProgress.Name = "panProgress";
            this.panProgress.Size = new System.Drawing.Size(431, 56);
            this.panProgress.TabIndex = 1;
            this.panProgress.Resize += new System.EventHandler(this.panProgress_Resize);
            // 
            // cmdCopy
            // 
            this.cmdCopy.Location = new System.Drawing.Point(362, 3);
            this.cmdCopy.Name = "cmdCopy";
            this.cmdCopy.Size = new System.Drawing.Size(66, 46);
            this.cmdCopy.TabIndex = 1;
            this.cmdCopy.Text = "&Copy";
            this.cmdCopy.UseVisualStyleBackColor = true;
            this.cmdCopy.Visible = false;
            this.cmdCopy.Click += new System.EventHandler(this.cmdCopy_Click);
            // 
            // txtprogress
            // 
            this.txtprogress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtprogress.Location = new System.Drawing.Point(3, 3);
            this.txtprogress.Multiline = true;
            this.txtprogress.Name = "txtprogress";
            this.txtprogress.Size = new System.Drawing.Size(356, 50);
            this.txtprogress.TabIndex = 0;
            this.txtprogress.WordWrap = false;
            this.txtprogress.TextChanged += new System.EventHandler(this.txtprogress_TextChanged);
            this.txtprogress.MouseEnter += new System.EventHandler(this.txtprogress_MouseEnter);
            this.txtprogress.MouseLeave += new System.EventHandler(this.txtprogress_MouseLeave);
            // 
            // panWin31
            // 
            this.panWin31.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panWin31.BackgroundImage")));
            this.panWin31.Location = new System.Drawing.Point(6, 55);
            this.panWin31.Name = "panWin31";
            this.panWin31.Size = new System.Drawing.Size(422, 501);
            this.panWin31.TabIndex = 2;
            this.panWin31.Visible = false;
            // 
            // PanelAbout
            // 
            this.PanelAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.PanelAbout.Controls.Add(this.chkShowLoaded);
            this.PanelAbout.Location = new System.Drawing.Point(106, 74);
            this.PanelAbout.Name = "PanelAbout";
            this.PanelAbout.Size = new System.Drawing.Size(427, 231);
            this.PanelAbout.TabIndex = 2;
            this.PanelAbout.TabStop = false;
            // 
            // chkShowLoaded
            // 
            this.chkShowLoaded.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkShowLoaded.AutoSize = true;
            this.chkShowLoaded.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkShowLoaded.Location = new System.Drawing.Point(50, 7);
            this.chkShowLoaded.Name = "chkShowLoaded";
            this.chkShowLoaded.Size = new System.Drawing.Size(138, 23);
            this.chkShowLoaded.TabIndex = 4;
            this.chkShowLoaded.Text = "Show Loaded Assemblies";
            this.chkShowLoaded.UseVisualStyleBackColor = true;
            this.chkShowLoaded.CheckedChanged += new System.EventHandler(this.chkShowLoaded_CheckedChanged);
            // 
            // tmrFade
            // 
            this.tmrFade.Interval = 10;
            this.tmrFade.Tick += new System.EventHandler(this.tmrFade_Tick);
            // 
            // panStandard
            // 
            this.panStandard.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panStandard.BackgroundImage")));
            this.panStandard.Location = new System.Drawing.Point(438, 263);
            this.panStandard.Name = "panStandard";
            this.panStandard.Size = new System.Drawing.Size(431, 260);
            this.panStandard.TabIndex = 4;
            this.panStandard.Visible = false;
            // 
            // panImage
            // 
            this.panImage.Location = new System.Drawing.Point(4, -1);
            this.panImage.Name = "panImage";
            this.panImage.Size = new System.Drawing.Size(428, 258);
            this.panImage.TabIndex = 5;
            this.panImage.TabStop = false;
            this.panImage.Click += new System.EventHandler(this.panImage_Click);
            this.panImage.Paint += new System.Windows.Forms.PaintEventHandler(this.panImage_Paint);
            // 
            // rdash
            // 
            this.rdash.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rdash.BackgroundImage")));
            this.rdash.Location = new System.Drawing.Point(156, 42);
            this.rdash.Name = "rdash";
            this.rdash.Size = new System.Drawing.Size(83, 39);
            this.rdash.TabIndex = 7;
            this.rdash.Visible = false;
            // 
            // ttoaster
            // 
            this.ttoaster.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ttoaster.BackgroundImage")));
            this.ttoaster.Location = new System.Drawing.Point(177, 139);
            this.ttoaster.Name = "ttoaster";
            this.ttoaster.Size = new System.Drawing.Size(80, 37);
            this.ttoaster.TabIndex = 9;
            this.ttoaster.Visible = false;
            // 
            // panStandard2
            // 
            this.panStandard2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panStandard2.BackgroundImage")));
            this.panStandard2.Location = new System.Drawing.Point(441, 12);
            this.panStandard2.Name = "panStandard2";
            this.panStandard2.Size = new System.Drawing.Size(431, 260);
            this.panStandard2.TabIndex = 5;
            this.panStandard2.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Location = new System.Drawing.Point(257, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(83, 39);
            this.panel1.TabIndex = 10;
            this.panel1.Visible = false;
            // 
            // panStandard3
            // 
            this.panStandard3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panStandard3.BackgroundImage")));
            this.panStandard3.Location = new System.Drawing.Point(438, 126);
            this.panStandard3.Name = "panStandard3";
            this.panStandard3.Size = new System.Drawing.Size(431, 260);
            this.panStandard3.TabIndex = 11;
            this.panStandard3.Visible = false;
            // 
            // panstandard4
            // 
            this.panstandard4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panstandard4.BackgroundImage")));
            this.panstandard4.Location = new System.Drawing.Point(350, 112);
            this.panstandard4.Name = "panstandard4";
            this.panstandard4.Size = new System.Drawing.Size(431, 260);
            this.panstandard4.TabIndex = 12;
            this.panstandard4.Visible = false;
            // 
            // panstandard5
            // 
            this.panstandard5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panstandard5.BackgroundImage")));
            this.panstandard5.Location = new System.Drawing.Point(411, 95);
            this.panstandard5.Name = "panstandard5";
            this.panstandard5.Size = new System.Drawing.Size(431, 260);
            this.panstandard5.TabIndex = 14;
            this.panstandard5.Visible = false;
            // 
            // pandetails
            // 
            this.pandetails.Controls.Add(this.tabDetails);
            this.pandetails.Location = new System.Drawing.Point(12, 4);
            this.pandetails.Name = "pandetails";
            this.pandetails.Size = new System.Drawing.Size(423, 268);
            this.pandetails.TabIndex = 15;
            this.pandetails.Visible = false;
            this.pandetails.Resize += new System.EventHandler(this.pandetails_Resize);
            // 
            // tabDetails
            // 
            this.tabDetails.Controls.Add(this.tabAssemblies);
            this.tabDetails.Controls.Add(this.tabPlugins);
            this.tabDetails.Location = new System.Drawing.Point(3, 17);
            this.tabDetails.Name = "tabDetails";
            this.tabDetails.SelectedIndex = 0;
            this.tabDetails.Size = new System.Drawing.Size(414, 248);
            this.tabDetails.TabIndex = 0;
            this.tabDetails.Resize += new System.EventHandler(this.tabDetails_Resize);
            // 
            // tabAssemblies
            // 
            this.tabAssemblies.Controls.Add(this.lvwAssemblies);
            this.tabAssemblies.Location = new System.Drawing.Point(4, 22);
            this.tabAssemblies.Name = "tabAssemblies";
            this.tabAssemblies.Padding = new System.Windows.Forms.Padding(3);
            this.tabAssemblies.Size = new System.Drawing.Size(406, 222);
            this.tabAssemblies.TabIndex = 0;
            this.tabAssemblies.Text = "Assemblies";
            this.tabAssemblies.UseVisualStyleBackColor = true;
            this.tabAssemblies.Resize += new System.EventHandler(this.tabAssemblies_Resize);
            // 
            // tabPlugins
            // 
            this.tabPlugins.Location = new System.Drawing.Point(4, 22);
            this.tabPlugins.Name = "tabPlugins";
            this.tabPlugins.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlugins.Size = new System.Drawing.Size(406, 236);
            this.tabPlugins.TabIndex = 1;
            this.tabPlugins.Text = "Plugins";
            this.tabPlugins.UseVisualStyleBackColor = true;
            // 
            // lvwAssemblies
            // 
            this.lvwAssemblies.Location = new System.Drawing.Point(2, 17);
            this.lvwAssemblies.Name = "lvwAssemblies";
            this.lvwAssemblies.Size = new System.Drawing.Size(398, 199);
            this.lvwAssemblies.TabIndex = 8;
            this.lvwAssemblies.UseCompatibleStateImageBehavior = false;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 502);
            this.ControlBox = false;
            this.Controls.Add(this.pandetails);
            this.Controls.Add(this.panstandard5);
            this.Controls.Add(this.panstandard4);
            this.Controls.Add(this.panStandard3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panStandard2);
            this.Controls.Add(this.ttoaster);
            this.Controls.Add(this.rdash);
            this.Controls.Add(this.PanelAbout);
            this.Controls.Add(this.panImage);
            this.Controls.Add(this.panStandard);
            this.Controls.Add(this.panProgress);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "SplashScreen";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "BASeBlock Starting...";
            this.Activated += new System.EventHandler(this.SplashScreen_Activated);
            this.Deactivate += new System.EventHandler(this.SplashScreen_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SplashScreen_FormClosed);
            this.Load += new System.EventHandler(this.SplashScreen_Load);
            this.Shown += new System.EventHandler(this.SplashScreen_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SplashScreen_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SplashScreen_KeyPress);
            this.panProgress.ResumeLayout(false);
            this.panProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PanelAbout)).EndInit();
            this.PanelAbout.ResumeLayout(false);
            this.PanelAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panImage)).EndInit();
            this.pandetails.ResumeLayout(false);
            this.tabDetails.ResumeLayout(false);
            this.tabAssemblies.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panProgress;
        private System.Windows.Forms.TextBox txtprogress;
        private System.Windows.Forms.Timer tmrFade;
        private System.Windows.Forms.Panel panWin31;
        private System.Windows.Forms.Panel panStandard;
        private System.Windows.Forms.PictureBox panImage;
        private System.Windows.Forms.Button cmdCopy;
        private System.Windows.Forms.PictureBox PanelAbout;
        private System.Windows.Forms.CheckBox chkShowLoaded;
        private System.Windows.Forms.Panel rdash;
        private System.Windows.Forms.Panel ttoaster;
        private System.Windows.Forms.Panel panStandard2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panStandard3;
        private System.Windows.Forms.Panel panstandard4;
        private System.Windows.Forms.Panel panstandard5;
        private System.Windows.Forms.Panel pandetails;
        private System.Windows.Forms.TabControl tabDetails;
        private System.Windows.Forms.TabPage tabAssemblies;
        private System.Windows.Forms.ListView lvwAssemblies;
        private System.Windows.Forms.TabPage tabPlugins;
    }
}