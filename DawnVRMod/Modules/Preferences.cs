using MelonLoader;

namespace DawnVR.Modules
{
    internal static class Preferences
    {
        public static MelonPreferences_Entry<bool> CheckForUpdatesOnStart { get; private set; }
        public static MelonPreferences_Entry<bool> Use2DCutsceneViewer { get; private set; }
        public static MelonPreferences_Entry<bool> UseSmoothTurning { get; private set; }
        public static MelonPreferences_Entry<int> SmoothTurnSpeed { get; private set; }
        public static MelonPreferences_Entry<bool> UseSnapTurning { get; private set; }
        public static MelonPreferences_Entry<int> SnapTurnAngle { get; private set; }

        public static MelonPreferences_Entry<bool> SpectatorEnabled { get; private set; }
        public static MelonPreferences_Entry<int> SpectatorFOV { get; private set; }

        public static MelonPreferences_Entry<bool> EnableInternalLogging { get; private set; }
        public static MelonPreferences_Entry<bool> EnablePlayerCollisionVisualization { get; private set; }

        public static void Init()
        {
            MelonPreferences_Category category = MelonPreferences.CreateCategory(baseCategoryName);
            CheckForUpdatesOnStart = category.CreateEntry(nameof(CheckForUpdatesOnStart), true);
            Use2DCutsceneViewer = category.CreateEntry(nameof(Use2DCutsceneViewer), true);
            UseSmoothTurning = category.CreateEntry(nameof(UseSmoothTurning), true);
            SmoothTurnSpeed = category.CreateEntry(nameof(SmoothTurnSpeed), 120);
            UseSnapTurning = category.CreateEntry(nameof(UseSnapTurning), false);
            SnapTurnAngle = category.CreateEntry(nameof(SnapTurnAngle), 45);

            category = MelonPreferences.CreateCategory(spectatorCategoryName);
            SpectatorEnabled = category.CreateEntry(nameof(SpectatorEnabled), false);
            SpectatorFOV = category.CreateEntry(nameof(SpectatorFOV), 90);

            category = MelonPreferences.CreateCategory(debugCategoryName);
            EnableInternalLogging = category.CreateEntry(nameof(EnableInternalLogging), false);
            EnablePlayerCollisionVisualization = category.CreateEntry(nameof(EnablePlayerCollisionVisualization), false);

            MelonPreferences.Save();
        }

        private static readonly string baseCategoryName = "DawnVR";
        private static readonly string debugCategoryName = baseCategoryName + "_Debug";
        private static readonly string spectatorCategoryName = baseCategoryName + "_Spectator";
    }
}
