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
using Firebase.Database;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Java.Util;
using Java.IO;
using MarieANToinette.Service;

[assembly: Xamarin.Forms.Dependency(typeof(MarieANToinette.Droid.FirebaseDatabase))]
namespace MarieANToinette.Droid
{
    public class FirebaseDataBaseEventListener<T> : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        public TaskCompletionSource<T> Promise;

        public FirebaseDataBaseEventListener()
        {
            Promise = new TaskCompletionSource<T>();
        }

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                Promise.SetResult(FirebaseAndroidDataWrapper.convertSnapshotToObject<T>(snapshot));
            } else
            {
                Promise.SetException(new Exception("Cannot retrieve data"));
            }
        }
    }

    public class TestObj
    {
        public string ID { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
    }

    public class FirebaseDatabase : IFirebaseDatabase
    {
        public bool IsConnected {get; set;}
        private Firebase.Database.FirebaseDatabase firebaseDb;

        public async Task Connect()
        {
            var firebaseApp = await FirebaseFactory.GetApp();
            firebaseDb = Firebase.Database.FirebaseDatabase.GetInstance(firebaseApp);
            IsConnected = true;
        }

        public async Task SaveData(string dataKind, string[] nodes, Object data)
        {
            if (!IsConnected)
            {
                await Connect();
            }
            await saveMap(dataKind, nodes, FirebaseAndroidDataWrapper.convertObjectToMap(data));
        }

        private async Task saveMap(string dataKind, string[] nodes, HashMap data)
        {
            var myRef = firebaseDb.GetReference(dataKind);
            foreach (var node in nodes)
            {
                myRef = myRef.Child(node);
            }
            await myRef.SetValueAsync(data);
        }

        public async Task DeleteData(string dataKind, string[] nodes)
        {
            if (!IsConnected)
            {
                await Connect();
            }
            var myRef = firebaseDb.GetReference(dataKind);
            foreach (var node in nodes)
            {
                myRef = myRef.Child(node);
            }
            await myRef.RemoveValueAsync();
        }

        public async Task<T> GetData<T>(string dataKind, string[] nodes)
        {
            if (!IsConnected)
            {
                await Connect();
            }
            var myRef = firebaseDb.GetReference(dataKind);
            foreach (var node in nodes)
            {
                myRef = myRef.Child(node);
            }
            FirebaseDataBaseEventListener<T> listener = new FirebaseDataBaseEventListener<T>();
            myRef.AddListenerForSingleValueEvent(listener);
            return await listener.Promise.Task;
        }
    }
}