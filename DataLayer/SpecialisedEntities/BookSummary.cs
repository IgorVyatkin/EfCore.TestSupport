﻿// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace DataLayer.SpecialisedEntities
{
    public class BookSummary
    {
        public int BookSummaryId { get; set; }

        public string Title { get; set; }

        public string AuthorsString { get; set; }

        public BookDetail Details { get; set; }
    }
}