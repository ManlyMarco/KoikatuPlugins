namespace MakerBridge.Remoting
{
    public interface IMessenger
    {
        void SendMessage(byte type, byte[] message);
        byte[] GetMessage(byte type);
        void ClearMessage(byte type);
    }
}
