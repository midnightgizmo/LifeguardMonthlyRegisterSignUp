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

// get the register id we were sent
$registerID = intval($jsonData->registerID);

$registerInfo = getRegister($registerID);

sendRegisterToClient($registerInfo);

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
        sendBlankRegisterToClient();
        die;
    }

}


function getRegister($registerID)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbRegister = new dbSQLiteTrainingRegister($con);

    // convert all the dates to unix time stamp
    // and update the register in the database
    $registerInfo = $dbRegister->select($registerID);
    $con->CloseConnection();

    // if we could not find the register in the database
    if(is_null($registerInfo) == true)
    {
        sendBlankRegisterToClient();
    }

    return $registerInfo;


}


function sendRegisterToClient($registerInfo)
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    // send back a register object with its id set to -1 to indicate we were unable
    // to get the register.
    echo json_encode($registerInfo);
}



function sendBlankRegisterToClient()
{
    $aRegister = new TrainingRegister();
    $aRegister->id = -1;
    HelperFunctions::createHeadersToSendBackToClient();
    
    // send back a register object with its id set to -1 to indicate we were unable
    // to get the register.
    echo json_encode($aRegister);
}