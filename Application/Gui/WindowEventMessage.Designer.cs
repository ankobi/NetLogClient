namespace NetLogClient.Gui
{
	partial class WindowEventMessage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowEventMessage));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonCopyAsXML = new System.Windows.Forms.ToolStripButton();
			this.textBoxMessage = new System.Windows.Forms.TextBox();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCopyAsXML});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolStrip1.Size = new System.Drawing.Size(435, 29);
			this.toolStrip1.TabIndex = 5;
			this.toolStrip1.Text = "toolStrip";
			// 
			// toolStripButtonCopyAsXML
			// 
			this.toolStripButtonCopyAsXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCopyAsXML.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCopyAsXML.Image")));
			this.toolStripButtonCopyAsXML.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonCopyAsXML.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCopyAsXML.Name = "toolStripButtonCopyAsXML";
			this.toolStripButtonCopyAsXML.Size = new System.Drawing.Size(26, 26);
			this.toolStripButtonCopyAsXML.Text = "Copy to Clipboard";
			this.toolStripButtonCopyAsXML.ToolTipText = "Copy to clipboard as XML";
			this.toolStripButtonCopyAsXML.Click += new System.EventHandler(this.toolStripButtonCopyAsXML_Click);
			// 
			// textBoxMessage
			// 
			this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMessage.Location = new System.Drawing.Point(12, 32);
			this.textBoxMessage.Multiline = true;
			this.textBoxMessage.Name = "textBoxMessage";
			this.textBoxMessage.ReadOnly = true;
			this.textBoxMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxMessage.Size = new System.Drawing.Size(411, 127);
			this.textBoxMessage.TabIndex = 4;
			// 
			// WindowEventMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(435, 182);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.textBoxMessage);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
						| WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
						| WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
						| WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.HideOnClose = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "WindowEventMessage";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
			this.TabText = "Message";
			this.Text = "Message";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonCopyAsXML;
		private System.Windows.Forms.TextBox textBoxMessage;
	}
}
