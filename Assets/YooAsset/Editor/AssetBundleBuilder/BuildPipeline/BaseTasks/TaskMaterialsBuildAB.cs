using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

namespace YooAsset.Editor
{
    public enum AssetConfigEnum
    {
        Not_AB_Name,//打包不自动设置ab名  true
        Info,//其他信息
        AndroidDoNotModifyTexture//true
    }
    public class TaskMaterialsBuildAB
    {
        protected void ModifyUIAtlasFormat(BuildTarget target)
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                string[] filePaths = Directory.GetFiles("Assets/Res/UI/atlas(need split rgb & alpha)", "*.spriteatlas");
                foreach (string filePath in filePaths)
                {
                    SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);

                    if (atlas != null)
                    {
                        TextureImporterPlatformSettings settings = atlas.GetPlatformSettings(GetCurrentPlatformName());
                        TextureImporterFormat format = GetCurrentPlatforFormat(target);
                        if (settings.overridden)
                        {
                            if (settings.format != format)
                            {
                                settings.format = format;
                                atlas.SetPlatformSettings(settings);
                                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
                            }
                        }
                        else
                        {
                            settings.overridden = true;
                            settings.format = format;
                            atlas.SetPlatformSettings(settings);
                            SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
                        }
                    }
                }
                ModifyUISpritesFormat(target);
            }
        }
        static void ModifyUISpritesFormat(BuildTarget target)
        {
            List<string> imagePaths = Directory.GetFiles("Assets/Res/UI/", "*.*", SearchOption.AllDirectories).Where(file => file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".tga")).ToList();
            foreach (string imagePath in imagePaths)
            {
                if (imagePath.Contains("ui_effect"))
                    continue;
                TextureImporter importer = TextureImporter.GetAtPath(imagePath) as TextureImporter;
                if (importer != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imagePath);
                    if (IsAndroidDoNotModifyTextureFormat(importer))
                        continue;

                    TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(GetCurrentPlatformName());
                    TextureImporterFormat format = GetCurrentPlatforFormat(target);
                    if (settings.overridden)
                    {
                        if (settings.format != format)
                        {
                            settings.format = format;
                            importer.SetPlatformTextureSettings(settings);
                            importer.SaveAndReimport();
                        }
                    }
                    else
                    {
                        settings.overridden = true;
                        settings.format = format;
                        importer.SetPlatformTextureSettings(settings);
                        importer.SaveAndReimport();
                    }
                }
            }
        }
        //static NoCheckTextureFormatConfig config;
        public static bool IsAndroidDoNotModifyTextureFormat(TextureImporter importer)
        {
            bool AndroidDoNotModify = false;
            if (importer != null)
            {
                var user = importer.userData;

                if (!string.IsNullOrEmpty(user))
                {
                    var str = user.Split('|');
                    if (str.Length > 0)
                    {
                        for (int k = 0; k < str.Length; k++)
                        {
                            if (!string.IsNullOrEmpty(str[k]))
                            {
                                var sps = str[k].Split(':');
                                if (sps[0] == AssetConfigEnum.AndroidDoNotModifyTexture.ToString())
                                {
                                    AndroidDoNotModify = bool.Parse(sps[1]);
                                }
                            }
                        }
                    }
                }
            }

            return AndroidDoNotModify;
        }
        protected void ModifyEffectTextureFormat()
        {
            float progress = 0.0f;
            string[] texturePaths = { "Assets/Res/Effect/Textures", "Assets/Res/UI/ui_effect/textures" };
            for (int i = 0; i < texturePaths.Length; i++)
            {
                int count = 0;
                List<string> filePaths =
                    Directory.GetFiles(texturePaths[i], "*.*", SearchOption.AllDirectories).Where(file => file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".tga")).ToList();
                foreach (string filePath in filePaths)
                {
                    ++count;
                    progress = count / (float)filePaths.Count;
                    EditorUtility.DisplayProgressBar("check", "检测特效贴图格式中[" + Path.GetFileName(filePath) + "]......", progress);

                    TextureImporter importer = TextureImporter.GetAtPath(filePath) as TextureImporter;
                    if (IsAndroidDoNotModifyTextureFormat(importer))
                    {
                        continue;
                    }

                    TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(GetCurrentPlatformName());
                    TextureImporterFormat format = GetCurrentPlatformEffectTextureFormat(importer.DoesSourceTextureHaveAlpha());
                    if (settings.overridden)
                    {
                        if (settings.format != format)
                        {
                            settings.format = format;
                            importer.SetPlatformTextureSettings(settings);
                            importer.SaveAndReimport();
                        }
                    }
                    else
                    {
                        settings.overridden = true;
                        settings.format = format;
                        importer.SetPlatformTextureSettings(settings);
                        importer.SaveAndReimport();
                    }
                }
                EditorUtility.ClearProgressBar();
            }
        }
        public static TextureImporterFormat GetCurrentPlatformEffectTextureFormat(bool alpha, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            BuildTarget target = buildTarget == BuildTarget.NoTarget ? EditorUserBuildSettings.activeBuildTarget : buildTarget;
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSX:
                    if (alpha)
                    {
                        return TextureImporterFormat.DXT5;
                    }
                    return TextureImporterFormat.DXT1;
                case BuildTarget.Android:

                    if (alpha)
                    {
                        return TextureImporterFormat.ETC2_RGBA8;
                    }
                    return TextureImporterFormat.ETC2_RGB4;
                case BuildTarget.iOS:
                    if (alpha)
                    {
                        return TextureImporterFormat.ASTC_RGB_4x4;
                    }
                    return TextureImporterFormat.ASTC_RGB_6x6;
            }
            return TextureImporterFormat.RGBA32;
        }
        public static TextureImporterFormat GetCurrentPlatforFormat(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSX:
                    return TextureImporterFormat.RGBA32;
                case BuildTarget.Android:
                    return TextureImporterFormat.ETC2_RGBA8;
                case BuildTarget.iOS:
                    return TextureImporterFormat.ASTC_RGB_4x4;
            }
            return TextureImporterFormat.RGBA32;
        }
        static string GetCurrentPlatformName()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSX:
                    return "Standalone";
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iPhone";
            }
            return "Standalone";
        }
        protected void EffectMaterialsBuildAB()
        {
            string[] matPaths = { "Assets/Res/Effect/Material", "Assets/Res/UI/ui_effect/materials" };
            string[] modelPaths = { "Assets/Res/Effect/Model", "Assets/Res/UI/ui_effect/model" };
            string[] prefabPaths = { "Assets/Res/Effect/Prefab", "Assets/Res/UI/ui_effect/prefabs" };
            const string abName = "effect/materials.ab";
            //const string modelAbName = "effect/model/{0}.ab";
            const string modleABName = "effect/models.ab";
            // EffectFbxNotSetAbNameCfg cfg = AssetDatabase.LoadAssetAtPath<EffectFbxNotSetAbNameCfg>("Assets/Configs/EffectFbxNotSetAbNameCfg.asset");

            for (int i = 0; i < matPaths.Length; i++)
            {
                string[] matPath = Directory.GetFiles(matPaths[i], "*.mat", SearchOption.AllDirectories);
                foreach (string path in matPath)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        importer.assetBundleName = abName;
                    }
                }
            }
            for (int i = 0; i < modelPaths.Length; i++)
            {

                string[] filePaths = Directory.GetFiles(modelPaths[i], "*.fbx", SearchOption.AllDirectories);

                foreach (var filePath in filePaths)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(filePath);
                    if (importer != null)
                    {
                        var user = importer.userData;
                        bool Not_AB_Name = false;
                        if (!string.IsNullOrEmpty(user))
                        {
                            var str = user.Split('|');
                            if (str.Length > 0)
                            {
                                for (int k = 0; k < str.Length; k++)
                                {
                                    if (!string.IsNullOrEmpty(str[k]))
                                    {
                                        var sps = str[k].Split(':');
                                        if (sps[0] == AssetConfigEnum.Not_AB_Name.ToString())
                                        {
                                            Not_AB_Name = bool.Parse(sps[1]);
                                        }
                                    }
                                }
                            }
                        }

                        if (!Not_AB_Name)
                            importer.assetBundleName = modleABName;
                    }
                }
            }

            string prefabAbName = "effect/{0}.ab";
            for (int i = 0; i < prefabPaths.Length; i++)
            {
                string[] paths = Directory.GetFiles(prefabPaths[i], "*.prefab", SearchOption.AllDirectories);
                foreach (string pp in paths)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(pp);
                    if (importer != null)
                    {
                        string name = Path.GetFileNameWithoutExtension(pp);
                        importer.assetBundleName = string.Format(prefabAbName, name);
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}