
namespace DalApi;
using DO;
using System;
using System.Collections.Generic;

public interface IAssignment : ICrud<Assignment>
{
    void Print(Assignment item); 
    IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null);
}


