using AssetsTools.NET;
using AssetsTools.NET.Extra;
using MelonLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DawnVREnabler
{
    public static class BuildInfo
    {
        public const string Name = "DawnVREnabler";
        public const string Author = "raicuparta, digitalzombie, trev";
        public const string Company = null;
        public const string Version = "0.1.0";
        public const string DownloadLink = null;
    }

    public class VREnabler : MelonPlugin
    {
        internal string PluginsPath
        {
            get
            {
                string path = Path.Combine(MelonUtils.GetGameDataDirectory(), "Plugins");
                if (MelonUtils.IsGameIl2Cpp())
                    path = Path.Combine(path, "x86_64");
                return path;
            }
        }

        internal string[] VRAsms = new string[]
            {
                "openvr_api.dll"
            };

        public override void OnPreInitialization()
        {
            string ggmLocation = Path.Combine(MelonUtils.GetGameDataDirectory(), "globalgamemanagers");
            bool backupExists = CreateGameManagersBackup(ggmLocation);
            if (!backupExists)
            {
                if (MelonUtils.IsGameIl2Cpp())
                {
                    ModifyGGM(ggmLocation + ".bak", ggmLocation);
                    MelonLogger.Msg("Successfully modified GGM!");
                }
                else if (MelonLoader.InternalUtils.UnityInformationHandler.GameName == "Life is Strange: Before the Storm")
                {
                    using (Stream manifestResourceStream = MelonAssembly.Assembly.GetManifestResourceStream("DawnVREnabler.vr-ggm"))
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
            }

            MelonLogger.Msg("Checking for VR plugins...");
            string[] filePaths = Directory.GetFiles(PluginsPath, "*.dll");

            bool copiedPlugins = false;
            foreach (string asm in VRAsms)
            {
                if (!filePaths.Any((a) => a.Contains(asm)))
                {
                    copiedPlugins = true;
                    using (Stream manifestResourceStream = MelonAssembly.Assembly.GetManifestResourceStream("DawnVREnabler.VRPlugins." + asm))
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

        // thanks nes
        private void ModifyGGM(string ggmPath, string exportPath)
        {
            AssetsManager am = new AssetsManager();
            AssetsFileInstance afi = am.LoadAssetsFile(ggmPath, false);
            using (Stream stream = MelonAssembly.Assembly.GetManifestResourceStream("DawnVREnabler.TypeClassPackage.tpk"))
                am.LoadClassPackage(stream);

            am.LoadClassDatabaseFromPackage(afi.file.Metadata.UnityVersion);

            AssetFileInfo buildSettings = afi.file.GetAssetsOfType(AssetClassID.BuildSettings)[0];
            AssetTypeValueField buildBaseField = am.GetBaseField(afi, buildSettings);
            AssetTypeValueField enabledVRDevices = buildBaseField.Get("enabledVRDevices.Array");

            var noneField = ValueBuilder.DefaultValueFieldFromArrayTemplate(enabledVRDevices);
            noneField.AsString = "None";
            enabledVRDevices.Children.Add(noneField);

            var openVrField = ValueBuilder.DefaultValueFieldFromArrayTemplate(enabledVRDevices);
            openVrField.AsString = "OpenVR";
            enabledVRDevices.Children.Add(openVrField);

            List<AssetsReplacer> reps = new List<AssetsReplacer>
                {
                    new AssetsReplacerFromMemory(afi.file, buildSettings, buildBaseField)
                };

            using (MemoryStream memStream = new MemoryStream())
            using (AssetsFileWriter writer = new AssetsFileWriter(memStream))
            {
                afi.file.Write(writer, 0, reps);
                File.WriteAllBytes(exportPath, memStream.ToArray());
            }

            am.UnloadAllAssetsFiles();
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
            File.Move(path, backupPath);
            MelonLogger.Msg($"Backup created!");
            return false;
        }
    }
}
