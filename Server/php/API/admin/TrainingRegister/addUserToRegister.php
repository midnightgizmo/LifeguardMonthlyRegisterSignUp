<?php
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');

// This page requires admin user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if admin user is loggedin
if(!$authentication->isAdminLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();



// will hold the values sent from the client
$userID = -1;
$registerID = -1;


// find the user id & register id the client sent us
$userInput = getUserInput();
$userID = $userInput[0];
$registerID = $userInput[1];

// check use does not allready exist in register
doesUserExistInRegister($userID,$registerID);

// add the user to the register
addUserToRegister($userID, $registerID);

sendBackResponse(true);


/**
 * get the data from the client (userID and registerID)
 */
function getUserInput()
{
    
    try 
    {
        $inputVeriables = array();

        // get what the client sent in json string and convert it to an stdClass object
        $jsonData = HelperFunctions::getClientJsonObject();

        $userID = intval($jsonData->userID);
        $registerID = intval($jsonData->registerID);

        // if they are zero we did not get the values from the clieint
        if($userID == 0 || $registerID == 0)
            throw new Exception();// through an exception to do the code in the catch block

        array_push($inputVeriables,$userID);
        array_push($inputVeriables, $registerID);

        return $inputVeriables;

    }
    catch (\Throwable $th) 
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }


}


function doesUserExistInRegister($userID,$registerID)
{
    $con =HelperFunctions::createSQLConnection();
    $dbUsersinRegister = new dbSQLiteUsersInTrainingRegister($con);

    $userInregisterModel = $dbUsersinRegister->select($userID,$registerID);
    // close the SQLite connection
    $con->CloseConnection();

    // if we found the user in the register - we can't add them because they allready exist in this register
    if(is_null($userInregisterModel) == false)
    {
        // let the client know the user was not added
        sendBackResponse(false);
        die;
    }
}

function addUserToRegister($userID, $registerID)
{
    // get SQLite open connection
    $con = HelperFunctions::createSQLConnection();


    $dbUserInRegister = new dbSQLiteUsersInTrainingRegister($con);
    $modelUsersInTrainingRegister = $dbUserInRegister->insert($userID,$registerID);
    $con->CloseConnection();

    if(is_null($modelUsersInTrainingRegister) == true)
    {
        // let the client know we were not able to add user to register
        sendBackResponse(false);
        // dont' let any more code run
        die;
    }
}



function sendBackResponse($wasSucsefull)
{
    HelperFunctions::createHeadersToSendBackToClient();
    echo json_encode($wasSucsefull);
}