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
    /// <see cref="P:PasswordBox.SecurePassword">password</see> to a <see cref="PasswordBox"/>.
    /// </summary>
    [TypeConstraint( typeof( PasswordBox ) )]
    public class PasswordBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// Gets the password dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached( "Password", typeof( SecureString ), typeof( PasswordBindingBehavior ), new PropertyMetadata( OnPasswordPropertyChanged ) );

        private bool updatedByControl;

        /// <summary>
        /// Gets or sets the bound password.
        /// </summary>
        /// <value>A <see cref="SecureString">secure</see> password.</value>
        public SecureString Password
        {
            get
            {
                return (SecureString) this.GetValue( PasswordProperty );
            }
            set
            {
                this.SetValue( PasswordProperty, value );
            }
        }

        private static string ConvertToUnsecureString( SecureString password )
        {
            if ( password == null )
                return null;

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

        private static void OnPasswordPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var behavior = (PasswordBindingBehavior) sender;

            if ( behavior.updatedByControl )
                return;

            var passwordBox = behavior.AssociatedObject;

            if ( e.NewValue == null )
                passwordBox.Password = null;
            else
                passwordBox.Password = ConvertToUnsecureString( (SecureString) e.NewValue );
        }

        private void OnPasswordChanged( object sender, RoutedEventArgs e )
        {
            this.updatedByControl = true;
            this.Password = this.AssociatedObject.SecurePassword;
            this.updatedByControl = false;
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PasswordChanged += this.OnPasswordChanged;
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.PasswordChanged -= this.OnPasswordChanged;
            base.OnDetaching();
        }
    }
}
