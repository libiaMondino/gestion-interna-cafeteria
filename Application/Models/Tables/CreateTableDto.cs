using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Tables
{
    public class CreateTableDto
    {
        public int Number { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
