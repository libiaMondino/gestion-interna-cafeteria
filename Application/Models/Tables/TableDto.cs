using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
namespace Application.Models.Tables
{
    public class TableDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public bool Status { get; set; } 
    }
}
