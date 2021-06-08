<?php
include_once(dirname(__FILE__).'../../../Includes/AutoLoader.php');

// Setting up some server access controls to allow people to get information
//header("Access-Control-Allow-Origin: *");
//header('Access-Control-Allow-Methods:  POST, GET');

// get the json string sent from the client
//$jsonString = file_get_contents('php://input');

// convert it to a json object
//$jsonData = json_decode($jsonString,false);

// get what the client sent in json string and convert it to an stdClass object
$jsonData = HelperFunctions::getClientJsonObject();

// get the log in details the client sent
$userSurname = $jsonData->userName;
// get the year month and day the user was born and convert them to a DateTime object

$date = new DateTime($jsonData->year . '-' . $jsonData->month . '-' . $jsonData->day . '  00:00:00.000000', new DateTimeZone('Europe/London'));

$myTemp = $date->getTimezone();

// convert the date of birth to a unix time stamp and then convert that to a string
$userDateOfBirth =  strval($date->getTimestamp());


$authentication = new Authentication();
// get SQLite open connection
$con = HelperFunctions::createSQLConnection();
$dbUser = new dbSQLiteUser($con);

// try and find the user in the database by there surname and hashed date of birth
$userInfo = $dbUser->select_By_SurnameAndPassword($userSurname,$authentication->hashPassword($userDateOfBirth));

// close the connection to the database
$con->CloseConnection();







// this will be converted to a json string and sent back to the user
$loginAuthentication = new LoginAuthentication();

if(is_null(($userInfo)))
{
    HelperFunctions::createHeadersToSendBackToClient();
    $loginAuthentication->_isLoggedIn = false;
    $loginAuthentication->_errorMessage = 'invalid username or password';
    
    echo json_encode($loginAuthentication);
    die;
}
else
{
    // set to 60 days (in milli seconds) (also used for the json web token)
    //                 1 second      1 min          1 hour       1 day      number of days cookie to last
    $cookieMaxAge = 1000       *  60      *      60       *   24    *    30;
    
    $authentication->setCookieMaxAge($cookieMaxAge);
    // create a jwt and send it back to the client in the form of a cookie
    $jwtString = $authentication->createJWT($userInfo->id,
                                            $userInfo->firstName,
                                            $userInfo->surname);



    // create or update the cookie
    // httpOnly needs to be false because the client needs to be able to look for the cookie to see if the client is logged in.
    setcookie( $authentication->getCookieName() , $jwtString, $cookieMaxAge, '/' , Settings::get_domain_name() , false , false );
    
    
    $loginAuthentication->_isLoggedIn = true;
    $loginAuthentication->_jwt = $jwtString;

    // send the json string back to the client
    // header('Content-Type: application/json');
    // header('Access-Control-Allow-Credentials: true');
    // header('Access-Control-Allow-Origin:http://localhost:8080');
    HelperFunctions::createHeadersToSendBackToClient();
    echo json_encode($loginAuthentication);

}



