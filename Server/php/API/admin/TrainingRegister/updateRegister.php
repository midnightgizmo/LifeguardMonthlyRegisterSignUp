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

// extract the id
$registerID = intval($jsonData->registerID);
// extract the dates
$dateOfTraining = $jsonData->dateOfTraining;
$dateWhenUserCanSeeRegister = $jsonData->dateWhenUserCanSeeRegister;
$dateWhenUserCanJoinRegister = $jsonData->dateWhenUserCanJoinRegister;
$maxNumberCandidates = $jsonData->maxNumberOfCandidatesAllowedOnRegister;


// turn the dates (that are strings) that were sent from the client into a DateTime Object
$trainingDate = createDate($dateOfTraining);
$whenCanUsersSeeRegisterDate = createDate($dateWhenUserCanSeeRegister);
$whenCanUsersJoinRegisterDate = createDate($dateWhenUserCanJoinRegister);


UpdateRegister($registerID,$trainingDate,$whenCanUsersSeeRegisterDate,$whenCanUsersJoinRegisterDate,$maxNumberCandidates);

// let the client know creating of a register was sucesfull
sendSucsessMessageBackToClient();



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
    if(isset($jsonObject->registerID) &&
        isset($jsonObject->dateOfTraining) &&
        isset($jsonObject->dateWhenUserCanSeeRegister) &&
        isset($jsonObject->dateWhenUserCanJoinRegister) &&
        isset($jsonObject->maxNumberOfCandidatesAllowedOnRegister))
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
  * Takes the date we received from the client as a string
  * and converts it to a DateTime object
  */
function createDate($dateAsString)
{
  
    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    $startDate = date_create ( $dateAsString , $timeZone );

    return $startDate;

}

/**
 * updates the register in the database with the specified registerID
 */
function UpdateRegister($registerID, $dateOfTraining,$dateWhenUserCanSeeRegister,$dateWhenUserCanJoinRegister,$maxNumberCandidates)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbRegister = new dbSQLiteTrainingRegister($con);

    // convert all the dates to unix time stamp
    // and update the register in the database
    $updatedRegister = $dbRegister->update($registerID,$dateOfTraining->getTimestamp(),$dateWhenUserCanSeeRegister->getTimestamp(),$dateWhenUserCanJoinRegister->getTimestamp(),$maxNumberCandidates);
    $con->CloseConnection();

    // if we were unsucsefull in updating the register in the database
    if(is_null($updatedRegister) == true)
    {
        // send failed response back to the user
        sendFailedToProcessRequestResponse();
        die;
    }

    return;
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
/**
 * Sends back a json 'true' to the client
 * to let them know the register was updated sucsefully
 */
function sendSucsessMessageBackToClient()
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = true;
    echo json_encode($dataToSendBackToUser);
}