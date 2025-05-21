using System;
using System.Collections;
using System.Collections.Generic;
using BO;

namespace PL
{
    public class CallTypesCollection : IEnumerable<CallType>  // שימי לב ל-public
    {
        public static readonly IEnumerable<CallType> s_values =  // גם פה public
            (CallType[])Enum.GetValues(typeof(CallType));

        public IEnumerator<CallType> GetEnumerator() => s_values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => s_values.GetEnumerator();
    }

    // אם צריך גם את השאר אז ככה:
    public class RolesCollection : IEnumerable<Role>
    {
        public static readonly IEnumerable<Role> s_values =
            (Role[])Enum.GetValues(typeof(Role));

        public IEnumerator<Role> GetEnumerator() => s_values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => s_values.GetEnumerator();
    }

    public class DistanceTypesCollection : IEnumerable<DistanceType>
    {
        public static readonly IEnumerable<DistanceType> s_values =
            (DistanceType[])Enum.GetValues(typeof(DistanceType));

        public IEnumerator<DistanceType> GetEnumerator() => s_values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => s_values.GetEnumerator();
    }
}
