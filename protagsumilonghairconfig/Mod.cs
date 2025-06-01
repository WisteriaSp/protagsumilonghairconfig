using protagsumilonghairconfig.Configuration;
using protagsumilonghairconfig.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using CriFs.V2.Hook.Interfaces;
using P5R.CostumeFramework;
using P5R.CostumeFramework.Interfaces;

namespace protagsumilonghairconfig
{
    public class Mod : ModBase
    {
        private readonly IModLoader _modLoader;
        private readonly IReloadedHooks? _hooks;
        private readonly ILogger _logger;
        private readonly IMod _owner;
        private Config _configuration;
        private readonly IModConfig _modConfig;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            // grab modDir and modId
            string modDir = _modLoader.GetDirectoryForModId(_modConfig.ModId);
            string modId = _modConfig.ModId;

            var criFsCtl = _modLoader.GetController<ICriFsRedirectorApi>();
            var costumeCtl = _modLoader.GetController<ICostumeApi>();

            if (criFsCtl == null || !criFsCtl.TryGetTarget(out var criFsApi)) { _logger.WriteLine("CRI FS Emu missing → config binds broken.", System.Drawing.Color.Red); return; }
            if (costumeCtl == null || !costumeCtl.TryGetTarget(out var costumeApi)) { _logger.WriteLine("Costume API missing → Costumes broken.", System.Drawing.Color.Red); return; }

            var active = _modLoader.GetActiveMods().Select(x => x.Generic.ModId).ToHashSet();
            bool isCBT = active.Contains("p5r.enhance.cbt");

            //CONFIGS BELOW, we pussy on

            // Better Dungeon Exit Model
            if (isCBT)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalFiles", "Model", "CBTBetterExitMaterials"),
                    modDir, criFsApi, modId
                );
            }

            // Black Tracksuit
            if (_configuration.TracksuitSelection == Config.TracksuitEnum.BlackTracksuit ||
                _configuration.TracksuitSelection == Config.TracksuitEnum.ConceptArt)
            {
                string selected =
                    _configuration.TracksuitSelection == Config.TracksuitEnum.BlackTracksuit
                        ? "OldTracksuit"
                        : "TracksuitConceptArt";

                BindAllFilesIn(
                    Path.Combine("OptionalFiles", selected),
                    modDir, criFsApi, modId
                );
            }

            // Costume Support
            if (_configuration.CostumeSupport)
            {
                var costumesFolder = Path.Combine(modDir, "OptionalFiles", "Costumes");
                costumeApi.AddCostumesFolder(modDir, costumesFolder);
            }
        }

        /// <summary>
        /// recursively enumerates all files under the given “subPath” (relative to the mod folder),
        /// and issues a single AddBind(...) per file. If the directory doesn’t exist, it silently does nothing.
        /// </summary>
        private static void BindAllFilesIn(
            string subPathRelativeToModDir,
            string modDir,
            ICriFsRedirectorApi criFsApi,
            string modId
        )
        {
            string absoluteFolder = Path.Combine(modDir, subPathRelativeToModDir);

            if (!Directory.Exists(absoluteFolder))
            {
                // _logger.WriteLine($"Folder not found: {absoluteFolder}", System.Drawing.Color.Yellow);
                return;
            }

            foreach (var filePath in Directory.EnumerateFiles(absoluteFolder, "*", SearchOption.AllDirectories))
            {
                string relativeCpkKey = Path.GetRelativePath(absoluteFolder, filePath).Replace(Path.DirectorySeparatorChar, '/');

                criFsApi.AddBind(
                    filePath,
                    relativeCpkKey,
                    modId
                );
            }
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}
