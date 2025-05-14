using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    // Defines an interface for administrative operations
    public interface IAdmin :IObservable //stage 5 הרחבת ממשק

    {
        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5

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
