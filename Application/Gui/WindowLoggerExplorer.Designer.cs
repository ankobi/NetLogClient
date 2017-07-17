namespace NetLogClient.Gui
{
	partial class WindowLoggerExplorer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowLoggerExplorer));
			this.listViewLogger = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// listViewLogger
			// 
			this.listViewLogger.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewLogger.HideSelection = false;
			this.listViewLogger.Location = new System.Drawing.Point(0, 0);
			this.listViewLogger.Name = "listViewLogger";
			this.listViewLogger.Size = new System.Drawing.Size(164, 327);
			this.listViewLogger.TabIndex = 0;
			this.listViewLogger.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.listViewLogger_AfterSelect);
			// 
			// WindowLoggerExplorer
			// 
			this.ClientSize = new System.Drawing.Size(164, 328);
			this.Controls.Add(this.listViewLogger);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
						| WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
			this.HideOnClose = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "WindowLoggerExplorer";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
			this.ShowInTaskbar = false;
			this.TabText = "Logger Explorer";
			this.Text = "Logger Explorer";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView listViewLogger;


	}
}
