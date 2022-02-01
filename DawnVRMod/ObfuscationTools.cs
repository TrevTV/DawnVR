using System.IO;
using System.Text;

namespace DawnVR
{
    // Taken from the unobfuscated Linux port, original class name "StringHash"
    internal static class ObfuscationTools
    {
        private static uint HashFNV1a(uint hashInitialValue, string szString)
        {
            if (string.IsNullOrEmpty(szString))
                return 0U;

            uint num = hashInitialValue;
            foreach (byte b in Encoding.ASCII.GetBytes(szString))
                num = (num * 16777619U ^ b);
            return num;
        }

        public static uint MakeTag(string szString)      
            => HashFNV1a(2166136261U, szString);

        public static string MakeTagString(string szString, string prefix = "__")
        {
            if (string.IsNullOrEmpty(szString))
                return prefix + "00000000";
            return prefix + HashFNV1a(2166136261U, szString).ToString("X");
        }

        public static string MakeTagStringPath(string szString, string prefix = "__")
        {
            if (string.IsNullOrEmpty(szString))
                return prefix + "00000000";

            char[] separator = new char[]
            {
                '\\',
                '/'
            };

            string[] array = szString.Split(separator);
            string text = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                text += MakeTagString(array[i].ToLowerInvariant(), "__");
                if (i < array.Length - 1)
                    text += "/";
            }
            return text;
        }

        public static string TagFilenamePath(string szString, string prefix = "__")
        {
            if (string.IsNullOrEmpty(szString))
                return prefix + "00000000";

            string text = Path.GetFileNameWithoutExtension(szString);
            text = MakeTagString(text.ToLowerInvariant(), "__");
            return Path.GetDirectoryName(szString) + "/" + text + Path.GetExtension(szString);
        }

        public static string ScrambleClassname(string className)
        {
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
        }

        public static string ScrambleMethodName(string name)
        {
            return MakeTagString(name, "_1");
        }

        public static string ScrambleGenericName(string name)
        {
            return MakeTagString(name, "_1");
        }

        private const uint kFnv1aPrime = 16777619U;
        private const uint kFnv1aOffsetBasis = 2166136261U;
        private static StringBuilder s_stringBuilder = new StringBuilder(512);
    }
}
