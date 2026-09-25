using System.Collections;
using System.ComponentModel;

namespace ChatApp.ViewModels
{
    internal class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private Dictionary<string, List<string>> _propertyErrors;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected ViewModelBase()
        {
            this._propertyErrors = new Dictionary<string, List<string>>();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool HasErrors
        {
            get { return this._propertyErrors.Count != 0; }
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null)
            {
                return this._propertyErrors.ContainsKey(propertyName) ?
                       this._propertyErrors[propertyName] :
                       Enumerable.Empty<List<string>>();
            }
            else
            {
                return Enumerable.Empty<List<string>>();
            }
        }

        protected void AddError(string propertyName, string errorMessage)
        {
            if (!this._propertyErrors.ContainsKey(propertyName))
                this._propertyErrors.Add(propertyName, new List<string>());

            if (!this._propertyErrors[propertyName].Contains(errorMessage))
            {
                this._propertyErrors[propertyName].Add(errorMessage);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (this._propertyErrors.ContainsKey(propertyName))
            {
                this._propertyErrors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}