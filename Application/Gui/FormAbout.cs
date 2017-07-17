using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetLogClient.Utility;

namespace NetLogClient.Gui
{
	internal partial class FormAbout : Form
	{
		internal FormAbout()
		{
			InitializeComponent();
			string html = EmbeddedResourceTextReader.GetFromResources("NetLogClient.About.htm");
			webBrowser1.DocumentText = html;
		}

		private void buttonOkay_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}