// * Intro
/*
    REST (Representational State Transfer) Architecture
        An architectural style that defines a set of standards and constraints for communication
        between two systems, typically over HTTP.

        Constraints:
        [1] Client-Server
            -> Client and server are independent.
            -> The client sends a request; the server responds with a resource.
            -> This separation improves scalability and flexibility.

        [2] Statelessness
            -> Each request from the client must contain all the necessary information 
              (tokens, parameters, ...).
            -> The server does not store any client session state between requests.

        [3] Cacheable
            -> Responses must explicitly indicate whether they are cacheable.
            -> Proper caching improves performance and reduces server load.

        [4] Uniform Interface (Most Important Constraint)
            A consistent and standardized way to interact with resources:
            
            1. Each resource (Employee, Product, Department, ...) is identified by a unique URL:
                -> BaseUrl/Employee
                -> BaseUrl/Product
                -> BaseUrl/Department

            2. HTTP methods are used to perform CRUD operations:
                -> [GET]    BaseUrl/Employee        → Get all employees
                -> [GET]    BaseUrl/Employee/{id}   → Get a specific employee
                -> [POST]   BaseUrl/Employee        → Create a new employee
                -> [PUT]    BaseUrl/Employee/{id}   → Update a specific employee
                -> [DELETE] BaseUrl/Employee/{id}   → Delete a specific employee

            3. Resources are typically represented in JSON format.

            4. Stateless communication — no session data is stored on the server.

    RESTful APIs
        -> Web services or APIs that implement and follow the principles of REST.
        -> "RESTful" means the API respects the REST constraints in practice.
*/

// * Onion Architecture
/*
    [1] Domain Layer (Core)
        -> Class Library 
        -> Contains:
            1. (Domain Models/Entities/POCO Classes)
            2. Interfaces (IGenericRepository<T>, IUnitOfWork, ...)
        -> No dependencies on other layers
        

    [2] Repository Layer (Infrastructure)
        -> Class Library
        -> Contains:
            1. DbContext (EF Core or other ORMs)
                If multiple DbContexts are needed (for different database providers), consider splitting this into multiple repository projects.
            2. Repository Implementations (Generic and Specific)
            3. Unit Of Work Implementation
        -> Depends on the Domain Layer.


    [3] Service Layer (Application)
        -> Class Library
        -> Contains:
            1. All Application Services like (Token Service, Caching Service, ...)
        -> Depends on Domain and Repository layers. 

    [4] Presentation Layer
        -> ASP.NET Core Web API / MVC Project
        -> Exposes APIs or views
        -> Depends on Service Layer

*/


// * Controller Attributes
/*
    [ApiController]
    Enhance the behavior of an API controller:
        [1] Automatic Model Validation
            If the model binding fails, it returns a Bad Request(400) with validation errors. (Without it, ModelState.IsValid needs to be checked)

        [2] Binding Source Inference
            Automatically infers [FromBody], [FromRoute], [FromQuery] based on parameter types.
            [HttpPost]
            public IActionResult Post(Product product) -> Inferred as [FromBody]


        [3] Improved Error Responses
            Returns standard, JSON-formatted error responses for client errors.

            Validation (Bad Request)
            {
                "errors": {
                    "Name": [ "The Name field is required." ]
                },
                "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                "title": "One or more validation errors occurred.",
                "status": 400,
                "traceId": "|...."
            }

            BadRequest()
            {
                "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                "title": "Bad Request",
                "status": 400,
                "traceId": "00-4525921cf23e880b49b8e83532607742-dc5d7abccf23917c-00"
            }

            NotFound()
            {
                "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                "title": "Not Found",
                "status": 404,
                "traceId": "00-32c0bfcc867a14b9d4bf4a7882ba94ea-1039664cbcd1b817-00"
            }

    [Route("api/[controller]")]
        Defines the route template for the controller.
        It tells ASP.NET Core how to match incoming HTTP requests to this controller.

        api/ is static -> part of the URL.
        [controller] is a token -> ASP.NET Core replaces with the controller name (minus the "Controller" suffix).

        Example
            [ApiController]
            [Route("api/[controller]")]
            public class ProductsController : ControllerBase
            {
                [HttpGet]
                public IActionResult GetAll() => Ok("All products");

                [HttpGet("{id}")]
                public IActionResult GetById(int id) => Ok($"Product {id}");
            }

            GET /api/products -> GetAll() is called.
            GET /api/products/13 -> GetById(13) is called.

                
*/


// * Error Handling
/*
    🗒️ Not Found (404)
        [1] Resource Not Found (manually returned)
            Example:
                return NotFound();

            Default JSON response:
                {
                    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    "title": "Not Found",
                    "status": 404,
                    "traceId": "00-...-00"
                }

            With custom message:
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, $"Product with id `{id}` was not found."));

        [2] Endpoint Not Found (no matching route)
            Triggered automatically by requesting a non-existent endpoint (/api/invalid-route)

            -> No JSON body by default
            -> Can be handled via middleware or fallback routes

            Using Fallback Route:
                app.MapControllers();
                app.MapFallback(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                    var response = new ApiErrorResponse(
                        StatusCodes.Status404NotFound,
                        $"The requested endpoint '{context.Request.Path}' was not found.");

                    await context.Response.WriteAsJsonAsync(response);
                });

            Using UseStatusCodePages Middleware:
                app.UseStatusCodePages(async context =>
                {
                    if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                    {
                        var response = new ApiErrorResponse(
                            StatusCodes.Status404NotFound,
                            $"The requested endpoint '{context.HttpContext.Request.Path}' was not found.");

                        await context.HttpContext.Response.WriteAsJsonAsync(response);
                    }
                });

    🗒️Bad Request (400)
        [1] Manually returned
            Example:
                return BadRequest();

            Default JSON response:
                {
                    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    "title": "Bad Request",
                    "status": 400,
                    "traceId": "00-...-00"
                }

            With validation message:
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ["Age exceeds 30 years."]
                });

        [2] Model Binding / Validation Failure
            Trigger:
                [HttpPost("{id}")] -> If Id Is Sent As String "One"
                public IActionResult Get(int id)

            Default JSON response:
                {
                    "errors": {
                        "id": [
                            "The value 'one' is not valid."
                        ]
                    },
                    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    "title": "One or more validation errors occurred.",
                    "status": 400,
                    "traceId": "00-...-00"
                }

        Customize globally via:
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var response = new ApiValidationErrorResponse();

                    foreach (var value in context.ModelState.Values)
                        foreach (var error in value.Errors)
                            response.Errors.Add(error.ErrorMessage);

                    return new BadRequestObjectResult(response);
                };
            });

    🗒️ Internal Server Error (500)
        Unhandled Exceptions
            Example:
                throw new Exception("Something went wrong");

            Behavior:
            -> In Development: Stack trace is shown
            -> In Production: No response body unless handled
            -> Should be captured via exception handling middleware

            Custom exception middleware
                public class ExceptionMiddleware(IWebHostEnvironment env, ILogger<ExceptionMiddleware> logger) : IMiddleware
                {
                    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
                    {
                        try
                        {
                            // Take an action with the request before invoking the next middleware.
                            await next.Invoke(context); // Invoke the next middleware.
                            // Take an action with the response after invoking the next middleware.
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(
                                exception: ex,
                                message: "Error: {Message} at {Endpoint}",
                                ex.Message,
                                context.GetEndpoint()?.DisplayName ?? string.Empty);

			                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			                await context.Response.WriteAsJsonAsync(new ApiExceptionErrorResponse(ex, env));
                        }
                    }
                }
                ❕Register the middleware as a scoped service
                    services.AddScoped(typeof(ExceptionMiddleware));

                ❕Apply the middleware in the request pipeline
                    app.UseMiddleware<ExceptionMiddleware>();




    🗒️Shared Error Response Models

        public class ApiErrorResponse(int statusCode, string? message = null)
        {
            public int StatusCode { get; set; } = statusCode;
            public string Message { get; set; } = message ?? GenerateDefaultStatusMessage(statusCode);

            private static string GenerateDefaultStatusMessage(int statusCode)
            {
                return statusCode switch
                {
                    StatusCodes.Status400BadRequest => "The request was invalid or cannot be served.",
                    StatusCodes.Status401Unauthorized => "Authentication is required and has failed or has not yet been provided.",
                    StatusCodes.Status403Forbidden => "You do not have permission to access this resource.",
                    StatusCodes.Status404NotFound => "The requested resource could not be found.",
                    StatusCodes.Status500InternalServerError => "An unexpected error occurred on the server.",
                    _ => "An error occurred."
                };
            }
        }

        public class ApiValidationErrorResponse()
            : ApiErrorResponse(StatusCodes.Status400BadRequest)
        {
            public List<string> Errors { get; set; } = [];
        }

        public class ApiExceptionErrorResponse : ApiErrorResponse
        {
            public ApiExceptionErrorResponse(Exception ex, IWebHostEnvironment env)
                : base(StatusCodes.Status500InternalServerError)
            {
                if (env.IsDevelopment())
                {
                    Message = ex.Message;
                    StackTrace = ex.StackTrace;
                }
            }
            public string? StackTrace { get; }

        }
*/


// * Manually Created Scope
/*
    What’s a “Scope”?
        A scope defines a boundary in which scoped services exist.
        If you register a service as *scoped*, it will:
            → Be created once per request
            OR
            → Be created once per manually created scope.

        ❕Creating a Scope:
            using IServiceScope scope = app.Services.CreateScope();

            All scoped services resolved from scope.ServiceProvider 
            will be disposed when the scope is disposed.

            The "using" keyword ensures the scope (and its resources) are disposed properly.

    What’s a “ServiceProvider”?
        → The ServiceProvider is the DI container that holds all registered services.
        → After service registration, ASP.NET Core internally builds the ServiceProvider.

        ❕Resolving a Service:
            var serviceProvider = scope.ServiceProvider;
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

    What Can Be Resolved Inside a Scope?
        • Singleton services → One instance for the entire application lifetime.
        • Scoped services → One instance per request or manual scope.
        • Transient services → A new instance every time they are requested.

    Note:
        Scoped services **cannot** be resolved from the root provider directly, 
        as there's no scope to manage their lifecycle outside of a request or manually created scope.
*/


// * Specification Design Pattern
/*
    Allow you to build the query dynamically.

    [1] Interface
        public interface ISpecification<TEntity> where TEntity : ModelBase
        {
            Expression<Func<TEntity, bool>>? Criteria { get; set; }
            List<Expression<Func<TEntity, object>>> Includes { get; set; }
            ...
        }

    [2] Base Specifications
        public class BaseSpecification<TEntity> : ISpecification<TEntity> where TEntity : ModelBase
        {
            public BaseSpecification()
            {
            }
            public BaseSpecification(Expression<Func<TEntity, bool>> criteria)
            {
                Criteria = criteria;
            }

            public Expression<Func<TEntity, bool>>? Criteria { get; set; }
            public List<Expression<Func<TEntity, object>>> Includes { get; set; } = [];

            protected void AddIncludeExpr(Expression<Func<TEntity, object>> includeExpr) => Includes.Add(includeExpr);
        }

    [3] On Demand Specification
        public class ProductSpecsWithRelatedData : BaseSpecification<Product>
        {
            // Multiple Products
            public ProductSpecsWithRelatedData() :
                base()
            {
                AddIncludeExpr(p => p.Brand);
                AddIncludeExpr(p => p.Category);
            }

            // Single Product
            public ProductSpecsWithRelatedData(int id) :
                base(p => p.Id == id)
            {
                AddIncludeExpr(p => p.Brand);
                AddIncludeExpr(p => p.Category);
            }
        }

    [4] Helper Method To Build The Query
        internal static class SpecificationEvaluator
        {
            internal static IQueryable<TEntity> BuildQuery<TEntity>(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specs)
                where TEntity : ModelBase
            {
                var query = inputQuery;

                if (specs.Criteria is not null)
                    query = query.Where(specs.Criteria);

                if (specs.Includes?.Count > 0)
                    query = specs.Includes.Aggregate(query, (currQuery, nextExpr) => currQuery.Include(nextExpr));


                return query;
            }
        }

    [5] Combine The Specifications With Repository
        public interface IRepository<TEntity> where TEntity : ModelBase
        {
            Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity>? specs);
        }

        public class Repository<TEntity>(AppDbContext dbContext) : IRepository<TEntity> where TEntity : ModelBase
        {
            public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity>? specs)
            {
                if (specs is null)
                    return await dbContext.Set<TEntity>()
                        .AsNoTracking()
                        .ToListAsync();

                return await SpecificationEvaluator
                    .BuildQuery(dbContext.Set<TEntity>(), specs)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

    [6] Using Specification In Controller
        [Route("api/[controller]")]
        [ApiController]
        public class ProductsController(IUnitOfWork unitOfWork) : ControllerBase
        {
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var allProducts = await unitOfWork.Repository<Product>().GetAllAsync(new ProductSpecsWithRelatedData());
                return Ok(allProducts);
            }
        }

*/