using System.Collections;
using DawnVR.Events;

namespace UnityEngine
{
    internal static class IL2CPPFixes
    {
        public static Coroutine RunCoroutine(this MonoBehaviour mb, IEnumerator enumerator)
        {
#if REMASTER
            return (Coroutine)MelonLoader.MelonCoroutines.Start(enumerator);
#else
            return mb.StartCoroutine(enumerator);
#endif
        }
    }
}
