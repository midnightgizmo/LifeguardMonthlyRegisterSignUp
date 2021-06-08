# php server side part of lifeguard monthly register sign up

This is the server side code (written in php) to the lifeguard monthly register sign up sheet.
To make this work on your server you need to do the below code changes


### Step 1.
navigate to the "/Classess/Settings.php"

find the function that has the name
```php
public static function isInDeveloperMode()
```
Change the value to true if in dev mode or false if in production mode.

You may also need to change the client sides port in the function
```php
public static function getClientOrigin()
```
By default, in dev mode, it is set to :8080 and in production it is set to default (:80)


## Step 2.

navigate to the "/env.php" file

Change the values in the file as described below.

> There are 3 Environment files you can create (env.php env.development.php & env.production.php). The order of precedence (from high to low) to determine which one is loaded is env.production.php, env.development.env then env.php

### ProjectFolderLocationOnWebSite

The root location of where this project exists .

e.g. if the site url is http://somthing.com and this folder exists at the main root of the site.
```php
"ProjectFolderLocationOnWebSite" => "/",
```
e.g. if the site url is http://somthing.com and the folder exists at '/my-project/server/
```php
"ProjectFolderLocationOnWebSite" => "/my-project/server/",
```

### sqlConectionStringLocation

The physical location on disk to where the database file exists

Currently the database file is in at the following location (relative to the main root of this program) /Data/Database/dbLifeguardMonthlyRegister.db
but you will need to prefix that location with the servers filesystem location

### domainName

The name of the domain we are running on.

e.g. localhost
```php
"domainName" => "localhost"
```


### Step 3.

You may not need to do this step.

run composer to install the 3rd party rbdwllr/reallysimplejwt libary.
The root folder should hold a vender folder with this already downloaded so you may not need to do this step.


