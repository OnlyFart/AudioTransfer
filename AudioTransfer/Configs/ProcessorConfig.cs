namespace AudioTransfer.Configs {
    public interface IProcessorConfig {
        /// <summary>
        /// Коллисество потоков для обработки файлов
        /// </summary>
        int ThreadsCount { get; set; }
    }
}
