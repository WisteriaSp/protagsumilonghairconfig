using protagsumilonghairconfig.Configuration;
using protagsumilonghairconfig.Template;
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

namespace protagsumilonghairconfig
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
            var isCBTActive = mods.Any(x => x.Generic.ModId == "p5r.enhance.cbt");

            // Better Dungeon Exit Model
            if (isCBTActive)
            {
                var assetFolder = Path.Combine(modDir, "OptionalFiles", "CBT", "BetterExitMaterials", "Characters", "Joker", "1");

                if (Directory.Exists(assetFolder))
                {
                    foreach (var file in Directory.EnumerateFiles(assetFolder, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(assetFolder, file);
                        criFsApi.AddBind(file, relativePath, _modConfig.ModId);
                    }
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
                    var assetFolder = Path.Combine(modDir, "OptionalFiles", TracksuitFolder, "Characters", "Joker", "1");

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

            //Costumes
            if (_configuration.CostumeSupport)
            {
                var CostumeFolder = Path.Combine(modDir, "OptionalFiles", "CostumeSupport", "Costumes");
                CostumeFrameworkAPI.AddCostumesFolder(modDir, CostumeFolder);
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