using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iso8583
{
    public static class IsoBitmap
    {
        public static string GenerateBitmap(IEnumerable<int> fields)
        {
            bool[] bitmap = new bool[64];

            foreach (int field in fields)
            {
                if (field < 1 || field > 64)
                    throw new Exception($"Champ ISO invalide : {field}");

                bitmap[field - 1] = true;
            }

            return BitmapToHex(bitmap);
        }

        private static string BitmapToHex(bool[] bitmap)
        {
            StringBuilder hex = new StringBuilder();

            for (int i = 0; i < 64; i += 4)
            {
                int value = 0;

                for (int j = 0; j < 4; j++)
                {
                    if (bitmap[i + j])
                        value += 1 << (3 - j);
                }

                hex.Append(value.ToString("X"));
            }

            return hex.ToString();
        }

        
        public static List<int> ParseBitmap(string hexBitmap)
        {
            List<int> fields = new List<int>();

            for (int i = 0; i < hexBitmap.Length; i++)
            {
                int nibble = Convert.ToInt32(hexBitmap[i].ToString(), 16);

                for (int bit = 0; bit < 4; bit++)
                {
                    if ((nibble & (1 << (3 - bit))) != 0)
                        fields.Add(i * 4 + bit + 1);
                }
            }

            return fields;
        }
    }
}
