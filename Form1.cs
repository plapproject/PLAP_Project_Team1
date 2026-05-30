using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottPlot;
using ScottPlot.WinForms;

namespace TeamApp
{
    public partial class Form1 : Form
    {
        // _allFrames : 로드된 모든 프레임입니다. 삭제된 항목도 포함하며 원본 순서를 유지합니다.
        // _visibleFrames : 현재 데이터 표에 표시 중인 목록입니다.
        //   - 필터 해제 시 _allFrames 전체를 표시합니다.
        //   - 필터 적용 시 조건에 맞는 항목만 표시합니다.
        private List<FrameData> _allFrames = new List<FrameData>();
        private List<FrameData> _visibleFrames = new List<FrameData>();

        private int _currentFrameIndex = -1;
        private System.Windows.Forms.Timer _playbackTimer;
        private bool _isPlaybackRunning = false;
        private bool _isDarkThemeEnabled = false;

        private string _currentDataFolderPath = "";

        private FormsPlot? _frameChart;
        private bool _isChartDirty = true;
        private bool _tutorialRunning = false;
        private bool _isFrameFilterActive = false;
        private bool _isFrameSelectionUpdating = false;
        private Process? _trainingProcess;
        private bool _hasUnsavedCleanupChanges = false;
        private bool _showDriveOverlay = true;
        private GroupBox? _grpFrameInfo;
        private FlowLayoutPanel? _thumbnailStrip;
        private System.Windows.Forms.Label? _lblFrameImageName;
        private System.Windows.Forms.Label? _lblFrameReviewHint;
        private System.Windows.Forms.Label? _lblThumbnailStripTitle;
        private ToolTip? _screenTip;
        private const string DeletedFramesMetaFileName = "deleted_frames_meta.txt";
        private const string TrainingSettingsFileName = "training_settings.json";

        private sealed class TutorialStep
        {
            public TutorialStep(string section, string title, string description, Control? target, TabPage? tabPage)
            {
                Section = section;
                Title = title;
                Description = description;
                Target = target;
                TabPage = tabPage;
            }

            public string Section { get; }
            public string Title { get; }
            public string Description { get; }
            public Control? Target { get; }
            public TabPage? TabPage { get; }
        }
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            _playbackTimer = new System.Windows.Forms.Timer();
            _playbackTimer.Tick += PlayTimer_Tick;

            if (statusStripDataViewer != null && !this.Controls.Contains(statusStripDataViewer))
            {
                statusStripDataViewer.Dock = DockStyle.Bottom;
                this.Controls.Add(statusStripDataViewer);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 버튼 이벤트를 코드에서 연결합니다.
            btnClearFrameFilter.Click      += BtnClearFrameFilter_Click;
            btnExcludeFrameRange.Click     += BtnExcludeFrameRange_Click;
            btnExcludeSelectedFrames.Click += BtnExcludeSelectedFrames_Click;
            btnExportCleanDataset.Click      += BtnExportCleanDataset_Click;
            btnRestoreFrames.Click     += BtnRestoreFrames_Click;
            btnSaveCleanupState.Click        += BtnSaveCleanupState_Click;
            btnStartTrainingProcess.Click    += BtnStartTrainingProcess_Click;
            btnStopTrainingProcess.Click     += BtnStopTrainingProcess_Click;
            btnSaveTrainingConfig.Click     += BtnSaveTrainingConfig_Click;
            btnSelectTrainingTubPath.Click          += (_, _) => SelectFolderInto(txtTrainingTubPath, "Tub 폴더 선택");

            mnuFileOpenDataFolder.Click   += (s, _) => BtnOpenDataFolder_Click(s!, EventArgs.Empty);
            mnuFileReloadData.Click       += (s, _) => BtnReloadData_Click(s!, EventArgs.Empty);
            mnuExit.Click             += (s, _) => Application.Exit();
            mnuHelpOpenTutorial.Visible = false;
            mnuHelpOpenTutorial.Enabled = false;

            tabControlMain.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            FormClosing += Form1_FormClosing;

            ConfigureFrameCatalogGrid();
            ConfigureDataExplorerLayout();
            ApplyDataManagerUiStyle();

            btnExcludeSelectedFrames.Text = "선택 프레임 제외";
            btnExportCleanDataset.Text          = "클린 폴더 추출";
            btnRestoreFrames.Text         = "복원";
            txtAngleMinFilter.Text    = "-1";
            txtAngleMaxFilter.Text    = "1";
            txtThrottleMinFilter.Text = "-1";
            txtThrottleMaxFilter.Text = "1";

            _screenTip = new ToolTip();
            _screenTip.SetToolTip(btnOpenDataFolder, "catalog_*.catalog가 있는 tub 폴더를 선택합니다.");
            _screenTip.SetToolTip(btnShowReviewCandidates, "급조향, 정지, 차선 이탈 등 자동 검토 후보만 표시합니다.");
            _screenTip.SetToolTip(btnApplyFrameFilter, "조향각/스로틀/모드/시나리오 조건으로 프레임을 좁힙니다.");
            _screenTip.SetToolTip(btnExcludeFrameRange, "원본 프레임 번호 기준으로 구간을 제외합니다.");
            _screenTip.SetToolTip(btnSaveCleanupState, "제외/복원 상태를 deleted_frames_meta.txt로 저장합니다.");

            InitializeTrainingControls();
            LoadTrainingSettings();
            UpdateStatusLabels();
            BeginInvoke(new Action(AskFirstUseTutorial));
        }

        // UI 이벤트 처리

        private void BtnOpenDataFolder_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            _ = LoadCatalogAsync(dlg.SelectedPath);
        }

        private void BtnReloadData_Click(object sender, EventArgs e)
        {
            string path = _currentDataFolderPath;
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(stsDataPath.Text) ||
                    stsDataPath.Text == "경로: -") return;
                path = stsDataPath.Text.Replace("경로: ", "").Trim();
            }
            if (Directory.Exists(path)) _ = LoadCatalogAsync(path);
        }

        private void btnToggleTheme_Click(object sender, EventArgs e) => ToggleTheme();

        private void btnGuide_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "상단 메뉴의 '튜토리얼'을 누르면 핵심 작업 흐름을 바로 다시 볼 수 있습니다.\n\n" +
                "추천 순서: 데이터 폴더 열기 -> 이상 후보 보기 -> 구간 제외 -> 상태 저장 -> 클린 폴더 추출",
                "단계별 가이드", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AskFirstUseTutorial()
        {
            if (HasSeenFirstUseTutorial()) return;

            var answer = MessageBox.Show(
                "처음 사용하시나요?\n\n6단계 핵심 튜토리얼을 시작할까요?\n(튜토리얼 중 X를 누르면 스킵됩니다.)",
                "첫 사용자 안내",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            MarkFirstUseTutorialSeen();

            if (answer == DialogResult.Yes)
            {
                RunFeatureTutorial("튜토리얼");
                return;
            }

            MessageBox.Show(
                "튜토리얼을 다시 보고 싶으면 상단 '튜토리얼' 메뉴를 눌러주세요.",
                "튜토리얼 안내",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private bool HasSeenFirstUseTutorial()
        {
            try
            {
                return File.Exists(GetTutorialSeenPath());
            }
            catch
            {
                return false;
            }
        }

        private void MarkFirstUseTutorialSeen()
        {
            try
            {
                Directory.CreateDirectory(Application.UserAppDataPath);
                File.WriteAllText(GetTutorialSeenPath(), DateTime.Now.ToString("O"));
            }
            catch
            {
                // 설정 저장 실패는 튜토리얼 실행 자체를 막지 않습니다.
            }
        }

        private string GetTutorialSeenPath()
        {
            return Path.Combine(Application.UserAppDataPath, "tutorial_seen.txt");
        }
        private void RunFeatureTutorial(string title)
        {
            if (_tutorialRunning)
            {
                MessageBox.Show("튜토리얼이 이미 진행 중입니다.", "튜토리얼", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _tutorialRunning = true;
            try
            {
                var steps = CreateTutorialSteps();
                foreach (var step in steps)
                {
                    if (!ShowTutorialStep(title, step)) break;
                }
            }
            finally
            {
                _tutorialRunning = false;
            }
        }

        private List<TutorialStep> CreateTutorialSteps()
        {
            return new List<TutorialStep>
            {
                new TutorialStep("기본", "1. 데이터 폴더 열기", "DonkeyCar tub 폴더를 선택합니다. catalog_*.catalog와 이미지를 함께 읽습니다.\n\n폴더 루트 또는 images 하위 폴더에 .jpg가 있어야 합니다.", btnOpenDataFolder, tabPageDataViewer),
                new TutorialStep("기본", "2. 프레임 표 읽기", "표의 조향각은 왼쪽(-) / 직진(0) / 오른쪽(+)입니다.\n스로틀은 후진(-) / 정지(0) / 전진(+)입니다. 모드는 user/local, 시나리오는 주행 상황입니다.", dgvFrameCatalog, tabPageDataViewer),
                new TutorialStep("기본", "3. 이상 후보 먼저 보기", "급조향, 정지에 가까운 스로틀, out_of_bound 같은 후보는 자동으로 표시됩니다.\n후보 행은 노란색으로 표시되니 먼저 확인하세요.", dgvFrameCatalog, tabPageDataViewer),
                new TutorialStep("기본", "4. 필터와 구간 제외", "필터는 의심 프레임을 좁히는 용도입니다.\n구간 제외/복원은 항상 원본 프레임 번호 기준입니다. 필터 적용 중이어도 기준은 원본입니다.", btnApplyFrameFilter, tabPageDataViewer),
                new TutorialStep("기본", "5. 상태 저장", "제외/복원 후에는 상태 저장을 눌러 deleted_frames_meta.txt에 기록합니다.\n저장하지 않고 종료하면 경고가 뜹니다.", btnSaveCleanupState, tabPageDataViewer),
                new TutorialStep("기본", "6. 클린 폴더 추출", "원본은 그대로 두고 _Clean 폴더를 새로 만듭니다.\n추출 전 전체/제외/유효 프레임 수를 확인한 뒤 진행하세요.", btnExportCleanDataset, tabPageDataViewer)
            };
        }

        private bool ShowTutorialStep(string tutorialTitle, TutorialStep step)
        {
            if (step.TabPage != null)
                tabControlMain.SelectedTab = step.TabPage;

            step.Target?.BringToFront();
            return HighlightControlDuringDialog(step.Target, step.Title, step.Description, tutorialTitle);
        }

        private bool HighlightControlDuringDialog(Control? target, string stepTitle, string description, string tutorialTitle)
        {
            if (target == null)
                return ShowTutorialDialog(tutorialTitle, stepTitle, description);

            var originalBackColor = target.BackColor;
            var originalForeColor = target.ForeColor;
            bool? originalUseVisualStyleBackColor = null;
            if (target is Button button)
            {
                originalUseVisualStyleBackColor = button.UseVisualStyleBackColor;
                button.UseVisualStyleBackColor = false;
            }

            bool isBlack = false;
            using var blinkTimer = new System.Windows.Forms.Timer { Interval = 260 };
            blinkTimer.Tick += (_, _) =>
            {
                isBlack = !isBlack;
                target.BackColor = isBlack ? System.Drawing.Color.Black : System.Drawing.Color.White;
                target.ForeColor = isBlack ? System.Drawing.Color.White : System.Drawing.Color.Black;
                target.Refresh();
            };

            try
            {
                target.Focus();
                blinkTimer.Start();
                return ShowTutorialDialog(tutorialTitle, stepTitle, description);
            }
            finally
            {
                blinkTimer.Stop();
                target.BackColor = originalBackColor;
                target.ForeColor = originalForeColor;
                if (target is Button restoreButton && originalUseVisualStyleBackColor.HasValue)
                    restoreButton.UseVisualStyleBackColor = originalUseVisualStyleBackColor.Value;
                target.Refresh();
            }
        }

        private bool ShowTutorialDialog(string tutorialTitle, string stepTitle, string description)
        {
            using var dialog = new Form
            {
                Text = tutorialTitle + " - " + stepTitle,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(520, 210)
            };

            var descriptionLabel = new System.Windows.Forms.Label
            {
                AutoSize = false,
                Location = new Point(18, 18),
                Size = new Size(484, 120),
                Text = description + "\n\n다음 버튼을 누르면 다음 기능으로 넘어갑니다. X 또는 스킵을 누르면 튜토리얼을 종료합니다.",
                Font = new Font("맑은 고딕", 10F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var nextButton = new Button
            {
                Text = "다음",
                DialogResult = DialogResult.OK,
                Location = new Point(308, 158),
                Size = new Size(92, 32)
            };

            var skipButton = new Button
            {
                Text = "스킵",
                DialogResult = DialogResult.Cancel,
                Location = new Point(410, 158),
                Size = new Size(92, 32)
            };

            dialog.Controls.Add(descriptionLabel);
            dialog.Controls.Add(nextButton);
            dialog.Controls.Add(skipButton);
            dialog.AcceptButton = nextButton;
            dialog.CancelButton = skipButton;

            return dialog.ShowDialog(this) == DialogResult.OK;
        }

        private void BtnApplyFrameFilter_Click(object sender, EventArgs e) => ApplyFrameFilter();

        private void BtnClearFrameFilter_Click(object? sender, EventArgs e) => ClearFrameFilter();

        private void BtnShowReviewCandidates_Click(object? sender, EventArgs e)
        {
            if (_allFrames == null || _allFrames.Count == 0)
            {
                MessageBox.Show("먼저 데이터 폴더를 열어 주세요.",
                    "데이터 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _visibleFrames = _allFrames
                .Where(frame => !frame.IsDeleted && frame.NeedsReview)
                .ToList();
            _isFrameFilterActive = true;
            _isChartDirty = true;
            RefreshFrameView();

            if (_visibleFrames.Count > 0)
            {
                SetIndex(0);
                MessageBox.Show(
                    $"검토 후보 {_visibleFrames.Count}개만 표시합니다.\n\n" +
                    "노란색 행을 확인하고, 문제가 맞으면 선택 프레임 제외 또는 구간 제외를 사용하세요.",
                    "이상 후보 보기", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ClearPreviewSelection();
                MessageBox.Show("현재 기준으로 자동 검토 후보가 없습니다.",
                    "이상 후보 없음", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 선택한 구간을 Soft Delete 처리합니다.
        private void BtnExcludeFrameRange_Click(object? sender, EventArgs e)
        {
            if (!TryReadSelectedRange(out int from, out int to))
            {
                var fallbackConfirm = MessageBox.Show(
                    "구간 정보가 없습니다.\n\n구간 제외는 원본 프레임 번호 기준입니다.\n현재 선택된 프레임을 대신 제외할까요?",
                    "구간 정보 없음", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (fallbackConfirm == DialogResult.Yes)
                    BtnExcludeSelectedFrames_Click(sender, e);
                return;
            }

            var confirm = MessageBox.Show(
                $"원본 프레임 번호 {from} ~ {to} 구간을 제외하시겠습니까?\n\n" +
                "필터 적용 중이어도 구간 기준은 항상 원본 프레임 번호입니다.",
                "구간 제외 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            SetDeletedByRange(from, to, true);
            FinishDataStateChange();
        }

        private void BtnExcludeSelectedFrames_Click(object? sender, EventArgs e)
        {
            var selectedFrames = GetSelectedFrames();
            if (selectedFrames.Count == 0)
            {
                MessageBox.Show("제외할 프레임을 하나 이상 선택해 주세요.",
                    "선택 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"선택한 {selectedFrames.Count}개의 프레임을 제외하시겠습니까?",
                "선택 프레임 제외", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            foreach (var frame in selectedFrames)
                frame.IsDeleted = true;

            MarkCleanupStateChanged();
            FinishDataStateChange();
        }

        private void BtnRestoreFrames_Click(object? sender, EventArgs e)
        {
            if (TryReadSelectedRange(out int from, out int to))
            {
                var rangeConfirm = MessageBox.Show(
                    "해당 구간을 복원합니다.",
                    "구간 복원", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rangeConfirm != DialogResult.Yes) return;

                SetDeletedByRange(from, to, false);
                FinishDataStateChange();
                return;
            }

            var selectedFrames = GetSelectedFrames();
            var selectedConfirm = MessageBox.Show(
                $"지정된 구간이 없습니다. 선택한 {selectedFrames.Count}개의 프레임을 복원할까요?",
                "선택 프레임 복원", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (selectedConfirm != DialogResult.Yes) return;

            foreach (var frame in selectedFrames)
                frame.IsDeleted = false;

            MarkCleanupStateChanged();
            FinishDataStateChange();
        }

        private void btnAutoPlay_Click(object sender, EventArgs e) => TogglePlayPause();

        private void DgvFrameCatalog_SelectionChanged(object? sender, EventArgs e)
        {
            if (_isFrameSelectionUpdating) return;

            var selectedRow = dgvFrameCatalog.CurrentRow;
            if (selectedRow?.DataBoundItem is not FrameData selectedFrame) return;

            int idx = _visibleFrames.IndexOf(selectedFrame);
            if (idx >= 0 && idx < _visibleFrames.Count)
                SetIndex(idx);
        }

        private void TrkFrameTimeline_Scroll(object sender, EventArgs e)
        {
            int idx = trkFrameTimeline.Value;
            if (idx >= 0 && idx < _visibleFrames.Count)
                SetIndex(idx);
        }

        private void btnFirst_Click(object sender, EventArgs e) => SetIndex(0);
        private void btnPrev_Click(object sender, EventArgs e)  => SetIndex(Math.Max(0, _currentFrameIndex - 1));
        private void btnNext_Click(object sender, EventArgs e)  => SetIndex(Math.Min(_visibleFrames.Count - 1, _currentFrameIndex + 1));
        private void btnLast_Click(object sender, EventArgs e)  => SetIndex(_visibleFrames.Count - 1);

        // 자동 재생 타이머 처리
        private void PlayTimer_Tick(object? sender, EventArgs e)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            int next = _currentFrameIndex + 1;
            // 제외된 프레임은 건너뜁니다.
            while (next < _visibleFrames.Count && _visibleFrames[next].IsDeleted)
                next++;
            if (next >= _visibleFrames.Count)
            {
                _playbackTimer.Stop();
                _isPlaybackRunning = false;
                btnAutoPlay.Text = "자동 재생";
                return;
            }
            SetIndex(next);
        }

        // 키보드 단축키 처리
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right: btnNext_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.Left:  btnPrev_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.Space: TogglePlayPause(); e.Handled = true; break;
                case Keys.Home:  btnFirst_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.End:   btnLast_Click(this, EventArgs.Empty); e.Handled = true; break;
            }
        }

        /// <summary>
        /// 프레임 목록을 열 단위 표로 구성합니다.
        /// 긴 파일명과 조향/스로틀 값을 분리해서 보여 주면 데이터 검수 속도가 빨라집니다.
        /// </summary>
        private void ConfigureFrameCatalogGrid()
        {
            dgvFrameCatalog.AutoGenerateColumns = false;
            dgvFrameCatalog.Columns.Clear();
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.FormattedIndex),
                HeaderText = "번호",
                Width = 70,
                Frozen = true
            });
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.Name),
                HeaderText = "이미지명",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 220
            });

            dgvFrameCatalog.RowTemplate.Height = 26;
            dgvFrameCatalog.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            dgvFrameCatalog.DataBindingComplete += (_, _) => ApplyFrameCatalogRowStyle();
        }

        private void ConfigureDataExplorerLayout()
        {
            // 기존 우측 정보/재생 컨트롤을 그룹박스로 묶어 화면을 읽기 쉽게 만듭니다.
            _grpFrameInfo = new GroupBox
            {
                Text = "프레임 정보",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(1284, 61),
                Size = new Size(276, 420)
            };

            grpDataExplorer.Controls.Add(_grpFrameInfo);

            MoveToFrameInfoGroup(lblFrameValue, 12, 28, 252, 30);
            _lblFrameImageName = CreateFrameInfoLabel(12, 61, 252, 25, "이미지: -");
            MoveToFrameInfoGroup(lblAngleValue, 12, 93, 252, 28);
            MoveToFrameInfoGroup(lblThrottleValue, 12, 126, 252, 28);
            MoveToFrameInfoGroup(lblModeValue, 12, 159, 252, 28);
            _lblFrameReviewHint = CreateFrameInfoLabel(12, 192, 252, 44, "검토: -");
            MoveToFrameInfoGroup(btnPrev, 12, 248, 120, 29);
            MoveToFrameInfoGroup(btnNext, 144, 248, 120, 29);
            MoveToFrameInfoGroup(btnFirst, 12, 283, 120, 29);
            MoveToFrameInfoGroup(btnLast, 144, 283, 120, 29);
            MoveToFrameInfoGroup(btnAutoPlay, 12, 318, 252, 29);
            MoveToFrameInfoGroup(lblPlayInterval, 12, 356, 118, 25);
            MoveToFrameInfoGroup(numPlaybackIntervalMs, 132, 356, 132, 27);

            lblFrameValue.Font = new Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            lblAngleValue.Font = new Font("맑은 고딕", 10F);
            lblThrottleValue.Font = new Font("맑은 고딕", 10F);
            lblModeValue.Font = new Font("맑은 고딕", 10F);
            lblPlayInterval.Font = new Font("맑은 고딕", 9F);

            splitContainerFramePreview.Size = new Size(1278, 445);
            trkFrameTimeline.Location = new Point(-1, 571);

            _lblThumbnailStripTitle = new System.Windows.Forms.Label
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Location = new Point(4, 490),
                Size = new Size(260, 20),
                Text = "주변 프레임 미리보기",
                Font = new Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold)
            };
            grpDataExplorer.Controls.Add(_lblThumbnailStripTitle);

            _thumbnailStrip = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(0, 512),
                Size = new Size(1278, 54),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245),
                Padding = new Padding(4, 3, 4, 3)
            };
            grpDataExplorer.Controls.Add(_thumbnailStrip);
            _thumbnailStrip.BringToFront();
        }

        private System.Windows.Forms.Label CreateFrameInfoLabel(int x, int y, int width, int height, string text)
        {
            var label = new System.Windows.Forms.Label
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                Text = text,
                Font = new Font("맑은 고딕", 9F),
                AutoEllipsis = true
            };
            _grpFrameInfo?.Controls.Add(label);
            return label;
        }

        private void MoveToFrameInfoGroup(Control control, int x, int y, int width, int height)
        {
            control.Parent?.Controls.Remove(control);
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            control.Location = new Point(x, y);
            control.Size = new Size(width, height);
            _grpFrameInfo?.Controls.Add(control);
        }

        /// <summary>
        /// 데이터 정제 흐름이 눈에 띄도록 버튼명, 구간 입력 영역, 상태 색상을 정리합니다.
        /// </summary>
        private void ApplyDataManagerUiStyle()
        {
            grpDataCleaner.Text = "터브 정리기 - 필터 / 구간 제외 / 복원 / 클린 추출";
            lblFrameRange.Text = "구간 제외/복원";

            foreach (var button in new[]
            {
                btnApplyFrameFilter, btnShowReviewCandidates, btnClearFrameFilter, btnExcludeFrameRange, btnExcludeSelectedFrames,
                btnRestoreFrames, btnSaveCleanupState, btnExportCleanDataset
            })
            {
                button.UseVisualStyleBackColor = false;
            }

            btnApplyFrameFilter.BackColor = System.Drawing.Color.FromArgb(229, 241, 255);
            btnShowReviewCandidates.BackColor = System.Drawing.Color.FromArgb(255, 249, 219);
            btnClearFrameFilter.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            btnExcludeFrameRange.BackColor = System.Drawing.Color.FromArgb(255, 232, 214);
            btnExcludeSelectedFrames.BackColor = System.Drawing.Color.FromArgb(255, 244, 230);
            btnRestoreFrames.BackColor = System.Drawing.Color.FromArgb(224, 247, 235);
            btnSaveCleanupState.BackColor = System.Drawing.Color.FromArgb(235, 239, 255);
            btnExportCleanDataset.BackColor = System.Drawing.Color.FromArgb(255, 238, 238);

            txtFrameRangeStart.BackColor = System.Drawing.Color.FromArgb(255, 252, 235);
            txtFrameRangeEnd.BackColor = System.Drawing.Color.FromArgb(255, 252, 235);
            lblFrameRange.ForeColor = System.Drawing.Color.FromArgb(120, 70, 0);
        }

        /// <summary>
        /// 제외된 프레임을 회색/붉은색 계열로 표시해서 학습 제외 대상을 바로 구분합니다.
        /// </summary>
        private void ApplyFrameCatalogRowStyle()
        {
            foreach (DataGridViewRow row in dgvFrameCatalog.Rows)
            {
                if (row.DataBoundItem is not FrameData frame) continue;

                if (frame.IsDeleted)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(145, 52, 52);
                    row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(255, 204, 204);
                    row.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(90, 20, 20);
                }
                else if (frame.NeedsReview)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 249, 219);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(94, 67, 0);
                    row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(255, 219, 125);
                    row.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = row.Index % 2 == 0
                        ? System.Drawing.Color.White
                        : System.Drawing.Color.FromArgb(248, 250, 252);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(30, 30, 30);
                    row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(51, 122, 183);
                    row.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
                }
            }
        }

        // 공통 로직

        /// <summary>
        /// 로딩 중 버튼 비활성화와 커서 변경을 처리합니다.
        /// </summary>
        private void SetLoadingState(bool loading)
        {
            btnOpenDataFolder.Enabled           = !loading;
            btnReloadData.Enabled               = !loading;
            btnApplyFrameFilter.Enabled          = !loading;
            btnShowReviewCandidates.Enabled      = !loading;
            btnClearFrameFilter.Enabled          = !loading;
            btnExcludeSelectedFrames.Enabled = !loading;
            btnRestoreFrames.Enabled         = !loading;
            this.Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
            if (loading) stsFrameSummary.Text = "로딩 중...";
        }

        // 목록과 상태 표시를 갱신합니다.
        /// <summary>
        /// FormattedIndex를 다시 계산하고 프레임 표를 갱신합니다.
        /// IsDeleted == false : [0000], [0001], [0002] ...
        /// IsDeleted == true  : [XXXX]
        /// </summary>
        private void RefreshFrameView()
        {
            if (_allFrames == null)
                _allFrames = new List<FrameData>();

            if (!_isFrameFilterActive)
            {
                int validIndex = 0;
                foreach (var frame in _allFrames)
                {
                    frame.FormattedIndex = frame.IsDeleted
                        ? "[XXXX]"
                        : $"[{validIndex++:D4}]";
                }

                _visibleFrames = _allFrames;
            }
            else
            {
                foreach (var frame in _visibleFrames)
                {
                    int originalIndex = frame.OriginalIndex >= 0
                        ? frame.OriginalIndex
                        : _allFrames.IndexOf(frame);

                    frame.FormattedIndex = frame.IsDeleted
                        ? "[XXXX]"
                        : $"[{Math.Max(0, originalIndex):D4}]";
                }
            }

            dgvFrameCatalog.DataSource = null;
            dgvFrameCatalog.DataSource = _visibleFrames;
            ApplyFrameCatalogRowStyle();
            UpdateNavigationButtons();

            trkFrameTimeline.Minimum = 0;
            trkFrameTimeline.Maximum = Math.Max(0, _visibleFrames.Count - 1);

            UpdateStatusLabels();
            BeginInvoke(new Action(AskFirstUseTutorial));
        }
        /// <summary>
        /// 전체 원본 프레임을 다시 표시 상태로 복원합니다.
        /// RefreshFrameView를 호출해 화면 상태도 함께 갱신합니다.
        /// </summary>
        private void RefreshFrameBinding()
        {
            _isFrameFilterActive = false;
            _visibleFrames = _allFrames;
            RefreshFrameView();
        }

        private void SetIndex(int idx)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            idx = Math.Max(0, Math.Min(_visibleFrames.Count - 1, idx));
            _currentFrameIndex = idx;

            // 선택 변경 이벤트가 중복 실행되지 않도록 잠시 막습니다.
            _isFrameSelectionUpdating = true;
            dgvFrameCatalog.ClearSelection();
            if (idx >= 0 && idx < dgvFrameCatalog.Rows.Count)
            {
                dgvFrameCatalog.Rows[idx].Selected = true;
                dgvFrameCatalog.CurrentCell = dgvFrameCatalog.Rows[idx].Cells[0];
                dgvFrameCatalog.FirstDisplayedScrollingRowIndex = Math.Max(0, idx);
            }
            _isFrameSelectionUpdating = false;

            if (trkFrameTimeline.Value != idx)
                trkFrameTimeline.Value = idx;

            var frame = _visibleFrames[idx];

            string resolvedPath = ResolveImagePath(frame.Name);
            UpdatePreviewImage(resolvedPath, frame);

            lblFrameValue.Text    = $"Frame: {idx + 1} / {_visibleFrames.Count}";
            lblAngleValue.Text    = $"조향: {frame.Angle:0.000} ({frame.SteeringText})";
            lblThrottleValue.Text = $"스로틀: {frame.Throttle:0.000} ({frame.ThrottleText})";
            lblModeValue.Text     = $"모드: {frame.ModeDescription}";
            if (_lblFrameImageName != null)
            {
                _lblFrameImageName.Text = $"이미지: {frame.Name}";
                _screenTip?.SetToolTip(_lblFrameImageName, frame.Name);
            }
            if (_lblFrameReviewHint != null)
            {
                _lblFrameReviewHint.Text = $"검토: {frame.ReviewHint}";
                _screenTip?.SetToolTip(_lblFrameReviewHint, frame.ReviewHint);
            }
            UpdateThumbnailStrip();
            UpdateNavigationButtons();
            UpdateStatusLabels();
            BeginInvoke(new Action(AskFirstUseTutorial));
        }

        private void UpdateNavigationButtons()
        {
            int count = _visibleFrames?.Count ?? 0;
            bool hasFrames = count > 0 && _currentFrameIndex >= 0;
            btnFirst.Enabled = hasFrames && _currentFrameIndex > 0;
            btnPrev.Enabled = hasFrames && _currentFrameIndex > 0;
            btnNext.Enabled = hasFrames && _currentFrameIndex < count - 1;
            btnLast.Enabled = hasFrames && _currentFrameIndex < count - 1;
            btnAutoPlay.Enabled = hasFrames && count > 1;
            trkFrameTimeline.Enabled = hasFrames;
            numPlaybackIntervalMs.Enabled = hasFrames;
        }
        /// <summary>
        /// 현재 폴더와 파일명으로 실제 이미지 경로를 찾습니다.
        /// 폴더 루트와 하위 images 폴더를 모두 확인합니다.
        /// </summary>
        private string ResolveImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(_currentDataFolderPath))
                return string.Empty;

            string p1 = Path.Combine(_currentDataFolderPath, fileName);
            if (File.Exists(p1)) return p1;

            string p2 = Path.Combine(_currentDataFolderPath, "images", fileName);
            if (File.Exists(p2)) return p2;

            return string.Empty;
        }

        private void UpdatePreviewImage(string path, FrameData? frame)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    picFramePreview.Image?.Dispose();
                    picFramePreview.Image = null;
                    return;
                }
                using var fs  = File.OpenRead(path);
                using var img = System.Drawing.Image.FromStream(fs);
                var bmp = new Bitmap(img);
                if (frame != null && _showDriveOverlay)
                    DrawDriveOverlay(bmp, frame);

                var old = picFramePreview.Image;
                picFramePreview.Image = bmp;
                old?.Dispose();
            }
            catch { /* 이미지 로드 실패는 미리보기만 비우고 무시합니다. */ }
        }

        private void DrawDriveOverlay(Bitmap bitmap, FrameData frame)
        {
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int centerX = bitmap.Width / 2;
            int baseY = Math.Max(30, bitmap.Height - Math.Max(55, bitmap.Height / 7));
            int length = Math.Max(25, Math.Min(bitmap.Width, bitmap.Height) / 10);
            int endX = centerX + (int)(frame.Angle * length);
            int endY = baseY - length;

            using var arrowPen = new Pen(System.Drawing.Color.FromArgb(210, 255, 193, 7), Math.Max(2, bitmap.Width / 300))
            {
                CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4, 5)
            };
            graphics.DrawLine(arrowPen, centerX, baseY, endX, endY);

            string overlayText = $"{frame.SteeringText} | {frame.ThrottleText}";
            if (frame.NeedsReview)
                overlayText += " | 검토";

            using var font = new Font("맑은 고딕", Math.Max(6, bitmap.Width / 180f), System.Drawing.FontStyle.Bold);
            SizeF textSize = graphics.MeasureString(overlayText, font);
            var textRect = new RectangleF(8, 8, textSize.Width + 8, textSize.Height + 4);
            using var backBrush = new SolidBrush(System.Drawing.Color.FromArgb(105, 0, 0, 0));
            using var textBrush = new SolidBrush(frame.NeedsReview ? System.Drawing.Color.Gold : System.Drawing.Color.White);
            graphics.FillRectangle(backBrush, textRect);
            graphics.DrawString(overlayText, font, textBrush, textRect.X + 4, textRect.Y + 2);
        }

        private void UpdateThumbnailStrip()
        {
            if (_thumbnailStrip == null || _visibleFrames == null || _visibleFrames.Count == 0) return;

            _thumbnailStrip.SuspendLayout();
            try
            {
                foreach (Control control in _thumbnailStrip.Controls)
                {
                    if (control is PictureBox pictureBox)
                        pictureBox.Image?.Dispose();
                    control.Dispose();
                }
                _thumbnailStrip.Controls.Clear();

                int start = Math.Max(0, _currentFrameIndex - 10);
                int end = Math.Min(_visibleFrames.Count - 1, start + 24);
                start = Math.Max(0, end - 24);

                for (int i = start; i <= end; i++)
                {
                    FrameData frame = _visibleFrames[i];
                    string imagePath = ResolveImagePath(frame.Name);
                    var thumbnailBox = new PictureBox
                    {
                        Size = new Size(74, 42),
                        Margin = new Padding(2),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderStyle = i == _currentFrameIndex ? BorderStyle.Fixed3D : BorderStyle.FixedSingle,
                        BackColor = frame.IsDeleted
                            ? System.Drawing.Color.FromArgb(235, 210, 210)
                            : frame.NeedsReview
                                ? System.Drawing.Color.FromArgb(255, 243, 186)
                                : System.Drawing.Color.White,
                        Tag = i
                    };

                    if (File.Exists(imagePath))
                    {
                        try
                        {
                            using var sourceImage = System.Drawing.Image.FromFile(imagePath);
                            thumbnailBox.Image = new Bitmap(sourceImage, thumbnailBox.Width, thumbnailBox.Height);
                        }
                        catch
                        {
                            thumbnailBox.Image = null;
                        }
                    }

                    thumbnailBox.Click += (_, _) =>
                    {
                        if (thumbnailBox.Tag is int frameIndex)
                            SetIndex(frameIndex);
                    };
                    _thumbnailStrip.Controls.Add(thumbnailBox);
                }
            }
            finally
            {
                _thumbnailStrip.ResumeLayout();
            }
        }

        private void TogglePlayPause()
        {
            if (_isPlaybackRunning)
            {
                _playbackTimer.Stop();
                _isPlaybackRunning = false;
                btnAutoPlay.Text = "자동 재생";
            }
            else
            {
                _playbackTimer.Interval = (int)numPlaybackIntervalMs.Value;
                _playbackTimer.Start();
                _isPlaybackRunning = true;
                btnAutoPlay.Text = "일시정지";
            }
        }

        private void ToggleTheme()
        {
            _isDarkThemeEnabled = !_isDarkThemeEnabled;
            System.Drawing.Color back = _isDarkThemeEnabled
                ? System.Drawing.Color.FromArgb(45, 45, 48)
                : SystemColors.Control;
            System.Drawing.Color fore = _isDarkThemeEnabled
                ? System.Drawing.Color.White
                : SystemColors.ControlText;
            this.BackColor = back;
            foreach (Control c in this.Controls)
                ApplyThemeToControl(c, back, fore);
            statusStripDataViewer.BackColor = back;
            statusStripDataViewer.ForeColor = fore;
        }

        private void ApplyThemeToControl(Control c, System.Drawing.Color back, System.Drawing.Color fore)
        {
            try
            {
                if (c is StatusStrip) return;
                c.BackColor = back;
                c.ForeColor = fore;
                foreach (Control child in c.Controls)
                    ApplyThemeToControl(child, back, fore);
            }
            catch { }
        }

        // 필터 적용
        /// <summary>
        /// Angle/Throttle 범위 필터를 적용합니다.
        /// 제외되지 않은 프레임만 화면에 표시합니다.
        /// </summary>
        private void ApplyFrameFilter()
        {
            if (_allFrames == null || _allFrames.Count == 0)
            {
                MessageBox.Show(
                    "필터를 적용할 데이터가 없습니다.\n먼저 '데이터 폴더 열기'로 DonkeyCar tub 폴더를 불러와 주세요.",
                    "데이터 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryReadFilterRanges(out double angleMin, out double angleMax,
                                     out double throttleMin, out double throttleMax))
                return;

            // 제외되지 않은 프레임 중 범위 조건에 맞는 항목만 남깁니다.
            _visibleFrames = _allFrames
                .Where(f => !f.IsDeleted &&
                            f.Angle    >= angleMin && f.Angle    <= angleMax &&
                            f.Throttle >= throttleMin && f.Throttle <= throttleMax &&
                            MatchesComboFilter(cmbModeFilter, f.Mode) &&
                            MatchesComboFilter(cmbScenarioFilter, f.Scenario))
                .ToList();

            _isFrameFilterActive = true;
            _isChartDirty = true;
            RefreshFrameView();
            if (_visibleFrames.Count > 0) SetIndex(0);
            else
            {
                ClearPreviewSelection();
                MessageBox.Show(
                    "필터 결과가 0개입니다.\n\n확인할 점:\n- 조향각/스로틀 최소값이 최대값보다 작은지\n- Mode/Scenario 조건이 실제 데이터에 있는 값인지\n- 필터 범위를 너무 좁게 잡지 않았는지",
                    "필터 결과 없음", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool TryReadFilterRanges(out double angleMin, out double angleMax,
                                          out double throttleMin, out double throttleMax)
        {
            angleMin = angleMax = throttleMin = throttleMax = 0;

            bool ok =
                double.TryParse(txtAngleMinFilter.Text.Trim(),   NumberStyles.Float, CultureInfo.InvariantCulture, out angleMin) &&
                double.TryParse(txtAngleMaxFilter.Text.Trim(),   NumberStyles.Float, CultureInfo.InvariantCulture, out angleMax) &&
                double.TryParse(txtThrottleMinFilter.Text.Trim(),NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMin) &&
                double.TryParse(txtThrottleMaxFilter.Text.Trim(),NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMax);

            if (!ok)
            {
                MessageBox.Show(
                    "조향각과 스로틀 범위는 숫자로 입력해 주세요.\n\n" +
                    "조향각: -1은 좌회전, 0은 직진, 1은 우회전입니다.\n" +
                    "스로틀: -1은 후진, 0은 정지, 1은 전진입니다.",
                    "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (angleMin > angleMax || throttleMin > throttleMax)
            {
                MessageBox.Show(
                    "범위의 최소값은 최대값보다 클 수 없습니다.\n예: 조향각 -1 ~ 1, 스로틀 -1 ~ 1",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool MatchesComboFilter(ComboBox comboBox, string value)
        {
            string selected = comboBox.SelectedItem?.ToString() ?? comboBox.Text;
            if (string.IsNullOrWhiteSpace(selected) ||
                selected.Equals("All", StringComparison.OrdinalIgnoreCase))
                return true;

            return string.Equals(
                NormalizeFilterText(value),
                NormalizeFilterText(selected),
                StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizeFilterText(string text)
        {
            return (text ?? string.Empty)
                .Trim()
                .Replace("-", "_")
                .Replace(" ", "_")
                .ToLowerInvariant();
        }

        // 필터 해제
        /// <summary>
        /// 필터를 해제하고 원본 프레임 목록을 다시 표시합니다.
        /// </summary>
        private void ClearFrameFilter()
        {
            if (_allFrames == null) return;
            // 원본 목록을 그대로 다시 표시합니다.
            _isFrameFilterActive = false;
            _visibleFrames = _allFrames;
            _isChartDirty = true;
            RefreshFrameView();
            if (_visibleFrames.Count > 0) SetIndex(0);
            else ClearPreviewSelection();
        }

        // Soft Delete 구간 처리
        private void SoftDeleteRange(int from, int to)
        {
            if (_allFrames == null || _allFrames.Count == 0) return;

            int safeFrom = Math.Max(0, from);
            int safeTo   = Math.Min(_allFrames.Count - 1, to);

            if (safeFrom > safeTo)
            {
                MessageBox.Show(
                    $"유효한 프레임 범위를 벗어났습니다.\n입력 범위: {from} ~ {to} | 전체 프레임: {_allFrames.Count}",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newlyDeleted = 0;
            for (int i = safeFrom; i <= safeTo; i++)
            {
                if (!_allFrames[i].IsDeleted)
                {
                    _allFrames[i].IsDeleted = true;
                    newlyDeleted++;
                }
            }

            // 삭제 상태를 반영하고 차트를 갱신합니다.
            _isChartDirty = true;
            RefreshFrameView();
            RenderFrameChart();

            int clamped = Math.Max(0, Math.Min(_currentFrameIndex, _visibleFrames.Count - 1));
            if (_visibleFrames.Count > 0) SetIndex(clamped);

            MessageBox.Show(
                $"완료: {newlyDeleted}개 프레임이 제외(Soft Delete) 처리되었습니다.\n" +
                "이미 제외된 상태였던 프레임은 건너뛰었습니다.\n" +
                "'복원' 버튼으로 언제든 되돌릴 수 있습니다.",
                "구간 제외 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool TryReadSelectedRange(out int from, out int to)
        {
            from = 0;
            to = 0;

            bool hasMin = !string.IsNullOrWhiteSpace(txtFrameRangeStart.Text);
            bool hasMax = !string.IsNullOrWhiteSpace(txtFrameRangeEnd.Text);
            if (!hasMin || !hasMax) return false;

            if (!int.TryParse(txtFrameRangeStart.Text.Trim(), out from) ||
                !int.TryParse(txtFrameRangeEnd.Text.Trim(), out to))
            {
                MessageBox.Show(
                    "구간 시작/끝에는 정수 프레임 번호를 입력해 주세요.\n예: 100 ~ 250",
                    "구간 입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (from > to)
            {
                MessageBox.Show("구간 시작 번호는 끝 번호보다 클 수 없습니다.",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private List<FrameData> GetSelectedFrames()
        {
            var selectedFrames = new List<FrameData>();
            foreach (DataGridViewRow row in dgvFrameCatalog.SelectedRows)
            {
                if (row.DataBoundItem is FrameData frame && !selectedFrames.Contains(frame))
                    selectedFrames.Add(frame);
            }
            return selectedFrames;
        }

        private void SetDeletedByRange(int from, int to, bool isDeleted)
        {
            if (_allFrames == null || _allFrames.Count == 0) return;

            int safeFrom = Math.Max(0, from);
            int safeTo = Math.Min(_allFrames.Count - 1, to);
            if (safeFrom > safeTo)
            {
                MessageBox.Show(
                    $"유효한 프레임 범위를 벗어났습니다.\n입력 범위: {from} ~ {to} | 전체 프레임: {_allFrames.Count}",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = safeFrom; i <= safeTo; i++)
                _allFrames[i].IsDeleted = isDeleted;

            MarkCleanupStateChanged();
        }

        private void FinishDataStateChange()
        {
            MarkCleanupStateChanged();
            txtFrameRangeStart.Clear();
            txtFrameRangeEnd.Clear();

            _isChartDirty = true;
            RefreshFrameView();
            RenderFrameChart();

            if (_visibleFrames.Count > 0)
                SetIndex(Math.Min(_currentFrameIndex < 0 ? 0 : _currentFrameIndex, _visibleFrames.Count - 1));
            else
                ClearPreviewSelection();
        }

        private void ClearPreviewSelection()
        {
            _currentFrameIndex = -1;
            dgvFrameCatalog.ClearSelection();
            picFramePreview.Image?.Dispose();
            picFramePreview.Image = null;
            lblFrameValue.Text = "Frame: 0 / 0";
            lblAngleValue.Text = "조향값: 0.000";
            lblThrottleValue.Text = "스로틀값: 0.000";
            lblModeValue.Text = "모드: -";
            if (_lblFrameImageName != null)
                _lblFrameImageName.Text = "이미지: -";
            if (_lblFrameReviewHint != null)
                _lblFrameReviewHint.Text = "검토: -";
            if (_thumbnailStrip != null)
                _thumbnailStrip.Controls.Clear();
            UpdateNavigationButtons();
        }

        private void BtnSaveCleanupState_Click(object? sender, EventArgs e)
        {
            if (!TryEnsureLoadedFolder()) return;

            try
            {
                string metaPath = Path.Combine(_currentDataFolderPath, DeletedFramesMetaFileName);
                var deletedNames = _allFrames
                    .Where(frame => frame.IsDeleted)
                    .Select(frame => frame.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                File.WriteAllLines(metaPath, deletedNames, Encoding.UTF8);
                _hasUnsavedCleanupChanges = false;
                UpdateStatusLabels();
                MessageBox.Show("제외 상태가 저장되었습니다.",
                    "상태 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("제외 상태 저장 중 오류가 발생했습니다.\n" + ex.Message,
                    "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportCleanDataset_Click(object? sender, EventArgs e)
        {
            if (!TryEnsureLoadedFolder()) return;

            int total = _allFrames.Count;
            int deleted = _allFrames.Count(frame => frame.IsDeleted);
            int valid = Math.Max(0, total - deleted);
            int review = _allFrames.Count(frame => frame.NeedsReview && !frame.IsDeleted);

            var confirm = MessageBox.Show(
                "현재 폴더를 복사한 뒤 제외된 프레임을 제거한 학습용 Clean 폴더를 생성합니다.\n원본 폴더는 변경하지 않습니다.\n\n" +
                $"전체 프레임: {total}개\n제외 프레임: {deleted}개\n추출 후 사용 프레임: {valid}개\n남은 이상 후보: {review}개\n\n계속하시겠습니까?",
                "클린 폴더 추출", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string sourceFolder = _currentDataFolderPath;
                string parentFolder = Directory.GetParent(sourceFolder)?.FullName ?? sourceFolder;
                string cleanFolder = Path.Combine(parentFolder, Path.GetFileName(sourceFolder) + "_Clean");

                if (Directory.Exists(cleanFolder))
                {
                    var overwriteConfirm = MessageBox.Show(
                        "이미 Clean 폴더가 있습니다. 기존 Clean 폴더를 삭제하고 다시 생성할까요?",
                        "Clean 폴더 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (overwriteConfirm != DialogResult.Yes) return;

                    Directory.Delete(cleanFolder, true);
                }

                CopyDirectory(sourceFolder, cleanFolder);

                var deletedNames = _allFrames
                    .Where(frame => frame.IsDeleted)
                    .Select(frame => frame.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                int deletedImageCount = DeleteImagesFromCleanFolder(cleanFolder, deletedNames);
                int cleanedCatalogCount = RewriteCleanCatalogFiles(cleanFolder, deletedNames);

                MessageBox.Show(
                    $"학습용 Clean 폴더가 생성되었습니다.\n\n경로: {cleanFolder}\n삭제된 이미지: {deletedImageCount}개\n정제된 카탈로그: {cleanedCatalogCount}개",
                    "클린 폴더 추출 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("클린 폴더 추출 중 오류가 발생했습니다.\n" + ex.Message,
                    "추출 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryEnsureLoadedFolder()
        {
            if (string.IsNullOrWhiteSpace(_currentDataFolderPath) || !Directory.Exists(_currentDataFolderPath))
            {
                MessageBox.Show(
                    "먼저 DonkeyCar 데이터 폴더를 열어 주세요.\n\n" +
                    "선택할 폴더 예시:\n" +
                    "- catalog_0.catalog, catalog_1.catalog가 들어 있는 tub 폴더\n" +
                    "- 이미지가 폴더 루트 또는 images 하위 폴더에 있는 폴더",
                    "폴더 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_allFrames == null || _allFrames.Count == 0)
            {
                MessageBox.Show("저장하거나 추출할 프레임 데이터가 없습니다.",
                    "데이터 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void CopyDirectory(string sourceFolder, string targetFolder)
        {
            Directory.CreateDirectory(targetFolder);

            foreach (string sourceFile in Directory.GetFiles(sourceFolder))
            {
                string targetFile = Path.Combine(targetFolder, Path.GetFileName(sourceFile));
                File.Copy(sourceFile, targetFile, true);
            }

            foreach (string sourceSubFolder in Directory.GetDirectories(sourceFolder))
            {
                string targetSubFolder = Path.Combine(targetFolder, Path.GetFileName(sourceSubFolder));
                CopyDirectory(sourceSubFolder, targetSubFolder);
            }
        }

        private int DeleteImagesFromCleanFolder(string cleanFolder, HashSet<string> deletedNames)
        {
            int deletedImageCount = 0;
            foreach (string imagePath in Directory.EnumerateFiles(cleanFolder, "*.jpg", SearchOption.AllDirectories))
            {
                string imageName = Path.GetFileName(imagePath);
                if (!deletedNames.Contains(imageName)) continue;

                File.Delete(imagePath);
                deletedImageCount++;
            }
            return deletedImageCount;
        }

        private int RewriteCleanCatalogFiles(string cleanFolder, HashSet<string> deletedNames)
        {
            int cleanedCatalogCount = 0;
            foreach (string catalogPath in Directory.EnumerateFiles(cleanFolder, "catalog_*.catalog", SearchOption.AllDirectories))
            {
                var keptLines = new List<string>();
                foreach (string rawLine in File.ReadAllLines(catalogPath, Encoding.UTF8))
                {
                    string imageName = TryExtractImageNameFromCatalogLine(rawLine);
                    if (string.IsNullOrWhiteSpace(imageName) || !deletedNames.Contains(imageName))
                        keptLines.Add(rawLine);
                }

                File.WriteAllLines(catalogPath, keptLines, Encoding.UTF8);
                cleanedCatalogCount++;
            }
            return cleanedCatalogCount;
        }

        private void RestoreAll()
        {
            if (_allFrames == null) return;

            foreach (var f in _allFrames)
                f.IsDeleted = false;

            MarkCleanupStateChanged();
            // 복원 후 전체 프레임을 다시 표시합니다.
            _visibleFrames = _allFrames;
            _isChartDirty = true;
            RefreshFrameView();
            RenderFrameChart();
            SetIndex(0);

            MessageBox.Show("모든 프레임이 복원되었습니다.",
                "복원 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStatusLabels()
        {
            int total   = _allFrames?.Count ?? 0;
            int deleted = _allFrames?.Count(f => f.IsDeleted) ?? 0;
            int valid   = Math.Max(0, total - deleted);
            int shown   = _visibleFrames?.Count ?? 0;
            string filterState = _isFrameFilterActive ? "필터 적용" : "전체 보기";

            int review = _allFrames?.Count(f => f.NeedsReview && !f.IsDeleted) ?? 0;
            string saveState = _hasUnsavedCleanupChanges ? "저장 필요" : "저장됨";

            stsFrameSummary.Text =
                $"{filterState}  |  전체: {total}  |  유효: {valid}  |  제외: {deleted}  |  후보: {review}  |  표시: {shown}  |  {saveState}";
        }

        private void MarkCleanupStateChanged()
        {
            _hasUnsavedCleanupChanges = true;
            UpdateStatusLabels();
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!_hasUnsavedCleanupChanges) return;

            var answer = MessageBox.Show(
                "제외/복원 상태가 아직 저장되지 않았습니다.\n\n저장하지 않고 종료하면 다음 실행 때 정리 상태가 복원되지 않을 수 있습니다.\n계속 종료하시겠습니까?",
                "저장되지 않은 정리 상태",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (answer != DialogResult.Yes)
                e.Cancel = true;
        }

        // FrameData 데이터 모델

        private class FrameData
        {
            public string ImagePath  { get; set; } = string.Empty;
            public double Angle      { get; set; }
            public double Throttle   { get; set; }
            public string Mode       { get; set; } = "-";
            public string Scenario   { get; set; } = "-";
            public string Name       { get; set; } = string.Empty;
            public int OriginalIndex { get; set; } = -1;

            /// <summary>
            /// true이면 학습에서 제외하지만 실제 파일은 보존합니다.
            /// </summary>
            public bool IsDeleted { get; set; } = false;

            /// <summary>
            /// catalog_0.catalog 파일을 찾지 못한 경우 true입니다.
            /// </summary>
            public bool IsCatalogMissing { get; set; } = false;

            /// <summary>
            /// catalog에는 있지만 읽을 데이터가 없을 경우 true입니다.
            /// </summary>
            public bool HasNoData { get; set; } = false;

            /// <summary>
            /// 화면 표시용 인덱스입니다.
            /// RefreshFrameView 호출 시 자동으로 갱신됩니다.
            /// IsDeleted == false : "[0000]", "[0001]", ...
            /// IsDeleted == true  : "[XXXX]"
            /// </summary>
            public string FormattedIndex { get; set; } = "[----]";

            /// <summary>
            /// 표의 상태 컬럼에 표시할 삭제 여부입니다.
            /// </summary>
            public string StateText => IsDeleted ? "제외됨" : "사용";

            public string SteeringText
            {
                get
                {
                    if (Angle <= -0.55) return "강한 좌회전";
                    if (Angle < -0.1) return "좌회전";
                    if (Angle >= 0.55) return "강한 우회전";
                    if (Angle > 0.1) return "우회전";
                    return "직진";
                }
            }

            public string ThrottleText
            {
                get
                {
                    if (Throttle < -0.05) return "후진";
                    if (Throttle <= 0.05) return "정지에 가까움";
                    if (Throttle >= 0.75) return "빠른 전진";
                    return "전진";
                }
            }

            public string ModeDescription
            {
                get
                {
                    string normalized = NormalizeDisplayValue(Mode);
                    return normalized switch
                    {
                        "user" => "사용자 조작",
                        "local" => "자율주행",
                        "local_angle" => "자율 조향",
                        "-" or "" => "모드 없음",
                        _ => Mode
                    };
                }
            }

            public string ScenarioDescription
            {
                get
                {
                    string normalized = NormalizeDisplayValue(Scenario);
                    return normalized switch
                    {
                        "normal" => "일반 주행",
                        "night" => "야간/어두움",
                        "left_turn" => "좌회전 구간",
                        "right_turn" => "우회전 구간",
                        "out_of_bound" => "차선 이탈",
                        "-" or "" => "시나리오 없음",
                        _ => Scenario
                    };
                }
            }

            public bool NeedsReview =>
                IsCatalogMissing ||
                HasNoData ||
                Math.Abs(Angle) >= 0.85 ||
                Math.Abs(Throttle) <= 0.02 ||
                NormalizeDisplayValue(Scenario).Contains("out_of_bound");

            public string ReviewHint
            {
                get
                {
                    var hints = new List<string>();
                    if (IsCatalogMissing) hints.Add("catalog 없음");
                    if (HasNoData) hints.Add("값 없음");
                    if (Math.Abs(Angle) >= 0.85) hints.Add("급조향");
                    if (Math.Abs(Throttle) <= 0.02) hints.Add("정지/무입력");
                    if (NormalizeDisplayValue(Scenario).Contains("out_of_bound")) hints.Add("차선 이탈");
                    return hints.Count == 0 ? "정상" : string.Join(", ", hints);
                }
            }

            private static string NormalizeDisplayValue(string text)
            {
                return (text ?? string.Empty)
                    .Trim()
                    .Replace("-", "_")
                    .Replace(" ", "_")
                    .ToLowerInvariant();
            }

            /// <summary>
            /// 이전 ListBox 표시 방식과 튜토리얼 설명에서 사용하는 요약 이름입니다.
            /// 삭제된 항목은 [XXXX] 접두어를 사용합니다.
            /// 표시 우선순위:
            ///   1) 카탈로그 파일 없음
            ///   2) catalog 데이터 없음
            ///   3) 주행 방향/상태 요약
            /// 예: "[XXXX] 3_cam-image_array_.jpg | 후진 (조향: -0.80, 스로틀: -0.50)"
            /// </summary>
            public string DisplayName
            {
                get
                {
                    if (IsCatalogMissing)
                        return $"{FormattedIndex} {Name} | [\uCE74\uD0C8\uB85C\uADF8 \uD30C\uC77C \uC5C6\uC74C]";
                    if (HasNoData)
                        return $"{FormattedIndex} {Name} | catalog \uAC12 \uC5C6\uC74C";

                    string dir;
                    if (Throttle < 0)
                        dir = "\uD6C4\uC9C4";
                    else if (Angle < -0.1)
                        dir = "\uC88C\uD68C\uC804";
                    else if (Angle > 0.1)
                        dir = "\uC6B0\uD68C\uC804";
                    else
                        dir = "\uC9C1\uC9C4";

                    return $"{FormattedIndex} {Name} | {dir} (\uC870\uD5A5: {Angle:0.00}, \uC2A4\uB85C\uD2C0: {Throttle:0.00})";
                }
            }
        }

        // DonkeyCar Tub V2 Parser: 여러 catalog_*.catalog 파일을 순서대로 읽어 하나의 목록으로 병합합니다.

        private int GetCatalogNumber(string catalogPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(catalogPath);
            string numberText = fileName.Replace("catalog_", "");
            return int.TryParse(numberText, out int number) ? number : int.MaxValue;
        }

        private IEnumerable<string> GetImageFiles(string folderPath)
        {
            var imageFiles = new List<string>();

            if (Directory.Exists(folderPath))
                imageFiles.AddRange(Directory.EnumerateFiles(folderPath, "*.jpg"));

            string imagesFolder = Path.Combine(folderPath, "images");
            if (Directory.Exists(imagesFolder))
                imageFiles.AddRange(Directory.EnumerateFiles(imagesFolder, "*.jpg"));

            return imageFiles
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);
        }

        private FrameData? ParseFrameFromCatalogLine(string rawLine, string folderPath)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) return null;

            using var doc = JsonDocument.Parse(line);
            JsonElement root = doc.RootElement;

            string imageName = ReadString(root, "cam/image_array");
            if (string.IsNullOrWhiteSpace(imageName) || imageName == "-")
                return null;

            return new FrameData
            {
                Name = imageName,
                ImagePath = ResolveImagePathFromFolder(folderPath, imageName),
                Angle = ReadDouble(root, "user/angle", "angle"),
                Throttle = ReadDouble(root, "user/throttle", "throttle"),
                Mode = ReadString(root, "user/mode", "mode", "user/behavior_state"),
                Scenario = ReadString(root, "user/scenario", "scenario", "sim/info"),
                IsDeleted = false
            };
        }

        private string TryExtractImageNameFromCatalogLine(string rawLine)
        {
            try
            {
                using var doc = JsonDocument.Parse(rawLine);
                return ReadString(doc.RootElement, "cam/image_array");
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ReadString(JsonElement root, params string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                if (!root.TryGetProperty(propertyName, out JsonElement value)) continue;
                if (value.ValueKind == JsonValueKind.String)
                    return value.GetString() ?? string.Empty;
                return value.ToString();
            }
            return "-";
        }

        private double ReadDouble(JsonElement root, params string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                if (!root.TryGetProperty(propertyName, out JsonElement value)) continue;
                if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out double number))
                    return number;
                if (double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out number))
                    return number;
            }
            return 0;
        }

        private string ResolveImagePathFromFolder(string folderPath, string imageName)
        {
            string rootImagePath = Path.Combine(folderPath, imageName);
            if (File.Exists(rootImagePath)) return rootImagePath;

            string imagesFolderPath = Path.Combine(folderPath, "images", imageName);
            if (File.Exists(imagesFolderPath)) return imagesFolderPath;

            return rootImagePath;
        }

        private void RestoreDeletedFramesMeta(string folderPath, List<FrameData> frames)
        {
            string metaPath = Path.Combine(folderPath, DeletedFramesMetaFileName);
            if (!File.Exists(metaPath)) return;

            var deletedNames = File.ReadAllLines(metaPath, Encoding.UTF8)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var frame in frames)
                frame.IsDeleted = deletedNames.Contains(frame.Name);
        }

        /// <summary>
        /// catalog_*.catalog 전체를 파일명 숫자 기준으로 정렬해 읽습니다.
        /// catalog가 없으면 이미지 파일 목록만으로 프레임을 구성합니다.
        /// </summary>
        private List<FrameData> LoadTubData(string folderPath)
        {
            _currentDataFolderPath = folderPath;

            var frames = new List<FrameData>();
            string[] catalogFiles = Directory.GetFiles(folderPath, "catalog_*.catalog")
                                             .OrderBy(GetCatalogNumber)
                                             .ThenBy(path => path, StringComparer.OrdinalIgnoreCase)
                                             .ToArray();

            if (catalogFiles.Length > 0)
            {
                foreach (string catalogPath in catalogFiles)
                {
                    foreach (string rawLine in File.ReadAllLines(catalogPath, Encoding.UTF8))
                    {
                        try
                        {
                            FrameData? frame = ParseFrameFromCatalogLine(rawLine, folderPath);
                            if (frame == null) continue;

                            frame.OriginalIndex = frames.Count;
                            frames.Add(frame);
                        }
                        catch { }
                    }
                }
            }
            else
            {
                foreach (string imagePath in GetImageFiles(folderPath))
                {
                    frames.Add(new FrameData
                    {
                        OriginalIndex = frames.Count,
                        Name = Path.GetFileName(imagePath),
                        ImagePath = imagePath,
                        IsCatalogMissing = true
                    });
                }
            }

            RestoreDeletedFramesMeta(folderPath, frames);
            return frames;
        }

        private async Task LoadCatalogAsync(string folder)
        {
            SetLoadingState(true);
            try
            {
                var frames = await Task.Run(() => LoadTubData(folder));

                if (frames.Count == 0)
                {
                    MessageBox.Show(
                        "선택한 폴더에서 .jpg 이미지 파일을 찾을 수 없습니다.\n" +
                        "이미지가 폴더 루트 또는 하위 'images' 폴더에 있는지 확인해 주세요.",
                        "이미지 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _allFrames = frames;
                _hasUnsavedCleanupChanges = false;
                // 초기 로드: 전체 프레임을 표시합니다.
                RefreshFrameBinding();  // _visibleFrames 설정과 RefreshFrameView 호출

                stsDataPath.Text = "경로: " + folder;
                _isChartDirty = true;
                SetIndex(0);
                ShowLoadedDataGuidance();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "데이터 로드 중 오류가 발생했습니다.\n\n" +
                    "확인할 점:\n" +
                    "- 선택한 폴더가 DonkeyCar tub 폴더인지\n" +
                    "- catalog_*.catalog 파일이 JSON line 형식인지\n" +
                    "- 이미지가 루트 또는 images 폴더에 있는지\n\n" +
                    "오류 내용: " + ex.Message,
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void ShowLoadedDataGuidance()
        {
            int reviewCount = _allFrames.Count(frame => frame.NeedsReview);
            if (reviewCount <= 0) return;

            MessageBox.Show(
                $"자동 검토 후보 {reviewCount}개를 찾았습니다.\n\n" +
                "노란색 행을 먼저 확인하세요.\n" +
                "후보 기준: 급조향, 정지에 가까운 스로틀, out_of_bound 시나리오, catalog 누락/값 없음",
                "이상 후보 안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 학습 실행 기능

        /// <summary>
        /// develop_CHS 브랜치의 학습 실행 흐름을 feature_test UI에 맞춰 초기화합니다.
        /// 실제 학습 엔진은 수정하지 않고, WSL에서 DonkeyCar train.py를 실행하고 로그만 관찰합니다.
        /// </summary>
        private void InitializeTrainingControls()
        {
            cmbTrainingModelType.Items.Clear();
            cmbTrainingModelType.Items.AddRange(new object[] { "linear", "categorical", "rnn", "3d", "imu", "behavior" });
            if (string.IsNullOrWhiteSpace(cmbTrainingModelType.Text))
                cmbTrainingModelType.Text = "linear";

            if (string.IsNullOrWhiteSpace(txtTrainingPythonEnvName.Text))
                txtTrainingPythonEnvName.Text = "donkey";

            if (string.IsNullOrWhiteSpace(txtMycarProjectPath.Text))
                txtMycarProjectPath.Text = "~/mycar";

            if (string.IsNullOrWhiteSpace(txtTrainingTubPath.Text))
                txtTrainingTubPath.Text = "data";

            if (string.IsNullOrWhiteSpace(txtTrainingModelPath.Text))
                txtTrainingModelPath.Text = "models/pilot.keras";

            numTrainingEpochs.Minimum = 1;
            numTrainingEpochs.Maximum = 10000;
            if (numTrainingEpochs.Value < 1)
                numTrainingEpochs.Value = 10;

            btnStopTrainingProcess.Enabled = false;
            stsTrainingStatus.Text = "학습 상태: 대기";
        }

        private async void BtnStartTrainingProcess_Click(object? sender, EventArgs e)
        {
            if (_trainingProcess is { HasExited: false })
            {
                MessageBox.Show("이미 학습 프로세스가 실행 중입니다.",
                    "학습 실행", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryBuildTrainingCommand(out string arguments)) return;

            btnStartTrainingProcess.Enabled = false;
            btnStopTrainingProcess.Enabled = true;
            stsTrainingStatus.Text = "학습 상태: 실행 중";
            rtbTrainingOutput.Clear();

            try
            {
                _trainingProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "wsl",
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    },
                    EnableRaisingEvents = true
                };

                _trainingProcess.OutputDataReceived += (_, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(ev.Data))
                        AppendTrainingLog(ev.Data);
                };
                _trainingProcess.ErrorDataReceived += (_, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(ev.Data))
                        AppendTrainingLog("[error] " + ev.Data);
                };

                AppendTrainingLog("학습 프로세스를 시작합니다.");
                AppendTrainingLog("실행 명령: wsl " + arguments);

                _trainingProcess.Start();
                _trainingProcess.BeginOutputReadLine();
                _trainingProcess.BeginErrorReadLine();

                await _trainingProcess.WaitForExitAsync();

                AppendTrainingLog("");
                AppendTrainingLog($"학습 프로세스가 종료되었습니다. 종료 코드: {_trainingProcess.ExitCode}");
                stsTrainingStatus.Text = $"학습 상태: 종료 코드 {_trainingProcess.ExitCode}";
            }
            catch (Exception ex)
            {
                stsTrainingStatus.Text = "학습 상태: 오류";
                MessageBox.Show(
                    "학습 실행 중 오류가 발생했습니다.\n\n" +
                    "WSL, conda 환경명, mycar 경로가 올바른지 확인해 주세요.\n\n" +
                    "오류 내용: " + ex.Message,
                    "학습 실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnStartTrainingProcess.Enabled = true;
                btnStopTrainingProcess.Enabled = false;
            }
        }

        private void BtnStopTrainingProcess_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_trainingProcess == null || _trainingProcess.HasExited)
                {
                    MessageBox.Show("중지할 학습 프로세스가 없습니다.",
                        "학습 중지", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _trainingProcess.Kill(entireProcessTree: true);
                AppendTrainingLog("사용자 요청으로 학습 프로세스를 중지했습니다.");
                stsTrainingStatus.Text = "학습 상태: 중지됨";
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 프로세스를 중지하지 못했습니다.\n\n오류 내용: " + ex.Message,
                    "학습 중지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryBuildTrainingCommand(out string arguments)
        {
            arguments = string.Empty;

            string envName = txtTrainingPythonEnvName.Text.Trim();
            string modelType = cmbTrainingModelType.Text.Trim().ToLowerInvariant();
            string mycarPath = txtMycarProjectPath.Text.Trim();
            string tubPath = txtTrainingTubPath.Text.Trim();
            string modelPath = txtTrainingModelPath.Text.Trim();
            int epochs = (int)numTrainingEpochs.Value;

            if (string.IsNullOrWhiteSpace(envName))
            {
                MessageBox.Show("Python 환경명을 입력해 주세요. 예: donkey",
                    "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(modelType))
            {
                MessageBox.Show("모델 종류를 입력하거나 선택해 주세요.",
                    "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(mycarPath))
                mycarPath = "~/mycar";
            if (string.IsNullOrWhiteSpace(tubPath))
                tubPath = "data";
            if (string.IsNullOrWhiteSpace(modelPath))
                modelPath = "models/pilot.keras";

            string command =
                "cd " + QuoteForBash(ToWslPath(mycarPath)) + " && " +
                "~/miniconda3/bin/conda run -n " + QuoteForBash(envName) + " " +
                "python train.py " +
                "--tub=" + QuoteForBash(ToWslPath(tubPath)) + " " +
                "--model=" + QuoteForBash(ToWslPath(modelPath)) + " " +
                "--type=" + QuoteForBash(modelType) + " " +
                "--epochs=" + epochs.ToString(CultureInfo.InvariantCulture);

            arguments = "bash -lc " + QuoteForBash(command);
            return true;
        }

        private string ToWslPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;
            path = path.Trim();

            if (path.StartsWith("~/") || path.StartsWith("/"))
                return path.Replace("\\", "/");

            if (path.Length >= 3 && path[1] == ':' && (path[2] == '\\' || path[2] == '/'))
            {
                char drive = char.ToLowerInvariant(path[0]);
                string rest = path.Substring(3).Replace("\\", "/");
                return $"/mnt/{drive}/{rest}";
            }

            return path.Replace("\\", "/");
        }

        private string QuoteForBash(string value)
        {
            return "'" + value.Replace("'", "'\"'\"'") + "'";
        }

        private void AppendTrainingLog(string text)
        {
            if (rtbTrainingOutput.IsDisposed) return;

            void Append()
            {
                rtbTrainingOutput.AppendText(text + Environment.NewLine);
                rtbTrainingOutput.SelectionStart = rtbTrainingOutput.TextLength;
                rtbTrainingOutput.ScrollToCaret();
            }

            if (rtbTrainingOutput.InvokeRequired)
                rtbTrainingOutput.BeginInvoke(new Action(Append));
            else
                Append();
        }

        private void SelectFolderInto(TextBox targetTextBox, string title)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = title,
                SelectedPath = Directory.Exists(targetTextBox.Text) ? targetTextBox.Text : string.Empty
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                targetTextBox.Text = dialog.SelectedPath;
        }

        private void BtnSaveTrainingConfig_Click(object? sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(Application.UserAppDataPath);
                var settings = new TrainingSettings
                {
                    MycarPath = txtMycarProjectPath.Text.Trim(),
                    TubPath = txtTrainingTubPath.Text.Trim(),
                    ModelPath = txtTrainingModelPath.Text.Trim(),
                    ModelType = cmbTrainingModelType.Text.Trim(),
                    PythonEnvName = txtTrainingPythonEnvName.Text.Trim(),
                    Epochs = (int)numTrainingEpochs.Value
                };

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetTrainingSettingsPath(), json, Encoding.UTF8);
                MessageBox.Show("학습 설정이 저장되었습니다.",
                    "설정 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 설정 저장 중 오류가 발생했습니다.\n\n오류 내용: " + ex.Message,
                    "설정 저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTrainingSettings()
        {
            try
            {
                string path = GetTrainingSettingsPath();
                if (!File.Exists(path)) return;

                string json = File.ReadAllText(path, Encoding.UTF8);
                var settings = JsonSerializer.Deserialize<TrainingSettings>(json);
                if (settings == null) return;

                txtMycarProjectPath.Text = settings.MycarPath;
                txtTrainingTubPath.Text = settings.TubPath;
                txtTrainingModelPath.Text = settings.ModelPath;
                cmbTrainingModelType.Text = settings.ModelType;
                txtTrainingPythonEnvName.Text = settings.PythonEnvName;
                if (settings.Epochs >= numTrainingEpochs.Minimum && settings.Epochs <= numTrainingEpochs.Maximum)
                    numTrainingEpochs.Value = settings.Epochs;
            }
            catch
            {
                // 설정 파일이 손상되어도 프로그램 실행은 계속되어야 합니다.
            }
        }

        private string GetTrainingSettingsPath()
        {
            return Path.Combine(Application.UserAppDataPath, TrainingSettingsFileName);
        }

        private sealed class TrainingSettings
        {
            public string MycarPath { get; set; } = "~/mycar";
            public string TubPath { get; set; } = "data";
            public string ModelPath { get; set; } = "models/pilot.keras";
            public string ModelType { get; set; } = "linear";
            public string PythonEnvName { get; set; } = "donkey";
            public int Epochs { get; set; } = 10;
        }

        // Designer 연결 스텁

        private void TabTrainingMonitor_Click(object sender, EventArgs e) { }
        private void LblFilterMin_Click(object sender, EventArgs e) { }
        private void MnuHelp_Click(object sender, EventArgs e) => RunFeatureTutorial("도움말");
        private void MnuViewOpenGraphStats_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabGraphStats;
        }
        private void LblModeValue_Click(object sender, EventArgs e) { }
        private void LblAngleValue_Click(object sender, EventArgs e) { }
        private void LblPlayInterval_Click(object sender, EventArgs e) { }
        private void LblThrottleValue_Click(object sender, EventArgs e) { }
        private void StsTrainingStatus_Click(object sender, EventArgs e) { }
        private void BtnSelectMycarPath_Click(object sender, EventArgs e) => SelectFolderInto(txtMycarProjectPath, "mycar 폴더 선택");
        private void GrpDataCleaner_Enter(object sender, EventArgs e) { }

        // ScottPlot 차트를 필요할 때 생성합니다.

        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabGraphStats && _isChartDirty)
                RenderFrameChart();
        }

        private void InitFrameChart()
        {
            _frameChart = new FormsPlot
            {
                Location = new System.Drawing.Point(0, 42),
                Size     = new System.Drawing.Size(
                    tabGraphStats.ClientSize.Width,
                    tabGraphStats.ClientSize.Height - 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right,
                Name = "formsPlotMain"
            };

            _frameChart.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1e1e1e");
            _frameChart.Plot.DataBackground.Color   = ScottPlot.Color.FromHex("#2d2d30");

            tabGraphStats.Controls.Add(_frameChart);
            _frameChart.BringToFront();
        }

        // 기본 모드는 선 그래프, 필터 모드는 원본 인덱스 기준 점 그래프로 표시합니다.
        /// <summary>
        /// 기본 모드에서는 제외되지 않은 원본 프레임을 연속 인덱스 Line으로 표시합니다.
        /// 필터 모드에서는 필터 결과를 원본 인덱스 Scatter로 표시합니다.
        /// </summary>
        private void RenderFrameChart()
        {
            if (_allFrames == null || _allFrames.Count == 0)
            {
                _isChartDirty = false;
                return;
            }

            if (_frameChart == null) InitFrameChart();

            var plot = _frameChart!.Plot;

            // 한글 폰트를 지정해 차트 라벨이 깨지지 않도록 합니다.
            _frameChart.Plot.Axes.Title.Label.FontName           = "Malgun Gothic";
            _frameChart.Plot.Axes.Bottom.Label.FontName          = "Malgun Gothic";
            _frameChart.Plot.Axes.Left.Label.FontName            = "Malgun Gothic";
            _frameChart.Plot.Axes.Bottom.TickLabelStyle.FontName = "Malgun Gothic";
            _frameChart.Plot.Axes.Left.TickLabelStyle.FontName   = "Malgun Gothic";

            plot.Clear();

            var chartFrames = _isFrameFilterActive
                ? _visibleFrames.Where(f => !f.IsDeleted).ToList()
                : _allFrames.Where(f => !f.IsDeleted).ToList();

            if (chartFrames.Count == 0)
            {
                plot.Title("유효한 데이터가 없습니다 (모두 제외됨)");
                _frameChart.Refresh();
                _isChartDirty = false;
                return;
            }

            int n = chartFrames.Count;
            double[] xs = _isFrameFilterActive
                ? chartFrames.Select(frame => (double)frame.OriginalIndex).ToArray()
                : Enumerable.Range(0, n).Select(i => (double)i).ToArray();
            double[] angleYs = chartFrames.Select(f => f.Angle).ToArray();
            double[] throttleYs = chartFrames.Select(f => f.Throttle).ToArray();

            if (_isFrameFilterActive)
            {
                var angleScatter = plot.Add.Scatter(xs, angleYs);
                angleScatter.Color = ScottPlot.Color.FromHex("#4FC3F7");
                angleScatter.LineWidth = 0;
                angleScatter.MarkerSize = 6;
                angleScatter.LegendText = "조향(Angle)";

                var throttleScatter = plot.Add.Scatter(xs, throttleYs);
                throttleScatter.Color = ScottPlot.Color.FromHex("#81C784");
                throttleScatter.LineWidth = 0;
                throttleScatter.MarkerSize = 6;
                throttleScatter.LegendText = "스로틀(Throttle)";
            }
            else
            {
                var sigAngle = plot.Add.SignalXY(xs, angleYs);
                sigAngle.Color = ScottPlot.Color.FromHex("#4FC3F7");
                sigAngle.LineWidth = 1.5f;
                sigAngle.LegendText = "조향(Angle)";

                var sigThrottle = plot.Add.SignalXY(xs, throttleYs);
                sigThrottle.Color = ScottPlot.Color.FromHex("#81C784");
                sigThrottle.LineWidth = 1.5f;
                sigThrottle.LegendText = "스로틀(Throttle)";
            }

            // 축 라벨과 제목
            plot.XLabel(_isFrameFilterActive
                ? "원본 프레임 인덱스(필터 결과)"
                : "유효 프레임 인덱스(제외된 프레임은 건너뜀)");
            plot.YLabel("값(Angle / Throttle)");
            plot.Title(_isFrameFilterActive
                ? $"필터 결과 Scatter 그래프 [표시: {n} / 전체: {_allFrames.Count}]"
                : $"조향값/스로틀 Line 그래프 [유효: {n} / 전체: {_allFrames.Count}]");
            plot.Axes.SetLimitsY(-1.2, 1.2);
            plot.ShowLegend(Alignment.UpperRight);

            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#444444");

            _frameChart.Refresh();
            _isChartDirty = false;
        }
    }
}
