namespace ProtoBuf.Extension
{
    public class MessageAttribute : BaseAttribute
    {
        public ushort Opcode { get; }
        public MessageAttribute(ushort opcode) => this.Opcode = opcode;
    }

    public class MessageResponseTypeAttribute : BaseAttribute
    {
        public ushort Opcode { get; }
        public MessageResponseTypeAttribute(ushort opcode) => this.Opcode = opcode;
    }
}