# System Architecure Model - TimeTracker

This system is **NOT** intended to be a fully functional time tracker system, but instead, a system architecture model that can be used as a reference for future projects.
This is **NOT** a production-ready system, and it is not intended to be used in production.
This is **ONLY** intended to be used as a reference for future projects.

This architectural model only comprises the backend of the system. It is a REST API that can be consumed by any frontend application.
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
* docker for running the database



create a new migration:

`$ dotnet ef migrations add "InitialDatabase" --project RGamaFelix.TimeTracker.Repository.Adapter.PostgresSql  --startup-project RGamaFelix.TimeTracker.Rest.EntryPoint --context TimeTrackerDbContext`

update database:

`$ dotnet ef database update --project RGamaFelix.TimeTracker.Repository.Adapter.PostgresSql  --startup-project RGamaFelix.TimeTracker.Rest.EntryPoint`