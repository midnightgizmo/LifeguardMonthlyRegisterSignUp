<?php 
class dbSQLiteTrainingRegister
{

    private $_con;// SQLiteConnection.php

    /**
     * Name of the table in the database 
     */
    public $tableName = 'TrainingRegister';

    
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
     * select 1 TrainingRegister from the database
     * @param ID the id of te TrainingRegister to look for
     * @return TrainingRegister an instance of TrainingRegister with data retreeved from database, or null if could not be found
     */
    public function select($ID)
    {
        $query  = "SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ";
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

        // holds the data we will get from the database (TrainingRegister model class)
        $trainingRegister = null;

        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$parametersArray);

        // did we get any data from the database
        if($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.TrainingRegister class
            $trainingRegister = $this->GetRowData($row);
        }


        // an instance of Models.TrainingRegister() or null
        return $trainingRegister;
    }

    /** selects all rows from the TrainingRegister table in the database and returns them as an array of TrainingRegister
     * @return Models.Training.TrainingRegister list of all TrainingRegister in database
     */
    public function selectAll()
    {
        // an array of TrainingRegister classes
        $TrainingRegisterArray = array();

        $query  = "SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ";
        $query .= "FROM " . $this->tableName . " ";

        

        // execute the sql statment
        $results = $this->_con->ExecuteSelectCommand($query);

        // did we get any data from the database
        //if($row = $results->fetchArray()) 
        //{
            // get the data from the row and add it to a Models.TrainingRegister class
        //    $trainingRegister = $this->GetRowData($row);
        //}

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.TrainingRegister class
            $trainingRegister = $this->GetRowData($row);
            // add the model class to the array
            array_push($TrainingRegisterArray, $trainingRegister);
        }


        // a list of Models.Training.TraingRegister
        return $TrainingRegisterArray;

    }

    /**
     * selects all rows from the TrainingRegister table in the database where dateTimeOfTraining falls between 2 dates
     * @param from Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     * @param to Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     * @return Models.Training.TrainingRegister[] list of all TrainingRegister in database that match the query
     */
    public function select_dateTimeOfTraining_Between($from, $to)
    {
        // an array of TrainingRegister classes
        $TrainingRegisterArray = array();

        $query  = "SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE dateTimeOfTraining >= :from AND dateTimeOfTraining <= :to ";
        $query .= "ORDER BY DateTimeOfTraining;";

        // executes the database query and returns a a list of Models.Training.TraingRegister
        return $this->executeSelectBetweenQuery($query,$to,$from);

    }

    /**
     * selects all rows from the TrainingRegister table in the database where dateTimeWhenRegisterIsActive falls between 2 dates
     * @param from Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     * @param to Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     * @return Models.Training.TrainingRegister[] list of all TrainingRegister in database that match the query
     */
    public function select_dateTimeWhenRegisterIsActive_Between($from, $to)
    {
        // an array of TrainingRegister classes
        $TrainingRegisterArray = array();

        $query  = "SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ";
        $query .= "FROM " . $this->tableName . " ";
        //$query .= "WHERE dateTimeWhenRegisterIsActive >= :from AND dateTimeWhenRegisterIsActive <= :to;";
        $query .= "WHERE dateTimeOfTraining >= :from AND dateTimeWhenRegisterIsActive <= :to ";
        $query .= "ORDER BY dateTimeOfTraining";

        
        // executes the database query and returns a a list of Models.Training.TraingRegister
        return $this->executeSelectBetweenQuery($query,$to,$from);
    }

    /**
     * selects all rows from the TrainingRegister table in the database where dateTimeFromWhenRegisterCanBeUsed falls between 2 dates
     * @param from Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     * @param to Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     * @return Models.Training.TrainingRegister list of all TrainingRegister in database that match the query
     */
    public function select_dateTimeFromWhenRegisterCanBeUsed_Between($from, $to)
    {
        // an array of TrainingRegister classes
        $TrainingRegisterArray = array();

        $query  = "SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE dateTimeFromWhenRegisterCanBeUsed >= :from AND dateTimeFromWhenRegisterCanBeUsed <= :to;";

        
        // executes the database query and returns a a list of Models.Training.TraingRegister
        return $this->executeSelectBetweenQuery($query,$to,$from);
    }





    /* *****************************
    **                            **
    **      Insert Functions      **
    **                            **    
    ********************************/


    /**
     * Inserts a new row into the database
     * @param DateTimeOfTraining unix time stamp (number)
     * @param DateTimeWHenRegisterIsActive unix time stamp (number)
     * @param DateTimeFromWhenRegisterCanBeUsed unix time stamp (number)
     * @param MaxNoCandidatesAllowed number
     * @return Models.Training.TrainingRegister an instance of the newly created row or null if insert fails
     * 
     */
    public function insert($DateTimeOfTraining, $DateTimeWhenRegisterIsActive, $DateTimeFromWhenRegisterCanBeUsed, $MaxNoCandidatesAllowed)
    {
        $query = "INSERT INTO " . $this->tableName . " ";
        $query .= "(DateTimeOfTraining,DateTimeWhenRegisterIsActive,DateTimeFromWhenRegisterCanBeUsed,MaxNoCandidatesAllowed) ";
        $query .= "VALUES(";
        $query .= ":DateTimeOfTraining, :DateTimeWhenRegisterIsActive, :DateTimeFromWhenRegisterCanBeUsed, :MaxNoCandidatesAllowed";
        $query .= ");";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":DateTimeOfTraining";
        $aParameter->value = $DateTimeOfTraining;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":DateTimeWhenRegisterIsActive";
        $aParameter->value = $DateTimeWhenRegisterIsActive;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":DateTimeFromWhenRegisterCanBeUsed";
        $aParameter->value = $DateTimeFromWhenRegisterCanBeUsed;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":MaxNoCandidatesAllowed";
        $aParameter->value = $MaxNoCandidatesAllowed;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);


        $results = $this->_con->ExecuteParameterizedNoneReader($query,$ParametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            $trainingRegisterID = $this->_con->get_last_insert_id();
            return $this->select($trainingRegisterID);
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
     * updates a row with the passed in values
     * @param id the row to update
     * @param DateTimeOfTraining unix time stamp (number)
     * @param DateTimeWhenRegisterIsActive unix time stamp (number)
     * @param DateTimeFromWhenRegisterCanBeUsed unix time stamp (number)
     * @param MaxNoCandidatesAllowed number
     * @return Models.Training.TrainingRegister an instance of the newly created row or null if insert fails
     */
    public function update($id,$DateTimeOfTraining, $DateTimeWhenRegisterIsActive, $DateTimeFromWhenRegisterCanBeUsed, $MaxNoCandidatesAllowed)
    {
        $query = "UPDATE " . $this->tableName . " ";
        $query .= "SET ";
        $query .= "DateTimeOfTraining=:DateTimeOfTraining,";
        $query .= "DateTimeWhenRegisterIsActive=:DateTimeWhenRegisterIsActive,";
        $query .= "DateTimeFromWhenRegisterCanBeUsed=:DateTimeFromWhenRegisterCanBeUsed,";
        $query .= "MaxNoCandidatesAllowed=:MaxNoCandidatesAllowed ";
        $query .= "WHERE id=:id";

        $ParametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":DateTimeOfTraining";
        $aParameter->value = $DateTimeOfTraining;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":DateTimeWhenRegisterIsActive";
        $aParameter->value = $DateTimeWhenRegisterIsActive;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":DateTimeFromWhenRegisterCanBeUsed";
        $aParameter->value = $DateTimeFromWhenRegisterCanBeUsed;
        $aParameter->type = SQLITE3_INTEGER;
        array_push($ParametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":MaxNoCandidatesAllowed";
        $aParameter->value = $MaxNoCandidatesAllowed;
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
     * takes a row of data from the database and converts it to a Models.Training.TrainingRegister class
     * @return Models.Training.TrainingRegister the row from the database in a TrainingRegister class
     */
    private function GetRowData($row)
    {
        $trainingRegister = new TrainingRegister();
            
        $trainingRegister->id = (int)$row['id'];
        $trainingRegister->dateTimeOfTraining = (int)$row['dateTimeOfTraining'];
        $trainingRegister->dateTimeWhenRegisterIsActive = (int)$row['dateTimeWhenRegisterIsActive'];
        $trainingRegister->dateTimeFromWhenRegisterCanBeUsed = (int)$row['dateTimeFromWhenRegisterCanBeUsed'];
        $trainingRegister->maxNoCandidatesAllowed = (int)$row['maxNoCandidatesAllowed'];


        return $trainingRegister;
    }



    /**
     * All the select between funcions were simlar, so extracted the code from
     * them that was the same and put it all into this function
     * @param query the sql query to run
     * @param to unix time stamp (number)
     * @param from unix time stamp (number)
     * @return Models.Training.TrainingRegister list of all TrainingRegister in database that match the query
     */
    private function executeSelectBetweenQuery($query, $to, $from)
    {
        // an array of TrainingRegister classes
        $TrainingRegisterArray = array();
        
        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":from";
        $aParameter->value = $from;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);

        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":to";
        $aParameter->value = $to;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);


        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$parametersArray);


        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.TrainingRegister class
            $trainingRegister = $this->GetRowData($row);
            // add the model class to the array
            array_push($TrainingRegisterArray, $trainingRegister);
        }


        // a list of Models.Training.TraingRegister
        return $TrainingRegisterArray;
    }



}
?>