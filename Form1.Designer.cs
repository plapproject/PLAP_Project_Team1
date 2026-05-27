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
            groupBox2 = new GroupBox();
            btnRepair = new Button();
            btnReloadTub = new Button();
            textBox1 = new TextBox();
            numFilterMax = new NumericUpDown();
            label3 = new Label();
            label2 = new Label();
            numFilterMin = new NumericUpDown();
            btnApplyFilter = new Button();
            btnClearFilter = new Button();
            groupBox1 = new GroupBox();
            trackBarMain = new TrackBar();
            numericUpDownInterval = new NumericUpDown();
            btnOpenFolder = new Button();
            label1 = new Label();
            btnLast = new Button();
            btnAutoPlay = new Button();
            btnNext = new Button();
            btnPrev = new Button();
            listBoxData = new ListBox();
            btnFirst = new Button();
            pbMainPreview = new PictureBox();
            btnReload = new Button();
            lblFrameValue = new Label();
            lblModeValue = new Label();
            btnToggleTheme = new Button();
            lblThrottleValue = new Label();
            btnGuide = new Button();
            comboBoxFilter = new ComboBox();
            lblAngleValue = new Label();
            tabPage2 = new TabPage();
            groupBox3 = new GroupBox();
            tabPage3 = new TabPage();
            panel1 = new Panel();
            lblDescription = new Label();
            tabPage4 = new TabPage();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelPath = new ToolStripStatusLabel();
            toolStripStatusLabelFrames = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numFilterMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFilterMin).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbMainPreview).BeginInit();
            tabPage2.SuspendLayout();
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
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1230, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            파일ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 데이터폴더열기ToolStripMenuItem, 다시불러오기ToolStripMenuItem, 종료ToolStripMenuItem });
            파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            파일ToolStripMenuItem.Size = new Size(43, 20);
            파일ToolStripMenuItem.Text = "파일";
            // 
            // 데이터폴더열기ToolStripMenuItem
            // 
            데이터폴더열기ToolStripMenuItem.Name = "데이터폴더열기ToolStripMenuItem";
            데이터폴더열기ToolStripMenuItem.Size = new Size(166, 22);
            데이터폴더열기ToolStripMenuItem.Text = "데이터 폴더 열기";
            // 
            // 다시불러오기ToolStripMenuItem
            // 
            다시불러오기ToolStripMenuItem.Name = "다시불러오기ToolStripMenuItem";
            다시불러오기ToolStripMenuItem.Size = new Size(166, 22);
            다시불러오기ToolStripMenuItem.Text = "다시 불러오기";
            // 
            // 종료ToolStripMenuItem
            // 
            종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            종료ToolStripMenuItem.Size = new Size(166, 22);
            종료ToolStripMenuItem.Text = "종료";
            // 
            // 보기ToolStripMenuItem
            // 
            보기ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 테마전환ToolStripMenuItem, 그래프테ToolStripMenuItem });
            보기ToolStripMenuItem.Name = "보기ToolStripMenuItem";
            보기ToolStripMenuItem.Size = new Size(43, 20);
            보기ToolStripMenuItem.Text = "보기";
            // 
            // 테마전환ToolStripMenuItem
            // 
            테마전환ToolStripMenuItem.Name = "테마전환ToolStripMenuItem";
            테마전환ToolStripMenuItem.Size = new Size(207, 22);
            테마전환ToolStripMenuItem.Text = "테마 전환";
            // 
            // 그래프테ToolStripMenuItem
            // 
            그래프테ToolStripMenuItem.Name = "그래프테ToolStripMenuItem";
            그래프테ToolStripMenuItem.Size = new Size(207, 22);
            그래프테ToolStripMenuItem.Text = "그래프/통계 탭으로 이동";
            그래프테ToolStripMenuItem.Click += 그래프테ToolStripMenuItem_Click;
            // 
            // 도움말ToolStripMenuItem
            // 
            도움말ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 단계별가이드ToolStripMenuItem });
            도움말ToolStripMenuItem.Name = "도움말ToolStripMenuItem";
            도움말ToolStripMenuItem.Size = new Size(55, 20);
            도움말ToolStripMenuItem.Text = "도움말";
            도움말ToolStripMenuItem.Click += 도움말ToolStripMenuItem_Click;
            // 
            // 단계별가이드ToolStripMenuItem
            // 
            단계별가이드ToolStripMenuItem.Name = "단계별가이드ToolStripMenuItem";
            단계별가이드ToolStripMenuItem.Size = new Size(150, 22);
            단계별가이드ToolStripMenuItem.Text = "단계별 가이드";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Location = new Point(2, 23);
            tabControl1.Margin = new Padding(2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1229, 690);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Margin = new Padding(2);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(2);
            tabPage1.Size = new Size(1221, 662);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "데이터 뷰어";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnRepair);
            groupBox2.Controls.Add(btnReloadTub);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Controls.Add(numFilterMax);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(numFilterMin);
            groupBox2.Controls.Add(btnApplyFilter);
            groupBox2.Controls.Add(btnClearFilter);
            groupBox2.Location = new Point(2, 556);
            groupBox2.Margin = new Padding(2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(2);
            groupBox2.Size = new Size(1216, 51);
            groupBox2.TabIndex = 23;
            groupBox2.TabStop = false;
            groupBox2.Text = "터브 정리기";
            // 
            // btnRepair
            // 
            btnRepair.Location = new Point(1017, 19);
            btnRepair.Margin = new Padding(2);
            btnRepair.Name = "btnRepair";
            btnRepair.Size = new Size(93, 22);
            btnRepair.TabIndex = 28;
            btnRepair.Text = "복원";
            btnRepair.UseVisualStyleBackColor = true;
            // 
            // btnReloadTub
            // 
            btnReloadTub.Location = new Point(1116, 19);
            btnReloadTub.Margin = new Padding(2);
            btnReloadTub.Name = "btnReloadTub";
            btnReloadTub.Size = new Size(93, 22);
            btnReloadTub.TabIndex = 29;
            btnReloadTub.Text = "터브 다시 로드";
            btnReloadTub.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(370, 19);
            textBox1.Margin = new Padding(2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(447, 23);
            textBox1.TabIndex = 27;
            // 
            // numFilterMax
            // 
            numFilterMax.Location = new Point(249, 19);
            numFilterMax.Margin = new Padding(2);
            numFilterMax.Name = "numFilterMax";
            numFilterMax.Size = new Size(117, 23);
            numFilterMax.TabIndex = 25;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 10F);
            label3.Location = new Point(197, 20);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(51, 19);
            label3.TabIndex = 26;
            label3.Text = "최댓값";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 10F);
            label2.Location = new Point(12, 20);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(51, 19);
            label2.TabIndex = 25;
            label2.Text = "최솟값";
            // 
            // numFilterMin
            // 
            numFilterMin.Location = new Point(64, 20);
            numFilterMin.Margin = new Padding(2);
            numFilterMin.Name = "numFilterMin";
            numFilterMin.Size = new Size(117, 23);
            numFilterMin.TabIndex = 24;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Location = new Point(821, 18);
            btnApplyFilter.Margin = new Padding(2);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(93, 22);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Location = new Point(919, 18);
            btnClearFilter.Margin = new Padding(2);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(93, 22);
            btnClearFilter.TabIndex = 19;
            btnClearFilter.Text = "필터 해제";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(trackBarMain);
            groupBox1.Controls.Add(numericUpDownInterval);
            groupBox1.Controls.Add(btnOpenFolder);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(btnLast);
            groupBox1.Controls.Add(btnAutoPlay);
            groupBox1.Controls.Add(btnNext);
            groupBox1.Controls.Add(btnPrev);
            groupBox1.Controls.Add(listBoxData);
            groupBox1.Controls.Add(btnFirst);
            groupBox1.Controls.Add(pbMainPreview);
            groupBox1.Controls.Add(btnReload);
            groupBox1.Controls.Add(lblFrameValue);
            groupBox1.Controls.Add(lblModeValue);
            groupBox1.Controls.Add(btnToggleTheme);
            groupBox1.Controls.Add(lblThrottleValue);
            groupBox1.Controls.Add(btnGuide);
            groupBox1.Controls.Add(comboBoxFilter);
            groupBox1.Controls.Add(lblAngleValue);
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2);
            groupBox1.Size = new Size(1220, 552);
            groupBox1.TabIndex = 22;
            groupBox1.TabStop = false;
            groupBox1.Text = "터브 탐색기";
            // 
            // trackBarMain
            // 
            trackBarMain.Location = new Point(0, 504);
            trackBarMain.Margin = new Padding(2);
            trackBarMain.Maximum = 0;
            trackBarMain.Name = "trackBarMain";
            trackBarMain.Size = new Size(1216, 45);
            trackBarMain.TabIndex = 18;
            trackBarMain.Scroll += trackBarMain_Scroll;
            // 
            // numericUpDownInterval
            // 
            numericUpDownInterval.Location = new Point(1091, 247);
            numericUpDownInterval.Margin = new Padding(2);
            numericUpDownInterval.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numericUpDownInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownInterval.Name = "numericUpDownInterval";
            numericUpDownInterval.Size = new Size(121, 23);
            numericUpDownInterval.TabIndex = 11;
            numericUpDownInterval.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Location = new Point(0, 20);
            btnOpenFolder.Margin = new Padding(2);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(109, 22);
            btnOpenFolder.TabIndex = 0;
            btnOpenFolder.Text = "데이터 폴더 열기";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // label1
            // 
            label1.Font = new Font("맑은 고딕", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.Location = new Point(999, 246);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(99, 23);
            label1.TabIndex = 21;
            label1.Text = "재생간격(ms)";
            label1.Click += label1_Click;
            // 
            // btnLast
            // 
            btnLast.Location = new Point(1107, 105);
            btnLast.Margin = new Padding(2);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(104, 22);
            btnLast.TabIndex = 17;
            btnLast.Text = ">>";
            btnLast.Click += btnLast_Click;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.Location = new Point(1001, 132);
            btnAutoPlay.Margin = new Padding(2);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(210, 20);
            btnAutoPlay.TabIndex = 20;
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;
            btnAutoPlay.Click += button3_Click;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(1107, 78);
            btnNext.Margin = new Padding(2);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(104, 22);
            btnNext.TabIndex = 16;
            btnNext.Text = ">";
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new Point(999, 78);
            btnPrev.Margin = new Padding(2);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(104, 22);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "<";
            btnPrev.Click += btnPrev_Click;
            // 
            // listBoxData
            // 
            listBoxData.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            listBoxData.Location = new Point(0, 76);
            listBoxData.Margin = new Padding(2);
            listBoxData.Name = "listBoxData";
            listBoxData.Size = new Size(219, 424);
            listBoxData.TabIndex = 13;
            listBoxData.SelectedIndexChanged += listBoxData_SelectedIndexChanged;
            // 
            // btnFirst
            // 
            btnFirst.Location = new Point(999, 105);
            btnFirst.Margin = new Padding(2);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(104, 22);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "<<";
            btnFirst.Click += btnFirst_Click;
            // 
            // pbMainPreview
            // 
            pbMainPreview.BorderStyle = BorderStyle.FixedSingle;
            pbMainPreview.Location = new Point(223, 46);
            pbMainPreview.Margin = new Padding(2);
            pbMainPreview.Name = "pbMainPreview";
            pbMainPreview.Size = new Size(772, 454);
            pbMainPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            pbMainPreview.TabIndex = 4;
            pbMainPreview.TabStop = false;
            // 
            // btnReload
            // 
            btnReload.Location = new Point(114, 20);
            btnReload.Margin = new Padding(2);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(109, 22);
            btnReload.TabIndex = 1;
            btnReload.Text = "다시 불러오기";
            btnReload.UseVisualStyleBackColor = true;
            btnReload.Click += btnReload_Click;
            // 
            // lblFrameValue
            // 
            lblFrameValue.Font = new Font("맑은 고딕", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblFrameValue.Location = new Point(999, 46);
            lblFrameValue.Margin = new Padding(2, 0, 2, 0);
            lblFrameValue.Name = "lblFrameValue";
            lblFrameValue.Size = new Size(215, 29);
            lblFrameValue.TabIndex = 5;
            lblFrameValue.Text = "Frame: 0/0";
            // 
            // lblModeValue
            // 
            lblModeValue.Font = new Font("맑은 고딕", 13.8F);
            lblModeValue.Location = new Point(1001, 215);
            lblModeValue.Margin = new Padding(2, 0, 2, 0);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(212, 84);
            lblModeValue.TabIndex = 8;
            lblModeValue.Text = "모드: -";
            lblModeValue.Click += lblModeValue_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Location = new Point(227, 20);
            btnToggleTheme.Margin = new Padding(2);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(109, 22);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "테마 전환";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // lblThrottleValue
            // 
            lblThrottleValue.Font = new Font("맑은 고딕", 13.8F);
            lblThrottleValue.Location = new Point(1001, 188);
            lblThrottleValue.Margin = new Padding(2, 0, 2, 0);
            lblThrottleValue.Name = "lblThrottleValue";
            lblThrottleValue.Size = new Size(212, 28);
            lblThrottleValue.TabIndex = 7;
            lblThrottleValue.Text = "스로틀값: 0.000";
            lblThrottleValue.Click += lblThrottleValue_Click;
            // 
            // btnGuide
            // 
            btnGuide.Location = new Point(341, 20);
            btnGuide.Margin = new Padding(2);
            btnGuide.Name = "btnGuide";
            btnGuide.Size = new Size(109, 22);
            btnGuide.TabIndex = 3;
            btnGuide.Text = "단계별 가이드";
            btnGuide.UseVisualStyleBackColor = true;
            btnGuide.Click += btnGuide_Click;
            // 
            // comboBoxFilter
            // 
            comboBoxFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFilter.Items.AddRange(new object[] { "전체 보기", "스로틀 최소값", "조향 범위", "모드 = 사용자", "이미지 없는 프레임" });
            comboBoxFilter.Location = new Point(0, 46);
            comboBoxFilter.Margin = new Padding(2);
            comboBoxFilter.Name = "comboBoxFilter";
            comboBoxFilter.Size = new Size(219, 23);
            comboBoxFilter.TabIndex = 9;
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;
            // 
            // lblAngleValue
            // 
            lblAngleValue.Font = new Font("맑은 고딕", 13.8F);
            lblAngleValue.Location = new Point(1001, 158);
            lblAngleValue.Margin = new Padding(2, 0, 2, 0);
            lblAngleValue.Name = "lblAngleValue";
            lblAngleValue.Size = new Size(212, 29);
            lblAngleValue.TabIndex = 6;
            lblAngleValue.Text = "조향값: 0.000";
            lblAngleValue.Click += lblAngleValue_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox3);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(2);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(2);
            tabPage2.Size = new Size(1221, 662);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "학습 실행";
            tabPage2.UseVisualStyleBackColor = true;
            tabPage2.Click += tabPage2_Click;
            // 
            // groupBox3
            // 
            groupBox3.Location = new Point(12, 14);
            groupBox3.Margin = new Padding(2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(2);
            groupBox3.Size = new Size(1206, 59);
            groupBox3.TabIndex = 0;
            groupBox3.TabStop = false;
            groupBox3.Text = "groupBox3";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panel1);
            tabPage3.Controls.Add(lblDescription);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Margin = new Padding(2);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(2);
            tabPage3.Size = new Size(1221, 662);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "그래프/통계";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Location = new Point(57, 56);
            panel1.Margin = new Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new Size(716, 306);
            panel1.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(57, 20);
            lblDescription.Margin = new Padding(2, 0, 2, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(739, 15);
            lblDescription.TabIndex = 0;
            lblDescription.Text = "조향값과 스로틀값 변화 추이를 시각화합니다. 파란색은 조향, 초록색은 스로틀입니다. 필터 적용 후 보이는 데이터 기준으로 갱신됩니다.";
            // 
            // tabPage4
            // 
            tabPage4.Location = new Point(4, 24);
            tabPage4.Margin = new Padding(2);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(1221, 662);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "tabPage4";
            tabPage4.UseVisualStyleBackColor = true;
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
            toolStripStatusLabelPath.Size = new Size(43, 17);
            toolStripStatusLabelPath.Text = "경로: -";
            // 
            // toolStripStatusLabelFrames
            // 
            toolStripStatusLabelFrames.Name = "toolStripStatusLabelFrames";
            toolStripStatusLabelFrames.Size = new Size(59, 17);
            toolStripStatusLabelFrames.Text = "Frames: 0";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1230, 715);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Data Manager";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numFilterMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFilterMin).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarMain).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbMainPreview).EndInit();
            tabPage2.ResumeLayout(false);
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
        private Label label1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private NumericUpDown numFilterMax;
        private NumericUpDown numFilterMin;
        private TextBox textBox1;
        private Label label3;
        private Label label2;
        private Button btnRepair;
        private Button btnReloadTub;
        private TabPage tabPage4;
        private TabPage tabPage2;
        private GroupBox groupBox3;
    }
}
