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

// remove any users that might be in the register from the register
removeUsersInRegister($jsonData->registerID);
// try and remove the register from the datbase
$wasSucsesfull = removeRegister($jsonData->registerID);
// send back a response to the user to let them know if the register was removed.
sendResponseBackToClient($wasSucsesfull);





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
    if(isset($jsonObject->registerID))
    {
        return;
    } 
    else
    {
        sendResponseBackToClient(false);
        die;
    }

}

// removes any users found in the training register
function removeUsersInRegister($registerID)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbUsersInRegister = new dbSQLiteUsersInTrainingRegister($con);
    // remove users from the register
    $dbUsersInRegister->deleteAllUsersFromRegister($registerID);
    $con->CloseConnection();
}

/**
 * Atempts to remove the register from the database
 */
function removeRegister($registerID)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbRegister = new dbSQLiteTrainingRegister($con);
    $wasSucesfull = $dbRegister->delete($registerID);
    $con->CloseConnection();

    return $wasSucesfull;
}


/**
 * send a response back to the user to let them know
 * if the register was deleted (true means it was delted, false means it was not)
 */
function sendResponseBackToClient($wasSucsesfull)
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = $wasSucsesfull;
    echo json_encode($dataToSendBackToUser);
}