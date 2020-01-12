using System;
using System.Threading;

namespace GZiper
{
    abstract class GZip
    {
        #region fields
        protected readonly int _chunkSize = 1048576;
        protected static   int _threads = Environment.ProcessorCount;
        protected static   int _counter = 0;//cancelled threads counter
        protected static   long currentPosition = 0;
        protected static   long fileLenght = 0;
        protected Thread[] _thPool = new Thread[_threads];
        protected QueueManager readingQueue;
        protected QueueManager writingQueue;
        #endregion

        #region properties
        protected string InputFile { get; set; }
        protected string OutputFile { get; set; }
        protected bool   Success { get; set; } = false;
        protected bool   Stopped { get; set; } = false; 
        #endregion

        public GZip(string inputFile, string outputFile)
        {
            readingQueue = new QueueManager();
            writingQueue = new QueueManager();
            InputFile = inputFile;
            OutputFile = outputFile;
        }

        public int ReturnResult()
        {
            if (!Stopped && Success)
                return 0;
            return 1;
        }

        public void StopProcess()
        {
            Stopped = true;
        }
        
        public abstract void Start();
        protected abstract void Read();
        protected abstract void Write();
    }
}
