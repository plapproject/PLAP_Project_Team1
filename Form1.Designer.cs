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
            mnuFileOpenDataFolder = new ToolStripMenuItem();
            mnuFileReloadData = new ToolStripMenuItem();
            mnuExit = new ToolStripMenuItem();
            mnuView = new ToolStripMenuItem();
            mnuViewToggleTheme = new ToolStripMenuItem();
            mnuViewOpenGraphStats = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            mnuHelpOpenTutorial = new ToolStripMenuItem();
            tabControlMain = new TabControl();
            tabPageDataViewer = new TabPage();
            statusStripDataFooter = new StatusStrip();
            stsDataFooterPath = new ToolStripStatusLabel();
            grpDataCleaner = new GroupBox();
            lblFrameRangeSeparator = new Label();
            txtFrameRangeEnd = new TextBox();
            txtFrameRangeStart = new TextBox();
            lblFrameRange = new Label();
            btnRestoreFrames = new Button();
            btnSaveCleanupState = new Button();
            btnExportCleanDataset = new Button();
            btnExcludeFrameRange = new Button();
            btnExcludeSelectedFrames = new Button();
            cmbScenarioFilter = new ComboBox();
            lblScenarioFilter = new Label();
            cmbModeFilter = new ComboBox();
            lblModeFilter = new Label();
            lblAngleRangeSeparator = new Label();
            lblThrottleRangeSeparator = new Label();
            txtThrottleMaxFilter = new TextBox();
            txtAngleMaxFilter = new TextBox();
            txtThrottleMinFilter = new TextBox();
            txtAngleMinFilter = new TextBox();
            lblThrottleRange = new Label();
            lblAngleRange = new Label();
            btnApplyFrameFilter = new Button();
            btnClearFrameFilter = new Button();
            grpDataExplorer = new GroupBox();
            trkFrameTimeline = new TrackBar();
            numPlaybackIntervalMs = new NumericUpDown();
            btnOpenDataFolder = new Button();
            lblPlayInterval = new Label();
            btnLast = new Button();
            btnAutoPlay = new Button();
            btnNext = new Button();
            btnPrev = new Button();
            splitContainerFramePreview = new SplitContainer();
            dgvFrameCatalog = new DataGridView();
            picFramePreview = new PictureBox();
            btnFirst = new Button();
            btnReloadData = new Button();
            lblFrameValue = new Label();
            lblModeValue = new Label();
            btnToggleTheme = new Button();
            lblThrottleValue = new Label();
            btnGuide = new Button();
            lblAngleValue = new Label();
            tabTrainingMonitor = new TabPage();
            statusStripTraining = new StatusStrip();
            stsTrainingStatus = new ToolStripStatusLabel();
            grpTrainingOutput = new GroupBox();
            rtbTrainingOutput = new RichTextBox();
            grpTrainingConfig = new GroupBox();
            numTrainingEpochs = new NumericUpDown();
            txtTrainingTubPath = new TextBox();
            txtTrainingModelPath = new TextBox();
            lblEpoch = new Label();
            lblTrainingModelPath = new Label();
            lblTrainingTubPath = new Label();
            btnSaveTrainingConfig = new Button();
            btnStopTrainingProcess = new Button();
            btnStartTrainingProcess = new Button();
            btnSelectTrainingTubPath = new Button();
            tabGraphStats = new TabPage();
            pnlChartHost = new Panel();
            lblChartDescription = new Label();
            statusStripDataViewer = new StatusStrip();
            stsDataPath = new ToolStripStatusLabel();
            stsFrameSummary = new ToolStripStatusLabel();
            menuStripMain.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageDataViewer.SuspendLayout();
            statusStripDataFooter.SuspendLayout();
            grpDataCleaner.SuspendLayout();
            grpDataExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkFrameTimeline).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPlaybackIntervalMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerFramePreview).BeginInit();
            splitContainerFramePreview.Panel1.SuspendLayout();
            splitContainerFramePreview.Panel2.SuspendLayout();
            splitContainerFramePreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFrameCatalog).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picFramePreview).BeginInit();
            tabTrainingMonitor.SuspendLayout();
            statusStripTraining.SuspendLayout();
            grpTrainingOutput.SuspendLayout();
            grpTrainingConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTrainingEpochs).BeginInit();
            tabGraphStats.SuspendLayout();
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
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileOpenDataFolder, mnuFileReloadData, mnuExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(53, 24);
            mnuFile.Text = "파일";
            // 
            // mnuFileOpenDataFolder
            // 
            mnuFileOpenDataFolder.Name = "mnuFileOpenDataFolder";
            mnuFileOpenDataFolder.Size = new Size(207, 26);
            mnuFileOpenDataFolder.Text = "데이터 폴더 열기";
            // 
            // mnuFileReloadData
            // 
            mnuFileReloadData.Name = "mnuFileReloadData";
            mnuFileReloadData.Size = new Size(207, 26);
            mnuFileReloadData.Text = "다시 불러오기";
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(207, 26);
            mnuExit.Text = "종료";
            // 
            // mnuView
            // 
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuViewToggleTheme, mnuViewOpenGraphStats });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(53, 24);
            mnuView.Text = "보기";
            // 
            // mnuViewToggleTheme
            // 
            mnuViewToggleTheme.Name = "mnuViewToggleTheme";
            mnuViewToggleTheme.Size = new Size(258, 26);
            mnuViewToggleTheme.Text = "테마 전환";
            // 
            // mnuViewOpenGraphStats
            // 
            mnuViewOpenGraphStats.Name = "mnuViewOpenGraphStats";
            mnuViewOpenGraphStats.Size = new Size(258, 26);
            mnuViewOpenGraphStats.Text = "그래프/통계 탭으로 이동";
            mnuViewOpenGraphStats.Click += MnuViewOpenGraphStats_Click;
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpOpenTutorial });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(68, 24);
            mnuHelp.Text = "도움말";
            mnuHelp.Click += MnuHelp_Click;
            // 
            // mnuHelpOpenTutorial
            // 
            mnuHelpOpenTutorial.Name = "mnuHelpOpenTutorial";
            mnuHelpOpenTutorial.Size = new Size(202, 26);
            mnuHelpOpenTutorial.Text = "기능별 튜토리얼";
            // 
            // tabControlMain
            // 
            tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlMain.Controls.Add(tabPageDataViewer);
            tabControlMain.Controls.Add(tabTrainingMonitor);
            tabControlMain.Controls.Add(tabGraphStats);
            tabControlMain.Location = new Point(3, 31);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1580, 920);
            tabControlMain.TabIndex = 1;
            // 
            // tabPageDataViewer
            // 
            tabPageDataViewer.Controls.Add(statusStripDataFooter);
            tabPageDataViewer.Controls.Add(grpDataCleaner);
            tabPageDataViewer.Controls.Add(grpDataExplorer);
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
            statusStripDataFooter.Items.AddRange(new ToolStripItem[] { stsDataFooterPath });
            statusStripDataFooter.Location = new Point(3, 858);
            statusStripDataFooter.Name = "statusStripDataFooter";
            statusStripDataFooter.Size = new Size(1566, 26);
            statusStripDataFooter.TabIndex = 24;
            statusStripDataFooter.Text = "statusStripDataFooter";
            // 
            // stsDataFooterPath
            // 
            stsDataFooterPath.Name = "stsDataFooterPath";
            stsDataFooterPath.Size = new Size(33, 20);
            stsDataFooterPath.Text = "C:\\";
            // 
            // grpDataCleaner
            // 
            grpDataCleaner.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpDataCleaner.Controls.Add(lblFrameRangeSeparator);
            grpDataCleaner.Controls.Add(txtFrameRangeEnd);
            grpDataCleaner.Controls.Add(txtFrameRangeStart);
            grpDataCleaner.Controls.Add(lblFrameRange);
            grpDataCleaner.Controls.Add(btnRestoreFrames);
            grpDataCleaner.Controls.Add(btnSaveCleanupState);
            grpDataCleaner.Controls.Add(btnExportCleanDataset);
            grpDataCleaner.Controls.Add(btnExcludeFrameRange);
            grpDataCleaner.Controls.Add(btnExcludeSelectedFrames);
            grpDataCleaner.Controls.Add(cmbScenarioFilter);
            grpDataCleaner.Controls.Add(lblScenarioFilter);
            grpDataCleaner.Controls.Add(cmbModeFilter);
            grpDataCleaner.Controls.Add(lblModeFilter);
            grpDataCleaner.Controls.Add(lblAngleRangeSeparator);
            grpDataCleaner.Controls.Add(lblThrottleRangeSeparator);
            grpDataCleaner.Controls.Add(txtThrottleMaxFilter);
            grpDataCleaner.Controls.Add(txtAngleMaxFilter);
            grpDataCleaner.Controls.Add(txtThrottleMinFilter);
            grpDataCleaner.Controls.Add(txtAngleMinFilter);
            grpDataCleaner.Controls.Add(lblThrottleRange);
            grpDataCleaner.Controls.Add(lblAngleRange);
            grpDataCleaner.Controls.Add(btnApplyFrameFilter);
            grpDataCleaner.Controls.Add(btnClearFrameFilter);
            grpDataCleaner.Location = new Point(3, 635);
            grpDataCleaner.Name = "grpDataCleaner";
            grpDataCleaner.Size = new Size(1563, 220);
            grpDataCleaner.TabIndex = 23;
            grpDataCleaner.TabStop = false;
            grpDataCleaner.Text = "터브 정리기";
            grpDataCleaner.Enter += GrpDataCleaner_Enter;
            // 
            // lblFrameRangeSeparator
            // 
            lblFrameRangeSeparator.AutoSize = true;
            lblFrameRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblFrameRangeSeparator.Location = new Point(402, 69);
            lblFrameRangeSeparator.Name = "lblFrameRangeSeparator";
            lblFrameRangeSeparator.Size = new Size(27, 23);
            lblFrameRangeSeparator.TabIndex = 47;
            lblFrameRangeSeparator.Text = "～";
            // 
            // txtFrameRangeEnd
            // 
            txtFrameRangeEnd.Location = new Point(435, 67);
            txtFrameRangeEnd.Margin = new Padding(4);
            txtFrameRangeEnd.Name = "txtFrameRangeEnd";
            txtFrameRangeEnd.Size = new Size(130, 27);
            txtFrameRangeEnd.TabIndex = 46;
            // 
            // txtFrameRangeStart
            // 
            txtFrameRangeStart.Location = new Point(266, 67);
            txtFrameRangeStart.Margin = new Padding(4);
            txtFrameRangeStart.Name = "txtFrameRangeStart";
            txtFrameRangeStart.Size = new Size(130, 27);
            txtFrameRangeStart.TabIndex = 45;
            // 
            // lblFrameRange
            // 
            lblFrameRange.AutoSize = true;
            lblFrameRange.Font = new Font("맑은 고딕", 10F);
            lblFrameRange.Location = new Point(96, 70);
            lblFrameRange.Margin = new Padding(4, 0, 4, 0);
            lblFrameRange.Name = "lblFrameRange";
            lblFrameRange.Size = new Size(84, 23);
            lblFrameRange.TabIndex = 44;
            lblFrameRange.Text = "구간 선택";
            // 
            // btnRestoreFrames
            // 
            btnRestoreFrames.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRestoreFrames.Location = new Point(1266, 143);
            btnRestoreFrames.Name = "btnRestoreFrames";
            btnRestoreFrames.Size = new Size(215, 29);
            btnRestoreFrames.TabIndex = 43;
            btnRestoreFrames.Text = "복구";
            btnRestoreFrames.UseVisualStyleBackColor = true;
            // 
            // btnSaveCleanupState
            // 
            btnSaveCleanupState.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveCleanupState.Location = new Point(1266, 180);
            btnSaveCleanupState.Name = "btnSaveCleanupState";
            btnSaveCleanupState.Size = new Size(215, 29);
            btnSaveCleanupState.TabIndex = 48;
            btnSaveCleanupState.Text = "상태 저장";
            btnSaveCleanupState.UseVisualStyleBackColor = true;
            // 
            // btnExportCleanDataset
            // 
            btnExportCleanDataset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportCleanDataset.Location = new Point(1035, 141);
            btnExportCleanDataset.Name = "btnExportCleanDataset";
            btnExportCleanDataset.Size = new Size(215, 29);
            btnExportCleanDataset.TabIndex = 42;
            btnExportCleanDataset.Text = "삭제";
            btnExportCleanDataset.UseVisualStyleBackColor = true;
            // 
            // btnExcludeFrameRange
            // 
            btnExcludeFrameRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeFrameRange.Location = new Point(1035, 66);
            btnExcludeFrameRange.Name = "btnExcludeFrameRange";
            btnExcludeFrameRange.Size = new Size(215, 29);
            btnExcludeFrameRange.TabIndex = 40;
            btnExcludeFrameRange.Text = "구간 제외";
            btnExcludeFrameRange.UseVisualStyleBackColor = true;
            // 
            // btnExcludeSelectedFrames
            // 
            btnExcludeSelectedFrames.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeSelectedFrames.Location = new Point(1266, 68);
            btnExcludeSelectedFrames.Name = "btnExcludeSelectedFrames";
            btnExcludeSelectedFrames.Size = new Size(215, 29);
            btnExcludeSelectedFrames.TabIndex = 41;
            btnExcludeSelectedFrames.Text = "선택 프레임 제외";
            btnExcludeSelectedFrames.UseVisualStyleBackColor = true;
            // 
            // cmbScenarioFilter
            // 
            cmbScenarioFilter.Font = new Font("맑은 고딕", 10F);
            cmbScenarioFilter.FormattingEnabled = true;
            cmbScenarioFilter.Items.AddRange(new object[] { "All", "Normal", "Night", "Left_turn", "Right_turn", "Out_of_bound" });
            cmbScenarioFilter.Location = new Point(705, 142);
            cmbScenarioFilter.Name = "cmbScenarioFilter";
            cmbScenarioFilter.Size = new Size(277, 31);
            cmbScenarioFilter.TabIndex = 39;
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
            // cmbModeFilter
            // 
            cmbModeFilter.Font = new Font("맑은 고딕", 10F);
            cmbModeFilter.FormattingEnabled = true;
            cmbModeFilter.Items.AddRange(new object[] { "All", "User", "Local", "Local_angle" });
            cmbModeFilter.Location = new Point(705, 101);
            cmbModeFilter.Name = "cmbModeFilter";
            cmbModeFilter.Size = new Size(277, 31);
            cmbModeFilter.TabIndex = 37;
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
            // txtThrottleMaxFilter
            // 
            txtThrottleMaxFilter.Font = new Font("맑은 고딕", 10F);
            txtThrottleMaxFilter.Location = new Point(435, 147);
            txtThrottleMaxFilter.Name = "txtThrottleMaxFilter";
            txtThrottleMaxFilter.Size = new Size(130, 30);
            txtThrottleMaxFilter.TabIndex = 33;
            // 
            // txtAngleMaxFilter
            // 
            txtAngleMaxFilter.Font = new Font("맑은 고딕", 10F);
            txtAngleMaxFilter.Location = new Point(435, 105);
            txtAngleMaxFilter.Name = "txtAngleMaxFilter";
            txtAngleMaxFilter.Size = new Size(130, 30);
            txtAngleMaxFilter.TabIndex = 32;
            // 
            // txtThrottleMinFilter
            // 
            txtThrottleMinFilter.Font = new Font("맑은 고딕", 10F);
            txtThrottleMinFilter.Location = new Point(266, 147);
            txtThrottleMinFilter.Name = "txtThrottleMinFilter";
            txtThrottleMinFilter.Size = new Size(130, 30);
            txtThrottleMinFilter.TabIndex = 31;
            // 
            // txtAngleMinFilter
            // 
            txtAngleMinFilter.Font = new Font("맑은 고딕", 10F);
            txtAngleMinFilter.Location = new Point(266, 105);
            txtAngleMinFilter.Name = "txtAngleMinFilter";
            txtAngleMinFilter.Size = new Size(130, 30);
            txtAngleMinFilter.TabIndex = 30;
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
            // btnApplyFrameFilter
            // 
            btnApplyFrameFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFrameFilter.Location = new Point(1036, 103);
            btnApplyFrameFilter.Name = "btnApplyFrameFilter";
            btnApplyFrameFilter.Size = new Size(215, 29);
            btnApplyFrameFilter.TabIndex = 12;
            btnApplyFrameFilter.Text = "필터 적용";
            btnApplyFrameFilter.UseVisualStyleBackColor = true;
            btnApplyFrameFilter.Click += BtnApplyFrameFilter_Click;
            // 
            // btnClearFrameFilter
            // 
            btnClearFrameFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFrameFilter.Location = new Point(1266, 105);
            btnClearFrameFilter.Name = "btnClearFrameFilter";
            btnClearFrameFilter.Size = new Size(215, 29);
            btnClearFrameFilter.TabIndex = 19;
            btnClearFrameFilter.Text = "필터 해제";
            btnClearFrameFilter.UseVisualStyleBackColor = true;
            // 
            // grpDataExplorer
            // 
            grpDataExplorer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpDataExplorer.Controls.Add(trkFrameTimeline);
            grpDataExplorer.Controls.Add(numPlaybackIntervalMs);
            grpDataExplorer.Controls.Add(btnOpenDataFolder);
            grpDataExplorer.Controls.Add(lblPlayInterval);
            grpDataExplorer.Controls.Add(btnLast);
            grpDataExplorer.Controls.Add(btnAutoPlay);
            grpDataExplorer.Controls.Add(btnNext);
            grpDataExplorer.Controls.Add(btnPrev);
            grpDataExplorer.Controls.Add(splitContainerFramePreview);
            grpDataExplorer.Controls.Add(btnFirst);
            grpDataExplorer.Controls.Add(btnReloadData);
            grpDataExplorer.Controls.Add(lblFrameValue);
            grpDataExplorer.Controls.Add(lblModeValue);
            grpDataExplorer.Controls.Add(btnToggleTheme);
            grpDataExplorer.Controls.Add(lblThrottleValue);
            grpDataExplorer.Controls.Add(btnGuide);
            grpDataExplorer.Controls.Add(lblAngleValue);
            grpDataExplorer.Location = new Point(0, 0);
            grpDataExplorer.Name = "grpDataExplorer";
            grpDataExplorer.Size = new Size(1569, 635);
            grpDataExplorer.TabIndex = 22;
            grpDataExplorer.TabStop = false;
            grpDataExplorer.Text = "터브 탐색기";
            // 
            // trkFrameTimeline
            // 
            trkFrameTimeline.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trkFrameTimeline.Location = new Point(-1, 571);
            trkFrameTimeline.Maximum = 0;
            trkFrameTimeline.Name = "trkFrameTimeline";
            trkFrameTimeline.Size = new Size(1563, 56);
            trkFrameTimeline.TabIndex = 18;
            trkFrameTimeline.Scroll += TrkFrameTimeline_Scroll;
            // 
            // numPlaybackIntervalMs
            // 
            numPlaybackIntervalMs.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numPlaybackIntervalMs.Location = new Point(1403, 329);
            numPlaybackIntervalMs.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numPlaybackIntervalMs.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numPlaybackIntervalMs.Name = "numPlaybackIntervalMs";
            numPlaybackIntervalMs.Size = new Size(156, 27);
            numPlaybackIntervalMs.TabIndex = 11;
            numPlaybackIntervalMs.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // btnOpenDataFolder
            // 
            btnOpenDataFolder.Location = new Point(0, 27);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new Size(140, 29);
            btnOpenDataFolder.TabIndex = 0;
            btnOpenDataFolder.Text = "데이터 폴더 열기";
            btnOpenDataFolder.UseVisualStyleBackColor = true;
            btnOpenDataFolder.Click += BtnOpenDataFolder_Click;
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
            lblPlayInterval.Click += LblPlayInterval_Click;
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
            splitContainerFramePreview.Panel1.Controls.Add(dgvFrameCatalog);
            // 
            // splitContainerFramePreview.Panel2
            // 
            splitContainerFramePreview.Panel2.Controls.Add(picFramePreview);
            splitContainerFramePreview.Size = new Size(1278, 503);
            splitContainerFramePreview.SplitterDistance = 360;
            splitContainerFramePreview.SplitterWidth = 6;
            splitContainerFramePreview.TabIndex = 44;
            // 
            // dgvFrameCatalog
            // 
            dgvFrameCatalog.AllowUserToAddRows = false;
            dgvFrameCatalog.AllowUserToDeleteRows = false;
            dgvFrameCatalog.AllowUserToResizeRows = false;
            dgvFrameCatalog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFrameCatalog.Dock = DockStyle.Fill;
            dgvFrameCatalog.Location = new Point(0, 0);
            dgvFrameCatalog.Name = "dgvFrameCatalog";
            dgvFrameCatalog.ReadOnly = true;
            dgvFrameCatalog.RowHeadersVisible = false;
            dgvFrameCatalog.RowHeadersWidth = 51;
            dgvFrameCatalog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFrameCatalog.Size = new Size(360, 503);
            dgvFrameCatalog.TabIndex = 13;
            dgvFrameCatalog.SelectionChanged += DgvFrameCatalog_SelectionChanged;
            // 
            // picFramePreview
            // 
            picFramePreview.BorderStyle = BorderStyle.FixedSingle;
            picFramePreview.Dock = DockStyle.Fill;
            picFramePreview.Location = new Point(0, 0);
            picFramePreview.Name = "picFramePreview";
            picFramePreview.Size = new Size(912, 503);
            picFramePreview.SizeMode = PictureBoxSizeMode.Zoom;
            picFramePreview.TabIndex = 4;
            picFramePreview.TabStop = false;
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
            // btnReloadData
            // 
            btnReloadData.Location = new Point(147, 27);
            btnReloadData.Name = "btnReloadData";
            btnReloadData.Size = new Size(140, 29);
            btnReloadData.TabIndex = 1;
            btnReloadData.Text = "다시 불러오기";
            btnReloadData.UseVisualStyleBackColor = true;
            btnReloadData.Click += BtnReloadData_Click;
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
            lblModeValue.Click += LblModeValue_Click;
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
            lblThrottleValue.Click += LblThrottleValue_Click;
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
            lblAngleValue.Click += LblAngleValue_Click;
            // 
            // tabTrainingMonitor
            // 
            tabTrainingMonitor.Controls.Add(statusStripTraining);
            tabTrainingMonitor.Controls.Add(grpTrainingOutput);
            tabTrainingMonitor.Controls.Add(grpTrainingConfig);
            tabTrainingMonitor.Location = new Point(4, 29);
            tabTrainingMonitor.Name = "tabTrainingMonitor";
            tabTrainingMonitor.Padding = new Padding(3);
            tabTrainingMonitor.Size = new Size(1572, 887);
            tabTrainingMonitor.TabIndex = 1;
            tabTrainingMonitor.Text = "학습 실행";
            tabTrainingMonitor.UseVisualStyleBackColor = true;
            tabTrainingMonitor.Click += TabTrainingMonitor_Click;
            // 
            // statusStripTraining
            // 
            statusStripTraining.ImageScalingSize = new Size(20, 20);
            statusStripTraining.Items.AddRange(new ToolStripItem[] { stsTrainingStatus });
            statusStripTraining.Location = new Point(3, 858);
            statusStripTraining.Name = "statusStripTraining";
            statusStripTraining.Size = new Size(1566, 26);
            statusStripTraining.TabIndex = 2;
            statusStripTraining.Text = "statusStripTraining";
            // 
            // stsTrainingStatus
            // 
            stsTrainingStatus.Name = "stsTrainingStatus";
            stsTrainingStatus.Size = new Size(102, 20);
            stsTrainingStatus.Text = "Tub 경로: C:\\";
            stsTrainingStatus.Click += StsTrainingStatus_Click;
            // 
            // grpTrainingOutput
            // 
            grpTrainingOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingOutput.Controls.Add(rtbTrainingOutput);
            grpTrainingOutput.Location = new Point(15, 461);
            grpTrainingOutput.Margin = new Padding(10, 11, 10, 11);
            grpTrainingOutput.Name = "grpTrainingOutput";
            grpTrainingOutput.Size = new Size(1547, 389);
            grpTrainingOutput.TabIndex = 1;
            grpTrainingOutput.TabStop = false;
            grpTrainingOutput.Text = "학습 로그";
            // 
            // rtbTrainingOutput
            // 
            rtbTrainingOutput.Dock = DockStyle.Fill;
            rtbTrainingOutput.Location = new Point(3, 23);
            rtbTrainingOutput.Name = "rtbTrainingOutput";
            rtbTrainingOutput.Size = new Size(1541, 363);
            rtbTrainingOutput.TabIndex = 0;
            rtbTrainingOutput.Text = "";
            // 
            // grpTrainingConfig
            // 
            grpTrainingConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingConfig.Controls.Add(numTrainingEpochs);
            grpTrainingConfig.Controls.Add(txtTrainingTubPath);
            grpTrainingConfig.Controls.Add(txtTrainingModelPath);
            grpTrainingConfig.Controls.Add(lblEpoch);
            grpTrainingConfig.Controls.Add(lblTrainingModelPath);
            grpTrainingConfig.Controls.Add(lblTrainingTubPath);
            grpTrainingConfig.Controls.Add(btnSaveTrainingConfig);
            grpTrainingConfig.Controls.Add(btnStopTrainingProcess);
            grpTrainingConfig.Controls.Add(btnStartTrainingProcess);
            grpTrainingConfig.Controls.Add(btnSelectTrainingTubPath);
            grpTrainingConfig.Location = new Point(15, 19);
            grpTrainingConfig.Name = "grpTrainingConfig";
            grpTrainingConfig.Size = new Size(1547, 437);
            grpTrainingConfig.TabIndex = 0;
            grpTrainingConfig.TabStop = false;
            grpTrainingConfig.Text = "학습 설정";
            // 
            // numTrainingEpochs
            // 
            numTrainingEpochs.Font = new Font("맑은 고딕", 12F);
            numTrainingEpochs.Location = new Point(219, 321);
            numTrainingEpochs.Name = "numTrainingEpochs";
            numTrainingEpochs.Size = new Size(391, 34);
            numTrainingEpochs.TabIndex = 17;
            // 
            // txtTrainingTubPath
            // 
            txtTrainingTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTrainingTubPath.Font = new Font("맑은 고딕", 12F);
            txtTrainingTubPath.Location = new Point(219, 127);
            txtTrainingTubPath.Name = "txtTrainingTubPath";
            txtTrainingTubPath.Size = new Size(1092, 34);
            txtTrainingTubPath.TabIndex = 16;
            // 
            // txtTrainingModelPath
            // 
            txtTrainingModelPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTrainingModelPath.Font = new Font("맑은 고딕", 12F);
            txtTrainingModelPath.Location = new Point(219, 178);
            txtTrainingModelPath.Name = "txtTrainingModelPath";
            txtTrainingModelPath.Size = new Size(1092, 34);
            txtTrainingModelPath.TabIndex = 14;
            // 
            // lblEpoch
            // 
            lblEpoch.AutoSize = true;
            lblEpoch.Font = new Font("맑은 고딕", 12F);
            lblEpoch.Location = new Point(45, 322);
            lblEpoch.Name = "lblEpoch";
            lblEpoch.Size = new Size(99, 28);
            lblEpoch.TabIndex = 11;
            lblEpoch.Text = "학습 횟수";
            // 
            // lblTrainingModelPath
            // 
            lblTrainingModelPath.AutoSize = true;
            lblTrainingModelPath.Font = new Font("맑은 고딕", 12F);
            lblTrainingModelPath.Location = new Point(45, 182);
            lblTrainingModelPath.Name = "lblTrainingModelPath";
            lblTrainingModelPath.Size = new Size(146, 28);
            lblTrainingModelPath.TabIndex = 9;
            lblTrainingModelPath.Text = "모델 저장 경로";
            // 
            // lblTrainingTubPath
            // 
            lblTrainingTubPath.AutoSize = true;
            lblTrainingTubPath.Font = new Font("맑은 고딕", 12F);
            lblTrainingTubPath.Location = new Point(45, 130);
            lblTrainingTubPath.Name = "lblTrainingTubPath";
            lblTrainingTubPath.Size = new Size(94, 28);
            lblTrainingTubPath.TabIndex = 8;
            lblTrainingTubPath.Text = "Tub 경로";
            // 
            // btnSaveTrainingConfig
            // 
            btnSaveTrainingConfig.Font = new Font("맑은 고딕", 12F);
            btnSaveTrainingConfig.Location = new Point(1045, 368);
            btnSaveTrainingConfig.Name = "btnSaveTrainingConfig";
            btnSaveTrainingConfig.Size = new Size(194, 43);
            btnSaveTrainingConfig.TabIndex = 5;
            btnSaveTrainingConfig.Text = "설정 저장";
            btnSaveTrainingConfig.UseVisualStyleBackColor = true;
            // 
            // btnStopTrainingProcess
            // 
            btnStopTrainingProcess.Font = new Font("맑은 고딕", 12F);
            btnStopTrainingProcess.Location = new Point(713, 368);
            btnStopTrainingProcess.Name = "btnStopTrainingProcess";
            btnStopTrainingProcess.Size = new Size(194, 43);
            btnStopTrainingProcess.TabIndex = 4;
            btnStopTrainingProcess.Text = "학습 중지";
            btnStopTrainingProcess.UseVisualStyleBackColor = true;
            // 
            // btnStartTrainingProcess
            // 
            btnStartTrainingProcess.Font = new Font("맑은 고딕", 12F);
            btnStartTrainingProcess.Location = new Point(378, 368);
            btnStartTrainingProcess.Name = "btnStartTrainingProcess";
            btnStartTrainingProcess.Size = new Size(194, 43);
            btnStartTrainingProcess.TabIndex = 3;
            btnStartTrainingProcess.Text = "학습 시작";
            btnStartTrainingProcess.UseVisualStyleBackColor = true;
            // 
            // btnSelectTrainingTubPath
            // 
            btnSelectTrainingTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectTrainingTubPath.Font = new Font("맑은 고딕", 12F);
            btnSelectTrainingTubPath.Location = new Point(1335, 114);
            btnSelectTrainingTubPath.Name = "btnSelectTrainingTubPath";
            btnSelectTrainingTubPath.Size = new Size(123, 43);
            btnSelectTrainingTubPath.TabIndex = 2;
            btnSelectTrainingTubPath.Text = "Tub 선택";
            btnSelectTrainingTubPath.UseVisualStyleBackColor = true;
            // 
            // tabGraphStats
            // 
            tabGraphStats.Controls.Add(pnlChartHost);
            tabGraphStats.Controls.Add(lblChartDescription);
            tabGraphStats.Location = new Point(4, 29);
            tabGraphStats.Name = "tabGraphStats";
            tabGraphStats.Padding = new Padding(3);
            tabGraphStats.Size = new Size(1572, 887);
            tabGraphStats.TabIndex = 2;
            tabGraphStats.Text = "그래프/통계";
            tabGraphStats.UseVisualStyleBackColor = true;
            // 
            // pnlChartHost
            // 
            pnlChartHost.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlChartHost.Location = new Point(73, 75);
            pnlChartHost.Name = "pnlChartHost";
            pnlChartHost.Size = new Size(921, 408);
            pnlChartHost.TabIndex = 1;
            // 
            // lblChartDescription
            // 
            lblChartDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblChartDescription.AutoSize = true;
            lblChartDescription.Location = new Point(73, 27);
            lblChartDescription.Name = "lblChartDescription";
            lblChartDescription.Size = new Size(921, 20);
            lblChartDescription.TabIndex = 0;
            lblChartDescription.Text = "조향값과 스로틀값 변화 추이를 시각화합니다. 파란색은 조향, 초록색은 스로틀입니다. 필터 적용 후 보이는 데이터 기준으로 갱신됩니다.";
            // 
            // statusStripDataViewer
            // 
            statusStripDataViewer.ImageScalingSize = new Size(20, 20);
            statusStripDataViewer.Items.AddRange(new ToolStripItem[] { stsDataPath, stsFrameSummary });
            statusStripDataViewer.Location = new Point(0, 0);
            statusStripDataViewer.Name = "statusStripDataViewer";
            statusStripDataViewer.Size = new Size(200, 22);
            statusStripDataViewer.TabIndex = 0;
            // 
            // stsDataPath
            // 
            stsDataPath.Name = "stsDataPath";
            stsDataPath.Size = new Size(53, 16);
            stsDataPath.Text = "경로: -";
            // 
            // stsFrameSummary
            // 
            stsFrameSummary.Name = "stsFrameSummary";
            stsFrameSummary.Size = new Size(72, 16);
            stsFrameSummary.Text = "Frames: 0";
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
            grpDataCleaner.ResumeLayout(false);
            grpDataCleaner.PerformLayout();
            grpDataExplorer.ResumeLayout(false);
            grpDataExplorer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkFrameTimeline).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPlaybackIntervalMs).EndInit();
            splitContainerFramePreview.Panel1.ResumeLayout(false);
            splitContainerFramePreview.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerFramePreview).EndInit();
            splitContainerFramePreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvFrameCatalog).EndInit();
            ((System.ComponentModel.ISupportInitialize)picFramePreview).EndInit();
            tabTrainingMonitor.ResumeLayout(false);
            tabTrainingMonitor.PerformLayout();
            statusStripTraining.ResumeLayout(false);
            statusStripTraining.PerformLayout();
            grpTrainingOutput.ResumeLayout(false);
            grpTrainingConfig.ResumeLayout(false);
            grpTrainingConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTrainingEpochs).EndInit();
            tabGraphStats.ResumeLayout(false);
            tabGraphStats.PerformLayout();
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
        private TabPage tabGraphStats;
        private Button btnOpenDataFolder;
        private Button btnReloadData;
        private Button btnToggleTheme;
        private Button btnGuide;
        private PictureBox picFramePreview;
        private Label lblFrameValue;
        private Label lblAngleValue;
        private Label lblThrottleValue;
        private Label lblModeValue;
        private NumericUpDown numPlaybackIntervalMs;
        private Button btnApplyFrameFilter;
        private DataGridView dgvFrameCatalog;
        private Button btnFirst;
        private Button btnPrev;
        private Button btnNext;
        private Button btnLast;
        private TrackBar trkFrameTimeline;
        private StatusStrip statusStripDataViewer;
        private ToolStripStatusLabel stsDataPath;
        private ToolStripStatusLabel stsFrameSummary;
        private Button btnClearFrameFilter;
        private Label lblChartDescription;
        private Panel pnlChartHost;
        private ToolStripMenuItem mnuViewToggleTheme;
        private ToolStripMenuItem mnuViewOpenGraphStats;
        private ToolStripMenuItem mnuHelpOpenTutorial;
        private ToolStripMenuItem mnuFileOpenDataFolder;
        private ToolStripMenuItem mnuFileReloadData;
        private ToolStripMenuItem mnuExit;
        private Button btnAutoPlay;
        private Label lblPlayInterval;
        private GroupBox grpDataExplorer;
        private SplitContainer splitContainerFramePreview;
        private GroupBox grpDataCleaner;
        private Label lblThrottleRange;
        private Label lblAngleRange;
        private TabPage tabTrainingMonitor;
        private GroupBox grpTrainingConfig;
        private GroupBox grpTrainingOutput;
        private Label lblEpoch;
        private Label lblTrainingModelPath;
        private Label lblTrainingTubPath;
        private Button btnSaveTrainingConfig;
        private Button btnStopTrainingProcess;
        private Button btnStartTrainingProcess;
        private Button btnSelectTrainingTubPath;
        private TextBox txtTrainingTubPath;
        private TextBox txtTrainingModelPath;
        private NumericUpDown numTrainingEpochs;
        private RichTextBox rtbTrainingOutput;
        private StatusStrip statusStripTraining;
        private ToolStripStatusLabel stsTrainingStatus;
        private StatusStrip statusStripDataFooter;
        private ToolStripStatusLabel stsDataFooterPath;
        private Label lblAngleRangeSeparator;
        private Label lblThrottleRangeSeparator;
        private TextBox txtThrottleMaxFilter;
        private TextBox txtAngleMaxFilter;
        private TextBox txtThrottleMinFilter;
        private TextBox txtAngleMinFilter;
        private ComboBox cmbScenarioFilter;
        private Label lblScenarioFilter;
        private ComboBox cmbModeFilter;
        private Label lblModeFilter;
        private Button btnRestoreFrames;
        private Button btnSaveCleanupState;
        private Button btnExportCleanDataset;
        private Button btnExcludeFrameRange;
        private Button btnExcludeSelectedFrames;
        private Label lblFrameRange;
        private TextBox txtFrameRangeStart;
        private TextBox txtFrameRangeEnd;
        private Label lblFrameRangeSeparator;
    }
}
