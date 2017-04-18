using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MarieANToinette.Model;
using MarieANToinette.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MarieANToinette
{
    public partial class App : Application
    {
        /*public static double ScreenWidth;
        public static double ScreenHeight;*/

        public App()
        {
            InitializeComponent();

            MainPage = new MarieANToinette.MainPage();
        }

        protected async override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
