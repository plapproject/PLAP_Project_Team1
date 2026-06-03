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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
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
            btnSelectCondaPath = new Button();
            btnSelectTrainingWslDistro = new Button();
            cmbCondaPath = new ComboBox();
            lblCondaPath = new Label();
            cmbTrainingWslDistro = new ComboBox();
            lblTrainingWslDistro = new Label();
            numTrainingEpochs = new NumericUpDown();
            cmbTrainingModelType = new ComboBox();
            txtTrainingPythonEnvName = new TextBox();
            txtTrainingTubPath = new TextBox();
            txtTrainingModelPath = new TextBox();
            cmbMycarProjectPath = new ComboBox();
            lblEpoch = new Label();
            lblTrainingModelType = new Label();
            lblTrainingPythonEnvName = new Label();
            lblTrainingModelPath = new Label();
            lblTrainingTubPath = new Label();
            lblMycarProjectPath = new Label();
            btnSaveTrainingConfig = new Button();
            btnStopTrainingProcess = new Button();
            btnStartTrainingProcess = new Button();
            btnDetectTrainingEnvironment = new Button();
            btnSelectTrainingModelPath = new Button();
            btnSelectTrainingTubPath = new Button();
            btnSelectMycarPath = new Button();
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
            menuStripMain.Padding = new Padding(5, 2, 0, 2);
            menuStripMain.Size = new Size(1230, 24);
            menuStripMain.TabIndex = 0;
            menuStripMain.Text = "menuStripMain";
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileOpenDataFolder, mnuFileReloadData, mnuExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(43, 20);
            mnuFile.Text = "파일";
            // 
            // mnuFileOpenDataFolder
            // 
            mnuFileOpenDataFolder.Name = "mnuFileOpenDataFolder";
            mnuFileOpenDataFolder.Size = new Size(166, 22);
            mnuFileOpenDataFolder.Text = "데이터 폴더 열기";
            // 
            // mnuFileReloadData
            // 
            mnuFileReloadData.Name = "mnuFileReloadData";
            mnuFileReloadData.Size = new Size(166, 22);
            mnuFileReloadData.Text = "다시 불러오기";
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(166, 22);
            mnuExit.Text = "종료";
            // 
            // mnuView
            // 
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuViewToggleTheme, mnuViewOpenGraphStats });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(43, 20);
            mnuView.Text = "보기";
            // 
            // mnuViewToggleTheme
            // 
            mnuViewToggleTheme.Name = "mnuViewToggleTheme";
            mnuViewToggleTheme.Size = new Size(207, 22);
            mnuViewToggleTheme.Text = "테마 전환";
            // 
            // mnuViewOpenGraphStats
            // 
            mnuViewOpenGraphStats.Name = "mnuViewOpenGraphStats";
            mnuViewOpenGraphStats.Size = new Size(207, 22);
            mnuViewOpenGraphStats.Text = "그래프/통계 탭으로 이동";
            mnuViewOpenGraphStats.Click += MnuViewOpenGraphStats_Click;
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpOpenTutorial });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(55, 20);
            mnuHelp.Text = "도움말";
            mnuHelp.Click += MnuHelp_Click;
            // 
            // mnuHelpOpenTutorial
            // 
            mnuHelpOpenTutorial.Name = "mnuHelpOpenTutorial";
            mnuHelpOpenTutorial.Size = new Size(162, 22);
            mnuHelpOpenTutorial.Text = "기능별 튜토리얼";
            // 
            // tabControlMain
            // 
            tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlMain.Controls.Add(tabPageDataViewer);
            tabControlMain.Controls.Add(tabTrainingMonitor);
            tabControlMain.Controls.Add(tabGraphStats);
            tabControlMain.Location = new Point(2, 23);
            tabControlMain.Margin = new Padding(2);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1229, 690);
            tabControlMain.TabIndex = 1;
            // 
            // tabPageDataViewer
            // 
            tabPageDataViewer.Controls.Add(statusStripDataFooter);
            tabPageDataViewer.Controls.Add(grpDataCleaner);
            tabPageDataViewer.Controls.Add(grpDataExplorer);
            tabPageDataViewer.Location = new Point(4, 24);
            tabPageDataViewer.Margin = new Padding(2);
            tabPageDataViewer.Name = "tabPageDataViewer";
            tabPageDataViewer.Padding = new Padding(2);
            tabPageDataViewer.Size = new Size(1221, 662);
            tabPageDataViewer.TabIndex = 0;
            tabPageDataViewer.Text = "데이터 뷰어";
            tabPageDataViewer.UseVisualStyleBackColor = true;
            // 
            // statusStripDataFooter
            // 
            statusStripDataFooter.ImageScalingSize = new Size(20, 20);
            statusStripDataFooter.Items.AddRange(new ToolStripItem[] { stsDataFooterPath });
            statusStripDataFooter.Location = new Point(2, 638);
            statusStripDataFooter.Name = "statusStripDataFooter";
            statusStripDataFooter.Padding = new Padding(1, 0, 11, 0);
            statusStripDataFooter.Size = new Size(1217, 22);
            statusStripDataFooter.TabIndex = 24;
            statusStripDataFooter.Text = "statusStripDataFooter";
            // 
            // stsDataFooterPath
            // 
            stsDataFooterPath.Name = "stsDataFooterPath";
            stsDataFooterPath.Size = new Size(27, 17);
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
            grpDataCleaner.Location = new Point(2, 476);
            grpDataCleaner.Margin = new Padding(2);
            grpDataCleaner.Name = "grpDataCleaner";
            grpDataCleaner.Padding = new Padding(2);
            grpDataCleaner.Size = new Size(1216, 165);
            grpDataCleaner.TabIndex = 23;
            grpDataCleaner.TabStop = false;
            grpDataCleaner.Text = "터브 정리기";
            grpDataCleaner.Enter += GrpDataCleaner_Enter;
            // 
            // lblFrameRangeSeparator
            // 
            lblFrameRangeSeparator.AutoSize = true;
            lblFrameRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblFrameRangeSeparator.Location = new Point(313, 52);
            lblFrameRangeSeparator.Margin = new Padding(2, 0, 2, 0);
            lblFrameRangeSeparator.Name = "lblFrameRangeSeparator";
            lblFrameRangeSeparator.Size = new Size(23, 19);
            lblFrameRangeSeparator.TabIndex = 47;
            lblFrameRangeSeparator.Text = "～";
            // 
            // txtFrameRangeEnd
            // 
            txtFrameRangeEnd.Location = new Point(338, 50);
            txtFrameRangeEnd.Name = "txtFrameRangeEnd";
            txtFrameRangeEnd.Size = new Size(102, 23);
            txtFrameRangeEnd.TabIndex = 46;
            // 
            // txtFrameRangeStart
            // 
            txtFrameRangeStart.Location = new Point(207, 50);
            txtFrameRangeStart.Name = "txtFrameRangeStart";
            txtFrameRangeStart.Size = new Size(102, 23);
            txtFrameRangeStart.TabIndex = 45;
            // 
            // lblFrameRange
            // 
            lblFrameRange.AutoSize = true;
            lblFrameRange.Font = new Font("맑은 고딕", 10F);
            lblFrameRange.Location = new Point(75, 52);
            lblFrameRange.Name = "lblFrameRange";
            lblFrameRange.Size = new Size(70, 19);
            lblFrameRange.TabIndex = 44;
            lblFrameRange.Text = "구간 선택";
            // 
            // btnRestoreFrames
            // 
            btnRestoreFrames.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRestoreFrames.Location = new Point(985, 107);
            btnRestoreFrames.Margin = new Padding(2);
            btnRestoreFrames.Name = "btnRestoreFrames";
            btnRestoreFrames.Size = new Size(167, 22);
            btnRestoreFrames.TabIndex = 43;
            btnRestoreFrames.Text = "복구";
            btnRestoreFrames.UseVisualStyleBackColor = true;
            // 
            // btnSaveCleanupState
            // 
            btnSaveCleanupState.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveCleanupState.Location = new Point(985, 135);
            btnSaveCleanupState.Margin = new Padding(2);
            btnSaveCleanupState.Name = "btnSaveCleanupState";
            btnSaveCleanupState.Size = new Size(167, 22);
            btnSaveCleanupState.TabIndex = 48;
            btnSaveCleanupState.Text = "상태 저장";
            btnSaveCleanupState.UseVisualStyleBackColor = true;
            // 
            // btnExportCleanDataset
            // 
            btnExportCleanDataset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportCleanDataset.Location = new Point(805, 106);
            btnExportCleanDataset.Margin = new Padding(2);
            btnExportCleanDataset.Name = "btnExportCleanDataset";
            btnExportCleanDataset.Size = new Size(167, 22);
            btnExportCleanDataset.TabIndex = 42;
            btnExportCleanDataset.Text = "삭제";
            btnExportCleanDataset.UseVisualStyleBackColor = true;
            // 
            // btnExcludeFrameRange
            // 
            btnExcludeFrameRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeFrameRange.Location = new Point(805, 50);
            btnExcludeFrameRange.Margin = new Padding(2);
            btnExcludeFrameRange.Name = "btnExcludeFrameRange";
            btnExcludeFrameRange.Size = new Size(167, 22);
            btnExcludeFrameRange.TabIndex = 40;
            btnExcludeFrameRange.Text = "구간 제외";
            btnExcludeFrameRange.UseVisualStyleBackColor = true;
            // 
            // btnExcludeSelectedFrames
            // 
            btnExcludeSelectedFrames.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeSelectedFrames.Location = new Point(985, 51);
            btnExcludeSelectedFrames.Margin = new Padding(2);
            btnExcludeSelectedFrames.Name = "btnExcludeSelectedFrames";
            btnExcludeSelectedFrames.Size = new Size(167, 22);
            btnExcludeSelectedFrames.TabIndex = 41;
            btnExcludeSelectedFrames.Text = "선택 프레임 제외";
            btnExcludeSelectedFrames.UseVisualStyleBackColor = true;
            // 
            // cmbScenarioFilter
            // 
            cmbScenarioFilter.Font = new Font("맑은 고딕", 10F);
            cmbScenarioFilter.FormattingEnabled = true;
            cmbScenarioFilter.Items.AddRange(new object[] { "All", "Normal", "Night", "Left_turn", "Right_turn", "Out_of_bound" });
            cmbScenarioFilter.Location = new Point(548, 106);
            cmbScenarioFilter.Margin = new Padding(2);
            cmbScenarioFilter.Name = "cmbScenarioFilter";
            cmbScenarioFilter.Size = new Size(216, 25);
            cmbScenarioFilter.TabIndex = 39;
            // 
            // lblScenarioFilter
            // 
            lblScenarioFilter.AutoSize = true;
            lblScenarioFilter.Font = new Font("맑은 고딕", 10F);
            lblScenarioFilter.Location = new Point(476, 106);
            lblScenarioFilter.Margin = new Padding(2, 0, 2, 0);
            lblScenarioFilter.Name = "lblScenarioFilter";
            lblScenarioFilter.Size = new Size(73, 19);
            lblScenarioFilter.TabIndex = 38;
            lblScenarioFilter.Text = "시나리오: ";
            // 
            // cmbModeFilter
            // 
            cmbModeFilter.Font = new Font("맑은 고딕", 10F);
            cmbModeFilter.FormattingEnabled = true;
            cmbModeFilter.Items.AddRange(new object[] { "All", "User", "Local", "Local_angle" });
            cmbModeFilter.Location = new Point(548, 76);
            cmbModeFilter.Margin = new Padding(2);
            cmbModeFilter.Name = "cmbModeFilter";
            cmbModeFilter.Size = new Size(216, 25);
            cmbModeFilter.TabIndex = 37;
            // 
            // lblModeFilter
            // 
            lblModeFilter.AutoSize = true;
            lblModeFilter.Font = new Font("맑은 고딕", 10F);
            lblModeFilter.Location = new Point(476, 77);
            lblModeFilter.Margin = new Padding(2, 0, 2, 0);
            lblModeFilter.Name = "lblModeFilter";
            lblModeFilter.Size = new Size(70, 19);
            lblModeFilter.TabIndex = 36;
            lblModeFilter.Text = "모드      :";
            // 
            // lblAngleRangeSeparator
            // 
            lblAngleRangeSeparator.AutoSize = true;
            lblAngleRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblAngleRangeSeparator.Location = new Point(313, 82);
            lblAngleRangeSeparator.Margin = new Padding(2, 0, 2, 0);
            lblAngleRangeSeparator.Name = "lblAngleRangeSeparator";
            lblAngleRangeSeparator.Size = new Size(23, 19);
            lblAngleRangeSeparator.TabIndex = 35;
            lblAngleRangeSeparator.Text = "～";
            // 
            // lblThrottleRangeSeparator
            // 
            lblThrottleRangeSeparator.AutoSize = true;
            lblThrottleRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblThrottleRangeSeparator.Location = new Point(313, 112);
            lblThrottleRangeSeparator.Margin = new Padding(2, 0, 2, 0);
            lblThrottleRangeSeparator.Name = "lblThrottleRangeSeparator";
            lblThrottleRangeSeparator.Size = new Size(23, 19);
            lblThrottleRangeSeparator.TabIndex = 34;
            lblThrottleRangeSeparator.Text = "～";
            // 
            // txtThrottleMaxFilter
            // 
            txtThrottleMaxFilter.Font = new Font("맑은 고딕", 10F);
            txtThrottleMaxFilter.Location = new Point(338, 110);
            txtThrottleMaxFilter.Margin = new Padding(2);
            txtThrottleMaxFilter.Name = "txtThrottleMaxFilter";
            txtThrottleMaxFilter.Size = new Size(102, 25);
            txtThrottleMaxFilter.TabIndex = 33;
            // 
            // txtAngleMaxFilter
            // 
            txtAngleMaxFilter.Font = new Font("맑은 고딕", 10F);
            txtAngleMaxFilter.Location = new Point(338, 79);
            txtAngleMaxFilter.Margin = new Padding(2);
            txtAngleMaxFilter.Name = "txtAngleMaxFilter";
            txtAngleMaxFilter.Size = new Size(102, 25);
            txtAngleMaxFilter.TabIndex = 32;
            // 
            // txtThrottleMinFilter
            // 
            txtThrottleMinFilter.Font = new Font("맑은 고딕", 10F);
            txtThrottleMinFilter.Location = new Point(207, 110);
            txtThrottleMinFilter.Margin = new Padding(2);
            txtThrottleMinFilter.Name = "txtThrottleMinFilter";
            txtThrottleMinFilter.Size = new Size(102, 25);
            txtThrottleMinFilter.TabIndex = 31;
            // 
            // txtAngleMinFilter
            // 
            txtAngleMinFilter.Font = new Font("맑은 고딕", 10F);
            txtAngleMinFilter.Location = new Point(207, 79);
            txtAngleMinFilter.Margin = new Padding(2);
            txtAngleMinFilter.Name = "txtAngleMinFilter";
            txtAngleMinFilter.Size = new Size(102, 25);
            txtAngleMinFilter.TabIndex = 30;
            // 
            // lblThrottleRange
            // 
            lblThrottleRange.AutoSize = true;
            lblThrottleRange.Font = new Font("맑은 고딕", 10F);
            lblThrottleRange.Location = new Point(75, 112);
            lblThrottleRange.Margin = new Padding(2, 0, 2, 0);
            lblThrottleRange.Name = "lblThrottleRange";
            lblThrottleRange.Size = new Size(146, 19);
            lblThrottleRange.TabIndex = 26;
            lblThrottleRange.Text = "스로틀값 범위(-1~1): ";
            // 
            // lblAngleRange
            // 
            lblAngleRange.AutoSize = true;
            lblAngleRange.Font = new Font("맑은 고딕", 10F);
            lblAngleRange.Location = new Point(75, 82);
            lblAngleRange.Margin = new Padding(2, 0, 2, 0);
            lblAngleRange.Name = "lblAngleRange";
            lblAngleRange.Size = new Size(137, 19);
            lblAngleRange.TabIndex = 25;
            lblAngleRange.Text = "조향각 범위 (-1~1): ";
            // 
            // btnApplyFrameFilter
            // 
            btnApplyFrameFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFrameFilter.Location = new Point(806, 77);
            btnApplyFrameFilter.Margin = new Padding(2);
            btnApplyFrameFilter.Name = "btnApplyFrameFilter";
            btnApplyFrameFilter.Size = new Size(167, 22);
            btnApplyFrameFilter.TabIndex = 12;
            btnApplyFrameFilter.Text = "필터 적용";
            btnApplyFrameFilter.UseVisualStyleBackColor = true;
            btnApplyFrameFilter.Click += BtnApplyFrameFilter_Click;
            // 
            // btnClearFrameFilter
            // 
            btnClearFrameFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFrameFilter.Location = new Point(985, 79);
            btnClearFrameFilter.Margin = new Padding(2);
            btnClearFrameFilter.Name = "btnClearFrameFilter";
            btnClearFrameFilter.Size = new Size(167, 22);
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
            grpDataExplorer.Margin = new Padding(2);
            grpDataExplorer.Name = "grpDataExplorer";
            grpDataExplorer.Padding = new Padding(2);
            grpDataExplorer.Size = new Size(1220, 476);
            grpDataExplorer.TabIndex = 22;
            grpDataExplorer.TabStop = false;
            grpDataExplorer.Text = "터브 탐색기";
            // 
            // trkFrameTimeline
            // 
            trkFrameTimeline.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trkFrameTimeline.Location = new Point(-1, 428);
            trkFrameTimeline.Margin = new Padding(2);
            trkFrameTimeline.Maximum = 0;
            trkFrameTimeline.Name = "trkFrameTimeline";
            trkFrameTimeline.Size = new Size(1216, 45);
            trkFrameTimeline.TabIndex = 18;
            trkFrameTimeline.Scroll += TrkFrameTimeline_Scroll;
            // 
            // numPlaybackIntervalMs
            // 
            numPlaybackIntervalMs.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numPlaybackIntervalMs.Location = new Point(1093, 396);
            numPlaybackIntervalMs.Margin = new Padding(2);
            numPlaybackIntervalMs.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numPlaybackIntervalMs.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numPlaybackIntervalMs.Name = "numPlaybackIntervalMs";
            numPlaybackIntervalMs.Size = new Size(115, 23);
            numPlaybackIntervalMs.TabIndex = 11;
            numPlaybackIntervalMs.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // btnOpenDataFolder
            // 
            btnOpenDataFolder.Location = new Point(7, 16);
            btnOpenDataFolder.Margin = new Padding(2);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new Size(125, 25);
            btnOpenDataFolder.TabIndex = 0;
            btnOpenDataFolder.Text = "데이터 폴더 열기";
            btnOpenDataFolder.UseVisualStyleBackColor = true;
            btnOpenDataFolder.Click += BtnOpenDataFolder_Click;
            // 
            // lblPlayInterval
            // 
            lblPlayInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPlayInterval.Font = new Font("맑은 고딕", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblPlayInterval.Location = new Point(1002, 401);
            lblPlayInterval.Margin = new Padding(2, 0, 2, 0);
            lblPlayInterval.Name = "lblPlayInterval";
            lblPlayInterval.Size = new Size(99, 23);
            lblPlayInterval.TabIndex = 21;
            lblPlayInterval.Text = "재생간격(ms)";
            lblPlayInterval.Click += LblPlayInterval_Click;
            // 
            // btnLast
            // 
            btnLast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLast.Location = new Point(1107, 108);
            btnLast.Margin = new Padding(2);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(104, 25);
            btnLast.TabIndex = 17;
            btnLast.Text = ">>";
            btnLast.Click += btnLast_Click;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAutoPlay.Location = new Point(1001, 138);
            btnAutoPlay.Margin = new Padding(2);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(210, 25);
            btnAutoPlay.TabIndex = 20;
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;
            btnAutoPlay.Click += btnAutoPlay_Click;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNext.Location = new Point(1107, 78);
            btnNext.Margin = new Padding(2);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(104, 25);
            btnNext.TabIndex = 16;
            btnNext.Text = ">";
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrev.Location = new Point(999, 78);
            btnPrev.Margin = new Padding(2);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(104, 25);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "<";
            btnPrev.Click += btnPrev_Click;
            // 
            // splitContainerFramePreview
            // 
            splitContainerFramePreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerFramePreview.Location = new Point(0, 46);
            splitContainerFramePreview.Margin = new Padding(2);
            splitContainerFramePreview.Name = "splitContainerFramePreview";
            // 
            // splitContainerFramePreview.Panel1
            // 
            splitContainerFramePreview.Panel1.Controls.Add(dgvFrameCatalog);
            // 
            // splitContainerFramePreview.Panel2
            // 
            splitContainerFramePreview.Panel2.Controls.Add(picFramePreview);
            splitContainerFramePreview.Size = new Size(994, 377);
            splitContainerFramePreview.SplitterDistance = 280;
            splitContainerFramePreview.SplitterWidth = 5;
            splitContainerFramePreview.TabIndex = 44;
            // 
            // dgvFrameCatalog
            // 
            dgvFrameCatalog.AllowUserToAddRows = false;
            dgvFrameCatalog.AllowUserToDeleteRows = false;
            dgvFrameCatalog.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvFrameCatalog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvFrameCatalog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFrameCatalog.Dock = DockStyle.Fill;
            dgvFrameCatalog.Location = new Point(0, 0);
            dgvFrameCatalog.Margin = new Padding(2);
            dgvFrameCatalog.Name = "dgvFrameCatalog";
            dgvFrameCatalog.ReadOnly = true;
            dgvFrameCatalog.RowHeadersVisible = false;
            dgvFrameCatalog.RowHeadersWidth = 51;
            dgvFrameCatalog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFrameCatalog.Size = new Size(280, 377);
            dgvFrameCatalog.TabIndex = 13;
            dgvFrameCatalog.SelectionChanged += DgvFrameCatalog_SelectionChanged;
            // 
            // picFramePreview
            // 
            picFramePreview.BorderStyle = BorderStyle.FixedSingle;
            picFramePreview.Dock = DockStyle.Fill;
            picFramePreview.Location = new Point(0, 0);
            picFramePreview.Margin = new Padding(2);
            picFramePreview.Name = "picFramePreview";
            picFramePreview.Size = new Size(709, 377);
            picFramePreview.SizeMode = PictureBoxSizeMode.Zoom;
            picFramePreview.TabIndex = 4;
            picFramePreview.TabStop = false;
            // 
            // btnFirst
            // 
            btnFirst.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFirst.Location = new Point(999, 108);
            btnFirst.Margin = new Padding(2);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(104, 25);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "<<";
            btnFirst.Click += btnFirst_Click;
            // 
            // btnReloadData
            // 
            btnReloadData.Location = new Point(134, 16);
            btnReloadData.Margin = new Padding(2);
            btnReloadData.Name = "btnReloadData";
            btnReloadData.Size = new Size(125, 25);
            btnReloadData.TabIndex = 1;
            btnReloadData.Text = "다시 불러오기";
            btnReloadData.UseVisualStyleBackColor = true;
            btnReloadData.Click += BtnReloadData_Click;
            // 
            // lblFrameValue
            // 
            lblFrameValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            lblModeValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblModeValue.Font = new Font("맑은 고딕", 13.8F);
            lblModeValue.Location = new Point(1001, 238);
            lblModeValue.Margin = new Padding(2, 0, 2, 0);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(212, 29);
            lblModeValue.TabIndex = 8;
            lblModeValue.Text = "모드: -";
            lblModeValue.Click += LblModeValue_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Location = new Point(260, 16);
            btnToggleTheme.Margin = new Padding(2);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(125, 25);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "테마 전환";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // lblThrottleValue
            // 
            lblThrottleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblThrottleValue.Font = new Font("맑은 고딕", 13.8F);
            lblThrottleValue.Location = new Point(1001, 211);
            lblThrottleValue.Margin = new Padding(2, 0, 2, 0);
            lblThrottleValue.Name = "lblThrottleValue";
            lblThrottleValue.Size = new Size(212, 28);
            lblThrottleValue.TabIndex = 7;
            lblThrottleValue.Text = "스로틀값: 0.000";
            lblThrottleValue.Click += LblThrottleValue_Click;
            // 
            // btnGuide
            // 
            btnGuide.Location = new Point(387, 16);
            btnGuide.Margin = new Padding(2);
            btnGuide.Name = "btnGuide";
            btnGuide.Size = new Size(125, 25);
            btnGuide.TabIndex = 3;
            btnGuide.Text = "단계별 가이드";
            btnGuide.UseVisualStyleBackColor = true;
            btnGuide.Click += btnGuide_Click;
            // 
            // lblAngleValue
            // 
            lblAngleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAngleValue.Font = new Font("맑은 고딕", 13.8F);
            lblAngleValue.Location = new Point(1001, 181);
            lblAngleValue.Margin = new Padding(2, 0, 2, 0);
            lblAngleValue.Name = "lblAngleValue";
            lblAngleValue.Size = new Size(212, 29);
            lblAngleValue.TabIndex = 6;
            lblAngleValue.Text = "조향값: 0.000";
            lblAngleValue.Click += LblAngleValue_Click;
            // 
            // tabTrainingMonitor
            // 
            tabTrainingMonitor.Controls.Add(statusStripTraining);
            tabTrainingMonitor.Controls.Add(grpTrainingOutput);
            tabTrainingMonitor.Controls.Add(grpTrainingConfig);
            tabTrainingMonitor.Location = new Point(4, 24);
            tabTrainingMonitor.Margin = new Padding(2);
            tabTrainingMonitor.Name = "tabTrainingMonitor";
            tabTrainingMonitor.Padding = new Padding(2);
            tabTrainingMonitor.Size = new Size(1221, 662);
            tabTrainingMonitor.TabIndex = 1;
            tabTrainingMonitor.Text = "학습 실행";
            tabTrainingMonitor.UseVisualStyleBackColor = true;
            tabTrainingMonitor.Click += TabTrainingMonitor_Click;
            // 
            // statusStripTraining
            // 
            statusStripTraining.ImageScalingSize = new Size(20, 20);
            statusStripTraining.Items.AddRange(new ToolStripItem[] { stsTrainingStatus });
            statusStripTraining.Location = new Point(2, 638);
            statusStripTraining.Name = "statusStripTraining";
            statusStripTraining.Padding = new Padding(1, 0, 9, 0);
            statusStripTraining.Size = new Size(1217, 22);
            statusStripTraining.TabIndex = 2;
            statusStripTraining.Text = "statusStripTraining";
            // 
            // stsTrainingStatus
            // 
            stsTrainingStatus.Name = "stsTrainingStatus";
            stsTrainingStatus.Size = new Size(82, 17);
            stsTrainingStatus.Text = "Tub 경로: C:\\";
            stsTrainingStatus.Click += StsTrainingStatus_Click;
            // 
            // grpTrainingOutput
            // 
            grpTrainingOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingOutput.Controls.Add(rtbTrainingOutput);
            grpTrainingOutput.Location = new Point(9, 292);
            grpTrainingOutput.Margin = new Padding(6);
            grpTrainingOutput.Name = "grpTrainingOutput";
            grpTrainingOutput.Padding = new Padding(2);
            grpTrainingOutput.Size = new Size(936, 186);
            grpTrainingOutput.TabIndex = 1;
            grpTrainingOutput.TabStop = false;
            grpTrainingOutput.Text = "학습 로그";
            // 
            // rtbTrainingOutput
            // 
            rtbTrainingOutput.BackColor = SystemColors.MenuText;
            rtbTrainingOutput.Dock = DockStyle.Fill;
            rtbTrainingOutput.ForeColor = SystemColors.Window;
            rtbTrainingOutput.Location = new Point(2, 18);
            rtbTrainingOutput.Margin = new Padding(2);
            rtbTrainingOutput.Name = "rtbTrainingOutput";
            rtbTrainingOutput.Size = new Size(932, 166);
            rtbTrainingOutput.TabIndex = 0;
            rtbTrainingOutput.Text = "";
            // 
            // grpTrainingConfig
            // 
            grpTrainingConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingConfig.Controls.Add(btnSelectCondaPath);
            grpTrainingConfig.Controls.Add(btnSelectTrainingWslDistro);
            grpTrainingConfig.Controls.Add(cmbCondaPath);
            grpTrainingConfig.Controls.Add(lblCondaPath);
            grpTrainingConfig.Controls.Add(cmbTrainingWslDistro);
            grpTrainingConfig.Controls.Add(lblTrainingWslDistro);
            grpTrainingConfig.Controls.Add(numTrainingEpochs);
            grpTrainingConfig.Controls.Add(cmbTrainingModelType);
            grpTrainingConfig.Controls.Add(txtTrainingPythonEnvName);
            grpTrainingConfig.Controls.Add(txtTrainingTubPath);
            grpTrainingConfig.Controls.Add(txtTrainingModelPath);
            grpTrainingConfig.Controls.Add(cmbMycarProjectPath);
            grpTrainingConfig.Controls.Add(lblEpoch);
            grpTrainingConfig.Controls.Add(lblTrainingModelType);
            grpTrainingConfig.Controls.Add(lblTrainingPythonEnvName);
            grpTrainingConfig.Controls.Add(lblTrainingModelPath);
            grpTrainingConfig.Controls.Add(lblTrainingTubPath);
            grpTrainingConfig.Controls.Add(lblMycarProjectPath);
            grpTrainingConfig.Controls.Add(btnSaveTrainingConfig);
            grpTrainingConfig.Controls.Add(btnStopTrainingProcess);
            grpTrainingConfig.Controls.Add(btnStartTrainingProcess);
            grpTrainingConfig.Controls.Add(btnDetectTrainingEnvironment);
            grpTrainingConfig.Controls.Add(btnSelectTrainingModelPath);
            grpTrainingConfig.Controls.Add(btnSelectTrainingTubPath);
            grpTrainingConfig.Controls.Add(btnSelectMycarPath);
            grpTrainingConfig.Location = new Point(9, 10);
            grpTrainingConfig.Margin = new Padding(2);
            grpTrainingConfig.Name = "grpTrainingConfig";
            grpTrainingConfig.Padding = new Padding(2);
            grpTrainingConfig.Size = new Size(936, 274);
            grpTrainingConfig.TabIndex = 0;
            grpTrainingConfig.TabStop = false;
            grpTrainingConfig.Text = "학습 설정";
            // 
            // btnSelectCondaPath
            // 
            btnSelectCondaPath.Location = new Point(0, 0);
            btnSelectCondaPath.Margin = new Padding(2);
            btnSelectCondaPath.Name = "btnSelectCondaPath";
            btnSelectCondaPath.Size = new Size(58, 17);
            btnSelectCondaPath.TabIndex = 0;
            // 
            // btnSelectTrainingWslDistro
            // 
            btnSelectTrainingWslDistro.Location = new Point(0, 0);
            btnSelectTrainingWslDistro.Margin = new Padding(2);
            btnSelectTrainingWslDistro.Name = "btnSelectTrainingWslDistro";
            btnSelectTrainingWslDistro.Size = new Size(58, 17);
            btnSelectTrainingWslDistro.TabIndex = 1;
            // 
            // cmbCondaPath
            // 
            cmbCondaPath.Location = new Point(0, 0);
            cmbCondaPath.Margin = new Padding(2);
            cmbCondaPath.Name = "cmbCondaPath";
            cmbCondaPath.Size = new Size(95, 23);
            cmbCondaPath.TabIndex = 2;
            // 
            // lblCondaPath
            // 
            lblCondaPath.Location = new Point(0, 0);
            lblCondaPath.Margin = new Padding(2, 0, 2, 0);
            lblCondaPath.Name = "lblCondaPath";
            lblCondaPath.Size = new Size(78, 17);
            lblCondaPath.TabIndex = 3;
            // 
            // cmbTrainingWslDistro
            // 
            cmbTrainingWslDistro.Location = new Point(0, 0);
            cmbTrainingWslDistro.Margin = new Padding(2);
            cmbTrainingWslDistro.Name = "cmbTrainingWslDistro";
            cmbTrainingWslDistro.Size = new Size(95, 23);
            cmbTrainingWslDistro.TabIndex = 4;
            // 
            // lblTrainingWslDistro
            // 
            lblTrainingWslDistro.Location = new Point(0, 0);
            lblTrainingWslDistro.Margin = new Padding(2, 0, 2, 0);
            lblTrainingWslDistro.Name = "lblTrainingWslDistro";
            lblTrainingWslDistro.Size = new Size(78, 17);
            lblTrainingWslDistro.TabIndex = 5;
            // 
            // numTrainingEpochs
            // 
            numTrainingEpochs.Font = new Font("맑은 고딕", 12F);
            numTrainingEpochs.Location = new Point(169, 181);
            numTrainingEpochs.Margin = new Padding(2);
            numTrainingEpochs.Name = "numTrainingEpochs";
            numTrainingEpochs.Size = new Size(236, 29);
            numTrainingEpochs.TabIndex = 17;
            // 
            // cmbTrainingModelType
            // 
            cmbTrainingModelType.Font = new Font("맑은 고딕", 12F);
            cmbTrainingModelType.FormattingEnabled = true;
            cmbTrainingModelType.Location = new Point(169, 129);
            cmbTrainingModelType.Margin = new Padding(2);
            cmbTrainingModelType.Name = "cmbTrainingModelType";
            cmbTrainingModelType.Size = new Size(238, 29);
            cmbTrainingModelType.TabIndex = 2;
            cmbTrainingModelType.SelectedIndexChanged += cmbTrainingModelType_SelectedIndexChanged;
            // 
            // txtTrainingPythonEnvName
            // 
            txtTrainingPythonEnvName.Font = new Font("맑은 고딕", 12F);
            txtTrainingPythonEnvName.Location = new Point(169, 156);
            txtTrainingPythonEnvName.Margin = new Padding(2);
            txtTrainingPythonEnvName.Name = "txtTrainingPythonEnvName";
            txtTrainingPythonEnvName.Size = new Size(238, 29);
            txtTrainingPythonEnvName.TabIndex = 18;
            // 
            // txtTrainingTubPath
            // 
            txtTrainingTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTrainingTubPath.Font = new Font("맑은 고딕", 12F);
            txtTrainingTubPath.Location = new Point(169, 71);
            txtTrainingTubPath.Margin = new Padding(2);
            txtTrainingTubPath.Name = "txtTrainingTubPath";
            txtTrainingTubPath.Size = new Size(619, 29);
            txtTrainingTubPath.TabIndex = 16;
            // 
            // txtTrainingModelPath
            // 
            txtTrainingModelPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTrainingModelPath.Font = new Font("맑은 고딕", 12F);
            txtTrainingModelPath.Location = new Point(169, 100);
            txtTrainingModelPath.Margin = new Padding(2);
            txtTrainingModelPath.Name = "txtTrainingModelPath";
            txtTrainingModelPath.Size = new Size(619, 29);
            txtTrainingModelPath.TabIndex = 14;
            // 
            // cmbMycarProjectPath
            // 
            cmbMycarProjectPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbMycarProjectPath.Font = new Font("맑은 고딕", 12F);
            cmbMycarProjectPath.FormattingEnabled = true;
            cmbMycarProjectPath.Location = new Point(169, 42);
            cmbMycarProjectPath.Margin = new Padding(2);
            cmbMycarProjectPath.Name = "cmbMycarProjectPath";
            cmbMycarProjectPath.Size = new Size(619, 29);
            cmbMycarProjectPath.TabIndex = 13;
            // 
            // lblEpoch
            // 
            lblEpoch.AutoSize = true;
            lblEpoch.Font = new Font("맑은 고딕", 12F);
            lblEpoch.Location = new Point(27, 182);
            lblEpoch.Margin = new Padding(2, 0, 2, 0);
            lblEpoch.Name = "lblEpoch";
            lblEpoch.Size = new Size(80, 21);
            lblEpoch.TabIndex = 11;
            lblEpoch.Text = "학습 횟수";
            // 
            // lblTrainingModelType
            // 
            lblTrainingModelType.AutoSize = true;
            lblTrainingModelType.Font = new Font("맑은 고딕", 12F);
            lblTrainingModelType.Location = new Point(27, 130);
            lblTrainingModelType.Margin = new Padding(2, 0, 2, 0);
            lblTrainingModelType.Name = "lblTrainingModelType";
            lblTrainingModelType.Size = new Size(80, 21);
            lblTrainingModelType.TabIndex = 10;
            lblTrainingModelType.Text = "모델 종류";
            // 
            // lblTrainingPythonEnvName
            // 
            lblTrainingPythonEnvName.AutoSize = true;
            lblTrainingPythonEnvName.Font = new Font("맑은 고딕", 12F);
            lblTrainingPythonEnvName.Location = new Point(27, 158);
            lblTrainingPythonEnvName.Margin = new Padding(2, 0, 2, 0);
            lblTrainingPythonEnvName.Name = "lblTrainingPythonEnvName";
            lblTrainingPythonEnvName.Size = new Size(115, 21);
            lblTrainingPythonEnvName.TabIndex = 19;
            lblTrainingPythonEnvName.Text = "Python 환경명";
            // 
            // lblTrainingModelPath
            // 
            lblTrainingModelPath.AutoSize = true;
            lblTrainingModelPath.Font = new Font("맑은 고딕", 12F);
            lblTrainingModelPath.Location = new Point(27, 102);
            lblTrainingModelPath.Margin = new Padding(2, 0, 2, 0);
            lblTrainingModelPath.Name = "lblTrainingModelPath";
            lblTrainingModelPath.Size = new Size(118, 21);
            lblTrainingModelPath.TabIndex = 9;
            lblTrainingModelPath.Text = "모델 저장 경로";
            // 
            // lblTrainingTubPath
            // 
            lblTrainingTubPath.AutoSize = true;
            lblTrainingTubPath.Font = new Font("맑은 고딕", 12F);
            lblTrainingTubPath.Location = new Point(27, 74);
            lblTrainingTubPath.Margin = new Padding(2, 0, 2, 0);
            lblTrainingTubPath.Name = "lblTrainingTubPath";
            lblTrainingTubPath.Size = new Size(76, 21);
            lblTrainingTubPath.TabIndex = 8;
            lblTrainingTubPath.Text = "Tub 경로";
            // 
            // lblMycarProjectPath
            // 
            lblMycarProjectPath.AutoSize = true;
            lblMycarProjectPath.Font = new Font("맑은 고딕", 12F);
            lblMycarProjectPath.Location = new Point(28, 44);
            lblMycarProjectPath.Margin = new Padding(2, 0, 2, 0);
            lblMycarProjectPath.Name = "lblMycarProjectPath";
            lblMycarProjectPath.Size = new Size(173, 21);
            lblMycarProjectPath.TabIndex = 7;
            lblMycarProjectPath.Text = "Donkey 프로젝트 경로";
            // 
            // btnSaveTrainingConfig
            // 
            btnSaveTrainingConfig.Font = new Font("맑은 고딕", 12F);
            btnSaveTrainingConfig.Location = new Point(633, 207);
            btnSaveTrainingConfig.Margin = new Padding(2);
            btnSaveTrainingConfig.Name = "btnSaveTrainingConfig";
            btnSaveTrainingConfig.Size = new Size(117, 24);
            btnSaveTrainingConfig.TabIndex = 5;
            btnSaveTrainingConfig.Text = "설정 저장";
            btnSaveTrainingConfig.UseVisualStyleBackColor = true;
            // 
            // btnStopTrainingProcess
            // 
            btnStopTrainingProcess.Font = new Font("맑은 고딕", 12F);
            btnStopTrainingProcess.Location = new Point(432, 207);
            btnStopTrainingProcess.Margin = new Padding(2);
            btnStopTrainingProcess.Name = "btnStopTrainingProcess";
            btnStopTrainingProcess.Size = new Size(117, 24);
            btnStopTrainingProcess.TabIndex = 4;
            btnStopTrainingProcess.Text = "학습 중지";
            btnStopTrainingProcess.UseVisualStyleBackColor = true;
            // 
            // btnStartTrainingProcess
            // 
            btnStartTrainingProcess.Font = new Font("맑은 고딕", 12F);
            btnStartTrainingProcess.Location = new Point(229, 207);
            btnStartTrainingProcess.Margin = new Padding(2);
            btnStartTrainingProcess.Name = "btnStartTrainingProcess";
            btnStartTrainingProcess.Size = new Size(117, 24);
            btnStartTrainingProcess.TabIndex = 3;
            btnStartTrainingProcess.Text = "학습 시작";
            btnStartTrainingProcess.UseVisualStyleBackColor = true;
            // 
            // btnDetectTrainingEnvironment
            // 
            btnDetectTrainingEnvironment.Location = new Point(0, 0);
            btnDetectTrainingEnvironment.Margin = new Padding(2);
            btnDetectTrainingEnvironment.Name = "btnDetectTrainingEnvironment";
            btnDetectTrainingEnvironment.Size = new Size(58, 17);
            btnDetectTrainingEnvironment.TabIndex = 20;
            // 
            // btnSelectTrainingModelPath
            // 
            btnSelectTrainingModelPath.Location = new Point(0, 0);
            btnSelectTrainingModelPath.Margin = new Padding(2);
            btnSelectTrainingModelPath.Name = "btnSelectTrainingModelPath";
            btnSelectTrainingModelPath.Size = new Size(58, 17);
            btnSelectTrainingModelPath.TabIndex = 21;
            // 
            // btnSelectTrainingTubPath
            // 
            btnSelectTrainingTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectTrainingTubPath.Font = new Font("맑은 고딕", 12F);
            btnSelectTrainingTubPath.Location = new Point(800, 70);
            btnSelectTrainingTubPath.Margin = new Padding(2);
            btnSelectTrainingTubPath.Name = "btnSelectTrainingTubPath";
            btnSelectTrainingTubPath.Size = new Size(75, 24);
            btnSelectTrainingTubPath.TabIndex = 2;
            btnSelectTrainingTubPath.Text = "경로 선택";
            btnSelectTrainingTubPath.UseVisualStyleBackColor = true;
            // 
            // btnSelectMycarPath
            // 
            btnSelectMycarPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectMycarPath.Font = new Font("맑은 고딕", 12F);
            btnSelectMycarPath.Location = new Point(800, 40);
            btnSelectMycarPath.Margin = new Padding(2);
            btnSelectMycarPath.Name = "btnSelectMycarPath";
            btnSelectMycarPath.Size = new Size(75, 24);
            btnSelectMycarPath.TabIndex = 1;
            btnSelectMycarPath.Text = "경로 선택";
            btnSelectMycarPath.UseVisualStyleBackColor = true;
            btnSelectMycarPath.Click += BtnSelectMycarPath_Click;
            // 
            // tabGraphStats
            // 
            tabGraphStats.Controls.Add(pnlChartHost);
            tabGraphStats.Controls.Add(lblChartDescription);
            tabGraphStats.Location = new Point(4, 24);
            tabGraphStats.Margin = new Padding(2);
            tabGraphStats.Name = "tabGraphStats";
            tabGraphStats.Padding = new Padding(2);
            tabGraphStats.Size = new Size(1221, 662);
            tabGraphStats.TabIndex = 2;
            tabGraphStats.Text = "그래프/통계";
            tabGraphStats.UseVisualStyleBackColor = true;
            // 
            // pnlChartHost
            // 
            pnlChartHost.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlChartHost.Location = new Point(57, 56);
            pnlChartHost.Margin = new Padding(2);
            pnlChartHost.Name = "pnlChartHost";
            pnlChartHost.Size = new Size(716, 306);
            pnlChartHost.TabIndex = 1;
            // 
            // lblChartDescription
            // 
            lblChartDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblChartDescription.AutoSize = true;
            lblChartDescription.Location = new Point(57, 20);
            lblChartDescription.Margin = new Padding(2, 0, 2, 0);
            lblChartDescription.Name = "lblChartDescription";
            lblChartDescription.Size = new Size(739, 15);
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
            stsDataPath.Size = new Size(43, 17);
            stsDataPath.Text = "경로: -";
            // 
            // stsFrameSummary
            // 
            stsFrameSummary.Name = "stsFrameSummary";
            stsFrameSummary.Size = new Size(59, 17);
            stsFrameSummary.Text = "Frames: 0";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1230, 715);
            Controls.Add(tabControlMain);
            Controls.Add(menuStripMain);
            MainMenuStrip = menuStripMain;
            MinimumSize = new Size(781, 533);
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
        private Label lblTrainingModelType;
        private Label lblTrainingPythonEnvName;
        private Label lblMycarProjectPath;
        private Button btnSelectMycarPath;
        private Button btnSelectTrainingModelPath;
        private Button btnDetectTrainingEnvironment;
        private Button btnSelectTrainingWslDistro;
        private Button btnSelectCondaPath;
        private ComboBox cmbMycarProjectPath;
        private TextBox txtTrainingPythonEnvName;
        private ComboBox cmbTrainingModelType;
        private Label lblTrainingWslDistro;
        private ComboBox cmbTrainingWslDistro;
        private ComboBox cmbCondaPath;
        private Label lblCondaPath;        private StatusStrip statusStripDataFooter;
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
