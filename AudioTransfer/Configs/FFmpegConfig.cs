namespace AudioTransfer.Configs {
    public interface IFFmpegConfig {
        /// <summary>
        /// Битрейт выходного файда
        /// </summary>
        string Codec { get; set; }

        /// <summary>
        /// Выходное расширение файла
        /// </summary>
        int Quality  { get; set; }

        /// <summary>
        /// Путь к файлу ffmpeg
        /// </summary>
        string FFmpegPath  { get; set; }
        
        public string OutputExtension { get; set; }
    }
}
