namespace AudioTransfer.Configs {
    public interface IProcessorConfig {
        /// <summary>
        /// Количество потоков для обработки файлов
        /// </summary>
        int ThreadsCount { get; set; }
        
    }
}
