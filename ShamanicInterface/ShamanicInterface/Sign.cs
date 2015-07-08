using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShamanicInterface.DataStructure
{
    //[Serializable]
    public class Sign
    {
        private double[] values;

        public Sign(int dim = 0) {
            values = new double[dim];
        }

        public Sign(double[] vals) {
            values = vals;
        }

        public void SetValues(double[] vals) {
            values = vals;
        }

        public double[] GetValues() {
            return values;
        }

        public int GetDimensions() {
            return values.Length;
        }
    }
}

