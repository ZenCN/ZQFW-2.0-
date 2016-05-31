using System.Collections.Generic;

namespace LogicProcessingClass.Model
{
    public class UserInfo
    {
        public Unit LocalUnit { get; set; }
        public IList<Unit> UnderUnits { get; set;}

    }

    public class Unit
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
    }
}