using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;


namespace NetLogClient.Gui
{
	[CLSCompliant(false)]
	public partial class WindowLoggerExplorer : DockContent
	{
		private static string _lastSelectedLogger = string.Empty;
		internal const string ROOT_LOGGER_NAME = @"RootLogger";
		internal const Char LOGGER_DELIMITER = '.';
		private List<string> nodesAddedList = new List<string>(LogEntryManager.CacheSize);


		public WindowLoggerExplorer()
		{
			InitializeComponent();
			AddDefaults();
		}

		private void AddDefaults()
		{
			listViewLogger.Nodes.Add(ROOT_LOGGER_NAME, ROOT_LOGGER_NAME);
		}

		internal void Clear()
		{
			lock (this)
			{
				nodesAddedList.Clear();
				listViewLogger.Nodes.Clear();
				AddDefaults();
			}
		}

		internal void AddIfNeeded(string nodeNameToAdd)
		{
			if (nodeNameToAdd == null || nodeNameToAdd.Length<1) return;
			if(nodesAddedList.Contains(nodeNameToAdd)) return;
			
			listViewLogger.BeginUpdate();
			string[] parts = nodeNameToAdd.Split(LOGGER_DELIMITER);
			TreeNodeCollection nodes = listViewLogger.Nodes[0].Nodes;
			foreach (string part in parts)
			{
				if (part.Equals(ROOT_LOGGER_NAME)) continue;
				nodes = AddNodeToCurrentLevelIfNeeded(nodes, part);
			}

			listViewLogger.EndUpdate();
			nodesAddedList.Add(nodeNameToAdd);
		}

		private static TreeNodeCollection AddNodeToCurrentLevelIfNeeded(TreeNodeCollection nodes, string nodeName)
		{
			TreeNode node = null;
			if (!nodes.ContainsKey(nodeName))
			{
				node = nodes.Add(nodeName, nodeName);
			}else{
				node=nodes[nodeName];
			}

			return node.Nodes;
		}

		internal string SelectedNodePath
		{
			get
			{
				if (listViewLogger.SelectedNode == null) return string.Empty;
				string path = listViewLogger.SelectedNode.FullPath;

				return path.Replace('\\', LOGGER_DELIMITER);
			}
		}

		private void listViewLogger_AfterSelect(object sender, TreeViewEventArgs e)
		{
			_lastSelectedLogger = e.Node.FullPath;
		}

		internal static string LastSelectedLogger
		{
			get { return _lastSelectedLogger; }
		}

		internal string SelectedLogger
		{
			get { return listViewLogger.SelectedNode.FullPath; }
		}

	}
}

