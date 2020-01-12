using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZiper
{
    class Compressor : GZip
    {
        public Compressor(string inputFile, string outputFile): base(inputFile,outputFile)
        {

        }
        public override void Start()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nCompressing process...");
            Thread reader = new Thread(Read);
            reader.Start();

            for (int i = 0; i < _threads; i++)
            {
                _thPool[i] = new Thread(() => Compress(Thread.CurrentThread.ManagedThreadId));
                _thPool[i].Start();
            }

            Thread writer = new Thread(Write);
            writer.Start();
            while (writer.IsAlive)
            {
                if (fileLenght == 0)
                    continue;
                Console.Write("\b\b\b\b{0}%", 100 * currentPosition / fileLenght);
            }
            writer.Join();

            if (!Stopped)
            {
                Console.WriteLine("\nFile succesfully compressed");
                Success = true;
            }
        }

        protected override void Read()
        {
            try
            {
                using (FileStream inputFile = new FileStream(InputFile, FileMode.Open))
                {
                    int bytesRead;
                    byte[] buffer;
                    fileLenght = inputFile.Length;

                    while (inputFile.Position < fileLenght && !Stopped)
                    {
                        if (fileLenght - inputFile.Position <= _chunkSize)
                            bytesRead = (int)(fileLenght - inputFile.Position);
                        else
                            bytesRead = _chunkSize;

                        buffer = new byte[bytesRead];
                        inputFile.Read(buffer, 0, bytesRead);
                        readingQueue.AddToCompressingQueue(buffer);
                        currentPosition = inputFile.Position;
                    }
                    readingQueue.Stop();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nFinalizing...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Stopped = true;
            }
        }

        /// <summary>
        /// Multithreading data compress method
        /// </summary>
        /// <param name="i">Thread Id</param>
        private void Compress(int i)
        {
            try
            {
                while (true && !Stopped)
                {
                    Chunk chunk = readingQueue.Dequeue();

                    if (chunk == null)
                    {
                        Interlocked.Increment(ref _counter);
                        if (_counter == _threads)//if all threads put data to BlockingCollection
                            writingQueue.Stop();   //set IsAddingCompleted property
                        return;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (GZipStream gZipStream = new GZipStream(ms,CompressionMode.Compress))
                        {
                            gZipStream.Write(chunk.Buffer, 0, chunk.Buffer.Length);
                        }

                        byte[] data = ms.ToArray();
                        Chunk compressedChunk = new Chunk(chunk.Id, data);
                        writingQueue.AddToWritingQueue(compressedChunk);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in thread #{i}");
                Console.WriteLine("--- Error description---\n",ex.Message);
                Stopped = true;
            }
        }

        protected override void Write()
        {
            try
            {
                using (FileStream outputFile = new FileStream(OutputFile+".gz",FileMode.Append))
                {
                    while (true && !Stopped)
                    {
                        Chunk chunk = writingQueue.Dequeue();
                        if (chunk == null)
                        {
                            return;
                        }

                        BitConverter.GetBytes(chunk.Buffer.Length).CopyTo(chunk.Buffer, 4);
                        outputFile.Write(chunk.Buffer, 0, chunk.Buffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Stopped = true;
            }
        }
    }
}
