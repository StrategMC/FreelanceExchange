using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace FreelanceBirga.Models.DB
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string Content { get;set; }
        public bool Sender { get; set; }
        public DateTime SendTime {  get; set; }
    }
}
