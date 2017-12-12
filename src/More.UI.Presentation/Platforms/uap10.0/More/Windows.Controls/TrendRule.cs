namespace More.Windows.Controls
{
    using global::Windows.UI.Xaml.Markup;
    using More.Windows.Data;
    using System;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    [ContentProperty( Name = nameof( Rules ) )]
    public partial class TrendRule : NumericRule
    {
    }
}