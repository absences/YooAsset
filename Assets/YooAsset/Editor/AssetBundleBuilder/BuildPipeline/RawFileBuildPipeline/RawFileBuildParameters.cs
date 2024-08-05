using System.Collections;
using System.Collections.Generic;

namespace YooAsset.Editor
{
    public class RawFileBuildParameters : BuildParameters
    {
        /// <summary>
        /// 从game_assets拷贝资源
        /// </summary>
        public string CopyGameAssetsFrom;

        public string buildVersionName;

        public string patchDir;

        public bool complielua;

        //patch collection
        /// <summary>
        /// 资源列表
        /// </summary>
        internal List<PackageAsset> AssetList = new List<PackageAsset>();

        /// <summary>
        /// 资源包列表
        /// </summary>
        internal List<PackageBundle> BundleList = new List<PackageBundle>();
    }
}