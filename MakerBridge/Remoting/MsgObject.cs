using MessagePack;

namespace MakerBridge.Remoting
{
    [MessagePackObject(true)]
    public class MsgObject
    {
        public string path;
    }
}
