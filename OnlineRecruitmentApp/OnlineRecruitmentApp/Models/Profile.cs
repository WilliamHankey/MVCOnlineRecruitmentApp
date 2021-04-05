using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineRecruitmentApp.Models
{
    public class Profile
    {
        [Key]
        public virtual int ProfileId { get; set; } // PK
        public virtual string FullName { get; set; }
        public virtual int Age { get; set; }
        public virtual string Skills { get; set; }
        public virtual int YearsOfExperince { get; set; }
        public virtual bool ShowCV { get; set; }
    }
}