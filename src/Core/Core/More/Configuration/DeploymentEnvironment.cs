namespace More.Configuration
{
    using global::System;

    /// <summary>
    /// Defines the possible deployment environments.
    /// </summary>
    public enum DeploymentEnvironment
    {
        /// <summary>
        /// Indicates an unspecified deployment environment.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Indicates the development deployment environment.
        /// </summary>
        Development,

        /// <summary>
        /// Indicates the staging and integration testing deployment environment.
        /// </summary>
        StagingAndIntegration,

        /// <summary>
        /// Indicates the user acceptance testing deployment environment.
        /// </summary>
        UserAcceptance,

        /// <summary>
        /// Indicates the production deployment environment.
        /// </summary>
        Production,

        /// <summary>
        /// Indicates the preproduction deployment environment.
        /// </summary>
        Preproduction,

        /// <summary>
        /// Indicates the demonstration deployment environment.
        /// </summary>
        Demonstration,

        /// <summary>
        /// Indicates deployment environment that doesn't conform to any other well-known environment.
        /// </summary>
        Other
    }
}
