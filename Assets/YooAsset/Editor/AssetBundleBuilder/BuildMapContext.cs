using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace YooAsset.Editor
{
    public class BuildMapContext : IContextObject
    {
        /// <summary>
        /// 资源包集合
        /// </summary>
        private readonly Dictionary<string, BuildBundleInfo> _bundleInfoDic = new Dictionary<string, BuildBundleInfo>(10000);
        /// <summary>
        /// 资源包信息列表
        /// </summary>
        public Dictionary<string, BuildBundleInfo> Collection
        {
            get
            {
                return _bundleInfoDic;
            }
        }

        private AssetBundleBuild[] assetBundleBuilds = null;
        /// <summary>
        /// 获取构建管线里需要的数据 存在空
        /// </summary>
        public AssetBundleBuild[] GetPipelineBuilds(bool debug = false)
        {
            if (assetBundleBuilds != null)
                return assetBundleBuilds;

            // 定义排除列表，包含不需要打包的Asset Bundle名称
            List<string> exclusionList = new List<string>();
            if (!debug)
                exclusionList.Add("debug/");

            // 获取所有的Asset Bundle路径
            string[] allBundlePaths = AssetDatabase.GetAllAssetBundleNames();

            // 创建AssetBundleBuild数组，用于存储要打包的Asset Bundle信息
            assetBundleBuilds = new AssetBundleBuild[allBundlePaths.Length];

            for (int i = 0; i < allBundlePaths.Length; i++)
            {
                string bundlePath = allBundlePaths[i];

                // 检查当前的Asset Bundle路径是否在排除列表中
                if (!IsExcludedBundle(bundlePath, exclusionList))
                {
                    // 创建AssetBundleBuild对象并添加Asset Bundle信息
                    AssetBundleBuild buildInfo = new AssetBundleBuild();
                    buildInfo.assetBundleName = bundlePath;

                    BuildBundleInfo info = new BuildBundleInfo(bundlePath);

                    buildInfo.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundlePath);
                    for (int j = 0; j < buildInfo.assetNames.Length; j++)
                    {
                        //Assets/Res/WorldMap/Building_field/Texture/alliance_fort_mask.png

                        info.PackAsset(
                            new BuildAssetInfo(new AssetInfo(buildInfo.assetNames[j]), bundlePath));
                    }
                    assetBundleBuilds[i] = buildInfo;

                    _bundleInfoDic.Add(bundlePath, info);
                }
            }
            return assetBundleBuilds;
        }


        private static bool IsExcludedBundle(string bundlePath, List<string> exclusionList)
        {
            // 检查当前的Asset Bundle路径是否在排除列表中
            foreach (string excludedPrefix in exclusionList)
            {
                if (bundlePath.StartsWith(excludedPrefix))
                    return true;
            }
            return false;
        }
    }
}