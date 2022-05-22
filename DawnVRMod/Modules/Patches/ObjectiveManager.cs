using UnityEngine;
using DawnVR.Modules.VR;
#if !REMASTER
using ChloeReminderDisplay = T_81803C2C;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static void SetReminderTexture(ChloeReminderDisplay __instance)
        {
            __instance.SetAlpha(1);
            VRRig.Instance.ActiveHandRenderers[0].transform.parent.Find("handpad").GetComponent<MeshRenderer>().sharedMaterial = __instance.m_reminderRenderer.material;
        }
    }
}
