using System;
using UnityEngine;
using MelonLoader;

namespace DawnVR.Modules
{
    internal static class Preferences
    {
        public static bool UseSmoothTurning { get; private set; }
        public static int SmoothTurnSpeed { get; private set; }
        public static bool UseSnapTurning { get; private set; }
        public static int SnapTurnAngle { get; private set; }

        public static void Init()
        {
            MelonPreferences.CreateCategory(categoryName);
            MelonPreferences.CreateEntry(categoryName, nameof(UseSmoothTurning), true);
            MelonPreferences.CreateEntry(categoryName, nameof(SmoothTurnSpeed), 120);
            MelonPreferences.CreateEntry(categoryName, nameof(UseSnapTurning), false);
            MelonPreferences.CreateEntry(categoryName, nameof(SnapTurnAngle), 45);

            UseSmoothTurning = MelonPreferences.GetEntryValue<bool>(categoryName, nameof(UseSmoothTurning));
            SmoothTurnSpeed = MelonPreferences.GetEntryValue<int>(categoryName, nameof(SmoothTurnSpeed));
            UseSnapTurning = MelonPreferences.GetEntryValue<bool>(categoryName, nameof(UseSnapTurning));
            SnapTurnAngle = MelonPreferences.GetEntryValue<int>(categoryName, nameof(SnapTurnAngle));
        }

        private static readonly string categoryName = "DawnVR";
    }
}
