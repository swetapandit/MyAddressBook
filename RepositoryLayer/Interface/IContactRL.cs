using System;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IContactRL
    {
        ResponseModel<ContactEntity> AddContact(CreateContactRequestModel contact, int userId);
        List<ContactEntity> FetchAllContact(int userId);
        ResponseModel<ContactEntity> FetchContactById(int id, int userID);
        ResponseModel<ContactEntity> DeleteContactById(int id, int userId); 
        ResponseModel<ContactEntity> UpdateContactById(int id, UpdateContactRequestModel contact, int userId);
    }
}