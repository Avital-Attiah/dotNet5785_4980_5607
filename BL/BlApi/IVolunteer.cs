using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IVolunteer
    {
        Role GetUserRole(string Name);

        IEnumerable<VolunteerInList> GetVolunteersList(bool? sortByActive = null, VolunteerInLIstFields? filterFields = null);

        Volunteer GetVolunteerById(int id);

        void UpdateVolunteer(int id, Volunteer volunteerToUpdate);

        void RemoveVolunteer(int id);

        void AddVolunteer(Volunteer newVolunteer);




    }
}
