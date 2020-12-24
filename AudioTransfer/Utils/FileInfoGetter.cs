using System.IO;
using System.Threading.Tasks;
using AudioTransfer.Types;
using NAudio.Wave;

namespace AudioTransfer.Utils {
    public static class FileInfoGetter {
        public static async Task<AudioFileInfo> GetFileInfo(string path) {
            await using var input = new AudioFileReader(path);
            var info = new FileInfo(path);
            
            return new AudioFileInfo {
                Path = path,
                Duration = input.TotalTime,
                Size = info.Length * 1.0 / 1024 / 1024
            };
        }
    }
}
