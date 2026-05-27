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
            menuStripMain = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuOpenDataFolder = new ToolStripMenuItem();
            mnuReloadData = new ToolStripMenuItem();
            mnuExit = new ToolStripMenuItem();
            mnuView = new ToolStripMenuItem();
            mnuToggleTheme = new ToolStripMenuItem();
            mnuOpenGraphStats = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            mnuOpenGuide = new ToolStripMenuItem();
            tabControlMain = new TabControl();
            tabPageDataViewer = new TabPage();
            grpTubCleaner = new GroupBox();
            btnRepair = new Button();
            btnReloadTub = new Button();
            txtTubCleanerPath = new TextBox();
            numFilterMax = new NumericUpDown();
            lblFilterMax = new Label();
            lblFilterMin = new Label();
            numFilterMin = new NumericUpDown();
            btnApplyFilter = new Button();
            btnClearFilter = new Button();
            grpTubExplorer = new GroupBox();
            trkFramePosition = new TrackBar();
            numPlaybackInterval = new NumericUpDown();
            btnOpenFolder = new Button();
            lblPlayInterval = new Label();
            btnLast = new Button();
            btnAutoPlay = new Button();
            btnNext = new Button();
            btnPrev = new Button();
            lstFrameData = new ListBox();
            btnFirst = new Button();
            picMainPreview = new PictureBox();
            btnReload = new Button();
            lblFrameValue = new Label();
            lblModeValue = new Label();
            btnToggleTheme = new Button();
            lblThrottleValue = new Label();
            btnGuide = new Button();
            lblAngleValue = new Label();
            tabPageTraining = new TabPage();
            grpTrainingLog = new GroupBox();
            grpTrainingSettings = new GroupBox();
            tbxTubPath = new TextBox();
            tbxModelPath = new TextBox();
            tbxMycarPath = new TextBox();
            tbxPythonPath = new TextBox();
            lblEpoch = new Label();
            lblModeType = new Label();
            lblModelPath = new Label();
            lblTubPath = new Label();
            lblMycarPath = new Label();
            lblPythonPath = new Label();
            btnSaveSettings = new Button();
            btnStopTraining = new Button();
            btnStartTraining = new Button();
            btnTubPath = new Button();
            btnMycarPath = new Button();
            btnPythonPath = new Button();
            tabPageGraphStats = new TabPage();
            pnlGraphStats = new Panel();
            lblDescription = new Label();
            statusStripDataViewer = new StatusStrip();
            toolStripStatusLabelPath = new ToolStripStatusLabel();
            toolStripStatusLabelFrames = new ToolStripStatusLabel();
            cbxModelType = new ComboBox();
            nudEpoch = new NumericUpDown();
            rtbTrainingLog = new RichTextBox();
            statusStripTraining = new StatusStrip();
            toolStripStatusLabelTraining = new ToolStripStatusLabel();
            menuStripMain.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageDataViewer.SuspendLayout();
            grpTubCleaner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numFilterMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFilterMin).BeginInit();
            grpTubExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkFramePosition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPlaybackInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picMainPreview).BeginInit();
            tabPageTraining.SuspendLayout();
            grpTrainingLog.SuspendLayout();
            grpTrainingSettings.SuspendLayout();
            tabPageGraphStats.SuspendLayout();
            statusStripDataViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudEpoch).BeginInit();
            statusStripTraining.SuspendLayout();
            SuspendLayout();
            //
            // menuStripMain
            //
            menuStripMain.ImageScalingSize = new Size(20, 20);
            menuStripMain.Items.AddRange(new ToolStripItem[] { mnuFile, mnuView, mnuHelp });
            menuStripMain.Location = new Point(0, 0);
            menuStripMain.Name = "menuStripMain";
            menuStripMain.Padding = new Padding(6, 3, 0, 3);
            menuStripMain.Size = new Size(1581, 30);
            menuStripMain.TabIndex = 0;
            menuStripMain.Text = "menuStripMain";
            //
            // mnuFile
            //
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuOpenDataFolder, mnuReloadData, mnuExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(53, 24);
            mnuFile.Text = "파일";
            //
            // mnuOpenDataFolder
            //
            mnuOpenDataFolder.Name = "mnuOpenDataFolder";
            mnuOpenDataFolder.Size = new Size(207, 26);
            mnuOpenDataFolder.Text = "데이터 폴더 열기";
            //
            // mnuReloadData
            //
            mnuReloadData.Name = "mnuReloadData";
            mnuReloadData.Size = new Size(207, 26);
            mnuReloadData.Text = "다시 불러오기";
            //
            // mnuExit
            //
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(207, 26);
            mnuExit.Text = "종료";
            //
            // mnuView
            //
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuToggleTheme, mnuOpenGraphStats });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(53, 24);
            mnuView.Text = "보기";
            //
            // mnuToggleTheme
            //
            mnuToggleTheme.Name = "mnuToggleTheme";
            mnuToggleTheme.Size = new Size(258, 26);
            mnuToggleTheme.Text = "테마 전환";
            //
            // mnuOpenGraphStats
            //
            mnuOpenGraphStats.Name = "mnuOpenGraphStats";
            mnuOpenGraphStats.Size = new Size(258, 26);
            mnuOpenGraphStats.Text = "그래프/통계 탭으로 이동";
            mnuOpenGraphStats.Click += mnuOpenGraphStats_Click;
            //
            // mnuHelp
            //
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuOpenGuide });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(68, 24);
            mnuHelp.Text = "도움말";
            mnuHelp.Click += mnuHelp_Click;
            //
            // mnuOpenGuide
            //
            mnuOpenGuide.Name = "mnuOpenGuide";
            mnuOpenGuide.Size = new Size(187, 26);
            mnuOpenGuide.Text = "단계별 가이드";
            //
            // tabControlMain
            //
            tabControlMain.Controls.Add(tabPageDataViewer);
            tabControlMain.Controls.Add(tabPageTraining);
            tabControlMain.Controls.Add(tabPageGraphStats);
            tabControlMain.Location = new Point(3, 31);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1580, 920);
            tabControlMain.TabIndex = 1;
            //
            // tabPageDataViewer
            //
            tabPageDataViewer.Controls.Add(grpTubCleaner);
            tabPageDataViewer.Controls.Add(grpTubExplorer);
            tabPageDataViewer.Location = new Point(4, 29);
            tabPageDataViewer.Name = "tabPageDataViewer";
            tabPageDataViewer.Padding = new Padding(3);
            tabPageDataViewer.Size = new Size(1572, 887);
            tabPageDataViewer.TabIndex = 0;
            tabPageDataViewer.Text = "데이터 뷰어";
            tabPageDataViewer.UseVisualStyleBackColor = true;
            //
            // grpTubCleaner
            //
            grpTubCleaner.Controls.Add(btnRepair);
            grpTubCleaner.Controls.Add(btnReloadTub);
            grpTubCleaner.Controls.Add(txtTubCleanerPath);
            grpTubCleaner.Controls.Add(numFilterMax);
            grpTubCleaner.Controls.Add(lblFilterMax);
            grpTubCleaner.Controls.Add(lblFilterMin);
            grpTubCleaner.Controls.Add(numFilterMin);
            grpTubCleaner.Controls.Add(btnApplyFilter);
            grpTubCleaner.Controls.Add(btnClearFilter);
            grpTubCleaner.Location = new Point(3, 741);
            grpTubCleaner.Name = "grpTubCleaner";
            grpTubCleaner.Size = new Size(1563, 68);
            grpTubCleaner.TabIndex = 23;
            grpTubCleaner.TabStop = false;
            grpTubCleaner.Text = "터브 정리기";
            //
            // btnRepair
            //
            btnRepair.Location = new Point(1308, 25);
            btnRepair.Name = "btnRepair";
            btnRepair.Size = new Size(120, 29);
            btnRepair.TabIndex = 28;
            btnRepair.Text = "복원";
            btnRepair.UseVisualStyleBackColor = true;
            //
            // btnReloadTub
            //
            btnReloadTub.Location = new Point(1435, 25);
            btnReloadTub.Name = "btnReloadTub";
            btnReloadTub.Size = new Size(120, 29);
            btnReloadTub.TabIndex = 29;
            btnReloadTub.Text = "터브 다시 로드";
            btnReloadTub.UseVisualStyleBackColor = true;
            //
            // txtTubCleanerPath
            //
            txtTubCleanerPath.Location = new Point(476, 25);
            txtTubCleanerPath.Name = "txtTubCleanerPath";
            txtTubCleanerPath.Size = new Size(574, 27);
            txtTubCleanerPath.TabIndex = 27;
            //
            // numFilterMax
            //
            numFilterMax.Location = new Point(320, 25);
            numFilterMax.Name = "numFilterMax";
            numFilterMax.Size = new Size(150, 27);
            numFilterMax.TabIndex = 25;
            //
            // lblFilterMax
            //
            lblFilterMax.AutoSize = true;
            lblFilterMax.Font = new Font("맑은 고딕", 10F);
            lblFilterMax.Location = new Point(253, 27);
            lblFilterMax.Name = "lblFilterMax";
            lblFilterMax.Size = new Size(61, 23);
            lblFilterMax.TabIndex = 26;
            lblFilterMax.Text = "최댓값";
            //
            // lblFilterMin
            //
            lblFilterMin.AutoSize = true;
            lblFilterMin.Font = new Font("맑은 고딕", 10F);
            lblFilterMin.Location = new Point(15, 27);
            lblFilterMin.Name = "lblFilterMin";
            lblFilterMin.Size = new Size(61, 23);
            lblFilterMin.TabIndex = 25;
            lblFilterMin.Text = "최솟값";
            //
            // numFilterMin
            //
            numFilterMin.Location = new Point(82, 27);
            numFilterMin.Name = "numFilterMin";
            numFilterMin.Size = new Size(150, 27);
            numFilterMin.TabIndex = 24;
            //
            // btnApplyFilter
            //
            btnApplyFilter.Location = new Point(1056, 24);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(120, 29);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            //
            // btnClearFilter
            //
            btnClearFilter.Location = new Point(1182, 24);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(120, 29);
            btnClearFilter.TabIndex = 19;
            btnClearFilter.Text = "필터 해제";
            btnClearFilter.UseVisualStyleBackColor = true;
            //
            // grpTubExplorer
            //
            grpTubExplorer.Controls.Add(trkFramePosition);
            grpTubExplorer.Controls.Add(numPlaybackInterval);
            grpTubExplorer.Controls.Add(btnOpenFolder);
            grpTubExplorer.Controls.Add(lblPlayInterval);
            grpTubExplorer.Controls.Add(btnLast);
            grpTubExplorer.Controls.Add(btnAutoPlay);
            grpTubExplorer.Controls.Add(btnNext);
            grpTubExplorer.Controls.Add(btnPrev);
            grpTubExplorer.Controls.Add(lstFrameData);
            grpTubExplorer.Controls.Add(btnFirst);
            grpTubExplorer.Controls.Add(picMainPreview);
            grpTubExplorer.Controls.Add(btnReload);
            grpTubExplorer.Controls.Add(lblFrameValue);
            grpTubExplorer.Controls.Add(lblModeValue);
            grpTubExplorer.Controls.Add(btnToggleTheme);
            grpTubExplorer.Controls.Add(lblThrottleValue);
            grpTubExplorer.Controls.Add(btnGuide);
            grpTubExplorer.Controls.Add(lblAngleValue);
            grpTubExplorer.Location = new Point(0, 0);
            grpTubExplorer.Name = "grpTubExplorer";
            grpTubExplorer.Size = new Size(1569, 736);
            grpTubExplorer.TabIndex = 22;
            grpTubExplorer.TabStop = false;
            grpTubExplorer.Text = "터브 탐색기";
            //
            // trkFramePosition
            //
            trkFramePosition.Location = new Point(0, 672);
            trkFramePosition.Maximum = 0;
            trkFramePosition.Name = "trkFramePosition";
            trkFramePosition.Size = new Size(1563, 56);
            trkFramePosition.TabIndex = 18;
            trkFramePosition.Scroll += trkFramePosition_Scroll;
            //
            // numPlaybackInterval
            //
            numPlaybackInterval.Location = new Point(1403, 329);
            numPlaybackInterval.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numPlaybackInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numPlaybackInterval.Name = "numPlaybackInterval";
            numPlaybackInterval.Size = new Size(156, 27);
            numPlaybackInterval.TabIndex = 11;
            numPlaybackInterval.Value = new decimal(new int[] { 200, 0, 0, 0 });
            //
            // btnOpenFolder
            //
            btnOpenFolder.Location = new Point(0, 27);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(140, 29);
            btnOpenFolder.TabIndex = 0;
            btnOpenFolder.Text = "데이터 폴더 열기";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            //
            // lblPlayInterval
            //
            lblPlayInterval.Font = new Font("맑은 고딕", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblPlayInterval.Location = new Point(1284, 328);
            lblPlayInterval.Name = "lblPlayInterval";
            lblPlayInterval.Size = new Size(127, 31);
            lblPlayInterval.TabIndex = 21;
            lblPlayInterval.Text = "재생간격(ms)";
            lblPlayInterval.Click += lblPlayInterval_Click;
            //
            // btnLast
            //
            btnLast.Location = new Point(1423, 140);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(134, 29);
            btnLast.TabIndex = 17;
            btnLast.Text = ">>";
            btnLast.Click += btnLast_Click;
            //
            // btnAutoPlay
            //
            btnAutoPlay.Location = new Point(1287, 176);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(270, 27);
            btnAutoPlay.TabIndex = 20;
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;
            btnAutoPlay.Click += btnAutoPlay_Click;
            //
            // btnNext
            //
            btnNext.Location = new Point(1423, 104);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(134, 29);
            btnNext.TabIndex = 16;
            btnNext.Text = ">";
            btnNext.Click += btnNext_Click;
            //
            // btnPrev
            //
            btnPrev.Location = new Point(1284, 104);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(134, 29);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "<";
            btnPrev.Click += btnPrev_Click;
            //
            // lstFrameData
            //
            lstFrameData.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lstFrameData.Location = new Point(0, 61);
            lstFrameData.Name = "lstFrameData";
            lstFrameData.Size = new Size(280, 604);
            lstFrameData.TabIndex = 13;
            lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;
            //
            // btnFirst
            //
            btnFirst.Location = new Point(1284, 140);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(134, 29);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "<<";
            btnFirst.Click += btnFirst_Click;
            //
            // picMainPreview
            //
            picMainPreview.BorderStyle = BorderStyle.FixedSingle;
            picMainPreview.Location = new Point(287, 61);
            picMainPreview.Name = "picMainPreview";
            picMainPreview.Size = new Size(992, 605);
            picMainPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            picMainPreview.TabIndex = 4;
            picMainPreview.TabStop = false;
            //
            // btnReload
            //
            btnReload.Location = new Point(147, 27);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(140, 29);
            btnReload.TabIndex = 1;
            btnReload.Text = "다시 불러오기";
            btnReload.UseVisualStyleBackColor = true;
            btnReload.Click += btnReload_Click;
            //
            // lblFrameValue
            //
            lblFrameValue.Font = new Font("맑은 고딕", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblFrameValue.Location = new Point(1284, 61);
            lblFrameValue.Name = "lblFrameValue";
            lblFrameValue.Size = new Size(276, 39);
            lblFrameValue.TabIndex = 5;
            lblFrameValue.Text = "Frame: 0/0";
            //
            // lblModeValue
            //
            lblModeValue.Font = new Font("맑은 고딕", 13.8F);
            lblModeValue.Location = new Point(1287, 287);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(273, 112);
            lblModeValue.TabIndex = 8;
            lblModeValue.Text = "모드: -";
            lblModeValue.Click += lblModeValue_Click;
            //
            // btnToggleTheme
            //
            btnToggleTheme.Location = new Point(292, 27);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(140, 29);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "테마 전환";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            //
            // lblThrottleValue
            //
            lblThrottleValue.Font = new Font("맑은 고딕", 13.8F);
            lblThrottleValue.Location = new Point(1287, 251);
            lblThrottleValue.Name = "lblThrottleValue";
            lblThrottleValue.Size = new Size(273, 37);
            lblThrottleValue.TabIndex = 7;
            lblThrottleValue.Text = "스로틀값: 0.000";
            lblThrottleValue.Click += lblThrottleValue_Click;
            //
            // btnGuide
            //
            btnGuide.Location = new Point(438, 27);
            btnGuide.Name = "btnGuide";
            btnGuide.Size = new Size(140, 29);
            btnGuide.TabIndex = 3;
            btnGuide.Text = "단계별 가이드";
            btnGuide.UseVisualStyleBackColor = true;
            btnGuide.Click += btnGuide_Click;
            //
            // lblAngleValue
            //
            lblAngleValue.Font = new Font("맑은 고딕", 13.8F);
            lblAngleValue.Location = new Point(1287, 211);
            lblAngleValue.Name = "lblAngleValue";
            lblAngleValue.Size = new Size(273, 39);
            lblAngleValue.TabIndex = 6;
            lblAngleValue.Text = "조향값: 0.000";
            lblAngleValue.Click += lblAngleValue_Click;
            //
            // tabPageTraining
            //
            tabPageTraining.Controls.Add(statusStripTraining);
            tabPageTraining.Controls.Add(grpTrainingLog);
            tabPageTraining.Controls.Add(grpTrainingSettings);
            tabPageTraining.Location = new Point(4, 29);
            tabPageTraining.Name = "tabPageTraining";
            tabPageTraining.Padding = new Padding(3);
            tabPageTraining.Size = new Size(1572, 887);
            tabPageTraining.TabIndex = 1;
            tabPageTraining.Text = "학습 실행";
            tabPageTraining.UseVisualStyleBackColor = true;
            tabPageTraining.Click += tabPageTraining_Click;
            //
            // grpTrainingLog
            //
            grpTrainingLog.Controls.Add(rtbTrainingLog);
            grpTrainingLog.Location = new Point(15, 462);
            grpTrainingLog.Margin = new Padding(10);
            grpTrainingLog.Name = "grpTrainingLog";
            grpTrainingLog.Size = new Size(1547, 397);
            grpTrainingLog.TabIndex = 1;
            grpTrainingLog.TabStop = false;
            grpTrainingLog.Text = "학습 로그";
            //
            // grpTrainingSettings
            //
            grpTrainingSettings.Controls.Add(nudEpoch);
            grpTrainingSettings.Controls.Add(cbxModelType);
            grpTrainingSettings.Controls.Add(tbxTubPath);
            grpTrainingSettings.Controls.Add(tbxModelPath);
            grpTrainingSettings.Controls.Add(tbxMycarPath);
            grpTrainingSettings.Controls.Add(tbxPythonPath);
            grpTrainingSettings.Controls.Add(lblEpoch);
            grpTrainingSettings.Controls.Add(lblModeType);
            grpTrainingSettings.Controls.Add(lblModelPath);
            grpTrainingSettings.Controls.Add(lblTubPath);
            grpTrainingSettings.Controls.Add(lblMycarPath);
            grpTrainingSettings.Controls.Add(lblPythonPath);
            grpTrainingSettings.Controls.Add(btnSaveSettings);
            grpTrainingSettings.Controls.Add(btnStopTraining);
            grpTrainingSettings.Controls.Add(btnStartTraining);
            grpTrainingSettings.Controls.Add(btnTubPath);
            grpTrainingSettings.Controls.Add(btnMycarPath);
            grpTrainingSettings.Controls.Add(btnPythonPath);
            grpTrainingSettings.Location = new Point(15, 19);
            grpTrainingSettings.Name = "grpTrainingSettings";
            grpTrainingSettings.Size = new Size(1547, 437);
            grpTrainingSettings.TabIndex = 0;
            grpTrainingSettings.TabStop = false;
            grpTrainingSettings.Text = "학습 설정";
            //
            // tbxTubPath
            //
            tbxTubPath.Font = new Font("맑은 고딕", 12F);
            tbxTubPath.Location = new Point(218, 148);
            tbxTubPath.Name = "tbxTubPath";
            tbxTubPath.Size = new Size(1092, 34);
            tbxTubPath.TabIndex = 16;
            //
            // tbxModelPath
            //
            tbxModelPath.Font = new Font("맑은 고딕", 12F);
            tbxModelPath.Location = new Point(218, 199);
            tbxModelPath.Name = "tbxModelPath";
            tbxModelPath.Size = new Size(1092, 34);
            tbxModelPath.TabIndex = 14;
            //
            // tbxMycarPath
            //
            tbxMycarPath.Font = new Font("맑은 고딕", 12F);
            tbxMycarPath.Location = new Point(218, 95);
            tbxMycarPath.Name = "tbxMycarPath";
            tbxMycarPath.Size = new Size(1092, 34);
            tbxMycarPath.TabIndex = 13;
            //
            // tbxPythonPath
            //
            tbxPythonPath.Font = new Font("맑은 고딕", 12F);
            tbxPythonPath.Location = new Point(218, 43);
            tbxPythonPath.Name = "tbxPythonPath";
            tbxPythonPath.Size = new Size(1092, 34);
            tbxPythonPath.TabIndex = 12;
            //
            // lblEpoch
            //
            lblEpoch.AutoSize = true;
            lblEpoch.Font = new Font("맑은 고딕", 12F);
            lblEpoch.Location = new Point(45, 308);
            lblEpoch.Name = "lblEpoch";
            lblEpoch.Size = new Size(99, 28);
            lblEpoch.TabIndex = 11;
            lblEpoch.Text = "학습 횟수";
            //
            // lblModeType
            //
            lblModeType.AutoSize = true;
            lblModeType.Font = new Font("맑은 고딕", 12F);
            lblModeType.Location = new Point(45, 254);
            lblModeType.Name = "lblModeType";
            lblModeType.Size = new Size(99, 28);
            lblModeType.TabIndex = 10;
            lblModeType.Text = "모델 종류";
            //
            // lblModelPath
            //
            lblModelPath.AutoSize = true;
            lblModelPath.Font = new Font("맑은 고딕", 12F);
            lblModelPath.Location = new Point(45, 202);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Size = new Size(146, 28);
            lblModelPath.TabIndex = 9;
            lblModelPath.Text = "모델 저장 경로";
            //
            // lblTubPath
            //
            lblTubPath.AutoSize = true;
            lblTubPath.Font = new Font("맑은 고딕", 12F);
            lblTubPath.Location = new Point(45, 151);
            lblTubPath.Name = "lblTubPath";
            lblTubPath.Size = new Size(94, 28);
            lblTubPath.TabIndex = 8;
            lblTubPath.Text = "Tub 경로";
            //
            // lblMycarPath
            //
            lblMycarPath.AutoSize = true;
            lblMycarPath.Font = new Font("맑은 고딕", 12F);
            lblMycarPath.Location = new Point(45, 98);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Size = new Size(113, 28);
            lblMycarPath.TabIndex = 7;
            lblMycarPath.Text = "mycar 경로";
            //
            // lblPythonPath
            //
            lblPythonPath.AutoSize = true;
            lblPythonPath.Font = new Font("맑은 고딕", 12F);
            lblPythonPath.Location = new Point(45, 46);
            lblPythonPath.Name = "lblPythonPath";
            lblPythonPath.Size = new Size(119, 28);
            lblPythonPath.TabIndex = 6;
            lblPythonPath.Text = "파이썬 경로";
            //
            // btnSaveSettings
            //
            btnSaveSettings.Font = new Font("맑은 고딕", 12F);
            btnSaveSettings.Location = new Point(1006, 368);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(194, 42);
            btnSaveSettings.TabIndex = 5;
            btnSaveSettings.Text = "설정 저장";
            btnSaveSettings.UseVisualStyleBackColor = true;
            //
            // btnStopTraining
            //
            btnStopTraining.Font = new Font("맑은 고딕", 12F);
            btnStopTraining.Location = new Point(672, 368);
            btnStopTraining.Name = "btnStopTraining";
            btnStopTraining.Size = new Size(194, 42);
            btnStopTraining.TabIndex = 4;
            btnStopTraining.Text = "학습 중지";
            btnStopTraining.UseVisualStyleBackColor = true;
            //
            // btnStartTraining
            //
            btnStartTraining.Font = new Font("맑은 고딕", 12F);
            btnStartTraining.Location = new Point(337, 368);
            btnStartTraining.Name = "btnStartTraining";
            btnStartTraining.Size = new Size(194, 42);
            btnStartTraining.TabIndex = 3;
            btnStartTraining.Text = "학습 시작";
            btnStartTraining.UseVisualStyleBackColor = true;
            //
            // btnTubPath
            //
            btnTubPath.Font = new Font("맑은 고딕", 12F);
            btnTubPath.Location = new Point(1334, 135);
            btnTubPath.Name = "btnTubPath";
            btnTubPath.Size = new Size(123, 42);
            btnTubPath.TabIndex = 2;
            btnTubPath.Text = "Tub 선택";
            btnTubPath.UseVisualStyleBackColor = true;
            //
            // btnMycarPath
            //
            btnMycarPath.Font = new Font("맑은 고딕", 12F);
            btnMycarPath.Location = new Point(1334, 87);
            btnMycarPath.Name = "btnMycarPath";
            btnMycarPath.Size = new Size(123, 42);
            btnMycarPath.TabIndex = 1;
            btnMycarPath.Text = "찾기";
            btnMycarPath.UseVisualStyleBackColor = true;
            btnMycarPath.Click += btnMycarPath_Click;
            //
            // btnPythonPath
            //
            btnPythonPath.Font = new Font("맑은 고딕", 12F);
            btnPythonPath.Location = new Point(1334, 39);
            btnPythonPath.Name = "btnPythonPath";
            btnPythonPath.Size = new Size(123, 42);
            btnPythonPath.TabIndex = 0;
            btnPythonPath.Text = "찾기";
            btnPythonPath.UseVisualStyleBackColor = true;
            //
            // tabPageGraphStats
            //
            tabPageGraphStats.Controls.Add(pnlGraphStats);
            tabPageGraphStats.Controls.Add(lblDescription);
            tabPageGraphStats.Location = new Point(4, 29);
            tabPageGraphStats.Name = "tabPageGraphStats";
            tabPageGraphStats.Padding = new Padding(3);
            tabPageGraphStats.Size = new Size(1572, 887);
            tabPageGraphStats.TabIndex = 2;
            tabPageGraphStats.Text = "그래프/통계";
            tabPageGraphStats.UseVisualStyleBackColor = true;
            //
            // pnlGraphStats
            //
            pnlGraphStats.Location = new Point(73, 75);
            pnlGraphStats.Name = "pnlGraphStats";
            pnlGraphStats.Size = new Size(921, 408);
            pnlGraphStats.TabIndex = 1;
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
            // statusStripDataViewer
            //
            statusStripDataViewer.ImageScalingSize = new Size(20, 20);
            statusStripDataViewer.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelPath, toolStripStatusLabelFrames });
            statusStripDataViewer.Location = new Point(0, 0);
            statusStripDataViewer.Name = "statusStripDataViewer";
            statusStripDataViewer.Size = new Size(200, 22);
            statusStripDataViewer.TabIndex = 0;
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
            // cbxModelType
            //
            cbxModelType.Font = new Font("맑은 고딕", 12F);
            cbxModelType.FormattingEnabled = true;
            cbxModelType.Location = new Point(218, 251);
            cbxModelType.Name = "cbxModelType";
            cbxModelType.Size = new Size(391, 36);
            cbxModelType.TabIndex = 2;
            //
            // nudEpoch
            //
            nudEpoch.Font = new Font("맑은 고딕", 12F);
            nudEpoch.Location = new Point(218, 306);
            nudEpoch.Name = "nudEpoch";
            nudEpoch.Size = new Size(391, 34);
            nudEpoch.TabIndex = 17;
            //
            // rtbTrainingLog
            //
            rtbTrainingLog.Dock = DockStyle.Fill;
            rtbTrainingLog.Location = new Point(3, 23);
            rtbTrainingLog.Name = "rtbTrainingLog";
            rtbTrainingLog.Size = new Size(1541, 371);
            rtbTrainingLog.TabIndex = 0;
            rtbTrainingLog.Text = "";
            //
            // statusStripTraining
            //
            statusStripTraining.ImageScalingSize = new Size(20, 20);
            statusStripTraining.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelTraining });
            statusStripTraining.Location = new Point(3, 858);
            statusStripTraining.Name = "statusStripTraining";
            statusStripTraining.Size = new Size(1566, 26);
            statusStripTraining.TabIndex = 2;
            statusStripTraining.Text = "statusStripTraining";
            //
            // toolStripStatusLabelTraining
            //
            toolStripStatusLabelTraining.Name = "toolStripStatusLabelTraining";
            toolStripStatusLabelTraining.Size = new Size(102, 20);
            toolStripStatusLabelTraining.Text = "Tub 경로: C:\\";
            toolStripStatusLabelTraining.Click += toolStripStatusLabelTraining_Click;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1581, 953);
            Controls.Add(tabControlMain);
            Controls.Add(menuStripMain);
            MainMenuStrip = menuStripMain;
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Data Manager";
            Load += Form1_Load;
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabPageDataViewer.ResumeLayout(false);
            grpTubCleaner.ResumeLayout(false);
            grpTubCleaner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numFilterMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFilterMin).EndInit();
            grpTubExplorer.ResumeLayout(false);
            grpTubExplorer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkFramePosition).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPlaybackInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)picMainPreview).EndInit();
            tabPageTraining.ResumeLayout(false);
            tabPageTraining.PerformLayout();
            grpTrainingLog.ResumeLayout(false);
            grpTrainingSettings.ResumeLayout(false);
            grpTrainingSettings.PerformLayout();
            tabPageGraphStats.ResumeLayout(false);
            tabPageGraphStats.PerformLayout();
            statusStripDataViewer.ResumeLayout(false);
            statusStripDataViewer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudEpoch).EndInit();
            statusStripTraining.ResumeLayout(false);
            statusStripTraining.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStripMain;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuView;
        private ToolStripMenuItem mnuHelp;
        private TabControl tabControlMain;
        private TabPage tabPageDataViewer;
        private TabPage tabPageGraphStats;
        private Button btnOpenFolder;
        private Button btnReload;
        private Button btnToggleTheme;
        private Button btnGuide;
        private PictureBox picMainPreview;
        private Label lblFrameValue;
        private Label lblAngleValue;
        private Label lblThrottleValue;
        private Label lblModeValue;
        private NumericUpDown numPlaybackInterval;
        private Button btnApplyFilter;
        private ListBox lstFrameData;
        private Button btnFirst;
        private Button btnPrev;
        private Button btnNext;
        private Button btnLast;
        private TrackBar trkFramePosition;
        private StatusStrip statusStripDataViewer;
        private ToolStripStatusLabel toolStripStatusLabelPath;
        private ToolStripStatusLabel toolStripStatusLabelFrames;
        private Button btnClearFilter;
        private Label lblDescription;
        private Panel pnlGraphStats;
        private ToolStripMenuItem mnuToggleTheme;
        private ToolStripMenuItem mnuOpenGraphStats;
        private ToolStripMenuItem mnuOpenGuide;
        private ToolStripMenuItem mnuOpenDataFolder;
        private ToolStripMenuItem mnuReloadData;
        private ToolStripMenuItem mnuExit;
        private Button btnAutoPlay;
        private Label lblPlayInterval;
        private GroupBox grpTubExplorer;
        private GroupBox grpTubCleaner;
        private NumericUpDown numFilterMax;
        private NumericUpDown numFilterMin;
        private TextBox txtTubCleanerPath;
        private Label lblFilterMax;
        private Label lblFilterMin;
        private Button btnRepair;
        private Button btnReloadTub;
        private TabPage tabPageTraining;
        private GroupBox grpTrainingSettings;
        private GroupBox grpTrainingLog;
        private Label lblEpoch;
        private Label lblModeType;
        private Label lblModelPath;
        private Label lblTubPath;
        private Label lblMycarPath;
        private Label lblPythonPath;
        private Button btnSaveSettings;
        private Button btnStopTraining;
        private Button btnStartTraining;
        private Button btnTubPath;
        private Button btnMycarPath;
        private Button btnPythonPath;
        private TextBox tbxTubPath;
        private TextBox tbxModelPath;
        private TextBox tbxMycarPath;
        private TextBox tbxPythonPath;
        private NumericUpDown nudEpoch;
        private ComboBox cbxModelType;
        private RichTextBox rtbTrainingLog;
        private StatusStrip statusStripTraining;
        private ToolStripStatusLabel toolStripStatusLabelTraining;
    }
}
