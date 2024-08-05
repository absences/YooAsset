namespace YooAsset.Editor
{
    public class TaskCleanMat_BBP : TaskCleanMat, IBuildTask
    {
        public void Run(BuildContext context)
        {
            var buildParameters = context.GetContextObject<BuildParametersContext>();

            var val = buildParameters.Parameters.CleanMaterial;

            if (val)
            {
                CleanMaterialsCache();
            }
        }
    }
}