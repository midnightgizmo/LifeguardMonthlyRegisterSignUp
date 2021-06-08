
<?php

include_once(dirname(__FILE__).'/../../../Includes/AutoLoader.php');

//'2019-04-01 10:32:00';

// This page requires admin user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if admin user is loggedin
if(!$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();


// get what the client sent in json string and convert it to an stdClass object
$jsonData = getDataSentFromClient();
// get the start date the user sent
$startDate = getStartDate($jsonData);
// get the end date the user sent
$endDate = getEndDate($jsonData);

// get all registers between the 2 dates
$listOfRegisters = getRegisterBetween($startDate,$endDate);
// converts the list of registers to a json object and sends them back to the client
sendRegisterBackToClient($listOfRegisters);

/**
 * converts the json data the client sent (username & password) to an object and returns it
 */
function getDataSentFromClient()
{
    return HelperFunctions::getClientJsonObject();
}

/**
 * gets the start date from the data the clietn sent in
 * @return DateTime
 */
function getStartDate($jsonObject)
{
    // get the timezone we are using
    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    
    // get the data the client sent for the start date
    $startDateDay = intval($jsonObject->startDateDay);
    $startDateMonth = intval($jsonObject->startDateMonth);
    $startDateYear = intval($jsonObject->startDateYear);

    // if we were unabel to get the dates
    if($startDateDay == 0 || $startDateMonth == 0 || $startDateYear == 0)
    {
        sendBackEmptyArray();
        die;
    }

    // create the date in a formated string
    $startDateAsString = $startDateYear .'-'.$startDateMonth . '-' . $startDateDay;

    // create a DateTime object using the start date string
    $startDate = date_create ( $startDateAsString , $timeZone );

    return $startDate;
}

/**
 * gets the end date from the data the client sent in
 * @return DateTime
 */
function getEndDate($jsonObject)
{
    // get the timezone we are using
    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    
    // get the data the client sent for the end date
    $endDateDay = intval($jsonObject->endDateDay);
    $endDateMonth = intval($jsonObject->endDateMonth);
    $endDateYear = intval($jsonObject->endDateYear);

    // if we were unabel to get the dates
    if($endDateDay == 0 || $endDateMonth == 0 || $endDateYear == 0)
    {
        sendBackEmptyArray();
        die;
    }


    // create the date in a formated string
    $endDateAsString = $endDateYear .'-'.$endDateMonth . '-' . $endDateDay;

    // create a DateTime object using the end date string
    $startDate = date_create ( $endDateAsString , $timeZone );

    return $startDate;
}


/**
 * finds all the registers in the database that exist between the start and end dates
 * @return array
 */
function getRegisterBetween($startDate,$endDate)
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbRegister = new dbSQLiteTrainingRegister($con);

    // get training records staff are allowed to see
    // (start of this month to six months onwards)
    $trainingRegisterArray = $dbRegister->select_dateTimeOfTraining_Between($startDate->format('U'),$endDate->format('U'));

    $con->CloseConnection();

    return $trainingRegisterArray;
}

/**
 * converts the list of registers to a json object and sends them back to the client
 */
function sendRegisterBackToClient($listOfRegisters)
{
    HelperFunctions::createHeadersToSendBackToClient();
    echo json_encode($listOfRegisters);
}

/**
 * if we are unable to forfill the users request we will send back
 * an empty array
 */
function sendBackEmptyArray()
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    $dataToSendBackToUser = array();
    echo json_encode($dataToSendBackToUser);
}