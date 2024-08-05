namespace YooAsset.Editor
{
    public class TaskBuildPatch_RFBP : IBuildTask
    {
        public void Run(BuildContext context)
        {
            var buildParametersContext = context.GetContextObject<BuildParametersContext>();

            var buildMapContext = context.GetContextObject<BuildMapContext>();
            var param = buildParametersContext.Parameters as RawFileBuildParameters;

            string pipelineOutputDirectory = buildParametersContext.GetPipelineOutputDirectory();
            int outputNameStyle = (int)param.FileNameStyle;

            var buildPackageName = param.PackageName;
            var buildPackageVersion = param.PackageVersion;

            string packageOutputDirectory = buildParametersContext.GetPackageOutputDirectory();

            // 拷贝补丁清单文件
            {
                string fileName = YooAssetSettingsData.GetManifestBinaryFileName(buildPackageName, buildPackageVersion);

                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{param.patchDir}/{param.PackageName}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);

                fileName = YooAssetSettingsData.GetManifestJsonFileName(buildPackageName, buildPackageVersion);

                sourcePath = $"{packageOutputDirectory}/{fileName}";
                destPath = $"{param.patchDir}/{param.PackageName}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }

            // 拷贝补丁清单哈希文件
            {
                string fileName = YooAssetSettingsData.GetPackageHashFileName(buildPackageName, buildPackageVersion);
                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{param.patchDir}/{param.PackageName}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }

            // 拷贝补丁清单版本文件
            {
                string fileName = YooAssetSettingsData.GetPackageVersionFileName(buildPackageName);
                string sourcePath = $"{packageOutputDirectory}/{fileName}";
                string destPath = $"{param.patchDir}/{param.PackageName}/{fileName}";
                EditorTools.CopyFile(sourcePath, destPath, true);
            }


            // 拷贝所有补丁文件
            int progressValue = 0;
            int fileTotalCount = buildMapContext.Collection.Count;
            foreach (var bundleInfo in buildMapContext.Collection.Values)
            {

                string from = $"{pipelineOutputDirectory}/{bundleInfo.BundleName}";

                string bundleName = bundleInfo.BundleName;
                string fileHash = bundleInfo.PackageFileHash;
                string fileExtension = ManifestTools.GetRemoteBundleFileExtension(bundleName);
                string fileName = ManifestTools.GetRemoteBundleFileName(outputNameStyle, bundleName, fileExtension, fileHash);

                string dest = $"{param.patchDir}/{param.PackageName}/{fileName}";

                EditorTools.CopyFile(from, dest, true);
                EditorTools.DisplayProgressBar("Copy patch file", ++progressValue, fileTotalCount);

                UnityEngine.Debug.Log(bundleName + "  " + dest);
            }
            EditorTools.ClearProgressBar();

        }
    }
}