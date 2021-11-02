using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace XamlBrewer.Models
{
    public class Settings : ObservableObject
    {
        private bool isLightTheme;

        public Settings()
        {
            // Required for serialization.
        }

        public bool IsLightTheme
        {
            get => isLightTheme;
            set => SetProperty(ref isLightTheme, value);
        }
    }
}