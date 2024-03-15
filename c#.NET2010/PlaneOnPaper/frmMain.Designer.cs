namespace PlaneOnPaper
{
    partial class frmMain
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
            this.lblTest = new System.Windows.Forms.Label();
            this.pnlBoard = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblPlaneCount = new System.Windows.Forms.Label();
            this.cmbPlaneCount = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnClearGhosts = new System.Windows.Forms.Button();
            this.chkEnemyPlanes = new System.Windows.Forms.CheckBox();
            this.btnRandEnemies = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAI = new System.Windows.Forms.CheckBox();
            this.chkMine = new System.Windows.Forms.CheckBox();
            this.chkPossibilities = new System.Windows.Forms.CheckBox();
            this.btnAI = new System.Windows.Forms.RadioButton();
            this.btnReady = new System.Windows.Forms.RadioButton();
            this.grpSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTest
            // 
            this.lblTest.AutoSize = true;
            this.lblTest.Location = new System.Drawing.Point(261, 254);
            this.lblTest.Name = "lblTest";
            this.lblTest.Size = new System.Drawing.Size(0, 13);
            this.lblTest.TabIndex = 1;
            // 
            // pnlBoard
            // 
            this.pnlBoard.BackColor = System.Drawing.SystemColors.Window;
            this.pnlBoard.BackgroundImage = global::PlaneOnPaper.Properties.Resources.board;
            this.pnlBoard.Location = new System.Drawing.Point(0, 0);
            this.pnlBoard.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBoard.MaximumSize = new System.Drawing.Size(251, 482);
            this.pnlBoard.MinimumSize = new System.Drawing.Size(251, 482);
            this.pnlBoard.Name = "pnlBoard";
            this.pnlBoard.Size = new System.Drawing.Size(251, 482);
            this.pnlBoard.TabIndex = 1;
            this.pnlBoard.TabStop = true;
            this.pnlBoard.MouseLeave += new System.EventHandler(this.pnlBoard_MouseLeave);
            this.pnlBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBoard_Paint);
            this.pnlBoard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlBoard_MouseMove);
            this.pnlBoard.Click += new System.EventHandler(this.pnlBoard_Click);
            this.pnlBoard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlBoard_MouseUp);
            this.pnlBoard.MouseEnter += new System.EventHandler(this.pnlBoard_MouseEnter);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(255, 447);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Цэвэрлэх";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(6, 72);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(81, 20);
            this.txtName.TabIndex = 4;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.lblName);
            this.grpSettings.Controls.Add(this.lblPlaneCount);
            this.grpSettings.Controls.Add(this.cmbPlaneCount);
            this.grpSettings.Controls.Add(this.txtName);
            this.grpSettings.Location = new System.Drawing.Point(255, 9);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(93, 98);
            this.grpSettings.TabIndex = 6;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Тохиргоо";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 56);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(27, 13);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "Нэр";
            // 
            // lblPlaneCount
            // 
            this.lblPlaneCount.AutoSize = true;
            this.lblPlaneCount.Location = new System.Drawing.Point(6, 16);
            this.lblPlaneCount.Name = "lblPlaneCount";
            this.lblPlaneCount.Size = new System.Drawing.Size(26, 13);
            this.lblPlaneCount.TabIndex = 1;
            this.lblPlaneCount.Text = "Тоо";
            // 
            // cmbPlaneCount
            // 
            this.cmbPlaneCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlaneCount.FormattingEnabled = true;
            this.cmbPlaneCount.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.cmbPlaneCount.Location = new System.Drawing.Point(6, 32);
            this.cmbPlaneCount.Name = "cmbPlaneCount";
            this.cmbPlaneCount.Size = new System.Drawing.Size(81, 21);
            this.cmbPlaneCount.TabIndex = 0;
            this.cmbPlaneCount.SelectedIndexChanged += new System.EventHandler(this.cmbPlaneCount_SelectedIndexChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(255, 151);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(0);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(93, 21);
            this.btnConnect.TabIndex = 7;
            this.btnConnect.Text = "холбогдох";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            this.btnConnect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            // 
            // btnClearGhosts
            // 
            this.btnClearGhosts.Location = new System.Drawing.Point(254, 270);
            this.btnClearGhosts.Name = "btnClearGhosts";
            this.btnClearGhosts.Size = new System.Drawing.Size(94, 23);
            this.btnClearGhosts.TabIndex = 9;
            this.btnClearGhosts.Text = "ghost цэвэрлэх";
            this.btnClearGhosts.UseVisualStyleBackColor = true;
            this.btnClearGhosts.Click += new System.EventHandler(this.btnClearGhosts_Click);
            this.btnClearGhosts.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            // 
            // chkEnemyPlanes
            // 
            this.chkEnemyPlanes.AutoSize = true;
            this.chkEnemyPlanes.Location = new System.Drawing.Point(261, 299);
            this.chkEnemyPlanes.Name = "chkEnemyPlanes";
            this.chkEnemyPlanes.Size = new System.Drawing.Size(59, 17);
            this.chkEnemyPlanes.TabIndex = 10;
            this.chkEnemyPlanes.Text = "харуул";
            this.chkEnemyPlanes.UseVisualStyleBackColor = true;
            // 
            // btnRandEnemies
            // 
            this.btnRandEnemies.Location = new System.Drawing.Point(263, 371);
            this.btnRandEnemies.Name = "btnRandEnemies";
            this.btnRandEnemies.Size = new System.Drawing.Size(75, 44);
            this.btnRandEnemies.TabIndex = 11;
            this.btnRandEnemies.Text = "random enemies";
            this.btnRandEnemies.UseVisualStyleBackColor = true;
            this.btnRandEnemies.Click += new System.EventHandler(this.btnRandEnemies_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAI);
            this.groupBox1.Controls.Add(this.chkMine);
            this.groupBox1.Location = new System.Drawing.Point(255, 171);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(93, 66);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "зурах онгоц";
            // 
            // chkAI
            // 
            this.chkAI.AutoSize = true;
            this.chkAI.Location = new System.Drawing.Point(6, 38);
            this.chkAI.Name = "chkAI";
            this.chkAI.Size = new System.Drawing.Size(65, 30);
            this.chkAI.TabIndex = 1;
            this.chkAI.Text = "Хиймэл\r\nоюун";
            this.chkAI.UseVisualStyleBackColor = true;
            // 
            // chkMine
            // 
            this.chkMine.AutoSize = true;
            this.chkMine.Checked = true;
            this.chkMine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMine.Location = new System.Drawing.Point(6, 19);
            this.chkMine.Name = "chkMine";
            this.chkMine.Size = new System.Drawing.Size(64, 17);
            this.chkMine.TabIndex = 0;
            this.chkMine.Text = "Минийх";
            this.chkMine.UseVisualStyleBackColor = true;
            // 
            // chkPossibilities
            // 
            this.chkPossibilities.AutoSize = true;
            this.chkPossibilities.Location = new System.Drawing.Point(261, 322);
            this.chkPossibilities.Name = "chkPossibilities";
            this.chkPossibilities.Size = new System.Drawing.Size(77, 17);
            this.chkPossibilities.TabIndex = 15;
            this.chkPossibilities.Text = "боломжит";
            this.chkPossibilities.UseVisualStyleBackColor = true;
            // 
            // btnAI
            // 
            this.btnAI.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnAI.Location = new System.Drawing.Point(255, 109);
            this.btnAI.Name = "btnAI";
            this.btnAI.Size = new System.Drawing.Size(93, 21);
            this.btnAI.TabIndex = 16;
            this.btnAI.TabStop = true;
            this.btnAI.Text = "AI";
            this.btnAI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnAI.UseVisualStyleBackColor = true;
            this.btnAI.CheckedChanged += new System.EventHandler(this.btnAI_CheckedChanged);
            // 
            // btnReady
            // 
            this.btnReady.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnReady.Location = new System.Drawing.Point(255, 130);
            this.btnReady.Name = "btnReady";
            this.btnReady.Size = new System.Drawing.Size(93, 21);
            this.btnReady.TabIndex = 17;
            this.btnReady.TabStop = true;
            this.btnReady.Text = "бэлэн";
            this.btnReady.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnReady.UseVisualStyleBackColor = true;
            this.btnReady.CheckedChanged += new System.EventHandler(this.btnReady_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 482);
            this.Controls.Add(this.btnReady);
            this.Controls.Add(this.btnAI);
            this.Controls.Add(this.chkPossibilities);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRandEnemies);
            this.Controls.Add(this.chkEnemyPlanes);
            this.Controls.Add(this.btnClearGhosts);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblTest);
            this.Controls.Add(this.pnlBoard);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(367, 520);
            this.MinimumSize = new System.Drawing.Size(367, 520);
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Онгоц";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBoard;
        private System.Windows.Forms.Label lblTest;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Label lblPlaneCount;
        private System.Windows.Forms.ComboBox cmbPlaneCount;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnClearGhosts;
        private System.Windows.Forms.CheckBox chkEnemyPlanes;
        private System.Windows.Forms.Button btnRandEnemies;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkPossibilities;
        private System.Windows.Forms.CheckBox chkAI;
        private System.Windows.Forms.CheckBox chkMine;
        private System.Windows.Forms.RadioButton btnAI;
        private System.Windows.Forms.RadioButton btnReady;
    }
}

