using System.Collections.Concurrent;
using System.Threading;

namespace GZiper
{
    public class QueueManager
    {
        static EventWaitHandle _wh = new ManualResetEvent(false);
        BlockingCollection<Chunk> _bc = new BlockingCollection<Chunk>(new ConcurrentQueue<Chunk>(), 100);

        private int _chunkId = 0;

        public void AddToCompressingQueue(byte[] buffer)
        {
            Chunk chunk = new Chunk(_chunkId, buffer);
            _bc.Add(chunk);
            _chunkId++;
        }

        public void AddToWritingQueue(Chunk chunk)
        {
            int id = chunk.Id;

            while (id != _chunkId)
                _wh.WaitOne();
            _bc.Add(chunk);
            Interlocked.Increment(ref _chunkId);
            _wh.Set();
        }

        public Chunk Dequeue()
        {
            foreach (var chunk in _bc.GetConsumingEnumerable())
            {
                return chunk;
            }

            return null;
        }
        public void Stop()
        {
            _bc.CompleteAdding();
        }
    }
}