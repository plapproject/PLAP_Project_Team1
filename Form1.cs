using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        // ── 데이터 목록 ────────────────────────────────────────────────────────────
        // _originalFrames : 로드된 모든 프레임 (삭제된 것 포함, 순서 불변)
        // _currentDisplayedFrames : 현재 ListBox에 표시 중인 목록
        //   - 필터 해제 시: _originalFrames 그대로
        //   - 필터 적용 시: IsDeleted == false 인 것만
        private List<FrameData> _originalFrames = new List<FrameData>();
        private List<FrameData> _currentDisplayedFrames = new List<FrameData>();

        private int currentIndex = -1;
        private System.Windows.Forms.Timer playTimer;
        private bool isPlaying = false;
        private bool isDarkTheme = false;

        // ── 현재 열린 폴더 경로 ───────────────────────────────────────────────────
        private string _currentFolderPath = "";

        // ── ScottPlot 차트 ────────────────────────────────────────────────────────
        private FormsPlot? _formsPlot;
        private bool _chartDirty = true;

        // ══════════════════════════════════════════════════════════════════════════
        // 생성자 / 초기화
        // ══════════════════════════════════════════════════════════════════════════

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            playTimer = new System.Windows.Forms.Timer();
            playTimer.Tick += PlayTimer_Tick;

            if (statusStripDataViewer != null && !this.Controls.Contains(statusStripDataViewer))
            {
                statusStripDataViewer.Dock = DockStyle.Bottom;
                this.Controls.Add(statusStripDataViewer);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ── 버튼 이벤트 동적 연결 ─────────────────────────────────────────────
            btnClearFilter.Click      += BtnClearFilter_Click;
            btnExcludeSelectedFrame.Click += BtnRepair_Click;
            btnRestoreFrame.Click     += BtnReloadTub_Click;

            mnuOpenDataFolder.Click   += (s, _) => btnOpenFolder_Click(s!, EventArgs.Empty);
            mnuReloadData.Click       += (s, _) => btnReload_Click(s!, EventArgs.Empty);
            mnuExit.Click             += (s, _) => Application.Exit();
            mnuOpenGuide.Click        += (s, _) => btnGuide_Click(s!, EventArgs.Empty);

            tabControlMain.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

            btnExcludeSelectedFrame.Text = "구간 제외";
            btnRestoreFrame.Text         = "복원";
            txtAngleMin.Text    = "-1";
            txtAngleMax.Text    = "1";
            txtThrottleMin.Text = "0";
            txtThrottleMax.Text = "1";

            // ── [3] ListBox OwnerDraw 설정 ────────────────────────────────────────
            lstFrameData.DrawMode = DrawMode.OwnerDrawFixed;
            lstFrameData.DrawItem += lstFrameData_DrawItem;

            UpdateStatusLabels();
        }

        // ══════════════════════════════════════════════════════════════════════════
        // UI 이벤트 핸들러
        // ══════════════════════════════════════════════════════════════════════════

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            _ = LoadCatalogAsync(dlg.SelectedPath);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            string path = _currentFolderPath;
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(toolStripStatusLabelPath.Text) ||
                    toolStripStatusLabelPath.Text == "경로: -") return;
                path = toolStripStatusLabelPath.Text.Replace("경로: ", "").Trim();
            }
            if (Directory.Exists(path)) _ = LoadCatalogAsync(path);
        }

        private void btnToggleTheme_Click(object sender, EventArgs e) => ToggleTheme();

        private void btnGuide_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Data Manager 사용 순서\n\n" +
                "1. 데이터 뷰어 탭에서 data 폴더를 엽니다.\n" +
                "2. 이미지, Angle, Throttle 값을 확인합니다.\n" +
                "3. 필터로 학습 데이터 품질을 점검합니다.\n" +
                "4. 제외할 프레임을 선택 후 '구간 제외'를 실행합니다.\n" +
                "5. '복원'으로 언제든 삭제 표시를 되돌릴 수 있습니다.\n" +
                "6. 그래프/통계 탭에서 Angle·Throttle 분포를 확인합니다.",
                "단계별 가이드", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnApplyFilter_Click(object sender, EventArgs e) => ApplyFilter();

        private void BtnClearFilter_Click(object sender, EventArgs e) => ClearFilter();

        // ── 구간 제외 (Soft Delete) ───────────────────────────────────────────────
        private void BtnRepair_Click(object? sender, EventArgs e)
        {
            // 현재 ListBox에서 선택된 항목들의 첫/끝 originalFrames 인덱스를 구함
            if (lstFrameData.SelectedItems.Count == 0)
            {
                MessageBox.Show("제외할 프레임을 하나 이상 선택해 주세요.",
                    "선택 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 선택된 항목들 중 _originalFrames 내 인덱스를 수집
            var selectedOriginalIndices = new List<int>();
            foreach (var item in lstFrameData.SelectedItems)
            {
                if (item is FrameData fd)
                {
                    int idx = _originalFrames.IndexOf(fd);
                    if (idx >= 0) selectedOriginalIndices.Add(idx);
                }
            }

            if (selectedOriginalIndices.Count == 0) return;

            int from = selectedOriginalIndices.Min();
            int to   = selectedOriginalIndices.Max();

            var confirm = MessageBox.Show(
                $"원본 인덱스 {from} ~ {to} 구간을 제외(Soft Delete) 처리합니다.\n" +
                "실제 파일은 삭제되지 않으며, '복원'으로 되돌릴 수 있습니다.\n\n계속하시겠습니까?",
                "구간 제외 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            SoftDeleteRange(from, to);
        }

        // ── 전체 복원 ─────────────────────────────────────────────────────────────
        private void BtnReloadTub_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "모든 Soft Delete 플래그를 초기화하고 전체 데이터를 복원합니다.\n계속하시겠습니까?",
                "전체 복원", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            RestoreAll();
        }

        private void btnAutoPlay_Click(object sender, EventArgs e) => TogglePlayPause();

        private void lstFrameData_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstFrameData.SelectedItem == null) return;
            int idx = lstFrameData.SelectedIndex;
            if (idx >= 0 && idx < _currentDisplayedFrames.Count)
                SetIndex(idx);
        }

        private void trkFramePosition_Scroll(object sender, EventArgs e)
        {
            int idx = trkFramePosition.Value;
            if (idx >= 0 && idx < _currentDisplayedFrames.Count)
                SetIndex(idx);
        }

        private void btnFirst_Click(object sender, EventArgs e) => SetIndex(0);
        private void btnPrev_Click(object sender, EventArgs e)  => SetIndex(Math.Max(0, currentIndex - 1));
        private void btnNext_Click(object sender, EventArgs e)  => SetIndex(Math.Min(_currentDisplayedFrames.Count - 1, currentIndex + 1));
        private void btnLast_Click(object sender, EventArgs e)  => SetIndex(_currentDisplayedFrames.Count - 1);

        // ── 재생 타이머 ───────────────────────────────────────────────────────────
        private void PlayTimer_Tick(object? sender, EventArgs e)
        {
            if (_currentDisplayedFrames == null || _currentDisplayedFrames.Count == 0) return;
            int next = currentIndex + 1;
            // 삭제된 프레임은 건너뜀
            while (next < _currentDisplayedFrames.Count && _currentDisplayedFrames[next].IsDeleted)
                next++;
            if (next >= _currentDisplayedFrames.Count)
            {
                playTimer.Stop();
                isPlaying = false;
                btnAutoPlay.Text = "자동 재생";
                return;
            }
            SetIndex(next);
        }

        // ── 키보드 단축키 ─────────────────────────────────────────────────────────
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

        // ══════════════════════════════════════════════════════════════════════════
        // [3] ListBox OwnerDraw 핸들러
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// 항목별 색상을 직접 그립니다.
        /// • IsDeleted == true  → 굵은 빨간 글씨 (선택 시 연핑크 배경으로 가시성 확보)
        /// • IsDeleted == false → 기본 색
        /// </summary>
        private void lstFrameData_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _currentDisplayedFrames.Count) return;

            e.DrawBackground();

            var frame = _currentDisplayedFrames[e.Index];
            bool isSelected = (e.State & DrawItemState.Selected) != 0;
            bool isDeleted  = frame.IsDeleted;

            // ── 배경색 ────────────────────────────────────────────────────────
            System.Drawing.Color backColor;
            if (isDeleted)
                backColor = isSelected
                    ? System.Drawing.Color.FromArgb(255, 180, 180)   // 연핑크 (선택)
                    : System.Drawing.Color.FromArgb(255, 235, 235);  // 더 연한 분홍 (일반)
            else
                backColor = isSelected ? SystemColors.Highlight : e.BackColor;

            using (var backBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(backBrush, e.Bounds);

            // ── 글꼴 & 글자색 ─────────────────────────────────────────────────
            Font  font;
            System.Drawing.Color foreColor;

            if (isDeleted)
            {
                font      = new Font(e.Font ?? lstFrameData.Font, System.Drawing.FontStyle.Bold);
                foreColor = isSelected ? System.Drawing.Color.DarkRed : System.Drawing.Color.Red;
            }
            else
            {
                font      = e.Font ?? lstFrameData.Font;
                foreColor = isSelected ? SystemColors.HighlightText : e.ForeColor;
            }

            using (var foreBrush = new SolidBrush(foreColor))
            {
                string text = frame.DisplayName;
                e.Graphics.DrawString(text, font, foreBrush,
                    new RectangleF(e.Bounds.X + 2, e.Bounds.Y + 1,
                                   e.Bounds.Width - 4, e.Bounds.Height - 2));
            }

            // 자원 해제: 삭제 항목에만 new Font를 생성했으므로 조건부 Dispose
            if (isDeleted) font.Dispose();

            e.DrawFocusRectangle();
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 핵심 로직
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// 로딩 중 버튼 비활성화 / 커서 변경
        /// </summary>
        private void SetLoadingState(bool loading)
        {
            btnOpenFolder.Enabled           = !loading;
            btnReload.Enabled               = !loading;
            btnApplyFilter.Enabled          = !loading;
            btnClearFilter.Enabled          = !loading;
            btnExcludeSelectedFrame.Enabled = !loading;
            btnRestoreFrame.Enabled         = !loading;
            this.Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
            if (loading) toolStripStatusLabelFrames.Text = "로딩 중...";
        }

        // ── [2] UpdateUIState ─────────────────────────────────────────────────────
        /// <summary>
        /// FormattedIndex를 재계산하고 ListBox를 즉시 갱신합니다.
        /// • IsDeleted == false : [0000], [0001], [0002] ...
        /// • IsDeleted == true  : [XXXX]
        /// </summary>
        private void UpdateUIState()
        {
            // 1) 유효 인덱스 재계산
            int validIndex = 0;
            foreach (var f in _originalFrames)
            {
                if (f.IsDeleted)
                    f.FormattedIndex = "[XXXX]";
                else
                    f.FormattedIndex = $"[{validIndex++:D4}]";
            }

            // 2) ListBox 즉시 갱신 (BeginUpdate/EndUpdate 없이 DataSource 교체로 강제 갱신)
            lstFrameData.DataSource    = null;
            lstFrameData.DataSource    = _currentDisplayedFrames;
            lstFrameData.DisplayMember = "DisplayName";

            // 3) TrackBar 범위 갱신
            trkFramePosition.Minimum = 0;
            trkFramePosition.Maximum = Math.Max(0, _currentDisplayedFrames.Count - 1);

            UpdateStatusLabels();
        }

        /// <summary>
        /// 초기 로드 직후 또는 복원 직후에 사용합니다.
        /// _currentDisplayedFrames를 _originalFrames로 설정하고 UpdateUIState를 호출합니다.
        /// </summary>
        private void RefreshListBinding()
        {
            _currentDisplayedFrames = _originalFrames;
            UpdateUIState();
        }

        private void SetIndex(int idx)
        {
            if (_currentDisplayedFrames == null || _currentDisplayedFrames.Count == 0) return;
            idx = Math.Max(0, Math.Min(_currentDisplayedFrames.Count - 1, idx));
            currentIndex = idx;

            // 이벤트 루프 방지
            lstFrameData.SelectedIndexChanged -= lstFrameData_SelectedIndexChanged;
            lstFrameData.SelectedIndex = idx;
            lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;

            if (trkFramePosition.Value != idx)
                trkFramePosition.Value = idx;

            var frame = _currentDisplayedFrames[idx];

            string resolvedPath = ResolveImagePath(frame.Name);
            UpdatePreviewImage(resolvedPath);

            lblFrameValue.Text    = $"Frame: {idx + 1} / {_currentDisplayedFrames.Count}";
            lblAngleValue.Text    = $"조향값: {frame.Angle:0.000}";
            lblThrottleValue.Text = $"스로틀값: {frame.Throttle:0.000}";
            lblModeValue.Text     = $"모드: {frame.Mode}";
            UpdateStatusLabels();
        }

        /// <summary>
        /// _currentFolderPath + 파일명으로 실제 이미지 경로를 탐색합니다.
        /// 1) 루트 폴더 직접, 2) 하위 images 폴더
        /// </summary>
        private string ResolveImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(_currentFolderPath))
                return string.Empty;

            string p1 = Path.Combine(_currentFolderPath, fileName);
            if (File.Exists(p1)) return p1;

            string p2 = Path.Combine(_currentFolderPath, "images", fileName);
            if (File.Exists(p2)) return p2;

            return string.Empty;
        }

        private void UpdatePreviewImage(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    picMainPreview.Image?.Dispose();
                    picMainPreview.Image = null;
                    return;
                }
                using var fs  = File.OpenRead(path);
                using var img = System.Drawing.Image.FromStream(fs);
                var bmp = new Bitmap(img);

                var old = picMainPreview.Image;
                picMainPreview.Image = bmp;
                old?.Dispose();
            }
            catch { /* 이미지 로드 실패 무시 */ }
        }

        private void TogglePlayPause()
        {
            if (isPlaying)
            {
                playTimer.Stop();
                isPlaying = false;
                btnAutoPlay.Text = "자동 재생";
            }
            else
            {
                playTimer.Interval = (int)numPlaybackInterval.Value;
                playTimer.Start();
                isPlaying = true;
                btnAutoPlay.Text = "⏸ 일시정지";
            }
        }

        private void ToggleTheme()
        {
            isDarkTheme = !isDarkTheme;
            System.Drawing.Color back = isDarkTheme
                ? System.Drawing.Color.FromArgb(45, 45, 48)
                : SystemColors.Control;
            System.Drawing.Color fore = isDarkTheme
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

        // ── [4] 필터 적용 ─────────────────────────────────────────────────────────
        /// <summary>
        /// Angle/Throttle 범위 필터 적용.
        /// 필터 적용 시 _currentDisplayedFrames = IsDeleted==false 인 것만.
        /// </summary>
        private void ApplyFilter()
        {
            if (_originalFrames == null || _originalFrames.Count == 0) return;

            if (!TryReadFilterRanges(out double angleMin, out double angleMax,
                                     out double throttleMin, out double throttleMax))
                return;

            // [4] 필터 적용: IsDeleted==false 인 것만 + 범위 조건
            _currentDisplayedFrames = _originalFrames
                .Where(f => !f.IsDeleted &&
                            f.Angle    >= angleMin && f.Angle    <= angleMax &&
                            f.Throttle >= throttleMin && f.Throttle <= throttleMax)
                .ToList();

            _chartDirty = true;
            UpdateUIState();
            if (_currentDisplayedFrames.Count > 0) SetIndex(0);
        }

        private bool TryReadFilterRanges(out double angleMin, out double angleMax,
                                          out double throttleMin, out double throttleMax)
        {
            angleMin = angleMax = throttleMin = throttleMax = 0;

            bool ok =
                double.TryParse(txtAngleMin.Text.Trim(),   NumberStyles.Float, CultureInfo.InvariantCulture, out angleMin) &&
                double.TryParse(txtAngleMax.Text.Trim(),   NumberStyles.Float, CultureInfo.InvariantCulture, out angleMax) &&
                double.TryParse(txtThrottleMin.Text.Trim(),NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMin) &&
                double.TryParse(txtThrottleMax.Text.Trim(),NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMax);

            if (!ok)
            {
                MessageBox.Show("조향각과 스로틀 범위는 숫자로 입력해 주세요.",
                    "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (angleMin > angleMax || throttleMin > throttleMax)
            {
                MessageBox.Show("범위의 최솟값은 최댓값보다 클 수 없습니다.",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // ── [4] 필터 해제 ─────────────────────────────────────────────────────────
        /// <summary>
        /// 필터 해제: _currentDisplayedFrames = _originalFrames 원상복구.
        /// </summary>
        private void ClearFilter()
        {
            if (_originalFrames == null) return;
            // [4] 필터 해제
            _currentDisplayedFrames = _originalFrames;
            _chartDirty = true;
            UpdateUIState();
            if (_currentDisplayedFrames.Count > 0) SetIndex(0);
        }

        // ── [4] Soft Delete 구간 처리 ────────────────────────────────────────────
        private void SoftDeleteRange(int from, int to)
        {
            if (_originalFrames == null || _originalFrames.Count == 0) return;

            int safeFrom = Math.Max(0, from);
            int safeTo   = Math.Min(_originalFrames.Count - 1, to);

            if (safeFrom > safeTo)
            {
                MessageBox.Show(
                    $"유효한 프레임 범위를 벗어났습니다.\n입력 범위: {from} ~ {to} | 전체 프레임: {_originalFrames.Count}",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newlyDeleted = 0;
            for (int i = safeFrom; i <= safeTo; i++)
            {
                if (!_originalFrames[i].IsDeleted)
                {
                    _originalFrames[i].IsDeleted = true;
                    newlyDeleted++;
                }
            }

            // [4] 삭제 후 → UpdateUIState + RenderChart
            _chartDirty = true;
            UpdateUIState();
            RenderChart();

            int clamped = Math.Max(0, Math.Min(currentIndex, _currentDisplayedFrames.Count - 1));
            if (_currentDisplayedFrames.Count > 0) SetIndex(clamped);

            MessageBox.Show(
                $"완료: {newlyDeleted}개 프레임이 제외(Soft Delete) 처리되었습니다.\n" +
                "(이미 삭제 상태였던 프레임은 제외)\n" +
                "'복원' 버튼으로 언제든 되돌릴 수 있습니다.",
                "구간 제외 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RestoreAll()
        {
            if (_originalFrames == null) return;

            foreach (var f in _originalFrames)
                f.IsDeleted = false;

            // 복원 후 전체 표시
            _currentDisplayedFrames = _originalFrames;
            _chartDirty = true;
            UpdateUIState();
            RenderChart();
            SetIndex(0);

            MessageBox.Show("모든 프레임이 복원되었습니다.",
                "복원 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStatusLabels()
        {
            int total   = _originalFrames?.Count ?? 0;
            int deleted = _originalFrames?.Count(f => f.IsDeleted) ?? 0;
            int shown   = _currentDisplayedFrames?.Count ?? 0;
            toolStripStatusLabelFrames.Text =
                $"전체: {total}  |  제외됨: {deleted}  |  표시 중: {shown}";
        }

        // ══════════════════════════════════════════════════════════════════════════
        // [1] FrameData 데이터 모델
        // ══════════════════════════════════════════════════════════════════════════

        private class FrameData
        {
            public string ImagePath  { get; set; } = string.Empty;
            public double Angle      { get; set; }
            public double Throttle   { get; set; }
            public string Mode       { get; set; } = "-";
            public string Name       { get; set; } = string.Empty;

            /// <summary>
            /// Soft Delete 플래그. true이면 학습에서 제외되지만 파일은 보존됩니다.
            /// </summary>
            public bool IsDeleted { get; set; } = false;

            /// <summary>
            /// 카탈로그 파일(catalog_0.catalog)이 폴더에 없는 경우 true.
            /// </summary>
            public bool IsCatalogMissing { get; set; } = false;

            /// <summary>
            /// 카탈로그는 있지만 이 이미지에 대응하는 데이터 행이 없는 경우 true.
            /// </summary>
            public bool HasNoData { get; set; } = false;

            /// <summary>
            /// [1] 동적 재인덱싱 결과를 담는 속성.
            /// UpdateUIState() 호출 시 자동으로 갱신됩니다.
            /// • IsDeleted == false : "[0000]", "[0001]", ...
            /// • IsDeleted == true  : "[XXXX]"
            /// </summary>
            public string FormattedIndex { get; set; } = "[----]";

            /// <summary>
            /// [1] ListBox 바인딩용 표시 이름.
            /// 맨 앞에 FormattedIndex가 오며 [X] 접두어는 사용하지 않습니다.
            /// 우선순위:
            ///   1) IsCatalogMissing → "{FormattedIndex} {Name} | [카탈로그 파일 없음]"
            ///   2) HasNoData        → "{FormattedIndex} {Name} | catalog 값 없음"
            ///   3) 정상: Throttle &lt; 0 → "▼ 후진",
            ///           Angle &lt; -0.1 → "◀ 좌", Angle &gt; 0.1 → "우 ▶", 그 외 → "▲ 직진"
            /// 출력 예시: "[XXXX] 3_cam-image_array_.jpg | ▼ 후진 (조향: -0.80, 스로틀: -0.50)"
            /// </summary>
            public string DisplayName
            {
                get
                {
                    if (IsCatalogMissing)
                        return $"{FormattedIndex} {Name} | [카탈로그 파일 없음]";
                    if (HasNoData)
                        return $"{FormattedIndex} {Name} | catalog 값 없음";

                    string dir;
                    if (Throttle < 0)
                        dir = "▼ 후진";
                    else if (Angle < -0.1)
                        dir = "◀ 좌";
                    else if (Angle > 0.1)
                        dir = "우 ▶";
                    else
                        dir = "▲ 직진";

                    return $"{FormattedIndex} {Name} | {dir} (조향: {Angle:0.00}, 스로틀: {Throttle:0.00})";
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        // Mock Parser — 이미지 파일 기준 + catalog_0.catalog 병합
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// 이미지 파일(.jpg)을 기준으로 FrameData 리스트를 생성합니다.
        /// 1) 폴더(및 하위 images 폴더)에서 .jpg 검색·정렬
        /// 2) catalog_0.catalog 를 JSON Lines로 파싱하여 Dictionary 구축
        /// 3) 이미지 목록 기준으로 catalog 데이터 병합
        ///    - 카탈로그 없음       → IsCatalogMissing = true
        ///    - 카탈로그 행 없음    → HasNoData = true
        /// </summary>
        private List<FrameData> LoadMockData(string folderPath)
        {
            _currentFolderPath = folderPath;

            // ── 1. .jpg 파일 목록 수집 ─────────────────────────────────────────
            var jpgFiles = new List<string>();

            if (Directory.Exists(folderPath))
                jpgFiles.AddRange(
                    Directory.EnumerateFiles(folderPath, "*.jpg")
                             .Select(Path.GetFileName)!);

            string subImagesDir = Path.Combine(folderPath, "images");
            if (Directory.Exists(subImagesDir))
            {
                var existing = new HashSet<string>(jpgFiles, StringComparer.OrdinalIgnoreCase);
                foreach (var f in Directory.EnumerateFiles(subImagesDir, "*.jpg"))
                {
                    string name = Path.GetFileName(f)!;
                    if (!existing.Contains(name)) jpgFiles.Add(name);
                }
            }

            // 자연 정렬 (숫자 접두어 기준)
            jpgFiles.Sort((a, b) =>
            {
                string nA = new string(a.TakeWhile(char.IsDigit).ToArray());
                string nB = new string(b.TakeWhile(char.IsDigit).ToArray());
                if (int.TryParse(nA, out int iA) && int.TryParse(nB, out int iB))
                    return iA.CompareTo(iB);
                return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            });

            if (jpgFiles.Count == 0) return new List<FrameData>();

            // ── 2. catalog_0.catalog 파싱 ─────────────────────────────────────
            string catalogPath = Path.Combine(folderPath, "catalog_0.catalog");
            bool catalogExists = File.Exists(catalogPath);

            var catalogMap = new Dictionary<string, (double angle, double throttle, string mode)>(
                StringComparer.OrdinalIgnoreCase);

            if (catalogExists)
            {
                try
                {
                    foreach (var rawLine in File.ReadAllLines(catalogPath, Encoding.UTF8))
                    {
                        string line = rawLine.Trim();
                        if (string.IsNullOrEmpty(line)) continue;
                        try
                        {
                            using var doc  = JsonDocument.Parse(line);
                            var root = doc.RootElement;

                            string imgName = string.Empty;
                            if (root.TryGetProperty("cam/image_array", out var imgEl))
                                imgName = imgEl.GetString() ?? string.Empty;
                            if (string.IsNullOrEmpty(imgName)) continue;

                            double angle = 0;
                            if (root.TryGetProperty("user/angle", out var aEl))
                            {
                                if (aEl.ValueKind == JsonValueKind.Number) aEl.TryGetDouble(out angle);
                                else double.TryParse(aEl.GetString(), NumberStyles.Float,
                                         CultureInfo.InvariantCulture, out angle);
                            }

                            double throttle = 0;
                            if (root.TryGetProperty("user/throttle", out var tEl))
                            {
                                if (tEl.ValueKind == JsonValueKind.Number) tEl.TryGetDouble(out throttle);
                                else double.TryParse(tEl.GetString(), NumberStyles.Float,
                                         CultureInfo.InvariantCulture, out throttle);
                            }

                            string mode = "-";
                            if (root.TryGetProperty("user/mode", out var mEl))
                                mode = mEl.GetString() ?? "-";

                            catalogMap[imgName] = (angle, throttle, mode);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // ── 3. 이미지 기준 FrameData 리스트 구축 ─────────────────────────
            var result = new List<FrameData>(jpgFiles.Count);
            foreach (var imgName in jpgFiles)
            {
                var frame = new FrameData
                {
                    Name      = imgName,
                    ImagePath = Path.Combine(folderPath, imgName),
                    IsDeleted = false
                };

                if (!catalogExists)
                    frame.IsCatalogMissing = true;
                else if (catalogMap.TryGetValue(imgName, out var data))
                {
                    frame.Angle    = data.angle;
                    frame.Throttle = data.throttle;
                    frame.Mode     = data.mode;
                }
                else
                    frame.HasNoData = true;

                result.Add(frame);
            }
            return result;
        }

        private async Task LoadCatalogAsync(string folder)
        {
            SetLoadingState(true);
            try
            {
                var frames = await Task.Run(() => LoadMockData(folder));

                if (frames.Count == 0)
                {
                    MessageBox.Show(
                        "선택한 폴더에서 .jpg 이미지 파일을 찾을 수 없습니다.\n" +
                        "이미지가 폴더 루트 또는 하위 'images' 폴더에 있는지 확인해 주세요.",
                        "이미지 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _originalFrames = frames;
                // 초기 로드: 전체 표시
                RefreshListBinding();  // _currentDisplayedFrames = _originalFrames + UpdateUIState 호출

                toolStripStatusLabelPath.Text = "경로: " + folder;
                _chartDirty = true;
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

        // ══════════════════════════════════════════════════════════════════════════
        // Designer 연결 스텁
        // ══════════════════════════════════════════════════════════════════════════

        private void tabPageTraining_Click(object sender, EventArgs e) { }
        private void lblFilterMin_Click(object sender, EventArgs e) { }
        private void mnuHelp_Click(object sender, EventArgs e) { }
        private void mnuOpenGraphStats_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageGraphStats;
        }
        private void lblModeValue_Click(object sender, EventArgs e) { }
        private void lblAngleValue_Click(object sender, EventArgs e) { }
        private void lblPlayInterval_Click(object sender, EventArgs e) { }
        private void lblThrottleValue_Click(object sender, EventArgs e) { }
        private void toolStripStatusLabelTraining_Click(object sender, EventArgs e) { }
        private void btnMycarPath_Click(object sender, EventArgs e) { }
        private void grpTubCleaner_Enter(object sender, EventArgs e) { }

        // ══════════════════════════════════════════════════════════════════════════
        // ScottPlot 차트 — 동적 생성 / Lazy 렌더링
        // ══════════════════════════════════════════════════════════════════════════

        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageGraphStats && _chartDirty)
                RenderChart();
        }

        private void InitFormsPlot()
        {
            _formsPlot = new FormsPlot
            {
                Location = new System.Drawing.Point(0, 42),
                Size     = new System.Drawing.Size(
                    tabPageGraphStats.ClientSize.Width,
                    tabPageGraphStats.ClientSize.Height - 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right,
                Name = "formsPlotMain"
            };

            _formsPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1e1e1e");
            _formsPlot.Plot.DataBackground.Color   = ScottPlot.Color.FromHex("#2d2d30");

            tabPageGraphStats.Controls.Add(_formsPlot);
            _formsPlot.BringToFront();
        }

        // ── [5] RenderChart — 유효 데이터만 연속 인덱스로 표시 ───────────────────
        /// <summary>
        /// _originalFrames 중 IsDeleted == false 인 것만 모아,
        /// X축은 0, 1, 2, ... 연속 인덱스를 사용합니다.
        /// VerticalSpan은 더 이상 사용하지 않습니다.
        /// </summary>
        private void RenderChart()
        {
            if (_originalFrames == null || _originalFrames.Count == 0)
            {
                _chartDirty = false;
                return;
            }

            if (_formsPlot == null) InitFormsPlot();

            var plot = _formsPlot!.Plot;

            // ── 한글 폰트 강제 적용 (SkiaSharp 기본 폰트는 한글 미지원) ──────────
            _formsPlot.Plot.Axes.Title.Label.FontName           = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.Label.FontName          = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.Label.FontName            = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.TickLabelStyle.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.TickLabelStyle.FontName   = "Malgun Gothic";

            plot.Clear();

            // ── [5] IsDeleted == false 인 것만 연속 인덱스로 추출 ────────────────
            var validFrames = _originalFrames.Where(f => !f.IsDeleted).ToList();

            if (validFrames.Count == 0)
            {
                plot.Title("유효한 데이터가 없습니다 (모두 제외됨)");
                _formsPlot.Refresh();
                _chartDirty = false;
                return;
            }

            int n = validFrames.Count;
            double[] xs        = Enumerable.Range(0, n).Select(i => (double)i).ToArray();
            double[] angleYs   = validFrames.Select(f => f.Angle).ToArray();
            double[] throttleYs = validFrames.Select(f => f.Throttle).ToArray();

            // ── Angle 라인 ─────────────────────────────────────────────────────
            var sigAngle = plot.Add.SignalXY(xs, angleYs);
            sigAngle.Color       = ScottPlot.Color.FromHex("#4FC3F7");  // 하늘색
            sigAngle.LineWidth   = 1.5f;
            sigAngle.LegendText  = "조향(Angle)";

            // ── Throttle 라인 ──────────────────────────────────────────────────
            var sigThrottle = plot.Add.SignalXY(xs, throttleYs);
            sigThrottle.Color      = ScottPlot.Color.FromHex("#81C784");  // 연초록
            sigThrottle.LineWidth  = 1.5f;
            sigThrottle.LegendText = "스로틀(Throttle)";

            // ── 축 레이블 ──────────────────────────────────────────────────────
            plot.XLabel("유효 프레임 인덱스 (제외된 프레임 건너뜀)");
            plot.YLabel("값 (Angle / Throttle)");
            plot.Title($"조향값·스로틀 분포 그래프  [유효: {n} / 전체: {_originalFrames.Count}]");
            plot.Axes.SetLimitsY(-1.2, 1.2);
            plot.ShowLegend(Alignment.UpperRight);

            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#444444");

            _formsPlot.Refresh();
            _chartDirty = false;
        }
    }
}
