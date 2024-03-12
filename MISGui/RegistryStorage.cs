﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISGui
{
    public class RegistryStorage : IBasicStorage
    {
        private readonly string appName;
        private readonly string defaultStore;
        public RegistryStorage(string appName, string defaultStore)
        {
            this.appName = appName;
            this.defaultStore = defaultStore;
        }

        public RegistryStorage(App.Context context, string defaultStore) : this(context.AppName, defaultStore)
        {
        }

        public RegistryStorage With(string defaultStore)
        {
            return new RegistryStorage(appName, defaultStore);
        }

        public string Read(string key)
        {
            return Read(defaultStore, key);
        }

        private string Read(string store, string key)
        {
            object value = "";
            var appKey = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\{appName}");
            if (appKey != null)
            {
                var valueKey = appKey.OpenSubKey(store);
                if (valueKey != null)
                {
                    value = valueKey.GetValue(key);
                    valueKey.Close();
                }
                appKey.Close();
            }
            return (string)value;
        }

        public void Write(string key, string value)
        {
            Write(defaultStore, key, value);
        }

        private void Write(string store, string key, string value)
        {
            var appKey = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{appName}");
            var valueKey = appKey.CreateSubKey(store);
            valueKey.SetValue(key, value);
            valueKey.Close();
            appKey.Close();
        }
    }
}