using System;
using UnityEngine;

namespace Framework.Runtime
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class PropertyVariableValueAttribute : PropertyAttribute
    {
        private Type _propertyAttributeType;

        public Type propertyAttributeType { get => _propertyAttributeType; set => _propertyAttributeType = value; }

        public PropertyVariableValueAttribute()
        {
            
        }

        public PropertyVariableValueAttribute(Type propertyAttributeType)
        {
            this._propertyAttributeType = propertyAttributeType;
        }

    }
}
