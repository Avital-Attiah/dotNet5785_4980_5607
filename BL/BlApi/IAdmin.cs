using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IAdmin
    {
        void UpdateClock(BO.TimeUnit timeUnit);
        DateTime GetClock();
        TimeSpan GetRiskRange();
        void SetRiskRange(TimeSpan riskRange);
        void ResetDB();
        void InitializeDB();
    }
}
