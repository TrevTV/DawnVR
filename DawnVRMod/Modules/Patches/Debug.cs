#if REMASTER
using PrototyperData;
using DataEditor;
#else
using GraphManager = T_BF5A5EEC;
using Sequence = _15C6DD6D9.T_58A5E6E2;
using Timeline = T_156BDACC;
using TimelineManager = T_14474339;
using CriAudioManager = T_E8819104;
using SceneObject = _169E4A3E.T_4B84CB26;
using GameMaster = T_A6E913D1;
using SplashScreenWindow = T_EDB11480;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool CutsceneSkipPressed(GraphManager __instance)
        {
            Sequence currentMode = __instance.GetCurrentMode<Sequence>();
            if (currentMode != null)
            {
                Timeline timeline = TimelineManager.GetTimeline(currentMode);
                if (timeline != null)
                {
                    float sequenceEndTime = currentMode.sequenceEndTime;
                    float timeS = sequenceEndTime - timeline.CurrentTime;
                    CriAudioManager.Singleton.AdvanceAllSounds(timeS);
                    timeline.SetTime(sequenceEndTime);
                    SceneObject.s_forceFullEvaluate = true;
                    TimelineManager.UpdateCurrentTimelinesForFrame();
                }
            }

            if (GameMaster.Instance.m_rumbleManager != null)
                GameMaster.Instance.m_rumbleManager.ClearAllRumbles(0f);

            return false;
        }

        public static void DisableSplashScreen(SplashScreenWindow __instance) => __instance.m_splashList.Clear();
    }
}
