using MelonLoader;

namespace DawnVR.Modules
{
    internal static class Preferences
    {
        public static MelonPreferences_Entry<bool> UseSmoothTurning { get; private set; }
        public static MelonPreferences_Entry<int> SmoothTurnSpeed { get; private set; }
        public static MelonPreferences_Entry<bool> UseSnapTurning { get; private set; }
        public static MelonPreferences_Entry<int> SnapTurnAngle { get; private set; }

        public static MelonPreferences_Entry<bool> EnableInternalLogging { get; private set; }

        public static void Init()
        {
            MelonPreferences_Category category = MelonPreferences.CreateCategory(categoryName);
            UseSmoothTurning = category.CreateEntry(nameof(UseSmoothTurning), true);
            SmoothTurnSpeed = category.CreateEntry(nameof(SmoothTurnSpeed), 120);
            UseSnapTurning = category.CreateEntry(nameof(UseSnapTurning), false);
            SnapTurnAngle = category.CreateEntry(nameof(SnapTurnAngle), 45);
            EnableInternalLogging = category.CreateEntry(nameof(EnableInternalLogging), false);

            MelonPreferences.Save();
        }

        private static readonly string categoryName = "DawnVR";
    }
}
