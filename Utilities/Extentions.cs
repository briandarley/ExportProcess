using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace ExportProcess.Utilities
{
    public static class Extentions
    {



        public static string GetString(this SqlDataReader dr, string name)
        {
            int ordinal = dr.GetOrdinal(name);
            if (dr.IsDBNull(ordinal)) return string.Empty;
            return dr.GetString(ordinal);
        }
        public static int GetInt32(this SqlDataReader dr, string name)
        {

            int ordinal = dr.GetOrdinal(name);
            if (dr.IsDBNull(ordinal)) return 0;
            return dr.GetInt32(ordinal);
        }
        public static decimal GetDecimal(this SqlDataReader dr, string name)
        {

            int ordinal = dr.GetOrdinal(name);
            if (dr.IsDBNull(ordinal)) return 0m;
            return dr.GetDecimal(ordinal);
        }
        public static double GetDouble(this SqlDataReader dr, string name)
        {

            int ordinal = dr.GetOrdinal(name);
            return dr.GetDouble(ordinal);
        }
        public static DateTime GetDateTime(this SqlDataReader dr, string name)
        {
            int ordinal = dr.GetOrdinal(name);
            if (dr.IsDBNull(ordinal)) return DateTime.MinValue;

            return dr.GetDateTime(ordinal);
        }

        public static DateTimeOffset GetDateTimeOffset(this SqlDataReader dr, string name)
        {
            int ordinal = dr.GetOrdinal(name);
            if (dr.IsDBNull(ordinal)) return DateTimeOffset.MinValue;

            return dr.GetDateTimeOffset(ordinal);
        }


        public static float GetFloat(this SqlDataReader dr, string name)
        {

            int ordinal = dr.GetOrdinal(name);
            return dr.GetFloat(ordinal);
        }

        public static bool IsEmpty(this DateTime date)
        {
            if (date == DateTime.MinValue)
                return true;
            return false;
        }

        public static T ConvertToEnum<T>(this string value)
        {

            //Enum.Parse(typeof(WorkflowStatus), m_ddlTaskStatus.SelectedValue)
            Contract.Requires(typeof(T).IsEnum);
            Contract.Requires(value != null);
            Contract.Requires(Enum.IsDefined(typeof(T), value));
            return (T)Enum.Parse(typeof(T), value);
        }
        public static T GetValueFromDescription<T>(this string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
            // or return default(T);
        }
        public static string GetDescription<T>(this T source)
        {
            var fi = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            return source.ToString();
        }


        public static bool Between(this int current, int begin, int end)
        {
            return (current >= begin && current <= end);
        }

        public static bool Between(this DateTime current, DateTime begin, DateTime end)
        {
            return (current >= begin && current <= end);
        }
        private static string ToLiteral(this string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        } 

        public static void SaveToFile(this string fileContents, string path, string fileName, bool appendToFile)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var sw = new StreamWriter(Path.Combine(path, fileName), appendToFile);
            sw.Write(fileContents);
            sw.Flush();
            sw.Close();
        }

        public static bool IsJpg(this string fileName)
        {
            if (!Path.HasExtension(fileName)) return false;
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension)) return false;
            extension = extension.ToLower();
            extension = System.Text.RegularExpressions.Regex.Replace(extension, @"^\.", "");
            return extension == "jpg";
        }
        public static bool IsTiff(this string fileName)
        {
            if (!Path.HasExtension(fileName)) return false;
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension)) return false;
            extension = extension.ToLower();

            extension = System.Text.RegularExpressions.Regex.Replace(extension, @"^\.", "");

            return extension == "tif" || extension == "tiff";
        }
    }
}
