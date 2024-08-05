using System;
using UnityEngine;
using UnityEditor;

namespace YooAsset.Editor
{
    public static class AssetBundleBuilderSetting
    {
        // EBuildPipeline
        public static EBuildPipeline GetPackageBuildPipeline(string packageName)
        {
            string key = $"{Application.productName}_{packageName}_{nameof(EBuildPipeline)}";
            return (EBuildPipeline)EditorPrefs.GetInt(key, (int)EBuildPipeline.BuiltinBuildPipeline);
        }
        public static void SetPackageBuildPipeline(string packageName, EBuildPipeline buildPipeline)
        {
            string key = $"{Application.productName}_{packageName}_{nameof(EBuildPipeline)}";
            EditorPrefs.SetInt(key, (int)buildPipeline);
        }
        //BuildVersion
        public static string GetPackageVersion(string packageName)
        {
            string key = $"{Application.productName}_{packageName}_BuildVersion";
            return EditorPrefs.GetString(key, "2024");
        }
        public static void SetPackageGetPackageVersion(string packageName, string version)
        {
            string key = $"{Application.productName}_{packageName}_BuildVersion";
            EditorPrefs.SetString(key, version);
        }
        // EBuildMode
        public static EBuildMode GetPackageBuildMode(EBuildPipeline buildPipeline)
        {
            string key = $"{Application.productName}_{buildPipeline}_{nameof(EBuildMode)}";
            return (EBuildMode)EditorPrefs.GetInt(key, (int)EBuildMode.ForceRebuild);
        }
        public static void SetPackageBuildMode(EBuildPipeline buildPipeline, EBuildMode buildMode)
        {
            string key = $"{Application.productName}_{buildPipeline}_{nameof(EBuildMode)}";
            EditorPrefs.SetInt(key, (int)buildMode);
        }

        // EncyptionClassName
        public static string GetPackageEncyptionClassName(EBuildPipeline buildPipeline)
        {
            string key = $"{Application.productName}_{buildPipeline}_EncyptionClassName";
            return EditorPrefs.GetString(key, string.Empty);
        }
        public static void SetPackageEncyptionClassName(EBuildPipeline buildPipeline, string encyptionClassName)
        {
            string key = $"{Application.productName}_{buildPipeline}_EncyptionClassName";
            EditorPrefs.SetString(key, encyptionClassName);
        }

        //bool 

        public static bool GetPackageVal(string name)
        {
            var key = $"{Application.productName}_{name}_bool";
            return EditorPrefs.GetBool(key, false);
        }
        public static void SetPackageVal(string name, bool val)
        {
            string key = $"{Application.productName}_{name}_bool";
            EditorPrefs.SetBool(key, val);
        }
    }
}