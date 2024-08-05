using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace YooAsset.Editor
{
    public static class AssetBundleBuilderHelper
    {
        /// <summary>
        /// 获取默认的输出根目录
        /// </summary>
        public static string GetDefaultBuildOutputRoot()
        {
            string projectPath = EditorTools.GetProjectPath();
            return $"{projectPath}/Bundles";
        }

        /// <summary>
        /// 获取流文件夹路径
        /// </summary>
        public static string GetStreamingAssetsRoot()
        {
            return $"{Application.dataPath}/StreamingAssets/";
        }

        /// <summary>
        /// 获取client临时输出根路录
        /// </summary>
        public static string GetClientBuildOutputRoot()
        {
            string projectPath = EditorTools.GetProjectPath();
            return $"{projectPath}/../../client/project_s_client/Bundles";
        }

        /// <summary>
        /// 获取client手动构建输出根路录
        /// </summary>
        public static string GetManualClientBuildOutputRoot()
        {
            string projectPath = EditorTools.GetProjectPath();
            return $"{projectPath}/game_assets";
        }
        /// <summary>
        /// 获取assets输出根路录
        /// </summary>
        public static string GetAssetsOutputRoot()
        {
            string projectPath = EditorTools.GetProjectPath();
            return $"{projectPath}/game_assets";
        }
    }
}