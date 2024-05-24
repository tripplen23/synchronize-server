# Fullstack Project of Binh Nguyen - Ecommerce inspiration

![PostgreSQL](https://img.shields.io/badge/PostgreSQL-v.16-blue)
![PgAdmin 4](https://img.shields.io/badge/PgAdmin%204-v.8.2-lightblue)
![.NET Core](https://img.shields.io/badge/.NET%20Core-v.8-purple)
![EF Core](https://img.shields.io/badge/EF%20Core-v.8-cyan)
![XUnit](https://img.shields.io/badge/XUnit-v.2.4.2-green)
![Moq](https://img.shields.io/badge/Moq-v.4.20-pink)
![TypeScript](https://img.shields.io/badge/TypeScript-v.4-green)
![React](https://img.shields.io/badge/React-v.18.2-blue)
![Redux toolkit](https://img.shields.io/badge/Redux-v.2.2-brown)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-v.3.4.1-lightblue)

## Project Description

The project was done as a final project at [Integrify](https://www.integrify.io/) bootcamp. It will offer core functionality concepts for a typical Ecommerce Website.

### Project overview

This repository contains the backend server for an E-Commerce Platform. The project implements RESTful API endpoints for managing users, products, orders, carts, categories, and reviews.

**NOTE**: The frontend repository can be found [here](https://github.com/tripplen23/fs17-Frontend-project)

Link to deployed Frontend Web UI: [Frontend](https://fs17-frontend-project-zln9-kl59btf0o.vercel.app/)

Link to deployed Backend Server: [Backend](https://sync-ecommerce.azurewebsites.net/index.html)

## Table of Contents

- [Technologies](#technologies)
- [Getting start](#getting-start)
- [Folder structure](#folder-structure)
- [Relational database design](#relational-database-design)
- [Functionalities](#functionalities)
- [RESTful API design](#restful-api-design)
- [CLEAN Architecture](#clean-architecture)
- [Data flow](#data-flow)
- [Testing](#testing)

## Technologies

- **Frontend**: TailwindCSS, TypeScript, React, Redux Toolkit
- **Backend**: ASP.NET Core, Entity Framework Core, PostgreSQL
- **Testing**: Jest for frontend and XUnit, Moq for backend
- **Deployment**: Vercel for Frontend, Azure for Backend and Neon.tech for Database

## Getting start

1. Open your terminal and clone the front-end repository with the following command:

```
git clone https://github.com/tripplen23/fs17-Frontend-project.git
```

2. Next, clone the back-end repository:

```
git clone https://github.com/tripplen23/fs17_CSharp_FullStack.git
```

3. Navigate the Web API layer in the back-end directory.

```
  cd Backend_Ecommerce
  cd Backend_Ecommerce/Ecommerce.WebAPI
```

4. Set up your database connection in `appsettings.json` file with these values, replace them with your own database info and:

```
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Localhost": "Host=localhost;Username=<your db username>;Database=<your db name>;Password=<your password>" // Password is optional depend if the user setup the password for admin or not
  },
  "Secrets": {
    "JwtKey": "[Your JWT Key]",
    "Issuer": "[Your Custom Name]"
  }
```

5. change database connection configuration in the `Program.cs` to Local host (replace the following line of code to the line 59 in `Program.cs`)
   `var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Remote"));`
6. Try to build the application

```
dotnet build
```

7. If build successfully, run this command to create a new Migrations folder, which stores the snapshot of all the database context changes:
   _If there is already a folder Migrations in the Web API layer, delete it._

```
dotnet ef database drop
dotnet ef migrations add Create
```

8. Apply all the changes to the database

```
dotnet ef database update
```

9. Then run the backend

```
dotnet watch
```

10. Then navigate to the Frontend Project, install all the necessary dependencies

```
npm install
```

11. Navigate to the path inside frontend project /src/redux/newAxiosConfig.ts, then change the `baseURL: "https://sync-ecommerce.azurewebsites.net/api/v1/" ` to `baseURL: "http://localhost:5227/api/v1/" `
12. Then run the frontend

```
npm start
```

## Folder structure

```
.
├── Backend_Ecommerce
│   ├── Backend_Ecommerce.sln
│   ├── Ecommerce.Controller
│   │   ├── Ecommerce.Controller.csproj
│   │   └── src
│   │       └── Controller
│   │           ├── AuthController.cs
│   │           ├── CartController.cs
│   │           ├── CategoryController.cs
│   │           ├── OrderController.cs
│   │           ├── ProductController.cs
│   │           ├── ReviewController.cs
│   │           └── UserController.cs
│   ├── Ecommerce.Core
│   │   ├── Ecommerce.Core.csproj
│   │   └── src
│   │       ├── Common
│   │       │   ├── AppException.cs
│   │       │   ├── BaseQueryOptions.cs
│   │       │   ├── ProductQueryOptions.cs
│   │       │   ├── UserCredential.cs
│   │       │   └── UserQueryOptions.cs
│   │       ├── Entity
│   │       │   ├── CartAggregate
│   │       │   |	  ├── Cart.cs
│   │       │   |	  └── CartItem.cs
│   │       │   ├── OrderAggregate
│   │       │   |	  ├── Order.cs
│   │       │   |	  └── OrderProduct.cs
│   │       │   ├── BaseEntity.cs
│   │       │   ├── Category.cs
│   │       │   ├── Product.cs
│   │       │   ├── ProductImage.cs
│   │       │   ├── Review.cs
│   │       │   ├── ShippingInfo.cs
│   │       │   ├── TimeStamp.cs
│   │       │   └── User.cs
│   │       ├── RepoAbstract
│   │       │   ├── ICartItemRepo.cs
│   │       │   ├── ICartRepo.cs
│   │       │   ├── ICategoryRepo.cs
│   │       │   ├── IOrderRepo.cs
│   │       │   ├── IProductImageRepo.cs
│   │       │   ├── IProductRepo.cs
│   │       │   ├── IReviewRepo.cs
│   │       │   └── IUserRepo.cs
│   │       └── ValueObject
│   │           ├── OrderStatus.cs
│   │           └── UserRole.cs
│   ├── Ecommerce.Service
│   │   ├── Ecommerce.Service.csproj
│   │   └── src
│   │       ├── DTO
│   │       │   ├── CartDto.cs
│   │       │   ├── CartItemDto.cs
│   │       │   ├── CategoryDto.cs
│   │       │   ├── OrderDto.cs
│   │       │   ├── OrderProductDto.cs
│   │       │   ├── ProductImageDto.cs
│   │       │   ├── ProductDto.cs
│   │       │   ├── ReviewDto.cs
│   │       │   ├── ShippingInfoDto.cs
│   │       │   └── UserDto.cs
│   │       ├── Service
│   │       │   ├── AuthService.cs
│   │       │   ├── CartItemService.cs
│   │       │   ├── CartService.cs
│   │       │   ├── CategoryService.cs
│   │       │   ├── OrderService.cs
│   │       │   ├── ProductService.cs
│   │       │   ├── ReviewService.cs
│   │       │   └── UserService.cs
│   │       ├── ServiceAbstract
│   │       │   ├── IAuthService.cs
│   │       │   ├── ICartItemService.cs
│   │       │   ├── ICartService.cs
│   │       │   ├── ICategoryService.cs
│   │       │   ├── IOrderService.cs
│   │       │   ├── IPasswordService.cs
│   │       │   ├── IProductImageService.cs
│   │       │   ├── IProductService.cs
│   │       │   ├── IReviewService.cs
│   │       │   ├── ITokenService.cs
│   │       │   └── IUserService.cs
│   │       └── Shared
│   │           └── MapperProfile.cs
│   ├── Ecommerce.Test
│   │   ├── Ecommerce.Test.csproj
│   │   └── src
│   │       ├── Core
│   │       │   └── removeme.txt
│   │       └── Service
│   │           ├── CategoryServiceTest.cs
│   │           ├── OrderServiceTest.cs
│   │           ├── ProductServiceTests.cs
│   │           ├── ReviewServiceTest.cs
│   │           └── UserServiceTest.cs
│   ├── Ecommerce.WebAPI
│   │   ├── Ecommerce.WebAPI.csproj
│   │   ├── Ecommerce.WebAPI.http
│   │   ├── Properties
│   │   │   └── launchSettings.json
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.json
│   │   └── src
│   │       ├── AuthorizationPolicy
│   │       |   ├── AdminOrOwnerAccountRequirement.cs
│   │       │   ├── AdminOrOwnerCartRequirement.cs
│   │       │   ├── AdminOrOwnerOrderRequirement.cs
│   │       │   └── AdminOrOwnerReviewRequirement.cs
│   │       ├── Database
│   │       |   ├── AppDbContext.cs
│   │       │   ├── SeedingData.cs
│   │       │   └── TimeStampInterceptor.cs
│   │       ├── ExternalService
│   │       │   ├── PasswordService.cs
│   │       │   └── TokenService.cs
│   │       ├── Middleware
│   │       │   └── ExceptionHandlerMiddleware.cs
│   │       ├── Program.cs
│   │       └── Repo
│   │           ├── CategoryRepo.cs
│   │           ├── OrderRepo.cs
│   │           ├── ProductRepo.cs
│   │           ├── ReviewRepo.cs
│   │           └── UserRepo.cs
├── image
├──.gitignore
└── README.md
```

## Relational database design

![erd](image/README/erd.png)
_Click to see clearer_

## Functionalities

1. Allow Anonymous:

- Login -> `POST http://localhost:5227/api/v1/auth/login`
- Get All Categories -> `GET http://localhost:5227/api/v1/categories`
- Get Category By ID -> `GET http://localhost:5227/api/v1/categories/:category_id`
- Get Order By User ID -> `GET http://localhost:5227/api/v1/orders/user/:userId` (Should be changed into Admin and Owner later!)
- Get Order by ID -> `GET http://localhost:5227/api/v1/orders/:orderId` (Should be changed into Admin and Owner later!)
- Get All Products -> `GET http://localhost:5227/api/v1/products`
- Get Products By Category -> `GET http://localhost:5227/api/v1/products/category/:categoryId`
- Get Product By Id -> `GET http://localhost:5227/api/v1/products/:productId`
- Get All Reviews -> `GET http://localhost:5227/api/v1/reviews`
- Get All Reviews Of A Product -> `GET http://localhost:5227/api/v1/reviews/product/:productId`
- Get Review By ID -> `GET http://localhost:5227/api/v1/reviews/:reviewId`
- Create An User (Register) -> `POST http://localhost:5227/api/v1/users`

2. Owner only (Logged-in Users):

- Logout -> `POST http://localhost:5227/api/v1/auth/logout`
- Create An Order -> `POST http://localhost:5227/api/v1/orders`
- Get Review By ID -> `GET http://localhost:5227/api/v1/reviews/:reviewId`
- Create A Review -> `POST http://localhost:5227/api/v1/reviews`
- Update An User Profile -> `PUT http://localhost:5227/api/v1/users/:userId`

3. Admin and Owner:

- Get Current User Profile -> `GET http://localhost:5227/api/v1/auth/profile`
- Get Cart By ID -> `GET http://localhost:5227/api/v1/carts/:cartId`
- Delete Cart By ID -> `DELETE http://localhost:5227/api/v1/carts/:cartId`
- Update Cart Item's Quantity -> `PATCH http://localhost:5227/api/v1/carts/:cartId`
- Update A Review By ID -> `PATCH http://localhost:5227/api/v1/reviews/:reviewId`
- Delete A Review By ID -> `DELETE http://localhost:5227/api/v1/reviews/:reviewId`

4. Admin Only:

- Get All Carts -> `GET http://localhost:5227/api/v1/carts`
- Create A New Category -> `POST http://localhost:5227/api/v1/categories`
- Update A Category By ID -> `PATCH http://localhost:5227/api/v1/categories/:category_id`
- Delele A Category By ID -> `DELETE http://localhost:5227/api/v1/categories/:category_id`
- Get All Orders -> `GET http://localhost:5227/api/v1/orders`
- Update Order Status or Shipping Information -> `PATCH http://localhost:5227/api/v1/orders/:orderId`
- Delete An Order -> `DELETE http://localhost:5227/api/v1/orders/:orderId`
- Create A Product -> `POST http://localhost:5227/api/v1/products`
- Update A Product -> `PATCH http://localhost:5227/api/v1/products/:productId`
- Delete A Product -> `DELETE http://localhost:5227/api/v1/products/:productId`
- Get All Users -> `GET http://localhost:5227/api/v1/users`
- Get User By ID -> `GET http://localhost:5227/api/v1/users/{userId}` (Should be changed into Admin and Owner later!)
- Delete An User By ID -> `DELETE http://localhost:5227/api/v1/users/{userId}` (Should be changed into Admin and Owner later!)

## RESTful API design

Check [/Endpoints](https://github.com/yuankeMiao/fs17_CSharp_8-BackendTeamwork/tree/main/Endpoints) folder for all api endpoints and the usage, or click the links below to check descriptions and example request & response for each endpoint.

![swaggerUI](image/README/swaggerUI.png)

## CLEAN Architecture

![image](https://github.com/yuankeMiao/fs17_CSharp_8-BackendTeamwork/assets/114677249/0f37b987-9f8f-4dc5-a975-1d6ecf10f01c)

This project follows the principles of Clean Architecture, emphasizing separation of concerns and modularity. It is organized into several layers, each with distinct responsibilities.

1. Core Domain Layer (Ecommerce.Core)

- Centralizes core domain logic and entities.
- Includes common functionalities, repository abstractions, and value objects.

2. Application Service Layer (Ecommerce.Service)

- Implements business logic and orchestrates interactions between controllers and the core domain.
- Services handle DTO transformations and business operations related to each resource.

3. Controller Layer (Ecommerce.Controller)

- Contains controllers responsible for handling HTTP requests and responses.
- Controllers are organised by resource types (e.g., Auth, Category, Order, Product, Review, User).

4. Infrastructure Layer (Ecommerce.WebAPI)

- Manages infrastructure tasks and interaction with external systems.
- Contains database context, repositories, and middleware for error handling.

5. Testing Layer (Ecommerce.Test)

- Holds unit tests for core domain and application services.
- Ensures the reliability and correctness of the implemented functionalities.

### Data flow:

![image](https://github.com/tripplen23/fs17_CSharp_8-BackendTeamwork/assets/114677249/4b058b32-0b07-4c3f-b752-50e34985a57f)

### Testing

Run tests:

- Navigate to root folder of backend module, then run the test

```
dotnet test
```

![test result](image/README/testResult.png)
