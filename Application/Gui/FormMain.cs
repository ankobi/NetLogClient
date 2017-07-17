using NetLogClient.Properties;
using System;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace NetLogClient.Gui
{
    public partial class FormMain : System.Windows.Forms.Form
    {
        #region Member Variables

        internal const string FILTER_DEBUG = "== Debug";
        internal const string FILTER_DEBUG_AND_GREATER = ">= Debug";
        internal const string FILTER_ERROR = "== Error";
        internal const string FILTER_ERROR_AND_GREATER = ">= Error";
        internal const string FILTER_FATAL = "== Fatal";
        internal const string FILTER_INFO = "== Info";
        internal const string FILTER_INFO_AND_GREATER = ">= Info";
        internal const string FILTER_TRACE = "== Trace";
        internal const string FILTER_TRACE_AND_GREATER = ">= Trace";
        internal const string FILTER_WARN = "== Warn";
        internal const string FILTER_WARN_AND_GREATER = ">= Warn";
        private const string STATUSBAR_CACHE_PREFIX = "% Cache Used: ";
        private const string STATUSBAR_ENTRIESCACHED_PREFIX = "Entries Cached: ";
        private const string STATUSBAR_ENTRIESDISPLAYED_PREFIX = "Entries Displayed: ";
        private const string STATUSBAR_STATUS_PREFIX = "Status: ";
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private NetLogClientConfig _config = new NetLogClientConfig();
        private DeserializeDockContent _deserializeDockContent;
        private string _lastSelectedComputer = string.Empty;
        private string _lastSelectedFilter = string.Empty;
        private string _lastSelectedLogger = WindowLoggerExplorer.ROOT_LOGGER_NAME;
        private bool _lastSelectedUseColor = true;
        private UdpListener _listener;
        private DateTime _timeLastEntryAddedToDisplay = DateTime.Now;
        private bool _useColor = true;
        private WindowComputerExplorer _windowComputerExplorer = new WindowComputerExplorer();
        private WindowEventDetail _windowEventDetail = new WindowEventDetail();
        private WindowEventExceptionDetail _windowEventException = new WindowEventExceptionDetail();
        private WindowEventList _windowEventList = new WindowEventList();
        private WindowEventMessage _windowEventMessage = new WindowEventMessage();
        private WindowEventPropertyList _windowEventProperties = new WindowEventPropertyList();
        private WindowEventXml _windowEventXml = new WindowEventXml();
        private WindowLoggerExplorer _windowLoggerExplorer = new WindowLoggerExplorer();
        private bool _collectionIsPaused = false;
        private bool _displayIsPaused = false;

        #endregion Member Variables

        #region Constructors

        public FormMain()
        {
            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            InitializeComponent();
            InitializeDisplayFilterDropDown();
            InitializeCacheThresholdDropDown();
            ApplyUserSettings();

            _listener = new UdpListener(_config.Port);
            _listener.Start();

            LogEntryManager.CacheSize = _config.CacheSize;

            timerRefresh.Interval = _config.RefreshIntervalMilliseconds;
            timerRefresh.Tick += new EventHandler(TimerEventProcessor);
            timerRefresh.Start();

            _windowEventList.EntrySelectionChange += new EventHandler(this.Entry_Selection_Change);
            _windowComputerExplorer.HostSelectionChange += new EventHandler(this.Host_Selection_Change);

            _log.Info(string.Format("Initializing NetLogClient (version {0}; Framework version {1})", GetAssemblyVersion(), Environment.Version.ToString()));

            #region Production Debug Warning

            //This will give a false positive if the "Define DEBUG Constant"
            //option is enabled under project settings
#if(DEBUG)
            _log.Warn("This code is compiled with debugging enabled.  This can cause serious degradation of performance.  If this is a production environment, please request a new build that targets the RELEASE configuration.  Executing assembly = " + System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
#endif

            #endregion Production Debug Warning
        }

        #endregion Constructors

        #region Methods

        internal static string GetAssemblyVersion()
        {
            System.Version vs = new System.Version(System.Windows.Forms.Application.ProductVersion);
            return vs.ToString(4);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        private void ApplyUserSettings()
        {
            lockLayoutToolStripMenuItem.Checked = UserSettings.Default.LockLayout;
            dockPanel.AllowEndUserDocking = !UserSettings.Default.LockLayout;

            toolStripComboBoxFilter.SelectedIndex = UserSettings.Default.DisplayFilterIndex;

            toolStripComboBoxCacheThreshold.SelectedIndex = UserSettings.Default.CacheThresholdIndex;
        }

        private void SaveUserSettings()
        {
            UserSettings.Default.DisplayFilterIndex = toolStripComboBoxFilter.SelectedIndex;
            UserSettings.Default.LockLayout = !dockPanel.AllowEndUserDocking;
            UserSettings.Default.CacheThresholdIndex = toolStripComboBoxCacheThreshold.SelectedIndex;


            UserSettings.Default.Save();
        }

        private void ClearEventDetails()
        {
            _windowEventDetail.Clear();
            _windowEventException.Clear();
            _windowEventProperties.Clear();
            _windowEventMessage.Clear();
            _windowEventXml.Clear();
        }

        private void DeleteAllFromCache()
        {
            _windowEventList.Clear();
            LogEntryManager.Clear();
            _windowComputerExplorer.Clear();
            _windowLoggerExplorer.Clear();

            ClearEventDetails();
            RefreshListWindow(true);

            _log.Debug("Cache emptied....");
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(WindowLoggerExplorer).ToString())
                return _windowLoggerExplorer;
            else if (persistString == typeof(WindowEventList).ToString())
                return _windowEventList;
            else if (persistString == typeof(WindowEventDetail).ToString())
                return _windowEventDetail;
            else if (persistString == typeof(WindowEventXml).ToString())
                return _windowEventXml;
            else if (persistString == typeof(WindowEventPropertyList).ToString())
                return _windowEventProperties;
            else if (persistString == typeof(WindowEventMessage).ToString())
                return _windowEventMessage;
            else if (persistString == typeof(WindowComputerExplorer).ToString())
                return _windowComputerExplorer;
            else if (persistString == typeof(WindowEventExceptionDetail).ToString())
                return _windowEventException;
            else
            {
                _log.Error(string.Format("Could not load default layout for a window: {0}", persistString));
                return null;
            }
        }

        private void InitializeCacheThresholdDropDown()
        {
            toolStripComboBoxCacheThreshold.SelectedIndex = 0;
        }

        private void InitializeDisplayFilterDropDown()
        {
            toolStripComboBoxFilter.BeginUpdate();
            toolStripComboBoxFilter.Items.Add(FILTER_TRACE);
            toolStripComboBoxFilter.Items.Add(FILTER_TRACE_AND_GREATER);
            toolStripComboBoxFilter.Items.Add(FILTER_DEBUG);
            toolStripComboBoxFilter.Items.Add(FILTER_DEBUG_AND_GREATER);
            toolStripComboBoxFilter.Items.Add(FILTER_INFO);
            toolStripComboBoxFilter.Items.Add(FILTER_INFO_AND_GREATER);
            toolStripComboBoxFilter.Items.Add(FILTER_WARN);
            toolStripComboBoxFilter.Items.Add(FILTER_WARN_AND_GREATER);
            toolStripComboBoxFilter.Items.Add(FILTER_ERROR);
            toolStripComboBoxFilter.Items.Add(FILTER_ERROR_AND_GREATER);
            toolStripComboBoxFilter.Items.Add(FILTER_FATAL);


            toolStripComboBoxFilter.SelectedIndex = 5;
            _lastSelectedFilter = (string)toolStripComboBoxFilter.SelectedItem;

            toolStripComboBoxFilter.EndUpdate();
        }

        private void lockLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
        }

        private void SetDefaultGui()
        {
            _log.Debug("No GUI layout settings found; Using defaults.");
            _windowEventList.Show(dockPanel);
            _windowEventDetail.Show(dockPanel);
            _windowEventProperties.Show(dockPanel);
            _windowEventMessage.Show(dockPanel);
            _windowEventException.Show(dockPanel);
            _windowEventXml.Show(dockPanel);
            _windowLoggerExplorer.Show(dockPanel);
            _windowComputerExplorer.Show(_windowLoggerExplorer.Pane, DockAlignment.Bottom, 0.5);

            //make this window the active top most tab
            _windowEventDetail.Show();
        }

        private void toolStripComboBoxCacheThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItemText = (string)toolStripComboBoxCacheThreshold.SelectedItem;
            LogEntryManager.Threshold = (LogEntryManager.ThresholdLevel)Enum.Parse(typeof(LogEntryManager.ThresholdLevel), selectedItemText.ToUpper());
        }

        #endregion Methods

        #region Event Handlers

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout about = new FormAbout();
            about.ShowDialog(this);
        }

        private void computerExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowComputerExplorer.Show(dockPanel);
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllFromCache();
        }

        private void Entry_Selection_Change(object sender, EventArgs e)
        {
            ListView view = (ListView)sender;

            //if (view.SelectedItems.Count > 0)
            //{
            //ListViewItem item = view.SelectedItems[0];
            if (view.SelectedIndices.Count > 0)
            {
                ListViewItem item = _windowEventList.RetrieveVirtualItem(view.SelectedIndices[0]);
                if (item != null)
                {
                    LogEntry entry = (LogEntry)item.Tag;
                    if (entry != null)
                    {
                        _windowEventDetail.SetEntry(entry);
                        _windowEventXml.SetEntry(entry);
                        _windowEventMessage.SetEntry(entry);
                        _windowEventException.SetEntry(entry);
                        _windowEventProperties.SetEntry(entry);
                    }
                }
            }
            //}
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _listener.Stop();
            _listener = null;
            Application.Exit();
        }

        private void Host_Selection_Change(object sender, EventArgs e)
        {
            this.RefreshListWindow(true);
        }

        private void logEntryDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowEventDetail.Show(dockPanel);
        }

        private void logEntryExceptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowEventException.Show(dockPanel);
        }

        private void logEntryListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowEventList.Show(dockPanel);
        }

        private void logEntryMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowEventMessage.Show(dockPanel);
        }

        private void logEntryPropertiesWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowEventProperties.Show(dockPanel);
        }

        private void logEntryXMLWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowEventXml.Show(dockPanel);
        }

        private void loggerExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowLoggerExplorer.Show(dockPanel);
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string configFile = Path.Combine(Application.UserAppDataPath, "DockPanel.config");
            dockPanel.SaveAsXml(configFile);
            SaveUserSettings();
            _listener.Stop();
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            string configFile = Path.Combine(Application.UserAppDataPath, "DockPanel.config");

            if (File.Exists(configFile))
            {
                _log.Debug("GUI settings found; Attempting to load...");
                dockPanel.LoadFromXml(configFile, _deserializeDockContent);
                _log.Info("Successfully loaded previously used GUI layout.");
            }
            else
                SetDefaultGui();
        }

        private void RefreshListWindow()
        {
            RefreshListWindow(false);
        }

        private void RefreshListWindow(bool forcedRefresh)
        {
            string currentlySelectedLogger = _windowLoggerExplorer.SelectedNodePath;
            string currentlySelectedComputer = _windowComputerExplorer.SelectedComputer;
            string currentlySelectedFilter = _lastSelectedFilter;

            if (!toolStripComboBoxFilter.DroppedDown)
            {
                currentlySelectedFilter = (string)toolStripComboBoxFilter.SelectedItem;
            }

            int count = LogEntryManager.LogEntryList.Count;
            bool selectedLoggerHasChanged = !(_lastSelectedLogger.Equals(currentlySelectedLogger));
            bool selectedFilterHasChanged = !(_lastSelectedFilter.Equals(currentlySelectedFilter));
            bool selectedComputerHasChanged = !(_lastSelectedComputer.Equals(currentlySelectedComputer));
            bool selectedUseColorHasChanged = !(_useColor == _lastSelectedUseColor);
            bool refresh = false;
            bool cacheHasChanged = _timeLastEntryAddedToDisplay < LogEntryManager.TimeCacheLastChanged;

            //if the display is paused, only update if the selected logger changed or the filter changed
            if (
                    (selectedLoggerHasChanged) ||
                    (selectedFilterHasChanged) ||
                    (selectedUseColorHasChanged) ||
                    (!_displayIsPaused && cacheHasChanged) ||
                    forcedRefresh
                )
            {
                refresh = true;
            }

            LogEntry[] logEntryArray = new LogEntry[] { };
            lock (LogEntryManager.LogEntryList)
            {
                logEntryArray = LogEntryManager.LogEntryList.ToArray();
            }

            if (refresh)
            {
                try
                {
                    _windowEventList.Refresh(
                        logEntryArray,
                        currentlySelectedComputer,
                        selectedComputerHasChanged,
                        currentlySelectedLogger,
                        selectedLoggerHasChanged,
                        currentlySelectedFilter,
                        selectedFilterHasChanged,
                        _windowLoggerExplorer,
                        _windowComputerExplorer,
                        _useColor,
                        selectedUseColorHasChanged,
                        forcedRefresh);
                    _timeLastEntryAddedToDisplay = DateTime.Now;
                    _lastSelectedLogger = _windowLoggerExplorer.SelectedNodePath;
                    _lastSelectedComputer = _windowComputerExplorer.SelectedComputer;
                    _lastSelectedUseColor = _useColor;
                    _lastSelectedFilter = (string)toolStripComboBoxFilter.SelectedItem;
                }
                catch (System.ArgumentException ex)
                {
                    _log.Warn("ArgumentException Caught; Failed to refresh the Event List: " + ex.Message, ex);
                }
            }
            else
            {
                try
                {
                    foreach (LogEntry logEntry in logEntryArray)
                    {
                        string loggerName = (WindowLoggerExplorer.ROOT_LOGGER_NAME + "."
                            + logEntry.LogEvent.logger);
                        _windowLoggerExplorer.AddIfNeeded(loggerName);
                        _windowComputerExplorer.AddIfNeeded(logEntry.SenderIP);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _log.Warn("An ArgumentOutOfRangeException occured while updating the log event list: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    _log.Warn("An Exception occured while updating the log event list: " + ex.Message, ex);
                }

                //log.Info("Successfully refreshed the display.");
            }
        }

        private void RefreshStatusBar()
        {
            int count = LogEntryManager.LogEntryList.Count;
            decimal percentCacheUsed = 100 * (decimal)count / (decimal)LogEntryManager.CacheSize;
            toolStripStatusLabelEntriesCached.Text = STATUSBAR_ENTRIESCACHED_PREFIX + count;
            toolStripStatusLabelCacheFilled.Text = STATUSBAR_CACHE_PREFIX + (percentCacheUsed.ToString("N0"));
            toolStripStatusLabelEntriesDisplayed.Text = STATUSBAR_ENTRIESDISPLAYED_PREFIX + _windowEventList.Count;
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            timerRefresh.Stop();
            //log.Debug("Attempting to refresh the display...");
            try
            {
                RefreshListWindow();
                RefreshStatusBar();
            }
            finally
            {
                timerRefresh.Start();
            }
        }

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            aboutToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButtonClearCache_Click(object sender, EventArgs e)
        {
            DeleteAllFromCache();
        }

        private void toolStripButtonColor_Click(object sender, EventArgs e)
        {
            _useColor = toolStripButtonColor.Checked;
            RefreshListWindow(true);
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            exitToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButtonPauseCollection_Click(object sender, EventArgs e)
        {
            _collectionIsPaused = !_collectionIsPaused;
            LogEntryManager.CollectionIsPaused = _collectionIsPaused;

            if (_collectionIsPaused)
            {
                toolStripStatusLabelIsPaused.Text = STATUSBAR_STATUS_PREFIX + "Stopped";
            }
            else if (_displayIsPaused)
            {
                toolStripStatusLabelIsPaused.Text = STATUSBAR_STATUS_PREFIX + "Display Locked";
            }
            else
            {
                toolStripStatusLabelIsPaused.Text = STATUSBAR_STATUS_PREFIX + "Active";
            }
        }

        private void toolStripButtonPauseDisplay_Click(object sender, EventArgs e)
        {
            _displayIsPaused = !_displayIsPaused;

            if (_displayIsPaused)
                _log.Info("The display process is paused.  New events will be added to the cache but not shown until the display is resumed.");
            else
                _log.Info("The display process has been resumed.  New events will be added to the display.");

            if (_displayIsPaused)
            {
                toolStripStatusLabelIsPaused.Text = STATUSBAR_STATUS_PREFIX + "Display Locked";
            }
            else
            {
                toolStripStatusLabelIsPaused.Text = STATUSBAR_STATUS_PREFIX + "Active";
            }
            if (_collectionIsPaused)
            {
                toolStripStatusLabelIsPaused.Text = STATUSBAR_STATUS_PREFIX + "Stopped";
            }
        }

        private void toolStripComboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshListWindow(true);
        }

        #endregion Event Handlers
    }
}