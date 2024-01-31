namespace Infrastructure.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using GlobalSpace.Common.Guardly;

    [ExcludeFromCodeCoverage]
    public sealed class FileSystemService : IFileSystemService
    {
        public char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }

        public void CreateDirectory(string directoryName)
        {
            Guard.Argument(() => directoryName, Is.NotNull);

            Directory.CreateDirectory(directoryName);
        }

        public bool DirectoryExists(string directoryName)
        {
            Guard.Argument(() => directoryName, Is.NotNull);

            return Directory.Exists(directoryName);
        }

        public void DirectoryDelete(string directoryName, bool recursive)
        {
            Guard.Argument(() => directoryName, Is.NotNull);

            Directory.Delete(directoryName, recursive);
        }

        public bool FileExists(string fileName)
        {
            Guard.Argument(() => fileName, Is.NotNull);

            return File.Exists(fileName);
        }

        public void FileDelete(string fileName)
        {
            Guard.Argument(() => fileName, Is.NotNull);

            File.Delete(fileName);
        }

        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public string GetFileName(string fileName)
        {
            Guard.Argument(() => fileName, Is.NotNull);

            return Path.GetFileName(fileName);
        }

        public string GetExtension(string fileName)
        {
            Guard.Argument(() => fileName, Is.NotNull);

            return Path.GetExtension(fileName);
        }

        public string GetFileNameWithoutExtension(string fileName)
        {
            Guard.Argument(() => fileName, Is.NotNull);

            return Path.GetFileNameWithoutExtension(fileName);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            Guard.Argument(() => path, Is.NotNull);
            Guard.Argument(() => searchPattern, Is.NotNull);

            return Directory.GetFiles(path, searchPattern);
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            Guard.Argument(() => path, Is.NotNull);
            Guard.Argument(() => searchPattern, Is.NotNull);

            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        public string PathCombine(params string[] paths)
        {
            Guard.Argument(() => paths, Is.NotNull);

            return Path.Combine(paths);
        }

        public string GetApplicationDataDirectory()
        {
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            var applicationDirectoryName = Path.Combine(applicationData, applicationName);

            if (!Directory.Exists(applicationDirectoryName))
            {
                Directory.CreateDirectory(applicationDirectoryName);
            }

            return applicationDirectoryName;
        }

        public byte[] GetFileContentAsBytes(string fileName)
        {
            Guard.Argument(() => fileName, Is.NotNull);

            return File.ReadAllBytes(fileName);
        }

        public void WriteFileContent(string fileName, byte[] fileContent)
        {
            Guard.Argument(() => fileName, Is.NotNull);
            Guard.Argument(() => fileContent, Is.NotNull);

            File.WriteAllBytes(fileName, fileContent);
        }

        public void WriteFileContent(string fileName, string fileContent)
        {
            Guard.Argument(() => fileName, Is.NotNull);
            Guard.Argument(() => fileContent, Is.NotNull);

            File.WriteAllText(fileName, fileContent);
        }

        public string GetDirectoryPath(string fileName)
        {
            Guard.Argument(() => fileName, Is.NotNull);

            return Path.GetDirectoryName(fileName);
        }

        public string GetDirectoryName(string directoryPath)
        {
            Guard.Argument(() => directoryPath, Is.NotNull);

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

                return directoryInfo.Name;
            }
            catch (Exception e)
            {
                // logAdapter.Error(e, e.Message);
                throw;
            }
        }

        public string GetDirectoryParentPath(string directoryPath)
        {
            Guard.Argument(() => directoryPath, Is.NotNull);

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

                return directoryInfo.Parent != null ? directoryInfo.Parent.FullName : string.Empty;
            }
            catch (Exception e)
            {
                //logAdapter.Error(e, e.Message);
                throw;
            }
        }

        public string GetExecutableDirectory()
        {
            return Environment.CurrentDirectory;
        }

        public bool IsPathRooted(string fileNamePath)
        {
            Guard.Argument(() => fileNamePath, Is.NotNull);

            return Path.IsPathRooted(fileNamePath);
        }


        public IEnumerable<string> EnumerateDirectories(string path)
        {
            Guard.Argument(() => path, Is.NotNull);

            return Directory.EnumerateDirectories(path);
        }

        public IEnumerable<string> EnumerateFiles(string path)
        {
            Guard.Argument(() => path, Is.NotNullOrEmpty);

            return Directory.EnumerateFiles(path);
        }

        public void WriteStreamToFile(Stream content, string path)
        {
            Guard.Argument(() => content, Is.NotNull);
            Guard.Argument(() => path, Is.NotNullOrEmpty);

            using (var file = File.Create(path))
            {
                content.CopyTo(file);
            }
        }

        public Stream OpenRead(string path)
        {
            Guard.Argument(() => path, Is.NotNull);

            return File.OpenRead(path);
        }

        public Stream Open(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            Guard.Argument(() => path, Is.NotNull);

            return File.Open(path, fileMode, fileAccess, fileShare);
        }

        public TextReader CreateReader(string path)
        {
            Guard.Argument(() => path, Is.NotNull);

            return new StreamReader(path);
        }

        public TextWriter CreateWriter(string path)
        {
            Guard.Argument(() => path, Is.NotNull);

            return new StreamWriter(path, false, Encoding.UTF8);
        }

        public string ChangeExtention(string path, string extension)
        {
            Guard.Argument(() => path, Is.NotNull);
            Guard.Argument(() => extension, Is.NotNull);

            return Path.ChangeExtension(path, extension);
        }

        public void CopyFile(string source, string destination)
        {
            Guard.Argument(() => source, Is.NotNull);
            Guard.Argument(() => destination, Is.NotNull);

            File.Copy(source, destination);
        }

        public void CopyFile(string source, string destination, bool overwrite)
        {
            Guard.Argument(() => source, Is.NotNull);
            Guard.Argument(() => destination, Is.NotNull);

            File.Copy(source, destination, overwrite);
        }

        public void CopyDirectoryContent(string source, string destination)
        {
            Guard.Argument(() => source, Is.NotNull);
            Guard.Argument(() => destination, Is.NotNull);

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            foreach (string directoryPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directoryPath.Replace(source, destination));
            }

            foreach (string filePath in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, filePath.Replace(source, destination), true);
            }
        }

        public void MoveFile(string source, string destination)
        {
            Guard.Argument(() => source, Is.NotNull);
            Guard.Argument(() => destination, Is.NotNull);

            File.Move(source, destination);
        }

        public DateTime GetFileLastAccessTime(string fullFileName)
        {
            Guard.Argument(() => fullFileName, Is.NotNull);

            return new FileInfo(fullFileName).LastAccessTime;
        }

        public long GetFileLength(string fullFileName)
        {
            return new FileInfo(fullFileName).Length;
        }

        public string GetApplicationBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public string RemoveTrailingSeparator(string value)
        {
            Guard.Argument(() => value, Is.NotNullOrEmpty);

            return value.TrimEnd(DirectorySeparatorChar);
        }

        public string GetFullPath(string path)
        {
            Guard.Argument(() => path, Is.NotNull);

            return Path.GetFullPath(path);
        }
    }
}
