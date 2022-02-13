using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DawnVR
{
    // Modified from the unobfuscated Linux port, original class name "StringHash"
    internal static class ObfuscationTools
    {
        #region Value Getters Setters

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

        public static T GetFieldValue<T>(this object objInstance, string unobfuscatedFieldName)
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
                FieldInfo field = objInstance.GetType().GetField(fieldName, AccessTools.all);
                cachedFieldInfos.Add(unobfuscatedFieldName, field);
                return (T)field.GetValue(objInstance);
            }
        }

        public static void SetFieldValue(Type staticType, string unobfuscatedFieldName, object value)
        {
            string fieldName;
#if REMASTER
            fieldName = unobfuscatedFieldName;
#else
            fieldName = GetRealGenericName(unobfuscatedFieldName);
#endif
            FieldInfo field = staticType.GetField(fieldName, AccessTools.all);
            field.SetValue(null, value);
        }

        public static void SetFieldValue(this object objInstance, string unobfuscatedFieldName, object value)
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
                fi.SetValue(objInstance, value, null);
            else
            {
                PropertyInfo field = objInstance.GetType().GetProperty(propName, AccessTools.all);
                cachedPropInfos.Add(unobfuscatedFieldName, field);
                field.SetValue(objInstance, value, null);
            }
        }

        #endregion

        #region Method Callers

        public static void CallMethod(this MonoBehaviour mb, string unobfuscatedMethodName, params object[] parameters)
        {
            string methodName = unobfuscatedMethodName.ToMethodName();
            mb.GetType().GetMethod(methodName).Invoke(mb, parameters);
        }

        public static T CallMethod<T>(this MonoBehaviour mb, string unobfuscatedMethodName, params object[] parameters)
        {
            string methodName = unobfuscatedMethodName.ToMethodName();
            return (T)mb.GetType().GetMethod(methodName).Invoke(mb, parameters);
        }

        #endregion

        #region Math

        public static void AddToFloatField(this object objInstance, string field, float val)
            => SetFieldValue(objInstance, field, GetFieldValue<float>(objInstance, field) + val);

        public static void SubtractFromFloatField(this object objInstance, string field, float val)
            => SetFieldValue(objInstance, field, GetFieldValue<float>(objInstance, field) - val);

        public static void MultiplyFloatField(this object objInstance, string field, float val)
            => SetFieldValue(objInstance, field, GetFieldValue<float>(objInstance, field) * val);

        public static void DividendFloatField(this object objInstance, string field, float val)
            => SetFieldValue(objInstance, field, GetFieldValue<float>(objInstance, field) / val);

        public static void DivisorFloatField(this object objInstance, string field, float val)
            => SetFieldValue(objInstance, field, val / GetFieldValue<float>(objInstance, field));

        public static void SubtractFromV3Field(this object objInstance, string field, Vector3 val)
            => SetFieldValue(objInstance, field, GetFieldValue<Vector3>(objInstance, field) - val);

        #endregion

        #region StringHash

        public static Type GetRealType(string className) => assemblyCSharp.GetType(GetRealClassName(className));

        public static uint MakeTag(string szString) => HashFNV1a(kFnv1aOffsetBasis, szString);

        public static string ToMethodName(this string name) => GetRealMethodName(name);
        public static string ToGenericName(this string name) => GetRealGenericName(name);

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
            // uh yeah
            if (prefix == "_1")
            {
                if (cachedGenericObfuscatedNames.TryGetValue(szString, out string val))
                    return val;

                if (string.IsNullOrEmpty(szString))
                    return prefix + "00000000";
                string final = prefix + HashFNV1a(kFnv1aOffsetBasis, szString).ToString("X");
                cachedGenericObfuscatedNames.Add(szString, final);
                return final;
            }
            else
            {
                if (cachedClassObfuscatedNames.TryGetValue(szString, out string val))
                    return val;

                if (string.IsNullOrEmpty(szString))
                    return prefix + "00000000";
                string final = prefix + HashFNV1a(kFnv1aOffsetBasis, szString).ToString("X");
                cachedClassObfuscatedNames.Add(szString, final);
                return final;
            }
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

        #endregion

        private const uint kFnv1aPrime = 16777619U;
        private const uint kFnv1aOffsetBasis = 2166136261U;
        private static Assembly assemblyCSharp = typeof(eJoystickKey).Assembly;
        private static Dictionary<string, PropertyInfo> cachedPropInfos = new Dictionary<string, PropertyInfo>();
        private static Dictionary<string, FieldInfo> cachedFieldInfos = new Dictionary<string, FieldInfo>();
        private static Dictionary<string, string> cachedGenericObfuscatedNames = new Dictionary<string, string>();
        private static Dictionary<string, string> cachedClassObfuscatedNames = new Dictionary<string, string>();
        private static StringBuilder s_stringBuilder = new StringBuilder(512);
    }
}
