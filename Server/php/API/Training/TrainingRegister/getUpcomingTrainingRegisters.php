<?php

include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');
//'2019-04-01 10:32:00';

// This page requires user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if user is loggedin
if(!$authentication->isLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();

$timeZone = new DateTimeZone( Settings::getTimeZone() );
// get the current date and time
$currentDateTime = new DateTime('now', $timeZone);
// using current date work out the date for the begining of the month
$dateFromStartOfCurrentMonth = new DateTime($currentDateTime->format('Y-m-01 00:00:00'), $timeZone);

// make a new date time that will hold the date and time six months from now
$dateInSixMonthsTime = new DateTime($dateFromStartOfCurrentMonth->format('Y-m-01 00:00:00'), $timeZone);
// add six months to the current 
$dateInSixMonthsTime->add(new DateInterval('P6M'));



// get SQLite open connection
$con =HelperFunctions::createSQLConnection();
$dbRegister = new dbSQLiteTrainingRegister($con);

// get training records staff are allowed to see
// (start of this month to six months onwards)
$trainingRegisterArray = $dbRegister->select_dateTimeWhenRegisterIsActive_Between($dateFromStartOfCurrentMonth->format('U'),$dateInSixMonthsTime->format('U'));

$con->CloseConnection();
// convert to a json string
$jsonString = json_encode($trainingRegisterArray);

// send the json string back to the client
header('Content-Type: application/json');
header('Access-Control-Allow-Credentials: true');
//header('Access-Control-Allow-Origin:http://localhost:8080');
header('Access-Control-Allow-Origin:' . Settings::getClientOrigin());

echo $jsonString;

/*
echo $currentDateTime->format('Y-m-d h:i:s');
echo '<br />';
echo $dateFromStartOfCurrentMonth->format('Y-m-d h:i:s');
echo '<br />';
echo $dateInSixMonthsTime->format('Y-m-d h:i:s');
echo '<br />';
*/