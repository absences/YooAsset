using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace YooAsset.Editor
{
    public class TaskEncryption
    {

        /// <summary>
        /// 加密原生文件
        /// </summary>
        /// <param name="buildParametersContext"></param>
        /// <param name="buildMapContext"></param>
        protected void EncryptingRawFiles(BuildParametersContext buildParametersContext, BuildMapContext buildMapContext)
        {

            var encryptionServices = buildParametersContext.Parameters.EncryptionServices;
            if (encryptionServices == null)
                return;

            if (encryptionServices.GetType() == typeof(EncryptionNone))
                return;

            int progressValue = 0;
            string pipelineOutputDirectory = buildParametersContext.GetPipelineOutputDirectory();

            var Collection = buildMapContext.Collection;

            foreach (var info in Collection.Values)
            {
                var BundleName = info.BundleName;
                if (string.IsNullOrEmpty(BundleName))
                {
                    continue;
                }
                EncryptFileInfo fileInfo = new EncryptFileInfo();

                fileInfo.BundleName = BundleName;

                fileInfo.FileLoadPath = $"{pipelineOutputDirectory}/{BundleName}";
                var encryptResult = encryptionServices.Encrypt(fileInfo);

                info.Encrypted = encryptResult.Encrypted;

                if (encryptResult.Encrypted)
                {
                    string filePath = $"{pipelineOutputDirectory}/{BundleName}.encrypt";

                    FileUtility.WriteAllBytes(filePath, encryptResult.EncryptedData);

                    info.EncryptedFilePath = filePath;

                    //BuildLogger.Log($"Bundle file encryption complete: {filePath}");
                }

                EditorTools.DisplayProgressBar("Encrypting rawfile", ++progressValue, Collection.Count);
            }
            EditorTools.ClearProgressBar();
        }

        /// <summary>
        /// 加密资源文件
        /// </summary>
        public void EncryptingBundleFiles(BuildParametersContext buildParametersContext, BuildMapContext buildMapContext)
        {
            var encryptionServices = buildParametersContext.Parameters.EncryptionServices;
            if (encryptionServices == null)
                return;

            if (encryptionServices.GetType() == typeof(EncryptionNone))
                return;

            int progressValue = 0;
            string pipelineOutputDirectory = buildParametersContext.GetPipelineOutputDirectory();
            var Collection = buildMapContext.GetPipelineBuilds();

            foreach (var bundleInfo in Collection)
            {
                var BundleName = bundleInfo.assetBundleName;
                if (string.IsNullOrEmpty(BundleName))
                {
                    continue;
                }
                EncryptFileInfo fileInfo = new EncryptFileInfo();

                fileInfo.BundleName = BundleName;

                fileInfo.FileLoadPath = $"{pipelineOutputDirectory}/{BundleName}";
                var encryptResult = encryptionServices.Encrypt(fileInfo);

                var info = buildMapContext.Collection[BundleName];
                info.Encrypted = encryptResult.Encrypted;

                if (encryptResult.Encrypted)
                {
                    string filePath = $"{pipelineOutputDirectory}/{BundleName}.encrypt";

                    FileUtility.WriteAllBytes(filePath, encryptResult.EncryptedData);

                    info.EncryptedFilePath = filePath;

                    //BuildLogger.Log($"Bundle file encryption complete: {filePath}");
                }
                // 进度条
                EditorTools.DisplayProgressBar("Encrypting bundle", ++progressValue, Collection.Length);
            }
            EditorTools.ClearProgressBar();
        }
    }
}