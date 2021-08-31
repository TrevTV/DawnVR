using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DawnVR
{
    internal static class ResourceLoader
    {
        private static Assembly CurrentAsm => Assembly.GetExecutingAssembly();

        public static AssetBundle GetAssetBundle(string name)
        {
            MemoryStream memoryStream;
            using (Stream stream = CurrentAsm.GetManifestResourceStream("DawnVR.Resources." + name))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, buffer.Length);
            }
            return AssetBundle.LoadFromMemory(memoryStream.ToArray());
        }

        public static bool WriteResourceToFile(string path, string name)
        {
            try
            {
                using (Stream manifestResourceStream = CurrentAsm.GetManifestResourceStream("DawnVRMod.Resources." + name))
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Delete))
                    {
                        byte[] buffer = new byte[manifestResourceStream.Length];
                        manifestResourceStream.Read(buffer, 0, buffer.Length);
                        fileStream.Write(buffer, 0, buffer.Length);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public static T LoadAssetWithHF<T>(this AssetBundle b, string name) where T : UnityEngine.Object
        {
            UnityEngine.Object asset = b.LoadAsset<UnityEngine.Object>(name);
            asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return (T)asset;
        }
    }
}
