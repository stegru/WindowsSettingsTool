namespace WindowsSettingsTool
{
    partial class MainForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.settingGrid = new System.Windows.Forms.PropertyGrid();
            this.settingsTree = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.settingsTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.settingGrid);
            this.splitContainer1.Size = new System.Drawing.Size(851, 555);
            this.splitContainer1.SplitterDistance = 370;
            this.splitContainer1.TabIndex = 0;
            // 
            // settingGrid
            // 
            this.settingGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingGrid.Location = new System.Drawing.Point(0, 0);
            this.settingGrid.Name = "settingGrid";
            this.settingGrid.Size = new System.Drawing.Size(477, 555);
            this.settingGrid.TabIndex = 0;
            // 
            // settingsTree
            // 
            this.settingsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsTree.Location = new System.Drawing.Point(0, 0);
            this.settingsTree.Name = "settingsTree";
            this.settingsTree.Size = new System.Drawing.Size(370, 555);
            this.settingsTree.TabIndex = 0;
            this.settingsTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.settingsTree_BeforeSelect);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 555);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid settingGrid;
        private System.Windows.Forms.TreeView settingsTree;
    }
}

