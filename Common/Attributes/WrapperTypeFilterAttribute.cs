using System;
using Microsoft.AspNetCore.Mvc;

namespace Common.Attributes
{
    /// <summary>
    /// Used to get ImplementationType of TypeFilterAttribute
    /// </summary>
    public class WrapperTypeFilterAttribute : TypeFilterAttribute
    {
        public Type TypeInfo { get; }

        public WrapperTypeFilterAttribute(Type type) : base(type)
        {
            TypeInfo = type;
        }
    }
}