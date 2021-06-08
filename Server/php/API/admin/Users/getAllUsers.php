<?php
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');

// This page requires admin user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if admin user is loggedin
if(!$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();

$allUsersList = getAllUsersFromDatabase();

sendUserListToClient($allUsersList);







/**
 * Gets all users from the database 
 */
function getAllUsersFromDatabase()
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbUser = new dbSQLiteUser($con);

    // get all active users from the database
    $listOfUsers = $dbUser->selectAll();
    $con->CloseConnection();

    return $listOfUsers;
}

/** sends back to the client a json list of users */
function sendUserListToClient($userArray)
{
    // don't send the password information back.
    foreach ($userArray as $eachUser)
        $eachUser->password = '';

    HelperFunctions::createHeadersToSendBackToClient();
    
    
    echo json_encode($userArray);
}