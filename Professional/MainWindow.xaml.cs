using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;

namespace RobbyCopy
{
    public class Loc : INotifyPropertyChanged
    {
        private static Loc _current;
        public static Loc Current => _current ??= new Loc();

        private string _language = "en";

        public string Language
        {
            get => _language;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && _language != value)
                {
                    _language = value;
                    Refresh();
                }
            }
        }

        public string this[string key]
        {
            get
            {
                if (Texts.TryGetValue(_language, out var dict) && dict.TryGetValue(key, out var value))
                {
                    return value;
                }

                if (Texts.TryGetValue("en", out var en) && en.TryGetValue(key, out var enValue))
                {
                    return enValue;
                }

                return key;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        private static readonly Dictionary<string, Dictionary<string, string>> Texts =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string>
                {
                    ["AppTitle"] = "RobbyCopy Pro",
                    ["Subtitle"] = "RoboCopy made easy",
                    ["Language"] = "Language",
                    ["TabBackup"] = "Backup",
                    ["TabHistory"] = "History",
                    ["TabSettings"] = "Settings",
                    ["Sources"] = "Source folders",
                    ["AddSource"] = "Add folder",
                    ["RemoveSource"] = "Remove",
                    ["ClearSources"] = "Clear",
                    ["DragDropHint"] = "Tip: Drag folders or files into the list. Files will add their folder.",
                    ["Target"] = "Target folder",
                    ["Browse"] = "Browse...",
                    ["Options"] = "Options",
                    ["Preset"] = "Preset",
                    ["PresetSimple"] = "Simple backup",
                    ["PresetMirror"] = "Mirror",
                    ["PresetOnlyNewer"] = "Only newer files",
                    ["PresetExternal"] = "External drive backup",
                    ["IncludeSubdirs"] = "Include subfolders",
                    ["IncludeEmptyDirs"] = "Include empty subfolders",
                    ["Mirror"] = "Mirror (deletes files in target that are missing in source)",
                    ["NoOverwrite"] = "Do not overwrite existing files",
                    ["OnlyNewer"] = "Copy only newer files",
                    ["CopyPermissions"] = "Copy permissions (admin recommended)",
                    ["UseSourceSubfolder"] = "Create subfolder per source in target",
                    ["FolderStructureOnly"] = "Copy folder structure only (no files)",
                    ["TestRun"] = "Test run (no changes)",
                    ["Retries"] = "Retries",
                    ["WaitSeconds"] = "Wait (sec.)",
                    ["CommandPreview"] = "RoboCopy command preview",
                    ["Schedule"] = "Schedule",
                    ["EnableSchedule"] = "Enable schedule",
                    ["ScheduleTime"] = "Time",
                    ["ScheduleDaily"] = "Daily",
                    ["ScheduleHint"] = "The app must be running for the internal scheduler to start jobs. For true background execution, export the profile to Windows Task Scheduler.",
                    ["NextRun"] = "Next run:",
                    ["Profiles"] = "Profiles",
                    ["ProfileName"] = "Profile name",
                    ["SaveProfile"] = "Save profile",
                    ["NewProfile"] = "New",
                    ["DeleteProfile"] = "Delete",
                    ["ExportTask"] = "Export to Task Scheduler",
                    ["ExportTaskDone"] = "Scheduled task created:\n{0}",
                    ["ExportTaskFailed"] = "Could not create scheduled task.",
                    ["ExportTaskNeedName"] = "Please enter a profile name before exporting to Task Scheduler.",
                    ["Start"] = "Start",
                    ["Cancel"] = "Cancel",
                    ["Progress"] = "Progress",
                    ["OverallProgress"] = "Overall progress (approx.)",
                    ["CurrentFileProgress"] = "Current file progress",
                    ["Ready"] = "Ready.",
                    ["Running"] = "Running...",
                    ["Log"] = "Log",
                    ["AutoLog"] = "Automatic log file",
                    ["OpenLogFolder"] = "Open log folder",
                    ["SaveLog"] = "Save log...",
                    ["Error"] = "Error",
                    ["Warning"] = "Warning",
                    ["Info"] = "Information",
                    ["NoSources"] = "Please add at least one source folder.",
                    ["TargetMissing"] = "Please choose a target folder.",
                    ["InvalidPath"] = "Invalid path.",
                    ["SourceNotExist"] = "Source folder does not exist:",
                    ["SourceTargetSame"] = "Source and target must not be identical.",
                    ["TargetInsideSource"] = "The target must not be inside a source folder.",
                    ["MirrorWarningTitle"] = "Confirm mirror",
                    ["MirrorWarningText"] = "Mirror mode can delete files in the target. Continue?",
                    ["AdminHint"] = "Copying permissions may require administrator rights.",
                    ["FolderSelect"] = "Select folder",
                    ["SelectTarget"] = "Select target folder",
                    ["RunCompleted"] = "Run completed.",
                    ["RunCancelled"] = "Run cancelled.",
                    ["FailedJobs"] = "Failed jobs",
                    ["Job"] = "Job",
                    ["Of"] = "of",
                    ["ProfileSaved"] = "Profile saved.",
                    ["ProfileLoaded"] = "Profile loaded.",
                    ["ProfileDeleted"] = "Profile deleted.",
                    ["ConfirmDeleteTitle"] = "Delete profile",
                    ["ConfirmDeleteText"] = "Delete this profile?",
                    ["LogSaved"] = "Log saved.",
                    ["CannotOpenFolder"] = "Could not open folder.",
                    ["ScheduleRunStarted"] = "Scheduler started backup run.",
                    ["Exit0"] = "No changes were necessary.",
                    ["Exit1"] = "Files were copied successfully.",
                    ["ExitInfo"] = "RoboCopy finished with information/changes.",
                    ["ExitError"] = "Some files could not be copied.",
                    ["ExitFatal"] = "Fatal RoboCopy error.",
                    ["ExitUnknown"] = "RoboCopy finished with unknown status.",
                    ["ConfirmExitTitle"] = "Exit",
                    ["ConfirmExitText"] = "A backup is still running. Exit anyway?",
                    ["TrayShow"] = "Show RobbyCopy",
                    ["TrayRun"] = "Run current profile",
                    ["TrayExit"] = "Exit",
                    ["DarkMode"] = "Dark mode",
                    ["MinimizeToTray"] = "Minimize to tray / close to tray",
                    ["SettingsAppearance"] = "Appearance",
                    ["SettingsEmail"] = "E-mail notification after backup",
                    ["EmailEnabled"] = "Send e-mail after backup",
                    ["SmtpHost"] = "SMTP host",
                    ["SmtpPort"] = "SMTP port",
                    ["SmtpUser"] = "SMTP user",
                    ["SmtpPassword"] = "SMTP password",
                    ["EmailFrom"] = "From",
                    ["EmailTo"] = "To",
                    ["EmailUseSsl"] = "Use SSL/TLS",
                    ["SaveSettings"] = "Save settings",
                    ["SettingsSaved"] = "Settings saved.",
                    ["HistoryTime"] = "Time",
                    ["HistoryProfile"] = "Profile",
                    ["HistoryStatus"] = "Status",
                    ["HistoryJobs"] = "Jobs",
                    ["HistoryFailed"] = "Failed",
                    ["HistoryDuration"] = "Duration",
                    ["HistoryLog"] = "Log file",
                    ["ClearHistory"] = "Clear history",
                    ["Status"] = "Status"
                },

                ["de"] = new Dictionary<string, string>
                {
                    ["AppTitle"] = "RobbyCopy Pro",
                    ["Subtitle"] = "RoboCopy einfach gemacht",
                    ["Language"] = "Sprache",
                    ["TabBackup"] = "Sicherung",
                    ["TabHistory"] = "Verlauf",
                    ["TabSettings"] = "Einstellungen",
                    ["Sources"] = "Quellordner",
                    ["AddSource"] = "Ordner hinzufügen",
                    ["RemoveSource"] = "Entfernen",
                    ["ClearSources"] = "Leeren",
                    ["DragDropHint"] = "Tipp: Ordner oder Dateien per Drag & Drop in die Liste ziehen. Dateien fügen ihren Ordner hinzu.",
                    ["Target"] = "Zielordner",
                    ["Browse"] = "Durchsuchen...",
                    ["Options"] = "Optionen",
                    ["Preset"] = "Voreinstellung",
                    ["PresetSimple"] = "Einfache Sicherung",
                    ["PresetMirror"] = "Spiegelung",
                    ["PresetOnlyNewer"] = "Nur neue Dateien",
                    ["PresetExternal"] = "Externe Festplatte sichern",
                    ["IncludeSubdirs"] = "Unterverzeichnisse einbeziehen",
                    ["IncludeEmptyDirs"] = "Leere Unterordner einbeziehen",
                    ["Mirror"] = "Spiegelung (löscht Dateien im Ziel, die in der Quelle fehlen)",
                    ["NoOverwrite"] = "Vorhandene Dateien nicht überschreiben",
                    ["OnlyNewer"] = "Nur neuere Dateien kopieren",
                    ["CopyPermissions"] = "Berechtigungen kopieren (Admin empfohlen)",
                    ["UseSourceSubfolder"] = "Für jede Quelle einen Unterordner im Ziel anlegen",
                    ["FolderStructureOnly"] = "Nur Ordnerstruktur kopieren (keine Dateien)",
                    ["TestRun"] = "Testlauf (keine Änderungen)",
                    ["Retries"] = "Wiederholungen",
                    ["WaitSeconds"] = "Wartezeit (Sek.)",
                    ["CommandPreview"] = "RoboCopy-Befehlvorschau",
                    ["Schedule"] = "Zeitplanung",
                    ["EnableSchedule"] = "Zeitplanung aktivieren",
                    ["ScheduleTime"] = "Uhrzeit",
                    ["ScheduleDaily"] = "Täglich",
                    ["ScheduleHint"] = "Der interne Zeitplaner läuft nur, solange die App geöffnet ist. Für echte Hintergrundläufe das Profil in die Windows-Aufgabenplanung exportieren.",
                    ["NextRun"] = "Nächster Lauf:",
                    ["Profiles"] = "Profile",
                    ["ProfileName"] = "Profilname",
                    ["SaveProfile"] = "Profil speichern",
                    ["NewProfile"] = "Neu",
                    ["DeleteProfile"] = "Löschen",
                    ["ExportTask"] = "In Taskplaner exportieren",
                    ["ExportTaskDone"] = "Geplanter Task wurde erstellt:\n{0}",
                    ["ExportTaskFailed"] = "Der geplante Task konnte nicht erstellt werden.",
                    ["ExportTaskNeedName"] = "Bitte zuerst einen Profilnamen vergeben, bevor in den Taskplaner exportiert wird.",
                    ["Start"] = "Start",
                    ["Cancel"] = "Abbrechen",
                    ["Progress"] = "Fortschritt",
                    ["OverallProgress"] = "Gesamtfortschritt (ungefähr)",
                    ["CurrentFileProgress"] = "Fortschritt aktuelle Datei",
                    ["Ready"] = "Bereit.",
                    ["Running"] = "Läuft...",
                    ["Log"] = "Protokoll",
                    ["AutoLog"] = "Automatische Protokolldatei",
                    ["OpenLogFolder"] = "Protokollordner öffnen",
                    ["SaveLog"] = "Protokoll speichern...",
                    ["Error"] = "Fehler",
                    ["Warning"] = "Warnung",
                    ["Info"] = "Information",
                    ["NoSources"] = "Bitte mindestens einen Quellordner hinzufügen.",
                    ["TargetMissing"] = "Bitte einen Zielordner wählen.",
                    ["InvalidPath"] = "Ungültiger Pfad.",
                    ["SourceNotExist"] = "Quellordner existiert nicht:",
                    ["SourceTargetSame"] = "Quelle und Ziel dürfen nicht identisch sein.",
                    ["TargetInsideSource"] = "Das Ziel darf nicht innerhalb eines Quellordners liegen.",
                    ["MirrorWarningTitle"] = "Spiegelung bestätigen",
                    ["MirrorWarningText"] = "Der Spiegelungsmodus kann Dateien im Ziel löschen. Fortfahren?",
                    ["AdminHint"] = "Das Kopieren von Berechtigungen erfordert ggf. Administratorrechte.",
                    ["FolderSelect"] = "Ordner auswählen",
                    ["SelectTarget"] = "Zielordner auswählen",
                    ["RunCompleted"] = "Lauf abgeschlossen.",
                    ["RunCancelled"] = "Lauf abgebrochen.",
                    ["FailedJobs"] = "Fehlgeschlagene Jobs",
                    ["Job"] = "Job",
                    ["Of"] = "von",
                    ["ProfileSaved"] = "Profil gespeichert.",
                    ["ProfileLoaded"] = "Profil geladen.",
                    ["ProfileDeleted"] = "Profil gelöscht.",
                    ["ConfirmDeleteTitle"] = "Profil löschen",
                    ["ConfirmDeleteText"] = "Dieses Profil löschen?",
                    ["LogSaved"] = "Protokoll gespeichert.",
                    ["CannotOpenFolder"] = "Ordner konnte nicht geöffnet werden.",
                    ["ScheduleRunStarted"] = "Der Zeitplaner hat einen Sicherungslauf gestartet.",
                    ["Exit0"] = "Keine Änderungen erforderlich.",
                    ["Exit1"] = "Dateien wurden erfolgreich kopiert.",
                    ["ExitInfo"] = "RoboCopy wurde mit Änderungen/Informationen beendet.",
                    ["ExitError"] = "Einige Dateien konnten nicht kopiert werden.",
                    ["ExitFatal"] = "Schwerer RoboCopy-Fehler.",
                    ["ExitUnknown"] = "RoboCopy wurde mit unbekanntem Status beendet.",
                    ["ConfirmExitTitle"] = "Beenden bestätigen",
                    ["ConfirmExitText"] = "Es läuft gerade ein Sicherungsvorgang. Trotzdem beenden?",
                    ["TrayShow"] = "RobbyCopy anzeigen",
                    ["TrayRun"] = "Aktuelles Profil starten",
                    ["TrayExit"] = "Beenden",
                    ["DarkMode"] = "Dunkler Modus",
                    ["MinimizeToTray"] = "In Tray minimieren / Close in Tray",
                    ["SettingsAppearance"] = "Darstellung",
                    ["SettingsEmail"] = "E-Mail-Benachrichtigung nach Sicherung",
                    ["EmailEnabled"] = "E-Mail nach Sicherung senden",
                    ["SmtpHost"] = "SMTP-Host",
                    ["SmtpPort"] = "SMTP-Port",
                    ["SmtpUser"] = "SMTP-Benutzer",
                    ["SmtpPassword"] = "SMTP-Passwort",
                    ["EmailFrom"] = "Von",
                    ["EmailTo"] = "An",
                    ["EmailUseSsl"] = "SSL/TLS verwenden",
                    ["SaveSettings"] = "Einstellungen speichern",
                    ["SettingsSaved"] = "Einstellungen gespeichert.",
                    ["HistoryTime"] = "Zeit",
                    ["HistoryProfile"] = "Profil",
                    ["HistoryStatus"] = "Status",
                    ["HistoryJobs"] = "Jobs",
                    ["HistoryFailed"] = "Fehler",
                    ["HistoryDuration"] = "Dauer",
                    ["HistoryLog"] = "Protokolldatei",
                    ["ClearHistory"] = "Verlauf leeren",
                    ["Status"] = "Status"
                }
            };
    }

    public class AppSettings
    {
        public string Language { get; set; } = "en";
        public bool AutoLog { get; set; } = true;
        public string LastTarget { get; set; } = "";
        public string LastProfile { get; set; } = "";

        public bool DarkMode { get; set; }
        public bool MinimizeToTray { get; set; } = true;

        public bool EmailEnabled { get; set; }
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = "";
        public string SmtpPassword { get; set; } = "";
        public string EmailFrom { get; set; } = "";
        public string EmailTo { get; set; } = "";
        public bool EmailUseSsl { get; set; } = true;
    }

    public class BackupProfile
    {
        public string Name { get; set; } = "";
        public List<string> Sources { get; set; } = new List<string>();
        public string Target { get; set; } = "";
        public string Preset { get; set; } = "Simple";

        public bool IncludeSubdirs { get; set; } = true;
        public bool IncludeEmptyDirs { get; set; } = true;
        public bool Mirror { get; set; }
        public bool NoOverwrite { get; set; }
        public bool OnlyNewer { get; set; }
        public bool CopyPermissions { get; set; }
        public bool UseSourceSubfolder { get; set; } = true;
        public bool FolderStructureOnly { get; set; }
        public bool TestRun { get; set; }

        public int Retries { get; set; } = 1;
        public int WaitSeconds { get; set; } = 3;

        public bool ScheduleEnabled { get; set; }
        public int ScheduleHour { get; set; } = 20;
        public int ScheduleMinute { get; set; }
        public bool ScheduleDaily { get; set; } = true;
    }

    public class HistoryEntry
    {
        public DateTime Timestamp { get; set; }
        public string Profile { get; set; }
        public string Status { get; set; }
        public int Jobs { get; set; }
        public int FailedJobs { get; set; }
        public string Duration { get; set; }
        public string LogFile { get; set; }
    }

    public class RoboCopyJob
    {
        public string Source { get; set; }
        public string Target { get; set; }
    }

    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetOEMCP();

        private readonly string _appFolder;
        private readonly string _profilesPath;
        private readonly string _settingsPath;
        private readonly string _logFolder;
        private readonly string _historyPath;

        private AppSettings _settings = new AppSettings();
        private List<BackupProfile> _profiles = new List<BackupProfile>();
        private List<HistoryEntry> _history = new List<HistoryEntry>();
        private readonly List<RoboCopyJob> _jobs = new List<RoboCopyJob>();

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        private bool _initializing;
        private bool _uiReady;
        private bool _loadingProfilesList;
        private bool _loadingProfile;

        private bool _isRunning;
        private bool _cancelRequested;
        private bool _pendingAutoRun;
        private bool _forceExit;
        private bool _exitAfterRun;

        private Process _process;
        private int _jobIndex;
        private int _totalJobs;
        private int _failedJobs;

        private DateTime _runStartTime;
        private string _currentLogPath;

        private StreamWriter _logWriter;
        private readonly object _logLock = new object();

        private DispatcherTimer _schedulerTimer;
        private DateTime? _nextScheduledRun;

        private WinForms.NotifyIcon _notifyIcon;
        private WinForms.ToolStripMenuItem _trayShowItem;
        private WinForms.ToolStripMenuItem _trayRunItem;
        private WinForms.ToolStripMenuItem _trayExitItem;

        private static readonly Regex PercentRegex =
            new Regex(@"(?<value>\d{1,3}(?:[.,]\d+)?)\s*%", RegexOptions.Compiled);

        public MainWindow()
        {
            _initializing = true;
            _uiReady = false;

            _appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RobbyCopy");

            _profilesPath = Path.Combine(_appFolder, "profiles.json");
            _settingsPath = Path.Combine(_appFolder, "settings.json");
            _logFolder = Path.Combine(_appFolder, "Logs");
            _historyPath = Path.Combine(_appFolder, "history.json");

            InitializeComponent();

            DataContext = Loc.Current;

            try
            {
                Directory.CreateDirectory(_appFolder);
                Directory.CreateDirectory(_logFolder);
            }
            catch
            {
            }

            _settings = LoadSettingsFromFile();
            Loc.Current.Language = _settings.Language == "de" ? "de" : "en";

            chkAutoLog.IsChecked = _settings.AutoLog;
            txtTarget.Text = _settings.LastTarget ?? "";

            chkDarkMode.IsChecked = _settings.DarkMode;
            chkMinimizeToTray.IsChecked = _settings.MinimizeToTray;

            chkEmailEnabled.IsChecked = _settings.EmailEnabled;
            txtSmtpHost.Text = _settings.SmtpHost;
            txtSmtpPort.Text = _settings.SmtpPort.ToString(CultureInfo.InvariantCulture);
            txtSmtpUser.Text = _settings.SmtpUser;
            txtSmtpPassword.Password = _settings.SmtpPassword;
            txtEmailFrom.Text = _settings.EmailFrom;
            txtEmailTo.Text = _settings.EmailTo;
            chkEmailUseSsl.IsChecked = _settings.EmailUseSsl;

            SelectLanguageInCombo(Loc.Current.Language);
            InitializeHoursMinutes();
            ApplyTheme();

            LoadProfiles();
            LoadHistory();

            historyGrid.ItemsSource = _history;

            if (!string.IsNullOrWhiteSpace(_settings.LastProfile))
            {
                var profile = _profiles.FirstOrDefault(p =>
                    string.Equals(p.Name, _settings.LastProfile, StringComparison.OrdinalIgnoreCase));

                if (profile != null)
                {
                    ReloadProfileList(profile.Name);
                    LoadProfileIntoUi(profile);
                }
            }

            if (cmbPreset.SelectedItem == null)
            {
                ApplyPreset("Simple", true, true);
            }

            ParseCommandLine();

            _initializing = false;
            _uiReady = true;

            UpdateOptionAvailability();
            UpdateScheduleLabel();
            UpdateCommandPreview();

            InitializeTray();

            _schedulerTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(20)
            };

            _schedulerTimer.Tick += SchedulerTimer_Tick;
            _schedulerTimer.Start();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            if (_notifyIcon == null)
            {
                InitializeTray();
            }

            if (WindowState == WindowState.Minimized && chkMinimizeToTray?.IsChecked == true)
            {
                Hide();
            }

            if (_pendingAutoRun)
            {
                StartRun(true);
            }
        }

        private static string GetCommandLineValue(string name)
        {
            var args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals("/" + name, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        private static bool HasCommandLineFlag(string name)
        {
            var args = Environment.GetCommandLineArgs();

            foreach (string arg in args)
            {
                if (arg.Equals("/" + name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void ParseCommandLine()
        {
            _exitAfterRun = HasCommandLineFlag("exit");

            string profileName = GetCommandLineValue("profile");

            if (!string.IsNullOrWhiteSpace(profileName))
            {
                var profile = _profiles.FirstOrDefault(p =>
                    string.Equals(p.Name, profileName, StringComparison.OrdinalIgnoreCase));

                if (profile != null)
                {
                    ReloadProfileList(profile.Name);
                    LoadProfileIntoUi(profile);
                }

                if (HasCommandLineFlag("run"))
                {
                    _pendingAutoRun = true;
                }
            }

            if (HasCommandLineFlag("minimized") || HasCommandLineFlag("silent"))
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_uiReady || _initializing)
            {
                return;
            }

            if (cmbLanguage.SelectedItem is ComboBoxItem item && item.Tag is string lang)
            {
                Loc.Current.Language = lang;
                Title = Loc.Current["AppTitle"];
                SaveSettings();

                lblStatus.Text = _isRunning ? Loc.Current["Running"] : Loc.Current["Ready"];
                UpdateScheduleLabel();
                UpdateTrayText();
            }
        }

        private void SelectLanguageInCombo(string lang)
        {
            if (cmbLanguage == null)
            {
                return;
            }

            foreach (ComboBoxItem item in cmbLanguage.Items)
            {
                if (string.Equals(item.Tag as string, lang, StringComparison.OrdinalIgnoreCase))
                {
                    cmbLanguage.SelectedItem = item;
                    return;
                }
            }
        }

        private void ApplyTheme()
        {
            bool dark = chkDarkMode?.IsChecked == true;

            SetResource("WindowBackground", Brush(dark ? "#0F172A" : "#F3F4F6"));
            SetResource("HeaderBackground", Brush(dark ? "#111827" : "#FFFFFF"));
            SetResource("CardBackground", Brush(dark ? "#111827" : "#FFFFFF"));
            SetResource("CardBorder", Brush(dark ? "#1F2937" : "#E5E7EB"));

            SetResource("TextPrimary", Brush(dark ? "#F9FAFB" : "#111827"));
            SetResource("TextSecondary", Brush(dark ? "#9CA3AF" : "#6B7280"));

            SetResource("InputBackground", Brush(dark ? "#1F2937" : "#FFFFFF"));
            SetResource("InputBorder", Brush(dark ? "#374151" : "#D1D5DB"));

            SetResource("ButtonBackground", Brush(dark ? "#2563EB" : "#0067C0"));
            SetResource("ButtonHoverBackground", Brush(dark ? "#3B82F6" : "#1975C5"));
            SetResource("ButtonForeground", Brush("#FFFFFF"));

            SetResource("SecondaryButtonBackground", Brush(dark ? "#1F2937" : "#E5E7EB"));
            SetResource("SecondaryButtonHover", Brush(dark ? "#374151" : "#D1D5DB"));
            SetResource("SecondaryButtonForeground", Brush(dark ? "#F9FAFB" : "#111827"));

            SetResource("ProgressBackground", Brush(dark ? "#1F2937" : "#E5E7EB"));
            SetResource("ProgressForeground", Brush(dark ? "#3B82F6" : "#0067C0"));
        }

        private void SetResource(string key, object value)
        {
            if (Resources.Contains(key))
            {
                Resources[key] = value;
            }
            else
            {
                Resources.Add(key, value);
            }
        }

        private static System.Windows.Media.Brush Brush(string hex)
        {
            return new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex));
        }

        private void Theme_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing)
            {
                return;
            }

            ApplyTheme();
            SaveSettings();
        }

        private void InitializeHoursMinutes()
        {
            if (cmbHour == null || cmbMinute == null)
            {
                return;
            }

            cmbHour.Items.Clear();
            cmbMinute.Items.Clear();

            for (int i = 0; i < 24; i++)
            {
                cmbHour.Items.Add(new ComboBoxItem
                {
                    Content = i.ToString("00"),
                    Tag = i
                });
            }

            for (int i = 0; i < 60; i++)
            {
                cmbMinute.Items.Add(new ComboBoxItem
                {
                    Content = i.ToString("00"),
                    Tag = i
                });
            }

            cmbHour.SelectedIndex = 20;
            cmbMinute.SelectedIndex = 0;
        }

        private AppSettings LoadSettingsFromFile()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_settingsPath)) ?? new AppSettings();
                }
            }
            catch
            {
            }

            return new AppSettings();
        }

        private void SaveSettings()
        {
            if (!_uiReady)
            {
                return;
            }

            try
            {
                _settings.Language = Loc.Current.Language;
                _settings.AutoLog = chkAutoLog.IsChecked == true;
                _settings.LastTarget = txtTarget.Text?.Trim();
                _settings.LastProfile = (cmbProfiles.SelectedItem as BackupProfile)?.Name;

                _settings.DarkMode = chkDarkMode.IsChecked == true;
                _settings.MinimizeToTray = chkMinimizeToTray.IsChecked == true;

                _settings.EmailEnabled = chkEmailEnabled.IsChecked == true;
                _settings.SmtpHost = txtSmtpHost.Text?.Trim();
                _settings.SmtpPort = ParsePort(txtSmtpPort.Text, 587);
                _settings.SmtpUser = txtSmtpUser.Text?.Trim();
                _settings.SmtpPassword = txtSmtpPassword.Password;
                _settings.EmailFrom = txtEmailFrom.Text?.Trim();
                _settings.EmailTo = txtEmailTo.Text?.Trim();
                _settings.EmailUseSsl = chkEmailUseSsl.IsChecked == true;

                Directory.CreateDirectory(_appFolder);
                File.WriteAllText(_settingsPath, JsonSerializer.Serialize(_settings, _jsonOptions));
            }
            catch
            {
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            SaveSettings();

            MessageBox.Show(
                this,
                Loc.Current["SettingsSaved"],
                Loc.Current["Info"],
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void LoadProfiles()
        {
            try
            {
                if (File.Exists(_profilesPath))
                {
                    _profiles = JsonSerializer.Deserialize<List<BackupProfile>>(File.ReadAllText(_profilesPath))
                                ?? new List<BackupProfile>();
                }
            }
            catch
            {
                _profiles = new List<BackupProfile>();
            }

            ReloadProfileList(null);
        }

        private void ReloadProfileList(string selectedName)
        {
            if (cmbProfiles == null)
            {
                return;
            }

            _loadingProfilesList = true;

            cmbProfiles.ItemsSource = null;
            cmbProfiles.ItemsSource = _profiles;
            cmbProfiles.DisplayMemberPath = nameof(BackupProfile.Name);

            if (!string.IsNullOrWhiteSpace(selectedName))
            {
                var profile = _profiles.FirstOrDefault(p =>
                    string.Equals(p.Name, selectedName, StringComparison.OrdinalIgnoreCase));

                if (profile != null)
                {
                    cmbProfiles.SelectedItem = profile;
                }
            }

            _loadingProfilesList = false;
        }

        private void Profiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfilesList || _loadingProfile)
            {
                return;
            }

            if (cmbProfiles.SelectedItem is BackupProfile profile)
            {
                LoadProfileIntoUi(profile);
            }
        }

        private void Profiles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_uiReady || _isRunning)
            {
                return;
            }

            if (cmbProfiles.SelectedItem is BackupProfile)
            {
                StartRun(false);
            }
        }

        private void LoadProfileIntoUi(BackupProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            _loadingProfile = true;

            txtProfileName.Text = profile.Name;

            lstSources.Items.Clear();

            if (profile.Sources != null)
            {
                foreach (var source in profile.Sources)
                {
                    AddSourcePath(source);
                }
            }

            txtTarget.Text = profile.Target ?? "";

            SelectPresetByTag(profile.Preset ?? "Simple");

            chkSubdirs.IsChecked = profile.IncludeSubdirs;
            chkEmptyDirs.IsChecked = profile.IncludeEmptyDirs;
            chkMirror.IsChecked = profile.Mirror;
            chkNoOverwrite.IsChecked = profile.NoOverwrite;
            chkOnlyNewer.IsChecked = profile.OnlyNewer;
            chkPermissions.IsChecked = profile.CopyPermissions;
            chkUseSourceSubfolder.IsChecked = profile.UseSourceSubfolder;
            chkFolderStructureOnly.IsChecked = profile.FolderStructureOnly;
            chkTestRun.IsChecked = profile.TestRun;

            txtRetries.Text = profile.Retries.ToString(CultureInfo.InvariantCulture);
            txtWait.Text = profile.WaitSeconds.ToString(CultureInfo.InvariantCulture);

            chkScheduleEnabled.IsChecked = profile.ScheduleEnabled;
            chkScheduleDaily.IsChecked = profile.ScheduleDaily;
            SelectHourMinute(profile.ScheduleHour, profile.ScheduleMinute);

            _loadingProfile = false;

            UpdateOptionAvailability();
            UpdateScheduleLabel();
            UpdateCommandPreview();
        }

        private void SelectPresetByTag(string tag)
        {
            if (cmbPreset == null)
            {
                return;
            }

            foreach (ComboBoxItem item in cmbPreset.Items)
            {
                if (string.Equals(item.Tag as string, tag, StringComparison.OrdinalIgnoreCase))
                {
                    cmbPreset.SelectedItem = item;
                    return;
                }
            }

            if (cmbPreset.Items.Count > 0)
            {
                cmbPreset.SelectedIndex = 0;
            }
        }

        private string GetSelectedPresetTag()
        {
            return (cmbPreset.SelectedItem as ComboBoxItem)?.Tag as string ?? "Simple";
        }

        private void Preset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            ApplyPreset(GetSelectedPresetTag(), false, false);
        }

        private void ApplyPreset(string preset, bool updateComboBox, bool suppressEvents)
        {
            bool wasLoading = _loadingProfile;

            if (suppressEvents)
            {
                _loadingProfile = true;
            }

            if (updateComboBox)
            {
                SelectPresetByTag(preset);
            }

            switch (preset)
            {
                case "Mirror":
                    chkSubdirs.IsChecked = true;
                    chkEmptyDirs.IsChecked = true;
                    chkMirror.IsChecked = true;
                    chkNoOverwrite.IsChecked = false;
                    chkOnlyNewer.IsChecked = false;
                    chkPermissions.IsChecked = false;
                    chkUseSourceSubfolder.IsChecked = true;
                    chkFolderStructureOnly.IsChecked = false;
                    chkTestRun.IsChecked = false;
                    break;

                case "OnlyNewer":
                    chkSubdirs.IsChecked = true;
                    chkEmptyDirs.IsChecked = true;
                    chkMirror.IsChecked = false;
                    chkNoOverwrite.IsChecked = false;
                    chkOnlyNewer.IsChecked = true;
                    chkPermissions.IsChecked = false;
                    chkUseSourceSubfolder.IsChecked = true;
                    chkFolderStructureOnly.IsChecked = false;
                    chkTestRun.IsChecked = false;
                    break;

                case "External":
                    chkSubdirs.IsChecked = true;
                    chkEmptyDirs.IsChecked = true;
                    chkMirror.IsChecked = false;
                    chkNoOverwrite.IsChecked = false;
                    chkOnlyNewer.IsChecked = true;
                    chkPermissions.IsChecked = false;
                    chkUseSourceSubfolder.IsChecked = true;
                    chkFolderStructureOnly.IsChecked = false;
                    chkTestRun.IsChecked = false;
                    break;

                default:
                    chkSubdirs.IsChecked = true;
                    chkEmptyDirs.IsChecked = true;
                    chkMirror.IsChecked = false;
                    chkNoOverwrite.IsChecked = false;
                    chkOnlyNewer.IsChecked = false;
                    chkPermissions.IsChecked = false;
                    chkUseSourceSubfolder.IsChecked = true;
                    chkFolderStructureOnly.IsChecked = false;
                    chkTestRun.IsChecked = false;
                    break;
            }

            if (suppressEvents)
            {
                _loadingProfile = wasLoading;
            }

            UpdateOptionAvailability();

            if (!_loadingProfile)
            {
                UpdateCommandPreview();
            }
        }

        private void Subdirs_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            UpdateOptionAvailability();
            UpdateCommandPreview();
        }

        private void Mirror_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            if (chkMirror.IsChecked == true)
            {
                chkNoOverwrite.IsChecked = false;
                chkOnlyNewer.IsChecked = false;
            }

            UpdateOptionAvailability();
            UpdateCommandPreview();
        }

        private void NoOverwrite_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            if (chkNoOverwrite.IsChecked == true)
            {
                chkOnlyNewer.IsChecked = false;
            }

            UpdateCommandPreview();
        }

        private void OnlyNewer_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            if (chkOnlyNewer.IsChecked == true)
            {
                chkNoOverwrite.IsChecked = false;
            }

            UpdateCommandPreview();
        }

        private void FolderStructureOnly_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            if (chkFolderStructureOnly.IsChecked == true)
            {
                chkMirror.IsChecked = false;
                chkNoOverwrite.IsChecked = false;
                chkOnlyNewer.IsChecked = false;
                chkPermissions.IsChecked = false;
            }

            UpdateOptionAvailability();
            UpdateCommandPreview();
        }

        private void Options_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            UpdateCommandPreview();
        }

        private void Target_Changed(object sender, TextChangedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            UpdateCommandPreview();
        }

        private void RetriesWait_Changed(object sender, TextChangedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            UpdateCommandPreview();
        }

        private void UpdateOptionAvailability()
        {
            if (!_uiReady)
            {
                return;
            }

            if (chkSubdirs == null ||
                chkEmptyDirs == null ||
                chkMirror == null ||
                chkNoOverwrite == null ||
                chkOnlyNewer == null ||
                chkPermissions == null ||
                chkFolderStructureOnly == null)
            {
                return;
            }

            bool mirror = chkMirror.IsChecked == true;
            bool folderOnly = chkFolderStructureOnly.IsChecked == true;

            chkSubdirs.IsEnabled = !mirror && !folderOnly;
            chkEmptyDirs.IsEnabled = !mirror && !folderOnly && chkSubdirs.IsChecked == true;
            chkMirror.IsEnabled = !folderOnly;
            chkNoOverwrite.IsEnabled = !mirror && !folderOnly;
            chkOnlyNewer.IsEnabled = !mirror && !folderOnly;
            chkPermissions.IsEnabled = !folderOnly;
        }

        private void UpdateCommandPreview()
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            if (txtCommandPreview == null ||
                lstSources == null ||
                txtTarget == null ||
                chkUseSourceSubfolder == null ||
                chkMirror == null ||
                chkSubdirs == null ||
                chkEmptyDirs == null ||
                chkNoOverwrite == null ||
                chkOnlyNewer == null ||
                chkPermissions == null ||
                chkTestRun == null ||
                txtRetries == null ||
                txtWait == null ||
                chkFolderStructureOnly == null)
            {
                return;
            }

            var sb = new StringBuilder();

            string target = txtTarget.Text?.Trim().Trim('"') ?? "";

            if (lstSources.Items.Count == 0 || string.IsNullOrWhiteSpace(target))
            {
                txtCommandPreview.Text = "robocopy \"source\" \"target\" /COPY:DAT /R:1 /W:3";
                return;
            }

            foreach (string sourceObj in lstSources.Items)
            {
                try
                {
                    string source = Path.GetFullPath(Environment.ExpandEnvironmentVariables(sourceObj?.Trim().Trim('"') ?? ""));

                    string dest = target;

                    if (chkUseSourceSubfolder.IsChecked == true)
                    {
                        dest = Path.Combine(target, GetSourceFolderName(source));
                    }

                    sb.AppendLine("robocopy.exe " + BuildArguments(source, dest));
                }
                catch
                {
                    sb.AppendLine("robocopy.exe [invalid path]");
                }
            }

            txtCommandPreview.Text = sb.ToString();
        }

        private void Sources_DragOver(object sender, DragEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }

        private void Sources_Drop(object sender, DragEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
            {
                foreach (var path in paths)
                {
                    AddSourcePath(GetDirectoryFromPath(path));
                }

                UpdateCommandPreview();
            }
        }

        private void Target_DragOver(object sender, DragEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }

        private void Target_Drop(object sender, DragEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is string[] paths && paths.Length > 0)
            {
                var dir = GetDirectoryFromPath(paths[0]);

                if (!string.IsNullOrWhiteSpace(dir))
                {
                    txtTarget.Text = dir;
                }
            }
        }

        private static string GetDirectoryFromPath(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }

                if (Directory.Exists(path))
                {
                    return path;
                }

                if (File.Exists(path))
                {
                    return Path.GetDirectoryName(path);
                }
            }
            catch
            {
            }

            return null;
        }

        private void AddSource_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            string initial = lstSources.Items.Count > 0
                ? lstSources.Items[lstSources.Items.Count - 1] as string
                : txtTarget.Text;

            string path = BrowseFolder(initial, Loc.Current["FolderSelect"]);

            if (!string.IsNullOrWhiteSpace(path))
            {
                AddSourcePath(path);
                UpdateCommandPreview();
            }
        }

        private void RemoveSource_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            var selected = lstSources.SelectedItems.Cast<string>().ToList();

            foreach (var item in selected)
            {
                lstSources.Items.Remove(item);
            }

            UpdateCommandPreview();
        }

        private void ClearSources_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            lstSources.Items.Clear();
            UpdateCommandPreview();
        }

        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            string path = BrowseFolder(txtTarget.Text, Loc.Current["SelectTarget"]);

            if (!string.IsNullOrWhiteSpace(path))
            {
                txtTarget.Text = path;
            }
        }

        private string BrowseFolder(string initial, string description)
        {
            try
            {
                using var dlg = new WinForms.FolderBrowserDialog();
                dlg.Description = description;

                if (!string.IsNullOrWhiteSpace(initial) && Directory.Exists(initial))
                {
                    dlg.SelectedPath = initial;
                }

                return dlg.ShowDialog() == WinForms.DialogResult.OK
                    ? dlg.SelectedPath
                    : null;
            }
            catch
            {
                return null;
            }
        }

        private void AddSourcePath(string path)
        {
            if (lstSources == null || string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                path = Path.GetFullPath(path.Trim().Trim('"'));
            }
            catch
            {
                return;
            }

            if (!Directory.Exists(path))
            {
                return;
            }

            bool exists = lstSources.Items.Cast<string>()
                .Any(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                lstSources.Items.Add(path);
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            StartRun(false);
        }

        private void StartRun(bool fromScheduler)
        {
            if (!_uiReady || _isRunning)
            {
                return;
            }

            if (lstSources.Items.Count == 0)
            {
                ReportError(Loc.Current["NoSources"], fromScheduler);
                return;
            }

            string targetRaw = txtTarget.Text?.Trim().Trim('"') ?? "";

            if (string.IsNullOrWhiteSpace(targetRaw))
            {
                ReportError(Loc.Current["TargetMissing"], fromScheduler);
                return;
            }

            string target = Environment.ExpandEnvironmentVariables(targetRaw);

            try
            {
                target = Path.GetFullPath(target);
            }
            catch
            {
                ReportError(Loc.Current["InvalidPath"], fromScheduler);
                return;
            }

            if (!fromScheduler && chkMirror.IsChecked == true && chkTestRun.IsChecked != true)
            {
                var result = MessageBox.Show(
                    this,
                    Loc.Current["MirrorWarningText"],
                    Loc.Current["MirrorWarningTitle"],
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            if (!fromScheduler && chkPermissions.IsChecked == true && !IsAdministrator())
            {
                MessageBox.Show(
                    this,
                    Loc.Current["AdminHint"],
                    Loc.Current["Info"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            SaveSettings();

            _jobs.Clear();

            foreach (string sourceObj in lstSources.Items)
            {
                string sourceRaw = sourceObj?.Trim().Trim('"') ?? "";
                string source = Environment.ExpandEnvironmentVariables(sourceRaw);

                string fullSource;

                try
                {
                    fullSource = Path.GetFullPath(source);
                }
                catch
                {
                    ReportError(Loc.Current["InvalidPath"] + " " + sourceRaw, fromScheduler);
                    return;
                }

                if (!Directory.Exists(fullSource))
                {
                    ReportError(Loc.Current["SourceNotExist"] + " " + fullSource, fromScheduler);
                    return;
                }

                if (string.Equals(fullSource, target, StringComparison.OrdinalIgnoreCase))
                {
                    ReportError(Loc.Current["SourceTargetSame"], fromScheduler);
                    return;
                }

                string dest = target;

                if (chkUseSourceSubfolder.IsChecked == true)
                {
                    dest = Path.Combine(target, GetSourceFolderName(fullSource));
                }

                string sourceWithSlash = fullSource.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

                if (dest.StartsWith(sourceWithSlash, StringComparison.OrdinalIgnoreCase))
                {
                    ReportError(Loc.Current["TargetInsideSource"], fromScheduler);
                    return;
                }

                _jobs.Add(new RoboCopyJob
                {
                    Source = fullSource,
                    Target = dest
                });
            }

            if (_jobs.Count == 0)
            {
                return;
            }

            _totalJobs = _jobs.Count;
            _jobIndex = 0;
            _failedJobs = 0;
            _cancelRequested = false;
            _runStartTime = DateTime.Now;

            txtLog.Clear();

            OpenLogFile();
            SetUiRunning(true);

            AppendLog($"{Loc.Current["AppTitle"]} - {DateTime.Now}");

            if (chkTestRun.IsChecked == true)
            {
                AppendLog(Loc.Current["TestRun"]);
            }

            progressOverall.Value = 0;
            progressCurrent.Value = 0;
            progressOverall.IsIndeterminate = true;
            progressCurrent.IsIndeterminate = true;

            lblStatus.Text = Loc.Current["Running"];

            StartCurrentJob();
        }

        private void ReportError(string message, bool fromScheduler)
        {
            if (fromScheduler)
            {
                AppendLog("ERROR: " + message);

                if (_exitAfterRun && !_forceExit)
                {
                    _forceExit = true;
                    Close();
                }
            }
            else
            {
                MessageBox.Show(
                    this,
                    message,
                    Loc.Current["Error"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void OpenLogFile()
        {
            CloseLogWriter();
            _currentLogPath = null;

            try
            {
                if (chkAutoLog.IsChecked == true)
                {
                    _currentLogPath = Path.Combine(
                        _logFolder,
                        $"RobbyCopy_{DateTime.Now:yyyyMMdd_HHmmss}.log");

                    _logWriter = new StreamWriter(_currentLogPath, false, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };

                    AppendLog("Log: " + _currentLogPath);
                }
            }
            catch
            {
                _logWriter = null;
                _currentLogPath = null;
            }
        }

        private void CloseLogWriter()
        {
            try
            {
                lock (_logLock)
                {
                    _logWriter?.Flush();
                    _logWriter?.Dispose();
                    _logWriter = null;
                }
            }
            catch
            {
            }
        }

        private void StartCurrentJob()
        {
            if (_cancelRequested || _jobIndex >= _jobs.Count)
            {
                FinishRun();
                return;
            }

            var job = _jobs[_jobIndex];

            try
            {
                if (chkTestRun.IsChecked != true)
                {
                    Directory.CreateDirectory(job.Target);
                }
            }
            catch (Exception ex)
            {
                AppendLog("ERROR: " + ex.Message);
                _failedJobs++;
                _jobIndex++;
                StartCurrentJob();
                return;
            }

            string args = BuildArguments(job.Source, job.Target);

            AppendLog("");
            AppendLog($"=== {Loc.Current["Job"]} {_jobIndex + 1} {Loc.Current["Of"]} {_jobs.Count}: {job.Source} -> {job.Target} ===");
            AppendLog("robocopy.exe " + args);

            lblStatus.Text = $"{Loc.Current["Job"]} {_jobIndex + 1} {Loc.Current["Of"]} {_jobs.Count}";

            try
            {
                StartRoboCopyProcess(args);
            }
            catch (Exception ex)
            {
                AppendLog("ERROR: " + ex.Message);
                _failedJobs++;
                _jobIndex++;
                StartCurrentJob();
            }
        }

        private void StartRoboCopyProcess(string args)
        {
            _process = new Process();
            _process.StartInfo.FileName = "robocopy.exe";
            _process.StartInfo.Arguments = args;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.CreateNoWindow = true;

            var encoding = GetOemEncoding();
            _process.StartInfo.StandardOutputEncoding = encoding;
            _process.StartInfo.StandardErrorEncoding = encoding;

            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += Process_OutputDataReceived;
            _process.ErrorDataReceived += Process_ErrorDataReceived;
            _process.Exited += Process_Exited;

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        private static Encoding GetOemEncoding()
        {
            try
            {
                return Encoding.GetEncoding((int)GetOEMCP());
            }
            catch
            {
                return Encoding.Default;
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            if (e.Data.IndexOf('\b') >= 0)
            {
                var percent = TryParsePercent(e.Data);

                if (percent.HasValue)
                {
                    Ui(() => UpdateCurrentProgress(percent.Value));
                }

                return;
            }

            double? parsedPercent = TryParsePercent(e.Data);
            bool progressOnly = parsedPercent.HasValue && e.Data.Trim().Length <= 12;

            if (!progressOnly)
            {
                AppendLog(e.Data);
            }

            if (parsedPercent.HasValue)
            {
                Ui(() => UpdateCurrentProgress(parsedPercent.Value));
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                AppendLog("ERROR: " + e.Data);
            }
        }

        private double? TryParsePercent(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }

            var match = PercentRegex.Match(line);

            if (!match.Success)
            {
                return null;
            }

            string value = match.Groups["value"].Value.Replace(",", ".");

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                return Math.Clamp(result, 0, 100);
            }

            return null;
        }

        private void UpdateCurrentProgress(double value)
        {
            if (progressCurrent == null || progressOverall == null)
            {
                return;
            }

            if (progressCurrent.IsIndeterminate)
            {
                progressCurrent.IsIndeterminate = false;
                progressOverall.IsIndeterminate = false;
            }

            progressCurrent.Value = value;

            if (_totalJobs > 0)
            {
                double overall = (_jobIndex * 100.0 + value) / _totalJobs;
                progressOverall.Value = Math.Min(100, overall);
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            var p = sender as Process;
            int exitCode = -1;

            try
            {
                p?.WaitForExit();

                if (p != null)
                {
                    exitCode = p.ExitCode;
                }
            }
            catch
            {
            }

            bool cancelled = _cancelRequested;

            try
            {
                p?.Dispose();
            }
            catch
            {
            }

            Ui(() =>
            {
                _process = null;

                string message = cancelled
                    ? Loc.Current["RunCancelled"]
                    : EvaluateExitCode(exitCode);

                AppendLog($"RoboCopy exit code: {exitCode} - {message}");

                if (!cancelled && exitCode >= 8)
                {
                    _failedJobs++;
                }

                _jobIndex++;

                if (progressCurrent != null)
                {
                    progressCurrent.Value = 0;
                    progressCurrent.IsIndeterminate = true;
                }

                if (cancelled || _jobIndex >= _jobs.Count)
                {
                    FinishRun();
                }
                else
                {
                    StartCurrentJob();
                }
            });
        }

        private static string EvaluateExitCode(int code)
        {
            switch (code)
            {
                case 0:
                    return Loc.Current["Exit0"];

                case 1:
                    return Loc.Current["Exit1"];

                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    return Loc.Current["ExitInfo"];

                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    return Loc.Current["ExitError"];

                default:
                    if (code >= 16)
                    {
                        return Loc.Current["ExitFatal"];
                    }

                    return Loc.Current["ExitUnknown"];
            }
        }

        private void FinishRun()
        {
            if (!_isRunning)
            {
                return;
            }

            string summary;

            if (_cancelRequested)
            {
                summary = Loc.Current["RunCancelled"];
            }
            else if (_failedJobs > 0)
            {
                summary = $"{Loc.Current["RunCompleted"]} {Loc.Current["FailedJobs"]}: {_failedJobs}";
            }
            else
            {
                summary = Loc.Current["RunCompleted"];
            }

            lblStatus.Text = summary;
            AppendLog(summary);

            CloseLogWriter();

            AddHistory(summary);
            SendEmailNotification(summary);

            SetUiRunning(false);

            if (progressCurrent != null)
            {
                progressCurrent.Value = 0;
            }

            if (!_cancelRequested && _failedJobs == 0 && progressOverall != null)
            {
                progressOverall.Value = 100;
            }

            if (_exitAfterRun)
            {
                _forceExit = true;
                Close();
            }
        }

        private void SetUiRunning(bool running)
        {
            _isRunning = running;

            if (settingsPanel != null)
            {
                settingsPanel.IsEnabled = !running;
            }

            if (btnStart != null)
            {
                btnStart.IsEnabled = !running;
            }

            if (btnCancel != null)
            {
                btnCancel.IsEnabled = running;
            }

            if (!running)
            {
                if (progressCurrent != null)
                {
                    progressCurrent.IsIndeterminate = false;
                }

                if (progressOverall != null)
                {
                    progressOverall.IsIndeterminate = false;
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            _cancelRequested = true;

            try
            {
                if (_process != null && !_process.HasExited)
                {
                    _process.Kill();
                }
            }
            catch
            {
            }

            if (_process == null && _isRunning)
            {
                FinishRun();
            }

            if (lblStatus != null)
            {
                lblStatus.Text = Loc.Current["RunCancelled"];
            }
        }

        private string BuildArguments(string source, string target)
        {
            var sb = new StringBuilder();

            sb.Append(Quote(source));
            sb.Append(' ');
            sb.Append(Quote(target));

            if (chkFolderStructureOnly.IsChecked == true)
            {
                // Nur Ordnerstruktur: /E für alle Unterordner, /XF *.* um alle Dateien auszuschließen
                sb.Append(" /E /XF *.*");
            }
            else
            {
                if (chkMirror.IsChecked == true)
                {
                    sb.Append(" /MIR");
                }
                else if (chkSubdirs.IsChecked == true)
                {
                    sb.Append(chkEmptyDirs.IsChecked == true ? " /E" : " /S");
                }

                if (chkMirror.IsChecked != true)
                {
                    if (chkNoOverwrite.IsChecked == true)
                    {
                        sb.Append(" /XC /XN /XO");
                    }
                    else if (chkOnlyNewer.IsChecked == true)
                    {
                        sb.Append(" /XO");
                    }
                }

                if (chkPermissions.IsChecked == true)
                {
                    sb.Append(" /COPYALL");
                }
                else
                {
                    sb.Append(" /COPY:DAT");
                }
            }

            int retries = ParseInt(txtRetries.Text, 1);
            int wait = ParseInt(txtWait.Text, 3);

            sb.Append($" /R:{retries}");
            sb.Append($" /W:{wait}");

            if (chkTestRun.IsChecked == true)
            {
                sb.Append(" /L");
            }

            return sb.ToString();
        }

        private static string Quote(string path)
        {
            return "\"" + path.Trim().Trim('"') + "\"";
        }

        private static int ParseInt(string value, int fallback)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                if (result < 0)
                {
                    return fallback;
                }

                if (result > 1000)
                {
                    return 1000;
                }

                return result;
            }

            return fallback;
        }

        private static int ParsePort(string value, int fallback)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                if (result > 0 && result <= 65535)
                {
                    return result;
                }
            }

            return fallback;
        }

        private static string GetSourceFolderName(string source)
        {
            try
            {
                string trimmed = source.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string root = Path.GetPathRoot(trimmed) ?? "";

                if (string.IsNullOrWhiteSpace(trimmed) ||
                    string.Equals(trimmed, root.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                {
                    string drive = root.Replace(":", "").Replace("\\", "").Replace("/", "");
                    return SanitizeFileName(string.IsNullOrWhiteSpace(drive) ? "Root" : drive + "_Drive");
                }

                string name = Path.GetFileName(trimmed);

                if (string.IsNullOrWhiteSpace(name))
                {
                    var di = new DirectoryInfo(trimmed);
                    name = di.Name;
                }

                return SanitizeFileName(string.IsNullOrWhiteSpace(name) ? "Source" : name);
            }
            catch
            {
                return "Source";
            }
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Source";
            }

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c.ToString(), "_");
            }

            return string.IsNullOrWhiteSpace(name) ? "Source" : name;
        }

        private static bool IsAdministrator()
        {
            try
            {
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);

                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private void Schedule_Changed(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || _initializing || _loadingProfile)
            {
                return;
            }

            UpdateScheduleLabel();
        }

        private void UpdateScheduleLabel()
        {
            if (!_uiReady)
            {
                return;
            }

            if (chkScheduleEnabled == null ||
                cmbHour == null ||
                cmbMinute == null ||
                lblNextRun == null)
            {
                return;
            }

            if (chkScheduleEnabled.IsChecked == true && TryGetScheduleTime(out int hour, out int minute))
            {
                var now = DateTime.Now;
                var next = now.Date.AddHours(hour).AddMinutes(minute);

                if (next <= now)
                {
                    next = next.AddDays(1);
                }

                _nextScheduledRun = next;
                lblNextRun.Text = $"{Loc.Current["NextRun"]} {next.ToString("g")}";
            }
            else
            {
                _nextScheduledRun = null;
                lblNextRun.Text = "";
            }
        }

        private bool TryGetScheduleTime(out int hour, out int minute)
        {
            hour = 20;
            minute = 0;

            if (cmbHour.SelectedItem is ComboBoxItem h && h.Tag is int hh)
            {
                hour = hh;
            }
            else
            {
                return false;
            }

            if (cmbMinute.SelectedItem is ComboBoxItem m && m.Tag is int mm)
            {
                minute = mm;
            }
            else
            {
                return false;
            }

            return hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59;
        }

        private void SchedulerTimer_Tick(object sender, EventArgs e)
        {
            if (!_uiReady || _isRunning || chkScheduleEnabled.IsChecked != true || _nextScheduledRun == null)
            {
                return;
            }

            if (DateTime.Now >= _nextScheduledRun.Value)
            {
                AppendLog(Loc.Current["ScheduleRunStarted"]);

                bool daily = chkScheduleDaily.IsChecked == true;

                StartRun(true);

                if (!daily)
                {
                    chkScheduleEnabled.IsChecked = false;
                }

                UpdateScheduleLabel();
            }
        }

        private BackupProfile ReadProfileFromUi()
        {
            return new BackupProfile
            {
                Name = txtProfileName.Text?.Trim(),
                Sources = lstSources.Items.Cast<string>().ToList(),
                Target = txtTarget.Text?.Trim(),
                Preset = GetSelectedPresetTag(),

                IncludeSubdirs = chkSubdirs.IsChecked == true,
                IncludeEmptyDirs = chkEmptyDirs.IsChecked == true,
                Mirror = chkMirror.IsChecked == true,
                NoOverwrite = chkNoOverwrite.IsChecked == true,
                OnlyNewer = chkOnlyNewer.IsChecked == true,
                CopyPermissions = chkPermissions.IsChecked == true,
                UseSourceSubfolder = chkUseSourceSubfolder.IsChecked == true,
                FolderStructureOnly = chkFolderStructureOnly.IsChecked == true,
                TestRun = chkTestRun.IsChecked == true,

                Retries = ParseInt(txtRetries.Text, 1),
                WaitSeconds = ParseInt(txtWait.Text, 3),

                ScheduleEnabled = chkScheduleEnabled.IsChecked == true,
                ScheduleHour = GetComboInt(cmbHour, 20),
                ScheduleMinute = GetComboInt(cmbMinute, 0),
                ScheduleDaily = chkScheduleDaily.IsChecked == true
            };
        }

        private static int GetComboInt(ComboBox combo, int fallback)
        {
            if (combo?.SelectedItem is ComboBoxItem item && item.Tag is int value)
            {
                return value;
            }

            return fallback;
        }

        private void SelectHourMinute(int hour, int minute)
        {
            if (cmbHour == null || cmbMinute == null)
            {
                return;
            }

            foreach (ComboBoxItem item in cmbHour.Items)
            {
                if (item.Tag is int h && h == hour)
                {
                    cmbHour.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in cmbMinute.Items)
            {
                if (item.Tag is int m && m == minute)
                {
                    cmbMinute.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            var profile = ReadProfileFromUi();

            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                if (cmbProfiles.SelectedItem is BackupProfile selected)
                {
                    profile.Name = selected.Name;
                }
                else
                {
                    profile.Name = "Profile " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                }
            }

            UpsertProfile(profile);
            SaveProfilesToFile();
            ReloadProfileList(profile.Name);

            MessageBox.Show(
                this,
                Loc.Current["ProfileSaved"],
                Loc.Current["Info"],
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void UpsertProfile(BackupProfile profile)
        {
            var existing = _profiles.FirstOrDefault(p =>
                string.Equals(p.Name, profile.Name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                _profiles[_profiles.IndexOf(existing)] = profile;
            }
            else
            {
                _profiles.Add(profile);
            }
        }

        private void SaveProfilesToFile()
        {
            try
            {
                Directory.CreateDirectory(_appFolder);
                File.WriteAllText(_profilesPath, JsonSerializer.Serialize(_profiles, _jsonOptions));
            }
            catch
            {
            }
        }

        private void NewProfile_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            ResetUiToDefaults();
        }

        private void ResetUiToDefaults()
        {
            _loadingProfile = true;

            lstSources.Items.Clear();
            txtProfileName.Text = "";
            txtTarget.Text = _settings?.LastTarget ?? "";

            txtRetries.Text = "1";
            txtWait.Text = "3";

            chkScheduleEnabled.IsChecked = false;
            chkScheduleDaily.IsChecked = true;
            SelectHourMinute(20, 0);

            ApplyPreset("Simple", true, true);

            _loadingProfile = false;

            UpdateOptionAvailability();
            UpdateScheduleLabel();
            UpdateCommandPreview();
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            if (cmbProfiles.SelectedItem is BackupProfile profile)
            {
                var result = MessageBox.Show(
                    this,
                    Loc.Current["ConfirmDeleteText"],
                    Loc.Current["ConfirmDeleteTitle"],
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _profiles.Remove(profile);
                    SaveProfilesToFile();
                    ReloadProfileList(null);
                }
            }
        }

        private static (string FileName, string PrefixArgs) GetExecutableCommand()
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length >= 2 && args[1].EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                return (args[0], $"\"{args[1]}\"");
            }

            string exe = Environment.ProcessPath;

            if (string.IsNullOrWhiteSpace(exe))
            {
                try
                {
                    exe = Process.GetCurrentProcess().MainModule.FileName;
                }
                catch
                {
                    exe = "";
                }
            }

            return (exe, "");
        }

        private void ExportTask_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady)
            {
                return;
            }

            var profile = ReadProfileFromUi();

            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                MessageBox.Show(
                    this,
                    Loc.Current["ExportTaskNeedName"],
                    Loc.Current["Warning"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            UpsertProfile(profile);
            SaveProfilesToFile();
            ReloadProfileList(profile.Name);

            if (!TryGetScheduleTime(out int hour, out int minute))
            {
                hour = 20;
                minute = 0;
            }

            var command = GetExecutableCommand();
            string fileName = command.FileName;
            string prefixArgs = command.PrefixArgs;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show(
                    this,
                    Loc.Current["ExportTaskFailed"],
                    Loc.Current["Error"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            string safeName = SanitizeFileName(profile.Name);
            string taskName = $"RobbyCopy - {safeName}";

            string programArgs = string.IsNullOrWhiteSpace(prefixArgs)
                ? $"/profile \"{profile.Name}\" /run /minimized /exit"
                : $"{prefixArgs} /profile \"{profile.Name}\" /run /minimized /exit";

            string tr = $"\"{fileName}\" {programArgs}";

            try
            {
                var psi = new ProcessStartInfo("schtasks.exe")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                psi.ArgumentList.Add("/Create");
                psi.ArgumentList.Add("/TN");
                psi.ArgumentList.Add(taskName);
                psi.ArgumentList.Add("/TR");
                psi.ArgumentList.Add(tr);
                psi.ArgumentList.Add("/SC");
                psi.ArgumentList.Add("DAILY");
                psi.ArgumentList.Add("/ST");
                psi.ArgumentList.Add($"{hour:00}:{minute:00}");
                psi.ArgumentList.Add("/F");

                using var proc = Process.Start(psi);

                if (proc == null)
                {
                    MessageBox.Show(
                        this,
                        Loc.Current["ExportTaskFailed"],
                        Loc.Current["Error"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();

                proc.WaitForExit();

                if (proc.ExitCode == 0)
                {
                    MessageBox.Show(
                        this,
                        string.Format(Loc.Current["ExportTaskDone"], taskName),
                        Loc.Current["Info"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        this,
                        Loc.Current["ExportTaskFailed"] + "\n\n" + stderr,
                        Loc.Current["Error"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    Loc.Current["ExportTaskFailed"] + "\n\n" + ex.Message,
                    Loc.Current["Error"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(_historyPath))
                {
                    _history = JsonSerializer.Deserialize<List<HistoryEntry>>(File.ReadAllText(_historyPath))
                               ?? new List<HistoryEntry>();
                }
            }
            catch
            {
                _history = new List<HistoryEntry>();
            }
        }

        private void SaveHistory()
        {
            try
            {
                Directory.CreateDirectory(_appFolder);
                File.WriteAllText(_historyPath, JsonSerializer.Serialize(_history, _jsonOptions));
            }
            catch
            {
            }
        }

        private void AddHistory(string status)
        {
            try
            {
                var entry = new HistoryEntry
                {
                    Timestamp = DateTime.Now,
                    Profile = string.IsNullOrWhiteSpace(txtProfileName.Text) ? "Ad-hoc" : txtProfileName.Text.Trim(),
                    Status = status,
                    Jobs = _totalJobs,
                    FailedJobs = _failedJobs,
                    Duration = (DateTime.Now - _runStartTime).ToString(@"hh\:mm\:ss"),
                    LogFile = _currentLogPath ?? ""
                };

                _history.Insert(0, entry);

                if (_history.Count > 200)
                {
                    _history.RemoveRange(200, _history.Count - 200);
                }

                SaveHistory();

                if (historyGrid != null)
                {
                    historyGrid.ItemsSource = null;
                    historyGrid.ItemsSource = _history;
                }
            }
            catch
            {
            }
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!_uiReady || historyGrid == null)
            {
                return;
            }

            _history.Clear();
            SaveHistory();

            historyGrid.ItemsSource = null;
            historyGrid.ItemsSource = _history;
        }

        private void SendEmailNotification(string summary)
        {
            if (!_settings.EmailEnabled)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_settings.SmtpHost) ||
                string.IsNullOrWhiteSpace(_settings.EmailTo))
            {
                return;
            }

            string profile = string.IsNullOrWhiteSpace(txtProfileName.Text)
                ? "Ad-hoc"
                : txtProfileName.Text.Trim();

            string duration = (DateTime.Now - _runStartTime).ToString(@"hh\:mm\:ss");

            string subject = $"RobbyCopy: {summary}";

            var body = new StringBuilder();
            body.AppendLine(Loc.Current["AppTitle"]);
            body.AppendLine();
            body.AppendLine($"{Loc.Current["ProfileName"]}: {profile}");
            body.AppendLine($"{Loc.Current["Status"]}: {summary}");
            body.AppendLine($"{Loc.Current["Job"]}: {_totalJobs}");
            body.AppendLine($"{Loc.Current["FailedJobs"]}: {_failedJobs}");
            body.AppendLine($"{Loc.Current["HistoryDuration"]}: {duration}");
            body.AppendLine($"{Loc.Current["HistoryLog"]}: {_currentLogPath}");

#pragma warning disable SYSLIB0014
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort);
                    client.EnableSsl = _settings.EmailUseSsl;

                    if (!string.IsNullOrWhiteSpace(_settings.SmtpUser))
                    {
                        client.Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPassword);
                    }
                    else
                    {
                        client.UseDefaultCredentials = true;
                    }

                    string from = string.IsNullOrWhiteSpace(_settings.EmailFrom)
                        ? "RobbyCopy@localhost"
                        : _settings.EmailFrom;

                    using var message = new MailMessage(from, _settings.EmailTo, subject, body.ToString());
                    client.Send(message);
                }
                catch
                {
                }
            });
#pragma warning restore SYSLIB0014
        }

        private void OpenLogFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _logFolder,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show(
                    this,
                    Loc.Current["CannotOpenFolder"],
                    Loc.Current["Error"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void SaveLog_Click(object sender, RoutedEventArgs e)
        {
            if (txtLog == null)
            {
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*",
                FileName = $"RobbyCopy_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, txtLog.Text, Encoding.UTF8);

                    MessageBox.Show(
                        this,
                        Loc.Current["LogSaved"],
                        Loc.Current["Info"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        ex.Message,
                        Loc.Current["Error"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void AppendLog(string line)
        {
            if (line == null)
            {
                return;
            }

            WriteToFile(line);
            Ui(() => AppendLogUi(line));
        }

        private void AppendLogUi(string line)
        {
            try
            {
                if (!IsLoaded || txtLog == null)
                {
                    return;
                }

                txtLog.AppendText(line + Environment.NewLine);
                txtLog.ScrollToEnd();
            }
            catch
            {
            }
        }

        private void WriteToFile(string line)
        {
            if (_logWriter == null)
            {
                return;
            }

            try
            {
                lock (_logLock)
                {
                    _logWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line);
                }
            }
            catch
            {
            }
        }

        private void Ui(Action action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                if (Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    Dispatcher.BeginInvoke(action);
                }
            }
            catch
            {
            }
        }

        private Drawing.Icon CreateSealIcon()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/robben.png", UriKind.Absolute);
                var info = System.Windows.Application.GetResourceStream(uri);

                if (info != null)
                {
                    using (info.Stream)
                    {
                        using (var bmp = new Drawing.Bitmap(info.Stream))
                        {
                            using (var small = new Drawing.Bitmap(bmp, 32, 32))
                            {
                                return Drawing.Icon.FromHandle(small.GetHicon());
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private void InitializeTray()
        {
            try
            {
                if (_notifyIcon != null)
                {
                    return;
                }

                Drawing.Icon icon = CreateSealIcon();

                if (icon == null)
                {
                    try
                    {
                        icon = Drawing.SystemIcons.Application;
                    }
                    catch
                    {
                    }
                }

                if (icon == null)
                {
                    try
                    {
                        using (var bmp = new Drawing.Bitmap(16, 16))
                        {
                            using (var g = Drawing.Graphics.FromImage(bmp))
                            {
                                g.Clear(Drawing.Color.FromArgb(255, 0, 103, 192));
                            }

                            icon = Drawing.Icon.FromHandle(bmp.GetHicon());
                        }
                    }
                    catch
                    {
                    }
                }

                _notifyIcon = new WinForms.NotifyIcon();

                if (icon != null)
                {
                    _notifyIcon.Icon = icon;
                }

                _notifyIcon.Visible = true;

                var menu = new WinForms.ContextMenuStrip();

                _trayShowItem = new WinForms.ToolStripMenuItem();
                _trayRunItem = new WinForms.ToolStripMenuItem();
                _trayExitItem = new WinForms.ToolStripMenuItem();

                _trayShowItem.Click += (s, e) => Ui(() => ShowFromTray());
                _trayRunItem.Click += (s, e) => Ui(() => StartRun(true));
                _trayExitItem.Click += (s, e) => Ui(() => ExitFromTray());

                menu.Items.Add(_trayShowItem);
                menu.Items.Add(_trayRunItem);
                menu.Items.Add(new WinForms.ToolStripSeparator());
                menu.Items.Add(_trayExitItem);

                _notifyIcon.ContextMenuStrip = menu;
                _notifyIcon.DoubleClick += (s, e) => Ui(() => ShowFromTray());

                UpdateTrayText();
            }
            catch (Exception ex)
            {
                try
                {
                    File.AppendAllText(
                        Path.Combine(_appFolder, "crash.log"),
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " TRAY ERROR: " + ex + Environment.NewLine);
                }
                catch
                {
                }
            }
        }

        private void UpdateTrayText()
        {
            try
            {
                if (_notifyIcon == null)
                {
                    return;
                }

                _notifyIcon.Text = Loc.Current["AppTitle"];

                if (_trayShowItem != null)
                {
                    _trayShowItem.Text = Loc.Current["TrayShow"];
                }

                if (_trayRunItem != null)
                {
                    _trayRunItem.Text = Loc.Current["TrayRun"];
                }

                if (_trayExitItem != null)
                {
                    _trayExitItem.Text = Loc.Current["TrayExit"];
                }
            }
            catch
            {
            }
        }

        private void ShowFromTray()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void ExitFromTray()
        {
            _forceExit = true;
            Close();
        }

        private void CleanupTray()
        {
            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                }
            }
            catch
            {
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (!_uiReady)
            {
                base.OnStateChanged(e);
                return;
            }

            if (WindowState == WindowState.Minimized && chkMinimizeToTray?.IsChecked == true)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SaveSettings();

            if (!_forceExit && chkMinimizeToTray?.IsChecked == true)
            {
                e.Cancel = true;
                Hide();
                base.OnClosing(e);
                return;
            }

            if (_isRunning)
            {
                var result = MessageBox.Show(
                    this,
                    Loc.Current["ConfirmExitText"],
                    Loc.Current["ConfirmExitTitle"],
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    base.OnClosing(e);
                    return;
                }

                _cancelRequested = true;

                try
                {
                    _process?.Kill();
                }
                catch
                {
                }

                CloseLogWriter();
            }

            CleanupTray();

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            CleanupTray();

            try
            {
                _schedulerTimer?.Stop();
            }
            catch
            {
            }

            base.OnClosed(e);
        }
    }
}