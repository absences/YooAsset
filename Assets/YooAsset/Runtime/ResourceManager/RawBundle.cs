
using System;
using System.Text;
namespace YooAsset
{
    internal class RawBundle
    {
        private readonly IFileSystem _fileSystem;
        private readonly PackageBundle _packageBundle;
        private readonly string _filePath;

        public readonly byte[] RawFileData;
        internal RawBundle(IFileSystem fileSystem, PackageBundle packageBundle, string filePath)
        {
            _fileSystem = fileSystem;
            _packageBundle = packageBundle;
            _filePath = filePath;
        }
        internal RawBundle(IFileSystem fileSystem, PackageBundle packageBundle, sbyte[] data)
        {
            _fileSystem = fileSystem;
            _packageBundle = packageBundle;
            RawFileData = (byte[])(Array)data;
        }
        public string GetFilePath()
        {
            return _filePath;
        }
        public byte[] ReadFileData()
        {
            if (RawFileData != null)
                return RawFileData;

            if (_fileSystem != null)
                return _fileSystem.ReadFileData(_packageBundle);
            else
                return FileUtility.ReadAllBytes(_filePath);
        }
        public string ReadFileText()
        {
            if (RawFileData != null)
            {
                return Encoding.UTF8.GetString(RawFileData);
            }
                if (_fileSystem != null)
                return _fileSystem.ReadFileText(_packageBundle);
            else
                return FileUtility.ReadAllText(_filePath);
        }
    }
}