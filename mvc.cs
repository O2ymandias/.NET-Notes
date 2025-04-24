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
    🗒️ What is Dependency Injection?
        A design pattern used to implement IoC (Inversion of Control).
        It allows a class to receive its dependencies from an external source rather than creating them itself.

    🗒️ Why use Dependency Injection?
        - Promotes loose coupling between classes.
        - Enhances testability (easier to mock dependencies).
        - Improves code maintainability and readability.

    🗒️ Register Built-in Services

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

                Plus adds:
                    1. Razor View Engine:
                        Enables `.cshtml` file rendering using Razor syntax.
                        Used to generate dynamic HTML for browser-based apps.

                    2. View Helpers:
                        Provides `HtmlHelper`, `UrlHelper`, `ViewData`, `ViewBag`, etc.

                    3. Layouts and Partial Views:
                        Supports view composition via `_Layout.cshtml`, partial views, etc.

            → Enables support for Controllers and Views.

        [3] AddRazorPages()
            → Registers Razor Pages services
            → Ideal for building web applications using Razor Pages.

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