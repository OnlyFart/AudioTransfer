using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.FFmpeg;
using AudioTransfer.Types;
using AudioTransfer.Utils;

namespace AudioTransfer.Logic.FileProcessor {
    /// <summary>
    /// Процессор для склеивания файлов
    /// </summary>
    public class JoinFileProcessor : FileProcessorBase {
        public override string FileName => "join.txt";
        
        public JoinFileProcessor(IProcessorConfig config, FFmpegWrapper fFmpegWrapper) : base(config, fFmpegWrapper) { }
        
        /// <summary>
        /// Обработка конкретной папки
        /// </summary>
        /// <param name="directory">Директория</param>
        /// <param name="inputFiles">Входящие файлы</param>
        /// <returns></returns>
        public override async Task Process(FileSystemInfo directory, IReadOnlyCollection<string> inputFiles) {
            var outputFileName = GetOutputFileName(directory);
            if (await _fFmpegWrapper.Convert(inputFiles, outputFileName)) {
                ConsoleHelper.Success($"Успешно обработали каталог {directory}");
                await CreateJoinReport(directory, inputFiles, outputFileName);
            } else {
                ConsoleHelper.Error($"Не удалось обработать файлы в директории {directory}");
            }
        }
        
        /// <summary>
        /// Получение результируюшего пути для сохранения файлов с созданием дерева каталогов
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetOutputFileName(FileSystemInfo directory) {
            var outputDirectory = GetOutputDirectory(directory);
            var outputFile = $"{directory.Name}_{Const.OUTPUT_EXTENSION.ToUpper()}WRAP.{Const.OUTPUT_EXTENSION.ToLower()}";

            return Path.Combine(outputDirectory, outputFile);
        }

        /// <summary>
        /// Создание отчета по склеиванию файлов
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        private  async Task CreateJoinReport(FileSystemInfo directory, IEnumerable<string> inputFiles, string outputFile) {
            var lines = new List<string> {"Слияние:"};
            foreach (var file in inputFiles) {
                lines.Add($"- {await _fFmpegWrapper.GetFileFormat(file)}");
            }
            
            lines.Add("Результат:");
            lines.Add($"- {await _fFmpegWrapper.GetFileFormat(outputFile)}");
            
            await File.WriteAllLinesAsync(Path.Combine(directory.FullName, Const.REPORT_FILENAME), lines, Encoding.UTF8);
        }
    }
}
