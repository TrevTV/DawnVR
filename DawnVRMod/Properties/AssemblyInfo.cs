using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(DawnVR.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(DawnVR.BuildInfo.Company)]
[assembly: AssemblyProduct(DawnVR.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + DawnVR.BuildInfo.Author)]
[assembly: AssemblyTrademark(DawnVR.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(DawnVR.BuildInfo.Version)]
[assembly: AssemblyFileVersion(DawnVR.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(DawnVR.VRMain), DawnVR.BuildInfo.Name, DawnVR.BuildInfo.Version, DawnVR.BuildInfo.Author, DawnVR.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]