using System;
using System.Collections;
using System.Reflection;

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

        // this is techinically a mono fix but it was caused by a il2cpp fix
#if !REMASTER
        public static MethodInfo GetMethodInfo(this Delegate del)
        {
            if (del == null) throw new ArgumentNullException("del");

            return del.Method;
        }   
#endif
    }
}
