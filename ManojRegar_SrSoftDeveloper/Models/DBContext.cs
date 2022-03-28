using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManojRegar_SrSoftDeveloper.Models
{
    public class DBContext : DbContext
    {
        public DBContext() : base("DefaultConnection") { }
        public DbSet<Students> tbl_Students { get; set; }
        public DbSet<Subjects> tbl_Subjects { get; set; }
    }

    public class Students
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Class { get; set; }
        public virtual ICollection<Subjects> Subjects { get; set; }
    }

    public class Subjects
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Subject Name")]
        public string Name { get; set; }
        [Required]
        public decimal Marks { get; set; }
        [Required]
        [Display(Name = "Student Name")]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; }
    }

    public class StudentListModel
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Class { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Marks { get; set; }
        public bool IsFirst { get; set; }
    }
}