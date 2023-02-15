﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAppCore.Entity.StudentManagement.Entity
{
    public partial class Student_Mark_List
    {
        [Key]
        public int Student_Id { get; set; }
        public int Reg_Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Student_Name { get; set; }
        public int Roll_No { get; set; }
        public int Tamil { get; set; }
        public int English { get; set; }
        public int Maths { get; set; }
        public int Science { get; set; }
        public int Social { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Average { get; set; }
        public int Total { get; set; }
        public bool Is_Deleted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created_Time_Stamp { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Updated_Time_Stamp { get; set; }

        [ForeignKey(nameof(Reg_Id))]
        [InverseProperty(nameof(Student_Registration.Student_Mark_List))]
        public virtual Student_Registration Reg_ { get; set; }
    }
}