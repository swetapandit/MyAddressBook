----------------------------Address Book API---------------------------------------------

------------------------------Introduction-----------------------------------------------

-This API provides functionality for managing users and their contacts. 
-It supports user authentication, contact management, and password recovery features.

-----------------------------Technologies Used-------------------------------------------

-C# (.NET Core) for backend development

-Entity Framework Core for database interaction

-JWT Authentication for secure login and authorization

-SMTP (Gmail) for sending emails

-BCrypt for password hashing

-MSSQL as the database

-------------------------------API Endpoints---------------------------------------------

-User Authentication

-Register User

	-Endpoint: POST /api/user/register

	-Description: Registers a new user.

	-Request Body:

	{
  		"name": "John Doe",
  		"email": "johndoe@example.com",
  		"password": "securepassword"
	}

	-Response: Returns user details on successful registration.

-Login User

	-Endpoint: POST /api/user/login

	-Description: Authenticates a user and returns a JWT token.

	-Request Body:

	{
	  "email": "johndoe@example.com",
	  "password": "securepassword"
	}

	-Response: Returns a JWT token upon successful authentication.

-Forgot Password

	-Endpoint: POST /api/user/forgot-password

	-Description: Sends an OTP to the user's email for password reset.

	-Request Body:

	{
	  "email": "johndoe@example.com"
	}

	-Response: Confirms OTP sent.

-Reset Password

	-Endpoint: POST /api/user/reset-password

	-Description: Resets the user's password using OTP verification.

	-Request Body:

	{
	  "email": "johndoe@example.com",
	  "otp": 123456,
	  "password": "newpassword"
	}

	-Response: Confirms password reset.

--------------------------------------Contact Management---------------------------------------------

-Add Contact

	-Endpoint: POST /api/contact

	-Description: Adds a new contact for the logged-in user.

	-Headers: Authorization: Bearer <token>

	-Request Body:

	{
	  "name": "Alice Smith",
	  "email": "alice@example.com",
	  "phoneNumber": "9876543210"
	}

	-Response: Returns the added contact details.

-Get All Contacts

	-Endpoint: GET /api/contact

	-Description: Retrieves all contacts for the logged-in user.

	-Headers: Authorization: Bearer <token>

	-Response: List of contacts.

-Get Contact by ID

	-Endpoint: GET /api/contact/{id}

	-Description: Retrieves details of a specific contact.

	-Headers: Authorization: Bearer <token>

	-Response: Contact details.

-Update Contact

	-Endpoint: PUT /api/contact/{id}

	-Description: Updates a contact's details.

	-Headers: Authorization: Bearer <token>

	-Request Body:

	{
	  "name": "Updated Name",
	  "email": "updated@example.com",
	  "phoneNumber": "1234567890"
	}

	-Response: Updated contact details.

-Delete Contact

	-Endpoint: DELETE /api/contact/{id}

	-Description: Deletes a contact.

	-Headers: Authorization: Bearer <token>

	-Response: Deletion confirmation.

-------------------------------------Security and Authentication-----------------------------------

-JWT Token is used for authentication.

-Users need to send the token in the Authorization header as Bearer <token>.

-Passwords are securely stored using BCrypt hashing.

-------------------------------------------Email Service--------------------------------------------

-Uses SMTP (Gmail) to send emails for password recovery.

-The IEmailSender service handles email sending.

------------------------------------------Database Structure----------------------------------------

-User Table

-Column
-------------------
Id : int

Name : string

Email : string

Password : string

Otp : int
--------------------
-Contact Table

-Column
--------------------
Id : int

Name : string

Email : string

PhoneNumber : string

OwnerId : int
----------------------
