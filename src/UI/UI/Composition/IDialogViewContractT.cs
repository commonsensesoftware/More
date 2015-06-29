namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IDialogView<> ) )]
    internal abstract class IDialogViewContract<T> : IDialogView<T> where T : class
    {
        bool IDialogView<T>.IsActive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool? IDialogView<T>.DialogResult
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IDialogView<T>.Activate()
        {
            throw new NotImplementedException();
        }

        void IDialogView<T>.Close()
        {
            throw new NotImplementedException();
        }

        void IDialogView<T>.Close( bool? dialogResult )
        {
            throw new NotImplementedException();
        }

        void IDialogView<T>.Show()
        {
            throw new NotImplementedException();
        }

        Task<bool?> IDialogView<T>.ShowDialogAsync()
        {
            Contract.Ensures( Contract.Result<Task<bool?>>() != null );
            return null;
        }

        void IView<T, T>.AttachModel( T model )
        {
            throw new NotImplementedException();
        }

        T IView<T>.Model
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
