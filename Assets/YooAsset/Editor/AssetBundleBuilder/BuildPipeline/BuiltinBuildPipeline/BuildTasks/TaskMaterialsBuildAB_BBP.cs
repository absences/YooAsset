using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YooAsset.Editor
{
    public class TaskMaterialsBuildAB_BBP : TaskMaterialsBuildAB, IBuildTask
    {
        public void Run(BuildContext context)
        {
            EffectMaterialsBuildAB();

            var buildParameters = context.GetContextObject<BuildParametersContext>();
            var target = buildParameters.Parameters.BuildTarget;

            ModifyUIAtlasFormat(target);
            ModifyEffectTextureFormat();
        }
    }
}