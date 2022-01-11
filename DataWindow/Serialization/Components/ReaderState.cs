namespace DataWindow.Serialization.Components
{
    public enum ReaderState
    {
        Initial,
        StartElement,
        Value,
        EndElement,
        EOF,
        Error
    }
}