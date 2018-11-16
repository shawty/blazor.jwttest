# blazor.jwttest
Quick test using JWT authentication for a blazor hosted (Client/Serverside) app with API and Authentication.

Nothing Special, it simply has a login form, a changing nav bar based on login state, a small PostgreSQL based data layer that's designed to work with a postgres data base via EF Core (But should work with any DB that EF core supports)

It's not designed to be a mainstream project, it's me learning Blazor and trying to adopt the same ideas and programming model I already use for my Aurelia/.NET Core applications.

At present, you can log in, and add/remove/edit users.

There is an anoying bug in the data layer, in that if you run this on an empty DB to set it up automatically, the first new user you create after the data has seeded will throw an exception.  I think, it's something to do with the postgresql sequence type (it's better than I had 2 revisions ago, I couldn't create new at all), after that first fail however, everything works as expected.

## Note
This MUST be run on an empty PostgreSQL database.  You'll need to update the connection string in the app settings file in the server project, then when run the app will create the two tables it needs and seed an initial user called "admin" with password "letmein"

If there are ANY objects at all in the DB your running against, the create will fail, and then when you run the app, you'll get errors about the tables not existing, I'll add some SQL scripts later on for those who want to create tables manually.

This should be useable against other DB's (since it uses EF) but I've not tested it, so you'll need to do some work yourself for that.  It's all portable code however, and this is really just me playing around with and testing Blazor out, so don't expect amazing code :-)

## Credit
Credit where credit is due, some large chunks of the code in here came from Chris Saintly at Codedaze.io and his article on doing JWT in a blazor app.
