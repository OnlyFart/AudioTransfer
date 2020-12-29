using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.FFmpeg;
using AudioTransfer.Types;

namespace AudioTransfer.Logic.FileProcessor {
    public abstract class FileProcessorBase {
        private readonly IProcessorConfig _config;
        protected readonly FFmpegWrapper _fFmpegWrapper;

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
        public abstract Task Process(FileSystemInfo directory, List<string> inputFiles);

        public abstract string FileName { get; }

        /// <summary>
        /// Создание результирующей папки
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetOutputDirectory(FileSystemInfo directory) {
            var outputDirectory = Path.Combine(_config.Destination, Path.GetRelativePath(_config.Source, directory.FullName));
            if (!Directory.Exists(outputDirectory)) {
                Directory.CreateDirectory(outputDirectory);
            }

            return outputDirectory;
        }
        
        /// <summary>
        /// Получение результируюшего пути для сохранения файлов с созданием дерева каталогов
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        protected string GetOutputFileName(FileSystemInfo directory) {
            var outputDirectory = GetOutputDirectory(directory);
            var outputFile = $"{directory.Name}_{Const.OUTPUT_EXTENSION.ToUpper()}WRAP.{Const.OUTPUT_EXTENSION.ToLower()}";

            return Path.Combine(outputDirectory, outputFile);
        }

        /// <summary>
        /// Получение результируюшего пути для сохранения файлов с созданием дерева каталогов
        /// </summary>
        /// <param name="file"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        protected string GetOutputFileName(string file, FileSystemInfo directory) {
            var outputDirectory = GetOutputDirectory(directory);
            var outputFile = $"{Path.GetFileNameWithoutExtension(file)}.{Const.OUTPUT_EXTENSION.ToLower()}";

            return Path.Combine(outputDirectory, outputFile);
        }
    }
}
