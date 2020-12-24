using CommandLine;

namespace AudioTransfer.Configs {
    public class Options : IFFmpegConfig, IProcessorConfig {
        [Option('s', "source", Required = true, HelpText = "Исходная директория с файлами для конвертации")]
        public string Source { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Результирующая директория для сохранения файлов")]
        public string Destination { get; set; }
        
        [Option("ff", Required = true, HelpText = "Путь к ffmpeg")]
        public string FFmpegPath { get; set; }
        
        [Option("fp", Required = true, HelpText = "Путь к ffprobe")]
        public string FFprobePath { get; set; }

        [Option("codec", Default = "libmp3lame", Required = false, HelpText = "Кодек")]
        public string Codec { get; set; }
        
        [Option("qa", Default = 7, Required = false, HelpText = "Кодек")]
        public int Quality { get; set; }

        [Option("th", Default = 1, Required = false, HelpText = "Количество потоков для обработки")]
        public int ThreadsCount { get; set; }
        
        [Option("dl", Default = 60, Required = false, HelpText = "Задержка в секундах между итерациями")]
        public int DelaySeconds { get; set; }
    }
}
