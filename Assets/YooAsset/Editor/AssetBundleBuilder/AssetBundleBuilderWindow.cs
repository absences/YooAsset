using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace YooAsset.Editor
{
    public class AssetBundleBuilderWindow : EditorWindow
    {
        public static readonly Type[] DockedWindowTypes =
             {
            typeof(AssetBundleBuilderWindow),
        };
        [MenuItem("YooAsset/AssetBundle Builder", false, 102)]
        public static void OpenWindow()
        {
            AssetBundleBuilderWindow window =
                GetWindow<AssetBundleBuilderWindow>("资源包构建工具", true, DockedWindowTypes);
        }
        private Toolbar _toolbar;
        private VisualElement _container;
        private ToolbarMenu _pipelineMenu;
        private EBuildPipeline _buildPipeline;

        public void CreateGUI()
        {
            try
            {
                VisualElement root = this.rootVisualElement;

                // 加载布局文件
                var visualAsset = UxmlLoader.LoadWindowUXML<AssetBundleBuilderWindow>();
                if (visualAsset == null)
                    return;

                visualAsset.CloneTree(root);
                _toolbar = root.Q<Toolbar>("Toolbar");
                _container = root.Q("Container");

                var filed = new Label();
                filed.style.width = 80;
                filed.style.height = 24;
                filed.style.unityTextAlign = TextAnchor.MiddleLeft;
                filed.text = "构建管线";
                _toolbar.Add(filed);
                // 构建管线
                {
                    _pipelineMenu = new ToolbarMenu();
                    _pipelineMenu.style.width = 200;

                    _pipelineMenu.menu.AppendAction(EBuildPipeline.RawFileBuildPipeline.ToString(), PipelineMenuAction, PipelineMenuFun, EBuildPipeline.RawFileBuildPipeline);

                    _toolbar.Add(_pipelineMenu);
                }
                _buildPipeline = EBuildPipeline.RawFileBuildPipeline;

                RefreshBuildPipelineView();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        private void RefreshBuildPipelineView()
        {
            // 清空扩展区域
            _container.Clear();


            _pipelineMenu.text = _buildPipeline.ToString();

            if (_buildPipeline == EBuildPipeline.BuiltinBuildPipeline)
            {
                var viewer = new BuiltinBuildPipelineViewer(_container);
            }
            //else if (_buildPipeline == EBuildPipeline.ScriptableBuildPipeline)
            //{
            //    var viewer = new ScriptableBuildPipelineViewer(_container);
            //}
            else if (_buildPipeline == EBuildPipeline.RawFileBuildPipeline)
            {
                var viewer = new RawfileBuildpipelineViewer(_container);
            }
            else
            {
                throw new System.NotImplementedException(_buildPipeline.ToString());
            }
        }
        private void PipelineMenuAction(DropdownMenuAction action)
        {
            var pipelineType = (EBuildPipeline)action.userData;
            if (_buildPipeline != pipelineType)
            {
                _buildPipeline = pipelineType;
                RefreshBuildPipelineView();
            }
        }
        private DropdownMenuAction.Status PipelineMenuFun(DropdownMenuAction action)
        {
            var pipelineType = (EBuildPipeline)action.userData;
            if (_buildPipeline == pipelineType)
                return DropdownMenuAction.Status.Checked;
            else
                return DropdownMenuAction.Status.Normal;
        }
    }
}