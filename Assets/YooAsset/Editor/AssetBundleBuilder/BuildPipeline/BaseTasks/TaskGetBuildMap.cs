using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace YooAsset.Editor
{
    public class TaskGetBuildMap
    {
        public BuildMapContext CreateBuildMap(BuildParameters buildParameters)
        {
            BuildMapContext context = new BuildMapContext();
            return context;
        }
    }
}