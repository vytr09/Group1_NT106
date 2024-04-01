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
