# blazor.jwttest
Quick test using JWT authentication for a blazor hosted (Client/Serverside) app with API and Authentication.

Nothing Special, it simply has a login form, a changing menu bar based on login state, a small PostgreSQL based data layer that's designed to work with a postgres data base via EF Core (So should work with any DB that EF core supports)

It's not designed to be a mainstream project, it's me learning Blazor and trying to adopt the same ideas and programming model I already use for my Aurelia/.NET Core applications.

At the moment, it works but there is nothing in the way of client side page handling, so once your logged in, you can access anything that requires you to be logged in.

Actually, if you know the page route, at present you can access that page directly by typing it in, but the requests to the API in the back end won't work as ASP.NET won't have authenticated them.

It's work in progress basically, and what you see is what you get.

I will be updating it, and I am in the process of porting the Aurelia Authentication and JWT libraries to work with it, keep checking back as I update my main application, I was also copy those changes into this public sample version.

Credit where credit is due, some large chunks of the code in here came from Chris Saintly at Codedaze.io
