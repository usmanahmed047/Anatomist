// (c) Vasian Cepa 2005
// Version 2

using System;
using System.Collections; // required for NumericComparer : IComparer only
using System.Text.RegularExpressions;
namespace ns
{

	public class NumericComparer : IComparer
	{
        

		public NumericComparer()
		{}
		
		public int Compare(object a, object b)
		{
            Regex rgx = new Regex("([^0-9]*)([0-9]+)"); 

            var ma = rgx.Matches(a.ToString());
            var mb = rgx.Matches(b.ToString());
            for (int i = 0; i < ma.Count; ++i)
            {
                int ret = ma[i].Groups[1].Value.CompareTo(mb[i].Groups[1].Value);
                if (ret != 0)
                    return ret;

                ret = int.Parse(ma[i].Groups[2].Value) - int.Parse(mb[i].Groups[2].Value);
                if (ret != 0)
                    return ret;
            }

            return 0;
        }
	}//EOC

}