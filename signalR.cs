// * Intro to SignalR
/*
    SignalR
        It’s a framework that allows you to build real-time web applications using:
            1. WebSockets         (preferred, fastest)
            2. Server-Sent Events (fallback)
            3. Long Polling       (fallback if others unavailable)

        SignalR automatically selects the best transport method available
        based on the client and server capabilities.

    HTTP
        It is a request-response protocol where the client sends a request
        to the server, and the server responds back.

        Each request is independent, and once the server responds,
        the connection is closed.



    WebSockets
        It is a communication protocol that creates a persistent,
        two-way (full-duplex) connection between the client and the server.

        Unlike HTTP where the client must send a request each time,
        WebSockets keep the connection open, allowing both sides to send
        data at any time with very low latency.



    Server-Sent Events (SSE)
        It is a web technology that allows the server to push real-time
        updates to the client, but only in one direction (server -> client).

        It uses a long-lived HTTP connection where the server continuously
        streams events/data to the client as they occur.



    Long Polling
        It is a technique that provides real-time-like updates using 
        only regular HTTP (no WebSockets or SSE needed).

        It is not a special protocol, just a smart use of HTTP:
            - The client sends a request to the server.
            - If no new data is available, the server holds the request open.
            - When new data arrives (or a timeout occurs), the server responds.
            - After receiving a response, the client immediately sends 
              another request, creating a continuous loop.

        This gives an “almost real-time” experience but is less efficient
        than WebSockets or SSE.



    What is a Hub?
        It’s a central communication point between the server and clients.
        A Hub is a server-side class that handles client requests and sends
        responses or notifications back to clients.



    SignalR Typical Flow:
        1. Create a SignalR Hub (inherit from Hub class)
            public class ChatHub : Hub
            {
                // Hub methods here
            }

        2. Add Methods to the Hub (public methods clients can invoke)

        3. Configure SignalR in Program.cs
            builder.Services.AddSignalR();
            app.MapHub<ChatHub>("/hubs/chatHub");

        4. Add client-side SignalR library (@microsoft/signalr)
            npm install @microsoft/signalr

        5. Connect to SignalR Hub from client-side (JavaScript)
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/chatHub")
                .build();
            await connection.start();

        6.1. Client invokes a SignalR Hub method

            1. Using send()
                await connection.send("MethodName", arg1, arg2);
                
                - Sends a message to the Hub method.
                - Does NOT wait for a return value from the server.
                - Best for "fire-and-forget" operations where you just notify the server.

            2. Using invoke()
                await connection
                    .invoke("MethodName", arg1, arg2)
                    .then((result) => console.log(result))
                    .catch(console.error);

                - Sends a message to the Hub method.
                - Waits for the Hub method to return a value.
                - Can handle server responses or errors.
                - Best for operations where you need feedback from the server.

        6.2. Hub invokes a client-side method to notify clients
            await Clients.All.SendAsync("ReceiveMessage", message);

    Hub Lifecycle Methods:
        public override async Task OnConnectedAsync()
        {
            // Called when a client connects
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Called when a client disconnects
            await base.OnDisconnectedAsync(exception);
        }



    Important Hub Properties:
        - Context: Information about the connection
        - Context.ConnectionId: Unique ID for each client
        - Clients: Access to connected clients
        - Groups: Manage client groups



    Hub Client Methods:
        - Clients.All               -> Send to all connected clients
        - Clients.Caller            -> Send to the caller only
        - Clients.Others            -> Send to all except the caller
        - Clients.Client(id)        -> Send to a specific client
        - Clients.Clients(ids)      -> Send to multiple specific clients
        - Clients.AllExcept(ids)    -> Send to all except specified clients
        - Clients.User(userId)     -> Send to a specific user (Identity user)
        - Clients.Users(userIds)   -> Send to multiple specific users (Identity users)
*/
