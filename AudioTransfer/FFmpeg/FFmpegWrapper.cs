using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.Extensions;
using AudioTransfer.Utils;

namespace AudioTransfer.FFMPEG {
    /// <summary>
    /// Обертка на ffmpeg
    /// </summary>
    public class FFmpegWrapper {
        public readonly IFFmpegConfig Config;

        public FFmpegWrapper(IFFmpegConfig config) {
            Config = config;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Convert(string inputFile, string outputFile) {
            return await Convert(new List<string>{inputFile}, outputFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Convert(List<string> inputFiles, string outputFile) {
            try {
                if (inputFiles.Count == 0) {
                    return false;
                }

                var arguments = "-y -loglevel error"
                    .AppendThroughWhitespace(string.Join(" ", inputFiles.Select(f => $"-i {f.CoverQuotes()}")))
                    .AppendThroughWhitespace("-filter_complex")
                    .AppendThroughWhitespace(string.Join(string.Empty, inputFiles.Select((_, i) => $"[{i}:a]")) + $"concat=n={inputFiles.Count}:v=0:a=1")
                    .AppendThroughWhitespace("-vn")
                    .AppendThroughWhitespace($"-codec:a {Config.Codec}")
                    .AppendThroughWhitespace($"-q:a {Config.Quality}")
                    .AppendThroughWhitespace(outputFile.CoverQuotes());

                var info = new ProcessStartInfo {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = Config.FFmpegPath,
                    Arguments = arguments
                };
                
                var tcs = new TaskCompletionSource<int>();

                var process = new Process {
                    StartInfo = info,
                    EnableRaisingEvents = true
                };
                
                process.Exited += (sender, args) => {
                    tcs.SetResult(process.ExitCode);
                    process.Dispose();
                };
                
                process.Start();

                return await tcs.Task == 0;
            } catch (Exception ex) {
                ConsoleHelper.Error($"Не удалось обработать файл {outputFile} {ex}");
                return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetFileDuration(string file) {
            try {
                var arguments = $"-i {file}";

                var info = new ProcessStartInfo {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = Config.FFprobePath,
                    Arguments = arguments,
                    RedirectStandardError = true
                };

                using var process = new Process {
                    StartInfo = info,
                    EnableRaisingEvents = true
                };
                process.Start();

                var result = string.Empty;
                while (!process.StandardError.EndOfStream) {
                    var line = (await process.StandardError.ReadLineAsync()).Trim();
                    var value = "Duration: ";
                    if (line.StartsWith(value)) {
                        result = line.Substring(value.Length, 8);
                    }
                }

                process.WaitForExit();
                return result;
            } catch (Exception ex) {
                ConsoleHelper.Error($"Не удалось обработать файл {ex}");
                return string.Empty;
            }
        }
    }
}