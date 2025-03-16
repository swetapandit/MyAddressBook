using System;
using System.Collections.Generic;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using ModelLayer.Model;
using Newtonsoft.Json;

namespace BusinessLayer.Service
{
    public class ContactBL : IContactBL
    {
        private readonly IContactRL contactRL;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly int userId;
        private readonly IDatabase _redisDatabase;

        public ContactBL(IContactRL _contactRL, IHttpContextAccessor _httpContextAccessor, IConnectionMultiplexer redisConnection)
        {
            contactRL = _contactRL;
            httpContextAccessor = _httpContextAccessor;
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                userId = int.Parse(userIdClaim.Value);
            }
            _redisDatabase = redisConnection.GetDatabase(); // Get Redis database instance
        }

        public ResponseModel<ContactEntity> AddContact(CreateContactRequestModel contact)
        {
            var response = contactRL.AddContact(contact, userId);

            // Clear the cache after adding new contact
            _redisDatabase.KeyDelete($"contacts:{userId}");

            return response;
        }

        public List<ContactEntity> FetchAllContact()
        {
            // Check if the contacts are already cached in Redis
            var cachedContacts = _redisDatabase.StringGet($"contacts:{userId}");

            if (!cachedContacts.IsNullOrEmpty)
            {
                // If cached data is available, deserialize it and return
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContactEntity>>(cachedContacts);
            }

            // If no cache, fetch contacts from the database
            var contacts = contactRL.FetchAllContact(userId);

            // Cache the contacts in Redis for faster future access
            _redisDatabase.StringSet($"contacts:{userId}",
                                      Newtonsoft.Json.JsonConvert.SerializeObject(contacts),
                                      TimeSpan.FromMinutes(30)); // Cache expiry time is 30 minutes

            return contacts;
        }

        public ResponseModel<ContactEntity> FetchContactById(int id)
        {
            // Check if the contact is already cached in Redis
            var cachedContact = _redisDatabase.StringGet($"contact:{userId}:{id}");

            if (!cachedContact.IsNullOrEmpty)
            {
                // If cached data is available, deserialize it and return
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModel<ContactEntity>>(cachedContact);
            }

            // If no cache, fetch contact from the database
            var contact = contactRL.FetchContactById(id, userId);

            // Cache the contact in Redis for faster future access
            _redisDatabase.StringSet($"contact:{userId}:{id}",
                                      Newtonsoft.Json.JsonConvert.SerializeObject(contact),
                                      TimeSpan.FromMinutes(30)); // Cache expiry time is 30 minutes

            return contact;
        }

        public ResponseModel<ContactEntity> DeleteContactById(int id)
        {
            var response = contactRL.DeleteContactById(id, userId);

            // Clear the cache after deleting a contact
            _redisDatabase.KeyDelete($"contact:{userId}:{id}");
            _redisDatabase.KeyDelete($"contacts:{userId}");

            return response;
        }

        public ResponseModel<ContactEntity> UpdateContactById(int id, UpdateContactRequestModel contact)
        {
            var response = contactRL.UpdateContactById(id, contact, userId);

            // Clear the cache after updating the contact
            _redisDatabase.KeyDelete($"contact:{userId}:{id}");
            _redisDatabase.KeyDelete($"contacts:{userId}");

            return response;
        }
    }
}
