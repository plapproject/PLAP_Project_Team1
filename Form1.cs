using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        private bool _hasUnsavedCleanupChanges = false;
        private bool _showDriveOverlay = true;
        private Process? _trainingProcess;
        private Button? _btnShowReviewCandidates;
        private Button? _btnSelectTrainingModelPath;
        private Button? _btnOpenTrainingModelFolder;
        private Button? _btnCopyModelUseCommand;
        private Button? _btnCheckTrainingEnvironment;
        private Button? _btnAutoSetupTrainingEnvironment;
        private Button? _btnCopyTrainingSetupCommands;
        private Button? _btnEditTrainingEnvironment;
        private System.Windows.Forms.Label? _lblTrainingStepGuide;
        private System.Windows.Forms.Label? _lblTrainingEnvironmentSummary;
        private System.Windows.Forms.Label? _lblTrainingEnvironmentStatus;
        private System.Windows.Forms.Label? _lblTrainingActionHint;
        private System.Windows.Forms.Label? _lblTrainingModelSafetyNote;
        private System.Windows.Forms.Label? _lblTrainingSummaryStatus;
        private System.Windows.Forms.Label? _lblTrainingSummaryEpoch;
        private System.Windows.Forms.Label? _lblTrainingSummaryProgress;
        private System.Windows.Forms.Label? _lblTrainingSummaryLoss;
        private System.Windows.Forms.Label? _lblTrainingSummaryModelPath;
        private System.Windows.Forms.Label? _lblFrameReviewHint;
        private ToolTip? _trainingToolTip;
        private int _lastTrainingProgressPercent = -1;
        private bool _isTrainingEnvironmentReady = false;
        private bool _isCheckingTrainingEnvironment = false;
        private string _trainingMycarPath = "~/mycar";
        private string _trainingPythonEnvName = "e2e_env";
        private string _trainingModelType = "linear";
        private const string DeletedFramesMetaFileName = "deleted_frames_meta.txt";
        private const string TrainingSettingsFileName = "training_settings.json";
        private const string RunnerWindowsDonkey = "Windows donkey CLI";
        private const string RunnerWindowsConda = "Windows conda";
        private const string RunnerWslConda = "WSL conda";
        private const string RunnerWslDonkey = "WSL donkey CLI";

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
            btnSelectTrainingTubPath.Click          += (_, _) => SelectFolderInto(txtTrainingTubPath, "학습 데이터 폴더 선택");

            mnuFileOpenDataFolder.Click   += (s, _) => BtnOpenDataFolder_Click(s!, EventArgs.Empty);
            mnuFileReloadData.Click       += (s, _) => BtnReloadData_Click(s!, EventArgs.Empty);
            mnuExit.Click             += (s, _) => Application.Exit();
            mnuHelpOpenTutorial.Click        += (s, _) => RunFeatureTutorial("도움말");

            tabControlMain.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            FormClosing += Form1_FormClosing;

            ConfigureFrameCatalogGrid();
            ApplyDataManagerUiStyle();
            ConfigureReviewCandidateControls();

            btnExcludeSelectedFrames.Text = "선택 프레임 제외";
            btnExportCleanDataset.Text          = "클린 폴더 추출";
            btnRestoreFrames.Text         = "복원";
            txtAngleMinFilter.Text    = "-1";
            txtAngleMaxFilter.Text    = "1";
            txtThrottleMinFilter.Text = "-1";
            txtThrottleMaxFilter.Text = "1";

            InitializeTrainingControls();
            LoadTrainingSettings();
            UpdateTrainingEnvironmentSummary("검사 전");
            UpdateStatusLabels();
            BeginInvoke(new Action(AskFirstUseTutorial));
        }

        // UI 이벤트 처리

        private void BtnOpenDataFolder_Click(object sender, EventArgs e)
        {
            if (!ConfirmUnsavedCleanupBeforeDataChange("새 데이터 폴더 열기")) return;

            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            _ = LoadCatalogAsync(dlg.SelectedPath);
        }

        private void BtnReloadData_Click(object sender, EventArgs e)
        {
            if (!ConfirmUnsavedCleanupBeforeDataChange("현재 데이터 다시 불러오기")) return;

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
                "단계별 가이드 버튼은 다음 안내 기능을 위해 비워두었습니다.\n\n현재 기능별 튜토리얼은 상단 도움말 메뉴에서 볼 수 있습니다.",
                "단계별 가이드", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AskFirstUseTutorial()
        {
            if (HasSeenFirstUseTutorial()) return;

            var answer = MessageBox.Show(
                "처음 사용하시나요?\n\n기능별 튜토리얼을 시작할까요?\n(튜토리얼 중 X를 누르면 스킵됩니다.)",
                "첫 사용자 안내",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            MarkFirstUseTutorialSeen();

            if (answer == DialogResult.Yes)
            {
                RunFeatureTutorial("기능별 튜토리얼");
                return;
            }

            MessageBox.Show(
                "만약 튜토리얼을 다시 보고 싶으면 상단 도움말 메뉴를 눌러주세요.",
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
                string? selectedSection = SelectTutorialSection(title);
                if (string.IsNullOrEmpty(selectedSection)) return;

                foreach (var step in steps.Where(step => step.Section == selectedSection))
                {
                    if (!ShowTutorialStep(title, step)) break;
                }
            }
            finally
            {
                _tutorialRunning = false;
            }
        }

        private string? SelectTutorialSection(string tutorialTitle)
        {
            using var dialog = new Form
            {
                Text = tutorialTitle + " - 섹션 선택",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(430, 230)
            };

            var titleLabel = new System.Windows.Forms.Label
            {
                AutoSize = false,
                Location = new Point(18, 18),
                Size = new Size(394, 46),
                Text = "보고 싶은 튜토리얼 섹션을 선택하세요.\nX를 누르면 튜토리얼을 시작하지 않습니다.",
                Font = new Font("맑은 고딕", 10F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            string? selectedSection = null;
            var sections = new[] { "데이터 보기", "정리", "학습", "그래프" };
            for (int i = 0; i < sections.Length; i++)
            {
                string section = sections[i];
                var button = new Button
                {
                    Text = section,
                    Location = new Point(32 + (i % 2) * 190, 82 + (i / 2) * 58),
                    Size = new Size(168, 38),
                    Tag = section
                };
                button.Click += (_, _) =>
                {
                    selectedSection = section;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                };
                dialog.Controls.Add(button);
            }

            dialog.Controls.Add(titleLabel);
            return dialog.ShowDialog(this) == DialogResult.OK ? selectedSection : null;
        }
        private List<TutorialStep> CreateTutorialSteps()
        {
            return new List<TutorialStep>
            {
                new TutorialStep("데이터 보기", "데이터 폴더 열기", "DonkeyCar tub 또는 mock data 폴더를 선택합니다. 이미지와 catalog_0.catalog를 읽어 프레임 목록을 만듭니다.", btnOpenDataFolder, tabPageDataViewer),
                new TutorialStep("데이터 보기", "다시 불러오기", "현재 선택된 데이터 폴더를 다시 읽습니다. 파일을 추가하거나 catalog를 수정한 뒤 갱신할 때 사용합니다.", btnReloadData, tabPageDataViewer),
                new TutorialStep("데이터 보기", "테마 전환", "화면 색상을 밝은 테마와 어두운 테마로 전환합니다.", btnToggleTheme, tabPageDataViewer),
                new TutorialStep("데이터 보기", "단계별 가이드", "이 튜토리얼을 다시 실행합니다. 기능을 잊었을 때 언제든 다시 누르면 됩니다.", btnGuide, tabPageDataViewer),
                new TutorialStep("데이터 보기", "프레임 목록", "왼쪽 목록에서 프레임을 선택합니다. 제외된 프레임은 다른 색과 [XXXX] 표시로 구분됩니다.", dgvFrameCatalog, tabPageDataViewer),
                new TutorialStep("데이터 보기", "이미지 미리보기", "선택한 프레임의 이미지를 보여줍니다. 비율을 유지하는 Zoom 방식이라 이미지가 왜곡되지 않습니다.", picFramePreview, tabPageDataViewer),
                new TutorialStep("데이터 보기", "처음으로", "현재 필터 결과의 첫 번째 프레임으로 이동합니다.", btnFirst, tabPageDataViewer),
                new TutorialStep("데이터 보기", "이전 프레임", "현재 프레임 바로 앞 프레임으로 이동합니다.", btnPrev, tabPageDataViewer),
                new TutorialStep("데이터 보기", "다음 프레임", "현재 프레임 바로 다음 프레임으로 이동합니다.", btnNext, tabPageDataViewer),
                new TutorialStep("데이터 보기", "마지막으로", "현재 필터 결과의 마지막 프레임으로 이동합니다.", btnLast, tabPageDataViewer),
                new TutorialStep("데이터 보기", "자동 재생", "프레임을 지정한 간격으로 자동 재생합니다. 재생 중에는 버튼이 일시정지로 바뀝니다.", btnAutoPlay, tabPageDataViewer),
                new TutorialStep("데이터 보기", "재생 속도", "자동 재생 속도를 배속으로 조절합니다. 1.00x는 기본 속도이고, 숫자가 클수록 더 빠르게 넘어갑니다.", numPlaybackIntervalMs, tabPageDataViewer),
                new TutorialStep("데이터 보기", "프레임 위치 슬라이더", "현재 프레임 위치를 빠르게 이동합니다. 많은 프레임을 훑어볼 때 사용합니다.", trkFrameTimeline, tabPageDataViewer),
                new TutorialStep("데이터 보기", "조향값", "선택한 프레임의 Angle 값을 표시합니다. 왼쪽/오른쪽 조향 상태를 확인할 때 봅니다.", lblAngleValue, tabPageDataViewer),
                new TutorialStep("데이터 보기", "스로틀값", "선택한 프레임의 Throttle 값을 표시합니다. 전진/정지/후진 정도를 확인할 때 봅니다.", lblThrottleValue, tabPageDataViewer),
                new TutorialStep("데이터 보기", "모드", "선택한 프레임의 주행 모드 정보를 표시합니다. user/local 같은 상태를 확인합니다.", lblModeValue, tabPageDataViewer),
                new TutorialStep("정리", "조향각 범위", "필터에 사용할 조향각 최소값과 최대값을 입력합니다. 예: -1부터 1까지.", txtAngleMinFilter, tabPageDataViewer),
                new TutorialStep("정리", "스로틀 범위", "필터에 사용할 스로틀 최소값과 최대값을 입력합니다. 예: -1부터 1까지.", txtThrottleMinFilter, tabPageDataViewer),
                new TutorialStep("정리", "모드 필터", "user, local 등 특정 주행 모드만 보고 싶을 때 선택합니다.", cmbModeFilter, tabPageDataViewer),
                new TutorialStep("정리", "시나리오 필터", "normal, night, turn 같은 시나리오별로 프레임을 좁혀 봅니다.", cmbScenarioFilter, tabPageDataViewer),
                new TutorialStep("정리", "필터 적용", "입력한 범위와 선택한 모드/시나리오 조건으로 목록을 필터링합니다.", btnApplyFrameFilter, tabPageDataViewer),
                new TutorialStep("정리", "필터 해제", "필터 조건을 풀고 원본 프레임 목록을 다시 표시합니다.", btnClearFrameFilter, tabPageDataViewer),
                new TutorialStep("정리", "구간 선택", "제외하고 싶은 시작/끝 프레임 번호를 입력하는 영역입니다.", txtFrameRangeStart, tabPageDataViewer),
                new TutorialStep("정리", "구간 제외", "선택한 프레임 범위를 Soft Delete 처리합니다. 실제 파일은 삭제하지 않고 학습 제외 표시만 합니다.", btnExcludeFrameRange, tabPageDataViewer),
                new TutorialStep("정리", "선택 프레임 제외", "목록에서 선택한 프레임들을 기준으로 제외 범위를 만들고 Soft Delete 처리합니다.", btnExcludeSelectedFrames, tabPageDataViewer),
                new TutorialStep("정리", "복원", "Soft Delete 처리된 프레임을 모두 다시 사용 가능 상태로 되돌립니다.", btnRestoreFrames, tabPageDataViewer),
                new TutorialStep("정리", "클린 폴더 추출", "제외 표시된 프레임을 빼고 학습에 사용할 수 있는 Clean 폴더를 새로 만듭니다. 원본 폴더는 변경하지 않습니다.", btnExportCleanDataset, tabPageDataViewer),
                new TutorialStep("그래프", "그래프/통계", "필터와 제외 상태를 반영한 조향값/스로틀값 분포 그래프를 확인합니다.", tabControlMain, tabGraphStats),
                new TutorialStep("학습", "학습 설정", "Python 환경, DonkeyCar 작업 폴더, 학습 데이터 폴더, 모델 저장 경로와 학습 횟수를 확인합니다.", grpTrainingConfig, tabTrainingMonitor),
                new TutorialStep("학습", "학습 로그", "학습 실행 과정의 로그를 표시할 영역입니다.", grpTrainingOutput, tabTrainingMonitor)
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

            if (_visibleFrames.Count == 0)
            {
                MessageBox.Show("현재 기준으로 자동 검토 후보가 없습니다.",
                    "이상 후보 없음", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isFrameFilterActive = true;
            _isChartDirty = true;
            RefreshFrameView();
            SetIndex(0);

            MessageBox.Show(
                $"검토 후보 {_visibleFrames.Count}개만 표시합니다.\n\n" +
                "후보 기준: 급조향, 정지에 가까운 속도값, out_of_bound 상황, 카탈로그 누락/값 없음",
                "이상 후보 보기", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 선택한 구간을 Soft Delete 처리합니다.
        private void BtnExcludeFrameRange_Click(object? sender, EventArgs e)
        {
            if (!TryReadSelectedRange(out int from, out int to))
            {
                var fallbackConfirm = MessageBox.Show(
                    "구간 정보가 없습니다. 현재 선택된 프레임을 제외할까요?",
                    "구간 정보 없음", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (fallbackConfirm == DialogResult.Yes)
                    BtnExcludeSelectedFrames_Click(sender, e);
                return;
            }

            var confirm = MessageBox.Show(
                $"원본 인덱스 {from} ~ {to} 구간을 제외하시겠습니까?",
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
                HeaderText = "이미지 파일",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 220
            });
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.Angle),
                HeaderText = "방향값",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.000" }
            });
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.Throttle),
                HeaderText = "속도값",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.000" }
            });
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.Mode),
                HeaderText = "주행 방식",
                Width = 90
            });
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.Scenario),
                HeaderText = "상황",
                Width = 110
            });
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.StateText),
                HeaderText = "상태",
                Width = 80
            });

            dgvFrameCatalog.RowTemplate.Height = 26;
            dgvFrameCatalog.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            dgvFrameCatalog.DataBindingComplete += (_, _) => ApplyFrameCatalogRowStyle();
        }

        /// <summary>
        /// 데이터 정제 흐름이 눈에 띄도록 버튼명, 구간 입력 영역, 상태 색상을 정리합니다.
        /// </summary>
        private void ApplyDataManagerUiStyle()
        {
            tabPageDataViewer.Text = "데이터 확인";
            tabGraphStats.Text = "그래프/통계";
            grpDataExplorer.Text = "데이터 탐색";
            grpDataCleaner.Text = "데이터 정리 - 검색 / 제외 / 복원 / 학습용 폴더 만들기";
            lblFrameRange.Text = "구간 제외/복원";
            lblAngleRange.Text = "방향값 범위 (-1~1):";
            lblThrottleRange.Text = "속도값 범위 (-1~1):";
            lblModeFilter.Text = "주행 방식:";
            lblScenarioFilter.Text = "상황:";
            btnOpenDataFolder.Text = "데이터 폴더 열기";
            btnApplyFrameFilter.Text = "검색 적용";
            btnClearFrameFilter.Text = "검색 해제";
            btnExcludeFrameRange.Text = "구간 제외";
            btnExcludeSelectedFrames.Text = "선택 제외";
            btnRestoreFrames.Text = "복원";
            btnSaveCleanupState.Text = "상태 저장";
            btnExportCleanDataset.Text = "학습용 폴더 만들기";
            lblPlayInterval.Text = "재생속도";

            // 기존 컨트롤 이름은 numPlaybackIntervalMs이지만, 화면에서는 배속 입력으로 사용합니다.
            // 1.00x를 기준으로 내부 재생 간격을 계산해 사용자가 ms 값을 몰라도 조절할 수 있게 합니다.
            numPlaybackIntervalMs.DecimalPlaces = 2;
            numPlaybackIntervalMs.Increment = 0.25M;
            numPlaybackIntervalMs.Minimum = 0.25M;
            numPlaybackIntervalMs.Maximum = 10.00M;
            numPlaybackIntervalMs.Value = 1.00M;
            numPlaybackIntervalMs.ValueChanged += (_, _) =>
            {
                if (_isPlaybackRunning)
                    _playbackTimer.Interval = GetPlaybackIntervalFromSpeed();
            };

            foreach (var button in new[]
            {
                btnApplyFrameFilter, btnClearFrameFilter, btnExcludeFrameRange, btnExcludeSelectedFrames,
                btnRestoreFrames, btnSaveCleanupState, btnExportCleanDataset
            })
            {
                button.UseVisualStyleBackColor = false;
            }

            btnApplyFrameFilter.BackColor = System.Drawing.Color.FromArgb(229, 241, 255);
            btnClearFrameFilter.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            btnExcludeFrameRange.BackColor = System.Drawing.Color.FromArgb(255, 232, 214);
            btnExcludeSelectedFrames.BackColor = System.Drawing.Color.FromArgb(255, 244, 230);
            btnRestoreFrames.BackColor = System.Drawing.Color.FromArgb(224, 247, 235);
            btnSaveCleanupState.BackColor = System.Drawing.Color.FromArgb(235, 239, 255);
            btnExportCleanDataset.BackColor = System.Drawing.Color.FromArgb(255, 238, 238);

            txtFrameRangeStart.BackColor = System.Drawing.Color.FromArgb(255, 252, 235);
            txtFrameRangeEnd.BackColor = System.Drawing.Color.FromArgb(255, 252, 235);
            lblFrameRange.ForeColor = System.Drawing.Color.FromArgb(120, 70, 0);

            ArrangeFrameInfoPanel();
        }

        /// <summary>
        /// 오른쪽 프레임 정보 영역을 촘촘하게 재배치합니다.
        /// 검토 힌트와 재생속도 입력이 겹치지 않도록 런타임에서 위치를 정리합니다.
        /// </summary>
        private void ArrangeFrameInfoPanel()
        {
            int left = lblFrameValue.Left;
            int width = lblFrameValue.Width;

            lblFrameValue.Font = new Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            lblAngleValue.Font = new Font("맑은 고딕", 10.5F);
            lblThrottleValue.Font = new Font("맑은 고딕", 10.5F);
            lblModeValue.Font = new Font("맑은 고딕", 10.5F);
            lblPlayInterval.Font = new Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);

            lblFrameValue.Location = new Point(left, 58);
            lblFrameValue.Size = new Size(width, 32);
            lblAngleValue.Location = new Point(left, 212);
            lblAngleValue.Size = new Size(width, 31);
            lblThrottleValue.Location = new Point(left, 245);
            lblThrottleValue.Size = new Size(width, 42);
            lblModeValue.Location = new Point(left, 289);
            lblModeValue.Size = new Size(width, 30);

            lblPlayInterval.Location = new Point(left, 390);
            lblPlayInterval.Size = new Size(92, 28);
            numPlaybackIntervalMs.Location = new Point(left + 96, 389);
            numPlaybackIntervalMs.Size = new Size(width - 96, 27);
        }

        /// <summary>
        /// 이상 후보 확인 UI는 Designer 파일을 수정하지 않고 코드에서 추가합니다.
        /// 버튼은 검색 버튼 근처에 두고, 힌트 라벨은 현재 프레임 정보 영역에 표시합니다.
        /// </summary>
        private void ConfigureReviewCandidateControls()
        {
            if (_btnShowReviewCandidates == null)
            {
                _btnShowReviewCandidates = new Button
                {
                    Name = "btnShowReviewCandidates",
                    Text = "이상 후보 보기",
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new Point(btnApplyFrameFilter.Left, 29),
                    Size = btnApplyFrameFilter.Size,
                    UseVisualStyleBackColor = false,
                    BackColor = System.Drawing.Color.FromArgb(255, 249, 219)
                };
                _btnShowReviewCandidates.Click += BtnShowReviewCandidates_Click;
                grpDataCleaner.Controls.Add(_btnShowReviewCandidates);
                _btnShowReviewCandidates.BringToFront();
            }

            if (_lblFrameReviewHint == null)
            {
                _lblFrameReviewHint = new System.Windows.Forms.Label
                {
                    Name = "lblFrameReviewHint",
                    Text = "검토: -",
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new Point(lblModeValue.Left, 326),
                    Size = new Size(lblModeValue.Width, 54),
                    Font = new Font("맑은 고딕", 9F),
                    ForeColor = System.Drawing.Color.DimGray
                };
                grpDataExplorer.Controls.Add(_lblFrameReviewHint);
                _lblFrameReviewHint.BringToFront();
            }
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
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 249, 196);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(80, 60, 0);
                    row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(255, 214, 102);
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
            btnClearFrameFilter.Enabled          = !loading;
            if (_btnShowReviewCandidates != null)
                _btnShowReviewCandidates.Enabled = !loading;
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

            lblFrameValue.Text    = $"프레임: {idx + 1} / {_visibleFrames.Count}";
            lblAngleValue.Text    = $"방향값: {frame.Angle:0.000} ({frame.SteeringText})";
            lblThrottleValue.Text = $"속도값: {frame.Throttle:0.000} ({frame.ThrottleText})";
            lblModeValue.Text     = $"주행 방식: {frame.ModeDescription}";
            if (_lblFrameReviewHint != null)
            {
                _lblFrameReviewHint.Text = $"검토: {frame.ReviewHint}";
                _lblFrameReviewHint.ForeColor = frame.NeedsReview
                    ? System.Drawing.Color.FromArgb(150, 95, 0)
                    : System.Drawing.Color.DimGray;
            }
            UpdateStatusLabels();
            BeginInvoke(new Action(AskFirstUseTutorial));
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

        private void UpdatePreviewImage(string path, FrameData? frame = null)
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

        /// <summary>
        /// 미리보기 이미지 위에 작은 주행 방향 화살표와 상태 텍스트를 그립니다.
        /// 사용자가 사진과 조향값이 서로 맞는지 빠르게 비교할 수 있게 하는 보조 표시입니다.
        /// </summary>
        private void DrawDriveOverlay(Bitmap bitmap, FrameData frame)
        {
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            float scale = Math.Max(0.4f, bitmap.Width / 640f);
            float arrowLength = Math.Max(12f, bitmap.Height * 0.075f);
            float arrowWidth = Math.Max(5f, bitmap.Width * 0.010f);
            float centerX = bitmap.Width / 2f;
            float centerY = bitmap.Height - Math.Max(32f, bitmap.Height * 0.16f);
            float directionOffset = (float)Math.Max(-1.0, Math.Min(1.0, frame.Angle)) * arrowLength * 0.7f;

            PointF tip = new PointF(centerX + directionOffset, centerY - arrowLength);
            PointF baseLeft = new PointF(centerX - arrowWidth, centerY);
            PointF baseRight = new PointF(centerX + arrowWidth, centerY);

            using var arrowBrush = new SolidBrush(System.Drawing.Color.FromArgb(185, 255, 193, 7));
            using var arrowPen = new Pen(System.Drawing.Color.FromArgb(210, 60, 45, 0), Math.Max(1f, 1.2f * scale));
            PointF[] arrow = { tip, baseRight, new PointF(centerX + arrowWidth * 0.35f, centerY), new PointF(centerX + arrowWidth * 0.35f, centerY + arrowLength * 0.45f), new PointF(centerX - arrowWidth * 0.35f, centerY + arrowLength * 0.45f), new PointF(centerX - arrowWidth * 0.35f, centerY), baseLeft };
            graphics.FillPolygon(arrowBrush, arrow);
            graphics.DrawPolygon(arrowPen, arrow);

            string overlayText = $"{frame.SteeringText} / {frame.ThrottleText}";
            if (frame.NeedsReview)
                overlayText += " / 검토";

            using var font = new System.Drawing.Font("맑은 고딕", Math.Max(5.2f, 5.6f * scale), System.Drawing.FontStyle.Bold);
            SizeF textSize = graphics.MeasureString(overlayText, font);
            float boxWidth = textSize.Width + 12;
            float boxHeight = textSize.Height + 7;
            var textBox = new RectangleF((bitmap.Width - boxWidth) / 2f, 6, boxWidth, boxHeight);
            using var boxBrush = new SolidBrush(System.Drawing.Color.FromArgb(165, 0, 0, 0));
            using var outlineBrush = new SolidBrush(System.Drawing.Color.FromArgb(180, 40, 30, 0));
            using var textBrush = new SolidBrush(frame.NeedsReview ? System.Drawing.Color.Gold : System.Drawing.Color.White);
            graphics.FillRectangle(boxBrush, textBox);
            float textX = textBox.Left + 6;
            float textY = textBox.Top + 3;
            graphics.DrawString(overlayText, font, outlineBrush, textX + 0.8f, textY + 0.8f);
            graphics.DrawString(overlayText, font, textBrush, textX, textY);
        }

        private int GetPlaybackIntervalFromSpeed()
        {
            decimal speed = Math.Max(0.25M, numPlaybackIntervalMs.Value);
            const int baseIntervalMs = 200;
            return Math.Max(20, (int)Math.Round(baseIntervalMs / speed));
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
                _playbackTimer.Interval = GetPlaybackIntervalFromSpeed();
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
            if (_allFrames == null || _allFrames.Count == 0) return;

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
            else ClearPreviewSelection();
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
                MessageBox.Show("조향각과 스로틀 범위는 숫자로 입력해 주세요.",
                    "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (angleMin > angleMax || throttleMin > throttleMax)
            {
                MessageBox.Show("범위의 최소값은 최대값보다 클 수 없습니다.",
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
                return false;

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
        }

        private void FinishDataStateChange()
        {
            txtFrameRangeStart.Clear();
            txtFrameRangeEnd.Clear();

            MarkCleanupStateChanged();
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
            lblFrameValue.Text = "프레임: 0 / 0";
            lblAngleValue.Text = "조향값: 0.000";
            lblThrottleValue.Text = "스로틀값: 0.000";
            lblModeValue.Text = "모드: -";
            if (_lblFrameReviewHint != null)
                _lblFrameReviewHint.Text = "검토: -";
        }

        private void BtnSaveCleanupState_Click(object? sender, EventArgs e)
        {
            SaveCleanupState(showSuccessMessage: true);
        }

        private bool SaveCleanupState(bool showSuccessMessage)
        {
            if (!TryEnsureLoadedFolder()) return false;

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
                UpdateCleanupSaveUi();
                if (showSuccessMessage)
                {
                    MessageBox.Show("제외 상태가 저장되었습니다.",
                        "상태 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("제외 상태 저장 중 오류가 발생했습니다.\n" + ex.Message,
                    "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void BtnExportCleanDataset_Click(object? sender, EventArgs e)
        {
            if (!TryEnsureLoadedFolder()) return;

            var confirm = MessageBox.Show(
                "현재 폴더를 복사한 뒤 제외된 프레임을 제거한 학습용 Clean 폴더를 생성합니다.\n원본 폴더는 변경하지 않습니다.\n\n계속하시겠습니까?",
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

                var useForTraining = MessageBox.Show(
                    "방금 만든 Clean 폴더를 학습 데이터 폴더로 사용할까요?\n\n" +
                    "예를 누르면 학습 실행 탭으로 이동하고 학습 데이터 경로가 자동으로 입력됩니다.",
                    "학습 경로 연결",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (useForTraining == DialogResult.Yes)
                {
                    txtTrainingTubPath.Text = cleanFolder;
                    tabControlMain.SelectedTab = tabTrainingMonitor;
                    UpdateTrainingActionHint("Clean 폴더가 학습 데이터 경로로 설정되었습니다. 이제 환경 검사를 누르세요.");
                    _ = RefreshTrainingEnvironmentAsync(showSuccessMessage: false);
                }
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
                MessageBox.Show("먼저 DonkeyCar 데이터 폴더를 열어 주세요.",
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

            // 복원 후 전체 프레임을 다시 표시합니다.
            _visibleFrames = _allFrames;
            MarkCleanupStateChanged();
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
            int review  = _allFrames?.Count(f => !f.IsDeleted && f.NeedsReview) ?? 0;
            int valid   = Math.Max(0, total - deleted);
            int shown   = _visibleFrames?.Count ?? 0;
            string filterState = _isFrameFilterActive ? "필터 적용" : "전체 보기";
            string saveState = _hasUnsavedCleanupChanges ? "저장 필요" : "저장됨";

            stsFrameSummary.Text =
                $"{filterState}  |  전체: {total}  |  유효: {valid}  |  제외: {deleted}  |  후보: {review}  |  표시: {shown}  |  {saveState}";

            UpdateCleanupSaveUi();
        }

        private void MarkCleanupStateChanged()
        {
            _hasUnsavedCleanupChanges = true;
            UpdateCleanupSaveUi();
        }

        private void UpdateCleanupSaveUi()
        {
            if (btnSaveCleanupState == null) return;

            btnSaveCleanupState.BackColor = _hasUnsavedCleanupChanges
                ? System.Drawing.Color.FromArgb(255, 235, 180)
                : System.Drawing.Color.FromArgb(235, 239, 255);

            btnSaveCleanupState.Font = new Font(
                btnSaveCleanupState.Font,
                _hasUnsavedCleanupChanges ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);

            Text = _hasUnsavedCleanupChanges
                ? "Data Manager *"
                : "Data Manager";
        }

        private bool ConfirmUnsavedCleanupBeforeDataChange(string actionName)
        {
            if (!_hasUnsavedCleanupChanges) return true;

            var answer = MessageBox.Show(
                $"저장하지 않은 제외/복원 변경사항이 있습니다.\n\n'{actionName}' 전에 현재 제외 상태를 저장할까요?",
                "저장되지 않은 변경사항",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (answer == DialogResult.Cancel) return false;
            if (answer == DialogResult.Yes) return SaveCleanupState(showSuccessMessage: false);
            return true;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!_hasUnsavedCleanupChanges) return;

            var answer = MessageBox.Show(
                "저장하지 않은 제외/복원 변경사항이 있습니다.\n\n종료 전에 현재 제외 상태를 저장할까요?",
                "저장되지 않은 변경사항",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (answer == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (answer == DialogResult.Yes && !SaveCleanupState(showSuccessMessage: false))
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

            /// <summary>
            /// 조향값을 초보자도 이해할 수 있는 말로 바꿔 보여줍니다.
            /// </summary>
            public string SteeringText
            {
                get
                {
                    if (Angle <= -0.7) return "강한 좌회전";
                    if (Angle < -0.1) return "좌회전";
                    if (Angle >= 0.7) return "강한 우회전";
                    if (Angle > 0.1) return "우회전";
                    return "직진";
                }
            }

            /// <summary>
            /// 속도값을 초보자도 이해할 수 있는 말로 바꿔 보여줍니다.
            /// </summary>
            public string ThrottleText
            {
                get
                {
                    if (Throttle < -0.05) return "후진";
                    if (Throttle <= 0.05) return "정지에 가까움";
                    return "전진";
                }
            }

            public string ModeDescription => NormalizeDisplayValue(Mode) switch
            {
                "user" => "사용자 조작",
                "local" => "AI 주행",
                "local_angle" => "AI 조향",
                _ => string.IsNullOrWhiteSpace(Mode) || Mode == "-" ? "-" : Mode
            };

            public string ScenarioDescription => NormalizeDisplayValue(Scenario) switch
            {
                "normal" => "정상",
                "night" => "야간",
                "left_turn" => "좌회전 구간",
                "right_turn" => "우회전 구간",
                "out_of_bound" => "차선 이탈",
                _ => string.IsNullOrWhiteSpace(Scenario) || Scenario == "-" ? "-" : Scenario
            };

            /// <summary>
            /// 사람이 먼저 확인하면 좋은 프레임입니다.
            /// 완전 자동 삭제가 아니라 "검토 후보"로만 표시해서 사용자가 최종 판단하게 합니다.
            /// </summary>
            public bool NeedsReview =>
                IsCatalogMissing ||
                HasNoData ||
                Math.Abs(Angle) >= 0.85 ||
                Math.Abs(Throttle) <= 0.03 ||
                NormalizeDisplayValue(Scenario).Contains("out_of_bound");

            public string ReviewHint
            {
                get
                {
                    var reasons = new List<string>();
                    if (IsCatalogMissing) reasons.Add("카탈로그 없음");
                    if (HasNoData) reasons.Add("데이터 없음");
                    if (Math.Abs(Angle) >= 0.85) reasons.Add("급조향");
                    if (Math.Abs(Throttle) <= 0.03) reasons.Add("정지에 가까운 속도");
                    if (NormalizeDisplayValue(Scenario).Contains("out_of_bound")) reasons.Add("차선 이탈 상황");

                    return reasons.Count == 0 ? "정상" : string.Join(", ", reasons);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터 로드 중 오류: " + ex.Message,
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        // 학습 실행 기능

        /// <summary>
        /// 학습 실행 화면을 초기화합니다.
        /// 사용자가 매번 환경 값을 만지지 않아도 되도록 로컬 기본 경로와 자동 감지 값을 먼저 넣습니다.
        /// </summary>
        private void InitializeTrainingControls()
        {
            _trainingModelType = "linear";
            _trainingPythonEnvName = DetectLocalWslDonkeyEnvironment() ?? "e2e_env";
            _trainingMycarPath = DetectLocalWslDonkeyProjectPath() ?? "~/mycar";
            txtTrainingTubPath.Text = GetDefaultTrainingTubPath();
            txtTrainingModelPath.Text = GetDefaultTrainingModelPath();

            numTrainingEpochs.Minimum = 1;
            numTrainingEpochs.Maximum = 10000;
            if (numTrainingEpochs.Value < 1)
                numTrainingEpochs.Value = 1;

            SimplifyTrainingUi();
            btnStartTrainingProcess.Enabled = false;
            btnStopTrainingProcess.Enabled = false;
            stsTrainingStatus.Text = "학습 상태: 대기";

            txtTrainingTubPath.TextChanged += (_, _) => UpdateTrainingEnvironmentSummary(
                _isTrainingEnvironmentReady ? "정상" : "검사 필요");
            txtTrainingModelPath.TextChanged += (_, _) =>
            {
                UpdateTrainingEnvironmentSummary(_isTrainingEnvironmentReady ? "정상" : "검사 필요");
                UpdateTrainingSummary(modelPath: txtTrainingModelPath.Text.Trim());
            };
        }

        private string GetDefaultTrainingTubPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "data");
        }

        private string GetDefaultTrainingModelPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "donkeycar_models",
                "pilot.h5");
        }

        private string GetLegacyDesktopTrainingModelPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "study", "pilot.keras");
        }

        private string GetLegacyDocumentsTrainingModelPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "pilot.keras");
        }

        private string GetLegacyDocumentsModelFolderKerasPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "donkeycar_models",
                "pilot.keras");
        }

        private void ConfigureTrainingModelPathButton()
        {
            if (_btnSelectTrainingModelPath != null) return;

            _btnSelectTrainingModelPath = new Button
            {
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Font = new Font("맑은 고딕", 12F),
                    Location = new Point(1335, 156),
                    Name = "btnSelectTrainingModelPath",
                    Size = new Size(145, 43),
                    Text = "저장 위치",
                UseVisualStyleBackColor = true
            };
            _btnSelectTrainingModelPath.Click += BtnSelectTrainingModelPath_Click;
            grpTrainingConfig.Controls.Add(_btnSelectTrainingModelPath);
            _btnSelectTrainingModelPath.BringToFront();
        }

        private void ConfigureTrainingResultButtons()
        {
            if (_btnOpenTrainingModelFolder == null)
            {
                _btnOpenTrainingModelFolder = new Button
                {
                    Font = new Font("맑은 고딕", 12F),
                    Location = new Point(699, 292),
                    Name = "btnOpenTrainingModelFolder",
                    Size = new Size(194, 43),
                    Text = "모델 폴더 열기",
                    UseVisualStyleBackColor = true
                };
                _btnOpenTrainingModelFolder.Click += BtnOpenTrainingModelFolder_Click;
                grpTrainingConfig.Controls.Add(_btnOpenTrainingModelFolder);
            }

            if (_btnCopyModelUseCommand == null)
            {
                _btnCopyModelUseCommand = new Button
                {
                    Font = new Font("맑은 고딕", 12F),
                    Location = new Point(939, 292),
                    Name = "btnCopyModelUseCommand",
                    Size = new Size(220, 43),
                    Text = "사용 명령 복사",
                    UseVisualStyleBackColor = true
                };
                _btnCopyModelUseCommand.Click += BtnCopyModelUseCommand_Click;
                grpTrainingConfig.Controls.Add(_btnCopyModelUseCommand);
            }

            _btnOpenTrainingModelFolder.BringToFront();
            _btnCopyModelUseCommand.BringToFront();
        }

        private void ConfigureTrainingEnvironmentControls()
        {
            ConfigureTrainingGuideLabels();

            if (_lblTrainingEnvironmentStatus == null)
            {
                _lblTrainingEnvironmentStatus = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    Font = new Font("맑은 고딕", 10F),
                    Location = new Point(45, 398),
                    Name = "lblTrainingEnvironmentStatus",
                    Size = new Size(1450, 24),
                    Text = "학습 환경: 검사 전"
                };
                grpTrainingConfig.Controls.Add(_lblTrainingEnvironmentStatus);
            }

            if (_btnCheckTrainingEnvironment == null)
            {
                _btnCheckTrainingEnvironment = new Button
                {
                    Font = new Font("맑은 고딕", 10F),
                    Location = new Point(219, 350),
                    Name = "btnCheckTrainingEnvironment",
                    Size = new Size(170, 36),
                    Text = "환경 검사",
                    UseVisualStyleBackColor = true
                };
                _btnCheckTrainingEnvironment.Click += async (_, _) => await RefreshTrainingEnvironmentAsync(showSuccessMessage: true);
                grpTrainingConfig.Controls.Add(_btnCheckTrainingEnvironment);
            }

            if (_btnAutoSetupTrainingEnvironment == null)
            {
                _btnAutoSetupTrainingEnvironment = new Button
                {
                    Font = new Font("맑은 고딕", 10F),
                    Location = new Point(404, 350),
                    Name = "btnAutoSetupTrainingEnvironment",
                    Size = new Size(170, 36),
                    Text = "자동 설정 시도",
                    UseVisualStyleBackColor = true
                };
                _btnAutoSetupTrainingEnvironment.Click += async (_, _) => await TryAutoSetupTrainingEnvironmentAsync();
                grpTrainingConfig.Controls.Add(_btnAutoSetupTrainingEnvironment);
            }

            if (_btnCopyTrainingSetupCommands == null)
            {
                _btnCopyTrainingSetupCommands = new Button
                {
                    Font = new Font("맑은 고딕", 10F),
                    Location = new Point(589, 350),
                    Name = "btnCopyTrainingSetupCommands",
                    Size = new Size(170, 36),
                    Text = "설치 명령 복사",
                    UseVisualStyleBackColor = true
                };
                _btnCopyTrainingSetupCommands.Click += (_, _) => CopyTrainingSetupCommands();
                grpTrainingConfig.Controls.Add(_btnCopyTrainingSetupCommands);
            }

            if (_btnEditTrainingEnvironment == null)
            {
                _btnEditTrainingEnvironment = new Button
                {
                    Font = new Font("맑은 고딕", 10F),
                    Location = new Point(774, 350),
                    Name = "btnEditTrainingEnvironment",
                    Size = new Size(170, 36),
                    Text = "환경 설정",
                    UseVisualStyleBackColor = true
                };
                _btnEditTrainingEnvironment.Click += (_, _) => EditTrainingEnvironmentOverride();
                grpTrainingConfig.Controls.Add(_btnEditTrainingEnvironment);
            }

            _lblTrainingEnvironmentStatus.BringToFront();
            _btnCheckTrainingEnvironment.BringToFront();
            _btnAutoSetupTrainingEnvironment.BringToFront();
            _btnCopyTrainingSetupCommands.BringToFront();
            _btnEditTrainingEnvironment.BringToFront();
            _lblTrainingModelSafetyNote?.BringToFront();
            ConfigureTrainingToolTips();
        }

        private void ConfigureTrainingToolTips()
        {
            _trainingToolTip ??= new ToolTip
            {
                AutoPopDelay = 8000,
                InitialDelay = 400,
                ReshowDelay = 200,
                ShowAlways = true
            };

            _trainingToolTip.SetToolTip(btnSelectTrainingTubPath, "1단계: 학습에 사용할 DonkeyCar 데이터 폴더를 선택합니다.");
            if (_btnCheckTrainingEnvironment != null)
                _trainingToolTip.SetToolTip(_btnCheckTrainingEnvironment, "2단계: 현재 PC에서 WSL, Python 환경, DonkeyCar, 학습 데이터 경로가 준비됐는지 확인합니다.");
            if (_btnAutoSetupTrainingEnvironment != null)
                _trainingToolTip.SetToolTip(_btnAutoSetupTrainingEnvironment, "부족한 환경을 프로그램이 가능한 범위에서 자동으로 설치하거나 생성합니다.");
            if (_btnCopyTrainingSetupCommands != null)
                _trainingToolTip.SetToolTip(_btnCopyTrainingSetupCommands, "자동 설정이 실패할 때 PowerShell에 붙여넣을 설치 명령을 복사합니다.");
            if (_btnEditTrainingEnvironment != null)
                _trainingToolTip.SetToolTip(_btnEditTrainingEnvironment, "친구 PC처럼 Python 환경명이나 DonkeyCar 작업 폴더명이 다를 때 직접 입력합니다.");
            _trainingToolTip.SetToolTip(btnStartTrainingProcess, "3단계: 환경 검사가 정상일 때 모델 학습을 시작합니다.");
            _trainingToolTip.SetToolTip(btnStopTrainingProcess, "학습을 중지합니다. 기존 최종 모델은 유지되고 임시 학습 파일만 중단됩니다.");
        }

        private void ConfigureTrainingGuideLabels()
        {
            if (_lblTrainingStepGuide == null)
            {
                _lblTrainingStepGuide = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    Font = new Font("맑은 고딕", 11F, System.Drawing.FontStyle.Bold),
                    Location = new Point(45, 24),
                    Name = "lblTrainingStepGuide",
                    Size = new Size(1180, 26),
                    Text = "사용 순서: 1. 학습 데이터 선택 -> 2. 환경 검사 -> 3. 학습 시작"
                };
                grpTrainingConfig.Controls.Add(_lblTrainingStepGuide);
            }

            if (_lblTrainingEnvironmentSummary == null)
            {
                _lblTrainingEnvironmentSummary = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    Font = new Font("맑은 고딕", 10F),
                    Location = new Point(45, 54),
                    Name = "lblTrainingEnvironmentSummary",
                    Size = new Size(1450, 24),
                    Text = "현재 설정: 확인 전"
                };
                grpTrainingConfig.Controls.Add(_lblTrainingEnvironmentSummary);
            }

            if (_lblTrainingActionHint == null)
            {
                _lblTrainingActionHint = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    Font = new Font("맑은 고딕", 10F),
                    ForeColor = System.Drawing.Color.DimGray,
                    Location = new Point(45, 82),
                    Name = "lblTrainingActionHint",
                    Size = new Size(1450, 24),
                    Text = "먼저 학습 데이터 폴더를 선택한 뒤 환경 검사를 누르세요."
                };
                grpTrainingConfig.Controls.Add(_lblTrainingActionHint);
            }

            if (_lblTrainingModelSafetyNote == null)
            {
                _lblTrainingModelSafetyNote = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    Font = new Font("맑은 고딕", 9.5F),
                    ForeColor = System.Drawing.Color.DimGray,
                    Location = new Point(45, 426),
                    Name = "lblTrainingModelSafetyNote",
                    Size = new Size(1450, 24),
                    Text = "학습 중에는 임시 모델 파일을 사용합니다. 학습 중지를 눌러도 기존 최종 모델은 유지됩니다."
                };
                grpTrainingConfig.Controls.Add(_lblTrainingModelSafetyNote);
            }

            _lblTrainingStepGuide.BringToFront();
            _lblTrainingEnvironmentSummary.BringToFront();
            _lblTrainingActionHint.BringToFront();
        }

        private void ConfigureTrainingSummaryLabels()
        {
            _lblTrainingSummaryStatus ??= CreateTrainingSummaryLabel("lblTrainingSummaryStatus", "상태: 대기", 459, 216, 170);
            _lblTrainingSummaryEpoch ??= CreateTrainingSummaryLabel("lblTrainingSummaryEpoch", "Epoch: -", 639, 216, 150);
            _lblTrainingSummaryProgress ??= CreateTrainingSummaryLabel("lblTrainingSummaryProgress", "진행률: -", 799, 216, 170);
            _lblTrainingSummaryLoss ??= CreateTrainingSummaryLabel("lblTrainingSummaryLoss", "loss: -", 979, 216, 150);
            _lblTrainingSummaryModelPath ??= CreateTrainingSummaryLabel("lblTrainingSummaryModelPath", "저장 위치: -", 459, 244, 1000);

            _lblTrainingSummaryStatus.BringToFront();
            _lblTrainingSummaryEpoch.BringToFront();
            _lblTrainingSummaryProgress.BringToFront();
            _lblTrainingSummaryLoss.BringToFront();
            _lblTrainingSummaryModelPath.BringToFront();
        }

        private System.Windows.Forms.Label CreateTrainingSummaryLabel(string name, string text, int x, int y, int width)
        {
            var label = new System.Windows.Forms.Label
            {
                AutoSize = false,
                Font = new Font("맑은 고딕", 9.5F),
                ForeColor = System.Drawing.Color.DimGray,
                Location = new Point(x, y),
                Name = name,
                Size = new Size(width, 24),
                Text = text
            };
            grpTrainingConfig.Controls.Add(label);
            return label;
        }

        private void ResetTrainingSummary(string modelPath)
        {
            UpdateTrainingSummary(status: "대기", epoch: "-", progress: "-", loss: "-", modelPath: modelPath);
        }

        private void UpdateTrainingSummary(
            string? status = null,
            string? epoch = null,
            string? progress = null,
            string? loss = null,
            string? modelPath = null)
        {
            if (_lblTrainingSummaryStatus != null && status != null)
                _lblTrainingSummaryStatus.Text = "상태: " + status;
            if (_lblTrainingSummaryEpoch != null && epoch != null)
                _lblTrainingSummaryEpoch.Text = "Epoch: " + epoch;
            if (_lblTrainingSummaryProgress != null && progress != null)
                _lblTrainingSummaryProgress.Text = "진행률: " + progress;
            if (_lblTrainingSummaryLoss != null && loss != null)
                _lblTrainingSummaryLoss.Text = "loss: " + loss;
            if (_lblTrainingSummaryModelPath != null && modelPath != null)
                _lblTrainingSummaryModelPath.Text = "저장 위치: " + modelPath;
        }

        /// <summary>
        /// 발표/일반 사용자 기준으로 꼭 필요한 학습 입력만 남깁니다.
        /// mycar 경로, 모델 종류, Python 환경명은 자동 기본값을 사용합니다.
        /// </summary>
        private void SimplifyTrainingUi()
        {
            btnSaveTrainingConfig.Visible = false;

            lblTrainingTubPath.Text = "학습 데이터 경로";
            lblTrainingTubPath.Location = new Point(45, 116);
            txtTrainingTubPath.Location = new Point(219, 112);
            btnSelectTrainingTubPath.Text = "데이터 선택";
            btnSelectTrainingTubPath.Size = new Size(145, 43);
            btnSelectTrainingTubPath.Location = new Point(1335, 104);

            lblTrainingModelPath.Location = new Point(45, 168);
            txtTrainingModelPath.Location = new Point(219, 164);
            txtTrainingModelPath.Width = Math.Max(300, btnSelectTrainingTubPath.Left - txtTrainingModelPath.Left - 24);
            ConfigureTrainingModelPathButton();

            lblEpoch.Text = "학습 횟수";
            lblEpoch.Location = new Point(45, 220);
            numTrainingEpochs.Location = new Point(219, 216);
            numTrainingEpochs.Size = new Size(180, 34);
            ConfigureTrainingSummaryLabels();
            ResetTrainingSummary(txtTrainingModelPath.Text.Trim());

            btnStartTrainingProcess.Location = new Point(219, 292);
            btnStopTrainingProcess.Location = new Point(459, 292);
            ConfigureTrainingResultButtons();
            ConfigureTrainingEnvironmentControls();
            grpTrainingConfig.Height = 470;
            grpTrainingOutput.Location = new Point(15, 500);
            grpTrainingOutput.Height = Math.Max(220, tabTrainingMonitor.Height - 540);
        }

        private async void BtnStartTrainingProcess_Click(object? sender, EventArgs e)
        {
            if (_trainingProcess is { HasExited: false })
            {
                MessageBox.Show("이미 학습 프로세스가 실행 중입니다.",
                    "학습 실행", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TrainingEnvironmentCheck environmentCheck = await RefreshTrainingEnvironmentAsync(showSuccessMessage: false);
            if (!environmentCheck.IsReady)
            {
                string nextAction = BuildTrainingActionHint(environmentCheck);
                MessageBox.Show(
                    "학습을 시작할 수 없습니다.\n\n" +
                    environmentCheck.StatusMessage + "\n\n" +
                    nextAction + "\n\n" +
                    "버튼 설명:\n" +
                    "- 환경 검사: 현재 PC 설정을 다시 확인합니다.\n" +
                    "- 자동 설정 시도: 가능한 항목을 프로그램이 설치/생성합니다.\n" +
                    "- 설치 명령 복사: 자동 설정이 실패할 때 PowerShell에 붙여넣을 명령을 복사합니다.\n" +
                    "- 환경 설정: Python 환경명이나 DonkeyCar 작업 폴더가 다른 PC에서 직접 지정합니다.",
                    "학습 환경 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryBuildTrainingCommand(out TrainingCommand command)) return;
            if (!ConfirmOriginalTrainingDataIfNeeded(command.TubPath)) return;
            SaveTrainingSettingsSilently();

            btnStartTrainingProcess.Enabled = false;
            btnStopTrainingProcess.Enabled = true;
            stsTrainingStatus.Text = "학습 상태: 실행 중";
            ResetTrainingSummary(command.FinalModelPath);
            UpdateTrainingSummary(status: "실행 중", epoch: "-", progress: "0%", loss: "-");
            if (_lblTrainingModelSafetyNote != null)
                _lblTrainingModelSafetyNote.Text = "현재 임시 모델에 학습 중입니다. 정상 완료되면 최종 모델 위치로 복사되고, 중지하면 기존 최종 모델은 유지됩니다.";
            rtbTrainingOutput.Clear();

            try
            {
                _trainingProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command.FileName,
                        Arguments = command.Arguments,
                        WorkingDirectory = command.WorkingDirectory,
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
                        AppendTrainingLog(ev.Data, fromErrorStream: true);
                };

                _lastTrainingProgressPercent = -1;
                AppendTrainingLog("학습 프로세스를 시작합니다.");
                AppendTrainingLog("WSL/콘솔 출력은 이 학습 로그 창에 실시간으로 표시됩니다.");
                AppendTrainingLog("실행 방식: " + command.RunnerName);
                AppendTrainingLog("실행 명령: " + command.DisplayCommand);
                AppendTrainingLog("최종 모델 저장 위치: " + command.FinalModelPath);
                AppendTrainingLog("학습 중에는 임시 모델 파일을 사용합니다. 중지해도 기존 최종 모델은 유지됩니다.");

                _trainingProcess.Start();
                _trainingProcess.BeginOutputReadLine();
                _trainingProcess.BeginErrorReadLine();

                await _trainingProcess.WaitForExitAsync();

                AppendTrainingLog("");
                AppendTrainingLog($"학습 프로세스가 종료되었습니다. 종료 코드: {_trainingProcess.ExitCode}");
                if (_trainingProcess.ExitCode == 0)
                {
                    PublishCompletedTrainingModel(command);
                    AppendTrainingLog("학습 완료: 모델 파일을 저장했습니다. '모델 폴더 열기' 또는 '사용 명령 복사'를 사용할 수 있습니다.");
                    UpdateTrainingSummary(status: "완료", progress: "100%", modelPath: command.FinalModelPath);
                    if (_lblTrainingModelSafetyNote != null)
                        _lblTrainingModelSafetyNote.Text = "학습이 정상 완료되어 임시 모델을 최종 모델 위치로 복사했습니다.";
                    MessageBox.Show(
                        "학습이 완료되었습니다.\n\n" +
                        "모델 저장 위치:\n" + command.FinalModelPath + "\n\n" +
                        "다음 행동:\n" +
                        "- '모델 폴더 열기'로 저장된 파일을 확인할 수 있습니다.\n" +
                        "- '사용 명령 복사'로 DonkeyCar 실행 명령을 복사할 수 있습니다.",
                        "학습 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AppendTrainingLog("학습이 정상 완료되지 않아 최종 모델은 바꾸지 않았습니다. 기존 모델은 그대로 유지됩니다.");
                    UpdateTrainingSummary(status: "실패", modelPath: command.FinalModelPath);
                    if (_lblTrainingModelSafetyNote != null)
                        _lblTrainingModelSafetyNote.Text = "학습이 정상 완료되지 않았습니다. 기존 최종 모델은 그대로 유지됩니다.";
                    MessageBox.Show(
                        "학습이 정상 완료되지 않았습니다.\n\n" +
                        BuildTrainingFailureGuide(_trainingProcess.ExitCode) + "\n\n" +
                        "기존 최종 모델은 그대로 유지됩니다. 자세한 내용은 학습 로그의 '오류:' 줄을 확인하세요.",
                        "학습 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                stsTrainingStatus.Text = $"학습 상태: 종료 코드 {_trainingProcess.ExitCode}";
            }
            catch (Exception ex)
            {
                stsTrainingStatus.Text = "학습 상태: 오류";
                UpdateTrainingSummary(status: "오류");
                MessageBox.Show(
                    "학습 실행 중 오류가 발생했습니다.\n\n" +
                    BuildTrainingExceptionGuide(ex.Message) + "\n\n" +
                    "오류 내용: " + ex.Message,
                    "학습 실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UpdateTrainingStartButtonState();
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
                AppendTrainingLog("사용자 요청으로 학습 프로세스를 중지했습니다. 기존 최종 모델은 유지됩니다.");
                stsTrainingStatus.Text = "학습 상태: 중지됨";
                if (_lblTrainingModelSafetyNote != null)
                    _lblTrainingModelSafetyNote.Text = "학습을 중지했습니다. 중지 전 임시 모델은 최종 모델로 덮어쓰지 않으므로 기존 모델은 유지됩니다.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 프로세스를 중지하지 못했습니다.\n\n오류 내용: " + ex.Message,
                    "학습 중지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ConfirmOriginalTrainingDataIfNeeded(string tubPath)
        {
            if (IsLikelyCleanTrainingFolder(tubPath)) return true;

            var answer = MessageBox.Show(
                "현재 선택된 학습 데이터 폴더가 Clean 폴더로 보이지 않습니다.\n\n" +
                "데이터 정리 탭에서 제외 프레임을 반영한 Clean 폴더를 추출한 뒤 학습하는 것을 권장합니다.\n\n" +
                "그래도 현재 폴더로 학습을 시작할까요?",
                "원본 데이터 학습 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return answer == DialogResult.Yes;
        }

        private bool IsLikelyCleanTrainingFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath)) return false;

            string folderName = Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            return folderName.Contains("clean", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildTrainingFailureGuide(int exitCode)
        {
            return
                "종료 코드: " + exitCode + "\n\n" +
                "확인할 점:\n" +
                "- 학습 데이터 폴더에 catalog_*.catalog 또는 manifest.json이 있는지 확인하세요.\n" +
                "- Python 환경에 DonkeyCar가 설치되어 있는지 확인하세요.\n" +
                "- DonkeyCar 작업 폴더에 config.py가 있는지 확인하세요.\n" +
                "- 모델 저장 폴더에 쓰기 권한이 있는지 확인하세요.\n" +
                "- 로그에 '오류:' 또는 'Traceback'이 있으면 그 줄이 직접 원인입니다.";
        }

        private string BuildTrainingExceptionGuide(string message)
        {
            if (message.Contains("No such file", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("찾을 수", StringComparison.OrdinalIgnoreCase))
                return "경로를 찾지 못했습니다. 학습 데이터 폴더, DonkeyCar 작업 폴더, 모델 저장 위치를 다시 확인하세요.";

            if (message.Contains("conda", StringComparison.OrdinalIgnoreCase))
                return "Python 환경 문제가 의심됩니다. '환경 검사' 후 '자동 설정 시도' 또는 '설치 명령 복사'를 사용하세요.";

            if (message.Contains("denied", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("권한", StringComparison.OrdinalIgnoreCase))
                return "권한 문제가 의심됩니다. 모델 저장 폴더를 문서 폴더처럼 쓰기 가능한 위치로 바꿔보세요.";

            return
                "확인할 점:\n" +
                "- WSL과 DonkeyCar가 설치되어 있는지\n" +
                "- 학습 데이터 폴더와 모델 저장 경로가 올바른지\n" +
                "- Python 환경 안에서 donkey 명령이 실행되는지";
        }

        private bool TryBuildTrainingCommand(out TrainingCommand command)
        {
            command = TrainingCommand.Empty;

            string envName = ResolveTrainingPythonEnvName();
            string modelType = _trainingModelType.Trim().ToLowerInvariant();
            string mycarPath = ResolveTrainingProjectPath();
            string tubPath = txtTrainingTubPath.Text.Trim();
            string modelPath = txtTrainingModelPath.Text.Trim();
            string runner = DetectRecommendedTrainingRunner();
            int epochs = (int)numTrainingEpochs.Value;

            if (string.IsNullOrWhiteSpace(modelType))
                modelType = "linear";
            if (string.IsNullOrWhiteSpace(mycarPath))
                mycarPath = "~/mycar";
            if (string.IsNullOrWhiteSpace(tubPath))
                tubPath = GetDefaultTrainingTubPath();
            if (string.IsNullOrWhiteSpace(modelPath))
                modelPath = GetDefaultTrainingModelPath();

            txtTrainingTubPath.Text = tubPath;
            txtTrainingModelPath.Text = modelPath;

            if (!Directory.Exists(tubPath))
            {
                MessageBox.Show(
                    "학습 데이터 폴더를 찾을 수 없습니다.\n\n" +
                    "기본 경로: " + GetDefaultTrainingTubPath() + "\n" +
                    "Desktop의 data 폴더가 DonkeyCar 학습 데이터 폴더인지 확인해 주세요.",
                    "학습 데이터 경로 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                string? modelDirectory = Path.GetDirectoryName(modelPath);
                if (!string.IsNullOrWhiteSpace(modelDirectory))
                    Directory.CreateDirectory(modelDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "모델 저장 폴더를 만들 수 없습니다.\n\n" +
                    "저장 경로: " + modelPath + "\n\n" +
                    "오류 내용: " + ex.Message,
                    "모델 저장 경로 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string runModelPath = GetTemporaryTrainingModelPath(modelPath);

            if (runner == RunnerWindowsDonkey)
            {
                string configPath = FindTrainingConfigPath(mycarPath, useWslPaths: false);
                string trainArgs = BuildDonkeyTrainArguments(tubPath, runModelPath, modelType, configPath, useWslPaths: false);
                command = new TrainingCommand(
                    runner,
                    "donkey",
                    trainArgs,
                    ResolveWindowsWorkingDirectory(mycarPath),
                    "donkey " + trainArgs,
                    modelPath,
                    runModelPath,
                    tubPath);
                return true;
            }

            if (runner == RunnerWindowsConda)
            {
                if (string.IsNullOrWhiteSpace(envName))
                {
                    MessageBox.Show("Python 환경명을 확인하지 못했습니다.",
                        "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string configPath = FindTrainingConfigPath(mycarPath, useWslPaths: false);
                string trainArgs = BuildDonkeyTrainArguments(tubPath, runModelPath, modelType, configPath, useWslPaths: false);
                string arguments = "run --no-capture-output -n " + QuoteForCommandLine(envName) + " donkey " + trainArgs;
                command = new TrainingCommand(runner, "conda", arguments, string.Empty, "conda " + arguments, modelPath, runModelPath, tubPath);
                return true;
            }

            string wslConfigPath = "/tmp/teamapp_train_config.py";
            string wslMycarPath = ToWslPath(mycarPath);
            string sourceConfigPath = FindTrainingConfigPath(mycarPath, useWslPaths: true);
            string bashCommand = "cd " + QuotePathForBash(wslMycarPath) + " && " +
                BuildWslEpochConfigPrelude(sourceConfigPath, epochs, wslConfigPath) +
                BuildWslDonkeyTrainCommand(runner, envName, tubPath, runModelPath, modelType, wslConfigPath);

            string wslArguments = "bash -lc " + QuoteForCommandLine(bashCommand);
            command = new TrainingCommand(runner, "wsl", wslArguments, string.Empty, "wsl " + wslArguments, modelPath, runModelPath, tubPath);
            return true;
        }

        private string GetTemporaryTrainingModelPath(string finalModelPath)
        {
            string modelDirectory = Path.GetDirectoryName(finalModelPath) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(finalModelPath);
            string extension = Path.GetExtension(finalModelPath);

            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
                fileNameWithoutExtension = "pilot";
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".h5";

            string runDirectory = Path.Combine(modelDirectory, ".teamapp_training");
            Directory.CreateDirectory(runDirectory);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            return Path.Combine(runDirectory, fileNameWithoutExtension + "_" + timestamp + extension);
        }

        private void PublishCompletedTrainingModel(TrainingCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.FinalModelPath) ||
                string.IsNullOrWhiteSpace(command.RunModelPath) ||
                PathsEqual(command.FinalModelPath, command.RunModelPath))
            {
                return;
            }

            string? finalDirectory = Path.GetDirectoryName(command.FinalModelPath);
            if (!string.IsNullOrWhiteSpace(finalDirectory))
                Directory.CreateDirectory(finalDirectory);

            int copiedFileCount = 0;
            foreach (string extension in GetTrainingModelArtifactExtensions(command.FinalModelPath))
            {
                string sourcePath = Path.ChangeExtension(command.RunModelPath, extension);
                string targetPath = Path.ChangeExtension(command.FinalModelPath, extension);

                if (!File.Exists(sourcePath)) continue;

                File.Copy(sourcePath, targetPath, overwrite: true);
                copiedFileCount++;
                AppendTrainingLog("모델 파일 저장: " + targetPath);
            }

            if (copiedFileCount == 0)
                AppendTrainingLog("주의: 학습은 종료 코드 0으로 끝났지만 복사할 모델 파일을 찾지 못했습니다.");
        }

        private IEnumerable<string> GetTrainingModelArtifactExtensions(string modelPath)
        {
            string mainExtension = Path.GetExtension(modelPath);
            if (string.IsNullOrWhiteSpace(mainExtension))
                mainExtension = ".keras";

            yield return mainExtension;
            yield return ".tflite";
            yield return ".png";
        }

        private string BuildWslDonkeyTrainCommand(
            string runner,
            string envName,
            string tubPath,
            string modelPath,
            string modelType,
            string configPath)
        {
            string trainArgs = BuildDonkeyTrainArguments(
                tubPath,
                modelPath,
                modelType,
                configPath,
                useWslPaths: true,
                useBashQuotes: true);

            if (runner == RunnerWslConda)
            {
                return "stdbuf -oL -eL ~/miniconda3/bin/conda run --no-capture-output -n " +
                    QuoteForBash(envName) + " donkey " + trainArgs;
            }

            return "stdbuf -oL -eL donkey " + trainArgs;
        }

        private string DetectRecommendedTrainingRunner()
        {
            if (CanRunProcess("wsl", "--status", timeoutMs: 2500))
            {
                string envName = ResolveTrainingPythonEnvName();

                // WSL이 있는 PC에서는 conda 환경을 우선 사용합니다.
                // 환경 확인이 WSL cold start 때문에 늦어져도, 로컬 기본 환경(e2e_env)을 그대로 시도합니다.
                if (!string.IsNullOrWhiteSpace(envName))
                    return RunnerWslConda;

                return RunnerWslDonkey;
            }

            if (CanRunProcess("donkey", "--help", timeoutMs: 2500))
                return RunnerWindowsDonkey;

            if (CanRunProcess("conda", "--version", timeoutMs: 2500))
                return RunnerWindowsConda;

            return RunnerWslConda;
        }

        private string ResolveTrainingPythonEnvName()
        {
            string configuredEnvName = _trainingPythonEnvName.Trim();
            if (!string.IsNullOrWhiteSpace(configuredEnvName) && CanRunWslCondaEnvironment(configuredEnvName))
                return configuredEnvName;

            string? detectedEnvName = DetectLocalWslDonkeyEnvironment();
            if (!string.IsNullOrWhiteSpace(detectedEnvName))
            {
                _trainingPythonEnvName = detectedEnvName;
                return detectedEnvName;
            }

            _trainingPythonEnvName = "e2e_env";
            return "e2e_env";
        }

        private string ResolveTrainingProjectPath()
        {
            string configuredPath = _trainingMycarPath.Trim();
            if (!string.IsNullOrWhiteSpace(configuredPath) && IsWslDonkeyProjectPath(configuredPath))
                return configuredPath;

            string? detectedPath = DetectLocalWslDonkeyProjectPath();
            if (!string.IsNullOrWhiteSpace(detectedPath))
            {
                _trainingMycarPath = detectedPath;
                return detectedPath;
            }

            _trainingMycarPath = "~/mycar";
            return "~/mycar";
        }

        private string? DetectLocalWslDonkeyEnvironment()
        {
            foreach (string envName in new[] { "e2e_env", "donkey53", "donkey", "base" })
            {
                string bashCommand =
                    "test -x ~/miniconda3/bin/conda && " +
                    "~/miniconda3/bin/conda run --no-capture-output -n " + QuoteForBash(envName) +
                    " donkey --help >/dev/null 2>&1";

                if (CanRunProcess("wsl", "bash -lc " + QuoteForCommandLine(bashCommand), timeoutMs: 8000))
                    return envName;
            }

            return null;
        }

        private string? DetectLocalWslDonkeyProjectPath()
        {
            foreach (string projectPath in new[] { "~/mycar", "~/mysim", "~/donkeycar", "~/donkey" })
            {
                if (IsWslDonkeyProjectPath(projectPath))
                    return projectPath;
            }

            return null;
        }

        private bool IsWslDonkeyProjectPath(string projectPath)
        {
            if (string.IsNullOrWhiteSpace(projectPath)) return false;

            string bashPath = QuotePathForBash(projectPath);
            string bashCommand =
                "test -f " + bashPath + "/manage.py && " +
                "test -f " + bashPath + "/config.py";

            return CanRunProcess("wsl", "bash -lc " + QuoteForCommandLine(bashCommand), timeoutMs: 5000);
        }

        private string BuildWslEpochConfigPrelude(string configPath, int epochs, string temporaryConfigPath)
        {
            string sourceConfigPath = string.IsNullOrWhiteSpace(configPath)
                ? "~/mycar/config.py"
                : configPath;
            string safeEpochs = Math.Max(1, epochs).ToString(CultureInfo.InvariantCulture);

            return
                "cp " + QuotePathForBash(sourceConfigPath) + " " + QuotePathForBash(temporaryConfigPath) + " && " +
                "printf '\\n# TeamApp runtime override\\nMAX_EPOCHS = " + safeEpochs + "\\nBATCH_SIZE = 8\\n' >> " +
                QuotePathForBash(temporaryConfigPath) + " && ";
        }

        private string FindTrainingConfigPath(string mycarPath, bool useWslPaths)
        {
            if (string.IsNullOrWhiteSpace(mycarPath)) return string.Empty;

            if (useWslPaths || mycarPath.StartsWith("~/") || mycarPath.StartsWith("/"))
            {
                string normalized = mycarPath.TrimEnd('/', '\\').Replace("\\", "/");
                return normalized + "/config.py";
            }

            string configPath = Path.Combine(mycarPath, "config.py");
            if (File.Exists(configPath)) return configPath;

            string myConfigPath = Path.Combine(mycarPath, "myconfig.py");
            if (File.Exists(myConfigPath)) return myConfigPath;

            return string.Empty;
        }

        private string ResolveWindowsWorkingDirectory(string mycarPath)
        {
            if (string.IsNullOrWhiteSpace(mycarPath)) return string.Empty;
            if (mycarPath.StartsWith("~/") || mycarPath.StartsWith("/")) return string.Empty;
            return Directory.Exists(mycarPath) ? mycarPath : string.Empty;
        }

        private string BuildDonkeyTrainArguments(
            string tubPath,
            string modelPath,
            string modelType,
            string configPath,
            bool useWslPaths,
            bool useBashQuotes = false)
        {
            string QuoteValue(string value)
            {
                string resolvedValue = useWslPaths ? ToWslPath(value) : value;
                return useBashQuotes ? QuotePathForBash(resolvedValue) : QuoteForCommandLine(resolvedValue);
            }

            var parts = new List<string>
            {
                "train",
                "--tub=" + QuoteValue(tubPath),
                "--model=" + QuoteValue(modelPath),
                "--type=" + QuoteValue(modelType)
            };

            if (!string.IsNullOrWhiteSpace(configPath))
                parts.Add("--config=" + QuoteValue(configPath));

            return string.Join(" ", parts);
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

        private string QuotePathForBash(string value)
        {
            string path = ToWslPath(value);
            if (path == "~")
                return "$HOME";
            if (path.StartsWith("~/"))
                return "$HOME/" + QuoteForBash(path.Substring(2));

            return QuoteForBash(path);
        }

        private string ToBashLiteralPath(string value)
        {
            string path = ToWslPath(value);
            if (path == "~")
                return "$HOME";
            if (path.StartsWith("~/"))
                return "$HOME/" + path.Substring(2);

            return QuoteForBash(path);
        }

        private string QuoteForCommandLine(string value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }

        private bool CanRunProcess(string fileName, string arguments, int timeoutMs)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                if (!process.WaitForExit(timeoutMs))
                {
                    process.Kill(entireProcessTree: true);
                    return false;
                }

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private bool CanRunWslCondaEnvironment(string envName)
        {
            if (string.IsNullOrWhiteSpace(envName)) return false;

            // 환경 목록을 awk/grep으로 파싱하면 Windows -> WSL 따옴표 전달에서 깨질 수 있습니다.
            // conda run으로 Python을 직접 실행해 보는 방식이 더 단순하고 확실합니다.
            string command =
                "test -x ~/miniconda3/bin/conda && " +
                "~/miniconda3/bin/conda run --no-capture-output -n " + QuoteForBash(envName) +
                " python --version >/dev/null 2>&1";

            return CanRunProcess("wsl", "bash -lc " + QuoteForCommandLine(command), timeoutMs: 10000);
        }

        private async Task<TrainingEnvironmentCheck> RefreshTrainingEnvironmentAsync(bool showSuccessMessage)
        {
            if (_isCheckingTrainingEnvironment)
                return new TrainingEnvironmentCheck(false, "학습 환경: 이미 검사 중입니다.", Array.Empty<string>(), Array.Empty<string>());

            try
            {
                _isCheckingTrainingEnvironment = true;
                ApplyTrainingEnvironmentStatus("학습 환경: 검사 중...", false);
                SetTrainingEnvironmentButtonsEnabled(false);

                var details = new List<string>();
                EnsureDefaultTrainingPaths(details);
                string tubPath = txtTrainingTubPath.Text.Trim();
                string modelPath = txtTrainingModelPath.Text.Trim();
                string envName = ResolveTrainingPythonEnvName();
                string projectPath = ResolveTrainingProjectPath();

                TrainingEnvironmentCheck check = await Task.Run(() =>
                    CheckTrainingEnvironment(tubPath, modelPath, envName, projectPath, details));
                ApplyTrainingEnvironmentCheck(check);

                if (showSuccessMessage)
                {
                    string detail = string.Join(Environment.NewLine, check.Details);
                    MessageBox.Show(
                        check.StatusMessage + Environment.NewLine + Environment.NewLine +
                        BuildTrainingActionHint(check) + Environment.NewLine + Environment.NewLine +
                        detail,
                        "학습 환경 검사", MessageBoxButtons.OK,
                        check.IsReady ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }

                return check;
            }
            catch (Exception ex)
            {
                var failed = new TrainingEnvironmentCheck(
                    false,
                    "학습 환경: 검사 중 오류가 발생했습니다.",
                    new[] { "오류 내용: " + ex.Message },
                    BuildTrainingSetupCommands().Split(new[] { Environment.NewLine }, StringSplitOptions.None));
                ApplyTrainingEnvironmentCheck(failed);
                return failed;
            }
            finally
            {
                _isCheckingTrainingEnvironment = false;
                SetTrainingEnvironmentButtonsEnabled(true);
                UpdateTrainingStartButtonState();
            }
        }

        private TrainingEnvironmentCheck CheckTrainingEnvironment(
            string tubPath,
            string modelPath,
            string envName,
            string projectPath,
            List<string> details)
        {
            var commands = new List<string>();

            bool wslReady = CanRunProcess("wsl", "--status", timeoutMs: 5000);
            details.Add(wslReady ? "OK: WSL 설치 확인" : "FAIL: WSL이 설치되어 있지 않거나 실행할 수 없습니다.");
            if (!wslReady)
            {
                commands.Add("wsl --install -d Ubuntu-22.04");
                return BuildEnvironmentCheckResult(false, "학습 환경: WSL이 설치되어 있지 않습니다.", details, commands);
            }

            bool condaReady = CanRunWslBashCommand("test -x ~/miniconda3/bin/conda", timeoutMs: 5000);
            details.Add(condaReady ? "OK: WSL Python 환경 관리자 확인" : "FAIL: WSL에서 miniconda를 찾을 수 없습니다.");
            if (!condaReady)
            {
                commands.Add("Miniconda 설치가 필요합니다. 설치 후 WSL에서 ~/miniconda3/bin/conda가 실행되는지 확인해 주세요.");
                return BuildEnvironmentCheckResult(false, "학습 환경: WSL Python 환경 관리자가 없습니다.", details, commands);
            }

            bool envReady = CanRunWslCondaEnvironment(envName);
            details.Add(envReady ? $"OK: {envName} Python 환경 확인" : $"FAIL: {envName} Python 환경이 없습니다.");
            if (!envReady)
            {
                commands.Add("wsl bash -lc '~/miniconda3/bin/conda create -y -n " + envName + " python=3.11'");
                return BuildEnvironmentCheckResult(false, $"학습 환경: {envName} Python 환경이 없습니다.", details, commands);
            }

            bool donkeyReady = CanRunWslBashCommand(
                "~/miniconda3/bin/conda run --no-capture-output -n " + QuoteForBash(envName) +
                " donkey --help >/dev/null 2>&1",
                timeoutMs: 15000);
            details.Add(donkeyReady ? "OK: DonkeyCar CLI 확인" : "FAIL: DonkeyCar donkey 명령을 실행할 수 없습니다.");
            if (!donkeyReady)
            {
                commands.Add("wsl bash -lc '~/miniconda3/bin/conda run --no-capture-output -n " + envName + " python -m pip install \"donkeycar[pc]\"'");
                return BuildEnvironmentCheckResult(false, "학습 환경: DonkeyCar가 설치되어 있지 않습니다.", details, commands);
            }

            string bashProjectPath = QuotePathForBash(projectPath);
            bool configReady = CanRunWslBashCommand("test -f " + bashProjectPath + "/config.py", timeoutMs: 5000);
            details.Add(configReady ? "OK: DonkeyCar 작업 폴더 config.py 확인" : "FAIL: DonkeyCar 작업 폴더에 config.py가 없습니다. (" + projectPath + ")");
            if (!configReady)
            {
                commands.Add("wsl bash -lc '~/miniconda3/bin/conda run --no-capture-output -n " + envName + " donkey createcar --path=" + ToBashLiteralPath(projectPath) + "'");
                return BuildEnvironmentCheckResult(false, "학습 환경: DonkeyCar 작업 폴더 설정 파일이 없습니다.", details, commands);
            }

            bool tubReady = IsUsableTubFolder(tubPath);
            details.Add(tubReady ? "OK: 학습 데이터 폴더 확인" : "FAIL: 학습 데이터 폴더에 catalog_*.catalog 또는 manifest.json이 없습니다.");
            if (!tubReady)
                return BuildEnvironmentCheckResult(false, "학습 환경: 학습 데이터 폴더를 확인해 주세요.", details, commands);

            bool modelPathReady = CanPrepareModelSaveFolder(modelPath, out string modelError);
            details.Add(modelPathReady ? "OK: 모델 저장 폴더 쓰기 가능" : "FAIL: 모델 저장 폴더 오류 - " + modelError);
            if (!modelPathReady)
                return BuildEnvironmentCheckResult(false, "학습 환경: 모델 저장 폴더를 만들 수 없습니다.", details, commands);

            return BuildEnvironmentCheckResult(true, "학습 환경: 정상 (Python 환경: " + envName + ", 작업 폴더: " + projectPath + ")", details, commands);
        }

        private TrainingEnvironmentCheck BuildEnvironmentCheckResult(
            bool isReady,
            string statusMessage,
            List<string> details,
            List<string> fallbackCommands)
        {
            if (fallbackCommands.Count == 0)
                fallbackCommands.AddRange(BuildTrainingSetupCommands().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

            return new TrainingEnvironmentCheck(isReady, statusMessage, details, fallbackCommands);
        }

        private void EnsureDefaultTrainingPaths(List<string> details)
        {
            if (string.IsNullOrWhiteSpace(txtTrainingTubPath.Text))
                txtTrainingTubPath.Text = GetDefaultTrainingTubPath();

            if (string.IsNullOrWhiteSpace(txtTrainingModelPath.Text))
                txtTrainingModelPath.Text = GetDefaultTrainingModelPath();

            string defaultTubPath = GetDefaultTrainingTubPath();
            string currentTubPath = txtTrainingTubPath.Text.Trim();
            if (PathsEqual(currentTubPath, defaultTubPath) && !Directory.Exists(defaultTubPath))
            {
                Directory.CreateDirectory(defaultTubPath);
                details.Add("AUTO: Desktop\\data 폴더를 생성했습니다.");
            }

            string? modelDirectory = Path.GetDirectoryName(txtTrainingModelPath.Text.Trim());
            if (!string.IsNullOrWhiteSpace(modelDirectory) && !Directory.Exists(modelDirectory))
            {
                Directory.CreateDirectory(modelDirectory);
                details.Add("AUTO: 모델 저장 폴더를 생성했습니다.");
            }

            SaveTrainingSettingsSilently();
        }

        private bool IsUsableTubFolder(string tubPath)
        {
            if (string.IsNullOrWhiteSpace(tubPath) || !Directory.Exists(tubPath))
                return false;

            try
            {
                return File.Exists(Path.Combine(tubPath, "manifest.json")) ||
                    Directory.EnumerateFiles(tubPath, "catalog_*.catalog", SearchOption.TopDirectoryOnly).Any();
            }
            catch
            {
                return false;
            }
        }

        private bool CanPrepareModelSaveFolder(string modelPath, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                if (string.IsNullOrWhiteSpace(modelPath))
                    modelPath = GetDefaultTrainingModelPath();

                string? modelDirectory = Path.GetDirectoryName(modelPath);
                if (string.IsNullOrWhiteSpace(modelDirectory))
                {
                    errorMessage = "모델 저장 폴더가 비어 있습니다.";
                    return false;
                }

                Directory.CreateDirectory(modelDirectory);
                string testPath = Path.Combine(modelDirectory, ".teamapp_write_test");
                File.WriteAllText(testPath, "ok", Encoding.UTF8);
                File.Delete(testPath);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private bool PathsEqual(string left, string right)
        {
            try
            {
                return Path.GetFullPath(left).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Equals(Path.GetFullPath(right).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                        StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return left.Equals(right, StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool CanRunWslBashCommand(string bashCommand, int timeoutMs)
        {
            return CanRunProcess("wsl", "bash -lc " + QuoteForCommandLine(bashCommand), timeoutMs);
        }

        private async Task TryAutoSetupTrainingEnvironmentAsync()
        {
            try
            {
                ApplyTrainingEnvironmentStatus("학습 환경: 자동 설정 중...", false);
                SetTrainingEnvironmentButtonsEnabled(false);
                AppendTrainingLog("학습 환경 자동 설정을 시작합니다.");

                EnsureDefaultTrainingPaths(new List<string>());

                var details = new List<string>();
                EnsureDefaultTrainingPaths(details);
                string tubPath = txtTrainingTubPath.Text.Trim();
                string modelPath = txtTrainingModelPath.Text.Trim();
                string envName = ResolveTrainingPythonEnvName();
                string projectPath = ResolveTrainingProjectPath();
                TrainingEnvironmentCheck beforeCheck = await Task.Run(() =>
                    CheckTrainingEnvironment(tubPath, modelPath, envName, projectPath, details));
                if (beforeCheck.IsReady)
                {
                    ApplyTrainingEnvironmentCheck(beforeCheck);
                    MessageBox.Show("학습 환경이 이미 정상입니다.",
                        "자동 설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!CanRunProcess("wsl", "--status", timeoutMs: 5000))
                {
                    CopyTrainingSetupCommands();
                    ApplyTrainingEnvironmentCheck(beforeCheck);
                    return;
                }

                if (!CanRunWslBashCommand("test -x ~/miniconda3/bin/conda", timeoutMs: 5000))
                {
                    await RunAutoSetupCommandAsync(
                        "Miniconda 설치",
                        BuildMinicondaInstallBashCommand(),
                        timeoutMs: 900000);

                    if (!CanRunWslBashCommand("test -x ~/miniconda3/bin/conda", timeoutMs: 5000))
                        throw new InvalidOperationException("Miniconda 설치 후에도 ~/miniconda3/bin/conda를 찾지 못했습니다.");
                }

                if (!CanRunWslCondaEnvironment(envName))
                    await RunAutoSetupCommandAsync(
                        "Python 환경 생성",
                        "~/miniconda3/bin/conda create -y -n " + QuoteForBash(envName) + " python=3.11",
                        timeoutMs: 900000);

                if (!CanRunWslBashCommand("~/miniconda3/bin/conda run --no-capture-output -n " +
                    QuoteForBash(envName) + " donkey --help >/dev/null 2>&1", timeoutMs: 15000))
                {
                    await RunAutoSetupCommandAsync(
                        "DonkeyCar 설치",
                        "~/miniconda3/bin/conda run --no-capture-output -n " + QuoteForBash(envName) +
                        " python -m pip install \"donkeycar[pc]\"",
                        timeoutMs: 900000);
                }

                if (!CanRunWslBashCommand("~/miniconda3/bin/conda run --no-capture-output -n " +
                    QuoteForBash(envName) + " python -c \"import gym, gym_donkeycar, pygame\" >/dev/null 2>&1", timeoutMs: 15000))
                {
                    await RunAutoSetupCommandAsync(
                        "DonkeyCar 실행 의존성 설치",
                        "~/miniconda3/bin/conda run --no-capture-output -n " + QuoteForBash(envName) +
                        " python -m pip install gym==0.26.2 gym-donkeycar pygame",
                        timeoutMs: 300000);
                }

                string bashProjectPath = QuotePathForBash(projectPath);
                if (!CanRunWslBashCommand("test -f " + bashProjectPath + "/config.py", timeoutMs: 5000))
                {
                    await RunAutoSetupCommandAsync(
                        "DonkeyCar 프로젝트 설정 생성",
                        "~/miniconda3/bin/conda run --no-capture-output -n " + QuoteForBash(envName) +
                        " donkey createcar --path=" + ToBashLiteralPath(projectPath),
                        timeoutMs: 300000);
                }

                TrainingEnvironmentCheck afterCheck = await RefreshTrainingEnvironmentAsync(showSuccessMessage: false);
                if (!afterCheck.IsReady)
                {
                    CopyTrainingSetupCommands();
                    MessageBox.Show(
                        "자동 설정을 시도했지만 아직 부족한 항목이 있습니다.\n\n" +
                        afterCheck.StatusMessage + "\n\n" +
                        "설치 명령을 클립보드에 복사했습니다.",
                        "자동 설정", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                CopyTrainingSetupCommands();
                MessageBox.Show(
                    "자동 설정 중 오류가 발생했습니다.\n\n" +
                    "설치 명령을 클립보드에 복사했으니 직접 실행해 주세요.\n\n" +
                    "오류 내용: " + ex.Message,
                    "자동 설정 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetTrainingEnvironmentButtonsEnabled(true);
                UpdateTrainingStartButtonState();
            }
        }

        private async Task RunAutoSetupCommandAsync(string title, string bashCommand, int timeoutMs)
        {
            AppendTrainingLog(title + " 명령을 실행합니다.");
            ProcessRunResult result = await Task.Run(() =>
                RunProcessAndCapture("wsl", "bash -lc " + QuoteForCommandLine(bashCommand), timeoutMs));

            if (!string.IsNullOrWhiteSpace(result.Output))
                AppendTrainingLog(result.Output);

            if (!string.IsNullOrWhiteSpace(result.Error))
                AppendTrainingLog(result.Error, fromErrorStream: true);

            if (result.ExitCode != 0)
                throw new InvalidOperationException(title + " 실패: 종료 코드 " + result.ExitCode);
        }

        private string BuildMinicondaInstallBashCommand()
        {
            return
                "set -e; " +
                "cd \"$HOME\"; " +
                "if [ ! -x \"$HOME/miniconda3/bin/conda\" ]; then " +
                "installer=\"$HOME/Miniconda3-latest-Linux-x86_64.sh\"; " +
                "if command -v wget >/dev/null 2>&1; then " +
                "wget -O \"$installer\" https://repo.anaconda.com/miniconda/Miniconda3-latest-Linux-x86_64.sh; " +
                "else " +
                "curl -L -o \"$installer\" https://repo.anaconda.com/miniconda/Miniconda3-latest-Linux-x86_64.sh; " +
                "fi; " +
                "bash \"$installer\" -b -p \"$HOME/miniconda3\"; " +
                "fi; " +
                "\"$HOME/miniconda3/bin/conda\" --version";
        }

        private ProcessRunResult RunProcessAndCapture(string fileName, string arguments, int timeoutMs)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            process.Start();
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            if (!process.WaitForExit(timeoutMs))
            {
                process.Kill(entireProcessTree: true);
                return new ProcessRunResult(-1, outputTask.Result, "명령 실행 시간이 초과되었습니다.");
            }

            return new ProcessRunResult(process.ExitCode, outputTask.Result, errorTask.Result);
        }

        private void CopyTrainingSetupCommands()
        {
            try
            {
                string commands = BuildTrainingSetupCommands();
                Clipboard.SetText(commands);
                MessageBox.Show(
                    "학습 환경 설치/확인 명령을 클립보드에 복사했습니다.\n\n" +
                    "Windows PowerShell에 그대로 붙여넣어 실행하면 됩니다.\n" +
                    "WSL 설치처럼 재부팅이나 권한이 필요한 단계는 안내 메시지를 보고 따로 진행해 주세요.",
                    "설치 명령 복사", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("설치 명령을 복사하지 못했습니다.\n\n오류 내용: " + ex.Message,
                    "설치 명령 복사 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildTrainingSetupCommands()
        {
            string envName = string.IsNullOrWhiteSpace(_trainingPythonEnvName)
                ? (DetectLocalWslDonkeyEnvironment() ?? "e2e_env")
                : _trainingPythonEnvName.Trim();
            string projectPath = string.IsNullOrWhiteSpace(_trainingMycarPath)
                ? (DetectLocalWslDonkeyProjectPath() ?? "~/mycar")
                : _trainingMycarPath.Trim();
            string safeEnvName = GetSafeCondaEnvironmentName(envName);
            string setupProjectPath = ToBashLiteralPath(projectPath);
            string displayProjectPath = projectPath.Replace("'", "");

            return string.Join(Environment.NewLine, new[]
            {
                "# TeamApp DonkeyCar 학습 환경 준비용 PowerShell 명령",
                "# 현재 설정: Python 환경 = " + safeEnvName + ", DonkeyCar 작업 폴더 = " + displayProjectPath,
                "# 1) WSL이 없으면 아래 명령 실행 후 PC를 재부팅하고, 이 복사 명령을 다시 실행하세요.",
                "wsl.exe --status",
                "if ($LASTEXITCODE -ne 0) {",
                "    Write-Host 'WSL이 없습니다. 다음 명령을 관리자 PowerShell에서 실행한 뒤 재부팅하세요:'",
                "    Write-Host 'wsl --install -d Ubuntu-22.04'",
                "    return",
                "}",
                "",
                "# 2) Miniconda가 없으면 WSL 안에 자동 설치합니다.",
                "wsl.exe bash -lc '" + BuildMinicondaInstallBashCommand() + "'",
                "",
                "# 3) Python 환경과 DonkeyCar를 준비합니다.",
                "wsl.exe bash -lc '$HOME/miniconda3/bin/conda run --no-capture-output -n " + safeEnvName + " python --version >/dev/null 2>&1 || $HOME/miniconda3/bin/conda create -y -n " + safeEnvName + " python=3.11'",
                "wsl.exe bash -lc '$HOME/miniconda3/bin/conda run --no-capture-output -n " + safeEnvName + " python -m pip install \"donkeycar[pc]\"'",
                "wsl.exe bash -lc '$HOME/miniconda3/bin/conda run --no-capture-output -n " + safeEnvName + " python -m pip install gym==0.26.2 gym-donkeycar pygame'",
                "",
                "# 4) DonkeyCar 프로젝트 설정 파일이 없으면 생성합니다.",
                "wsl.exe bash -lc 'test -f " + setupProjectPath + "/config.py || $HOME/miniconda3/bin/conda run --no-capture-output -n " + safeEnvName + " donkey createcar --path=" + setupProjectPath + "'",
                "",
                "# 5) 준비 확인",
                "wsl.exe bash -lc '$HOME/miniconda3/bin/conda run --no-capture-output -n " + safeEnvName + " donkey --help'",
                "Write-Host 'TeamApp 학습 환경 준비가 끝났습니다.'"
            });
        }

        private string GetSafeCondaEnvironmentName(string value)
        {
            string trimmed = value.Trim();
            bool isSafe = trimmed.Length > 0 &&
                trimmed.All(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-' || ch == '.');

            return isSafe ? trimmed : "e2e_env";
        }

        private void ApplyTrainingEnvironmentCheck(TrainingEnvironmentCheck check)
        {
            _isTrainingEnvironmentReady = check.IsReady;
            ApplyTrainingEnvironmentStatus(check.StatusMessage, check.IsReady);
            UpdateTrainingActionHint(BuildTrainingActionHint(check));
            UpdateTrainingStartButtonState();
        }

        private void ApplyTrainingEnvironmentStatus(string message, bool isReady)
        {
            if (_lblTrainingEnvironmentStatus != null)
            {
                _lblTrainingEnvironmentStatus.Text = message;
                _lblTrainingEnvironmentStatus.ForeColor = isReady ? System.Drawing.Color.ForestGreen : System.Drawing.Color.Firebrick;
            }

            stsTrainingStatus.Text = message.Replace("학습 환경:", "학습 상태:");
            UpdateTrainingEnvironmentSummary(isReady ? "정상" : ExtractTrainingStatusSummary(message));
        }

        private void UpdateTrainingEnvironmentSummary(string statusText)
        {
            if (_lblTrainingEnvironmentSummary == null) return;

            string envName = string.IsNullOrWhiteSpace(_trainingPythonEnvName) ? "자동 감지 전" : _trainingPythonEnvName.Trim();
            string projectPath = string.IsNullOrWhiteSpace(_trainingMycarPath) ? "자동 감지 전" : _trainingMycarPath.Trim();
            string tubPath = string.IsNullOrWhiteSpace(txtTrainingTubPath.Text) ? "미선택" : txtTrainingTubPath.Text.Trim();
            string modelPath = string.IsNullOrWhiteSpace(txtTrainingModelPath.Text) ? "기본값 사용" : txtTrainingModelPath.Text.Trim();

            _lblTrainingEnvironmentSummary.Text =
                "현재 설정: Python 환경=" + envName +
                " | 작업 폴더=" + projectPath +
                " | 학습 데이터=" + tubPath +
                " | 모델=" + modelPath +
                " | 상태=" + statusText;
        }

        private string ExtractTrainingStatusSummary(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return "검사 전";

            string summary = message.Replace("학습 환경:", "").Trim();
            int detailStart = summary.IndexOf("(", StringComparison.Ordinal);
            if (detailStart > 0)
                summary = summary[..detailStart].Trim();

            return string.IsNullOrWhiteSpace(summary) ? "검사 전" : summary;
        }

        private void UpdateTrainingActionHint(string message)
        {
            if (_lblTrainingActionHint == null) return;

            _lblTrainingActionHint.Text = message;
            _lblTrainingActionHint.ForeColor = _isTrainingEnvironmentReady
                ? System.Drawing.Color.ForestGreen
                : System.Drawing.Color.DimGray;
        }

        private string BuildTrainingActionHint(TrainingEnvironmentCheck check)
        {
            if (check.IsReady)
                return "환경이 정상입니다. 이제 '학습 시작'을 누르면 됩니다.";

            string message = check.StatusMessage;
            if (message.Contains("Tub", StringComparison.OrdinalIgnoreCase))
                return "다음 행동: '데이터 선택'으로 DonkeyCar 학습 데이터 폴더를 먼저 선택하세요.";
            if (message.Contains("모델 저장", StringComparison.OrdinalIgnoreCase))
                return "다음 행동: '저장 위치'에서 모델을 저장할 폴더를 선택하세요.";
            if (message.Contains("Python 환경", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("conda 환경", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("DonkeyCar", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("config.py", StringComparison.OrdinalIgnoreCase))
                return "다음 행동: 자동 설정을 먼저 시도하고, 실패하면 설치 명령 복사를 사용하세요.";
            if (message.Contains("WSL", StringComparison.OrdinalIgnoreCase))
                return "다음 행동: 설치 명령 복사를 눌러 WSL 설치 명령을 확인하세요.";

            return "다음 행동: 환경 검사를 다시 누르거나 환경 설정에서 경로를 직접 지정하세요.";
        }

        private void UpdateTrainingStartButtonState()
        {
            bool isRunning = _trainingProcess is { HasExited: false };
            btnStartTrainingProcess.Enabled = _isTrainingEnvironmentReady && !isRunning && !_isCheckingTrainingEnvironment;
        }

        private void SetTrainingEnvironmentButtonsEnabled(bool enabled)
        {
            if (_btnCheckTrainingEnvironment != null)
                _btnCheckTrainingEnvironment.Enabled = enabled;
            if (_btnAutoSetupTrainingEnvironment != null)
                _btnAutoSetupTrainingEnvironment.Enabled = enabled;
            if (_btnCopyTrainingSetupCommands != null)
                _btnCopyTrainingSetupCommands.Enabled = enabled;
            if (_btnEditTrainingEnvironment != null)
                _btnEditTrainingEnvironment.Enabled = enabled;
        }

        private void AppendTrainingLog(string text, bool fromErrorStream = false)
        {
            if (rtbTrainingOutput.IsDisposed) return;

            string? displayText = FormatTrainingLogLine(text, fromErrorStream);
            if (string.IsNullOrWhiteSpace(displayText)) return;

            void Append()
            {
                rtbTrainingOutput.AppendText(displayText + Environment.NewLine);
                rtbTrainingOutput.SelectionStart = rtbTrainingOutput.TextLength;
                rtbTrainingOutput.ScrollToCaret();
            }

            if (rtbTrainingOutput.InvokeRequired)
                rtbTrainingOutput.BeginInvoke(new Action(Append));
            else
                Append();
        }

        private void UpdateTrainingSummarySafe(
            string? status = null,
            string? epoch = null,
            string? progress = null,
            string? loss = null,
            string? modelPath = null)
        {
            if (IsDisposed) return;

            void Update() => UpdateTrainingSummary(status, epoch, progress, loss, modelPath);

            if (InvokeRequired)
                BeginInvoke(new Action(Update));
            else
                Update();
        }

        private string? FormatTrainingLogLine(string rawText, bool fromErrorStream)
        {
            string text = CleanConsoleControlCharacters(rawText);
            if (string.IsNullOrWhiteSpace(text)) return null;

            Match epochMatch = Regex.Match(text, @"^Epoch\s+(\d+)\s*/\s*(\d+)", RegexOptions.IgnoreCase);
            if (epochMatch.Success)
                UpdateTrainingSummarySafe(status: "학습 중", epoch: epochMatch.Groups[1].Value + "/" + epochMatch.Groups[2].Value, progress: "0%");

            string? progressLine = TryFormatTrainingProgress(text);
            if (progressLine != null) return progressLine;

            if (!fromErrorStream && Regex.IsMatch(text, @"^\d{1,2}$"))
                return null;

            if (text.Contains("Failed writing database file", StringComparison.OrdinalIgnoreCase))
                return "참고: DonkeyCar 모델 DB 저장은 건너뛰었습니다. 모델 파일 저장에는 영향 없습니다.";

            if (text.Contains("Starting training", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainingSummarySafe(status: "학습 중");
                return "학습 시작: 데이터로 모델을 학습하는 중입니다.";
            }

            if (text.Contains("Finished training", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainingSummarySafe(status: "마무리 중");
                return "학습 종료: 모델 학습 단계가 완료되었습니다.";
            }

            if (text.Contains("TFLite conversion done", StringComparison.OrdinalIgnoreCase))
                return "변환 완료: TFLite 모델 파일도 생성했습니다.";

            if (text.Contains("saving model to", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainingSummarySafe(status: "모델 저장 중");
                return "모델 저장 중: " + ExtractTextAfter(text, "saving model to");
            }

            if (text.Contains("Found datastore at", StringComparison.OrdinalIgnoreCase))
                return "학습 데이터 확인: " + ExtractTextAfter(text, "Found datastore at");

            if (text.Contains("Records # Training", StringComparison.OrdinalIgnoreCase))
                return "학습 데이터 수: " + ExtractTextAfter(text, "Records # Training");

            if (text.Contains("Records # Validation", StringComparison.OrdinalIgnoreCase))
                return "검증 데이터 수: " + ExtractTextAfter(text, "Records # Validation");

            if (ShouldHideVerboseTrainingLine(text)) return null;

            bool looksLikeRealError =
                text.Contains("Traceback", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Exception", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("EnvironmentLocationNotFound", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase);

            if (fromErrorStream && looksLikeRealError)
            {
                UpdateTrainingSummarySafe(status: "오류");
                return "오류: " + text;
            }

            return text;
        }

        private string CleanConsoleControlCharacters(string rawText)
        {
            string text = Regex.Replace(rawText, @"\x1B\[[0-9;?]*[ -/]*[@-~]", "");
            text = text.Replace("\r", "");

            while (text.Contains('\b'))
            {
                int index = text.IndexOf('\b');
                if (index > 0)
                    text = text.Remove(index - 1, 2);
                else
                    text = text.Remove(index, 1);
            }

            return new string(text.Where(ch => !char.IsControl(ch) || ch == '\t').ToArray()).Trim();
        }

        private string? TryFormatTrainingProgress(string text)
        {
            Match match = Regex.Match(text, @"^\s*(\d+)\s*/\s*(\d+)\s+\[");
            if (!match.Success) return null;

            int current = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            int total = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            if (total <= 0) return null;

            int percent = Math.Min(100, Math.Max(0, current * 100 / total));
            if (percent < 100 && percent == _lastTrainingProgressPercent) return null;
            _lastTrainingProgressPercent = percent;

            int filled = percent * 20 / 100;
            string bar = new string('=', filled).PadRight(20, '-');
            string lossText = ExtractLossSummary(text);
            UpdateTrainingSummarySafe(progress: percent + "%", loss: ExtractLossValue(text));
            return $"학습 진행률 [{bar}] {percent,3}% ({current}/{total}){lossText}";
        }

        private string ExtractLossSummary(string text)
        {
            Match match = Regex.Match(text, @"loss:\s*([0-9.]+)");
            return match.Success ? $"  loss: {match.Groups[1].Value}" : "";
        }

        private string? ExtractLossValue(string text)
        {
            Match match = Regex.Match(text, @"loss:\s*([0-9.]+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        private bool ShouldHideVerboseTrainingLine(string text)
        {
            string[] noisyPatterns =
            {
                "tensorflow/core",
                "external/local_",
                "could not find cuda",
                "cuda drivers",
                "cuDNN",
                "cuFFT",
                "cuBLAS",
                "NUMA",
                "TF-TRT",
                "oneDNN",
                "This TensorFlow binary is optimized",
                "To enable the following instructions",
                "Skipping registering GPU devices",
                "Assets written to:",
                "Summary on the non-converted ops",
                "Accepted dialects",
                "Non-Converted Ops",
                "arith.constant",
                "(f32:",
                "Ignored output_format",
                "Ignored drop_control_dependency",
                "Reading SavedModel",
                "Restoring SavedModel",
                "SavedModel load",
                "MLIR",
                "disabling MLIR",
                "Closing tub",
                "Closing manifest",
                "Creating ImageTransformations",
                "Using last catalog",
                "Train with image caching"
            };

            return noisyPatterns.Any(pattern => text.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private string ExtractTextAfter(string text, string marker)
        {
            int index = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return text;
            return text.Substring(index + marker.Length).Trim(' ', ':');
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

        private void BtnSelectTrainingModelPath_Click(object? sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Title = "모델 저장 경로 선택",
                Filter = "H5 모델 (*.h5)|*.h5|Keras 모델 (*.keras)|*.keras|모든 파일 (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(txtTrainingModelPath.Text)
                    ? "pilot.h5"
                    : Path.GetFileName(txtTrainingModelPath.Text)
            };

            string currentPath = txtTrainingModelPath.Text.Trim();
            string? currentDirectory = Path.GetDirectoryName(currentPath);
            if (!string.IsNullOrWhiteSpace(currentDirectory) && Directory.Exists(currentDirectory))
                dialog.InitialDirectory = currentDirectory;

            if (dialog.ShowDialog(this) == DialogResult.OK)
                txtTrainingModelPath.Text = dialog.FileName;
        }

        private void EditTrainingEnvironmentOverride()
        {
            string detectedEnvName = DetectLocalWslDonkeyEnvironment() ?? "e2e_env";
            string detectedProjectPath = DetectLocalWslDonkeyProjectPath() ?? "~/mycar";

            string currentEnvName = string.IsNullOrWhiteSpace(_trainingPythonEnvName)
                ? detectedEnvName
                : _trainingPythonEnvName.Trim();
            string currentProjectPath = string.IsNullOrWhiteSpace(_trainingMycarPath)
                ? detectedProjectPath
                : _trainingMycarPath.Trim();

            string? envName = PromptForTrainingValue(
                "Conda 환경명",
                "DonkeyCar가 설치된 Python 환경명을 입력하세요.\n예: e2e_env, donkey53",
                currentEnvName);
            if (envName == null) return;

            string? projectPath = PromptForTrainingValue(
                "DonkeyCar 작업 폴더",
                "WSL 안의 DonkeyCar 작업 폴더를 입력하세요.\n예: ~/mycar, ~/mysim",
                currentProjectPath);
            if (projectPath == null) return;

            _trainingPythonEnvName = string.IsNullOrWhiteSpace(envName) ? detectedEnvName : envName.Trim();
            _trainingMycarPath = string.IsNullOrWhiteSpace(projectPath) ? detectedProjectPath : projectPath.Trim();
            SaveTrainingSettingsSilently();
            UpdateTrainingEnvironmentSummary("수동 설정 저장됨");

            ApplyTrainingEnvironmentStatus(
                "학습 환경: 수동 설정 저장됨 (Python 환경: " + _trainingPythonEnvName + ", 작업 폴더: " + _trainingMycarPath + ")",
                false);
            _ = RefreshTrainingEnvironmentAsync(showSuccessMessage: true);
        }

        private string? PromptForTrainingValue(string title, string message, string defaultValue)
        {
            using var form = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ClientSize = new Size(520, 150)
            };

            var label = new System.Windows.Forms.Label
            {
                AutoSize = false,
                Text = message,
                Location = new Point(12, 12),
                Size = new Size(496, 52)
            };
            var textBox = new TextBox
            {
                Text = defaultValue,
                Location = new Point(12, 70),
                Size = new Size(496, 28)
            };
            var okButton = new Button
            {
                Text = "확인",
                DialogResult = DialogResult.OK,
                Location = new Point(322, 110),
                Size = new Size(88, 30)
            };
            var cancelButton = new Button
            {
                Text = "취소",
                DialogResult = DialogResult.Cancel,
                Location = new Point(420, 110),
                Size = new Size(88, 30)
            };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(okButton);
            form.Controls.Add(cancelButton);
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog(this) == DialogResult.OK ? textBox.Text : null;
        }

        private void BtnOpenTrainingModelFolder_Click(object? sender, EventArgs e)
        {
            try
            {
                string modelPath = txtTrainingModelPath.Text.Trim();
                string? modelDirectory = Path.GetDirectoryName(modelPath);

                if (string.IsNullOrWhiteSpace(modelDirectory))
                {
                    MessageBox.Show("모델 저장 경로를 먼저 확인해 주세요.",
                        "모델 폴더 열기", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Directory.CreateDirectory(modelDirectory);
                Process.Start(new ProcessStartInfo
                {
                    FileName = modelDirectory,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("모델 폴더를 열 수 없습니다.\n\n오류 내용: " + ex.Message,
                    "모델 폴더 열기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCopyModelUseCommand_Click(object? sender, EventArgs e)
        {
            try
            {
                string command = BuildModelUseCommand();
                Clipboard.SetText(command);
                MessageBox.Show(
                    "학습된 모델을 DonkeyCar에서 실행하는 명령을 클립보드에 복사했습니다.\n\n" + command,
                    "사용 명령 복사", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("모델 사용 명령을 만들 수 없습니다.\n\n오류 내용: " + ex.Message,
                    "사용 명령 복사 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildModelUseCommand()
        {
            string envName = ResolveTrainingPythonEnvName();
            string mycarPath = ResolveTrainingProjectPath();
            string modelPath = string.IsNullOrWhiteSpace(txtTrainingModelPath.Text)
                ? GetDefaultTrainingModelPath()
                : txtTrainingModelPath.Text.Trim();
            string modelType = string.IsNullOrWhiteSpace(_trainingModelType)
                ? "linear"
                : _trainingModelType.Trim().ToLowerInvariant();

            string bashMycarPath = ToBashLiteralPath(mycarPath);
            string runtimeConfigFileName = "teamapp_drive_myconfig.py";
            string runtimeConfigPath = bashMycarPath + "/" + runtimeConfigFileName;
            string runtimeOverride =
                "cp " + bashMycarPath + "/myconfig.py " + runtimeConfigPath + " && " +
                "printf '\\n# TeamApp local PC run override\\nCAMERA_TYPE = \"MOCK\"\\nDONKEY_GYM = False\\nDRIVE_TRAIN_TYPE = \"MOCK\"\\n' >> " +
                runtimeConfigPath + " && ";

            string bashCommand =
                "cd " + bashMycarPath + " && " +
                runtimeOverride +
                "~/miniconda3/bin/conda run --no-capture-output -n " + QuoteForBash(envName) + " " +
                "python manage.py drive --myconfig=" + QuoteForBash(runtimeConfigFileName) +
                " --model=" + QuotePathForBash(modelPath) + " --type=" + QuoteForBash(modelType);

            return "wsl.exe --% bash -lc " + QuoteForCommandLine(bashCommand);
        }

        private void BtnSaveTrainingConfig_Click(object? sender, EventArgs e)
        {
            try
            {
                SaveTrainingSettingsSilently();
                UpdateTrainingEnvironmentSummary("설정 저장됨");
                MessageBox.Show(
                    "학습 설정이 저장되었습니다.\n\n" +
                    "현재 설정:\n" +
                    "- Python 환경: " + _trainingPythonEnvName + "\n" +
                    "- DonkeyCar 작업 폴더: " + _trainingMycarPath + "\n" +
                    "- 학습 데이터: " + txtTrainingTubPath.Text.Trim() + "\n" +
                    "- 모델: " + txtTrainingModelPath.Text.Trim(),
                    "설정 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 설정 저장 중 오류가 발생했습니다.\n\n오류 내용: " + ex.Message,
                "설정 저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveTrainingSettingsSilently()
        {
            Directory.CreateDirectory(Application.UserAppDataPath);
            var settings = new TrainingSettings
            {
                MycarPath = _trainingMycarPath.Trim(),
                TubPath = txtTrainingTubPath.Text.Trim(),
                ModelPath = txtTrainingModelPath.Text.Trim(),
                ModelType = _trainingModelType.Trim(),
                PythonEnvName = _trainingPythonEnvName.Trim(),
                Epochs = (int)numTrainingEpochs.Value
            };

            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GetTrainingSettingsPath(), json, Encoding.UTF8);
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

                _trainingMycarPath = string.IsNullOrWhiteSpace(settings.MycarPath)
                    ? (DetectLocalWslDonkeyProjectPath() ?? "~/mycar")
                    : settings.MycarPath;
                txtTrainingTubPath.Text = ShouldUseDefaultTubPath(settings.TubPath) ? GetDefaultTrainingTubPath() : settings.TubPath;
                txtTrainingModelPath.Text = ShouldUseDefaultModelPath(settings.ModelPath) ? GetDefaultTrainingModelPath() : settings.ModelPath;
                _trainingModelType = string.IsNullOrWhiteSpace(settings.ModelType) ? "linear" : settings.ModelType;
                _trainingPythonEnvName = string.IsNullOrWhiteSpace(settings.PythonEnvName)
                    ? (DetectLocalWslDonkeyEnvironment() ?? "e2e_env")
                    : settings.PythonEnvName;
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

        private bool ShouldUseDefaultTubPath(string savedPath)
        {
            return string.IsNullOrWhiteSpace(savedPath) ||
                savedPath.Equals("data", StringComparison.OrdinalIgnoreCase) ||
                !Directory.Exists(savedPath);
        }

        private bool ShouldUseDefaultModelPath(string savedPath)
        {
            if (string.IsNullOrWhiteSpace(savedPath)) return true;
            if (savedPath.Replace("\\", "/").Equals("models/pilot.keras", StringComparison.OrdinalIgnoreCase)) return true;
            if (PathsEqual(savedPath, GetLegacyDesktopTrainingModelPath())) return true;
            if (PathsEqual(savedPath, GetLegacyDocumentsTrainingModelPath())) return true;
            if (PathsEqual(savedPath, GetLegacyDocumentsModelFolderKerasPath())) return true;

            string? directory = Path.GetDirectoryName(savedPath);
            return string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory);
        }

        private sealed class TrainingSettings
        {
            public string MycarPath { get; set; } = "~/mycar";
            public string TubPath { get; set; } = "";
            public string ModelPath { get; set; } = "";
            public string ModelType { get; set; } = "linear";
            public string PythonEnvName { get; set; } = "e2e_env";
            public int Epochs { get; set; } = 1;
        }

        private sealed class TrainingEnvironmentCheck
        {
            public TrainingEnvironmentCheck(bool isReady, string statusMessage, IReadOnlyList<string> details, IReadOnlyList<string> fallbackCommands)
            {
                IsReady = isReady;
                StatusMessage = statusMessage;
                Details = details;
                FallbackCommands = fallbackCommands;
            }

            public bool IsReady { get; }
            public string StatusMessage { get; }
            public IReadOnlyList<string> Details { get; }
            public IReadOnlyList<string> FallbackCommands { get; }
        }

        private sealed class ProcessRunResult
        {
            public ProcessRunResult(int exitCode, string output, string error)
            {
                ExitCode = exitCode;
                Output = output;
                Error = error;
            }

            public int ExitCode { get; }
            public string Output { get; }
            public string Error { get; }
        }

        private sealed class TrainingCommand
        {
            public static readonly TrainingCommand Empty = new TrainingCommand(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            public TrainingCommand(
                string runnerName,
                string fileName,
                string arguments,
                string workingDirectory,
                string displayCommand,
                string finalModelPath,
                string runModelPath,
                string tubPath)
            {
                RunnerName = runnerName;
                FileName = fileName;
                Arguments = arguments;
                WorkingDirectory = workingDirectory;
                DisplayCommand = displayCommand;
                FinalModelPath = finalModelPath;
                RunModelPath = runModelPath;
                TubPath = tubPath;
            }

            public string RunnerName { get; }
            public string FileName { get; }
            public string Arguments { get; }
            public string WorkingDirectory { get; }
            public string DisplayCommand { get; }
            public string FinalModelPath { get; }
            public string RunModelPath { get; }
            public string TubPath { get; }
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
        private void GrpDataCleaner_Enter(object sender, EventArgs e) { }

        // ScottPlot 차트를 필요할 때 생성합니다.

        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabGraphStats && _isChartDirty)
                RenderFrameChart();

            if (tabControlMain.SelectedTab == tabTrainingMonitor)
                _ = RefreshTrainingEnvironmentAsync(showSuccessMessage: false);
        }

        private void InitFrameChart()
        {
            if (lblChartDescription != null)
            {
                lblChartDescription.AutoSize = false;
                lblChartDescription.Font = new Font("맑은 고딕", 10F);
                lblChartDescription.Location = new Point(20, 14);
                lblChartDescription.Size = new Size(Math.Max(300, tabGraphStats.ClientSize.Width - 40), 48);
                lblChartDescription.TextAlign = ContentAlignment.MiddleLeft;
                lblChartDescription.Text =
                    "방향값과 속도값의 흐름을 보여줍니다. 0 기준선에서 멀수록 조작이 강하고, 검색 적용 시에는 원본 프레임 위치 기준의 점으로 표시됩니다.";
            }

            pnlChartHost.Location = new Point(20, 76);
            pnlChartHost.Size = new Size(
                Math.Max(300, tabGraphStats.ClientSize.Width - 40),
                Math.Max(300, tabGraphStats.ClientSize.Height - 96));
            pnlChartHost.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlChartHost.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);

            _frameChart = new FormsPlot
            {
                Dock = DockStyle.Fill,
                Name = "formsPlotMain"
            };

            _frameChart.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1e1e1e");
            _frameChart.Plot.DataBackground.Color   = ScottPlot.Color.FromHex("#2d2d30");

            pnlChartHost.Controls.Clear();
            pnlChartHost.Controls.Add(_frameChart);
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
            plot.Legend.FontName = "Malgun Gothic";
            plot.Legend.FontSize = 13;
            plot.Legend.FontColor = ScottPlot.Color.FromHex("#222222");
            plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#F7F7F7");
            plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#777777");

            plot.Clear();

            var chartFrames = _isFrameFilterActive
                ? _visibleFrames.Where(f => !f.IsDeleted).ToList()
                : _allFrames.Where(f => !f.IsDeleted).ToList();

            if (chartFrames.Count == 0)
            {
                plot.Title("No valid frames");
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

            double[] zeroXs = { xs.Min(), xs.Max() };
            double[] zeroYs = { 0, 0 };
            var zeroLine = plot.Add.Scatter(zeroXs, zeroYs);
            zeroLine.Color = ScottPlot.Color.FromHex("#9E9E9E");
            zeroLine.LineWidth = 1.2f;
            zeroLine.MarkerSize = 0;
            zeroLine.LegendText = "Zero";

            if (_isFrameFilterActive)
            {
                var angleScatter = plot.Add.Scatter(xs, angleYs);
                angleScatter.Color = ScottPlot.Color.FromHex("#4FC3F7");
                angleScatter.LineWidth = 0;
                angleScatter.MarkerSize = 6;
                angleScatter.LegendText = "Steering";

                var throttleScatter = plot.Add.Scatter(xs, throttleYs);
                throttleScatter.Color = ScottPlot.Color.FromHex("#81C784");
                throttleScatter.LineWidth = 0;
                throttleScatter.MarkerSize = 6;
                throttleScatter.LegendText = "Speed";
            }
            else
            {
                var sigAngle = plot.Add.SignalXY(xs, angleYs);
                sigAngle.Color = ScottPlot.Color.FromHex("#4FC3F7");
                sigAngle.LineWidth = 1.5f;
                sigAngle.LegendText = "Steering";

                var sigThrottle = plot.Add.SignalXY(xs, throttleYs);
                sigThrottle.Color = ScottPlot.Color.FromHex("#81C784");
                sigThrottle.LineWidth = 1.5f;
                sigThrottle.LegendText = "Speed";
            }

            // 축 라벨과 제목
            plot.XLabel(_isFrameFilterActive
                ? "Original frame index"
                : "Training frame order");
            plot.YLabel("Value (-1 ~ 1)");
            plot.Title(_isFrameFilterActive
                ? $"Filtered distribution [shown: {n} / total: {_allFrames.Count}]"
                : $"Steering/Speed flow [train: {n} / total: {_allFrames.Count} / excluded: {_allFrames.Count(f => f.IsDeleted)}]");
            plot.Axes.SetLimitsY(-1.2, 1.2);
            plot.ShowLegend(Alignment.UpperRight);
            plot.Axes.Color(ScottPlot.Color.FromHex("#DDE3EA"));

            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#444444");

            _frameChart.Refresh();
            _isChartDirty = false;
        }
    }
}
