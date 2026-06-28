using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShieldText.Lite.Services;

namespace ShieldText.Lite
{
    public partial class Form1 : Form
    {
        private readonly CryptoService _crypto = new CryptoService();
        private TextBox txtInput, txtOutput, txtPassword;
        private Button btnEncrypt, btnDecrypt, btnCopy, btnSwap, btnClear, btnSave;
        private Label lblStatus, lblPowered;
        private CheckBox chkShowPassword;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            const int W = 660;
            const int M = 20;
            const int work = W - M * 2;   // 620
            const int gap = 8;
            const int boxW = (work - gap) / 2;  // 306

            this.Text = "ShieldText Lite";
            this.Size = new Size(W, 500);
            this.MinimumSize = new Size(W, 500);
            this.MaximumSize = new Size(W, 500);
            this.BackColor = Color.FromArgb(15, 17, 23);
            this.ForeColor = Color.FromArgb(240, 242, 255);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);

            // ── Header ───────────────────────────────────────
            var lblTitle = new Label
            {
                Text = "ShieldText Lite",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(240, 242, 255),
                AutoSize = true,
                Location = new Point(M, 20)
            };
            var lblSub = new Label
            {
                Text = "AES-256 Encryption",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = true,
                Location = new Point(M, 42)
            };

            // ── Password Row ─────────────────────────────────
            var lblPass = new Label
            {
                Text = "Password",
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                Location = new Point(M, 68)
            };
            txtPassword = new TextBox
            {
                UseSystemPasswordChar = true,
                Location = new Point(M, 84),
                Width = 440,
                BackColor = Color.FromArgb(19, 21, 30),
                ForeColor = Color.FromArgb(240, 242, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9)
            };
            chkShowPassword = new CheckBox
            {
                Text = "Show",
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(466, 87),
                Width = 60,
                AutoSize = false
            };
            chkShowPassword.CheckedChanged += (s, e) =>
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

            btnClear = new Button
            {
                Text = "Clear All",
                Location = new Point(530, 83),
                Size = new Size(110, 26),
                BackColor = Color.FromArgb(26, 29, 39),
                ForeColor = Color.FromArgb(107, 114, 128),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(42, 45, 58);
            btnClear.Click += BtnClear_Click;

            // ── Labels + Copy/Save ───────────────────────────
            var lblInput = new Label
            {
                Text = "INPUT",
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Location = new Point(M, 122)
            };
            var lblOutput = new Label
            {
                Text = "OUTPUT",
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Location = new Point(M + boxW + gap, 122)
            };

            btnCopy = new Button
            {
                Text = "Copy",
                Location = new Point(M + work - 118, 119),
                Size = new Size(56, 18),
                BackColor = Color.FromArgb(26, 29, 39),
                ForeColor = Color.FromArgb(107, 114, 128),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 7f)
            };
            btnCopy.FlatAppearance.BorderColor = Color.FromArgb(42, 45, 58);
            btnCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(txtOutput.Text))
                { Clipboard.SetText(txtOutput.Text); SetStatus("Copied to clipboard", true); }
            };

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(M + work - 58, 119),
                Size = new Size(58, 18),
                BackColor = Color.FromArgb(26, 29, 39),
                ForeColor = Color.FromArgb(107, 114, 128),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 7f)
            };
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(42, 45, 58);
            btnSave.Click += BtnSave_Click;

            // ── Text Areas ───────────────────────────────────
            txtInput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(M, 142),
                Size = new Size(boxW, 220),
                BackColor = Color.FromArgb(19, 21, 30),
                ForeColor = Color.FromArgb(240, 242, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9)
            };
            txtOutput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Location = new Point(M + boxW + gap, 142),
                Size = new Size(boxW, 220),
                BackColor = Color.FromArgb(19, 21, 30),
                ForeColor = Color.FromArgb(240, 242, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9)
            };

            // ── Action Buttons ───────────────────────────────
            // encW + gap + swapW + gap + decW = work = 620
            // 229  +  6  +  150  +  6  + 229  = 620
            const int btnH = 36;
            const int btnY = 374;
            const int encW = 229;
            const int swapW = 150;
            const int decW = 229;
            const int btnGap = 6;

            btnEncrypt = new Button
            {
                Text = "Encrypt >>",
                Location = new Point(M, btnY),
                Size = new Size(encW, btnH),
                BackColor = Color.FromArgb(79, 142, 247),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEncrypt.FlatAppearance.BorderSize = 0;
            btnEncrypt.Click += BtnEncrypt_Click;

            btnSwap = new Button
            {
                Text = "<< Swap >>",
                Location = new Point(M + encW + btnGap, btnY),
                Size = new Size(swapW, btnH),
                BackColor = Color.FromArgb(26, 29, 39),
                ForeColor = Color.FromArgb(107, 114, 128),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5f)
            };
            btnSwap.FlatAppearance.BorderColor = Color.FromArgb(42, 45, 58);
            btnSwap.Click += (s, e) =>
            { var t = txtInput.Text; txtInput.Text = txtOutput.Text; txtOutput.Text = t; };

            btnDecrypt = new Button
            {
                Text = "<< Decrypt",
                Location = new Point(M + encW + btnGap + swapW + btnGap, btnY),
                Size = new Size(decW, btnH),
                BackColor = Color.FromArgb(61, 220, 151),
                ForeColor = Color.FromArgb(15, 17, 23),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDecrypt.FlatAppearance.BorderSize = 0;
            btnDecrypt.Click += BtnDecrypt_Click;

            // ── Status Bar ───────────────────────────────────
            var statusPanel = new Panel
            {
                Location = new Point(M, 422),
                Size = new Size(work, 28),
                BackColor = Color.FromArgb(26, 29, 39)
            };
            lblStatus = new Label
            {
                Text = string.Empty,
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Size = new Size(300, 28),
                Location = new Point(8, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8)
            };
            lblPowered = new Label
            {
                Text = "Powered by Nader Asadpour",
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Size = new Size(296, 28),
                Location = new Point(312, 0),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 7.5f)
            };
            statusPanel.Controls.AddRange(new Control[] { lblStatus, lblPowered });

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblSub,
                lblPass, txtPassword, chkShowPassword, btnClear,
                lblInput, lblOutput, btnCopy, btnSave,
                txtInput, txtOutput,
                btnEncrypt, btnSwap, btnDecrypt,
                statusPanel
            });
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            try { txtOutput.Text = _crypto.Encrypt(txtInput.Text, txtPassword.Text); SetStatus("Encrypted successfully", true); }
            catch { SetStatus("Encryption failed", false); }
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            try { txtOutput.Text = _crypto.Decrypt(txtInput.Text, txtPassword.Text); SetStatus("Decrypted successfully", true); }
            catch { SetStatus("Wrong password or invalid input", false); }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtInput.Text = string.Empty;
            txtOutput.Text = string.Empty;
            txtPassword.Text = string.Empty;
            lblStatus.Text = string.Empty;
            lblStatus.ForeColor = Color.FromArgb(107, 114, 128);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutput.Text)) { SetStatus("Nothing to save", false); return; }
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Save Output";
                dialog.Filter = "Text File (*.txt)|*.txt";
                dialog.FileName = $"shieldtext_{DateTime.Now:yyyyMMdd_HHmmss}";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dialog.FileName, txtOutput.Text);
                    SetStatus($"Saved: {Path.GetFileName(dialog.FileName)}", true);
                }
            }
        }

        private new bool Validate()
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text)) { SetStatus("Please enter text", false); return false; }
            if (string.IsNullOrWhiteSpace(txtPassword.Text)) { SetStatus("Please enter password", false); return false; }
            return true;
        }

        private void SetStatus(string message, bool success)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = success
                ? Color.FromArgb(61, 220, 151)
                : Color.FromArgb(239, 68, 68);
        }
    }
}