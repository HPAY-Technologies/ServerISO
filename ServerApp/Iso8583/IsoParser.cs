using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iso8583
{
    public static class IsoParser
    {
        private static readonly Dictionary<int, IsoFieldSpec> FieldSpecs =
            new Dictionary<int, IsoFieldSpec>
        {
            { 2,  new IsoFieldSpec(2,  FieldType.LLVAR, 19) },
            { 3,  new IsoFieldSpec(3,  FieldType.FIXED, 6) },
            { 4,  new IsoFieldSpec(4,  FieldType.FIXED, 12) },
            { 7,  new IsoFieldSpec(7,  FieldType.FIXED, 10) },
            { 11, new IsoFieldSpec(11, FieldType.FIXED, 6) },
            { 12, new IsoFieldSpec(12, FieldType.FIXED, 6) },
            { 13, new IsoFieldSpec(13, FieldType.FIXED, 4) },
            { 37, new IsoFieldSpec(37, FieldType.FIXED, 12) },
            { 41, new IsoFieldSpec(41, FieldType.FIXED, 8) },
            { 49, new IsoFieldSpec(49, FieldType.FIXED, 3) },
            { 64, new IsoFieldSpec(64, FieldType.FIXED, 16) }
        };

        public static IsoMessage Parse(string isoData)
        {
            int index = 0;

            string mti = isoData.Substring(index, 4);
            index += 4;

            var message = new IsoMessage(mti);

            string bitmapHex = isoData.Substring(index, 16);
            index += 16;

            var activeFields = IsoBitmap.ParseBitmap(bitmapHex);

            foreach (int field in activeFields.Where(f => f != 1))
            {
                if (!FieldSpecs.ContainsKey(field))
                    throw new Exception($"Champ ISO non supporté : {field}");

                var spec = FieldSpecs[field];
                string value = ParseField(spec, isoData, ref index);

                message.SetField(field, value);
            }

            return message;
        }

        private static string ParseField(IsoFieldSpec spec, string data, ref int index)
        {
            switch (spec.Type)
            {
                case FieldType.FIXED:
                    string fixedValue = data.Substring(index, spec.Length);
                    index += spec.Length;
                    return fixedValue;

                case FieldType.LLVAR:
                    int len2 = int.Parse(data.Substring(index, 2));
                    index += 2;
                    string llvar = data.Substring(index, len2);
                    index += len2;
                    return llvar;

                case FieldType.LLLVAR:
                    int len3 = int.Parse(data.Substring(index, 3));
                    index += 3;
                    string lllvar = data.Substring(index, len3);
                    index += len3;
                    return lllvar;

                default:
                    throw new Exception("Type de champ inconnu");
            }
        }
    }
}
