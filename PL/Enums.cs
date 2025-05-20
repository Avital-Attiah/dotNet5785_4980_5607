using System;
using System.Collections;
using System.Collections.Generic;
using BO;

namespace PL
{
    /// <summary>
    /// אוסף של כל ערכי ה-CallType לשימוש ב-ComboBox
    /// </summary>
    internal class CallTypesCollection : IEnumerable<CallType>
    {
        static readonly IEnumerable<CallType> s_values =
            (CallType[])Enum.GetValues(typeof(CallType));

        public IEnumerator<CallType> GetEnumerator() => s_values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => s_values.GetEnumerator();
    }
    internal class RolesCollection : IEnumerable<Role>
    {
        static readonly IEnumerable<Role> s_values =
            (Role[])Enum.GetValues(typeof(Role));

        public IEnumerator<Role> GetEnumerator() => s_values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => s_values.GetEnumerator();
    }
    internal class DistanceTypesCollection : IEnumerable<DistanceType>
    {
        static readonly IEnumerable<DistanceType> s_values =
            (DistanceType[])Enum.GetValues(typeof(DistanceType));

        public IEnumerator<DistanceType> GetEnumerator() => s_values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => s_values.GetEnumerator();
    }
}
