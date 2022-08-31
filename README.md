# Contoso University App

This is an ASP.Net Core MVC web-app which follows the [Microsoft Tutorial](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-6.0)

## Data Model
![data-model](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro/_static/data-model-diagram.png?view=aspnetcore-6.0)

The preceding entries have the following relationships:
- one-to-many between `Student` and `Enrollment`. A student can be enrolled in multiple courses.
- one-to-many between `Course` and `Enrollment`. A course can have multiple students enrolled in it.

NOTE<br>
Scaffolding: Automatic creation of CRUD action methods and views. 

## More Complex Data Model

![complex-data-model](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/complex-data-model/_static/diagram.png?view=aspnetcore-6.0)