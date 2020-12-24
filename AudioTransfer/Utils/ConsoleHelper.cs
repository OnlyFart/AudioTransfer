using System;
using System.Text;

namespace AudioTransfer.Utils {
    public static class ConsoleHelper {
        public static readonly object Locker = new object();

        public static void Info(string message) {
            lock (Locker) {
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine(Format(message));
            }
        }
        
        public static void Warn(string message) {
            lock (Locker) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.OutputEncoding = Encoding.UTF8;
                Console.Error.WriteLine(Format(message));
                Console.ResetColor();
            }
        }
        
        public static void Error(string message) {
            lock (Locker) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.OutputEncoding = Encoding.UTF8;
                Console.Error.WriteLine(Format(message));
                Console.ResetColor();
            }
        }
        
        public static void Success(string message) {
            lock (Locker) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine(Format(message));
                Console.ResetColor();
            }
        }

        private static string Format(string message) {
            return $"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} - {message}";
        }
    }
}
