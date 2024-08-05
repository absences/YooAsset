using System.Collections.Generic;

namespace YooAsset.Editor
{
    public class RawFileBuildPipeline : IBuildPipeline
    {
        public BuildResult Run(BuildParameters buildParameters, bool enableLog)
        {
            AssetBundleBuilder builder = new AssetBundleBuilder();
            var rawParam = buildParameters as RawFileBuildParameters;
            if (rawParam.CopyGameAssetsFrom == "from_client_game_assets")
                return builder.Run(buildParameters, GetDefaultBuildPipeline(), enableLog);
            return builder.Run(buildParameters, GetPatchBuildPipeline(), enableLog);
        }

        /// <summary>
        /// 获取默认的构建流程
        /// </summary>
        private List<IBuildTask> GetDefaultBuildPipeline()
        {
            List<IBuildTask> pipeline = new List<IBuildTask>
                {
                    new TaskPrepare_RFBP(),
                    new TaskGetBuildMap_RFBP(),
                    new TaskBuilding_RFBP(),
                    new TaskEncryption_RFBP(),
                    new TaskUpdateBundleInfo_RFBP(),
                    new TaskCreateManifest_RFBP(),
                    new TaskCreatePackage_RFBP(),
                    new TaskCopyResult_RFBP(),
                };
            return pipeline;
        }

        private List<IBuildTask> GetPatchBuildPipeline()
        {
            List<IBuildTask> pipeline = new List<IBuildTask>
                {
                    new TaskPrepare_RFBP(),
                    new TaskGetBuildMap_RFBP(),
                    new TaskBuilding_RFBP(),
                    new TaskEncryption_RFBP(),
                    new TaskUpdateBundleInfo_RFBP(),
                    new TaskCreateManifest_RFBP(),
                   // new TaskCreatePackage_RFBP(),
                    new TaskBuildPatch_RFBP()
                };
            return pipeline;
        }

    }
}
