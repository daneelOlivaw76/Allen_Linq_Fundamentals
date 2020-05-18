using System;
using System.Collections.Generic;
using System.Numerics;
using static System.Console;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Employee> developers = new Employee[]
            {
                new Employee { Id = 1, Name = "Scott"},
                new Employee { Id = 2, Name = "Chris"}
            };

            IEnumerable<Employee> sales = new List<Employee>()
            {
                new Employee { Id = 3, Name = "Alex"}
            };

            //foreach(var person in sales)
            //{
            //    WriteLine(person.Name);
            //}

            IEnumerator<Employee> enumerator1 = sales.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                WriteLine(enumerator1.Current.Name);
            }
            Console.WriteLine(sales.Count());

            WriteLine("\n---\n");

            IEnumerator<Employee> enumerator2 = developers.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                WriteLine(enumerator2.Current.Name);
            }
            Console.WriteLine(developers.Count());

        }
    }
}
