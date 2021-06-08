<?php
class SqLiteConnection
{
    private $_con;
    function __construct() 
    {
    }

    /**
     * Atempts to open a connection to an SQLite database
     * @param location the physical path to the SQLite database file
     * @return bool true if sucsefull, else false
     */
    public function OpenConnection($location) // return bool
    {
        try
        {
            $this->_con = new SQLite3($location);
            return true;
        }
        catch(Exception $e)
        {
            
            return false;
        }
    }

    /**
     * Attemps to close the SQLite database connection 
     * @return bool true if sucsefull, else false
     */
    public function CloseConnection() // return bool
    {
        try
        {
            $this->_con->close();
            $this->_con = null;

            return true;
        }
        catch(Exception $e)
        {
            return false;
        }
    }



    public function ExecuteSelectCommand($SQLCommand)
    {
        $returnValue = null;

        try
        {
            $command = $this->_con->prepare($SQLCommand);
                            
            // execute the query
            $returnValue = $command->execute();
                

            
        }
        catch(Exception $e)
        {
            $returnValue = null;
        }



        return $returnValue;        
    }
    
    public function ExecuteParameterizedSelectCommand($SQLCommand, $parameters) //$parameters = SQLiteParameter[]
    {
        $returnValue = null;

        try
        {
            $command = $this->_con->prepare($SQLCommand);
            
            // add the parameters to the command
            foreach ($parameters as $aParameter)
                $command->bindValue($aParameter->ParamName, $aParameter->value, $aParameter->type);
                
            // execute the query
            $returnValue = $command->execute();
                

            
        }
        catch(Exception $e)
        {
            $returnValue = null;
        }



        return $returnValue;

    }

    public function ExecuteParameterizedNoneReader($SQLCommand, $parameters)
    {
        $returnValue = null;


        try
        {
            $command = $this->_con->prepare($SQLCommand);
            
            // add the parameters to the command
            foreach ($parameters as $aParameter)
                $command->bindValue($aParameter->ParamName, $aParameter->value, $aParameter->type);
                
            // execute the query
            $returnValue = $command->execute();
                

            
        }
        catch(Exception $e)
        {

            $returnValue = null;
        }



        return $returnValue;
    }

    public function get_last_insert_id()
    {
        return $this->_con->lastInsertRowID();
    }
    public function get_affected_rows_count_on_last_query()
    {
        return $this->_con->changes();
    }
}

class SQLiteParameter
{
    public $ParamName; // the place holder name given inside the sql query
    public $value; // the value we want inserted into the query
    public $type; // data type of the $value (e.g. SQLITE3_INTEGER, SQLITE3_FLOAT, SQLITE3_TEXT, SQLITE3_NULL)
}
?>