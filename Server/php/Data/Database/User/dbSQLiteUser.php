<?php 
class dbSQLiteUser
{

    private $_con;// SQLiteConnection.php

    /**
     * Name of the table in the database 
     */
    public $tableName = 'User';

    
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
     * @param ID the id of the User to look for
     * @return User an instance of User with data retreeved from database, or null if could not be found
     */
    public function select($ID)
    {
        $query  = "SELECT id, firstName, surname, password, isUserActive ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE id=:id";

        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":id";
        $aParameter->value = $ID;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);

        // holds the data we will get from the database (User model class)
        $user = null;

        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$parametersArray);

        // did we get any data from the database
        if($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $user = $this->GetRowData($row);
        }


        // an instance of Models.User() or null
        return $user;
    }


    /** selects all rows from the User table in the database and returns them as an array of User
     * @return Models.User.User list of all User in database
     */
    public function selectAll()
    {

        // an array of User classes
        $UserArray = array();

        $query  = "SELECT id, firstName, surname, password, isUserActive ";
        $query .= "FROM " . $this->tableName . " ";

        

        // execute the sql statment
        $results = $this->_con->ExecuteSelectCommand($query);


        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $user = $this->GetRowData($row);
            // add the model class to the array
            array_push($UserArray, $user);
        }


        // a list of Models.User
        return $UserArray;

    }

    /** selects all rows from the User table in the database and returns them as an array of User
     * @return Models.User.User list of all User in database
     */
    public function selectAllActiveUsers()
    {

        // an array of User classes
        $UserArray = array();

        $query  = "SELECT id, firstName, surname, password, isUserActive ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE isUserActive=1";

        

        // execute the sql statment
        $results = $this->_con->ExecuteSelectCommand($query);


        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $user = $this->GetRowData($row);
            // add the model class to the array
            array_push($UserArray, $user);
        }


        // a list of Models.User
        return $UserArray;

    }

    public function select_By_FirstNameAndPassword($firstName, $password)
    {
        // an array of User classes
        $UserArray = array();

        $query  = "SELECT id, firstName, surname, password, isUserActive ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE firstName = :firstName AND password = :password";


        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":firstName";
        $aParameter->value = $firstName;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":password";
        $aParameter->value = $password;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);



        // execute the sql statment
        $results = $this->_con->ExecuteSelectCommand($query);

        // did we get any data from the database
        if($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $UserArray = $this->GetRowData($row);
        }

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $user = $this->GetRowData($row);
            // add the model class to the array
            array_push($UserArray, $user);
        }


        // a list of Models.User
        return $UserArray;
    }

    /**
     * @return User | null
     */
    public function select_By_SurnameAndPassword($surname, $password)
    {
        // the user we will be looking for
        $usersInfo = null;

        $query  = "SELECT id, firstName, surname, password, isUserActive ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE surname = :surname COLLATE NOCASE AND password = :password";


        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":surname";
        $aParameter->value = $surname;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":password";
        $aParameter->value = $password;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);



        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$ParametersArray);

        // did we get any data from the database
        if($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $usersInfo = $this->GetRowData($row);
        }




        // found user or null
        return $usersInfo;
    }

    public function select_By_FirstNameAndSurname($firstName, $surname)
    {
        // an array of User classes
        $UserArray = array();

        $query  = "SELECT id, firstName, surname, password, isUserActive ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE firstName = :firstName AND surname = :surname";


        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":firstName";
        $aParameter->value = $firstName;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":surname";
        $aParameter->value = $surname;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);



        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$ParametersArray);
/*
        // did we get any data from the database
        if($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $UserArray = $this->GetRowData($row);
        }
*/        

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $user = $this->GetRowData($row);
            // add the model class to the array
            array_push($UserArray, $user);
        }


        // a list of Models.User
        return $UserArray;
    }

    public function selectUsersInRegister($regiserID)
    {
        $UserArray = array();

        $query = "SELECT ". $this->tableName . ".ID, " . $this->tableName . ".firstName, " . $this->tableName . ".surname ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "INNER JOIN UsersInTrainingRegister ON ";
        $query .= $this->tableName . ".id = UsersInTrainingRegister.userId ";
        $query .= "WHERE UsersInTrainingRegister.trainingRegisterId = :id ";
        $query .= "ORDER BY UsersInTrainingRegister.trainingRegisterId;";


        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":id";
        $aParameter->value = $regiserID;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);


        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$ParametersArray);

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $user = new User();
            
            $user->id = (int)$row['id'];
            $user->firstName = $row['firstName'];
            $user->surname = $row['surname'];

            // add the model class to the array
            array_push($UserArray, $user);
        }

        return $UserArray;

    }


/* *****************************
    **                            **
    **      Insert Functions      **
    **                            **    
    ********************************/


    /**
     * Inserts a new row into the database
     * @param firstName users first name (string)
     * @param surname users surname (string)
     * @param password hashed password (string)
     * @param isUserActive 0= user is incative; 1= user is active (number)
     * @return Models.User.User an instance of the newly created User or null if insert fails
     * 
     */
    public function insert($firstName, $surname, $password, $isUserActive)
    {
        $query = "INSERT INTO " . $this->tableName . " ";
        $query .= "(firstName,surname,password,isUserActive) ";
        $query .= "VALUES(";
        $query .= ":firstName, :surname, :password, :isUserActive";
        $query .= ");";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":firstName";
        $aParameter->value = $firstName;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":surname";
        $aParameter->value = $surname;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":password";
        $aParameter->value = $password;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":isUserActive";
        $aParameter->value = $isUserActive;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);


        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            $userID = $this->_con->get_last_insert_id();
            return $this->select($userID);
        }
        else
        {
            return null;
        }
    }


    
    /* *****************************
    **                            **
    **      Update Functions      **
    **                            **    
    ********************************/


    /**
     * Updates a row with the passed in values
     * @param id the row to update
     * @param firstName persons first name
     * @param surname persons surname
     * @param password users password in a SHA-2 hash
     * @param isUserActive bool - Indicates if user is active. An incative user may not be able to log in
     * @return Models.User.User a new model containing the updated users details
     */
    public function updateAll($id,$firstName, $surname, $password, $isUserActive)
    {
        $query = "UPDATE " . $this->tableName . " ";
        $query .= "SET ";
        $query .= "firstName=:firstName,";
        $query .= "surname=:surname,";
        $query .= "password=:password,";
        $query .= "isUserActive=:isUserActive ";
        $query .= "WHERE id=:id";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":firstName";
        $aParameter->value = $firstName;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":surname";
        $aParameter->value = $surname;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":password";
        $aParameter->value = $password;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":isUserActive";
        $aParameter->value = $isUserActive;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":id";
        $aParameter->value = $id;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);



        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);
        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            
            return $this->select($id);
        }
        else
        {
            return null;
        }
    }


    /**
     * Updates all columns except the password
     * @param id the row to update
     * @param firstName persons first name
     * @param surname persons surname
     * @param isUserActive bool - Indicates if user is active. An incative user may not be able to log in
     * @return Models.User.User a new model containing the updated users details
     */
    public function updateDetails($id,$firstName, $surname, $isUserActive)
    {
        $query = "UPDATE " . $this->tableName . " ";
        $query .= "SET ";
        $query .= "firstName=:firstName,";
        $query .= "surname=:surname,";
        $query .= "isUserActive=:isUserActive ";
        $query .= "WHERE id=:id";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":firstName";
        $aParameter->value = $firstName;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":surname";
        $aParameter->value = $surname;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":isUserActive";
        $aParameter->value = $isUserActive;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":id";
        $aParameter->value = $id;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);


        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);
        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            return $this->select($id);
        }
        else
        {
            return null;
        }
    }



    /**
     * Updates a users password to the new one passed in
     * @param id the row to update
     * @param password users password in a SHA-2 hash
     * @return Models.User.User a new model containing the updated users details
     */
    public function updatePassword($id,$password)
    {
        $query = "UPDATE " . $this->tableName . " ";
        $query .= "SET ";
        $query .= "password=:password ";
        $query .= "WHERE id=:id";

        $ParametersArray = array();


        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":password";
        $aParameter->value = $password;
        $aParameter->type = SQLITE3_TEXT;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":id";
        $aParameter->value = $id;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);



        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);
        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            return $this->select($id);
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
     * @param id the id of the row to be removed
     * @return bool true if sucsefull, else false
     */
    public function delete($id)
    {
        $query = "DELETE FROM " . $this->tableName . " ";
        $query .= "WHERE id = :id";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":id";
        $aParameter->value = $id;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
            return true;
        else
            return false;
    }
















    /**
     * takes a row of data from the database and converts it to a Models.User.TrainingRegister class
     * @return Models.Training.TrainingRegister the row from the database in a TrainingRegister class
     */
    private function GetRowData($row)
    {
        $user = new User();
            
        $user->id = (int)$row['id'];
        $user->firstName = $row['firstName'];
        $user->surname = $row['surname'];
        $user->password = $row['password'];
        $user->isUserActive = boolval($row['isUserActive']);


        return $user;
    }

}