using System.Windows;
using StudentAssistant.Services; 

namespace StudentAssistant
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            ApiClient.Initialize();
        }
    }
}