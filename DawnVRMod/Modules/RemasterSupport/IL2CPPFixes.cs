using System.Collections;
using UnityEngine.Events;

namespace UnityEngine
{
    internal static class IL2CPPFixes
    {
        private static System.Reflection.MethodInfo UnityEvent_Invoke;

        public static Coroutine RunCoroutine(this MonoBehaviour mb, IEnumerator enumerator)
        {
#if REMASTER
            return (Coroutine)MelonLoader.MelonCoroutines.Start(enumerator);
#else
            return mb.StartCoroutine(enumerator);
#endif
        }

        public static void DoInvoke(this UnityEventBase ue, params object[] parameters)
        {
#if REMASTER
            // todo: what
            var arr = new Il2CppSystem.Object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                arr[i] = (Il2CppSystem.Object)parameters[i];
            ue.Invoke(arr);
#else
            if (UnityEvent_Invoke == null)
                UnityEvent_Invoke = typeof(UnityEvent).GetMethod("Invoke", HarmonyLib.AccessTools.all);
            UnityEvent_Invoke.Invoke(ue, parameters);
#endif
        }
    }
}
