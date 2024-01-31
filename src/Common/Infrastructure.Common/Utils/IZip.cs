
namespace Infrastructure.Common.Utils
{
    public interface IZip
    {
        void CompressDirectory(string directorySource, string zipFileName);

        void Extract(string zipFileName, string directoryTarget);

        void CompressFile(string fullFileName);

        byte[] Compress(string text);
        string Extract(byte[] bytes);
    }
}
