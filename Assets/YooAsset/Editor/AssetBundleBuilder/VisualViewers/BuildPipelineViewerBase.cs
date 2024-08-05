using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace YooAsset.Editor
{
    public abstract class BuildPipelineViewerBase
    {
        private const int StyleWidth = 400;
        protected readonly EBuildPipeline BuildPipeline;
        protected TemplateContainer Root;
        private TextField _buildOutputField;
        private TextField _clientOutputField;
        private TextField _buildVersionField;
        private EnumField _targetField;
        protected BuildTarget BuildTarget;
        protected PopupField<Enum> _buildModeField;
        private PopupField<Type> _encryptionField;

        public BuildPipelineViewerBase(EBuildPipeline buildPipeline, VisualElement parent)
        {
            BuildPipeline = buildPipeline;
            CreateView(parent);
        }
        protected Toggle cleanMaterial, buildDebug, moveToClientPC;

        private void CreateView(VisualElement parent)
        {
            // 加载布局文件
            var visualAsset = UxmlLoader.LoadWindowUXML<BuildPipelineViewerBase>();
            if (visualAsset == null)
                return;

            Root = visualAsset.CloneTree();
            Root.style.flexGrow = 1f;
            parent.Add(Root);


            // 输出目录
            string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            _buildOutputField = Root.Q<TextField>("BuildOutput");
            _buildOutputField.SetValueWithoutNotify(defaultOutputRoot);
            _buildOutputField.SetEnabled(false);

            // client目录
            string clientOutputRoot = AssetBundleBuilderHelper.GetClientBuildOutputRoot();
            _clientOutputField = Root.Q<TextField>("ClientOutput");
            _clientOutputField.SetValueWithoutNotify(clientOutputRoot);
            _clientOutputField.SetEnabled(false);

            // 构建版本
            _buildVersionField = Root.Q<TextField>("BuildVersion");
            _buildVersionField.style.width = StyleWidth;
            _buildVersionField.SetValueWithoutNotify(AssetBundleBuilderSetting.GetPackageVersion(""));
            _buildVersionField.RegisterValueChangedCallback(evt => {

                AssetBundleBuilderSetting.SetPackageGetPackageVersion("", _buildVersionField.value);
            });
            // 构建模式
            {
                var buildModeContainer = Root.Q("BuildModeContainer");
                var buildMode = AssetBundleBuilderSetting.GetPackageBuildMode(BuildPipeline);
                var buildModeList = GetSupportBuildModes();
                if (!buildModeList.Contains(buildMode))
                {
                    buildMode = (EBuildMode)buildModeList[0];
                }
                int defaultIndex = buildModeList.FindIndex(x => x.Equals(buildMode));

                _buildModeField = new PopupField<Enum>(buildModeList, defaultIndex);
                _buildModeField.label = "Build Mode";
                _buildModeField.style.width = StyleWidth;

                var filed = new Label(":");
                filed.style.width = 200;
                filed.style.height = 24;
                filed.style.unityTextAlign = TextAnchor.MiddleLeft;
                void SetText()
                {
                    if ((EBuildMode)_buildModeField.value == EBuildMode.ForceRebuild)
                    {
                        filed.text = "强制重建模式,清理旧文件";
                    }
                    else
                    {
                        filed.text = "增量构建模式";
                    }
                }

                _buildModeField.RegisterValueChangedCallback(evt =>
                {
                    AssetBundleBuilderSetting.SetPackageBuildMode(BuildPipeline, (EBuildMode)_buildModeField.value);
                    SetText();
                });
                SetText();
                buildModeContainer.Add(_buildModeField);
                buildModeContainer.Add(filed);
            }

            // 加密方法
            {
                var encryptionContainer = Root.Q("EncryptionContainer");
                var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
                if (encryptionClassTypes.Count > 0)
                {
                    var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName(BuildPipeline);

                    int defaultIndex = encryptionClassTypes.FindIndex(x => x.FullName.Equals(encyptionClassName));
                    if (defaultIndex < 0)
                        defaultIndex = 0;
                    _encryptionField = new PopupField<Type>(encryptionClassTypes, defaultIndex);
                    _encryptionField.label = "Encryption";
                    _encryptionField.style.width = StyleWidth;

                    var filed = new Label();
                    filed.style.width = 200;
                    filed.style.height = 24;
                    filed.style.unityTextAlign = TextAnchor.MiddleLeft;
                    void SetText()
                    {
                        if (_encryptionField.value == typeof(FileOffsetEncryption))
                            filed.text = "偏移加密";
                        else
                            filed.text = "无加密";
                    }

                    _encryptionField.RegisterValueChangedCallback(evt =>
                    {
                        SetText();
                        AssetBundleBuilderSetting.SetPackageEncyptionClassName(BuildPipeline, _encryptionField.value.FullName);
                    });
                    SetText();
                    encryptionContainer.Add(_encryptionField);
                    encryptionContainer.Add(filed);
                }
                else
                {
                    _encryptionField = new PopupField<Type>();
                    _encryptionField.label = "Encryption";
                    _encryptionField.style.width = StyleWidth;
                    encryptionContainer.Add(_encryptionField);
                }
            }

            void setToggle(ref Toggle toggleRef, string serializeName)
            {
                toggleRef = Root.Q<Toggle>(serializeName);

                toggleRef.value = AssetBundleBuilderSetting.GetPackageVal(serializeName);

                toggleRef.RegisterValueChangedCallback(val =>
                {
                    AssetBundleBuilderSetting.SetPackageVal(serializeName, val.newValue);
                });
            }
            setToggle(ref cleanMaterial, "cleanMaterial");
            setToggle(ref buildDebug, "containDebugFolder");
            //setToggle(ref moveToClientPC, "moveToClientPC");

            _targetField = Root.Q<EnumField>("BuildTarget");

            BuildTarget = EditorUserBuildSettings.activeBuildTarget;

            _targetField.Init(BuildTarget);//初始化Enum
            _targetField.value = BuildTarget;
            _targetField.RegisterValueChangedCallback(val =>
            {
                BuildTarget = (BuildTarget)val.newValue;
                RefreshWindow();
            });

            // 构建按钮
            var buildButton = Root.Q<Button>("BuildAssets");
            buildButton.text = "Build Assets";
            buildButton.clicked += BuildButton_clicked;

            RefreshWindow();
        }
        private void RefreshWindow()
        {
            if (BuildTarget != BuildTarget.Android && BuildTarget != BuildTarget.iOS && BuildTarget != BuildTarget.StandaloneWindows64)
            {
                BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            }

            _targetField.value = BuildTarget;
        }
        private void BuildButton_clicked()
        {
            var buildMode = _buildModeField.value;
            if (EditorUtility.DisplayDialog("提示", $"通过构建模式【{buildMode}】来构建！", "Yes", "No"))
            {
                EditorTools.ClearUnityConsole();
                EditorApplication.delayCall += ExecuteBuild;
            }
            else
            {
                Debug.LogWarning("[Build] 打包已经取消");
            }
        }


        /// <summary>
        /// 执行构建任务
        /// </summary>
        protected abstract void ExecuteBuild();

        /// <summary>
        /// 获取构建管线支持的构建模式集合
        /// </summary>
        protected abstract List<Enum> GetSupportBuildModes();

        /// <summary>
        /// 创建加密类实例
        /// </summary>
        protected IEncryptionServices CreateEncryptionInstance()
        {
            var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName(BuildPipeline);
            var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
            var classType = encryptionClassTypes.Find(x => x.FullName.Equals(encyptionClassName));
            if (classType != null)
                return (IEncryptionServices)Activator.CreateInstance(classType);
            else
                return null;
        }
    }
}