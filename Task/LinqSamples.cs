// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {
        private DataSource dataSource = new DataSource();

        [Category("Restriction operators")]
        [Title("Where - Task 1")]
        [Description("List of all clients, whose total turnover exceeds some value X")]
        public void Linq1()
        {
            var customers = GetCustomersWhoseByMinimumTurnover(10000);

            var x = 10000;
            Console.WriteLine($"Customers with total > {x}");
            foreach (var customer in customers)
            {
                Console.WriteLine(
                    $"Customer ID: {customer.CustomerID}; Total: {customer.Orders.Select(o => o.Total).Sum()}");
            }
            Console.WriteLine();

            x = 50000;
            customers = GetCustomersWhoseByMinimumTurnover(50000);
            Console.WriteLine($"Customers with total > {x}");
            foreach (var customer in customers)
            {
                Console.WriteLine(
                    $"Customer ID: {customer.CustomerID}; Total: {customer.Orders.Select(o => o.Total).Sum()}");
            }
            Console.WriteLine();

            x = 100000;
            customers = GetCustomersWhoseByMinimumTurnover(100000);
            Console.WriteLine($"Customers with total > {x}");
            foreach (var customer in customers)
            {
                Console.WriteLine(
                    $"Customer ID: {customer.CustomerID}; Total: {customer.Orders.Select(o => o.Total).Sum()}");
            }
        }

        private IEnumerable<Customer> GetCustomersWhoseByMinimumTurnover(int turnover)
        {
            return dataSource.Customers.Where(c => c.Orders.Select(o => o.Total).Sum() > turnover);
        }

        [Category("Restriction operators")]
        [Title("Where - Task 2")]
        [Description("Find all customers with orders where total bigger than X")]
        public void Linq3()
        {
            var x = 2000;
            var customers = dataSource.Customers.Where(c => c.Orders.Any(o => o.Total > x));

            foreach (var customer in customers)
            {
                var order = customer.Orders.First(o => o.Total > x);
                Console.WriteLine($"{customer.CustomerID}: OrderId: {order.OrderID} - Total: {order.Total}");
            }
        }

        [Category("Restriction operators")]
        [Title("Where - Task 3")]
        [Description("Show clients without code or without region or without code of phone number")]
        public void Linq6()
        {
            var customers = dataSource.Customers.Where(c =>
            {
                if (!string.IsNullOrEmpty(c.PostalCode) && Regex.IsMatch(c.PostalCode, @"\D"))
                {
                    return true;
                }

                if (string.IsNullOrEmpty(c.Region))
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(c.Phone) && !c.Phone.StartsWith("("))
                {
                    return true;
                }

                return false;
            });

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerID} - {customer.PostalCode} - {customer.Region} - {customer.Phone}");
            }
        }

        [Category("Join operators")]
        [Title("Join - Task 1")]
        [Description(
            "Create list of suppliers where suppliers located in the same country and same city as customer's country and city"
            )]
        public void Linq2()
        {
            var result1 = dataSource.Customers.Join(dataSource.Suppliers, c => new { c.City, c.Country },
                s => new { s.City, s.Country },
                (c, s) =>
                    new
                    {
                        CustomerId = c.CustomerID,
                        CustomerCity = c.City,
                        CustomerCountry = c.Country,
                        s.SupplierName
                    });

            foreach (var element in result1)
            {
                Console.WriteLine(
                    $"{element.CustomerId} - {element.CustomerCity} - {element.CustomerCountry} - {element.SupplierName}");
            }

            Console.WriteLine();

            var result2 = result1.GroupBy(e => new { e.CustomerCountry, e.CustomerCity });
            foreach (var element in result2.SelectMany(@group => @group))
            {
                Console.WriteLine(
                    $"{element.CustomerCountry} - {element.CustomerCity} - {element.CustomerId} - {element.SupplierName}");
            }
        }

        [Category("Ordering operators")]
        [Title("OrderBy - Task 1")]
        [Description("Show client list with date of first order")]
        public void Linq4()
        {
            foreach (var customer in dataSource.Customers)
            {
                Console.WriteLine(
                    $"{customer.CustomerID} - {customer.Orders.OrderBy(o => o.OrderDate).FirstOrDefault()?.OrderDate}");
            }
        }

        [Category("Ordering operators")]
        [Title("OrderBy - Task 2")]
        [Description("Show client list with date of the first order ordered by: date, total sum, client")]
        public void Linq5()
        {
            var customers =
                dataSource.Customers.OrderBy(c => c.Orders.OrderBy(o => o.OrderDate.Year).FirstOrDefault()?.OrderDate.Year)
                    .ThenBy(c => c.Orders.OrderBy(o => o.OrderDate.Year).ThenBy(o => o.OrderDate.Month).FirstOrDefault()?.OrderDate.Month)
                    .ThenByDescending(c => c.Orders.Sum(o => o.Total))
                    .ThenBy(c => c.CustomerID);
            foreach (var customer in customers)
            {
                var year = customer.Orders.OrderBy(o => o.OrderDate.Year).FirstOrDefault()?.OrderDate.Year;
                var month = customer.Orders.OrderBy(o => o.OrderDate.Year).FirstOrDefault()?.OrderDate.Month;
                var total = customer.Orders.Sum(o => o.Total);
                var customerId = customer.CustomerID;
                Console.WriteLine($"{year} - {month} - {total} - {customerId}");
            }
        }

        [Category("Grouping operators")]
        [Title("GroupBy - Task 1")]
        [Description("Group products by category, then by units count, then last group sort by price")]
        public void Linq7()
        {
            var result = dataSource.Products.Select(p => new { p.Category, p.UnitsInStock, p.ProductName, Cost = p.UnitPrice * p.UnitsInStock })
                .OrderBy(e => e.Cost)
                .GroupBy(e => new { e.Category, e.UnitsInStock })
                .OrderBy(g => g.Key.Category)
                .ThenBy(g => g.Key.UnitsInStock)
                .GroupBy(g => g.Key.Category);

            foreach (var categoryGroup in result)
            {
                Console.WriteLine($"Category = {categoryGroup.Key}");
                foreach (var unitsInStockGroup in categoryGroup)
                {
                    Console.WriteLine($"    UnitsInStock = {unitsInStockGroup.Key.UnitsInStock}");
                    foreach (var element in unitsInStockGroup)
                    {
                        Console.WriteLine($"        ProductName = {element.ProductName}, Cost = {element.Cost}");
                    }
                }
            }
        }

        [Category("Grouping operators")]
        [Title("GroupBy - Task 2")]
        [Description("Group products by 3 groups: cheap, normal, costy")]
        public void Linq8()
        {
            var result = dataSource.Products.GroupBy(p =>
            {
                if (p.UnitPrice < 10)
                {
                    return "cheap";
                }

                return p.UnitPrice < 20 ? "normal" : "costy";
            });

            foreach (var element in result)
            {
                foreach (var product in element)
                {
                    Console.WriteLine($"{product.ProductName} - {product.UnitPrice} - {element.Key}");
                }
            }
        }

        [Category("Conversion operators")]
        [Title("ToDictionary - Task 1")]
        [Description("Calculate average profit and average intensity for every city")]
        public void Linq9()
        {
            var averages = dataSource.Customers.Select(c => c.City)
                .Distinct()
                .ToDictionary(city => city,
                    city =>
                        dataSource.Customers.Where(c => c.City == city).SelectMany(c => c.Orders).Average(o => o.Total));

            Console.WriteLine("Average profit per city:");
            foreach (var average in averages)
            {
                Console.WriteLine($"{average.Key} - {average.Value:C}");
            }

            Console.WriteLine();

            var intensities = dataSource.Customers.Select(c => c.City)
                .Distinct()
                .ToDictionary(city => city,
                    city =>
                        (float)dataSource.Customers.Where(c => c.City == city).SelectMany(c => c.Orders).Count() /
                        dataSource.Customers.Count(c => c.City == city));

            Console.WriteLine("Average intensity per city:");
            foreach (var intensity in intensities)
            {
                Console.WriteLine($"{intensity.Key} - {intensity.Value:F}");
            }
        }

        [Category("Conversion operators")]
        [Title("ToDictionary - Task 2")]
        [Description("Show statistic about activity of clients by months, by years, by years and months")]
        public void Linq10()
        {
            var ordersPerMonths =
                dataSource.Customers.SelectMany(c => c.Orders)
                    .Select(o => o.OrderDate.Month)
                    .Distinct()
                    .ToDictionary(month => month,
                        month => dataSource.Customers.SelectMany(c => c.Orders).Count(o => o.OrderDate.Month == month))
                    .OrderBy(o => o.Key);

            Console.WriteLine("Count of orders per months:");
            foreach (var ordersPerMonth in ordersPerMonths)
            {
                Console.WriteLine($"Month: {ordersPerMonth.Key}, Orders count: {ordersPerMonth.Value}");
            }

            Console.WriteLine();

            var ordersPerYears =
                dataSource.Customers.SelectMany(c => c.Orders)
                    .Select(o => o.OrderDate.Year)
                    .Distinct()
                    .ToDictionary(year => year,
                        year => dataSource.Customers.SelectMany(c => c.Orders).Count(o => o.OrderDate.Year == year))
                    .OrderBy(o => o.Key);

            Console.WriteLine("Count of orders per year:");
            foreach (var ordersPerYear in ordersPerYears)
            {
                Console.WriteLine($"Year: {ordersPerYear.Key}, Orders count: {ordersPerYear.Value}");
            }

            Console.WriteLine();

            var ordersPerMonthsPerYears =
                dataSource.Customers.SelectMany(c => c.Orders)
                    .Select(o => o.OrderDate.Year)
                    .Distinct()
                    .ToDictionary(year => year,
                        year =>
                            dataSource.Customers.SelectMany(c => c.Orders)
                                .Where(o => o.OrderDate.Year == year)
                                .Select(o => o.OrderDate.Month)
                                .Distinct()
                                .ToDictionary(month => month,
                                    month =>
                                        dataSource.Customers.SelectMany(c => c.Orders)
                                            .Count(o => o.OrderDate.Year == year && o.OrderDate.Month == month))
                                .OrderBy(o => o.Key))
                    .OrderBy(o => o.Key);

            Console.WriteLine("Count of orders per year per month:");
            foreach (var ordersPerMonthsPerYear in ordersPerMonthsPerYears)
            {
                Console.WriteLine($"Year: {ordersPerMonthsPerYear.Key}");
                foreach (var ordersPerMonthPerYear in ordersPerMonthsPerYear.Value)
                {
                    Console.WriteLine($"    Month: {ordersPerMonthPerYear.Key}, Orders count: {ordersPerMonthPerYear.Value}");
                }
            }
        }
    }
}
