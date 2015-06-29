namespace More
{
    using System;
    using System.Reflection;

    internal static class Util
    {
        internal static TObject CastOrDefault<TObject>( object parameter )
        {
            var defaultObject = default( TObject );
            return parameter == null && defaultObject != null ? defaultObject : (TObject) parameter;
        }
    }
}
