namespace CodeBase.CustomDIContainer
{
    /// <summary>
    /// A scoped DI container that can override or extend the global registrations.
    /// </summary>
    public sealed class ScopedContainer : BaseDiContainer
    {
        public ScopedContainer GetContainer() => this;
    }
}