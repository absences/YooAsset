using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace YooAsset.Editor
{
    public class RawfileBuildpipelineViewer : BuildPipelineViewerBase
    {
        public RawfileBuildpipelineViewer(VisualElement parent)
         : base(EBuildPipeline.RawFileBuildPipeline, parent)
        {
            var t = Root.Q<Toggle>("cleanMaterial");
            t.style.display = DisplayStyle.None;
            t.style.visibility = Visibility.Hidden;

            t = Root.Q<Toggle>("containDebugFolder");
            t.style.display = DisplayStyle.None;
            t.style.visibility = Visibility.Hidden;

            var v = Root.Q<TextField>("ClientOutput");
            v.style.display = DisplayStyle.None;
            v.style.visibility = Visibility.Hidden;
        }

        protected override void ExecuteBuild()
        {
            RawFileBuildParameters buildParameters = new RawFileBuildParameters();

            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
           // buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = BuildPipeline.ToString();
            buildParameters.BuildTarget = BuildTarget;
            buildParameters.PackageName = YooAssetSettingsData.Setting.clientName;
            buildParameters.PackageVersion = AssetBundleBuilderSetting.GetPackageVersion("");
            buildParameters.BuildMode = (EBuildMode)_buildModeField.value;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.CopyGameAssetsFrom = "from_client_game_assets";
            buildParameters.complielua = Root.Q<Toggle>("compilelua").value;

            buildParameters.FileNameStyle =
                BuildTarget == UnityEditor.BuildTarget.StandaloneWindows64
                ? EFileNameStyle.BundleName : EFileNameStyle.HashName;
            buildParameters.EncryptionServices = CreateEncryptionInstance();
            // buildParameters.MoveToClientPC = moveToClientPC.value;

            RawFileBuildPipeline pipeline = new RawFileBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {
                //EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
            }
        }

        protected override List<Enum> GetSupportBuildModes()
        {
            List<Enum> buildModeList = new List<Enum>
            {
                EBuildMode.ForceRebuild
            };
            return buildModeList;
        }
    }
}