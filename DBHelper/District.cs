using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper
{
    public class District
    {
        public string UnitCode { get; set; }
        public string RiverCode { get; set; }
        public string UnitName { get; set; }
        public string UnitLevel { get; set; }
        public string Del { get; set; }
        public Dictionary<string, District> LowerUnits { get; set; }
    }
}
