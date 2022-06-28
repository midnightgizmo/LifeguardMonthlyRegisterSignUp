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

$wasDeletedFromDatabase = removeManikinLungFromUser(intval($jsonData->userID),intval($manikinLungTypeID),$DateGivenLung);

// send back a response to the user. if passed in parameter is true, its a good response
// if passed parameter is false, its a bad response
SendBackResponse($wasDeletedFromDatabase);




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
    if(isset($jsonObject->userID) &&
        isset($jsonObject->manikinLungTypeID) &&
        isset($jsonObject->DateGivenLung))
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
 * @return true if deleted, else false
 */
function removeManikinLungFromUser($userID, $manikinLungTypeID, $dateGivenLung)
{
    $unixTimeStamp = createTimeStampFromDate($dateGivenLung);

    if($userID != 0 && $manikinLungTypeID = 0 & $unixTimeStamp != 0)
    {
        // get SQLite open connection
        $con = HelperFunctions::createSQLConnection();
        $LungsUserGivenDb = new dbLungsUserGiven($con);
        $WasDeleted = $LungsUserGivenDb->DeleteRow($userID, $manikinLungTypeID, $unixTimeStamp);
        $con->CloseConnection();

        return $WasDeleted;
    }
    else
        return false;
}

/**
 * Convert the passed in date to a unix time stamp
 */
function createTimeStampFromDate($dateAsString)
{
    $date = new DateTime($dateAsString . '  00:00:00.000000', new DateTimeZone('Europe/London'));
    
    return strval($date->getTimestamp());
}



function SendBackResponse($WasSucsefull)
{
    HelperFunctions::createHeadersToSendBackToClient();

    // if deletion from the database was done, send back true response, else send back false response.
    echo json_decode($WasSucsefull);
}

/**
 * Sends back a json 'false' to the client
 * to let them know somthing went wrong and the request was not sucsefull
 */
function sendFailedToProcessRequestResponse()
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = false;
    echo json_encode($dataToSendBackToUser);
}

?>