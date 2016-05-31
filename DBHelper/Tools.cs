using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper
{
    public class Tools
    {
        static public int GetLimitByCode(string unitCode)
        {
            int Limit = -1;
            if (unitCode == "00000000")
            {
                Limit = 1;
            }
            else if (unitCode.Substring(2, 6) == "000000")
            {
                Limit = 2;
            }
            else if (unitCode.Substring(4, 4) == "0000")
            {
                Limit = 3;
            }
            else if (unitCode.Substring(6, 2) == "00")
            {
                Limit = 4;
            }

            return Limit;
        }

    }
}
