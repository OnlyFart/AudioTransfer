using System;

namespace AudioTransfer.Types {
    public class AudioFileInfo {
        /// <summary>
        /// Название файла
        /// </summary>
        public string Path;

        /// <summary>
        /// Длительность файла
        /// </summary>
        public TimeSpan Duration;

        /// <summary>
        /// Размер файла
        /// </summary>
        public double Size;

        public override string ToString() {
            return $"{Path} ({Duration:hh\\:mm\\:ss}, {Size} MBytes)";
        }
    }
}
