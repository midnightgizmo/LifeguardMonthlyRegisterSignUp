<?php
class TrainingRegisterWithUsers extends TrainingRegister
{
    /**
     * a list of Models.User.User
     */
    public $usersInTrainingSessionArray = array();

    /**
     * populates the $usersInTrainingSessionArray with any users
     * that are in this training register
     */
    public function findUsersInThisTrainingRegister()
    {
        // get SQLite open connection
        $con = HelperFunctions::createSQLConnection();

        //$dbUsersInTrainingRegister = new dbSQLiteUsersInTrainingRegister($con);
        // get a list of all the users that are in this training register
        //$this->usersInTrainingSessionArray = $dbUsersInTrainingRegister->select_Where_TrainingRegisterID($this->id);

        $dbUserInTrainingRegisterArray = new dbSQLiteUser($con);
        // get all the user (there id, first name and last name, other fields are left blank e.g. password);
        $foundUsersArray = $dbUserInTrainingRegisterArray->selectUsersInRegister($this->id);

        // add all the found users to the $usersInTrainingSessionArray
        foreach ($foundUsersArray as $eachUser)
            array_push($this->usersInTrainingSessionArray, $eachUser);

        $con->CloseConnection();
        
    }
}