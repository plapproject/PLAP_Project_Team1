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
            statusStripDataFooter = new StatusStrip();
            toolStripStatusLabelDataPath = new ToolStripStatusLabel();
            grpTubCleaner = new GroupBox();
            lblSelectRangeSeparator = new Label();
            txtSelectRangeMax = new TextBox();
            txtSelectRangeMin = new TextBox();
            lblSelectRange = new Label();
            btnRestoreFrame = new Button();
            btnFrameSave = new Button();
            btnDeleteFrame = new Button();
            btnExcludeRange = new Button();
            btnExcludeSelectedFrame = new Button();
            cbxScenarioFilter = new ComboBox();
            lblScenarioFilter = new Label();
            cbxModeFilter = new ComboBox();
            lblModeFilter = new Label();
            lblAngleRangeSeparator = new Label();
            lblThrottleRangeSeparator = new Label();
            txtThrottleMax = new TextBox();
            txtAngleMax = new TextBox();
            txtThrottleMin = new TextBox();
            txtAngleMin = new TextBox();
            lblThrottleRange = new Label();
            lblAngleRange = new Label();
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
            splitContainerFramePreview = new SplitContainer();
            lstFrameData = new DataGridView();
            picMainPreview = new PictureBox();
            btnFirst = new Button();
            btnReload = new Button();
            lblFrameValue = new Label();
            lblModeValue = new Label();
            btnToggleTheme = new Button();
            lblThrottleValue = new Label();
            btnGuide = new Button();
            lblAngleValue = new Label();
            tabPageTraining = new TabPage();
            statusStripTraining = new StatusStrip();
            toolStripStatusLabelTraining = new ToolStripStatusLabel();
            grpTrainingLog = new GroupBox();
            rtbTrainingLog = new RichTextBox();
            grpTrainingSettings = new GroupBox();
            nudEpoch = new NumericUpDown();
            cbxModelType = new ComboBox();
            tbxTubPath = new TextBox();
            tbxModelPath = new TextBox();
            tbxMycarPath = new TextBox();
            lblEpoch = new Label();
            lblModeType = new Label();
            lblModelPath = new Label();
            lblTubPath = new Label();
            lblMycarPath = new Label();
            btnSaveSettings = new Button();
            btnStopTraining = new Button();
            btnStartTraining = new Button();
            btnTubPath = new Button();
            btnMycarPath = new Button();
            tabPageGraphStats = new TabPage();
            pnlGraphStats = new Panel();
            lblDescription = new Label();
            statusStripDataViewer = new StatusStrip();
            toolStripStatusLabelPath = new ToolStripStatusLabel();
            toolStripStatusLabelFrames = new ToolStripStatusLabel();
            menuStripMain.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageDataViewer.SuspendLayout();
            statusStripDataFooter.SuspendLayout();
            grpTubCleaner.SuspendLayout();
            grpTubExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkFramePosition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPlaybackInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerFramePreview).BeginInit();
            splitContainerFramePreview.Panel1.SuspendLayout();
            splitContainerFramePreview.Panel2.SuspendLayout();
            splitContainerFramePreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lstFrameData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picMainPreview).BeginInit();
            tabPageTraining.SuspendLayout();
            statusStripTraining.SuspendLayout();
            grpTrainingLog.SuspendLayout();
            grpTrainingSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudEpoch).BeginInit();
            tabPageGraphStats.SuspendLayout();
            statusStripDataViewer.SuspendLayout();
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
            mnuOpenGuide.Size = new Size(202, 26);
            mnuOpenGuide.Text = "기능별 튜토리얼";
            // 
            // tabControlMain
            // 
            tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            tabPageDataViewer.Controls.Add(statusStripDataFooter);
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
            // statusStripDataFooter
            // 
            statusStripDataFooter.ImageScalingSize = new Size(20, 20);
            statusStripDataFooter.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelDataPath });
            statusStripDataFooter.Location = new Point(3, 858);
            statusStripDataFooter.Name = "statusStripDataFooter";
            statusStripDataFooter.Size = new Size(1566, 26);
            statusStripDataFooter.TabIndex = 24;
            statusStripDataFooter.Text = "statusStripDataFooter";
            // 
            // toolStripStatusLabelDataPath
            // 
            toolStripStatusLabelDataPath.Name = "toolStripStatusLabelDataPath";
            toolStripStatusLabelDataPath.Size = new Size(33, 20);
            toolStripStatusLabelDataPath.Text = "C:\\";
            // 
            // grpTubCleaner
            // 
            grpTubCleaner.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTubCleaner.Controls.Add(lblSelectRangeSeparator);
            grpTubCleaner.Controls.Add(txtSelectRangeMax);
            grpTubCleaner.Controls.Add(txtSelectRangeMin);
            grpTubCleaner.Controls.Add(lblSelectRange);
            grpTubCleaner.Controls.Add(btnRestoreFrame);
            grpTubCleaner.Controls.Add(btnFrameSave);
            grpTubCleaner.Controls.Add(btnDeleteFrame);
            grpTubCleaner.Controls.Add(btnExcludeRange);
            grpTubCleaner.Controls.Add(btnExcludeSelectedFrame);
            grpTubCleaner.Controls.Add(cbxScenarioFilter);
            grpTubCleaner.Controls.Add(lblScenarioFilter);
            grpTubCleaner.Controls.Add(cbxModeFilter);
            grpTubCleaner.Controls.Add(lblModeFilter);
            grpTubCleaner.Controls.Add(lblAngleRangeSeparator);
            grpTubCleaner.Controls.Add(lblThrottleRangeSeparator);
            grpTubCleaner.Controls.Add(txtThrottleMax);
            grpTubCleaner.Controls.Add(txtAngleMax);
            grpTubCleaner.Controls.Add(txtThrottleMin);
            grpTubCleaner.Controls.Add(txtAngleMin);
            grpTubCleaner.Controls.Add(lblThrottleRange);
            grpTubCleaner.Controls.Add(lblAngleRange);
            grpTubCleaner.Controls.Add(btnApplyFilter);
            grpTubCleaner.Controls.Add(btnClearFilter);
            grpTubCleaner.Location = new Point(3, 635);
            grpTubCleaner.Name = "grpTubCleaner";
            grpTubCleaner.Size = new Size(1563, 220);
            grpTubCleaner.TabIndex = 23;
            grpTubCleaner.TabStop = false;
            grpTubCleaner.Text = "터브 정리기";
            grpTubCleaner.Enter += grpTubCleaner_Enter;
            // 
            // lblSelectRangeSeparator
            // 
            lblSelectRangeSeparator.AutoSize = true;
            lblSelectRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblSelectRangeSeparator.Location = new Point(402, 69);
            lblSelectRangeSeparator.Name = "lblSelectRangeSeparator";
            lblSelectRangeSeparator.Size = new Size(27, 23);
            lblSelectRangeSeparator.TabIndex = 47;
            lblSelectRangeSeparator.Text = "～";
            // 
            // txtSelectRangeMax
            // 
            txtSelectRangeMax.Location = new Point(435, 67);
            txtSelectRangeMax.Margin = new Padding(4);
            txtSelectRangeMax.Name = "txtSelectRangeMax";
            txtSelectRangeMax.Size = new Size(130, 27);
            txtSelectRangeMax.TabIndex = 46;
            // 
            // txtSelectRangeMin
            // 
            txtSelectRangeMin.Location = new Point(266, 67);
            txtSelectRangeMin.Margin = new Padding(4);
            txtSelectRangeMin.Name = "txtSelectRangeMin";
            txtSelectRangeMin.Size = new Size(130, 27);
            txtSelectRangeMin.TabIndex = 45;
            // 
            // lblSelectRange
            // 
            lblSelectRange.AutoSize = true;
            lblSelectRange.Font = new Font("맑은 고딕", 10F);
            lblSelectRange.Location = new Point(96, 70);
            lblSelectRange.Margin = new Padding(4, 0, 4, 0);
            lblSelectRange.Name = "lblSelectRange";
            lblSelectRange.Size = new Size(84, 23);
            lblSelectRange.TabIndex = 44;
            lblSelectRange.Text = "구간 선택";
            // 
            // btnRestoreFrame
            // 
            btnRestoreFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRestoreFrame.Location = new Point(1265, 143);
            btnRestoreFrame.Name = "btnRestoreFrame";
            btnRestoreFrame.Size = new Size(215, 29);
            btnRestoreFrame.TabIndex = 43;
            btnRestoreFrame.Text = "복구";
            btnRestoreFrame.UseVisualStyleBackColor = true;
            // 
            // btnFrameSave
            // 
            btnFrameSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFrameSave.Location = new Point(1265, 180);
            btnFrameSave.Name = "btnFrameSave";
            btnFrameSave.Size = new Size(215, 29);
            btnFrameSave.TabIndex = 48;
            btnFrameSave.Text = "상태 저장";
            btnFrameSave.UseVisualStyleBackColor = true;
            // 
            // btnDeleteFrame
            // 
            btnDeleteFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteFrame.Location = new Point(1034, 141);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(215, 29);
            btnDeleteFrame.TabIndex = 42;
            btnDeleteFrame.Text = "삭제";
            btnDeleteFrame.UseVisualStyleBackColor = true;
            // 
            // btnExcludeRange
            // 
            btnExcludeRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeRange.Location = new Point(1034, 66);
            btnExcludeRange.Name = "btnExcludeRange";
            btnExcludeRange.Size = new Size(215, 29);
            btnExcludeRange.TabIndex = 40;
            btnExcludeRange.Text = "구간 제외";
            btnExcludeRange.UseVisualStyleBackColor = true;
            // 
            // btnExcludeSelectedFrame
            // 
            btnExcludeSelectedFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeSelectedFrame.Location = new Point(1265, 68);
            btnExcludeSelectedFrame.Name = "btnExcludeSelectedFrame";
            btnExcludeSelectedFrame.Size = new Size(215, 29);
            btnExcludeSelectedFrame.TabIndex = 41;
            btnExcludeSelectedFrame.Text = "선택 프레임 제외";
            btnExcludeSelectedFrame.UseVisualStyleBackColor = true;
            // 
            // cbxScenarioFilter
            // 
            cbxScenarioFilter.Font = new Font("맑은 고딕", 10F);
            cbxScenarioFilter.FormattingEnabled = true;
            cbxScenarioFilter.Items.AddRange(new object[] { "All", "Normal", "Night", "Left_turn", "Right_turn", "Out_of_bound" });
            cbxScenarioFilter.Location = new Point(705, 142);
            cbxScenarioFilter.Name = "cbxScenarioFilter";
            cbxScenarioFilter.Size = new Size(277, 31);
            cbxScenarioFilter.TabIndex = 39;
            // 
            // lblScenarioFilter
            // 
            lblScenarioFilter.AutoSize = true;
            lblScenarioFilter.Font = new Font("맑은 고딕", 10F);
            lblScenarioFilter.Location = new Point(612, 142);
            lblScenarioFilter.Name = "lblScenarioFilter";
            lblScenarioFilter.Size = new Size(88, 23);
            lblScenarioFilter.TabIndex = 38;
            lblScenarioFilter.Text = "시나리오: ";
            // 
            // cbxModeFilter
            // 
            cbxModeFilter.Font = new Font("맑은 고딕", 10F);
            cbxModeFilter.FormattingEnabled = true;
            cbxModeFilter.Items.AddRange(new object[] { "All", "User", "Local", "Local_angle" });
            cbxModeFilter.Location = new Point(705, 101);
            cbxModeFilter.Name = "cbxModeFilter";
            cbxModeFilter.Size = new Size(277, 31);
            cbxModeFilter.TabIndex = 37;
            // 
            // lblModeFilter
            // 
            lblModeFilter.AutoSize = true;
            lblModeFilter.Font = new Font("맑은 고딕", 10F);
            lblModeFilter.Location = new Point(612, 103);
            lblModeFilter.Name = "lblModeFilter";
            lblModeFilter.Size = new Size(84, 23);
            lblModeFilter.TabIndex = 36;
            lblModeFilter.Text = "모드      :";
            // 
            // lblAngleRangeSeparator
            // 
            lblAngleRangeSeparator.AutoSize = true;
            lblAngleRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblAngleRangeSeparator.Location = new Point(402, 109);
            lblAngleRangeSeparator.Name = "lblAngleRangeSeparator";
            lblAngleRangeSeparator.Size = new Size(27, 23);
            lblAngleRangeSeparator.TabIndex = 35;
            lblAngleRangeSeparator.Text = "～";
            // 
            // lblThrottleRangeSeparator
            // 
            lblThrottleRangeSeparator.AutoSize = true;
            lblThrottleRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblThrottleRangeSeparator.Location = new Point(402, 150);
            lblThrottleRangeSeparator.Name = "lblThrottleRangeSeparator";
            lblThrottleRangeSeparator.Size = new Size(27, 23);
            lblThrottleRangeSeparator.TabIndex = 34;
            lblThrottleRangeSeparator.Text = "～";
            // 
            // txtThrottleMax
            // 
            txtThrottleMax.Font = new Font("맑은 고딕", 10F);
            txtThrottleMax.Location = new Point(435, 147);
            txtThrottleMax.Name = "txtThrottleMax";
            txtThrottleMax.Size = new Size(130, 30);
            txtThrottleMax.TabIndex = 33;
            // 
            // txtAngleMax
            // 
            txtAngleMax.Font = new Font("맑은 고딕", 10F);
            txtAngleMax.Location = new Point(435, 105);
            txtAngleMax.Name = "txtAngleMax";
            txtAngleMax.Size = new Size(130, 30);
            txtAngleMax.TabIndex = 32;
            // 
            // txtThrottleMin
            // 
            txtThrottleMin.Font = new Font("맑은 고딕", 10F);
            txtThrottleMin.Location = new Point(266, 147);
            txtThrottleMin.Name = "txtThrottleMin";
            txtThrottleMin.Size = new Size(130, 30);
            txtThrottleMin.TabIndex = 31;
            // 
            // txtAngleMin
            // 
            txtAngleMin.Font = new Font("맑은 고딕", 10F);
            txtAngleMin.Location = new Point(266, 105);
            txtAngleMin.Name = "txtAngleMin";
            txtAngleMin.Size = new Size(130, 30);
            txtAngleMin.TabIndex = 30;
            // 
            // lblThrottleRange
            // 
            lblThrottleRange.AutoSize = true;
            lblThrottleRange.Font = new Font("맑은 고딕", 10F);
            lblThrottleRange.Location = new Point(96, 150);
            lblThrottleRange.Name = "lblThrottleRange";
            lblThrottleRange.Size = new Size(175, 23);
            lblThrottleRange.TabIndex = 26;
            lblThrottleRange.Text = "스로틀값 범위(-1~1): ";
            // 
            // lblAngleRange
            // 
            lblAngleRange.AutoSize = true;
            lblAngleRange.Font = new Font("맑은 고딕", 10F);
            lblAngleRange.Location = new Point(96, 109);
            lblAngleRange.Name = "lblAngleRange";
            lblAngleRange.Size = new Size(164, 23);
            lblAngleRange.TabIndex = 25;
            lblAngleRange.Text = "조향각 범위 (-1~1): ";
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFilter.Location = new Point(1035, 103);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(215, 29);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilter.Location = new Point(1265, 105);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(215, 29);
            btnClearFilter.TabIndex = 19;
            btnClearFilter.Text = "필터 해제";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // grpTubExplorer
            // 
            grpTubExplorer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTubExplorer.Controls.Add(trkFramePosition);
            grpTubExplorer.Controls.Add(numPlaybackInterval);
            grpTubExplorer.Controls.Add(btnOpenFolder);
            grpTubExplorer.Controls.Add(lblPlayInterval);
            grpTubExplorer.Controls.Add(btnLast);
            grpTubExplorer.Controls.Add(btnAutoPlay);
            grpTubExplorer.Controls.Add(btnNext);
            grpTubExplorer.Controls.Add(btnPrev);
            grpTubExplorer.Controls.Add(splitContainerFramePreview);
            grpTubExplorer.Controls.Add(btnFirst);
            grpTubExplorer.Controls.Add(btnReload);
            grpTubExplorer.Controls.Add(lblFrameValue);
            grpTubExplorer.Controls.Add(lblModeValue);
            grpTubExplorer.Controls.Add(btnToggleTheme);
            grpTubExplorer.Controls.Add(lblThrottleValue);
            grpTubExplorer.Controls.Add(btnGuide);
            grpTubExplorer.Controls.Add(lblAngleValue);
            grpTubExplorer.Location = new Point(0, 0);
            grpTubExplorer.Name = "grpTubExplorer";
            grpTubExplorer.Size = new Size(1569, 635);
            grpTubExplorer.TabIndex = 22;
            grpTubExplorer.TabStop = false;
            grpTubExplorer.Text = "터브 탐색기";
            // 
            // trkFramePosition
            // 
            trkFramePosition.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trkFramePosition.Location = new Point(-1, 571);
            trkFramePosition.Maximum = 0;
            trkFramePosition.Name = "trkFramePosition";
            trkFramePosition.Size = new Size(1563, 56);
            trkFramePosition.TabIndex = 18;
            trkFramePosition.Scroll += trkFramePosition_Scroll;
            // 
            // numPlaybackInterval
            // 
            numPlaybackInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            lblPlayInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            btnLast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLast.Location = new Point(1423, 140);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(134, 29);
            btnLast.TabIndex = 17;
            btnLast.Text = ">>";
            btnLast.Click += btnLast_Click;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNext.Location = new Point(1423, 104);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(134, 29);
            btnNext.TabIndex = 16;
            btnNext.Text = ">";
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrev.Location = new Point(1284, 104);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(134, 29);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "<";
            btnPrev.Click += btnPrev_Click;
            // 
            // splitContainerFramePreview
            // 
            splitContainerFramePreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerFramePreview.Location = new Point(0, 61);
            splitContainerFramePreview.Name = "splitContainerFramePreview";
            // 
            // splitContainerFramePreview.Panel1
            // 
            splitContainerFramePreview.Panel1.Controls.Add(lstFrameData);
            // 
            // splitContainerFramePreview.Panel2
            // 
            splitContainerFramePreview.Panel2.Controls.Add(picMainPreview);
            splitContainerFramePreview.Size = new Size(1278, 503);
            splitContainerFramePreview.SplitterDistance = 360;
            splitContainerFramePreview.SplitterWidth = 6;
            splitContainerFramePreview.TabIndex = 44;
            // 
            // lstFrameData
            // 
            lstFrameData.AllowUserToAddRows = false;
            lstFrameData.AllowUserToDeleteRows = false;
            lstFrameData.AllowUserToResizeRows = false;
            lstFrameData.BorderStyle = BorderStyle.FixedSingle;
            lstFrameData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            lstFrameData.Dock = DockStyle.Fill;
            lstFrameData.Location = new Point(0, 0);
            lstFrameData.MultiSelect = true;
            lstFrameData.Name = "lstFrameData";
            lstFrameData.ReadOnly = true;
            lstFrameData.RowHeadersVisible = false;
            lstFrameData.RowHeadersWidth = 51;
            lstFrameData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            lstFrameData.Size = new Size(360, 503);
            lstFrameData.TabIndex = 13;
            lstFrameData.SelectionChanged += lstFrameData_SelectedIndexChanged;
            // 
            // picMainPreview
            // 
            picMainPreview.BorderStyle = BorderStyle.FixedSingle;
            picMainPreview.Dock = DockStyle.Fill;
            picMainPreview.Location = new Point(0, 0);
            picMainPreview.Name = "picMainPreview";
            picMainPreview.Size = new Size(912, 503);
            picMainPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picMainPreview.TabIndex = 4;
            picMainPreview.TabStop = false;
            // 
            // btnFirst
            // 
            btnFirst.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFirst.Location = new Point(1284, 140);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(134, 29);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "<<";
            btnFirst.Click += btnFirst_Click;
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
            lblFrameValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblFrameValue.Font = new Font("맑은 고딕", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblFrameValue.Location = new Point(1284, 61);
            lblFrameValue.Name = "lblFrameValue";
            lblFrameValue.Size = new Size(276, 39);
            lblFrameValue.TabIndex = 5;
            lblFrameValue.Text = "Frame: 0/0";
            // 
            // lblModeValue
            // 
            lblModeValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblModeValue.Font = new Font("맑은 고딕", 13.8F);
            lblModeValue.Location = new Point(1287, 287);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(273, 39);
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
            lblThrottleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            lblAngleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            // grpTrainingLog
            // 
            grpTrainingLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingLog.Controls.Add(rtbTrainingLog);
            grpTrainingLog.Location = new Point(15, 461);
            grpTrainingLog.Margin = new Padding(10, 11, 10, 11);
            grpTrainingLog.Name = "grpTrainingLog";
            grpTrainingLog.Size = new Size(1547, 389);
            grpTrainingLog.TabIndex = 1;
            grpTrainingLog.TabStop = false;
            grpTrainingLog.Text = "학습 로그";
            // 
            // rtbTrainingLog
            // 
            rtbTrainingLog.Dock = DockStyle.Fill;
            rtbTrainingLog.Location = new Point(3, 23);
            rtbTrainingLog.Name = "rtbTrainingLog";
            rtbTrainingLog.Size = new Size(1541, 363);
            rtbTrainingLog.TabIndex = 0;
            rtbTrainingLog.Text = "";
            // 
            // grpTrainingSettings
            // 
            grpTrainingSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingSettings.Controls.Add(nudEpoch);
            grpTrainingSettings.Controls.Add(cbxModelType);
            grpTrainingSettings.Controls.Add(tbxTubPath);
            grpTrainingSettings.Controls.Add(tbxModelPath);
            grpTrainingSettings.Controls.Add(tbxMycarPath);
            grpTrainingSettings.Controls.Add(lblEpoch);
            grpTrainingSettings.Controls.Add(lblModeType);
            grpTrainingSettings.Controls.Add(lblModelPath);
            grpTrainingSettings.Controls.Add(lblTubPath);
            grpTrainingSettings.Controls.Add(lblMycarPath);
            grpTrainingSettings.Controls.Add(btnSaveSettings);
            grpTrainingSettings.Controls.Add(btnStopTraining);
            grpTrainingSettings.Controls.Add(btnStartTraining);
            grpTrainingSettings.Controls.Add(btnTubPath);
            grpTrainingSettings.Controls.Add(btnMycarPath);
            grpTrainingSettings.Location = new Point(15, 19);
            grpTrainingSettings.Name = "grpTrainingSettings";
            grpTrainingSettings.Size = new Size(1547, 437);
            grpTrainingSettings.TabIndex = 0;
            grpTrainingSettings.TabStop = false;
            grpTrainingSettings.Text = "학습 설정";
            // 
            // nudEpoch
            // 
            nudEpoch.Font = new Font("맑은 고딕", 12F);
            nudEpoch.Location = new Point(219, 286);
            nudEpoch.Name = "nudEpoch";
            nudEpoch.Size = new Size(391, 34);
            nudEpoch.TabIndex = 17;
            // 
            // cbxModelType
            // 
            cbxModelType.Font = new Font("맑은 고딕", 12F);
            cbxModelType.FormattingEnabled = true;
            cbxModelType.Location = new Point(219, 230);
            cbxModelType.Name = "cbxModelType";
            cbxModelType.Size = new Size(391, 36);
            cbxModelType.TabIndex = 2;
            // 
            // tbxTubPath
            // 
            tbxTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbxTubPath.Font = new Font("맑은 고딕", 12F);
            tbxTubPath.Location = new Point(219, 127);
            tbxTubPath.Name = "tbxTubPath";
            tbxTubPath.Size = new Size(1092, 34);
            tbxTubPath.TabIndex = 16;
            // 
            // tbxModelPath
            // 
            tbxModelPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbxModelPath.Font = new Font("맑은 고딕", 12F);
            tbxModelPath.Location = new Point(219, 178);
            tbxModelPath.Name = "tbxModelPath";
            tbxModelPath.Size = new Size(1092, 34);
            tbxModelPath.TabIndex = 14;
            // 
            // tbxMycarPath
            // 
            tbxMycarPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbxMycarPath.Font = new Font("맑은 고딕", 12F);
            tbxMycarPath.Location = new Point(219, 74);
            tbxMycarPath.Name = "tbxMycarPath";
            tbxMycarPath.Size = new Size(1092, 34);
            tbxMycarPath.TabIndex = 13;
            // 
            // lblEpoch
            // 
            lblEpoch.AutoSize = true;
            lblEpoch.Font = new Font("맑은 고딕", 12F);
            lblEpoch.Location = new Point(45, 287);
            lblEpoch.Name = "lblEpoch";
            lblEpoch.Size = new Size(99, 28);
            lblEpoch.TabIndex = 11;
            lblEpoch.Text = "학습 횟수";
            // 
            // lblModeType
            // 
            lblModeType.AutoSize = true;
            lblModeType.Font = new Font("맑은 고딕", 12F);
            lblModeType.Location = new Point(45, 232);
            lblModeType.Name = "lblModeType";
            lblModeType.Size = new Size(99, 28);
            lblModeType.TabIndex = 10;
            lblModeType.Text = "모델 종류";
            // 
            // lblModelPath
            // 
            lblModelPath.AutoSize = true;
            lblModelPath.Font = new Font("맑은 고딕", 12F);
            lblModelPath.Location = new Point(45, 182);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Size = new Size(146, 28);
            lblModelPath.TabIndex = 9;
            lblModelPath.Text = "모델 저장 경로";
            // 
            // lblTubPath
            // 
            lblTubPath.AutoSize = true;
            lblTubPath.Font = new Font("맑은 고딕", 12F);
            lblTubPath.Location = new Point(45, 130);
            lblTubPath.Name = "lblTubPath";
            lblTubPath.Size = new Size(94, 28);
            lblTubPath.TabIndex = 8;
            lblTubPath.Text = "Tub 경로";
            // 
            // lblMycarPath
            // 
            lblMycarPath.AutoSize = true;
            lblMycarPath.Font = new Font("맑은 고딕", 12F);
            lblMycarPath.Location = new Point(45, 78);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Size = new Size(113, 28);
            lblMycarPath.TabIndex = 7;
            lblMycarPath.Text = "mycar 경로";
            // 
            // btnSaveSettings
            // 
            btnSaveSettings.Font = new Font("맑은 고딕", 12F);
            btnSaveSettings.Location = new Point(1046, 347);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(194, 43);
            btnSaveSettings.TabIndex = 5;
            btnSaveSettings.Text = "설정 저장";
            btnSaveSettings.UseVisualStyleBackColor = true;
            // 
            // btnStopTraining
            // 
            btnStopTraining.Font = new Font("맑은 고딕", 12F);
            btnStopTraining.Location = new Point(713, 347);
            btnStopTraining.Name = "btnStopTraining";
            btnStopTraining.Size = new Size(194, 43);
            btnStopTraining.TabIndex = 4;
            btnStopTraining.Text = "학습 중지";
            btnStopTraining.UseVisualStyleBackColor = true;
            // 
            // btnStartTraining
            // 
            btnStartTraining.Font = new Font("맑은 고딕", 12F);
            btnStartTraining.Location = new Point(378, 347);
            btnStartTraining.Name = "btnStartTraining";
            btnStartTraining.Size = new Size(194, 43);
            btnStartTraining.TabIndex = 3;
            btnStartTraining.Text = "학습 시작";
            btnStartTraining.UseVisualStyleBackColor = true;
            // 
            // btnTubPath
            // 
            btnTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTubPath.Font = new Font("맑은 고딕", 12F);
            btnTubPath.Location = new Point(1335, 114);
            btnTubPath.Name = "btnTubPath";
            btnTubPath.Size = new Size(123, 43);
            btnTubPath.TabIndex = 2;
            btnTubPath.Text = "Tub 선택";
            btnTubPath.UseVisualStyleBackColor = true;
            // 
            // btnMycarPath
            // 
            btnMycarPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMycarPath.Font = new Font("맑은 고딕", 12F);
            btnMycarPath.Location = new Point(1335, 66);
            btnMycarPath.Name = "btnMycarPath";
            btnMycarPath.Size = new Size(123, 43);
            btnMycarPath.TabIndex = 1;
            btnMycarPath.Text = "찾기";
            btnMycarPath.UseVisualStyleBackColor = true;
            btnMycarPath.Click += btnMycarPath_Click;
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
            pnlGraphStats.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlGraphStats.Location = new Point(73, 75);
            pnlGraphStats.Name = "pnlGraphStats";
            pnlGraphStats.Size = new Size(921, 408);
            pnlGraphStats.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1581, 953);
            Controls.Add(tabControlMain);
            Controls.Add(menuStripMain);
            MainMenuStrip = menuStripMain;
            Margin = new Padding(4);
            MinimumSize = new Size(999, 698);
            Name = "Form1";
            Text = "Data Manager";
            Load += Form1_Load;
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabPageDataViewer.ResumeLayout(false);
            tabPageDataViewer.PerformLayout();
            statusStripDataFooter.ResumeLayout(false);
            statusStripDataFooter.PerformLayout();
            grpTubCleaner.ResumeLayout(false);
            grpTubCleaner.PerformLayout();
            grpTubExplorer.ResumeLayout(false);
            grpTubExplorer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkFramePosition).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPlaybackInterval).EndInit();
            splitContainerFramePreview.Panel1.ResumeLayout(false);
            splitContainerFramePreview.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerFramePreview).EndInit();
            splitContainerFramePreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lstFrameData).EndInit();
            ((System.ComponentModel.ISupportInitialize)picMainPreview).EndInit();
            tabPageTraining.ResumeLayout(false);
            tabPageTraining.PerformLayout();
            statusStripTraining.ResumeLayout(false);
            statusStripTraining.PerformLayout();
            grpTrainingLog.ResumeLayout(false);
            grpTrainingSettings.ResumeLayout(false);
            grpTrainingSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudEpoch).EndInit();
            tabPageGraphStats.ResumeLayout(false);
            tabPageGraphStats.PerformLayout();
            statusStripDataViewer.ResumeLayout(false);
            statusStripDataViewer.PerformLayout();
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
        private DataGridView lstFrameData;
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
        private SplitContainer splitContainerFramePreview;
        private GroupBox grpTubCleaner;
        private Label lblThrottleRange;
        private Label lblAngleRange;
        private TabPage tabPageTraining;
        private GroupBox grpTrainingSettings;
        private GroupBox grpTrainingLog;
        private Label lblEpoch;
        private Label lblModeType;
        private Label lblModelPath;
        private Label lblTubPath;
        private Label lblMycarPath;
        private Button btnSaveSettings;
        private Button btnStopTraining;
        private Button btnStartTraining;
        private Button btnTubPath;
        private Button btnMycarPath;
        private TextBox tbxTubPath;
        private TextBox tbxModelPath;
        private TextBox tbxMycarPath;
        private NumericUpDown nudEpoch;
        private ComboBox cbxModelType;
        private RichTextBox rtbTrainingLog;
        private StatusStrip statusStripTraining;
        private ToolStripStatusLabel toolStripStatusLabelTraining;
        private StatusStrip statusStripDataFooter;
        private ToolStripStatusLabel toolStripStatusLabelDataPath;
        private Label lblAngleRangeSeparator;
        private Label lblThrottleRangeSeparator;
        private TextBox txtThrottleMax;
        private TextBox txtAngleMax;
        private TextBox txtThrottleMin;
        private TextBox txtAngleMin;
        private ComboBox cbxScenarioFilter;
        private Label lblScenarioFilter;
        private ComboBox cbxModeFilter;
        private Label lblModeFilter;
        private Button btnRestoreFrame;
        private Button btnFrameSave;
        private Button btnDeleteFrame;
        private Button btnExcludeRange;
        private Button btnExcludeSelectedFrame;
        private Label lblSelectRange;
        private TextBox txtSelectRangeMin;
        private TextBox txtSelectRangeMax;
        private Label lblSelectRangeSeparator;
    }
}
