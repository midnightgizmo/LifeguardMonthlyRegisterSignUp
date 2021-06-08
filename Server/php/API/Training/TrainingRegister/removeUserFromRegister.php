<?php

// gets all the training registers and the people booked into them for the specified month
include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');

// This page requires user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if user is loggedin
if(!$authentication->isLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();


// will hold the values sent from the client
$userID = -1;
$registerID = -1;


// find the user id & register id the user wants to be removed from
$userInput = getUserInput();
$userID = $userInput[0];
$registerID = $userInput[1];

// finds the users details in the database and returns back a user model
$userModel = findUser($userID);

// make sure the userID input parameter that was sent from the client is the same userID 
// of the logged in user (a user can not add another user to the register, they can only add them self)
isUserRemovingThemSelfFromRegister($authentication->getLoggedInUsersIdFromJwtCookie(),$userID);

// remove the user from the table in the database
removeUserFromRegister($userID,$registerID);

// send back the users data to indicate they have been added to the register
sendClientSucsessResponse($userModel);

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
/**
 * Make sure the userID input parameter that was sent from the client is the same userID 
 * of the logged in user (a user can not remove another user from the register, they can only remove them self)
 */
function isUserRemovingThemSelfFromRegister($loggedInUserId, $UserIdSentFromClientParameter)
{
    if($loggedInUserId != $UserIdSentFromClientParameter)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }
}


/**
 * Atempts to find the user in the database.
 * If not found, an empty user will be sent back to the client
 * with the user.id set to -1;
 */
function findUser($userID)
{
    $con =HelperFunctions::createSQLConnection();
    $dbUser = new dbSQLiteUser($con);

    // get the user from the database
    $aUser = $dbUser->select($userID);

    $con->CloseConnection();

    if(is_null($aUser) == true)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }

    return $aUser;
}

/**
 * Removes the User from the register
 */
function removeUserFromRegister($userID, $registerID)
{
    $con = HelperFunctions::createSQLConnection();

    $dbUserInRegister = new dbSQLiteUsersInTrainingRegister($con);

    // atempt to delete the user from the register
    $wasSucsefull = $dbUserInRegister->delete($userID, $registerID);

    // if we were unabel to delete the user from the database
    if($wasSucsefull == false)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }

}
/**
 * send back the users data to indicate they have been added to the register (id, firstName, surname)
 */
function sendClientSucsessResponse($userAddedToRegisterModel)
{
    HelperFunctions::createHeadersToSendBackToClient();
    // take the information we want out of the $dataToSendBackToUser and
    // add to the the new object we are about to create
    $dataToSendBackToUser = (object) [
        'id' => $userAddedToRegisterModel->id,
        'firstName' => $userAddedToRegisterModel->firstName,
        'surname' => $userAddedToRegisterModel->surname
      ];
    echo json_encode($dataToSendBackToUser);
}

/**
 * if we are unable to forfill the users request we will send back
 * an empty user with there id set to -1
 */
function sendBackEmptyUser()
{
    HelperFunctions::createHeadersToSendBackToClient();
    // if the user does exist in one of the registers, we can't add them
    // so send back an empty user to the client.
    // set id to -1 to let the client know we were able to carry out there request.
    $dataToSendBackToUser = (object) [
        'id' => -1,
        'firstName' => '',
        'surname' => ''
      ];
    echo json_encode($dataToSendBackToUser);
}