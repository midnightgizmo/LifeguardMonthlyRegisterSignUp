<?php
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');

// This page requires admin user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if admin user is loggedin
if(!$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();


// get an array of all the latest lungs given to the active users
$ListOfLungsGivenToUsers = GetLatestLungs();
// send the list back to the user in json format
sendSucsessMessageBackToClient($ListOfLungsGivenToUsers);


function GetLatestLungs()
{

        // get SQLite open connection
        $con =HelperFunctions::createSQLConnection();
        $dbLungsGiven = new dbLungsUserGiven($con);
        $ListOfLungsGivenToUsers = $dbLungsGiven->SelectLatestLungsGivenToActiveUsers();
        $con->CloseConnection();

        
        return $ListOfLungsGivenToUsers;
}


/**
 * Sends back to the client an array in json format
 */
function sendSucsessMessageBackToClient($LatestLungsGivenToUsers)
{

    HelperFunctions::createHeadersToSendBackToClient();
    
    echo json_encode($LatestLungsGivenToUsers);
}

?>