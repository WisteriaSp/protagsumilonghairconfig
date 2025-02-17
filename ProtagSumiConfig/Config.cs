using ProtagSumiConfig.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;
using CriFs.V2.Hook;
using CriFs.V2.Hook.Interfaces;
using System.Reflection;

namespace ProtagSumiConfig.Configuration
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

        [Category("Animation")]
        [DisplayName("Alternate Metaverse Run Animation")]
        [Description("Replaces the metaverse run animation with her field running animation, similar to lpspectrum's mod.")]
        [DefaultValue(false)]
        public bool AltMetaRun { get; set; } = false;

        [Category("Bustup")]
        [DisplayName("L7M3's Bustups")]
        [Description("Enable this to use L7M3's custom drawn bustups.")]
        [DefaultValue(false)]
        public bool Bustup1 { get; set; } = false;

        [Category("Model")]
        [DisplayName("Undarkened Face")]
        [Description("Removes the face darkening when summoning a persona. Meant to be used with the No Darkened Faces mod, disable if you'd like.")]
        [DefaultValue(true)]
        public bool DarkenedFace { get; set; } = true;

        [Category("Model")]
        [DisplayName("Blue Dress over Winter Casual")]
        [Description("Replaces the winter casual outfit with the blue dress outfit from Kasumi's SL.")]
        [DefaultValue(false)]
        public bool BlueDress { get; set; } = false;

        [Category("Model")]
        [DisplayName("Recolored Black Tracksuit")]
        [Description("Replaces the gold and black workout outfit with a recolored Shujin Academy tracksuit outfit.")]
        [DefaultValue(false)]
        public bool BlackTracksuit { get; set; } = false;

        [Category("Flowscript and BMD")]
        [DisplayName("Women's Bath House")]
        [Description("Contains flowscript and msg edits to make the Yongen-Jaya Bath House women's only.")]
        [DefaultValue(true)]
        public bool Bathhouse { get; set; } = true;

        [Category("Flowscript and BMD")]
        [DisplayName("Enterable Women's Bathroom")]
        [Description("Allows you to enter the women's bathroom at Shujin Academy.")]
        [DefaultValue(true)]
        public bool Restroom { get; set; } = true;

        [Category("Events")]
        [DisplayName("Event Fixes")]
		[Description("Tweaks various events to fix issues with protag Sumi. Disable this if you're having issues with other mods that edit events.")]
		[DefaultValue(true)]
		public bool EventEdits1 { get; set; } = true; // bool used in Mod.CS, not the folder name, but the bool name

        [Category("Events")]
        [DisplayName("Event Additions")]
        [Description("Contains major edits to select events. Disable this if you're having issues with other mods that edit events.")]
        [DefaultValue(true)]
        public bool EventEditsBig { get; set; } = true;

        [Category("Events")]
        [DisplayName("Women's Bath House Event")]
        [Description("Contains event edits needed for the Bath House activity. Disable if causing issues or if the flowscript config is disabled.")]
        [DefaultValue(true)]
        public bool BathActivity { get; set; } = true; // bool used in Mod.CS, not the folder name, but the bool name
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