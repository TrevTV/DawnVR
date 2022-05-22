using AssetsTools.NET;
using AssetsTools.NET.Extra;
using MelonLoader;
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
                else
                {
                    using (Stream manifestResourceStream = Assembly.GetManifestResourceStream("DawnVREnabler.vr-ggm"))
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
                    using (Stream manifestResourceStream = Assembly.GetManifestResourceStream("DawnVREnabler.VRPlugins." + asm))
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

        private void ModifyGGM(string ggmPath, string exportPath)
        {
            AssetsManager am = new AssetsManager();
            AssetsFileInstance afi = am.LoadAssetsFile(ggmPath, false);
            using (Stream stream = Assembly.GetManifestResourceStream("DawnVREnabler.TypeClassPackage.tpk"))
                am.LoadClassPackage(stream);
            am.LoadClassDatabaseFromPackage(afi.file.typeTree.unityVersion);

            AssetFileInfoEx buildSettings = afi.table.GetAssetInfo(11);
            AssetTypeValueField buildBaseField = am.GetTypeInstance(afi.file, buildSettings).GetBaseField();
            AssetTypeValueField enabledVRDevices = buildBaseField.Get("enabledVRDevices").Get("Array");
            AssetTypeTemplateField stringTemplate = enabledVRDevices.templateField.children[1];
            AssetTypeValueField[] vrDevicesList = new AssetTypeValueField[] { StringField("None", stringTemplate), StringField("OpenVR", stringTemplate) };
            enabledVRDevices.SetChildrenList(vrDevicesList);

            byte[] vrAsset;
            using (MemoryStream memStream = new MemoryStream())
            using (AssetsFileWriter writer = new AssetsFileWriter(memStream))
            {
                writer.bigEndian = false;
                buildBaseField.Write(writer);
                vrAsset = memStream.ToArray();
            }
            System.Collections.Generic.List<AssetsReplacer> rep = new System.Collections.Generic.List<AssetsReplacer>();
            rep.Add(new AssetsReplacerFromMemory(0, buildSettings.index, (int)buildSettings.curFileType, 0xFFFF, vrAsset));
            using (MemoryStream memStream = new MemoryStream())
            using (AssetsFileWriter writer = new AssetsFileWriter(memStream))
            {
                afi.file.Write(writer, 0, rep, 0);
                File.WriteAllBytes(exportPath, memStream.ToArray());
            }

            using (AssetsFileWriter writer = new AssetsFileWriter(File.OpenWrite(exportPath)))
            {
                afi.file.Write(writer, 0, rep, 0);
            }

            afi.AssetsStream.Close();

            AssetTypeValueField StringField(string str, AssetTypeTemplateField template)
            {
                return new AssetTypeValueField()
                {
                    children = null,
                    childrenCount = 0,
                    templateField = template,
                    value = new AssetTypeValue(EnumValueTypes.ValueType_String, str)
                };
            }
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