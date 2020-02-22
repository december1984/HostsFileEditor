using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HostsFileEditor.Controls
{
    class HostsArchiveListToolStripMenuItem : ToolStripMenuItem
    {
        private readonly HostsArchiveList _model;

        public HostsArchiveListToolStripMenuItem(HostsArchiveList model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            Reset();
            _model.ListChanged += HostsArchiveList_ListChanged;
        }

        private void Reset()
        {
            DropDownItems.Clear();
            DropDownItems.AddRange(_model.Select(a => new HostsArchiveToolStripMenuItem(a)).ToArray());
        }

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is HostsArchiveToolStripMenuItem item && item.Model != null)
            {
                HostsFile.Instance.Import(item.Model.FilePath);
                HostsFile.Instance.Save();
            }
        }

        private void HostsArchiveList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case System.ComponentModel.ListChangedType.Reset:
                    Reset();
                    break;
                case System.ComponentModel.ListChangedType.ItemAdded:
                    {
                        DropDownItems.Insert(e.NewIndex, new HostsArchiveToolStripMenuItem(_model[e.NewIndex]));
                    }
                    break;
                case System.ComponentModel.ListChangedType.ItemDeleted:
                    DropDownItems.RemoveAt(e.NewIndex);
                    break;
                case System.ComponentModel.ListChangedType.ItemMoved:
                    {
                        var item = DropDownItems[e.OldIndex];
                        DropDownItems.RemoveAt(e.OldIndex);
                        DropDownItems.Insert(e.NewIndex, item);
                    }
                    break;
                case System.ComponentModel.ListChangedType.ItemChanged:
                    {
                        if (DropDownItems[e.NewIndex] is HostsArchiveToolStripMenuItem item)
                        {
                            item.Model = _model[e.NewIndex];
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
