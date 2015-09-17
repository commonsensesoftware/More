namespace More.VisualStudio.Templates
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    internal static class OptionStateMapping
    {
        private static readonly Dictionary<string, string> interactions = new Dictionary<string, string>()
        {
            { "OpenFile", "$enableOpenFile$" },
            { "SaveFile", "$enableSaveFile$" },
            { "SelectFolder", "$enableSelectFolder$" },
            { "TextInput", "$enableTextInput$" },
            { "SelectContact", "$enableSelectContact$" }
        };

        private static readonly Dictionary<string, string> appContracts = new Dictionary<string, string>()
        {
            { "Search", "$enableSearch$" },
            { "AppSearch", "$enableAppSearch$" },
            { "Share", "$enableSharing$" },
            { "AppShare", "$enableAppSharing$" },
            { "Settings", "$enableSettings$" }
        };

        internal static IReadOnlyDictionary<string, string> Interactions
        {
            get
            {
                Contract.Ensures( interactions != null );
                return interactions;
            }
        }

        internal static IReadOnlyDictionary<string, string> ApplicationContracts
        {
            get
            {
                Contract.Ensures( appContracts != null );
                return appContracts;
            }
        }
    }
}
