namespace More
{
    using global::System;
    using global::System.Reflection;

    internal static class Util
    {
        internal static TObject CastOrDefault<TObject>( object parameter )
        {
            return parameter == null && typeof( TObject ).GetTypeInfo().IsValueType ? default( TObject ) : (TObject) parameter;
        }
    }
}
