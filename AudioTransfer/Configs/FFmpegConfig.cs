namespace AudioTransfer.Configs {
    public class FFmpegConfig {
        /// <summary>
        /// Битрейт выходного файда
        /// </summary>
        public int BitRate;

        /// <summary>
        /// Количество каналов выходного файла
        /// </summary>
        public int ChannelsCount;

        /// <summary>
        /// Частота выходного файла
        /// </summary>
        public int SampleRate;

        /// <summary>
        /// Выходное расширение файла
        /// </summary>
        public string OutputExtension;

        /// <summary>
        /// Путь к файлу ffmpeg
        /// </summary>
        public string FFmpegPath;
    }
}
