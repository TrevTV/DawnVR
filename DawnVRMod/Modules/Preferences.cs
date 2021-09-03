using MelonLoader;

namespace DawnVR.Modules
{
    internal static class Preferences
    {
        public static bool UseSmoothTurning { get; private set; }
        public static int SmoothTurnSpeed { get; private set; }
        public static bool UseSnapTurning { get; private set; }
        public static int SnapTurnAngle { get; private set; }

        public static bool EnableCutsceneBorder { get; private set; }

        public static bool EnableInternalLogging { get; private set; }

        public static void Init()
        {
            MelonPreferences_Category category = MelonPreferences.CreateCategory(categoryName);
            MelonPreferences_Entry<bool> smoothTurning = category.CreateEntry(nameof(UseSmoothTurning), true);
            MelonPreferences_Entry<int> smoothTurnSpeed = category.CreateEntry(nameof(SmoothTurnSpeed), 120);
            MelonPreferences_Entry<bool> snapTurning = category.CreateEntry(nameof(UseSnapTurning), false);
            MelonPreferences_Entry<int> snapTurnAngle = category.CreateEntry(nameof(SnapTurnAngle), 45);
            MelonPreferences_Entry<bool> enableCutsceneBorder = category.CreateEntry(nameof(EnableCutsceneBorder), true);
            MelonPreferences_Entry<bool> enableInternalLogging = category.CreateEntry(nameof(EnableInternalLogging), false);

            MelonPreferences.Save();

            UseSmoothTurning = smoothTurning.Value;
            SmoothTurnSpeed = smoothTurnSpeed.Value;
            UseSnapTurning = snapTurning.Value;
            SnapTurnAngle = snapTurnAngle.Value;
            EnableCutsceneBorder = enableCutsceneBorder.Value;
            EnableInternalLogging = enableInternalLogging.Value;
        }

        private static readonly string categoryName = "DawnVR";
    }
}
