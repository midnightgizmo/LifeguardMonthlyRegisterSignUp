# asp.net core 5 api server side part of lifeguard monthly register sign up

This is the server side code (written in c# asp.net core 5) to the lifeguard monthly register sign up sheet.
To make this work on your server you need to do the below code changes


### Change values in appsettings.json
navigate to the `appsettings.json` file in the root of the poject directory


Change the values in the file as described below.

> There are several Environment files you can create but only one will get loaded depending on which one has the higher precedence. Some are listed (appsettings.json appsettings.Development & appsettings.Production.json). You can find out more information about which json file will get loaded in the Microsoft documentation. Just search using your favourite search engine for "asp.net core configuration files"



### domainName

The name of the domain we are running on.

e.g. localhost
```php
"domainName" => "localhost"
```

### jwtsecretKey

The phrase used when creating a json web token. this should be a secrete phrase that no one else knows

### sqlConectionStringLocation

The physical location on disk to where the database file exists

Currently the database file is in at the following location (relative to the main root of this program) /Data/Database/dbLifeguardMonthlyRegister.db
but you will need to prefix that location with the servers filesystem location
