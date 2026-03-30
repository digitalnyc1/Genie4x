using GeniePlugin.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace GenieClient
{
    public partial class FormPlugins
    {
        public FormPlugins()
        {
            InitializeComponent();
        }

        public FormPlugins(ref Genie.Collections.ArrayList pluginList)
        {
            // This call is required by the designer
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call
            m_PluginList = pluginList;
        }

        public event LoadPluginEventHandler LoadPlugin;
        public delegate void LoadPluginEventHandler(string filename);

        public event ReloadPluginByNameEventHandler ReloadPluginByName;
        public delegate void ReloadPluginByNameEventHandler(string name);

        public event UnloadPluginByNameEventHandler UnloadPluginByName;
        public delegate void UnloadPluginByNameEventHandler(string name);

        public event ReloadPluginsEventHandler ReloadPlugins;
        public delegate void ReloadPluginsEventHandler();

        private Collection<ComponentPluginItem> m_PluginListItems = new Collection<ComponentPluginItem>();

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadPlugins?.Invoke();
            ReloadList();
        }

        private void ToolStripButtonLoad_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadPlugin?.Invoke(OpenFileDialog1.FileName.Trim());
                ReloadList();
            }
        }

        public void ReloadList()
        {
            if (Visible)
            {
                FlowLayoutPanel1.Controls.Clear();
                PopulatePluginList();
            }
        }

        private Genie.Collections.ArrayList m_PluginList;

        private void PopulatePluginList()
        {
            foreach (IPlugin plugin in m_PluginList)
            {
                var pluginItem = new ComponentPluginItem();
                pluginItem.Plugin = plugin;
                pluginItem.PluginButtonUnload += UnloadButton_Click;
                pluginItem.PluginButtonEnable += EnableButton_Click;
                pluginItem.PluginButtonDisable += DisableButton_Click;
                pluginItem.PluginButtonReload += ReloadButton_Click;
                m_PluginListItems.Add(pluginItem);
                FlowLayoutPanel1.Controls.Add(pluginItem);
                FlowLayoutPanel1.Invalidate();
            }
        }

        private void UnloadButton_Click(ComponentPluginItem sender)
        {
            UnloadPluginByName?.Invoke(sender.Plugin.Name);
            ReloadList();
        }

        private void ReloadButton_Click(ComponentPluginItem sender)
        {
            ReloadPluginByName?.Invoke(sender.Plugin.Name);
            ReloadList();
        }

        private void EnableButton_Click(ComponentPluginItem sender)
        {
            sender.Plugin.Enabled = true;
        }

        private void DisableButton_Click(ComponentPluginItem sender)
        {
            sender.Plugin.Enabled = false;
        }

        private void FormPlugins_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Visible = false;
                e.Cancel = true;
            }
        }

        private void FormPlugins_Shown(object sender, EventArgs e)
        {
            PopulatePluginList();
        }
    }
}
