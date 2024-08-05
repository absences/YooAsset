using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace YooAsset.Editor
{
    public class TaskBuilding_RFBP : IBuildTask
    {
        void IBuildTask.Run(BuildContext context)
        {
            var buildParametersContext = context.GetContextObject<BuildParametersContext>();
            var buildParameters = context.GetContextObject<BuildParametersContext>();
            var buildMapContext = context.GetContextObject<BuildMapContext>();

            var buildMode = buildParameters.Parameters.BuildMode;
            if (buildMode == EBuildMode.ForceRebuild || buildMode == EBuildMode.IncrementalBuild)
            {
                CopyRawBundle(buildMapContext, buildParametersContext);
            }
        }

        /// <summary>
        /// 拷贝原生文件
        /// </summary>
        private void CopyRawBundle(BuildMapContext buildMapContext, BuildParametersContext buildParametersContext)
        {
            string pipelineOutputDirectory = buildParametersContext.GetPipelineOutputDirectory();
            foreach (var bundleInfo in buildMapContext.Collection.Values)
            {
                string dest = $"{pipelineOutputDirectory}/{bundleInfo.BundleName}";
                string from = bundleInfo.BundlePath;
                EditorTools.CopyFile(from, dest, true);
            }
        }
    }
}