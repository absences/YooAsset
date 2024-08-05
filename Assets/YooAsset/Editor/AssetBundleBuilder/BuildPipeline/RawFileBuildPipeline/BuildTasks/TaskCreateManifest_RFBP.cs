using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace YooAsset.Editor
{
    public class TaskCreateManifest_RFBP : TaskCreateManifest, IBuildTask
    {
        RawFileBuildParameters param;
        public void Run(BuildContext context)
        {
            var buildParametersContext = context.GetContextObject<BuildParametersContext>();
            var buildParameters = buildParametersContext.Parameters;
            param = buildParameters as RawFileBuildParameters;
            CreateManifestFile(context);
        }

        protected override string[] GetBundleDepends(BuildContext context, string bundleName)
        {
            return new string[] { };
        }
        internal override void AddPatchList(PackageManifest manifest)
        {
            if (param == null || param.CopyGameAssetsFrom == "from_client_game_assets")
            {
                return;
            }

            var assetList = manifest.AssetList;
            var bundleList = manifest.BundleList;

            manifest.AssetList = param.AssetList;
            manifest.BundleList = param.BundleList;//从文件读取的信息
            // 注意：缓存资源包索引
            _cachedBundleID.Clear();//清理旧索引
            for (int index = 0; index < manifest.BundleList.Count; index++)
            {
                string bundleName = manifest.BundleList[index].BundleName;
                _cachedBundleID.Add(bundleName, index);
            }


            foreach (var asset in assetList)//patch资源列表
            {
                bool modify = false;
                var bundle = bundleList[asset.BundleID];
                //如果是修改 ，那么更改BundleList的 hash
                foreach (var item in manifest.BundleList)
                {
                    if (item.BundleName == bundle.BundleName)//修改bundle, assinfo不会变化
                    {
                        item.UnityCRC = bundle.UnityCRC;
                        item.FileHash = bundle.FileHash;
                        item.FileCRC = bundle.FileCRC;
                        item.FileSize = bundle.FileSize;
                        item.Encrypted = bundle.Encrypted;
                        item.DependIDs = bundle.DependIDs;
                        item.Tags = bundle.Tags;

                        modify = true;
                        break;
                    }
                }
                //新增 添加到列表 并修改其BundleID
                if (!modify)
                {
                    manifest.BundleList.Add(bundle);
                    var index = _cachedBundleID.Count;
                    _cachedBundleID.Add(bundle.BundleName, index);
                    asset.BundleID = index;
                    manifest.AssetList.Add(asset);
                }
            }

            param.AssetList = assetList;
            param.BundleList = bundleList;
        }
    }
}