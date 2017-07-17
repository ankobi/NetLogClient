using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;


namespace NetLogClient
{
	class ListViewAutoSize : ListView
	{
		private const int WM_PAINT = 0xf;


		protected override void WndProc(ref Message m)
		{
			// Work based on message
			switch (m.Msg)
			{
				//Resize that last darn column!
				//Credit: http://www.thecodeproject.com/cs/miscctrl/listviewautosize.asp
				case WM_PAINT:
					if (this.View == View.Details && this.Columns.Count > 0)
						this.Columns[this.Columns.Count - 1].Width = -2;
					break;
			}

			// Pass to base
			base.WndProc(ref m);
		}
	}
}
