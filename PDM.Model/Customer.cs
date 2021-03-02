using PDM.Data.Entity;
using PDM.Data.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDM.Model
{
    
    public class Customer : Base
    {
        string mobile = "";
        string dependentContactNumber = "";
        string alternativeContact = "";
        string landlineNumber = "";
        string email = "";
        Address address;
        public long CustomerId { get; set; }
        public long AddressId { get; set; }
        [DisplayName("National Health Service Number")]
        [Required(ErrorMessage = "NHS number is required")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Numbers & Alphabets are allowed")]
        public new string NHSNumber { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Numbers & Alphabets are allowed")]
        public new string FirstName { get; set; }

        [DisplayName("Middle name")]
        public new string MiddleName { get; set; }

        [DisplayName("Last name")]
        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Numbers & Alphabets are allowed")]
        public new string LastName { get; set; }

        [DisplayName("Gender")]
        public string Gender { get; set; }
        public string Title { get; set; }
        [DisplayName("DOB (dd/MM/yyyy)")]
        [Required(ErrorMessage = "DOB is required")]
        public System.DateTime? InputDob { get; set; }
        public System.DateTime Dob { get; set; }
        public System.DateTime? LastOrderDate { get; set; }
        public System.DateTime? NextOrderDate { get; set; }
        public int? PickupTypeId { get; set; }
        public int? RouteId { get; set; }
        public int IsActive { get; set; }
        public string Mobile {
            get
            {
                return mobile;
            }
            set
            {
                mobile = value;
            }
        }
        [DisplayName("Landline Number")]
        public string LandlineNumber
        {
            get
            {
                return landlineNumber;
            }
            set
            {
                landlineNumber = value;
                
            }
        }
        [DisplayName("Alternative Contact")]
        public string AlternativeContact
        {
            get
            {
                return alternativeContact;
            }
            set
            {
                alternativeContact = value;
            }
        }
        [DisplayName("Dependent Contact Number")]
        public string DependentContactNumber
        {
            get
            {
                return dependentContactNumber;
            }
            set
            {
                dependentContactNumber = value;
                
            }
        }

        [DisplayName("Email ID")]
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;               
            }
        }
        public Address Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                if(value!=null)
                {
                    if (Email != null)
                    {
                        address.Contacts.Add(new Contact
                        {
                            ContactTypeId = (int)ContactTypes.Email,
                            Value = email
                        });
                    }
                    if (DependentContactNumber != null)
                    {
                        address.Contacts.Add(new Contact
                        {
                            ContactTypeId = (int)ContactTypes.DependentContactNumber,
                            Value = dependentContactNumber
                        });
                    }
                    if (AlternativeContact != null)
                    {
                        address.Contacts.Add(new Contact
                        {
                            ContactTypeId = (int)ContactTypes.AlternativeContact,
                            Value = alternativeContact
                        });
                    }
                    if (LandlineNumber != null)
                    {
                        address.Contacts.Add(new Contact
                        {
                            ContactTypeId = (int)ContactTypes.LandLine,
                            Value = landlineNumber
                        });
                    }
                    if (Mobile != null)
                    {
                        address.Contacts.Add(new Contact
                        {
                            ContactTypeId = (int)ContactTypes.Mobile,
                            Value = mobile
                        });
                    }
                }
               
            }
        
        }
    }
}
