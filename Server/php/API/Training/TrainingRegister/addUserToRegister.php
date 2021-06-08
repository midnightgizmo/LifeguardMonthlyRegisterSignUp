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


// find the user id & register id the user wants to be added too
$userInput = getUserInput();
$userID = $userInput[0];
$registerID = $userInput[1];

// make sure the userID input parameter that was sent from the client is the same userID 
// of the logged in user (a user can not add another user to the register, they can only add them self)
isUserAddingThemSelfToRegister($authentication->getLoggedInUsersIdFromJwtCookie(),$userID);


// finds the users details in the database and returns back a user model
$userModel = findUser($userID);


// find the register details 
$aRegister = getRegisterDetails($registerID);

$yearAndMonth = getYearAndMonthFromUnixTimeStamp($aRegister->dateTimeOfTraining);

// find all register in the month of the register the user wants to be added too.
$allRegistersInMonthArray = findAllRegistersInMonth($yearAndMonth[0],$yearAndMonth[1]);

// get all the users that are currently in the registers for the chosen month
$registersWithUsersArray = getAllUsersWithinRegisters($allRegistersInMonthArray);

// does user exist in any of the registers
doesUserExistInRegisters($registersWithUsersArray, $userID);

// is the user allowed to add them self to this register (make sure dates are corrrect for editing register)
areDatesOkForUserToBeAddedToRegister($registersWithUsersArray, $aRegister);

// make sure there is space on the register for the user to be added to it
isThereEnoughSpaceOnRegister($aRegister);

// add the user to the register
addUserToRegister($aRegister->id, $userID);

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
 * of the logged in user (a user can not add another user to the register, they can only add them self)
 */
function isUserAddingThemSelfToRegister($loggedInUserId, $UserIdSentFromClientParameter)
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


function getRegisterDetails($registerID)
{
    $con =HelperFunctions::createSQLConnection();
    $dbRegister = new dbSQLiteTrainingRegister($con);

    $registerModel = $dbRegister->select($registerID);
    // close the SQLite connection
    $con->CloseConnection();

    // if we did not find the register
    if(is_null($registerModel) == true)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }

    return $registerModel;
}

/**
 * returns an int array of year and month
 * [0] = Year
 * [1] = month
 */
function getYearAndMonthFromUnixTimeStamp($unixTimeStamp)
{
    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    $aDate = DateTime::createFromFormat('U', $unixTimeStamp, $timeZone );
    
    $year = intval($aDate->format('Y'));
    $month = intval($aDate->format('n'));

    // if we were not able to get the year or the month
    if($year == 0 || $month == 0)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }

    // return the year and the month in an array
    return array($year,$month);
}

/**
 * returns an array of register that exist in the given year,month
 */
function findAllRegistersInMonth($year,$month)
{
    // create date as a string using year and month (day of month will be set to 1)
    // year-month-day
    $dateAsString = $year . '-' . $month . '-1';
    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    // create a DateTime for the begining of the selected month
    $startDate = DateTime::createFromFormat('Y-n-j',$dateAsString,$timeZone);
    // create a date time for the end of the month (t in format gives us the last day in given month)
    $endDate = new DateTime($startDate->format('Y-m-t 00:00:00'), $timeZone);

    // get SQLite open connection
    $con =HelperFunctions::createSQLConnection();
    $dbRegister = new dbSQLiteTrainingRegister($con);
    // get all training register within the given month
    $trainingRegisterArray = $dbRegister->select_dateTimeOfTraining_Between($startDate->format('U'),$endDate->format('U'));
    // close the SQLite connection
    $con->CloseConnection();

    // if no register were found, then the user can't be added to somthing that does not exist
    if(count($trainingRegisterArray) < 1)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }

    // return the list of found registers
    return $trainingRegisterArray;
}

function getAllUsersWithinRegisters($allRegistersInMonthArray)
{
    $trainingRegisterArrayWithUsers = array();
    // go through reach trainingRegiser
    for ($i = 0; $i < count($allRegistersInMonthArray); $i++)
    {
        // create a TrainingRegisterWithUsers object
        // and copy all the values from the TrainingRegiser into it.
        $aTrainingRegisterWithUsers = new TrainingRegisterWithUsers();
        
        $aTrainingRegister = $allRegistersInMonthArray[$i];

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

    // return an array of training registers populated with users (if any were found)
    return $trainingRegisterArrayWithUsers;
}

/**
 * returns false if no user found. 
 * If user does exists, sends empty json user object to client
 * and terminates the code by calling die
 * @return bool
 */
function doesUserExistInRegisters($registersWithUsersArray, $userID)
{

    // go through each register
    foreach ($registersWithUsersArray as $aRegister) 
    {
        // go through each user that is in the register
        foreach($aRegister->usersInTrainingSessionArray as $eachUser)
        {
            // see if the user wanting to add them self to one of the registers
            // in the given month exists in this register.
            // If they do exist, it means the user can not be added to any register
            // in the month because a user can only appear on one register in any 
            // given month
            if($eachUser->id == $userID)
            {
                // send back empty user model to let them know we could 
                // not forfill there request
                sendBackEmptyUser();
                // dont' let any more code run
                die;
            }
        }
    }

    // return false to idicate user was not found
    return false;
}

/**
 * Does date checks to make sure its ok to add user to the given register.
 * @param TrainingRegisterWithUsers[] $registersWithUsersArray
 * @param TrainingRegister $registerToAddToo
 * @return bool
 */
function areDatesOkForUserToBeAddedToRegister($registersWithUsersArray, $registerToAddToo)
{
    // check to see if this register can be edited by the user (add or remove them self from the register)

    $timeZone = new DateTimeZone( Settings::getTimeZone() );
    
    // get the current date and time
    $currentDateTime = new DateTime('NOW',$timeZone);
    // get todays date in unix time stamp
    $DateTimeNowAsUnixTimeStamp = intval($currentDateTime->format('U'));

    
    ///////////////////////////////////////////////////////////////////////////////////////
    // make a DateTime object that is set to 5 days before the first register in the array

    // get the start date and time of the first register in the array
    $dateWhenAllRegistersInMonthAreNoLongerEditable = DateTime::createFromFormat('U', $registersWithUsersArray[0]->dateTimeOfTraining, $timeZone );
    // create an internal of 5 days
    $interval = new DateInterval('P5D');
    // make it so the interval acts as minus 5 days
    $interval->invert = 1;
    // minus 5 days onto the date
    $dateWhenAllRegistersInMonthAreNoLongerEditable->add($interval);
    //
    /////////////////////////////////////////////////////////////////////////////////////////

    // don't allow user to add or remove them self from this months register if todays date
    // is greater than the first register in the list, minus 5 days
    if($DateTimeNowAsUnixTimeStamp >= intval($dateWhenAllRegistersInMonthAreNoLongerEditable->format('U')) )
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }

    // if we have set the editable date to the future (past todays date) the user can't edit it
    if($registerToAddToo->dateTimeFromWhenRegisterCanBeUsed > $DateTimeNowAsUnixTimeStamp)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }
    
    // if the start date for the register has pased todays date, they can't edit it
    if($registerToAddToo->dateTimeOfTraining < $DateTimeNowAsUnixTimeStamp)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }

    // dates ok for user to be added to register.
    return true;
}
/**
 * @param TrainingRegister $aRegister
 */
function isThereEnoughSpaceOnRegister($aRegister)
{
    // create a TrainingRegisterWithUsers object
    // and copy all the values from the TrainingRegiser into it.
    $aTrainingRegisterWithUsers = new TrainingRegisterWithUsers();
    
    // copy the data
    $aTrainingRegisterWithUsers->id = $aRegister->id;
    $aTrainingRegisterWithUsers->dateTimeOfTraining = $aRegister->dateTimeOfTraining;
    $aTrainingRegisterWithUsers->dateTimeWhenRegisterIsActive = $aRegister->dateTimeWhenRegisterIsActive;
    $aTrainingRegisterWithUsers->dateTimeFromWhenRegisterCanBeUsed = $aRegister->dateTimeFromWhenRegisterCanBeUsed;
    $aTrainingRegisterWithUsers->maxNoCandidatesAllowed = $aRegister->maxNoCandidatesAllowed;
    $aTrainingRegisterWithUsers->id = $aRegister->id;

    // find all the users within this register
    $aTrainingRegisterWithUsers->findUsersInThisTrainingRegister();

    // if the register is full
    if( count($aTrainingRegisterWithUsers->usersInTrainingSessionArray) >= $aTrainingRegisterWithUsers->maxNoCandidatesAllowed)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }
}

/**
 * Atempts to add user to register. If fails,
 * returns empty User object back to client with user.id set to -1
 */
function addUserToRegister($registerID, $userID)
{
    // get SQLite open connection
    $con = HelperFunctions::createSQLConnection();
    //$dbRegister = new dbSQLiteTrainingRegister($con);

    $dbUserInRegister = new dbSQLiteUsersInTrainingRegister($con);
    $modelUsersInTrainingRegister = $dbUserInRegister->insert($userID,$registerID);
    $con->CloseConnection();

    if(is_null($modelUsersInTrainingRegister) == true)
    {
        // send back empty user model to let them know we could 
        // not forfill there request
        sendBackEmptyUser();
        // dont' let any more code run
        die;
    }
}

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