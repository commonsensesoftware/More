﻿namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for the <see cref="IContentManager"/> interface.
    /// </summary>
    public static class IContentManagerExtensions
    {
        /// <summary>
        /// Merges the specified content.
        /// </summary>
        /// <param name="contentManager">The extended <see cref="IContentManager">content manager</see>.</param>
        /// <param name="content">The content to merge.</param>
        /// <remarks>This method will determine whether the <paramref name="content"/> exists in the
        /// <see cref="IContentManager.Content">content manager content</see>.  If it does, then no action is taken;
        /// otherwise, the <paramref name="content"/> is <see cref="IContentManager.AddToContent">added to the content manager</see>.
        /// <para><see cref="IContentManager">Content managers</see> will often throw exceptions if duplicate content is added.
        /// this method simplifies the process of constantly verifying content is not added more than once.</para></remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static void MergeContent( this IContentManager contentManager, object content )
        {
            Arg.NotNull( contentManager, nameof( contentManager ) );
            Arg.NotNull( content, nameof( content ) );

            if ( !contentManager.Content.Contains( content ) )
            {
                contentManager.AddToContent( content );
            }
        }
    }
}