namespace More
{
    using System;

    static class Util
    {
        internal static TObject CastOrDefault<TObject>( object parameter )
        {
            var defaultObject = default( TObject );
            return parameter == null && defaultObject != null ? defaultObject : (TObject) parameter;
        }
    }
}