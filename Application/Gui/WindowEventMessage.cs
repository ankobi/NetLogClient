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
	public partial class WindowEventMessage : DockContent
	{
		private LogEntry _currentEntry = null;

		public WindowEventMessage()
		{
			InitializeComponent();
		}

		internal void SetEntry(LogEntry entry)
		{
			_currentEntry = entry;
			textBoxMessage.Text = string.Concat(_currentEntry.LogEvent.message.Text);
		}

		private void toolStripButtonCopyAsXML_Click(object sender, EventArgs e)
		{
			if (_currentEntry == null) return;
			Clipboard.SetText(string.Concat(_currentEntry.LogEvent.message.Text));
		}

		internal void Clear()
		{
			textBoxMessage.Clear();
		}
	}
}
