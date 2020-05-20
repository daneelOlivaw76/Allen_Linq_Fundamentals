using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using static System.Console;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {

            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) => x + y;

            Console.WriteLine(square(3));
            Console.WriteLine(add(3,4));

            Action<int> write = x => Console.WriteLine(x);
            write(square(add(3, 5)));

            IEnumerable<Employee> developers = new Employee[]
            {
                new Employee { Id = 1, Name = "Scott"},
                new Employee { Id = 3, Name = "John"},
                new Employee { Id = 4, Name = "Sally"},
                new Employee { Id = 5, Name = "Robert"},
                new Employee { Id = 6, Name = "Ann"},
                new Employee { Id = 7, Name = "Janny"},
                new Employee { Id = 2, Name = "Chris"}
            };

            IEnumerable<Employee> sales = new List<Employee>()
            {
                new Employee { Id = 3, Name = "Alex"}
            };

            var query = developers.Where(e => e.Name.Length == 5).OrderByDescending(e => e.Name);

            var query2 = from employee in developers
                         where employee.Name.Length == 5
                         orderby employee.Name descending
                         select employee;

            foreach (var employee in query2)
            {
                Console.WriteLine(employee.Name);
            }

            //foreach(var person in sales)
            //{
            //    WriteLine(person.Name);
            //}

            //IEnumerator<Employee> enumerator1 = sales.GetEnumerator();
            //while (enumerator1.MoveNext())
            //{
            //    WriteLine(enumerator1.Current.Name);
            //}
            //Console.WriteLine(sales.Count());

            //WriteLine("\n---\n");

            //IEnumerator<Employee> enumerator2 = developers.GetEnumerator();
            //while (enumerator2.MoveNext())
            //{
            //    WriteLine(enumerator2.Current.Name);
            //}
            //Console.WriteLine(developers.Count());

        }
    }
}
