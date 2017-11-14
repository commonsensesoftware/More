namespace More.VisualStudio.Templates
{
    using System.Collections.Generic;

    static class OptionStateMapping
    {
        internal static IReadOnlyDictionary<string, string> Interactions { get; } = new Dictionary<string, string>()
        {
            ["OpenFile"] = "$enableOpenFile$",
            ["SaveFile"] = "$enableSaveFile$",
            ["SelectFolder"] = "$enableSelectFolder$",
            ["TextInput"] = "$enableTextInput$",
            ["SelectContact"] = "$enableSelectContact$",
        };

        internal static IReadOnlyDictionary<string, string> ApplicationContracts { get; } = new Dictionary<string, string>()
        {
            ["Search"] = "$enableSearch$",
            ["AppSearch"] = "$enableAppSearch$",
            ["Share"] = "$enableSharing$",
            ["AppShare"] = "$enableAppSharing$",
            ["Settings"] = "$enableSettings$"
        };
    }
}