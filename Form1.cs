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
        private bool _isDraggingFrameRows = false;
        private int _dragStartFrameRowIndex = -1;
        private bool _isDraggingTimelineRange = false;
        private int _timelineRangeStartIndex = -1;
        private bool _hasUnsavedCleanupChanges = false;
        private bool _showDriveOverlay = true;
        private bool _hasAutoDetectedTrainingTab = false;
        private Process? _trainingProcess;
        private Button? _btnShowReviewCandidates;
        private readonly StringBuilder _trainingOutputLineBuffer = new StringBuilder();
        private System.Windows.Forms.Label? _lblTrainingSummaryTitle;
        private System.Windows.Forms.Label? _lblTrainingSummaryStatus;
        private System.Windows.Forms.Label? _lblTrainingSummaryEpoch;
        private System.Windows.Forms.Label? _lblTrainingSummaryProgress;
        private System.Windows.Forms.Label? _lblTrainingSummaryLoss;
        private System.Windows.Forms.Label? _lblCleanupSummary;
        private System.Windows.Forms.Label? _lblCleanupWorkflowHint;
        private System.Windows.Forms.Label? _lblFrameReviewHint;
        private bool _isTrainingAutoDetectRunning = false;
        private const string DeletedFramesMetaFileName = "deleted_frames_meta.txt";
        private const string TrainingSettingsFileName = "training_settings.json";
        private static readonly Regex AnsiEscapeRegex = new(@"\x1B(?:[@-Z\\-_]|\[[0-?]*[ -/]*[@-~])", RegexOptions.Compiled);

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
            btnSelectTrainingTubPath.Click += (_, _) => SelectFolderInto(txtTrainingTubPath, "Tub 폴더 선택");
            btnSelectTrainingModelPath.Click += BtnSelectTrainingModelPath_Click;
            btnDetectTrainingEnvironment.Click += BtnDetectTrainingEnvironment_Click;
            btnSelectTrainingWslDistro.Click += BtnSelectTrainingWslDistro_Click;
            btnSelectCondaPath.Click += BtnSelectCondaPath_Click;

            mnuFileOpenDataFolder.Click   += (s, _) => BtnOpenDataFolder_Click(s!, EventArgs.Empty);
            mnuFileReloadData.Click       += (s, _) => BtnReloadData_Click(s!, EventArgs.Empty);
            mnuExit.Click             += (s, _) => Application.Exit();
            mnuHelpOpenTutorial.Click        += (s, _) => RunFeatureTutorial("도움말");

            tabControlMain.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            FormClosing += Form1_FormClosing;

            ConfigureFrameCatalogGrid();
            ApplyDataManagerUiStyle();
            ConfigureReviewCandidateControls();
            ConfigureCleanupGuidanceControls();

            btnExcludeSelectedFrames.Text = "선택 프레임 제외";
            btnExportCleanDataset.Text          = "클린 폴더 추출";
            btnRestoreFrames.Text         = "복원";
            txtAngleMinFilter.Text    = "-1";
            txtAngleMaxFilter.Text    = "1";
            txtThrottleMinFilter.Text = "-1";
            txtThrottleMaxFilter.Text = "1";

            InitializeTrainingControls();
            LoadTrainingSettings();
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
            ShowWorkflowGuide();
        }

        private void ShowWorkflowGuide()
        {
            string nextAction = GetRecommendedNextAction();
            string guide =
                "권장 작업 순서\n\n" +
                "1. 데이터 폴더 열기\n" +
                "2. 이상 후보 보기 또는 표/슬라이더로 범위 선택\n" +
                "3. 선택 제외 또는 복원\n" +
                "4. 상태 저장\n" +
                "5. 학습용 폴더 만들기\n" +
                "6. 학습 실행 탭에서 Clean 폴더로 학습\n\n" +
                "현재 다음 추천 작업\n" +
                nextAction + "\n\n" +
                "기능별 설명을 다시 보고 싶으면 상단 '도움말' 메뉴를 누르세요.";

            MessageBox.Show(guide, "단계별 가이드", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetRecommendedNextAction()
        {
            if (_allFrames == null || _allFrames.Count == 0)
                return "- 먼저 '데이터 폴더 열기'로 DonkeyCar tub 폴더를 불러오세요.";

            int selectedCount = GetSelectedFrames().Count;
            if (selectedCount > 1)
                return $"- 현재 {selectedCount}개 프레임이 선택되어 있습니다. 제외할 구간이 맞으면 '선택 제외'를 누르세요.";

            int reviewCount = _allFrames.Count(frame => !frame.IsDeleted && frame.NeedsReview);
            if (reviewCount > 0 && !_isFrameFilterActive)
                return $"- 자동 검토 후보가 {reviewCount}개 있습니다. 먼저 '이상 후보 보기'로 확인하세요.";

            if (_hasUnsavedCleanupChanges)
                return "- 제외/복원 변경사항이 저장되지 않았습니다. '상태 저장'을 눌러 작업 내용을 보관하세요.";

            int validCount = _allFrames.Count(frame => !frame.IsDeleted);
            int deletedCount = _allFrames.Count - validCount;
            if (deletedCount > 0)
                return $"- 제외 {deletedCount}개, 사용 {validCount}개 상태입니다. 학습 전 '학습용 폴더 만들기'로 Clean 폴더를 만드세요.";

            return "- 데이터를 훑어보며 이상 후보나 직접 선택한 구간을 제외하세요. 표/슬라이더 드래그로 여러 프레임을 선택할 수 있습니다.";
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
                new TutorialStep("데이터 보기", "프레임 위치 슬라이더", "현재 프레임 위치를 빠르게 이동하거나, 드래그해서 시작~끝 프레임 범위를 선택할 수 있습니다.", trkFrameTimeline, tabPageDataViewer),
                new TutorialStep("데이터 보기", "조향값", "선택한 프레임의 Angle 값을 표시합니다. 왼쪽/오른쪽 조향 상태를 확인할 때 봅니다.", lblAngleValue, tabPageDataViewer),
                new TutorialStep("데이터 보기", "스로틀값", "선택한 프레임의 Throttle 값을 표시합니다. 전진/정지/후진 정도를 확인할 때 봅니다.", lblThrottleValue, tabPageDataViewer),
                new TutorialStep("데이터 보기", "모드", "선택한 프레임의 주행 모드 정보를 표시합니다. user/local 같은 상태를 확인합니다.", lblModeValue, tabPageDataViewer),
                new TutorialStep("정리", "조향각 범위", "필터에 사용할 조향각 최소값과 최대값을 입력합니다. 예: -1부터 1까지.", txtAngleMinFilter, tabPageDataViewer),
                new TutorialStep("정리", "스로틀 범위", "필터에 사용할 스로틀 최소값과 최대값을 입력합니다. 예: -1부터 1까지.", txtThrottleMinFilter, tabPageDataViewer),
                new TutorialStep("정리", "모드 필터", "user, local 등 특정 주행 모드만 보고 싶을 때 선택합니다.", cmbModeFilter, tabPageDataViewer),
                new TutorialStep("정리", "시나리오 필터", "normal, night, turn 같은 시나리오별로 프레임을 좁혀 봅니다.", cmbScenarioFilter, tabPageDataViewer),
                new TutorialStep("정리", "필터 적용", "입력한 범위와 선택한 모드/시나리오 조건으로 목록을 필터링합니다.", btnApplyFrameFilter, tabPageDataViewer),
                new TutorialStep("정리", "필터 해제", "필터 조건을 풀고 원본 프레임 목록을 다시 표시합니다.", btnClearFrameFilter, tabPageDataViewer),
                new TutorialStep("정리", "여러 프레임 선택", "표에서 마우스로 행을 드래그하거나, 아래 슬라이더를 드래그하면 여러 프레임을 한 번에 선택할 수 있습니다.", dgvFrameCatalog, tabPageDataViewer),
                new TutorialStep("정리", "선택 프레임 제외", "선택한 여러 프레임을 학습 제외 상태로 바꿉니다. 실제 파일은 삭제하지 않고 제외 표시만 합니다.", btnExcludeSelectedFrames, tabPageDataViewer),
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
                DisplayFrameAtIndex(idx);
        }

        private void TrkFrameTimeline_Scroll(object sender, EventArgs e)
        {
            int idx = trkFrameTimeline.Value;
            if (idx >= 0 && idx < _visibleFrames.Count)
            {
                if (_isDraggingTimelineRange)
                    SelectFrameGridRange(_timelineRangeStartIndex, idx);
                else
                    SetIndex(idx);
            }
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
                case Keys.Delete:
                    if (tabControlMain.SelectedTab == tabPageDataViewer)
                    {
                        BtnExcludeSelectedFrames_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
                case Keys.S:
                    if (e.Control && tabControlMain.SelectedTab == tabPageDataViewer)
                    {
                        BtnSaveCleanupState_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
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
            dgvFrameCatalog.MultiSelect = true;
            dgvFrameCatalog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFrameCatalog.Font = new Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Regular);
            dgvFrameCatalog.ColumnHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            dgvFrameCatalog.DefaultCellStyle.Font = new Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Regular);
            dgvFrameCatalog.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvFrameCatalog.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.FormattedIndex),
                HeaderText = "번호",
                Width = 76,
                Frozen = true
            });
            dgvFrameCatalog.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FrameData.Name),
                HeaderText = "이미지 파일",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 220
            });

            dgvFrameCatalog.RowTemplate.Height = 25;
            dgvFrameCatalog.ColumnHeadersHeight = 28;
            dgvFrameCatalog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvFrameCatalog.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            dgvFrameCatalog.DataBindingComplete += (_, _) => ApplyFrameCatalogRowStyle();
            dgvFrameCatalog.MouseDown += DgvFrameCatalog_MouseDown;
            dgvFrameCatalog.MouseMove += DgvFrameCatalog_MouseMove;
            dgvFrameCatalog.MouseUp += DgvFrameCatalog_MouseUp;
            trkFrameTimeline.MouseDown += TrkFrameTimeline_MouseDown;
            trkFrameTimeline.MouseMove += TrkFrameTimeline_MouseMove;
            trkFrameTimeline.MouseUp += TrkFrameTimeline_MouseUp;
        }

        private void DgvFrameCatalog_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            int rowIndex = GetFrameGridRowIndexAt(e.Location);
            if (rowIndex < 0) return;

            _dragStartFrameRowIndex = rowIndex;
            _isDraggingFrameRows = true;
        }

        private void DgvFrameCatalog_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDraggingFrameRows || e.Button != MouseButtons.Left) return;

            int rowIndex = GetFrameGridRowIndexAt(e.Location);
            if (rowIndex < 0 || _dragStartFrameRowIndex < 0) return;

            SelectFrameGridRange(_dragStartFrameRowIndex, rowIndex);
        }

        private void DgvFrameCatalog_MouseUp(object? sender, MouseEventArgs e)
        {
            _isDraggingFrameRows = false;
            _dragStartFrameRowIndex = -1;
        }

        private void TrkFrameTimeline_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _visibleFrames.Count == 0) return;

            int clickedIndex = GetTimelineIndexAt(e.X);

            _isDraggingTimelineRange = true;
            _timelineRangeStartIndex = clickedIndex;
            trkFrameTimeline.Value = clickedIndex;
            SelectFrameGridRange(clickedIndex, clickedIndex);
        }

        private void TrkFrameTimeline_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDraggingTimelineRange || e.Button != MouseButtons.Left || _visibleFrames.Count == 0) return;

            int currentIndex = GetTimelineIndexAt(e.X);
            if (trkFrameTimeline.Value != currentIndex)
                trkFrameTimeline.Value = currentIndex;

            SelectFrameGridRange(_timelineRangeStartIndex, currentIndex);
        }

        private void TrkFrameTimeline_MouseUp(object? sender, MouseEventArgs e)
        {
            _isDraggingTimelineRange = false;
            _timelineRangeStartIndex = -1;
        }

        private int GetFrameGridRowIndexAt(Point point)
        {
            var hit = dgvFrameCatalog.HitTest(point.X, point.Y);
            return hit.Type == DataGridViewHitTestType.Cell ? hit.RowIndex : -1;
        }

        private int GetTimelineIndexAt(int mouseX)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return 0;

            int min = trkFrameTimeline.Minimum;
            int max = trkFrameTimeline.Maximum;
            if (max <= min) return min;

            double ratio = Math.Clamp(mouseX / (double)Math.Max(1, trkFrameTimeline.ClientSize.Width), 0.0, 1.0);
            int value = min + (int)Math.Round((max - min) * ratio);
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// 마우스 드래그 선택에서 사용하는 범위 선택입니다.
        /// 선택 자체를 유지해야 하므로 SetIndex를 호출하지 않고 현재 행 표시만 갱신합니다.
        /// </summary>
        private void SelectFrameGridRange(int startRowIndex, int endRowIndex)
        {
            if (dgvFrameCatalog.Rows.Count == 0) return;

            int from = Math.Max(0, Math.Min(startRowIndex, endRowIndex));
            int to = Math.Min(dgvFrameCatalog.Rows.Count - 1, Math.Max(startRowIndex, endRowIndex));

            _isFrameSelectionUpdating = true;
            dgvFrameCatalog.ClearSelection();
            for (int rowIndex = from; rowIndex <= to; rowIndex++)
                dgvFrameCatalog.Rows[rowIndex].Selected = true;

            dgvFrameCatalog.CurrentCell = dgvFrameCatalog.Rows[endRowIndex].Cells[0];
            _isFrameSelectionUpdating = false;

            if (endRowIndex >= 0 && endRowIndex < _visibleFrames.Count)
                DisplayFrameAtIndex(endRowIndex);
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

            HideRangeCleanupControls();
            ArrangeFrameInfoPanel();
        }

        private void HideRangeCleanupControls()
        {
            lblFrameRange.Visible = false;
            lblFrameRangeSeparator.Visible = false;
            txtFrameRangeStart.Visible = false;
            txtFrameRangeEnd.Visible = false;
            btnExcludeFrameRange.Visible = false;

            btnExcludeSelectedFrames.Location = btnExcludeFrameRange.Location;
            btnExcludeSelectedFrames.Size = btnExcludeFrameRange.Size;
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

        private void ConfigureCleanupGuidanceControls()
        {
            if (_lblCleanupSummary == null)
            {
                _lblCleanupSummary = new System.Windows.Forms.Label
                {
                    Name = "lblCleanupSummary",
                    Text = "정리 상태: 데이터 없음",
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold),
                    ForeColor = System.Drawing.Color.FromArgb(45, 65, 90)
                };
                grpDataCleaner.Controls.Add(_lblCleanupSummary);
                _lblCleanupSummary.BringToFront();
            }

            if (_lblCleanupWorkflowHint == null)
            {
                _lblCleanupWorkflowHint = new System.Windows.Forms.Label
                {
                    Name = "lblCleanupWorkflowHint",
                    Text = "대량 정리: 표/트랙바 드래그 선택 -> Delete 또는 '선택 제외' -> Ctrl+S 저장",
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new System.Drawing.Font("맑은 고딕", 9F),
                    ForeColor = System.Drawing.Color.DimGray
                };
                grpDataCleaner.Controls.Add(_lblCleanupWorkflowHint);
                _lblCleanupWorkflowHint.BringToFront();
            }

            grpDataCleaner.Resize += (_, _) => LayoutCleanupGuidanceControls();
            LayoutCleanupGuidanceControls();
        }

        private void LayoutCleanupGuidanceControls()
        {
            int width = Math.Max(300, grpDataCleaner.ClientSize.Width - 40);
            int y = Math.Max(18, grpDataCleaner.ClientSize.Height - 50);

            if (_lblCleanupSummary != null)
            {
                _lblCleanupSummary.Location = new Point(20, y);
                _lblCleanupSummary.Size = new Size(width, 22);
            }

            if (_lblCleanupWorkflowHint != null)
            {
                _lblCleanupWorkflowHint.Location = new Point(20, y + 23);
                _lblCleanupWorkflowHint.Size = new Size(width, 22);
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

            DisplayFrameAtIndex(idx);
        }

        private void DisplayFrameAtIndex(int idx)
        {
            if (_visibleFrames == null || idx < 0 || idx >= _visibleFrames.Count) return;

            _currentFrameIndex = idx;

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
                    AppendTrainingLog("[안내] Clean 폴더가 Tub 경로로 설정되었습니다. 자동 감지를 눌러 WSL, conda, 프로젝트 경로를 확인한 뒤 학습을 시작하세요.");
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

            if (_lblCleanupSummary != null)
            {
                _lblCleanupSummary.Text =
                    $"정리 상태: 전체 {total} / 학습 사용 {valid} / 제외 {deleted} / 이상 후보 {review} / 표시 {shown} / {saveState}";
                _lblCleanupSummary.ForeColor = _hasUnsavedCleanupChanges
                    ? System.Drawing.Color.FromArgb(150, 80, 0)
                    : System.Drawing.Color.FromArgb(45, 65, 90);
            }

            UpdateCleanupSaveUi();
        }

        private void MarkCleanupStateChanged()
        {
            _hasUnsavedCleanupChanges = true;
            UpdateStatusLabels();
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

        private void InitializeTrainingControls()
        {
            cmbTrainingModelType.Items.Clear();
            cmbTrainingModelType.Items.AddRange(new object[] { "linear", "inferred", "tensorrt_linear", "tflite_linear", "categorical", "rnn", "3d", "imu", "behavior" });
            if (string.IsNullOrWhiteSpace(cmbTrainingModelType.Text))
                cmbTrainingModelType.Text = "linear";

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

            ConfigureTrainingSummaryLabels();
            ApplyTrainingControlStyle();
            LayoutTrainingTab();
            grpTrainingConfig.Resize += (_, _) => LayoutTrainingTab();
            tabTrainingMonitor.Resize += (_, _) => LayoutTrainingTab();
        }

        private void ConfigureTrainingSummaryLabels()
        {
            _lblTrainingSummaryTitle ??= CreateTrainingSummaryLabel("lblTrainingSummaryTitle", "학습 요약", true);
            _lblTrainingSummaryStatus ??= CreateTrainingSummaryLabel("lblTrainingSummaryStatus", "상태: 대기", false);
            _lblTrainingSummaryEpoch ??= CreateTrainingSummaryLabel("lblTrainingSummaryEpoch", "Epoch: -", false);
            _lblTrainingSummaryProgress ??= CreateTrainingSummaryLabel("lblTrainingSummaryProgress", "진행률: -", false);
            _lblTrainingSummaryLoss ??= CreateTrainingSummaryLabel("lblTrainingSummaryLoss", "loss: -", false);
        }

        private System.Windows.Forms.Label CreateTrainingSummaryLabel(string name, string text, bool bold)
        {
            var label = new System.Windows.Forms.Label
            {
                Name = name,
                AutoSize = false,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("맑은 고딕", 10.5F, bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular),
                ForeColor = System.Drawing.Color.FromArgb(40, 40, 40)
            };

            grpTrainingConfig.Controls.Add(label);
            label.BringToFront();
            return label;
        }

        /// <summary>
        /// 학습 탭의 글자 크기와 버튼 크기를 통일합니다.
        /// Designer에서 가져온 컨트롤마다 폰트 크기가 다를 수 있어 실행 시점에 한 번 정리합니다.
        /// </summary>
        private void ApplyTrainingControlStyle()
        {
            System.Drawing.Font commonFont = new System.Drawing.Font("맑은 고딕", 10.5F, System.Drawing.FontStyle.Regular);
            System.Drawing.Font labelFont = new System.Drawing.Font("맑은 고딕", 10.5F, System.Drawing.FontStyle.Regular);
            System.Drawing.Font buttonFont = new System.Drawing.Font("맑은 고딕", 10.5F, System.Drawing.FontStyle.Regular);

            foreach (Control control in grpTrainingConfig.Controls)
            {
                control.Font = control is Button ? buttonFont : commonFont;

                if (control is System.Windows.Forms.Label label)
                {
                    label.Font = labelFont;
                    label.AutoEllipsis = true;
                }
            }

            rtbTrainingOutput.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular);
            grpTrainingConfig.Font = labelFont;
            grpTrainingOutput.Font = labelFont;
        }

        /// <summary>
        /// View_fix에서 가져온 학습 탭은 작은 해상도 기준 좌표를 가지고 있습니다.
        /// 현재 폼 크기에 맞춰 모든 입력칸과 버튼을 다시 배치해서 글자 잘림과 컨트롤 겹침을 방지합니다.
        /// </summary>
        private void LayoutTrainingTab()
        {
            if (grpTrainingConfig == null || grpTrainingOutput == null) return;

            int margin = 16;
            int labelX = 40;
            int inputX = 220;
            int buttonWidth = 140;
            int buttonHeight = 32;
            int rowHeight = 39;
            int top = 42;
            int groupWidth = Math.Max(900, tabTrainingMonitor.ClientSize.Width - margin * 2);
            int buttonX = groupWidth - buttonWidth - 40;
            int inputWidth = Math.Max(280, buttonX - inputX - 16);
            int halfWidth = Math.Max(220, (inputWidth - 24) / 2);
            int rightLabelX = inputX + halfWidth + 54;
            int rightInputX = rightLabelX + 130;
            int rightInputWidth = Math.Max(180, buttonX - rightInputX - 16);

            grpTrainingConfig.Location = new Point(margin, 18);
            grpTrainingConfig.Size = new Size(groupWidth, 382);
            grpTrainingConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            PlaceTrainingRow(lblMycarProjectPath, cmbMycarProjectPath, btnSelectMycarPath, labelX, inputX, buttonX, top, inputWidth, buttonWidth, buttonHeight);
            PlaceTrainingRow(lblTrainingTubPath, txtTrainingTubPath, btnSelectTrainingTubPath, labelX, inputX, buttonX, top + rowHeight, inputWidth, buttonWidth, buttonHeight);
            PlaceTrainingRow(lblTrainingModelPath, txtTrainingModelPath, btnSelectTrainingModelPath, labelX, inputX, buttonX, top + rowHeight * 2, inputWidth, buttonWidth, buttonHeight);

            PlaceTrainingRow(lblTrainingModelType, cmbTrainingModelType, null, labelX, inputX, buttonX, top + rowHeight * 3, halfWidth, buttonWidth, buttonHeight);
            PlaceTrainingRow(lblTrainingPythonEnvName, txtTrainingPythonEnvName, null, labelX, inputX, buttonX, top + rowHeight * 4, halfWidth, buttonWidth, buttonHeight);
            PlaceTrainingRow(lblEpoch, numTrainingEpochs, null, labelX, inputX, buttonX, top + rowHeight * 5, 180, buttonWidth, buttonHeight);

            PlaceTrainingRow(lblTrainingWslDistro, cmbTrainingWslDistro, btnSelectTrainingWslDistro, rightLabelX, rightInputX, buttonX, top + rowHeight * 3, rightInputWidth, buttonWidth, buttonHeight);
            PlaceTrainingRow(lblCondaPath, cmbCondaPath, btnSelectCondaPath, rightLabelX, rightInputX, buttonX, top + rowHeight * 4, rightInputWidth, buttonWidth, buttonHeight);

            int actionY = top + rowHeight * 6 + 16;
            int actionWidth = 150;
            btnStartTrainingProcess.Location = new Point(inputX, actionY);
            btnStopTrainingProcess.Location = new Point(inputX + actionWidth + 24, actionY);
            btnSaveTrainingConfig.Location = new Point(inputX + (actionWidth + 24) * 2, actionY);
            btnDetectTrainingEnvironment.Location = new Point(inputX + (actionWidth + 24) * 3, actionY);

            foreach (var button in new[] { btnStartTrainingProcess, btnStopTrainingProcess, btnSaveTrainingConfig, btnDetectTrainingEnvironment })
            {
                button.Size = new Size(actionWidth, buttonHeight);
                button.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }
            btnDetectTrainingEnvironment.Text = "자동 감지";

            int summaryY = actionY + buttonHeight + 18;
            int summaryX = inputX;
            int summaryGap = 18;
            int summaryTitleWidth = 110;
            int summaryItemWidth = Math.Max(150, (buttonX - summaryX - summaryTitleWidth - summaryGap * 4) / 4);
            PlaceTrainingSummaryLabel(_lblTrainingSummaryTitle, summaryX, summaryY, summaryTitleWidth, buttonHeight);
            PlaceTrainingSummaryLabel(_lblTrainingSummaryStatus, summaryX + summaryTitleWidth + summaryGap, summaryY, summaryItemWidth, buttonHeight);
            PlaceTrainingSummaryLabel(_lblTrainingSummaryEpoch, summaryX + summaryTitleWidth + summaryGap + (summaryItemWidth + summaryGap), summaryY, summaryItemWidth, buttonHeight);
            PlaceTrainingSummaryLabel(_lblTrainingSummaryProgress, summaryX + summaryTitleWidth + summaryGap + (summaryItemWidth + summaryGap) * 2, summaryY, summaryItemWidth, buttonHeight);
            PlaceTrainingSummaryLabel(_lblTrainingSummaryLoss, summaryX + summaryTitleWidth + summaryGap + (summaryItemWidth + summaryGap) * 3, summaryY, summaryItemWidth, buttonHeight);

            grpTrainingOutput.Location = new Point(margin, grpTrainingConfig.Bottom + 16);
            grpTrainingOutput.Size = new Size(groupWidth, Math.Max(260, tabTrainingMonitor.ClientSize.Height - grpTrainingOutput.Top - 42));
            grpTrainingOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            rtbTrainingOutput.Dock = DockStyle.Fill;
            statusStripTraining.Dock = DockStyle.Bottom;
        }

        private void PlaceTrainingRow(
            System.Windows.Forms.Label label,
            Control input,
            Button? button,
            int labelX,
            int inputX,
            int buttonX,
            int y,
            int inputWidth,
            int buttonWidth,
            int buttonHeight)
        {
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Location = new Point(labelX, y + 2);
            label.Size = new Size(inputX - labelX - 12, buttonHeight);

            input.Location = new Point(inputX, y);
            input.Size = new Size(inputWidth, buttonHeight);
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            if (button == null) return;

            button.Location = new Point(buttonX, y);
            button.Size = new Size(buttonWidth, buttonHeight);
            button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button.Text = button == btnSelectTrainingWslDistro || button == btnSelectCondaPath ? "감지" : "경로 선택";
        }

        private void PlaceTrainingSummaryLabel(System.Windows.Forms.Label? label, int x, int y, int width, int height)
        {
            if (label == null) return;

            label.Location = new Point(x, y);
            label.Size = new Size(width, height);
            label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        }

        private async void BtnStartTrainingProcess_Click(object? sender, EventArgs e)
        {
            if (_trainingProcess is { HasExited: false })
            {
                MessageBox.Show("이미 학습 프로세스가 실행 중입니다.",
                    "학습 실행", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool detected = await DetectTrainingEnvironmentAsync(showSuccessMessage: false, clearLog: false);
            if (!detected)
            {
                MessageBox.Show(
                    "학습 환경 자동 감지에 실패했습니다.\n\n" +
                    "자동 감지 버튼을 눌러 로그를 확인하거나, WSL Ubuntu / Conda 경로 / Donkey 프로젝트 경로를 직접 선택해 주세요.",
                    "학습 환경 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryBuildTrainingCommand(out var arguments, out string displayArguments)) return;

            btnStartTrainingProcess.Enabled = false;
            btnStopTrainingProcess.Enabled = true;
            stsTrainingStatus.Text = "학습 상태: 실행 중";
            UpdateTrainingSummary(status: "실행 중", epoch: "-", progress: "-", loss: "-");
            rtbTrainingOutput.Clear();

            try
            {
                _trainingProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "wsl",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    },
                    EnableRaisingEvents = true
                };

                foreach (string argument in arguments)
                    _trainingProcess.StartInfo.ArgumentList.Add(argument);

                AppendTrainingLog("학습 프로세스를 시작합니다.");
                AppendTrainingLog("실행 명령: wsl " + displayArguments);

                _trainingProcess.Start();
                Task outputReader = ReadStreamAsync(_trainingProcess.StandardOutput);
                Task errorReader = ReadStreamAsync(_trainingProcess.StandardError);

                await _trainingProcess.WaitForExitAsync();
                await Task.WhenAll(outputReader, errorReader);

                AppendTrainingLog("");
                AppendTrainingLog($"학습 프로세스가 종료되었습니다. 종료 코드: {_trainingProcess.ExitCode}");
                UpdateTrainingSummary(status: _trainingProcess.ExitCode == 0 ? "완료" : "오류");
                stsTrainingStatus.Text = $"학습 상태: 종료 코드 {_trainingProcess.ExitCode}";
            }
            catch (Exception ex)
            {
                stsTrainingStatus.Text = "학습 상태: 오류";
                UpdateTrainingSummary(status: "오류");
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

                _trainingProcess.Kill(true);
                AppendTrainingLog("[시스템] 학습이 사용자에 의해 강제 중단되었습니다.");
                stsTrainingStatus.Text = "학습 상태: 중지됨";
                UpdateTrainingSummary(status: "중지됨");
            }
            catch (Exception ex)
            {
                MessageBox.Show("학습 프로세스를 중지하지 못했습니다.\n\n오류 내용: " + ex.Message,
                    "학습 중지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryBuildTrainingCommand(out List<string> arguments, out string displayArguments)
        {
            arguments = new List<string>();
            displayArguments = string.Empty;

            string envName = txtTrainingPythonEnvName.Text.Trim();
            string modelType = cmbTrainingModelType.Text.Trim().ToLowerInvariant();
            string mycarPath = cmbMycarProjectPath.Text.Trim();
            string tubPath = txtTrainingTubPath.Text.Trim();
            string modelPath = txtTrainingModelPath.Text.Trim();
            string wslDistro = NormalizeWslDistroName(cmbTrainingWslDistro.Text);
            string condaPath = cmbCondaPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(wslDistro))
            {
                MessageBox.Show("WSL Ubuntu를 자동 감지하거나 선택해 주세요.",
                    "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(condaPath))
            {
                MessageBox.Show("Conda 경로를 자동 감지하거나 선택해 주세요.",
                    "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(envName))
            {
                MessageBox.Show("Conda 환경명을 입력해 주세요. 예: e2e_env",
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
            {
                MessageBox.Show("Donkey 프로젝트 경로를 입력하거나 자동 감지 결과를 확인해 주세요.",
                    "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(tubPath))
                tubPath = "data";
            if (string.IsNullOrWhiteSpace(modelPath))
                modelPath = "models/pilot.keras";

            string wslMycarPath = ConvertToWslPath(mycarPath);
            string wslCondaPath = ConvertToWslPath(condaPath);
            string wslTubPath = ConvertToWslPath(tubPath);
            string wslModelPath = ConvertToWslPath(modelPath);

            string command =
                "export PYTHONUNBUFFERED=1 && " +
                "if [ ! -d " + QuotePathForBash(wslMycarPath) + " ]; then echo " +
                QuoteForBash("[error] Donkey 프로젝트 경로를 찾을 수 없습니다: " + wslMycarPath) + "; exit 2; fi && " +
                "if [ ! -x " + QuotePathForBash(wslCondaPath) + " ]; then echo " +
                QuoteForBash("[error] Conda 실행 파일을 찾을 수 없습니다: " + wslCondaPath) + "; exit 127; fi && " +
                "cd " + QuotePathForBash(wslMycarPath) + " && " +
                "if [ ! -f config.py ] && [ ! -f manage.py ]; then echo " +
                QuoteForBash("[error] Donkey 프로젝트 파일(config.py 또는 manage.py)을 찾을 수 없습니다: " + wslMycarPath) + "; exit 2; fi && " +
                QuotePathForBash(wslCondaPath) + " run --no-capture-output -n " + QuoteForBash(envName) + " " +
                "donkey train " +
                "--tub " + QuotePathForBash(wslTubPath) + " " +
                "--model " + QuotePathForBash(wslModelPath) + " " +
                "--type " + QuoteForBash(modelType);

            arguments.AddRange(new[] { "-d", wslDistro, "bash", "-lc", command });
            displayArguments = "-d " + QuoteProcessArgument(wslDistro) + " bash -lc " + QuoteForBash(command);
            return true;
        }

        private string ConvertToWslPath(string winPath)
        {
            if (string.IsNullOrWhiteSpace(winPath)) return winPath;
            string path = winPath.Trim().Replace("\\", "/");

            if (path.StartsWith("~/") || path.StartsWith("/"))
                return path;

            if (path.Length >= 2 && path[1] == ':')
            {
                char drive = char.ToLowerInvariant(path[0]);
                string rest = path.Length > 2 ? path.Substring(2).TrimStart('/') : string.Empty;
                return string.IsNullOrEmpty(rest)
                    ? $"/mnt/{drive}"
                    : $"/mnt/{drive}/{rest}";
            }

            return path;
        }

        private string QuoteForBash(string value)
        {
            return "'" + value.Replace("'", "'\"'\"'") + "'";
        }

        private string QuotePathForBash(string path)
        {
            if (path == "~") return path;

            if (path.StartsWith("~/"))
            {
                string rest = path.Substring(2);
                return string.IsNullOrEmpty(rest)
                    ? "~"
                    : "~/" + QuoteForBash(rest);
            }

            return QuoteForBash(path);
        }

        private async Task ReadStreamAsync(StreamReader reader)
        {
            char[] buffer = new char[512];

            try
            {
                while (true)
                {
                    int read = await reader.ReadAsync(buffer, 0, buffer.Length);
                    if (read <= 0) break;

                    AppendTrainingOutputChunk(new string(buffer, 0, read));
                }
            }
            catch (ObjectDisposedException)
            {
                // 프로세스 강제 종료 시 스트림이 먼저 닫힐 수 있습니다.
            }
            catch (IOException)
            {
                // 프로세스 종료와 동시에 pipe가 닫히는 경우는 정상 종료 흐름으로 취급합니다.
            }
            catch (InvalidOperationException)
            {
                // 리더가 닫힌 뒤 비동기 읽기가 이어지는 경우를 방어합니다.
            }
        }

        private void AppendTrainingLog(string text)
        {
            if (rtbTrainingOutput.IsDisposed) return;
            UpdateTrainingSummaryFromText(text);

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

        private void AppendTrainingOutputChunk(string chunk)
        {
            if (rtbTrainingOutput.IsDisposed || string.IsNullOrEmpty(chunk)) return;

            string sanitized = AnsiEscapeRegex.Replace(chunk, string.Empty).Replace("\0", string.Empty);
            if (sanitized.Length == 0) return;

            void AppendChunk()
            {
                ApplyTrainingOutputChunk(sanitized);
                rtbTrainingOutput.SelectionStart = rtbTrainingOutput.TextLength;
                rtbTrainingOutput.ScrollToCaret();
            }

            if (rtbTrainingOutput.InvokeRequired)
                rtbTrainingOutput.BeginInvoke(new Action(AppendChunk));
            else
                AppendChunk();
        }

        private void ApplyTrainingOutputChunk(string text)
        {
            foreach (char ch in text)
            {
                if (ch == '\r')
                {
                    UpdateTrainingSummaryFromText(_trainingOutputLineBuffer.ToString());
                    _trainingOutputLineBuffer.Clear();
                    RemoveLastTrainingOutputLine();
                    continue;
                }

                if (ch == '\n')
                {
                    UpdateTrainingSummaryFromText(_trainingOutputLineBuffer.ToString());
                    _trainingOutputLineBuffer.Clear();
                    rtbTrainingOutput.AppendText(Environment.NewLine);
                    continue;
                }

                _trainingOutputLineBuffer.Append(ch);
                rtbTrainingOutput.AppendText(ch.ToString());
            }
        }

        private void UpdateTrainingSummaryFromText(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText)) return;

            string text = rawText.Trim();
            if (text.Contains("Starting training", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Train with", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainingSummary(status: "학습 중");
            }

            if (text.Contains("학습 프로세스가 종료", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainingSummary(status: text.Contains("종료 코드: 0", StringComparison.OrdinalIgnoreCase) ? "완료" : "오류");
            }

            if (text.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Traceback", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Exception", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainingSummary(status: "오류");
            }

            Match epochMatch = Regex.Match(text, @"Epoch\s+(\d+)\s*/\s*(\d+)", RegexOptions.IgnoreCase);
            if (epochMatch.Success)
            {
                UpdateTrainingSummary(
                    status: "학습 중",
                    epoch: epochMatch.Groups[1].Value + "/" + epochMatch.Groups[2].Value,
                    progress: "0%");
            }

            Match progressMatch = Regex.Match(text, @"(?<current>\d+)\s*/\s*(?<total>\d+).*?(?:loss:\s*(?<loss>[0-9.]+))?", RegexOptions.IgnoreCase);
            if (progressMatch.Success &&
                int.TryParse(progressMatch.Groups["current"].Value, out int currentStep) &&
                int.TryParse(progressMatch.Groups["total"].Value, out int totalStep) &&
                totalStep > 0)
            {
                int percent = Math.Max(0, Math.Min(100, currentStep * 100 / totalStep));
                string? loss = progressMatch.Groups["loss"].Success ? progressMatch.Groups["loss"].Value : null;
                UpdateTrainingSummary(progress: percent + "%", loss: loss);
            }

            Match lossMatch = Regex.Match(text, @"(?:^|\s)loss:\s*(?<loss>[0-9.]+)", RegexOptions.IgnoreCase);
            if (lossMatch.Success)
                UpdateTrainingSummary(loss: lossMatch.Groups["loss"].Value);
        }

        private void UpdateTrainingSummary(
            string? status = null,
            string? epoch = null,
            string? progress = null,
            string? loss = null)
        {
            void Apply()
            {
                if (_lblTrainingSummaryStatus != null && status != null)
                {
                    _lblTrainingSummaryStatus.Text = "상태: " + status;
                    _lblTrainingSummaryStatus.ForeColor = status.Contains("오류") || status.Contains("중지")
                        ? System.Drawing.Color.Firebrick
                        : System.Drawing.Color.ForestGreen;
                }

                if (_lblTrainingSummaryEpoch != null && epoch != null)
                    _lblTrainingSummaryEpoch.Text = "Epoch: " + epoch;

                if (_lblTrainingSummaryProgress != null && progress != null)
                    _lblTrainingSummaryProgress.Text = "진행률: " + progress;

                if (_lblTrainingSummaryLoss != null && loss != null)
                    _lblTrainingSummaryLoss.Text = "loss: " + loss;
            }

            if (grpTrainingConfig.InvokeRequired)
                grpTrainingConfig.BeginInvoke(new Action(Apply));
            else
                Apply();
        }

        private void RemoveLastTrainingOutputLine()
        {
            string text = rtbTrainingOutput.Text;
            int lastLineStart = text.LastIndexOf('\n');
            lastLineStart = lastLineStart < 0 ? 0 : lastLineStart + 1;

            if (lastLineStart < text.Length)
            {
                rtbTrainingOutput.Select(lastLineStart, text.Length - lastLineStart);
                rtbTrainingOutput.SelectedText = string.Empty;
            }
        }

        private async void BtnDetectTrainingEnvironment_Click(object? sender, EventArgs e)
        {
            await DetectTrainingEnvironmentAsync(showSuccessMessage: true, clearLog: true);
        }

        private async Task<bool> DetectTrainingEnvironmentAsync(bool showSuccessMessage, bool clearLog)
        {
            btnDetectTrainingEnvironment.Enabled = false;
            if (clearLog)
                rtbTrainingOutput.Clear();
            AppendTrainingLog("[감지] 학습 환경 자동 감지를 시작합니다.");

            try
            {
                var distros = await DetectWslDistrosAsync();
                if (!ApplyDetectedWslDistros(distros, requireExistingSelectionWhenMultiple: false, showMessage: showSuccessMessage))
                    return false;

                string distro = NormalizeWslDistroName(cmbTrainingWslDistro.Text);
                bool condaDetected = await DetectCondaPathsAsync(distro, showMessage: showSuccessMessage);
                if (!condaDetected)
                    return false;

                await DetectCondaEnvironmentsAsync(distro);
                bool projectDetected = await DetectDonkeyProjectsAsync(distro);
                if (!projectDetected)
                    return false;

                if (string.IsNullOrWhiteSpace(txtTrainingPythonEnvName.Text) ||
                    string.IsNullOrWhiteSpace(cmbMycarProjectPath.Text))
                {
                    AppendTrainingLog("[감지] Python 환경명 또는 Donkey 프로젝트 경로를 자동으로 확정하지 못했습니다.");
                    return false;
                }

                if (showSuccessMessage)
                {
                    MessageBox.Show("자동 감지가 완료되었습니다.\n감지된 값이 맞는지 확인한 뒤 학습을 시작해 주세요.",
                        "자동 감지", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (showSuccessMessage)
                {
                    MessageBox.Show("자동 감지 중 오류가 발생했습니다.\n\n오류 내용: " + ex.Message,
                        "자동 감지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    AppendTrainingLog("[감지 오류] " + ex.Message);
                }

                return false;
            }
            finally
            {
                btnDetectTrainingEnvironment.Enabled = true;
            }
        }

        private async void BtnSelectTrainingWslDistro_Click(object? sender, EventArgs e)
        {
            btnSelectTrainingWslDistro.Enabled = false;
            AppendTrainingLog("[감지] WSL Ubuntu 목록을 확인합니다.");

            try
            {
                var distros = await DetectWslDistrosAsync();
                ApplyDetectedWslDistros(distros, requireExistingSelectionWhenMultiple: false, showMessage: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("WSL Ubuntu 목록 확인 중 오류가 발생했습니다.\n\n오류 내용: " + ex.Message,
                    "WSL Ubuntu 감지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSelectTrainingWslDistro.Enabled = true;
            }
        }

        private async void BtnSelectCondaPath_Click(object? sender, EventArgs e)
        {
            string distro = NormalizeWslDistroName(cmbTrainingWslDistro.Text);
            if (string.IsNullOrWhiteSpace(distro))
            {
                MessageBox.Show("먼저 WSL Ubuntu를 선택해 주세요.",
                    "Conda 경로 감지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSelectCondaPath.Enabled = false;
            AppendTrainingLog("[감지] Conda 경로를 확인합니다. WSL Ubuntu: " + distro);

            try
            {
                string condaPath = await DetectCondaPathAsync(distro);
                if (string.IsNullOrWhiteSpace(condaPath))
                {
                    MessageBox.Show("Conda 경로를 찾을 수 없습니다.",
                        "Conda 경로 감지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppendTrainingLog("[감지] Conda 경로를 찾지 못했습니다.");
                    return;
                }

                SetComboText(cmbCondaPath, condaPath);
                AppendTrainingLog("[감지] Conda 경로: " + condaPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Conda 경로 확인 중 오류가 발생했습니다.\n\n오류 내용: " + ex.Message,
                    "Conda 경로 감지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSelectCondaPath.Enabled = true;
            }
        }

        private async Task<List<string>> DetectWslDistrosAsync()
        {
            var distroResult = await RunProcessCaptureAsync("wsl", new[] { "-l", "-q" }, 10000);
            if (distroResult.ExitCode != 0)
            {
                AppendTrainingLog("[감지 오류] WSL 목록을 가져오지 못했습니다.");
                AppendTrainingLog(distroResult.Error);
                MessageBox.Show("WSL Ubuntu 목록을 가져오지 못했습니다.\nWSL 설치 상태를 확인해 주세요.",
                    "자동 감지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return new List<string>();
            }

            return ParseLines(distroResult.Output)
                .Select(NormalizeWslDistroName)
                .Where(line => !line.Contains("Windows Subsystem", StringComparison.OrdinalIgnoreCase))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private bool ApplyDetectedWslDistros(List<string> distros, bool requireExistingSelectionWhenMultiple, bool showMessage)
        {
            cmbTrainingWslDistro.Items.Clear();
            cmbTrainingWslDistro.Items.AddRange(distros.Cast<object>().ToArray());

            if (distros.Count == 0)
            {
                if (showMessage)
                {
                    MessageBox.Show("감지된 WSL Ubuntu가 없습니다.\nUbuntu 설치 후 다시 자동 감지를 실행해 주세요.",
                        "자동 감지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }

            if (distros.Count == 1)
            {
                cmbTrainingWslDistro.Text = distros[0];
                AppendTrainingLog("[감지] WSL Ubuntu: " + distros[0]);
                return true;
            }

            string selected = NormalizeWslDistroName(cmbTrainingWslDistro.Text);
            bool hasValidSelection = distros.Contains(selected, StringComparer.OrdinalIgnoreCase);
            if (!hasValidSelection && !requireExistingSelectionWhenMultiple)
                cmbTrainingWslDistro.Text = distros[0];

            AppendTrainingLog("[감지] 여러 WSL Ubuntu가 감지되었습니다: " + string.Join(", ", distros));
            if (showMessage)
            {
                MessageBox.Show(
                    "WSL Ubuntu가 여러 개 감지되었습니다.\n목록에서 학습 환경이 설치된 Ubuntu를 선택해 주세요.",
                    "자동 감지", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return hasValidSelection || !requireExistingSelectionWhenMultiple;
        }

        private async Task<bool> DetectCondaPathsAsync(string distro, bool showMessage)
        {
            distro = NormalizeWslDistroName(distro);
            string condaPath = await DetectCondaPathAsync(distro);
            cmbCondaPath.Items.Clear();

            if (string.IsNullOrWhiteSpace(condaPath))
            {
                AppendTrainingLog("[감지] Conda 경로를 찾지 못했습니다.");
                if (showMessage)
                {
                    MessageBox.Show("Conda 경로를 찾을 수 없습니다.",
                        "자동 감지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }

            SetComboText(cmbCondaPath, condaPath);
            AppendTrainingLog("[감지] Conda 경로: " + condaPath);
            return true;
        }

        private Task<string> DetectCondaPathAsync(string distro)
        {
            distro = NormalizeWslDistroName(distro);

            return Task.Run(async () =>
            {
                var whichResult = await RunWslBashCaptureAsync(distro, "which conda", 10000);
                string detected = FirstNonEmptyLine(whichResult.Output);
                if (!string.IsNullOrWhiteSpace(detected))
                    return detected.Trim();

                string defaultPathScript =
                    "ls ~/miniconda3/bin/conda 2>/dev/null || ls ~/anaconda3/bin/conda 2>/dev/null || ls ~/miniforge3/bin/conda 2>/dev/null";
                var defaultPathResult = await RunWslBashCaptureAsync(distro, defaultPathScript, 10000, useLoginShell: false);
                detected = FirstNonEmptyLine(defaultPathResult.Output);
                if (!string.IsNullOrWhiteSpace(detected))
                    return detected.Trim();

                string searchScript = "find ~ -type f -name 'conda' -path '*/bin/conda' 2>/dev/null | head -n 1";
                var searchResult = await RunWslBashCaptureAsync(distro, searchScript, 30000, useLoginShell: false);
                detected = FirstNonEmptyLine(searchResult.Output);
                return detected.Trim();
            });
        }

        private string FirstNonEmptyLine(string text)
        {
            return ParseLines(text).FirstOrDefault() ?? string.Empty;
        }

        private async Task DetectCondaEnvironmentsAsync(string distro)
        {
            distro = NormalizeWslDistroName(distro);
            string condaPath = cmbCondaPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(condaPath)) return;

            string script = QuotePathForBash(ConvertToWslPath(condaPath)) + " env list";
            var result = await RunWslBashCaptureAsync(distro, script, 20000);
            if (result.ExitCode != 0)
            {
                AppendTrainingLog("[감지 오류] Conda 환경 목록 조회가 실패했습니다.");
                AppendTrainingLog(result.Error);
                AppendTrainingLog(result.Output);
                return;
            }

            var envs = ParseCondaEnvNames(result.Output).ToList();
            if (envs.Count == 0)
            {
                AppendTrainingLog("[감지] Conda 환경 목록을 가져오지 못했습니다.");
                return;
            }

            string? selected = null;
            foreach (string env in envs.Take(12))
            {
                string checkScript = QuotePathForBash(ConvertToWslPath(condaPath)) +
                    " run -n " + QuoteForBash(env) +
                    " python -c \"import donkeycar, tensorflow; print(donkeycar.__version__)\"";
                var check = await RunWslBashCaptureAsync(distro, checkScript, 30000);
                if (check.ExitCode == 0)
                {
                    selected = env;
                    AppendTrainingLog("[감지] Donkey/TensorFlow 사용 가능 환경: " + env);
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(selected))
                txtTrainingPythonEnvName.Text = selected;
            else
                AppendTrainingLog("[감지] Donkey와 TensorFlow를 모두 import할 수 있는 Conda 환경을 찾지 못했습니다.");
        }

        private async Task<bool> DetectDonkeyProjectsAsync(string distro)
        {
            distro = NormalizeWslDistroName(distro);
            string script = "find \"$HOME\" -maxdepth 6 -type f \\( -name config.py -o -name manage.py \\) 2>/dev/null";
            var result = await RunWslBashCaptureAsync(distro, script, 20000);
            if (result.ExitCode != 0)
            {
                AppendTrainingLog("[감지 오류] Donkey 프로젝트 경로 감지 명령이 실패했습니다.");
                AppendTrainingLog("[감지 오류] 사용한 WSL Ubuntu: " + distro);
                AppendTrainingLog(result.Error);
                AppendTrainingLog(result.Output);
                return false;
            }

            var projectPaths = ParseLines(result.Output)
                .Select(GetLinuxDirectoryName)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Where(path => !path.Contains("/miniconda3/", StringComparison.Ordinal))
                .Where(path => !path.Contains("/anaconda3/", StringComparison.Ordinal))
                .Where(path => !path.Contains("/.conda/", StringComparison.Ordinal))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (projectPaths.Count == 0)
            {
                AppendTrainingLog("[감지] Donkey 프로젝트 후보를 찾지 못했습니다.");
                return false;
            }

            if (projectPaths.Count == 1)
            {
                SetComboText(cmbMycarProjectPath, projectPaths[0]);
                AppendTrainingLog("[감지] Donkey 프로젝트 경로: " + projectPaths[0]);
                return true;
            }

            cmbMycarProjectPath.Items.Clear();
            cmbMycarProjectPath.Items.AddRange(projectPaths.Cast<object>().ToArray());

            AppendTrainingLog("[감지] Donkey 프로젝트 후보:");
            foreach (string path in projectPaths)
                AppendTrainingLog("  - " + path);

            string? selectedProject = ShowDonkeyProjectSelectionDialog(projectPaths);
            if (string.IsNullOrWhiteSpace(selectedProject))
            {
                AppendTrainingLog("[감지] Donkey 프로젝트 선택이 취소되었습니다.");
                return false;
            }

            SetComboText(cmbMycarProjectPath, selectedProject);
            AppendTrainingLog("[감지] 선택된 Donkey 프로젝트 경로: " + selectedProject);
            return true;
        }

        /// <summary>
        /// Donkey 프로젝트 후보가 여러 개일 때 사용자가 직접 고르게 하는 작은 선택 창입니다.
        /// Designer 파일을 수정하지 않기 위해 코드에서 일회성 대화창을 만듭니다.
        /// </summary>
        private string? ShowDonkeyProjectSelectionDialog(IReadOnlyList<string> projectPaths)
        {
            using var dialog = new Form
            {
                Text = "Donkey 프로젝트 선택",
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = new Size(640, 320),
                Font = new Font("맑은 고딕", 10F)
            };

            var description = new System.Windows.Forms.Label
            {
                Text = "자동 감지된 Donkey 프로젝트 후보입니다.\n학습에 사용할 프로젝트 폴더를 선택한 뒤 확인을 누르세요.",
                Location = new Point(18, 16),
                Size = new Size(600, 48)
            };

            var list = new ListBox
            {
                Location = new Point(18, 72),
                Size = new Size(600, 170),
                HorizontalScrollbar = true
            };
            list.Items.AddRange(projectPaths.Cast<object>().ToArray());
            list.SelectedIndex = 0;

            var okButton = new Button
            {
                Text = "확인",
                DialogResult = DialogResult.OK,
                Location = new Point(386, 262),
                Size = new Size(110, 34)
            };

            var cancelButton = new Button
            {
                Text = "취소",
                DialogResult = DialogResult.Cancel,
                Location = new Point(508, 262),
                Size = new Size(110, 34)
            };

            dialog.Controls.Add(description);
            dialog.Controls.Add(list);
            dialog.Controls.Add(okButton);
            dialog.Controls.Add(cancelButton);
            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;

            return dialog.ShowDialog(this) == DialogResult.OK
                ? list.SelectedItem?.ToString()
                : null;
        }

        private string GetLinuxDirectoryName(string path)
        {
            string normalized = path.Trim().Replace("\\", "/");
            int index = normalized.LastIndexOf('/');
            return index > 0 ? normalized.Substring(0, index) : string.Empty;
        }

        private async Task<(int ExitCode, string Output, string Error)> RunWslBashCaptureAsync(string distro, string script, int timeoutMs)
        {
            return await RunWslBashCaptureAsync(distro, script, timeoutMs, useLoginShell: true);
        }

        private async Task<(int ExitCode, string Output, string Error)> RunWslBashCaptureAsync(string distro, string script, int timeoutMs, bool useLoginShell)
        {
            distro = NormalizeWslDistroName(distro);
            string shellOption = useLoginShell ? "-lc" : "-c";
            return await RunProcessCaptureAsync("wsl", new[] { "-d", distro, "bash", shellOption, script }, timeoutMs);
        }

        private async Task<(int ExitCode, string Output, string Error)> RunProcessCaptureAsync(string fileName, string arguments, int timeoutMs)
        {
            return await RunProcessCaptureAsync(fileName, SplitProcessArguments(arguments), timeoutMs);
        }

        private async Task<(int ExitCode, string Output, string Error)> RunProcessCaptureAsync(string fileName, IEnumerable<string> arguments, int timeoutMs)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            foreach (string argument in arguments)
                process.StartInfo.ArgumentList.Add(argument);

            var output = new StringBuilder();
            var error = new StringBuilder();
            process.Start();
            Task outputTask = process.StandardOutput.ReadToEndAsync().ContinueWith(task => output.Append(task.Result));
            Task errorTask = process.StandardError.ReadToEndAsync().ContinueWith(task => error.Append(task.Result));
            Task exitTask = process.WaitForExitAsync();

            Task completed = await Task.WhenAny(exitTask, Task.Delay(timeoutMs));
            if (completed != exitTask)
            {
                try { process.Kill(true); } catch { }
                return (-1, output.ToString(), "명령 실행 시간이 초과되었습니다.");
            }

            await Task.WhenAll(outputTask, errorTask);
            return (process.ExitCode, output.ToString(), error.ToString());
        }

        private IEnumerable<string> SplitProcessArguments(string arguments)
        {
            return arguments
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(argument => argument.Trim());
        }

        private IEnumerable<string> ParseLines(string text)
        {
            return text
                .Replace("\0", string.Empty)
                .Replace("\u001c", string.Empty)
                .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => NormalizeWslText(line))
                .Where(line => !line.Contains("Wsl/Service/", StringComparison.OrdinalIgnoreCase))
                .Where(line => !string.IsNullOrWhiteSpace(line));
        }

        private string NormalizeWslText(string text)
        {
            return text
                .Replace("\0", string.Empty)
                .Replace("\u001c", string.Empty)
                .Trim();
        }

        private string NormalizeWslDistroName(string distro)
        {
            string normalized = NormalizeWslText(distro);
            normalized = normalized.Replace("*", string.Empty).Trim();

            int runningIndex = normalized.IndexOf(" Running", StringComparison.OrdinalIgnoreCase);
            if (runningIndex >= 0)
                normalized = normalized.Substring(0, runningIndex).Trim();

            int stoppedIndex = normalized.IndexOf(" Stopped", StringComparison.OrdinalIgnoreCase);
            if (stoppedIndex >= 0)
                normalized = normalized.Substring(0, stoppedIndex).Trim();

            return normalized;
        }

        private IEnumerable<string> ParseCondaEnvNames(string text)
        {
            foreach (string line in ParseLines(text))
            {
                if (line.StartsWith("#")) continue;
                string normalized = Regex.Replace(line, @"\s+", " ").Trim();
                string[] parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                string envName = parts[0] == "*" && parts.Length > 1 ? parts[1] : parts[0];
                if (envName.Contains('/') || envName.Contains('\\')) continue;
                yield return envName;
            }
        }

        private string QuoteProcessArgument(string value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
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

        private void SelectFolderInto(ComboBox targetComboBox, string title)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = title,
                SelectedPath = Directory.Exists(targetComboBox.Text) ? targetComboBox.Text : string.Empty
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                SetComboText(targetComboBox, dialog.SelectedPath);
        }

        private void BtnSelectTrainingModelPath_Click(object? sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Title = "모델 저장 경로 선택",
                Filter = "Keras Model (*.keras)|*.keras|H5 Model (*.h5)|*.h5|All files (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(txtTrainingModelPath.Text)
                    ? "pilot.keras"
                    : Path.GetFileName(txtTrainingModelPath.Text)
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                txtTrainingModelPath.Text = dialog.FileName;
        }

        private void BtnSaveTrainingConfig_Click(object? sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(Application.UserAppDataPath);
                var settings = new TrainingSettings
                {
                    WslDistroName = cmbTrainingWslDistro.Text.Trim(),
                    CondaPath = cmbCondaPath.Text.Trim(),
                    MycarPath = cmbMycarProjectPath.Text.Trim(),
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

                if (!string.IsNullOrWhiteSpace(settings.MycarPath))
                    SetComboText(cmbMycarProjectPath, settings.MycarPath);
                if (!string.IsNullOrWhiteSpace(settings.TubPath))
                    txtTrainingTubPath.Text = settings.TubPath;
                if (!string.IsNullOrWhiteSpace(settings.ModelPath))
                    txtTrainingModelPath.Text = settings.ModelPath;
                if (!string.IsNullOrWhiteSpace(settings.ModelType))
                    cmbTrainingModelType.Text = settings.ModelType;
                if (!string.IsNullOrWhiteSpace(settings.PythonEnvName))
                    txtTrainingPythonEnvName.Text = settings.PythonEnvName;
                SetComboText(cmbTrainingWslDistro, settings.WslDistroName);
                SetComboText(cmbCondaPath, settings.CondaPath);
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

        private void SetComboText(ComboBox comboBox, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!comboBox.Items.Cast<object>().Any(item => string.Equals(item.ToString(), value, StringComparison.Ordinal)))
                comboBox.Items.Add(value);
            comboBox.Text = value;
        }

        private sealed class TrainingSettings
        {
            public string WslDistroName { get; set; } = "";
            public string CondaPath { get; set; } = "";
            public string MycarPath { get; set; } = "";
            public string TubPath { get; set; } = "data";
            public string ModelPath { get; set; } = "models/pilot.keras";
            public string ModelType { get; set; } = "linear";
            public string PythonEnvName { get; set; } = "";
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
        private void BtnSelectMycarPath_Click(object sender, EventArgs e) => SelectFolderInto(cmbMycarProjectPath, "Donkey 프로젝트 폴더 선택");
        private void GrpDataCleaner_Enter(object sender, EventArgs e) { }

        // ScottPlot 차트를 필요할 때 생성합니다.

        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabGraphStats && _isChartDirty)
                RenderFrameChart();

            if (tabControlMain.SelectedTab == tabTrainingMonitor &&
                !_hasAutoDetectedTrainingTab &&
                !_isTrainingAutoDetectRunning)
            {
                _ = AutoDetectTrainingTabOnFirstOpenAsync();
            }
        }

        private async Task AutoDetectTrainingTabOnFirstOpenAsync()
        {
            if (_trainingProcess is { HasExited: false })
                return;

            _isTrainingAutoDetectRunning = true;
            stsTrainingStatus.Text = "학습 상태: 자동 감지 중";

            try
            {
                bool detected = await DetectTrainingEnvironmentAsync(showSuccessMessage: false, clearLog: true);
                _hasAutoDetectedTrainingTab = detected;
                stsTrainingStatus.Text = detected
                    ? "학습 상태: 자동 감지 완료"
                    : "학습 상태: 자동 감지 필요";
            }
            finally
            {
                _isTrainingAutoDetectRunning = false;
            }
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
