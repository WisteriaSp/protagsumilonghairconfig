using protagsumilonghairconfig.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;
using CriFs.V2.Hook;
using CriFs.V2.Hook.Interfaces;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace protagsumilonghairconfig.Configuration
{
	public class Config : Configurable<Config>
	{
        /*
            User Properties:
                - Please put all of your configurable properties here.

            By default, configuration saves as "Config.json" in mod user config folder.    
            Need more config files/classes? See Configuration.cs

            Available Attributes:
            - Category
            - DisplayName
            - Description
            - DefaultValue

            // Technically Supported but not Useful
            - Browsable
            - Localizable

            The `DefaultValue` attribute is used as part of the `Reset` button in Reloaded-Launcher.
        */
        public enum TracksuitEnum
        {
            [Display(Name = "Disabled")]
            Default,

            [Display(Name = "Black Tracksuit")]
            BlackTracksuit,

            [Display(Name = "Concept Art Tracksuit")]
            ConceptArt,
        }

        [Category("Model")]
        [DisplayName("Costumes")]
        [Description("Enables costumes. Keep this option ENABLED unless it's causing issues.")]
        [DefaultValue(true)]
        [Display(Order = 1)]
        public bool CostumeSupport { get; set; } = true;

        [Category("Model")]
        [DisplayName("Recolored Tracksuit")]
        [Description("Replaces the gold workout outfit with a recolored Shujin Academy tracksuit outfit or a recolor by MyTamagos based on concept art.")]
        [DefaultValue(TracksuitEnum.Default)]
        [Display(Order = 2)]
        public TracksuitEnum TracksuitSelection { get; set; }
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
	public class ConfiguratorMixin : ConfiguratorMixinBase
	{
		// 
	}
}