namespace AudioTransfer.Configs {
    public interface IFtpConfig {
        /// <summary>
        /// Адрес сервера для заливки файлов
        /// </summary>
        string FtpServer { get; set; }
        
        /// <summary>
        /// Логин от ftp сервера
        /// </summary>
        string FtpLogin { get; set; }
        
        /// <summary>
        /// Пароль от ftp сервера
        /// </summary>
        string FtpPassword { get; set; }
    }
}
