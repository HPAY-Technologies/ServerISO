using System.Collections.Generic;

namespace Iso8583
{
    public class IsoMessage
    {
        public string MTI { get; }
        private readonly Dictionary<int, string> _fields = new();

        public IsoMessage(string mti)
        {
            MTI = mti;
        }

        public void SetField(int field, string value)
        {
            _fields[field] = value;
        }

        public string GetField(int field)
        {
            return _fields[field];
        }

        public bool HasField(int field)
        {
            return _fields.ContainsKey(field);
        }

        public Dictionary<int, string> GetFields()
        {
            return _fields;
        }
    }
}
