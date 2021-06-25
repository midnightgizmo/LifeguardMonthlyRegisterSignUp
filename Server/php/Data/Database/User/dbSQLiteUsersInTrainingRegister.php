<?php
class dbSQLiteUsersInTrainingRegister
{
    private $_con;// SQLiteConnection.php

    /**
     * Name of the table in the database 
     */
    public $tableName = 'UsersInTrainingRegister';

    
    /**
     * inishalized the class ready for interacting with the database.
     * @param connection SQLiteConnection to the database which should have allready been opened before passing in
     */
    function __construct($connection) 
    {
        $this->_con = $connection;// this should be an allready open connection
    }




    /* *****************************
    **                            **
    **      Select Functions      **
    **                            **    
    ********************************/

    /**
     * select 1 User from the database
     * @param userId the id of the User to look for
     * @param trainingRegisterId the id of the trainingRegister to look for
     * @return UsersInTrainingRegister an instance of UsersInTrainingRegister with data retreeved from database, or null if could not be found
     */
    public function select($userId, $trainingRegisterId)
    {
        $query  = "SELECT userId, trainingRegisterId ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE userId=:userId AND trainingRegisterId = :trainingRegisterId;";

        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userId";
        $aParameter->value = $userId;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":trainingRegisterId";
        $aParameter->value = $trainingRegisterId;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);

        // holds the data we will get from the database (UsersInTrainingRegister model class)
        $usersInTrainingRegister = null;

        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$parametersArray);

        // did we get any data from the database
        if($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $usersInTrainingRegister = $this->GetRowData($row);
        }


        // an instance of Models.usersInTrainingRegister() or null
        return $usersInTrainingRegister;
    }

    /**
     * Gets all rows from the table
     * @return array an array of UsersInTrainingRegister
     */
    public function selectAll()
    {
        // an array of UsersInTrainingRegister classes
        $UsersInTrainingRegisterArray = array();

        $query  = "SELECT userId, trainingRegisterId";
        $query .= "FROM " . $this->tableName . " ";


        // execute the sql statment
        $results = $this->_con->ExecuteSelectCommand($query);

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.UsersInTrainingRegister class
            $usersInTrainingRegister = $this->GetRowData($row);
            // add the model class to the array
            array_push($UsersInTrainingRegisterArray, $usersInTrainingRegister);
        }

        
        return $UsersInTrainingRegisterArray;
    }


    /**
     * Gets all rows where it finds $userID
     * @param userId the id we use to search for
     * @return array list of rows that match the userID
     */
    public function select_Where_UserID($userId)
    {
        // an array of UsersInTrainingRegister classes
        $UsersInTrainingRegisterArray = array();

        $query  = "SELECT userId, trainingRegisterId ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE userId = :userId";


        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userId";
        $aParameter->value = $userId;
        $aParameter->type = SQLITE3_INTEGER;
        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);



        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$parametersArray);

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.UsersInTrainingRegister class
            $usersInTrainingRegister = $this->GetRowData($row);
            // add the model class to the array
            array_push($UsersInTrainingRegisterArray, $usersInTrainingRegister);
        }

        
        return $UsersInTrainingRegisterArray;
    }

    /**
     * Gets all rows where it finds $trainingRegisterId
     * @param trainingRegisterId the id we use to search for
     * @return array list of rows that match the trainingRegisterId
     */
    public function select_Where_TrainingRegisterID($trainingRegisterId)
    {
        // an array of UsersInTrainingRegister classes
        $UsersInTrainingRegisterArray = array();

        $query  = "SELECT userId, trainingRegisterId ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE trainingRegisterId = :trainingRegisterId";


        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":trainingRegisterId";
        $aParameter->value = $trainingRegisterId;
        $aParameter->type = SQLITE3_INTEGER;
        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);



        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$parametersArray);

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.UsersInTrainingRegister class
            $usersInTrainingRegister = $this->GetRowData($row);
            // add the model class to the array
            array_push($UsersInTrainingRegisterArray, $usersInTrainingRegister);
        }

        
        return $UsersInTrainingRegisterArray;
    }









    /* *****************************
    **                            **
    **      Insert Functions      **
    **                            **    
    ********************************/


    /**
     * Inserts a new row into the database
     * @param userId Users ID (number)
     * @param trainingRegisterId Training Registers ID (number)
     * @return Models.Training.TrainingRegister an instance of the newly created row or null if insert fails
     * 
     */
    public function insert($userId, $trainingRegisterId)
    {
        $query = "INSERT INTO " . $this->tableName . " ";
        $query .= "(userId,trainingRegisterId) ";
        $query .= "VALUES(";
        $query .= ":userId, :trainingRegisterId";
        $query .= ");";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userId";
        $aParameter->value = $userId;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":trainingRegisterId";
        $aParameter->value = $trainingRegisterId;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);


        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            return $this->select($userId, $trainingRegisterId);
        }
        else
        {
            return null;
        }
    }



 

    /* *****************************
    **                            **
    **      Delete Functions      **
    **                            **    
    ********************************/

    /**
     * Removes the selected row from the database
     * @param userId the User id of the row to be removed
     * @param trainingRegisterId the Training Register id of the row to be removed
     * @return bool true if sucsefull, else false
     */
    public function delete($userId, $trainingRegisterId)
    {
        $query = "DELETE FROM " . $this->tableName . " ";
        $query .= "WHERE userId = :userId AND trainingRegisterId = :trainingRegisterId";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userId";
        $aParameter->value = $userId;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":trainingRegisterId";
        $aParameter->value = $trainingRegisterId;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
            return true;
        else
            return false;
    }


    /**
     * removes the user from any register they are currently in
     * @param userId the id of the user to look for
     * @return bool true if sucsefull, else false
     */
    public function deleteUserFromAllRegisters($userId)
    {
        $query = "DELETE FROM " . $this->tableName . " ";
        $query .= "WHERE userId = :userId;";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userId";
        $aParameter->value = $userId;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
            return true;
        else
            return false;
    }

    /**
     * removes all users from a single register
     * @param registerID the register id we want to remove all users from
     * @return bool true if any rows were removed, false if no rows were removed
     */
    public function deleteAllUsersFromRegister($registerID)
    {
        $query = "DELETE FROM " . $this->tableName . " ";
        $query .= "WHERE trainingRegisterId = :trainingRegisterId;";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":trainingRegisterId";
        $aParameter->value = $registerID;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
            return true;
        else
            return false;
    }










    
    /**
     * takes a row of data from the database and converts it to a Models.User.UsersInTrainingRegister class
     * @return Models.User.UsersInTrainingRegister the row from the database in a UsersInTrainingRegister class
     */
    private function GetRowData($row)
    {
        $usersInTrainingRegister = new UsersInTrainingRegister();
            
        $usersInTrainingRegister->userId = (int)$row['userId'];
        $usersInTrainingRegister->trainingRegisterId = (int)$row['trainingRegisterId'];

        return $usersInTrainingRegister;
    }


}
?>