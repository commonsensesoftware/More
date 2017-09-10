namespace More.Composition.Hosting
{
    using More.Windows;
    using System;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class Host
    {
        partial void AddWinRTDefaultServices() => base.AddService( typeof( IContinuationManager ), ( sc, t ) => new ContinuationManager() );
    }
}