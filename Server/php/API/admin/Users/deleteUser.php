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

// remove the user from the database and also any registers they appeared in
deleteUser($jsonData->userID);


// let the client know the user was deleted
sendResponseToClient(true);










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
    if(isset($jsonObject->userID) )
    {
        return;
    } 
    else
    {
        sendResponseToClient(false);
        die;
    }

}

/**
 * Removes the user from the database and any registers they are in
 */
function deleteUser($userID)
{
    $wasSucsefull = false;

    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();

    // remove all the lungs assigned to this user
    $dbLungsGivenToUser = new dbLungsUserGiven($con);
    $dbLungsGivenToUser->DeleteUser($userID);
    
    // remove the user from the registers (if in any)
    $dbUsersInRegister = new dbSQLiteUsersInTrainingRegister($con);
    $dbUsersInRegister->deleteUserFromAllRegisters($userID);

    // remove the user
    $dbUser = new dbSQLiteUser($con);
    $wasSucsefull = $dbUser->delete($userID);

    // close the sql connection
    $con->CloseConnection();

    // if all went well return from function
    if($wasSucsefull)
        return;
    // if we were unable to delete the user, send false back to the client
    else
        sendResponseToClient(false);


}



/**
 * Sends back to the client a Json string set to true or false
 */
function sendResponseToClient($wasSucsesfull)
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = $wasSucsesfull;
    echo json_encode($dataToSendBackToUser);
}