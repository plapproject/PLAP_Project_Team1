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
            txtSelectRangeMin = new TextBox();
            txtSelectRangeMax = new TextBox();
            lblSelectRangeSeparator = new Label();
            menuStripMain.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageDataViewer.SuspendLayout();
            statusStripDataFooter.SuspendLayout();
            grpTubCleaner.SuspendLayout();
            grpTubExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkFramePosition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPlaybackInterval).BeginInit();
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
            menuStripMain.Padding = new Padding(5, 2, 0, 2);
            menuStripMain.Size = new Size(1230, 24);
            menuStripMain.TabIndex = 0;
            menuStripMain.Text = "menuStripMain";
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuOpenDataFolder, mnuReloadData, mnuExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(43, 20);
            mnuFile.Text = "ÆÄÀÏ";
            // 
            // mnuOpenDataFolder
            // 
            mnuOpenDataFolder.Name = "mnuOpenDataFolder";
            mnuOpenDataFolder.Size = new Size(166, 22);
            mnuOpenDataFolder.Text = "µ¥ÀÌÅÍ Æú´õ ¿­±â";
            // 
            // mnuReloadData
            // 
            mnuReloadData.Name = "mnuReloadData";
            mnuReloadData.Size = new Size(166, 22);
            mnuReloadData.Text = "´Ù½Ã ºÒ·¯¿À±â";
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(166, 22);
            mnuExit.Text = "Á¾·á";
            // 
            // mnuView
            // 
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuToggleTheme, mnuOpenGraphStats });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(43, 20);
            mnuView.Text = "º¸±â";
            // 
            // mnuToggleTheme
            // 
            mnuToggleTheme.Name = "mnuToggleTheme";
            mnuToggleTheme.Size = new Size(207, 22);
            mnuToggleTheme.Text = "Å×¸¶ ÀüÈ¯";
            // 
            // mnuOpenGraphStats
            // 
            mnuOpenGraphStats.Name = "mnuOpenGraphStats";
            mnuOpenGraphStats.Size = new Size(207, 22);
            mnuOpenGraphStats.Text = "±×·¡ÇÁ/Åë°è ÅÇÀ¸·Î ÀÌµ¿";
            mnuOpenGraphStats.Click += mnuOpenGraphStats_Click;
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuOpenGuide });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(55, 20);
            mnuHelp.Text = "µµ¿ò¸»";
            mnuHelp.Click += mnuHelp_Click;
            // 
            // mnuOpenGuide
            // 
            mnuOpenGuide.Name = "mnuOpenGuide";
            mnuOpenGuide.Size = new Size(150, 22);
            mnuOpenGuide.Text = "´Ü°èº° °¡ÀÌµå";
            // 
            // tabControlMain
            // 
            tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlMain.Controls.Add(tabPageDataViewer);
            tabControlMain.Controls.Add(tabPageTraining);
            tabControlMain.Controls.Add(tabPageGraphStats);
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
            tabPageDataViewer.Controls.Add(grpTubCleaner);
            tabPageDataViewer.Controls.Add(grpTubExplorer);
            tabPageDataViewer.Location = new Point(4, 24);
            tabPageDataViewer.Margin = new Padding(2);
            tabPageDataViewer.Name = "tabPageDataViewer";
            tabPageDataViewer.Padding = new Padding(2);
            tabPageDataViewer.Size = new Size(1221, 662);
            tabPageDataViewer.TabIndex = 0;
            tabPageDataViewer.Text = "µ¥ÀÌÅÍ ºä¾î";
            tabPageDataViewer.UseVisualStyleBackColor = true;
            // 
            // statusStripDataFooter
            // 
            statusStripDataFooter.ImageScalingSize = new Size(20, 20);
            statusStripDataFooter.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelDataPath });
            statusStripDataFooter.Location = new Point(2, 638);
            statusStripDataFooter.Name = "statusStripDataFooter";
            statusStripDataFooter.Padding = new Padding(1, 0, 11, 0);
            statusStripDataFooter.Size = new Size(1217, 22);
            statusStripDataFooter.TabIndex = 24;
            statusStripDataFooter.Text = "statusStripDataFooter";
            // 
            // toolStripStatusLabelDataPath
            // 
            toolStripStatusLabelDataPath.Name = "toolStripStatusLabelDataPath";
            toolStripStatusLabelDataPath.Size = new Size(27, 17);
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
            grpTubCleaner.Location = new Point(2, 476);
            grpTubCleaner.Margin = new Padding(2);
            grpTubCleaner.Name = "grpTubCleaner";
            grpTubCleaner.Padding = new Padding(2);
            grpTubCleaner.Size = new Size(1216, 165);
            grpTubCleaner.TabIndex = 23;
            grpTubCleaner.TabStop = false;
            grpTubCleaner.Text = "ÅÍºê Á¤¸®±â";
            grpTubCleaner.Enter += grpTubCleaner_Enter;
            // 
            // lblSelectRange
            // 
            lblSelectRange.AutoSize = true;
            lblSelectRange.Font = new Font("¸¼Àº °íµñ", 10F);
            lblSelectRange.Location = new Point(263, 18);
            lblSelectRange.Name = "lblSelectRange";
            lblSelectRange.Size = new Size(70, 19);
            lblSelectRange.TabIndex = 44;
            lblSelectRange.Text = "±¸°£ ¼±ÅÃ";
            // 
            // btnRestoreFrame
            // 
            btnRestoreFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRestoreFrame.Location = new Point(849, 112);
            btnRestoreFrame.Margin = new Padding(2);
            btnRestoreFrame.Name = "btnRestoreFrame";
            btnRestoreFrame.Size = new Size(116, 22);
            btnRestoreFrame.TabIndex = 43;
            btnRestoreFrame.Text = "º¹±¸";
            btnRestoreFrame.UseVisualStyleBackColor = true;
            // 
            // btnDeleteFrame
            // 
            btnDeleteFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteFrame.Location = new Point(729, 112);
            btnDeleteFrame.Margin = new Padding(2);
            btnDeleteFrame.Name = "btnDeleteFrame";
            btnDeleteFrame.Size = new Size(116, 22);
            btnDeleteFrame.TabIndex = 42;
            btnDeleteFrame.Text = "»èÁ¦";
            btnDeleteFrame.UseVisualStyleBackColor = true;
            // 
            // btnExcludeRange
            // 
            btnExcludeRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeRange.Location = new Point(488, 112);
            btnExcludeRange.Margin = new Padding(2);
            btnExcludeRange.Name = "btnExcludeRange";
            btnExcludeRange.Size = new Size(116, 22);
            btnExcludeRange.TabIndex = 40;
            btnExcludeRange.Text = "±¸°£ Á¦¿Ü";
            btnExcludeRange.UseVisualStyleBackColor = true;
            // 
            // btnExcludeSelectedFrame
            // 
            btnExcludeSelectedFrame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcludeSelectedFrame.Location = new Point(608, 112);
            btnExcludeSelectedFrame.Margin = new Padding(2);
            btnExcludeSelectedFrame.Name = "btnExcludeSelectedFrame";
            btnExcludeSelectedFrame.Size = new Size(116, 22);
            btnExcludeSelectedFrame.TabIndex = 41;
            btnExcludeSelectedFrame.Text = "¼±ÅÃ ÇÁ·¹ÀÓ Á¦¿Ü";
            btnExcludeSelectedFrame.UseVisualStyleBackColor = true;
            // 
            // cbxScenarioFilter
            // 
            cbxScenarioFilter.Font = new Font("¸¼Àº °íµñ", 10F);
            cbxScenarioFilter.FormattingEnabled = true;
            cbxScenarioFilter.Items.AddRange(new object[] { "All", "Normal", "Night", "Left_turn", "Right_turn", "Out_of_bound" });
            cbxScenarioFilter.Location = new Point(733, 69);
            cbxScenarioFilter.Margin = new Padding(2);
            cbxScenarioFilter.Name = "cbxScenarioFilter";
            cbxScenarioFilter.Size = new Size(216, 25);
            cbxScenarioFilter.TabIndex = 39;
            // 
            // lblScenarioFilter
            // 
            lblScenarioFilter.AutoSize = true;
            lblScenarioFilter.Font = new Font("¸¼Àº °íµñ", 10F);
            lblScenarioFilter.Location = new Point(660, 69);
            lblScenarioFilter.Margin = new Padding(2, 0, 2, 0);
            lblScenarioFilter.Name = "lblScenarioFilter";
            lblScenarioFilter.Size = new Size(73, 19);
            lblScenarioFilter.TabIndex = 38;
            lblScenarioFilter.Text = "½Ã³ª¸®¿À: ";
            // 
            // cbxModeFilter
            // 
            cbxModeFilter.Font = new Font("¸¼Àº °íµñ", 10F);
            cbxModeFilter.FormattingEnabled = true;
            cbxModeFilter.Items.AddRange(new object[] { "All", "User", "Local", "Local_angel" });
            cbxModeFilter.Location = new Point(733, 38);
            cbxModeFilter.Margin = new Padding(2);
            cbxModeFilter.Name = "cbxModeFilter";
            cbxModeFilter.Size = new Size(216, 25);
            cbxModeFilter.TabIndex = 37;
            // 
            // lblModeFilter
            // 
            lblModeFilter.AutoSize = true;
            lblModeFilter.Font = new Font("¸¼Àº °íµñ", 10F);
            lblModeFilter.Location = new Point(660, 40);
            lblModeFilter.Margin = new Padding(2, 0, 2, 0);
            lblModeFilter.Name = "lblModeFilter";
            lblModeFilter.Size = new Size(70, 19);
            lblModeFilter.TabIndex = 36;
            lblModeFilter.Text = "¸ðµå      :";
            // 
            // lblAngleRangeSeparator
            // 
            lblAngleRangeSeparator.AutoSize = true;
            lblAngleRangeSeparator.Font = new Font("¸¼Àº °íµñ", 10F);
            lblAngleRangeSeparator.Location = new Point(497, 44);
            lblAngleRangeSeparator.Margin = new Padding(2, 0, 2, 0);
            lblAngleRangeSeparator.Name = "lblAngleRangeSeparator";
            lblAngleRangeSeparator.Size = new Size(23, 19);
            lblAngleRangeSeparator.TabIndex = 35;
            lblAngleRangeSeparator.Text = "¢¦";
            // 
            // lblThrottleRangeSeparator
            // 
            lblThrottleRangeSeparator.AutoSize = true;
            lblThrottleRangeSeparator.Font = new Font("¸¼Àº °íµñ", 10F);
            lblThrottleRangeSeparator.Location = new Point(497, 75);
            lblThrottleRangeSeparator.Margin = new Padding(2, 0, 2, 0);
            lblThrottleRangeSeparator.Name = "lblThrottleRangeSeparator";
            lblThrottleRangeSeparator.Size = new Size(23, 19);
            lblThrottleRangeSeparator.TabIndex = 34;
            lblThrottleRangeSeparator.Text = "¢¦";
            // 
            // txtThrottleMax
            // 
            txtThrottleMax.Font = new Font("¸¼Àº °íµñ", 10F);
            txtThrottleMax.Location = new Point(523, 73);
            txtThrottleMax.Margin = new Padding(2);
            txtThrottleMax.Name = "txtThrottleMax";
            txtThrottleMax.Size = new Size(102, 25);
            txtThrottleMax.TabIndex = 33;
            // 
            // txtAngleMax
            // 
            txtAngleMax.Font = new Font("¸¼Àº °íµñ", 10F);
            txtAngleMax.Location = new Point(523, 41);
            txtAngleMax.Margin = new Padding(2);
            txtAngleMax.Name = "txtAngleMax";
            txtAngleMax.Size = new Size(102, 25);
            txtAngleMax.TabIndex = 32;
            // 
            // txtThrottleMin
            // 
            txtThrottleMin.Font = new Font("¸¼Àº °íµñ", 10F);
            txtThrottleMin.Location = new Point(391, 73);
            txtThrottleMin.Margin = new Padding(2);
            txtThrottleMin.Name = "txtThrottleMin";
            txtThrottleMin.Size = new Size(102, 25);
            txtThrottleMin.TabIndex = 31;
            // 
            // txtAngleMin
            // 
            txtAngleMin.Font = new Font("¸¼Àº °íµñ", 10F);
            txtAngleMin.Location = new Point(391, 41);
            txtAngleMin.Margin = new Padding(2);
            txtAngleMin.Name = "txtAngleMin";
            txtAngleMin.Size = new Size(102, 25);
            txtAngleMin.TabIndex = 30;
            // 
            // lblThrottleRange
            // 
            lblThrottleRange.AutoSize = true;
            lblThrottleRange.Font = new Font("¸¼Àº °íµñ", 10F);
            lblThrottleRange.Location = new Point(259, 75);
            lblThrottleRange.Margin = new Padding(2, 0, 2, 0);
            lblThrottleRange.Name = "lblThrottleRange";
            lblThrottleRange.Size = new Size(140, 19);
            lblThrottleRange.TabIndex = 26;
            lblThrottleRange.Text = "½º·ÎÆ²°ª ¹üÀ§(0~1): ";
            // 
            // lblAngleRange
            // 
            lblAngleRange.AutoSize = true;
            lblAngleRange.Font = new Font("¸¼Àº °íµñ", 10F);
            lblAngleRange.Location = new Point(259, 44);
            lblAngleRange.Margin = new Padding(2, 0, 2, 0);
            lblAngleRange.Name = "lblAngleRange";
            lblAngleRange.Size = new Size(137, 19);
            lblAngleRange.TabIndex = 25;
            lblAngleRange.Text = "Á¶Çâ°¢ ¹üÀ§ (-1~1): ";
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnApplyFilter.Location = new Point(247, 112);
            btnApplyFilter.Margin = new Padding(2);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(116, 22);
            btnApplyFilter.TabIndex = 12;
            btnApplyFilter.Text = "ÇÊÅÍ Àû¿ë";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilter.Location = new Point(367, 112);
            btnClearFilter.Margin = new Padding(2);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(116, 22);
            btnClearFilter.TabIndex = 19;
            btnClearFilter.Text = "ÇÊÅÍ ÇØÁ¦";
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
            grpTubExplorer.Margin = new Padding(2);
            grpTubExplorer.Name = "grpTubExplorer";
            grpTubExplorer.Padding = new Padding(2);
            grpTubExplorer.Size = new Size(1220, 476);
            grpTubExplorer.TabIndex = 22;
            grpTubExplorer.TabStop = false;
            grpTubExplorer.Text = "ÅÍºê Å½»ö±â";
            // 
            // trkFramePosition
            // 
            trkFramePosition.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trkFramePosition.Location = new Point(-1, 428);
            trkFramePosition.Margin = new Padding(2);
            trkFramePosition.Maximum = 0;
            trkFramePosition.Name = "trkFramePosition";
            trkFramePosition.Size = new Size(1216, 45);
            trkFramePosition.TabIndex = 18;
            trkFramePosition.Scroll += trkFramePosition_Scroll;
            // 
            // numPlaybackInterval
            // 
            numPlaybackInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numPlaybackInterval.Location = new Point(1091, 247);
            numPlaybackInterval.Margin = new Padding(2);
            numPlaybackInterval.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numPlaybackInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numPlaybackInterval.Name = "numPlaybackInterval";
            numPlaybackInterval.Size = new Size(121, 23);
            numPlaybackInterval.TabIndex = 11;
            numPlaybackInterval.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Location = new Point(0, 20);
            btnOpenFolder.Margin = new Padding(2);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(109, 22);
            btnOpenFolder.TabIndex = 0;
            btnOpenFolder.Text = "µ¥ÀÌÅÍ Æú´õ ¿­±â";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // lblPlayInterval
            // 
            lblPlayInterval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPlayInterval.Font = new Font("¸¼Àº °íµñ", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblPlayInterval.Location = new Point(999, 246);
            lblPlayInterval.Margin = new Padding(2, 0, 2, 0);
            lblPlayInterval.Name = "lblPlayInterval";
            lblPlayInterval.Size = new Size(99, 23);
            lblPlayInterval.TabIndex = 21;
            lblPlayInterval.Text = "Àç»ý°£°Ý(ms)";
            lblPlayInterval.Click += lblPlayInterval_Click;
            // 
            // btnLast
            // 
            btnLast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            btnAutoPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAutoPlay.Location = new Point(1001, 132);
            btnAutoPlay.Margin = new Padding(2);
            btnAutoPlay.Name = "btnAutoPlay";
            btnAutoPlay.Size = new Size(210, 20);
            btnAutoPlay.TabIndex = 20;
            btnAutoPlay.Text = "ÀÚµ¿ Àç»ý";
            btnAutoPlay.UseVisualStyleBackColor = true;
            btnAutoPlay.Click += btnAutoPlay_Click;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            btnPrev.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrev.Location = new Point(999, 78);
            btnPrev.Margin = new Padding(2);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(104, 22);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "<";
            btnPrev.Click += btnPrev_Click;
            // 
            // lstFrameData
            // 
            lstFrameData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lstFrameData.Location = new Point(0, 46);
            lstFrameData.Margin = new Padding(2);
            lstFrameData.Name = "lstFrameData";
            lstFrameData.Size = new Size(281, 379);
            lstFrameData.TabIndex = 13;
            lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;
            // 
            // btnFirst
            // 
            btnFirst.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFirst.Location = new Point(999, 105);
            btnFirst.Margin = new Padding(2);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(104, 22);
            btnFirst.TabIndex = 14;
            btnFirst.Text = "<<";
            btnFirst.Click += btnFirst_Click;
            // 
            // picMainPreview
            // 
            picMainPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picMainPreview.BorderStyle = BorderStyle.FixedSingle;
            picMainPreview.Location = new Point(285, 46);
            picMainPreview.Margin = new Padding(2);
            picMainPreview.Name = "picMainPreview";
            picMainPreview.Size = new Size(710, 378);
            picMainPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            picMainPreview.TabIndex = 4;
            picMainPreview.TabStop = false;
            // 
            // btnReload
            // 
            btnReload.Location = new Point(114, 20);
            btnReload.Margin = new Padding(2);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(109, 22);
            btnReload.TabIndex = 1;
            btnReload.Text = "´Ù½Ã ºÒ·¯¿À±â";
            btnReload.UseVisualStyleBackColor = true;
            btnReload.Click += btnReload_Click;
            // 
            // lblFrameValue
            // 
            lblFrameValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblFrameValue.Font = new Font("¸¼Àº °íµñ", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
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
            lblModeValue.Font = new Font("¸¼Àº °íµñ", 13.8F);
            lblModeValue.Location = new Point(1001, 215);
            lblModeValue.Margin = new Padding(2, 0, 2, 0);
            lblModeValue.Name = "lblModeValue";
            lblModeValue.Size = new Size(212, 84);
            lblModeValue.TabIndex = 8;
            lblModeValue.Text = "¸ðµå: -";
            lblModeValue.Click += lblModeValue_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Location = new Point(227, 20);
            btnToggleTheme.Margin = new Padding(2);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(109, 22);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "Å×¸¶ ÀüÈ¯";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // lblThrottleValue
            // 
            lblThrottleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblThrottleValue.Font = new Font("¸¼Àº °íµñ", 13.8F);
            lblThrottleValue.Location = new Point(1001, 188);
            lblThrottleValue.Margin = new Padding(2, 0, 2, 0);
            lblThrottleValue.Name = "lblThrottleValue";
            lblThrottleValue.Size = new Size(212, 28);
            lblThrottleValue.TabIndex = 7;
            lblThrottleValue.Text = "½º·ÎÆ²°ª: 0.000";
            lblThrottleValue.Click += lblThrottleValue_Click;
            // 
            // btnGuide
            // 
            btnGuide.Location = new Point(341, 20);
            btnGuide.Margin = new Padding(2);
            btnGuide.Name = "btnGuide";
            btnGuide.Size = new Size(109, 22);
            btnGuide.TabIndex = 3;
            btnGuide.Text = "´Ü°èº° °¡ÀÌµå";
            btnGuide.UseVisualStyleBackColor = true;
            btnGuide.Click += btnGuide_Click;
            // 
            // lblAngleValue
            // 
            lblAngleValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAngleValue.Font = new Font("¸¼Àº °íµñ", 13.8F);
            lblAngleValue.Location = new Point(1001, 158);
            lblAngleValue.Margin = new Padding(2, 0, 2, 0);
            lblAngleValue.Name = "lblAngleValue";
            lblAngleValue.Size = new Size(212, 29);
            lblAngleValue.TabIndex = 6;
            lblAngleValue.Text = "Á¶Çâ°ª: 0.000";
            lblAngleValue.Click += lblAngleValue_Click;
            // 
            // tabPageTraining
            // 
            tabPageTraining.Controls.Add(statusStripTraining);
            tabPageTraining.Controls.Add(grpTrainingLog);
            tabPageTraining.Controls.Add(grpTrainingSettings);
            tabPageTraining.Location = new Point(4, 24);
            tabPageTraining.Margin = new Padding(2);
            tabPageTraining.Name = "tabPageTraining";
            tabPageTraining.Padding = new Padding(2);
            tabPageTraining.Size = new Size(1221, 662);
            tabPageTraining.TabIndex = 1;
            tabPageTraining.Text = "ÇÐ½À ½ÇÇà";
            tabPageTraining.UseVisualStyleBackColor = true;
            tabPageTraining.Click += tabPageTraining_Click;
            // 
            // statusStripTraining
            // 
            statusStripTraining.ImageScalingSize = new Size(20, 20);
            statusStripTraining.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelTraining });
            statusStripTraining.Location = new Point(2, 638);
            statusStripTraining.Name = "statusStripTraining";
            statusStripTraining.Padding = new Padding(1, 0, 11, 0);
            statusStripTraining.Size = new Size(1217, 22);
            statusStripTraining.TabIndex = 2;
            statusStripTraining.Text = "statusStripTraining";
            // 
            // toolStripStatusLabelTraining
            // 
            toolStripStatusLabelTraining.Name = "toolStripStatusLabelTraining";
            toolStripStatusLabelTraining.Size = new Size(82, 17);
            toolStripStatusLabelTraining.Text = "Tub °æ·Î: C:\\";
            toolStripStatusLabelTraining.Click += toolStripStatusLabelTraining_Click;
            // 
            // grpTrainingLog
            // 
            grpTrainingLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTrainingLog.Controls.Add(rtbTrainingLog);
            grpTrainingLog.Location = new Point(12, 346);
            grpTrainingLog.Margin = new Padding(8);
            grpTrainingLog.Name = "grpTrainingLog";
            grpTrainingLog.Padding = new Padding(2);
            grpTrainingLog.Size = new Size(1203, 292);
            grpTrainingLog.TabIndex = 1;
            grpTrainingLog.TabStop = false;
            grpTrainingLog.Text = "ÇÐ½À ·Î±×";
            // 
            // rtbTrainingLog
            // 
            rtbTrainingLog.Dock = DockStyle.Fill;
            rtbTrainingLog.Location = new Point(2, 18);
            rtbTrainingLog.Margin = new Padding(2);
            rtbTrainingLog.Name = "rtbTrainingLog";
            rtbTrainingLog.Size = new Size(1199, 272);
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
            grpTrainingSettings.Location = new Point(12, 14);
            grpTrainingSettings.Margin = new Padding(2);
            grpTrainingSettings.Name = "grpTrainingSettings";
            grpTrainingSettings.Padding = new Padding(2);
            grpTrainingSettings.Size = new Size(1203, 328);
            grpTrainingSettings.TabIndex = 0;
            grpTrainingSettings.TabStop = false;
            grpTrainingSettings.Text = "ÇÐ½À ¼³Á¤";
            // 
            // nudEpoch
            // 
            nudEpoch.Font = new Font("¸¼Àº °íµñ", 12F);
            nudEpoch.Location = new Point(170, 230);
            nudEpoch.Margin = new Padding(2);
            nudEpoch.Name = "nudEpoch";
            nudEpoch.Size = new Size(304, 29);
            nudEpoch.TabIndex = 17;
            // 
            // cbxModelType
            // 
            cbxModelType.Font = new Font("¸¼Àº °íµñ", 12F);
            cbxModelType.FormattingEnabled = true;
            cbxModelType.Location = new Point(170, 188);
            cbxModelType.Margin = new Padding(2);
            cbxModelType.Name = "cbxModelType";
            cbxModelType.Size = new Size(305, 29);
            cbxModelType.TabIndex = 2;
            // 
            // tbxTubPath
            // 
            tbxTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbxTubPath.Font = new Font("¸¼Àº °íµñ", 12F);
            tbxTubPath.Location = new Point(170, 111);
            tbxTubPath.Margin = new Padding(2);
            tbxTubPath.Name = "tbxTubPath";
            tbxTubPath.Size = new Size(850, 29);
            tbxTubPath.TabIndex = 16;
            // 
            // tbxModelPath
            // 
            tbxModelPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbxModelPath.Font = new Font("¸¼Àº °íµñ", 12F);
            tbxModelPath.Location = new Point(170, 149);
            tbxModelPath.Margin = new Padding(2);
            tbxModelPath.Name = "tbxModelPath";
            tbxModelPath.Size = new Size(850, 29);
            tbxModelPath.TabIndex = 14;
            // 
            // tbxMycarPath
            // 
            tbxMycarPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbxMycarPath.Font = new Font("¸¼Àº °íµñ", 12F);
            tbxMycarPath.Location = new Point(170, 71);
            tbxMycarPath.Margin = new Padding(2);
            tbxMycarPath.Name = "tbxMycarPath";
            tbxMycarPath.Size = new Size(850, 29);
            tbxMycarPath.TabIndex = 13;
            // 
            // tbxPythonPath
            // 
            tbxPythonPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbxPythonPath.Font = new Font("¸¼Àº °íµñ", 12F);
            tbxPythonPath.Location = new Point(170, 32);
            tbxPythonPath.Margin = new Padding(2);
            tbxPythonPath.Name = "tbxPythonPath";
            tbxPythonPath.Size = new Size(850, 29);
            tbxPythonPath.TabIndex = 12;
            // 
            // lblEpoch
            // 
            lblEpoch.AutoSize = true;
            lblEpoch.Font = new Font("¸¼Àº °íµñ", 12F);
            lblEpoch.Location = new Point(35, 231);
            lblEpoch.Margin = new Padding(2, 0, 2, 0);
            lblEpoch.Name = "lblEpoch";
            lblEpoch.Size = new Size(80, 21);
            lblEpoch.TabIndex = 11;
            lblEpoch.Text = "ÇÐ½À È½¼ö";
            // 
            // lblModeType
            // 
            lblModeType.AutoSize = true;
            lblModeType.Font = new Font("¸¼Àº °íµñ", 12F);
            lblModeType.Location = new Point(35, 190);
            lblModeType.Margin = new Padding(2, 0, 2, 0);
            lblModeType.Name = "lblModeType";
            lblModeType.Size = new Size(80, 21);
            lblModeType.TabIndex = 10;
            lblModeType.Text = "¸ðµ¨ Á¾·ù";
            // 
            // lblModelPath
            // 
            lblModelPath.AutoSize = true;
            lblModelPath.Font = new Font("¸¼Àº °íµñ", 12F);
            lblModelPath.Location = new Point(35, 152);
            lblModelPath.Margin = new Padding(2, 0, 2, 0);
            lblModelPath.Name = "lblModelPath";
            lblModelPath.Size = new Size(118, 21);
            lblModelPath.TabIndex = 9;
            lblModelPath.Text = "¸ðµ¨ ÀúÀå °æ·Î";
            // 
            // lblTubPath
            // 
            lblTubPath.AutoSize = true;
            lblTubPath.Font = new Font("¸¼Àº °íµñ", 12F);
            lblTubPath.Location = new Point(35, 113);
            lblTubPath.Margin = new Padding(2, 0, 2, 0);
            lblTubPath.Name = "lblTubPath";
            lblTubPath.Size = new Size(76, 21);
            lblTubPath.TabIndex = 8;
            lblTubPath.Text = "Tub °æ·Î";
            // 
            // lblMycarPath
            // 
            lblMycarPath.AutoSize = true;
            lblMycarPath.Font = new Font("¸¼Àº °íµñ", 12F);
            lblMycarPath.Location = new Point(35, 74);
            lblMycarPath.Margin = new Padding(2, 0, 2, 0);
            lblMycarPath.Name = "lblMycarPath";
            lblMycarPath.Size = new Size(92, 21);
            lblMycarPath.TabIndex = 7;
            lblMycarPath.Text = "mycar °æ·Î";
            // 
            // lblPythonPath
            // 
            lblPythonPath.AutoSize = true;
            lblPythonPath.Font = new Font("¸¼Àº °íµñ", 12F);
            lblPythonPath.Location = new Point(35, 34);
            lblPythonPath.Margin = new Padding(2, 0, 2, 0);
            lblPythonPath.Name = "lblPythonPath";
            lblPythonPath.Size = new Size(96, 21);
            lblPythonPath.TabIndex = 6;
            lblPythonPath.Text = "ÆÄÀÌ½ã °æ·Î";
            // 
            // btnSaveSettings
            // 
            btnSaveSettings.Font = new Font("¸¼Àº °íµñ", 12F);
            btnSaveSettings.Location = new Point(782, 276);
            btnSaveSettings.Margin = new Padding(2);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(151, 32);
            btnSaveSettings.TabIndex = 5;
            btnSaveSettings.Text = "¼³Á¤ ÀúÀå";
            btnSaveSettings.UseVisualStyleBackColor = true;
            // 
            // btnStopTraining
            // 
            btnStopTraining.Font = new Font("¸¼Àº °íµñ", 12F);
            btnStopTraining.Location = new Point(523, 276);
            btnStopTraining.Margin = new Padding(2);
            btnStopTraining.Name = "btnStopTraining";
            btnStopTraining.Size = new Size(151, 32);
            btnStopTraining.TabIndex = 4;
            btnStopTraining.Text = "ÇÐ½À ÁßÁö";
            btnStopTraining.UseVisualStyleBackColor = true;
            // 
            // btnStartTraining
            // 
            btnStartTraining.Font = new Font("¸¼Àº °íµñ", 12F);
            btnStartTraining.Location = new Point(262, 276);
            btnStartTraining.Margin = new Padding(2);
            btnStartTraining.Name = "btnStartTraining";
            btnStartTraining.Size = new Size(151, 32);
            btnStartTraining.TabIndex = 3;
            btnStartTraining.Text = "ÇÐ½À ½ÃÀÛ";
            btnStartTraining.UseVisualStyleBackColor = true;
            // 
            // btnTubPath
            // 
            btnTubPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTubPath.Font = new Font("¸¼Àº °íµñ", 12F);
            btnTubPath.Location = new Point(1038, 101);
            btnTubPath.Margin = new Padding(2);
            btnTubPath.Name = "btnTubPath";
            btnTubPath.Size = new Size(96, 32);
            btnTubPath.TabIndex = 2;
            btnTubPath.Text = "Tub ¼±ÅÃ";
            btnTubPath.UseVisualStyleBackColor = true;
            // 
            // btnMycarPath
            // 
            btnMycarPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMycarPath.Font = new Font("¸¼Àº °íµñ", 12F);
            btnMycarPath.Location = new Point(1038, 65);
            btnMycarPath.Margin = new Padding(2);
            btnMycarPath.Name = "btnMycarPath";
            btnMycarPath.Size = new Size(96, 32);
            btnMycarPath.TabIndex = 1;
            btnMycarPath.Text = "Ã£±â";
            btnMycarPath.UseVisualStyleBackColor = true;
            btnMycarPath.Click += btnMycarPath_Click;
            // 
            // btnPythonPath
            // 
            btnPythonPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPythonPath.Font = new Font("¸¼Àº °íµñ", 12F);
            btnPythonPath.Location = new Point(1038, 29);
            btnPythonPath.Margin = new Padding(2);
            btnPythonPath.Name = "btnPythonPath";
            btnPythonPath.Size = new Size(96, 32);
            btnPythonPath.TabIndex = 0;
            btnPythonPath.Text = "Ã£±â";
            btnPythonPath.UseVisualStyleBackColor = true;
            // 
            // tabPageGraphStats
            // 
            tabPageGraphStats.Controls.Add(pnlGraphStats);
            tabPageGraphStats.Controls.Add(lblDescription);
            tabPageGraphStats.Location = new Point(4, 24);
            tabPageGraphStats.Margin = new Padding(2);
            tabPageGraphStats.Name = "tabPageGraphStats";
            tabPageGraphStats.Padding = new Padding(2);
            tabPageGraphStats.Size = new Size(1221, 662);
            tabPageGraphStats.TabIndex = 2;
            tabPageGraphStats.Text = "±×·¡ÇÁ/Åë°è";
            tabPageGraphStats.UseVisualStyleBackColor = true;
            // 
            // pnlGraphStats
            // 
            pnlGraphStats.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlGraphStats.Location = new Point(57, 56);
            pnlGraphStats.Margin = new Padding(2);
            pnlGraphStats.Name = "pnlGraphStats";
            pnlGraphStats.Size = new Size(716, 306);
            pnlGraphStats.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(57, 20);
            lblDescription.Margin = new Padding(2, 0, 2, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(739, 15);
            lblDescription.TabIndex = 0;
            lblDescription.Text = "Á¶Çâ°ª°ú ½º·ÎÆ²°ª º¯È­ ÃßÀÌ¸¦ ½Ã°¢È­ÇÕ´Ï´Ù. ÆÄ¶õ»öÀº Á¶Çâ, ÃÊ·Ï»öÀº ½º·ÎÆ²ÀÔ´Ï´Ù. ÇÊÅÍ Àû¿ë ÈÄ º¸ÀÌ´Â µ¥ÀÌÅÍ ±âÁØÀ¸·Î °»½ÅµË´Ï´Ù.";
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
            toolStripStatusLabelPath.Size = new Size(43, 17);
            toolStripStatusLabelPath.Text = "°æ·Î: -";
            // 
            // toolStripStatusLabelFrames
            // 
            toolStripStatusLabelFrames.Name = "toolStripStatusLabelFrames";
            toolStripStatusLabelFrames.Size = new Size(59, 17);
            toolStripStatusLabelFrames.Text = "Frames: 0";
            // 
            // txtSelectRangeMin
            // 
            txtSelectRangeMin.Location = new Point(391, 13);
            txtSelectRangeMin.Name = "txtSelectRangeMin";
            txtSelectRangeMin.Size = new Size(102, 23);
            txtSelectRangeMin.TabIndex = 45;
            // 
            // txtSelectRangeMax
            // 
            txtSelectRangeMax.Location = new Point(523, 13);
            txtSelectRangeMax.Name = "txtSelectRangeMax";
            txtSelectRangeMax.Size = new Size(102, 23);
            txtSelectRangeMax.TabIndex = 46;
            // 
            // lblSelectRangeSeparator
            // 
            lblSelectRangeSeparator.AutoSize = true;
            lblSelectRangeSeparator.Font = new Font("¸¼Àº °íµñ", 10F);
            lblSelectRangeSeparator.Location = new Point(497, 14);
            lblSelectRangeSeparator.Margin = new Padding(2, 0, 2, 0);
            lblSelectRangeSeparator.Name = "lblSelectRangeSeparator";
            lblSelectRangeSeparator.Size = new Size(23, 19);
            lblSelectRangeSeparator.TabIndex = 47;
            lblSelectRangeSeparator.Text = "¢¦";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1230, 715);
            Controls.Add(tabControlMain);
            Controls.Add(menuStripMain);
            MainMenuStrip = menuStripMain;
            MinimumSize = new Size(781, 535);
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
