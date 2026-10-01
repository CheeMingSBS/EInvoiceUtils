using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SBS.Core.EInvoiceUtils;
using SBS.Core.EInvoiceUtils.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SBS.Core.EInvoiceUtilsDemo.ViewModels
{
    internal partial class LoginResponseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? response;

        public LoginResponseViewModel(string response)
        {
            this.Response = response;
        }
    }
}
