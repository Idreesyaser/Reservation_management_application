## Introduction
This document provides an overview of the reservation management application's functionality. The program's capabilities and limitations are presented window by window, detailing each field and action available to the user.

## Login
### Database
- Username and password are required for login.
- After successful login, the user is directed to the program's homepage.
- Admin account username: "admin", password: "admin". Remove this account before final deployment.
- The database creation script, sample data, and setup user account can be found in the project's SQL folder
- The database connection requires the HeidiSQL database interface with a user named "student" and the password "student1". Ensure the user has the necessary permissions.

### Login Window
Users are given a username and password to log in. Usernames and passwords can include letters, numbers, and special characters. Usernames are limited to 50 characters and passwords to 30 characters. Successful login directs users to the home page; unsuccessful attempts return an error message. The login window size is fixed.

An admin account is created for setup purposes. It should be deleted via user management before final deployment:
- Username: "admin"
- Password: "admin"
- 
## User Permissions
- Users have different levels of access rights. Standard users cannot manage branches, rooms, or users; only administrators can.
- When making a reservation, the user who is logged in will be recorded as the creator of the reservation in the database.

## Home Window
Upon logging in, users arrive at the home window. The interface remains consistent across windows, with a sidebar for navigation. The sidebar allows selection of various functions. The minimum window size is 500x850 pixels, but it can be resized by dragging the edges.

## Make a Reservation
Selecting "Make a Reservation" allows users to create a new reservation in the system. Note that a customer must be created before a reservation can be made. Once a customer is selected from the dropdown, users can choose the location, branch, and time slot for the reservation. Available rooms and services are displayed, with options to add additional services and notes.

## Customers
### Add New Customer
Users can add new customers by filling in the required fields and clicking "Add". Field restrictions include:
- Name: 50 characters
- Phone Number: 15 characters
- Address: 50 characters
- Postal Code: 5 characters
- City: 30 characters
- Email: 40 characters

### Delete Customer
- Select the customer from the dropdown menu and click "Retrieve Information".
- Confirm deletion by clicking "Delete Customer".
- 
### Edit Customers
- Select the customer from the dropdown menu and click "Retrieve Information".
- Edit the information and click "Save Changes".
- 
### History
- On the History tab, you can view customers' orders.

## Reservations
### Past Reservations
Displays a list of past reservations with details.

### Current Reservations
Shows current reservations with details.

### Upcoming Reservations
Lists future reservations with details.

### Edit Reservation
Allows users to edit reservation details, including customer information, services, and notes. Changes can be saved or the reservation can be deleted.

## Locations
### Add Location
Users can add new locations by entering details and clicking "Add". Field restrictions include:
- Name: 30 characters
- City: 30 characters
- Address: 50 characters
- Postal Code: 5 characters
- City: 30 characters
- Phone Number: 15 characters

### Delete Location
- Only admin can delete a branch.
- Select the branch from the dropdown menu and click "Retrieve Information".
- Delete information by clicking "Delete".

### Edit Location
- Only admin can Edit a branch Information.
- Select the branch from the dropdown menu and click "Retrieve Information".
- Edit the information and click "Save Changes".

## Rooms
### Add Room
Users can add new rooms by entering details and clicking "Add". Field restrictions include:
- Name: 40 characters
- Price: 8 characters and 2 decimals
- VAT: 8 characters and 2 decimals
- Capacity: 11 characters

### Delete Room
- Only admin can Delete a space.
- Select the space from the dropdown menu and click "Retrieve Information".
- Delete information by clicking "Delete".

### Edit Rooms
- Only admin can Edit a space Information.
- Select the space from the dropdown menu and click "Retrieve Information".
- Edit the information and click "Save Changes".

## Services
- Lists services and locations.
- Edit services by clicking on the field and saving changes.
- Delete services by selecting and clicking "Delete".
- Add a new service by filling in the details and clicking "Save".

## Users
### Add User
Users can add new users by entering details and clicking "Add". Field restrictions include:
- Name: 40 characters
- Address: 50 characters
- Phone Number: 15 characters
- Username: 50 characters
- Password: 30 characters

### Delete User
- Select the user from the dropdown menu and click "Retrieve Information".
- Delete the user by clicking "Delete User".
  
### Edit Users
- Select the user from the dropdown menu and click "Retrieve Information".
- Edit the information and click "Save Changes".
  
### User History
 - View reservations associated with employees.

## Reports
- View order details and generate printable reports.
- Filter reports by time, location, and branch.

## Invoices
### New Invoice
Users can create new invoices by filling in the fields. Field restrictions include:
- Net Amount: 8 characters and 2 decimals
- VAT Amount: 8 characters and 2 decimals
- Total Amount: 8 characters and 2 decimals

### Saved Invoices
Users can view open invoices by selecting and viewing details.
