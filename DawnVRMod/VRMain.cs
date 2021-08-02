using System.Collections;
using DawnVR.Modules.VR;
using DawnVR.Modules;
using UnityEngine;
using MelonLoader;
using System.Linq;
using Valve.VR;
using System;

namespace DawnVR
{
    public static class BuildInfo
    {
        public const string Name = "DawnVR";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "0.1.0";
        public const string DownloadLink = null;
    }

    public class VRMain : MelonMod
    {
        private bool vrEnabled;
        private bool steamInitRunning;

        public override void OnApplicationStart()
        {
            if (Environment.GetCommandLineArgs().Contains("OpenVR"))
                vrEnabled = true;
            else
            {
                MelonLogger.Msg("Launch parameter \"-vrmode\" not set to OpenVR, not loading VR patches!");
                HarmonyPatches.InitNoVR(HarmonyInstance);
                vrEnabled = false;          
                return;
            }

            Modules.Resources.Init();
            HarmonyPatches.Init(HarmonyInstance);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!vrEnabled) return;

            if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess && !steamInitRunning)
                MelonCoroutines.Start(InitSteamVR());
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                MelonLogger.Msg(" ----- BEGINNING CAMERA STUFF ----- ");
                foreach (Camera cam in GameObject.FindObjectsOfType<Camera>())
                {
                    string path = cam.name;
                    Transform parent = cam.transform.parent;
                    while (parent != null)
                    {
                        path = parent.name + "/" + path;
                        parent = parent.parent;
                    }
                    path = "/" + path;
                    MelonLogger.Msg(path);
                }
            }

            else if (Input.GetKeyDown(KeyCode.K))
            {
                MelonLogger.Msg("Current Mode: " + T_A6E913D1.Instance.m_gameModeManager.CurrentMode);
            }
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