using CommandLine;

namespace AudioTransfer.Configs {
    public class Options {
        [Option('s', "source", Required = true, HelpText = "Исходная директория с файлами для конвертации")]
        public string Source { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Результирующая директория для сохранения файлов")]
        public string Destination { get; set; }
        
        [Option("ff", Required = true, HelpText = "Путь к ffmpeg")]
        public string FFmpegPath { get; set; }
        
        [Option("ftp", Required = true, HelpText = "Адрес ftp сервера")]
        public string FtpServer { get; set; }
        
        [Option("th", Default = 1, Required = false, HelpText = "Количество потоков для обработки")]
        public int ThreadsCount { get; set; }
        
        [Option("login", Required = false, HelpText = "Логин от ftp сервера")]
        public string FtpLogin { get; set; }
        
        [Option("pass", Required = false, HelpText = "Пароль от ftp сервера")]
        public string FtpPassword { get; set; }
        
        [Option("oe", Default = "mp3", Required = false, HelpText = "Расширение выходного файла")]
        public string OutputExtension { get; set; }

        [Option("ar", Default = 44100, Required = false, HelpText = "Частота выходного файла")]
        public int SampleRate { get; set; }
        
        [Option("ac", Default = 2, Required = false, HelpText = "Количество каналов выходного файла. 1 - моно, 2 - стерео")]
        public int ChannelsCount { get; set; }
        
        [Option("ba", Default = 192, Required = false, HelpText = "Битрейт выходного файла в килобайтах")]
        public int BitRate { get; set; }
    }
}
