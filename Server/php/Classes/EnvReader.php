<?php
include_once(dirname(__FILE__).'/../Includes/AutoLoader.php');

// Looks for a env.php file in the root of this project where variable are set.
// There are 3 types on env files that can exist and the one with the highest presedence gets loaded.
//
// env.php <!-- will be loaded if development or production are not present
// env.development.php <-- will be loaded if production is not present
// env.production.php <-- highest presedence, will always be loaded
class EnvReader
{
    private static $envFileName = 'env.php';
    private static $envDevFileName = 'env.development.php';
    private static $envProdFileName = 'env.production.php';

    private static $wasFileLoaded = false;
    private static $envValuesArray = array();

    /**
     * get the value from the passed in key 
     */
    public static function getEnvVarValue($varName) 
    {
        // if the env file has not been loaded, atempt to load it.
        if(!EnvReader::$wasFileLoaded)
            EnvReader::loadEnvFile();

        // check the file was loaded, if not return null
        if(!EnvReader::$wasFileLoaded)
            return null;

        
        // return the value from key we were asked to get in the env variable
        return EnvReader::$envValuesArray[$varName];
        

    }

    /** Atempts to load the env file. If sucessfull EnvReader::$wasFileLoaded will be set to true */
    private static function loadEnvFile()
    {
        // get the env file we will want to load into memory
        $fileToLoad = EnvReader::getFileNameWeWantToLoad();
        
        // check to see if we were sucsefull in finding an env file
        if(strlen($fileToLoad) > 0)
        {// an env file was found

            // attempt to load the file
            $wasSucsesfull = include_once($fileToLoad);

            if($wasSucsesfull)
            {
                EnvReader::$wasFileLoaded = true;
                // get all the env key values (which are stored as an array)
                EnvReader::$envValuesArray = getEnvironmentVariable();
            }
            else
                EnvReader::$wasFileLoaded = false;
        }
        // env file was not found
        else
            EnvReader::$wasFileLoaded = false;

        

        // return if we were sucsefull or not in loading the file into memory
        return EnvReader::$wasFileLoaded;
    }

    /**
     * gets the correct env file to be loaded. returns empty string if could not find out
     */
    private static function getFileNameWeWantToLoad()
    {
        $folderLocationOfEnvFiles = dirname(__FILE__) . '/../';
        
        // see if the env.production.php file exists, if so return its location
        if(file_exists($folderLocationOfEnvFiles . EnvReader::$envProdFileName) == true)
            return $folderLocationOfEnvFiles . EnvReader::$envProdFileName;
        // if above file does not exist, see if the env.development.php file exists, if so return its location
        else if(file_exists($folderLocationOfEnvFiles . EnvReader::$envDevFileName) == true)
            return $folderLocationOfEnvFiles . EnvReader::$envDevFileName;
        // if above file does not exist, see if the env.php file exists, if so return its location
        else if (file_exists($folderLocationOfEnvFiles . EnvReader::$envFileName) == true)
            return $folderLocationOfEnvFiles . EnvReader::$envFileName;
        // no env file could be found so return empty string to indicate nothing was found
        else
            return '';
    }
}


