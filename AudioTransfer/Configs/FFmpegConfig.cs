namespace AudioTransfer.Configs {
    public interface IFFmpegConfig {
        /// <summary>
        /// Битрейт выходного файда
        /// </summary>
        int BitRate { get; set; }

        /// <summary>
        /// Количество каналов выходного файла
        /// </summary>
        int ChannelsCount  { get; set; }

        /// <summary>
        /// Частота выходного файла
        /// </summary>
        int SampleRate  { get; set; }

        /// <summary>
        /// Выходное расширение файла
        /// </summary>
        string OutputExtension  { get; set; }

        /// <summary>
        /// Путь к файлу ffmpeg
        /// </summary>
        string FFmpegPath  { get; set; }
    }
}
