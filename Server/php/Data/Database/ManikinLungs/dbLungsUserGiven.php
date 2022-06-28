<?php

class dbLungsUserGiven
{
        /**
     * Name of the table in the database 
     */
    public $tableName = 'LungsUserGiven';

     /**
     * inishalized the class ready for interacting with the database.
     * @param connection SQLiteConnection to the database which should have allready been opened before passing in
     */
    function __construct($connection) 
    {
        $this->_con = $connection;// this should be an allready open connection
    }

    
    /**
     * Get all the lungs given to a specific user, orderd by date (latest first)
     * @return array of LungsUserGiven
     */
    public function SelectLungsGivenToUser($userID, $MaxResults)
    {


        // an array of TrainingRegister classes
        $foundLungs = array();

        $query  = "SELECT userID, manikinLungTypeID, dateGivenManikinLung ";
        $query .= "FROM " . $this->tableName . " ";
        $query .= "WHERE userID=:userID ";
        $query .= "ORDER By dateGivenManikinLung DESC ";
        $query .= "LIMIT :MaxResults";



        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        // userID
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userID";
        $aParameter->value = $userID;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);


        // MaxResults
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":MaxResults";
        $aParameter->value = $MaxResults;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);

        // execute the sql statment
        $results = $this->_con->ExecuteParameterizedSelectCommand($query,$parametersArray);

        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.ManikinLungs.LungsUserGiven class
            $lungsUserGivenData = $this->GetRowData($row);
            // add the model class to the array
            array_push($foundLungs, $lungsUserGivenData);
        }

        return $foundLungs;
    }

    /**
     * for each user that has been given lungs. Get all the latest lungs they have been given for each lung type.
     * @return array of LungsUserGiven
     */
    public function SelectLatestLungsGivenToActiveUsers()
    {
        // an array of TrainingRegister classes
        $foundLungs = array();

        $query  = "SELECT LungsUserGiven.userID, LungsUserGiven.manikinLungTypeID, MAX(LungsUserGiven.dateGivenManikinLung) as dateGivenManikinLung ";
        $query .= "FROM User ";
        $query .= "INNER JOIN " . $this->tableName . " ";
        $query .= "ON User.id = LungsUserGiven.userId ";
        $query .= "WHERE User.isUserActive = 1 ";
        $query .= "GROUP BY  userId, manikinLungTypeID ";
        $query .= "ORDER BY User.firstName;";

        $results = $this->_con->ExecuteSelectCommand($query);

        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.ManikinLungs.LungsUserGiven class
            $lungsUserGivenData = $this->GetRowData($row);
            // add the model class to the array
            array_push($foundLungs, $lungsUserGivenData);
        }

        return $foundLungs;


    }

    public function Insert($UserID, $ManikinLungTypeID, $UnixTimeStamp)
    {
        $query = "INSERT INTO " . $this->tableName . " ";
        $query .= "(userID, manikinLungTypeID,dateGivenManikinLung) ";
        $query .= "VALUES(";
        $query .= ":UserID, :ManikinLungTypeID, :UnixTimeStamp";
        $query .= ");";


        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        // userID
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":UserID";
        $aParameter->value = $UserID;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);



        // manikinLungTypeID
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":ManikinLungTypeID";
        $aParameter->value = $ManikinLungTypeID;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);



        // dateGivenManikinLung
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":UnixTimeStamp";
        $aParameter->value = $UnixTimeStamp;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);

        $results = $this->_con->ExecuteParameterizedNoneReader($query,$parametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            $lungsUserGivenData = new LungsUserGiven();
            
            $lungsUserGivenData->userID = $UserID;
            $lungsUserGivenData->manikinLungTypeID = $ManikinLungTypeID;
            $lungsUserGivenData->dateGivenManikinLung = $UnixTimeStamp;

            return $lungsUserGivenData;
        }
        else
        {
            return null;
        }

    }

    /**
     * Delete the specified row
     */
    public function DeleteRow($UserID, $ManikinLungTypeID, $UnixTimeStamp)
    {
        $query = "DELETE FROM " . $this->tableName . " ";
        $query .= "WHERE ";
        $query .= "userID=:userID AND ";
        $query .= "manikinLungTypeID=:manikinLungTypeID AND ";
        $query .= "dateGivenManikinLung=:dateGivenManikinLung;";


        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        // userID
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userID";
        $aParameter->value = $UserID;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);



        // manikinLungTypeID
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":manikinLungTypeID";
        $aParameter->value = $ManikinLungTypeID;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);



        // dateGivenManikinLung
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":dateGivenManikinLung";
        $aParameter->value = $UnixTimeStamp;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);


        $results = $this->_con->ExecuteParameterizedNoneReader($query,$parametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            return true;
        }
        else
        {
            return false;
        }





    }

    /**
     * Delete all rows that contain the UserID
     */
    public function DeleteUser($UserID)
    {
        $query = "DELETE FROM " . $this->tableName . " ";
        $query .= "userID=:userID;";

        // holds a list of parameters to insert into the sql query
        $parametersArray = array();

        // userID
        $aParameter = new SQLiteParameter();
        $aParameter->ParamName = ":userID";
        $aParameter->value = $UserID;
        $aParameter->type = SQLITE3_INTEGER;

        // add the parameter to the parameter array
        array_push($parametersArray, $aParameter);


        $results = $this->_con->ExecuteParameterizedNoneReader($query,$parametersArray);

        if($results && $this->_con->get_affected_rows_count_on_last_query() > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /**
     * takes a row of data from the database and converts it to a Models.ManikinLungs.LungsUserGiven class
     * @return Models.ManikinLungs.LungsUserGiven the row from the database in a LungsUserGiven class
     */
    private function GetRowData($row)
    {
        $lungsUserGivenData = new LungsUserGiven();
            
        $lungsUserGivenData->userID = (int)$row['userId'];
        $lungsUserGivenData->manikinLungTypeID = (int)$row['manikinLungTypeID'];
        $lungsUserGivenData->dateGivenManikinLung = (int)$row['dateGivenManikinLung'];


        return $lungsUserGivenData;
    }


}

?>