
namespace YooAsset
{
    public interface IRemoteServices
    {
        /// <summary>
        /// 获取主资源站的资源地址
        /// </summary>
        /// <param name="fileName">请求的文件名称</param>
        string GetRemoteMainURL(string package, string fileName);

    }
}