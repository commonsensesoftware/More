namespace Microsoft.Win32
{
    using System;

    [Flags]
    enum ShellItemArrayAttributeFlags
    {
        And = 1,                    // if multiple items and the attirbutes together.
        Or,                         // if multiple items or the attributes together.
        ApplicationCompatibility,   // Call GetAttributes directly on the ShellFolder for multiple attributes
    }
}