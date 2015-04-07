namespace More
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides empty or "no operation" methods that can be used to satisfy the Null Object pattern for method callbacks
    /// <seealso cref="Action"/><seealso cref="Action{T}"/><seealso cref="Action{T1,T2}"/><seealso cref="Action{T1,T2,T3}"/>
    /// <seealso cref="Action{T1,T2,T3,T4}"/>.
    /// </summary>
    public static class DefaultAction
    {
        /// <summary>
        /// Defines an empty method for an <see cref="Action">action</see>.
        /// </summary>
        /// <remarks>This method has no implementation and performs no action.</remarks>
        public static void None()
        {
        }

        /// <summary>
        /// Defines an empty method for an <see cref="Action{T}">action</see>.
        /// </summary>
        /// <typeparam name="T">The method argument <see cref="Type">type</see>.</typeparam>
        /// <param name="arg">The method argument.</param>
        /// <remarks>This method has no implementation and performs no action.</remarks>
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg", Justification = "Required for method signature which implements the Null Object pattern." )]
        public static void None<T>( T arg )
        {
        }

        /// <summary>
        /// Defines an empty method for an <see cref="Action{T1,T2}">action</see>.
        /// </summary>
        /// <typeparam name="T1">The first argument <see cref="Type">type</see>.</typeparam>
        /// <typeparam name="T2">The second argument <see cref="Type">type</see>.</typeparam>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <remarks>This method has no implementation and performs no action.</remarks>
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1", Justification = "Required for method signature which implements the Null Object pattern." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2", Justification = "Required for method signature which implements the Null Object pattern." )]
        public static void None<T1, T2>( T1 arg1, T2 arg2 )
        {
        }

        /// <summary>
        /// Defines an empty method for an <see cref="Action{T1,T2,T3}">action</see>.
        /// </summary>
        /// <typeparam name="T1">The first argument <see cref="Type">type</see>.</typeparam>
        /// <typeparam name="T2">The second argument <see cref="Type">type</see>.</typeparam>
        /// <typeparam name="T3">The third argument <see cref="Type">type</see>.</typeparam>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        /// <remarks>This method has no implementation and performs no action.</remarks>
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1", Justification = "Required for method signature which implements the Null Object pattern." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2", Justification = "Required for method signature which implements the Null Object pattern." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3", Justification = "Required for method signature which implements the Null Object pattern." )]
        public static void None<T1, T2, T3>( T1 arg1, T2 arg2, T3 arg3 )
        {
        }

        /// <summary>
        /// Defines an empty method for an <see cref="Action{T1,T2,T3,T4}">action</see>.
        /// </summary>
        /// <typeparam name="T1">The first argument <see cref="Type">type</see>.</typeparam>
        /// <typeparam name="T2">The second argument <see cref="Type">type</see>.</typeparam>
        /// <typeparam name="T3">The third argument <see cref="Type">type</see>.</typeparam>
        /// <typeparam name="T4">The forth argument <see cref="Type">type</see>.</typeparam>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        /// <param name="arg4">The fourth argument.</param>
        /// <remarks>This method has no implementation and performs no action.</remarks>
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1", Justification = "Required for method signature which implements the Null Object pattern." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2", Justification = "Required for method signature which implements the Null Object pattern." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3", Justification = "Required for method signature which implements the Null Object pattern." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg4", Justification = "Required for method signature which implements the Null Object pattern." )]
        public static void None<T1, T2, T3, T4>( T1 arg1, T2 arg2, T3 arg3, T4 arg4 )
        {
        }
    }
}
