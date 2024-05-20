# System Architecure Model - TimeTracker

This system is **NOT** intended to be a fully functional time tracker system, but instead, a system architecture model that can be used as a reference for future projects.\
This is **NOT** a production-ready system, and it is not intended to be used in production.\
This is **ONLY** intended to be used as a reference for future projects.\
This project is, purpusely, over-engineered to show how to implement a system using the following technologies:

* **.NET Core**
* **AspNet Core**
* **Entity Framework Core**
* **MediatR**
* **FluentValidation**
* **xUnit**

This architectural model only comprises the backend of the system. It is a REST API that can be consumed by any frontend application.\
It is orgazined in folders that represent the layers of the system.\
The system is organized in a way that the layers are dependent only on the layers below them.\
The layers are organized as follows:

* **Rest.EntryPoint**: This is the entry point of the system. It is the component that is exposed to the outside world. It is the component that receives the requests and sends the responses.
* **Rest.Api**: This project contains the controllers. In a real case scenario this project and __Rest.EntryPoint__ could be merged into one project. It is separated here to show how to implement a controller separated from the entry point of the web application.
* **Request.Model**: This project contains the request and response models that are used by the endpoints. In this model, all requests must implement **MediatR**'s _IRequest_ interface and all responses must implement are wrapped in _IServiceResponseOf<T>_ or _IServiceResponse_. Both provided by the package **RGamaFelix.ServiceResponse**.
* **Request.Preprocessor**: This project contains the preprocessor of the system. It is the component that receives the requests and sends the responses.
* **Domain.Model**: This project contains the Entities and Value Objects of the system.
* **Domain.Service**: This project implements the business logic of the system. It contains the use cases of the system implemented as **MediatR**'s _IRequestHandler_. **Domain.Service** is the only project that has access to **Domain.Model**.
* **Application.Service**: This project encapsulate business logic that doesn't naturally fit within a domain object.
* **Repository**: This project contains the abstract and database agnostic repositories of the system. It is the only project that has access to the database. It is implemented using **Entity Framework Core**.
* **Repository.Adapter.PostgresSql**: This project contains the code to run the **Reposritory** in a **Postgres** database.
* **Repository.Adapter.SqlServer**: This project contains the code to run the **Reposritory** in a **Sql Server** database.
* **Repository.Adapter.InMemory**: This project contains the code to run the **Reposritory** in a in-memory database - For development/demonsration purposes only.
* **ServiceResponse.FluentValidation**: This project contains the extensions to **FluentValidation** that help integrate with **RGamaFelix.ServiceResponse**.
* **Tests**: This project contains the unit tests of the system. It is implemented using **xUnit**.
 



It is composed of the following projects:

* **Rest.EntryPoint**: This is the entry point of the system. It is the component that is exposed to the outside world. It is the component that receives the requests and sends the responses.

* **Rest.Api**: This project contains the controllers. In a real case scenario this project and __Rest.EntryPoint__ would be merged into one project. It is separated here to show how to implement a controller separated from the entry point of the web application.

* **Rest.Model**: This project contains the request and response models that are used by the controllers. In this model, all requests must implemene **MediatR**'s _IRequest_ interface and all responses must implement are wrapped in _IServiceResponseOf<T>_ or _IServiceResponse_. Both provided by the package **RGamaFelix.ServiceResponse**.

* **Domain.Model**: This project contains the Entities and Value Objects of the system.

* **Domain.Service**: This project implements the business logic of the system. It contains the use cases of the system implemented as **MediatR**'s _IRequestHandler_. **Domain.Service** is the only project that has access to **Domain.Model**.

* **Application.Service**: This project encapsulate business logic that doesn't naturally fit within a domain object.

* **Repository**: This project contains the repositories of the system. It is the only project that has access to the database. It is implemented using **Entity Framework Core**.

Aside of those core projects there are also the following accessory projects:

* **Tests**: This project contains the unit tests of the system. It is implemented using **xUnit**.

* **ServiceResponse.FluentValidation**: This project contains the extensions to **FluentValidation** that help integrate with **RGamaFelix.ServiceResponse**.

* **Repository.Adapter.PostgresSql**: This project contains the code to run the **Reposritory** in a **Postgres** database.

## Pre Requisites

* dotnet sdk v8 

* (Optional) docker-compose for running the database

* (Optional) Postgres or Sql Server database (if not using docker-compose)

## Running the system

To run the system you must have the dotnet sdk installed. You can run the system using the following command:

`$ dotnet run --project RGamaFelix.TimeTracker.Rest.EntryPoint`

## Running the tests

To run the tests you must have the dotnet sdk installed. You can run the tests using the following command:

`$ dotnet test`

## Running the database

To run the database you must have docker-compose installed. You can run the database using the following command:

`$ docker-compose up # this will run both sql and postgres databases`


## Running the database migrations

To run the database migrations you must have the dotnet sdk installed. You can run the migrations using the following commands:
create a new migration:

`$ dotnet ef migrations add "InitialDatabase" --project RGamaFelix.TimeTracker.Repository.Adapter.PostgresSql  --startup-project RGamaFelix.TimeTracker.Rest.EntryPoint --context TimeTrackerDbContext`

update database:

`$ dotnet ef database update --project RGamaFelix.TimeTracker.Repository.Adapter.PostgresSql  --startup-project RGamaFelix.TimeTracker.Rest.EntryPoint`

You can replace `PostgresSql` with `SqlServer` in the commands above to run the migrations in a Sql Server database.
