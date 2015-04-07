namespace More.Composition
{
    using System;

    /// <summary>
    /// Represents the well-known sharing boundary names. The composition provider uses all of these when handling a web request.
    /// </summary>
    public static class Boundary
    {
        /// <summary>
        /// The boundary within which a current HTTP request is accessible.
        /// </summary>
        /// <example>The following example demonstrates how to import a part that uses Per-Http-Request lifetime management.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Data.Entity;
        /// 
        /// /// <summary>
        /// /// Represents a DbContext for domain entities that are shared using Per-Http-Request lifetime management.
        /// /// </summary>
        /// [Shared( Boundary.HttpRequest )]
        /// public class MyEntities : DbContext
        /// {
        /// }
        /// 
        /// /// <summary>
        /// /// Represents a repository for domain entities that are shared using Per-Http-Request lifetime management.
        /// /// </summary>
        /// public class MyRepository : ReadOnlyRepository<MyEntity>
        /// {
        ///     private readonly DbContext context;
        ///     
        ///     public MyRepository( [SharingBoundary( Boundary.HttpRequest )] DbContext context )
        ///     {
        ///         this.context = context;
        ///     }
        ///     
        ///     public override IQueryable<MyEntity> All
        ///     {
        ///         get
        ///         {
        ///             return this.context.Set<MyEntity>();
        ///         }
        ///     }
        /// }
        /// ]]></code></example>
        public const string HttpRequest = "HttpRequest";

        /// <summary>
        /// The boundary within which a consistent view of persistent data is available.
        /// </summary>
        public const string DataConsistency = "DataConsistency";

        /// <summary>
        /// The boundary within which a single user can be identified.
        /// </summary>
        public const string UserIdentity = "UserIdentity";
    }
}