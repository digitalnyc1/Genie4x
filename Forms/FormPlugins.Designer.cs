using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace GenieClient
{
    [DesignerGenerated()]
    public partial class FormPlugins : Form
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is object)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPlugins));
            _TabControlPlugins = new TabControl();
            _TabPage1 = new TabPage();
            _FlowLayoutPanel1 = new FlowLayoutPanel();
            _ImageListPlugin = new ImageList(components);
            _OpenFileDialog1 = new OpenFileDialog();
            _ToolStrip1 = new ToolStrip();
            _ToolStripButtonLoad = new ToolStripButton();
            _ToolStripButton1 = new ToolStripButton();
            _TabControlPlugins.SuspendLayout();
            _TabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // TabControlPlugins
            // 
            _TabControlPlugins.Controls.Add(_TabPage1);
            _TabControlPlugins.Dock = DockStyle.Fill;
            _TabControlPlugins.ImageList = _ImageListPlugin;
            _TabControlPlugins.Location = new Point(0, 0);
            _TabControlPlugins.Name = "TabControlPlugins";
            _TabControlPlugins.SelectedIndex = 0;
            _TabControlPlugins.Size = new Size(401, 529);
            _TabControlPlugins.TabIndex = 4;
            // 
            // TabPage1
            // 
            _TabPage1.Controls.Add(_FlowLayoutPanel1);
            _TabPage1.ImageIndex = 0;
            _TabPage1.Location = new Point(4, 23);
            _TabPage1.Name = "TabPage1";
            _TabPage1.Padding = new Padding(3);
            _TabPage1.Size = new Size(393, 502);
            _TabPage1.TabIndex = 0;
            _TabPage1.Text = "Plugins";
            _TabPage1.UseVisualStyleBackColor = true;
            // 
            // FlowLayoutPanel1
            // 
            _FlowLayoutPanel1.AutoScroll = true;
            _FlowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _FlowLayoutPanel1.Dock = DockStyle.Fill;
            _FlowLayoutPanel1.Location = new Point(3, 3);
            _FlowLayoutPanel1.Name = "FlowLayoutPanel1";
            _FlowLayoutPanel1.Size = new Size(387, 496);
            _FlowLayoutPanel1.TabIndex = 0;
            // 
            // ImageListPlugin
            // 
            _ImageListPlugin.ImageStream = (ImageListStreamer)resources.GetObject("ImageListPlugin.ImageStream");
            _ImageListPlugin.TransparentColor = Color.Transparent;
            _ImageListPlugin.Images.SetKeyName(0, "component_green.png");
            _ImageListPlugin.Images.SetKeyName(1, "download.png");
            // 
            // OpenFileDialog1
            // 
            _OpenFileDialog1.DefaultExt = "dll";
            _OpenFileDialog1.Filter = "Plugin files|*.dll|All files|*.*";
            // 
            // ToolStrip1
            // 
            _ToolStrip1.Dock = DockStyle.Top;
            _ToolStrip1.ImageList = _ImageListPlugin;
            _ToolStrip1.Items.AddRange(new ToolStripItem[] { _ToolStripButtonLoad, _ToolStripButton1 });
            _ToolStrip1.Name = "ToolStrip1";
            _ToolStrip1.Size = new Size(401, 25);
            _ToolStrip1.TabIndex = 5;
            // 
            // ToolStripButtonLoad
            // 
            _ToolStripButtonLoad.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            _ToolStripButtonLoad.ImageIndex = 1;
            _ToolStripButtonLoad.Name = "ToolStripButtonLoad";
            _ToolStripButtonLoad.Text = "Load Plugin";
            _ToolStripButtonLoad.Click += new EventHandler(ToolStripButtonLoad_Click);
            // 
            // ToolStripButton1
            // 
            _ToolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _ToolStripButton1.Name = "ToolStripButton1";
            _ToolStripButton1.Text = "Reload All";
            _ToolStripButton1.Click += new EventHandler(ToolStripButton1_Click);
            // 
            // FormPlugins
            // 
            AutoScaleDimensions = new SizeF(6.0F, 13.0F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(401, 529);
            Controls.Add(_TabControlPlugins);
            Controls.Add(_ToolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPlugins";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Plugins";
            _TabControlPlugins.ResumeLayout(false);
            _TabPage1.ResumeLayout(false);
            FormClosing += new FormClosingEventHandler(FormPlugins_FormClosing);
            Shown += new EventHandler(FormPlugins_Shown);
            ResumeLayout(false);
        }

        private TabControl _TabControlPlugins;

        internal TabControl TabControlPlugins
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabControlPlugins;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_TabControlPlugins != null)
                {
                }

                _TabControlPlugins = value;
                if (_TabControlPlugins != null)
                {
                }
            }
        }

        private TabPage _TabPage1;

        internal TabPage TabPage1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabPage1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_TabPage1 != null)
                {
                }

                _TabPage1 = value;
                if (_TabPage1 != null)
                {
                }
            }
        }

        private ImageList _ImageListPlugin;

        internal ImageList ImageListPlugin
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ImageListPlugin;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_ImageListPlugin != null)
                {
                }

                _ImageListPlugin = value;
                if (_ImageListPlugin != null)
                {
                }
            }
        }

        private FlowLayoutPanel _FlowLayoutPanel1;

        internal FlowLayoutPanel FlowLayoutPanel1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _FlowLayoutPanel1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_FlowLayoutPanel1 != null)
                {
                }

                _FlowLayoutPanel1 = value;
                if (_FlowLayoutPanel1 != null)
                {
                }
            }
        }

        private OpenFileDialog _OpenFileDialog1;

        internal OpenFileDialog OpenFileDialog1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _OpenFileDialog1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_OpenFileDialog1 != null)
                {
                }

                _OpenFileDialog1 = value;
                if (_OpenFileDialog1 != null)
                {
                }
            }
        }

        private ToolStrip _ToolStrip1;

        internal ToolStrip ToolStrip1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ToolStrip1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_ToolStrip1 != null)
                {
                }

                _ToolStrip1 = value;
                if (_ToolStrip1 != null)
                {
                }
            }
        }

        private ToolStripButton _ToolStripButtonLoad;

        internal ToolStripButton ToolStripButtonLoad
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ToolStripButtonLoad;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_ToolStripButtonLoad != null)
                {
                }

                _ToolStripButtonLoad = value;
                if (_ToolStripButtonLoad != null)
                {
                }
            }
        }

        private ToolStripButton _ToolStripButton1;

        internal ToolStripButton ToolStripButton1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ToolStripButton1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_ToolStripButton1 != null)
                {
                }

                _ToolStripButton1 = value;
                if (_ToolStripButton1 != null)
                {
                }
            }
        }
    }
}