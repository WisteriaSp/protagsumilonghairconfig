using ProtagSumiConfig.Configuration;
using ProtagSumiConfig.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using CriFs.V2.Hook;
using CriFs.V2.Hook.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using P5R.CostumeFramework;
using BF.File.Emulator.Interfaces;
using BMD.File.Emulator.Interfaces;
using PAK.Stream.Emulator.Interfaces;
using P5R.CostumeFramework.Interfaces;
using Ryo.Interfaces;

namespace ProtagSumiConfig
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;
    
        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;
    
        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;
    
        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;
    
        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;
    
        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;
    
        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            var modDir = _modLoader.GetDirectoryForModId(_modConfig.ModId); // modDir variable for file emulation

            // For more information about this template, please see
            // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

            // If you want to implement e.g. unload support in your mod,
            // and some other neat features, override the methods in ModBase.

            // TODO: Implement some mod logic

            // Define controllers and other variables, set warning messages

            var criFsController = _modLoader.GetController<ICriFsRedirectorApi>();
            if (criFsController == null || !criFsController.TryGetTarget(out var criFsApi))
            {
                _logger.WriteLine($"Something in CriFS broke! Normal files will not load properly!", System.Drawing.Color.Red);
                return;
            }

            var BfEmulatorController = _modLoader.GetController<IBfEmulator>();
            if (BfEmulatorController == null || !BfEmulatorController.TryGetTarget(out var _BfEmulator))
            {
                _logger.WriteLine($"Something in BF Emulator broke! Files requiring bf merging will not load properly!", System.Drawing.Color.Red);
                return;
            }

            var BmdEmulatorController = _modLoader.GetController<IBmdEmulator>();
            if (BmdEmulatorController == null || !BmdEmulatorController.TryGetTarget(out var _BmdEmulator))
            {
                _logger.WriteLine($"Something in BMD Emulator broke! Files requiring msg merging will not load properly!", System.Drawing.Color.Red);
                return;
            }

            var PakEmulatorController = _modLoader.GetController<IPakEmulator>();
            if (PakEmulatorController == null || !PakEmulatorController.TryGetTarget(out var _PakEmulator))
            {
                _logger.WriteLine($"Something in PAK Emulator broke! Files requiring bin merging will not load properly!", System.Drawing.Color.Red);
                return;
            }

            var CostumeFrameworkController = _modLoader.GetController<ICostumeApi>();
            if (CostumeFrameworkController == null || !CostumeFrameworkController.TryGetTarget(out var CostumeFrameworkAPI))
            {
                _logger.WriteLine($"Something in Costume Framework broke! Costumes will not load properly!", System.Drawing.Color.Red);
                return;
            }

            var RyoController = _modLoader.GetController<IRyoApi>();
            if (RyoController == null || !RyoController.TryGetTarget(out var ryo))
            {
                _logger.WriteLine($"Something in Ryo broke! Audio configs will not load!", System.Drawing.Color.Red);
                return;
            }

            /*                        var BGMEController = _modLoader.GetController<IBgmeApi>();
                        if (BGMEController == null || !BGMEController.TryGetTarget(out var _BGME))
                        {
                            _logger.WriteLine($"Something in BGME shit its pants! Files requiring bin merging will not load properly!", System.Drawing.Color.Red);
                            return;
                        }

            */
            // Set configuration options - obviously you don't need all of these, pick and choose what you need!

            // criFS
            var mods = _modLoader.GetActiveMods();
            var isRoseAndVioletActive = mods.Any(x => x.Generic.ModId == "p5rpc.kasumi.roseandviolet");
            var isCBTActive = mods.Any(x => x.Generic.ModId == "p5r.enhance.cbt");


            if (isRoseAndVioletActive)
            {
                _logger.WriteLine($"Found Rose and Violet story overhaul, disabling event fixes to prevent conflicts.", System.Drawing.Color.Green);
            }
            else if (_configuration.EventEdits1)
            {
                criFsApi.AddProbingPath("OptionalModFiles\\Events\\Fixes");
            }

            if (isRoseAndVioletActive)
            {
                _logger.WriteLine($"Found Rose and Violet story overhaul, disabling major event edits to prevent conflicts.", System.Drawing.Color.Green);
            }
            else if (_configuration.EventEditsBig)
            {
                criFsApi.AddProbingPath("OptionalModFiles\\Events\\LargeEdits");
                _BfEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Events", "LargeEdits", "BF"));
                _BmdEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Events", "LargeEdits", "BMD"));
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Events", "LargeEdits", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
            }


            // Better Dungeon Exit Model
            if (isCBTActive)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Model", "CBT", "BetterExitMaterials", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
            }

            // Darkened Face
            if (_configuration.DarkenedFace)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Model", "DarkenedFace", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
            }

            // Blue Dress
            if (_configuration.BlueDress)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Model", "BlueDress", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
                else
                {
                    _logger.WriteLine($"Character asset folder not found: {assetFolder}", System.Drawing.Color.Yellow);
                }
            }

            // Black Tracksuit
            if (_configuration.TracksuitSelection == Config.TracksuitEnum.BlackTracksuit ||
                _configuration.TracksuitSelection == Config.TracksuitEnum.ConceptArt)
            {
                string? TracksuitFolder = _configuration.TracksuitSelection switch
                {
                    Config.TracksuitEnum.BlackTracksuit => "OldTracksuit",
                    Config.TracksuitEnum.ConceptArt => "TracksuitConceptArt",
                    _ => null
                };

                if (!string.IsNullOrEmpty(TracksuitFolder))
                {
                    var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Model", TracksuitFolder, "Characters", "Joker", "1");

                    if (Directory.Exists(assetFolder))
                    {
                        foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                        {
                            var relativePath = Path.GetRelativePath(assetFolder, file);
                            criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                        }
                    }
                }
            }

            // Metaverse Field Run
            if (_configuration.AltMetaRun)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Animation", "AltMetaRun", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
                else
                {
                    _logger.WriteLine($"Character asset folder not found: {assetFolder}", System.Drawing.Color.Yellow);
                }
            }

            // Women's Bath House
            if (_configuration.Bathhouse)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Bath"));
                _BfEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Bath", "BF"));
                _BmdEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Bath", "BMD"));
            }

            // Thieves Den
            if (_configuration.ThievesDenAddon)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen"));
                _BfEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen", "BF"));
                _BmdEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen", "BMD"));
                _PakEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen", "PAK"));
            }

            // Women's Bath House Event
            if (_configuration.BathActivity)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Events", "Bath"));
            }

            // Shujin Restroom
            if (_configuration.Restroom)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Restroom"));
                _BfEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Restroom", "BF"));
            }

            // Equipment Config
            if (_configuration.Equipment)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Equipment"));
                _PakEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Equipment", "FEmulator", "PAK"));
            }

            // Persona Swap Config
            if (_configuration.PersonasMod == Config.CendrillonMod.DefaultCendrillon ||
                _configuration.PersonasMod == Config.CendrillonMod.RedCendrillon)
            {

                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas"));
                _PakEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas", "FEmulator", "PAK"));
                _BmdEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas", "FEmulator", "BMD"));

                string? cendrillonFolder = _configuration.PersonasMod switch
                {
                    Config.CendrillonMod.DefaultCendrillon => "Cendrillon",
                    Config.CendrillonMod.RedCendrillon => "CurseCendrillon",
                    _ => null
                };

                if (!string.IsNullOrEmpty(cendrillonFolder))
                {
                    criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", cendrillonFolder));
                }
            }


            // Skillset Config
            if (_configuration.Skillset)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Skillset"));
                _BfEmulator.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Skillset", "FEmulator", "BF"));
            }

            // Opening Movie by Arbiter
            if (_configuration.OpeningMovie)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Misc", "Movie"));
            }


            // NoAoAArt
            if (_configuration.AoAArt == Config.AoAArtEnum.Enabled ||
                _configuration.AoAArt == Config.AoAArtEnum.Smug)
            {
                List<string> aoaFolders = new();

                if (_configuration.AoAArt == Config.AoAArtEnum.Enabled)
                {
                    aoaFolders.Add("NoAoAPortrait");
                }
                else if (_configuration.AoAArt == Config.AoAArtEnum.Smug)
                {
                    aoaFolders.AddRange(new[] { "NoAoAPortrait", "SmugAoA" });
                }

                foreach (var folder in aoaFolders)
                {
                    var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Model", folder, "Characters", "Joker", "1");

                    if (Directory.Exists(assetFolder))
                    {
                        foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                        {
                            var relativePath = Path.GetRelativePath(assetFolder, file);
                            criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                        }
                    }
                }
            }


            // OneCalledJay Bustup
            if (_configuration.Bustup1 == Config.BustupSelection.OnedCalledJay)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Bustup", "OneCalledJay", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
                else
                {
                    _logger.WriteLine($"Character asset folder not found: {assetFolder}", System.Drawing.Color.Yellow);
                }
            }

            // Weapon Ranged
            if (_configuration.WeaponRanged == Config.WeaponRangedEnum.LeverAction)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Model", "Ranged", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
            }

            // Weapon Melee
            if (_configuration.MeleeRanged == Config.MeleeRangedEnum.Rapier)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Model", "Melee", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
            }

            //Costumes
            if (_configuration.CostumeSupport)
            {
                var CostumeFolder = Path.Combine(modDir, "OptionalModFiles", "CostumeSupport", "Costumes");
                CostumeFrameworkAPI.AddCostumesFolder(modDir, CostumeFolder);
            }

            if (_configuration.MiniBossMusic)
            {
                var audioFolder = Path.Combine(modDir, "OptionalModFiles", "Audio", "MiniBoss1");
                if (Directory.Exists(audioFolder))
                    ryo.AddAudioPath(audioFolder, null);
            }

            // criFS Bustup
            if (_configuration.Bustup1 == Config.BustupSelection.L7M3)
            {
                var assetFolder = Path.Combine(modDir, "OptionalModFiles", "Bustup", "L7M3", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
                }
                else
                {
                    _logger.WriteLine($"Character asset folder not found: {assetFolder}", System.Drawing.Color.Yellow);
                }
            }

        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion
    
        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
    }
}