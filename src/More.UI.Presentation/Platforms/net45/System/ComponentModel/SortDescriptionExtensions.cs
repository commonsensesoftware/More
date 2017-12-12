namespace System.ComponentModel
{
    using More;
    using More.Windows.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="SortDescription"/> class <seealso cref="SortDescriptionCollection"/>.
    /// </summary>
    public static class SortDescriptionExtensions
    {
        sealed class TypeComparer : IEqualityComparer<Type>
        {
            internal static TypeComparer Default { get; } = new TypeComparer();

            public bool Equals( Type x, Type y )
            {
                if ( x == null )
                {
                    return y == null;
                }
                else if ( y == null )
                {
                    return false;
                }
                else if ( x.Equals( y ) )
                {
                    return true;
                }

                var a = x.GetTypeInfo();

                if ( a.IsGenericType )
                {
                    var b = y.GetTypeInfo();

                    if ( b.IsGenericType )
                    {
                        return a.GetGenericTypeDefinition().Equals( b.GetGenericTypeDefinition() );
                    }
                }

                return false;
            }

            public int GetHashCode( Type obj ) => obj?.GetHashCode() ?? 0;
        }

        static MethodInfo GetGenericMethod( Type type, string name, params Type[] parameterTypes )
        {
            Contract.Requires( !string.IsNullOrEmpty( name ) );
            Contract.Ensures( Contract.Result<MethodInfo>() != null );

            var query = from m in type.GetRuntimeMethods()
                        where m.Name == name && m.IsGenericMethod
                        let argTypes = m.GetParameters().Select( arg => arg.ParameterType ).ToArray()
                        where argTypes.Length == parameterTypes.Length && argTypes.SequenceEqual( parameterTypes, TypeComparer.Default )
                        select m;

            return query.Single();
        }

        static Expression GetExpressionForPropertyPath( Type itemType, string sortDescriptorPropertyPath, Expression expression, out Type returnType )
        {
            Contract.Requires( itemType != null );
            Contract.Requires( !string.IsNullOrEmpty( sortDescriptorPropertyPath ) );
            Contract.Requires( expression != null );
            Contract.Ensures( Contract.ValueAtReturn<Type>( out returnType ) != null );
            Contract.Ensures( Contract.Result<Expression>() != null );

            returnType = itemType;

            var propertyPath = sortDescriptorPropertyPath.Split( '.' );

            for ( var i = 0; i < propertyPath.Length; i++ )
            {
                var propertyName = propertyPath[i];
                var property = returnType.GetRuntimeProperty( propertyName );

                if ( property == null )
                {
                    throw new MissingMemberException( ExceptionMessage.MissingMemberException.FormatDefault( returnType.FullName, propertyName ) );
                }

                expression = Expression.Property( expression, property );
                returnType = property.PropertyType;
            }

            return expression;
        }

        static object CreateLambdaExpression( Type itemType, string propertyName, out Type returnType )
        {
            Contract.Requires( itemType != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
            Contract.Ensures( Contract.ValueAtReturn( out returnType ) != null );
            Contract.Ensures( Contract.Result<object>() != null );

            var param = Expression.Parameter( itemType, "item" );
            var expression = GetExpressionForPropertyPath( itemType, propertyName, param, out returnType );
            var func = typeof( Func<,> ).MakeGenericType( itemType, returnType );
            var method = GetGenericMethod( typeof( Expression ), "Lambda", typeof( Expression ), typeof( ParameterExpression[] ) );
            var lambda = method.MakeGenericMethod( func ).Invoke( null, new object[] { expression, new ParameterExpression[] { param } } );

            return lambda;
        }

        static IEnumerable<T> InvokeEnumerable<T>( IEnumerable<T> sequence, string methodName, string propertyName, bool ordered )
        {
            Contract.Requires( sequence != null );
            Contract.Requires( !string.IsNullOrEmpty( methodName ) );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
            Contract.Ensures( Contract.Result<IEnumerable<T>>() != null );

            var itemType = typeof( T );
            var lambda = CreateLambdaExpression( itemType, propertyName, out var returnType );
            var func = ( (LambdaExpression) lambda ).Compile();
            var parameterTypes = new[] { ( ordered ? typeof( IOrderedEnumerable<> ) : typeof( IEnumerable<> ) ), typeof( Func<,> ) };
            var method = GetGenericMethod( typeof( Enumerable ), methodName, parameterTypes ).MakeGenericMethod( itemType, returnType );

            return (IEnumerable<T>) method.Invoke( null, new object[] { sequence, func } );
        }

        static IQueryable<T> InvokeQueryable<T>( IQueryable<T> sequence, string methodName, string propertyName, bool ordered )
        {
            Contract.Requires( sequence != null );
            Contract.Requires( !string.IsNullOrEmpty( methodName ) );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
            Contract.Ensures( Contract.Result<IQueryable<T>>() != null );

            var itemType = typeof( T );
            var lambda = CreateLambdaExpression( itemType, propertyName, out var returnType );
            var parameterTypes = new[] { ( ordered ? typeof( IOrderedQueryable<> ) : typeof( IQueryable<> ) ), typeof( Expression<> ) };
            var method = GetGenericMethod( typeof( Queryable ), methodName, parameterTypes ).MakeGenericMethod( itemType, returnType );

            return (IQueryable<T>) method.Invoke( null, new object[] { sequence, lambda } );
        }

        static bool IsQueryableOrdered( Expression expression )
        {
            Contract.Requires( expression != null );

            if ( expression.NodeType != ExpressionType.Call )
            {
                return false;
            }

            var method = ( (MethodCallExpression) expression ).Method.Name;
            var comparer = StringComparer.Ordinal;

            return comparer.Equals( method, nameof( Queryable.OrderBy ) ) || comparer.Equals( method, nameof( Queryable.OrderByDescending ) );
        }

        /// <summary>
        /// Applies the sort operation defined by the sort description to the specified sequence.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of items to sort.</typeparam>
        /// <param name="sequence">The <see cref="IEnumerable{T}"/> sequence of items to sort.</param>
        /// <param name="sortDescription">The <see cref="SortDescription"/> containing the sorting information.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> object.</returns>
        /// <remarks>If a property name in the <see cref="SortDescription"/> contains a property path, only the last expression in the
        /// path will be used.  Use the overloaded version of this method to override this behavior.</remarks>
        /// <example>This example illustrates how to sort items using the provided <see cref="SortDescriptionCollection">sort descriptions</see>.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        ///
        /// public partial class MyUserControl : UserControl
        /// {
        ///     private readonly List<MyData> items = new List<MyData>();
        ///     internal DataPager DataPager;
        ///
        ///     private void Task<PagedItemCollection<MyData>> GetMyDataAsync( PagingArguments args )
        ///     {
        ///         // TODO: populate the list
        ///
        ///         IEnumerable<MyData> pagedItems = this.items;
        ///
        ///         if ( args.SortDescriptions.Any() )
        ///         {
        ///             pagedItems = this.items.ApplySortDescription( args.SortDescriptions.First() );
        ///         }
        ///
        ///         pagedItems = pagedItems.Skip( args.InitialIndex ).Take( args.PageSize ).ToArray();
        ///
        ///         return Task.FromResult( new PagedItemCollection<MyData>( pagedItems, this.items.Count ) );
        ///     }
        ///
        ///     public MyUserControl()
        ///     {
        ///         this.InitializeComponent();
        ///         this.DataPager.ItemsSource = new VirtualCollectionView<MyData>( this.GetMyDataAsync );
        ///     }
        /// }
        /// ]]></code></example>
        public static IEnumerable<T> ApplySortDescription<T>( this IEnumerable<T> sequence, SortDescription sortDescription )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IEnumerable<T>>() != null );

            var ordered = sequence is IOrderedEnumerable<T>;
            string sortMethod = null;

            switch ( sortDescription.Direction )
            {
                case ListSortDirection.Ascending:
                    sortMethod = ordered ? nameof( Queryable.ThenBy ) : nameof( Queryable.OrderBy );
                    break;
                case ListSortDirection.Descending:
                    sortMethod = ordered ? nameof( Queryable.ThenByDescending ) : nameof( Queryable.OrderByDescending );
                    break;
                default:
                    return sequence;
            }

            return InvokeEnumerable<T>( sequence, sortMethod, sortDescription.PropertyName, ordered );
        }

        /// <summary>
        /// Applies the sort operation defined by the sort description to the specified sequence.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of items to sort.</typeparam>
        /// <param name="sequence">The <see cref="IQueryable{T}"/> sequence of items to sort.</param>
        /// <param name="sortDescription">The <see cref="SortDescription"/> containing the sorting information.</param>
        /// <returns>An <see cref="IQueryable{T}"/> object.</returns>
        /// <remarks>If a property name in the <see cref="SortDescription"/> contains a property path, only the last expression in the
        /// path will be used.  Use the overloaded version of this method to override this behavior.</remarks>
        /// <example>This example illustrates how to create an sorted data service request using the provided <see cref="SortDescriptionCollection">sort descriptions</see>.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Data.Services;
        /// using System.Linq;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        ///
        /// public partial class MyUserControl : UserControl
        /// {
        ///     private readonly MyEntityContext context = new MyEntityContext();
        ///     internal DataPager DataPager;
        ///
        ///     private async void Task<PagedItemCollection<MyData>> GetMyDataAsync( PagingArguments args )
        ///     {
        ///         // build data query
        ///         var query = from data in this.context.MyDataCollection
        ///                     select data;
        ///
        ///         // apply sort description
        ///         if ( args.SortDescriptions.Any() )
        ///         {
        ///             query = query.ApplySortDescription( args.SortDescriptions.First() );
        ///         }
        ///
        ///         // make the query a paged request
        ///         query = query.Skip( args.InitialIndex ).Take( args.PageSize );
        ///
        ///         // include the total number of items that match the query
        ///         var service = ( (DataServiceQuery<MyData>) query ).IncludeTotalCount();
        ///         IEnumerable<MyData> result = null;
        ///
        ///         try
        ///         {
        ///             result = await Task<IEnumerable<MyData>>.Factory.FromAsync( service.BeginExecute, service.EndExecute );
        ///         }
        ///         catch ( Exception ex )
        ///         {
        ///             // VirtualCollectionView<T> cannot handle a paging exception so handle it here
        ///             MessageBox.Show( ex.Message ) );
        ///             return new PagedCollection<MyData>( Enumerable.Empty<MyData>(), 0L );
        ///         }
        ///
        ///         var response = result as QueryOperationResponse<MyData>;
        ///         return new PagedCollection<MyData>( response.ToArray(), response.TotalCount );
        ///     }
        ///
        ///     public MyUserControl()
        ///     {
        ///         this.InitializeComponent();
        ///         this.DataPager.ItemsSource = new VirtualCollectionView<MyData>( this.GetMyDataAsync );
        ///     }
        /// }
        /// ]]></code></example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IQueryable<T> ApplySortDescription<T>( this IQueryable<T> sequence, SortDescription sortDescription )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IQueryable<T>>() != null );

            var ordered = false;
            string sortMethod;

            switch ( sortDescription.Direction )
            {
                case ListSortDirection.Ascending:
                    if ( ordered = IsQueryableOrdered( sequence.Expression ) )
                    {
                        sortMethod = nameof( Queryable.ThenBy );
                    }
                    else
                    {
                        sortMethod = nameof( Queryable.OrderBy );
                    }

                    break;
                case ListSortDirection.Descending:
                    if ( ordered = IsQueryableOrdered( sequence.Expression ) )
                    {
                        sortMethod = nameof( Queryable.ThenByDescending );
                    }
                    else
                    {
                        sortMethod = nameof( Queryable.OrderByDescending );
                    }

                    break;
                default:
                    return sequence;
            }

            return InvokeQueryable<T>( sequence, sortMethod, sortDescription.PropertyName, ordered );
        }

        /// <summary>
        /// Applies the sort operations defined by the sort descriptions to the specified sequence.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of items to sort.</typeparam>
        /// <param name="sequence">The <see cref="IEnumerable{T}"/> sequence of items to sort.</param>
        /// <param name="sortDescriptions">The <see cref="SortDescriptionCollection"/> containing the sorting information.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> object.</returns>
        /// <remarks>If a property name in the <see cref="SortDescriptionCollection"/> contains a property path, only the last expression in the
        /// path will be used.  Use the overloaded version of this method to override this behavior.</remarks>
        /// <example>This example illustrates how to sort items using the provided <see cref="SortDescriptionCollection">sort descriptions</see>.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Data.Services;
        /// using System.Linq;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        ///
        /// public partial class MyUserControl : UserControl
        /// {
        ///     private readonly List<MyData> items = new List<MyData>();
        ///     internal DataPager DataPager;
        ///
        ///     private void Task<PagedItemCollection<MyData>> GetMyDataAsync( PagingArguments args )
        ///     {
        ///         // TODO: populate the list
        ///         var pagedItems = this.items.ApplySortDescriptions( args.SortDescriptions ).Skip( args.InitialIndex ).Take( args.PageSize ).ToArray();
        ///         return Task.FromResult( new PagedItemCollection<MyData>( pagedItems, this.items.Count ) );
        ///     }
        ///
        ///     public MyUserControl()
        ///     {
        ///         this.InitializeComponent();
        ///         this.DataPager.ItemsSource = new VirtualCollectionView<MyData>( this.GetMyDataAsync );
        ///     }
        /// }
        /// ]]></code></example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IEnumerable<T> ApplySortDescriptions<T>( this IEnumerable<T> sequence, SortDescriptionCollection sortDescriptions )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( sortDescriptions, nameof( sortDescriptions ) );
            Contract.Ensures( Contract.Result<IEnumerable<T>>() != null );

            foreach ( var sortDescription in sortDescriptions )
            {
                sequence = sequence.ApplySortDescription( sortDescription );
            }

            return sequence;
        }

        /// <summary>
        /// Applies the sort operations defined by the sort descriptions to the specified sequence.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of items to sort.</typeparam>
        /// <param name="sequence">The <see cref="IQueryable{T}"/> sequence of items to sort.</param>
        /// <param name="sortDescriptions">The <see cref="SortDescriptionCollection"/> containing the sorting information.</param>
        /// <returns>An <see cref="IQueryable{T}"/> object.</returns>
        /// <remarks>If a property name in the <see cref="SortDescriptionCollection"/> contains a property path, only the last expression in the
        /// path will be used.  Use the overloaded version of this method to override this behavior.</remarks>
        /// <example>This example illustrates how to create an sorted data service request using the provided <see cref="SortDescriptionCollection">sort descriptions</see>.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Data.Services;
        /// using System.Linq;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        ///
        /// public partial class MyUserControl : UserControl
        /// {
        ///     private readonly MyEntityContext context = new MyEntityContext();
        ///     internal DataPager DataPager;
        ///
        ///     private async void Task<PagedItemCollection<MyData>> GetMyDataAsync( PagingArguments args )
        ///     {
        ///         // build data query
        ///         var query = from data in this.context.MyDataCollection
        ///                     select data;
        ///
        ///         // make the query a paged request
        ///         query = query.ApplySortDescriptions( args.SortDescriptions ).Skip( args.InitialIndex ).Take( args.PageSize );
        ///
        ///         // include the total number of items that match the query
        ///         var service = ( (DataServiceQuery<MyData>) query ).IncludeTotalCount();
        ///         IEnumerable<MyData> result = null;
        ///
        ///         try
        ///         {
        ///             result = await Task<IEnumerable<MyData>>.Factory.FromAsync( service.BeginExecute, service.EndExecute );
        ///         }
        ///         catch ( Exception ex )
        ///         {
        ///             // VirtualCollectionView<T> cannot handle a paging exception so handle it here
        ///             MessageBox.Show( ex.Message ) );
        ///             return new PagedCollection<MyData>( Enumerable.Empty<MyData>(), 0L );
        ///         }
        ///
        ///         var response = result as QueryOperationResponse<MyData>;
        ///         return new PagedCollection<MyData>( response.ToList(), response.TotalCount );
        ///     }
        ///
        ///     public MyUserControl()
        ///     {
        ///         this.InitializeComponent();
        ///         this.DataPager.ItemsSource = new VirtualCollectionView<MyData>( this.GetMyDataAsync );
        ///     }
        /// }
        /// ]]></code></example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IQueryable<T> ApplySortDescriptions<T>( this IQueryable<T> sequence, SortDescriptionCollection sortDescriptions )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( sortDescriptions, nameof( sortDescriptions ) );
            Contract.Ensures( Contract.Result<IQueryable<T>>() != null );

            foreach ( var sortDescription in sortDescriptions )
            {
                sequence = sequence.ApplySortDescription( sortDescription );
            }

            return sequence;
        }

        /// <summary>
        /// Translates the property names in the specified sort descriptions using the provided function.
        /// </summary>
        /// <param name="sortDescriptions">The <see cref="SortDescriptionCollection"/> to translate.</param>
        /// <param name="propertyNameTranslator">The <see cref="Func{T,TResult}"/> used to translate property names.</param>
        /// <returns>A <see cref="SortDescriptionCollection"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static SortDescriptionCollection Translate( this SortDescriptionCollection sortDescriptions, Func<string, string> propertyNameTranslator )
        {
            Arg.NotNull( sortDescriptions, nameof( sortDescriptions ) );
            Arg.NotNull( propertyNameTranslator, nameof( propertyNameTranslator ) );
            Contract.Ensures( Contract.Result<SortDescriptionCollection>() != null );

            var translated = new SortDescriptionCollection();

            foreach ( var sortDescription in sortDescriptions )
            {
                var newSortDescription = new SortDescription( propertyNameTranslator( sortDescription.PropertyName ), sortDescription.Direction );
                translated.Add( newSortDescription );
            }

            return translated;
        }

        /// <summary>
        /// Translates the property names for the sort descriptions in the specified paging arguments using the provided function.
        /// </summary>
        /// <param name="pagingArgs">The <see cref="PagingArguments"/> containing the sort descriptions to translate.</param>
        /// <param name="propertyNameTranslator">The <see cref="Func{T,TResult}"/> used to translate property names.</param>
        /// <returns>A <see cref="PagingArguments"/> object.</returns>
        /// <remarks>This method is useful when property names in the <see cref="PagingArguments.SortDescriptions"/> do not have a one-to-one correlation to the
        /// properties of the objects being sorted. This most frequently occurs when with items that are adapted for presentation.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static PagingArguments Translate( this PagingArguments pagingArgs, Func<string, string> propertyNameTranslator )
        {
            Arg.NotNull( pagingArgs, nameof( pagingArgs ) );
            Arg.NotNull( propertyNameTranslator, nameof( propertyNameTranslator ) );
            Contract.Ensures( Contract.Result<PagingArguments>() != null );

            return new PagingArguments( pagingArgs.PageIndex, pagingArgs.PageSize, pagingArgs.SortDescriptions.Translate( propertyNameTranslator ) );
        }
    }
}