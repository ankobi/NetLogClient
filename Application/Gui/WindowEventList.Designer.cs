namespace NetLogClient.Gui
{
	partial class WindowEventList
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowEventList));
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnTimestamp = new System.Windows.Forms.ColumnHeader();
			this.columnMachine = new System.Windows.Forms.ColumnHeader();
			this.columnThread = new System.Windows.Forms.ColumnHeader();
			this.columnLevel = new System.Windows.Forms.ColumnHeader();
			this.columnException = new System.Windows.Forms.ColumnHeader();
			this.columnLogger = new System.Windows.Forms.ColumnHeader();
			this.columnMessage = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.AllowColumnReorder = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnTimestamp,
            this.columnMachine,
            this.columnThread,
            this.columnLevel,
            this.columnException,
            this.columnLogger,
            this.columnMessage});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HideSelection = false;
			this.listView1.LabelWrap = false;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(898, 590);
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.VirtualMode = true;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			this.listView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView1_RetrieveVirtualItem);
			// 
			// columnTimestamp
			// 
			this.columnTimestamp.Text = "Timestamp";
			this.columnTimestamp.Width = 129;
			// 
			// columnMachine
			// 
			this.columnMachine.Text = "Machine";
			this.columnMachine.Width = 109;
			// 
			// columnThread
			// 
			this.columnThread.Text = "Thread";
			this.columnThread.Width = 53;
			// 
			// columnLevel
			// 
			this.columnLevel.Text = "Level";
			// 
			// columnException
			// 
			this.columnException.Text = "Exception";
			// 
			// columnLogger
			// 
			this.columnLogger.Text = "Logger";
			this.columnLogger.Width = 137;
			// 
			// columnMessage
			// 
			this.columnMessage.Text = "Message";
			this.columnMessage.Width = 346;
			// 
			// WindowEventList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(898, 590);
			this.Controls.Add(this.listView1);
			this.DoubleBuffered = true;
			this.HideOnClose = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "WindowEventList";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			this.ShowInTaskbar = false;
			this.TabText = "Log Entries";
			this.Text = "Log Entries";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnTimestamp;
		private System.Windows.Forms.ColumnHeader columnLevel;
		private System.Windows.Forms.ColumnHeader columnException;
		private System.Windows.Forms.ColumnHeader columnMachine;
		private System.Windows.Forms.ColumnHeader columnThread;
		private System.Windows.Forms.ColumnHeader columnLogger;
		private System.Windows.Forms.ColumnHeader columnMessage;
	}
}
