using System.IO;

namespace YooAsset.Editor
{
    /// <summary>
    /// 将资源拷贝至sa
    /// </summary>
    public class TaskCopyBuildIn_RF
    {

        /// <summary>
        /// 拷贝纳入资源列表文件
        /// </summary>
        /// <param name="patchPath"></param>
        /// <param name="buildPackageName"></param>
        /// <param name="hotPack">热更包</param>
        public void CopyPackageFiles(string patchPath, string buildPackageName, bool hotPack)
        {
            // 从patch拷贝工程package文件
            string BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

            string versionName = YooAssetSettingsData.GetPackageVersionFileName(buildPackageName);

            if (Directory.Exists($"{BuildinFileRoot}{buildPackageName}"))
                Directory.Delete($"{BuildinFileRoot}{buildPackageName}", true);

            string versionfilePath = $"{patchPath}/{buildPackageName}/{versionName}";

            var version = File.ReadAllText(versionfilePath);

            string ManifestName = YooAssetSettingsData.GetManifestBinaryFileName(buildPackageName, version);

            var ManifestfilePath = $"{patchPath}/{buildPackageName}/{ManifestName}";

            // 拷贝补丁清单文件
            {
                string destPath = $"{BuildinFileRoot}{buildPackageName}/{ManifestName}";
                EditorTools.CopyFile(ManifestfilePath, destPath, true);
            }

            // 拷贝补丁清单哈希文件
            {
                string fileName = YooAssetSettingsData.GetPackageHashFileName(buildPackageName, version);
                string sourcePath = $"{patchPath}/{buildPackageName}/{fileName}";
                string destPath = $"{BuildinFileRoot}{buildPackageName}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }

            // 拷贝补丁清单版本文件
            {
                string fileName = YooAssetSettingsData.GetPackageVersionFileName(buildPackageName);
                string sourcePath = $"{patchPath}/{buildPackageName}/{fileName}";
                string destPath = $"{BuildinFileRoot}{buildPackageName}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }

            byte[] bytesData = FileUtility.ReadAllBytes(ManifestfilePath);
            var Manifest = ManifestTools.DeserializeFromBinary(bytesData);


            var ignoreTags = new string[]{}; //{ "atlas/ui" };//不移动至sa的tag

            foreach (var packageBundle in Manifest.BundleList)
            {
                if (hotPack && packageBundle.HasTag(ignoreTags))
                    continue;

                string sourcePath = $"{patchPath}/{buildPackageName}/{packageBundle.FileName}";
                string destPath = $"{BuildinFileRoot}{buildPackageName}/{packageBundle.FileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }
        }
    }
}