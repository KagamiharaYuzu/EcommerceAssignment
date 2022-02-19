# EcommerceAssignment

This is my final assignment for my last web programming class in college
Please ignore the KLH60 at the start of the folder names. That was a naming convention from the class.
Just keep in mind the rest of the folder name.

## Tech used in this project

- Identity Framework
- Entity Framework
- Blazor Web Pages
- .NET Core 5
- Entity Framework SQL Server for updating and migrating the database

### Customer Project

Web pages and CSS for the customer side of the application

### Manager Project

Web app from the Manager's perspective

### Services Project

Project for storing the ReST API Services and holds all the database operations

### Store Project

Almost the same as the manager project but the clerk of the store also has access but has some limitations on what actions can they do.

### Store Class Library

Class library with all the necessary Classes and methods used throughout the application. Here the CRUD methods call the ReST API with the localhost link as of now and it can be changed fairly easily as it's a static variable at the start of the class.
