namespace WindowsSettingsTool
{
    using Microsoft.Win32;
    using SettingsHelper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private Dictionary<string, SettingType> allSettings;

        /// <summary>
        /// Get all settings from the registry.
        /// </summary>
        /// <returns>The settings.</returns>
        private IEnumerable<KeyValuePair<string, SettingType>> getSettings()
        {
            string path = @"SOFTWARE\Microsoft\SystemSettings\SettingId";

            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(path, false))
            {
                foreach (string key in regKey.GetSubKeyNames())
                {
                    string settingPath = Path.Combine("HKEY_LOCAL_MACHINE", path, key);
                    string typeName = Registry.GetValue(settingPath, "Type", null) as string;
                    SettingType type;
                    if (Enum.TryParse(typeName, true, out type))
                    {
                        //if ((type != SettingType.Custom && type != SettingType.SettingCollection))
                        {
                            yield return new KeyValuePair<string, SettingType>(key, type);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Loads the tree view.
        /// </summary>
        private void loadSettingsList()
        {
            this.settingsTree.Nodes.Clear();
            allSettings = new Dictionary<string, SettingType>();

            // Add the items.
            foreach (KeyValuePair<string, SettingType> item in this.getSettings())
            {
                allSettings.Add(item.Key, item.Value);
                string settingId = item.Key;
                SettingType type = item.Value;

                string[] parts = settingId.Split('_');

                string currentPath = null;
                TreeNodeCollection parentNodes = this.settingsTree.Nodes;

                foreach (string part in parts)
                {
                    if (currentPath != null)
                    {
                        currentPath += "_";
                    }
                    currentPath += part;

                    TreeNode node = parentNodes[currentPath] ?? parentNodes.Add(currentPath, part);
                    parentNodes = node.Nodes;
                    
                }
            }

            // Merge single node branches
            this.MergeSingleNodes(null, this.settingsTree.Nodes);

        }

        private void MergeSingleNodes(TreeNode parent, TreeNodeCollection nodes = null)
        {
            if (parent != null)
            {
                nodes = parent.Nodes;

                if (nodes.Count == 1)
                {
                    TreeNode child = nodes[0];
                    parent.Name = child.Name;
                    parent.Text += "_" + child.Text;
                    parent.Nodes.Clear();
                }
            }
            foreach (TreeNode node in nodes)
            {
                this.MergeSingleNodes(node);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            this.loadSettingsList();
        }

        private void settingsTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            string settingId = e.Node.Name;
            SettingInfo setting = null;
            SettingType type;

            try
            {
                if (e.Node.Tag == null)
                {
                    if (this.allSettings.TryGetValue(settingId, out type))
                    {
                        e.Node.Tag = SettingInfo.Get(settingId, type);
                    }
                }

                setting = e.Node.Tag as SettingInfo;
                if (setting != null)
                {
                    this.settingGrid.SelectedObject = new
                    {
                        Loading = ""
                    };
                    this.settingGrid.Update();
                }

                this.settingGrid.SelectedObject = setting;
            }
            catch (Exception ex)
            {
                this.settingGrid.SelectedObject = ex.InnerException ?? ex;
            }

        }

        private void filterText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
