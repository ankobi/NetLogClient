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
    public partial class WindowComputerExplorer : DockContent
    {
        public event EventHandler HostSelectionChange;

        public const string ROOT_NAME = @"Sender";


        public WindowComputerExplorer()
        {
            InitializeComponent();
            AddDefaults();
        }

        private void AddDefaults()
        {
            listBox1.Items.Add("*");
        }

        internal void AddIfNeeded(string nodeNameToAdd)
        {
            if (!listBox1.Items.Contains(nodeNameToAdd))
            {
                listBox1.Items.Add(nodeNameToAdd);
            }
        }

        internal string SelectedComputer
        {
            get
            {
                string computer = (string)listBox1.SelectedItem;
                if (computer == null) computer = string.Empty;
                return computer;
            }
        }

        internal void Clear()
        {
            listBox1.Items.Clear();
            AddDefaults();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //fire event
            if (HostSelectionChange != null) HostSelectionChange(listBox1, e);
        }
    }
}

