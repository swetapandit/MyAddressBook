using System;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBook.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class ContactController : ControllerBase
    {
        private readonly IContactBL contactBL;
        public ContactController(IContactBL _contactBL)
        {
            contactBL = _contactBL;
        }

        //Add a new Contact
        [Authorize]
        [HttpPost("addressbook")]
        public IActionResult AddContact(CreateContactRequestModel contact)
        {
            var result = contactBL.AddContact(contact);
            if (result == null)
            {
                return BadRequest(new { message = "PhoneNumber already exists!" });
            }
            return Ok(result);
        }

        // Fetch all Contact
        [Authorize]
        [HttpGet("addressbook")]
        public IActionResult FetchAllContact()
        {
            var result = contactBL.FetchAllContact();
            if (result == null) return BadRequest(new { message = "No Contact Exixts" });
            return Ok(result);
        }

        // Fetch Contact by Id
        [Authorize]
        [HttpGet("addressbook/{id}")]
        public IActionResult GetContactById(int id)
        {
            var response = contactBL.FetchContactById(id);

            if (!response.Success)
            {
                return NotFound(new { Message = response.Message });
            }

            return Ok(response);
        }

        // PUT Contact by Id
        [Authorize]
        [HttpPut("addressbook/{id}")]
        public IActionResult UpdateContactById(int id, UpdateContactRequestModel contact)
        {
            var response = contactBL.UpdateContactById(id, contact);

            if (!response.Success)
            {
                return NotFound(new { Message = response.Message });
            }

            return Ok(response);
        }

        // Delete Contact by Id
        [Authorize]
        [HttpDelete("addressbook/{id}")]
        public IActionResult DeleteContactById(int id)
        {
            var response = contactBL.DeleteContactById(id);

            if (!response.Success)
            {
                return NotFound(new { Message = response.Message });
            }

            return Ok(response);
        }
    }
}