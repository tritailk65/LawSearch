using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class MessageModel
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }

        public string Function { get; set; }

        public MessageModel()
        {
            
        }
        public MessageModel(int status, string message, string description, string function)
        {
            Status = status;
            Message = message;
            Description = description;
            Function = function;
                    
        }

    }
}
