using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DawnVR.Modules
{
    internal static class UpdateHandler
    {
        public static IntPtr updateLibrary;
        public static bool hasLoadedLib;

        #region Kernel Externs

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("Kernel32.dll")]
        private static extern UInt32 GetLastError();

        #endregion

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool ConnectedToInternet();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string LatestDawnRelease(string gitUrl);

        public static ConnectedToInternet IsConnectedToInternet;
        public static LatestDawnRelease GetLatestDawnRelease;

        public static void Setup()
        {
            if (!hasLoadedLib)
            {
                string dllPath = Path.Combine(MelonLoader.MelonUtils.UserDataDirectory, "DawnUpdateChecker.dll");

                if (!File.Exists(dllPath))
                    ResourceLoader.WriteResourceToFile(dllPath, "DawnUpdateChecker.dll");

                updateLibrary = LoadLibrary(dllPath);
                IsConnectedToInternet = GetFunction<ConnectedToInternet>("ConnectedToInternet", updateLibrary);
                GetLatestDawnRelease = GetFunction<LatestDawnRelease>("LatestDawnRelease", updateLibrary);
                hasLoadedLib = true;
            }
        }

        public static void Dispose()
        {
            if (hasLoadedLib)
                FreeLibrary(updateLibrary);
        }

        private static T GetFunction<T>(string signature, IntPtr hModule) where T : Delegate
        {
            if (hModule == IntPtr.Zero) throw new NullReferenceException("hModule is null!");

            IntPtr procAddress = GetProcAddress(hModule, signature);
            return (T)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(T));
        }

    }
}
