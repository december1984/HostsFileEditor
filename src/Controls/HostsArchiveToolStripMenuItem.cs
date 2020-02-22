using System;
using System.Windows.Forms;

namespace HostsFileEditor.Controls
{
    class HostsArchiveToolStripMenuItem : ToolStripMenuItem
    {
        private HostsArchive _model;

        public HostsArchiveToolStripMenuItem(HostsArchive model)
        {
            Model = model;
            Text = model.FileName;
        }

        public HostsArchive Model
        {
            get => _model;
            internal set
            {
                if (_model == value) return;
                if (_model != null) _model.IsActiveChanged -= Model_IsActiveChanged;
                _model = value;
                if (_model != null) _model.IsActiveChanged += Model_IsActiveChanged;
                Checked = _model != null && _model.IsActive;
            }
        }

        private void Model_IsActiveChanged(object sender, EventArgs e)
        {
            Checked = _model != null && _model.IsActive;
        }
    }
}
