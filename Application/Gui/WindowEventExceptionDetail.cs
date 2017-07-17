using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetLogClient.Gui
{
	[CLSCompliant(false)]
	public partial class WindowEventExceptionDetail : DockContent
	{
		private LogEntry _currentEntry = null;

		public WindowEventExceptionDetail()
		{
			InitializeComponent();
		}

		internal void SetEntry(LogEntry entry)
		{
			_currentEntry = entry;
			if (_currentEntry.LogEvent.throwable != null)
			{
				string text = string.Concat(_currentEntry.LogEvent.throwable.Text);
				text = text.Replace("\r", Environment.NewLine);
				text = text.Replace("\n", Environment.NewLine);
				textBoxException.Text = text;
			}
			else
			{
				Clear();
			}
		}

		internal void Clear()
		{
			textBoxException.Clear(); 
		}

		private void toolStripButtonCopyAsXML_Click(object sender, EventArgs e)
		{
			if (_currentEntry == null) return;
			if (_currentEntry.LogEvent == null) return;
			if (_currentEntry.LogEvent.throwable == null) return;
			if (_currentEntry.LogEvent.throwable.Text == null) return;
			Clipboard.SetText(string.Concat(_currentEntry.LogEvent.throwable.Text));
		}
	}
}
