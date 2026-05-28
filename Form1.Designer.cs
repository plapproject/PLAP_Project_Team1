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
            lstFrameData = new ListBox();
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
            cbxModelType = new ComboBox();
            txtPythonPath = new TextBox();
            lblModeType = new Label();
            lblPythonPath = new Label();
            btnStopTraining = new Button();
            btnStartTraining = new Button();
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
            ((System.ComponentModel.ISupportInitialize)picMainPreview).BeginInit();
            tabPageTraining.SuspendLayout();
            statusStripTraining.SuspendLayout();
            grpTrainingLog.SuspendLayout();
            grpTrainingSettings.SuspendLayout();
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
            menuStripMain.Padding = new Padding(7, 4, 0, 4);
            menuStripMain.Size = new Size(1757, 37);
            menuStripMain.TabIndex = 0;
            menuStripMain.Text = "menuStripMain";
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuOpenDataFolder, mnuReloadData, mnuExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(64, 29);
            mnuFile.Text = "파일";
            // 
            // mnuOpenDataFolder
            // 
            mnuOpenDataFolder.Name = "mnuOpenDataFolder";
            mnuOpenDataFolder.Size = new Size(252, 34);
            mnuOpenDataFolder.Text = "데이터 폴더 열기";
            // 
            // mnuReloadData
            // 
            mnuReloadData.Name = "mnuReloadData";
            mnuReloadData.Size = new Size(252, 34);
            mnuReloadData.Text = "다시 불러오기";
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(252, 34);
            mnuExit.Text = "종료";
            // 
            // mnuView
            // 
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuToggleTheme, mnuOpenGraphStats });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(64, 29);
            mnuView.Text = "보기";
            // 
            // mnuToggleTheme
            // 
            mnuToggleTheme.Name = "mnuToggleTheme";
            mnuToggleTheme.Size = new Size(313, 34);
            mnuToggleTheme.Text = "테마 전환";
            // 
            // mnuOpenGraphStats
            // 
            mnuOpenGraphStats.Name = "mnuOpenGraphStats";
            mnuOpenGraphStats.Size = new Size(313, 34);
            mnuOpenGraphStats.Text = "그래프/통계 탭으로 이동";
            mnuOpenGraphStats.Click += mnuOpenGraphStats_Click;
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuOpenGuide });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(82, 29);
            mnuHelp.Text = "도움말";
            mnuHelp.Click += mnuHelp_Click;
            // 
            // mnuOpenGuide
            // 
            mnuOpenGuide.Name = "mnuOpenGuide";
            mnuOpenGuide.Size = new Size(228, 34);
            mnuOpenGuide.Text = "단계별 가이드";
            // 
            // tabControlMain
            // 
            tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlMain.Controls.Add(tabPageDataViewer);
            tabControlMain.Controls.Add(tabPageTraining);
            tabControlMain.Controls.Add(tabPageGraphStats);
            tabControlMain.Location = new Point(3, 39);
            tabControlMain.Margin = new Padding(3, 4, 3, 4);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1756, 1150);
            tabControlMain.TabIndex = 1;
            // 
            // tabPageDataViewer
            // 
            tabPageDataViewer.Controls.Add(statusStripDataFooter);
            tabPageDataViewer.Controls.Add(grpTubCleaner);
            tabPageDataViewer.Controls.Add(grpTubExplorer);
            tabPageDataViewer.Location = new Point(4, 34);
            tabPageDataViewer.Margin = new Padding(3, 4, 3, 4);
            tabPageDataViewer.Name = "tabPageDataViewer";
            tabPageDataViewer.Padding = new Padding(3, 4, 3, 4);
            tabPageDataViewer.Size = new Size(1748, 1112);
            tabPageDataViewer.TabIndex = 0;
            tabPageDataViewer.Text = "데이터 뷰어";
            tabPageDataViewer.UseVisualStyleBackColor = true;
            // 
            // statusStripDataFooter
            // 
            statusStripDataFooter.ImageScalingSize = new Size(20, 20);
            statusStripDataFooter.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelDataPath });
            statusStripDataFooter.Location = new Point(3, 1076);
            statusStripDataFooter.Name = "statusStripDataFooter";
            statusStripDataFooter.Padding = new Padding(1, 0, 16, 0);
            statusStripDataFooter.Size = new Size(1742, 32);
            statusStripDataFooter.TabIndex = 24;
            statusStripDataFooter.Text = "statusStripDataFooter";
            // 
            // toolStripStatusLabelDataPath
            // 
            toolStripStatusLabelDataPath.Name = "toolStripStatusLabelDataPath";
            toolStripStatusLabelDataPath.Size = new Size(41, 25);
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
            grpTubCleaner.Location = new Point(3, 794);
            grpTubCleaner.Margin = new Padding(3, 4, 3, 4);
            grpTubCleaner.Name = "grpTubCleaner";
            grpTubCleaner.Padding = new Padding(3, 4, 3, 4);
            grpTubCleaner.Size = new Size(1737, 275);
            grpTubCleaner.TabIndex = 23;
            grpTubCleaner.TabStop = false;
            grpTubCleaner.Text = "터브 정리기";
            grpTubCleaner.Enter += grpTubCleaner_Enter;
            // 
            // lblSelectRangeSeparator
            // 
            lblSelectRangeSeparator.AutoSize = true;
            lblSelectRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblSelectRangeSeparator.Location = new Point(447, 86);
            lblSelectRangeSeparator.Name = "lblSelectRangeSeparator";
            lblSelectRangeSeparator.Size = new Size(32, 28);
            lblSelectRangeSeparator.TabIndex = 47;
            lblSelectRangeSeparator.Text = "～";
            // 
            // txtSelectRangeMax
            // 
            txtSelectRangeMax.Location = new Point(483, 84);
            txtSelectRangeMax.Margin = new Padding(4, 5, 4, 5);
            txtSelectRangeMax.Name = "txtSelectRangeMax";
            txtSelectRangeMax.Size = new Size(144, 31);
            txtSelectRangeMax.TabIndex = 46;
            // 
            // txtSelectRangeMin
            // 
            txtSelectRangeMin.Location = new Point(296, 84);
            txtSelectRangeMin.Margin = new Padding(4, 5, 4, 5);
            txtSelectRangeMin.Name = "txtSelectRangeMin";
            txtSelectRangeMin.Size = new Size(144, 31);
            txtSelectRangeMin.TabIndex = 45;
            // 
            // lblSelectRange
            // 
            lblSelectRange.AutoSize = true;
            lblSelectRange.Font = new Font("맑은 고딕", 10F);
            lblSelectRange.Location = new Point(107, 88);
            lblSelectRange.Margin = new Padding(4, 0, 4, 0);
            lblSelectRange.Name = "lblSelectRange";
            lblSelectRange.Size = new Size(99, 28);
            lblSelectRange.TabIndex = 44;
            lblSelectRange.Text = "구간 선택";
            // 
            // btnRestoreFrame
            // 
            btnRestoreFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRestoreFrame.Location = new Point(1404, 179);
            btnRestoreFrame.Margin = new Padding(3, 4, 3, 4);
            btnRestoreFrame.Name = "btnRestoreFrame";
            btnRestoreFrame.Size = new Size(239, 36);
            btnRestoreFrame.TabIndex = 43;
            btnRestoreFrame.Text = "복구";
            btnRestoreFrame.UseVisualStyleBackColor = true;
            // 
            // btnDeleteFrame
            // 
            btnDeleteFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteFrame.Location = new Point(1148, 176);
            btnDeleteFrame.Margin = new Padding(3, 4, 3, 4);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(239, 36);
            btnDeleteFrame.TabIndex = 42;
            btnDeleteFrame.Text = "삭제";
            btnDeleteFrame.UseVisualStyleBackColor = true;
            // 
            // btnExcludeRange
            // 
            btnExcludeRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeRange.Location = new Point(1148, 82);
            btnExcludeRange.Margin = new Padding(3, 4, 3, 4);
            btnExcludeRange.Name = "btnExcludeRange";
            btnExcludeRange.Size = new Size(239, 36);
            btnExcludeRange.TabIndex = 40;
            btnExcludeRange.Text = "구간 제외";
            btnExcludeRange.UseVisualStyleBackColor = true;
            // 
            // btnExcludeSelectedFrame
            // 
            btnExcludeSelectedFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeSelectedFrame.Location = new Point(1404, 85);
            btnExcludeSelectedFrame.Margin = new Padding(3, 4, 3, 4);
            btnExcludeSelectedFrame.Name = "btnExcludeSelectedFrame";
            btnExcludeSelectedFrame.Size = new Size(239, 36);
            btnExcludeSelectedFrame.TabIndex = 41;
            btnExcludeSelectedFrame.Text = "선택 프레임 제외";
            btnExcludeSelectedFrame.UseVisualStyleBackColor = true;
            // 
            // cbxScenarioFilter
            // 
            cbxScenarioFilter.Font = new Font("맑은 고딕", 10F);
            cbxScenarioFilter.FormattingEnabled = true;
            cbxScenarioFilter.Items.AddRange(new object[] { "All", "Normal", "Night", "Left_turn", "Right_turn", "Out_of_bound" });
            cbxScenarioFilter.Location = new Point(783, 178);
            cbxScenarioFilter.Margin = new Padding(3, 4, 3, 4);
            cbxScenarioFilter.Name = "cbxScenarioFilter";
            cbxScenarioFilter.Size = new Size(307, 36);
            cbxScenarioFilter.TabIndex = 39;
            // 
            // lblScenarioFilter
            // 
            lblScenarioFilter.AutoSize = true;
            lblScenarioFilter.Font = new Font("맑은 고딕", 10F);
            lblScenarioFilter.Location = new Point(680, 178);
            lblScenarioFilter.Name = "lblScenarioFilter";
            lblScenarioFilter.Size = new Size(103, 28);
            lblScenarioFilter.TabIndex = 38;
            lblScenarioFilter.Text = "시나리오: ";
            // 
            // cbxModeFilter
            // 
            cbxModeFilter.Font = new Font("맑은 고딕", 10F);
            cbxModeFilter.FormattingEnabled = true;
            cbxModeFilter.Items.AddRange(new object[] { "All", "User", "Local", "Local_angle" });
            cbxModeFilter.Location = new Point(783, 126);
            cbxModeFilter.Margin = new Padding(3, 4, 3, 4);
            cbxModeFilter.Name = "cbxModeFilter";
            cbxModeFilter.Size = new Size(307, 36);
            cbxModeFilter.TabIndex = 37;
            // 
            // lblModeFilter
            // 
            lblModeFilter.AutoSize = true;
            lblModeFilter.Font = new Font("맑은 고딕", 10F);
            lblModeFilter.Location = new Point(680, 129);
            lblModeFilter.Name = "lblModeFilter";
            lblModeFilter.Size = new Size(98, 28);
            lblModeFilter.TabIndex = 36;
            lblModeFilter.Text = "모드      :";
            // 
            // lblAngleRangeSeparator
            // 
            lblAngleRangeSeparator.AutoSize = true;
            lblAngleRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblAngleRangeSeparator.Location = new Point(447, 136);
            lblAngleRangeSeparator.Name = "lblAngleRangeSeparator";
            lblAngleRangeSeparator.Size = new Size(32, 28);
            lblAngleRangeSeparator.TabIndex = 35;
            lblAngleRangeSeparator.Text = "～";
            // 
            // lblThrottleRangeSeparator
            // 
            lblThrottleRangeSeparator.AutoSize = true;
            lblThrottleRangeSeparator.Font = new Font("맑은 고딕", 10F);
            lblThrottleRangeSeparator.Location = new Point(447, 188);
            lblThrottleRangeSeparator.Name = "lblThrottleRangeSeparator";
            lblThrottleRangeSeparator.Size = new Size(32, 28);
            lblThrottleRangeSeparator.TabIndex = 34;
            lblThrottleRangeSeparator.Text = "～";
            // 
            // txtThrottleMax
            // 
            txtThrottleMax.Font = new Font("맑은 고딕", 10F);
            txtThrottleMax.Location = new Point(483, 184);
            txtThrottleMax.Margin = new Padding(3, 4, 3, 4);
            txtThrottleMax.Name = "txtThrottleMax";
            txtThrottleMax.Size = new Size(144, 34);
            txtThrottleMax.TabIndex = 33;
            // 
            // txtAngleMax
            // 
            txtAngleMax.Font = new Font("맑은 고딕", 10F);
            txtAngleMax.Location = new Point(483, 131);
            txtAngleMax.Margin = new Padding(3, 4, 3, 4);
            txtAngleMax.Name = "txtAngleMax";
            txtAngleMax.Size = new Size(144, 34);
            txtAngleMax.TabIndex = 32;
            // 
            // txtThrottleMin
            // 
            txtThrottleMin.Font = new Font("맑은 고딕", 10F);
            txtThrottleMin.Location = new Point(296, 184);
            txtThrottleMin.Margin = new Padding(3, 4, 3, 4);
            txtThrottleMin.Name = "txtThrottleMin";
            txtThrottleMin.Size = new Size(144, 34);
            txtThrottleMin.TabIndex = 31;
            // 
            // txtAngleMin
            // 
            txtAngleMin.Font = new Font("맑은 고딕", 10F);
            txtAngleMin.Location = new Point(296, 131);
            txtAngleMin.Margin = new Padding(3, 4, 3, 4);
            txtAngleMin.Name = "txtAngleMin";
            txtAngleMin.Size = new Size(144, 34);
            txtAngleMin.TabIndex = 30;
            // 
            // lblThrottleRange
            // 
            lblThrottleRange.AutoSize = true;
            lblThrottleRange.Font = new Font("맑은 고딕", 10F);
            lblThrottleRange.Location = new Point(107, 188);
            lblThrottleRange.Name = "lblThrottleRange";
            lblThrottleRange.Size = new Size(198, 28);
            lblThrottleRange.TabIndex = 26;
            lblThrottleRange.Text = "스로틀값 범위(0~1): ";
            // 
            // lblAngleRange
            // 
            lblAngleRange.AutoSize = true;
            lblAngleRange.Font = new Font("맑은 고딕", 10F);
            lblAngleRange.Location = new Point(107, 136);
            lblAngleRange.Name = "lblAngleRange";
            lblAngleRange.Size = new Size(193, 28);
            lblAngleRange.TabIndex = 25;
            lblAngleRange.Text = "조향각 범위 (-1~1): ";
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFilter.Location = new Point(1149, 129);
            btnApplyFilter.Margin = new Padding(3, 4, 3, 4);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(239, 36);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilter.Location = new Point(1404, 131);
            btnClearFilter.Margin = new Padding(3, 4, 3, 4);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(239, 36);
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
            grpTubExplorer.Margin = new Padding(3, 4, 3, 4);
            grpTubExplorer.Name = "grpTubExplorer";
            grpTubExplorer.Padding = new Padding(3, 4, 3, 4);
            grpTubExplorer.Size = new Size(1743, 794);
            grpTubExplorer.TabIndex = 22;
            grpTubExplorer.TabStop = false;
            grpTubExplorer.Text = "터브 탐색기";
            // 
            // trkFramePosition
            // 
            trkFramePosition.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trkFramePosition.Location = new Point(-1, 714);
            trkFramePosition.Margin = new Padding(3, 4, 3, 4);
            trkFramePosition.Maximum = 0;
            trkFramePosition.Name = "trkFramePosition";
            trkFramePosition.Size = new Size(1737, 69);
            trkFramePosition.TabIndex = 18;
            trkFramePosition.Scroll += trkFramePosition_Scroll;
            // 
            // numPlaybackInterval
            // 
            numPlaybackInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numPlaybackInterval.Location = new Point(1559, 411);
            numPlaybackInterval.Margin = new Padding(3, 4, 3, 4);
            numPlaybackInterval.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numPlaybackInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numPlaybackInterval.Name = "numPlaybackInterval";
            numPlaybackInterval.Size = new Size(173, 31);
            numPlaybackInterval.TabIndex = 11;
            numPlaybackInterval.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Location = new Point(0, 34);
            btnOpenFolder.Margin = new Padding(3, 4, 3, 4);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(156, 36);
            btnOpenFolder.TabIndex = 0;
            btnOpenFolder.Text = "데이터 폴더 열기";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // lblPlayInterval
            // 
            lblPlayInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPlayInterval.Font = new Font("맑은 고딕", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblPlayInterval.Location = new Point(1427, 410);
            lblPlayInterval.Name = "lblPlayInterval";
            lblPlayInterval.Size = new Size(141, 39);
            lblPlayInterval.TabIndex = 21;
            lblPlayInterval.Text = "재생간격(ms)";
            lblPlayInterval.Click += lblPlayInterval_Click;
            // 
            // btnLast
            // 
            btnLast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLast.Location = new Point(1581, 175);
            btnLast.Margin = new Padding(3, 4, 3, 4);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(149, 36);
            btnLast.TabIndex = 17;
            btnLast.Text = ">>";
            btnLast.Click += btnLast_Click;
            // 
            // btnAutoPlay
            // 
            btnAutoPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAutoPlay.Location = new Point(1430, 220);
            btnAutoPlay.Margin = new Padding(3, 4, 3, 4);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(300, 34);
            btnAutoPlay.TabIndex = 20;
            btnAutoPlay.Text = "자동 재생";
            btnAutoPlay.UseVisualStyleBackColor = true;
            btnAutoPlay.Click += btnAutoPlay_Click;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNext.Location = new Point(1581, 130);
            btnNext.Margin = new Padding(3, 4, 3, 4);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(149, 36);
            btnNext.TabIndex = 16;
            btnNext.Text = ">";
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrev.Location = new Point(1427, 130);
            btnPrev.Margin = new Padding(3, 4, 3, 4);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(149, 36);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "<";
            btnPrev.Click += btnPrev_Click;
            // 
            // splitContainerFramePreview
            // 
            splitContainerFramePreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerFramePreview.Location = new Point(0, 76);
            splitContainerFramePreview.Margin = new Padding(3, 4, 3, 4);
            splitContainerFramePreview.Name = "splitContainerFramePreview";
            // 
            // splitContainerFramePreview.Panel1
            // 
            splitContainerFramePreview.Panel1.Controls.Add(lstFrameData);
            // 
            // splitContainerFramePreview.Panel2
            // 
            splitContainerFramePreview.Panel2.Controls.Add(picMainPreview);
            splitContainerFramePreview.Size = new Size(1420, 629);
            splitContainerFramePreview.SplitterDistance = 400;
            splitContainerFramePreview.SplitterWidth = 7;
            splitContainerFramePreview.TabIndex = 44;
            // 
            // lstFrameData
            // 
            lstFrameData.Dock = DockStyle.Fill;
            lstFrameData.Location = new Point(0, 0);
            lstFrameData.Margin = new Padding(3, 4, 3, 4);
            lstFrameData.Name = "lstFrameData";
            lstFrameData.Size = new Size(400, 629);
            lstFrameData.TabIndex = 13;
            lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;
            // 
            // picMainPreview
            // 
            picMainPreview.BorderStyle = BorderStyle.FixedSingle;
            picMainPreview.Dock = DockStyle.Fill;
            picMainPreview.Location = new Point(0, 0);
            picMainPreview.Margin = new Padding(3, 4, 3, 4);
            picMainPreview.Name = "picMainPreview";
            picMainPreview.Size = new Size(1013, 629);
            picMainPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picMainPreview.TabIndex = 4;
            picMainPreview.TabStop = false;
            // 
            // btnFirst
            // 
            btnFirst.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFirst.Location = new Point(1427, 175);
            btnFirst.Margin = new Padding(3, 4, 3, 4);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(149, 36);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "<<";
            btnFirst.Click += btnFirst_Click;
            // 
            // btnReload
            // 
            btnReload.Location = new Point(163, 34);
            btnReload.Margin = new Padding(3, 4, 3, 4);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(156, 36);
            btnReload.TabIndex = 1;
            btnReload.Text = "다시 불러오기";
            btnReload.UseVisualStyleBackColor = true;
            btnReload.Click += btnReload_Click;
            // 
            // lblFrameValue
            // 
            lblFrameValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblFrameValue.Font = new Font("맑은 고딕", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblFrameValue.Location = new Point(1427, 76);
            lblFrameValue.Name = "lblFrameValue";
            lblFrameValue.Size = new Size(307, 49);
            lblFrameValue.TabIndex = 5;
            lblFrameValue.Text = "Frame: 0/0";
            // 
            // lblModeValue
            // 
            lblModeValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblModeValue.Font = new Font("맑은 고딕", 13.8F);
            lblModeValue.Location = new Point(1430, 359);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(303, 140);
            lblModeValue.TabIndex = 8;
            lblModeValue.Text = "모드: -";
            lblModeValue.Click += lblModeValue_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Location = new Point(324, 34);
            btnToggleTheme.Margin = new Padding(3, 4, 3, 4);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(156, 36);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "테마 전환";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // lblThrottleValue
            // 
            lblThrottleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblThrottleValue.Font = new Font("맑은 고딕", 13.8F);
            lblThrottleValue.Location = new Point(1430, 314);
            lblThrottleValue.Name = "lblThrottleValue";
            lblThrottleValue.Size = new Size(303, 46);
            lblThrottleValue.TabIndex = 7;
            lblThrottleValue.Text = "스로틀값: 0.000";
            lblThrottleValue.Click += lblThrottleValue_Click;
            // 
            // btnGuide
            // 
            btnGuide.Location = new Point(487, 34);
            btnGuide.Margin = new Padding(3, 4, 3, 4);
            btnGuide.Name = "btnGuide";
            btnGuide.Size = new Size(156, 36);
            btnGuide.TabIndex = 3;
            btnGuide.Text = "단계별 가이드";
            btnGuide.UseVisualStyleBackColor = true;
            btnGuide.Click += btnGuide_Click;
            // 
            // lblAngleValue
            // 
            lblAngleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAngleValue.Font = new Font("맑은 고딕", 13.8F);
            lblAngleValue.Location = new Point(1430, 264);
            lblAngleValue.Name = "lblAngleValue";
            lblAngleValue.Size = new Size(303, 49);
            lblAngleValue.TabIndex = 6;
            lblAngleValue.Text = "조향값: 0.000";
            lblAngleValue.Click += lblAngleValue_Click;
            // 
            // tabPageTraining
            // 
            tabPageTraining.Controls.Add(statusStripTraining);
            tabPageTraining.Controls.Add(grpTrainingLog);
            tabPageTraining.Controls.Add(grpTrainingSettings);
            tabPageTraining.Location = new Point(4, 34);
            tabPageTraining.Margin = new Padding(3, 4, 3, 4);
            tabPageTraining.Name = "tabPageTraining";
            tabPageTraining.Padding = new Padding(3, 4, 3, 4);
            tabPageTraining.Size = new Size(1748, 1112);
            tabPageTraining.TabIndex = 1;
            tabPageTraining.Text = "학습 실행";
            tabPageTraining.UseVisualStyleBackColor = true;
            tabPageTraining.Click += tabPageTraining_Click;
            // 
            // statusStripTraining
            // 
            statusStripTraining.ImageScalingSize = new Size(20, 20);
            statusStripTraining.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelTraining });
            statusStripTraining.Location = new Point(3, 1076);
            statusStripTraining.Name = "statusStripTraining";
            statusStripTraining.Padding = new Padding(1, 0, 16, 0);
            statusStripTraining.Size = new Size(1742, 32);
            statusStripTraining.TabIndex = 2;
            statusStripTraining.Text = "statusStripTraining";
            // 
            // toolStripStatusLabelTraining
            // 
            toolStripStatusLabelTraining.Name = "toolStripStatusLabelTraining";
            toolStripStatusLabelTraining.Size = new Size(124, 25);
            toolStripStatusLabelTraining.Text = "Tub 경로: C:\\";
            toolStripStatusLabelTraining.Click += toolStripStatusLabelTraining_Click;
            // 
            // grpTrainingLog
            // 
            grpTrainingLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingLog.Controls.Add(rtbTrainingLog);
            grpTrainingLog.Location = new Point(17, 576);
            grpTrainingLog.Margin = new Padding(11, 14, 11, 14);
            grpTrainingLog.Name = "grpTrainingLog";
            grpTrainingLog.Padding = new Padding(3, 4, 3, 4);
            grpTrainingLog.Size = new Size(1719, 486);
            grpTrainingLog.TabIndex = 1;
            grpTrainingLog.TabStop = false;
            grpTrainingLog.Text = "학습 로그";
            // 
            // rtbTrainingLog
            // 
            rtbTrainingLog.Dock = DockStyle.Fill;
            rtbTrainingLog.Location = new Point(3, 28);
            rtbTrainingLog.Margin = new Padding(3, 4, 3, 4);
            rtbTrainingLog.Name = "rtbTrainingLog";
            rtbTrainingLog.Size = new Size(1713, 454);
            rtbTrainingLog.TabIndex = 0;
            rtbTrainingLog.Text = "";
            // 
            // grpTrainingSettings
            // 
            grpTrainingSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingSettings.Controls.Add(cbxModelType);
            grpTrainingSettings.Controls.Add(txtPythonPath);
            grpTrainingSettings.Controls.Add(lblModeType);
            grpTrainingSettings.Controls.Add(lblPythonPath);
            grpTrainingSettings.Controls.Add(btnStopTraining);
            grpTrainingSettings.Controls.Add(btnStartTraining);
            grpTrainingSettings.Location = new Point(17, 24);
            grpTrainingSettings.Margin = new Padding(3, 4, 3, 4);
            grpTrainingSettings.Name = "grpTrainingSettings";
            grpTrainingSettings.Padding = new Padding(3, 4, 3, 4);
            grpTrainingSettings.Size = new Size(1719, 546);
            grpTrainingSettings.TabIndex = 0;
            grpTrainingSettings.TabStop = false;
            grpTrainingSettings.Text = "학습 설정";
            // 
            // cbxModelType
            // 
            cbxModelType.Font = new Font("맑은 고딕", 12F);
            cbxModelType.FormattingEnabled = true;
            cbxModelType.Items.AddRange(new object[] { "linear", "categorical", "inferred" });
            cbxModelType.Location = new Point(243, 164);
            cbxModelType.Margin = new Padding(3, 4, 3, 4);
            cbxModelType.Name = "cbxModelType";
            cbxModelType.Size = new Size(434, 40);
            cbxModelType.TabIndex = 2;
            // 
            // txtPythonPath
            // 
            txtPythonPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPythonPath.Font = new Font("맑은 고딕", 12F);
            txtPythonPath.Location = new Point(243, 92);
            txtPythonPath.Margin = new Padding(3, 4, 3, 4);
            txtPythonPath.Name = "txtPythonPath";
            txtPythonPath.Size = new Size(1412, 39);
            txtPythonPath.TabIndex = 13;
            // 
            // lblModeType
            // 
            lblModeType.AutoSize = true;
            lblModeType.Font = new Font("맑은 고딕", 12F);
            lblModeType.Location = new Point(50, 168);
            lblModeType.Name = "lblModeType";
            lblModeType.Size = new Size(118, 32);
            lblModeType.TabIndex = 10;
            lblModeType.Text = "모델 종류";
            lblModeType.Click += lblModeType_Click;
            // 
            // lblPythonPath
            // 
            lblPythonPath.AutoSize = true;
            lblPythonPath.Font = new Font("맑은 고딕", 12F);
            lblPythonPath.Location = new Point(50, 98);
            lblPythonPath.Name = "lblPythonPath";
            lblPythonPath.Size = new Size(166, 32);
            lblPythonPath.TabIndex = 7;
            lblPythonPath.Text = "가상환경 이름";
            // 
            // btnStopTraining
            // 
            btnStopTraining.Font = new Font("맑은 고딕", 12F);
            btnStopTraining.Location = new Point(974, 434);
            btnStopTraining.Margin = new Padding(3, 4, 3, 4);
            btnStopTraining.Name = "btnStopTraining";
            btnStopTraining.Size = new Size(216, 54);
            btnStopTraining.TabIndex = 4;
            btnStopTraining.Text = "학습 중지";
            btnStopTraining.UseVisualStyleBackColor = true;
            // 
            // btnStartTraining
            // 
            btnStartTraining.Font = new Font("맑은 고딕", 12F);
            btnStartTraining.Location = new Point(538, 434);
            btnStartTraining.Margin = new Padding(3, 4, 3, 4);
            btnStartTraining.Name = "btnStartTraining";
            btnStartTraining.Size = new Size(216, 54);
            btnStartTraining.TabIndex = 3;
            btnStartTraining.Text = "학습 시작";
            btnStartTraining.UseVisualStyleBackColor = true;
            btnStartTraining.Click += btnStartTraining_Click;
            // 
            // tabPageGraphStats
            // 
            tabPageGraphStats.Controls.Add(pnlGraphStats);
            tabPageGraphStats.Controls.Add(lblDescription);
            tabPageGraphStats.Location = new Point(4, 34);
            tabPageGraphStats.Margin = new Padding(3, 4, 3, 4);
            tabPageGraphStats.Name = "tabPageGraphStats";
            tabPageGraphStats.Padding = new Padding(3, 4, 3, 4);
            tabPageGraphStats.Size = new Size(1748, 1112);
            tabPageGraphStats.TabIndex = 2;
            tabPageGraphStats.Text = "그래프/통계";
            tabPageGraphStats.UseVisualStyleBackColor = true;
            // 
            // pnlGraphStats
            // 
            pnlGraphStats.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlGraphStats.Location = new Point(81, 94);
            pnlGraphStats.Margin = new Padding(3, 4, 3, 4);
            pnlGraphStats.Name = "pnlGraphStats";
            pnlGraphStats.Size = new Size(1023, 510);
            pnlGraphStats.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(81, 34);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(1108, 25);
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
            toolStripStatusLabelPath.Size = new Size(65, 15);
            toolStripStatusLabelPath.Text = "경로: -";
            // 
            // toolStripStatusLabelFrames
            // 
            toolStripStatusLabelFrames.Name = "toolStripStatusLabelFrames";
            toolStripStatusLabelFrames.Size = new Size(90, 15);
            toolStripStatusLabelFrames.Text = "Frames: 0";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1757, 1191);
            Controls.Add(tabControlMain);
            Controls.Add(menuStripMain);
            MainMenuStrip = menuStripMain;
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1108, 858);
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
            ((System.ComponentModel.ISupportInitialize)picMainPreview).EndInit();
            tabPageTraining.ResumeLayout(false);
            tabPageTraining.PerformLayout();
            statusStripTraining.ResumeLayout(false);
            statusStripTraining.PerformLayout();
            grpTrainingLog.ResumeLayout(false);
            grpTrainingSettings.ResumeLayout(false);
            grpTrainingSettings.PerformLayout();
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
        private SplitContainer splitContainerFramePreview;
        private GroupBox grpTubCleaner;
        private Label lblThrottleRange;
        private Label lblAngleRange;
        private TabPage tabPageTraining;
        private GroupBox grpTrainingSettings;
        private GroupBox grpTrainingLog;
        private Label lblModeType;
        private Label lblPythonPath;
        private Button btnStopTraining;
        private Button btnStartTraining;
        private TextBox txtPythonPath;
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
        private Button btnDeleteFrame;
        private Button btnExcludeRange;
        private Button btnExcludeSelectedFrame;
        private Label lblSelectRange;
        private TextBox txtSelectRangeMin;
        private TextBox txtSelectRangeMax;
        private Label lblSelectRangeSeparator;
    }
}
