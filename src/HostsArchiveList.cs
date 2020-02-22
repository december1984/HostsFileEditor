// <copyright file="HostsArchiveList.cs" company="N/A">
// Copyright 2011 Scott M. Lerch
// 
// This file is part of HostsFileEditor.
// 
// HostsFileEditor is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 2 of the License, or (at your option)
// any later version.
// 
// HostsFileEditor is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
// 
// You should have received a copy of the GNU General Public   License along
// with HostsFileEditor. If not, see http://www.gnu.org/licenses/.
// </copyright>

namespace HostsFileEditor
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Security.Cryptography;
    using HostsFileEditor.Extensions;
    using HostsFileEditor.Utilities;

    /// <summary>
    /// Hosts archive list.
    /// </summary>
    internal class HostsArchiveList : BindingList<HostsArchive>
    {
        /// <summary>
        /// The archive directory.
        /// </summary>
        public static readonly string ArchiveDirectory = 
            Path.Combine(HostsFile.DefaultHostFileDirectory, "archive");

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static readonly Lazy<HostsArchiveList> instance = 
            new Lazy<HostsArchiveList>(() => new HostsArchiveList());

        /// <summary>
        /// Prevents a default instance of the <see cref="HostsArchiveList"/> class from being created.
        /// </summary>
        private HostsArchiveList()
        {
            this.Refresh();
            HostsFile.Instance.DefaultHostFileSaved += (s, e) => CheckActiveArchive();
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static HostsArchiveList Instance 
        { 
            get { return instance.Value; } 
        }

        /// <summary>
        /// Deletes the specified archive.
        /// </summary>
        /// <param name="archive">The archive.</param>
        public void Delete(HostsArchive archive)
        {
            using (FileEx.DisableAttributes(archive.FilePath, FileAttributes.ReadOnly))
            {
                File.Delete(archive.FilePath);
            }

            this.Remove(archive);
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            this.BatchUpdate(() =>
            {
                this.Clear();

                if (Directory.Exists(ArchiveDirectory))
                {
                    var files = Directory.GetFiles(ArchiveDirectory);

                    foreach (var file in files)
                    {
                        this.Add(new HostsArchive { FilePath = file });
                    }
                }

                CheckActiveArchive();
            });
        }

        private string _hostsHash;

        private void CheckActiveArchive()
        {
            _hostsHash = ComputeHash(HostsFile.DefaultHostFilePath);
            foreach (var archive in this)
            {
                var hash = ComputeHash(archive.FilePath);
                archive.IsActive = hash == _hostsHash;
            }
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                var archive = this[e.NewIndex] as HostsArchive;
                var hash = ComputeHash(archive.FilePath);
                archive.IsActive = hash == _hostsHash;
            }
            base.OnListChanged(e);
        }

        private string ComputeHash(string filePath)
        {
            return Convert.ToBase64String(SHA256.Create().ComputeHash(File.ReadAllBytes(filePath)));
        }
    }
}
