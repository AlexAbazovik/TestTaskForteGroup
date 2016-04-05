using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestTask_28._03._16_.Models
{
    public class Record
    {
        public int ID { get; set; }
        [Display(Name = "Date of last edit:")]
        public DateTime Date { get; set; }
        [Display(Name = "Author:")]
        public string Author { get; set; }
        public RecordBody Body { get; set; }
        public bool RemovedByUser { get; set; }
        public bool RemovedByAdmin { get; set; }
    }


}