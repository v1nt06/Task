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
            var x = 10000;
            var customers = dataSource.Customers.Where(c => c.Orders.Select(o => o.Total).Sum() > x);

            Console.WriteLine($"Customers with total > {x}");
            foreach (var customer in customers)
            {
                Console.WriteLine($"Customer ID: {customer.CustomerID}; Total: {customer.Orders.Select(o => o.Total).Sum()}");
            }
            Console.WriteLine();

            x = 50000;
            customers = dataSource.Customers.Where(c => c.Orders.Select(o => o.Total).Sum() > x);
            Console.WriteLine($"Customers with total > {x}");
            foreach (var customer in customers)
            {
                Console.WriteLine($"Customer ID: {customer.CustomerID}; Total: {customer.Orders.Select(o => o.Total).Sum()}");
            }
            Console.WriteLine();

            x = 100000;
            customers = dataSource.Customers.Where(c => c.Orders.Select(o => o.Total).Sum() > x);
            Console.WriteLine($"Customers with total > {x}");
            foreach (var customer in customers)
            {
                Console.WriteLine($"Customer ID: {customer.CustomerID}; Total: {customer.Orders.Select(o => o.Total).Sum()}");
            }
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
            var customers = dataSource.Customers.Where(c => string.IsNullOrWhiteSpace(c.CustomerID) || string.IsNullOrWhiteSpace(c.Region)
                || !c.Phone.StartsWith("("));

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerID} - {customer.Region} - {customer.Phone}");
            }
        }

        [Category("Join operators")]
        [Title("Join - Task 1")]
        [Description("Create list of suppliers where suppliers located in the same country and same city as customer's country and city")]
        public void Linq2()
        {
            var result1 = dataSource.Customers.Join(dataSource.Suppliers, c => c.City, s => s.City, (c, s) => new
            {
                CustomerId = c.CustomerID,
                CustomerCity = c.City,
                SupplierCity = s.City,
                SupplierName = s.SupplierName
            });

            foreach (var element in result1)
            {
                Console.WriteLine($"{element.CustomerId} - {element.CustomerCity} - {element.SupplierCity} - {element.SupplierName}");
            }

            Console.WriteLine();

            var result2 = dataSource.Customers.GroupJoin(dataSource.Suppliers, c => c.City, s => s.City, (c, s) => new
            {
                CustomerId = c.CustomerID,
                CustomerCity = c.City,
                Suppliers = s
            });

            foreach (var element in result2)
            {
                Console.WriteLine($"{element.CustomerId} - {element.CustomerCity}:");
                foreach (var supplier in element.Suppliers)
                {
                    Console.WriteLine($" {supplier.City} - {supplier.SupplierName}");
                }
            }
        }

        [Category("Ordering operators")]
        [Title("OrderBy - Task 1")]
        [Description("Show client list with date of first order")]
        public void Linq4()
        {
            foreach (var customer in dataSource.Customers)
            {
                Console.WriteLine($"{customer.CustomerID} - {customer.Orders.OrderBy(o => o.OrderDate).FirstOrDefault()?.OrderDate}");
            }
        }

        [Category("Ordering operators")]
        [Title("OrderBy - Task 2")]
        [Description("Show client list with date of the first order ordered by: date, total sum, client")]
        public void Linq5()
        {
            var customers = dataSource.Customers.OrderByDescending(c => c.Orders.OrderBy(o => o.OrderDate).FirstOrDefault()?.OrderDate);
            Console.WriteLine("Sorted by date of first order:");
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerID} - {customer.Orders.OrderBy(o => o.OrderDate).FirstOrDefault()?.OrderDate}");
            }

            Console.WriteLine();
            Console.WriteLine("Sorted by total sum:");
            customers = dataSource.Customers.OrderByDescending(c => c.Orders.Select(o => o.Total).Sum());
            foreach (var customer in customers)
            {
                var dateOfFirstOrder = customer.Orders.OrderBy(o => o.OrderDate).FirstOrDefault()?.OrderDate;
                Console.WriteLine($"{customer.CustomerID} - {dateOfFirstOrder} - {customer.Orders.Select(o => o.Total).Sum()}");
            }

            Console.WriteLine();
            Console.WriteLine("Sorted by client");
            customers = dataSource.Customers.OrderByDescending(c => c.CustomerID);
            foreach (var customer in customers)
            {
                var dateOfFirstOrder = customer.Orders.OrderBy(o => o.OrderDate).FirstOrDefault()?.OrderDate;
                Console.WriteLine($"{customer.CustomerID} - {dateOfFirstOrder}");
            }
        }

        [Category("Grouping operators")]
        [Title("GroupBy - Task 1")]
        [Description("Group products by category, then by units count, then last group sort by price")]
        public void Linq7()
        {
            var result =
                dataSource.Products.GroupBy(p => p.Category)
                    .Take(dataSource.Products.GroupBy(p => p.Category).Count() - 1)
                    .Select(g => g.OrderBy(p => p.UnitsInStock))
                    .Union(new List<IOrderedEnumerable<Product>>
                    {
                        dataSource.Products.GroupBy(p => p.Category).Last().OrderBy(p => p.UnitPrice)
                    });

            foreach (var product in result.SelectMany(element => element))
            {
                Console.WriteLine($"{product.ProductName} - {product.Category} - {product.UnitsInStock} - {product.UnitPrice}");
            }
        }
    }
}
