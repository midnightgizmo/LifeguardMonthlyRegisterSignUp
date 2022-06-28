<?php
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');

// This page requires admin user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if admin user is loggedin
if(!$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();



// get all the lungs types in the database
$LungsTypeArray = getAllLungTypes();
// send back the lungs in json format
sendSucsessMessageBackToClient($LungsTypeArray);





function getAllLungTypes()
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbLungTypes = new dbManikinLungType($con);
    $ListOfLungsModel = $dbLungTypes->SelectAll();
    $con->CloseConnection();

    return $ListOfLungsModel;
}

/**
 * Sends back to the client an array in json format
 */
function sendSucsessMessageBackToClient($LungsListArray)
{

    HelperFunctions::createHeadersToSendBackToClient();
    
    echo json_encode($LungsListArray);
}

?>