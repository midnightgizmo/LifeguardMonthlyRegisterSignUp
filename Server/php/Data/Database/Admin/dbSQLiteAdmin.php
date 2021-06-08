<?php 
class dbSQLiteAdmin
{
    private $_con;// SQLiteConnection.php

    /**
     * Name of the table in the database 
     */
    public $tableName = 'Admin';


    /**
     * inishalized the class ready for interacting with the database.
     * @param connection SQLiteConnection to the database which should have allready been opened before passing in
     */
    function __construct($connection) 
    {
        $this->_con = $connection;// this should be an allready open connection
    }
    
    /**
     * select the admin username and hashed password from the database
     * @return Array first element in array contains username, second element in array contains hashed password
     */
    public function selectUsernameAndPassword()
    {
        $query  = "SELECT username, password ";
        $query .= "FROM " . $this->tableName . " ";
        
        // will hold the username and password
        $adminLoginDetails = array();

        // holds the data we will get from the database (User model class)
        $user = null;

        // execute the sql statment
        $results = $this->_con->ExecuteSelectCommand($query);

        // did we get any data from the database
        if($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.User class
            $username = $row['username'];
            $password = $row['password'];

            array_push($adminLoginDetails,$username,$password);
        }


        // return the username and password as an array
        return $adminLoginDetails;
    }

}
