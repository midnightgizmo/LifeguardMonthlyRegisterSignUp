<?php
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');

// This page requires admin user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if admin user is loggedin
if(!$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();

$activeUsersList = getAllActiveUsersFromDatabase();

sendUserListToClient($activeUsersList);







/**
 * Gets all users from the database whos isUserActive is set to true
 */
function getAllActiveUsersFromDatabase()
{
    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbUser = new dbSQLiteUser($con);

    // get all active users from the database
    $listOfUsers = $dbUser->selectAllActiveUsers();
    $con->CloseConnection();

    return $listOfUsers;
}

/** sends back to the client a json list of users */
function sendUserListToClient($userArray)
{
    HelperFunctions::createHeadersToSendBackToClient();
    
    
    echo json_encode($userArray);
}