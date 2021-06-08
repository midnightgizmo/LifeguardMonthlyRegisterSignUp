<?php

class HelperFunctions
{
    /**
     * Attempts to create an opend SQL Connection to the database.
     * @return SqLiteConnection
     */
    public static function createSQLConnection()
    {
        $settings = new Settings();
        
        $con = new SqLiteConnection();
        $con->OpenConnection($settings->getSQLiteConnectionString());

        return $con;
    }

    /**
     * Convert the passed in json string from the client to a php json object (stdClass)
     * @return stdClass
     */
    public static function getClientJsonObject()
    {
        try
        {
            $jsonString = file_get_contents('php://input');
            // convert it to a json object
            $jsonData = json_decode($jsonString,false);

            return $jsonData;
        }
        // if at any point an error occurs, just return a blank stdClass
        catch(Exception $e)
        {
            return new stdClass();
        }
    }

    /** Addeds the Content-type, Access-Control-Allow-Credentials 
     * & Access-Control-Allow-Origin headers 
     * */
    public static function createHeadersToSendBackToClient()
    {
        // foreach($_SERVER as $key => $value)
        //     error_log($key . ':' . $value);
        // send the json string back to the client
        header('Content-Type: application/json');
        header('Access-Control-Allow-Credentials: true');
        //header('Access-Control-Allow-Origin:http://localhost:8080');
        header('Access-Control-Allow-Origin:' . Settings::getClientOrigin());


    }
}