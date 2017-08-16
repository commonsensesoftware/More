namespace More.Windows.Interactivity
{
    using System;
    using System.Diagnostics.Contracts;
#if UAP10_0
    using System.Windows.Data;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows;
    using System.Windows.Data;
#endif

    internal static class DependencyPropertyExtensions
    {
        internal static void SetDataContext( this DependencyObject source, DependencyProperty property, FrameworkElement element )
        {
            Contract.Requires( source != null );
            Contract.Requires( property != null );

            // if there's no context that can be used to for updating, there's no sense in continuing
            if ( element == null || element.DataContext == null )
            {
                return;
            }

            // get the data binding expression
            var expression = source.ReadLocalValue( property ) as BindingExpression;

            // if there's no expression, then parameter isn't data bound
            if ( expression == null || expression.ParentBinding == null )
            {
                return;
            }

            var currentBinding = expression.ParentBinding;

            // skip one-time or relative source bindings
            if ( currentBinding.Mode == BindingMode.OneTime || currentBinding.RelativeSource != null )
            {
                return;
            }

            // clone current binding and set source to data context of associated object
            var binding = currentBinding.Clone();

            if ( !string.IsNullOrEmpty( binding.ElementName ) )
            {
                binding.Source = element.FindName( binding.ElementName ) ?? element.DataContext;
            }
            else
            {
                binding.Source = element.DataContext;
            }

            // update binding
            BindingOperations.SetBinding( source, property, binding );
        }
    }
}