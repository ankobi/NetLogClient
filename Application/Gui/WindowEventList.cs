using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;


namespace NetLogClient.Gui
{
	[CLSCompliant(false)]
	public partial class WindowEventList : DockContent
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public event EventHandler EntrySelectionChange;
		private const string DATETIME_FORMATTER = "G";
		private const string LEVEL_TRACE = "TRACE";
		private const string LEVEL_DEBUG = "DEBUG";
		private const string LEVEL_WARN = "WARN";
		private const string LEVEL_INFO = "INFO";
		private const string LEVEL_ERROR = "ERROR";
		private const string LEVEL_FATAL = "FATAL";
		private const string LOGGER_NAME_SUFFIX = "@@@END@@@";
		private const string LOGGER_CONATINS_EXCEPTION_TRUE = "Yes";
		private const string LOGGER_CONATINS_EXCEPTION_FALSE = "";
		private const string PROPERTY_MACHINE_NAME_IDENTIFIER = "log4jmachinename";
		private const string PROPERTY_HOST_NAME_IDENTIFIER = "hostname";
		private const string PAREN_LEFT = "(";
		private const string PAREN_RIGHT = ")";
		private const string SPACE = " ";
		private static StringBuilder createListViewItemStringBuilder = new StringBuilder();
		private static bool _showUnrecognizedLevels = true;

		private DateTime _timeLastRefreshed = DateTime.Now;
		private List<ListViewItem> viewableItems = new List<ListViewItem>();
        private object viewableItemsLock = new Object();

		public WindowEventList()
		{
			InitializeComponent();
		}

		internal static DateTime ConvertDateTime(string dateString)
		{
			DateTime convertedDate;
			long javaCurrentTimeMillis = 0L;

			if (long.TryParse(dateString, out javaCurrentTimeMillis))
			{
				convertedDate = ConvertDateTimeFromJava(dateString);
			}
			else if (DateTime.TryParse(dateString, out convertedDate))
			{
				//do nothing, our out var is set
			}
			else
			{
				convertedDate = DateTime.Now;
			}

			return convertedDate;
		}

		private static DateTime ConvertDateTimeFromJava(string dateAsNumberOfMillisecondsSince1970)
		{
			//get date for 1970
			DateTime jan1970 = new DateTime(1970, 1, 1);

			//Add this many ms to it
			DateTime date = jan1970.AddMilliseconds(Convert.ToInt64(dateAsNumberOfMillisecondsSince1970));
			return date;
		}

		private static string GetMachineNameFromProperties(LogEntry entry)
		{
			for (int i = 0; i < entry.LogEvent.properties.Length; i++)
			{
				NetLogClient.Log4j.eventPropertiesData data = entry.LogEvent.properties[i];
				if (
					(data.name.ToLower() == PROPERTY_MACHINE_NAME_IDENTIFIER) ||
					(data.name.ToLower() == PROPERTY_HOST_NAME_IDENTIFIER)
					)
				{
					return data.value;
				}
			}
			return null;
		}


		private static ListViewItem CreateListviewItem(LogEntry logEntry)
		{
			if(logEntry.LogEvent==null) throw new ArgumentNullException("Invalid log entry.");

            string msg = string.Empty;
            if (logEntry.LogEvent.message !=null && logEntry.LogEvent.message.Text != null && logEntry.LogEvent.message.Text.Length > 0)
            {
                msg = string.Concat(logEntry.LogEvent.message.Text);
            }
			DateTime convertedDate = ConvertDateTime(logEntry.LogEvent.timestamp);
			string machineNameFromProperties = GetMachineNameFromProperties(logEntry);

			ListViewItem listViewItem = new ListViewItem(
				convertedDate.ToLocalTime().ToString(DATETIME_FORMATTER)
				);

			if (machineNameFromProperties != null)
			{
				createListViewItemStringBuilder.Remove(0,createListViewItemStringBuilder.Length);
				createListViewItemStringBuilder.Append(logEntry.SenderIP);
				createListViewItemStringBuilder.Append(SPACE);
				createListViewItemStringBuilder.Append(PAREN_LEFT);
				createListViewItemStringBuilder.Append(machineNameFromProperties);
				createListViewItemStringBuilder.Append(PAREN_RIGHT);
				listViewItem.SubItems.Add(createListViewItemStringBuilder.ToString());
			}
			else
			{
				listViewItem.SubItems.Add(logEntry.SenderIP);
			}
			listViewItem.SubItems.Add(logEntry.LogEvent.thread);
			listViewItem.SubItems.Add(logEntry.LogEvent.level);

			if (logEntry.LogEvent.throwable == null)
			{
				listViewItem.SubItems.Add(LOGGER_CONATINS_EXCEPTION_FALSE);
			}
			else
			{
				listViewItem.SubItems.Add(LOGGER_CONATINS_EXCEPTION_TRUE);
			}

			listViewItem.SubItems.Add(logEntry.LogEvent.logger);
			listViewItem.SubItems.Add(msg);

			// Link item to entry
			listViewItem.Tag = logEntry;

			return listViewItem;
		}

		private static ListViewItem ColorListviewItem(ListViewItem listViewItem, LogEntry logEntry)
		{
			// Color
			if (logEntry.LogEvent.level.IndexOf(LEVEL_TRACE) > -1)
			{
				listViewItem.BackColor = Color.FromArgb(255, 255, 255);
				listViewItem.ForeColor = Color.FromArgb(110, 110, 110);
			}
			if (logEntry.LogEvent.level.IndexOf(LEVEL_DEBUG) > -1)
			{
				//listViewItem.BackColor = Color.FromArgb(240, 240, 240);
				listViewItem.BackColor = Color.FromArgb(220, 220, 220);
				listViewItem.ForeColor = Color.FromArgb(20, 20, 255);
			}
			if (logEntry.LogEvent.level.IndexOf(LEVEL_INFO) > -1) listViewItem.ForeColor = Color.FromArgb(20, 20, 150);
			if (logEntry.LogEvent.level.IndexOf(LEVEL_WARN) > -1) listViewItem.BackColor = Color.FromArgb(255, 255, 180);
			if (logEntry.LogEvent.level.IndexOf(LEVEL_ERROR) > -1) listViewItem.BackColor = Color.FromArgb(255, 180, 180);
			if (logEntry.LogEvent.level.IndexOf(LEVEL_FATAL) > -1) listViewItem.BackColor = Color.FromArgb(255, 120, 120);

			return listViewItem;
		}

		internal void Refresh(LogEntry[] logEntryArray,
			string selectedComputer,
			bool selectedComputerHasChanged,
			string selectedLoggerName,
			bool selectedLoggerHasChanged,
			string selectedFilterName,
			bool selectedFilterHasChanged,
			WindowLoggerExplorer windowLoggerExplorer,
			WindowComputerExplorer windowComputerExplorer,
			bool useColor,
			bool selectedUseColorHasChanged,
			bool forcedRefresh)
		{
			try
			{
				if (
						(LogEntryManager.TimeCacheLastChanged <= _timeLastRefreshed) &&
						(!selectedLoggerHasChanged) &&
						(!selectedFilterHasChanged) &&
						(!selectedComputerHasChanged) &&
						(!selectedUseColorHasChanged) &&
						(!forcedRefresh)
					)
					return;

                lock (viewableItemsLock)
                {
                    viewableItems.Clear();

                    foreach (LogEntry logEntry in logEntryArray)
                    {
                        if (logEntry == null) continue;

                        string loggerName = (WindowLoggerExplorer.ROOT_LOGGER_NAME + "." + logEntry.LogEvent.logger);

                        if ((loggerName + LOGGER_NAME_SUFFIX).StartsWith(selectedLoggerName))
                        {
                            if (!FilteredByLevel(selectedFilterName, logEntry))
                            {
                                if (!FilteredByComputer(selectedComputer, logEntry))
                                {
                                    ListViewItem listViewItem = CreateListviewItem(logEntry);

                                    if (useColor)
                                        listViewItem = ColorListviewItem(listViewItem, logEntry);

                                    viewableItems.Insert(0, listViewItem);
                                    windowLoggerExplorer.AddIfNeeded(loggerName);
                                    windowComputerExplorer.AddIfNeeded(logEntry.SenderIP);
                                }
                            }
                        }
                    }

                    listView1.VirtualListSize = viewableItems.Count;
                    _timeLastRefreshed = DateTime.Now;

                    listView1_SelectedIndexChanged(this, null);
                    if (forcedRefresh) listView1.Refresh();  //if you enable or disable color, the listviews contents may not have changed so it is not redrawn even though you have recolored the entries.
                }
			}
			catch (ArgumentOutOfRangeException ex)
			{
				log.Warn("An ArgumentOutOfRangeException occured while updating the log event list: " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				log.Warn("An Exception occured while updating the log event list: " + ex.Message, ex);
			}
		}

		internal int Count
		{
			get { return viewableItems.Count; }
		}

		private static bool FilteredByComputer(string selectedComputer, LogEntry logEntry)
		{
			if (
				(selectedComputer == null) || 
				(selectedComputer.Trim().Length < 1) ||
				(selectedComputer == "*")
				) return false;
			if (logEntry.SenderIP == selectedComputer) return false;
			return true;
		}

		private static bool FilteredByLevel(string selectedFilter, LogEntry logEntry)
		{
			string level = logEntry.LogEvent.level.ToUpper();

			if (selectedFilter.ToUpper().Contains(level))
			{
				return false;
			}
			if (level == LEVEL_TRACE)
			{
				if (selectedFilter == FormMain.FILTER_TRACE_AND_GREATER)
				{
					return false;
                }
                else
                {
                    return true;
                }
			}
			if (level==LEVEL_DEBUG)
			{
                if (
                    (selectedFilter == FormMain.FILTER_TRACE_AND_GREATER) ||
                    (selectedFilter == FormMain.FILTER_DEBUG_AND_GREATER)
                    )
                {
                    return false;
                }
                else
                {
                    return true;
                }
			}
			if (level==LEVEL_INFO)
			{
				if (
					(selectedFilter == FormMain.FILTER_TRACE_AND_GREATER) ||
					(selectedFilter==FormMain.FILTER_DEBUG_AND_GREATER) ||
					(selectedFilter==FormMain.FILTER_INFO_AND_GREATER) 
					)
				{
					return false;
                }
                else
                {
                    return true;
                }
			}
			if (level == LEVEL_WARN)
			{
				if (
					(selectedFilter == FormMain.FILTER_TRACE_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_DEBUG_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_INFO_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_WARN_AND_GREATER)
					)
				{
					return false;
                }
                else
                {
                    return true;
                }
			}
			if (level == LEVEL_ERROR)
			{
				if (
					(selectedFilter == FormMain.FILTER_TRACE_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_DEBUG_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_INFO_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_WARN_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_ERROR_AND_GREATER)
					)
				{
					return false;
                }
                else
                {
                    return true;
                }
			}
			if (level == LEVEL_FATAL)
			{
				if (
					(selectedFilter == FormMain.FILTER_TRACE_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_DEBUG_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_INFO_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_WARN_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_ERROR_AND_GREATER) ||
					(selectedFilter == FormMain.FILTER_FATAL)
					)
				{
					return false;
                }
                else
                {
                    return true;
                }
			}

			return ! _showUnrecognizedLevels;
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{

			//fire event
			if (EntrySelectionChange != null) EntrySelectionChange(listView1, e);
		}

		internal void Clear()
		{
			listView1.Items.Clear();
		}

		private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
            lock (viewableItemsLock)
            {
                if (e.ItemIndex < 0 || e.ItemIndex > (viewableItems.Count - 1))
                {
                    log.Debug("A request was received to retrieve an item at index " + e.ItemIndex + "; There are " + viewableItems.Count + " items in the list.");
                    return;
                }
                e.Item = viewableItems[e.ItemIndex];
            }
		}

		internal ListViewItem RetrieveVirtualItem(int index)
		{
            lock (viewableItemsLock)
            {
                if (index < 0 || index > (viewableItems.Count - 1))
                {
                    log.Debug("A request was received to retrieve an item at index " + index + "; There are " + viewableItems.Count + " items in the list.");
                    return null;
                }
                return viewableItems[index];
            }
		}

		public static bool ShowUnrecognizedLevels
		{
			get { return _showUnrecognizedLevels; }
			set { _showUnrecognizedLevels = value; }
		}
	}
}
