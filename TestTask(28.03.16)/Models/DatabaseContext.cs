using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TestTask_28._03._16_.Models
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext():
            base("DefaultConnection")
        {

        }
        public DbSet<RecordBody> RecordBodies { get; set; }
        public DbSet<Record> Records { get; set; }
    }
}