namespace More.Windows.Interactivity
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    /// <summary>
    /// Represents a behavior which supports <see cref="SecureString">securely</see> binding a
    /// <see cref="PasswordBox.SecurePassword">password</see> to a <see cref="PasswordBox"/>.
    /// </summary>
    [TypeConstraint( typeof( PasswordBox ) )]
    public class PasswordBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// Gets the password dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached( nameof( Password ), typeof( SecureString ), typeof( PasswordBindingBehavior ), new PropertyMetadata( OnPasswordPropertyChanged ) );

        bool updatedByControl;

        /// <summary>
        /// Gets or sets the bound password.
        /// </summary>
        /// <value>A <see cref="SecureString">secure</see> password.</value>
        public SecureString Password
        {
            get => (SecureString) GetValue( PasswordProperty );
            set => SetValue( PasswordProperty, value );
        }

        static string ConvertToUnsecureString( SecureString password )
        {
            if ( password == null )
            {
                return null;
            }

            var unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode( password );
                return Marshal.PtrToStringUni( unmanagedString );
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode( unmanagedString );
            }
        }

        static void OnPasswordPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var behavior = (PasswordBindingBehavior) sender;

            if ( behavior.updatedByControl )
            {
                return;
            }

            var passwordBox = behavior.AssociatedObject;

            if ( e.NewValue == null )
            {
                passwordBox.Password = null;
            }
            else
            {
                passwordBox.Password = ConvertToUnsecureString( (SecureString) e.NewValue );
            }
        }

        void OnPasswordChanged( object sender, RoutedEventArgs e )
        {
            updatedByControl = true;
            Password = AssociatedObject.SecurePassword;
            updatedByControl = false;
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += OnPasswordChanged;
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= OnPasswordChanged;
            base.OnDetaching();
        }
    }
}