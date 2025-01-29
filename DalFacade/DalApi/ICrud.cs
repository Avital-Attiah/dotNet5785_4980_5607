using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi;

public interface ICrud<T> where T : class
{
    void Create(T item); // Creates a new entity object in the DAL
    T? Read(int id); // Reads an entity object by its ID
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null);
    void Update(T item); // Updates an entity object
    void Delete(int id); // Deletes an object by its ID
    void DeleteAll(); // Deletes all entity objects
    T? Read(Func<T, bool> filter);
}
