namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IDialogView<> ) )]
    abstract class IDialogViewContract<T> : IDialogView<T> where T : class
    {
        bool IDialogView<T>.IsActive => default( bool );

        bool? IDialogView<T>.DialogResult => null;

        bool IDialogView<T>.Activate() => default( bool );

        void IDialogView<T>.Close() { }

        void IDialogView<T>.Close( bool? dialogResult ) { }

        void IDialogView<T>.Show() { }

        Task<bool?> IDialogView<T>.ShowDialogAsync()
        {
            Contract.Ensures( Contract.Result<Task<bool?>>() != null );
            return null;
        }

        void IView<T, T>.AttachModel( T model ) { }

        T IView<T>.Model => default( T );
    }
}