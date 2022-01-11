namespace DataWindow.Serialization.Components
{
    public interface ILazyWrite
    {
        void Begin();

        void End(bool cancel);
    }
}