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

// extract the dates
$dateOfTraining = $jsonData->dateOfTraining;
$dateWhenUserCanSeeRegister = $jsonData->dateWhenUserCanSeeRegister;
$dateWhenUserCanJoinRegister = $jsonData->dateWhenUserCanJoinRegister;
$maxNumberCandidates = $jsonData->maxNumberOfCandidatesAllowedOnRegister;




// public static DateTime::createFromFormat ( string $format , string $datetime , DateTimeZone|null $timezone = null ) : DateTime|false
// dateWhenUserCanSeeRegister:"2020-12-06T00:00:00.000Z"

// get the timezone we are using
// $timeZone = new DateTimeZone( Settings::getTimeZone() );
// $aDate = DateTime::createFromFormat("Y-m-dTH:i:s:v",$dateOfTraining, $timeZone);
// $aDate = DateTime::createFromFormat('Y-m-d\TH:i:sP',$dateOfTraining, $timeZone);
// $aDate = DateTime::createFromFormat('Y-m-d\TH:i:s.vP',$dateOfTraining);

$trainingDate = createDate($dateOfTraining);
$whenCanUsersSeeRegisterDate = createDate($dateWhenUserCanSeeRegister);
$whenCanUsersJoinRegisterDate = createDate($dateWhenUserCanJoinRegister);


createRegister($trainingDate,$whenCanUsersSeeRegisterDate,$whenCanUsersJoinRegisterDate,$maxNumberCandidates);

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
    if(isset($jsonObject->dateOfTraining) &&
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
    /*
    $aDate = DateTime::createFromFormat('Y-m-d\TH:i:s.vP',$dateAsString);
    


    // create the date in a formated string
    $endDateAsString = $aDate->format('Y') .'-'.$aDate->format('m') . '-' . $aDate->format('d');
    $endDateAsString .= ' ' . $aDate->format('H') . ':' . $aDate->format('i') . ':' . $aDate->format('s') . '.000000';

    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    // create a DateTime object using the end date string
    $startDate = date_create ( $endDateAsString , $timeZone );

    */

    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    $startDate = date_create ( $dateAsString , $timeZone );

    return $startDate;

}

/**
 * Inserts a new register into the database
 */
function createRegister($dateOfTraining,$dateWhenUserCanSeeRegister,$dateWhenUserCanJoinRegister,$maxNumberCandidates)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbRegister = new dbSQLiteTrainingRegister($con);

    // convert all the dates to unix time stamp
    // and insert the new register into the database
    $newTrainingRegister = $dbRegister->insert($dateOfTraining->getTimestamp(),$dateWhenUserCanSeeRegister->getTimestamp(),$dateWhenUserCanJoinRegister->getTimestamp(),$maxNumberCandidates);

    $con->CloseConnection();

    // if we were unsucsefull in adding a new register to the database
    if(is_null($newTrainingRegister) == true)
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
 * to let them know the creation of a new register was sucsefull
 */
function sendSucsessMessageBackToClient()
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = true;
    echo json_encode($dataToSendBackToUser);
}