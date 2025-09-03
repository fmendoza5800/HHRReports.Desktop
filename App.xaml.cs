namespace HHRReports.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            
            // Configure window properties
            window.Title = "HHR Reports Desktop";
            window.Width = 1400;
            window.Height = 900;
            window.MinimumWidth = 1024;
            window.MinimumHeight = 600;
            
            // Center the window on screen
            if (DeviceDisplay.MainDisplayInfo.Width > 0 && DeviceDisplay.MainDisplayInfo.Height > 0)
            {
                window.X = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density - window.Width) / 2;
                window.Y = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density - window.Height) / 2;
            }
            
            return window;
        }
    }
}