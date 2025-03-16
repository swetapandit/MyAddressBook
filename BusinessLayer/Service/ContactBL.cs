using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace BusinessLayer.Service
{
    public class ContactBL : IContactBL
    {
        private readonly IContactRL contactRL;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly int userId;
        public ContactBL(IContactRL _contactRL, IHttpContextAccessor _httpContextAccessor)
        {
            contactRL = _contactRL;
            httpContextAccessor = _httpContextAccessor;
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                userId = int.Parse(userIdClaim.Value);
            }
        }

        public ResponseModel<ContactEntity> AddContact(CreateContactRequestModel contact)
        {
            return contactRL.AddContact(contact, userId);
        }

        public List<ContactEntity> FetchAllContact()
        {
            return contactRL.FetchAllContact(userId);
        }

        public ResponseModel<ContactEntity> FetchContactById(int id)
        {
            return contactRL.FetchContactById(id,userId);
        }

        public ResponseModel<ContactEntity> DeleteContactById(int id)
        {
            return contactRL.DeleteContactById(id, userId);
        }

        public ResponseModel<ContactEntity> UpdateContactById(int id, UpdateContactRequestModel contact)
        {
            return contactRL.UpdateContactById(id, contact, userId);
        }
    }
}