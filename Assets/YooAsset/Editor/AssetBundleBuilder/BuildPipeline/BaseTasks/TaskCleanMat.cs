using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    public class TaskCleanMat
    {
        public void CleanMaterialsCache()
        {
            string path = "Assets/Res";
            string[] filesPath = Directory.GetFiles(path, "*.mat", SearchOption.AllDirectories);
            for (int j = 0; j < filesPath.Length; ++j)
            {
                bool dirty = false;
                float progress = j / (float)filesPath.Length;
                string filePath = filesPath[j];
                string relativePath = "Assets" + Path.GetFullPath(filePath).Replace(Path.GetFullPath(Application.dataPath), "").Replace("\\", "/");
                EditorUtility.DisplayProgressBar("check", "检测特效贴图格式中[" + Path.GetFileName(relativePath) + "]......", progress);
                Material m = AssetDatabase.LoadAssetAtPath<Material>(relativePath);
                if (m == null)
                {
                    Debug.LogError("材质[" + relativePath + "]的为空!!!");
                    continue;
                }
                if (m.shader == null)
                {
                    Debug.LogError("材质[" + m.name + "]的shader为空!!!", m);
                    continue;
                }
                if (GetShaderKeywords(m.shader, out var global, out var local))
                {
                    HashSet<string> keywords = new HashSet<string>();
                    foreach (var g in global)
                    {
                        keywords.Add(g);
                    }
                    foreach (var l in local)
                    {
                        keywords.Add(l);
                    }
                    List<string> resetKeywords = new List<string>(m.shaderKeywords);
                    foreach (var item in m.shaderKeywords)
                    {
                        if (!keywords.Contains(item))
                        {
                            resetKeywords.Remove(item);
                            dirty = true;
                        }
                    }
                    m.shaderKeywords = resetKeywords.ToArray();
                }
                HashSet<string> property = new HashSet<string>();
                int count = m.shader.GetPropertyCount();
                for (int i = 0; i < count; ++i)
                {
                    property.Add(m.shader.GetPropertyName(i));
                }

                SerializedObject o = new SerializedObject(m);
                SerializedProperty disabledShaderPasses = o.FindProperty("disabledShaderPasses");
                SerializedProperty SavedProperties = o.FindProperty("m_SavedProperties");
                SerializedProperty TexEnvs = SavedProperties.FindPropertyRelative("m_TexEnvs");
                SerializedProperty Floats = SavedProperties.FindPropertyRelative("m_Floats");
                SerializedProperty Colors = SavedProperties.FindPropertyRelative("m_Colors");
                for (int i = disabledShaderPasses.arraySize - 1; i >= 0; --i)
                {
                    if (!property.Contains(disabledShaderPasses.GetArrayElementAtIndex(i).displayName))
                    {
                        disabledShaderPasses.DeleteArrayElementAtIndex(i);
                        dirty = true;
                    }
                }
                for (int i = TexEnvs.arraySize - 1; i >= 0; --i)
                {
                    if (!property.Contains(TexEnvs.GetArrayElementAtIndex(i).displayName))
                    {
                        TexEnvs.DeleteArrayElementAtIndex(i);
                        dirty = true;
                    }
                }
                for (int i = Floats.arraySize - 1; i >= 0; --i)
                {
                    if (!property.Contains(Floats.GetArrayElementAtIndex(i).displayName))
                    {
                        Floats.DeleteArrayElementAtIndex(i);
                        dirty = true;
                    }
                }
                for (int i = Colors.arraySize - 1; i >= 0; --i)
                {
                    if (!property.Contains(Colors.GetArrayElementAtIndex(i).displayName))
                    {
                        Colors.DeleteArrayElementAtIndex(i);
                        dirty = true;
                    }
                }
                if (dirty)
                {
                    o.ApplyModifiedProperties();
                    EditorUtility.SetDirty(m);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static bool GetShaderKeywords(Shader target, out string[] global, out string[] local)
        {
            try
            {
                MethodInfo globalKeywords = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                global = (string[])globalKeywords.Invoke(null, new object[] { target });
                MethodInfo localKeywords = typeof(ShaderUtil).GetMethod("GetShaderLocalKeywords", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                local = (string[])localKeywords.Invoke(null, new object[] { target });
                return true;
            }
            catch
            {
                global = local = null;
                return false;
            }
        }
    }
}