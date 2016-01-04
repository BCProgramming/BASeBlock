namespace BASeCamp.BASeBlock
{
    partial class BallBehave
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboAvailableBehaviours = new System.Windows.Forms.ComboBox();
            this.fraApplied = new System.Windows.Forms.GroupBox();
            this.lvwApplied = new System.Windows.Forms.ListView();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.PropertyApplied = new BASeBlock.PropertyGridCustom();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.fraApplied.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboAvailableBehaviours);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 52);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Available Behaviours";
            // 
            // cboAvailableBehaviours
            // 
            this.cboAvailableBehaviours.FormattingEnabled = true;
            this.cboAvailableBehaviours.Location = new System.Drawing.Point(8, 18);
            this.cboAvailableBehaviours.Name = "cboAvailableBehaviours";
            this.cboAvailableBehaviours.Size = new System.Drawing.Size(215, 21);
            this.cboAvailableBehaviours.TabIndex = 0;
            this.cboAvailableBehaviours.SelectionChangeCommitted += new System.EventHandler(this.cboAvailableBehaviours_SelectionChangeCommitted);
            this.cboAvailableBehaviours.SelectedIndexChanged += new System.EventHandler(this.cboAvailableBehaviours_SelectedIndexChanged);
            // 
            // fraApplied
            // 
            this.fraApplied.Controls.Add(this.lvwApplied);
            this.fraApplied.Location = new System.Drawing.Point(4, 95);
            this.fraApplied.Name = "fraApplied";
            this.fraApplied.Size = new System.Drawing.Size(247, 241);
            this.fraApplied.TabIndex = 1;
            this.fraApplied.TabStop = false;
            this.fraApplied.Text = "Applied Behaviours";
            // 
            // lvwApplied
            // 
            this.lvwApplied.Location = new System.Drawing.Point(6, 20);
            this.lvwApplied.MultiSelect = false;
            this.lvwApplied.Name = "lvwApplied";
            this.lvwApplied.Size = new System.Drawing.Size(231, 219);
            this.lvwApplied.TabIndex = 0;
            this.lvwApplied.UseCompatibleStateImageBehavior = false;
            this.lvwApplied.View = System.Windows.Forms.View.List;
            this.lvwApplied.SelectedIndexChanged += new System.EventHandler(this.lvwApplied_SelectedIndexChanged);
            this.lvwApplied.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwApplied_ItemSelectionChanged);
            // 
            // cmdAdd
            // 
            this.cmdAdd.Location = new System.Drawing.Point(9, 63);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(65, 26);
            this.cmdAdd.TabIndex = 2;
            this.cmdAdd.Text = "Add";
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // cmdRemove
            // 
            this.cmdRemove.Location = new System.Drawing.Point(176, 63);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(65, 26);
            this.cmdRemove.TabIndex = 3;
            this.cmdRemove.Text = "Remove";
            this.cmdRemove.UseVisualStyleBackColor = false;
            this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
            // 
            // PropertyApplied
            // 
            this.PropertyApplied.Location = new System.Drawing.Point(258, 3);
            this.PropertyApplied.Name = "PropertyApplied";
            this.PropertyApplied.Size = new System.Drawing.Size(267, 299);
            this.PropertyApplied.TabIndex = 4;
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(442, 309);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(79, 26);
            this.cmdOK.TabIndex = 5;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // BallBehave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 343);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.PropertyApplied);
            this.Controls.Add(this.cmdRemove);
            this.Controls.Add(this.cmdAdd);
            this.Controls.Add(this.fraApplied);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BallBehave";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ball Behaviours";
            this.Load += new System.EventHandler(this.BallBehave_Load);
            this.groupBox1.ResumeLayout(false);
            this.fraApplied.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboAvailableBehaviours;
        private System.Windows.Forms.GroupBox fraApplied;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.Button cmdRemove;
        private PropertyGridCustom PropertyApplied;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.ListView lvwApplied;

    }
}