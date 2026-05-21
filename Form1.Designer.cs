namespace TeamApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            파일ToolStripMenuItem = new ToolStripMenuItem();
            보기ToolStripMenuItem = new ToolStripMenuItem();
            도움말ToolStripMenuItem = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            btnOpenFolder = new Button();
            btnReload = new Button();
            btnToggleTheme = new Button();
            btnGuide = new Button();
            pbMainPreview = new PictureBox();
            lblFrameValue = new Label();
            lblAngleValue = new Label();
            lblThrottleValue = new Label();
            lblModeValue = new Label();
            comboBoxFilter = new ComboBox();
            txtFilter = new TextBox();
            numericUpDownInterval = new NumericUpDown();
            btnApplyFilter = new Button();
            listBoxData = new ListBox();
            btnFirst = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            btnLast = new Button();
            trackBarMain = new TrackBar();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelPath = new ToolStripStatusLabel();
            toolStripStatusLabelFrames = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbMainPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMain).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 파일ToolStripMenuItem, 보기ToolStripMenuItem, 도움말ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1025, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            파일ToolStripMenuItem.Size = new Size(53, 24);
            파일ToolStripMenuItem.Text = "파일";
            // 
            // 보기ToolStripMenuItem
            // 
            보기ToolStripMenuItem.Name = "보기ToolStripMenuItem";
            보기ToolStripMenuItem.Size = new Size(53, 24);
            보기ToolStripMenuItem.Text = "보기";
            // 
            // 도움말ToolStripMenuItem
            // 
            도움말ToolStripMenuItem.Name = "도움말ToolStripMenuItem";
            도움말ToolStripMenuItem.Size = new Size(68, 24);
            도움말ToolStripMenuItem.Text = "도움말";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(1, 31);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1024, 560);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnOpenFolder);
            tabPage1.Controls.Add(btnReload);
            tabPage1.Controls.Add(btnToggleTheme);
            tabPage1.Controls.Add(btnGuide);
            tabPage1.Controls.Add(pbMainPreview);
            tabPage1.Controls.Add(lblFrameValue);
            tabPage1.Controls.Add(lblAngleValue);
            tabPage1.Controls.Add(lblThrottleValue);
            tabPage1.Controls.Add(lblModeValue);
            tabPage1.Controls.Add(comboBoxFilter);
            tabPage1.Controls.Add(txtFilter);
            tabPage1.Controls.Add(numericUpDownInterval);
            tabPage1.Controls.Add(btnApplyFilter);
            tabPage1.Controls.Add(listBoxData);
            tabPage1.Controls.Add(btnFirst);
            tabPage1.Controls.Add(btnPrev);
            tabPage1.Controls.Add(btnNext);
            tabPage1.Controls.Add(btnLast);
            tabPage1.Controls.Add(trackBarMain);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1016, 527);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "데이터 뷰어";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Location = new Point(6, 6);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(140, 30);
            btnOpenFolder.TabIndex = 0;
            btnOpenFolder.Text = "데이터 폴더 열기";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // btnReload
            // 
            btnReload.Location = new Point(152, 6);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(140, 30);
            btnReload.TabIndex = 1;
            btnReload.Text = "다시 불러오기";
            btnReload.UseVisualStyleBackColor = true;
            btnReload.Click += btnReload_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Location = new Point(298, 6);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(140, 30);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "테마 전환";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // btnGuide
            // 
            btnGuide.Location = new Point(444, 6);
            btnGuide.Name = "btnGuide";
            btnGuide.Size = new Size(140, 30);
            btnGuide.TabIndex = 3;
            btnGuide.Text = "단계별 가이드";
            btnGuide.UseVisualStyleBackColor = true;
            btnGuide.Click += btnGuide_Click;
            // 
            // pbMainPreview
            // 
            pbMainPreview.BorderStyle = BorderStyle.FixedSingle;
            pbMainPreview.Location = new Point(6, 42);
            pbMainPreview.Name = "pbMainPreview";
            pbMainPreview.Size = new Size(680, 360);
            pbMainPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            pbMainPreview.TabIndex = 4;
            pbMainPreview.TabStop = false;
            // 
            // lblFrameValue
            // 
            lblFrameValue.Location = new Point(6, 408);
            lblFrameValue.Name = "lblFrameValue";
            lblFrameValue.Size = new Size(200, 20);
            lblFrameValue.TabIndex = 5;
            lblFrameValue.Text = "Frame: 0/0";
            // 
            // lblAngleValue
            // 
            lblAngleValue.Location = new Point(212, 408);
            lblAngleValue.Name = "lblAngleValue";
            lblAngleValue.Size = new Size(200, 20);
            lblAngleValue.TabIndex = 6;
            lblAngleValue.Text = "Angle: 0.000";
            // 
            // lblThrottleValue
            // 
            lblThrottleValue.Location = new Point(418, 408);
            lblThrottleValue.Name = "lblThrottleValue";
            lblThrottleValue.Size = new Size(200, 20);
            lblThrottleValue.TabIndex = 7;
            lblThrottleValue.Text = "Throttle: 0.000";
            // 
            // lblModeValue
            // 
            lblModeValue.Location = new Point(624, 408);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(200, 20);
            lblModeValue.TabIndex = 8;
            lblModeValue.Text = "Mode: -";
            // 
            // comboBoxFilter
            // 
            comboBoxFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFilter.Items.AddRange(new object[] { "Angle", "Throttle", "Mode" });
            comboBoxFilter.Location = new Point(700, 42);
            comboBoxFilter.Name = "comboBoxFilter";
            comboBoxFilter.Size = new Size(150, 28);
            comboBoxFilter.TabIndex = 9;
            // 
            // txtFilter
            // 
            txtFilter.Location = new Point(856, 42);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(148, 27);
            txtFilter.TabIndex = 10;
            txtFilter.Text = "min,max";
            // 
            // numericUpDownInterval
            // 
            numericUpDownInterval.Location = new Point(700, 76);
            numericUpDownInterval.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numericUpDownInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownInterval.Name = "numericUpDownInterval";
            numericUpDownInterval.Size = new Size(120, 27);
            numericUpDownInterval.TabIndex = 11;
            numericUpDownInterval.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Location = new Point(826, 76);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(178, 27);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // listBoxData
            // 
            listBoxData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            listBoxData.Location = new Point(700, 110);
            listBoxData.Name = "listBoxData";
            listBoxData.Size = new Size(304, 284);
            listBoxData.TabIndex = 13;
            listBoxData.SelectedIndexChanged += listBoxData_SelectedIndexChanged;
            // 
            // btnFirst
            // 
            btnFirst.Location = new Point(6, 468);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(75, 30);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "처음";
            btnFirst.Click += btnFirst_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new Point(87, 468);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(75, 30);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "이전";
            btnPrev.Click += btnPrev_Click;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(840, 469);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(75, 30);
            btnNext.TabIndex = 16;
            btnNext.Text = "다음";
            btnNext.Click += btnNext_Click;
            // 
            // btnLast
            // 
            btnLast.Location = new Point(921, 469);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(75, 30);
            btnLast.TabIndex = 17;
            btnLast.Text = "끝";
            btnLast.Click += btnLast_Click;
            // 
            // trackBarMain
            // 
            trackBarMain.Location = new Point(168, 468);
            trackBarMain.Maximum = 0;
            trackBarMain.Name = "trackBarMain";
            trackBarMain.Size = new Size(666, 56);
            trackBarMain.TabIndex = 18;
            trackBarMain.Scroll += trackBarMain_Scroll;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1016, 527);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "학습 실행";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1016, 527);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "그래프/통계";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelPath, toolStripStatusLabelFrames });
            statusStrip1.Location = new Point(0, 0);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(200, 22);
            statusStrip1.TabIndex = 0;
            // 
            // toolStripStatusLabelPath
            // 
            toolStripStatusLabelPath.Name = "toolStripStatusLabelPath";
            toolStripStatusLabelPath.Size = new Size(53, 16);
            toolStripStatusLabelPath.Text = "경로: -";
            // 
            // toolStripStatusLabelFrames
            // 
            toolStripStatusLabelFrames.Name = "toolStripStatusLabelFrames";
            toolStripStatusLabelFrames.Size = new Size(72, 16);
            toolStripStatusLabelFrames.Text = "Frames: 0";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1025, 610);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Data Manager";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbMainPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMain).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem 파일ToolStripMenuItem;
        private ToolStripMenuItem 보기ToolStripMenuItem;
        private ToolStripMenuItem 도움말ToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private PictureBox pictureBox1;
        private ListBox listBox1;
        private Label lblStatus;
        private TrackBar trackBar1;
        private Button btnStart;
        private Button btnStop;
        private CheckBox chkShowImage;
        private Button btnOpenFolder;
        private Button btnReload;
        private Button btnToggleTheme;
        private Button btnGuide;
        private PictureBox pbMainPreview;
        private Label lblFrameValue;
        private Label lblAngleValue;
        private Label lblThrottleValue;
        private Label lblModeValue;
        private ComboBox comboBoxFilter;
        private TextBox txtFilter;
        private NumericUpDown numericUpDownInterval;
        private Button btnApplyFilter;
        private ListBox listBoxData;
        private Button btnFirst;
        private Button btnPrev;
        private Button btnNext;
        private Button btnLast;
        private TrackBar trackBarMain;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelPath;
        private ToolStripStatusLabel toolStripStatusLabelFrames;
    }
}
