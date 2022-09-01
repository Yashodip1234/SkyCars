using System;

namespace SkyCarsWebAPI.Extensions
{

    [AttributeUsage(AttributeTargets.Method)]
    public class DontValidateAttribute: Attribute
    {
    }
}