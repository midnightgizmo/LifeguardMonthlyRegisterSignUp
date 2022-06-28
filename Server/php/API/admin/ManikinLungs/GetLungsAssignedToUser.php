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

$LungsUserHas = getListOfLungsUserHas(intval($jsonData->userId), intval($jsonData->maxNumberToReturn));

SendBackResponse($LungsUserHas);
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
        isset($jsonObject->maxNumberToReturn))
    {
        return;
    } 
    else
    {
        sendFailedToProcessRequestResponse();
        die;
    }

}


function getListOfLungsUserHas($userId, $MaxNumberToReturn)
{
    $lungsGivenToUser = null;
    //$unixTimeStamp = createTimeStampFromDate($dateGivenLung);

    if($userId != 0 && $MaxNumberToReturn != 0)
    {
        // get SQLite open connection
        $con = HelperFunctions::createSQLConnection();
        $LungsUserGivenDb = new dbLungsUserGiven($con);
        $lungsGivenToUser = $LungsUserGivenDb->SelectLungsGivenToUser($userId, $MaxNumberToReturn);
        $con->CloseConnection();
    }

    return $lungsGivenToUser;
}


function SendBackResponse($lungsGivenToUser)
{
    
    
    // if lungsGivenToUser is null, data was not added to databse.
    if(is_null($lungsGivenToUser) == true)
    {
        // send back false to indicate we were unable to assign manikin lung to user
        sendFailedToProcessRequestResponse();
    }
    // we have a instance of LungsUserGiven class
    else
    {
        HelperFunctions::createHeadersToSendBackToClient();
        // send back the data that was added to the database.
        // this indicates it was sucsefull
        echo json_encode($lungsGivenToUser);
    }
    
    
    
}


/**
 * Sends back a json 'false' to the client
 * to let them know somthing went wrong and the request was not sucsefull
 */
function sendFailedToProcessRequestResponse()
{
    http_response_code(400);
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = false;
    echo json_encode($dataToSendBackToUser);
}

?>