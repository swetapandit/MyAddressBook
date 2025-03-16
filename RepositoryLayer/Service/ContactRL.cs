using System;
using ModelLayer.Model;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class ContactRL : IContactRL
    {
        private readonly AddressBookContext addressBookContext;
        private readonly EmailValidator emailValidator;

        public ContactRL(AddressBookContext _addressBookContext, EmailValidator _emailValidator)
        {
            addressBookContext = _addressBookContext;
            emailValidator = _emailValidator;
        }


        // Add a new Contact
        public ResponseModel<ContactEntity> AddContact(CreateContactRequestModel contact, int userId)
        {
            if (!emailValidator.IsValidEmail(contact.Email))
            {
                return new ResponseModel<ContactEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid email format.",
                    StatusCode = 400 // Bad Request
                };
            }
            var existingContact = addressBookContext.Contacts.FirstOrDefault(g => g.PhoneNumber == contact.PhoneNumber && g.OwnerId == userId);
            if (existingContact != null) return null;
            var owner = addressBookContext.Users.FirstOrDefault(g => g.Id == userId);

            var newContact = new ContactEntity
            {
                Name = contact.Name,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                OwnerId = userId,
                Owner = owner!
            };
            addressBookContext.Contacts.Add(newContact);
            addressBookContext.SaveChanges(); 
            return new ResponseModel<ContactEntity>
            {
                Data = newContact,
                Success = true,
                Message = "Contact Added Successfully.",
                StatusCode = 200 // OK
            };
        }

        // Fetch all contact
        public List<ContactEntity> FetchAllContact(int userId)
        {
            return addressBookContext.Contacts.Where(c => c.OwnerId == userId).ToList();
        }

        //Fetch Contact by Id
        public ResponseModel<ContactEntity> FetchContactById(int id, int userId)
        {
            var contact = addressBookContext.Contacts.FirstOrDefault(c => c.Id == id && c.OwnerId == userId);

            if (contact == null)
            {
                return new ResponseModel<ContactEntity>
                {
                    Success = false,
                    Message = "Contact not found",
                    Data = null
                };
            }

            return new ResponseModel<ContactEntity>
            {
                Success = true,
                Message = "Contact fetched successfully",
                Data = contact
            };
        }


        //Delete Contact by Id
        public ResponseModel<ContactEntity> DeleteContactById(int id, int userId)
        {
            var contact = addressBookContext.Contacts.FirstOrDefault(c => c.Id == id && c.OwnerId == userId);

            if (contact == null)
            {
                return new ResponseModel<ContactEntity>
                {
                    Success = false,
                    Message = "Contact not found",
                    Data = null
                };
            }

            addressBookContext.Contacts.Remove(contact);
            addressBookContext.SaveChanges();

            return new ResponseModel<ContactEntity>
            {
                Success = true,
                Message = "Contact Deleted successfully",
                Data = contact
            };
        }

        public ResponseModel<ContactEntity> UpdateContactById(int id, UpdateContactRequestModel contact,int userId)
        {
            if (contact.Email != null && !emailValidator.IsValidEmail(contact.Email))
            {
                return new ResponseModel<ContactEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid email format.",
                    StatusCode = 400 // Bad Request
                };
            }

            var updatedContact = addressBookContext.Contacts.FirstOrDefault(c => c.Id == id && c.OwnerId == userId);
            if (updatedContact == null)
            {
                return new ResponseModel<ContactEntity>
                {
                    Success = false,
                    Message = "Contact not found",
                    Data = null
                };
            }

            if (contact.Name != null) updatedContact.Name = contact.Name;
            if (contact.Email != null) updatedContact.Email = contact.Email;
            if (contact.PhoneNumber != null) updatedContact.PhoneNumber = contact.PhoneNumber;
            addressBookContext.SaveChanges();

            


            return new ResponseModel<ContactEntity>
            {
                Success = true,
                Message = "Contact Updated successfully",
                Data = updatedContact
            };
        }
        
    }
}