using NaoSmiley.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;
using CriFs.V2.Hook;
using CriFs.V2.Hook.Interfaces;
using System.Reflection;

namespace NaoSmiley.Configuration
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

        [Category("Events")]
        [DisplayName("Event Edits")]
		[Description("Disable this if you're having issues with other mods that edit events.")]
		[DefaultValue(true)]
		public bool EventEdits1 { get; set; } = true; // bool used in Mod.CS, not the folder name, but the bool name

        [Category("Bustup")]
        [DisplayName("L7M3's Bustups")]
        [Description("Enable this to use L7M3's custom drawn bustups.")]
        [DefaultValue(false)]
        public bool Bustup1 { get; set; } = false;
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