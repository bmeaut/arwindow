namespace Configuration
{
    /// <summary>
    /// Currently unused because Unity's inspector doesn't like interfaces...
    /// 
    /// An interface representing components that need certain settings to be read from a configuration file at the start of runtime.
    /// The configuration is directed by the Configuration Manager.
    /// </summary>
    public interface IConfigurable
    {
        void UpdateConfiguration();
    }
}