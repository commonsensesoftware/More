namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Globalization;

    /// <summary>
    /// Provides a general-purpose attribute that lets you specify localizable strings for types and members of entity partial classes.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.DisplayAttribute.</remarks>
    [AttributeUsage( AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false )]
    public sealed class DisplayAttribute : Attribute
    {
        private readonly LocalizableString shortName = new LocalizableString( "ShortName" );
        private readonly LocalizableString name = new LocalizableString( "Name" );
        private readonly LocalizableString description = new LocalizableString( "Description" );
        private readonly LocalizableString prompt = new LocalizableString( "Prompt" );
        private readonly LocalizableString groupName = new LocalizableString( "GroupName" );
        private Type resourceType;
        private bool? autoGenerateField;
        private bool? autoGenerateFilter;
        private int? order;

        /// <summary>
        /// Gets or sets a value that is used for the grid column label.
        /// </summary>
        /// <value>A value that is for the grid column label.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string ShortName
        {
            get
            {
                return shortName.Value;
            }
            set
            {
                shortName.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that is used for display in the UI.
        /// </summary>
        /// <value>A value that is used for display in the UI.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string Name
        {
            get
            {
                return name.Value;
            }
            set
            {
                name.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that is used to display a description in the UI.
        /// </summary>
        /// <value>The value that is used to display a description in the UI.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string Description
        {
            get
            {
                return description.Value;
            }
            set
            {
                description.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that will be used to set the watermark for prompts in the UI.
        /// </summary>
        /// <value>A value that will be used to display a watermark in the UI.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string Prompt
        {
            get
            {
                return prompt.Value;
            }
            set
            {
                prompt.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that is used to group fields in the UI.
        /// </summary>
        /// <value>A value that is used to group fields in the UI.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string GroupName
        {
            get
            {
                return groupName.Value;
            }
            set
            {
                groupName.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the type that contains the resources for the <see cref="P:ShortName" />, <see cref="P:Name" />,
        /// <see cref="P:Prompt" />, and <see cref="P:Description" /> properties.</summary>
        /// <value>The type of the resource that contains the <see cref="P:ShortName" />, <see cref="P:Name" />,
        /// <see cref="P:Prompt" />, and <see cref="P:Description" /> properties.</value>
        public Type ResourceType
        {
            get
            {
                return resourceType;
            }
            set
            {
                if ( resourceType == value )
                    return;

                resourceType = value;
                shortName.ResourceType = value;
                name.ResourceType = value;
                description.ResourceType = value;
                prompt.ResourceType = value;
                groupName.ResourceType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UI should be generated automatically in order to display this field.
        /// </summary>
        /// <value>True if UI should be generated automatically to display this field; otherwise, false.</value>
        /// <exception cref="T:System.InvalidOperationException">An attempt was made to get the property value before it was set.</exception>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public bool AutoGenerateField
        {
            get
            {
                if ( !autoGenerateField.HasValue )
                {
                    var message = DataAnnotationsResources.PropertyNotSet.FormatDefault( "AutoGenerateField", "GetAutoGenerateField" );
                    throw new InvalidOperationException( message );
                }

                return autoGenerateField.Value;
            }
            set
            {
                autoGenerateField = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UI should be generated automatically in order to display filtering for this field.
        /// </summary>
        /// <value>True if UI should be generated automatically to display filtering for this field; otherwise, false.</value>
        /// <exception cref="InvalidOperationException">An attempt was made to get the property value before it was set.</exception>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public bool AutoGenerateFilter
        {
            get
            {
                if ( !autoGenerateFilter.HasValue )
                {
                    var message = DataAnnotationsResources.PropertyNotSet.FormatDefault( "AutoGenerateFilter", "GetAutoGenerateFilter" );
                    throw new InvalidOperationException( message );
                }

                return autoGenerateFilter.Value;
            }
            set
            {
                autoGenerateFilter = value;
            }
        }

        /// <summary>
        /// Gets or sets the order weight of the column.
        /// </summary>
        /// <value>The order weight of the column.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public int Order
        {
            get
            {
                if ( !order.HasValue )
                {
                    var message = DataAnnotationsResources.PropertyNotSet.FormatDefault( "Order", "GetOrder" );
                    throw new InvalidOperationException( message );
                }

                return order.Value;
            }
            set
            {
                order = value;
            }
        }

        /// <summary>
        /// Returns the value of the <see cref="P:ShortName" /> property.
        /// </summary>
        /// <returns>The localized string for the <see cref="P:ShortName" /> property if the <see cref="P:ResourceType" />
        /// property has been specified and if the <see cref="P:ShortName" /> property represents a resource key; otherwise,
        /// the non-localized value of the <see cref="P:ShortName" /> value property.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string GetShortName()
        {
            return shortName.GetLocalizableValue() ?? GetName();
        }

        /// <summary>
        /// Returns a value that is used for field display in the UI.
        /// </summary>
        /// <returns>The localized string for the <see cref="P:Name" /> property, if the <see cref="P:ResourceType" /> property
        /// has been specified and the <see cref="P:Name" /> property represents a resource key; otherwise, the non-localized
        /// value of the <see cref="P:Name" /> property.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="P:ResourceType" /> property and the <see cref="P:Name" />
        /// property are initialized, but a public static property that has a name that matches the <see cref="P:Name" /> value
        /// could not be found for the <see cref="P:ResourceType" /> property.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string GetName()
        {
            return name.GetLocalizableValue();
        }

        /// <summary>
        /// Returns the value of the <see cref="P:Description" /> property.
        /// </summary>
        /// <returns>The localized description, if the <see cref="P:ResourceType" /> has been specified and the
        /// <see cref="P:Description" /> property represents a resource key; otherwise, the non-localized value of the
        /// <see cref="P:Description" /> property.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="P:ResourceType" /> property and the
        /// <see cref="P:Description" /> property are initialized, but a public static property that has a name that
        /// matches the <see cref="P:Description" /> value could not be found for the <see cref="P:ResourceType" /> property.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string GetDescription()
        {
            return description.GetLocalizableValue();
        }

        /// <summary>
        /// Returns the value of the <see cref="P:Prompt" /> property.
        /// </summary>
        /// <returns>Gets the localized string for the <see cref="P:Prompt" /> property if the <see cref="P:ResourceType" /> property
        /// has been specified and if the <see cref="P:Prompt" /> property represents a resource key; otherwise, the non-localized
        /// value of the <see cref="P:Prompt" /> property.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string GetPrompt()
        {
            return prompt.GetLocalizableValue();
        }

        /// <summary>
        /// Returns the value of the <see cref="P:GroupName" /> property.
        /// </summary>
        /// <returns>A value that will be used for grouping fields in the UI, if <see cref="P:GroupName" /> has been initialized;
        /// otherwise, null. If the <see cref="P:ResourceType" /> property has been specified and the <see cref="P:GroupName" />
        /// property represents a resource key, a localized string is returned; otherwise, a non-localized string is returned.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public string GetGroupName()
        {
            return groupName.GetLocalizableValue();
        }

        /// <summary>
        /// Returns the value of the <see cref="P:AutoGenerateField" /> property.
        /// </summary>
        /// <returns>The value of <see cref="P:AutoGenerateField" /> if the property has been initialized; otherwise, null.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public bool? GetAutoGenerateField()
        {
            return autoGenerateField;
        }

        /// <summary>
        /// Returns a value that indicates whether UI should be generated automatically in order to display filtering for this field.
        /// </summary>
        /// <returns>The value of <see cref="P:AutoGenerateFilter" /> if the property has been initialized; otherwise, null.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public bool? GetAutoGenerateFilter()
        {
            return autoGenerateFilter;
        }

        /// <summary>
        /// Returns the value of the <see cref="P:Order" /> property.
        /// </summary>
        /// <returns>The value of the <see cref="P:Order" /> property, if it has been set; otherwise, null.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Maintains consistency with .NET 4.0 implementation." )]
        public int? GetOrder()
        {
            return order;
        }
    }
}
