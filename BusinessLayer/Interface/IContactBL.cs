using System;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IContactBL
    {
        ResponseModel<ContactEntity> AddContact(CreateContactRequestModel contact);
        List<ContactEntity> FetchAllContact();
        ResponseModel<ContactEntity> FetchContactById(int id);
        ResponseModel<ContactEntity> DeleteContactById(int id);
        ResponseModel<ContactEntity> UpdateContactById(int id, UpdateContactRequestModel contact);
        
    }
}