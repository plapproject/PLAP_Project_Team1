using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

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

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            playTimer = new System.Windows.Forms.Timer();
            playTimer.Tick += PlayTimer_Tick;
            // interval will be taken from numPlaybackInterval when playing

            // ensure status strip is added
            if (statusStripDataViewer != null && !this.Controls.Contains(statusStripDataViewer))
            {
                statusStripDataViewer.Dock = DockStyle.Bottom;
                this.Controls.Add(statusStripDataViewer);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // initial state
            UpdateStatusLabels();
        }

        // --- UI event handlers ---
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            string folder = dlg.SelectedPath;
            LoadFolder(folder);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripStatusLabelPath.Text) || toolStripStatusLabelPath.Text == "Í≤ΩÎ°ú: -") return;
            string path = toolStripStatusLabelPath.Text.Replace("Í≤ΩÎ°ú: ", "").Trim();
            if (Directory.Exists(path)) LoadFolder(path);
        }

        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            ToggleTheme();
        }

        private void btnGuide_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Data Manager ?¨Ïö© ?úÏÑú\n\n1. ?∞Ïù¥??Î∑∞Ïñ¥ ??óê??data ?¥ÎçîÎ•??ΩÎãà??\n2. ?¥Î?ÏßÄ, Angle, Throttle Í∞íÏùÑ ?ïÏù∏?©Îãà??\n3. Throttle > 0 ?êÎäî Angle Î≤îÏúÑ ?ÑÌÑ∞Î°??ôÏäµ ?àÏßà???êÍ??©Îãà??\n4. Î∂àÎüâ ?ÑÎ†à?ÑÏ? ??†ú ??catalogÎ•??Ä?•Ìï©?àÎã§.\n5. ?ôÏäµ ?§Ìñâ ??óê??Python Í≤ΩÎ°ú?Ä mycar Í≤ΩÎ°úÎ•?ÏßÄ?ïÌï©?àÎã§.\n6. ?ôÏäµ ?úÏûë???åÎü¨ train.py Î°úÍ∑∏Î•??ïÏù∏?©Îãà??",
            "?®Í≥ÑÎ≥?Í∞Ä?¥Îìú", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void lstFrameData_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int idx = lstFrameData.SelectedIndex;
            if (idx >= 0 && idx < filteredFrames.Count)
            {
                SetIndex(idx);
            }
        }

        private void trkFramePosition_Scroll(object sender, EventArgs e)
        {
            int idx = trkFramePosition.Value;
            if (idx >= 0 && idx < filteredFrames.Count)
            {
                SetIndex(idx);
            }
        }

        private void btnFirst_Click(object sender, EventArgs e) => SetIndex(0);
        private void btnPrev_Click(object sender, EventArgs e) => SetIndex(Math.Max(0, currentIndex - 1));
        private void btnNext_Click(object sender, EventArgs e) => SetIndex(Math.Min(filteredFrames.Count - 1, currentIndex + 1));
        private void btnLast_Click(object sender, EventArgs e) => SetIndex(filteredFrames.Count - 1);

        // --- playback timer ---
        private void PlayTimer_Tick(object? sender, EventArgs e)
        {
            if (filteredFrames.Count == 0) return;
            int next = currentIndex + 1;
            if (next >= filteredFrames.Count)
            {
                playTimer.Stop();
                isPlaying = false;
                return;
            }
            SetIndex(next);
        }

        // --- keyboard shortcuts ---
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                btnNext_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                btnPrev_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Space)
            {
                TogglePlayPause();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Home)
            {
                btnFirst_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.End)
            {
                btnLast_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        // --- core logic ---
        private void LoadFolder(string folder)
        {
            try
            {
                var imageExt = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
                var images = Directory.EnumerateFiles(folder)
                    .Where(f => imageExt.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .OrderBy(f => f)
                    .ToList();

                // try read a CSV mapping or individual JSONs
                var csvFiles = Directory.GetFiles(folder, "*.csv");
                Dictionary<string, (double angle, double throttle, string mode)> csvMap = new Dictionary<string, (double, double, string)>();
                if (csvFiles.Length > 0)
                {
                    // load first csv
                    try
                    {
                        var lines = File.ReadAllLines(csvFiles[0]);
                        foreach (var line in lines)
                        {
                            var parts = line.Split(',');
                            if (parts.Length >= 4)
                            {
                                var name = parts[0].Trim();
                                if (double.TryParse(parts[1], out double a) && double.TryParse(parts[2], out double t))
                                {
                                    var m = parts[3].Trim();
                                    csvMap[name] = (a, t, m);
                                }
                            }
                        }
                    }
                    catch { }
                }

                originalFrames.Clear();
                foreach (var img in images)
                {
                    var baseName = Path.GetFileName(img);
                    double angle = 0, throttle = 0; string mode = "-";
                    // per-image json
                    var jsonPath = Path.Combine(folder, Path.GetFileNameWithoutExtension(img) + ".json");
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
                    else if (csvMap.TryGetValue(baseName, out var tuple))
                    {
                        angle = tuple.angle; throttle = tuple.throttle; mode = tuple.mode;
                    }

                    originalFrames.Add(new FrameData
                    {
                        ImagePath = img,
                        Angle = angle,
                        Throttle = throttle,
                        Mode = mode,
                        Name = baseName
                    });
                }

                // default filtered = original
                filteredFrames = new List<FrameData>(originalFrames);
                RefreshListBinding();
                toolStripStatusLabelPath.Text = "Í≤ΩÎ°ú: " + folder;
                UpdateStatusLabels();
                SetIndex(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("?¥Îçî Î°úÎìú Ï§??§Î•ò: " + ex.Message);
            }
        }

        private void RefreshListBinding()
        {
            lstFrameData.BeginUpdate();
            lstFrameData.Items.Clear();
            for (int i = 0; i < filteredFrames.Count; i++)
            {
                var f = filteredFrames[i];
                lstFrameData.Items.Add($"{i:0000} - {f.Name}");
            }
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
            // update selection without triggering events
            lstFrameData.SelectedIndexChanged -= lstFrameData_SelectedIndexChanged;
            lstFrameData.SelectedIndex = idx;
            lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;

            if (trkFramePosition.Value != idx)
            {
                trkFramePosition.Value = idx;
            }

            var frame = filteredFrames[idx];
            UpdatePreviewImage(frame.ImagePath);
            lblFrameValue.Text = $"Frame: {idx + 1}/{filteredFrames.Count}";
            lblAngleValue.Text = $"Angle: {frame.Angle:0.000}";
            lblThrottleValue.Text = $"Throttle: {frame.Throttle:0.000}";
            lblModeValue.Text = $"Mode: {frame.Mode}";
            UpdateStatusLabels();
        }

        private void UpdatePreviewImage(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    picMainPreview.Image?.Dispose();
                    picMainPreview.Image = null;
                    return;
                }

                using var fs = File.OpenRead(path);
                var img = Image.FromStream(fs);
                // clone to release file handle immediately
                var bmp = new Bitmap(img);
                img.Dispose();

                var old = picMainPreview.Image;
                picMainPreview.Image = bmp;
                old?.Dispose();
            }
            catch
            {
                // ignore image load errors
            }
        }

        private void TogglePlayPause()
        {
            if (isPlaying)
            {
                playTimer.Stop();
                isPlaying = false;
            }
            else
            {
                playTimer.Interval = (int)numPlaybackInterval.Value;
                playTimer.Start();
                isPlaying = true;
            }
        }

        private void ToggleTheme()
        {
            isDarkTheme = !isDarkTheme;
            Color back = isDarkTheme ? Color.FromArgb(45, 45, 48) : SystemColors.Control;
            Color fore = isDarkTheme ? Color.White : SystemColors.ControlText;
            this.BackColor = back;
            foreach (Control c in this.Controls)
            {
                ApplyThemeToControl(c, back, fore);
            }
            // ensure status strip colors
            statusStripDataViewer.BackColor = back;
            statusStripDataViewer.ForeColor = fore;
        }

        private void ApplyThemeToControl(Control c, Color back, Color fore)
        {
            try
            {
                // don't override complex controls too aggressively
                if (c is StatusStrip) return;
                c.BackColor = back;
                c.ForeColor = fore;
                foreach (Control child in c.Controls)
                {
                    ApplyThemeToControl(child, back, fore);
                }
            }
            catch { }
        }

        private void ApplyFilter()
        {
            if (originalFrames == null || originalFrames.Count == 0) return;

            double minValue = (double)numFilterMin.Value;
            double maxValue = (double)numFilterMax.Value;

            if (minValue > maxValue)
            {
                MessageBox.Show(
                    "√÷º⁄∞™¿∫ √÷¥Ò∞™∫∏¥Ÿ ≈¨ ºˆ æ¯Ω¿¥œ¥Ÿ. « ≈Õ π¸¿ß∏¶ ¥ŸΩ√ »Æ¿Œ«ÿ ¡÷ººø‰.",
                    "« ≈Õ π¸¿ß ø¿∑˘",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // «ˆ¿Á »≠∏Èø°¥¬ √÷º⁄∞™/√÷¥Ò∞™ ¿‘∑¬∏∏ ¿÷¿∏π«∑Œ Ω∫∑Œ∆≤ π¸¿ß ±‚¡ÿ¿∏∑Œ « ≈Õ∏µ«’¥œ¥Ÿ.
            filteredFrames = originalFrames.Where(f =>
                f.Throttle >= minValue && f.Throttle <= maxValue).ToList();

            RefreshListBinding();
            SetIndex(0);
        }
        private void UpdateStatusLabels()
        {
            toolStripStatusLabelFrames.Text = $"Frames: {originalFrames?.Count ?? 0}";
        }

        // --- data model ---
        private class FrameData
        {
            public string ImagePath { get; set; } = string.Empty;
            public double Angle { get; set; }
            public double Throttle { get; set; }
            public string Mode { get; set; } = "-";
            public string Name { get; set; } = string.Empty;
        }

        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPageTraining_Click(object sender, EventArgs e)
        {

        }

        private void mnuHelp_Click(object sender, EventArgs e)
        {

        }

        private void mnuOpenGraphStats_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageGraphStats;
        }
        private void btnAutoPlay_Click(object sender, EventArgs e)
        {

        }

        private void lblModeValue_Click(object sender, EventArgs e)
        {

        }

        private void lblAngleValue_Click(object sender, EventArgs e)
        {

        }

        private void lblPlayInterval_Click(object sender, EventArgs e)
        {

        }

        private void lblThrottleValue_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabelTraining_Click(object sender, EventArgs e)
        {

        }

        private void btnMycarPath_Click(object sender, EventArgs e)
        {

        }
    }
}
