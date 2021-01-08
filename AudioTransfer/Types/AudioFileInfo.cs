using System;

namespace AudioTransfer.Types {
    public class AudioFileFormat {
        public readonly AudioFileInfo Format = new AudioFileInfo();
    }
    
    public class AudioFileInfo {
        /// <summary>
        /// Название файла
        /// </summary>
        public string FileName;

        /// <summary>
        /// Длительность файла
        /// </summary>
        public double Duration;

        /// <summary>
        /// Размер файла
        /// </summary>
        public decimal Size;

        public override string ToString() {
            return $"{FileName} ({TimeSpan.FromSeconds(Duration):hh\\:mm\\:ss}, {Size / 1024 / 1024:#.##} MBytes)";
        }
    }
}
