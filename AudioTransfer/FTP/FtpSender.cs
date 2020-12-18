using System;
using System.Threading.Tasks;
using AudioTransfer.Configs;
using AudioTransfer.Utils;
using FluentFTP;

namespace AudioTransfer.FTP {
    public class FtpSender {
        public readonly FtpConfig Config;

        public FtpSender(FtpConfig config) {
            Config = config;
        }

        public async Task<bool> Send(string destination, string file) {
            try {
                using var client = GetClient();
                await client.ConnectAsync();
                return await client.UploadFileAsync(file, destination, FtpRemoteExists.Overwrite, true, FtpVerify.Retry) == FtpStatus.Success;
            } catch (Exception ex) {
                ConsoleHelper.Error($"Не удалось отправить файл по ftp {ex}");
                return false;
            }
        }

        private FtpClient GetClient() {
            return string.IsNullOrWhiteSpace(Config.FtpLogin) && string.IsNullOrWhiteSpace(Config.FtpPassword) ? 
                new FtpClient(Config.FtpServer) : 
                new FtpClient(Config.FtpServer, Config.FtpLogin, Config.FtpPassword);
        }
    }
}
