namespace ProtoBuf.Extension
{
    public abstract class Object
    {
        public override string ToString() => LitJson.JsonMapper.ToJson(this);
    }
}