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
using Firebase.Auth;
using System.Threading.Tasks;
using MarieANToinette.Service;

[assembly: Xamarin.Forms.Dependency(typeof(MarieANToinette.Droid.FirebaseAuthentication))]
namespace MarieANToinette.Droid
{
    public class FirebaseAuthenticationListener : Java.Lang.Object, FirebaseAuth.IAuthStateListener
    {
        public void Dispose()
        {
        }

        public void OnAuthStateChanged(FirebaseAuth auth)
        {
            Console.WriteLine(auth);
            if (auth.CurrentUser != null)
            {
                FirebaseAuthentication.User = auth.CurrentUser;
            }
        }
    }

    public class FirebaseAuthentication : IFirebaseAuthentication
    {
        public static FirebaseUser User;

        public static async Task<FirebaseUser> GetUser()
        {
            if (User == null)
            {
                var firebaseApp = await FirebaseFactory.GetApp();
                var mAuth = Firebase.Auth.FirebaseAuth.GetInstance(firebaseApp);
                mAuth.AddAuthStateListener(new FirebaseAuthenticationListener());
                await mAuth.SignInAnonymouslyAsync();
            }
            return User;
        }

        public async Task<string> GetUserId()
        {
            var user = await GetUser();
            return user.Uid;
        }
    }
}