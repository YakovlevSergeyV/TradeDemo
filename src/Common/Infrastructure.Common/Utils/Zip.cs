namespace Infrastructure.Common.Utils
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;

    public class Zip : IZip
    {
        public void CompressDirectory(string directorySource, string zipFileName)
        {
            if (!Directory.Exists(directorySource))
            {
                throw new Exception($"Не найден каталог {directorySource}");
            }


            ZipFile.CreateFromDirectory(directorySource, zipFileName);
        }

        public void CompressFile(string fullFileName)
        {
            if (!File.Exists(fullFileName))
            {
                throw new Exception($"Не найден файл {fullFileName}");
            }

            var file = new FileInfo(fullFileName);
            var fullFileNameZip = Path.Combine(file.DirectoryName, $"{file.Name.Replace(file.Extension, "")}.zip");
            using (var fs = new FileStream(fullFileNameZip, FileMode.Create))
            using (var arch = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                arch.CreateEntryFromFile(file.FullName, file.Name);
            }
        }

        public byte[] Compress(string text)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var demoFile = zipArchive.CreateEntry("zipped.txt");

                    using (var entryStream = demoFile.Open())
                    {
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.Write(text);
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }

        public string Extract(byte[] bytes)
        {
            using (var zippedStream = new MemoryStream(bytes))
            {
                using (var archive = new ZipArchive(zippedStream))
                {
                    var entry = archive.Entries.FirstOrDefault();

                    if (entry != null)
                    {
                        using (var unzippedEntryStream = entry.Open())
                        {
                            using (var ms = new MemoryStream())
                            {
                                unzippedEntryStream.CopyTo(ms);
                                var unzippedArray = ms.ToArray();

                                return Encoding.UTF8.GetString(unzippedArray);
                            }
                        }
                    }

                    return String.Empty;
                }
            }
        }

        public void Extract(string zipFileName, string directoryTarget)
        {
            if (!File.Exists(zipFileName))
            {
                throw new Exception($"Не найден файл {zipFileName}");
            }

            ZipFile.ExtractToDirectory(zipFileName, directoryTarget);
        }
    }
}
