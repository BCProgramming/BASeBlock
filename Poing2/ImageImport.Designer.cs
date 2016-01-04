namespace BASeCamp.BASeBlock
{
    partial class ImageImport
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.UDWidth = new System.Windows.Forms.NumericUpDown();
            this.UDHeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdApply = new System.Windows.Forms.Button();
            this.cmdClose = new System.Windows.Forms.Button();
            this.UDCYPreScale = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.udcXPrescale = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.fraClipBlock = new System.Windows.Forms.GroupBox();
            this.UDCClipY = new System.Windows.Forms.NumericUpDown();
            this.UDCClipX = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chkImageClip = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.UDWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDCYPreScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcXPrescale)).BeginInit();
            this.fraClipBlock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDCClipY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDCClipX)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filename:";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(55, 5);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(248, 20);
            this.txtFilename.TabIndex = 1;
            this.txtFilename.TextChanged += new System.EventHandler(this.txtFilename_TextChanged);
            this.txtFilename.Validating += new System.ComponentModel.CancelEventHandler(this.txtFilename_Validating);
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(309, 5);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowse.TabIndex = 2;
            this.cmdBrowse.Text = "&Browse...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(167, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Block Width:";
            // 
            // UDWidth
            // 
            this.UDWidth.Location = new System.Drawing.Point(241, 28);
            this.UDWidth.Name = "UDWidth";
            this.UDWidth.Size = new System.Drawing.Size(62, 20);
            this.UDWidth.TabIndex = 4;
            // 
            // UDHeight
            // 
            this.UDHeight.Location = new System.Drawing.Point(241, 54);
            this.UDHeight.Name = "UDHeight";
            this.UDHeight.Size = new System.Drawing.Size(62, 20);
            this.UDHeight.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(167, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Block Height:";
            // 
            // cmdApply
            // 
            this.cmdApply.Location = new System.Drawing.Point(228, 167);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(75, 23);
            this.cmdApply.TabIndex = 7;
            this.cmdApply.Text = "&Apply";
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(309, 167);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 8;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // UDCYPreScale
            // 
            this.UDCYPreScale.DecimalPlaces = 2;
            this.UDCYPreScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.UDCYPreScale.Location = new System.Drawing.Point(81, 54);
            this.UDCYPreScale.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.UDCYPreScale.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.UDCYPreScale.Name = "UDCYPreScale";
            this.UDCYPreScale.Size = new System.Drawing.Size(62, 20);
            this.UDCYPreScale.TabIndex = 12;
            this.UDCYPreScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Y Prescale:";
            // 
            // udcXPrescale
            // 
            this.udcXPrescale.DecimalPlaces = 2;
            this.udcXPrescale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.udcXPrescale.Location = new System.Drawing.Point(81, 28);
            this.udcXPrescale.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udcXPrescale.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.udcXPrescale.Name = "udcXPrescale";
            this.udcXPrescale.Size = new System.Drawing.Size(62, 20);
            this.udcXPrescale.TabIndex = 10;
            this.udcXPrescale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "X Prescale:";
            // 
            // fraClipBlock
            // 
            this.fraClipBlock.Controls.Add(this.chkImageClip);
            this.fraClipBlock.Controls.Add(this.label7);
            this.fraClipBlock.Controls.Add(this.label6);
            this.fraClipBlock.Controls.Add(this.UDCClipY);
            this.fraClipBlock.Controls.Add(this.UDCClipX);
            this.fraClipBlock.Location = new System.Drawing.Point(15, 80);
            this.fraClipBlock.Name = "fraClipBlock";
            this.fraClipBlock.Size = new System.Drawing.Size(368, 78);
            this.fraClipBlock.TabIndex = 13;
            this.fraClipBlock.TabStop = false;
            // 
            // UDCClipY
            // 
            this.UDCClipY.Enabled = false;
            this.UDCClipY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.UDCClipY.Location = new System.Drawing.Point(83, 45);
            this.UDCClipY.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.UDCClipY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UDCClipY.Name = "UDCClipY";
            this.UDCClipY.Size = new System.Drawing.Size(62, 20);
            this.UDCClipY.TabIndex = 18;
            this.UDCClipY.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // UDCClipX
            // 
            this.UDCClipX.Enabled = false;
            this.UDCClipX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.UDCClipX.Location = new System.Drawing.Point(83, 19);
            this.UDCClipX.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.UDCClipX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UDCClipX.Name = "UDCClipX";
            this.UDCClipX.Size = new System.Drawing.Size(62, 20);
            this.UDCClipX.TabIndex = 17;
            this.UDCClipX.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Clip Width:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Clip Height:";
            // 
            // chkImageClip
            // 
            this.chkImageClip.AutoSize = true;
            this.chkImageClip.Location = new System.Drawing.Point(6, 2);
            this.chkImageClip.Name = "chkImageClip";
            this.chkImageClip.Size = new System.Drawing.Size(72, 17);
            this.chkImageClip.TabIndex = 15;
            this.chkImageClip.Text = "ImageClip";
            this.chkImageClip.UseVisualStyleBackColor = true;
            this.chkImageClip.CheckedChanged += new System.EventHandler(this.chkImageClip_CheckedChanged);
            // 
            // ImageImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 202);
            this.Controls.Add(this.fraClipBlock);
            this.Controls.Add(this.UDCYPreScale);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.udcXPrescale);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.UDHeight);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.UDWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdBrowse);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageImport";
            this.Text = "ImageImport";
            ((System.ComponentModel.ISupportInitialize)(this.UDWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDCYPreScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcXPrescale)).EndInit();
            this.fraClipBlock.ResumeLayout(false);
            this.fraClipBlock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDCClipY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDCClipX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown UDWidth;
        private System.Windows.Forms.NumericUpDown UDHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.NumericUpDown UDCYPreScale;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown udcXPrescale;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox fraClipBlock;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown UDCClipY;
        private System.Windows.Forms.NumericUpDown UDCClipX;
        private System.Windows.Forms.CheckBox chkImageClip;
    }
}