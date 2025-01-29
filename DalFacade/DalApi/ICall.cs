
namespace DalApi;
using DO;
public interface ICall : ICrud<Call> 

{
    
    void Print(Call item); // print the item
}