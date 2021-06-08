<?php
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');

// This page requires admin user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if admin user is loggedin
if(!$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();


// get what the client sent in json string and convert it to an stdClass object
$jsonData = getDataSentFromClient();
// make sure we have recieved the data
haveWeRecievedData($jsonData);

$authentication = new Authentication();
// create the user in the databse and get back the created user
$newUser = createUser($jsonData->firstName, $jsonData->surname, $jsonData->isActive, $authentication->hashPassword(createTimeStampFromDate($jsonData->password)));

// send back the created user to the client
sendSucsessMessageBackToClient($newUser);











/**
 * converts the json data the client sent (username & password) to an object and returns it
 */
function getDataSentFromClient()
{
    return HelperFunctions::getClientJsonObject();
}


/**
 * Checks to see if the client has sent any data.
 * If no data is found, they are sent a failed response
 * to indicate register was not added
 */
function haveWeRecievedData($jsonObject)
{
    // make sure all the veriables are set
    if(isset($jsonObject->firstName) &&
        isset($jsonObject->surname) &&
        isset($jsonObject->isActive) &&
        isset($jsonObject->password) )
    {
        return;
    } 
    else
    {
        sendFailedToProcessRequestResponse();
        die;
    }

}

/**
 * Convert the passed in date to a unix time stamp
 */
function createTimeStampFromDate($dateAsString)
{
    $date = new DateTime($dateAsString . '  00:00:00.000000', new DateTimeZone('Europe/London'));
    
    return strval($date->getTimestamp());
}

function createUser($firstName, $surname, $isActive, $password)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbUser = new dbSQLiteUser($con);
    $newUser = $dbUser->insert($firstName,$surname,$password,$isActive == true ? '1' : '0');
    $con->CloseConnection();

    // if the user was not created
    if(is_null($newUser))
    {
        sendFailedToProcessRequestResponse();
        die;
    }

    // user was created
    return $newUser;
}


/**
 * Sends back to the client the newUserModel in json format
 */
function sendSucsessMessageBackToClient($newUserModel)
{

    HelperFunctions::createHeadersToSendBackToClient();
    
    // don't send the password back
    $newUserModel->password = '';
    $dataToSendBackToUser = $newUserModel;
    echo json_encode($dataToSendBackToUser);
}

/**
 * Sends back a json User object to the client with the id 
 * property set to -1 to indicate there was a problem
 */
function sendFailedToProcessRequestResponse()
{
    $aUser = new User();
    $aUser->id = -1;

    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = $aUser;
    echo json_encode($dataToSendBackToUser);
}