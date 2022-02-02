using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;

namespace DawnVR
{
    // Taken from the unobfuscated Linux port, original class name "StringHash"
    internal static class ObfuscationTools
    {
        public static T GetFieldValue<T>(object objInstance, string unobfuscatedFieldName, Type type = null)
        {
            string fieldName;
#if REMASTER
            fieldName = unobfuscatedFieldName;
#else
            fieldName = GetRealGenericName(unobfuscatedFieldName);
#endif
            if (cachedFieldInfos.TryGetValue(unobfuscatedFieldName, out FieldInfo fi))
                return (T)fi.GetValue(objInstance);
            else
            {
                FieldInfo field = type != null ? type.GetField(fieldName, AccessTools.all) : objInstance.GetType().GetField(fieldName, AccessTools.all);
                cachedFieldInfos.Add(unobfuscatedFieldName, field);
                return (T)field.GetValue(objInstance);
            }
        }

        public static void SetFieldValue(object objInstance, string unobfuscatedFieldName, object value)
        {
            string fieldName;
#if REMASTER
            fieldName = unobfuscatedFieldName;
#else
            fieldName = GetRealGenericName(unobfuscatedFieldName);
#endif
            if (cachedFieldInfos.TryGetValue(unobfuscatedFieldName, out FieldInfo fi))
                fi.SetValue(objInstance, value);
            else
            {
                FieldInfo field = objInstance.GetType().GetField(fieldName, AccessTools.all);
                cachedFieldInfos.Add(unobfuscatedFieldName, field);
                field.SetValue(objInstance, value);
            }
        }

        public static T GetPropertyValue<T>(object objInstance, string unobfuscatedPropName, Type type = null)
        {
            string propName;
#if REMASTER
            propName = unobfuscatedPropName;
#else
            propName = GetRealGenericName(unobfuscatedPropName);
#endif
            if (cachedPropInfos.TryGetValue(unobfuscatedPropName, out PropertyInfo fi))
                return (T)fi.GetValue(objInstance, null);
            else
            {
                PropertyInfo prop = type != null ? type.GetProperty(propName, AccessTools.all) : objInstance.GetType().GetProperty(propName, AccessTools.all);
                cachedPropInfos.Add(unobfuscatedPropName, prop);
                return (T)prop.GetValue(objInstance, null);
            }
        }

        public static void SetPropertyValue(object objInstance, string unobfuscatedFieldName, object value)
        {
            string propName;
#if REMASTER
            propName = unobfuscatedFieldName;
#else
            propName = GetRealGenericName(unobfuscatedFieldName);
#endif
            if (cachedPropInfos.TryGetValue(unobfuscatedFieldName, out PropertyInfo fi))
                fi.SetValue(objInstance, value);
            else
            {
                PropertyInfo field = objInstance.GetType().GetProperty(propName, AccessTools.all);
                cachedPropInfos.Add(unobfuscatedFieldName, field);
                field.SetValue(objInstance, value);
            }
        }

        public static Type GetRealType(string className) => assemblyCSharp.GetType(GetRealClassName(className));

        public static uint MakeTag(string szString) => HashFNV1a(kFnv1aOffsetBasis, szString);

        public static string GetRealMethodName(string name)
        {
#if REMASTER
            return name;
#else
            return MakeTagString(name, "_1");
#endif
        }

        public static string GetRealGenericName(string name)
        {
#if REMASTER
            return name;
#else
            return MakeTagString(name, "_1");
#endif
        }

        public static string GetRealClassName(string className)
        {
#if REMASTER
            return className;
#else
            s_stringBuilder.Length = 0;
            string[] array = className.Split('.');
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i].StartsWith("_1"))
                    s_stringBuilder.Append(array[i]);
                else
                    s_stringBuilder.Append(MakeTagString(array[i], "_1"));
                s_stringBuilder.Append('.');
            }

            if (array[array.Length - 1].StartsWith("T_"))
                s_stringBuilder.Append(array[array.Length - 1]);
            else
                s_stringBuilder.Append(MakeTagString(array[array.Length - 1], "T_"));

            return s_stringBuilder.ToString();
#endif
        }

        public static string MakeTagString(string szString, string prefix = "__")
        {
            if (string.IsNullOrEmpty(szString))
                return prefix + "00000000";
            return prefix + HashFNV1a(kFnv1aOffsetBasis, szString).ToString("X");
        }

        private static uint HashFNV1a(uint hashInitialValue, string szString)
        {
            if (string.IsNullOrEmpty(szString))
                return 0U;

            uint num = hashInitialValue;
            foreach (byte b in Encoding.ASCII.GetBytes(szString))
                num = (num * kFnv1aPrime ^ b);
            return num;
        }

        private const uint kFnv1aPrime = 16777619U;
        private const uint kFnv1aOffsetBasis = 2166136261U;
        private static Assembly assemblyCSharp = typeof(eJoystickKey).Assembly;
        private static Dictionary<string, PropertyInfo> cachedPropInfos = new Dictionary<string, PropertyInfo>();
        private static Dictionary<string, FieldInfo> cachedFieldInfos = new Dictionary<string, FieldInfo>();
        private static StringBuilder s_stringBuilder = new StringBuilder(512);
    }
}
