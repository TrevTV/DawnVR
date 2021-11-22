using UnityEngine;
using static MelonLoader.MelonLogger;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
		public static void UpdateHighlightedItem(T_9806DB88 __instance)
        {
			if (!__instance._1E858F24B.m_bIsAtMirror)
			{			
				int newItemIndex = -1;
				int previousItemIndex = __instance._12A4ACD3B;
				if (VR.VRRig.Instance.Input.GetThumbstickLeft(VR.VRInput.Hand.Left).stateDown)
                {
					if (previousItemIndex - 1 < 0)
						newItemIndex = __instance._114310DDF.Count - 1;
					else
						newItemIndex = __instance._12A4ACD3B - 1;
				}
				if (VR.VRRig.Instance.Input.GetThumbstickRight(VR.VRInput.Hand.Left).stateDown)
                {
					if (previousItemIndex + 1 > __instance._114310DDF.Count - 1)
						newItemIndex = 0;
					else
						newItemIndex = __instance._12A4ACD3B + 1;
				}

				if (newItemIndex > -1 && newItemIndex != __instance._12A4ACD3B)
				{
					__instance._12A4ACD3B = newItemIndex;
					__instance._116AB0FE7(previousItemIndex);
					if (__instance._14D5ED31B)
					{
						__instance.m_hoverEffect.Chosen();
					}
				}
			}
			else
			{
				__instance._19644D4FC();
			}
			__instance._14D5ED31B = false;

			Transform ui = __instance.m_hoverEffect.transform;
            ui.rotation = Quaternion.Euler(0f, 240f, 0f);
            ui.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
    }
}
