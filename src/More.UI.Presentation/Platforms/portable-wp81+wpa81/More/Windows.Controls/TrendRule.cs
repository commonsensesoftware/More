namespace More.Windows.Controls
{
    using More.Windows.Data;
    using System;
    using global::Windows.UI.Xaml.Markup;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    [ContentProperty( Name = nameof( Rules ) )]
    public partial class TrendRule : NumericRule
    {
    }
}