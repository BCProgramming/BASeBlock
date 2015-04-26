namespace BASeBlock
{
    partial class frmEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditor));
            this.PicEditor = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.levelSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.behavioursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolBar = new System.Windows.Forms.ToolStrip();
            this.DropDownAddBlocks = new System.Windows.Forms.ToolStripDropDownButton();
            this.ghostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddBall = new System.Windows.Forms.ToolStripButton();
            this.tabSidebar = new System.Windows.Forms.TabControl();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.propgridblock = new System.Windows.Forms.PropertyGrid();
            this.tabBlocks = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.UpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.UpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.GenBlockNumber = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.imlTabPics = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PicEditor)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.ToolBar.SuspendLayout();
            this.tabSidebar.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.tabBlocks.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GenBlockNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // PicEditor
            // 
            this.PicEditor.Location = new System.Drawing.Point(0, 65);
            this.PicEditor.Name = "PicEditor";
            this.PicEditor.Size = new System.Drawing.Size(493, 427);
            this.PicEditor.TabIndex = 0;
            this.PicEditor.TabStop = false;
            this.PicEditor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PicEditor_MouseMove);
            this.PicEditor.Click += new System.EventHandler(this.PicEditor_Click);
            this.PicEditor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PicEditor_MouseDown);
            this.PicEditor.Paint += new System.Windows.Forms.PaintEventHandler(this.PicEditor_Paint);
            this.PicEditor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PicEditor_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.editToolStripMenuItem1,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(128, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem1
            // 
            this.editToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.levelSetToolStripMenuItem,
            this.levelToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.blocksToolStripMenuItem,
            this.behavioursToolStripMenuItem});
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem1.Text = "Edit";
            // 
            // levelSetToolStripMenuItem
            // 
            this.levelSetToolStripMenuItem.Name = "levelSetToolStripMenuItem";
            this.levelSetToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.levelSetToolStripMenuItem.Text = "LevelSet";
            // 
            // levelToolStripMenuItem
            // 
            this.levelToolStripMenuItem.Name = "levelToolStripMenuItem";
            this.levelToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.levelToolStripMenuItem.Text = "Level";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // blocksToolStripMenuItem
            // 
            this.blocksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.blocksToolStripMenuItem.Name = "blocksToolStripMenuItem";
            this.blocksToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.blocksToolStripMenuItem.Text = "Blocks";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // behavioursToolStripMenuItem
            // 
            this.behavioursToolStripMenuItem.Name = "behavioursToolStripMenuItem";
            this.behavioursToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.behavioursToolStripMenuItem.Text = "Behaviours";
            this.behavioursToolStripMenuItem.Click += new System.EventHandler(this.behavioursToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gridToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // gridToolStripMenuItem
            // 
            this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
            this.gridToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.gridToolStripMenuItem.Text = "Grid";
            this.gridToolStripMenuItem.Click += new System.EventHandler(this.gridToolStripMenuItem_Click);
            // 
            // ToolBar
            // 
            this.ToolBar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ToolBar.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DropDownAddBlocks,
            this.AddBall});
            this.ToolBar.Location = new System.Drawing.Point(3, 30);
            this.ToolBar.Name = "ToolBar";
            this.ToolBar.Size = new System.Drawing.Size(141, 25);
            this.ToolBar.TabIndex = 4;
            this.ToolBar.Text = "toolStrip1";
            // 
            // DropDownAddBlocks
            // 
            this.DropDownAddBlocks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.DropDownAddBlocks.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ghostToolStripMenuItem});
            this.DropDownAddBlocks.Image = ((System.Drawing.Image)(resources.GetObject("DropDownAddBlocks.Image")));
            this.DropDownAddBlocks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DropDownAddBlocks.Name = "DropDownAddBlocks";
            this.DropDownAddBlocks.Size = new System.Drawing.Size(74, 22);
            this.DropDownAddBlocks.Text = "Add Block";
            this.DropDownAddBlocks.DropDownOpening += new System.EventHandler(this.DropDownAddBlocks_DropDownOpening);
            // 
            // ghostToolStripMenuItem
            // 
            this.ghostToolStripMenuItem.Name = "ghostToolStripMenuItem";
            this.ghostToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.ghostToolStripMenuItem.Text = "ghost";
            // 
            // AddBall
            // 
            this.AddBall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.AddBall.Image = ((System.Drawing.Image)(resources.GetObject("AddBall.Image")));
            this.AddBall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddBall.Name = "AddBall";
            this.AddBall.Size = new System.Drawing.Size(55, 22);
            this.AddBall.Text = "Add Ball";
            this.AddBall.Click += new System.EventHandler(this.AddBall_Click);
            // 
            // tabSidebar
            // 
            this.tabSidebar.Controls.Add(this.tabProperties);
            this.tabSidebar.Controls.Add(this.tabBlocks);
            this.tabSidebar.ImageList = this.imlTabPics;
            this.tabSidebar.Location = new System.Drawing.Point(499, 30);
            this.tabSidebar.Name = "tabSidebar";
            this.tabSidebar.SelectedIndex = 0;
            this.tabSidebar.Size = new System.Drawing.Size(236, 462);
            this.tabSidebar.TabIndex = 6;
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.propgridblock);
            this.tabProperties.ImageIndex = 0;
            this.tabProperties.Location = new System.Drawing.Point(4, 23);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabProperties.Size = new System.Drawing.Size(228, 435);
            this.tabProperties.TabIndex = 0;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // propgridblock
            // 
            this.propgridblock.Location = new System.Drawing.Point(7, 2);
            this.propgridblock.Name = "propgridblock";
            this.propgridblock.Size = new System.Drawing.Size(218, 401);
            this.propgridblock.TabIndex = 6;
            // 
            // tabBlocks
            // 
            this.tabBlocks.Controls.Add(this.groupBox1);
            this.tabBlocks.ImageIndex = 1;
            this.tabBlocks.Location = new System.Drawing.Point(4, 23);
            this.tabBlocks.Name = "tabBlocks";
            this.tabBlocks.Padding = new System.Windows.Forms.Padding(3);
            this.tabBlocks.Size = new System.Drawing.Size(228, 435);
            this.tabBlocks.TabIndex = 1;
            this.tabBlocks.Text = "Blocks";
            this.tabBlocks.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.UpDownWidth);
            this.groupBox1.Controls.Add(this.UpDownHeight);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnGenerate);
            this.groupBox1.Controls.Add(this.GenBlockNumber);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 229);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Generate ";
            // 
            // UpDownWidth
            // 
            this.UpDownWidth.Location = new System.Drawing.Point(63, 72);
            this.UpDownWidth.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.UpDownWidth.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.UpDownWidth.Name = "UpDownWidth";
            this.UpDownWidth.Size = new System.Drawing.Size(49, 20);
            this.UpDownWidth.TabIndex = 9;
            this.UpDownWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // UpDownHeight
            // 
            this.UpDownHeight.Location = new System.Drawing.Point(63, 104);
            this.UpDownHeight.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.UpDownHeight.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.UpDownHeight.Name = "UpDownHeight";
            this.UpDownHeight.Size = new System.Drawing.Size(49, 20);
            this.UpDownHeight.TabIndex = 8;
            this.UpDownHeight.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Height:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Width:";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(60, 169);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(87, 27);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // GenBlockNumber
            // 
            this.GenBlockNumber.Location = new System.Drawing.Point(63, 134);
            this.GenBlockNumber.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.GenBlockNumber.Name = "GenBlockNumber";
            this.GenBlockNumber.Size = new System.Drawing.Size(50, 20);
            this.GenBlockNumber.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Number:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Block Type:";
            // 
            // imlTabPics
            // 
            this.imlTabPics.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlTabPics.ImageStream")));
            this.imlTabPics.TransparentColor = System.Drawing.Color.Transparent;
            this.imlTabPics.Images.SetKeyName(0, "property.png");
            this.imlTabPics.Images.SetKeyName(1, "miniblock.png");
            // 
            // frmEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 492);
            this.Controls.Add(this.tabSidebar);
            this.Controls.Add(this.ToolBar);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.PicEditor);
            this.KeyPreview = true;
            this.Name = "frmEditor";
            this.Text = "BASeBlock Editor - Untitled";
            this.Load += new System.EventHandler(this.frmEditor_Load);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmEditor_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.PicEditor)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ToolBar.ResumeLayout(false);
            this.ToolBar.PerformLayout();
            this.tabSidebar.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.tabBlocks.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GenBlockNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();


        #endregion

        private System.Windows.Forms.PictureBox PicEditor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem levelSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem levelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blocksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStrip ToolBar;
        private System.Windows.Forms.ToolStripDropDownButton DropDownAddBlocks;
        private System.Windows.Forms.ToolStripMenuItem ghostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem behavioursToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
        private System.Windows.Forms.TabControl tabSidebar;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.PropertyGrid propgridblock;
        private System.Windows.Forms.TabPage tabBlocks;
        private System.Windows.Forms.ToolStripButton AddBall;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown GenBlockNumber;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown UpDownWidth;
        private System.Windows.Forms.NumericUpDown UpDownHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ImageList imlTabPics;
    }
}