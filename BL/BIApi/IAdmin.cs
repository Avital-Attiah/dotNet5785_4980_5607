using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApi
{
    public interface IAdmin
    {
        DateTime GetClock();
        TimeSpan GetRiskRange();
        void SetRiskRange(TimeSpan riskRange);
        void ResetDB();
        void InitializeDB();
    }
}
