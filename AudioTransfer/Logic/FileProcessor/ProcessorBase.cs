using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.FFmpeg;

namespace AudioTransfer.Logic.FileProcessor {
    public abstract class FileProcessorBase {
        private readonly IProcessorConfig _config;
        protected readonly FFmpegWrapper _fFmpegWrapper;
        
        public abstract string FileName { get; }

        protected FileProcessorBase(IProcessorConfig config, FFmpegWrapper fFmpegWrapper) {
            _config = config;
            _fFmpegWrapper = fFmpegWrapper;
        }

        /// <summary>
        /// Обработка конкретной папки
        /// </summary>
        /// <param name="directory">Директория</param>
        /// <param name="inputFiles">Входящие файлы</param>
        /// <returns></returns>
        public abstract Task Process(FileSystemInfo directory, IReadOnlyCollection<string> inputFiles);

        /// <summary>
        /// Создание результирующей папки
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        protected string GetOutputDirectory(FileSystemInfo directory) {
            var outputDirectory = Path.Combine(_config.Destination, Path.GetRelativePath(_config.Source, directory.FullName));
            if (!Directory.Exists(outputDirectory)) {
                Directory.CreateDirectory(outputDirectory);
            }

            return outputDirectory;
        }
    }
}
