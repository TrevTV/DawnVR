using System.Collections;
using DawnVR.Modules.VR;
using DawnVR.Modules;
using MelonLoader;
using UnityEngine;
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
        public const string Version = "0.0.1";
        public const string DownloadLink = null;
    }

    public class VRMain : MelonMod
    {
        private bool vrEnabled;
        private bool steamInitRunning;

        public override void OnApplicationStart()
        {
            // todo: add check for any updates on github
            if (Environment.GetCommandLineArgs().Contains("OpenVR"))
                vrEnabled = true;
            else
            {
                MelonLogger.Msg("Launch parameter \"-vrmode\" not set to OpenVR, not loading VR patches!");
                Preferences.Init();
                if (Preferences.EnableInternalLogging)
                    OutputRedirect.Init(HarmonyInstance);
                HarmonyPatches.InitNoVR(HarmonyInstance);
                vrEnabled = false;
                return;
            }

            Modules.Resources.Init();
            Preferences.Init();
            HarmonyPatches.Init(HarmonyInstance);
            if (Preferences.EnableInternalLogging)
                OutputRedirect.Init(HarmonyInstance);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                MelonLogger.Msg("Is Freeroam? (true) " + (T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam));
                MelonLogger.Msg("Is Debug? (false) " + T_A6E913D1.Instance.m_gameModeManager.isDebug);
                MelonLogger.Msg("Is That Thing? (true) " + (!T_408CFC35.s_inHotspotRange || T_A6E913D1.Instance.m_inputManager.GetInputState(eGameInput.kInteract, true, true, true) == eInputState.kNone || T_A6E913D1.Instance.m_inputManager.GetInputState(eGameInput.kInteract, true, true, true) == eInputState.kUp));
                MelonLogger.Msg("Is FollowCam Null? (true) " + (T_A6E913D1.Instance.m_followCamera != null));
                MelonLogger.Msg("Is Interaction Blocked? (false) " + T_A6E913D1.Instance.m_followCamera.m_isInteractionBlocked);
                MelonLogger.Msg("Is GenericBlock? (false) " + (T_A6E913D1.Instance.m_gameModeManager.genericMovementBlock));
                MelonLogger.Msg("Is Freelook? (false) " + (T_A6E913D1.Instance.m_followCamera.m_isFreelook));
                MelonLogger.Msg("Is Paused? (false) " + (T_A6E913D1.Instance.Paused));
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!vrEnabled) return;

            if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess && !steamInitRunning)
                MelonCoroutines.Start(InitSteamVR());
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