using System;
using System.IO;

namespace AudioTransfer.Extensions {
    public static class FileInfoExtensions {
        /// <summary>
        /// Проверка на соответсвие расширения файла переданному
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="extension">Расширение</param>
        /// <returns></returns>
        public static bool ExtensionEquals(this FileInfo file, string extension) {
            return extension.StartsWith(".") ? 
                string.Equals(file.Extension, extension, StringComparison.InvariantCultureIgnoreCase) : 
                string.Equals(file.Extension, "." + extension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Проверка на соотвествие имени файла переданному
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public static bool FileNameEquals(this FileInfo file, string name) {
            return string.Equals(file.Name, name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
