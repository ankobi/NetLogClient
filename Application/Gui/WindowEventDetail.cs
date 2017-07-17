using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetLogClient.Gui
{
	[CLSCompliant(false)]
	public partial class WindowEventDetail : DockContent
	{
		private LogEntry _currentEntry = null;
		private const string DEFAULT_VALUE = "<NULL>";

		public WindowEventDetail()
		{
			InitializeComponent();
		}

		internal void SetEntry(LogEntry entry){

			Clear();

			 if (entry != null && entry.LogEvent!=null)
			{
				_currentEntry = entry;
				string logger = (entry.LogEvent.logger!=null) ? entry.LogEvent.logger : string.Empty ;
				string throwable = (entry.LogEvent.throwable != null) ? string.Concat(entry.LogEvent.throwable.Text) : string.Empty;
				DateTime date = WindowEventList.ConvertDateTime(entry.LogEvent.timestamp);

				if (entry.LogEvent.locationInfo != null)
				{
					labelValueClass.Text = entry.LogEvent.locationInfo[0].@class;
					labelValueFile.Text = entry.LogEvent.locationInfo[0].file;
					labelValueMethod.Text = entry.LogEvent.locationInfo[0].method;
					labelValueLine.Text = entry.LogEvent.locationInfo[0].line;

					SetToolTips();

				} 

				labelValueThread.Text = entry.LogEvent.thread;
				
				//labelProperties = entry.LogEvent.pr;
				labelValueLevel.Text = entry.LogEvent.level;
				labelValueLogger.Text = entry.LogEvent.logger;
				labelValueThread.Text = entry.LogEvent.thread;
				labelValueTime.Text = date.ToLocalTime().ToString();

				textBoxMessage.Text = Sanitize(string.Concat(entry.LogEvent.message.Text));
				textBoxException.Text = Sanitize(throwable);
			}
		}

		private void SetToolTips()
		{
			toolTip1.SetToolTip(labelValueClass, labelValueClass.Text);
			toolTip1.SetToolTip(labelValueFile, labelValueFile.Text);
			toolTip1.SetToolTip(labelValueMethod, labelValueMethod.Text);
			toolTip1.SetToolTip(labelValueLine, labelValueLine.Text);
		}

		internal void Clear()
		{
			labelValueThread.Text = DEFAULT_VALUE;
			labelValueLevel.Text = DEFAULT_VALUE;
			labelValueLogger.Text = DEFAULT_VALUE;
			labelValueThread.Text = DEFAULT_VALUE;
			labelValueTime.Text = DEFAULT_VALUE;
			textBoxMessage.Text = DEFAULT_VALUE;
			textBoxException.Text = DEFAULT_VALUE;

			labelValueClass.Text = DEFAULT_VALUE;
			labelValueFile.Text = DEFAULT_VALUE;
			labelValueMethod.Text = DEFAULT_VALUE;
			labelValueLine.Text = DEFAULT_VALUE;

			SetToolTips();
		}

		private static string Sanitize(string text)
		{
			text = text.Replace("\r\n", Environment.NewLine);
			text = text.Replace("\r", Environment.NewLine);
			text = text.Replace("\n", Environment.NewLine);

			return text;
		}

	}
}
