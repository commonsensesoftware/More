namespace More
{
    using System;

    /// <summary>
    /// Defines a bridge interface to System.ICloneable, which is not portable.
    /// </summary>
    internal interface ICloneable
    {
        object Clone();
    }
}
