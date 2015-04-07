namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents the base implemention for a <see cref="IShellView">shell view</see> with a view model using a <see cref="Frame">frame</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of start <see cref="Page">page</see> view associated with the <see cref="IShellView">shell view</see>.</typeparam>
    [CLSCompliant( false )]
    public class FrameShellView<T> : FrameShellViewBase where T : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameShellView{T}"/> class.
        /// </summary>
        public FrameShellView()
            : this( More.ServiceProvider.Current )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameShellView{T}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> associated with the shell view.</param>
        public FrameShellView( IServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null, "serviceProvider" );
            this.StartPage = typeof( T );
        }
    }
}
