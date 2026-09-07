using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>子类选择</summary>
    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SubclassSelectAttribute : PropertyAttribute
    {
        public readonly Type parentType;

        public SubclassSelectAttribute(Type parentType)
        {
            this.parentType = parentType;
        }
    }
}