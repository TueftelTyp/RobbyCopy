using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

public class RoboCopyGui : Form
{
    [DllImport("kernel32.dll")]
    private static extern uint GetOEMCP();

    private TextBox txtSource;
    private TextBox txtTarget;
    private Button btnBrowseSource;
    private Button btnBrowseTarget;
    private Button btnSwap;

    private GroupBox grpOptions;
    private CheckBox chkSubDirs;
    private CheckBox chkEmptyDirs;
    private CheckBox chkMirror;
    private CheckBox chkNoOverwrite;
    private CheckBox chkOnlyNewer;
    private CheckBox chkPermissions;
    private CheckBox chkTestRun;

    private Label lblRetries;
    private NumericUpDown numRetries;
    private Label lblWait;
    private NumericUpDown numWait;

    private ProgressBar progressBar;
    private Label lblStatus;

    private Button btnStart;
    private Button btnCancel;
    private Button btnSaveLog;

    private TextBox txtLog;

    private Process roboProcess;
    private volatile bool isRunning;
    private volatile bool cancelRequested;

    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new RoboCopyGui());
    }

    public RoboCopyGui()
    {
        InitializeUi();
    }

    private void InitializeUi()
    {
        Text = "RobbyCopy";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(760, 640);

        Label lblSource = new Label
        {
            Text = "Quelle:",
            Location = new Point(12, 18),
            AutoSize = true
        };

        txtSource = new TextBox
        {
            Location = new Point(80, 15),
            Width = 470
        };

        btnBrowseSource = new Button
        {
            Text = "Durchsuchen...",
            Location = new Point(560, 14),
            Width = 100
        };

        Label lblTarget = new Label
        {
            Text = "Ziel:",
            Location = new Point(12, 48),
            AutoSize = true
        };

        txtTarget = new TextBox
        {
            Location = new Point(80, 45),
            Width = 470
        };

        btnBrowseTarget = new Button
        {
            Text = "Durchsuchen...",
            Location = new Point(560, 44),
            Width = 100
        };

        btnSwap = new Button
        {
            Text = "Quelle/Ziel tauschen",
            Location = new Point(560, 74),
            Width = 180
        };

        grpOptions = new GroupBox
        {
            Text = "Optionen",
            Location = new Point(12, 105),
            Size = new Size(736, 160)
        };

        chkSubDirs = new CheckBox
        {
            Text = "Alle Unterverzeichnisse kopieren",
            Location = new Point(20, 25),
            AutoSize = true,
            Checked = true
        };

        chkEmptyDirs = new CheckBox
        {
            Text = "Auch leere Unterordner kopieren",
            Location = new Point(20, 50),
            AutoSize = true,
            Checked = true
        };

        chkMirror = new CheckBox
        {
            Text = "Spiegelung: Ziel exakt wie Quelle (loescht Dateien im Ziel!)",
            Location = new Point(20, 75),
            AutoSize = true
        };

        chkTestRun = new CheckBox
        {
            Text = "Testlauf: nur anzeigen, nichts aendern",
            Location = new Point(20, 100),
            AutoSize = true
        };

        chkNoOverwrite = new CheckBox
        {
            Text = "Vorhandene Dateien nicht ueberschreiben",
            Location = new Point(380, 25),
            AutoSize = true
        };

        chkOnlyNewer = new CheckBox
        {
            Text = "Nur neuere Dateien kopieren",
            Location = new Point(380, 50),
            AutoSize = true
        };

        chkPermissions = new CheckBox
        {
            Text = "Berechtigungen kopieren (Admin empfohlen)",
            Location = new Point(380, 75),
            AutoSize = true
        };

        lblRetries = new Label
        {
            Text = "Wiederholungen:",
            Location = new Point(380, 105),
            AutoSize = true
        };

        numRetries = new NumericUpDown
        {
            Location = new Point(490, 100),
            Width = 60,
            Minimum = 0,
            Maximum = 100,
            Value = 1
        };

        lblWait = new Label
        {
            Text = "Wartezeit (Sek.):",
            Location = new Point(570, 105),
            AutoSize = true
        };

        numWait = new NumericUpDown
        {
            Location = new Point(670, 100),
            Width = 50,
            Minimum = 0,
            Maximum = 600,
            Value = 3
        };

        grpOptions.Controls.AddRange(new Control[]
        {
            chkSubDirs,
            chkEmptyDirs,
            chkMirror,
            chkTestRun,
            chkNoOverwrite,
            chkOnlyNewer,
            chkPermissions,
            lblRetries,
            numRetries,
            lblWait,
            numWait
        });

        progressBar = new ProgressBar
        {
            Location = new Point(12, 275),
            Size = new Size(736, 22),
            Style = ProgressBarStyle.Blocks
        };

        lblStatus = new Label
        {
            Text = "Bereit.",
            Location = new Point(12, 302),
            AutoSize = true,
            MaximumSize = new Size(736, 0)
        };

        btnStart = new Button
        {
            Text = "Start",
            Location = new Point(12, 330),
            Width = 100,
            Height = 32
        };

        btnCancel = new Button
        {
            Text = "Abbrechen",
            Location = new Point(120, 330),
            Width = 100,
            Height = 32,
            Enabled = false
        };

        btnSaveLog = new Button
        {
            Text = "Protokoll speichern...",
            Location = new Point(228, 330),
            Width = 150,
            Height = 32
        };

        txtLog = new TextBox
        {
            Location = new Point(12, 375),
            Size = new Size(736, 250),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            WordWrap = true,
            BackColor = SystemColors.Window,
            Font = new Font("Consolas", 8.5F)
        };

        Controls.AddRange(new Control[]
        {
            lblSource,
            txtSource,
            btnBrowseSource,
            lblTarget,
            txtTarget,
            btnBrowseTarget,
            btnSwap,
            grpOptions,
            progressBar,
            lblStatus,
            btnStart,
            btnCancel,
            btnSaveLog,
            txtLog
        });

        btnBrowseSource.Click += BtnBrowseSource_Click;
        btnBrowseTarget.Click += BtnBrowseTarget_Click;
        btnSwap.Click += BtnSwap_Click;
        btnStart.Click += BtnStart_Click;
        btnCancel.Click += BtnCancel_Click;
        btnSaveLog.Click += BtnSaveLog_Click;

        chkSubDirs.CheckedChanged += (s, e) =>
        {
            if (!chkMirror.Checked)
            {
                chkEmptyDirs.Enabled = chkSubDirs.Checked;
            }
        };

        chkMirror.CheckedChanged += ChkMirror_CheckedChanged;

        chkNoOverwrite.CheckedChanged += (s, e) =>
        {
            if (chkNoOverwrite.Checked)
            {
                chkOnlyNewer.Checked = false;
            }
        };

        chkOnlyNewer.CheckedChanged += (s, e) =>
        {
            if (chkOnlyNewer.Checked)
            {
                chkNoOverwrite.Checked = false;
            }
        };
    }

    private void BtnBrowseSource_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dlg = new FolderBrowserDialog())
        {
            dlg.Description = "Quellordner auswaehlen";

            if (!string.IsNullOrWhiteSpace(txtSource.Text) && Directory.Exists(txtSource.Text))
            {
                dlg.SelectedPath = txtSource.Text;
            }

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                txtSource.Text = dlg.SelectedPath;
            }
        }
    }

    private void BtnBrowseTarget_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dlg = new FolderBrowserDialog())
        {
            dlg.Description = "Zielordner auswaehlen";

            if (!string.IsNullOrWhiteSpace(txtTarget.Text) && Directory.Exists(txtTarget.Text))
            {
                dlg.SelectedPath = txtTarget.Text;
            }

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                txtTarget.Text = dlg.SelectedPath;
            }
        }
    }

    private void BtnSwap_Click(object sender, EventArgs e)
    {
        string temp = txtSource.Text;
        txtSource.Text = txtTarget.Text;
        txtTarget.Text = temp;
    }

    private void ChkMirror_CheckedChanged(object sender, EventArgs e)
    {
        bool mirror = chkMirror.Checked;

        chkSubDirs.Enabled = !mirror;
        chkEmptyDirs.Enabled = !mirror && chkSubDirs.Checked;
        chkNoOverwrite.Enabled = !mirror;
        chkOnlyNewer.Enabled = !mirror;

        if (mirror)
        {
            chkNoOverwrite.Checked = false;
            chkOnlyNewer.Checked = false;
        }
    }

    private void BtnStart_Click(object sender, EventArgs e)
    {
        string source;
        string target;

        if (!ValidatePaths(out source, out target))
        {
            return;
        }

        if (chkMirror.Checked && !chkTestRun.Checked)
        {
            DialogResult result = MessageBox.Show(
                this,
                "Im Spiegelungsmodus werden im Ziel auch Dateien geloescht, " +
                "die in der Quelle nicht mehr vorhanden sind.\n\nWirklich starten?",
                "Spiegelung bestaetigen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }
        }

        if (chkPermissions.Checked && !IsAdministrator())
        {
            MessageBox.Show(
                this,
                "Zum Kopieren von Berechtigungen/NTFS-Informationen sollte das Programm " +
                "moeglichst als Administrator gestartet werden.",
                "Hinweis",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        if (!chkTestRun.Checked)
        {
            try
            {
                Directory.CreateDirectory(target);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Der Zielordner konnte nicht erstellt werden:\n" + ex.Message,
                    "Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }

        string arguments = BuildArguments(source, target);

        txtLog.Clear();
        AppendLog("Start: robocopy.exe " + arguments);
        AppendLog(new string('-', 80));

        try
        {
            Encoding oem;

            try
            {
                oem = Encoding.GetEncoding((int)GetOEMCP());
            }
            catch
            {
                oem = Encoding.Default;
            }

            roboProcess = new Process();
            roboProcess.StartInfo.FileName = "robocopy.exe";
            roboProcess.StartInfo.Arguments = arguments;
            roboProcess.StartInfo.UseShellExecute = false;
            roboProcess.StartInfo.RedirectStandardOutput = true;
            roboProcess.StartInfo.RedirectStandardError = true;
            roboProcess.StartInfo.CreateNoWindow = true;
            roboProcess.StartInfo.StandardOutputEncoding = oem;
            roboProcess.StartInfo.StandardErrorEncoding = oem;
            roboProcess.EnableRaisingEvents = true;

            roboProcess.OutputDataReceived += RoboProcess_OutputDataReceived;
            roboProcess.ErrorDataReceived += RoboProcess_ErrorDataReceived;
            roboProcess.Exited += RoboProcess_Exited;

            cancelRequested = false;
            SetUiRunning(true);
            lblStatus.Text = "RoboCopy laeuft...";

            roboProcess.Start();
            roboProcess.BeginOutputReadLine();
            roboProcess.BeginErrorReadLine();
        }
        catch (Exception ex)
        {
            try
            {
                if (roboProcess != null)
                {
                    if (!roboProcess.HasExited)
                    {
                        roboProcess.Kill();
                    }

                    roboProcess.Dispose();
                    roboProcess = null;
                }
            }
            catch
            {
            }

            SetUiRunning(false);

            MessageBox.Show(
                this,
                "RoboCopy konnte nicht gestartet werden:\n" + ex.Message,
                "Fehler",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private bool ValidatePaths(out string source, out string target)
    {
        source = null;
        target = null;

        source = Environment.ExpandEnvironmentVariables(txtSource.Text.Trim().Trim('"'));
        target = Environment.ExpandEnvironmentVariables(txtTarget.Text.Trim().Trim('"'));

        if (string.IsNullOrWhiteSpace(source))
        {
            MessageBox.Show(
                this,
                "Bitte einen Quellordner auswaehlen.",
                "Quelle fehlt",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        if (!Directory.Exists(source))
        {
            MessageBox.Show(
                this,
                "Der Quellordner existiert nicht:\n" + source,
                "Quelle ungueltig",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(target))
        {
            MessageBox.Show(
                this,
                "Bitte einen Zielordner auswaehlen.",
                "Ziel fehlt",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        try
        {
            string fullSource = Path.GetFullPath(source);
            string fullTarget = Path.GetFullPath(target);

            if (string.Equals(fullSource, fullTarget, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    this,
                    "Quelle und Ziel duerfen nicht identisch sein.",
                    "Ungueltige Auswahl",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            string sourceWithSlash = fullSource.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string targetWithSlash = fullTarget.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (fullTarget.StartsWith(sourceWithSlash, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    this,
                    "Das Ziel darf nicht innerhalb der Quelle liegen.",
                    "Ungueltige Auswahl",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (fullSource.StartsWith(targetWithSlash, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    this,
                    "Die Quelle darf nicht innerhalb des Ziels liegen.",
                    "Ungueltige Auswahl",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            source = fullSource;
            target = fullTarget;
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                "Ungueltiger Pfad:\n" + ex.Message,
                "Pfadfehler",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return false;
        }
    }

    private string BuildArguments(string source, string target)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(QuotePath(source));
        sb.Append(' ');
        sb.Append(QuotePath(target));

        if (chkMirror.Checked)
        {
            sb.Append(" /MIR");
        }
        else if (chkSubDirs.Checked)
        {
            sb.Append(chkEmptyDirs.Checked ? " /E" : " /S");
        }

        if (!chkMirror.Checked)
        {
            if (chkNoOverwrite.Checked)
            {
                sb.Append(" /XC /XN /XO");
            }
            else if (chkOnlyNewer.Checked)
            {
                sb.Append(" /XO");
            }
        }

        if (chkPermissions.Checked)
        {
            sb.Append(" /COPYALL");
        }
        else
        {
            sb.Append(" /COPY:DAT");
        }

        sb.Append(" /R:" + (int)numRetries.Value);
        sb.Append(" /W:" + (int)numWait.Value);
        sb.Append(" /NP");

        if (chkTestRun.Checked)
        {
            sb.Append(" /L");
        }

        return sb.ToString();
    }

    private static string QuotePath(string path)
    {
        return "\"" + path.Trim().Trim('"') + "\"";
    }

    private void SetUiRunning(bool running)
    {
        isRunning = running;

        btnStart.Enabled = !running;
        btnCancel.Enabled = running;
        btnBrowseSource.Enabled = !running;
        btnBrowseTarget.Enabled = !running;
        btnSwap.Enabled = !running;
        grpOptions.Enabled = !running;

        progressBar.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;

        if (!running)
        {
            progressBar.Value = 0;
        }
    }

    private void RoboProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            AppendLogThreadSafe(e.Data);
        }
    }

    private void RoboProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            AppendLogThreadSafe("FEHLER: " + e.Data);
        }
    }

    private void AppendLogThreadSafe(string line)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        try
        {
            BeginInvoke((MethodInvoker)delegate
            {
                AppendLog(line);
            });
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (Exception)
        {
        }
    }

    private void AppendLog(string line)
    {
        if (IsDisposed || txtLog.IsDisposed)
        {
            return;
        }

        txtLog.AppendText(line + Environment.NewLine);
    }

    private void RoboProcess_Exited(object sender, EventArgs e)
    {
        Process p = sender as Process;
        int exitCode = -1;

        try
        {
            if (p != null)
            {
                p.WaitForExit();
                exitCode = p.ExitCode;
            }
        }
        catch
        {
        }

        bool wasCancelled = cancelRequested;
        roboProcess = null;

        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        try
        {
            BeginInvoke((MethodInvoker)delegate
            {
                SetUiRunning(false);

                string message;

                if (wasCancelled)
                {
                    message = "Vorgang wurde abgebrochen.";
                }
                else
                {
                    message = EvaluateExitCode(exitCode);
                }

                lblStatus.Text = message;

                AppendLog(new string('-', 80));
                AppendLog("RoboCopy beendet. Exit-Code: " + exitCode + " - " + message);

                if (!wasCancelled && exitCode >= 8)
                {
                    MessageBox.Show(
                        this,
                        message + "\n\nDetails stehen im Protokollfenster.",
                        "RoboCopy-Fehler",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            });
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (Exception)
        {
        }
    }

    private static string EvaluateExitCode(int code)
    {
        switch (code)
        {
            case 0:
                return "Keine Änderungen erforderlich.";

            case 1:
                return "Dateien wurden erfolgreich kopiert.";

            case 2:
                return "Es wurden zusaetzliche Dateien/Ordner im Ziel erkannt.";

            case 3:
                return "Dateien kopiert und zusaetzliche Daten erkannt.";

            case 4:
                return "Es gab Unterschiede/Aenderungen.";

            case 5:
                return "Dateien kopiert und Unterschiede erkannt.";

            case 6:
                return "Zusaetzliche Dateien und Unterschiede erkannt.";

            case 7:
                return "Dateien kopiert, zusaetzliche Dateien und Unterschiede erkannt.";

            case 8:
                return "Einige Dateien konnten nicht kopiert werden.";

            case 9:
                return "Einige Dateien wurden kopiert, andere scheiterten.";

            case 10:
                return "Einige Dateien konnten nicht kopiert werden; zusaetzliche Daten erkannt.";

            case 16:
                return "Schwerer Fehler. RoboCopy wurde nicht erfolgreich beendet.";

            default:
                if (code > 16)
                {
                    return "Schwerer Fehler (Exit-Code " + code + ").";
                }

                if (code >= 8)
                {
                    return "Fehler beim Kopieren (Exit-Code " + code + ").";
                }

                return "RoboCopy beendet (Exit-Code " + code + ").";
        }
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        if (roboProcess != null)
        {
            cancelRequested = true;

            try
            {
                if (!roboProcess.HasExited)
                {
                    roboProcess.Kill();
                }
            }
            catch
            {
            }

            lblStatus.Text = "Abbruch...";
        }
    }

    private void BtnSaveLog_Click(object sender, EventArgs e)
    {
        using (SaveFileDialog dlg = new SaveFileDialog())
        {
            dlg.Title = "Protokoll speichern";
            dlg.Filter = "Textdateien (*.txt)|*.txt|Logdateien (*.log)|*.log|Alle Dateien (*.*)|*.*";
            dlg.FileName = "RoboCopy_Protokoll_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, txtLog.Text, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        "Fehler beim Speichern:\n" + ex.Message,
                        "Fehler",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }

    private static bool IsAdministrator()
    {
        try
        {
            System.Security.Principal.WindowsIdentity identity =
                System.Security.Principal.WindowsIdentity.GetCurrent();

            System.Security.Principal.WindowsPrincipal principal =
                new System.Security.Principal.WindowsPrincipal(identity);

            return principal.IsInRole(
                System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (isRunning && roboProcess != null)
        {
            DialogResult result = MessageBox.Show(
                this,
                "Der Kopiervorgang laeuft noch.\nWirklich beenden?",
                "Beenden bestaetigen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                base.OnFormClosing(e);
                return;
            }

            try
            {
                if (!roboProcess.HasExited)
                {
                    roboProcess.Kill();
                }
            }
            catch
            {
            }
        }

        base.OnFormClosing(e);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        try
        {
            if (roboProcess != null)
            {
                roboProcess.Dispose();
                roboProcess = null;
            }
        }
        catch
        {
        }

        base.OnFormClosed(e);
    }
}