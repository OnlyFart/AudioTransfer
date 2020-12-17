namespace AudioTransfer.Configs {
    public class FtpConfig {
        /// <summary>
        /// Адрес сервера для заливки файлов
        /// </summary>
        public string FtpServer { get; set; }
        
        /// <summary>
        /// Логин от ftp сервера
        /// </summary>
        public string FtpLogin { get; set; }
        
        /// <summary>
        /// Пароль от ftp сервера
        /// </summary>
        public string FtpPassword { get; set; }
    }
}
