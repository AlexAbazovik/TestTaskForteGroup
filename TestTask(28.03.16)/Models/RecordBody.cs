using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TestTask_28._03._16_.Models;

namespace TestTask_28._03._16_.Models
{
    public class RecordBody
    {
        [Key]
        [ForeignKey("curRecord")]
        public int ID { get; set; }
        [Required(ErrorMessage = "Your ideas are no themes?")]
        [Display(Name = "Enter a subject of your idea:")]
        public string Theme { get; set; }
        [Required(ErrorMessage = "It is strange to create record about the idea if its not?")]
        [Display(Name = "Enter directly the idea:")]
        public string Idea { get; set; }
        [Required(ErrorMessage = "Describe your idea in more detail.")]
        [Display(Name="Enter the note:")]
        public string Note { get; set; }
        public Record curRecord { get; set; }
    }
}