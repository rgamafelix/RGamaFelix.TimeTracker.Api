# System Architecture Model - TimeTracker

This system is **NOT** intended to be a fully functional time tracker system, but instead, a system architecture model
that can be used as a reference for future projects.\
This is **NOT** a production-ready system, and it is not intended to be used in production.\
This is **ONLY** intended to be used as a reference for future projects.\
This project is, purposely, over-engineered to show how to implement a system using the following technologies:

* **.NET Core**
* **AspNet Core**
* **Entity Framework Core**
* **MediatR**
* **FluentValidation**
* **xUnit**

This architectural model only comprises the backend of the system. It is a REST API that can be consumed by any frontend application.\
It is organized in folders that group together in layers functional related components.\
The system is organized in a way that the layers are dependent only on the layers below them.\
The layers and projects are organized as follows:

* **Rest**: This layer is responsible for expose de services as a REST API. Currently, it is the only entry point of the system, but this architecture allows for the implementation of other entry points, like a GraphQL API, a gRPC API, etc. without having to reimplement the business logic.

    * _**Rest.EntryPoint**_: This is the entry point of the system. It is the component that is exposed to the outside world.\
        It is the component that receives the requests and sends the responses.

    * _**Rest.Api**_: This project contains the controllers. In a real case scenario this project and _Rest.EntryPoint_ could be merged into one project. It is separated here to show how to implement a controller separated from the entrypoint of the web application.

* **Request**: This layer holds the abstract request and response models of the system. As it is agnostic in terms of exposing technology used to expose the services, it can be used by any entry point of the system.

    * _**Request.Model**_: This project contains the request and response models that are used by the endpoints. In this model, all requests must implement **MediatR**'s _IRequest_ interface and all responses must be wrapped in _IServiceResponseOf<T>_ or _IServiceResponse_, provided by the package **RGamaFelix.ServiceResponse**.

    * _**Request.Preprocessor**_: This project contains the preprocessor of the system. It is the component that receives the requests and sends the responses.

* **Domain**: This layer holds the business logic of the system. It is the core of the system.

    * _**Domain.Model**_: This project contains the Entities and Value Objects of the system.

    * _**Domain.Service**_: This project implements the business logic of the system. It contains the use cases of the system implemented as **MediatR**'s _IRequestHandler_. **Domain.Service** is the only project that has access to **Domain.Model**.

* **Application**: This layer is responsible for encapsulating the business logic that doesn't naturally fit within a domain object.

    * _**Application.Service**_: This project encapsulate business logic that doesn't naturally fit within a domain object.

* **Repository**: This layer is responsible for encapsulating the data access logic of the system. It is the only layer that has access to the database.

    * _**DataContext**_: This project contains the abstract and database agnostic repositories of the system. It is the only project that has access to the database. It is implemented using **Entity Framework Core**.

    * _**Repository.Adapter.PostgresSql**_: This project contains the code to run the **DataContext** in a **Postgres** database.

    * _**Repository.Adapter.SqlServer**_: This project contains the code to run the **DataContext** in a **Sql Server** database.

Aside of those core projects there are also the following accessory projects:

* **Repository.Adapter.InMemory**: This project contains the code to run the **DataContext** in an in-memory database\
    For development/demonstration purposes only.

* **Tests**: This project contains the unit tests of the system. It is implemented using **xUnit**.

## Pre Requisites

* dotnet sdk v8

* (Optional) docker-compose for running the database

* (Optional) Postgres or Sql Server database (if not using docker)

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

To run the database migrations you must have the dotnet sdk installed. You can run the migrations using the following
commands:
create a new migration:

`$ dotnet ef migrations add "InitialDatabase" --project RGamaFelix.TimeTracker.Repository.Adapter.PostgresSql --startup-project RGamaFelix.TimeTracker.Rest.EntryPoint --context TimeTrackerDbContext`

update database:

`$ dotnet ef database update --project RGamaFelix.TimeTracker.Repository.Adapter.PostgresSql --startup-project RGamaFelix.TimeTracker.Rest.EntryPoint`

You can replace `PostgresSql` with `SqlServer` in the commands above to run the migrations in a Sql Server database.

## Project Explanation

### Rest.EntryPoint

This project is the entry point of the system. All the REST endpoints will be exposed here.\
It is responsible for the system's initialization and configuration.

### Rest.Api

This project contains the controllers exposed by the system.\
Normally it would be merged with the **Rest.EntryPoint** project, but it is separated here to show how to implement controller injection in case of need.

#### Controllers

This folder holds the implemented controllers.\
The majority of the controllers are implemented simply by calling the **ControllerHelper.ProcessRequest** method that relays the request to the **MediatR** infrastructure.

#### Middleware

Middleware is software that's assembled into an app pipeline to handle requests and responses.\

#### Configuration

This folder holds the configuration classes for the rest API.
The methods declared in these classes are called by the entry point. This allows for the configuration to be injected into the entry point without having to modify the entry point code to reflect the changes in the configuration.

### Request

This layer holds the abstract request and response models of the system and logic required to manipulate them before being sent to the domain layer.

#### Request.Model

This project contains the request and response models that are used by the endpoints.\
In this model, all requests must implement **MediatR**'s _IRequest_ interface and all responses must be wrapped in _IServiceResponseOf<T>_, provided by the package **RGamaFelix.ServiceResponse**.

#### Request.Preprocessor

This project contains logic to manipulate the requests before they are sent to the domain layer.\
It has the following files:

* _**RequestPreprocessorBase**_: This class is the base class for all request preprocessors. It implements the **IPipelineBehavior** interface.

* _**ValidationRequestPreprocessor**_: This class is responsible for validating the requests before they are sent to the domain layer.\
  Only self-contained validations should be implemented here, i.e, only validations that don't require access to the database or other external resource should be implemented here.\
  It is implemented as a **MediatR**'s **IPipelineBehavior** instead of a __Middleware__ in order to delegate the validation rules to a layer common to all endpoints.

* _**AuthorizeRequestAttribute**_: This attribute is used to specify the authorization policy that the request must comply with.\
  It must be placed in the request class that requires authorization.\

* _**AuthorizationRequestPreprocessor**_: This class is responsible for authorizing the requests before they are sent to the domain layer based on the information provided by the **AuthorizeRequestAttribute**.\
  It is implemented as a **MediatR**'s **IPipelineBehavior** instead of a __Middleware__ in order to delegate the authorization rules to a layer common to all endpoints.

### Domain

This layer implements all the business logic of the system.

#### Domain.Model

This project contains the Entities and associated data used to represent the system's business model.\
As a good practice, all the properties of the entities are read-only and can only be accessed through methods. This ensures that the entities are always in a valid state.

#### Domain.Service

This project contains the use cases of the system implemented as **MediatR**'s _IRequestHandler_.\
It is the only project that has access to **Domain.Model** as well as the **Repository** layer.\
It is structured in the following folders:

* _**Handler**_: This folder contains the handlers of the system.\
  Each request type in the **Request.Model** project must have a corresponding handler in this folder.\

* _**Validator**_: This folder contains the validation rules for the request models.\
  Each request model must have a corresponding validator in this folder.\
  The validators are implemented using **FluentValidation**.

* _**Configuration**_: This folder contains the configuration classes for the domain layer.\
  The methods declared in these classes are called by the entry point. This allows for the configuration to be injected into the entry point without having to modify the entry point code to reflect the changes in the configuration.

### Application

This layer is responsible for encapsulating the business logic that doesn't naturally fit within a domain object.

#### Application.Service

This project provides helpers and services that can be reused in vários requests or provide accessory logic to the domain layer.\

### Repository

This layer is responsible for encapsulating the data access logic of the system. It is the only layer that has access to the database.

#### DataContext

This project contains the abstract and database agnostic repositories of the system.\

#### Repository.Adapter.PostgresSql

This project contains the code to run the **DataContext** in a **Postgres** database.\

#### Repository.Adapter.SqlServer

This project contains the code to run the **DataContext** in a **Sql Server** database.\

### Repository.Adapter.InMemory

This project contains the code to run the **DataContext** in an in-memory database.\
