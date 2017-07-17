using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetLogClient.Gui
{
	[CLSCompliant(false)]
	public partial class WindowEventXml : DockContent
	{
		private LogEntry _currentEntry = null;

		public WindowEventXml()
		{
			InitializeComponent();
		}

		private void toolStripButtonCopyAsXML_Click(object sender, EventArgs e)
		{
			if (_currentEntry == null) return;
			Clipboard.SetText(textBoxXml.Text);
		}

		internal void SetEntry(LogEntry entry)
		{
			_currentEntry = entry;
			string xml = Beautify(_currentEntry.RawMessage);
			textBoxXml.Text = xml;
		}

		private static string Beautify(string xml)
		{
			string outXml = null;
			StringWriter stringWriter = new StringWriter();
			try
			{
				TextReader stringReader = new StringReader(xml);
				XmlDocument dom = new XmlDocument();
				dom.Load(stringReader);
				dom.Save(stringWriter);
			}
			catch
			{
				outXml = xml;
			}

			outXml = stringWriter.ToString();
			return outXml;
		}

		internal void Clear()
		{
			textBoxXml.Clear();
		}

	}
}
