using Microsoft.AspNetCore.Mvc.Rendering;
using PDM.Data.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PDM.Model
{
    public class User : Base
    {
        string mobile = "";
        string dependentContactNumber = "";
        string alternativeContact = "";
        string landlineNumber = "";
        string email = "";
        Address address;
        public long UserId { get; set; }
        public int IsActive { get; set; }
        [Required(ErrorMessage = "User name is required")]
        [RegularExpression(@"^[a-zA-Z-_.@0-9]*$", ErrorMessage = "Numbers & Alphabets are allowed")]
        [ScaffoldColumn(false)]
        [StringLength(100, ErrorMessage = "User name must be minimum of 10 characters", MinimumLength = 10)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^[A-Za-z0-9!@#$%^&*](?=.*[A-Za-z])(?=.*\d)[A-Za-z0-9!@#$%^&*\d]{7,}$", ErrorMessage = "Password must be minimum of 8 characters and contain at least 1 number and 1 letter")]
        [StringLength(100, ErrorMessage = "Password must be minimum of 8 characters and contain at least 1 number and 1 letter", MinimumLength = 8)]
        public string Password { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Numbers & Alphabets are allowed")]

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [DisplayName("Last name")]
        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Numbers & Alphabets are allowed")]

        public string LastName { get; set; }
        public long AddressId { get; set; }
        public long? BranchAdminId { get; set; }
        public string Comments { get; set; }

        [Required(ErrorMessage = "Please enter licenese no")]
        public string LicenseId { get; set; }
        [Required(ErrorMessage = "Please select the role")]
        public int UserTypeId { get; set; }

        [Required(ErrorMessage = "Please select the document")]
        public long DoucmentId { get; set; }
        [Required(ErrorMessage = "Please select the route")]

        public long RouteId { get; set; }
        [Required(ErrorMessage = "Please select the country")]

        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> DocumentType { get; set; }
        public List<SelectListItem> Route { get; set; }
        public List<SelectListItem> Country { get; set; }

        [Required(ErrorMessage = "Please enter mobile number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid mobile number")]
        public string Mobile
        {
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
        [Required(ErrorMessage = "Please enter email id")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
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
                if (value != null)
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
