
namespace Infrastructure.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IFileSystemService
    {
        char DirectorySeparatorChar { get; }

        void CreateDirectory(string directoryName);

        bool DirectoryExists(string directoryName);

        void DirectoryDelete(string directoryName, bool recursive);

        bool FileExists(string fileName);

        void FileDelete(string fileName);

        string GetTempPath();

        string GetFileName(string fileName);

        string GetExtension(string fileName);

        string GetFileNameWithoutExtension(string fileName);

        string[] GetFiles(string path, string searchPattern);

        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

        string PathCombine(params string[] paths);

        string GetApplicationDataDirectory();

        byte[] GetFileContentAsBytes(string fileName);

        void WriteFileContent(string fileName, byte[] fileContent);

        void WriteFileContent(string fileName, string fileContent);

        string GetDirectoryPath(string fileName);

        string GetDirectoryName(string directoryPath);

        string GetDirectoryParentPath(string directoryPath);

        string GetExecutableDirectory();

        bool IsPathRooted(string fileNamePath);

        IEnumerable<string> EnumerateDirectories(string path);

        IEnumerable<string> EnumerateFiles(string path);

        void WriteStreamToFile(Stream content, string path);

        Stream OpenRead(string path);

        Stream Open(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);

        TextReader CreateReader(string path);

        TextWriter CreateWriter(string path);

        string ChangeExtention(string path, string extension);

        void CopyFile(string source, string destination);

        void CopyFile(string source, string destination, bool overwrite);

        void MoveFile(string source, string destination);

        void CopyDirectoryContent(string source, string destination);

        DateTime GetFileLastAccessTime(string fullFileName);

        long GetFileLength(string fullFileName);

        string GetApplicationBaseDirectory();

        string RemoveTrailingSeparator(string value);

        string GetFullPath(string path);
    }
}
