using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Core.Infrastruct
{
    public static class EnumHelper
    {
        #region Methods

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        //In short this is generic method to get any type of attribute.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes.FirstOrDefault();//attributes.Length > 0 ? (T)attributes[0] : null;
        }

        // This method creates a specific call to the above method, requesting the
        // Display MetaData attribute.
        //e.g. [Display(Name = "Sunday")]
        public static string ToDisplayName(this Enum value)
        {
            var attribute = value.GetAttribute<DisplayAttribute>();
            return attribute == null ? value.ToString() : attribute.Name;
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        //e.g. [Description("Day of week. Sunday")]
        public static string ToDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description.Replace(" ", string.Empty) == description.Replace(" ", string.Empty))
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name.Replace(" ", string.Empty) == description.Replace(" ", string.Empty))
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }

        #endregion
    }
}
