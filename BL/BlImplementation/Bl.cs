using BlApi;  // Importing the BlApi namespace for the interface definitions
using BlImplementation;  // Importing the BlImplementation namespace where the implementation classes reside
using System;  // Importing the System namespace for basic functionality
using System.Collections.Generic;  // Importing the Collections.Generic namespace for working with collections
using System.Linq;  // Importing the LINQ methods for querying collections
using System.Text;  // Importing the Text namespace for text-related operations
using System.Threading.Tasks;  // Importing the Task namespace for asynchronous programming

namespace BlImplementation  // Defining the BlImplementation namespace
{
    internal class Bl : IBl  // Defining the 'Bl' class that implements the IBl interface
    {
        public IVolunteer Volunteer { get; } = new VolunteerImplementation();  // Creating a property for 'Volunteer' with an implementation of IVolunteer
        public ICall Call { get; } = new CallImplementation();  // Creating a property for 'Call' with an implementation of ICall
        public IAdmin Admin { get; } = new AdminImplementation();  // Creating a property for 'Admin' with an implementation of IAdmin
        public void AddObserver(Action listObserver)
            => Call.AddObserver(listObserver);

        public void AddObserver(int id, Action observer)
            => Call.AddObserver(id, observer);

        public void RemoveObserver(Action listObserver)
            => Call.RemoveObserver(listObserver);

        public void RemoveObserver(int id, Action observer)
            => Call.RemoveObserver(id, observer);
    }
}
