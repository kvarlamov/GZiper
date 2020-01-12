using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace GZiper.Archivator
{
    class Decompressor : GZip
    {
        int counter = 0;
        public Decompressor(string inputFile, string outputFile) : base(inputFile, outputFile)
        {
            
        }

        public override void Start()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDecompressing process...");
            Thread reader = new Thread(Read);
            reader.Start();

            for (int i = 0; i < _threads; i++)
            {
                _thPool[i] = new Thread(() => Decompress(Thread.CurrentThread.ManagedThreadId));
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
                Console.WriteLine("\nFile succesfully decompressed");
                Success = true;
            }
        }

        protected override void Read()
        {
            try
            {
                using (FileStream inputFile = new FileStream(InputFile,FileMode.Open))
                {
                    fileLenght = inputFile.Length;
                    while (inputFile.Position < inputFile.Length && !Stopped)
                    {
                        byte[] lenghtBuffer = new byte[8];//get header
                        inputFile.Read(lenghtBuffer, 0, lenghtBuffer.Length);
                        int chunkLenght = BitConverter.ToInt32(lenghtBuffer, 4);//get chunk length from header info
                        byte[] compressedData = new byte[chunkLenght];//new array with necessary length
                        lenghtBuffer.CopyTo(compressedData, 0);

                        inputFile.Read(compressedData, 8, chunkLenght - 8);
                        int dataSize = BitConverter.ToInt32(compressedData, chunkLenght - 4);
                        byte[] buffer = new byte[dataSize];

                        Chunk chunk = new Chunk(counter, buffer, compressedData);
                        readingQueue.AddToWritingQueue(chunk);
                        counter++;
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
        private void Decompress(int i)
        {
            try
            {
                while (true && !Stopped)
                {
                    Chunk chunk = readingQueue.Dequeue();
                    if (chunk == null)
                    {
                        Interlocked.Increment(ref _counter);
                        if (_counter == _threads)
                            writingQueue.Stop();
                        return; 
                    }

                    using (MemoryStream ms = new MemoryStream(chunk.CompressedBuffer))
                    {
                        using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            gzipStream.Read(chunk.Buffer, 0, chunk.Buffer.Length);
                            byte[] decompressedData = chunk.Buffer.ToArray();
                            Chunk decompressedChunk = new Chunk(chunk.Id, decompressedData);
                            writingQueue.AddToWritingQueue(decompressedChunk);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error in thread #{i}");
                Console.WriteLine("--- Error description---\n", ex.Message);
                Stopped = true;
            }
        }

        protected override void Write()
        {
            try
            {
                using (FileStream outputFile = new FileStream(OutputFile, FileMode.Append))
                {
                    while (true && !Stopped)
                    {
                        Chunk chunk = writingQueue.Dequeue();
                        if (chunk == null)
                            return;

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
