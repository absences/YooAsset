
using System;
using System.Text;
using UnityEngine;

namespace YooAsset
{
    /// <summary>
    /// 解压文件系统
    /// </summary>
    internal class DefaultUnpackFileSystem : DefaultCacheFileSystem
    {
        public DefaultUnpackFileSystem()
        {
        }
        public override void OnCreate(string packageName, string rootDirectory)
        {
            base.OnCreate(packageName, rootDirectory);

            // 注意：重写保存根目录和临时目录
            _saveFileRoot = PathUtility.Combine(_packageRoot, DefaultUnpackFileSystemDefine.SaveFilesFolderName);
            _tempFileRoot = PathUtility.Combine(_packageRoot, DefaultUnpackFileSystemDefine.TempFilesFolderName);
        }

        public override FSLoadBundleOperation LoadBundleFile(PackageBundle bundle)
        {
            //原生方法加载
#if UNITY_ANDROID
            if (RawFileBuildPipeline)
            {
                if(bundle.HasTag(new string[] { "scripts", "cfg" }))
                {
                    var operation = new DCFSLoadAndroidBundleDataOperation(this, bundle);
                    OperationSystem.StartOperation(PackageName, operation);
                    return operation;
                }
                else if(bundle.HasTag(new string[] { "video" }))
                {
                    var operation = new DCFSLoadAndroidVideoOperation(this, bundle);
                    OperationSystem.StartOperation(PackageName, operation);
                    return operation;
                }
            }
#endif

            return base.LoadBundleFile(bundle);
        }

        internal class DCFSLoadAndroidVideoOperation: FSLoadBundleOperation
        {
            protected readonly DefaultCacheFileSystem _fileSystem;
            protected readonly PackageBundle _bundle;

            internal DCFSLoadAndroidVideoOperation(DefaultCacheFileSystem fileSystem, PackageBundle bundle)
            {
                _fileSystem = fileSystem;
                _bundle = bundle;
            }
            public override void AbortDownloadOperation()
            {

            }
            internal override void InternalOnStart()
            {

                Status = EOperationStatus.Succeed;

                var path = 
                    PathUtility.Combine(Application.streamingAssetsPath, _bundle.PackageName, _bundle.FileName);

                Result = new RawBundle(_fileSystem, _bundle, path);

                DownloadProgress = 1f;
            }

            internal override void InternalOnUpdate()
            {

            }
        }

        internal class DCFSLoadAndroidBundleDataOperation : FSLoadBundleOperation
        {
            protected readonly DefaultCacheFileSystem _fileSystem;
            protected readonly PackageBundle _bundle;

            static AndroidJavaObject nativeReader;
            static AndroidJavaObject NativeReader
            {
                get
                {
                    if (nativeReader == null) 
                        nativeReader = new AndroidJavaClass("com.game.extralibrary.NativeReader");

                    return nativeReader;
                }

            }
            internal DCFSLoadAndroidBundleDataOperation(DefaultCacheFileSystem fileSystem, PackageBundle bundle)
            {
                _fileSystem = fileSystem;
                _bundle = bundle;
            }
            public override void AbortDownloadOperation()
            {

            }
            internal override void InternalOnStart()
            {
                Status = EOperationStatus.Succeed;

                var path = PathUtility.Combine(_bundle.PackageName, _bundle.FileName);

                sbyte[] o = NativeReader.CallStatic<sbyte[]>("loadFile", path);
                Result = new RawBundle(_fileSystem, _bundle, o);
                DownloadedBytes = o.LongLength;

                DownloadProgress = 1f;
            }

            internal override void InternalOnUpdate()
            {

            }
        }
    }
}