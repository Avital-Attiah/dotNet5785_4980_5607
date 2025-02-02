using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    // Defines an interface for administrative operations
    public interface IAdmin
    {
        // Updates the system clock with a new time unit
        void UpdateClock(BO.TimeUnit timeUnit);

        // Retrieves the current system clock time
        DateTime GetClock();

        // Gets the defined risk range for operations
        TimeSpan GetRiskRange();

        // Sets a new risk range for operations
        void SetRiskRange(TimeSpan riskRange);

        // Resets the database to its initial state
        void ResetDB();

        // Initializes the database with default values or configurations
        void InitializeDB();
    }
}
