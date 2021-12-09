using UnityEngine;
using DawnVR.Modules.VR;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static void SetReminderTexture(T_81803C2C __instance)
        {
            __instance.SetAlpha(1);
            VRRig.Instance.ActiveHandRenderers[0].transform.parent.Find("handpad").GetComponent<MeshRenderer>().sharedMaterial = __instance.m_reminderRenderer.material;
        }
    }
}
