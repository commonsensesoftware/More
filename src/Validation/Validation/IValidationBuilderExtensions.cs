namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides extension methods for the <see cref="IValidationBuilder{TObject,TValue}"/> interface.
    /// </summary>
    public static class IValidationBuilderExtensions
    {
        /// <summary>
        /// Applies the <see cref="RequiredRule{T}">required rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Required<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new RequiredRule<TValue>() );
        }

        /// <summary>
        /// Applies the <see cref="RequiredRule{T}">required rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Required<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new RequiredRule<TValue>( errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="RequiredRule{T}">required rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="allowEmptyStrings">Indicates whether empty strings allowed for required values.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Required<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, bool allowEmptyStrings )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new RequiredRule<TValue>() { AllowEmptyStrings = allowEmptyStrings } );
        }

        /// <summary>
        /// Applies the <see cref="RequiredRule{T}">required rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="allowEmptyStrings">Indicates whether empty strings allowed for required values.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Required<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, bool allowEmptyStrings, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new RequiredRule<TValue>( errorMessage ) { AllowEmptyStrings = allowEmptyStrings } );
        }

        /// <summary>
        /// Applies the <see cref="StringLengthRule{T}">string length rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="maximumLength">The maximum string length allowed.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> StringLength<TObject>( this IValidationBuilder<TObject, string> builder, int maximumLength )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            Arg.GreaterThanOrEqualTo( maximumLength, 0, "maximumLength" );

            return builder.Apply( new StringLengthRule( maximumLength ) );
        }

        /// <summary>
        /// Applies the <see cref="StringLengthRule{T}">string length rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="maximumLength">The maximum string length allowed.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> StringLength<TObject>( this IValidationBuilder<TObject, string> builder, int maximumLength, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            Arg.GreaterThanOrEqualTo( maximumLength, 0, "maximumLength" );
            return builder.Apply( new StringLengthRule( maximumLength, errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="StringLengthRule{T}">string length rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimumLength">The minimum string length allowed.</param>
        /// <param name="maximumLength">The maximum string length allowed.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> StringLength<TObject>( this IValidationBuilder<TObject, string> builder, int minimumLength, int maximumLength )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            Arg.GreaterThanOrEqualTo( minimumLength, 0, "minimumLength" );
            Arg.GreaterThanOrEqualTo( maximumLength, minimumLength, "maximumLength" );
            return builder.Apply( new StringLengthRule( minimumLength, maximumLength ) );
        }

        /// <summary>
        /// Applies the <see cref="StringLengthRule{T}">string length rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimumLength">The minimum string length allowed.</param>
        /// <param name="maximumLength">The maximum string length allowed.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> StringLength<TObject>( this IValidationBuilder<TObject, string> builder, int minimumLength, int maximumLength, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            Arg.GreaterThanOrEqualTo( minimumLength, 0, "minimumLength" );
            Arg.GreaterThanOrEqualTo( maximumLength, minimumLength, "maximumLength" );
            return builder.Apply( new StringLengthRule( minimumLength, maximumLength, errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="RangeRule{T}">range rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimum">The minimum value allowed.</param>
        /// <param name="maximum">The maximum value allowed.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Range<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, TValue minimum, TValue maximum ) where TValue : struct,IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            Arg.GreaterThanOrEqualTo( maximum, minimum, "maximum" );
            return builder.Apply( new RangeRule<TValue>( minimum, maximum ) );
        }

        /// <summary>
        /// Applies the <see cref="RangeRule{T}">range rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimum">The minimum value allowed.</param>
        /// <param name="maximum">The maximum value allowed.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Range<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, TValue minimum, TValue maximum, string errorMessage ) where TValue : struct,IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            Arg.GreaterThanOrEqualTo( maximum, minimum, "maximum" );
            return builder.Apply( new RangeRule<TValue>( minimum, maximum, errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="NullableRangeRule{T}">range rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimum">The minimum value allowed.</param>
        /// <param name="maximum">The maximum value allowed.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue?> Range<TObject, TValue>( this IValidationBuilder<TObject, TValue?> builder, TValue minimum, TValue maximum ) where TValue : struct,IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue?>>() != null );
            Arg.GreaterThanOrEqualTo( maximum, minimum, "maximum" );
            return builder.Apply( new NullableRangeRule<TValue>( minimum, maximum ) );
        }

        /// <summary>
        /// Applies the <see cref="NullableRangeRule{T}">range rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimum">The minimum value allowed.</param>
        /// <param name="maximum">The maximum value allowed.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue?> Range<TObject, TValue>( this IValidationBuilder<TObject, TValue?> builder, TValue minimum, TValue maximum, string errorMessage ) where TValue : struct,IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue?>>() != null );
            Arg.GreaterThanOrEqualTo( maximum, minimum, "maximum" );
            return builder.Apply( new NullableRangeRule<TValue>( minimum, maximum, errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="RegularExpressionRule{T}">regular expression rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> RegularExpression<TObject>( this IValidationBuilder<TObject, string> builder, string pattern )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( pattern, "pattern" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new RegularExpressionRule( pattern ) );
        }

        /// <summary>
        /// Applies the <see cref="RegularExpressionRule{T}">regular expression rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> RegularExpression<TObject>( this IValidationBuilder<TObject, string> builder, string pattern, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( pattern, "pattern" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new RegularExpressionRule( pattern, errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="SizeRule">size rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimumCount">The minimum count allowed.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Size<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, int minimumCount ) where TValue : IEnumerable
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );
            return builder.Apply( new SizeRule<TValue>( minimumCount ) );
        }

        /// <summary>
        /// Applies the <see cref="SizeRule">size rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimumCount">The minimum count allowed.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Size<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, int minimumCount, string errorMessage ) where TValue : IEnumerable
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );
            return builder.Apply( new SizeRule<TValue>( minimumCount, errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="SizeRule">size rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimumCount">The minimum count allowed.</param>
        /// <param name="maximumCount">The maximum count allowed.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Size<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, int minimumCount, int maximumCount ) where TValue : IEnumerable
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );
            Arg.GreaterThanOrEqualTo( maximumCount, minimumCount, "maximumCount" );
            return builder.Apply( new SizeRule<TValue>( minimumCount, maximumCount ) );
        }

        /// <summary>
        /// Applies the <see cref="SizeRule">size rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="minimumCount">The minimum count allowed.</param>
        /// <param name="maximumCount">The maximum count allowed.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, TValue> Size<TObject, TValue>( this IValidationBuilder<TObject, TValue> builder, int minimumCount, int maximumCount, string errorMessage ) where TValue : IEnumerable
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );
            Arg.GreaterThanOrEqualTo( maximumCount, minimumCount, "maximumCount" );
            return builder.Apply( new SizeRule<TValue>( minimumCount, maximumCount, errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="CreditCardRule{T}">credit card rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> CreditCard<TObject>( this IValidationBuilder<TObject, string> builder )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new CreditCardRule() );
        }

        /// <summary>
        /// Applies the <see cref="CreditCardRule{T}">credit card rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> CreditCard<TObject>( this IValidationBuilder<TObject, string> builder, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new CreditCardRule( errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="EmailRule{T}">email rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Email<TObject>( this IValidationBuilder<TObject, string> builder )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new EmailRule() );
        }

        /// <summary>
        /// Applies the <see cref="EmailRule{T}">email rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Email<TObject>( this IValidationBuilder<TObject, string> builder, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new EmailRule( errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="PhoneRule{T}">phone rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Phone<TObject>( this IValidationBuilder<TObject, string> builder )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new PhoneRule() );
        }

        /// <summary>
        /// Applies the <see cref="PhoneRule{T}">phone rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Phone<TObject>( this IValidationBuilder<TObject, string> builder, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new PhoneRule( errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="UrlRule{T}">URL rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Url<TObject>( this IValidationBuilder<TObject, string> builder )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new UrlRule() );
        }

        /// <summary>
        /// Applies the <see cref="UrlRule{T}">URL rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Url<TObject>( this IValidationBuilder<TObject, string> builder, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new UrlRule( errorMessage ) );
        }

        /// <summary>
        /// Applies the <see cref="UrlRule{T}">URL rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="kind">The <see cref="UriKind">kind</see> of URL to evaluate.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Url<TObject>( this IValidationBuilder<TObject, string> builder, UriKind kind )
        {
            Arg.NotNull( builder, "builder" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new UrlRule( kind ) );
        }

        /// <summary>
        /// Applies the <see cref="UrlRule{T}">URL rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="kind">The <see cref="UriKind">kind</see> of URL to evaluate.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IValidationBuilder<TObject, string> Url<TObject>( this IValidationBuilder<TObject, string> builder, UriKind kind, string errorMessage )
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, string>>() != null );
            return builder.Apply( new UrlRule( kind, errorMessage ) );
        }

        /// <summary>
        /// Applies the less than <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> LessThan<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );

            var rule = new PropertyComparisonRule<TObject, TValue>(
                targetProperty,
                ( v1, v2 ) => v1.CompareTo( v2 ) < 0,
                ( p1, p2 ) => ValidationMessage.PropertyLessThan.FormatDefault( p1, p2 ) );

            return builder.Apply( rule );
        }

        /// <summary>
        /// Applies the less than <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> LessThan<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty,
            string errorMessage ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new PropertyComparisonRule<TObject, TValue>( targetProperty, ( v1, v2 ) => v1.CompareTo( v2 ) < 0, errorMessage ) );
        }

        /// <summary>
        /// Applies the less than or equal to <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> LessThanOrEqualTo<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );

            var rule = new PropertyComparisonRule<TObject, TValue>(
                targetProperty,
                ( v1, v2 ) => v1.CompareTo( v2 ) <= 0,
                ( p1, p2 ) => ValidationMessage.PropertyLessThanOrEqualTo.FormatDefault( p1, p2 ) );

            return builder.Apply( rule );
        }

        /// <summary>
        /// Applies the less than or equal to <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> LessThanOrEqualTo<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty,
            string errorMessage ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new PropertyComparisonRule<TObject, TValue>( targetProperty, ( v1, v2 ) => v1.CompareTo( v2 ) <= 0, errorMessage ) );
        }

        /// <summary>
        /// Applies the greater than <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> GreaterThan<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );

            var rule = new PropertyComparisonRule<TObject, TValue>(
                targetProperty,
                ( v1, v2 ) => v1.CompareTo( v2 ) > 0,
                ( p1, p2 ) => ValidationMessage.PropertyGreaterThan.FormatDefault( p1, p2 ) );

            return builder.Apply( rule );
        }

        /// <summary>
        /// Applies the greater than <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> GreaterThan<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty,
            string errorMessage ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new PropertyComparisonRule<TObject, TValue>( targetProperty, ( v1, v2 ) => v1.CompareTo( v2 ) > 0, errorMessage ) );
        }

        /// <summary>
        /// Applies the greater than or equal to <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> GreaterThanOrEqualTo<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );

            var rule = new PropertyComparisonRule<TObject, TValue>(
                targetProperty,
                ( v1, v2 ) => v1.CompareTo( v2 ) >= 0,
                ( p1, p2 ) => ValidationMessage.PropertyGreaterThanOrEqualTo.FormatDefault( p1, p2 ) );

            return builder.Apply( rule );
        }

        /// <summary>
        /// Applies the greater than or equal to <see cref="PropertyRule{TObject,TValue}">property rule</see> to the specified <see cref="IValidationBuilder{TObject,TValue}">builder</see>.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
        /// <param name="builder">The extended <see cref="IValidationBuilder{TObject,TValue}"/>.</param>
        /// <param name="targetProperty">The <see cref="Expression{T}">expression</see> for the target property to validate against.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IValidationBuilder<TObject, TValue> GreaterThanOrEqualTo<TObject, TValue>(
            this IValidationBuilder<TObject, TValue> builder,
            Expression<Func<TObject, TValue>> targetProperty,
            string errorMessage ) where TValue : IComparable<TValue>
        {
            Arg.NotNull( builder, "builder" );
            Arg.NotNull( targetProperty, "targetProperty" );
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return builder.Apply( new PropertyComparisonRule<TObject, TValue>( targetProperty, ( v1, v2 ) => v1.CompareTo( v2 ) >= 0, errorMessage ) );
        }
    }
}
