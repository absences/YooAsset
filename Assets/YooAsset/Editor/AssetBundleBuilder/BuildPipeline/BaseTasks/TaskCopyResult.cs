using System.IO;

namespace YooAsset.Editor
{
    public class TaskCopyResult
    {
        internal void CopyResult(BuildParametersContext buildParametersContext, PackageManifest manifest)
        {
            // client目录
            var outputRoot = AssetBundleBuilderHelper.GetManualClientBuildOutputRoot();

            string clientDirectory = outputRoot + "/"
                    + buildParametersContext.Parameters.BuildTarget + "/" + YooAssetSettingsData.Setting.clientName;

            string buildPackageName = buildParametersContext.Parameters.PackageName;
            string buildPackageVersion = buildParametersContext.Parameters.PackageVersion;


            if (Directory.Exists(clientDirectory))
            {
                Directory.Delete(clientDirectory, true);
            }
            Directory.CreateDirectory(clientDirectory);


            // 拷贝所需文件
            string packageOutputDirectory = buildParametersContext.GetPackageOutputDirectory();
            // 拷贝补丁清单文件
            {
                string fileName = YooAssetSettingsData.GetManifestBinaryFileName(buildPackageName, buildPackageVersion);

                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{clientDirectory}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);

                fileName = YooAssetSettingsData.GetManifestJsonFileName(buildPackageName, buildPackageVersion);

                sourcePath = $"{packageOutputDirectory}/{fileName}";
                destPath = $"{clientDirectory}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }

            // 拷贝补丁清单哈希文件
            {
                string fileName = YooAssetSettingsData.GetPackageHashFileName(buildPackageName, buildPackageVersion);
                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{clientDirectory}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }

            // 拷贝补丁清单版本文件
            {
                string fileName = YooAssetSettingsData.GetPackageVersionFileName(buildPackageName);
                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{clientDirectory}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }

            foreach (var packageBundle in manifest.BundleList)
            {
                string sourcePath = $"{packageOutputDirectory}/{packageBundle.FileName}";
                string destPath = $"{clientDirectory}/{packageBundle.FileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }
        }
    }
}