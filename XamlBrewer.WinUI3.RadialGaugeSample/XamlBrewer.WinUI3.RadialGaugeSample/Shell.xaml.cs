﻿using Microsoft.UI.Xaml;

namespace XamlBrewer.WinUI3.RadialGaugeSample
{
    public sealed partial class Shell : Window
    {
        public Shell()
        {
            Title = "XAML Brewer WinUI 3 Community Toolkit Radial Gauge Sample";

            InitializeComponent();

            (Application.Current as App).EnsureSettings();
            ApplyTheme();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = (Application.Current as App).Settings;
            settings.IsLightTheme = !settings.IsLightTheme;
            (Application.Current as App).SaveSettings();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            var settings = (Application.Current as App).Settings;
            Root.RequestedTheme = settings.IsLightTheme ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}
