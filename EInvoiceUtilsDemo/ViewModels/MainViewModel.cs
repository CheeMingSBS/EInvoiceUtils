using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SBS.Core.EInvoiceUtilsDemo.ViewModels
{
    [INotifyPropertyChanged]
    internal partial class MainViewModel
    {
        [ObservableProperty]
        private string? selectedMode;

        public RelayCommand<ComboBox> CloseDropdownOnSelectCommand { get; set; }
        public RelayCommand<object> UpdateMainUIOnSelectCommand { get; set; }

        public MainViewModel()
        {
            this.selectedMode = "Validate Taxpayer TIN";
            this.CloseDropdownOnSelectCommand = new RelayCommand<ComboBox>(CloseDropdownOnSelect, CanCloseDropdownOnSelect);
            this.UpdateMainUIOnSelectCommand = new RelayCommand<object>(UpdateMainUIOnSelect, CanUpdateMainUIOnSelect);
        }

        public bool CanCloseDropdownOnSelect(ComboBox? comboBox)
        {
            return true;
        }

        public void CloseDropdownOnSelect(ComboBox? comboBox)
        {
            if (comboBox == null)
                return;

            comboBox.IsDropDownOpen = false;
        }

        public bool CanUpdateMainUIOnSelect(object? values)
        {
            return true;
        }

        public void UpdateMainUIOnSelect(object? values)
        {
            if (values == null)
                return;

            this.SelectedMode = ((object[])values)[0] as string;

            ToggleButton popup = ((object[])values)[1] as ToggleButton;
            if (popup != null)
                popup.IsChecked = false;
        }
    }
}
