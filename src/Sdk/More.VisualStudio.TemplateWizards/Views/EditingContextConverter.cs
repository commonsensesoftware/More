namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using More.VisualStudio.ViewModels;
    using System;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;

    /// <summary>
    /// Represents a <see cref="IValueConverter">value converter</see> that can convert from a
    /// <see cref="AssemblyName">assembly name</see> to an <see cref="EditingContext">editing context</see>.
    /// </summary>
    public sealed class EditingContextConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value to a function.
        /// </summary>
        /// <param name="value">The <see cref="ISpecification{T}">specification</see> to convert.</param>
        /// <param name="targetType">The target <see cref="Type">type</see>; only <see cref="Func{T,TResult}"/> is supported.</param>
        /// <param name="parameter">A user-defined conversion parameter. This parameter is not used.</param>
        /// <param name="culture">The <see cref="CultureInfo">culture</see> used to perform the conversion.</param>
        /// <returns>A converted <see cref="Func{T,TResult}"/> object.</returns>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The converted object cannot be disposed." )]
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            var source = (ILocalAssemblySource) value;
            var item = new AssemblyContextControlItem()
            {
                LocalAssemblyName = source.LocalAssemblyName,
                ReferencedAssemblyNames = source.LocalAssemblyReferences.ToList()
            };
            var context = new EditingContext();

            context.Services.Publish( () => new WindowHelperService( IntPtr.Zero ) );
            context.Services.Publish( () => (IMultiTargetingSupportService) new MultiTargetingSupportService( source ) );
            context.Items.SetValue( item );

            return context;
        }

        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) => throw new NotSupportedException();
    }
}