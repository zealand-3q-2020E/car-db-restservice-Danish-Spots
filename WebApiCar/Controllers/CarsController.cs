using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCar.Model;

namespace WebApiCar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {

        static string conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        //public static List<Car> carList = new List<Car>()
        //{
        //    new Car(){Id = 1,Model="x3",Vendor="Tesla", Price=400000},
        //    new Car(){Id = 2,Model="x2",Vendor="Tesla", Price=600000},
        //    new Car(){Id = 3,Model="x1",Vendor="Tesla", Price=800000},
        //    new Car(){Id = 4,Model="x0",Vendor="Tesla", Price=1400000},
        //};

        /// <summary>
        /// Method to get all the cars from the database
        /// </summary>
        /// <returns>List of cars</returns>
        // GET: api/Cars
        [HttpGet]
        public IEnumerable<Car> Get()
        {
            return getCarsFromDB("select id, vendor, model, price from Car");
        }

        //[Route("/byVendor/{vendor}")]
        [HttpGet(("byVendor/{vendor}"), Name ="GetByVendor")]
        public IEnumerable<Car> GetByVendor(string vendor)
        {
            //should be an SQL statement
            return getCarsFromDB($"select id, vendor, model, price from Car WHERE vendor LIKE '{vendor}'");
        }

        [HttpGet(("byVendor/{vendor}/price/{price}"), Name = "GetByVendorAndPrice")]
        public IEnumerable<Car> GetByVendorandPrice(string vendor, int price)
        {
            //shold be an sql statements
            return getCarsFromDB($"select id, vendor, model, price from Car where vendor='{vendor}' AND price='{price}'");
        }

        [HttpGet(("byPrice/{price}"), Name = "GetByPrice")]
        public IEnumerable<Car> GetByPrice(int price)
        {
            //shold be an sql statements
            return getCarsFromDB($"select id, vendor, model, price from Car where price='{price}'");
        }

        [HttpGet(("orderByPrice/{order}"), Name = "GetAscendDescendByPrice")]
        public IEnumerable<Car> GetAscendDescendByPrice(string order)
        {
            //shold be an sql statements
            List<Car> cars = new List<Car>();
            if (order.ToLower() == "ascending")
                cars = getCarsFromDB($"select id, vendor, model, price from Car ORDER BY price ASC");
            else if (order.ToLower() == "descending")
                cars = getCarsFromDB($"select id, vendor, model, price from Car ORDER BY price DESC");
            return cars;
        }

        [HttpGet(("byPrice/{price}/{order}"), Name = "GetByPriceAscDesc")]
        public IEnumerable<Car> GetByPriceAscDesc(int price, string order)
        {
            //shold be an sql statements
            List<Car> cars = new List<Car>();
            if (order.ToLower() == "ascending")
                cars = getCarsFromDB($"select id, vendor, model, price from Car where price='{price}' ORDER BY price ASC");
            else if (order.ToLower() == "descending")
                cars = getCarsFromDB($"select id, vendor, model, price from Car where price='{price}' ORDER BY price DESC");
            return cars;
        }

        // GET: api/Cars/5
        [HttpGet("{id}", Name = "Get")]
        public Car Get(int id)
        {
            return getCarsFromDB($"select id, vendor, model, price from Car Where id={id}")[0];
        }

        /// <summary>
        /// Post a new car to the static list
        /// </summary>
        /// <param name="value"></param>
        // POST: api/Cars
        [HttpPost]
        public void Post([FromBody] Car value)
        {
            string insertCarSql = "insert into car (vendor, model, price) values (@vendor, @model, @price)";
            using (SqlConnection databaseconnection = new SqlConnection(conn))
            {
                databaseconnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(insertCarSql, databaseconnection))
                {
                    insertCommand.Parameters.AddWithValue("@vendor", value.Vendor);
                    insertCommand.Parameters.AddWithValue("@model", value.Model);
                    insertCommand.Parameters.AddWithValue("@price", value.Price);
                    int rowaffected = insertCommand.ExecuteNonQuery();
                    //Console.WriteLine($"rows affected: {rowaffected}");
                }
                //    Car newcar = new Car() { Id = GetId(), Model = value.Model, Vendor = value.Vendor, Price = value.Price };
                //carList.Add(newcar);
            }
        }
        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Car value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            //carList.Remove(Get(id));
        }

       int GetId()
        {
            //int max = carList.Max(x => x.Id);
            //return max+1;
            return 0;
        }

       private List<Car> getCarsFromDB(string sqlQuery)
       {
           var carList = new List<Car>();

           string selectall = sqlQuery;

           using (SqlConnection databaseConnection = new SqlConnection(conn))
           {
               using (SqlCommand selectCommand = new SqlCommand(selectall, databaseConnection))
               {
                   databaseConnection.Open();

                   using (SqlDataReader reader = selectCommand.ExecuteReader())
                   {
                       while (reader.Read())
                       {
                           int id = reader.GetInt32(0);
                           string vendor = reader.GetString(1);
                           string model = reader.GetString(2);
                           int price = reader.GetInt32(3);

                           carList.Add(new Car(id, vendor, model, price));

                       }

                   }
               }
           }

           return carList;
        }

    }
}
