<?php

$wasSucsesfull = include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');


// get what the client sent in json string and convert it to an stdClass object
$jsonData = getDataSentFromClient();

// get the log in details the client sent
$username = $jsonData->userName;
$password = $jsonData->password;


// gets the correct admin user name and password details from the database
$adminLoginDetailsArray = getAdminLoginDetailsFromDatabase();

// checks the username and password the clieint sent to make sure they are correct
areClientLoginDetailsCorrect($username,$password, $adminLoginDetailsArray[0], $adminLoginDetailsArray[1]);

// creates a cookie containing the json web token the client will use to work out if the admin is logged in or notj.
// sends back a login sucess to the client to let them know they have logged in
sendLoginSucsessBackToClient();







/**
 * converts the json data the client sent (username & password) to an object and returns it
 */
function getDataSentFromClient()
{
    return HelperFunctions::getClientJsonObject();
}

/**
 * gets the admin login username and hashed password from the database
 * @return Array 
 */
function getAdminLoginDetailsFromDatabase()
{
    $authentication = new Authentication();
    // get SQLite open connection
    $con = HelperFunctions::createSQLConnection();
    $dbAdmin = new dbSQLiteAdmin($con);
    
    // try and find the admin username and password in the database
    $adminLoginInfo = $dbAdmin->selectUsernameAndPassword();
    
    // close the connection to the database
    $con->CloseConnection();

    // if we were unable to get the login info
    if(count($adminLoginInfo) != 2)
    {
        // tell the client we were unable to log them in
        returnBadLoginToClient();
        die;
    }

    return $adminLoginInfo;
}

function areClientLoginDetailsCorrect($usernameClientSent,$passwordClientSent, $userNameFromDatabase, $hashedPasswordFromDatabase)
{
    // hash the client password they sent
    $authentication = new Authentication();
    $clientHashedPassword = $authentication->hashPassword(strtolower($passwordClientSent));

    // make sure the login details the user sent in are correct
    if(strtolower($usernameClientSent) == strtolower($userNameFromDatabase) && 
       strtolower($clientHashedPassword) == $hashedPasswordFromDatabase) 
    {
        return true; 
    }
    // log in details user sent in are wrong
    else
    {
        // tell the client we were unable to log them in
        returnBadLoginToClient();
        die;
    }
}

function sendLoginSucsessBackToClient()
{
    

    // this will be converted to a json string and sent back to the user
    $loginAuthentication = new LoginAuthentication();

    // set to 60 days (in milli seconds) (also used for the json web token)
    //                 1 second      1 min          1 hour       1 day      number of days cookie to last
    $cookieMaxAge = 1000       *  60      *      60       *   24    *    30;
    
    $authentication = new Authentication();
    $authentication->setCookieMaxAge($cookieMaxAge);
    // create a jwt and send it back to the client in the form of a cookie
    $jwtString = $authentication->createAdminJWT();

    // create or update the cookie
    // httpOnly needs to be false because the client needs to be able to look for the cookie to see if the client is logged in.
    setcookie ( $authentication->getAdminCookieName() , $jwtString , $cookieMaxAge , '/' , Settings::get_domain_name() , false , false );
    
    $loginAuthentication->_isLoggedIn = true;
    $loginAuthentication->_jwt = $jwtString;

    HelperFunctions::createHeadersToSendBackToClient();
    echo json_encode($loginAuthentication);
}


function returnBadLoginToClient()
{
    $loginAuthentication = new LoginAuthentication();

    $loginAuthentication->_isLoggedIn = false;
    $loginAuthentication->_errorMessage = 'invalid username or password';
    
    HelperFunctions::createHeadersToSendBackToClient();
    echo json_encode($loginAuthentication);
    die;
}