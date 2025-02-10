using BO; 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    // Defines an interface for handling volunteer-related operations
    public interface IVolunteer
    {
        // Retrieves the role of a user based on their name and password
        Role GetUserRole(string Name, string password);

        // Retrieves a list of volunteers with optional filtering and sorting
        IEnumerable<BO.VolunteerInList> GetVolunteersList(
            bool? filterByActive = null, // Optional filter by active status
            BO.VolunteerInLIstFields? sortByField = null // Optional sorting criteria
        );

        // Reads and returns the details of a specific volunteer by their ID
        BO.Volunteer Read(int id);

        // Updates a volunteer's details using a given updated object
        void Update(int id, BO.Volunteer updateVolunteerObj);

        // Deletes a volunteer from the system based on their ID
        void Delete(int id);

        // Creates a new volunteer record
        void Create(BO.Volunteer boVolunteer);

        // Logs in a volunteer using their full name and password and returns their role
        BO.Role Login(string fullName, string password);
    }
}
