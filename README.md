# AudioTransfer
Для работы необходим net.core 3.1 https://dotnet.microsoft.com/download/dotnet-core/3.1 и https://ffmpeg.org/

Пример вызова сервиса
```
audiotransfer --s D:\wav --ftp 127.0.0.1 --d books/converted/ --login login --pass pass --th 5 --oe mp3 --ff D:\ffmpeg.exe
```

Где 
1. --s - папка, в которой лежат папки с файлами для конвертации; 
2. --ftp - ftp хост для заливки готовых файлов
3. --d - путь относительно корня ftp для заливки готовых файлов;
4. --login - логин от ftp сервера (опционален);
5. --pass - пароль от ftp сервера (опционален);
6. --th - количество потоков для обработки файлов
7. --oe - формат файла в который осуществляется конвертация
8. -ff - путь к ffmpeg

Для полного списка опций вызвать 

```
audiotransfer --help
```

