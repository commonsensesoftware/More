namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using global::Windows.Foundation;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;

    /// <summary>
    /// Represents a control that applies a layout transformation to its Content.
    /// </summary>
    [CLSCompliant( false )]
    [TemplatePart( Name = TransformRootName, Type = typeof( Grid ) )]
    [TemplatePart( Name = PresenterName, Type = typeof( ContentPresenter ) )]
    public partial class LayoutTransformer : ContentControl
    {
        /// <summary>
        /// Gets the layout transform dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty LayoutTransformProperty =
            DependencyProperty.Register( nameof( LayoutTransform ), typeof( Transform ), typeof( LayoutTransformer ), new PropertyMetadata( null, OnLayoutTransformChanged ) );

        const string TransformRootName = "TransformRoot";
        const string PresenterName = "Presenter";
        const int DecimalsAfterRound = 4;

        Panel transformRoot;
        ContentPresenter contentPresenter;
        MatrixTransform renderTransform;
        Matrix transformation;
        Size childActualSize = Size.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutTransformer"/> class.
        /// </summary>
        public LayoutTransformer()
        {
            DefaultStyleKey = typeof( LayoutTransformer );
            IsTabStop = false;
        }

        /// <summary>
        /// Gets or sets the layout transform to apply on the control content.
        /// </summary>
        /// <value>The layout <see cref="Transform">tranform</see>.</value>
        public Transform LayoutTransform
        {
            get => (Transform) GetValue( LayoutTransformProperty );
            set => SetValue( LayoutTransformProperty, value );
        }

        FrameworkElement Child => contentPresenter == null ? null : ( contentPresenter.Content as FrameworkElement ?? contentPresenter );

        static void OnLayoutTransformChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) => ( (LayoutTransformer) sender ).ProcessTransform( (Transform) e.NewValue );

        /// <summary>
        /// Overrides the default behavior when the control template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            transformRoot = GetTemplateChild( TransformRootName ) as Grid;
            contentPresenter = GetTemplateChild( PresenterName ) as ContentPresenter;
            renderTransform = new MatrixTransform();

            if ( transformRoot != null )
            {
                transformRoot.RenderTransform = renderTransform;
            }

            ApplyLayoutTransform();
        }

        /// <summary>
        /// Applies the layout transform on the control content.
        /// </summary>
        /// <remarks>This method should only used in advanced scenarios (like animating the LayoutTransform). 
        /// This method should be used to notify the control that some aspect of its
        /// <see cref="P:LayoutTransform"/> property has changed. 
        /// </remarks>
        public void ApplyLayoutTransform() => ProcessTransform( LayoutTransform );

        void ProcessTransform( Transform transform )
        {
            transformation = RoundMatrix( GetTransformMatrix( transform ) );

            if ( renderTransform != null )
            {
                renderTransform.Matrix = transformation;
            }

            InvalidateMeasure();
        }

        Matrix GetTransformMatrix( Transform transform )
        {
            if ( transform == null )
            {
                return Matrix.Identity;
            }

            // NOTE: WPF equivalent of this entire method:
            // return transform.Value;

            if ( transform is TransformGroup transformGroup )
            {
                var groupMatrix = Matrix.Identity;

                foreach ( Transform child in transformGroup.Children )
                {
                    groupMatrix = MatrixMultiply( groupMatrix, GetTransformMatrix( child ) );
                }

                return groupMatrix;
            }

            if ( transform is RotateTransform rotateTransform )
            {
                var angle = rotateTransform.Angle;
                var angleRadians = ( 2d * Math.PI * angle ) / 360d;
                var sine = Math.Sin( angleRadians );
                var cosine = Math.Cos( angleRadians );
                return new Matrix( cosine, sine, -sine, cosine, 0d, 0d );
            }

            if ( transform is ScaleTransform scaleTransform )
            {
                var scaleX = scaleTransform.ScaleX;
                var scaleY = scaleTransform.ScaleY;
                return new Matrix( scaleX, 0d, 0d, scaleY, 0d, 0d );
            }

            if ( transform is SkewTransform skewTransform )
            {
                var angleX = skewTransform.AngleX;
                var angleY = skewTransform.AngleY;
                var angleXRadians = ( 2d * Math.PI * angleX ) / 360d;
                var angleYRadians = ( 2d * Math.PI * angleY ) / 360d;
                return new Matrix( 1d, angleYRadians, angleXRadians, 1d, 0d, 0d );
            }

            if ( transform is MatrixTransform matrixTransform )
            {
                return matrixTransform.Matrix;
            }

            // NOTE: TranslateTransform has no effect in LayoutTransform
            return Matrix.Identity;
        }

        [SuppressMessage( "Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Closely corresponds to WPF's FrameworkElement.FindMaximalAreaLocalSpaceRect." )]
        Size ComputeLargestTransformedSize( Size arrangeBounds )
        {
            // compute the largest usable size (greatest area) after applying the transformation to the specified bounds.
            var computedSize = Size.Empty;

            // detect infinite bounds and constrain the scenario
            var infiniteWidth = double.IsInfinity( arrangeBounds.Width );

            if ( infiniteWidth )
            {
                arrangeBounds.Width = arrangeBounds.Height;
            }

            var infiniteHeight = double.IsInfinity( arrangeBounds.Height );

            if ( infiniteHeight )
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }

            // capture the matrix parameters
            var a = transformation.M11;
            var b = transformation.M12;
            var c = transformation.M21;
            var d = transformation.M22;

            // compute maximum possible transformed width/height based on starting width/height
            // these constraints define two lines in the positive x/y quadrant
            var maxWidthFromWidth = Math.Abs( arrangeBounds.Width / a );
            var maxHeightFromWidth = Math.Abs( arrangeBounds.Width / c );
            var maxWidthFromHeight = Math.Abs( arrangeBounds.Height / b );
            var maxHeightFromHeight = Math.Abs( arrangeBounds.Height / d );

            // the transformed width/height that maximize the area under each segment is its midpoint
            // at most one of the two midpoints will satisfy both constraints
            var idealWidthFromWidth = maxWidthFromWidth / 2d;
            var idealHeightFromWidth = maxHeightFromWidth / 2d;
            var idealWidthFromHeight = maxWidthFromHeight / 2d;
            var idealHeightFromHeight = maxHeightFromHeight / 2d;

            // compute slope of both constraint lines
            var slopeFromWidth = -( maxHeightFromWidth / maxWidthFromWidth );
            var slopeFromHeight = -( maxHeightFromHeight / maxWidthFromHeight );

            if ( arrangeBounds.Width == 0d || arrangeBounds.Height == 0d )
            {
                // check for empty bounds
                computedSize = new Size( arrangeBounds.Width, arrangeBounds.Height );
            }
            else if ( infiniteWidth && infiniteHeight )
            {
                // check for completely unbound scenario
                computedSize = new Size( double.PositiveInfinity, double.PositiveInfinity );
            }
            else if ( !MatrixHasInverse( transformation ) )
            {
                // check for singular matrix
                computedSize = new Size( 0d, 0d );
            }
            else if ( b == 0d || c == 0d )
            {
                // check for 0/180 degree special cases
                var maxHeight = infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight;
                var maxWidth = infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth;

                if ( b == 0d && c == 0d )
                {
                    // no constraints
                    computedSize = new Size( maxWidth, maxHeight );
                }
                else if ( b == 0d )
                {
                    // constrained by width
                    var computedHeight = Math.Min( idealHeightFromWidth, maxHeight );
                    computedSize = new Size( maxWidth - Math.Abs( ( c * computedHeight ) / a ), computedHeight );
                }
                else if ( c == 0d )
                {
                    // constrained by height
                    var computedWidth = Math.Min( idealWidthFromHeight, maxWidth );
                    computedSize = new Size( computedWidth, maxHeight - Math.Abs( ( b * computedWidth ) / d ) );
                }
            }
            else if ( a == 0d || d == 0d )
            {
                // check for 90/270 degree special cases
                var maxWidth = infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight;
                var maxHeight = infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth;

                if ( a == 0d && d == 0d )
                {
                    // no constraints
                    computedSize = new Size( maxWidth, maxHeight );
                }
                else if ( a == 0d )
                {
                    // constrained by width
                    var computedHeight = Math.Min( idealHeightFromHeight, maxHeight );
                    computedSize = new Size( maxWidth - Math.Abs( ( d * computedHeight ) / b ), computedHeight );
                }
                else if ( d == 0d )
                {
                    // constrained by height
                    var computedWidth = Math.Min( idealWidthFromWidth, maxWidth );
                    computedSize = new Size( computedWidth, maxHeight - Math.Abs( ( a * computedWidth ) / c ) );
                }
            }
            else if ( idealHeightFromWidth <= ( ( slopeFromHeight * idealWidthFromWidth ) + maxHeightFromHeight ) )
            {
                // check the width midpoint for viability (by being below the height constraint line)
                computedSize = new Size( idealWidthFromWidth, idealHeightFromWidth );
            }
            else if ( idealHeightFromHeight <= ( ( slopeFromWidth * idealWidthFromHeight ) + maxHeightFromWidth ) )
            {
                // check the height midpoint for viability (by being below the width constraint line)
                computedSize = new Size( idealWidthFromHeight, idealHeightFromHeight );
            }
            else
            {
                // neither midpoint is viable; use the intersection of the two constraint lines instead
                // compute width by setting heights equal (m1*x+c1=m2*x+c2)
                var computedWidth = ( maxHeightFromHeight - maxHeightFromWidth ) / ( slopeFromWidth - slopeFromHeight );

                // compute height from width constraint line (y=m*x+c; using height would give same result)
                computedSize = new Size( computedWidth, ( slopeFromWidth * computedWidth ) + maxHeightFromWidth );
            }

            return computedSize;
        }

        static Matrix RoundMatrix( Matrix matrix )
        {
            // rounds the non-offset elements of a Matrix to avoid issues due to floating point imprecision.
            return new Matrix(
                Math.Round( matrix.M11, DecimalsAfterRound ),
                Math.Round( matrix.M12, DecimalsAfterRound ),
                Math.Round( matrix.M21, DecimalsAfterRound ),
                Math.Round( matrix.M22, DecimalsAfterRound ),
                matrix.OffsetX,
                matrix.OffsetY );
        }

        static Rect RectTransform( Rect rect, Matrix matrix )
        {
            // WPF equivalent of following code:
            // Rect rectTransformed = Rect.Transform(rect, matrix);
            var leftTop = matrix.Transform( new Point( rect.Left, rect.Top ) );
            var rightTop = matrix.Transform( new Point( rect.Right, rect.Top ) );
            var leftBottom = matrix.Transform( new Point( rect.Left, rect.Bottom ) );
            var rightBottom = matrix.Transform( new Point( rect.Right, rect.Bottom ) );
            var left = Math.Min( Math.Min( leftTop.X, rightTop.X ), Math.Min( leftBottom.X, rightBottom.X ) );
            var top = Math.Min( Math.Min( leftTop.Y, rightTop.Y ), Math.Min( leftBottom.Y, rightBottom.Y ) );
            var right = Math.Max( Math.Max( leftTop.X, rightTop.X ), Math.Max( leftBottom.X, rightBottom.X ) );
            var bottom = Math.Max( Math.Max( leftTop.Y, rightTop.Y ), Math.Max( leftBottom.Y, rightBottom.Y ) );
            var rectTransformed = new Rect( left, top, right - left, bottom - top );
            return rectTransformed;
        }

        static Matrix MatrixMultiply( Matrix matrix1, Matrix matrix2 )
        {
            // WPF equivalent of following code:
            // return Matrix.Multiply(matrix1, matrix2);
            return new Matrix(
                ( matrix1.M11 * matrix2.M11 ) + ( matrix1.M12 * matrix2.M21 ),
                ( matrix1.M11 * matrix2.M12 ) + ( matrix1.M12 * matrix2.M22 ),
                ( matrix1.M21 * matrix2.M11 ) + ( matrix1.M22 * matrix2.M21 ),
                ( matrix1.M21 * matrix2.M12 ) + ( matrix1.M22 * matrix2.M22 ),
                ( ( matrix1.OffsetX * matrix2.M11 ) + ( matrix1.OffsetY * matrix2.M21 ) ) + matrix2.OffsetX,
                ( ( matrix1.OffsetX * matrix2.M12 ) + ( matrix1.OffsetY * matrix2.M22 ) ) + matrix2.OffsetY );
        }

        /// <summary>
        /// WPF equivalent of following code:  return matrix.HasInverse;
        /// </summary>
        static bool MatrixHasInverse( Matrix matrix ) => ( ( matrix.M11 * matrix.M22 ) - ( matrix.M12 * matrix.M21 ) ) != 0d;

        /// <summary>
        /// Provides the behavior for the measure pass of the layout.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride( Size availableSize )
        {
            var child = Child;

            if ( transformRoot == null || child == null )
            {
                return Size.Empty;
            }

            Size measureSize;

            if ( childActualSize == Size.Empty )
            {
                // determine the largest size after the transformation
                measureSize = ComputeLargestTransformedSize( availableSize );
            }
            else
            {
                // previous measure/arrange pass determined that Child.DesiredSize was larger than believed
                measureSize = childActualSize;
            }

            // perform a mesaure on the _transformRoot (containing Child)
            transformRoot.Measure( measureSize );

            // WPF equivalent of _childActualSize technique
            // If the child is going to render larger than the available size, re-measure according to that size: child.Arrange(new Rect());
            // if (child.RenderSize != child.DesiredSize)
            // {
            //     transformRoot.Measure(child.RenderSize);
            // }

            // transform DesiredSize to find its width/height
            var transformedDesiredRect = RectTransform( new Rect( 0d, 0d, transformRoot.DesiredSize.Width, transformRoot.DesiredSize.Height ), transformation );
            var transformedDesiredSize = new Size( transformedDesiredRect.Width, transformedDesiredRect.Height );
            return transformedDesiredSize;
        }

        /// <summary>
        /// Provides the behavior for the arrange pass of the layout.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride( Size finalSize )
        {
            var child = Child;

            if ( transformRoot == null || child == null )
            {
                return finalSize;
            }

            // determine the largest available size after the transformation
            var finalSizeTransformed = ComputeLargestTransformedSize( finalSize );

            if ( finalSizeTransformed.Width < transformRoot.DesiredSize.Width || finalSizeTransformed.Height < transformRoot.DesiredSize.Height )
            {
                // some elements do not like being given less space than they asked for (ex: TextBlock)
                // bump the working size up to do the right thing by them
                finalSizeTransformed = transformRoot.DesiredSize;
            }

            // transform the working size to find its width/height and create the arrange rect to center the transformed content
            var transformedRect = RectTransform( new Rect( 0d, 0d, finalSizeTransformed.Width, finalSizeTransformed.Height ), transformation );
            var finalRect = new Rect(
                -transformedRect.Left + ( ( finalSize.Width - transformedRect.Width ) / 2d ),
                -transformedRect.Top + ( ( finalSize.Height - transformedRect.Height ) / 2d ),
                finalSizeTransformed.Width,
                finalSizeTransformed.Height );

            // perform an Arrange on _transformRoot (containing Child)
            transformRoot.Arrange( finalRect );

            // this is the first opportunity to find out the Child's true DesiredSize
            if ( ( finalSizeTransformed.Width < child.RenderSize.Width || finalSizeTransformed.Height < child.RenderSize.Height ) && ( childActualSize == Size.Empty ) )
            {
                // unfortunately, all the work so far is invalid because the wrong DesiredSize was used; make a note of the actual DesiredSize
                childActualSize = new Size( child.ActualWidth, child.ActualHeight );

                // force a new measure/arrange pass
                InvalidateMeasure();
            }
            else
            {
                // clear the "need to measure/arrange again" flag
                childActualSize = Size.Empty;
            }

            return finalSize;
        }
    }
}