<?php 
class dbManikinLungType
{

    /**
     * Name of the table in the database 
     */
    public $tableName = 'ManikinLungType';

     /**
     * inishalized the class ready for interacting with the database.
     * @param connection SQLiteConnection to the database which should have allready been opened before passing in
     */
    function __construct($connection) 
    {
        $this->_con = $connection;// this should be an allready open connection
    }



    /**
     * Selects all Manikin lung types from the database.
     * @return array of Models.ManikinLUngs.ManikinLUngType class
     */
    public function SelectAll()
    {
        // an array of TrainingRegister classes
        $LungTypesArray = array();

        $query  = "SELECT id, name ";
        $query .= "FROM " . $this->tableName . " ";


        // execute the sql statment
        $results = $this->_con->ExecuteSelectCommand($query);


        // get each row
        while ($row = $results->fetchArray()) 
        {
            // get the data from the row and add it to a Models.TrainingRegister class
            $manikinLungData = $this->GetRowData($row);
            // add the model class to the array
            array_push($LungTypesArray, $manikinLungData);
        }


        // a list of Models.Training.TraingRegister
        return $LungTypesArray;
    }


    /**
     * takes a row of data from the database and converts it to a Models.ManikinLungs.ManikinLungType class
     * @return Models.ManikinLungs.ManikinLungType the row from the database in a ManikinLungType class
     */
    private function GetRowData($row)
    {
        $manikinLungType = new ManikinLungType();
            
        $manikinLungType->id = (int)$row['id'];
        $manikinLungType->name = $row['name'];
        


        return $manikinLungType;
    }
}
?>