using SteganographyInterfaces;
using System.Drawing;
using System.Text;

namespace SteganographyImplementation
{
    internal class BitPairCollector
    {
        private readonly List<byte> _resultBytes;
        private byte _currentByte;
        private int _bitPosition;

        public BitPairCollector()
        {
            _resultBytes = new List<byte>();
            _currentByte = 0;
            _bitPosition = 6;
        }

        public void AddBitPair(byte bitPair)
        {
            _currentByte |= (byte)((bitPair & 0b11) << _bitPosition);

            _bitPosition -= 2;

            if (_bitPosition < 0)
            {
                _resultBytes.Add(_currentByte);
                _currentByte = 0;
                _bitPosition = 6;
            }
        }

        public byte[] GetBytes()
        {
            return _resultBytes.ToArray();
        }

        public void Reset()
        {
            _resultBytes.Clear();
            _currentByte = 0;
            _bitPosition = 6;
        }
    }

    internal class Finder : IFinder
    {
        private Bitmap bitmap;

        public Finder(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public string Find(long seed)
        {
            int messageLength = (int)(seed >> 32);
            int randomSeed = (int)(seed & 0xFFFFFFFF);

            int pixelsCount = bitmap.Width * bitmap.Height;
            int pixelsNeeded = (messageLength * 4 + 2) / 3;

            IEnumerable<int> pixelMappingSequence = GeneratePixelMappingSequence(pixelsNeeded, pixelsCount, randomSeed);

            var collector = new BitPairCollector();

            foreach (int pixelIndex in pixelMappingSequence)
            {
                ExtractBitPairsFromPixel(pixelIndex, collector);
            }

            byte[] allBytes = collector.GetBytes();
            byte[] messageBytes = allBytes.Take(messageLength).ToArray();

            return Encoding.UTF8.GetString(messageBytes);
        }

        private void ExtractBitPairsFromPixel(int pixelIndex, BitPairCollector collector)
        {
            int x = pixelIndex % bitmap.Width;
            int y = pixelIndex / bitmap.Width;

            Color color = bitmap.GetPixel(x, y);

            collector.AddBitPair((byte)(color.R & 0b11));
            collector.AddBitPair((byte)(color.G & 0b11));
            collector.AddBitPair((byte)(color.B & 0b11));
        }

        private IEnumerable<int> GeneratePixelMappingSequence(int inputLength, int pixelsCount, int seed)
        {
            Random random = new Random(seed);

            HashSet<int> result = new HashSet<int>();

            while (result.Count < inputLength)
            {
                result.Add(random.Next(0, pixelsCount));
            }

            return result;

        }

    }
}