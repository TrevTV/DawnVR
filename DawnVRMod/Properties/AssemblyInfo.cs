using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(VRMod.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(VRMod.BuildInfo.Company)]
[assembly: AssemblyProduct(VRMod.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + VRMod.BuildInfo.Author)]
[assembly: AssemblyTrademark(VRMod.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(VRMod.BuildInfo.Version)]
[assembly: AssemblyFileVersion(VRMod.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(VRMod.VRMain), VRMod.BuildInfo.Name, VRMod.BuildInfo.Version, VRMod.BuildInfo.Author, VRMod.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]