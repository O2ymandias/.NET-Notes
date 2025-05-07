// * Deployment Architecture of ASP.NET Core Application
/*
    🗒️ASP.NET Core Deployment Architecture
        [1] Client (User)
            -> Typically a browser or frontend app (e.g., Angular, React, mobile app).
            -> Sends an HTTP request to the server (GET, POST, etc.).

        [2] Server Side
            The server handles the request in several layers:
                [A] Reverse Proxy Server
                    -> Acts as a middleman between the client and the Kestrel server.
                    -> Handles:
                        - SSL Termination
                            Decrypts HTTPS requests from the client before passing them to the app.

                        - Load Balancing
                            Distributes incoming requests across multiple app servers.

                        - URL Rewriting
                            Modifies the request URL before passing it to the app.

                        - Logging
                            Records request and response data for monitoring and debugging.

                        - Request forwarding
                            Forwards the request to the Kestrel server.

                    ❕Common Reverse Proxies:
                        - Windows: IIS
                        - Linux: Apache, Nginx


                    Why use a Reverse Proxy?
                        - Kestrel is a raw web server; it lacks production-grade features.
                        - Prevents Kestrel from being exposed to the internet.
                        - Improves security (firewall, DDoS protection, etc.)
                        - Enhances scalability (can distribute traffic)

                [B] Web Server (Kestrel)
                    -> Accept request from Reverse Proxy.
                    -> Match routing pattern.
                    -> Pass the request to the middleware components.
                    -> Middleware calls Controller/Endpoint.
                    -> Response generated and passed back through the pipeline.
                    -> Returned to the Reverse Proxy → Client.


                [C] ASP.NET Core Application (Business Logic)
                    -> Contains:
                        - MVC Controllers
                        - Razor Views
                        - Web APIs
                        - SignalR (Real-time communication)
                        - Middleware (custom or built-in)

                    ❕Middleware Examples:
                        1. Authentication/Authorization
                        2. Error Handling
                        3. CORS
                        4. Static Files
                        5. Routing

        Deployment Flow:
            Client
                ↓
            Reverse Proxy (IIS, Nginx, Apache)
                ↓
            Kestrel (Internal Web Server)
                ↓
            ASP.NET Core App (Controllers, APIs, Razor Pages)
                ↑
            Response goes back in the same flow

*/

// * MVC Architecture
/*
    🗒️ What is MVC?
        The MVC pattern helps achieve a separation of concerns, which makes code more maintainable, scalable, and testable.

        Model (Data):
            Represents the application's data and business logic.
            It interacts with the database and encapsulates the data-related rules.

        View (HTML/UI):
            The user interface part, responsible for displaying data to the user.
            Views are typically Razor pages or HTML templates.

        Controller (Processing):
            Handles user requests, processes input, and coordinates with the Model and View.
            It acts as the middleman between Model and View.  
*/

// * Dependency Injection (DI)
/*
    🗒️What is Dependency Injection?
        A design pattern used to implement IoC (Inversion of Control).
        It allows a class to receive its dependencies from an external source rather than creating them itself.

    🗒️DI Lifecycle
        [1] Transient
            Created: every time they are requested.
            Use Cases: lightweight (not expensive to create), stateless services (don't hold state).
            Example:
                1. Helper service
                    services.AddTransient<IFormatter, JsonFormatter>();
                    public class JsonFormatter : IFormatter
                    {
                        public string Format(object data) => JsonSerializer.Serialize(data);
                    }
                2. Mapping service (AutoMapper)


        [2] Scoped
            Created: Once per request (same instance shared within the request).
            Use Cases: 
                1. Services that need to share state within a request (DbContext)
                2. Repositories service.

        [3] Singleton
            Created: Once per the application lifetime (As long as the app running on the server).
            Use Cases:
                1. Services that maintain global state
                2. Services that require heavy initialization.
                3. Caching service.
                4. Logging service.
                5. AutoMapper service.

            ❕A singleton service should NOT depend on a scoped service, because the singleton outlives the scoped service and could access a disposed object, causing runtime errors.

    🗒️Registered Built-in Services:
        [1] IConfiguration
            When the application starts, the ASP.NET Core hosting system does a lot of setup behind the scenes.
            Specifically:
                1. It creates a ConfigurationBuilder.
                2. It loads configuration sources like:
                    -> appsettings.json
                    -> appsettings.{Environment}.json
                    -> Environment variables
                    -> Command-line arguments

                3. It builds an IConfiguration object.
                4. It registers the final IConfiguration instance into the DI container automatically.


        [2] IHostEnvironment
            Represents the environment in which the application is running (Development, Staging, Production).
            It provides information about the environment and allows you to configure services accordingly.

        [3] ILogger
            It allows to log messages, warnings, errors, and other information.


    

    🗒️Register Built-in Services
        [1] AddControllers() 
            → Registers core MVC services (for API applications):
                1. Routing:
                    Enables attributes like [Route], [HttpGet], [HttpPost].
                    Maps HTTP requests to controller actions.

                2. Model Binding:
                    Converts HTTP request data (query strings, JSON, forms) into C# objects.

                3. Validation:
                    Supports model validation using data annotations: [Required], [StringLength], [Range].
                    Validates input before action method executes.

                4. Action Results:
                    Supports return types like:
                      Ok(), BadRequest(), NotFound(), Json(), etc.

                5. Filters:
                    Supports filters like:
                      1. Authorization filters
                      2. Action filters
                      3. Exception filters

                6. Dependency Injection:
                    Automatically injects services into controller constructors.

            → Enables support for Controllers only (ideal for APIs).
            → ❌ Does NOT support Razor Views (no .cshtml rendering).

        [2] AddControllersWithViews()
            → Registers full MVC services (Controllers + Razor Views):
                Includes everything from AddControllers():
                    Routing, Model Binding, Validation, Filters, DI.

                Additionally Adds:
                    1. AddViews()
                        Registers View-Related Services

                    2. AddRazorViewEngine()
                        Registers the Razor View Engine (which compiles .cshtml files)


            → Enables support for Controllers and Views.

        [3] AddRazorPages()
            → Registers Razor Pages services
            → Ideal for building web applications using Razor Pages.


        [4] AddMvc()
            → Registers all MVC services (Controllers + Views + Razor Pages).
            → It’s a combination of AddControllersWithViews() and AddRazorPages().
            → ❌ Not recommended for new projects; use AddControllersWithViews() or AddRazorPages() instead.

        [5] AddDbContext<TContext>()
            → Registers the DbContext and its associated DbContextOptions into the Dependency Injection (DI) container with a scoped lifetime by default.
            → Allows you to inject the DbContext into your controllers or services.

            → Example:
                services.AddDbContext<MyDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

                public class MyDbContext : DbContext
                {
                    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
                }


*/

// * MVC Controller
/*
    🗒️Controller
        A class that inherits from ControllerBase:
            1. Handles incoming HTTP requests.
            2. Gets data from the Model.
            3. Sends data to the View.
            4. Returns an HTTP response.
        It acts as the middleman between the Model and View.

    🗒️Action Method:
        A public object member method in a controller that responds to HTTP requests.
        Can return different types of results:
            1. IActionResult:
                Base interface for all action results.
                Provides flexibility — lets you return:
                    Views
                    JSON
                    Status codes
                    Redirects
                    Custom content

            2. ViewResult:
                Returns a view (HTML) to the client.
                Used in MVC applications with Razor views.
                Example:
                    return new ViewResult(); ❕If no view name is specified, it looks for a view with the same name as the action method.
                    return View();
                    return View("ViewName");

            2. JsonResult:
                Returns JSON-formatted data.
                Automatically serializes C# objects to JSON format.
                Ideal for API responses or AJAX calls.
                Example:
                    return new JsonResult(new { message = "Hello" });
                    return Json(new { status = "ok" });

            3. ViewResult:
                Renders a Razor View (.cshtml) and returns HTML.
                Used in MVC applications with views.
                Example:
                    return View("ViewName"); Renders Index.cshtml by default


            4. RedirectToAction():
                Redirects to a different action method (in the same or another controller).
                Performs a 302 Redirect.
                Example:
                    return RedirectToAction("Details", new { id = 5 });

            5. RedirectResult:
                Performs a redirect to a specified URL.
                Can be used for external URLs or different routes.
                Example:
                    return new RedirectResult("https://example.com");
                    return Redirect("https://example.com");

            6. RedirectToRouteResult:
                Redirects to a specific route using route values.
                Example:
                    return new RedirectToRouteResult("Default", new
                    {
                        controller = "Home",
                        action = "Index",
                        id = 13
                    });

                    return RedirectToRoute("Default", new
                    {
                        controller = "Home",
                        action = "Index",
                        id = 13
                    });

            7. ContentResult:
                Returns plain text or custom HTML.
                You can set content type and status code.
                Example:
                    return new ContentResult
                    {
                        Content = "<h1>Hello</h1>",
                        ContentType = "text/html",
                        StatusCode = 200
                    };

                    return Content("Hello, World!"); // Shorthand

            8. BadRequestResult:
                Returns 400 Bad Request.
                Indicates the request is invalid.
                Example:
                    return new BadRequestResult();
                    return BadRequest();

            9. BadRequestObjectResult:
                Returns 400 Bad Request with an error object/message.
                Example:
                    return new BadRequestObjectResult(new { Error = "Invalid input" });
                    return BadRequest(new { Error = "Invalid input" });

            10. NotFoundResult:
                Returns 404 Not Found.
                Used when the requested data/resource doesn't exist.
                Example:
                    return new NotFoundResult();
                    return NotFound("Resource not found");

            OTHER TYPES:
                1. CreatedResult/Created()
                2. NoContentResult/NoContent()
                3. UnauthorizedResult/Unauthorized()
                4. ForbidResult/Forbid()
                5. StatusCodeResult/StatusCode()
                6. OkResult/Ok()
*/

// * Model Binding
/*
    The process by which ASP.NET Core automatically maps HTTP request data (like form fields, query strings, route data, and headers) to action method parameters

    🗒️How Model Binding Works:
        When a request is routed to a controller action, ASP.NET Core looks at the request data in this order:
            [1] Form data 
            [2] Route data
            [3] Query string

        The model binder parses the data source and matches it to the parameters of the controller action method.
            Simple Types
                If the action parameter is a simple type (int, string, bool, ...), The model binder directly maps the value from the request.
                public IActionResult Get(int id) -> /get?id=13

            Complex Types
                If the action parameter is a complex type (class or struct), the model binder will create an instance of that type and populate its properties.
                public IActionResult Get(User user) -> /get?Name=John&Age=30 || /get?user.Name=John&user.Age=30

            Collections
                If the action parameter is a collection (List, Array), the model binder will create a collection and populate it with values from the request.
                public IActionResult Get(List<int> ids) -> /get?ids=1&ids=2&ids=3 || /get?ids[0]=1&ids[1]=2&ids[2]=3

        The model binder will stop at the first source that provides a value
            For example: if both form data and route data contain values for the same property, the form data will be used.


    🗒️Explicit Binding Attributes
        [FromForm] —> bind from form fields
        [FromRoute] —> bind from route data
        [FromQuery] —> bind from query string

        [FromBody] —> bind from the request body
            By default, Model Binder won't bind complex types from the request body unless [FromBody] is specified.
*/

// * Middlewares
/*
    Built-in Middleware:
        1. UseStaticFiles()
            Serves static files (CSS, JS, images) from the wwwroot folder.
            Should be used after UseRouting() but before UseAuthorization().

            In ASP.NET Core newer versions, MapStaticAssets() was introduced
                Replaces UseStaticFiles() for a more integrated approach with endpoint routing.
                Automatically serves static files from wwwroot using endpoint definitions.
                Works with .WithStaticAssets() to register static routes alongside controller routes.
            
                app.MapStaticAssets(); ❕Registers static file serving middleware
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();  ❕Merges controller and static asset routes


        2. UseRouting()
            This activates the routing system.
            Matches the incoming URL path to an endpoint.
            Does NOT execute the matched endpoint (it only identifies which one matches).

        3. MapControllerRoute()
            Defines a route pattern for matching incoming URLs.
            It maps the URL to a specific controller and action method.






*/

// * Razor View
/*
    Razor View is essentially a (.cshtml) file used to render HTML content with embedded C# code.
    For each razor view, ASP.NET Core creates a class at runtime that inherits from RazorPage<TModel> and has access to:
        1. Model: The data passed from the controller to the view.
        2. ViewData: Dictionary for passing data from the controller to the view.
        3. ViewBag: Dynamic wrapper around ViewData.
        4. TempData: Dictionary for passing data between requests.
        5. Html: HTML helper methods for generating HTML elements.

    If @model is not specified, the generated class will inherit from RazorPage<dynamic>.

    Folder Structure
        Views/
        ├── Home/
        │   └── Index.cshtml
        ├── Shared/
        │   └── _Layout.cshtml
        ├── _ViewStart.cshtml
        └── _ViewImports.cshtml

        _Layout.cshtml:
            The main layout that defines the overall structure of the HTML page.
            One project can have multiple layouts.
            @RenderBody()
                It’s replaced by the content of the child view.
                Only renders content not defined in a @section.
                One per layout file.

            @RenderSection()
                Used to define named sections that are optional or required in child views.

            Example:
                <html>
                    <head>
                        <title>@ViewData["Title"]</title>
                    </head>
                    <body>
                        @RenderBody()
                        @RenderSection("Scripts", required: false)
                    </body>
                </html>



        _ViewStart.cshtml:
            A special file that runs before any view in the folder.
            One project has EXACTLY one _ViewStart.cshtml file.
            It can set the layout for all views.
            Example:
                @{
                    Layout = "_Layout.cshtml";
                }
                This means all views  will use _Layout.cshtml as their layout unless specified otherwise.

        _ViewImports.cshtml:
            Contains namespaces for all views.
    

    HTML Helpers:
        [1] DisplayNameFor()
            Displays the display name of a model property.
            Example:
                class Person
                {
                    [DisplayName("Full Name")]
                    public string Name { get; set; }
                }
                @Html.DisplayNameFor(m => m.Name) ➔ "Full Name"

        [2] DisplayFor()
            Displays the value of a model property.
            Example:
                @Html.DisplayFor(m => m.Name) ➔ "John Doe"



    Tag Helpers:
        [1] asp-for
            -> Binds a model property to an HTML element.

            -> Auto-generates attributes (name, id, value, type) based on the bound model property.

            -> Applies model metadata automatically ([DisplayName("Full Name")]).

            -> Validation:
                Auto-generates client-side validation attributes like: data-val="true", data-val-required="true" and others based on model attributes such as [Required], [StringLength], [Range].

            Example:
                class Person
                {
                    [Required(ErrorMessage = "The Name field is required.")]
                    public string Name { get; set; }
                }

                <input asp-for="Name"/> Will generate:
                    <input
                        type="text"
                        id="Name" 
                        name="Name" 
                        value="@(Model.Name)" 
                        data-val="true" 
                        data-val-required="The Name field is required." 
                    />


        [2] asp-controller & asp-action & asp-route-parameter
            Purpose: Generates a URL to a specific controller action with route parameter.
            Example:
                <a
                    asp-controller="Home"
                    asp-action="Details"
                    asp-route-id="1"
                >
                    Details"
                </a> 
                This will generate: <a href="/Home/Details/1">Home</a>

        [3] asp-validation-for
            Purpose: Integrates client-side validation with the model property.
            Example: (MUST be span tag)
                <span asp-validation-for="Name"></span>

        [4] asp-validation-summary
            Displays a summary of validation errors for the entire model.
            MUST be div tag
            Example:
                <div asp-validation-summary="None/ModelOnly/All" class="text-danger"></div>
                Options:
                    None:
                        -> No validation summary is displayed (default).

                    ModelOnly:
                        -> Displays only model-level errors (Custom errors with empty key "").
                            ModelState.AddModelError("", "...")
                        -> Ignores property-level errors.

                    All:
                        -> Displays both:
                            Model-level errors (ModelState.AddModelError("", "..."))
                            Property-level errors
                                1. ModelState.AddModelError("PropertyName", "...")
                                2. Errors triggered by validation attributes like [Required], [StringLength].

                ❕Custom error display behavior:
                    ModelState.AddModelError("PropertyName", "Error Message")
                        -> Shows in <span asp-validation-for="PropertyName">
                        -> Also appears in the summary only if asp-validation-summary="All"

                    ModelState.AddModelError("", "Error Message")
                        -> Appears in the summary if asp-validation-summary="ModelOnly" or "All"


*/

// * 3 Tiers Architecture
/*
    🗒️3-Tier Architecture
        [1] Presentation Layer (UI)
            Responsible for: Displaying data to the user and handling user interactions.
            Examples: ASP.NET MVC, Web API, Desktop (WPF), Mobile (Xamarin), Blazor (SPA).

        [2] Business Logic Layer (BLL)
            Responsible for: Containing the core business rules and logic of the application.
            Function: Processes data from the DAL and prepares it for the UI.
            Typically a: Class Library.

        [3] Data Access Layer (DAL)
            Responsible for: Interacting with the database or any data source.
            Function: Handles CRUD operations and data retrieval.
            Examples: Entity Framework Core, Dapper.
*/

// * Repository Pattern
/*
    🗒️Repository Pattern
        A design pattern that provides an abstraction layer over data access logic.
        It separates the data access code from the business logic, making it easier to manage and test.

        Benefits:
            1. Separation of Concerns: Keeps data access logic separate from business logic.
            2. Testability, Reusability

        Example:
            public interface IDepartmentRepository
            {
                Task<Department> GetByIdAsync(int id);
                Task<IEnumerable<Department>> GetAllAsync();
                Task AddAsync(Department department);
                Task UpdateAsync(Department department);
                Task DeleteAsync(int id);
            }

            public class DepartmentRepository : IDepartmentRepository
            {
                private readonly MyDbContext _context;

                public DepartmentRepository(MyDbContext context)
                {
                    _context = context;
                }

                public async Task<Department> GetByIdAsync(int id)
                {
                    ❕Using FirstOrDefault()
                        Always sends a query to the database.
                        Useful for flexible conditions (filtering by non-primary key fields).
                        Example:
                            var department = _context.Departments.Local.FirstOrDefault(d => d.Id == id);
                            if (department is not null) return department;
                            return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);

                    ❕Using Find()
                        First checks DbContext's Change Tracker (in-memory entities).
                        If found in memory, returns immediately (no database query).
                        If not found, queries the database by primary key (Id).
                        More efficient when searching by primary key.
                        Only works for primary key lookups.
                        Example:
                            return await _context.Departments.FindAsync(id);
                }

            }
*/

// * Generic Repository Pattern
/*
    🗒️Generic Repository Pattern
        A variation of the Repository Pattern that uses generics to create a single repository interface and implementation for multiple entities.

        Benefits:
            1. Code Reusability: One generic repository can handle multiple entities.
            2. Flexibility: Can work with any entity type without creating separate repositories.

        Example:
            public interface IGenericRepository<TEntity> where TEntity : class
            {
                Task<IEnumerable<TEntity>> GetAllAsync();
                Task<TEntity> GetByIdAsync(int id);
                Task AddAsync(TEntity entity);
                Task UpdateAsync(TEntity entity);
                Task DeleteAsync(int id);
            }

            public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class "It's better to use a ModelBase class that all models inherit from"
            {
                private readonly MyDbContext _context;

                public GenericRepository(MyDbContext context)
                {
                    _context = context;
                }

                public async Task<TEntity> GetByIdAsync(int id)
                {
                    return await _dbSet.FindAsync(id);
                }

                ...
            }
*/

// * Unit of Work Pattern
/*
    Manages multiple repositories and commits all changes as a single transaction.

    Benefits:
        [1] Transaction Management —> Ensures all changes are committed or rolled back together.
        [2] Scalability —> Dynamically provides repositories when needed.

    Interface:
        public interface IUnitOfWork : IDisposable
        {
            IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ModelBase;
            Task<int> SaveAsync();
        }

    Implementation:
        public class UnitOfWork : IUnitOfWork
        {
            private readonly MyDbContext _context;
            private readonly Dictionary<Type, object> _repositories = new();
            ❕Without Dictionary every call creates a new repository instance, which:
                1. Wastes memory.
                2. Breaks the idea of a single shared repository per entity during a unit of work (bad if you want to track entity state consistently).

            public UnitOfWork(MyDbContext context)
            {
                _context = context;
            }

            public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
            {
                var type = typeof(TEntity);
                if (_repos.TryGetValue(type, out var value))
                    return (IGenericRepo<TEntity>)value;

                var repo = new GenericRepo<TEntity>(dbContext);
                _repos[type] = repo;
                return repo;
            }

            public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

            public void Dispose() => _context.Dispose();
        }

    Example
        public class UserService
        {
            private readonly IUnitOfWork _unitOfWork;

            public UserService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task AddUserAsync(User user)
            {
                var userRepo = _unitOfWork.Repository<User>();
                await userRepo.AddAsync(user);
                await _unitOfWork.SaveAsync();
            }
        }
*/

// * [HttpGet] [HttpPost] Pattern
/*
    Pattern used in ASP.NET Core MVC to handle form operations.
    [HttpGet]: Fetch data, display the form.
    [HttpPost]: Validate and process the form submission.

    Example:
        [HttpGet]
        public IActionResult Update(int id)
        {
            var department = _departmentRepo.GetById(id);
            if (department is null) return NotFound();
            return View(department);
        }

        [HttpPost]
        public IActionResult Update(Department department)
        {
            if (!ModelState.IsValid) return View(department);
            
            _departmentRepo.Update(department);
            return RedirectToAction(nameof(Index));
        }

    ❕Important EF Core Behavior:
        Each HTTP request (GET or POST) typically uses a new instance of DbContext (In case of Scoped Lifetime "Default").

        In the [HttpGet] request, the DbContext fetches the entity from the database, and it is tracked by the DbContext instance "By Default".

        In the [HttpPost] request, the form data is bound to a new Department object created by the model binder. But this entity:
            1. Was not fetched via the current DbContext (By the previous GET request NOT the current POST request).
            2. Is not tracked.
            3.Therefore, its EntityState is Detached.

        EF Core behavior when call Update() method
            1. If the entity is already being tracked
                -> All properties are marked as Modified, even if they haven't changed.
                -> An UPDATE statement will be generated based on the PK
                    UPDATE [TableName]
                    SET [Column1] = @value1, [Column2] = @value2
                    WHERE [PK] = @pkValue

            2. If the entity is not being tracked
                -> It attaches it to the dbContext.
                -> All properties are marked as Modified.
                -> An UPDATE statement will be generated based on the PK.

                -> If the Primary Key doesn't match any record in the database:
                    EF Core will still generate and execute an UPDATE statement.
                    But the UPDATE affects zero rows because the WHERE [PK] = @pkValue condition doesn't match anything.


        In ASP.NET Core, DbContext usually has a Scoped lifetime (one per request).
        Each HTTP request (GET or POST) uses a **different DbContext instance**.

        [HttpGet]
            -> The DbContext fetches the entity.
            -> EF Core **tracks** this entity (EntityState = Unchanged by default).

        [HttpPost]
            -> The model binder creates a new `Department` object.
            -> This object is:
                1. Not tracked (was not fetched in the current request).
                2. Treated as a new "detached" instance by EF Core.
                3. Has `EntityState = Detached` by default.

        EF Core Update() Behavior:
            1. If entity is already tracked:
                -> EF Core marks all properties as Modified (even if unchanged).
                -> An UPDATE is issued:
                    UPDATE [Table]
                    SET [Column1] = @value1, ...
                    WHERE [PK] = @pk

            2. If entity is not tracked:
                -> EF Core **attaches** it and marks all properties as Modified.
                -> UPDATE is still issued using the PK.
                -> If PK doesn't match any record:
                    The UPDATE executes but affects **zero rows**.

    ✔ Best Practices:
        -> Avoid using `Update()` on detached entities to prevent unintended updates.
        -> Fetch the original entity in the [HttpPost] method.
        -> Manually update only the properties that changed.

*/

// * Anti-Forgery Token
/*
    ASP.NET Core provides protection against Cross-Site Request Forgery (CSRF) attacks using anti-forgery tokens.

    -> When a form is rendered (typically using tag helpers):
        1. ASP.NET Core generates an anti-forgery token pair:
           - A hidden input field named "__RequestVerificationToken" is added to the form.
           - A corresponding token is stored in a cookie (".AspNetCore.Antiforgery...").

        2. These tokens are generated per user session and vary by application instance.

    -> When the form is submitted:
        1. The browser sends both tokens:
           - One from the hidden form field (request body).
           - One from the cookie (request headers).

        2. The `[ValidateAntiForgeryToken]` attribute on the controller action triggers the validation.

        3. ASP.NET Core checks:
           - If the form token and cookie token exist.
           - If they match and are valid for the current user/session.

        4. If validation passes, the request is processed. Otherwise, a 400 Bad Request is returned.

    ❕This mechanism ensures that malicious sites cannot submit forms on behalf of authenticated users.
*/

// * Client Side Validation
/*
    🗒️Client-Side Validation Dependencies:
        ASP.NET Core uses **jQuery Validation Unobtrusive** for client-side form validation. This depends on:
            1. jQuery Validation
            2. jQuery

        Therefore, these scripts are typically added in _Layout.cshtml:
            <script src="~/lib/jquery/dist/jquery.min.js"></script>
            @await RenderSectionAsync("Scripts", required: false)
                `required: false` means the "Scripts" section is optional.
                    If a view does **not** define @section Scripts { ... }, the layout will still render without errors.

                `required: true` means the section is **mandatory**.
                    All views using this layout **must** define the "Scripts" section, or an error will occur at runtime.

        ❕To use jQuery Validation Unobtrusive in a Razor view, include:
            @section Scripts {
                <partial name="_ValidationScriptsPartial" />
            }

        This partial view (_ValidationScriptsPartial.cshtml) includes:
            <script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
            <script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>

    🗒️How It Works:
        When a form is rendered, ASP.NET Core generates validation attributes based on model data annotations.

        Example:
            [Required(ErrorMessage = "Name is required.")]
            public string Name { get; set; }

        Generates HTML like:
            <input asp-for="Name" required data-val="true" data-val-required="Name is required." />

        These attributes:
            `data-val`: Enables validation
            `data-val-required`: Contains the error message

        The jQuery Unobtrusive Validation library reads these and performs client-side validation.
        If validation fails, the form is not submitted and the user receives immediate feedback.

    ❕Server-Side Validation:
        Still necessary for security and data integrity since client-side validation can be bypassed.
*/

// * Partial Views
/*
    Partial views are reusable Razor components that can be rendered within other views.
    They help break down complex UIs into smaller, maintainable, and reusable parts.

    A Partial View:
        -> Does not have a layout page.
        -> Cannot be accessed directly via a URL.
        -> Is typically used for UI components like forms, menus, lists, etc.

    🗒️How to Create and Use a Partial View:
        1. Create a Razor view file with the `.cshtml` extension.
           Suggested location: `Views/Shared` or the relevant controller folder.
           Naming convention: Start with an underscore `_PartialViewName.cshtml`.

        2. To render the partial view inside another view:
            Use the Tag Helper syntax: 
               <partial name="_PartialViewName" />

            If passing a model:
               <partial name="_PartialViewName" model="Model.SomeProperty" />
*/

// * Sending Data From Action To A View
/*
    🗒️Sending Data From Action To A View:

        [1] Model
            -> Used to send the **main structured data** from the controller to the view.
            -> It is recommended to use a strongly typed model by specifying the model type in the view(@model MyModel).

            Example:
                public IActionResult Index()
                {
                    var model = new MyModel { Name = "John" };
                    return View(model); // Pass the model to the view
                }

                @model MyModel // Specify the model type.

        [2] ViewData:
            -> Used to send **additional data**:
                1. From action to view
                2. From view to layout
                3. From partial view to main view

            -> Type: ViewDataDictionary ViewData { get; set; }; -> Implements IDictionary<string, object?>.

            -> ViewDataDictionary has a property called Model that contains the passed model from the controller to the view.

            -> Requires Type Casting and enforces type safety.
                When setting data in ViewData, you can use any type (IDictionary<string, object?>)
                When retrieving data from ViewData, you need to cast it.
                Example:
                    string? title = ViewData["Title"] as string; // Type casting needed

        [3] ViewBag:
            -> A dynamic wrapper around ViewData (Both ViewData and ViewBag are accessing the same dictionary).

            -> Allows you to use dynamic properties instead of string keys.

            -> Type: dynamic ViewBag { get; set; };

            -> Does not require type casting and does not enforce type safety.
                When setting data in ViewBag, you can use any type (dynamic).
                When retrieving data from ViewBag, you do not need to cast it.
                Example:
                    ViewBag.Title = "Home Page"; // No type casting needed

*/

// * Sending Data between requests [TempData]
/*
    🗒️TempData Overview:
        -> Used to send data between requests (e.g., from one action to another).
        -> Data is stored in:
            1. Client via Cookies (default)
            2. Server via Session (if configured)
                AddControllerWithViews().AddSessionStateTempDataProvider()

        -> Type: TempDataDictionary TempData { get; set; }; -> Implements IDictionary<string, object?>.

        -> The stored data persists until:
            1. It is read.
            2. The session ends (20 minutes from the last interaction "Default").
            3. The data is explicitly removed.

            Example:
                TempData["Message"] = "Data saved successfully!";
                return RedirectToAction("Index");

                In the Index action, you can access it using:
                string message = TempData["Message"] as string; // Type casting needed


    🗒️Methods:
        1. Keep()
            -> Marks the key/keys for retention.
            Example:
                TempData.Keep("Message"); // Keeps the "Message" for the next request.

        2. Peek()
            -> Retrieves the value without marking key for deletion.
            Example:
                string message = TempData.Peek("Message") as string; // Type casting needed

        3. Remove()
            -> Marks the key for deletion.
            Example:
                TempData.Remove("Message"); // Removes the "Message" from TempData

        TempData["key"]
            -> Key will be marked for deletion
*/

// * Sending Data Between Requests [TempData]
/*
    🗒️ TempData Overview:
        -> Used to persist data between two requests (commonly during redirects).
        -> Backing store options:
            1. Client-side via Cookies (default).
            2. Server-side via Session (if configured):
               services.AddControllersWithViews()
                       .AddSessionStateTempDataProvider();
               app.UseSession(); // Required in middleware

        -> Declaration:
            TempData is of type TempDataDictionary and implements IDictionary<string, object?>.

        -> Lifespan:
            1. Data is removed once it is read.
            2. Or if session ends (default is 20 minutes of inactivity).
            3. Or if explicitly removed.

        Example Usage:
            ❕In Create action
                TempData["Message"] = "Data saved successfully!";
                return RedirectToAction("Index");

            ❕In Index action
                var message = TempData["Message"] as string;

    🗒️TempData Methods:
        1. Keep()
            -> Marks (all keys / specified key )in the dictionary for retention.
            Example:
                TempData.Keep("Message");

        2. Peek()
            -> Reads the value *without* marking it for deletion.
            Example:
                string message = TempData.Peek("Message") as string;

        3. Remove()
            -> Explicitly removes a key-value from TempData.
            Example:
                TempData.Remove("Message");

        ❕Access via TempData["key"] -> marks the key for deletion after the current request.
*/

// * AutoMapper
/*
    AutoMapper is a library that helps map properties from one object type to another, typically used to map between DTOs (Data Transfer Objects) and domain models.

    [1] Define Your Models
        public class Source
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class Destination
        {
            public string FullName { get; set; }
            public int Age { get; set; }
        }

    [2] Create a Mapping Profile
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Source, Destination>()
                    .ForMember(
                        dest => dest.FullName,
                        opt => opt.MapFrom(src => src.Name)
                    )
                    .ReverseMap(); 
                        -> Enables mapping back from Destination to Source
                        -> Use only when two-way mapping is needed.
            }
        }

    [3] Register AutoMapper in the DI Container
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        Alternatively, if using multiple profiles in an assembly:
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

    [4] Inject and Use AutoMapper
        public class MyService
        {
            private readonly IMapper _mapper;

            public MyService(IMapper mapper)
            {
                _mapper = mapper;
            }

            public void MapObjects()
            {
                var source = new Source { Name = "John", Age = 30 };
                var destination = _mapper.Map<Destination>(source);

                Console.WriteLine(destination.FullName); // Output: John
            }
        }
*/