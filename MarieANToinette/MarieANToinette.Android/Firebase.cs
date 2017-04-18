using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using System.Threading.Tasks;

namespace MarieANToinette.Droid
{
    public class FirebaseFactory
    {
        public static Context context { get; set; }
        private static FirebaseApp firebaseApp;

        public async static Task<FirebaseApp> GetApp()
        {
            if (firebaseApp == null)
            {
                var options = new FirebaseOptions.Builder()
                .SetApplicationId("prototype-149014")
                .SetApiKey("AIzaSyAubOeJSnAisxRXjrIsGHY9bHjy3BBTrjY")
                .SetDatabaseUrl("https://prototype-149014.firebaseio.com/")
                .Build();
                firebaseApp = FirebaseApp.InitializeApp(context, options);
                await firebaseApp.GetTokenAsync(true);
            }
            return firebaseApp;
        }
    }
}