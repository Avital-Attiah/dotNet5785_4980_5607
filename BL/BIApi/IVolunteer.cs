using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApi
{
    public interface IVolunteer
    {
        Role GetUserRole(string Name);

        IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? sortByActive = null, VolunteerInListFields? filterFields = null);

        BO.Volunteer GetVolunteerById(int id);

        void UpdateVolunteer(int id, BO.Volunteer volunteerToUpdate);

        void RemoveVolunteer(int id);

        void AddVolunteer(BO.Volunteer newVolunteer);




    }
}
