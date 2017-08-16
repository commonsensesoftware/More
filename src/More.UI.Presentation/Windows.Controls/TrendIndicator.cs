namespace More.Windows.Controls
{
    using More.Windows.Data;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
#if UAP10_0
    using System.Windows.Data;
    using global::Windows.Foundation;
    using global::Windows.UI;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Represents a control that indicates trends.
    /// </summary>
#if UAP10_0
    [CLSCompliant( false )]
#endif
    [TemplateVisualState( GroupName = "TrendStates", Name = "Undefined" )]
    [TemplateVisualState( GroupName = "TrendStates", Name = "Flat" )]
    [TemplateVisualState( GroupName = "TrendStates", Name = "Positive" )]
    [TemplateVisualState( GroupName = "TrendStates", Name = "Negative" )]
    public partial class TrendIndicator : Control
    {
        /// <summary>
        /// Gets the trend dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty TrendProperty =
            DependencyProperty.Register( nameof( Trend ), typeof( decimal? ), typeof( TrendIndicator ), new PropertyMetadata( null, OnTrendPropertyChanged ) );

        /// <summary>
        /// Gets the score dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ScoreProperty =
            DependencyProperty.Register( nameof( Score ), typeof( decimal? ), typeof( TrendIndicator ), new PropertyMetadata( null, OnScorePropertyChanged ) );

        const string UndefinedState = "Undefined";
        static readonly Brush DefaultScoreBrush = new SolidColorBrush( Colors.Transparent );
        static readonly IReadOnlyList<TrendRule> DefaultTrendRules = CreateDefaultTrendRules();
        static readonly IReadOnlyList<ConditionalBrushRule> DefaultScoreRules = CreateDefaultScoreRules();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrendIndicator"/> class.
        /// </summary>
        public TrendIndicator()
        {
            DefaultStyleKey = typeof( TrendIndicator );
            Loaded += ( s, e ) => EndInit();
        }

        /// <summary>
        /// Gets or sets the indicator trend.
        /// </summary>
        /// <value>The indicator trend.  A positive value indicates a trend incline, a negative value indicates a trend decline,
        /// a value of zero indicates a flat trend, and a null value is undefined.</value>
        public decimal? Trend
        {
            get => (decimal?) GetValue( TrendProperty );
            set => SetValue( TrendProperty, value );
        }

        /// <summary>
        /// Gets or sets the indicator score.
        /// </summary>
        /// <value>The indicator score.  A positive score indicates a positive correlation, a negative score indicates a negative correlation,
        /// a score of zero indicates an even correlation, and a null score indicates no correlation.</value>
        public decimal? Score
        {
            get => (decimal?) GetValue( ScoreProperty );
            set => SetValue( ScoreProperty, value );
        }

        /// <summary>
        /// Gets a collection of rules that determine the visual state of the indicator.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        public Collection<TrendRule> TrendRules { get; } = new ObservableCollection<TrendRule>();

        /// <summary>
        /// Gets a collection of rules that determine the color of the indicator.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        public Collection<ConditionalBrushRule> ScoreRules { get; } = new ObservableCollection<ConditionalBrushRule>();

        static void OnTrendPropertyChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );

            var control = (TrendIndicator) sender;

            if ( !control.IsInitialized )
            {
                return;
            }

            control.UpdateVisualState();
            control.OnTrendChanged( EventArgs.Empty );
        }

        static void OnScorePropertyChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );

            var control = (TrendIndicator) sender;

            if ( control.IsInitialized )
            {
                control.OnScoreChanged( EventArgs.Empty );
            }
        }

        private static IReadOnlyList<TrendRule> CreateDefaultTrendRules()
        {
            Contract.Ensures( Contract.Result<IReadOnlyList<TrendRule>>() != null );

            return new[]
            {
                new TrendRule()
                {
                    VisualState = "Undefined",
                    Rules =
                    {
                        new EqualToRule( null )
                    }
                },
                new TrendRule()
                {
                    VisualState = "Positive",
                    Rules =
                    {
                        new GreaterThanRule( 0 )
                    }
                },
                new TrendRule()
                {
                    VisualState = "Flat",
                    Rules =
                    {
                        new EqualToRule( 0 )
                    }
                },
                new TrendRule()
                {
                    VisualState = "Negative",
                    Rules =
                    {
                        new LessThanRule( 0 )
                    }
                }
            };
        }

        static IReadOnlyList<ConditionalBrushRule> CreateDefaultScoreRules()
        {
            Contract.Ensures( Contract.Result<IReadOnlyList<ConditionalBrushRule>>() != null );

            return new[]
            {
                new ConditionalBrushRule()
                {
                    Brush = new SolidColorBrush( Colors.Transparent ),
                    Rules =
                    {
                        new EqualToRule( null )
                    }
                },
                new ConditionalBrushRule()
                {
                    // green brush
                    Brush = new LinearGradientBrush()
                    {
                         StartPoint = new Point( 0.5, 0.0 ),
                         EndPoint = new Point( 0.5, 1.0 ),
                         GradientStops =
                         {
                             new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x33, G = 0xCC, B = 0x33 }, Offset = 0.0 },
                             new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x33, G = 0xCC, B = 0x33 }, Offset = 0.5 },
                             new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x00, G = 0x92, B = 0x44 }, Offset = 0.5 },
                             new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x33, G = 0xCC, B = 0x33 }, Offset = 1.0 }
                         }
                    },
                    Rules =
                    {
                        new GreaterThanRule( 0 )
                    }
                },
                new ConditionalBrushRule()
                {
                    // gray brush
                    Brush = new LinearGradientBrush()
                    {
                        StartPoint = new Point( 0.5, 0.0 ),
                        EndPoint = new Point( 0.5, 1.0 ),
                        GradientStops =
                        {
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x64, G = 0x64, B = 0x64 }, Offset = 0.0 },
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x64, G = 0x64, B = 0x64 }, Offset = 0.5 },
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x19, G = 0x19, B = 0x19 }, Offset = 0.5 },
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0x64, G = 0x64, B = 0x64 }, Offset = 1.0 }
                        }
                    },
                    Rules =
                    {
                        new EqualToRule( 0 )
                    }
                },
                new ConditionalBrushRule()
                {
                    // red brush
                    Brush = new LinearGradientBrush()
                    {
                        StartPoint = new Point( 0.5, 1.0 ),
                        EndPoint = new Point( 0.5, 0.0 ),
                        GradientStops =
                        {
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0xFF, G = 0x00, B = 0x00 }, Offset = 0.0 },
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0xC0, G = 0x26, B = 0x2D }, Offset = 0.5 },
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0xFF, G = 0x00, B = 0x00 }, Offset = 0.5 },
                            new GradientStop() { Color = new Color(){ A = 0xFF, R = 0xFF, G = 0x00, B = 0x00 }, Offset = 0.1 }
                        }
                    },
                    Rules =
                    {
                        new LessThanRule( 0 )
                    }
                }
            };
        }

        void UpdateVisualState( bool useTransitions = true )
        {
            IEnumerable<TrendRule> rules = TrendRules;

            if ( !rules.Any() )
            {
                rules = DefaultTrendRules;
            }

            var trend = Trend;
            var stateName = rules.Where( r => r.Evaluate( trend ) ).Select( r => r.VisualState ).FirstOrDefault();

            if ( string.IsNullOrWhiteSpace( stateName ) )
            {
                stateName = UndefinedState;
            }

            VisualStateManager.GoToState( this, stateName, useTransitions );
        }

        /// <summary>
        /// Occurs when the indicator trend has changed.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnTrendChanged( EventArgs e ) => Arg.NotNull( e, nameof( e ) );

        /// <summary>
        /// Occurs when the indicator score has changed.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnScoreChanged( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            IEnumerable<ConditionalBrushRule> rules = ScoreRules;

            if ( !rules.Any() )
            {
                rules = DefaultScoreRules;
            }

            var score = Score;
            var brush = rules.Where( r => r.Evaluate( score ) ).Select( r => r.Brush ).FirstOrDefault();

            ScoreBrush = brush ?? DefaultScoreBrush;
        }
    }
}