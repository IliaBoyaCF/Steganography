using SteganographyInterfaces;
using System.Drawing;
using System.Text;

namespace SteganographyImplementation
{
    internal class BitPairEnumerator
    {
        private readonly byte[] _data;
        private int _byteIndex;
        private int _bitPosition;

        public BitPairEnumerator(byte[] data)
        {
            _data = data;
            _byteIndex = 0;
            _bitPosition = 6;
        }

        public IEnumerable<byte> GetBitPairs()
        {
            while (_byteIndex < _data.Length)
            {
                byte currentByte = _data[_byteIndex];
                byte bitPair = (byte)((currentByte >> _bitPosition) & 0b11);

                yield return bitPair;

                _bitPosition -= 2;
                if (_bitPosition < 0)
                {
                    _byteIndex++;
                    _bitPosition = 6;
                }
            }
        }

        public int TotalBitPairs => _data.Length * 4;
    }

    internal class Hidder : IHidder
    {
        private string input;
        private Bitmap sourceImage;
        private Bitmap destinationImage;

        public Hidder(string input, Bitmap sourceImage, Bitmap destinationImage)
        {
            this.input = input;
            this.sourceImage = sourceImage;
            this.destinationImage = destinationImage;
        }

        public long Hide()
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            BitPairEnumerator sequenceSource = new BitPairEnumerator(inputBytes);

            int randomSeed = input.Length;
            int inputLength = inputBytes.Length;
            int pixelsCount = sourceImage.Size.Width * sourceImage.Size.Height;

            IEnumerator<byte> bitPairSequence = sequenceSource.GetBitPairs().GetEnumerator();
            IEnumerator<int> pixelMappingSequence = GeneratePixelMappingSequence(
                (sequenceSource.TotalBitPairs + 2) / 3, pixelsCount, randomSeed).GetEnumerator();

            while (pixelMappingSequence.MoveNext())
            {
                int hiddenPairsCount = HideInPixel(bitPairSequence, pixelMappingSequence.Current);

                if (hiddenPairsCount < 3)
                {
                    break;
                }
            }

            return ((long)inputLength << 32) | randomSeed;
        }

        private int HideInPixel(IEnumerator<byte> enumerator, int pixelIndex)
        {
            int x = pixelIndex % sourceImage.Width;
            int y = pixelIndex / sourceImage.Width;
            Color color = sourceImage.GetPixel(x, y);

            byte a = color.A;
            byte r = color.R, g = color.G, b = color.B;
            int hiddenPairsCount = 0;

            if (enumerator.MoveNext())
            {
                r = (byte)((r & 0b11111100) | enumerator.Current);
                hiddenPairsCount++;
            }

            if (enumerator.MoveNext())
            {
                g = (byte)((g & 0b11111100) | enumerator.Current);
                hiddenPairsCount++;
            }

            if (enumerator.MoveNext())
            {
                b = (byte)((b & 0b11111100) | enumerator.Current);
                hiddenPairsCount++;
            }

            if (hiddenPairsCount > 0)
            {
                destinationImage.SetPixel(x, y, Color.FromArgb(a, r, g, b));
            }

            return hiddenPairsCount;
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
