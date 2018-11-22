namespace MakerBridge.Remoting
{
    public interface IMessenger
    {
        void SendMessage(byte[] message);
        byte[] GetMessage();
        void ClearMessage();
    }
}
