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
        Role GetUserRole(string Name, string password);

        IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? filterByActive = null, BO.VolunteerInLIstFields? sortByField = null);

        BO.Volunteer Read(int id);

        void Update(int id, BO.Volunteer updateVolunteerObj);

        void Delete(int id);

        void Create(BO.Volunteer boVolunteer);
        BO.Role Login(string fullName, string password);



    }
}
