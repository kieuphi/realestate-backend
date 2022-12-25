using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class CommonAttachmentModel
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public CommonAttachmentModel()
        {

        }
       
    }
}
