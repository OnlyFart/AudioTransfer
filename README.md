# AudioTransfer
Для работы необходимо: 

* [Net.core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) 
* [ffmpeg](https://ffmpeg.org/download.html)

## Пример вызова сервиса
```
audiotransfer --s D:\source --d D:\destination --th 5 --ff D:\ffmpeg.exe --fp D:\ffprobe.exe
```

## Где 
```
--s - папка, в которой лежат папки с файлами для конвертации; 
--d - папка, в которую отправляются обработанные файлы;
--ff - путь к ffmpeg
--fp - путь к ffprobe
--th - кол-во одновременно обрабатывающихся папок
```

## Полный список опций 

```
audiotransfer --help
```

