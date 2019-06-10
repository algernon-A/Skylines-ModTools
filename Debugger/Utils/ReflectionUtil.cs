﻿using System;
using System.Reflection;

namespace ModTools.Utils
{
    public static class ReflectionUtil
    {
        public static FieldInfo FindField<T>(string fieldName)
            => Array.Find(typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), f => f.Name == fieldName);

        public static T GetPrivate<T>(object o, string fieldName)
        {
            FieldInfo field = FindField<T>(fieldName);
            return field != null ? (T)field.GetValue(o) : default;
        }

        public static void SetPrivate<T>(object o, string fieldName, object value)
        {
            FieldInfo field = FindField<T>(fieldName);
            field?.SetValue(o, value);
        }
    }
}