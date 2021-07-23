using MelonLoader;
using System.IO;
using System.Linq;
using System.Reflection;

namespace VREnabler
{
    public static class BuildInfo
    {
        public const string Name = "VREnabler";
        public const string Author = "mrpurple & trev";
        public const string Company = null;
        public const string Version = "0.1.0";
        public const string DownloadLink = null;
    }

    public class VREnabler : MelonPlugin
    {
        internal string CurrentAsmPath => Path.GetDirectoryName(Location);
		internal string ManagedPath => MelonUtils.GetManagedDirectory();
		internal string PluginsPath => Path.Combine(ManagedPath, "../Plugins");

        internal string[] vrAsms = new string[]
            {
                "AudioPluginOculusSpatializer.dll",
                "openvr_api.dll",
                "OVRGamepad.dll",
                "OVRPlugin.dll"
            };

        public override void OnPreInitialization()
        {
            string ggmLocation = Path.Combine(MelonUtils.GetGameDataDirectory(), "globalgamemanagers");
            bool backupExists = CreateGameManagersBackup(ggmLocation);
            if (!backupExists)
            {
                using (Stream manifestResourceStream = Assembly.GetManifestResourceStream("VREnabler.vr-ggm"))
                {
                    using (FileStream fileStream = new FileStream(ggmLocation, FileMode.Create, FileAccess.Write, FileShare.Delete))
                    {
                        MelonLogger.Msg("Copying VR patch...");
                        byte[] buffer = new byte[manifestResourceStream.Length];
                        manifestResourceStream.Read(buffer, 0, buffer.Length);
                        fileStream.Write(buffer, 0, buffer.Length);
                    }
                }
                MelonLogger.Msg("Successfully copied VR patch!");
            }

            MelonLogger.Msg("Checking for VR plugins...");
            string[] filePaths = Directory.GetFiles(PluginsPath, "*.dll");

            bool copiedPlugins = false;

            foreach (string asm in vrAsms)
            {
                if (!filePaths.Any((a) => a.Contains(asm)))
                {
                    copiedPlugins = true;
                    using (Stream manifestResourceStream = Assembly.GetManifestResourceStream("VREnabler.VRPlugins." + asm))
                    {
                        using (FileStream fileStream = new FileStream(Path.Combine(PluginsPath, asm), FileMode.Create, FileAccess.Write, FileShare.Delete))
                        {
                            MelonLogger.Msg("Copying " + asm);
                            byte[] buffer = new byte[manifestResourceStream.Length];
                            manifestResourceStream.Read(buffer, 0, buffer.Length);
                            fileStream.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }

            if (copiedPlugins)
                MelonLogger.Msg("Successfully copied VR plugins!");
            else
                MelonLogger.Msg("VR plugins already present");
        }

        private bool CreateGameManagersBackup(string path)
        {
            MelonLogger.Msg($"Backing up GGM...");
            var backupPath = path + ".bak";
            if (File.Exists(backupPath))
            {
                MelonLogger.Msg($"Backup already exists.");
                return true;
            }
            File.Copy(path, backupPath);
            MelonLogger.Msg($"Backup created!");
            return false;
        }
    }
}