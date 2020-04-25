using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratec_Challenge.Model
{
    public class Pin
    {
        public int Value { get; set; }


        public Pin(int val = 0)
        {
            Value = val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}