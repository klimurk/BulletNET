using BulletNET.Services.BarcodeReader.Interface;

namespace BulletNET.Services.BarcodeReader
{
    public class BarcodeCRC : IBarcodeCRC
    {
        #region Fields

        public const long __MAXNUMBER = 1000000000000L - 1;
        public const long __MOD10POSITION = 100000000L;
        private const long __CRCPOSITION = 1000000000L;
        public const long __TRIDAPOS = 100000000000L;
        private const int __MAXVALUE = 99999999;

        #endregion Fields

        #region CRC

        private long MakeAllCRC_Inner(long bcode)
        {
            long zaklad = bcode % __MOD10POSITION;
            long trida = bcode / __TRIDAPOS;
            int x = (int)zaklad * 10 + (int)trida;

            //conversion to BCD
            long decimalized = 0;
            int shifter = 0;
            while (x > 0)
            {
                decimalized |= (long)(x % 10) << shifter;
                shifter += 4;
                x /= 10;
            }

            //polynomial CRC
            long crc = decimalized << 7;
            for (long pol = 241L << 35, polpos = 1L << 42; polpos >= 1L << 7; pol >>= 1, polpos >>= 1)
            {
                if (crc >= polpos)
                {
                    crc ^= pol;//241 = "11110001"
                }
            }
            crc %= 100;//conversion to 00-99 range

            decimalized = decimalized << 8 | crc / 10 << 4 | crc % 10;

            //mod 10
            int multiplier = 3;
            int sum = 0;
            for (int d = 0; d < 11; ++d)
            {
                int digit = (int)(decimalized & 0xF);
                sum += digit * multiplier;
                multiplier = 4 - multiplier;//3, 1, 3, 1...
                decimalized >>= 4;
            }

            return __TRIDAPOS * trida + crc * __CRCPOSITION + (1000 - sum) % 10 * __MOD10POSITION + zaklad;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="number">0..99999999</param>
        /// <param name="trida">0..9</param>
        /// <returns></returns>
        public long MakeAllCRC(int number, int trida)
        {
            if (number < 0 || number > __MAXVALUE) { throw new ArgumentException("Číslo " + number + " je mimo rozsah našich čárových kódů."); }
            if (trida < 0 || trida > 9) { throw new ArgumentException("Třída " + trida + " je mimo rozsah našich čárových kódů."); }

            long x = number + trida * __TRIDAPOS;

            return MakeAllCRC_Inner(x);
        }

        public long DeleteCRC(long x)
        {
            if (!InRange(x)) { throw new ArgumentException("Číslo mimo rozsah našich čárových kódů."); }

            return x - x / __MOD10POSITION % 1000 * __MOD10POSITION;
        }

        /// <summary>
        /// Number is in range for our barcode (0..999999999999)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool InRange(long x)
        {
            return x >= 0 && x <= __MAXNUMBER;
        }

        /// <summary>
        /// Tests CRC of the barcode
        /// </summary>
        /// <param name="x">Throws exception when number is not in correct range (test inRange first if not sure)</param>
        /// <returns></returns>
        public bool IsAllCRCOk(long x)
        {
            if (!InRange(x)) { throw new ArgumentException("Číslo mimo rozsah našich čárových kódů."); }

            return x == MakeAllCRC_Inner(x);
        }

        #endregion CRC
    }
}