<?php
// gets all the training registers and the people booked into them for the specified month
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');


// This page requires user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if user is loggedin or if Admin is logged in (this can be accessed by user or admin)
if(!$authentication->isLoggedIn() && !$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();



$selectedYear = null;
$selectedMonth = null;

try 
{
    // get what the client sent in json string and convert it to an stdClass object
    $jsonData = HelperFunctions::getClientJsonObject();

    $selectedYear = intval($jsonData->year);
    $selectedMonth = intval($jsonData->month);
}
catch (\Throwable $th) 
{
    
}

// if we could not find the year or month (which should have been sent from the client)
if($selectedYear == 0 || $selectedMonth == 0)
{
    
    HelperFunctions::createHeadersToSendBackToClient();
    // send back an empty array
    $RegisterArray = array();    
    echo json_encode($RegisterArray);
    die;

}
// add one to the month because client starts months at zero
$selectedMonth++;

// if we make it this far we have recieved a year and month from the client

// construct a string representation of the first day of the month for the passed in year and month from the client.
// This will be used to create the DateTime objects
$query_date = $selectedYear .'-'.$selectedMonth . '-01';


// the the time zone we are working with
$timeZone = new DateTimeZone( Settings::getTimeZone() );
// create a DateTime object for the first day of the month based on the year and month passed to us from the client
$dateFromStartOfSelectedMonth = date_create ( $query_date , $timeZone );
// create a DateTime object for the end of the month
$dateFromEndOfSelectedMonth = new DateTime($dateFromStartOfSelectedMonth->format('Y-m-t 00:00:00'), $timeZone);




// get SQLite open connection
$con =HelperFunctions::createSQLConnection();
$dbRegister = new dbSQLiteTrainingRegister($con);

// get training records staff are allowed to see
// (start of this month to six months onwards)
$trainingRegisterArray = $dbRegister->select_dateTimeOfTraining_Between($dateFromStartOfSelectedMonth->format('U'),$dateFromEndOfSelectedMonth->format('U'));

$con->CloseConnection();

$trainingRegisterArrayWithUsers = convertToRegisterWithUsersArray($trainingRegisterArray);

HelperFunctions::createHeadersToSendBackToClient();
echo json_encode($trainingRegisterArrayWithUsers);


// creates a new array of trainingRegisters and populates it with users (each array item holds a TrainingRegisterWithUsersObject)
function convertToRegisterWithUsersArray($trainingRegisterArray)
{
    $trainingRegisterArrayWithUsers = array();
    // go through reach trainingRegiser
    for ($i = 0; $i < count($trainingRegisterArray); $i++)
    {
        // create a TrainingRegisterWithUsers object
        // and copy all the values from the TrainingRegiser into it.
        $aTrainingRegisterWithUsers = new TrainingRegisterWithUsers();
        
        $aTrainingRegister = $trainingRegisterArray[$i];

        // copy the data
        $aTrainingRegisterWithUsers->id = $aTrainingRegister->id;
        $aTrainingRegisterWithUsers->dateTimeOfTraining = $aTrainingRegister->dateTimeOfTraining;
        $aTrainingRegisterWithUsers->dateTimeWhenRegisterIsActive = $aTrainingRegister->dateTimeWhenRegisterIsActive;
        $aTrainingRegisterWithUsers->dateTimeFromWhenRegisterCanBeUsed = $aTrainingRegister->dateTimeFromWhenRegisterCanBeUsed;
        $aTrainingRegisterWithUsers->maxNoCandidatesAllowed = $aTrainingRegister->maxNoCandidatesAllowed;
        $aTrainingRegisterWithUsers->id = $aTrainingRegister->id;

        // find all the users within this register
        $aTrainingRegisterWithUsers->findUsersInThisTrainingRegister();
        // add to the array
        array_push($trainingRegisterArrayWithUsers,$aTrainingRegisterWithUsers);
    }

    return $trainingRegisterArrayWithUsers;
}
 
?>