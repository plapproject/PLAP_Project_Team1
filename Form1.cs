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
        private List<FrameData> originalFrames = new List<FrameData>();
        private List<FrameData> filteredFrames = new List<FrameData>();
        private int currentIndex = -1;
        private System.Windows.Forms.Timer playTimer;
        private bool isPlaying = false;
        private bool isDarkTheme = false;

        // ── 현재 열려 있는 폴더 경로 (이미지 경로 조합에 사용) ──────────────────────
        private string _currentFolderPath = "";

        // ── ScottPlot 그래프 관련 필드 ────────────────────────────────────────────
        private FormsPlot? _formsPlot;      // tabPage3에 동적으로 생성되는 차트 컨트롤
        private bool _chartDirty = true;    // true이면 tabPage3 진입 시 RenderChart() 실행

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            playTimer = new System.Windows.Forms.Timer();
            playTimer.Tick += PlayTimer_Tick;

            // ensure status strip is docked
            if (statusStrip1 != null && !this.Controls.Contains(statusStrip1))
            {
                statusStrip1.Dock = DockStyle.Bottom;
                this.Controls.Add(statusStrip1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ── Designer에서 Click이 연결되지 않은 버튼 핸들러 동적 등록 ──────────────
            btnClearFilter.Click += BtnClearFilter_Click;
            btnRepair.Click += BtnRepair_Click;
            btnReloadTub.Click += BtnReloadTub_Click;

            // 메뉴 항목 핸들러 동적 등록
            데이터폴더열기ToolStripMenuItem.Click += (s, _) => btnOpenFolder_Click(s!, EventArgs.Empty);
            다시불러오기ToolStripMenuItem.Click += (s, _) => btnReload_Click(s!, EventArgs.Empty);
            종료ToolStripMenuItem.Click += (s, _) => Application.Exit();
            단계별가이드ToolStripMenuItem.Click += (s, _) => btnGuide_Click(s!, EventArgs.Empty);

            // ── 탭 전환 이벤트: tabPage3 진입 시에만 RenderChart() 호출 ────────────
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

            // ── 버튼 텍스트: Designer 값을 코드에서 덮어씀 (Designer.cs 수정 없이) ──
            btnRepair.Text = "구간 삭제";
            btnReloadTub.Text = "전체 복원/재로드";

            // ── numFilterMin / numFilterMax: 필터값(소수) + 프레임 인덱스(정수) 겸용 ─
            //    DecimalPlaces=3 으로 설정해 Angle/Throttle 필터에도 사용 가능하게 함
            numFilterMin.DecimalPlaces = 3;
            numFilterMax.DecimalPlaces = 3;
            numFilterMin.Minimum = -100000;
            numFilterMin.Maximum = 100000;
            numFilterMax.Minimum = -100000;
            numFilterMax.Maximum = 100000;
            numFilterMin.Value = 0;
            numFilterMax.Value = 0;

            // 초기 상태 표시
            UpdateStatusLabels();
        }

        // ══════════════════════════════════════════════════════════════════════════
        // UI 이벤트 핸들러
        // ══════════════════════════════════════════════════════════════════════════

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // 항상 이미지 파일 기준 로더 사용 (catalog 유무 자동 판별)
            _ = LoadCatalogAsync(dlg.SelectedPath);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            // _currentFolderPath 우선 사용, 없으면 상태 표시줄에서 경로 추출
            string path = _currentFolderPath;
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(toolStripStatusLabelPath.Text) || toolStripStatusLabelPath.Text == "경로: -") return;
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
                "3. Throttle > 0 또는 Angle 범위 필터로 학습 품질을 점검합니다.\n" +
                "4. 최솟값/최댓값에 삭제할 프레임 인덱스 범위를 입력 후\n" +
                "   '구간 삭제' 버튼으로 Soft Delete 처리합니다.\n" +
                "5. '전체 복원/재로드'로 언제든 복원할 수 있습니다.\n" +
                "6. 그래프/통계 탭에서 Angle·Throttle 분포를 시각화할 수 있습니다.",
                "단계별 가이드", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnApplyFilter_Click(object sender, EventArgs e) => ApplyFilter();

        // Designer에서 미연결 → Form1_Load에서 동적 연결
        private void BtnClearFilter_Click(object sender, EventArgs e) => ClearFilter();

        // ── 구간 삭제 (Soft Delete) ───────────────────────────────────────────────
        private void BtnRepair_Click(object sender, EventArgs e)
        {
            int from = (int)numFilterMin.Value;
            int to = (int)numFilterMax.Value;

            if (from > to)
            {
                MessageBox.Show(
                    "최솟값(시작 인덱스)이 최댓값(끝 인덱스)보다 큽니다.\n범위를 다시 확인해 주세요.",
                    "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int count = Math.Min(to, originalFrames.Count - 1) - Math.Max(from, 0) + 1;
            if (count <= 0)
            {
                MessageBox.Show(
                    $"유효한 프레임 범위를 벗어났습니다.\n전체 프레임 수: {originalFrames.Count}",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"프레임 인덱스 {from} ~ {to} (최대 {count}개)를 Soft Delete 처리합니다.\n" +
                "실제 파일은 삭제되지 않으며, '전체 복원/재로드'로 되돌릴 수 있습니다.\n\n계속하시겠습니까?",
                "구간 삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            SoftDeleteRange(from, to);
        }

        // ── 전체 복원 ─────────────────────────────────────────────────────────────
        private void BtnReloadTub_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "모든 Soft Delete 플래그를 초기화하고 전체 데이터를 복원합니다.\n계속하시겠습니까?",
                "전체 복원", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            RestoreAll();
        }

        // ── 자동 재생: Designer에서 button3_Click에 연결됨 → 이 메서드를 구현 ──
        private void button3_Click(object sender, EventArgs e) => TogglePlayPause();

        private void listBoxData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxData.SelectedItem == null) return;
            int idx = listBoxData.SelectedIndex;
            if (idx >= 0 && idx < filteredFrames.Count)
                SetIndex(idx);
        }

        private void trackBarMain_Scroll(object sender, EventArgs e)
        {
            int idx = trackBarMain.Value;
            if (idx >= 0 && idx < filteredFrames.Count)
                SetIndex(idx);
        }

        private void btnFirst_Click(object sender, EventArgs e) => SetIndex(0);
        private void btnPrev_Click(object sender, EventArgs e) => SetIndex(Math.Max(0, currentIndex - 1));
        private void btnNext_Click(object sender, EventArgs e) => SetIndex(Math.Min(filteredFrames.Count - 1, currentIndex + 1));
        private void btnLast_Click(object sender, EventArgs e) => SetIndex(filteredFrames.Count - 1);

        // ── 재생 타이머 ───────────────────────────────────────────────────────────
        private void PlayTimer_Tick(object? sender, EventArgs e)
        {
            if (filteredFrames == null || filteredFrames.Count == 0) return;
            int next = currentIndex + 1;
            // 삭제된 프레임을 건너뛰고 다음 정상 프레임을 찾음
            while (next < filteredFrames.Count && filteredFrames[next].IsDeleted)
                next++;
            if (next >= filteredFrames.Count)
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
                case Keys.Left: btnPrev_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.Space: TogglePlayPause(); e.Handled = true; break;
                case Keys.Home: btnFirst_Click(this, EventArgs.Empty); e.Handled = true; break;
                case Keys.End: btnLast_Click(this, EventArgs.Empty); e.Handled = true; break;
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 핵심 로직
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// 폴더에서 이미지와 JSON/CSV 데이터를 비동기로 로드합니다.
        /// 수만 개 파일도 UI 블로킹 없이 처리합니다.
        /// </summary>
        private async Task LoadFolderAsync(string folder)
        {
            SetLoadingState(true);

            try
            {
                // ── 백그라운드 스레드에서 I/O 수행 ──────────────────────────────
                var frames = await Task.Run(() =>
                {
                    var imageExt = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
                    var images = Directory.EnumerateFiles(folder)
                        .Where(f => imageExt.Contains(Path.GetExtension(f).ToLowerInvariant()))
                        .OrderBy(f => f)
                        .ToList();

                    // CSV 매핑 선행 로드 (있는 경우)
                    var csvMap = new Dictionary<string, (double angle, double throttle, string mode)>();
                    var csvFiles = Directory.GetFiles(folder, "*.csv");
                    if (csvFiles.Length > 0)
                    {
                        try
                        {
                            foreach (var line in File.ReadAllLines(csvFiles[0]))
                            {
                                var parts = line.Split(',');
                                if (parts.Length >= 4 &&
                                    double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double a) &&
                                    double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double t))
                                {
                                    csvMap[parts[0].Trim()] = (a, t, parts[3].Trim());
                                }
                            }
                        }
                        catch { }
                    }

                    // 프레임 목록 생성
                    var result = new List<FrameData>(images.Count);
                    foreach (var img in images)
                    {
                        var baseName = Path.GetFileName(img);
                        double angle = 0, throttle = 0;
                        string mode = "-";

                        var jsonPath = Path.Combine(folder,
                            Path.GetFileNameWithoutExtension(img) + ".json");

                        if (File.Exists(jsonPath))
                        {
                            try
                            {
                                using var fs = File.OpenRead(jsonPath);
                                var doc = JsonDocument.Parse(fs);
                                if (doc.RootElement.TryGetProperty("angle", out var aEl) && aEl.TryGetDouble(out double a)) angle = a;
                                if (doc.RootElement.TryGetProperty("throttle", out var tEl) && tEl.TryGetDouble(out double t)) throttle = t;
                                if (doc.RootElement.TryGetProperty("mode", out var mEl)) mode = mEl.GetString() ?? "-";
                            }
                            catch { }
                        }
                        else if (csvMap.TryGetValue(baseName, out var row))
                        {
                            angle = row.angle; throttle = row.throttle; mode = row.mode;
                        }

                        result.Add(new FrameData
                        {
                            ImagePath = img,
                            Angle = angle,
                            Throttle = throttle,
                            Mode = mode,
                            Name = baseName,
                            IsDeleted = false   // 새로 로드 시 모두 유효 상태
                        });
                    }
                    return result;
                });

                // ── UI 스레드에서 결과 반영 ──────────────────────────────────────
                originalFrames = frames;
                filteredFrames = new List<FrameData>(originalFrames);

                // 로드 완료 후 numFilterMax의 최대값을 프레임 수에 맞게 확장
                numFilterMin.Maximum = Math.Max(100000, originalFrames.Count);
                numFilterMax.Maximum = Math.Max(100000, originalFrames.Count);
                numFilterMax.Value = Math.Max(0, originalFrames.Count - 1);

                toolStripStatusLabelPath.Text = "경로: " + folder;
                _chartDirty = true;   // 새 데이터 로드 → 차트 재렌더링 필요
                RefreshListBinding();
                SetIndex(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("폴더 로드 중 오류: " + ex.Message,
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// 로딩 중 버튼 비활성화 / 커서 변경으로 사용자에게 진행 상황을 안내합니다.
        /// </summary>
        private void SetLoadingState(bool loading)
        {
            btnOpenFolder.Enabled = !loading;
            btnReload.Enabled = !loading;
            btnApplyFilter.Enabled = !loading;
            btnClearFilter.Enabled = !loading;
            btnRepair.Enabled = !loading;
            btnReloadTub.Enabled = !loading;
            this.Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
            if (loading) toolStripStatusLabelFrames.Text = "로딩 중...";
        }

        /// <summary>
        /// listBoxData와 trackBarMain을 filteredFrames 기준으로 갱신합니다.
        /// Soft Delete된 항목은 [X] 접두어로 시각적으로 구분합니다.
        /// DisplayMember 바인딩으로 FrameData.DisplayName을 표시합니다.
        /// </summary>
        private void RefreshListBinding()
        {
            listBoxData.BeginUpdate();
            listBoxData.DataSource = null;
            listBoxData.DataSource = filteredFrames;
            listBoxData.DisplayMember = "DisplayName";
            listBoxData.EndUpdate();

            trackBarMain.Minimum = 0;
            trackBarMain.Maximum = Math.Max(0, filteredFrames.Count - 1);
            UpdateStatusLabels();
        }

        private void SetIndex(int idx)
        {
            if (filteredFrames == null || filteredFrames.Count == 0) return;
            idx = Math.Max(0, Math.Min(filteredFrames.Count - 1, idx));
            currentIndex = idx;

            // 이벤트 루프 방지: 핸들러를 잠시 해제 후 재등록
            listBoxData.SelectedIndexChanged -= listBoxData_SelectedIndexChanged;
            listBoxData.SelectedIndex = idx;
            listBoxData.SelectedIndexChanged += listBoxData_SelectedIndexChanged;

            if (trackBarMain.Value != idx)
                trackBarMain.Value = idx;

            var frame = filteredFrames[idx];

            // ── 이미지 경로 조합: _currentFolderPath + Name 우선, 하위 images 폴더도 탐색 ──
            string resolvedImagePath = ResolveImagePath(frame.Name);
            UpdatePreviewImage(resolvedImagePath);

            lblFrameValue.Text = $"Frame: {idx + 1} / {filteredFrames.Count}";
            lblAngleValue.Text = $"조향값: {frame.Angle:0.000}";
            lblThrottleValue.Text = $"스로틀값: {frame.Throttle:0.000}";
            lblModeValue.Text = $"모드: {frame.Mode}";
            UpdateStatusLabels();
        }

        /// <summary>
        /// _currentFolderPath와 파일명을 조합하여 실제 이미지 절대 경로를 찾습니다.
        /// 1) folderPath 직접 → 2) folderPath\images 하위 폴더
        /// 둘 다 없으면 빈 문자열 반환
        /// </summary>
        private string ResolveImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(_currentFolderPath))
                return string.Empty;

            // 1) 카탈로그와 같은 폴더에 있는지
            string path1 = Path.Combine(_currentFolderPath, fileName);
            if (File.Exists(path1)) return path1;

            // 2) 하위 images 폴더에 있는지
            string path2 = Path.Combine(_currentFolderPath, "images", fileName);
            if (File.Exists(path2)) return path2;

            return string.Empty; // 이미지 없음
        }

        private void UpdatePreviewImage(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    pbMainPreview.Image?.Dispose();
                    pbMainPreview.Image = null;
                    return;
                }
                // 파일 핸들을 즉시 해제하기 위해 스트림에서 읽고 Bitmap으로 복사
                using var fs = File.OpenRead(path);
                using var img = System.Drawing.Image.FromStream(fs);
                var bmp = new Bitmap(img);

                var old = pbMainPreview.Image;
                pbMainPreview.Image = bmp;
                old?.Dispose();
            }
            catch { /* 이미지 로드 실패는 무시 */ }
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
                playTimer.Interval = (int)numericUpDownInterval.Value;
                playTimer.Start();
                isPlaying = true;
                btnAutoPlay.Text = "⏸ 일시정지";
            }
        }

        private void ToggleTheme()
        {
            isDarkTheme = !isDarkTheme;
            System.Drawing.Color back = isDarkTheme ? System.Drawing.Color.FromArgb(45, 45, 48) : SystemColors.Control;
            System.Drawing.Color fore = isDarkTheme ? System.Drawing.Color.White : SystemColors.ControlText;
            this.BackColor = back;
            foreach (Control c in this.Controls)
                ApplyThemeToControl(c, back, fore);
            statusStrip1.BackColor = back;
            statusStrip1.ForeColor = fore;
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

        /// <summary>
        /// comboBoxFilter + numFilterMin/Max 기반으로 필터링합니다.
        /// IsDeleted 항목도 리스트에 항상 포함되며, [X] 접두어로 구분됩니다.
        /// </summary>
        private void ApplyFilter()
        {
            if (originalFrames == null || originalFrames.Count == 0) return;

            var sel = comboBoxFilter.SelectedItem?.ToString() ?? "";
            var txt = textBox1?.Text?.Trim() ?? "";
            double min = double.MinValue, max = double.MaxValue;

            try
            {
                if (numFilterMin != null) min = (double)numFilterMin.Value;
                if (numFilterMax != null) max = (double)numFilterMax.Value;
            }
            catch { }

            // textBox1에 "min,max" 형식으로 입력 시 숫자 범위 오버라이드
            if (!string.IsNullOrEmpty(txt) && (txt.Contains(",") || txt.Contains(";")))
            {
                var parts = txt.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1) double.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out min);
                if (parts.Length >= 2) double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out max);
            }

            bool isAngle = sel.IndexOf("Angle", StringComparison.OrdinalIgnoreCase) >= 0 || sel.Contains("조향");
            bool isThrottle = sel.IndexOf("Throttle", StringComparison.OrdinalIgnoreCase) >= 0 || sel.Contains("스로틀");
            bool isMode = sel.IndexOf("Mode", StringComparison.OrdinalIgnoreCase) >= 0 || sel.Contains("모드");

            // ── 핵심: IsDeleted 항목도 항상 포함 (리스트에서 [X]로 표시) ──
            filteredFrames = originalFrames
                .Where(f =>
                {
                    // 삭제된 항목은 필터 조건과 무관하게 항상 포함
                    if (f.IsDeleted) return true;
                    if (isAngle) return f.Angle >= min && f.Angle <= max;
                    if (isThrottle) return f.Throttle >= min && f.Throttle <= max;
                    if (isMode) return f.Mode != null && f.Mode.Contains(txt, StringComparison.OrdinalIgnoreCase);
                    return true; // 전체 보기
                })
                .ToList();

            _chartDirty = true;   // 필터 변경 → 차트 재렌더링 필요
            RefreshListBinding();
            SetIndex(0);
        }

        /// <summary>
        /// 필터 조건을 초기화하고 삭제되지 않은 전체 프레임을 표시합니다.
        /// </summary>
        private void ClearFilter()
        {
            if (originalFrames == null) return;
            filteredFrames = new List<FrameData>(originalFrames);
            comboBoxFilter.SelectedIndex = -1;
            textBox1.Clear();
            _chartDirty = true;   // 필터 해제 → 차트 재렌더링 필요
            RefreshListBinding();
            SetIndex(0);
        }

        /// <summary>
        /// originalFrames 기준의 인덱스 범위로 Soft Delete를 수행합니다.
        /// 실제 파일은 건드리지 않습니다.
        /// </summary>
        private void SoftDeleteRange(int from, int to)
        {
            if (originalFrames == null || originalFrames.Count == 0) return;

            int safeFrom = Math.Max(0, from);
            int safeTo = Math.Min(originalFrames.Count - 1, to);

            if (safeFrom > safeTo)
            {
                MessageBox.Show(
                    $"유효한 프레임 범위를 벗어났습니다.\n" +
                    $"입력 범위: {from} ~ {to} | 전체 프레임 수: {originalFrames.Count}",
                    "범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newlyDeleted = 0;
            for (int i = safeFrom; i <= safeTo; i++)
            {
                if (!originalFrames[i].IsDeleted)
                {
                    originalFrames[i].IsDeleted = true;
                    newlyDeleted++;
                }
            }

            // 삭제 후 filteredFrames 재구성 (삭제 항목도 [X]로 표시하기 위해 전체 유지)
            filteredFrames = new List<FrameData>(originalFrames);
            _chartDirty = true;   // 삭제 구간 변경 → 차트 재렌더링 필요
            RefreshListBinding();

            // currentIndex가 범위를 벗어나지 않도록 보정
            int adjustedIdx = Math.Max(0, Math.Min(currentIndex, filteredFrames.Count - 1));
            if (filteredFrames.Count > 0)
                SetIndex(adjustedIdx);

            MessageBox.Show(
                $"완료: {newlyDeleted}개 프레임이 Soft Delete 처리되었습니다.\n" +
                $"(이미 삭제 상태였던 프레임은 제외)\n" +
                $"'전체 복원/재로드' 버튼으로 언제든 복원 가능합니다.",
                "구간 삭제 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 모든 IsDeleted 플래그를 false로 초기화하여 전체 데이터를 복원합니다.
        /// </summary>
        private void RestoreAll()
        {
            if (originalFrames == null) return;

            foreach (var f in originalFrames)
                f.IsDeleted = false;

            filteredFrames = new List<FrameData>(originalFrames);
            _chartDirty = true;   // 전체 복원 → 차트 재렌더링 필요
            RefreshListBinding();
            SetIndex(0);

            MessageBox.Show("모든 프레임이 복원되었습니다.",
                "복원 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 상태 표시줄에 전체/삭제됨/현재 표시 중인 프레임 수를 갱신합니다.
        /// </summary>
        private void UpdateStatusLabels()
        {
            int total = originalFrames?.Count ?? 0;
            int deleted = originalFrames?.Count(f => f.IsDeleted) ?? 0;
            int shown = filteredFrames?.Count ?? 0;
            toolStripStatusLabelFrames.Text = $"전체: {total}  |  삭제됨: {deleted}  |  표시 중: {shown}";
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 데이터 모델
        // ══════════════════════════════════════════════════════════════════════════

        private class FrameData
        {
            public string ImagePath { get; set; } = string.Empty;
            public double Angle { get; set; }
            public double Throttle { get; set; }
            public string Mode { get; set; } = "-";
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Soft Delete 플래그.
            /// true이면 삭제 대상으로 마킹되었지만 실제 파일은 보존됩니다.
            /// RestoreAll()로 언제든 false로 되돌릴 수 있습니다.
            /// </summary>
            public bool IsDeleted { get; set; } = false;

            /// <summary>
            /// 카탈로그 파일(catalog_0.catalog)이 폴더에 없는 경우 true.
            /// </summary>
            public bool IsCatalogMissing { get; set; } = false;

            /// <summary>
            /// 카탈로그 파일은 존재하지만 이 이미지에 대응하는 데이터 행이 없는 경우 true.
            /// </summary>
            public bool HasNoData { get; set; } = false;

            /// <summary>
            /// ListBox 바인딩용 표시 이름.
            /// 우선순위:
            ///   1) IsCatalogMissing → "{Name} | [카탈로그 파일 없음]"
            ///   2) HasNoData        → "{Name} | catalog 파일이 존재하지만 값이 없습니다."
            ///   3) 정상: Throttle < 0 → "▼ 후진",
            ///           Angle < -0.1 → "◀ 좌", Angle > 0.1 → "우 ▶", 그 외 → "▲ 직진"
            ///   4) IsDeleted → "[X] " 접두어
            /// 출력 예시: "[X] 0_cam-image_array_.jpg | ▼ 후진 (조향: -0.80, 스로틀: -0.50)"
            /// </summary>
            public string DisplayName
            {
                get
                {
                    // ── 예외 상태 우선 ──
                    if (IsCatalogMissing)
                        return $"{Name} | [카탈로그 파일 없음]";
                    if (HasNoData)
                        return $"{Name} | catalog 파일이 존재하지만 값이 없습니다.";

                    // ── 정상 데이터: 방향 판별 ──
                    string dir;
                    if (Throttle < 0)
                        dir = "▼ 후진";
                    else if (Angle < -0.1)
                        dir = "◀ 좌";
                    else if (Angle > 0.1)
                        dir = "우 ▶";
                    else
                        dir = "▲ 직진";

                    string body = $"{Name} | {dir} (조향: {Angle:0.00}, 스로틀: {Throttle:0.00})";
                    return IsDeleted ? $"[X] {body}" : body;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        // [Mock Parser] 이미지 파일 기준 + catalog_0.catalog 병합 파서
        // 팀원 B의 실제 파서가 완성되기 전까지 임시로 사용하는 DataLoader입니다.
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// 이미지 파일(.jpg) 기준으로 FrameData 리스트를 생성합니다.
        /// 1) 폴더(및 하위 images 폴더)에서 모든 .jpg 파일을 검색·정렬
        /// 2) catalog_0.catalog 파일이 있으면 JSON Lines로 파싱하여 Dictionary 구축
        /// 3) 이미지 목록 기준으로 catalog 데이터 병합
        ///    - 카탈로그 없음 → IsCatalogMissing = true
        ///    - 카탈로그는 있으나 이미지에 대응하는 행 없음 → HasNoData = true
        /// </summary>
        private List<FrameData> LoadMockData(string folderPath)
        {
            // ── 0. 경로 저장 ──────────────────────────────────────────────────────
            _currentFolderPath = folderPath;

            // ── 1. .jpg 이미지 파일 검색 (folderPath + folderPath\images) ────────
            var jpgFiles = new List<string>();

            if (Directory.Exists(folderPath))
            {
                jpgFiles.AddRange(
                    Directory.EnumerateFiles(folderPath, "*.jpg")
                        .Select(Path.GetFileName)!);
            }

            string imagesSubDir = Path.Combine(folderPath, "images");
            if (Directory.Exists(imagesSubDir))
            {
                // 하위 images 폴더의 파일 중 루트에 없는 것만 추가 (중복 방지)
                var existing = new HashSet<string>(jpgFiles, StringComparer.OrdinalIgnoreCase);
                foreach (var f in Directory.EnumerateFiles(imagesSubDir, "*.jpg"))
                {
                    string name = Path.GetFileName(f)!;
                    if (!existing.Contains(name))
                        jpgFiles.Add(name);
                }
            }

            // 자연 정렬: 파일명에 포함된 숫자 기준으로 정렬
            jpgFiles.Sort((a, b) =>
            {
                // 숫자 접두어 추출 시도
                string numA = new string(a.TakeWhile(char.IsDigit).ToArray());
                string numB = new string(b.TakeWhile(char.IsDigit).ToArray());
                if (int.TryParse(numA, out int nA) && int.TryParse(numB, out int nB))
                    return nA.CompareTo(nB);
                return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            });

            if (jpgFiles.Count == 0)
                return new List<FrameData>();

            // ── 2. catalog_0.catalog 파싱 → Dictionary<이미지명, 데이터> ──────────
            string catalogPath = Path.Combine(folderPath, "catalog_0.catalog");
            bool catalogExists = File.Exists(catalogPath);

            // Key: 이미지 파일명, Value: (angle, throttle, mode)
            var catalogMap = new Dictionary<string, (double angle, double throttle, string mode)>(
                StringComparer.OrdinalIgnoreCase);

            if (catalogExists)
            {
                try
                {
                    var lines = File.ReadAllLines(catalogPath, Encoding.UTF8);
                    foreach (var rawLine in lines)
                    {
                        string line = rawLine.Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        try
                        {
                            using var doc = JsonDocument.Parse(line);
                            var root = doc.RootElement;

                            // 이미지 파일명
                            string imageName = string.Empty;
                            if (root.TryGetProperty("cam/image_array", out var imgEl))
                                imageName = imgEl.GetString() ?? string.Empty;
                            if (string.IsNullOrEmpty(imageName)) continue;

                            // Angle (InvariantCulture)
                            double angle = 0.0;
                            if (root.TryGetProperty("user/angle", out var angleEl))
                            {
                                if (angleEl.ValueKind == JsonValueKind.Number)
                                    angleEl.TryGetDouble(out angle);
                                else
                                    double.TryParse(angleEl.GetString(), NumberStyles.Float,
                                        CultureInfo.InvariantCulture, out angle);
                            }

                            // Throttle (InvariantCulture)
                            double throttle = 0.0;
                            if (root.TryGetProperty("user/throttle", out var throttleEl))
                            {
                                if (throttleEl.ValueKind == JsonValueKind.Number)
                                    throttleEl.TryGetDouble(out throttle);
                                else
                                    double.TryParse(throttleEl.GetString(), NumberStyles.Float,
                                        CultureInfo.InvariantCulture, out throttle);
                            }

                            // Mode (없으면 기본값 "-")
                            string mode = "-";
                            if (root.TryGetProperty("user/mode", out var modeEl))
                                mode = modeEl.GetString() ?? "-";

                            catalogMap[imageName] = (angle, throttle, mode);
                        }
                        catch { /* 파싱 실패한 줄은 건너뜀 */ }
                    }
                }
                catch { /* catalog 파일 읽기 실패 시 catalogExists를 유지하되 빈 맵 사용 */ }
            }

            // ── 3. 이미지 목록 기준 FrameData 리스트 구축 ─────────────────────────
            var result = new List<FrameData>(jpgFiles.Count);

            foreach (var imgName in jpgFiles)
            {
                var frame = new FrameData
                {
                    Name = imgName,
                    ImagePath = Path.Combine(folderPath, imgName), // 기본 경로
                    IsDeleted = false
                };

                if (!catalogExists)
                {
                    // 카탈로그 파일 자체가 없음
                    frame.IsCatalogMissing = true;
                }
                else if (catalogMap.TryGetValue(imgName, out var data))
                {
                    // 정상 매칭
                    frame.Angle = data.angle;
                    frame.Throttle = data.throttle;
                    frame.Mode = data.mode;
                }
                else
                {
                    // 카탈로그는 있으나 이 이미지에 대응하는 행이 없음
                    frame.HasNoData = true;
                }

                result.Add(frame);
            }

            return result;
        }

        /// <summary>
        /// 이미지 파일 기준 데이터를 비동기로 로드합니다.
        /// 팀원 B의 파서가 완성되면 이 메서드를 교체하면 됩니다.
        /// </summary>
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

                originalFrames = frames;
                filteredFrames = new List<FrameData>(originalFrames);

                numFilterMin.Maximum = Math.Max(100000, originalFrames.Count);
                numFilterMax.Maximum = Math.Max(100000, originalFrames.Count);
                numFilterMax.Value = Math.Max(0, originalFrames.Count - 1);

                toolStripStatusLabelPath.Text = "경로: " + folder;
                _chartDirty = true;
                RefreshListBinding();
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
        // Designer 연결 스텁 (빈 메서드 유지 - 변수명 변경 금지)
        // ══════════════════════════════════════════════════════════════════════════

        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e) { }
        private void tabPage2_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void 도움말ToolStripMenuItem_Click(object sender, EventArgs e) { /* 단계별가이드로 위임됨 */ }
        private void 그래프테ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 보기 메뉴 → 그래프/통계 탭으로 이동 (SelectedIndexChanged가 RenderChart 호출)
            tabControl1.SelectedTab = tabPage3;
        }
        private void lblModeValue_Click(object sender, EventArgs e) { }
        private void lblAngleValue_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void lblThrottleValue_Click(object sender, EventArgs e) { }

        // ══════════════════════════════════════════════════════════════════════════
        // Step 5~7: ScottPlot 그래프 — 동적 생성 / Lazy 렌더링 / 삭제 구간 시각화
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// TabControl 탭 전환 이벤트.
        /// tabPage3(그래프 탭)가 선택될 때만 RenderChart()를 호출합니다.
        /// 필터 적용·삭제 직후에는 _chartDirty만 true로 설정하고, 실제 렌더링은 여기서만 수행합니다.
        /// </summary>
        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage3 && _chartDirty)
                RenderChart();
        }

        /// <summary>
        /// FormsPlot 컨트롤을 tabPage3에 최초 1회 동적으로 생성합니다.
        /// Form1.Designer.cs는 절대 수정하지 않습니다.
        /// </summary>
        private void InitFormsPlot()
        {
            _formsPlot = new FormsPlot();

            // lblDescription(Y=20, AutoSize) 아래에 차트를 배치
            // tabPage3 크기에 맞게 Anchor를 사방으로 설정해 리사이즈 대응
            _formsPlot.Location = new System.Drawing.Point(0, 42);
            _formsPlot.Size = new System.Drawing.Size(
                tabPage3.ClientSize.Width,
                tabPage3.ClientSize.Height - 42);
            _formsPlot.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                              | AnchorStyles.Left | AnchorStyles.Right;
            _formsPlot.Name = "formsPlotMain";

            // 다크 스타일 기본 적용
            _formsPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1e1e1e");
            _formsPlot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#2d2d30");

            tabPage3.Controls.Add(_formsPlot);
            _formsPlot.BringToFront();
        }

        /// <summary>
        /// originalFrames 전체를 기반으로 그래프를 렌더링합니다.
        /// • 파란 선: 조향(Angle)
        /// • 초록 선: 스로틀(Throttle)
        /// • 붉은 반투명 배경: IsDeleted == true 인 연속 구간
        /// 수만 개 데이터도 SignalXY(FastRenderer)로 고속 처리합니다.
        /// </summary>
        private void RenderChart()
        {
            if (originalFrames == null || originalFrames.Count == 0)
            {
                _chartDirty = false;
                return;
            }

            // FormsPlot 컨트롤이 없으면 최초 생성
            if (_formsPlot == null)
                InitFormsPlot();

            var plot = _formsPlot!.Plot;
            // ── 한글 폰트 강제 적용 (ScottPlot 5 SkiaSharp 기본 폰트는 한글 미지원) ──
            _formsPlot.Plot.Axes.Title.Label.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.Label.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.Label.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.TickLabelStyle.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.TickLabelStyle.FontName = "Malgun Gothic";
            plot.Clear();

            int total = originalFrames.Count;

            // ── 1. 데이터 배열 구성 (비삭제 프레임만 플롯) ──────────────────────────
            //    삭제된 프레임은 포인트를 추가하지 않아 선이 끊기게 됩니다.
            //    대신 연속된 삭제 구간은 VerticalSpan으로 표시합니다.
            var angleXs = new List<double>(total);
            var angleYs = new List<double>(total);
            var throttleXs = new List<double>(total);
            var throttleYs = new List<double>(total);

            for (int i = 0; i < total; i++)
            {
                var f = originalFrames[i];
                if (!f.IsDeleted)
                {
                    angleXs.Add(i); angleYs.Add(f.Angle);
                    throttleXs.Add(i); throttleYs.Add(f.Throttle);
                }
            }

            // ── 2. Series 추가 ────────────────────────────────────────────────────
            if (angleXs.Count > 0)
            {
                var sigAngle = plot.Add.SignalXY(angleXs.ToArray(), angleYs.ToArray());
                sigAngle.Color = ScottPlot.Color.FromHex("#4FC3F7");  // 하늘색
                sigAngle.LineWidth = 1.5f;
                sigAngle.LegendText = "조향(Angle)";
            }

            if (throttleXs.Count > 0)
            {
                var sigThrottle = plot.Add.SignalXY(throttleXs.ToArray(), throttleYs.ToArray());
                sigThrottle.Color = ScottPlot.Color.FromHex("#81C784");  // 연초록
                sigThrottle.LineWidth = 1.5f;
                sigThrottle.LegendText = "스로틀(Throttle)";
            }

            // ── 3. 삭제 구간 VerticalSpan (붉은 반투명 세로 영역) ──────────────────
            //    연속된 IsDeleted 구간을 탐지하여 한 번에 하나의 Span으로 추가합니다.
            //    X축 = 프레임 인덱스(0, 1, 2...) 이므로 startIdx ~ endIdx 그대로 사용.
            bool inDeleted = false;
            int spanStart = 0;
            bool anyDeleted = false;

            for (int i = 0; i <= total; i++)
            {
                bool deleted = i < total && originalFrames[i].IsDeleted;

                if (deleted && !inDeleted)
                {
                    // 새 삭제 구간 시작
                    spanStart = i;
                    inDeleted = true;
                }
                else if (!deleted && inDeleted)
                {
                    // 구간 종료: [spanStart, i-1] 범위를 세로 영역으로 표시
                    // VerticalSpan(xMin, xMax) — X축 기준 세로 영역
                    var span = plot.Add.VerticalSpan(spanStart, i - 1);
                    // WithAlpha(byte) : 0~255 범위. 50 ≈ 20% 불투명도
                    span.FillStyle.Color = ScottPlot.Colors.Red.WithAlpha(50);
                    span.LineStyle.Width = 0;   // 테두리 선 숨김
                    if (!anyDeleted)
                    {
                        span.LegendText = "삭제된 구간";  // 범례에는 첫 구간만 표시
                        anyDeleted = true;
                    }
                    inDeleted = false;
                }
            }

            // ── 4. 축 레이블 및 범례 ─────────────────────────────────────────────
            plot.XLabel("프레임 인덱스");
            plot.YLabel("값 (Angle / Throttle)");
            plot.Title("조향값·스로틀 분포 그래프");
            plot.Axes.SetLimitsY(-1.2, 1.2);
            plot.ShowLegend(Alignment.UpperRight);

            // 눈금선 색 (다크 배경 대응)
            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#444444");

            _formsPlot.Refresh();
            _chartDirty = false;
        }
    }
}
