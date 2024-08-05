using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;

namespace YooAsset.Editor
{
    public class TaskGetBuildMap_RFBP : TaskGetBuildMap, IBuildTask
    {
        void IBuildTask.Run(BuildContext context)
        {
            var buildParametersContext = context.GetContextObject<BuildParametersContext>();
            var buildMapContext = CreateBuildMap(buildParametersContext.Parameters);

            Dictionary<string, BuildBundleInfo> Collection = buildMapContext.Collection;

            var rawParam = buildParametersContext.Parameters as RawFileBuildParameters;

            if (rawParam.complielua)
            {
                string exePath = Directory.GetCurrentDirectory() + "/../libs/lua/luac.exe";

                if (!File.Exists(exePath))
                {
                    throw new Exception("luac 不存在");
                 
                }

                string clientAssets = EditorTools.GetProjectPath() + "/game_assets";

                List<string> allFiles = new List<string>(512);
                string resPath = clientAssets + "/scripts/";
                string[] fileNames = Directory.GetFiles(resPath, "*.lua", SearchOption.AllDirectories);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    allFiles.Add(fileNames[i].Replace("\\", "/"));
                }

                resPath = clientAssets + "/cfg/";
                fileNames = Directory.GetFiles(resPath, "*.lua", SearchOption.AllDirectories);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    allFiles.Add(fileNames[i].Replace("\\", "/"));
                }

        
                string workPath = clientAssets + "/";

                ProcessStartInfo processStartInfo = new ProcessStartInfo();


                for (int i = 0; i < allFiles.Count; ++i)
                {
                    string fileName = allFiles[i];

                    fileName = fileName.Substring(workPath.Length, fileName.Length - workPath.Length);
                    processStartInfo.WorkingDirectory = workPath;
                    processStartInfo.CreateNoWindow = true;
                    processStartInfo.FileName = exePath;
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.RedirectStandardInput = true;
                    processStartInfo.RedirectStandardOutput = true;
                    processStartInfo.RedirectStandardError = true;

                    processStartInfo.Arguments = "-o " + fileName + " " + fileName;
                    var p = Process.Start(processStartInfo);

                    p.WaitForExit();

                }
            }


            if (rawParam.CopyGameAssetsFrom == "from_client_game_assets")
            {
                string clientAssets = EditorTools.GetProjectPath() + "/game_assets";
                //从game_assets 下生成MainAssets信息
                GenCollection(Collection, clientAssets);
            }
            else if(rawParam.CopyGameAssetsFrom == "from_patch_game_assets")
            {
                string versionName = YooAssetSettingsData.GetPackageVersionFileName(rawParam.PackageName);

                string versionfilePath = $"{rawParam.patchDir}/{rawParam.PackageName}/{versionName}";
                var version = File.ReadAllText(versionfilePath);

                string ManifestName =
                    YooAssetSettingsData.GetManifestBinaryFileName(rawParam.PackageName, version);

                string JsonManifest = YooAssetSettingsData.GetManifestJsonFileName(rawParam.PackageName, version);

                var ManifestJsonPath = $"{rawParam.patchDir}/{rawParam.PackageName}/{JsonManifest}";

                var ManifestfilePath = $"{rawParam.patchDir}/{rawParam.PackageName}/{ManifestName}";
                //读取Manifest信息
                byte[] bytesData = FileUtility.ReadAllBytes(ManifestfilePath);
                var Manifest = ManifestTools.DeserializeFromBinary(bytesData);

                rawParam.AssetList = Manifest.AssetList;
                rawParam.BundleList = Manifest.BundleList;

                string assets = rawParam.patchDir + "/game_assets";
                GenCollection(Collection, assets);

                var hashName = YooAssetSettingsData.GetPackageHashFileName(rawParam.PackageName, version);

                //删除旧文件
                File.Delete(versionfilePath);
                File.Delete(ManifestJsonPath);
                File.Delete(ManifestfilePath);
                File.Delete($"{rawParam.patchDir}/{rawParam.PackageName}/{hashName}");
            }

            context.SetContextObject(buildMapContext);
        }
        /// <summary>
        /// 目录名称与是否保持原后缀
        /// </summary>
        private RawFileCollectConfig _collectConfig;
        private RawFileCollectConfig CollectConfig
        {
            get
            {
                if (_collectConfig == null)
                {
                    _collectConfig = AssetDatabase.
                        LoadAssetAtPath<RawFileCollectConfig>("Assets/Config/rawFileCollect_cfg.asset");
                }

                return _collectConfig;
            }
        }
        /// <summary>
        /// 拷贝pc资源
        /// </summary>
        /// <param name="path"></param>
        public void CopyFiles(string path)
        {
            string pcAssets = path + "/game_assets";

    
            if (Directory.Exists(pcAssets))
                Directory.Delete(pcAssets, true);

            Directory.CreateDirectory(pcAssets);
            string clientAssets = EditorTools.GetProjectPath() + "/game_assets";
            for (int i = 0; i < CollectConfig.Cfgs.Length; i++)
            {
                var item= CollectConfig.Cfgs[i];    
                if (item.pathName == "data")
                    continue;
                var folder = clientAssets + "/" + item.pathName;
                FileUtil.CopyFileOrDirectory(folder, pcAssets + "/" + item.pathName);
            }

            pcAssets = path + "/game_assets/StandaloneWindows64";

            if (Directory.Exists(pcAssets))
                Directory.Delete(pcAssets, true);
            Directory.CreateDirectory(pcAssets);

            FileUtil.CopyFileOrDirectory(EditorTools.GetProjectPath()
                + "/game_assets/StandaloneWindows64/assets", pcAssets + "/assets");
        }


        int maxDeep = 10;//最大深度

        void GenCollection(Dictionary<string, BuildBundleInfo> collection, string path)
        {
            for (int i = 0; i < CollectConfig.Cfgs.Length; i++)
            {
                var groupName = CollectConfig.Cfgs[i].pathName;

                var folder = path + "/" + groupName;
                if (Directory.Exists(folder))
                {

                    string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);

                    for (int j = 0; j < files.Length; j++)
                    {
                        FileInfo fileInfo = new FileInfo(files[j]);

                      
                        var directory = fileInfo.Directory;

                        var fileName = fileInfo.Name;

                        string address = Path.GetFileNameWithoutExtension(fileName);

                        if (address.EndsWith("~"))
                        {
                            continue;
                        }

                        int cur = 0;
                        while (directory != null && directory.Name != groupName)
                        {
                            address = directory.Name + "_" + address;
                            directory = directory.Parent;
                            cur++;
                            if (cur == maxDeep)
                                break;
                        }
                        string AssetPath;
                        if (CollectConfig.Cfgs[i].originalExtension)
                        {
                            string fileExtension = Path.GetExtension(fileName);
                            AssetPath = $"{groupName}_{address}{fileExtension}";//aaa/xxx.mp4
                        }
                        else
                        {
                            AssetPath = $"{groupName}_{address}.rawfile";
                        }
                        var info = new BuildBundleInfo(AssetPath)
                        {
                            BundlePath = files[j],
                        };

                        var buildInfo = new BuildAssetInfo(
                                new AssetInfo($"{groupName}_{address}"), AssetPath);

                         buildInfo.AddAssetTags(new List<string>() { groupName }); //暂时不加tag

                        info.PackAsset(buildInfo);
                        // key=aaa/xxx.lua   BundleName= aaa_xxx
                        collection.Add(AssetPath, info);
                    }
                }
            }
        }
    }
}