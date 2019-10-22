using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TRS.Models
{
    public class Customer
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedDate { get; set; }

        public ICollection<CustomerStructure> Customers { get; set; }

    }

    public class CustomerStructure
    {

        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 2)]
        //Foreign key for Customer
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int SeqNo { get; set; }
        public string Name { get; set; }
        public string ArName { get; set; }

        public bool IsTID { get; set; }

        public Nullable<int> PerentId { get; set; }



    }



    public class CustomerStructureToUser
    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int NodeId { get; set; }
        [Required]
        public virtual int CustomerId { get; set; }

    }


}