using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YooAsset.Editor
{
    [CreateAssetMenu(fileName = "RawFileCollectConfig", menuName = "YooAsset/Create RawFile Collect Config")]
    public class RawFileCollectConfig : ScriptableObject
    {
        public RawFileCollectItem[] Cfgs;

     
    }

    //目录名称与是否保持原后缀
    [System.Serializable]
    public class RawFileCollectItem
    {
        public string pathName;

        public bool originalExtension;
    }
}