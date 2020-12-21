# AudioTransfer
Для работы необходимо: 

* [Net.core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) 
* [ffmpeg](https://ffmpeg.org/)

### Пример вызова сервиса
```
audiotransfer --s D:\wav --ftp 127.0.0.1 --d books/converted/ --login login --pass pass --th 5 --oe mp3 --ff D:\ffmpeg.exe
```

Где 
* --s - папка, в которой лежат папки с файлами для конвертации; 
* --ftp - ftp хост для заливки готовых файлов
* --d - путь относительно корня ftp для заливки готовых файлов;
* --login - логин от ftp сервера (опционален);
* --pass - пароль от ftp сервера (опционален);
* --th - количество потоков для обработки файлов
* --oe - формат файла в который осуществляется конвертация
* -ff - путь к ffmpeg

Для полного списка опций вызвать 

```
audiotransfer --help
```

