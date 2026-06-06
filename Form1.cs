using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private FormsPlot? _dataViewerFrameChart;
        private ScottPlot.Plottables.VerticalLine? _dataViewerPlayheadLine;
        private HScrollBar? _timelineScrollBar;
        private const int TimelineVisibleFrameWindow = 100;
        private int _timelineViewStart = 0;
        private bool _isSyncingTimelineScrollBar = false;
        private bool _showDeletedOnGraph = true;
        private HScrollBar? _filmstripScrollBar;
        private Button? _btnReturnFilmstripToCurrent;
        private int _filmstripViewOffsetPixels = 0;
        private bool _isSyncingFilmstripScrollBar = false;
        private bool _isFilmstripViewLockedByUser = false;
        private bool _isDraggingFilmstripSelection = false;
        private bool _filmstripDragMoved = false;
        private bool _suppressNextFilmstripClick = false;
        private int _filmstripDragStartIndex = -1;
        private int _lastFilmstripDragIndex = -1;
        private int _lastSelectedFrameIndex = -1;
        private bool _catalogDragMoved = false;
        private int _lastCatalogDragIndex = -1;
        private int _dragPreviewSelectionStartIndex = -1;
        private int _dragPreviewSelectionEndIndex = -1;
        private DateTime _lastDragChartRenderUtc = DateTime.MinValue;
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
        private bool _isValidationEnvironmentReady = false;
        private readonly HttpClient _pilotHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        private CancellationTokenSource? _pilotPredictionCts;
        private int _pilotPredictionRequestVersion = 0;
        private TabPage? tabPageModelValidation;
        private PictureBox? picValidationPreview;
        private Panel? pnlValidationBars;
        private TextBox? txtPilotApiUrl;
        private System.Windows.Forms.Label? lblPilotStatus;
        private System.Windows.Forms.Label? lblPilotActualAngle;
        private System.Windows.Forms.Label? lblPilotActualThrottle;
        private System.Windows.Forms.Label? lblPilotPredictedAngle;
        private System.Windows.Forms.Label? lblPilotPredictedThrottle;
        private Button? btnPilotHealthCheck;
        private TextBox? txtValidationDataFolderPath;
        private TextBox? txtValidationModelPath;
        private System.Windows.Forms.Label? lblValidationEnvironmentStatus;
        private Button? btnSelectValidationDataFolder;
        private Button? btnSelectValidationModelPath;
        private Button? btnDetectValidationEnvironment;
        private Button? btnStartPilotServer;
        private Button? btnStopPilotServer;
        private Button? btnLoadValidationDataFolder;
        private Button? btnShowPredictionMismatchCandidates;
        private RichTextBox? rtbPilotServerLog;
        private Process? _pilotServerProcess;
        private Process? _trainingProcess;
        private bool _isPrecomputingPilotPredictions;
        private CancellationTokenSource? _pilotBatchPredictionCts;
        private readonly Dictionary<int, Bitmap> _thumbnailCache = new();
        private int _previousThumbnailHighlightIndex = -1;
        private Button? _btnShowReviewCandidates;
        private Button? _btnToggleDeletedGraph;
        private readonly StringBuilder _trainingOutputLineBuffer = new StringBuilder();
        private System.Windows.Forms.Label? _lblTrainingSummaryTitle;
        private System.Windows.Forms.Label? _lblTrainingSummaryStatus;
        private System.Windows.Forms.Label? _lblTrainingSummaryEpoch;
        private System.Windows.Forms.Label? _lblTrainingSummaryProgress;
        private System.Windows.Forms.Label? _lblTrainingSummaryLoss;
        private Button? _btnOpenTrainedModelValidation;
        private CheckBox? _chkRestartPilotServerAfterTraining;
        private System.Windows.Forms.Label? _lblCleanupSummary;
        private System.Windows.Forms.Label? _lblCleanupWorkflowHint;
        private System.Windows.Forms.Label? _lblFrameReviewHint;
        private ComboBox? _cmbPlaybackSpeed;
        private ComboBox? _cmbValidationPlaybackSpeed;
        private readonly ToolTip _buttonToolTip = new ToolTip();
        private Button? btnValidationAutoPlay;
        private Button? btnValidationFirst;
        private Button? btnValidationPrevious;
        private Button? btnValidationNext;
        private Button? btnValidationLast;
        private bool _isPilotPredictionInFlight = false;
        private readonly List<double> _trainingLossEpochs = new();
        private readonly List<double> _trainingLossValues = new();
        private readonly List<double> _validationLossEpochs = new();
        private readonly List<double> _validationLossValues = new();
        private TrainingLossGraphForm? _trainingLossGraphForm;
        private int _currentTrainingEpochForGraph = 0;
        private int _currentTrainingStepForGraph = 0;
        private int _currentTrainingTotalStepsForGraph = 0;
        private bool _isTrainingAutoDetectRunning = false;
        private const string DeletedFramesMetaFileName = "deleted_frames_meta.txt";
        private const string TrainingSettingsFileName = "training_settings.json";
        private static readonly Regex AnsiEscapeRegex = new(@"\x1B(?:[@-Z\\-_]|\[[0-?]*[ -/]*[@-~])", RegexOptions.Compiled);

        // 외부 폰트 콜렉션 추가
        PrivateFontCollection mainFonts = new PrivateFontCollection();
        PrivateFontCollection cliFonts = new PrivateFontCollection();

        private void ApplyFont(Control parent, Font font)
        {
            foreach (Control control in parent.Controls)
            {
                control.Font = font;

                if (control.Controls.Count > 0)
                {
                    ApplyFont(control, font);
                }
            }
        }

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

        private enum TurnDirection
        {
            None,
            Left,
            Right
        }

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.Size = new Size(1600, 1000);

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
            btnClearFrameFilter.Click += BtnClearFrameFilter_Click;
            btnExcludeFrameRange.Click += BtnExcludeFrameRange_Click;
            btnExcludeSelectedFrames.Click += BtnExcludeSelectedFrames_Click;
            btnExportCleanDataset.Click += BtnExportCleanDataset_Click;
            btnRestoreFrames.Click += BtnRestoreFrames_Click;
            btnSaveCleanupState.Click += BtnSaveCleanupState_Click;
            btnStartTrainingProcess.Click += BtnStartTrainingProcess_Click;
            btnStopTrainingProcess.Click += BtnStopTrainingProcess_Click;
            btnSaveTrainingConfig.Click += BtnSaveTrainingConfig_Click;
            btnSelectTrainingTubPath.Click += (_, _) => SelectTrainingTubFolder();
            btnSelectTrainingModelPath.Click += BtnSelectTrainingModelPath_Click;
            btnDetectTrainingEnvironment.Click += BtnDetectTrainingEnvironment_Click;
            btnSelectTrainingWslDistro.Click += BtnSelectTrainingWslDistro_Click;
            btnSelectCondaPath.Click += BtnSelectCondaPath_Click;
            cmbScenarioFilter.SelectedIndexChanged += CmbScenarioFilter_SelectedIndexChanged;
            cmbScenarioFilter.TextChanged += CmbScenarioFilter_SelectedIndexChanged;

            mnuFileOpenDataFolder.Click += (s, _) => BtnOpenDataFolder_Click(s!, EventArgs.Empty);
            mnuFileReloadData.Click += (s, _) => BtnReloadData_Click(s!, EventArgs.Empty);
            mnuExit.Click += (s, _) => Application.Exit();
            mnuHelpOpenTutorial.Click += (s, _) => RunFeatureTutorial("도움말");

            tabControlMain.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            FormClosing += Form1_FormClosing;

            ConfigureFrameCatalogGrid();
            ApplyDataManagerUiStyle();
            ConfigurePlaybackSpeedComboBox();
            ConfigureModelValidationTab();
            ConfigureDeletedGraphToggleButton();
            ConfigureReviewCandidateControls();
            ConfigureCleanupGuidanceControls();
            ConfigureResponsiveDataViewerLayout();
            ArrangeDataCleanerPanel();

            btnExcludeSelectedFrames.Text = "선택 프레임 제외";
            btnExportCleanDataset.Text = "클린 폴더 추출";
            btnRestoreFrames.Text = "복원";
            txtAngleMinFilter.Text = "-1";
            txtAngleMaxFilter.Text = "1";
            txtThrottleMinFilter.Text = "-1";
            txtThrottleMaxFilter.Text = "1";

            InitializeTrainingControls();
            LoadTrainingSettings();
            UpdateStatusLabels();
            ConfigureButtonToolTips();
            BeginInvoke(new Action(AskFirstUseTutorial));

            // 로드 시 폰트 불러오기
            mainFonts.AddFontFile("resource/PretendardVariable.ttf");
            cliFonts.AddFontFile("resource/JetBrainsMono-Medium.ttf");

            Font myFont1 = new Font(mainFonts.Families[0], 11f);
            Font myFont2 = new Font(cliFonts.Families[0], 12f);

            ApplyFont(this, myFont1);
            rtbTrainingOutput.Font = myFont2;
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

        private void ConfigureButtonToolTips()
        {
            _buttonToolTip.AutoPopDelay = 10000;
            _buttonToolTip.InitialDelay = 450;
            _buttonToolTip.ReshowDelay = 120;
            _buttonToolTip.ShowAlways = true;

            SetButtonToolTip(btnOpenDataFolder, "DonkeyCar Tub 데이터 폴더를 선택해서 프레임 목록과 이미지를 불러옵니다.");
            SetButtonToolTip(btnReloadData, "현재 선택된 데이터 폴더를 다시 읽어 목록과 미리보기를 갱신합니다.");
            SetButtonToolTip(btnToggleTheme, "화면 테마를 밝은 모드와 어두운 모드로 전환합니다.");
            SetButtonToolTip(btnGuide, "현재 작업 순서와 주요 기능 설명을 다시 확인합니다.");
            SetButtonToolTip(btnFirst, "현재 표시 목록의 첫 번째 프레임으로 이동합니다.");
            SetButtonToolTip(btnPrev, "현재 프레임 바로 이전 프레임으로 이동합니다.");
            SetButtonToolTip(btnNext, "현재 프레임 바로 다음 프레임으로 이동합니다.");
            SetButtonToolTip(btnLast, "현재 표시 목록의 마지막 프레임으로 이동합니다.");
            SetButtonToolTip(btnAutoPlay, "프레임을 선택한 배속으로 자동 재생하거나 일시정지합니다.");
            SetButtonToolTip(btnApplyFrameFilter, "입력한 방향값, 속도값, 주행 방식, 상황 조건으로 프레임을 검색합니다.");
            SetButtonToolTip(btnClearFrameFilter, "검색 조건을 해제하고 전체 프레임 목록으로 돌아갑니다.");
            SetButtonToolTip(btnExcludeSelectedFrames, "선택한 프레임을 학습에서 제외합니다. 원본 파일은 삭제하지 않습니다.");
            SetButtonToolTip(btnRestoreFrames, "제외 처리된 프레임을 다시 학습 사용 상태로 복원합니다.");
            SetButtonToolTip(btnSaveCleanupState, "현재 제외/복원 상태를 메타 파일로 저장합니다.");
            SetButtonToolTip(btnExportCleanDataset, "제외 프레임을 뺀 학습용 Clean 폴더를 새로 만듭니다.");
            SetButtonToolTip(btnStartTrainingProcess, "선택한 Tub 데이터와 설정으로 DonkeyCar 모델 학습을 시작합니다.");
            SetButtonToolTip(btnStopTrainingProcess, "실행 중인 학습 프로세스를 중지합니다. 기존 최종 모델은 유지됩니다.");
            SetButtonToolTip(btnSaveTrainingConfig, "현재 학습 경로와 환경 설정을 저장합니다.");
            SetButtonToolTip(btnDetectTrainingEnvironment, "WSL, Conda, DonkeyCar 프로젝트와 Python 환경을 자동으로 찾습니다.");
            SetButtonToolTip(btnLoadValidationDataFolder, "추론/검증에 사용할 데이터 폴더를 데이터 확인 탭에도 불러옵니다.");
            SetButtonToolTip(btnDetectValidationEnvironment, "데이터 폴더와 모델 파일을 기준으로 추론 서버 실행 환경을 확인합니다.");
            SetButtonToolTip(btnStartPilotServer, "선택한 모델 파일로 Python 추론 서버를 시작합니다.");
            SetButtonToolTip(btnStopPilotServer, "현재 실행 중인 Python 추론 서버를 중지합니다.");
            SetButtonToolTip(btnPilotHealthCheck, "추론 서버가 응답하는지 확인합니다.");
            SetButtonToolTip(btnValidationFirst, "추론/검증 프레임을 처음으로 이동합니다.");
            SetButtonToolTip(btnValidationPrevious, "추론/검증 프레임을 이전으로 이동합니다.");
            SetButtonToolTip(btnValidationNext, "추론/검증 프레임을 다음으로 이동합니다.");
            SetButtonToolTip(btnValidationLast, "추론/검증 프레임을 마지막으로 이동합니다.");
            SetButtonToolTip(btnValidationAutoPlay, "추론/검증 프레임을 선택한 배속으로 자동 재생합니다.");
            SetButtonToolTip(btnShowPredictionMismatchCandidates, "실제 방향/속도값과 모델 예측값 차이가 큰 프레임만 모아 표시합니다.");
            SetButtonToolTip(_btnOpenTrainedModelValidation, "방금 학습한 모델을 추론/검증 탭으로 연결하고 서버를 시작합니다.");
            SetButtonToolTip(_chkRestartPilotServerAfterTraining, "학습이 성공하면 추론 서버를 새 모델 기준으로 다시 시작합니다.");
        }

        private void SetButtonToolTip(Control? control, string description)
        {
            if (control == null) return;
            _buttonToolTip.SetToolTip(control, description);
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
                Font = new Font("resource/PretendardVariable.ttf", 10F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            string? selectedSection = null;
            var sections = new[] { "데이터 보기", "정리", "학습", "추론/검증", "그래프" };
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
                new TutorialStep("데이터 보기", "재생 속도", "자동 재생 속도를 1배부터 10배까지 선택합니다. 숫자가 클수록 프레임이 더 빠르게 넘어갑니다.", _cmbPlaybackSpeed, tabPageDataViewer),
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
                new TutorialStep("학습", "학습 로그", "학습 실행 과정의 로그를 표시할 영역입니다.", grpTrainingOutput, tabTrainingMonitor),
                new TutorialStep("추론/검증", "추론/검증 사용 순서", "1. 데이터 폴더와 모델 파일을 선택합니다.\n2. 자동 감지로 WSL, Conda, Python 환경을 확인합니다.\n3. 서버 시작 후 프레임을 선택하면 실제 주행값과 모델 예측값을 비교합니다.", tabPageModelValidation, tabPageModelValidation),
                new TutorialStep("추론/검증", "데이터 폴더", "검증할 Tub 폴더를 선택합니다. 데이터 확인 탭에서 선택한 폴더와 다르게 지정할 수도 있습니다.", txtValidationDataFolderPath, tabPageModelValidation),
                new TutorialStep("추론/검증", "모델 파일", "비교할 DonkeyCar 모델 파일(.keras 또는 .h5)을 선택합니다. 이 모델의 예측값이 실제 주행값과 비교됩니다.", txtValidationModelPath, tabPageModelValidation),
                new TutorialStep("추론/검증", "자동 감지", "서버 시작 전에 WSL, Conda, Python 환경, DonkeyCar 의존성이 준비됐는지 확인합니다.", btnDetectValidationEnvironment, tabPageModelValidation),
                new TutorialStep("추론/검증", "서버 시작", "선택한 모델을 Python 추론 서버로 실행합니다. 서버가 정상일 때 프레임별 예측값을 받을 수 있습니다.", btnStartPilotServer, tabPageModelValidation),
                new TutorialStep("추론/검증", "프레임 검증", "현재 프레임 이미지 위에 실제 조향 방향과 모델 예측 방향을 함께 표시합니다. 값 차이가 크면 해당 구간을 검토 후보로 볼 수 있습니다.", picValidationPreview, tabPageModelValidation),
                new TutorialStep("추론/검증", "검증 재생", "처음/이전/다음/끝 버튼과 자동 재생, 배속 선택으로 검증 프레임을 빠르게 훑어볼 수 있습니다.", btnValidationAutoPlay, tabPageModelValidation)
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
                Font = new Font("resource/PretendardVariable.ttf", 10F),
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

        private void CmbScenarioFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (GetSelectedTurnDirection())
            {
                case TurnDirection.Right:
                    txtAngleMinFilter.Text = "0";
                    txtAngleMaxFilter.Text = "1";
                    break;
                case TurnDirection.Left:
                    txtAngleMinFilter.Text = "-1";
                    txtAngleMaxFilter.Text = "0";
                    break;
            }
        }

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
            {
                _lastSelectedFrameIndex = idx;
                DisplayFrameAtIndex(idx);
                RefreshSelectionVisuals();
            }
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

        private void btnFirst_Click(object sender, EventArgs e) => MoveToFirstFrame();
        private void btnPrev_Click(object sender, EventArgs e) => MoveToPreviousFrame();
        private void btnNext_Click(object sender, EventArgs e) => MoveToNextFrame();
        private void btnLast_Click(object sender, EventArgs e) => MoveToLastFrame();

        private void MoveToFirstFrame()
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            SetIndex(0);
        }

        private void MoveToPreviousFrame()
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            SetIndex(Math.Max(0, _currentFrameIndex - 1));
        }

        private void MoveToNextFrame()
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            SetIndex(Math.Min(_visibleFrames.Count - 1, _currentFrameIndex + 1));
        }

        private void MoveToLastFrame()
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            SetIndex(_visibleFrames.Count - 1);
        }

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
                SetPlaybackButtonText("자동 재생");
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
                case Keys.Left: btnPrev_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.Space: TogglePlayPause(); e.Handled = true; break;
                case Keys.Home: btnFirst_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.End: btnLast_Click(this, EventArgs.Empty); e.Handled = true; break;
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
            dgvFrameCatalog.Font = new Font("resource/PretendardVariable.ttf", 10F, System.Drawing.FontStyle.Regular);
            dgvFrameCatalog.ColumnHeadersDefaultCellStyle.Font = new Font("resource/PretendardVariable.ttf", 10F, System.Drawing.FontStyle.Bold);
            dgvFrameCatalog.DefaultCellStyle.Font = new Font("resource/PretendardVariable.ttf", 10F, System.Drawing.FontStyle.Regular);
            dgvFrameCatalog.EnableHeadersVisualStyles = false;
            dgvFrameCatalog.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvFrameCatalog.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
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

        private void ConfigureModelValidationTab()
        {
            if (tabPageModelValidation != null)
                return;

            tabPageModelValidation = new TabPage
            {
                Name = "tabPageModelValidation",
                Text = "추론/검증",
                Padding = new Padding(8),
                UseVisualStyleBackColor = true
            };

            int insertIndex = Math.Min(2, tabControlMain.TabPages.Count);
            tabControlMain.TabPages.Insert(insertIndex, tabPageModelValidation);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = System.Drawing.Color.FromArgb(245, 247, 250)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 74F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 26F));

            picValidationPreview = new PictureBox
            {
                Name = "picValidationPreview",
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            picValidationPreview.Paint += PicValidationPreview_Paint;

            pnlValidationBars = new Panel
            {
                Name = "pnlValidationBars",
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(26, 29, 34),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlValidationBars.Paint += PnlValidationBars_Paint;

            var sidePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 17,
                Padding = new Padding(14),
                BackColor = System.Drawing.Color.White
            };
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            sidePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            sidePanel.Controls.Add(CreateValidationWorkflowHeader(), 0, 0);

            txtValidationDataFolderPath = new TextBox
            {
                Name = "txtValidationDataFolderPath",
                Dock = DockStyle.Fill,
                Text = _currentDataFolderPath
            };
            txtValidationDataFolderPath.TextChanged += (_, _) => UpdateValidationActionButtons();
            btnSelectValidationDataFolder = CreateValidationButton("선택");
            btnSelectValidationDataFolder.Click += (_, _) => SelectValidationDataFolder();
            sidePanel.Controls.Add(CreateValidationPathRow("데이터 폴더", txtValidationDataFolderPath, btnSelectValidationDataFolder), 0, 1);

            txtValidationModelPath = new TextBox
            {
                Name = "txtValidationModelPath",
                Dock = DockStyle.Fill,
                Text = txtTrainingModelPath?.Text ?? string.Empty
            };
            txtValidationModelPath.TextChanged += (_, _) => UpdateValidationActionButtons();
            btnSelectValidationModelPath = CreateValidationButton("선택");
            btnSelectValidationModelPath.Click += (_, _) => SelectValidationModelPath();
            sidePanel.Controls.Add(CreateValidationPathRow("모델 파일", txtValidationModelPath, btnSelectValidationModelPath), 0, 2);

            txtPilotApiUrl = new TextBox
            {
                Name = "txtPilotApiUrl",
                Dock = DockStyle.Fill,
                Text = "http://127.0.0.1:5000/predict"
            };
            sidePanel.Controls.Add(CreateValidationPathRow("서버 주소", txtPilotApiUrl, null), 0, 3);

            btnLoadValidationDataFolder = CreateValidationButton("데이터 불러오기");
            btnLoadValidationDataFolder.Click += (_, _) => LoadValidationDataFolderFromTextBox();
            btnDetectValidationEnvironment = CreateValidationButton("자동 감지");
            btnDetectValidationEnvironment.Click += async (_, _) => await DetectValidationEnvironmentAsync(showMessage: true);
            btnStartPilotServer = CreateValidationButton("서버 시작");
            btnStartPilotServer.Click += async (_, _) => await StartPilotServerAsync(showErrorMessage: true);
            btnStartPilotServer.Enabled = false;
            btnStopPilotServer = CreateValidationButton("서버 중지");
            btnStopPilotServer.Click += (_, _) => StopPilotServer(showMessage: true);
            btnStopPilotServer.Enabled = false;
            sidePanel.Controls.Add(CreateValidationButtonRow(btnLoadValidationDataFolder, btnDetectValidationEnvironment), 0, 4);
            sidePanel.Controls.Add(CreateValidationButtonRow(btnStartPilotServer, btnStopPilotServer), 0, 5);

            lblPilotStatus = CreateValidationLabel("상태: 서버 대기 중");
            lblPilotActualAngle = CreateValidationLabel("실제 Angle: -");
            lblPilotActualThrottle = CreateValidationLabel("실제 Throttle: -");
            lblPilotPredictedAngle = CreateValidationLabel("예측 Angle: -");
            lblPilotPredictedThrottle = CreateValidationLabel("예측 Throttle: -");
            lblValidationEnvironmentStatus = CreateValidationLabel("환경: 자동 감지를 실행해 주세요.");

            sidePanel.Controls.Add(lblPilotStatus, 0, 6);
            sidePanel.Controls.Add(lblValidationEnvironmentStatus, 0, 7);
            sidePanel.Controls.Add(lblPilotActualAngle, 0, 8);
            sidePanel.Controls.Add(lblPilotActualThrottle, 0, 9);
            sidePanel.Controls.Add(lblPilotPredictedAngle, 0, 10);
            sidePanel.Controls.Add(lblPilotPredictedThrottle, 0, 11);

            btnPilotHealthCheck = new Button
            {
                Name = "btnPilotHealthCheck",
                Text = "서버 확인",
                Dock = DockStyle.Fill,
                UseVisualStyleBackColor = true
            };
            btnPilotHealthCheck.Click += async (_, _) => await CheckPilotServerHealthAsync();
            sidePanel.Controls.Add(btnPilotHealthCheck, 0, 12);

            btnShowPredictionMismatchCandidates = CreateValidationButton("예측 차이 후보");
            btnShowPredictionMismatchCandidates.Click += (_, _) => ShowPredictionMismatchCandidates();
            sidePanel.Controls.Add(btnShowPredictionMismatchCandidates, 0, 13);

            btnValidationFirst = CreateValidationButton("처음");
            btnValidationFirst.Click += (_, _) => MoveToFirstFrame();
            btnValidationPrevious = CreateValidationButton("이전");
            btnValidationPrevious.Click += (_, _) => MoveToPreviousFrame();
            btnValidationNext = CreateValidationButton("다음");
            btnValidationNext.Click += (_, _) => MoveToNextFrame();
            btnValidationLast = CreateValidationButton("끝");
            btnValidationLast.Click += (_, _) => MoveToLastFrame();
            sidePanel.Controls.Add(CreateValidationNavigationRow(), 0, 14);

            btnValidationAutoPlay = CreateValidationButton("자동 재생");
            btnValidationAutoPlay.Click += (_, _) => TogglePlayPause();
            _cmbValidationPlaybackSpeed = CreatePlaybackSpeedComboBox();
            _cmbValidationPlaybackSpeed.SelectedIndexChanged += (_, _) =>
            {
                SyncPlaybackSpeedFromCombo(_cmbValidationPlaybackSpeed);
                if (_isPlaybackRunning)
                    _playbackTimer.Interval = GetPlaybackIntervalFromSpeed();
            };
            sidePanel.Controls.Add(CreateValidationPlaybackRow(btnValidationAutoPlay, _cmbValidationPlaybackSpeed), 0, 15);

            rtbPilotServerLog = new RichTextBox
            {
                Name = "rtbPilotServerLog",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(20, 22, 26),
                ForeColor = System.Drawing.Color.White,
                Font = new Font("Consolas", 9.5F),
                Text = "사용 순서: 데이터 폴더 선택 -> 모델 파일 선택 -> 자동 감지 -> 서버 시작 -> 프레임 선택\r\n"
            };
            sidePanel.Controls.Add(rtbPilotServerLog, 0, 16);
            UpdateValidationActionButtons();

            root.Controls.Add(picValidationPreview, 0, 0);
            root.Controls.Add(sidePanel, 1, 0);
            root.Controls.Add(pnlValidationBars, 0, 1);
            root.SetColumnSpan(pnlValidationBars, 2);

            tabPageModelValidation.Controls.Add(root);
        }

        private System.Windows.Forms.Label CreateValidationLabel(string text, bool bold = false)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("맑은 고딕", bold ? 10.5F : 9.5F, bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular)
            };
        }

        private Control CreateValidationWorkflowHeader()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 6),
                BackColor = System.Drawing.Color.FromArgb(236, 244, 255),
                Padding = new Padding(10, 6, 10, 6)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));

            var title = new System.Windows.Forms.Label
            {
                Text = "추론/검증 설정",
                Dock = DockStyle.Fill,
                AutoSize = false,
                Font = new Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = System.Drawing.Color.FromArgb(20, 32, 48)
            };

            var workflow = new System.Windows.Forms.Label
            {
                Text = "1. 자동 감지  ->  2. 서버 시작  ->  3. 프레임 검증",
                Dock = DockStyle.Fill,
                AutoSize = false,
                Font = new Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = System.Drawing.Color.FromArgb(0, 86, 179)
            };

            panel.Controls.Add(title, 0, 0);
            panel.Controls.Add(workflow, 0, 1);
            return panel;
        }

        private Button CreateValidationButton(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Height = 30,
                UseVisualStyleBackColor = true
            };
        }

        private Control CreateValidationPathRow(string labelText, Control input, Button? button)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = button == null ? 2 : 3,
                RowCount = 1,
                Margin = new Padding(0, 2, 0, 2)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            if (button != null)
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72));

            row.Controls.Add(CreateValidationLabel(labelText), 0, 0);
            row.Controls.Add(input, 1, 0);
            if (button != null)
                row.Controls.Add(button, 2, 0);

            return row;
        }

        private Control CreateValidationButtonRow(params Button[] buttons)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = Math.Max(1, buttons.Length),
                RowCount = 1,
                Margin = new Padding(0, 2, 0, 2)
            };

            foreach (Button button in buttons)
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / buttons.Length));
                row.Controls.Add(button, row.Controls.Count, 0);
            }

            return row;
        }

        private Control CreateValidationNavigationRow()
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 2, 0, 2)
            };

            foreach (Button? button in new[] { btnValidationFirst, btnValidationPrevious, btnValidationNext, btnValidationLast })
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                if (button != null)
                    row.Controls.Add(button, row.Controls.Count, 0);
            }

            return row;
        }

        private Control CreateValidationPlaybackRow(Button playButton, ComboBox speedComboBox)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0, 2, 0, 2)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));

            row.Controls.Add(playButton, 0, 0);
            row.Controls.Add(CreateValidationLabel("배속"), 1, 0);
            row.Controls.Add(speedComboBox, 2, 0);
            return row;
        }

        private ComboBox CreatePlaybackSpeedComboBox()
        {
            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill
            };

            for (int speed = 1; speed <= 10; speed++)
                comboBox.Items.Add($"{speed}배");

            comboBox.SelectedIndex = 0;
            return comboBox;
        }

        private void SelectValidationDataFolder()
        {
            if (txtValidationDataFolderPath == null) return;

            using var dialog = new FolderBrowserDialog
            {
                Description = "검증할 Tub 데이터 폴더 선택",
                SelectedPath = Directory.Exists(txtValidationDataFolderPath.Text)
                    ? txtValidationDataFolderPath.Text
                    : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            txtValidationDataFolderPath.Text = dialog.SelectedPath;
            LoadValidationDataFolderFromTextBox();
        }

        private void SelectValidationModelPath()
        {
            if (txtValidationModelPath == null) return;

            using var dialog = new OpenFileDialog
            {
                Title = "검증할 모델 파일 선택",
                Filter = "Keras/TFLite Model (*.keras;*.h5;*.tflite)|*.keras;*.h5;*.tflite|All files (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(txtValidationModelPath.Text)
                    ? string.Empty
                    : Path.GetFileName(txtValidationModelPath.Text)
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                txtValidationModelPath.Text = dialog.FileName;
        }

        private void UpdateValidationActionButtons()
        {
            bool hasDataFolder = ValidationDataFolderExists();
            bool hasModelFile = ValidationModelFileExists();
            bool canDetect = hasDataFolder && hasModelFile;

            if (btnDetectValidationEnvironment != null)
                btnDetectValidationEnvironment.Enabled = canDetect;

            if (!canDetect)
                _isValidationEnvironmentReady = false;

            if (btnStartPilotServer != null)
            {
                bool serverRunning = _pilotServerProcess is { HasExited: false };
                btnStartPilotServer.Enabled = canDetect && _isValidationEnvironmentReady && !serverRunning;
            }

            if (lblValidationEnvironmentStatus != null)
            {
                if (!hasDataFolder && !hasModelFile)
                    lblValidationEnvironmentStatus.Text = "환경: 데이터 폴더와 모델 파일을 먼저 선택해 주세요.";
                else if (!hasDataFolder)
                    lblValidationEnvironmentStatus.Text = "환경: 데이터 폴더를 먼저 선택해 주세요.";
                else if (!hasModelFile)
                    lblValidationEnvironmentStatus.Text = "환경: 모델 파일을 먼저 선택해 주세요.";
                else if (!_isValidationEnvironmentReady)
                    lblValidationEnvironmentStatus.Text = "환경: 자동 감지를 실행해 주세요.";
            }
        }

        private bool ValidationDataFolderExists()
        {
            string folderPath = txtValidationDataFolderPath?.Text.Trim() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath);
        }

        private bool ValidationModelFileExists()
        {
            string modelPath = txtValidationModelPath?.Text.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(modelPath))
                return false;

            // WSL 내부 경로를 직접 입력하는 경우는 Windows File.Exists로 확인할 수 없습니다.
            // 이 경우 서버 시작 시 WSL 내부에서 한 번 더 검증합니다.
            if (modelPath.StartsWith("/", StringComparison.Ordinal) ||
                modelPath.StartsWith("~/", StringComparison.Ordinal))
                return true;

            return File.Exists(modelPath);
        }

        private void LoadValidationDataFolderFromTextBox()
        {
            string folderPath = txtValidationDataFolderPath?.Text.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                MessageBox.Show("검증할 데이터 폴더를 먼저 선택해 주세요.",
                    "데이터 폴더 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppendPilotServerLog("[검증] 데이터 폴더를 불러옵니다: " + folderPath);
            _ = LoadCatalogAsync(folderPath);
        }

        private async Task<bool> DetectValidationEnvironmentAsync(bool showMessage)
        {
            if (!ValidationDataFolderExists() || !ValidationModelFileExists())
            {
                _isValidationEnvironmentReady = false;
                UpdateValidationActionButtons();
                MessageBox.Show("데이터 폴더와 모델 파일을 모두 선택한 뒤 자동 감지를 실행해 주세요.",
                    "추론/검증 자동 감지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (lblValidationEnvironmentStatus != null)
                lblValidationEnvironmentStatus.Text = "환경: 자동 감지 중...";

            AppendPilotServerLog("[감지] WSL/Conda/DonkeyCar 환경을 확인합니다.");
            bool detected = await DetectTrainingEnvironmentAsync(showSuccessMessage: false, clearLog: false);
            _isValidationEnvironmentReady = detected;
            string summary = detected
                ? $"환경: {NormalizeWslDistroName(cmbTrainingWslDistro.Text)} / {txtTrainingPythonEnvName.Text.Trim()} / {cmbMycarProjectPath.Text.Trim()}"
                : "환경: 자동 감지 실패. 학습 탭에서 WSL, Conda, Python 환경, Donkey 프로젝트 경로를 확인해 주세요.";

            if (lblValidationEnvironmentStatus != null)
                lblValidationEnvironmentStatus.Text = summary;

            AppendPilotServerLog("[감지] " + summary);
            UpdateValidationActionButtons();
            if (showMessage)
            {
                MessageBox.Show(
                    detected
                        ? "추론/검증 환경 감지가 완료되었습니다."
                        : "추론/검증 환경 감지에 실패했습니다.\n학습 실행 탭의 자동 감지 로그를 확인해 주세요.",
                    "추론/검증 자동 감지",
                    MessageBoxButtons.OK,
                    detected ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }

            return detected;
        }

        private void DgvFrameCatalog_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            int rowIndex = GetFrameGridRowIndexAt(e.Location);
            if (rowIndex < 0) return;

            _dragStartFrameRowIndex = rowIndex;
            _isDraggingFrameRows = true;
            _catalogDragMoved = false;
            _lastCatalogDragIndex = rowIndex;
        }

        private void DgvFrameCatalog_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDraggingFrameRows || e.Button != MouseButtons.Left) return;

            AutoScrollFrameCatalogDuringDrag(e.Y);

            int rowIndex = GetFrameGridDragRowIndex(e.Location);
            if (rowIndex < 0 || _dragStartFrameRowIndex < 0) return;
            if (rowIndex == _lastCatalogDragIndex) return;

            _catalogDragMoved = true;
            _lastCatalogDragIndex = rowIndex;
            SelectFrameGridRange(_dragStartFrameRowIndex, rowIndex, centerCatalog: false);
        }

        private void DgvFrameCatalog_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_isDraggingFrameRows && !_catalogDragMoved)
            {
                int rowIndex = GetFrameGridRowIndexAt(e.Location);
                if (rowIndex < 0)
                    rowIndex = _dragStartFrameRowIndex;

                if (rowIndex >= 0 && rowIndex < _visibleFrames.Count)
                    ApplyFrameSelection(rowIndex, System.Windows.Forms.Keys.None);
            }

            _isDraggingFrameRows = false;
            _dragStartFrameRowIndex = -1;
            _catalogDragMoved = false;
            _lastCatalogDragIndex = -1;
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

        private int GetFrameGridDragRowIndex(Point point)
        {
            int rowIndex = GetFrameGridRowIndexAt(point);
            if (rowIndex >= 0)
                return rowIndex;

            if (dgvFrameCatalog.Rows.Count == 0)
                return -1;

            int first = dgvFrameCatalog.FirstDisplayedScrollingRowIndex >= 0
                ? dgvFrameCatalog.FirstDisplayedScrollingRowIndex
                : 0;
            int visible = Math.Max(1, dgvFrameCatalog.DisplayedRowCount(includePartialRow: true));
            int last = Math.Min(dgvFrameCatalog.Rows.Count - 1, first + visible - 1);

            if (point.Y <= dgvFrameCatalog.ColumnHeadersHeight)
                return first;
            if (point.Y >= dgvFrameCatalog.ClientSize.Height - 1)
                return last;

            return -1;
        }

        private void AutoScrollFrameCatalogDuringDrag(int mouseY)
        {
            if (dgvFrameCatalog.Rows.Count == 0 || dgvFrameCatalog.FirstDisplayedScrollingRowIndex < 0)
                return;

            int visible = Math.Max(1, dgvFrameCatalog.DisplayedRowCount(includePartialRow: true));
            int first = dgvFrameCatalog.FirstDisplayedScrollingRowIndex;
            int maxFirst = Math.Max(0, dgvFrameCatalog.Rows.Count - visible);
            int edge = Math.Max(28, dgvFrameCatalog.ClientSize.Height / 8);
            int nextFirst = first;

            if (mouseY <= dgvFrameCatalog.ColumnHeadersHeight + edge)
                nextFirst = Math.Max(0, first - 1);
            else if (mouseY >= dgvFrameCatalog.ClientSize.Height - edge)
                nextFirst = Math.Min(maxFirst, first + 1);

            if (nextFirst == first)
                return;

            try
            {
                dgvFrameCatalog.FirstDisplayedScrollingRowIndex = nextFirst;
            }
            catch (InvalidOperationException)
            {
            }
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
        private void SelectFrameGridRange(
            int startRowIndex,
            int endRowIndex,
            bool centerCatalog = true,
            bool renderChart = true,
            bool updateFilmstripNow = false)
        {
            if (dgvFrameCatalog.Rows.Count == 0) return;

            int from = Math.Max(0, Math.Min(startRowIndex, endRowIndex));
            int to = Math.Min(dgvFrameCatalog.Rows.Count - 1, Math.Max(startRowIndex, endRowIndex));

            if (updateFilmstripNow)
                SetDragSelectionPreview(from, to);

            _isFrameSelectionUpdating = true;
            dgvFrameCatalog.ClearSelection();
            for (int rowIndex = from; rowIndex <= to; rowIndex++)
                dgvFrameCatalog.Rows[rowIndex].Selected = true;
            InvalidateFrameCatalogRows(from, to);

            int focusRowIndex = Math.Clamp(endRowIndex, from, to);
            dgvFrameCatalog.CurrentCell = dgvFrameCatalog.Rows[focusRowIndex].Cells[0];
            if (centerCatalog)
                ScrollCatalogToFrame(focusRowIndex);
            _isFrameSelectionUpdating = false;

            if (updateFilmstripNow)
                RefreshSelectionVisuals(renderChart: false, updateFilmstripNow: true);

            if (focusRowIndex >= 0 && focusRowIndex < _visibleFrames.Count)
            {
                _lastSelectedFrameIndex = focusRowIndex;
                DisplayFrameAtIndex(focusRowIndex);
            }

            if (renderChart)
                RefreshSelectionVisuals(renderChart: true, updateFilmstripNow: false);
            else if (!updateFilmstripNow)
                RefreshSelectionVisuals(renderChart: false, updateFilmstripNow: false);
        }

        private void ApplyFrameSelectionFromFilmstrip(int clickedIndex)
            => ApplyFrameSelection(clickedIndex, ModifierKeys, removeRangeOnShift: true);

        private void ApplyFrameSelection(
            int clickedIndex,
            Keys modifiers,
            bool removeRangeOnShift = false,
            bool? ctrlSelectedBefore = null)
        {
            if (_visibleFrames == null || clickedIndex < 0 || clickedIndex >= _visibleFrames.Count)
                return;

            _isFrameSelectionUpdating = true;

            try
            {
                if ((modifiers & Keys.Shift) == Keys.Shift && _lastSelectedFrameIndex >= 0)
                {
                    int from = Math.Min(_lastSelectedFrameIndex, clickedIndex);
                    int to = Math.Max(_lastSelectedFrameIndex, clickedIndex);
                    if (!removeRangeOnShift)
                        dgvFrameCatalog.ClearSelection();

                    for (int i = from; i <= to && i < dgvFrameCatalog.Rows.Count; i++)
                        dgvFrameCatalog.Rows[i].Selected = !removeRangeOnShift;
                }
                else if ((modifiers & Keys.Control) == Keys.Control)
                {
                    if (clickedIndex < dgvFrameCatalog.Rows.Count)
                    {
                        bool wasSelected = ctrlSelectedBefore ?? dgvFrameCatalog.Rows[clickedIndex].Selected;
                        dgvFrameCatalog.Rows[clickedIndex].Selected = !wasSelected;
                    }
                }
                else
                {
                    dgvFrameCatalog.ClearSelection();
                    if (clickedIndex < dgvFrameCatalog.Rows.Count)
                        dgvFrameCatalog.Rows[clickedIndex].Selected = true;
                }

                if (clickedIndex < dgvFrameCatalog.Rows.Count)
                {
                    dgvFrameCatalog.CurrentCell = dgvFrameCatalog.Rows[clickedIndex].Cells[0];
                    ScrollCatalogToFrame(clickedIndex);
                }
            }
            finally
            {
                _isFrameSelectionUpdating = false;
            }

            _lastSelectedFrameIndex = clickedIndex;
            DisplayFrameAtIndex(clickedIndex);
            RefreshSelectionVisuals();
        }

        private void ScrollCatalogToFrame(int rowIndex)
        {
            if (dgvFrameCatalog.Rows.Count == 0 || rowIndex < 0 || rowIndex >= dgvFrameCatalog.Rows.Count)
                return;

            if (dgvFrameCatalog.FirstDisplayedScrollingRowIndex < 0)
                return;

            int visibleRows = Math.Max(1, dgvFrameCatalog.DisplayedRowCount(includePartialRow: true));
            int first = dgvFrameCatalog.FirstDisplayedScrollingRowIndex;
            int last = Math.Min(dgvFrameCatalog.Rows.Count - 1, first + visibleRows - 1);

            if (rowIndex >= first && rowIndex <= last)
                return;

            int targetFirst = rowIndex < first
                ? rowIndex
                : Math.Max(0, rowIndex - visibleRows + 1);

            try
            {
                dgvFrameCatalog.FirstDisplayedScrollingRowIndex = targetFirst;
            }
            catch (InvalidOperationException)
            {
                // 바인딩 직후 아직 표시 가능한 행 계산이 끝나지 않은 짧은 순간을 방어합니다.
            }
        }

        private void SetDragSelectionPreview(int from, int to)
        {
            _dragPreviewSelectionStartIndex = Math.Max(0, from);
            _dragPreviewSelectionEndIndex = Math.Max(0, to);
        }

        private void ClearDragSelectionPreview()
        {
            _dragPreviewSelectionStartIndex = -1;
            _dragPreviewSelectionEndIndex = -1;
        }

        private bool ShouldRenderDragChart()
        {
            DateTime now = DateTime.UtcNow;
            if ((now - _lastDragChartRenderUtc).TotalMilliseconds < 90)
                return false;

            _lastDragChartRenderUtc = now;
            return true;
        }

        private void InvalidateFrameCatalogRows(int from, int to)
        {
            if (dgvFrameCatalog.Rows.Count == 0)
                return;

            from = Math.Clamp(from, 0, dgvFrameCatalog.Rows.Count - 1);
            to = Math.Clamp(to, 0, dgvFrameCatalog.Rows.Count - 1);
            for (int rowIndex = Math.Min(from, to); rowIndex <= Math.Max(from, to); rowIndex++)
                dgvFrameCatalog.InvalidateRow(rowIndex);

            dgvFrameCatalog.Update();
        }

        /// <summary>
        /// 데이터 정제 흐름이 눈에 띄도록 버튼명, 구간 입력 영역, 상태 색상을 정리합니다.
        /// </summary>
        private void ApplyDataManagerUiStyle()
        {
            tabPageDataViewer.Text = "데이터 확인";
            tabGraphStats.Text = "그래프/통계";
            grpDataExplorer.Text = "데이터 탐색";
            grpDataCleaner.Text = "데이터 정리";
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
            ArrangeDataCleanerPanel();
            ArrangeFrameInfoPanel();
            grpDataCleaner.Resize += (_, _) => ArrangeDataCleanerPanel();
        }

        /// <summary>
        /// 기존 숫자 입력 컨트롤은 숨기고, 사용자가 바로 이해할 수 있는 1~10배 콤보박스를 표시합니다.
        /// </summary>
        private void ConfigurePlaybackSpeedComboBox()
        {
            numPlaybackIntervalMs.Visible = false;

            if (_cmbPlaybackSpeed == null)
            {
                _cmbPlaybackSpeed = new ComboBox
                {
                    Name = "cmbPlaybackSpeed",
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("resource/PretendardVariable.ttf", 9.5F),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                for (int speed = 1; speed <= 10; speed++)
                    _cmbPlaybackSpeed.Items.Add($"{speed}배");

                _cmbPlaybackSpeed.SelectedIndex = 0;
                _cmbPlaybackSpeed.SelectedIndexChanged += (_, _) =>
                {
                    SyncPlaybackSpeedFromCombo(_cmbPlaybackSpeed);
                    if (_isPlaybackRunning)
                        _playbackTimer.Interval = GetPlaybackIntervalFromSpeed();
                };

                grpDataExplorer.Controls.Add(_cmbPlaybackSpeed);
                _cmbPlaybackSpeed.BringToFront();
            }
        }

        private void ConfigureDeletedGraphToggleButton()
        {
            if (_btnToggleDeletedGraph == null)
            {
                _btnToggleDeletedGraph = new Button
                {
                    Name = "btnToggleDeletedGraph",
                    Text = "제외 구간 표시",
                    Size = new Size(124, 28),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    UseVisualStyleBackColor = false,
                    BackColor = System.Drawing.Color.FromArgb(255, 238, 238)
                };
                _btnToggleDeletedGraph.Click += BtnToggleDeletedGraph_Click;
                grpDataExplorer.Controls.Add(_btnToggleDeletedGraph);
            }

            UpdateDeletedGraphToggleButton();
            _btnToggleDeletedGraph.BringToFront();
        }

        private void BtnToggleDeletedGraph_Click(object? sender, EventArgs e)
        {
            _showDeletedOnGraph = !_showDeletedOnGraph;
            UpdateDeletedGraphToggleButton();
            RenderDataViewerFrameChart();
            RenderFrameChart();
        }

        private void UpdateDeletedGraphToggleButton()
        {
            if (_btnToggleDeletedGraph == null) return;

            _btnToggleDeletedGraph.Text = _showDeletedOnGraph
                ? "제외 구간 표시"
                : "제외 선 끊기";
            _btnToggleDeletedGraph.BackColor = _showDeletedOnGraph
                ? System.Drawing.Color.FromArgb(255, 238, 238)
                : System.Drawing.Color.FromArgb(240, 244, 248);
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

        private void ConfigureResponsiveDataViewerLayout()
        {
            tabPageDataViewer.Padding = new Padding(8);
            statusStripDataFooter.Dock = DockStyle.Bottom;

            grpDataExplorer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpDataCleaner.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            splitContainerFramePreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerFramePreview.IsSplitterFixed = false;
            splitContainerFramePreview.Panel1MinSize = 220;
            splitContainerFramePreview.Panel2MinSize = 360;

            EnsureFrameLabelDockedToGrid();
            ConfigureFrameThumbnailStrip();
            InitDataViewerFrameChart();
            ApplyDataExplorerAnchors();

            tabPageDataViewer.Resize += (_, _) => LayoutDataViewerResponsive();
            grpDataExplorer.Resize += (_, _) => LayoutDataViewerResponsive();
            splitContainerFramePreview.Resize += (_, _) => KeepFramePreviewSplitterRatio();

            BeginInvoke(new Action(LayoutDataViewerResponsive));
        }

        private void ApplyDataExplorerAnchors()
        {
            foreach (var button in new[] { btnOpenDataFolder, btnReloadData, btnToggleTheme, btnGuide })
                button.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            foreach (var button in new[] { btnFirst, btnPrev, btnAutoPlay, btnNext, btnLast })
                button.Anchor = AnchorStyles.Bottom;

            trkFrameTimeline.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlFrameThumbnailStrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            lblPlayInterval.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            if (_cmbPlaybackSpeed != null)
                _cmbPlaybackSpeed.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            lblAngleValue.Visible = false;
            lblThrottleValue.Visible = false;
            lblModeValue.Visible = false;
            if (_lblFrameReviewHint != null)
                _lblFrameReviewHint.Visible = false;
            numPlaybackIntervalMs.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        }

        private void EnsureFrameLabelDockedToGrid()
        {
            if (lblFrameValue.Parent != splitContainerFramePreview.Panel1)
            {
                lblFrameValue.Parent?.Controls.Remove(lblFrameValue);
                dgvFrameCatalog.Parent?.Controls.Remove(dgvFrameCatalog);
                splitContainerFramePreview.Panel1.Controls.Add(dgvFrameCatalog);
                splitContainerFramePreview.Panel1.Controls.Add(lblFrameValue);
            }

            if (picFramePreview.Parent != splitContainerFramePreview.Panel2)
            {
                picFramePreview.Parent?.Controls.Remove(picFramePreview);
                splitContainerFramePreview.Panel2.Controls.Add(picFramePreview);
            }

            lblFrameValue.AutoSize = false;
            lblFrameValue.Dock = DockStyle.Top;
            lblFrameValue.Height = 34;
            lblFrameValue.TextAlign = ContentAlignment.MiddleCenter;
            lblFrameValue.Font = new Font("resource/PretendardVariable.ttf", 12F, System.Drawing.FontStyle.Bold);
            lblFrameValue.BringToFront();

            dgvFrameCatalog.Dock = DockStyle.Fill;
            picFramePreview.Dock = DockStyle.Fill;
        }

        private void LayoutDataViewerResponsive()
        {
            if (tabPageDataViewer.ClientSize.Width <= 0 || tabPageDataViewer.ClientSize.Height <= 0)
                return;

            int margin = 8;
            int footerHeight = statusStripDataFooter.Visible ? statusStripDataFooter.Height : 0;
            int availableWidth = Math.Max(760, tabPageDataViewer.ClientSize.Width - margin * 2);
            int availableHeight = Math.Max(520, tabPageDataViewer.ClientSize.Height - footerHeight - margin * 2);
            int chartGap = 8;
            int chartHeight = availableHeight >= 700
                ? Math.Clamp((int)(availableHeight * 0.28), 210, 280)
                : Math.Clamp((int)(availableHeight * 0.24), 110, 190);
            int explorerHeight = Math.Max(360, availableHeight - chartHeight - chartGap);

            grpDataExplorer.Location = new Point(margin, margin);
            grpDataExplorer.Size = new Size(availableWidth, explorerHeight);

            int chartY = grpDataExplorer.Bottom + chartGap;
            pnlDataViewerChartHost.Location = new Point(margin, chartY);
            pnlDataViewerChartHost.Size = new Size(availableWidth, Math.Max(0, chartHeight));
            pnlDataViewerChartHost.Visible = chartHeight >= 60;

            LayoutDataExplorerContent();
            ArrangeDataCleanerPanel();
            LayoutCleanupGuidanceControls();

            statusStripDataFooter.BringToFront();
        }

        private void LayoutDataExplorerContent()
        {
            int width = Math.Max(760, grpDataExplorer.ClientSize.Width);
            int height = Math.Max(360, grpDataExplorer.ClientSize.Height);
            int margin = 14;
            int toolbarY = 22;
            int toolbarGap = 6;
            int toolbarButtonWidth = 144;
            int toolbarButtonHeight = 28;

            var toolbarButtons = new[] { btnOpenDataFolder, btnReloadData, btnToggleTheme, btnGuide };
            for (int i = 0; i < toolbarButtons.Length; i++)
            {
                toolbarButtons[i].Location = new Point(margin + i * (toolbarButtonWidth + toolbarGap), toolbarY);
                toolbarButtons[i].Size = new Size(toolbarButtonWidth, toolbarButtonHeight);
            }
            if (_btnToggleDeletedGraph != null)
            {
                _btnToggleDeletedGraph.Location = new Point(
                    Math.Max(btnGuide.Right + toolbarGap, width - margin - 124),
                    toolbarY);
                _btnToggleDeletedGraph.Size = new Size(124, toolbarButtonHeight);
            }

            int contentTop = toolbarY + toolbarButtonHeight + 12;
            int playbackButtonHeight = 32;
            int playbackY = Math.Max(contentTop + 250, height - 53);
            int timelineHeight = 42;
            int timelineY = Math.Max(contentTop + 190, playbackY - timelineHeight - 8);
            int thumbnailHeight = 58;
            int thumbnailScrollHeight = 17;
            int thumbnailY = Math.Max(contentTop + 120, timelineY - thumbnailHeight - thumbnailScrollHeight - 8);
            int splitHeight = Math.Max(120, thumbnailY - contentTop - 10);
            int cleanerWidth = Math.Clamp(width / 4, 300, 380);
            int cleanerLeft = width - margin - cleanerWidth;
            int editorLeft = margin;
            int editorRight = cleanerLeft - 12;
            int editorWidth = Math.Max(560, editorRight - editorLeft);

            grpDataCleaner.Location = new Point(cleanerLeft, contentTop);
            grpDataCleaner.Size = new Size(cleanerWidth, splitHeight);

            splitContainerFramePreview.Location = new Point(editorLeft, contentTop);
            splitContainerFramePreview.Size = new Size(editorWidth, splitHeight);
            KeepFramePreviewSplitterRatio();

            pnlFrameThumbnailStrip.Location = new Point(margin, thumbnailY);
            pnlFrameThumbnailStrip.Size = new Size(width - margin * 2, thumbnailHeight);
            LayoutFilmstripScrollControls(margin, thumbnailY + thumbnailHeight + 2, width - margin * 2, thumbnailScrollHeight);
            UpdateThumbnailStripScrollArea();

            trkFrameTimeline.Location = new Point(margin, timelineY);
            trkFrameTimeline.Size = new Size(width - margin * 2, timelineHeight);

            LayoutPlaybackControls(playbackY, playbackButtonHeight);
        }

        private void LayoutPlaybackControls(int y, int height)
        {
            int smallWidth = 42;
            int playWidth = 92;
            int speedLabelWidth = 64;
            int speedInputWidth = 86;
            int gap = 8;
            int margin = 14;
            int buttonGroupWidth = smallWidth * 4 + playWidth + gap * 4;
            int x = Math.Max(margin, (grpDataExplorer.ClientSize.Width - buttonGroupWidth) / 2);

            btnFirst.SetBounds(x, y, smallWidth, height);
            btnPrev.SetBounds(btnFirst.Right + gap, y, smallWidth, height);
            btnAutoPlay.SetBounds(btnPrev.Right + gap, y, playWidth, height);
            btnNext.SetBounds(btnAutoPlay.Right + gap, y, smallWidth, height);
            btnLast.SetBounds(btnNext.Right + gap, y, smallWidth, height);

            int speedX = Math.Max(btnLast.Right + gap * 3, grpDataExplorer.ClientSize.Width - margin - speedInputWidth - gap - speedLabelWidth);
            lblPlayInterval.SetBounds(speedX, y + 4, speedLabelWidth, height - 4);
            if (_cmbPlaybackSpeed != null)
            {
                _cmbPlaybackSpeed.SetBounds(lblPlayInterval.Right + gap, y + 2, speedInputWidth, height - 4);
                _cmbPlaybackSpeed.BringToFront();
            }
            else
            {
                numPlaybackIntervalMs.SetBounds(lblPlayInterval.Right + gap, y + 3, speedInputWidth, height - 6);
            }
        }

        private void KeepFramePreviewSplitterRatio()
        {
            int width = splitContainerFramePreview.ClientSize.Width;
            if (width <= splitContainerFramePreview.Panel1MinSize + splitContainerFramePreview.Panel2MinSize)
                return;

            int target = Math.Clamp(300, splitContainerFramePreview.Panel1MinSize, width - splitContainerFramePreview.Panel2MinSize);
            if (Math.Abs(splitContainerFramePreview.SplitterDistance - target) > 8)
                splitContainerFramePreview.SplitterDistance = target;
        }

        private void ConfigureFrameThumbnailStrip()
        {
            pnlFrameThumbnailStrip.AutoScroll = false;
            pnlFrameThumbnailStrip.BackColor = System.Drawing.Color.FromArgb(28, 31, 36);
            EnableDoubleBuffering(pnlFrameThumbnailStrip);
            pnlFrameThumbnailStrip.Paint += PnlFrameThumbnailStrip_Paint;
            pnlFrameThumbnailStrip.MouseClick += PnlFrameThumbnailStrip_MouseClick;
            pnlFrameThumbnailStrip.MouseDown += PnlFrameThumbnailStrip_MouseDown;
            pnlFrameThumbnailStrip.MouseMove += PnlFrameThumbnailStrip_MouseMove;
            pnlFrameThumbnailStrip.MouseUp += PnlFrameThumbnailStrip_MouseUp;
            pnlFrameThumbnailStrip.MouseWheel += PnlFrameThumbnailStrip_MouseWheel;
            pnlFrameThumbnailStrip.MouseEnter += (_, _) => pnlFrameThumbnailStrip.Focus();
            pnlFrameThumbnailStrip.Resize += (_, _) =>
            {
                UpdateThumbnailStripScrollArea();
                pnlFrameThumbnailStrip.Invalidate();
            };

            if (_filmstripScrollBar == null)
            {
                _filmstripScrollBar = new HScrollBar
                {
                    Name = "hsbFrameThumbnailStrip",
                    Height = 17,
                    SmallChange = 88,
                    LargeChange = Math.Max(88, pnlFrameThumbnailStrip.ClientSize.Width)
                };
                _filmstripScrollBar.Scroll += FilmstripScrollBar_Scroll;
                grpDataExplorer.Controls.Add(_filmstripScrollBar);
            }

            if (_btnReturnFilmstripToCurrent == null)
            {
                _btnReturnFilmstripToCurrent = new Button
                {
                    Name = "btnReturnFilmstripToCurrent",
                    Text = "선택 프레임",
                    Size = new Size(112, 24),
                    Visible = false,
                    UseVisualStyleBackColor = false,
                    BackColor = System.Drawing.Color.FromArgb(229, 241, 255)
                };
                _btnReturnFilmstripToCurrent.Click += (_, _) => CenterFilmstripOnCurrentFrame();
                grpDataExplorer.Controls.Add(_btnReturnFilmstripToCurrent);
                _btnReturnFilmstripToCurrent.BringToFront();
            }
        }

        private void UpdateThumbnailStripScrollArea()
        {
            pnlFrameThumbnailStrip.AutoScrollMinSize = Size.Empty;
            ConfigureFilmstripScrollBar();
            ClampFilmstripViewOffset(syncScrollBar: true);
            UpdateReturnFilmstripButton();
        }

        private void LayoutFilmstripScrollControls(int x, int y, int width, int height)
        {
            if (_filmstripScrollBar != null)
                _filmstripScrollBar.SetBounds(x, y, width, height);

            UpdateReturnFilmstripButton();
        }

        private void ConfigureFilmstripScrollBar()
        {
            if (_filmstripScrollBar == null) return;

            int minOffset = GetMinFilmstripViewOffset();
            int maxOffset = GetMaxFilmstripViewOffset();

            _isSyncingFilmstripScrollBar = true;
            try
            {
                _filmstripScrollBar.SmallChange = 88;
                _filmstripScrollBar.LargeChange = Math.Max(88, pnlFrameThumbnailStrip.ClientSize.Width);

                int scrollMaximum = Math.Max(minOffset, maxOffset + _filmstripScrollBar.LargeChange - 1);
                _filmstripScrollBar.Maximum = scrollMaximum;
                _filmstripScrollBar.Minimum = minOffset;
                _filmstripScrollBar.Enabled = maxOffset > minOffset;
                _filmstripScrollBar.Value = Math.Clamp(_filmstripViewOffsetPixels, minOffset, maxOffset);
            }
            finally
            {
                _isSyncingFilmstripScrollBar = false;
            }
        }

        private void FilmstripScrollBar_Scroll(object? sender, ScrollEventArgs e)
        {
            if (_isSyncingFilmstripScrollBar) return;

            _isFilmstripViewLockedByUser = true;
            SetFilmstripViewOffset(e.NewValue, syncScrollBar: false);
        }

        private int GetMaxFilmstripViewOffset()
        {
            int count = _visibleFrames?.Count ?? 0;
            if (count <= 0) return 0;

            const int pitch = 88;
            return GetMinFilmstripViewOffset() + Math.Max(0, count - 1) * pitch;
        }

        private int GetMinFilmstripViewOffset()
        {
            const int itemWidth = 80;
            int centerLeft = (pnlFrameThumbnailStrip.ClientSize.Width - itemWidth) / 2;
            return 8 - centerLeft;
        }

        private int GetCenteredFilmstripOffset(int frameIndex)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return 0;

            const int itemWidth = 80;
            const int pitch = 88;
            int centerLeft = (pnlFrameThumbnailStrip.ClientSize.Width - itemWidth) / 2;
            return Math.Clamp(8 + frameIndex * pitch - centerLeft, GetMinFilmstripViewOffset(), GetMaxFilmstripViewOffset());
        }

        private void SetFilmstripViewOffset(int offset, bool syncScrollBar)
        {
            _filmstripViewOffsetPixels = Math.Clamp(offset, GetMinFilmstripViewOffset(), GetMaxFilmstripViewOffset());

            if (syncScrollBar && _filmstripScrollBar != null)
            {
                _isSyncingFilmstripScrollBar = true;
                try
                {
                    int maxValue = Math.Max(_filmstripScrollBar.Minimum, _filmstripScrollBar.Maximum - _filmstripScrollBar.LargeChange + 1);
                    _filmstripScrollBar.Value = Math.Clamp(_filmstripViewOffsetPixels, _filmstripScrollBar.Minimum, maxValue);
                }
                finally
                {
                    _isSyncingFilmstripScrollBar = false;
                }
            }

            UpdateReturnFilmstripButton();
            RefreshFilmstripHighlight(forceFullRefresh: true);
        }

        private void ClampFilmstripViewOffset(bool syncScrollBar)
        {
            SetFilmstripViewOffset(_filmstripViewOffsetPixels, syncScrollBar);
        }

        private void CenterFilmstripOnCurrentFrame()
        {
            if (_currentFrameIndex < 0) return;

            _isFilmstripViewLockedByUser = false;
            SetFilmstripViewOffset(GetCenteredFilmstripOffset(_currentFrameIndex), syncScrollBar: true);
        }

        private void UpdateReturnFilmstripButton()
        {
            if (_btnReturnFilmstripToCurrent == null || _visibleFrames == null || _visibleFrames.Count == 0 || _currentFrameIndex < 0)
            {
                if (_btnReturnFilmstripToCurrent != null)
                    _btnReturnFilmstripToCurrent.Visible = false;
                return;
            }

            int targetOffset = GetCenteredFilmstripOffset(_currentFrameIndex);
            bool shouldShow = _isFilmstripViewLockedByUser && Math.Abs(_filmstripViewOffsetPixels - targetOffset) > 8;
            _btnReturnFilmstripToCurrent.Visible = shouldShow;
            if (!shouldShow) return;

            int margin = 8;
            int y = pnlFrameThumbnailStrip.Bottom - _btnReturnFilmstripToCurrent.Height - 5;
            if (_filmstripViewOffsetPixels > targetOffset)
            {
                _btnReturnFilmstripToCurrent.Text = "< 선택 프레임";
                _btnReturnFilmstripToCurrent.Location = new Point(pnlFrameThumbnailStrip.Left + margin, y);
            }
            else
            {
                _btnReturnFilmstripToCurrent.Text = "선택 프레임 >";
                _btnReturnFilmstripToCurrent.Location = new Point(
                    pnlFrameThumbnailStrip.Right - _btnReturnFilmstripToCurrent.Width - margin, y);
            }

            _btnReturnFilmstripToCurrent.BringToFront();
        }

        private void PnlFrameThumbnailStrip_MouseClick(object? sender, MouseEventArgs e)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            if (_suppressNextFilmstripClick)
            {
                _suppressNextFilmstripClick = false;
                return;
            }

            int index = GetFilmstripIndexAt(e.X);
            if (index >= 0 && index < _visibleFrames.Count)
                ApplyFrameSelectionFromFilmstrip(index);
        }

        private void PnlFrameThumbnailStrip_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _visibleFrames == null || _visibleFrames.Count == 0)
                return;

            int index = GetFilmstripIndexAt(e.X);
            if (index < 0 || index >= _visibleFrames.Count)
                return;

            LockFilmstripViewAtCurrentVisualPosition();
            _filmstripDragStartIndex = index;
            _lastFilmstripDragIndex = index;
            _isDraggingFilmstripSelection = true;
            _filmstripDragMoved = false;
            pnlFrameThumbnailStrip.Capture = true;
        }

        private void PnlFrameThumbnailStrip_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDraggingFilmstripSelection || e.Button != MouseButtons.Left || _filmstripDragStartIndex < 0)
                return;

            AutoScrollFilmstripDuringDrag(e.X);

            int index = GetFilmstripIndexAt(e.X);
            if (index < 0)
                index = GetNearestFilmstripIndexAt(e.X);
            if (index < 0 || index >= _visibleFrames.Count)
                return;

            if (index == _lastFilmstripDragIndex)
                return;

            _lastFilmstripDragIndex = index;
            _filmstripDragMoved = true;
            SelectFrameGridRange(
                _filmstripDragStartIndex,
                index,
                centerCatalog: false,
                renderChart: ShouldRenderDragChart(),
                updateFilmstripNow: true);
        }

        private void PnlFrameThumbnailStrip_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!_isDraggingFilmstripSelection)
                return;

            if (_filmstripDragMoved)
            {
                int releaseIndex = GetFilmstripIndexAt(e.X);
                if (releaseIndex < 0)
                    releaseIndex = GetNearestFilmstripIndexAt(e.X);

                if (releaseIndex >= 0 && releaseIndex < _visibleFrames.Count)
                {
                    SelectFrameGridRange(
                        _filmstripDragStartIndex,
                        releaseIndex,
                        centerCatalog: false,
                        renderChart: true,
                        updateFilmstripNow: true);
                }

                _suppressNextFilmstripClick = true;
            }

            ClearDragSelectionPreview();
            _isDraggingFilmstripSelection = false;
            _filmstripDragStartIndex = -1;
            _lastFilmstripDragIndex = -1;
            pnlFrameThumbnailStrip.Capture = false;
        }

        private void PnlFrameThumbnailStrip_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;

            int wheelSteps = Math.Max(-6, Math.Min(6, e.Delta / 120));
            if (wheelSteps == 0)
                wheelSteps = e.Delta > 0 ? 1 : -1;

            LockFilmstripViewAtCurrentVisualPosition();
            SetFilmstripViewOffset(_filmstripViewOffsetPixels - wheelSteps * 88, syncScrollBar: true);
        }

        private void LockFilmstripViewAtCurrentVisualPosition()
        {
            if (_isFilmstripViewLockedByUser)
                return;

            int currentSlidingOffset = GetFilmstripSlidingOffsetX();
            _filmstripViewOffsetPixels = 8 - currentSlidingOffset;
            _isFilmstripViewLockedByUser = true;
            SyncFilmstripScrollBarToView();
            UpdateReturnFilmstripButton();
        }

        private void SyncFilmstripScrollBarToView()
        {
            if (_filmstripScrollBar == null)
                return;

            _isSyncingFilmstripScrollBar = true;
            try
            {
                int maxValue = Math.Max(_filmstripScrollBar.Minimum, _filmstripScrollBar.Maximum - _filmstripScrollBar.LargeChange + 1);
                _filmstripScrollBar.Value = Math.Clamp(_filmstripViewOffsetPixels, _filmstripScrollBar.Minimum, maxValue);
            }
            finally
            {
                _isSyncingFilmstripScrollBar = false;
            }
        }

        private int GetFilmstripIndexAt(int mouseX)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return -1;

            var (first, last) = GetVisibleFilmstripIndexRange();
            for (int index = first; index <= last; index++)
            {
                if (GetFilmstripItemRect(index).Contains(mouseX, pnlFrameThumbnailStrip.ClientSize.Height / 2))
                    return index;
            }

            return -1;
        }

        private int GetNearestFilmstripIndexAt(int mouseX)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return -1;

            const int pitch = 88;
            int slidingOffsetX = GetFilmstripSlidingOffsetX();
            int index = (int)Math.Round((mouseX - slidingOffsetX) / (double)pitch);
            return Math.Clamp(index, 0, _visibleFrames.Count - 1);
        }

        private void AutoScrollFilmstripDuringDrag(int mouseX)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return;

            int edge = Math.Max(36, pnlFrameThumbnailStrip.ClientSize.Width / 12);
            int nextOffset = _filmstripViewOffsetPixels;

            if (mouseX <= edge)
                nextOffset -= 88;
            else if (mouseX >= pnlFrameThumbnailStrip.ClientSize.Width - edge)
                nextOffset += 88;

            if (nextOffset == _filmstripViewOffsetPixels)
                return;

            _isFilmstripViewLockedByUser = true;
            SetFilmstripViewOffset(nextOffset, syncScrollBar: true);
        }

        private int GetFilmstripSlidingOffsetX()
        {
            if (_visibleFrames != null && _visibleFrames.Count > 0 && !_isFilmstripViewLockedByUser && !_isDraggingFilmstripSelection)
            {
                const int itemWidth = 80;
                const int pitch = 88;
                int currentIndex = Math.Clamp(_currentFrameIndex, 0, _visibleFrames.Count - 1);
                int centerLeft = (pnlFrameThumbnailStrip.ClientSize.Width - itemWidth) / 2;
                return centerLeft - currentIndex * pitch;
            }

            return 8 - _filmstripViewOffsetPixels;
        }

        private Rectangle GetFilmstripItemRect(int index)
        {
            const int itemWidth = 80;
            const int itemHeight = 44;
            const int pitch = 88;
            int top = Math.Max(4, (pnlFrameThumbnailStrip.ClientSize.Height - itemHeight) / 2 - 2);
            return new Rectangle(GetFilmstripSlidingOffsetX() + index * pitch, top, itemWidth, itemHeight);
        }

        private (int First, int Last) GetVisibleFilmstripIndexRange(int paddingFrames = 0)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return (0, -1);

            const int pitch = 88;
            int slidingOffsetX = GetFilmstripSlidingOffsetX();
            int first = Math.Max(0, (int)Math.Floor((-slidingOffsetX - pitch) / (double)pitch) - paddingFrames);
            int last = Math.Min(_visibleFrames.Count - 1,
                (int)Math.Ceiling((pnlFrameThumbnailStrip.ClientSize.Width - slidingOffsetX + pitch) / (double)pitch) + paddingFrames);
            return (first, last);
        }

        private void PnlFrameThumbnailStrip_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.Clear(System.Drawing.Color.FromArgb(28, 31, 36));
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (_visibleFrames == null || _visibleFrames.Count == 0)
            {
                TextRenderer.DrawText(e.Graphics, "프레임 썸네일", new Font("resource/PretendardVariable.ttf", 9F),
                    pnlFrameThumbnailStrip.ClientRectangle, System.Drawing.Color.Gainsboro,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            const int itemWidth = 80;
            const int itemHeight = 44;
            int top = Math.Max(4, (pnlFrameThumbnailStrip.ClientSize.Height - itemHeight) / 2 - 2);
            int centerLeft = (pnlFrameThumbnailStrip.ClientSize.Width - itemWidth) / 2;
            var (first, last) = GetVisibleFilmstripIndexRange();

            using var borderPen = new Pen(System.Drawing.Color.FromArgb(88, 96, 110));
            using var centerHighlightPen = new Pen(System.Drawing.Color.FromArgb(0, 220, 120), 4);
            using var centerGlowPen = new Pen(System.Drawing.Color.FromArgb(170, 0, 220, 120), 7);
            using var selectedPen = new Pen(System.Drawing.Color.FromArgb(74, 144, 226), 4);
            using var selectedFillBrush = new SolidBrush(System.Drawing.Color.FromArgb(65, 74, 144, 226));
            using var deletedBrush = new SolidBrush(System.Drawing.Color.FromArgb(120, 30, 30, 30));
            using var deletedXPen = new Pen(System.Drawing.Color.FromArgb(230, 220, 40, 40), 5);
            using var reviewPen = new Pen(System.Drawing.Color.FromArgb(255, 152, 0), 2);
            using var numberBackBrush = new SolidBrush(System.Drawing.Color.FromArgb(170, 0, 0, 0));
            using var numberFont = new Font("맑은 고딕", 7.5F, System.Drawing.FontStyle.Bold);
            HashSet<int> selectedIndices = GetSelectedVisibleIndices();

            for (int i = first; i <= last; i++)
            {
                var rect = GetFilmstripItemRect(i);
                var frame = _visibleFrames[i];

                Bitmap? thumbnail = GetFrameThumbnail(i);
                if (thumbnail != null)
                    e.Graphics.DrawImage(thumbnail, rect);
                else
                    e.Graphics.FillRectangle(Brushes.DimGray, rect);

                if (frame.IsDeleted)
                {
                    e.Graphics.FillRectangle(deletedBrush, rect);
                    e.Graphics.DrawLine(deletedXPen, rect.Left + 9, rect.Top + 7, rect.Right - 9, rect.Bottom - 7);
                    e.Graphics.DrawLine(deletedXPen, rect.Right - 9, rect.Top + 7, rect.Left + 9, rect.Bottom - 7);
                }

                if (selectedIndices.Contains(i))
                    e.Graphics.FillRectangle(selectedFillBrush, rect);

                DrawFilmstripImageNumber(e.Graphics, rect, frame, numberBackBrush, numberFont);

                e.Graphics.DrawRectangle(frame.NeedsReview ? reviewPen : borderPen, rect);

                if (selectedIndices.Contains(i))
                    e.Graphics.DrawRectangle(selectedPen, Rectangle.Inflate(rect, 2, 2));
            }

            Rectangle highlightRect;
            if (_isFilmstripViewLockedByUser || _isDraggingFilmstripSelection)
            {
                int currentIndex = Math.Clamp(_currentFrameIndex, 0, _visibleFrames.Count - 1);
                highlightRect = GetFilmstripItemRect(currentIndex);
            }
            else
            {
                highlightRect = new Rectangle(centerLeft, top, itemWidth, itemHeight);
            }

            if (highlightRect.Right >= 0 && highlightRect.Left <= pnlFrameThumbnailStrip.ClientSize.Width)
            {
                e.Graphics.DrawRectangle(centerGlowPen, Rectangle.Inflate(highlightRect, 4, 4));
                e.Graphics.DrawRectangle(centerHighlightPen, Rectangle.Inflate(highlightRect, 3, 3));
            }
        }

        private void DrawFilmstripImageNumber(Graphics graphics, Rectangle rect, FrameData frame, Brush backBrush, Font font)
        {
            string text = frame.ImageNumber >= 0
                ? frame.ImageNumber.ToString(CultureInfo.InvariantCulture)
                : "-";

            var labelRect = new Rectangle(rect.Left, rect.Bottom - 14, rect.Width, 14);
            graphics.FillRectangle(backBrush, labelRect);
            TextRenderer.DrawText(
                graphics,
                text,
                font,
                labelRect,
                System.Drawing.Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void EnableDoubleBuffering(Control control)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(control, true, null);
        }

        private Bitmap? GetFrameThumbnail(int index)
        {
            if (_visibleFrames == null || index < 0 || index >= _visibleFrames.Count)
                return null;

            if (_thumbnailCache.TryGetValue(index, out Bitmap? cached))
                return cached;

            string path = ResolveImagePath(_visibleFrames[index].Name);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return null;

            try
            {
                using var fs = File.OpenRead(path);
                using var image = System.Drawing.Image.FromStream(fs);
                var thumbnail = new Bitmap(80, 44);
                using (Graphics graphics = Graphics.FromImage(thumbnail))
                {
                    graphics.Clear(System.Drawing.Color.FromArgb(35, 38, 44));
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    float scale = Math.Min(80f / image.Width, 44f / image.Height);
                    int drawWidth = Math.Max(1, (int)(image.Width * scale));
                    int drawHeight = Math.Max(1, (int)(image.Height * scale));
                    int x = (80 - drawWidth) / 2;
                    int y = (44 - drawHeight) / 2;
                    graphics.DrawImage(image, x, y, drawWidth, drawHeight);
                }

                _thumbnailCache[index] = thumbnail;
                TrimThumbnailCache();
                return thumbnail;
            }
            catch
            {
                return null;
            }
        }

        private void TrimThumbnailCache()
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
            {
                ClearThumbnailCache();
                return;
            }

            var keep = new HashSet<int>();
            var (first, last) = GetVisibleFilmstripIndexRange(paddingFrames: 18);
            for (int i = first; i <= last; i++)
                keep.Add(i);

            foreach (int key in _thumbnailCache.Keys.ToList())
            {
                if (keep.Contains(key)) continue;
                _thumbnailCache[key].Dispose();
                _thumbnailCache.Remove(key);
            }
        }

        private void ClearThumbnailCache()
        {
            foreach (Bitmap bitmap in _thumbnailCache.Values)
                bitmap.Dispose();
            _thumbnailCache.Clear();
        }

        private void RefreshFilmstripHighlight(bool forceFullRefresh = false)
        {
            if (pnlFrameThumbnailStrip.IsDisposed) return;

            pnlFrameThumbnailStrip.Invalidate();
            _previousThumbnailHighlightIndex = _currentFrameIndex;
        }

        private void ScrollThumbnailToCurrentFrame()
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0) return;
            if (!_isFilmstripViewLockedByUser)
            {
                SetFilmstripViewOffset(GetCenteredFilmstripOffset(_currentFrameIndex), syncScrollBar: true);
            }
            else
            {
                UpdateReturnFilmstripButton();
                RefreshFilmstripHighlight(forceFullRefresh: true);
            }
        }

        /// <summary>
        /// 데이터 정리 패널 안의 입력, 버튼, 상태 안내가 서로 겹치지 않도록 한곳에서 위치를 계산합니다.
        /// WinForms 디자이너 좌표는 화면 크기에 취약하므로, 실행 시 현재 패널 폭을 기준으로 다시 배치합니다.
        /// </summary>
        private void ArrangeDataCleanerPanel()
        {
            int panelWidth = Math.Max(280, grpDataCleaner.ClientSize.Width);
            int panelHeight = Math.Max(240, grpDataCleaner.ClientSize.Height);
            int left = 14;
            int rightPadding = 14;
            int contentWidth = Math.Max(220, panelWidth - left - rightPadding);
            int labelWidth = Math.Clamp(contentWidth / 3, 92, 112);
            int inputGap = 6;
            int inputWidth = Math.Max(52, (contentWidth - labelWidth - 22 - inputGap * 3) / 2);
            int row1 = 30;
            int row2 = 62;
            int row3 = 92;
            int row4 = 122;
            int rowHeight = 27;

            int minX = left + labelWidth;
            int separatorX = minX + inputWidth + inputGap;
            int maxX = separatorX + 22;

            int comboLabelX = left;
            int comboX = left + 82;
            int comboWidth = Math.Max(120, contentWidth - 82);
            int buttonWidth = contentWidth;
            int sectionGap = panelHeight < 360 ? 10 : 16;
            int buttonGap = panelHeight < 330 ? 5 : 8;
            int buttonHeight = Math.Clamp((panelHeight - 230) / 8, 24, 32);

            PlaceFilterRangeRow(lblAngleRange, txtAngleMinFilter, lblAngleRangeSeparator, txtAngleMaxFilter,
                left, minX, separatorX, maxX, row1, labelWidth, inputWidth, rowHeight);
            PlaceFilterRangeRow(lblThrottleRange, txtThrottleMinFilter, lblThrottleRangeSeparator, txtThrottleMaxFilter,
                left, minX, separatorX, maxX, row2, labelWidth, inputWidth, rowHeight);

            PlaceFilterComboRow(lblModeFilter, cmbModeFilter, comboLabelX, comboX, row3, comboWidth, rowHeight);
            PlaceFilterComboRow(lblScenarioFilter, cmbScenarioFilter, comboLabelX, comboX, row4, comboWidth, rowHeight);

            int buttonY = row4 + rowHeight + sectionGap;
            PlaceButton(btnApplyFrameFilter, left, buttonY, buttonWidth, buttonHeight);
            PlaceButton(btnClearFrameFilter, left, buttonY += buttonHeight + buttonGap, buttonWidth, buttonHeight);

            if (_btnShowReviewCandidates != null)
            {
                PlaceButton(_btnShowReviewCandidates, left, buttonY += buttonHeight + buttonGap, buttonWidth, buttonHeight);
                buttonY += buttonHeight + buttonGap;
            }

            PlaceButton(btnExcludeSelectedFrames, left, buttonY, buttonWidth, buttonHeight);
            PlaceButton(btnRestoreFrames, left, buttonY += buttonHeight + buttonGap, buttonWidth, buttonHeight);

            int finalActionY = Math.Max(buttonY + buttonHeight + sectionGap, panelHeight - (buttonHeight * 2 + buttonGap + 18));
            PlaceButton(btnSaveCleanupState, left, finalActionY, buttonWidth, buttonHeight);
            PlaceButton(btnExportCleanDataset, left, finalActionY + buttonHeight + buttonGap, buttonWidth, buttonHeight);

            LayoutCleanupGuidanceControls();
        }

        private static void PlaceFilterRangeRow(
            System.Windows.Forms.Label label,
            TextBox minTextBox,
            System.Windows.Forms.Label separator,
            TextBox maxTextBox,
            int labelX,
            int minX,
            int separatorX,
            int maxX,
            int y,
            int labelWidth,
            int inputWidth,
            int rowHeight)
        {
            label.AutoSize = false;
            label.Location = new Point(labelX, y + 3);
            label.Size = new Size(labelWidth, rowHeight);
            minTextBox.Location = new Point(minX, y);
            minTextBox.Size = new Size(inputWidth, rowHeight);
            separator.Location = new Point(separatorX, y + 4);
            separator.Size = new Size(28, rowHeight);
            maxTextBox.Location = new Point(maxX, y);
            maxTextBox.Size = new Size(inputWidth, rowHeight);
        }

        private static void PlaceFilterComboRow(
            System.Windows.Forms.Label label,
            ComboBox comboBox,
            int labelX,
            int comboX,
            int y,
            int comboWidth,
            int rowHeight)
        {
            label.AutoSize = false;
            label.Location = new Point(labelX, y + 3);
            label.Size = new Size(84, rowHeight);
            comboBox.Location = new Point(comboX, y);
            comboBox.Size = new Size(comboWidth, rowHeight);
        }

        private static void PlaceButton(Button button, int x, int y, int width, int height)
        {
            button.Location = new Point(x, y);
            button.Size = new Size(width, height);
            button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        /// <summary>
        /// 오른쪽 프레임 정보 영역을 촘촘하게 재배치합니다.
        /// 검토 힌트와 재생속도 입력이 겹치지 않도록 런타임에서 위치를 정리합니다.
        /// </summary>
        private void ArrangeFrameInfoPanel() => ArrangeFrameInfoPanel(Math.Max(0, grpDataExplorer.ClientSize.Height - 54));

        private void ArrangeFrameInfoPanel(int playbackY)
        {
            int margin = 14;
            int y = Math.Max(splitContainerFramePreview.Bottom + 8, playbackY - 42);
            int availableWidth = Math.Max(360, grpDataExplorer.ClientSize.Width - margin * 2);
            int gap = 12;
            int valueWidth = Math.Max(150, (availableWidth - gap * 3) / 4);

            lblFrameValue.Font = new Font("resource/PretendardVariable.ttf", 12F, System.Drawing.FontStyle.Bold);
            lblAngleValue.Font = new Font("resource/PretendardVariable.ttf", 10.5F);
            lblThrottleValue.Font = new Font("resource/PretendardVariable.ttf", 10.5F);
            lblModeValue.Font = new Font("resource/PretendardVariable.ttf", 10.5F);
            lblPlayInterval.Font = new Font("resource/PretendardVariable.ttf", 9.5F, System.Drawing.FontStyle.Bold);

            lblFrameValue.BringToFront();

            lblAngleValue.Location = new Point(margin, y);
            lblAngleValue.Size = new Size(valueWidth, 28);
            lblThrottleValue.Location = new Point(lblAngleValue.Right + gap, y);
            lblThrottleValue.Size = new Size(valueWidth, 28);
            lblModeValue.Location = new Point(lblThrottleValue.Right + gap, y);
            lblModeValue.Size = new Size(valueWidth, 28);

            if (_lblFrameReviewHint != null)
            {
                _lblFrameReviewHint.Location = new Point(lblModeValue.Right + gap, y);
                _lblFrameReviewHint.Size = new Size(Math.Max(160, availableWidth - (lblModeValue.Right + gap - margin)), 40);
                _lblFrameReviewHint.BringToFront();
            }
        }

        private void ConfigureReviewCandidateControls()
        {
            if (_btnShowReviewCandidates == null)
            {
                _btnShowReviewCandidates = btnShowReviewCandidates;
                _btnShowReviewCandidates.Click += BtnShowReviewCandidates_Click;
            }

            _btnShowReviewCandidates.Text = "이상 후보 보기";
            _btnShowReviewCandidates.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _btnShowReviewCandidates.UseVisualStyleBackColor = false;
            _btnShowReviewCandidates.BackColor = System.Drawing.Color.FromArgb(255, 249, 219);
            if (_btnShowReviewCandidates.Parent != grpDataCleaner)
                grpDataCleaner.Controls.Add(_btnShowReviewCandidates);
            _btnShowReviewCandidates.BringToFront();

            if (_lblFrameReviewHint == null)
            {
                _lblFrameReviewHint = new System.Windows.Forms.Label
                {
                    Name = "lblFrameReviewHint",
                    Text = "검토: -",
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new Point(lblModeValue.Left, 326),
                    Size = new Size(lblModeValue.Width, 54),
                    Font = new Font("resource/PretendardVariable.ttf", 9F),
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
                    Font = new System.Drawing.Font("resource/PretendardVariable.ttf", 9.5F, System.Drawing.FontStyle.Bold),
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
                    Font = new System.Drawing.Font("resource/PretendardVariable.ttf", 9F),
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
            if (grpDataCleaner.ClientSize.Width < 420)
            {
                if (_lblCleanupSummary != null)
                    _lblCleanupSummary.Visible = false;
                if (_lblCleanupWorkflowHint != null)
                    _lblCleanupWorkflowHint.Visible = false;
                return;
            }

            int buttonLeft = btnApplyFrameFilter.Left > 0
                ? btnApplyFrameFilter.Left
                : grpDataCleaner.ClientSize.Width - 500;
            int width = Math.Max(300, buttonLeft - 36);
            int y = Math.Max(148, grpDataCleaner.ClientSize.Height - 54);

            if (_lblCleanupSummary != null)
            {
                _lblCleanupSummary.Visible = true;
                _lblCleanupSummary.Location = new Point(20, y);
                _lblCleanupSummary.Size = new Size(width, 22);
            }

            if (_lblCleanupWorkflowHint != null)
            {
                _lblCleanupWorkflowHint.Visible = true;
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
                ApplyFrameCatalogRowStyle(row);
        }

        private void ApplyFrameCatalogRowStyle(DataGridViewRow row)
        {
            if (row.DataBoundItem is not FrameData frame) return;

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

        // 공통 로직

        /// <summary>
        /// 로딩 중 버튼 비활성화와 커서 변경을 처리합니다.
        /// </summary>
        private void SetLoadingState(bool loading)
        {
            btnOpenDataFolder.Enabled = !loading;
            btnReloadData.Enabled = !loading;
            btnApplyFrameFilter.Enabled = !loading;
            btnClearFrameFilter.Enabled = !loading;
            if (_btnShowReviewCandidates != null)
                _btnShowReviewCandidates.Enabled = !loading;
            btnExcludeSelectedFrames.Enabled = !loading;
            btnRestoreFrames.Enabled = !loading;
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
            ClearThumbnailCache();
            UpdateThumbnailStripScrollArea();
            RefreshFilmstripHighlight(forceFullRefresh: true);
            RenderDataViewerFrameChart();

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
            _lastSelectedFrameIndex = idx;

            // 선택 변경 이벤트가 중복 실행되지 않도록 잠시 막습니다.
            _isFrameSelectionUpdating = true;
            dgvFrameCatalog.ClearSelection();
            if (idx >= 0 && idx < dgvFrameCatalog.Rows.Count)
            {
                dgvFrameCatalog.Rows[idx].Selected = true;
                dgvFrameCatalog.CurrentCell = dgvFrameCatalog.Rows[idx].Cells[0];
                ScrollCatalogToFrame(idx);
            }
            _isFrameSelectionUpdating = false;

            if (trkFrameTimeline.Value != idx)
                trkFrameTimeline.Value = idx;

            DisplayFrameAtIndex(idx);
            RefreshSelectionVisuals();
        }

        private void DisplayFrameAtIndex(int idx)
        {
            if (_visibleFrames == null || idx < 0 || idx >= _visibleFrames.Count) return;

            _currentFrameIndex = idx;

            if (trkFrameTimeline.Value != idx)
                trkFrameTimeline.Value = idx;
            UpdateDataViewerTimelinePlayhead(autoScroll: _isPlaybackRunning);

            var frame = _visibleFrames[idx];

            string resolvedPath = ResolveImagePath(frame.Name);
            UpdatePreviewImage(resolvedPath, frame);

            lblFrameValue.Text = $"프레임: {idx + 1} / {_visibleFrames.Count}";
            lblAngleValue.Text = $"방향값: {frame.Angle:0.000} ({frame.SteeringText})";
            lblThrottleValue.Text = $"속도값: {frame.Throttle:0.000} ({frame.ThrottleText})";
            lblModeValue.Text = $"주행 방식: {frame.ModeDescription}";
            if (_lblFrameReviewHint != null)
            {
                _lblFrameReviewHint.Text = $"검토: {frame.ReviewHint}";
                _lblFrameReviewHint.ForeColor = frame.NeedsReview
                    ? System.Drawing.Color.FromArgb(150, 95, 0)
                    : System.Drawing.Color.DimGray;
            }
            ScrollThumbnailToCurrentFrame();
            UpdateStatusLabels();
            MaybeRequestPilotPredictionForCurrentFrame();
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

        private async Task<(double angle, double throttle)> GetPilotPredictionAsync(string imagePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                throw new FileNotFoundException("추론할 이미지 파일을 찾을 수 없습니다.", imagePath);

            string endpoint = GetPilotPredictUrl();
            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
            string json = JsonSerializer.Serialize(new
            {
                image_base64 = Convert.ToBase64String(imageBytes)
            });

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _pilotHttpClient.PostAsync(endpoint, content, cancellationToken);
            string responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            using JsonDocument document = JsonDocument.Parse(responseText);
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("error", out JsonElement error))
                throw new InvalidOperationException(error.GetString() ?? "Python 추론 서버 오류");

            response.EnsureSuccessStatusCode();
            double angle = ReadJsonDouble(root, "angle");
            double throttle = ReadJsonDouble(root, "throttle");
            return (angle, throttle);
        }

        private static double ReadJsonDouble(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out JsonElement value))
                return 0.0;

            if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out double number))
                return number;

            return double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out number)
                ? number
                : 0.0;
        }

        private string GetPilotPredictUrl()
        {
            string url = txtPilotApiUrl?.Text.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(url))
                return "http://127.0.0.1:5000/predict";

            return url.EndsWith("/predict", StringComparison.OrdinalIgnoreCase)
                ? url
                : url.TrimEnd('/') + "/predict";
        }

        private string GetPilotHealthUrl()
        {
            string url = GetPilotPredictUrl();
            return url.EndsWith("/predict", StringComparison.OrdinalIgnoreCase)
                ? url[..^"/predict".Length] + "/health"
                : url.TrimEnd('/') + "/health";
        }

        private async Task StartPilotServerAsync(bool showErrorMessage = true)
        {
            if (_pilotServerProcess is { HasExited: false })
            {
                if (showErrorMessage)
                {
                    MessageBox.Show("이미 추론 서버가 실행 중입니다.",
                        "추론 서버", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            if (!_isValidationEnvironmentReady)
            {
                MessageBox.Show("먼저 데이터 폴더와 모델 파일을 선택하고 자동 감지를 완료해 주세요.",
                    "추론 환경 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateValidationActionButtons();
                return;
            }

            string modelPath = txtValidationModelPath?.Text.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                MessageBox.Show("먼저 검증할 모델 파일을 선택해 주세요.",
                    "모델 파일 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!modelPath.StartsWith("/", StringComparison.Ordinal) &&
                !modelPath.StartsWith("~/", StringComparison.Ordinal) &&
                !File.Exists(modelPath))
            {
                MessageBox.Show("선택한 모델 파일을 찾을 수 없습니다.\n\n" + modelPath,
                    "모델 파일 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!HasTrainingEnvironmentInputs())
                await DetectValidationEnvironmentAsync(showMessage: false);

            if (!_isValidationEnvironmentReady)
            {
                UpdateValidationActionButtons();
                return;
            }

            if (!TryBuildPilotServerCommand(modelPath, out var arguments, out string displayArguments))
                return;

            try
            {
                await StopStalePilotServerOnPortAsync(5000);
                if (await IsTcpPortOpenAsync("127.0.0.1", 5000, 500))
                {
                    throw new InvalidOperationException("[port] 5000번 포트가 이미 사용 중입니다.");
                }

                _pilotServerProcess = new Process
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
                    _pilotServerProcess.StartInfo.ArgumentList.Add(argument);

                rtbPilotServerLog?.Clear();
                AppendPilotServerLog("[서버] 추론 서버를 시작합니다.");
                AppendPilotServerLog("[서버] 실행 명령: wsl " + displayArguments);
                if (lblPilotStatus != null)
                    lblPilotStatus.Text = "상태: 서버 시작 중...";

                _pilotServerProcess.Start();
                btnStartPilotServer!.Enabled = false;
                btnStopPilotServer!.Enabled = true;

                _ = ReadPilotServerStreamAsync(_pilotServerProcess.StandardOutput);
                _ = ReadPilotServerStreamAsync(_pilotServerProcess.StandardError);
                _ = WatchPilotServerExitAsync(_pilotServerProcess);

                bool serverHealthy = await WaitForPilotServerHealthAsync(TimeSpan.FromSeconds(10));
                if (!serverHealthy)
                    throw new InvalidOperationException("[health] 서버는 시작됐지만 /health 응답을 받지 못했습니다.");

                MaybeRequestPilotPredictionForCurrentFrame();
            }
            catch (Exception ex)
            {
                btnStartPilotServer!.Enabled = true;
                btnStopPilotServer!.Enabled = false;
                UpdateValidationActionButtons();
                if (lblPilotStatus != null)
                    lblPilotStatus.Text = "상태: 서버 시작 실패";

                if (showErrorMessage)
                {
                    MessageBox.Show(
                        GetPilotServerFailureMessage(ex),
                        "추론 서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    AppendPilotServerLog("[서버] 자동 후보 계산용 서버 시작 실패: " + ex.Message);
                }
            }
        }

        private async Task<bool> IsTcpPortOpenAsync(string host, int port, int timeoutMs)
        {
            try
            {
                using var client = new TcpClient();
                Task connectTask = client.ConnectAsync(host, port);
                Task completedTask = await Task.WhenAny(connectTask, Task.Delay(timeoutMs));
                return completedTask == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> WaitForPilotServerHealthAsync(TimeSpan timeout)
        {
            DateTime deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                if (_pilotServerProcess == null || _pilotServerProcess.HasExited)
                    return false;

                if (await VerifyPilotServerHealthAsync(showMessage: false))
                    return true;

                await Task.Delay(600);
            }

            return false;
        }

        private string GetPilotServerFailureMessage(Exception ex)
        {
            string log = rtbPilotServerLog?.Text ?? string.Empty;
            string detail = (ex.Message + "\n" + log).ToLowerInvariant();

            if (detail.Contains("[port]") ||
                detail.Contains("address already in use") ||
                detail.Contains("port 5000") ||
                detail.Contains("5000번 포트"))
            {
                return "추론 서버를 시작하지 못했습니다.\n\n" +
                    "원인: 5000번 포트가 이미 사용 중입니다.\n\n" +
                    "해결 방법:\n" +
                    "1. 기존 추론 서버 또는 다른 Flask 서버를 종료하세요.\n" +
                    "2. 그래도 안 되면 WSL 터미널에서 `lsof -i :5000`으로 점유 프로세스를 확인하세요.\n\n" +
                    "오류 내용: " + ex.Message;
            }

            if (detail.Contains("모델 파일을 찾을 수 없습니다") ||
                detail.Contains("no such file") ||
                detail.Contains("load_model") ||
                detail.Contains("unable to open file") ||
                detail.Contains(".keras") ||
                detail.Contains(".h5"))
            {
                return "추론 서버를 시작하지 못했습니다.\n\n" +
                    "원인: 모델 파일 경로가 잘못되었거나 모델 파일을 읽을 수 없습니다.\n\n" +
                    "해결 방법:\n" +
                    "1. 추론/검증 탭에서 실제 존재하는 `.keras` 또는 `.h5` 모델 파일을 다시 선택하세요.\n" +
                    "2. Windows 경로에 한글/공백이 많으면 모델을 `문서\\donkeycar_models`처럼 단순한 경로에 두고 다시 시도하세요.\n\n" +
                    "오류 내용: " + ex.Message;
            }

            if (detail.Contains("conda") ||
                detail.Contains("environment") ||
                detail.Contains("modulenotfounderror") ||
                detail.Contains("tensorflow") ||
                detail.Contains("donkeycar") ||
                detail.Contains("python"))
            {
                return "추론 서버를 시작하지 못했습니다.\n\n" +
                    "원인: Python/Conda/DonkeyCar 환경이 준비되지 않았습니다.\n\n" +
                    "해결 방법:\n" +
                    "1. 학습 실행 탭에서 자동 감지를 다시 실행하세요.\n" +
                    "2. Python 환경명과 Conda 경로가 맞는지 확인하세요.\n" +
                    "3. 해당 환경에 donkeycar, tensorflow, pillow가 설치되어 있는지 확인하세요.\n\n" +
                    "오류 내용: " + ex.Message;
            }

            return "추론 서버 실행 중 오류가 발생했습니다.\n\n" +
                "WSL, Conda 환경, 모델 경로, 서버 포트 상태를 확인해 주세요.\n\n" +
                "오류 내용: " + ex.Message;
        }

        private void SyncPlaybackSpeedFromCombo(ComboBox source)
        {
            string selected = source.SelectedItem?.ToString() ?? "1배";
            if (_cmbPlaybackSpeed != null && !ReferenceEquals(source, _cmbPlaybackSpeed))
                _cmbPlaybackSpeed.SelectedItem = selected;
            if (_cmbValidationPlaybackSpeed != null && !ReferenceEquals(source, _cmbValidationPlaybackSpeed))
                _cmbValidationPlaybackSpeed.SelectedItem = selected;
        }

        private bool TryBuildPilotServerCommand(string modelPath, out List<string> arguments, out string displayArguments)
        {
            arguments = new List<string>();
            displayArguments = string.Empty;

            string distro = NormalizeWslDistroName(cmbTrainingWslDistro.Text);
            string condaPath = cmbCondaPath.Text.Trim();
            string envName = txtTrainingPythonEnvName.Text.Trim();
            string scriptPath = GetPilotServerScriptPath();

            if (string.IsNullOrWhiteSpace(distro))
            {
                MessageBox.Show("WSL Ubuntu를 자동 감지하거나 학습 탭에서 선택해 주세요.",
                    "추론 환경 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(condaPath))
            {
                MessageBox.Show("Conda 경로를 자동 감지하거나 학습 탭에서 선택해 주세요.",
                    "추론 환경 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(envName))
            {
                MessageBox.Show("Python 환경명을 자동 감지하거나 학습 탭에서 입력해 주세요.",
                    "추론 환경 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!File.Exists(scriptPath))
            {
                MessageBox.Show("추론 서버 스크립트를 찾을 수 없습니다.\n\n" + scriptPath,
                    "추론 서버 파일 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string wslCondaPath = ConvertToWslPath(condaPath);
            string wslScriptPath = ConvertToWslPath(scriptPath);
            string wslModelPath = ConvertToWslPath(modelPath);
            string command =
                "export PYTHONUNBUFFERED=1 && " +
                "if [ ! -x " + QuotePathForBash(wslCondaPath) + " ]; then echo " +
                QuoteForBash("[error] Conda 실행 파일을 찾을 수 없습니다: " + wslCondaPath) + "; exit 127; fi && " +
                "if [ ! -f " + QuotePathForBash(wslScriptPath) + " ]; then echo " +
                QuoteForBash("[error] 추론 서버 스크립트를 찾을 수 없습니다: " + wslScriptPath) + "; exit 2; fi && " +
                "if [ ! -f " + QuotePathForBash(wslModelPath) + " ]; then echo " +
                QuoteForBash("[error] 모델 파일을 찾을 수 없습니다: " + wslModelPath) + "; exit 2; fi && " +
                QuotePathForBash(wslCondaPath) + " run --no-capture-output -n " + QuoteForBash(envName) + " " +
                "python " + QuotePathForBash(wslScriptPath) + " --model " + QuotePathForBash(wslModelPath) +
                " --host 127.0.0.1 --port 5000";

            arguments.AddRange(new[] { "-d", distro, "bash", "-lc", command });
            displayArguments = "-d " + QuoteProcessArgument(distro) + " bash -lc " + QuoteForBash(command);
            return true;
        }

        private async Task StopStalePilotServerOnPortAsync(int port)
        {
            string distro = NormalizeWslDistroName(cmbTrainingWslDistro.Text);
            if (string.IsNullOrWhiteSpace(distro))
                return;

            // 이전 실행에서 앱이 비정상 종료되면 WSL 안에 추론 서버만 남아 5000번 포트를 잡을 수 있습니다.
            // 새 서버를 안정적으로 띄우기 위해 같은 스크립트/포트 조합의 오래된 프로세스만 정리합니다.
            string script =
                "pkill -f " + QuoteForBash("pilot_inference_server.py.*--port " + port.ToString(CultureInfo.InvariantCulture)) +
                " 2>/dev/null || true";
            await RunWslBashCaptureAsync(distro, script, 5000, useLoginShell: false);
        }

        private string GetPilotServerScriptPath()
        {
            string outputScriptPath = Path.Combine(AppContext.BaseDirectory, "python", "pilot_inference_server.py");
            if (File.Exists(outputScriptPath))
                return outputScriptPath;

            return Path.Combine(Application.StartupPath, "python", "pilot_inference_server.py");
        }

        private void StopPilotServer(bool showMessage)
        {
            try
            {
                if (_pilotServerProcess == null || _pilotServerProcess.HasExited)
                {
                    if (showMessage)
                    {
                        MessageBox.Show("중지할 추론 서버가 없습니다.",
                            "추론 서버", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return;
                }

                _pilotServerProcess.Kill(true);
                AppendPilotServerLog("[서버] 추론 서버를 중지했습니다.");
                if (lblPilotStatus != null)
                    lblPilotStatus.Text = "상태: 서버 중지됨";
            }
            catch (Exception ex)
            {
                MessageBox.Show("추론 서버를 중지하지 못했습니다.\n\n오류 내용: " + ex.Message,
                    "추론 서버 중지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task WatchPilotServerExitAsync(Process process)
        {
            try
            {
                await process.WaitForExitAsync();
                AppendPilotServerLog($"[서버] 추론 서버가 종료되었습니다. 종료 코드: {process.ExitCode}");
                void Apply()
                {
                    if (btnStartPilotServer != null) btnStartPilotServer.Enabled = true;
                    if (btnStopPilotServer != null) btnStopPilotServer.Enabled = false;
                    UpdateValidationActionButtons();
                    if (lblPilotStatus != null && process.ExitCode != 0)
                        lblPilotStatus.Text = "상태: 서버 종료";
                }

                if (tabPageModelValidation?.InvokeRequired == true)
                    tabPageModelValidation.BeginInvoke(new Action(Apply));
                else
                    Apply();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private async Task ReadPilotServerStreamAsync(StreamReader reader)
        {
            try
            {
                while (true)
                {
                    string? line = await reader.ReadLineAsync();
                    if (line == null) break;
                    AppendPilotServerLog(line);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
        }

        private void AppendPilotServerLog(string text)
        {
            if (rtbPilotServerLog == null || rtbPilotServerLog.IsDisposed) return;

            string line = NormalizeTrainingLogChunk(text);
            void Append()
            {
                rtbPilotServerLog.AppendText(line + Environment.NewLine);
                rtbPilotServerLog.SelectionStart = rtbPilotServerLog.TextLength;
                rtbPilotServerLog.ScrollToCaret();
            }

            if (rtbPilotServerLog.InvokeRequired)
                rtbPilotServerLog.BeginInvoke(new Action(Append));
            else
                Append();
        }

        private async Task CheckPilotServerHealthAsync()
        {
            await VerifyPilotServerHealthAsync(showMessage: true);
        }

        private async Task<bool> VerifyPilotServerHealthAsync(bool showMessage)
        {
            if (lblPilotStatus != null)
                lblPilotStatus.Text = "상태: 서버 확인 중...";

            try
            {
                using HttpResponseMessage response = await _pilotHttpClient.GetAsync(GetPilotHealthUrl());
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                if (lblPilotStatus != null)
                    lblPilotStatus.Text = "상태: 서버 연결 성공";
                return true;
            }
            catch (Exception ex)
            {
                if (lblPilotStatus != null)
                    lblPilotStatus.Text = "상태: 서버 연결 실패";
                if (showMessage)
                {
                    MessageBox.Show(
                        "Python 추론 서버에 연결할 수 없습니다.\n\n" +
                        "서버가 아직 시작되지 않았거나, 서버 주소/포트가 다를 수 있습니다.\n\n" +
                        "오류 내용: " + ex.Message,
                        "모델 검증 서버 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                return false;
            }
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
                using var fs = File.OpenRead(path);
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

        private void UpdateModelValidationView(FrameData? frame)
        {
            if (tabPageModelValidation == null)
                return;

            if (frame == null)
            {
                if (picValidationPreview != null)
                {
                    picValidationPreview.Image?.Dispose();
                    picValidationPreview.Image = null;
                }

                if (lblPilotStatus != null) lblPilotStatus.Text = "상태: 프레임 없음";
                if (lblPilotActualAngle != null) lblPilotActualAngle.Text = "실제 Angle: -";
                if (lblPilotActualThrottle != null) lblPilotActualThrottle.Text = "실제 Throttle: -";
                if (lblPilotPredictedAngle != null) lblPilotPredictedAngle.Text = "예측 Angle: -";
                if (lblPilotPredictedThrottle != null) lblPilotPredictedThrottle.Text = "예측 Throttle: -";
                pnlValidationBars?.Invalidate();
                return;
            }

            if (lblPilotActualAngle != null)
                lblPilotActualAngle.Text = $"실제 Angle: {frame.Angle:0.000}";
            if (lblPilotActualThrottle != null)
                lblPilotActualThrottle.Text = $"실제 Throttle: {frame.Throttle:0.000}";

            if (frame.IsPilotPredictionLoading)
            {
                if (lblPilotStatus != null) lblPilotStatus.Text = "상태: 추론 요청 중...";
            }
            else if (!string.IsNullOrWhiteSpace(frame.PilotPredictionError))
            {
                if (lblPilotStatus != null) lblPilotStatus.Text = "상태: " + frame.PilotPredictionError;
            }
            else if (frame.PilotAngle.HasValue && frame.PilotThrottle.HasValue)
            {
                if (lblPilotStatus != null) lblPilotStatus.Text = "상태: 추론 완료";
            }
            else
            {
                if (lblPilotStatus != null) lblPilotStatus.Text = "상태: 추론 대기";
            }

            if (lblPilotPredictedAngle != null)
                lblPilotPredictedAngle.Text = frame.PilotAngle.HasValue
                    ? $"예측 Angle: {frame.PilotAngle.Value:0.000}"
                    : "예측 Angle: -";
            if (lblPilotPredictedThrottle != null)
                lblPilotPredictedThrottle.Text = frame.PilotThrottle.HasValue
                    ? $"예측 Throttle: {frame.PilotThrottle.Value:0.000}"
                    : "예측 Throttle: -";

            UpdateValidationPreviewImage(frame);
            picValidationPreview?.Invalidate();
            pnlValidationBars?.Invalidate();
        }

        private void UpdateValidationPreviewImage(FrameData frame)
        {
            if (picValidationPreview == null)
                return;

            string path = ResolveImagePath(frame.Name);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                picValidationPreview.Image?.Dispose();
                picValidationPreview.Image = null;
                return;
            }

            try
            {
                using var fs = File.OpenRead(path);
                using var image = System.Drawing.Image.FromStream(fs);
                var bitmap = new Bitmap(image);
                var old = picValidationPreview.Image;
                picValidationPreview.Image = bitmap;
                old?.Dispose();
            }
            catch
            {
                picValidationPreview.Image?.Dispose();
                picValidationPreview.Image = null;
            }
        }

        private async Task RequestPilotPredictionForFrameAsync(int frameIndex, FrameData frame)
        {
            if (_isPilotPredictionInFlight && _isPlaybackRunning)
                return;

            string imagePath = ResolveImagePath(frame.Name);
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                frame.PilotPredictionError = "이미지 없음";
                frame.IsPilotPredictionLoading = false;
                UpdateModelValidationView(frame);
                return;
            }

            _pilotPredictionCts?.Cancel();
            _pilotPredictionCts?.Dispose();
            _pilotPredictionCts = new CancellationTokenSource();
            CancellationToken cancellationToken = _pilotPredictionCts.Token;
            int requestVersion = Interlocked.Increment(ref _pilotPredictionRequestVersion);

            frame.IsPilotPredictionLoading = true;
            frame.PilotPredictionError = string.Empty;
            UpdateModelValidationView(frame);

            try
            {
                _isPilotPredictionInFlight = true;
                var prediction = await GetPilotPredictionAsync(imagePath, cancellationToken);
                if (cancellationToken.IsCancellationRequested ||
                    requestVersion != _pilotPredictionRequestVersion ||
                    frameIndex != _currentFrameIndex)
                    return;

                frame.PilotAngle = prediction.angle;
                frame.PilotThrottle = prediction.throttle;
                frame.PilotPredictionError = string.Empty;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (requestVersion == _pilotPredictionRequestVersion && frameIndex == _currentFrameIndex)
                    frame.PilotPredictionError = "추론 실패: " + ex.Message;
            }
            finally
            {
                if (requestVersion == _pilotPredictionRequestVersion)
                    _isPilotPredictionInFlight = false;
                if (requestVersion == _pilotPredictionRequestVersion && frameIndex == _currentFrameIndex)
                {
                    frame.IsPilotPredictionLoading = false;
                    UpdateModelValidationView(frame);
                }
            }
        }

        private void ShowPredictionMismatchCandidates()
        {
            if (_allFrames == null || _allFrames.Count == 0)
            {
                MessageBox.Show("먼저 검증할 데이터 폴더를 불러와 주세요.",
                    "예측 차이 후보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var candidates = _allFrames
                .Where(frame => !frame.IsDeleted && frame.HasPilotPredictionMismatch)
                .OrderByDescending(frame => frame.PilotPredictionDifferenceScore)
                .ToList();

            if (candidates.Count == 0)
            {
                int predictedCount = _allFrames.Count(frame => frame.PilotAngle.HasValue && frame.PilotThrottle.HasValue);
                MessageBox.Show(
                    predictedCount == 0
                        ? "아직 예측값이 계산된 프레임이 없습니다.\n서버 시작 후 자동 재생으로 프레임을 훑은 다음 다시 눌러 주세요."
                        : "실제값과 예측값 차이가 큰 후보가 없습니다.",
                    "예측 차이 후보",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            _visibleFrames = candidates;
            _isFrameFilterActive = true;
            RefreshFrameView();
            RenderDataViewerFrameChart();
            RenderFrameChart();
            SetIndex(0);

            MessageBox.Show(
                $"예측 차이 후보 {candidates.Count}개만 표시합니다.\n\n" +
                "기준: 조향 차이 0.35 이상 또는 속도 차이 0.25 이상\n" +
                "후보를 확인한 뒤 필요하면 선택 제외로 학습 데이터에서 제외하세요.",
                "예측 차이 후보",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void MaybeRequestPilotPredictionForCurrentFrame()
        {
            if (tabPageModelValidation == null || tabControlMain.SelectedTab != tabPageModelValidation)
                return;
            if (_visibleFrames == null || _currentFrameIndex < 0 || _currentFrameIndex >= _visibleFrames.Count)
            {
                UpdateModelValidationView(null);
                return;
            }

            FrameData frame = _visibleFrames[_currentFrameIndex];
            UpdateModelValidationView(frame);

            if (frame.PilotAngle.HasValue &&
                frame.PilotThrottle.HasValue &&
                string.IsNullOrWhiteSpace(frame.PilotPredictionError))
                return;

            if (_isPlaybackRunning && _isPilotPredictionInFlight)
                return;

            _ = RequestPilotPredictionForFrameAsync(_currentFrameIndex, frame);
        }

        /// <summary>
        /// 미리보기 이미지 위에 주행 방향 화살표와 상태 텍스트를 선명하게 그립니다.
        /// 사용자가 사진과 조향값이 서로 맞는지 빠르게 비교할 수 있게 하는 보조 표시입니다.
        /// </summary>
        private void DrawDriveOverlay(Bitmap bitmap, FrameData frame)
        {
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            DrawDataViewerDriveArrow(graphics, new Rectangle(0, 0, bitmap.Width, bitmap.Height), frame.Angle);

            string overlayText = $"{frame.SteeringText} / {frame.ThrottleText}";
            if (frame.NeedsReview)
                overlayText += " / 검토";

            float fontSize = Math.Max(4.8f, Math.Min(6.2f, bitmap.Height * 0.055f));
            using var font = new System.Drawing.Font("맑은 고딕", fontSize, System.Drawing.FontStyle.Bold);
            SizeF textSize = graphics.MeasureString(overlayText, font);
            float paddingX = Math.Max(4f, bitmap.Width * 0.025f);
            float paddingY = Math.Max(2.5f, bitmap.Height * 0.018f);
            var textBox = new RectangleF(
                (bitmap.Width - textSize.Width - paddingX * 2f) / 2f,
                Math.Max(4f, bitmap.Height * 0.035f),
                textSize.Width + paddingX * 2f,
                textSize.Height + paddingY * 2f);

            using var boxPath = CreateRoundedRectanglePath(textBox, Math.Max(3f, bitmap.Height * 0.025f));
            using var boxBrush = new SolidBrush(System.Drawing.Color.FromArgb(190, 16, 18, 24));
            using var boxPen = new Pen(System.Drawing.Color.FromArgb(210, 255, 196, 20), Math.Max(0.8f, bitmap.Width * 0.004f));
            using var shadowBrush = new SolidBrush(System.Drawing.Color.FromArgb(105, 0, 0, 0));
            using var textShadowBrush = new SolidBrush(System.Drawing.Color.FromArgb(210, 0, 0, 0));
            using var textBrush = new SolidBrush(frame.NeedsReview ? System.Drawing.Color.FromArgb(255, 255, 225, 80) : System.Drawing.Color.White);

            using (var shadowPath = CreateRoundedRectanglePath(
                       new RectangleF(textBox.X + 2f, textBox.Y + 2f, textBox.Width, textBox.Height),
                       Math.Max(3f, bitmap.Height * 0.025f)))
            {
                graphics.FillPath(shadowBrush, shadowPath);
            }

            graphics.FillPath(boxBrush, boxPath);
            graphics.DrawPath(boxPen, boxPath);

            float textX = textBox.Left + paddingX;
            float textY = textBox.Top + paddingY - 1f;
            graphics.DrawString(overlayText, font, textShadowBrush, textX + 1.2f, textY + 1.2f);
            graphics.DrawString(overlayText, font, textBrush, textX, textY);
        }

        private void DrawDataViewerDriveArrow(Graphics graphics, Rectangle bounds, double angle)
        {
            double clampedAngle = Math.Max(-1.0, Math.Min(1.0, angle));
            float baseX = bounds.Left + bounds.Width / 2f;
            float baseY = bounds.Bottom - Math.Max(14f, bounds.Height * 0.12f);
            float tipX = baseX + (float)(clampedAngle * bounds.Width * 0.22);
            float tipY = baseY - Math.Max(26f, bounds.Height * 0.34f);

            using var arrowCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3.8f, 5.6f, true);
            using var pen = new Pen(System.Drawing.Color.FromArgb(230, 255, 193, 7), Math.Max(1.6f, bounds.Width * 0.014f))
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                CustomEndCap = arrowCap
            };
            using var shadowPen = new Pen(System.Drawing.Color.FromArgb(145, 0, 0, 0), pen.Width + 1.3f)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                CustomEndCap = arrowCap
            };

            graphics.DrawLine(shadowPen, baseX + 1.2f, baseY + 1.2f, tipX + 1.2f, tipY + 1.2f);
            graphics.DrawLine(pen, baseX, baseY, tipX, tipY);
        }

        private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(RectangleF bounds, float radius)
        {
            float diameter = Math.Max(1f, radius * 2f);
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void PicValidationPreview_Paint(object? sender, PaintEventArgs e)
        {
            FrameData? frame = GetCurrentVisibleFrame();
            if (frame == null)
                return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle bounds = picValidationPreview?.ClientRectangle ?? Rectangle.Empty;
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            DrawValidationArrow(
                e.Graphics,
                bounds,
                frame.Angle,
                System.Drawing.Color.FromArgb(230, 255, 193, 7),
                "실제",
                0);

            if (frame.PilotAngle.HasValue)
            {
                DrawValidationArrow(
                    e.Graphics,
                    bounds,
                    frame.PilotAngle.Value,
                    System.Drawing.Color.FromArgb(235, 42, 146, 255),
                    "예측",
                    1);
            }
        }

        private void DrawValidationArrow(Graphics graphics, Rectangle bounds, double angle, System.Drawing.Color color, string label, int layer)
        {
            double clampedAngle = Math.Max(-1.0, Math.Min(1.0, angle));
            float baseX = bounds.Left + bounds.Width / 2f;
            float baseY = bounds.Bottom - 44f - layer * 34f;
            float tipX = baseX + (float)(clampedAngle * bounds.Width * 0.25);
            float tipY = baseY - 62f;

            using var pen = new Pen(color, layer == 0 ? 4f : 5f)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 8)
            };
            using var shadowPen = new Pen(System.Drawing.Color.FromArgb(150, 0, 0, 0), pen.Width + 2)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 8)
            };

            graphics.DrawLine(shadowPen, baseX + 1, baseY + 1, tipX + 1, tipY + 1);
            graphics.DrawLine(pen, baseX, baseY, tipX, tipY);

            string text = $"{label} {angle:0.000}";
            using var font = new Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            SizeF textSize = graphics.MeasureString(text, font);
            var box = new RectangleF(tipX - textSize.Width / 2f - 6, tipY - textSize.Height - 10, textSize.Width + 12, textSize.Height + 6);
            using var backBrush = new SolidBrush(System.Drawing.Color.FromArgb(170, 0, 0, 0));
            using var textBrush = new SolidBrush(System.Drawing.Color.White);
            graphics.FillRectangle(backBrush, box);
            graphics.DrawString(text, font, textBrush, box.Left + 6, box.Top + 3);
        }

        private void PnlValidationBars_Paint(object? sender, PaintEventArgs e)
        {
            FrameData? frame = GetCurrentVisibleFrame();
            e.Graphics.Clear(System.Drawing.Color.FromArgb(26, 29, 34));
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (frame == null)
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    "검증할 프레임이 없습니다.",
                    new Font("맑은 고딕", 10F),
                    pnlValidationBars?.ClientRectangle ?? Rectangle.Empty,
                    System.Drawing.Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            Rectangle bounds = pnlValidationBars?.ClientRectangle ?? Rectangle.Empty;
            int left = 120;
            int right = 28;
            int top = 26;
            int rowHeight = Math.Max(28, (bounds.Height - top - 20) / 4);
            int barWidth = Math.Max(120, bounds.Width - left - right);
            int centerX = left + barWidth / 2;

            using var axisPen = new Pen(System.Drawing.Color.FromArgb(110, 255, 255, 255), 1);
            e.Graphics.DrawLine(axisPen, centerX, 14, centerX, bounds.Bottom - 12);

            DrawValidationBar(e.Graphics, "실제 Angle", frame.Angle, top + rowHeight * 0, left, barWidth, rowHeight, System.Drawing.Color.FromArgb(255, 193, 7));
            DrawValidationBar(e.Graphics, "예측 Angle", frame.PilotAngle, top + rowHeight * 1, left, barWidth, rowHeight, System.Drawing.Color.FromArgb(42, 146, 255));
            DrawValidationBar(e.Graphics, "실제 Throttle", frame.Throttle, top + rowHeight * 2, left, barWidth, rowHeight, System.Drawing.Color.FromArgb(255, 193, 7));
            DrawValidationBar(e.Graphics, "예측 Throttle", frame.PilotThrottle, top + rowHeight * 3, left, barWidth, rowHeight, System.Drawing.Color.FromArgb(42, 146, 255));
        }

        private void DrawValidationBar(Graphics graphics, string label, double? value, int y, int left, int width, int height, System.Drawing.Color color)
        {
            using var labelFont = new Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            using var valueFont = new Font("맑은 고딕", 8.5F);
            using var labelBrush = new SolidBrush(System.Drawing.Color.White);
            using var backBrush = new SolidBrush(System.Drawing.Color.FromArgb(48, 52, 60));
            using var barBrush = new SolidBrush(System.Drawing.Color.FromArgb(215, color));
            using var emptyBrush = new SolidBrush(System.Drawing.Color.FromArgb(150, 160, 160, 160));

            graphics.DrawString(label, labelFont, labelBrush, 12, y + 5);

            var track = new Rectangle(left, y + 7, width, Math.Max(10, height - 14));
            graphics.FillRectangle(backBrush, track);

            if (!value.HasValue)
            {
                graphics.DrawString("-", valueFont, emptyBrush, left + width + 4, y + 5);
                return;
            }

            double clamped = Math.Max(-1.0, Math.Min(1.0, value.Value));
            int center = left + width / 2;
            int valueX = center + (int)Math.Round(clamped * width / 2.0);
            int barLeft = Math.Min(center, valueX);
            int barRight = Math.Max(center, valueX);
            graphics.FillRectangle(barBrush, barLeft, track.Top, Math.Max(2, barRight - barLeft), track.Height);
            graphics.DrawString(value.Value.ToString("0.000", CultureInfo.InvariantCulture), valueFont, labelBrush, left + width + 4, y + 5);
        }

        private FrameData? GetCurrentVisibleFrame()
        {
            if (_visibleFrames == null || _currentFrameIndex < 0 || _currentFrameIndex >= _visibleFrames.Count)
                return null;

            return _visibleFrames[_currentFrameIndex];
        }

        private int GetPlaybackIntervalFromSpeed()
        {
            decimal speed = 1.0M;
            if (_cmbPlaybackSpeed?.SelectedItem is string selectedSpeed &&
                int.TryParse(selectedSpeed.Replace("배", string.Empty), out int comboSpeed))
            {
                speed = Math.Clamp(comboSpeed, 1, 10);
            }
            else
            {
                speed = Math.Max(0.25M, numPlaybackIntervalMs.Value);
            }

            const int baseIntervalMs = 200;
            return Math.Max(20, (int)Math.Round(baseIntervalMs / speed));
        }

        private void TogglePlayPause()
        {
            if (_isPlaybackRunning)
            {
                _playbackTimer.Stop();
                _isPlaybackRunning = false;
                SetPlaybackButtonText("자동 재생");
            }
            else
            {
                _playbackTimer.Interval = GetPlaybackIntervalFromSpeed();
                _playbackTimer.Start();
                _isPlaybackRunning = true;
                SetPlaybackButtonText("일시정지");
            }
        }

        private void SetPlaybackButtonText(string text)
        {
            btnAutoPlay.Text = text;
            if (btnValidationAutoPlay != null)
                btnValidationAutoPlay.Text = text;
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
            if (!ValidateScenarioAngleRange(angleMin, angleMax))
                return;

            // 제외되지 않은 프레임 중 범위 조건에 맞는 항목만 남깁니다.
            _visibleFrames = _allFrames
                .Where(f => !f.IsDeleted &&
                            f.Angle >= angleMin && f.Angle <= angleMax &&
                            f.Throttle >= throttleMin && f.Throttle <= throttleMax &&
                            MatchesComboFilter(cmbModeFilter, f.Mode) &&
                            MatchesScenarioFilter(f))
                .ToList();

            _isFrameFilterActive = true;
            _isChartDirty = true;
            _timelineViewStart = 0;
            RefreshFrameView();
            if (_visibleFrames.Count > 0) SetIndex(0);
            else ClearPreviewSelection();
        }

        private bool TryReadFilterRanges(out double angleMin, out double angleMax,
                                          out double throttleMin, out double throttleMax)
        {
            angleMin = angleMax = throttleMin = throttleMax = 0;

            bool ok =
                double.TryParse(txtAngleMinFilter.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out angleMin) &&
                double.TryParse(txtAngleMaxFilter.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out angleMax) &&
                double.TryParse(txtThrottleMinFilter.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMin) &&
                double.TryParse(txtThrottleMaxFilter.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMax);

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

        private TurnDirection GetSelectedTurnDirection()
        {
            string selected = cmbScenarioFilter.SelectedItem?.ToString() ?? cmbScenarioFilter.Text;
            string normalized = NormalizeFilterText(selected);

            if (normalized.Contains("right") || normalized.Contains("우"))
                return TurnDirection.Right;
            if (normalized.Contains("left") || normalized.Contains("좌"))
                return TurnDirection.Left;

            return TurnDirection.None;
        }

        private bool ValidateScenarioAngleRange(double angleMin, double angleMax)
        {
            const double epsilon = 0.000001;
            TurnDirection direction = GetSelectedTurnDirection();

            if (direction == TurnDirection.Right &&
                (angleMin < 0 - epsilon || angleMax > 1 + epsilon))
            {
                MessageBox.Show(
                    "현재 선택된 상황은 우회전입니다.\n방향값 범위를 0 ~ 1 사이의 값으로 설정해 주세요.",
                    "상황/방향값 범위 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (direction == TurnDirection.Left &&
                (angleMin < -1 - epsilon || angleMax > 0 + epsilon))
            {
                MessageBox.Show(
                    "현재 선택된 상황은 좌회전입니다.\n방향값 범위를 -1 ~ 0 사이의 값으로 설정해 주세요.",
                    "상황/방향값 범위 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private bool MatchesScenarioFilter(FrameData frame)
        {
            string selected = cmbScenarioFilter.SelectedItem?.ToString() ?? cmbScenarioFilter.Text;
            if (string.IsNullOrWhiteSpace(selected) ||
                selected.Equals("All", StringComparison.OrdinalIgnoreCase))
                return true;

            return GetSelectedTurnDirection() switch
            {
                TurnDirection.Right => frame.Angle >= 0,
                TurnDirection.Left => frame.Angle <= 0,
                _ => string.Equals(
                    NormalizeFilterText(frame.Scenario),
                    NormalizeFilterText(selected),
                    StringComparison.OrdinalIgnoreCase)
            };
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
            _timelineViewStart = 0;
            RefreshFrameView();
            if (_visibleFrames.Count > 0) SetIndex(0);
            else ClearPreviewSelection();
        }

        private HashSet<int> GetSelectedVisibleIndices()
        {
            var selected = new HashSet<int>();

            if (_dragPreviewSelectionStartIndex >= 0 && _dragPreviewSelectionEndIndex >= 0)
            {
                int from = Math.Min(_dragPreviewSelectionStartIndex, _dragPreviewSelectionEndIndex);
                int to = Math.Max(_dragPreviewSelectionStartIndex, _dragPreviewSelectionEndIndex);
                for (int index = from; index <= to && index < (_visibleFrames?.Count ?? 0); index++)
                    selected.Add(index);
            }

            if (_visibleFrames == null || _visibleFrames.Count == 0 || dgvFrameCatalog.Rows.Count == 0)
                return selected;

            foreach (DataGridViewRow row in dgvFrameCatalog.SelectedRows)
            {
                if (row.DataBoundItem is not FrameData frame) continue;

                int index = _visibleFrames.IndexOf(frame);
                if (index >= 0)
                    selected.Add(index);
            }

            return selected;
        }

        private List<(int Start, int End)> GetSelectedVisibleRanges()
        {
            var indices = GetSelectedVisibleIndices().OrderBy(i => i).ToList();
            var ranges = new List<(int Start, int End)>();
            if (indices.Count == 0)
                return ranges;

            int start = indices[0];
            int end = indices[0];

            for (int i = 1; i < indices.Count; i++)
            {
                if (indices[i] == end + 1)
                {
                    end = indices[i];
                    continue;
                }

                ranges.Add((start, end));
                start = end = indices[i];
            }

            ranges.Add((start, end));
            return ranges;
        }

        private void RefreshSelectionVisuals(bool renderChart = true, bool updateFilmstripNow = false)
        {
            pnlFrameThumbnailStrip.Invalidate();
            if (updateFilmstripNow)
                pnlFrameThumbnailStrip.Update();
            if (renderChart)
                RenderDataViewerFrameChart();
        }

        // Soft Delete 구간 처리
        private void SoftDeleteRange(int from, int to)
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
            _pilotPredictionCts?.Cancel();
            UpdateModelValidationView(null);
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

                SyncDatasetPathsForTrainingAndValidation(cleanFolder, updateTrainingTubPath: true);

                var useForTraining = MessageBox.Show(
                    "방금 만든 Clean 폴더를 학습 데이터 폴더로 사용할까요?\n\n" +
                    "이미 학습 실행/추론 검증 경로는 Clean 폴더 기준으로 자동 변경되었습니다.\n" +
                    "예를 누르면 학습 실행 탭으로 이동합니다.",
                    "학습 경로 연결",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (useForTraining == DialogResult.Yes)
                {
                    tabControlMain.SelectedTab = tabTrainingMonitor;
                    AppendTrainingLog("[안내] Clean 폴더가 학습 데이터 경로와 추론/검증 데이터 경로로 설정되었습니다.");
                    AppendTrainingLog("[안내] 모델 저장 경로: " + txtTrainingModelPath.Text);
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
            int total = _allFrames?.Count ?? 0;
            int deleted = _allFrames?.Count(f => f.IsDeleted) ?? 0;
            int review = _allFrames?.Count(f => !f.IsDeleted && f.NeedsReview) ?? 0;
            int valid = Math.Max(0, total - deleted);
            int shown = _visibleFrames?.Count ?? 0;
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
            _pilotPredictionCts?.Cancel();
            StopPilotServer(showMessage: false);

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
            public string ImagePath { get; set; } = string.Empty;
            public double Angle { get; set; }
            public double Throttle { get; set; }
            public double? PilotAngle { get; set; }
            public double? PilotThrottle { get; set; }
            public bool IsPilotPredictionLoading { get; set; }
            public string PilotPredictionError { get; set; } = string.Empty;
            public string Mode { get; set; } = "-";
            public string Scenario { get; set; } = "-";
            public string Name { get; set; } = string.Empty;
            public int OriginalIndex { get; set; } = -1;
            public int ImageNumber { get; set; } = -1;

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

            /// <summary>
            /// 실제 조향값과 모델 예측 조향값의 차이입니다.
            /// 예측값이 아직 없으면 0으로 처리해 일반 검토 후보에 섞이지 않게 합니다.
            /// </summary>
            public double PilotAngleDifference => PilotAngle.HasValue ? Math.Abs(Angle - PilotAngle.Value) : 0.0;

            /// <summary>
            /// 실제 속도값과 모델 예측 속도값의 차이입니다.
            /// 예측값이 아직 없으면 0으로 처리합니다.
            /// </summary>
            public double PilotThrottleDifference => PilotThrottle.HasValue ? Math.Abs(Throttle - PilotThrottle.Value) : 0.0;

            /// <summary>
            /// 예측 차이 후보를 큰 순서로 정렬하기 위한 간단한 점수입니다.
            /// 값이 클수록 실제 데이터와 모델 예측이 더 다릅니다.
            /// </summary>
            public double PilotPredictionDifferenceScore => PilotAngleDifference + PilotThrottleDifference;

            /// <summary>
            /// 모델 예측값이 실제 조작값과 많이 다른 프레임입니다.
            /// 조향 차이는 0.35 이상, 속도 차이는 0.25 이상이면 발표 데모에서 검토할 후보로 봅니다.
            /// </summary>
            public bool HasPilotPredictionMismatch =>
                PilotAngle.HasValue &&
                PilotThrottle.HasValue &&
                (PilotAngleDifference >= 0.35 || PilotThrottleDifference >= 0.25);

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
                    if (HasPilotPredictionMismatch)
                        reasons.Add($"예측 차이(방향 {PilotAngleDifference:0.000}, 속도 {PilotThrottleDifference:0.000})");

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

        private int ExtractImageNumber(string imageName)
        {
            string fileName = Path.GetFileName(imageName) ?? string.Empty;
            Match match = Regex.Match(fileName, @"^\d+");
            return match.Success && int.TryParse(match.Value, out int number)
                ? number
                : -1;
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
                ImageNumber = ExtractImageNumber(imageName),
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
                        ImageNumber = ExtractImageNumber(Path.GetFileName(imagePath)),
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
                string normalizedFolder = Path.GetFullPath(folder);
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
                _timelineViewStart = 0;
                _previousThumbnailHighlightIndex = -1;
                // 초기 로드: 전체 프레임을 표시합니다.
                RefreshFrameBinding();  // _visibleFrames 설정과 RefreshFrameView 호출

                _currentDataFolderPath = normalizedFolder;
                SyncDatasetPathsForTrainingAndValidation(normalizedFolder, updateTrainingTubPath: true);

                stsDataPath.Text = "경로: " + normalizedFolder;
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
            lblEpoch.Text = "학습 횟수";

            cmbTrainingModelType.Items.Clear();
            cmbTrainingModelType.Items.AddRange(new object[] { "linear", "inferred", "tensorrt_linear", "tflite_linear", "categorical", "rnn", "3d", "imu", "behavior" });
            if (string.IsNullOrWhiteSpace(cmbTrainingModelType.Text))
                cmbTrainingModelType.Text = "linear";

            if (string.IsNullOrWhiteSpace(txtTrainingTubPath.Text))
                txtTrainingTubPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "data");

            if (string.IsNullOrWhiteSpace(txtTrainingModelPath.Text))
                txtTrainingModelPath.Text = GetDefaultTrainingModelPath();

            numTrainingEpochs.Minimum = 1;
            numTrainingEpochs.Maximum = 10000;
            if (numTrainingEpochs.Value < 1)
                numTrainingEpochs.Value = 10;

            btnStopTrainingProcess.Enabled = false;
            stsTrainingStatus.Text = "학습 상태: 대기";

            ConfigureTrainingSummaryLabels();
            ConfigureTrainingPostActionControls();
            ApplyTrainingControlStyle();
            LayoutTrainingTab();
            grpTrainingConfig.Resize += (_, _) => LayoutTrainingTab();
            tabTrainingMonitor.Resize += (_, _) => LayoutTrainingTab();
        }

        private string GetDefaultTrainingModelPath()
        {
            string modelFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "donkeycar_models");
            Directory.CreateDirectory(modelFolder);
            return Path.Combine(modelFolder, "pilot.keras");
        }

        private string GetTrainingModelPathForDataset(string datasetFolder)
        {
            string datasetName = GetSafeDatasetFolderName(datasetFolder);
            string modelFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "donkeycar_models",
                datasetName);

            Directory.CreateDirectory(modelFolder);
            return Path.Combine(modelFolder, "pilot.keras");
        }

        private string GetSafeDatasetFolderName(string datasetFolder)
        {
            string name = Path.GetFileName(datasetFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrWhiteSpace(name))
                name = "donkeycar_data";

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                name = name.Replace(invalidChar, '_');

            return name;
        }

        private void SyncDatasetPathsForTrainingAndValidation(string datasetFolder, bool updateTrainingTubPath)
        {
            if (string.IsNullOrWhiteSpace(datasetFolder) || !Directory.Exists(datasetFolder))
                return;

            string normalizedFolder = Path.GetFullPath(datasetFolder);
            string modelPath = GetTrainingModelPathForDataset(normalizedFolder);

            if (updateTrainingTubPath)
                txtTrainingTubPath.Text = normalizedFolder;

            txtTrainingModelPath.Text = modelPath;

            if (txtValidationDataFolderPath != null)
                txtValidationDataFolderPath.Text = normalizedFolder;

            if (txtValidationModelPath != null)
                txtValidationModelPath.Text = modelPath;

            _isValidationEnvironmentReady = false;
            UpdateValidationActionButtons();
        }

        private void NormalizeTrainingPathsForDisplay()
        {
            txtTrainingTubPath.Text = NormalizeWindowsTrainingPath(
                txtTrainingTubPath.Text,
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

            if (Directory.Exists(txtTrainingTubPath.Text))
            {
                txtTrainingModelPath.Text = GetTrainingModelPathForDataset(txtTrainingTubPath.Text);
            }
            else
            {
            txtTrainingModelPath.Text = NormalizeWindowsTrainingPath(
                txtTrainingModelPath.Text,
                Path.GetDirectoryName(GetDefaultTrainingModelPath()) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }

            if (txtValidationDataFolderPath != null)
                txtValidationDataFolderPath.Text = txtTrainingTubPath.Text;
            if (txtValidationModelPath != null)
                txtValidationModelPath.Text = txtTrainingModelPath.Text;
        }

        private string NormalizeWindowsTrainingPath(string path, string baseFolder)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            string trimmed = path.Trim();
            if (trimmed.StartsWith("~/", StringComparison.Ordinal) || trimmed.StartsWith("/", StringComparison.Ordinal))
                return trimmed;

            if (Path.IsPathRooted(trimmed))
                return Path.GetFullPath(trimmed);

            Directory.CreateDirectory(baseFolder);
            return Path.GetFullPath(Path.Combine(baseFolder, Path.GetFileName(trimmed)));
        }

        private void ConfigureTrainingSummaryLabels()
        {
            _lblTrainingSummaryTitle ??= CreateTrainingSummaryLabel("lblTrainingSummaryTitle", "학습 요약", true);
            _lblTrainingSummaryStatus ??= CreateTrainingSummaryLabel("lblTrainingSummaryStatus", "상태: 대기", false);
            _lblTrainingSummaryEpoch ??= CreateTrainingSummaryLabel("lblTrainingSummaryEpoch", "학습 횟수: -", false);
            _lblTrainingSummaryProgress ??= CreateTrainingSummaryLabel("lblTrainingSummaryProgress", "진행률: -", false);
            _lblTrainingSummaryLoss ??= CreateTrainingSummaryLabel("lblTrainingSummaryLoss", "점수(높을수록 좋음): -", false);
        }

        private void ConfigureTrainingPostActionControls()
        {
            if (_btnOpenTrainedModelValidation == null)
            {
                _btnOpenTrainedModelValidation = new Button
                {
                    Name = "btnOpenTrainedModelValidation",
                    Text = "이 모델로 추론/검증",
                    Enabled = false,
                    UseVisualStyleBackColor = true
                };
                _btnOpenTrainedModelValidation.Click += async (_, _) => await OpenTrainedModelValidationAsync(startServer: true);
                grpTrainingConfig.Controls.Add(_btnOpenTrainedModelValidation);
            }

            if (_chkRestartPilotServerAfterTraining == null)
            {
                _chkRestartPilotServerAfterTraining = new CheckBox
                {
                    Name = "chkRestartPilotServerAfterTraining",
                    Text = "학습 완료 후 추론 서버 자동 재시작",
                    AutoSize = false,
                    Checked = false
                };
                grpTrainingConfig.Controls.Add(_chkRestartPilotServerAfterTraining);
            }
        }

        private System.Windows.Forms.Label CreateTrainingSummaryLabel(string name, string text, bool bold)
        {
            var label = new System.Windows.Forms.Label
            {
                Name = name,
                AutoSize = false,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("resource/PretendardVariable.ttf", 10.5F, bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular),
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
            System.Drawing.Font commonFont = new System.Drawing.Font("resource/PretendardVariable.ttf", 10.5F, System.Drawing.FontStyle.Regular);
            System.Drawing.Font labelFont = new System.Drawing.Font("resource/PretendardVariable.ttf", 10.5F, System.Drawing.FontStyle.Regular);
            System.Drawing.Font buttonFont = new System.Drawing.Font("resource/PretendardVariable.ttf", 10.5F, System.Drawing.FontStyle.Regular);

            foreach (Control control in grpTrainingConfig.Controls)
            {
                control.Font = control is Button ? buttonFont : commonFont;

                if (control is System.Windows.Forms.Label label)
                {
                    label.Font = labelFont;
                    label.AutoEllipsis = true;
                }
            }

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

            if (_btnOpenTrainedModelValidation != null)
            {
                _btnOpenTrainedModelValidation.Location = new Point(buttonX - actionWidth - 24, summaryY + buttonHeight + 10);
                _btnOpenTrainedModelValidation.Size = new Size(actionWidth + 24, buttonHeight);
                _btnOpenTrainedModelValidation.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }

            if (_chkRestartPilotServerAfterTraining != null)
            {
                _chkRestartPilotServerAfterTraining.Location = new Point(inputX, summaryY + buttonHeight + 12);
                _chkRestartPilotServerAfterTraining.Size = new Size(Math.Max(320, inputWidth / 2), buttonHeight);
                _chkRestartPilotServerAfterTraining.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            grpTrainingOutput.Location = new Point(margin, grpTrainingConfig.Bottom + 16);
            grpTrainingOutput.Size = new Size(groupWidth, Math.Max(50, tabTrainingMonitor.ClientSize.Height - grpTrainingOutput.Top - 42));
            grpTrainingOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            rtbTrainingOutput.Dock = DockStyle.Fill;
            rtbTrainingOutput.BackColor = System.Drawing.Color.Black;
            rtbTrainingOutput.ForeColor = System.Drawing.Color.White;
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

            if (!HasTrainingEnvironmentInputs())
            {
                bool detected = await DetectTrainingEnvironmentAsync(showSuccessMessage: false, clearLog: false);
                if (!detected)
                {
                    MessageBox.Show(
                        "학습 환경 자동 감지에 실패했습니다.\n\n" +
                        "자동 감지 버튼을 눌러 로그를 확인하거나, WSL Ubuntu / Conda 경로 / Donkey 프로젝트 경로를 직접 선택해 주세요.",
                        "학습 환경 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            NormalizeTrainingPathsForDisplay();
            if (!PrepareTrainingModelOutputPath()) return;
            if (!TryBuildTrainingCommand(out var arguments, out string displayArguments)) return;

            btnStartTrainingProcess.Enabled = false;
            btnStopTrainingProcess.Enabled = true;
            stsTrainingStatus.Text = "학습 상태: 실행 중";
            UpdateTrainingSummary(status: "실행 중", epoch: "-", progress: "-", loss: "-");
            rtbTrainingOutput.Clear();
            ResetTrainingLossGraphData();
            ShowTrainingLossGraph();

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
                AppendTrainingLog("첫 학습 횟수는 데이터 캐싱과 모델 준비 때문에 오래 걸릴 수 있습니다.");
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
                if (_trainingProcess.ExitCode == 0)
                {
                    AppendTrainingCompletionExplanation();

                    if (_btnOpenTrainedModelValidation != null)
                        _btnOpenTrainedModelValidation.Enabled = true;

                    await PrepareValidationAfterSuccessfulTrainingAsync();
                }
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

        /// <summary>
        /// DonkeyCar는 검증 loss가 더 좋아지지 않으면 MAX_EPOCHS 전에 정상 종료할 수 있습니다.
        /// 사용자가 "왜 100회 전에 끝났지?"라고 혼동하지 않도록 학습 완료 직후 로그로 설명합니다.
        /// </summary>
        private void AppendTrainingCompletionExplanation()
        {
            int requestedEpochs = Math.Max(1, (int)numTrainingEpochs.Value);
            if (!TryGetCompletedEpochInfo(out int completedEpochs, out int totalEpochs))
                return;

            int configuredTotal = Math.Max(requestedEpochs, totalEpochs);
            if (completedEpochs >= configuredTotal)
                return;

            AppendTrainingLog(
                $"[안내] 학습이 {completedEpochs}/{configuredTotal}회에서 정상 완료되었습니다. " +
                "DonkeyCar 설정의 조기 종료(Early Stopping)가 켜져 있으면 검증 loss 개선이 멈출 때 최대 횟수 전에 자동 종료됩니다.");
            AppendTrainingLog(
                "[안내] 현재 기본 설정은 USE_EARLY_STOP=True, EARLY_STOP_PATIENCE=5, MIN_DELTA=0.0005 기준일 수 있습니다. " +
                "더 오래 학습하려면 mycar 설정에서 조기 종료를 끄거나 patience 값을 늘리면 됩니다.");
        }

        private bool TryGetCompletedEpochInfo(out int completedEpochs, out int totalEpochs)
        {
            completedEpochs = 0;
            totalEpochs = 0;

            string text = _lblTrainingSummaryEpoch?.Text ?? string.Empty;
            Match match = Regex.Match(text, @"(\d+)\s*/\s*(\d+)");
            if (!match.Success)
                return false;

            return int.TryParse(match.Groups[1].Value, out completedEpochs) &&
                   int.TryParse(match.Groups[2].Value, out totalEpochs) &&
                   completedEpochs > 0 &&
                   totalEpochs > 0;
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
            int epochCount = Math.Max(1, (int)numTrainingEpochs.Value);

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
            string wslTrainingConfigPath = "/tmp/teamapp_train_config.py";

            string command =
                "export PYTHONUNBUFFERED=1 && " +
                "if [ ! -d " + QuotePathForBash(wslMycarPath) + " ]; then echo " +
                QuoteForBash("[error] Donkey 프로젝트 경로를 찾을 수 없습니다: " + wslMycarPath) + "; exit 2; fi && " +
                "if [ ! -x " + QuotePathForBash(wslCondaPath) + " ]; then echo " +
                QuoteForBash("[error] Conda 실행 파일을 찾을 수 없습니다: " + wslCondaPath) + "; exit 127; fi && " +
                "cd " + QuotePathForBash(wslMycarPath) + " && " +
                "if [ ! -f config.py ] && [ ! -f manage.py ]; then echo " +
                QuoteForBash("[error] Donkey 프로젝트 파일(config.py 또는 manage.py)을 찾을 수 없습니다: " + wslMycarPath) + "; exit 2; fi && " +
                "if [ -f config.py ]; then cp config.py " + QuotePathForBash(wslTrainingConfigPath) + "; else : > " + QuotePathForBash(wslTrainingConfigPath) + "; fi && " +
                "printf " + QuoteForBash("\n# TeamApp runtime training settings\nMAX_EPOCHS = " + epochCount.ToString(CultureInfo.InvariantCulture) + "\n") +
                " >> " + QuotePathForBash(wslTrainingConfigPath) + " && " +
                QuotePathForBash(wslCondaPath) + " run --no-capture-output -n " + QuoteForBash(envName) + " " +
                "donkey train " +
                "--tub " + QuotePathForBash(wslTubPath) + " " +
                "--model " + QuotePathForBash(wslModelPath) + " " +
                "--type " + QuoteForBash(modelType) + " " +
                "--config " + QuotePathForBash(wslTrainingConfigPath);

            arguments.AddRange(new[] { "-d", wslDistro, "bash", "-lc", command });
            displayArguments = "-d " + QuoteProcessArgument(wslDistro) + " bash -lc " + QuoteForBash(command);
            return true;
        }

        private bool HasTrainingEnvironmentInputs()
        {
            return !string.IsNullOrWhiteSpace(NormalizeWslDistroName(cmbTrainingWslDistro.Text)) &&
                   !string.IsNullOrWhiteSpace(cmbCondaPath.Text.Trim()) &&
                   !string.IsNullOrWhiteSpace(txtTrainingPythonEnvName.Text.Trim()) &&
                   !string.IsNullOrWhiteSpace(cmbMycarProjectPath.Text.Trim());
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

        private void ShowTrainingLossGraph()
        {
            if (_trainingLossGraphForm == null || _trainingLossGraphForm.IsDisposed)
            {
                _trainingLossGraphForm = new TrainingLossGraphForm();
                _trainingLossGraphForm.Show(this);
            }
            else
            {
                _trainingLossGraphForm.Show();
                _trainingLossGraphForm.BringToFront();
            }

            UpdateTrainingLossGraph();
        }

        private void ResetTrainingLossGraphData()
        {
            _trainingLossEpochs.Clear();
            _trainingLossValues.Clear();
            _validationLossEpochs.Clear();
            _validationLossValues.Clear();
            _currentTrainingEpochForGraph = 0;
            _currentTrainingStepForGraph = 0;
            _currentTrainingTotalStepsForGraph = 0;
            UpdateTrainingLossGraph();
        }

        private void UpdateTrainingLossGraph()
        {
            if (_trainingLossGraphForm == null || _trainingLossGraphForm.IsDisposed)
                return;

            _trainingLossGraphForm.UpdateLossData(
                _trainingLossEpochs.ToArray(),
                _trainingLossValues.ToArray(),
                _validationLossEpochs.ToArray(),
                _validationLossValues.ToArray());
        }

        private void AppendTrainingOutputChunk(string chunk)
        {
            if (rtbTrainingOutput.IsDisposed || string.IsNullOrEmpty(chunk)) return;

            string sanitized = NormalizeTrainingLogChunk(chunk);
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

        private string NormalizeTrainingLogChunk(string chunk)
        {
            string sanitized = AnsiEscapeRegex.Replace(chunk, string.Empty).Replace("\0", string.Empty);

            // DonkeyCar may log this as ERROR even when the training process exits with code 0.
            // It means the optional model database file was not written, not that the model training failed.
            return sanitized.Replace(
                "ERROR:donkeycar.pipeline.database:Failed writing database file:",
                "[참고] 모델 기록 DB 저장 실패(학습 모델 저장과는 별개):",
                StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyTrainingOutputChunk(string text)
        {
            foreach (char ch in text)
            {
                if (char.IsControl(ch) && ch != '\r' && ch != '\n' && ch != '\t')
                    continue;

                if (ch == '\b')
                {
                    if (_trainingOutputLineBuffer.Length > 0)
                        _trainingOutputLineBuffer.Length--;
                    continue;
                }

                if (ch == '\r' || ch == '\n')
                {
                    AppendBufferedTrainingLine();
                    _trainingOutputLineBuffer.Clear();
                    continue;
                }

                _trainingOutputLineBuffer.Append(ch);
            }

            // 진행률 줄은 CR/LF 단위로 정리해서 출력합니다.
            // 미완성 버퍼를 매 글자마다 파싱하면 같은 loss가 그래프에 중복 기록될 수 있습니다.
        }

        private void AppendBufferedTrainingLine()
        {
            string line = _trainingOutputLineBuffer.ToString().Trim();
            if (string.IsNullOrWhiteSpace(line))
                return;

            UpdateTrainingSummaryFromText(line);
            if (TryFormatKerasProgressLine(line, out string progressLine))
            {
                rtbTrainingOutput.AppendText(progressLine + Environment.NewLine + Environment.NewLine);
                return;
            }

            rtbTrainingOutput.AppendText(line + Environment.NewLine);
        }

        private bool TryFormatKerasProgressLine(string line, out string progressLine)
        {
            progressLine = string.Empty;
            Match match = Regex.Match(line, @"(?<current>\d+)\s*/\s*(?<total>\d+).*?(?:^|\s)loss:\s*(?<loss>[0-9.]+)", RegexOptions.IgnoreCase);
            if (!match.Success ||
                !int.TryParse(match.Groups["current"].Value, out int currentStep) ||
                !int.TryParse(match.Groups["total"].Value, out int totalStep) ||
                totalStep <= 0)
                return false;

            double percent = Math.Max(0.0, Math.Min(100.0, currentStep * 100.0 / totalStep));
            progressLine = $"[ {BuildTrainingProgressBar(percent)} | {percent:0.00}% ]";
            return true;
        }

        private string BuildTrainingProgressBar(double percent)
        {
            const int blockCount = 20;
            int filled = Math.Max(0, Math.Min(blockCount, (int)Math.Round(percent / 100.0 * blockCount)));
            string blocks = new string('▮', filled) + new string('▯', blockCount - filled);

            // 너무 길게 붙어 보이지 않도록 5칸 단위로 끊어 표시합니다.
            return string.Join(" ", Enumerable.Range(0, (int)Math.Ceiling(blocks.Length / 5.0))
                .Select(groupIndex => blocks.Substring(groupIndex * 5, Math.Min(5, blocks.Length - groupIndex * 5))));
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

            if (text.Contains("[error]", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("train: error", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Traceback", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Exception", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainingSummary(status: "오류");
            }

            Match epochMatch = Regex.Match(text, @"Epoch\s+(\d+)\s*/\s*(\d+)", RegexOptions.IgnoreCase);
            if (epochMatch.Success)
            {
                if (int.TryParse(epochMatch.Groups[1].Value, out int epochNumber))
                {
                    _currentTrainingEpochForGraph = epochNumber;
                    _currentTrainingStepForGraph = 0;
                    _currentTrainingTotalStepsForGraph = 0;
                }

                UpdateTrainingSummary(
                    status: "학습 중",
                    epoch: epochMatch.Groups[1].Value + "/" + epochMatch.Groups[2].Value,
                    progress: "0%");
            }

            Match progressMatch = Regex.Match(text, @"(?<current>\d+)\s*/\s*(?<total>\d+)", RegexOptions.IgnoreCase);
            if (progressMatch.Success &&
                int.TryParse(progressMatch.Groups["current"].Value, out int currentStep) &&
                int.TryParse(progressMatch.Groups["total"].Value, out int totalStep) &&
                totalStep > 0)
            {
                int percent = Math.Max(0, Math.Min(100, currentStep * 100 / totalStep));
                _currentTrainingStepForGraph = currentStep;
                _currentTrainingTotalStepsForGraph = totalStep;
                UpdateTrainingSummary(progress: percent + "%");
            }

            Match valLossMatch = Regex.Match(text, @"(?:^|\s)val_loss:\s*(?<loss>[0-9.]+)", RegexOptions.IgnoreCase);
            if (valLossMatch.Success)
            {
                AddTrainingLossPoint(valLossMatch.Groups["loss"].Value, isValidation: true);
                UpdateTrainingSummary(loss: FormatTrainingScore(valLossMatch.Groups["loss"].Value));
            }

            Match lossMatch = Regex.Match(text, @"(?:^|\s)loss:\s*(?<loss>[0-9.]+)", RegexOptions.IgnoreCase);
            if (lossMatch.Success)
            {
                AddTrainingLossPoint(lossMatch.Groups["loss"].Value, isValidation: false);
                UpdateTrainingSummary(loss: FormatTrainingScore(lossMatch.Groups["loss"].Value));
            }
        }

        private bool PrepareTrainingModelOutputPath()
        {
            try
            {
                string modelPath = txtTrainingModelPath.Text.Trim();
                if (string.IsNullOrWhiteSpace(modelPath))
                {
                    MessageBox.Show("모델 저장 경로를 확인할 수 없습니다.",
                        "모델 저장 경로", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string? modelFolder = Path.GetDirectoryName(modelPath);
                if (string.IsNullOrWhiteSpace(modelFolder))
                {
                    MessageBox.Show("모델 저장 폴더를 확인할 수 없습니다.\n\n" + modelPath,
                        "모델 저장 경로", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (IsManagedDonkeyModelFolder(modelFolder))
                {
                    // 앱이 자동으로 만든 "문서/donkeycar_models/데이터폴더명" 폴더는 같은 이름으로 다시 학습할 때 덮어씁니다.
                    if (Directory.Exists(modelFolder) && Directory.EnumerateFileSystemEntries(modelFolder).Any())
                    {
                        DialogResult backupAnswer = MessageBox.Show(
                            "같은 데이터 폴더명으로 저장된 기존 모델 폴더가 있습니다.\n\n" +
                            "기존 모델을 백업한 뒤 새 모델로 덮어쓸까요?\n\n" +
                            "예: 백업 후 덮어쓰기\n" +
                            "아니요: 백업 없이 덮어쓰기\n" +
                            "취소: 학습 시작 취소",
                            "모델 폴더 덮어쓰기",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question);

                        if (backupAnswer == DialogResult.Cancel)
                            return false;

                        if (backupAnswer == DialogResult.Yes)
                        {
                            string backupFolder = CreateModelFolderBackup(modelFolder);
                            AppendTrainingLog("[안내] 기존 모델 폴더를 백업했습니다: " + backupFolder);
                        }
                    }

                    if (Directory.Exists(modelFolder))
                        Directory.Delete(modelFolder, recursive: true);
                    Directory.CreateDirectory(modelFolder);
                }
                else
                {
                    Directory.CreateDirectory(modelFolder);
                    if (File.Exists(modelPath))
                        File.Delete(modelPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("모델 저장 경로를 준비하지 못했습니다.\n\n" + ex.Message,
                    "모델 저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool IsManagedDonkeyModelFolder(string modelFolder)
        {
            try
            {
                string root = Path.GetFullPath(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "donkeycar_models"));
                string folder = Path.GetFullPath(modelFolder);
                return folder.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private string CreateModelFolderBackup(string modelFolder)
        {
            string parentFolder = Directory.GetParent(modelFolder)?.FullName
                ?? Path.GetDirectoryName(modelFolder)
                ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string modelFolderName = Path.GetFileName(modelFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string backupFolder = Path.Combine(parentFolder, modelFolderName + "_backup_" + timestamp);

            CopyDirectory(modelFolder, backupFolder);
            return backupFolder;
        }

        private void SyncValidationPathsAfterTraining()
        {
            string tubPath = txtTrainingTubPath.Text.Trim();
            string modelPath = txtTrainingModelPath.Text.Trim();

            if (txtValidationDataFolderPath != null && Directory.Exists(tubPath))
                txtValidationDataFolderPath.Text = tubPath;
            if (txtValidationModelPath != null && !string.IsNullOrWhiteSpace(modelPath))
                txtValidationModelPath.Text = modelPath;

            _isValidationEnvironmentReady = false;
            UpdateValidationActionButtons();
            AppendTrainingLog("[안내] 학습 완료 모델이 추론/검증 탭의 모델 파일 경로로 연결되었습니다.");
        }

        private async Task OpenTrainedModelValidationAsync(bool startServer)
        {
            SyncValidationPathsAfterTraining();
            if (tabPageModelValidation != null)
                tabControlMain.SelectedTab = tabPageModelValidation;

            if (!File.Exists(txtTrainingModelPath.Text.Trim()))
            {
                MessageBox.Show(
                    "학습 완료 모델 파일을 찾을 수 없습니다.\n\n" + txtTrainingModelPath.Text.Trim(),
                    "추론/검증 시작", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool detected = await DetectValidationEnvironmentAsync(showMessage: false);
            if (!detected)
            {
                MessageBox.Show(
                    "추론/검증 환경 자동 감지에 실패했습니다.\n추론/검증 탭의 서버 로그를 확인해 주세요.",
                    "추론/검증 시작", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!startServer)
                return;

            if (_pilotServerProcess is { HasExited: false })
            {
                StopPilotServer(showMessage: false);
                await Task.Delay(600);
            }

            await StartPilotServerAsync(showErrorMessage: true);
        }

        /// <summary>
        /// 학습 성공 직후 추론/검증 탭에서 바로 검토할 수 있도록 모델 경로, 데이터 경로, 예측 후보를 준비합니다.
        /// 학습 완료 후 사용자가 곧바로 발표 데모를 할 수 있게 만드는 후처리 흐름입니다.
        /// </summary>
        private async Task PrepareValidationAfterSuccessfulTrainingAsync()
        {
            SyncValidationPathsAfterTraining();

            bool shouldRestartServer = _chkRestartPilotServerAfterTraining?.Checked == true;
            if (shouldRestartServer && _pilotServerProcess is { HasExited: false })
            {
                StopPilotServer(showMessage: false);
                await Task.Delay(600);
            }

            _isValidationEnvironmentReady = HasTrainingEnvironmentInputs() &&
                                            ValidationDataFolderExists() &&
                                            ValidationModelFileExists();
            UpdateValidationActionButtons();
            if (!_isValidationEnvironmentReady)
                return;

            string tubPath = txtTrainingTubPath.Text.Trim();
            if (Directory.Exists(tubPath) &&
                !IsSameFullPath(tubPath, _currentDataFolderPath))
            {
                await LoadCatalogAsync(tubPath);
            }

            if (!await EnsurePilotServerReadyAsync(showErrorMessage: false))
                return;

            await PrecomputePilotPredictionCandidatesAsync();
        }

        private static bool IsSameFullPath(string pathA, string? pathB)
        {
            if (string.IsNullOrWhiteSpace(pathA) || string.IsNullOrWhiteSpace(pathB))
                return false;

            try
            {
                return string.Equals(
                    Path.GetFullPath(pathA),
                    Path.GetFullPath(pathB),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 전체 프레임 예측을 하기 전에 추론 서버가 살아 있는지 확인하고, 없으면 시작합니다.
        /// StartPilotServerAsync는 사용자 메시지를 직접 처리하므로 여기서는 성공 여부만 다시 확인합니다.
        /// </summary>
        private async Task<bool> EnsurePilotServerReadyAsync(bool showErrorMessage)
        {
            if (_pilotServerProcess is { HasExited: false } && await VerifyPilotServerHealthAsync(showMessage: false))
                return true;

            await StartPilotServerAsync(showErrorMessage);
            return _pilotServerProcess is { HasExited: false } && await VerifyPilotServerHealthAsync(showMessage: false);
        }

        /// <summary>
        /// 학습 완료 모델로 전체 프레임을 순회하며 예측값을 미리 계산합니다.
        /// 이 결과를 바탕으로 '예측 차이 후보' 버튼이 즉시 후보를 보여줄 수 있습니다.
        /// </summary>
        private async Task PrecomputePilotPredictionCandidatesAsync()
        {
            if (_isPrecomputingPilotPredictions)
                return;

            if (_allFrames == null || _allFrames.Count == 0)
                return;

            _isPrecomputingPilotPredictions = true;
            _pilotBatchPredictionCts?.Cancel();
            _pilotBatchPredictionCts?.Dispose();
            _pilotBatchPredictionCts = new CancellationTokenSource();
            CancellationToken cancellationToken = _pilotBatchPredictionCts.Token;

            var frames = _allFrames.Where(frame => !frame.IsDeleted).ToList();
            int successCount = 0;
            int failCount = 0;

            try
            {
                AppendPilotServerLog($"[검증] 전체 프레임 예측 후보 계산을 시작합니다. 대상: {frames.Count}개");

                foreach (FrameData frame in frames)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string imagePath = ResolveImagePath(frame.Name);
                    if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                    {
                        frame.PilotAngle = null;
                        frame.PilotThrottle = null;
                        frame.PilotPredictionError = "이미지 없음";
                        failCount++;
                        continue;
                    }

                    try
                    {
                        var prediction = await GetPilotPredictionAsync(imagePath, cancellationToken);
                        frame.PilotAngle = prediction.angle;
                        frame.PilotThrottle = prediction.throttle;
                        frame.PilotPredictionError = string.Empty;
                        successCount++;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        frame.PilotAngle = null;
                        frame.PilotThrottle = null;
                        frame.PilotPredictionError = "추론 실패: " + ex.Message;
                        failCount++;
                    }

                    int processed = successCount + failCount;
                    if (processed % 100 == 0 || processed == frames.Count)
                    {
                        int percent = frames.Count == 0 ? 100 : (int)Math.Round(processed * 100.0 / frames.Count);
                        AppendPilotServerLog($"[검증] 예측 후보 계산 중... {processed}/{frames.Count} ({percent}%)");
                        await Task.Yield();
                    }
                }

                int mismatchCount = frames.Count(frame => frame.HasPilotPredictionMismatch);
                AppendPilotServerLog($"[검증] 예측 후보 계산 완료: 후보 {mismatchCount}개 / 성공 {successCount}개 / 실패 {failCount}개");

                if (_visibleFrames != null && _currentFrameIndex >= 0 && _currentFrameIndex < _visibleFrames.Count)
                    UpdateModelValidationView(_visibleFrames[_currentFrameIndex]);
                RenderDataViewerFrameChart();
                RenderFrameChart();
            }
            catch (OperationCanceledException)
            {
                AppendPilotServerLog("[검증] 예측 후보 계산이 취소되었습니다.");
            }
            finally
            {
                _isPrecomputingPilotPredictions = false;
            }
        }

        private void AddTrainingLossPoint(string lossText, bool isValidation)
        {
            if (!double.TryParse(lossText, NumberStyles.Float, CultureInfo.InvariantCulture, out double loss))
                return;

            double epochX = Math.Max(1, _currentTrainingEpochForGraph);
            if (!isValidation)
            {
                // 학습 중 진행률 그래프가 계단식으로만 갱신되지 않도록 step 위치를 epoch 내부 소수점으로 표시합니다.
                double stepRatio = _currentTrainingTotalStepsForGraph > 0
                    ? Math.Clamp(_currentTrainingStepForGraph / (double)_currentTrainingTotalStepsForGraph, 0.0, 1.0)
                    : 0.0;
                epochX = Math.Max(0.0, epochX - 1.0) + stepRatio;
                _trainingLossEpochs.Add(epochX);
                _trainingLossValues.Add(loss);
            }
            else
            {
                _validationLossEpochs.Add(epochX);
                _validationLossValues.Add(loss);
            }

            UpdateTrainingLossGraph();
        }

        private string FormatTrainingScore(string lossText)
        {
            if (!double.TryParse(lossText, NumberStyles.Float, CultureInfo.InvariantCulture, out double loss))
                return "-";

            double score = Math.Clamp((1.0 - loss) * 100.0, 0.0, 100.0);
            return $"{score:0}점";
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
                    _lblTrainingSummaryEpoch.Text = "학습 횟수: " + epoch;

                if (_lblTrainingSummaryProgress != null && progress != null)
                    _lblTrainingSummaryProgress.Text = "진행률: " + progress;

                if (_lblTrainingSummaryLoss != null && loss != null)
                    _lblTrainingSummaryLoss.Text = "점수(높을수록 좋음): " + loss;
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
                Font = new Font("resource/PretendardVariable.ttf", 10F)
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
            {
                txtTrainingModelPath.Text = dialog.FileName;
                if (txtValidationModelPath != null)
                {
                    txtValidationModelPath.Text = dialog.FileName;
                    _isValidationEnvironmentReady = false;
                    UpdateValidationActionButtons();
                }
            }
        }

        private void SelectTrainingTubFolder()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Tub 폴더 선택",
                SelectedPath = Directory.Exists(txtTrainingTubPath.Text)
                    ? txtTrainingTubPath.Text
                    : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            SyncDatasetPathsForTrainingAndValidation(dialog.SelectedPath, updateTrainingTubPath: true);
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
                    ValidationTubPath = txtValidationDataFolderPath?.Text.Trim() ?? string.Empty,
                    ValidationModelPath = txtValidationModelPath?.Text.Trim() ?? string.Empty,
                    ValidationApiUrl = txtPilotApiUrl?.Text.Trim() ?? string.Empty,
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
                if (txtValidationDataFolderPath != null)
                {
                    txtValidationDataFolderPath.Text = !string.IsNullOrWhiteSpace(settings.ValidationTubPath)
                        ? settings.ValidationTubPath
                        : settings.TubPath;
                }
                if (txtValidationModelPath != null)
                {
                    txtValidationModelPath.Text = !string.IsNullOrWhiteSpace(settings.ValidationModelPath)
                        ? settings.ValidationModelPath
                        : settings.ModelPath;
                }
                if (txtPilotApiUrl != null && !string.IsNullOrWhiteSpace(settings.ValidationApiUrl))
                    txtPilotApiUrl.Text = settings.ValidationApiUrl;
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
            public string ValidationTubPath { get; set; } = "";
            public string ValidationModelPath { get; set; } = "";
            public string ValidationApiUrl { get; set; } = "http://127.0.0.1:5000/predict";
            public int Epochs { get; set; } = 10;
        }

        private sealed class TrainingLossGraphForm : Form
        {
            private readonly FormsPlot _lossChart;

            public TrainingLossGraphForm()
            {
                Text = "학습 결과 그래프";
                StartPosition = FormStartPosition.CenterParent;
                Size = new Size(760, 520);
                MinimumSize = new Size(520, 360);

                _lossChart = new FormsPlot
                {
                    Dock = DockStyle.Fill
                };
                Controls.Add(_lossChart);
                DrawEmptyChart();
            }

            public void UpdateLossData(double[] trainXs, double[] trainLosses, double[] validationXs, double[] validationLosses)
            {
                if (IsDisposed) return;

                void Apply()
                {
                    var plot = _lossChart.Plot;
                    plot.Clear();
                    ConfigureLossPlotStyle(plot);

                    if (trainXs.Length > 0 && trainLosses.Length > 0)
                    {
                        int count = Math.Min(trainXs.Length, trainLosses.Length);
                        var train = plot.Add.Scatter(trainXs.Take(count).ToArray(), trainLosses.Take(count).ToArray());
                        train.LegendText = "train";
                        train.Color = ScottPlot.Color.FromHex("#1f77b4");
                        train.LineWidth = 2;
                        train.MarkerSize = 3;
                    }

                    if (validationXs.Length > 0 && validationLosses.Length > 0)
                    {
                        int count = Math.Min(validationXs.Length, validationLosses.Length);
                        var validation = plot.Add.Scatter(validationXs.Take(count).ToArray(), validationLosses.Take(count).ToArray());
                        validation.LegendText = "validate";
                        validation.Color = ScottPlot.Color.FromHex("#ff7f0e");
                        validation.LineWidth = 2;
                        validation.MarkerSize = 5;
                    }

                    if (trainXs.Length == 0 && validationXs.Length == 0)
                        plot.Title("model loss");

                    plot.Axes.AutoScale();
                    _lossChart.Refresh();
                }

                if (InvokeRequired)
                    BeginInvoke(new Action(Apply));
                else
                    Apply();
            }

            private void DrawEmptyChart()
            {
                var plot = _lossChart.Plot;
                plot.Clear();
                ConfigureLossPlotStyle(plot);
                plot.Title("model loss");
                _lossChart.Refresh();
            }

            private static void ConfigureLossPlotStyle(Plot plot)
            {
                plot.Title("model loss");
                plot.XLabel("epoch");
                plot.YLabel("loss");
                plot.Axes.Title.Label.FontName = "Malgun Gothic";
                plot.Axes.Bottom.Label.FontName = "Malgun Gothic";
                plot.Axes.Left.Label.FontName = "Malgun Gothic";
                plot.Legend.IsVisible = true;
                plot.Legend.Alignment = Alignment.UpperRight;
            }
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

            if (tabPageModelValidation != null && tabControlMain.SelectedTab == tabPageModelValidation)
            {
                UpdateValidationActionButtons();
                MaybeRequestPilotPredictionForCurrentFrame();
            }

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
                lblChartDescription.Font = new Font("resource/PretendardVariable.ttf", 10F);
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
            _frameChart.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#2d2d30");

            pnlChartHost.Controls.Clear();
            pnlChartHost.Controls.Add(_frameChart);
        }

        private void InitDataViewerFrameChart()
        {
            if (_dataViewerFrameChart != null) return;

            pnlDataViewerChartHost.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlDataViewerChartHost.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);

            _dataViewerFrameChart = new FormsPlot
            {
                Dock = DockStyle.Fill,
                Name = "formsPlotDataViewer"
            };
            _dataViewerFrameChart.UserInputProcessor.Disable();
            _dataViewerFrameChart.MouseDown += DataViewerFrameChart_MouseDown;
            _dataViewerFrameChart.MouseWheel += DataViewerFrameChart_MouseWheel;
            _dataViewerFrameChart.MouseEnter += (_, _) => _dataViewerFrameChart.Focus();

            _dataViewerFrameChart.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1e1e1e");
            _dataViewerFrameChart.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#2d2d30");

            _timelineScrollBar = new HScrollBar
            {
                Dock = DockStyle.Bottom,
                Name = "hsbTimeline",
                Height = 18,
                SmallChange = 5,
                LargeChange = TimelineVisibleFrameWindow
            };
            _timelineScrollBar.Scroll += TimelineScrollBar_Scroll;

            pnlDataViewerChartHost.Controls.Clear();
            pnlDataViewerChartHost.Controls.Add(_dataViewerFrameChart);
            pnlDataViewerChartHost.Controls.Add(_timelineScrollBar);
        }

        private void DataViewerFrameChart_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _dataViewerFrameChart == null || _allFrames.Count == 0)
                return;

            // ScottPlot 5에서는 GetCoordinateX(e.X) 대신 Pixel 기반 GetCoordinates()를 사용합니다.
            Coordinates coordinates = _dataViewerFrameChart.Plot.GetCoordinates(
                new Pixel(e.X, e.Y),
                _dataViewerFrameChart.Plot.Axes.Bottom,
                _dataViewerFrameChart.Plot.Axes.Left);

            int targetIndex = GetNearestVisibleFrameIndexByImageNumber(coordinates.X);
            ApplyFrameSelection(targetIndex, ModifierKeys);
        }

        private int GetNearestVisibleFrameIndexByImageNumber(double imageNumber)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return 0;

            int bestIndex = 0;
            double bestDistance = double.MaxValue;
            for (int i = 0; i < _visibleFrames.Count; i++)
            {
                double distance = Math.Abs(GetFrameXAxisValue(_visibleFrames[i], i) - imageNumber);
                if (distance >= bestDistance) continue;

                bestDistance = distance;
                bestIndex = i;
            }

            return bestIndex;
        }

        private void DataViewerFrameChart_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (_dataViewerFrameChart == null || GetTimelineFrameCount() == 0)
                return;

            int wheelSteps = Math.Max(-6, Math.Min(6, e.Delta / 120));
            if (wheelSteps == 0)
                wheelSteps = e.Delta > 0 ? 1 : -1;

            int panStep = Math.Max(1, TimelineVisibleFrameWindow / 10);
            SetTimelineViewStart(_timelineViewStart - wheelSteps * panStep, syncScrollBar: true, refresh: true);
        }

        private int GetTimelineFrameCount()
        {
            if (_visibleFrames != null && _visibleFrames.Count > 0)
                return _visibleFrames.Count;

            return _allFrames?.Count ?? 0;
        }

        private int GetMaxTimelineViewStart()
        {
            int count = GetTimelineFrameCount();
            return Math.Max(0, count - TimelineVisibleFrameWindow);
        }

        private void ConfigureTimelineScrollBar()
        {
            if (_timelineScrollBar == null) return;

            int count = GetTimelineFrameCount();
            int maxStart = GetMaxTimelineViewStart();

            _isSyncingTimelineScrollBar = true;
            try
            {
                _timelineScrollBar.Enabled = count > TimelineVisibleFrameWindow;
                _timelineScrollBar.Minimum = 0;
                _timelineScrollBar.LargeChange = Math.Max(1, Math.Min(TimelineVisibleFrameWindow, Math.Max(1, count)));
                _timelineScrollBar.SmallChange = Math.Max(1, TimelineVisibleFrameWindow / 10);
                _timelineScrollBar.Maximum = Math.Max(0, count - 1);
                _timelineScrollBar.Value = Math.Clamp(_timelineViewStart, 0, maxStart);
            }
            finally
            {
                _isSyncingTimelineScrollBar = false;
            }
        }

        private void TimelineScrollBar_Scroll(object? sender, ScrollEventArgs e)
        {
            if (_isSyncingTimelineScrollBar) return;
            SetTimelineViewStart(e.NewValue, syncScrollBar: false, refresh: true);
        }

        private void SetTimelineViewStart(int start, bool syncScrollBar, bool refresh)
        {
            _timelineViewStart = Math.Clamp(start, 0, GetMaxTimelineViewStart());

            if (syncScrollBar && _timelineScrollBar != null)
            {
                _isSyncingTimelineScrollBar = true;
                try
                {
                    int scrollbarMaxValue = Math.Max(_timelineScrollBar.Minimum, _timelineScrollBar.Maximum - _timelineScrollBar.LargeChange + 1);
                    _timelineScrollBar.Value = Math.Clamp(_timelineViewStart, _timelineScrollBar.Minimum, scrollbarMaxValue);
                }
                finally
                {
                    _isSyncingTimelineScrollBar = false;
                }
            }

            if (refresh)
                ApplyDataViewerTimelineAxisLimits(refresh: true);
        }

        private void ApplyDataViewerTimelineAxisLimits(bool refresh)
        {
            if (_dataViewerFrameChart == null) return;

            int count = GetTimelineFrameCount();
            int start = Math.Clamp(_timelineViewStart, 0, GetMaxTimelineViewStart());
            int endIndex = count == 0
                ? 0
                : Math.Min(count - 1, count <= TimelineVisibleFrameWindow
                    ? count - 1
                    : start + TimelineVisibleFrameWindow);

            double startX = GetTimelineXForVisibleIndex(start);
            double endX = GetTimelineXForVisibleIndex(endIndex);
            if (endX <= startX)
                endX = startX + Math.Max(1, TimelineVisibleFrameWindow);

            _dataViewerFrameChart.Plot.Axes.SetLimits(startX, endX, -1.3, 1.3);
            SyncTimelineScrollBarToView();

            if (refresh)
                _dataViewerFrameChart.Refresh();
        }

        private double GetTimelineXForVisibleIndex(int index)
        {
            if (_visibleFrames == null || _visibleFrames.Count == 0)
                return index;

            index = Math.Clamp(index, 0, _visibleFrames.Count - 1);
            return GetFrameXAxisValue(_visibleFrames[index], index);
        }

        private void SyncTimelineScrollBarToView()
        {
            if (_timelineScrollBar == null) return;

            _isSyncingTimelineScrollBar = true;
            try
            {
                int scrollbarMaxValue = Math.Max(_timelineScrollBar.Minimum, _timelineScrollBar.Maximum - _timelineScrollBar.LargeChange + 1);
                _timelineScrollBar.Value = Math.Clamp(_timelineViewStart, _timelineScrollBar.Minimum, scrollbarMaxValue);
            }
            finally
            {
                _isSyncingTimelineScrollBar = false;
            }
        }

        private void UpdateDataViewerTimelinePlayhead(bool autoScroll)
        {
            if (_dataViewerFrameChart == null || _dataViewerPlayheadLine == null || _currentFrameIndex < 0)
                return;

            if (autoScroll && _currentFrameIndex > _timelineViewStart + TimelineVisibleFrameWindow / 2)
                SetTimelineViewStart(_currentFrameIndex - TimelineVisibleFrameWindow / 2, syncScrollBar: true, refresh: false);
            else if (_currentFrameIndex < _timelineViewStart)
                SetTimelineViewStart(_currentFrameIndex, syncScrollBar: true, refresh: false);

            _dataViewerPlayheadLine.X = GetTimelineXForVisibleIndex(_currentFrameIndex);
            ApplyDataViewerTimelineAxisLimits(refresh: true);
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
            RenderFrameChartTo(_frameChart!, compact: false);
            _isChartDirty = false;
        }

        private void RenderDataViewerFrameChart()
        {
            if (_allFrames == null || _allFrames.Count == 0 || pnlDataViewerChartHost == null)
                return;

            if (_dataViewerFrameChart == null) InitDataViewerFrameChart();
            ConfigureTimelineScrollBar();
            RenderFrameChartTo(_dataViewerFrameChart!, compact: true);
        }

        private double GetFrameXAxisValue(FrameData frame, int fallbackIndex)
        {
            if (frame.ImageNumber >= 0)
                return frame.ImageNumber;

            if (frame.OriginalIndex >= 0)
                return frame.OriginalIndex;

            return fallbackIndex;
        }

        private void AddDeletedFrameSpans(Plot plot, List<FrameData> chartFrames, double[] xs)
        {
            int start = -1;
            int end = -1;

            for (int i = 0; i < chartFrames.Count; i++)
            {
                if (chartFrames[i].IsDeleted)
                {
                    int xIndex = (int)Math.Round(xs[i]);
                    if (start < 0)
                        start = end = xIndex;
                    else
                        end = xIndex;
                    continue;
                }

                AddPendingDeletedSpan();
            }

            AddPendingDeletedSpan();

            void AddPendingDeletedSpan()
            {
                if (start < 0) return;

                var span = plot.Add.HorizontalSpan(start, end,
                    ScottPlot.Color.FromSDColor(System.Drawing.Color.FromArgb(50, System.Drawing.Color.Red)));
                span.LegendText = "Excluded";
                span.LineColor = ScottPlot.Color.FromSDColor(System.Drawing.Color.FromArgb(120, System.Drawing.Color.Red));
                span.LineWidth = 1;
                span.EnableAutoscale = false;

                start = end = -1;
            }
        }

        private void RenderFrameChartTo(FormsPlot chart, bool compact)
        {
            var plot = chart.Plot;

            // 한글 폰트를 지정해 차트 라벨이 깨지지 않도록 합니다.
            chart.Plot.Axes.Title.Label.FontName = "Malgun Gothic";
            chart.Plot.Axes.Bottom.Label.FontName = "Malgun Gothic";
            chart.Plot.Axes.Left.Label.FontName = "Malgun Gothic";
            chart.Plot.Axes.Bottom.TickLabelStyle.FontName = "Malgun Gothic";
            chart.Plot.Axes.Left.TickLabelStyle.FontName = "Malgun Gothic";
            plot.Legend.FontName = "Malgun Gothic";
            plot.Legend.FontSize = compact ? 9 : 13;
            plot.Legend.FontColor = ScottPlot.Color.FromHex("#222222");
            plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#F7F7F7");
            plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#777777");

            plot.Clear();
            if (ReferenceEquals(chart, _dataViewerFrameChart))
                _dataViewerPlayheadLine = null;

            var chartFrames = compact
                ? (_visibleFrames != null && _visibleFrames.Count > 0 ? _visibleFrames.ToList() : _allFrames.ToList())
                : _isFrameFilterActive
                    ? _visibleFrames.ToList()
                    : _allFrames.ToList();

            if (chartFrames.Count == 0)
            {
                plot.Title("No valid frames");
                chart.Refresh();
                return;
            }

            int n = chartFrames.Count;
            double[] xs = chartFrames
                .Select((frame, index) => GetFrameXAxisValue(frame, index))
                .ToArray();
            double[] angleYs = chartFrames.Select(f => !_showDeletedOnGraph && f.IsDeleted ? double.NaN : f.Angle).ToArray();
            double[] throttleYs = chartFrames.Select(f => !_showDeletedOnGraph && f.IsDeleted ? double.NaN : f.Throttle).ToArray();

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

            if (compact)
            {
                if (_showDeletedOnGraph)
                    AddDeletedFrameSpans(plot, chartFrames, xs);

                foreach (var (start, end) in _isPlaybackRunning
                    ? Enumerable.Empty<(int Start, int End)>()
                    : GetSelectedVisibleRanges())
                {
                    double startX = GetFrameXAxisValue(chartFrames[Math.Clamp(start, 0, chartFrames.Count - 1)], start);
                    double endX = GetFrameXAxisValue(chartFrames[Math.Clamp(end, 0, chartFrames.Count - 1)], end);
                    var selectedSpan = plot.Add.HorizontalSpan(Math.Min(startX, endX), Math.Max(startX, endX),
                        ScottPlot.Color.FromHex("#4A90E2").WithOpacity(0.24));
                    selectedSpan.LegendText = "Selected";
                    selectedSpan.LineColor = ScottPlot.Color.FromHex("#4A90E2").WithOpacity(0.75);
                    selectedSpan.LineWidth = 1.2f;
                    selectedSpan.EnableAutoscale = false;
                }
            }

            // 축 라벨과 제목
            plot.XLabel("Image number");
            plot.YLabel(compact ? "" : "Value (-1 ~ 1)");
            plot.Title(compact
                ? $"Steering / Speed   shown {n} / total {_allFrames.Count}"
                : _isFrameFilterActive
                    ? $"Filtered distribution [shown: {n} / total: {_allFrames.Count}]"
                    : $"Steering/Speed flow [train: {n} / total: {_allFrames.Count} / excluded: {_allFrames.Count(f => f.IsDeleted)}]");
            if (compact)
            {
                _dataViewerPlayheadLine = plot.Add.VerticalLine(GetTimelineXForVisibleIndex(_currentFrameIndex), 2.5f, ScottPlot.Color.FromHex("#FF2D2D"));
                _dataViewerPlayheadLine.LegendText = "Playhead";
                _dataViewerPlayheadLine.IsDraggable = false;
                ApplyDataViewerTimelineAxisLimits(refresh: false);
            }
            else
            {
                plot.Axes.SetLimitsY(-1.2, 1.2);
            }
            plot.ShowLegend(Alignment.UpperRight);
            plot.Axes.Color(ScottPlot.Color.FromHex("#DDE3EA"));

            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#444444");

            chart.Refresh();
        }

        private void dgvFrameCatalog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
