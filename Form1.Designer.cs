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
            데이터폴더열기ToolStripMenuItem = new ToolStripMenuItem();
            다시불러오기ToolStripMenuItem = new ToolStripMenuItem();
            종료ToolStripMenuItem = new ToolStripMenuItem();
            보기ToolStripMenuItem = new ToolStripMenuItem();
            테마전환ToolStripMenuItem = new ToolStripMenuItem();
            그래프테ToolStripMenuItem = new ToolStripMenuItem();
            도움말ToolStripMenuItem = new ToolStripMenuItem();
            단계별가이드ToolStripMenuItem = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            btnAutoPlay = new Button();
            btnClearFilter = new Button();
            listBoxData = new ListBox();
            btnOpenFolder = new Button();
            btnReload = new Button();
            btnApplyFilter = new Button();
            btnToggleTheme = new Button();
            numericUpDownInterval = new NumericUpDown();
            btnGuide = new Button();
            txtFilter = new TextBox();
            lblAngleValue = new Label();
            comboBoxFilter = new ComboBox();
            lblThrottleValue = new Label();
            lblModeValue = new Label();
            lblFrameValue = new Label();
            pbMainPreview = new PictureBox();
            btnFirst = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            btnLast = new Button();
            trackBarMain = new TrackBar();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            panel1 = new Panel();
            lblDescription = new Label();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelPath = new ToolStripStatusLabel();
            toolStripStatusLabelFrames = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbMainPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMain).BeginInit();
            tabPage3.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 파일ToolStripMenuItem, 보기ToolStripMenuItem, 도움말ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1582, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            파일ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 데이터폴더열기ToolStripMenuItem, 다시불러오기ToolStripMenuItem, 종료ToolStripMenuItem });
            파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            파일ToolStripMenuItem.Size = new Size(53, 24);
            파일ToolStripMenuItem.Text = "파일";
            // 
            // 데이터폴더열기ToolStripMenuItem
            // 
            데이터폴더열기ToolStripMenuItem.Name = "데이터폴더열기ToolStripMenuItem";
            데이터폴더열기ToolStripMenuItem.Size = new Size(207, 26);
            데이터폴더열기ToolStripMenuItem.Text = "데이터 폴더 열기";
            // 
            // 다시불러오기ToolStripMenuItem
            // 
            다시불러오기ToolStripMenuItem.Name = "다시불러오기ToolStripMenuItem";
            다시불러오기ToolStripMenuItem.Size = new Size(207, 26);
            다시불러오기ToolStripMenuItem.Text = "다시 불러오기";
            // 
            // 종료ToolStripMenuItem
            // 
            종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            종료ToolStripMenuItem.Size = new Size(207, 26);
            종료ToolStripMenuItem.Text = "종료";
            // 
            // 보기ToolStripMenuItem
            // 
            보기ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 테마전환ToolStripMenuItem, 그래프테ToolStripMenuItem });
            보기ToolStripMenuItem.Name = "보기ToolStripMenuItem";
            보기ToolStripMenuItem.Size = new Size(53, 24);
            보기ToolStripMenuItem.Text = "보기";
            // 
            // 테마전환ToolStripMenuItem
            // 
            테마전환ToolStripMenuItem.Name = "테마전환ToolStripMenuItem";
            테마전환ToolStripMenuItem.Size = new Size(258, 26);
            테마전환ToolStripMenuItem.Text = "테마 전환";
            // 
            // 그래프테ToolStripMenuItem
            // 
            그래프테ToolStripMenuItem.Name = "그래프테ToolStripMenuItem";
            그래프테ToolStripMenuItem.Size = new Size(258, 26);
            그래프테ToolStripMenuItem.Text = "그래프/통계 탭으로 이동";
            그래프테ToolStripMenuItem.Click += 그래프테ToolStripMenuItem_Click;
            // 
            // 도움말ToolStripMenuItem
            // 
            도움말ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 단계별가이드ToolStripMenuItem });
            도움말ToolStripMenuItem.Name = "도움말ToolStripMenuItem";
            도움말ToolStripMenuItem.Size = new Size(68, 24);
            도움말ToolStripMenuItem.Text = "도움말";
            도움말ToolStripMenuItem.Click += 도움말ToolStripMenuItem_Click;
            // 
            // 단계별가이드ToolStripMenuItem
            // 
            단계별가이드ToolStripMenuItem.Name = "단계별가이드ToolStripMenuItem";
            단계별가이드ToolStripMenuItem.Size = new Size(187, 26);
            단계별가이드ToolStripMenuItem.Text = "단계별 가이드";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(2, 31);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1580, 920);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnAutoPlay);
            tabPage1.Controls.Add(btnClearFilter);
            tabPage1.Controls.Add(listBoxData);
            tabPage1.Controls.Add(btnOpenFolder);
            tabPage1.Controls.Add(btnReload);
            tabPage1.Controls.Add(btnApplyFilter);
            tabPage1.Controls.Add(btnToggleTheme);
            tabPage1.Controls.Add(numericUpDownInterval);
            tabPage1.Controls.Add(btnGuide);
            tabPage1.Controls.Add(txtFilter);
            tabPage1.Controls.Add(lblAngleValue);
            tabPage1.Controls.Add(comboBoxFilter);
            tabPage1.Controls.Add(lblThrottleValue);
            tabPage1.Controls.Add(lblModeValue);
            tabPage1.Controls.Add(lblFrameValue);
            tabPage1.Controls.Add(pbMainPreview);
            tabPage1.Controls.Add(btnFirst);
            tabPage1.Controls.Add(btnPrev);
            tabPage1.Controls.Add(btnNext);
            tabPage1.Controls.Add(btnLast);
            tabPage1.Controls.Add(trackBarMain);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1572, 887);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "데이터 뷰어";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.Location = new Point(473, 832);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(150, 27);
            btnAutoPlay.TabIndex = 20;
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;
            btnAutoPlay.Click += button3_Click;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Location = new Point(230, 793);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(150, 27);
            btnClearFilter.TabIndex = 19;
            btnClearFilter.Text = "필터 해제";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // listBoxData
            // 
            listBoxData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            listBoxData.Location = new Point(7, 42);
            listBoxData.Name = "listBoxData";
            listBoxData.Size = new Size(281, 604);
            listBoxData.TabIndex = 13;
            listBoxData.SelectedIndexChanged += listBoxData_SelectedIndexChanged;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Location = new Point(215, 6);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(140, 30);
            btnOpenFolder.TabIndex = 0;
            btnOpenFolder.Text = "데이터 폴더 열기";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // btnReload
            // 
            btnReload.Location = new Point(361, 6);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(140, 30);
            btnReload.TabIndex = 1;
            btnReload.Text = "다시 불러오기";
            btnReload.UseVisualStyleBackColor = true;
            btnReload.Click += btnReload_Click;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Location = new Point(76, 793);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(150, 27);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Location = new Point(507, 6);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(140, 30);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "테마 전환";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // numericUpDownInterval
            // 
            numericUpDownInterval.Location = new Point(230, 759);
            numericUpDownInterval.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numericUpDownInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownInterval.Name = "numericUpDownInterval";
            numericUpDownInterval.Size = new Size(150, 27);
            numericUpDownInterval.TabIndex = 11;
            numericUpDownInterval.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // btnGuide
            // 
            btnGuide.Location = new Point(653, 6);
            btnGuide.Name = "btnGuide";
            btnGuide.Size = new Size(140, 30);
            btnGuide.TabIndex = 3;
            btnGuide.Text = "단계별 가이드";
            btnGuide.UseVisualStyleBackColor = true;
            btnGuide.Click += btnGuide_Click;
            // 
            // txtFilter
            // 
            txtFilter.Location = new Point(78, 759);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(148, 27);
            txtFilter.TabIndex = 10;
            txtFilter.Text = "min,max";
            // 
            // lblAngleValue
            // 
            lblAngleValue.Location = new Point(1292, 436);
            lblAngleValue.Name = "lblAngleValue";
            lblAngleValue.Size = new Size(164, 20);
            lblAngleValue.TabIndex = 6;
            lblAngleValue.Text = "Angle: 0.000";
            // 
            // comboBoxFilter
            // 
            comboBoxFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFilter.Items.AddRange(new object[] { "전체 보기", "스로틀 최소값", "조향 범위", "모드 = 사용자", "이미지 없는 프레임" });
            comboBoxFilter.Location = new Point(76, 726);
            comboBoxFilter.Name = "comboBoxFilter";
            comboBoxFilter.Size = new Size(154, 28);
            comboBoxFilter.TabIndex = 9;
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;
            // 
            // lblThrottleValue
            // 
            lblThrottleValue.Location = new Point(1292, 456);
            lblThrottleValue.Name = "lblThrottleValue";
            lblThrottleValue.Size = new Size(158, 20);
            lblThrottleValue.TabIndex = 7;
            lblThrottleValue.Text = "Throttle: 0.000";
            // 
            // lblModeValue
            // 
            lblModeValue.Location = new Point(1294, 479);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(156, 20);
            lblModeValue.TabIndex = 8;
            lblModeValue.Text = "Mode: -";
            // 
            // lblFrameValue
            // 
            lblFrameValue.Font = new Font("맑은 고딕", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblFrameValue.Location = new Point(1292, 42);
            lblFrameValue.Name = "lblFrameValue";
            lblFrameValue.Size = new Size(277, 39);
            lblFrameValue.TabIndex = 5;
            lblFrameValue.Text = "Frame: 0/0";
            // 
            // pbMainPreview
            // 
            pbMainPreview.BorderStyle = BorderStyle.FixedSingle;
            pbMainPreview.Location = new Point(294, 42);
            pbMainPreview.Name = "pbMainPreview";
            pbMainPreview.Size = new Size(992, 605);
            pbMainPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            pbMainPreview.TabIndex = 4;
            pbMainPreview.TabStop = false;
            // 
            // btnFirst
            // 
            btnFirst.Location = new Point(1292, 120);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(124, 30);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "처음";
            btnFirst.Click += btnFirst_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new Point(1292, 84);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(124, 30);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "이전";
            btnPrev.Click += btnPrev_Click;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(1427, 84);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(124, 30);
            btnNext.TabIndex = 16;
            btnNext.Text = "다음";
            btnNext.Click += btnNext_Click;
            // 
            // btnLast
            // 
            btnLast.Location = new Point(1427, 120);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(124, 30);
            btnLast.TabIndex = 17;
            btnLast.Text = "끝";
            btnLast.Click += btnLast_Click;
            // 
            // trackBarMain
            // 
            trackBarMain.Location = new Point(3, 653);
            trackBarMain.Maximum = 0;
            trackBarMain.Name = "trackBarMain";
            trackBarMain.Size = new Size(1563, 56);
            trackBarMain.TabIndex = 18;
            trackBarMain.Scroll += trackBarMain_Scroll;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1572, 887);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "학습 실행";
            tabPage2.UseVisualStyleBackColor = true;
            tabPage2.Click += tabPage2_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panel1);
            tabPage3.Controls.Add(lblDescription);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1572, 887);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "그래프/통계";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Location = new Point(73, 75);
            panel1.Name = "panel1";
            panel1.Size = new Size(921, 408);
            panel1.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(73, 27);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(921, 20);
            lblDescription.TabIndex = 0;
            lblDescription.Text = "조향값과 스로틀값 변화 추이를 시각화합니다. 파란색은 조향, 초록색은 스로틀입니다. 필터 적용 후 보이는 데이터 기준으로 갱신됩니다.";
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
            ClientSize = new Size(1582, 953);
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
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbMainPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMain).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
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
        private Button btnClearFilter;
        private Label lblDescription;
        private Panel panel1;
        private ToolStripMenuItem 테마전환ToolStripMenuItem;
        private ToolStripMenuItem 그래프테ToolStripMenuItem;
        private ToolStripMenuItem 단계별가이드ToolStripMenuItem;
        private ToolStripMenuItem 데이터폴더열기ToolStripMenuItem;
        private ToolStripMenuItem 다시불러오기ToolStripMenuItem;
        private ToolStripMenuItem 종료ToolStripMenuItem;
        private Button btnAutoPlay;
    }
}
