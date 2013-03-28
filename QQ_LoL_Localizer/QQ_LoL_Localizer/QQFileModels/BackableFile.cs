﻿using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QQ_LoL_Localizer.QQFileModels
{
    public abstract class BackableFile : IFixableFile, INotifyPropertyChanged
    {
        protected bool? IsFileFixed;

        public abstract bool? IsFixed { get; set; }
        public abstract Task FixAsync();

        public async Task RestoreAsync()
        {
            await Task.Run(() =>
            {
                if (!File.Exists(FilePath + ".backup"))
                    return;
                File.Copy(FilePath + ".backup", FilePath, true);
                IsFixed = null;
            });
        }

        public void Backup()
        {
            if (File.Exists(FilePath))
                File.Copy(FilePath, FilePath + ".backup", true);
        }

        public abstract string FilePath { get; }

        public async Task BackupAsync()
        {
            await Task.Run(() => Backup());
        }

        public async Task RefreshAsync()
        {
            await Task.Run(() =>
                {
                    IsFixed = null;
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OpenFolder()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = Directory.GetParent(FilePath).FullName,
                    Verb = "open"
                });
        }
    }
}
