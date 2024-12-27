﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Data.Common;
using System.Linq;
using DataLayer.BookApp.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class TestSqliteInMemory
    {
        private readonly ITestOutputHelper _output;

        public TestSqliteInMemory(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestSqliteOk()
        {
            //SETUP
            var options = SqliteInMemory         //#A
                .CreateOptions<BookContext>(); //#A
            using (var context = new BookContext(options)) //#B
            {
                using(new TimeThings(_output))
                    context.Database.EnsureCreated(); //#C

                //ATTEMPT
                context.SeedDatabaseFourBooks(); //#D

                //VERIFY
                context.Books.Count().ShouldEqual(4); //#E
            }
        }
        /*************************************************************
        #A Here I call my SqliteInMemory.CreateOptions to provide me with an in-memory database
        #B Now I use that option to create my application's DbContext
        #C I must call the context.Database.EnsureCreated(), which is a special method that creates a database using your application's DbContext and entity classes
        #D Here I run a test method I have written that adds four test books to the database
        #E Here I check that my SeedDatabaseFourBooks worked, and added four books to the database
         * *********************************************************/

        [Fact]
        public void TestSqliteDisposableOk()
        {
            //SETUP
            using var options = SqliteInMemory.CreateOptions<BookContext>();
            using var context = new BookContext(options);

            context.Database.EnsureCreated();

            //ATTEMPT
            context.SeedDatabaseFourBooks();

            //VERIFY
            context.Books.Count().ShouldEqual(4);
        }

        [Fact]
        public void TestSqliteLogToOk()
        {
            //SETUP
            using var options = SqliteInMemory.CreateOptions<BookContext>();
            using var context = new BookContext(options);

            context.Database.EnsureCreated();

            //ATTEMPT
            context.SeedDatabaseFourBooks();

            //VERIFY
            context.Books.Count().ShouldEqual(4);
        }


        [Fact]
        public void TestSqliteTwoInstancesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookContext>(); //#A
            options.StopNextDispose();
            using (var context = new BookContext(options))//#B
            {
                context.Database.EnsureCreated();//#C
                context.SeedDatabaseFourBooks(); //#C
            }
            using (var context = new BookContext(options))//#D
            {
                //ATTEMPT
                var books = context.Books.ToList(); //#E

                //VERIFY
                books.Last().Reviews.ShouldBeNull(); //#F
            }
        }
        /*************************************************************
        #A I create the in-memory sqlite options in the same was as the last example
        #B I create the first instance of the application's DbContext
        #C I create the database schema and then use my test method to write four books to the database
        #D I close that last instance and open a new instance of the application's DbContext. This means that the new instance does not have any tracked entities which could alter how the test runs
        #E I read in the books, without any includes
        #F The last book has two reviews, so I check it is null because I didn't have an Include on the query. NOTE this would FAIL if there was one instance
         * ***********************************************************/

        [Fact]
        public void TestSqliteThreeInstancesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookContext>();
            options.TurnOffDispose();
            using (var context = new BookContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new BookContext(options))
            {
                //ATTEMPT
                var books = context.Books.ToList();

                //VERIFY
                books.Last().Reviews.ShouldBeNull();
            }
            using (var context = new BookContext(options))
            {
                //ATTEMPT
                var books = context.Books.ToList();

                //VERIFY
                books.Last().Reviews.ShouldBeNull();
            }
            options.ManualDispose();
        }

        [Fact]
        public void TestSqliteOneInstanceWithChangeTrackerClearOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookContext>();
            using var context = new BookContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var books = context.Books.ToList();

            //VERIFY
            books.Last().Reviews.ShouldBeNull();
        }

        [Fact]
        public void TestSqliteSingleInstanceOk()
        {
            //SETUP
            var options = SqliteInMemory
                .CreateOptions<BookContext>();
            using (var context = new BookContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var books = context.Books.ToList();

                //VERIFY
                books.Last().Reviews.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestAddExtraOption()
        {
            //SETUP
            var options1 = SqliteInMemory.CreateOptions<BookContext>();
            options1.StopNextDispose();
            DbConnection connection;
            using (var context = new BookContext(options1))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                connection = context.Database.GetDbConnection();

                var book = context.Books.First();
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            }
            //ATTEMPT
            var options2 = SqliteInMemory.CreateOptions<BookContext>(builder =>
            {
                builder.UseSqlite(connection);
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            using (var context = new BookContext(options2))
            {
                //VERIFY
                var book = context.Books.First();
                context.Entry(book).State.ShouldEqual(EntityState.Detached);
            }
        }
    }
}
