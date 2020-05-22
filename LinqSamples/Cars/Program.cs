using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessCars("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            //var query = cars.GroupBy(c => c.Manufacturer.ToUpper())
            //                .OrderBy(g => g.Key);

            var query = cars.GroupBy(c => c.Manufacturer.ToUpper())
                            .Select(g => 
                            {
                                var results = g.Aggregate(new CarStatistics(),
                                                    (acc, c) => acc.Accumulate(c),
                                                    acc => acc.Compute());
                                return new
                                {
                                    Name = g.Key,
                                    Avg = results.Average,
                                    Min = results.Min,
                                    Max = results.Max
                                };
                            })
                            .OrderByDescending(r => r.Max);
                             

            var query2 = manufacturers.GroupJoin(cars,
                                       m => m.Name,
                                       c => c.Manufacturer,
                                       (m, g) => new
                                       {
                                           Manufacturer = m,
                                           Cars = g
                                       })
                                       .OrderBy(m => m.Manufacturer.Headquarters)
                                       .GroupBy(m => m.Manufacturer.Headquarters);

            foreach (var result in query)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Max: {result.Max}.");
                Console.WriteLine($"\t Min: {result.Min}.");
                Console.WriteLine($"\t Avg: {result.Avg}.");
            }
            
            //foreach (var group in query)
            //{
            //    var queryCars = group.SelectMany(g => g.Cars).OrderByDescending(c => c.Combined).Take(3);
            //    Console.WriteLine($"{group.Key}");

            //    foreach(var car in queryCars)
            //    {
            //        Console.WriteLine($"\t{car.Name,-33} : {car.Combined}.");
            //    }
            //}
            
            
            //foreach (var group in query)
            //{
            //    Console.WriteLine(group.Key);
            //    foreach(var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name,-33} : {car.Combined}.");
            //    }
            //}
            

            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Headquarters, -15} {car.Name, -25} : {car.Combined}");
            //}

            //var query = cars.Join(manufacturers, 
            //                 c => new { c.Manufacturer, c.Year }, 
            //                 m => new { Manufacturer = m.Name, m.Year }, 
            //                 (c,m) => new {
            //                     m.Headquarters,
            //                     c.Name,
            //                     c.Combined
            //                 })
            //                .OrderByDescending(c => c.Combined)
            //                .ThenBy(c => c.Name);

            //var queryM = manufacturers.Where(m => m.Name.Contains('A')).OrderByDescending(m => m.Name);

            //foreach (var m in queryM)
            //{
            //    Console.WriteLine($"Manufacturer: {m.Name,-35} HeadQuarters: {m.Headquarters,-15}");
            //}


            //var query = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
            //                .OrderByDescending(c => c.Combined)
            //                .ThenBy(c => c.Name);

            //var result = cars.SelectMany(c => c.Name)
            //                 .OrderBy(c => c);

            //var top = cars.OrderByDescending(c => c.Combined)
            //                .ThenBy(c => c.Name)
            //                .LastOrDefault(c => c.Manufacturer == "BMW" && c.Year == 2016);

            //var result = cars.All(c => c.Manufacturer == "Ford");

            //Console.WriteLine(result);

            //Console.WriteLine($"{top.Manufacturer} {top.Name} : {top.Combined}");

            //foreach (var car in query2.Take(10))
            //{
            //    Console.WriteLine($"{car.Manufacturer} {car.Name} : {car.Combined}");
            //}
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            return
                File.ReadAllLines(path)
                    .Where(line => line.Length > 1)
                    .ToManufacturer()
                    .ToList();
        }

        private static List<Car> ProcessCars(string path)
        {
            return
                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(line => line.Length > 1)
                    .ToCar()
                    .ToList();
        }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {

            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3], CultureInfo.InvariantCulture),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }

        public static IEnumerable<Manufacturer> ToManufacturer(this IEnumerable<string> source)
        {

            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Manufacturer
                {
                    Name = columns[0],
                    Headquarters = columns[1],
                    Year = int.Parse(columns[2])
                };
            }
        }
    }
}
