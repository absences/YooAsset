﻿namespace YooAsset.Editor
{
    public class TaskCopyResult_BBP : TaskCopyResult, IBuildTask
    {
        public void Run(BuildContext context)
        {
            var buildParameters = context.GetContextObject<BuildParametersContext>();
            var manifestContext = context.GetContextObject<ManifestContext>();
            CopyResult(buildParameters, manifestContext.Manifest);
        }
    }
}