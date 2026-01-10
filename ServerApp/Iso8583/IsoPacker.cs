using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iso8583
{
    public static class IsoPacker
    {
        private static readonly Dictionary<int, IsoFieldSpec> FieldSpecs =
            new Dictionary<int, IsoFieldSpec>
        {
            { 2,  new IsoFieldSpec(2,  FieldType.LLVAR, 19) },   // PAN
            { 3,  new IsoFieldSpec(3,  FieldType.FIXED, 6) },    // Processing Code
            { 4,  new IsoFieldSpec(4,  FieldType.FIXED, 12) },   // Amount
            { 7,  new IsoFieldSpec(7,  FieldType.FIXED, 10) },   // Transmission DT
            { 11, new IsoFieldSpec(11, FieldType.FIXED, 6) },    // STAN
            { 12, new IsoFieldSpec(12, FieldType.FIXED, 6) },    // Local Time
            { 13, new IsoFieldSpec(13, FieldType.FIXED, 4) },    // Local Date
            { 37, new IsoFieldSpec(37, FieldType.FIXED, 12) },   // RRN
            { 41, new IsoFieldSpec(41, FieldType.FIXED, 8) },    // Terminal ID
            { 49, new IsoFieldSpec(49, FieldType.FIXED, 3) },    // Currency
            { 64, new IsoFieldSpec(64, FieldType.FIXED, 16) }    // MAC
        };

        public static string Pack(IsoMessage message)
        {
            var fields = message.GetFields();

            string bitmap = IsoBitmap.GenerateBitmap(fields.Keys.ToList());

            var dataBuilder = new StringBuilder();

            foreach (var field in fields.OrderBy(f => f.Key))
            {
                if (!FieldSpecs.ContainsKey(field.Key))
                    throw new Exception($"Champ ISO non défini : {field.Key}");

                var spec = FieldSpecs[field.Key];
                dataBuilder.Append(PackField(spec, field.Value));
            }

            return message.MTI + bitmap + dataBuilder;
        }

        private static string PackField(IsoFieldSpec spec, string value)
        {
            if (value == null)
                throw new Exception($"Champ {spec.FieldNumber} vide");

            switch (spec.Type)
            {
                case FieldType.FIXED:
                    return value.PadLeft(spec.Length, '0')
                                .Substring(0, spec.Length);

                case FieldType.LLVAR:
                    if (value.Length > spec.Length)
                        throw new Exception($"Champ {spec.FieldNumber} trop long");

                    return value.Length.ToString("D2") + value;

                case FieldType.LLLVAR:
                    if (value.Length > spec.Length)
                        throw new Exception($"Champ {spec.FieldNumber} trop long");

                    return value.Length.ToString("D3") + value;

                default:
                    throw new Exception("Type de champ inconnu");
            }
        }
    }
}
