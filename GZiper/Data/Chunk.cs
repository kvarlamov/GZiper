namespace GZiper
{
    public class Chunk
    {
        public int Id { get; private set; }
        public byte[] Buffer { get; private set; }
        public byte[] CompressedBuffer { get; private set; }

        public Chunk(int id, byte[] buffer)
        {
            Id = id;
            Buffer = buffer;
        }
        public Chunk(int id, byte[] buffer, byte[] compressedBuffer):this(id, buffer)
        {
            CompressedBuffer = compressedBuffer;
        }
    }
}