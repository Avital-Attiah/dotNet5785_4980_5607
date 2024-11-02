// See https://aka.ms/new-console-template for more information
partial class Program
{
    private static void Main(string[] args)
    {
        Welcom4980();
        Welcome5607();
    }
    static partial void Welcome5607();
    private static void Welcom4980()
    {
        Console.WriteLine("Enter your name");
        string name = Console.ReadLine();
        Console.WriteLine("Hello " + name);
    }
}
