namespace More.Windows.Controls
{
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;
    using System;
    using System.ComponentModel;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    public partial class TrendIndicator : ISupportInitialize
    {
        static readonly DependencyProperty ScoreBrushProperty =
            DependencyProperty.Register( nameof( ScoreBrush ), typeof( Brush ), typeof( TrendIndicator ), new PropertyMetadata( DefaultScoreBrush ) );

        /// <summary>
        /// Gets or sets the the score brush color.
        /// </summary>
        /// <value>A <see cref="Brush"/> object.</value>
        public Brush ScoreBrush
        {
            get => (Brush) GetValue( ScoreBrushProperty ) ?? DefaultScoreBrush;
            protected set => SetValue( ScoreBrushProperty, value );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control has been initialized.
        /// </summary>
        /// <value>True if the control has been initialized; otherwise, false.</value>
        protected bool IsInitialized { get; set; }

        /// <summary>
        /// Starts the initialization process for the control.
        /// </summary>
        public virtual void BeginInit() { }

        /// <summary>
        /// Indicates that the initialization process for the control is complete.
        /// </summary>
        public virtual void EndInit()
        {
            if ( IsInitialized )
            {
                return;
            }

            IsInitialized = true;

            // if the Trend property is set, trigger a property change after initialization
            if ( ReadLocalValue( TrendProperty ) != DependencyProperty.UnsetValue )
            {
                UpdateVisualState();
                OnTrendChanged( EventArgs.Empty );
            }

            // if the Score property is set, trigger a property change after initialization
            if ( ReadLocalValue( ScoreProperty ) != DependencyProperty.UnsetValue )
            {
                OnScoreChanged( EventArgs.Empty );
            }
        }

        /// <summary>
        /// Overrides the default behavior when the control template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState();
        }
    }
}