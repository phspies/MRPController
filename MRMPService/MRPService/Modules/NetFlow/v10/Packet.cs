using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MRMPService.Modules.Netflow.v10
{
    public class V10Packet
    {
        private V9Header _header;
        private List<FlowSet> _flowset;
        private Byte[] _bytes;

        public V9Header Header
        {
            get
            {
                return this._header;
            }
        }
        public List<FlowSet> FlowSet
        {
            get
            {
                return this._flowset;
            }
        }

        public V10Packet(Byte[] bytes, TemplatesV10 templates)
        {
            this._bytes = bytes;
            this.Parse(templates);
        }

        private void Parse(TemplatesV10 templates)
        {
            this._flowset = new List<FlowSet>();

            Int32 length = _bytes.Length - 16;

            Byte[] header = new Byte[16];
            Byte[] flowset = new Byte[length];

            Array.Copy(_bytes, 0, header, 0, 16);
            Array.Copy(_bytes, 16, flowset, 0, length);

            this._header = new V9Header(header);
            byte[] reverse = flowset.Reverse().ToArray();

            int templengh = 0;

            while ((templengh + 2) < flowset.Length)
            {
                UInt16 lengths = BitConverter.ToUInt16(reverse, flowset.Length - sizeof(Int16) - (templengh + 2));
                Byte[] bflowsets = new Byte[lengths];
                Array.Copy(flowset, templengh, bflowsets, 0, lengths);

                FlowSet flowsets = new FlowSet(bflowsets, templates);
                this._flowset.Add(flowsets);

                templengh += lengths;
            }
        }

    }
}
