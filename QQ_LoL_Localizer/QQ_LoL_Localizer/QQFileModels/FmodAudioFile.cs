﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QQ_LoL_Localizer.QQFileModels
{
    class FmodAudioFile : BackableFile
    {
        public override bool? IsFixed
        {
            get
            {
                if (!IsFileFixed.HasValue)
                {
                    if (!File.Exists(FilePath))
                        return (IsFileFixed = false);

                    var fileEnUs = new FileInfo(FilePath);
                    var fileZhCn = new FileInfo(FilePath.Replace("en_US", "zh_CN"));
                    IsFileFixed = fileEnUs.Length == fileZhCn.Length;
                }
                return IsFileFixed.Value;
            }
            set
            {
                IsFileFixed = value;
                OnPropertyChanged();
            }
        }
        public override async Task FixAsync()
        {
            if (IsFixed.GetValueOrDefault(false))
                return;

            await Task.Run(() =>
            {
                Backup();
                File.Copy(FilePath.Replace("en_US", "zh_CN"), FilePath, true);

                var bytes = File.ReadAllBytes(FilePath);

                var searchBytes = Encoding.UTF8.GetBytes("LoL_Audio_zh_CN");
                var replaceBytes = Encoding.UTF8.GetBytes("LoL_Audio_en_US");

                bytes = Helper.ReplaceBytes(bytes, searchBytes, replaceBytes);

                File.WriteAllBytes(FilePath, bytes);

                IsFixed = null;
            });
        }

        public override string FilePath { get { return Path.Combine(Helper.LoLPath, "Game\\Data\\Sounds\\FMOD\\LoL_Audio_en_US.fev"); } }

    }
}
