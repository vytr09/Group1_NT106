using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebApplication1
{
    [WebService(Namespace = "http://group1.com/nt106.o21.antt")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Group1Webservice : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloNT106()
        {
            return "Hello NT106.021.ANTT - From group 1";
        }

        [WebMethod]
        public Person Hello(string name, int age)
        {
            return new Person
            {
                Name = name,
                Age = age
            };
        }
        [WebMethod]
        public int sum(int a, int b)
        {
            return a + b;
        }
        [WebMethod]
        public int sub(int a, int b)
        {
            return a - b;
        }

        [WebMethod]
        public void AddPerson(string name, int age)
        {
            // Create a new Person object with the provided details
            Person newPerson = new Person { Name = name, Age = age };

            // Add the new person to your data source (e.g., database)
            AddPersonToDatabase(newPerson);
        }

        private void AddPersonToDatabase(Person person)
        {
            // database logic here
            Console.WriteLine("Adding person:");
            Console.WriteLine($"Name: {person.Name}, Age: {person.Age}");
        }

        [WebMethod]
        public List<Person> SearchByName(string name)
        {
            // Perform search logic here
            // For demonstration purposes, let's assume we have a list of persons
            List<Person> persons = GetPersonsFromDatabase();

            // Filter persons by name
            List<Person> searchResults = persons.Where(p => p.Name.ToLower().Contains(name.ToLower())).ToList();

            return searchResults;
        }

        private List<Person> GetPersonsFromDatabase()
        {
            List<Person> persons = new List<Person>
            {
                new Person { Name = "John", Age = 25 },
                new Person { Name = "Jane", Age = 30 },
                new Person { Name = "Alice", Age = 28 }
            };
            return persons;
        }

        [WebMethod]
        public void end()
        {
            System.Environment.Exit(0);
        }

    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
