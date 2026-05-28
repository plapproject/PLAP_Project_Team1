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
        // _originalFrames : 濡쒕뱶??紐⑤뱺 ?꾨젅??(??젣??寃??ы븿, ?쒖꽌 遺덈?)
        // _currentDisplayedFrames : ?꾩옱 ListBox???쒖떆 以묒씤 紐⑸줉
        //   - ?꾪꽣 ?댁젣 ?? _originalFrames 洹몃?濡?
        //   - ?꾪꽣 ?곸슜 ?? IsDeleted == false ??寃껊쭔
        private List<FrameData> _originalFrames = new List<FrameData>();
        private List<FrameData> _currentDisplayedFrames = new List<FrameData>();

        private int currentIndex = -1;
        private System.Windows.Forms.Timer playTimer;
        private bool isPlaying = false;
        private bool isDarkTheme = false;

        private string _currentFolderPath = "";

        private FormsPlot? _formsPlot;
        private bool _chartDirty = true;

        // ?앹꽦??/ 珥덇린??

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
            // 버튼 이벤트를 코드에서 연결합니다.
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

            // 삭제된 프레임을 다른 색으로 표시하기 위해 직접 그립니다.
            lstFrameData.DrawMode = DrawMode.OwnerDrawFixed;
            lstFrameData.DrawItem += lstFrameData_DrawItem;

            UpdateStatusLabels();
        }

        // UI 이벤트 처리

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
                "3. 필터로 학습 데이터에 포함할 프레임을 찾습니다.\n" +
                "4. 제외할 프레임을 선택 후 '구간 제외'를 실행합니다.\n" +
                "5. '복원'으로 언제든 제외 표시를 되돌릴 수 있습니다.\n" +
                "6. 그래프/통계 탭에서 Angle/Throttle 분포를 확인합니다.",
                "단계별 가이드", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnApplyFilter_Click(object sender, EventArgs e) => ApplyFilter();

        private void BtnClearFilter_Click(object? sender, EventArgs e) => ClearFilter();

        // 선택한 구간을 Soft Delete 처리합니다.
        private void BtnRepair_Click(object? sender, EventArgs e)
        {
            // 현재 ListBox에서 선택한 항목의 원본 프레임 인덱스를 구합니다.
            if (lstFrameData.SelectedItems.Count == 0)
            {
                MessageBox.Show("제외할 프레임을 하나 이상 선택해 주세요.",
                    "선택 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 선택된 항목에서 원본 프레임 인덱스를 수집합니다.
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
                "실제 파일은 삭제하지 않으며 '복원'으로 되돌릴 수 있습니다.\n\n계속하시겠습니까?",
                "구간 제외 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            SoftDeleteRange(from, to);
        }

        // 모든 프레임을 복원합니다.
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

        // 자동 재생 타이머 처리
        private void PlayTimer_Tick(object? sender, EventArgs e)
        {
            if (_currentDisplayedFrames == null || _currentDisplayedFrames.Count == 0) return;
            int next = currentIndex + 1;
            // 제외된 프레임은 건너뜁니다.
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

        // ListBox 항목 색상 처리

        /// <summary>
        /// <summary>
        /// 삭제된 항목은 붉은색 계열 배경으로 표시합니다.
        /// </summary>
        /// </summary>
        private void lstFrameData_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _currentDisplayedFrames.Count) return;

            e.DrawBackground();

            var frame = _currentDisplayedFrames[e.Index];
            bool isSelected = (e.State & DrawItemState.Selected) != 0;
            bool isDeleted  = frame.IsDeleted;

            // 삭제 여부와 선택 상태에 따라 배경색을 정합니다.
            System.Drawing.Color backColor;
            if (isDeleted)
                backColor = isSelected
                    ? System.Drawing.Color.FromArgb(255, 180, 180)   // 선택된 삭제 항목
                    : System.Drawing.Color.FromArgb(255, 235, 235);  // 일반 삭제 항목
            else
                backColor = isSelected ? SystemColors.Highlight : e.BackColor;

            using (var backBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(backBrush, e.Bounds);

            // 삭제된 항목은 굵은 빨간 글씨로 표시합니다.
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

            // 새로 만든 Font 객체만 해제합니다.
            if (isDeleted) font.Dispose();

            e.DrawFocusRectangle();
        }

        // 공통 로직

        /// <summary>
        /// 로딩 중 버튼 비활성화와 커서 변경을 처리합니다.
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

        // 목록과 상태 표시를 갱신합니다.
        /// <summary>
        /// FormattedIndex를 다시 계산하고 ListBox를 갱신합니다.
        /// ??IsDeleted == false : [0000], [0001], [0002] ...
        /// ??IsDeleted == true  : [XXXX]
        /// </summary>
        private void UpdateUIState()
        {
            // 1) ?좏슚 ?몃뜳???ш퀎??
            int validIndex = 0;
            foreach (var f in _originalFrames)
            {
                if (f.IsDeleted)
                    f.FormattedIndex = "[XXXX]";
                else
                    f.FormattedIndex = $"[{validIndex++:D4}]";
            }

            // 2) ListBox 利됱떆 媛깆떊 (BeginUpdate/EndUpdate ?놁씠 DataSource 援먯껜濡?媛뺤젣 媛깆떊)
            lstFrameData.DataSource    = null;
            lstFrameData.DataSource    = _currentDisplayedFrames;
            lstFrameData.DisplayMember = "DisplayName";

            // 3) TrackBar 踰붿쐞 媛깆떊
            trkFramePosition.Minimum = 0;
            trkFramePosition.Maximum = Math.Max(0, _currentDisplayedFrames.Count - 1);

            UpdateStatusLabels();
        }

        /// <summary>
        /// 전체 원본 프레임을 다시 표시 상태로 복원합니다.
        /// UpdateUIState를 호출해 화면 상태도 함께 갱신합니다.
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

            // 선택 변경 이벤트가 중복 실행되지 않도록 잠시 해제합니다.
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
        /// 현재 폴더와 파일명으로 실제 이미지 경로를 찾습니다.
        /// 폴더 루트와 하위 images 폴더를 모두 확인합니다.
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
            catch { /* 이미지 로드 실패는 미리보기만 비우고 무시합니다. */ }
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
                btnAutoPlay.Text = "일시정지";
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

        // 필터 적용
        /// <summary>
        /// Angle/Throttle 범위 필터를 적용합니다.
        /// 제외되지 않은 프레임만 화면에 표시합니다.
        /// </summary>
        private void ApplyFilter()
        {
            if (_originalFrames == null || _originalFrames.Count == 0) return;

            if (!TryReadFilterRanges(out double angleMin, out double angleMax,
                                     out double throttleMin, out double throttleMax))
                return;

            // 제외되지 않은 프레임 중 범위 조건에 맞는 항목만 남깁니다.
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
                MessageBox.Show("범위의 최소값은 최대값보다 클 수 없습니다.",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // 필터 해제
        /// <summary>
        /// 필터를 해제하고 원본 프레임 목록을 다시 표시합니다.
        /// </summary>
        private void ClearFilter()
        {
            if (_originalFrames == null) return;
            // 원본 목록을 그대로 다시 표시합니다.
            _currentDisplayedFrames = _originalFrames;
            _chartDirty = true;
            UpdateUIState();
            if (_currentDisplayedFrames.Count > 0) SetIndex(0);
        }

        // Soft Delete 구간 처리
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

            // 삭제 상태를 반영하고 차트를 갱신합니다.
            _chartDirty = true;
            UpdateUIState();
            RenderChart();

            int clamped = Math.Max(0, Math.Min(currentIndex, _currentDisplayedFrames.Count - 1));
            if (_currentDisplayedFrames.Count > 0) SetIndex(clamped);

            MessageBox.Show(
                $"완료: {newlyDeleted}개 프레임이 제외(Soft Delete) 처리되었습니다.\n" +
                "이미 제외된 상태였던 프레임은 건너뛰었습니다.\n" +
                "'복원' 버튼으로 언제든 되돌릴 수 있습니다.",
                "구간 제외 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RestoreAll()
        {
            if (_originalFrames == null) return;

            foreach (var f in _originalFrames)
                f.IsDeleted = false;

            // 복원 후 전체 프레임을 다시 표시합니다.
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

        // FrameData 데이터 모델

        private class FrameData
        {
            public string ImagePath  { get; set; } = string.Empty;
            public double Angle      { get; set; }
            public double Throttle   { get; set; }
            public string Mode       { get; set; } = "-";
            public string Name       { get; set; } = string.Empty;

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
            /// UpdateUIState 호출 시 자동으로 갱신됩니다.
            /// ??IsDeleted == false : "[0000]", "[0001]", ...
            /// ??IsDeleted == true  : "[XXXX]"
            /// </summary>
            public string FormattedIndex { get; set; } = "[----]";

            /// <summary>
            /// ListBox에 표시할 이름입니다.
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

        // Mock Parser: 이미지 파일 목록과 catalog_0.catalog를 병합합니다.

        /// <summary>
        /// 이미지 파일(.jpg)을 기준으로 FrameData 목록을 생성합니다.
        /// 1) 폴더 루트와 하위 images 폴더에서 .jpg 파일 검색
        /// 2) catalog_0.catalog를 JSON Lines로 파싱해 Dictionary 구성
        /// 3) 이미지 목록과 catalog 데이터를 병합
        ///    - 카탈로그 없음: IsCatalogMissing = true
        ///    - catalog 데이터 없음: HasNoData = true
        /// </summary>
        private List<FrameData> LoadMockData(string folderPath)
        {
            _currentFolderPath = folderPath;

            // 1. .jpg 파일 목록을 수집합니다.
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

            // 숫자 접두어 기준으로 자연 정렬합니다.
            jpgFiles.Sort((a, b) =>
            {
                string nA = new string(a.TakeWhile(char.IsDigit).ToArray());
                string nB = new string(b.TakeWhile(char.IsDigit).ToArray());
                if (int.TryParse(nA, out int iA) && int.TryParse(nB, out int iB))
                    return iA.CompareTo(iB);
                return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            });

            if (jpgFiles.Count == 0) return new List<FrameData>();

            // 2. catalog_0.catalog를 파싱합니다.
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

            // 3. 이미지 기준으로 FrameData 목록을 구성합니다.
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
                // 초기 로드: 전체 프레임을 표시합니다.
                RefreshListBinding();  // _currentDisplayedFrames 설정과 UpdateUIState 호출

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

        // Designer 연결 스텁

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

        // ScottPlot 차트를 필요할 때 생성합니다.

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

        // 유효 데이터만 연속 인덱스로 차트에 표시합니다.
        /// <summary>
        /// 제외되지 않은 원본 프레임만 차트 데이터로 사용합니다.
        /// X축은 0, 1, 2 순서의 연속 인덱스입니다.
        /// VerticalSpan은 현재 사용하지 않습니다.
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

            // 한글 폰트를 지정해 차트 라벨이 깨지지 않도록 합니다.
            _formsPlot.Plot.Axes.Title.Label.FontName           = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.Label.FontName          = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.Label.FontName            = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.TickLabelStyle.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.TickLabelStyle.FontName   = "Malgun Gothic";

            plot.Clear();

            // 제외되지 않은 프레임만 추출합니다.
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

            // Angle 라인
            var sigAngle = plot.Add.SignalXY(xs, angleYs);
            sigAngle.Color       = ScottPlot.Color.FromHex("#4FC3F7");  // 하늘색
            sigAngle.LineWidth   = 1.5f;
            sigAngle.LegendText  = "조향(Angle)";

            // Throttle 라인
            var sigThrottle = plot.Add.SignalXY(xs, throttleYs);
            sigThrottle.Color      = ScottPlot.Color.FromHex("#81C784");  // 연초록
            sigThrottle.LineWidth  = 1.5f;
            sigThrottle.LegendText = "스로틀(Throttle)";

            // 축 라벨과 제목
            plot.XLabel("유효 프레임 인덱스(제외된 프레임은 건너뜀)");
            plot.YLabel("값(Angle / Throttle)");
            plot.Title($"조향값/스로틀 분포 그래프 [유효: {n} / 전체: {_originalFrames.Count}]");
            plot.Axes.SetLimitsY(-1.2, 1.2);
            plot.ShowLegend(Alignment.UpperRight);

            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#444444");

            _formsPlot.Refresh();
            _chartDirty = false;
        }
    }
}
