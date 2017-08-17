namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;

    sealed class ValidationResultAdapter : IValidationResult
    {
        readonly ValidationResult adapted;

        internal ValidationResultAdapter( ValidationResult adapted )
        {
            Contract.Requires( adapted != null );
            this.adapted = adapted;
        }

        public string ErrorMessage
        {
            get => adapted.ErrorMessage;
            set => adapted.ErrorMessage = value;
        }

        public IEnumerable<string> MemberNames => adapted.MemberNames;

        public override string ToString() => ErrorMessage ?? base.ToString();
    }
}