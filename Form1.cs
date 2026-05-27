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

        private string _currentFolderPath = "";

        private FormsPlot? _formsPlot;
        private bool _chartDirty = true;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            playTimer = new System.Windows.Forms.Timer();
            playTimer.Tick += PlayTimer_Tick;

            // ensure status strip is docked
            if (statusStripDataViewer != null && !this.Controls.Contains(statusStripDataViewer))
            {
                statusStripDataViewer.Dock = DockStyle.Bottom;
                this.Controls.Add(statusStripDataViewer);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnClearFilter.Click += BtnClearFilter_Click;
            btnExcludeSelectedFrame.Click += BtnRepair_Click;
            btnRestoreFrame.Click += BtnReloadTub_Click;

            mnuOpenDataFolder.Click += (s, _) => btnOpenFolder_Click(s!, EventArgs.Empty);
            mnuReloadData.Click += (s, _) => btnReload_Click(s!, EventArgs.Empty);
            mnuExit.Click += (s, _) => Application.Exit();
            mnuOpenGuide.Click += (s, _) => btnGuide_Click(s!, EventArgs.Empty);

            tabControlMain.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

            btnExcludeSelectedFrame.Text = "선택 프레임 제외";
            btnRestoreFrame.Text = "복구";
            txtAngleMin.Text = "-1";
            txtAngleMax.Text = "1";
            txtThrottleMin.Text = "0";
            txtThrottleMax.Text = "1";

            UpdateStatusLabels();
        }

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
                if (string.IsNullOrEmpty(toolStripStatusLabelPath.Text) || toolStripStatusLabelPath.Text == "\uACBD\uB85C: -") return;
                path = toolStripStatusLabelPath.Text.Replace("\uACBD\uB85C: ", "").Trim();
            }
            if (Directory.Exists(path)) _ = LoadCatalogAsync(path);
        }

        private void btnToggleTheme_Click(object sender, EventArgs e) => ToggleTheme();

        private void btnGuide_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Data Manager \uC0AC\uC6A9 \uC21C\uC11C\n\n" +
                "1. \uB370\uC774\uD130 \uBDF0\uC5B4 \uD0ED\uC5D0\uC11C data \uD3F4\uB354\uB97C \uC5FD\uB2C8\uB2E4.\n" +
                "2. \uC774\uBBF8\uC9C0, Angle, Throttle \uAC12\uC744 \uD655\uC778\uD569\uB2C8\uB2E4.\n" +
                "3. \uD544\uD130\uB85C \uD559\uC2B5 \uB370\uC774\uD130 \uD488\uC9C8\uC744 \uC810\uAC80\uD569\uB2C8\uB2E4.\n" +
                "4. \uCD5C\uC19F\uAC12/\uCD5C\uB313\uAC12\uC5D0 \uAD6C\uAC04 \uD504\uB808\uC784 \uC778\uB371\uC2A4\uB97C \uC785\uB825\uD55C \uB4A4 \uAD6C\uAC04 \uC0AD\uC81C\uB97C \uC2E4\uD589\uD569\uB2C8\uB2E4.\n" +
                "5. \uC804\uCCB4 \uBCF5\uC6D0/\uC0C8\uB85C\uACE0\uCE68\uC73C\uB85C \uC0AD\uC81C \uD45C\uC2DC\uB97C \uBCF5\uC6D0\uD560 \uC218 \uC788\uC2B5\uB2C8\uB2E4.\n" +
                "6. \uADF8\uB798\uD504/\uD1B5\uACC4 \uD0ED\uC5D0\uC11C Angle\u00B7Throttle \uBD84\uD3EC\uB97C \uC2DC\uAC01\uD654\uD569\uB2C8\uB2E4.",
                "\uB2E8\uACC4\uBCC4 \uAC00\uC774\uB4DC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnApplyFilter_Click(object sender, EventArgs e) => ApplyFilter();

        private void BtnClearFilter_Click(object? sender, EventArgs e) => ClearFilter();

        private void BtnRepair_Click(object? sender, EventArgs e)
        {
            int originalIndex = GetSelectedOriginalIndex();
            if (originalIndex < 0)
            {
                MessageBox.Show(
                    "\uC0AD\uC81C\uD560 \uD504\uB808\uC784\uC744 \uBA3C\uC800 \uC120\uD0DD\uD574 \uC8FC\uC138\uC694.",
                    "\uC120\uD0DD \uD544\uC694", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"\uC120\uD0DD\uD55C \uD504\uB808\uC784({originalIndex})\uC744 Soft Delete \uCC98\uB9AC\uD569\uB2C8\uB2E4.\n\uACC4\uC18D\uD558\uC2DC\uACA0\uC2B5\uB2C8\uAE4C?",
                "\uC120\uD0DD \uD504\uB808\uC784 \uC81C\uC678", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            SoftDeleteRange(originalIndex, originalIndex);
        }

        private void BtnReloadTub_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "\uBAA8\uB4E0 Soft Delete \uD50C\uB798\uADF8\uB97C \uCD08\uAE30\uD654\uD558\uACE0 \uC804\uCCB4 \uB370\uC774\uD130\uB97C \uBCF5\uC6D0\uD569\uB2C8\uB2E4.\n\uACC4\uC18D\uD558\uC2DC\uACA0\uC2B5\uB2C8\uAE4C?",
                "\uC804\uCCB4 \uBCF5\uC6D0", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            RestoreAll();
        }
        private void btnAutoPlay_Click(object sender, EventArgs e) => TogglePlayPause();

        private void lstFrameData_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstFrameData.SelectedItem == null) return;
            int idx = lstFrameData.SelectedIndex;
            if (idx >= 0 && idx < filteredFrames.Count)
                SetIndex(idx);
        }

        private void trkFramePosition_Scroll(object sender, EventArgs e)
        {
            int idx = trkFramePosition.Value;
            if (idx >= 0 && idx < filteredFrames.Count)
                SetIndex(idx);
        }

        private void btnFirst_Click(object sender, EventArgs e) => SetIndex(0);
        private void btnPrev_Click(object sender, EventArgs e) => SetIndex(Math.Max(0, currentIndex - 1));
        private void btnNext_Click(object sender, EventArgs e) => SetIndex(Math.Min(filteredFrames.Count - 1, currentIndex + 1));
        private void btnLast_Click(object sender, EventArgs e) => SetIndex(filteredFrames.Count - 1);

        private void PlayTimer_Tick(object? sender, EventArgs e)
        {
            if (filteredFrames == null || filteredFrames.Count == 0) return;
            int next = currentIndex + 1;
            while (next < filteredFrames.Count && filteredFrames[next].IsDeleted)
                next++;
            if (next >= filteredFrames.Count)
            {
                playTimer.Stop();
                isPlaying = false;
                btnAutoPlay.Text = "\uC790\uB3D9 \uC7AC\uC0DD";
                return;
            }
            SetIndex(next);
        }

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


        private async Task LoadFolderAsync(string folder)
        {
            SetLoadingState(true);

            try
            {
                var frames = await Task.Run(() =>
                {
                    var imageExt = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
                    var images = Directory.EnumerateFiles(folder)
                        .Where(f => imageExt.Contains(Path.GetExtension(f).ToLowerInvariant()))
                        .OrderBy(f => f)
                        .ToList();

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
                            IsDeleted = false
                        });
                    }
                    return result;
                });

                originalFrames = frames;
                filteredFrames = new List<FrameData>(originalFrames);

                toolStripStatusLabelPath.Text = "\uACBD\uB85C: " + folder;
                _chartDirty = true;
                RefreshListBinding();
                SetIndex(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("\uD3F4\uB354 \uB85C\uB4DC \uC911 \uC624\uB958: " + ex.Message,
                    "\uC624\uB958", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void SetLoadingState(bool loading)
        {
            btnOpenFolder.Enabled = !loading;
            btnReload.Enabled = !loading;
            btnApplyFilter.Enabled = !loading;
            btnClearFilter.Enabled = !loading;
            btnExcludeSelectedFrame.Enabled = !loading;
            btnRestoreFrame.Enabled = !loading;
            this.Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
            if (loading) toolStripStatusLabelFrames.Text = "\uB85C\uB529 \uC911...";
        }

        private void RefreshListBinding()
        {
            lstFrameData.BeginUpdate();
            lstFrameData.DataSource = null;
            lstFrameData.DataSource = filteredFrames;
            lstFrameData.DisplayMember = "DisplayName";
            lstFrameData.EndUpdate();

            trkFramePosition.Minimum = 0;
            trkFramePosition.Maximum = Math.Max(0, filteredFrames.Count - 1);
            UpdateStatusLabels();
        }

        private void SetIndex(int idx)
        {
            if (filteredFrames == null || filteredFrames.Count == 0) return;
            idx = Math.Max(0, Math.Min(filteredFrames.Count - 1, idx));
            currentIndex = idx;

            lstFrameData.SelectedIndexChanged -= lstFrameData_SelectedIndexChanged;
            lstFrameData.SelectedIndex = idx;
            lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;

            if (trkFramePosition.Value != idx)
                trkFramePosition.Value = idx;

            var frame = filteredFrames[idx];

            string resolvedImagePath = ResolveImagePath(frame.Name);
            UpdatePreviewImage(resolvedImagePath);

            lblFrameValue.Text = $"Frame: {idx + 1} / {filteredFrames.Count}";
            lblAngleValue.Text = $"\uC870\uD5A5\uAC12: {frame.Angle:0.000}";
            lblThrottleValue.Text = $"\uC2A4\uB85C\uD2C0\uAC12: {frame.Throttle:0.000}";
            lblModeValue.Text = $"\uBAA8\uB4DC: {frame.Mode}";
            UpdateStatusLabels();
        }

        private string ResolveImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(_currentFolderPath))
                return string.Empty;

            string path1 = Path.Combine(_currentFolderPath, fileName);
            if (File.Exists(path1)) return path1;

            string path2 = Path.Combine(_currentFolderPath, "images", fileName);
            if (File.Exists(path2)) return path2;

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
                using var fs = File.OpenRead(path);
                using var img = System.Drawing.Image.FromStream(fs);
                var bmp = new Bitmap(img);

                var old = picMainPreview.Image;
                picMainPreview.Image = bmp;
                old?.Dispose();
            }
            catch {  }
        }

        private void TogglePlayPause()
        {
            if (isPlaying)
            {
                playTimer.Stop();
                isPlaying = false;
                btnAutoPlay.Text = "\uC790\uB3D9 \uC7AC\uC0DD";
            }
            else
            {
                playTimer.Interval = (int)numPlaybackInterval.Value;
                playTimer.Start();
                isPlaying = true;
                btnAutoPlay.Text = "\uC77C\uC2DC\uC815\uC9C0";
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

        private void ApplyFilter()
        {
            if (originalFrames == null || originalFrames.Count == 0) return;

            if (!TryReadFilterRanges(out double angleMin, out double angleMax, out double throttleMin, out double throttleMax))
                return;

            filteredFrames = originalFrames
                .Where(frame => frame.IsDeleted ||
                    (frame.Angle >= angleMin && frame.Angle <= angleMax &&
                     frame.Throttle >= throttleMin && frame.Throttle <= throttleMax))
                .ToList();

            _chartDirty = true;
            RefreshListBinding();
            SetIndex(0);
        }

        private bool TryReadFilterRanges(out double angleMin, out double angleMax, out double throttleMin, out double throttleMax)
        {
            angleMin = angleMax = throttleMin = throttleMax = 0;

            bool isValid =
                double.TryParse(txtAngleMin.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out angleMin) &&
                double.TryParse(txtAngleMax.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out angleMax) &&
                double.TryParse(txtThrottleMin.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMin) &&
                double.TryParse(txtThrottleMax.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out throttleMax);

            if (!isValid)
            {
                MessageBox.Show(
                    "\uC870\uD5A5\uAC01\uACFC \uC2A4\uB85C\uD2C0 \uBC94\uC704\uB294 \uC22B\uC790\uB85C \uC785\uB825\uD574 \uC8FC\uC138\uC694.",
                    "\uC785\uB825 \uC624\uB958", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (angleMin > angleMax || throttleMin > throttleMax)
            {
                MessageBox.Show(
                    "\uBC94\uC704\uC758 \uCD5C\uC18C\uAC12\uC740 \uCD5C\uB300\uAC12\uBCF4\uB2E4 \uD074 \uC218 \uC5C6\uC2B5\uB2C8\uB2E4.",
                    "\uBC94\uC704 \uC624\uB958", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private int GetSelectedOriginalIndex()
        {
            if (lstFrameData.SelectedItem is not FrameData selectedFrame) return -1;
            return originalFrames.IndexOf(selectedFrame);
        }

        private void ClearFilter()
        {
            if (originalFrames == null) return;
            filteredFrames = new List<FrameData>(originalFrames);
            _chartDirty = true;
            RefreshListBinding();
            SetIndex(0);
        }

        private void SoftDeleteRange(int from, int to)
        {
            if (originalFrames == null || originalFrames.Count == 0) return;

            int safeFrom = Math.Max(0, from);
            int safeTo = Math.Min(originalFrames.Count - 1, to);

            if (safeFrom > safeTo)
            {
                MessageBox.Show(
                    $"\uC720\uD6A8\uD55C \uD504\uB808\uC784 \uBC94\uC704\uB97C \uBC97\uC5B4\uB0AC\uC2B5\uB2C8\uB2E4.\n" +
                    $"\uC785\uB825 \uBC94\uC704: {from} ~ {to} | \uC804\uCCB4 \uD504\uB808\uC784: {originalFrames.Count}",
                    "\uBC94\uC704 \uC624\uB958", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            filteredFrames = new List<FrameData>(originalFrames);
            _chartDirty = true;
            RefreshListBinding();

            int adjustedIdx = Math.Max(0, Math.Min(currentIndex, filteredFrames.Count - 1));
            if (filteredFrames.Count > 0)
                SetIndex(adjustedIdx);

            MessageBox.Show(
                $"\uC644\uB8CC: {newlyDeleted}\uAC1C \uD504\uB808\uC784\uC774 Soft Delete \uCC98\uB9AC\uB418\uC5C8\uC2B5\uB2C8\uB2E4.\n" +
                "(\uC774\uBBF8 \uC0AD\uC81C \uC0C1\uD0DC\uC600\uB358 \uD504\uB808\uC784\uC740 \uC81C\uC678)\n" +
                "'\uC804\uCCB4 \uBCF5\uC6D0/\uC0C8\uB85C\uACE0\uCE68' \uBC84\uD2BC\uC73C\uB85C \uC5B8\uC81C\uB4E0 \uBCF5\uC6D0 \uAC00\uB2A5\uD569\uB2C8\uB2E4.",
                "\uAD6C\uAC04 \uC0AD\uC81C \uC644\uB8CC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RestoreAll()
        {
            if (originalFrames == null) return;

            foreach (var f in originalFrames)
                f.IsDeleted = false;

            filteredFrames = new List<FrameData>(originalFrames);
            _chartDirty = true;
            RefreshListBinding();
            SetIndex(0);

            MessageBox.Show("\uBAA8\uB4E0 \uD504\uB808\uC784\uC774 \uBCF5\uC6D0\uB418\uC5C8\uC2B5\uB2C8\uB2E4.",
                "\uBCF5\uC6D0 \uC644\uB8CC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStatusLabels()
        {
            int total = originalFrames?.Count ?? 0;
            int deleted = originalFrames?.Count(f => f.IsDeleted) ?? 0;
            int shown = filteredFrames?.Count ?? 0;
            toolStripStatusLabelFrames.Text = $"\uC804\uCCB4: {total}  |  \uC0AD\uC81C\uB428: {deleted}  |  \uD45C\uC2DC \uC911: {shown}";
        }


        private class FrameData
        {
            public string ImagePath { get; set; } = string.Empty;
            public double Angle { get; set; }
            public double Throttle { get; set; }
            public string Mode { get; set; } = "-";
            public string Name { get; set; } = string.Empty;

            public bool IsDeleted { get; set; } = false;

            public bool IsCatalogMissing { get; set; } = false;

            public bool HasNoData { get; set; } = false;

            public string DisplayName
            {
                get
                {
                    if (IsCatalogMissing)
                        return $"{Name} | [\uCE74\uD0C8\uB85C\uADF8 \uD30C\uC77C \uC5C6\uC74C]";
                    if (HasNoData)
                        return $"{Name} | catalog \uD30C\uC77C\uC740 \uC788\uC9C0\uB9CC \uB9E4\uCE6D\uB41C \uAC12\uC774 \uC5C6\uC2B5\uB2C8\uB2E4.";

                    string dir;
                    if (Throttle < 0)
                        dir = "\uD6C4\uC9C4";
                    else if (Angle < -0.1)
                        dir = "\uC88C\uD68C\uC804";
                    else if (Angle > 0.1)
                        dir = "\uC6B0\uD68C\uC804";
                    else
                        dir = "\uC9C1\uC9C4";

                    string body = $"{Name} | {dir} (\uC870\uD5A5: {Angle:0.00}, \uC2A4\uB85C\uD2C0: {Throttle:0.00})";
                    return IsDeleted ? $"[X] {body}" : body;
                }
            }
        }


        private List<FrameData> LoadMockData(string folderPath)
        {
            _currentFolderPath = folderPath;

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
                var existing = new HashSet<string>(jpgFiles, StringComparer.OrdinalIgnoreCase);
                foreach (var f in Directory.EnumerateFiles(imagesSubDir, "*.jpg"))
                {
                    string name = Path.GetFileName(f)!;
                    if (!existing.Contains(name))
                        jpgFiles.Add(name);
                }
            }

            jpgFiles.Sort((a, b) =>
            {
                string numA = new string(a.TakeWhile(char.IsDigit).ToArray());
                string numB = new string(b.TakeWhile(char.IsDigit).ToArray());
                if (int.TryParse(numA, out int nA) && int.TryParse(numB, out int nB))
                    return nA.CompareTo(nB);
                return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            });

            if (jpgFiles.Count == 0)
                return new List<FrameData>();

            string catalogPath = Path.Combine(folderPath, "catalog_0.catalog");
            bool catalogExists = File.Exists(catalogPath);

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

                            string mode = "-";
                            if (root.TryGetProperty("user/mode", out var modeEl))
                                mode = modeEl.GetString() ?? "-";

                            catalogMap[imageName] = (angle, throttle, mode);
                        }
                        catch {  }
                    }
                }
                catch {  }
            }

            var result = new List<FrameData>(jpgFiles.Count);

            foreach (var imgName in jpgFiles)
            {
                var frame = new FrameData
                {
                    Name = imgName,
                    ImagePath = Path.Combine(folderPath, imgName),
                    IsDeleted = false
                };

                if (!catalogExists)
                {
                    frame.IsCatalogMissing = true;
                }
                else if (catalogMap.TryGetValue(imgName, out var data))
                {
                    frame.Angle = data.angle;
                    frame.Throttle = data.throttle;
                    frame.Mode = data.mode;
                }
                else
                {
                    frame.HasNoData = true;
                }

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
                        "\uC120\uD0DD\uD55C \uD3F4\uB354\uC5D0\uC11C .jpg \uC774\uBBF8\uC9C0 \uD30C\uC77C\uC744 \uCC3E\uC744 \uC218 \uC5C6\uC2B5\uB2C8\uB2E4.\n" +
                        "\uC774\uBBF8\uC9C0\uAC00 \uD3F4\uB354 \uB8E8\uD2B8 \uB610\uB294 \uD558\uC704 'images' \uD3F4\uB354\uC5D0 \uC788\uB294\uC9C0 \uD655\uC778\uD574 \uC8FC\uC138\uC694.",
                        "\uC774\uBBF8\uC9C0 \uC5C6\uC74C", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                originalFrames = frames;
                filteredFrames = new List<FrameData>(originalFrames);

                toolStripStatusLabelPath.Text = "\uACBD\uB85C: " + folder;
                _chartDirty = true;
                RefreshListBinding();
                SetIndex(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("\uB370\uC774\uD130 \uB85C\uB4DC \uC911 \uC624\uB958: " + ex.Message,
                    "\uC624\uB958", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

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


        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageGraphStats && _chartDirty)
                RenderChart();
        }

        private void InitFormsPlot()
        {
            _formsPlot = new FormsPlot();

            _formsPlot.Location = new System.Drawing.Point(0, 42);
            _formsPlot.Size = new System.Drawing.Size(
                tabPageGraphStats.ClientSize.Width,
                tabPageGraphStats.ClientSize.Height - 42);
            _formsPlot.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                              | AnchorStyles.Left | AnchorStyles.Right;
            _formsPlot.Name = "formsPlotMain";

            _formsPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1e1e1e");
            _formsPlot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#2d2d30");

            tabPageGraphStats.Controls.Add(_formsPlot);
            _formsPlot.BringToFront();
        }

        private void RenderChart()
        {
            if (originalFrames == null || originalFrames.Count == 0)
            {
                _chartDirty = false;
                return;
            }

            if (_formsPlot == null)
                InitFormsPlot();

            var plot = _formsPlot!.Plot;
            _formsPlot.Plot.Axes.Title.Label.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.Label.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.Label.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Bottom.TickLabelStyle.FontName = "Malgun Gothic";
            _formsPlot.Plot.Axes.Left.TickLabelStyle.FontName = "Malgun Gothic";
            plot.Clear();

            int total = originalFrames.Count;

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

            if (angleXs.Count > 0)
            {
                var sigAngle = plot.Add.SignalXY(angleXs.ToArray(), angleYs.ToArray());
                sigAngle.Color = ScottPlot.Color.FromHex("#4FC3F7");
                sigAngle.LineWidth = 1.5f;
                sigAngle.LegendText = "\uC870\uD5A5(Angle)";
            }

            if (throttleXs.Count > 0)
            {
                var sigThrottle = plot.Add.SignalXY(throttleXs.ToArray(), throttleYs.ToArray());
                sigThrottle.Color = ScottPlot.Color.FromHex("#81C784");
                sigThrottle.LineWidth = 1.5f;
                sigThrottle.LegendText = "\uC2A4\uB85C\uD2C0(Throttle)";
            }

            bool inDeleted = false;
            int spanStart = 0;
            bool anyDeleted = false;

            for (int i = 0; i <= total; i++)
            {
                bool deleted = i < total && originalFrames[i].IsDeleted;

                if (deleted && !inDeleted)
                {
                    spanStart = i;
                    inDeleted = true;
                }
                else if (!deleted && inDeleted)
                {
                    var span = plot.Add.VerticalSpan(spanStart, i - 1);
                    span.FillStyle.Color = ScottPlot.Colors.Red.WithAlpha(50);
                    span.LineStyle.Width = 0;
                    if (!anyDeleted)
                    {
                        span.LegendText = "\uC0AD\uC81C\uB41C \uAD6C\uAC04";
                        anyDeleted = true;
                    }
                    inDeleted = false;
                }
            }

            plot.XLabel("프레임 인덱스");
            plot.YLabel("값 (Angle / Throttle)");
            plot.Title("조향값/스로틀 분포 그래프");
            plot.Axes.SetLimitsY(-1.2, 1.2);
            plot.ShowLegend(Alignment.UpperRight);

            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#444444");

            _formsPlot.Refresh();
            _chartDirty = false;
        }
    }
}
