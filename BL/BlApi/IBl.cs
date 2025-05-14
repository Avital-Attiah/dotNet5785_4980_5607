using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    // Defines the main interface for the business logic layer (BL)
    public interface IBl : IObservable //stage 5 הרחבת ממשק

    {
        // Provides access to volunteer-related operations
        IVolunteer Volunteer { get; }

        // Provides access to call-related operations
        ICall Call { get; }

        // Provides access to administrative operations
        IAdmin Admin { get; }
    }
}
