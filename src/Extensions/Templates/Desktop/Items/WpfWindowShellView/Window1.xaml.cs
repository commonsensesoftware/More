namespace $rootnamespace$
{
    using $viewmodelnamespace$;
    using More;
    using More.Composition;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents a <see cref="Window">window</see>-based shell view.
    /// </summary>
    public partial class $safeitemname$ : DialogView<$viewmodel$>, IShellView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemname$"/> class.
        /// </summary>
        public $safeitemname$()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemname$"/> class.
        /// </summary>
        /// <param name="model">The <see cref="$viewmodel$">view model</see> associated with the view.</param>
        public $safeitemname$( $viewmodel$ model )
        {
            Contract.Requires( model != null );
            this.AttachModel( model );
        }

        /// <summary>
        /// Shows the view as the main window.
        /// </summary>
        new public virtual void Show()
        {
            Application.Current.MainWindow = this;
        }

        string IShellView.Language
        {
            get
            {
                if ( this.Language == null )
                    return null;

                return this.Language.IetfLanguageTag;
            }
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                    this.Language = null;
                else
                    this.Language = System.Windows.Markup.XmlLanguage.GetLanguage( value );
            }
        }

        string IShellView.FlowDirection
        {
            get
            {
                return this.FlowDirection.ToString();
            }
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                    this.FlowDirection = new FlowDirection();
                else
                    this.FlowDirection = (FlowDirection) Enum.Parse( typeof( FlowDirection ), value, false );
            }
        }
    }
}