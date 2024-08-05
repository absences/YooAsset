using System;
using System.Collections.Generic;
using System.IO;

namespace YooAsset.Editor
{
    public class FileOffsetEncryption : IEncryptionServices
    {
        readonly List<string> encryptionBundle = 
            new List<string>() {"scripts_" };
         //脚本
        public EncryptResult Encrypt(EncryptFileInfo fileInfo)
        {
            EncryptResult result = new EncryptResult();
            //atlas/alliance_resource_core.ab
            //aicfg_104.rawfile
           
            for (int i = 0; i < encryptionBundle.Count; i++)
            {
                if (fileInfo.BundleName.StartsWith(encryptionBundle[i]))
                {
                    int offset = 32;
                    byte[] fileData = File.ReadAllBytes(fileInfo.FileLoadPath);
                    var encryptedData = new byte[fileData.Length + offset];
                    Buffer.BlockCopy(fileData, 0, encryptedData, offset, fileData.Length);

                    result.Encrypted = true;
                    result.EncryptedData = encryptedData;
                    return result;
                }
            }

            return result;
        }
    }
}
