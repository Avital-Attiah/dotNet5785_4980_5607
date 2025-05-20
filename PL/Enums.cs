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
}
