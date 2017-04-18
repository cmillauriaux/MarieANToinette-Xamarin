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
using Java.Util;
using Firebase.Database;

namespace MarieANToinette.Droid
{
    public class FirebaseAndroidDataWrapper
    {
        public static HashMap convertObjectToMap(Object obj)
        {
            HashMap map = new HashMap();

            if (obj.GetType() == typeof(string))
            {
                map.Put("value", (string)obj);
                return map;
            }
            if (obj.GetType() == typeof(long))
            {
                map.Put("value", (long)obj);
                return map;
            }
            if (obj.GetType() == typeof(double))
            {
                map.Put("value", (double)obj);
                return map;
            }
            if (obj.GetType() == typeof(float))
            {
                map.Put("value", (float)obj);
                return map;
            }
            if (obj.GetType() == typeof(bool))
            {
                map.Put("value", (bool)obj);
                return map;
            }

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    map.Put(prop.Name, (string)prop.GetValue(obj, null));
                    continue;
                }
                if (prop.PropertyType == typeof(long))
                {
                    map.Put(prop.Name, (long)prop.GetValue(obj, null));
                    continue;
                }
                if (prop.PropertyType == typeof(double))
                {
                    map.Put(prop.Name, (double)prop.GetValue(obj, null));
                    continue;
                }
                if (prop.PropertyType == typeof(float))
                {
                    map.Put(prop.Name, (float)prop.GetValue(obj, null));
                    continue;
                }
                if (prop.PropertyType == typeof(bool))
                {
                    map.Put(prop.Name, (bool)prop.GetValue(obj, null));
                    continue;
                }
                if (prop.PropertyType == typeof(DateTime))
                {
                    map.Put(prop.Name, ((DateTime)prop.GetValue(obj, null)).Millisecond);
                    continue;
                }
                //map.Put(prop.Name, convertObjectToMap(prop.GetValue(obj, null)));
            }
            return map;
        }

        public static T convertSnapshotToObject<T>(DataSnapshot snapshot)
        {
            T model = Activator.CreateInstance<T>();

            if (snapshot.HasChildren)
            {
                Java.Util.IIterator ie = snapshot.Children.Iterator();

                while (ie.HasNext)
                {
                    var child = (DataSnapshot)ie.Next();
                    var prop = model.GetType().GetProperty(child.Key);
                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(model, child.Value.ToString());
                        continue;
                    }
                }
            }

            return model;
        }
    }
}