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

        [Category("Join operators")]
        [Title("Join - Task 1")]
        [Description("Create list of suppliers where suppliers located in the same country and same city as customer's country and city")]
        public void Linq2()
        {
            var customers = dataSource.Customers;
            var suppliers = dataSource.Suppliers;

            var result1 = customers.Join(suppliers, c => c.City, s => s.City, (c, s) => new {
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

            var result2 = customers.GroupJoin(suppliers, c => c.City, s => s.City, (c, s) => new {
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


    }
}
