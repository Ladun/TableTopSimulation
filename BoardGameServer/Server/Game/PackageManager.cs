using System.IO;

using ServerCore;

namespace Server
{
    public class PackageManager : Singleton<PackageManager>
    {
        private string packageDir;
        private const string packageDirName = "CustomPackages";

        private object _lock = new object();

        public PackageManager()
        {
            packageDir = Path.Combine(Directory.GetCurrentDirectory(), packageDirName);
            if (!Directory.Exists(packageDir))
            {
                Directory.CreateDirectory(packageDir);
            }
        }

        public bool HasFile(string fileName)
        {
            lock (_lock)
            {
                return File.Exists(GetPath(fileName));
            }
        }

        public byte[] GetFileByte(string fileName)
        {
            lock (_lock)
            {
                byte[] bytes = File.ReadAllBytes(GetPath(fileName));
                return bytes;
            }
        }

        public void SaveFile(string fileName, byte[] fileBytes)
        {
            lock (_lock)
            {
                File.WriteAllBytes(GetPath(fileName), fileBytes);
            }
        }


        public string GetPath(string fileName)
        {
            return Path.Combine(packageDir, fileName);
        }
    }
}
