using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace YooAsset.Editor
{
    public class BuiltinBuildPipelineViewer : BuildPipelineViewerBase
    {
        public BuiltinBuildPipelineViewer(VisualElement parent)
            : base(EBuildPipeline.BuiltinBuildPipeline, parent)
        {

        }

        protected override void ExecuteBuild()
        {
            BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
            buildParameters.CleanMaterial = cleanMaterial.value;
            buildParameters.BuildDebug = buildDebug.value;
            buildParameters.MoveToClientPC = moveToClientPC.value;

            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            //buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = BuildPipeline.ToString();
            buildParameters.BuildTarget = BuildTarget;
            buildParameters.BuildMode = (EBuildMode)_buildModeField.value;
            buildParameters.PackageName = YooAssetSettingsData.Setting.artName;
            buildParameters.PackageVersion = AssetBundleBuilderSetting.GetPackageVersion("");
            buildParameters.EnableSharePackRule = true;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = BuildTarget == UnityEditor.BuildTarget.StandaloneWindows64 ? EFileNameStyle.BundleName : EFileNameStyle.HashName;
            //buildParameters.BuildinFileCopyOption = buildinFileCopyOption;
            //buildParameters.BuildinFileCopyParams = buildinFileCopyParams;
            buildParameters.EncryptionServices = CreateEncryptionInstance();

            buildParameters.CompressOption = ECompressOption.Uncompressed;

            BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);

            if (buildResult.Success)
            {
                //  EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
            }
        }

        protected override List<Enum> GetSupportBuildModes()
        {
            List<Enum> buildModeList = new List<Enum>
            {
                EBuildMode.ForceRebuild,
                EBuildMode.IncrementalBuild
            };
            return buildModeList;
        }

    }
}
