using System;

namespace CodeBase.CustomDIContainer
{
    /// <summary>
    /// Attribute to mark fields or properties for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute { }
}