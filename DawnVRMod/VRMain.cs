using System.Collections;
using DawnVR.Modules.VR;
using DawnVR.Modules;
using MelonLoader;
using UnityEngine;
using System.Linq;
using Valve.VR;
using System;
using System.Net;

namespace DawnVR
{
    public static class BuildInfo
    {
        public const string Name = "DawnVR";
        public const string Author = "trev (full credits in README)";
        public const string Company = null;
        public const string Version = "0.2.2";
        public const string DownloadLink = null;
    }

    public class VRMain : MelonMod
    {
        public static string CurrentSceneName;

        private const string GithubApiUrl = "https://api.github.com/repos/TrevTV/DawnVR/releases/latest";
        private bool vrEnabled;
        private bool steamInitRunning;

        public override void OnApplicationStart()
        {
            Preferences.Init();

#if REMASTER
            HarmonyInstance.Patch(
                typeof(UnhollowerBaseLib.LogSupport).GetMethod("Warning", HarmonyLib.AccessTools.all),
                typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.UnhollowerWarningPrefix)).ToNewHarmonyMethod());

            foreach (var mb in Assembly.GetTypes().Where(a => a.BaseType == typeof(MonoBehaviour)))
                UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp(mb, false);
#endif

            if (Preferences.CheckForUpdatesOnStart.Value)
                CheckForUpdates();

            if (Environment.GetCommandLineArgs().Contains("OpenVR"))
                vrEnabled = true;
            else
            {
                MelonLogger.Msg("Launch parameter \"-vrmode\" not set to OpenVR, not loading VR patches!");
                if (Preferences.EnableInternalLogging.Value)
                    OutputRedirect.Init(HarmonyInstance);
                //HarmonyPatches.InitNoVR(HarmonyInstance);
                vrEnabled = false;
                return;
            }

            Modules.Resources.Init();
            HarmonyPatches.Init(HarmonyInstance);
            if (Preferences.EnableInternalLogging.Value)
                OutputRedirect.Init(HarmonyInstance);

            // This will always be active in case of hard to recreate errors preventing progress
#if REMASTER
            Application.add_logMessageReceived(new Action<string, string, LogType>(OnUnityLogReceived));
#else
            Application.logMessageReceived += OnUnityLogReceived;
#endif
        }

        private void OnUnityLogReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    if (stackTrace.Contains("Tomlet") || condition.StartsWith("The AssetBundle")) return;
                    MelonLogger.Error("[Unity] " + condition);
                    MelonLogger.Error("[Unity_ST] " + stackTrace);
                    break;
            }
        }

        private void CheckForUpdates()
        {
            MelonLogger.Msg("Checking for updates...");

#if REMASTER
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                var pingReply = ping.Send("github.com");
                if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Headers["User-Agent"] = "DawnVR/" + BuildInfo.Version;
                        string data = client.DownloadString(GithubApiUrl);
                        var node = Newtonsoft.Json.Linq.JObject.Parse(data);
                        Version version = new Version(node["tag_name"].ToString());

                        if (version > new Version(BuildInfo.Version))
                        {
                            MelonLogger.Warning("============================================================");
                            MelonLogger.Warning($"    A new version of DawnVR ({version}) is available!     ");
                            MelonLogger.Warning("Download it here, https://github.com/TrevTV/DawnVR/releases");
                            MelonLogger.Warning("============================================================");
                        }
                        else
                            MelonLogger.Msg("Up to date.");
                    }
                }
                else
                {
                    MelonLogger.Warning("You are not connected to the internet or GitHub is down, skipping update check.");
                }
            }
            catch
            {
                MelonLogger.Warning("Failed to check for updates, skipping.");
            }
#else
            try
            {
                string scriptPath = System.IO.Path.Combine(MelonUtils.UserDataDirectory, "CheckForUpdate.ps1");

                if (!System.IO.File.Exists(scriptPath))
                    ResourceLoader.WriteResourceToFile(scriptPath, "CheckForUpdate.ps1");

                var process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-file \"{scriptPath}\" \"{GithubApiUrl}\"";

                process.Start();
                string returnVal = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (returnVal == "ERR:NO_INTERNET")
                {
                    MelonLogger.Warning("You are not connected to the internet, skipping update check.");
                    return;
                }

                var node = Newtonsoft.Json.Linq.JObject.Parse(returnVal);
                Version version = new Version(node["tag_name"].ToString());

                if (version > new Version(BuildInfo.Version))
                {
                    MelonLogger.Warning("============================================================");
                    MelonLogger.Warning($"    A new version of DawnVR ({version}) is available!     ");
                    MelonLogger.Warning("Download it here, https://github.com/TrevTV/DawnVR/releases");
                    MelonLogger.Warning("============================================================");
                }
                else
                    MelonLogger.Msg("Up to date.");
            }
            catch
            {
                MelonLogger.Warning("Failed to check for updates, skipping.");
            }
#endif
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!vrEnabled) return;

            if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess && !steamInitRunning)
                MelonCoroutines.Start(InitSteamVR());

            VRRig.Instance?.Calibrator?.ResetUI();
            CurrentSceneName = sceneName;
        }

        private IEnumerator InitSteamVR()
        {
            yield return new WaitForSeconds(1f);
            steamInitRunning = true;
            SteamVR.Initialize(false);

            while (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
            {
                if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeFailure)
                {
                    MelonLogger.Error("[SteamVR] Initialization failure! Disabling VR modules.");
                    vrEnabled = false;
                    yield break;
                }
                yield return null;
            }

            steamInitRunning = false;

            CreateCameraRig();
        }

        private void CreateCameraRig()
        {
            GameObject rig = GameObject.Instantiate(Modules.Resources.VRCameraRig);
            if (!VRRig.Instance)
                VRRig.Instance = rig.AddComponent<VRRig>();
        }
    }
}