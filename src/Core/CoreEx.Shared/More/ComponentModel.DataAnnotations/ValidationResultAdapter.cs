namespace More.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel.DataAnnotations;
    using global::System.Diagnostics.Contracts;

    internal sealed class ValidationResultAdapter : IValidationResult
    {
        private readonly ValidationResult adapted;

        internal ValidationResultAdapter( ValidationResult adapted )
        {
            Contract.Requires( adapted != null );
            this.adapted = adapted;
        }

        public string ErrorMessage
        {
            get
            {
                return this.adapted.ErrorMessage;
            }
            set
            {
                this.adapted.ErrorMessage = value;
            }
        }

        public IEnumerable<string> MemberNames
        {
            get
            {
                return this.adapted.MemberNames;
            }
        }
    }
}
