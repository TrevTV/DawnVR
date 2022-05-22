using System;
using MelonLoader;
using System.Reflection;
#if REMASTER
using CommonTools;
#else
using _1A25B0A83;
using Output = _1A25B0A83.T_EF48DC26;
#endif

namespace DawnVR.Modules
{
    internal static class OutputRedirect
    {
        private static HarmonyLib.Harmony HarmonyInstance;

        private static void PatchPost(MethodInfo original, string postfixMethodName)
            => HarmonyInstance.Patch(original, null, typeof(OutputRedirect).GetMethod(postfixMethodName).ToNewHarmonyMethod());

        public static void Init(HarmonyLib.Harmony hInstance)
        {
			MelonLogger.Msg("Enabled internal output redirect.");

            HarmonyInstance = hInstance;
            PatchPost(typeof(Output).GetMethod("ErrorMsg"), nameof(ErrorMsg));
            PatchPost(typeof(Output).GetMethod("GreenMsg"), nameof(GreenMsg));
            PatchPost(typeof(Output).GetMethod("WarningMsg"), nameof(WarningMsg));
            PatchPost(typeof(Output).GetMethod("Msg"), nameof(Msg));
            PatchPost(typeof(Output).GetMethod("BailMsg"), nameof(BailMsg));
        }

        public static void ErrorMsg(string _12C7DF5B8) => MelonLogger.Error("[INTERNAL] " + _12C7DF5B8);

        public static void GreenMsg(string _12C7DF5B8) => MelonLogger.Msg(ConsoleColor.Green, "[INTERNAL] " + _12C7DF5B8);

        public static void WarningMsg(string _12C7DF5B8) => MelonLogger.Warning("[INTERNAL] " + _12C7DF5B8);

		public static void Msg(string _12C7DF5B8) => MelonLogger.Msg("[INTERNAL] " + _12C7DF5B8);

		public static void BailMsg(string _12C7DF5B8) => MelonLogger.Error("[INTERNAL] BAIL: " + _12C7DF5B8);
	}
}
