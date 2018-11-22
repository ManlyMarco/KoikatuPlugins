using MessagePack;

namespace MakerBridge.Remoting
{
    [MessagePackObject(true)]
    public class MsgObject
    {
        public ChaFileFace face;
        public ChaFileBody body;
        public ChaFileHair hair;
        public ChaFileParameter param;
        public byte[] coord;
    }
}
