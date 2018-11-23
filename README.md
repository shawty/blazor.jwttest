# blazor.jwttest
Quick test using JWT authentication for a blazor hosted (Client/Serverside) app with API and Authentication.

Nothing Special, it simply has a login form, a changing nav bar based on login state, a small PostgreSQL based data layer that's designed to work with a postgres data base via EF Core (But should work with any DB that EF core supports)

It's not designed to be a mainstream project, it's me learning Blazor and trying to adopt the same ideas and programming model I already use for my Aurelia/.NET Core applications.

The template is more or less feature complete to what I wanted to make it do, it does however still run on Blazor 0.6.0 (See issue 1 in the issues for the reason)

## features
* JWT based token authentication
* Roles from user record in database are used to prevent navigation to pages the logged in user does not have access too, and also protect the rest endpoints in the server
* Custom component's and event based communication to parent pages
* Generic design where permitted
* all cross platform, dotnet core throughout
* Entity framework datalayer, which will build initial database if run on an empty DB server, and seed data
* Singleton based application state, accessible throughout the app

## Note
This MUST be run on an empty PostgreSQL database.  You'll need to update the connection string in the app settings file in the server project, then when run the app will create the two tables it needs and seed an initial user called "admin" with password "letmein"

If there are ANY objects at all in the DB your running against, the create will fail, and then when you run the app, you'll get errors about the tables not existing, I'll add some SQL scripts later on for those who want to create tables manually.

This should be useable against other DB's (since it uses EF) but I've not tested it, so you'll need to do some work yourself for that.  It's all portable code however, and this is really just me playing around with and testing Blazor out, so don't expect amazing code :-)

## Credit
Credit where credit is due, some large chunks of the code in here came from Chris Saintly at Codedaze.io and his article on doing JWT in a blazor app, I'd also like to say thanks to @SQL-MisterMagoo and @kswoll in the Blazor gitter group for pointers on component communication, and overriding the HTTP client to get better error handling.
