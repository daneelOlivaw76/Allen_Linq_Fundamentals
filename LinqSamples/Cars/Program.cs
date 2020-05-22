using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Diagnostics.Tracing;
using System.Data.Entity;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();

        }

        private static void QueryData()
        {
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            var query = db.Cars.GroupBy(c => c.Manufacturer)
                               .Select(g => new
                               {
                                   Name = g.Key,
                                   Cars = g.OrderByDescending(c => c.Combined).Take(2)
                               });

            var query2 = db.Cars.Where(c => c.Manufacturer == "BMW")
                               .OrderByDescending(c => c.Combined)
                               .ThenBy(c => c.Name)
                               .Take(10)
                               .ToList();

            foreach (var group in query)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name,-35} : { car.Combined}");
                }
            }

            //Console.WriteLine(query.Count());
            //foreach(var car in query2)
            //{
            //    Console.WriteLine($"{car.Name, -35} : { car.Combined}");
            //}
        }
        

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDb();
            //db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static void QueryXml()
        {

            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = XDocument.Load("fuel.xml");

            var query = document.Element(ns + "Cars")?.Elements(ex + "Car")                                                   //Descendants(ex + "Car")
                                .Where(e => e.Attribute("Manufacturer")?.Value == "BMW")
                                .OrderByDescending(e => e.Attribute("Combined").Value)
                                .Take(20);

            foreach(var element in query)
            {
                Console.WriteLine($"Name: {element.Attribute("Name").Value, -35}, Combined: {element.Attribute("Combined").Value} ");
            }
        }

        private static void CreateXml()
        {
            var records = ProcessCars("fuel.csv");
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = new XDocument();
            var cars = new XElement(ns + "Cars",
                                        from record in records
                                        select new XElement(ex + "Car",
                                                new XAttribute("Name", record.Name),
                                                new XAttribute("Combined", record.Combined),
                                                new XAttribute("Manufacturer", record.Manufacturer))
                                        );

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));

            document.Add(cars);
            document.Save("fuel.xml");

            //var filename = "fuel.xml";
            //CreateXMLByElements(records, document, cars, filename);
            //CreateXMLByAttributes(records, document, cars, filename);

        }

        private static void CreateXMLByAttributes(List<Car> records, XDocument document, XElement cars, string filename)
        {
            foreach (var record in records)
            {
                var car = new XElement("Car", 
                                        new XAttribute("Name", record.Name), 
                                        new XAttribute("Combined", record.Combined),
                                        new XAttribute("Manufacturer", record.Manufacturer));

                cars.Add(car);
            }
            document.Add(cars);
            document.Save(filename);
        }

        private static void CreateXMLByElements(List<Car> records, XDocument document, XElement cars, string filename)
        {
            foreach (var record in records)
            {
                var car = new XElement("Car");
                var name = new XElement("Name", record.Name);
                var combined = new XElement("Combined", record.Combined);
                car.Add(name);
                car.Add(combined);

                cars.Add(car);
            }
            document.Add(cars);
            document.Save(filename);
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
    }
}
