namespace SBS.Core.EInvoiceUtilsDemo.ViewModels
{
    internal class MainViewModel: ViewModelBase
    {
        public RelayCommand ToggleNavbarDropdownCommand { get; set; }

        public MainViewModel()
        {
            this.ToggleNavbarDropdownCommand = new RelayCommand(ToggleNavbarDropdown, CanToggleNavbarDropdown);
        }

        public bool CanToggleNavbarDropdown(object? obj)
        {
            return true;
        }

        public void ToggleNavbarDropdown(object? obj)
        {
            //PART_Popup.IsOpen = !PART_Popup.IsOpen;
        }
    }
}
