# Bidster
Simple event fundraiser bidding system built on ASP.NET Core. High level features include

- Support for multiple events with friendly URLs (eg. `https://localhost:5000/my-fundraiser`), with start and end date/times
- Multiple products per event, with ability to set a starting price and minimum bid amount
- Users can place mulitple bids on items
- See [issues](https://github.com/mmillican/Bidster/issues?q=is%3Aopen+is%3Aissue+label%3Aenhancement) for a backlog

## Running locally

The project can run with either IIS Express or `dotnet run` on the CLI. Locally, it uses `MSLocalDB` 
and should create the database for you, otherwise run `dotnet ef database update`.

There is a flag in `appsettings.json` for doing a first user check. If set to `true`, the system will do a check
on registration and if it's the first user, will create an "Admin" role and assign the user to it.
