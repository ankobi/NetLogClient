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
	public partial class WindowEventPropertyList : DockContent
	{
		public WindowEventPropertyList()
		{
			InitializeComponent();
		}

		internal void Clear()
		{
			listViewProperties.Items.Clear();
		}

		internal void SetEntry(LogEntry entry)
		{
			Clear();

			if (entry.LogEvent.properties != null)
			{
				listViewProperties.BeginUpdate();
				for (int i = 0; i < entry.LogEvent.properties.Length; i++)
				{
					NetLogClient.Log4j.eventPropertiesData data = entry.LogEvent.properties[i];
					ListViewItem item = new ListViewItem(data.name);
					item.SubItems.Add(data.value);
					listViewProperties.Items.Add(item);
				}

				listViewProperties.Sort();

				listViewProperties.EndUpdate();
			}
		}
	}
}
