
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
namespace SkyCarsWebAPI.Infrastructure
{ 
    public static class Common
    {
        public static List<SelectListItem> DropDownBindWithEnum(Type enumType)
        {
            return enumType.ToSelectListItems();
        }
        
        /// <summary>
        /// From enum type convert to SelectListItems
        /// </summary>
        /// <param name="enumType">Type of enum</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectListItems(this Type enumType)
        {
            List<SelectListItem> items = new();
            foreach (Enum cur in Enum.GetValues(enumType))
            {
                items.Add(new SelectListItem()
                {
                    Text = cur.ToString(),
                    Value = GetEnumValue(cur)
                });
            }
            return items;
        }

        #region :: Get Enum value ::
        public static string GetEnumValue(this Enum EnumType)
        {
            return Convert.ToString((int)(object)EnumType);
        }
        #endregion

        #region  :: To Int & Decimal ::
        public static int ToInt(this object a)
        {
            try
            {
                return Convert.ToInt32(a);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal ToDecimal(this object a)
        {
            try
            {
                return Convert.ToDecimal(a);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion
        #region  :: Cache Api for Get list of All message ::
        
        public static bool GetCache(IMemoryCache cache,string cacheKey,out object resObject) 
        {
            return cache.TryGetValue(cacheKey, out resObject);
        }        
        public static void SetCache(IMemoryCache cache,string cacheKey,object resObject) 
        {
            var cacheExpirationOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(8),
                Priority = CacheItemPriority.Normal
            };
            cache.Set(cacheKey, resObject,cacheExpirationOptions);
        }        
        public static void RemoveCache(IMemoryCache cache,string cacheKey)
        {
            cache.Remove(cacheKey);  
        }
        #endregion
    }
}