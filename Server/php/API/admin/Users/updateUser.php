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

isRecievedDataAcceptable($jsonData);

$userId = $jsonData->userId;
$userFirstName = trim($jsonData->userFirstName);
$userSurname = trim($jsonData->userSurname);
$userIsActive = $jsonData->usersIsActive ? '1' : '0';
$userPassword = $jsonData->userNewPassword;

$updatedUserDetails = updateUser($userId, $userFirstName, $userSurname, $userIsActive, $userPassword);

sendSucsessMessageBackToClient($updatedUserDetails);



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
    if(isset($jsonObject->userId) &&
        isset($jsonObject->userFirstName) &&
        isset($jsonObject->userSurname) &&
        isset($jsonObject->usersIsActive) &&
        isset($jsonObject->userNewPassword) )
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
 * Checks to make sure the data sent from the client is accetable
 */
function isRecievedDataAcceptable($jsonObject)
{
    if (is_numeric($jsonObject->userId) == false)
    {
        sendFailedToProcessRequestResponse();
        die;
    }
    if(strlen(trim($jsonObject->userFirstName)) < 1)
    {
        sendFailedToProcessRequestResponse();
        die;
    }
    if(strlen(trim($jsonObject->userSurname)) < 1)
    {
        sendFailedToProcessRequestResponse();
        die;
    }
    if(is_bool($jsonObject->usersIsActive) == false)
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

/**
 * updates the user with the passed in details. If $userPassword string length is set to zero,
 * it will not be updated and the old password will be kept
 */
function updateUser($userId, $userFirstName, $userSurname, $userIsActive, $userPassword)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbUser = new dbSQLiteUser($con);

    // update the users details
    $updatedUserDetails = $dbUser->updateDetails($userId, $userFirstName,$userSurname,$userIsActive);
    

    
    $wasPasswordUpdated = false;
    // check to see if the password needs updateing
    if(strlen($userPassword) > 0)
    {
        
        $authentication = new Authentication();
        $updatedUserWithNewPassword = $dbUser->updatePassword($userId, $authentication->hashPassword(createTimeStampFromDate($userPassword)) );
        
        $wasPasswordUpdated = true;
        
    }

    $con->CloseConnection();

    //  make sure the details were updated ok
    if(is_null($updatedUserDetails) == true || ( $wasPasswordUpdated == true && is_null($updatedUserWithNewPassword)) )
    {
        sendFailedToProcessRequestResponse();
        die;
    }

    

    return $updatedUserDetails;

}

function sendSucsessMessageBackToClient($updatedUserDetails)
{
    // don't send the password back
    $updatedUserDetails->password = '';

    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = $updatedUserDetails;
    echo json_encode($dataToSendBackToUser);
}

/**
 * Sends back a json User object to the client with the id 
 * property set to -1 to indicate there was a problem
 */
function sendFailedToProcessRequestResponse()
{
    http_response_code(400);
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = false;
    echo json_encode($dataToSendBackToUser);
}