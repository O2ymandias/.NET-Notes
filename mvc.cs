// * Into
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