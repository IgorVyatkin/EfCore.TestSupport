﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.BookApp;
using DataLayer.BookApp.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class ExampleTest
    {

        private readonly ITestOutputHelper _output;

        public ExampleTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestExample()
        {
            //SETUP
            var logs = new List<string>();
            var options = this.CreateUniqueClassOptionsWithLogTo<BookContext>(log => logs.Add(log));
            using (var context = new BookContext(options))
            {
                context.Database.EnsureClean();
                logs.Clear();

                //ATTEMPT
                context.Add(new Book {Title = "New Book"});
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(1);
                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}