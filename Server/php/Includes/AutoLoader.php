<?php 
//////////////////////////////////////////////////
// This file is where all classes are loaded in the project.
// Each file in the project that relys on another file will
// call this file (Autoloader.php) which will work out
// the path of the file that needs including.

// Please note. In order for this to work, any classes
// you create must have exactly the same name as its files name (case sensitive)
// Also note, if you create any new folders where classes exist, you will need
// to come back to this folder and add those folders to theautoLoadIncludes function below.
// You will also need to add each new class file you create to the swtich statement below.
require_once(dirname(__FILE__).'/../Classes/Settings.php');
require_once(dirname(__FILE__).'/../Classes/EnvReader.php');

spl_autoload_register('autoLoadIncludes');




function autoLoadIncludes($className)
{
    $docRoot = $_SERVER['DOCUMENT_ROOT'];
    // add the folder(s) to where the root of this app exists on the website.
    $appRootLocation = $docRoot . Settings::folderLocation();






    //////////////////////////
    // location of all folder

    // Classes folder and all folder under it
    $Classes_Folder = 'Classes';
    // Classess/Authentication
    $Classes_Authentication_Folder = $Classes_Folder . '/Authentication';



    // Data folder and all folders under it
    $Data_Folder = 'Data';

    // Data/Database
    $Data_Database_Folder = $Data_Folder . '/Database';
    // Data/Database/Admin
    $Data_Database_Admin_Folder = $Data_Database_Folder . '/Admin';
    // Data/Database/Training
    $Data_Database_Training_Folder = $Data_Database_Folder . '/Training';
    // Data/Database/User
    $Data_Database_User_Folder = $Data_Database_Folder . '/User';
    // Data/Database/ManikinLungs
    $Data_Database_ManikinLungs_Folder = $Data_Database_Folder . '/ManikinLungs';

    // Data/Models
    $Data_Models_Folder = $Data_Folder . '/Models';
    // Data/Models/Training
    $Data_Models_Training_Folder = $Data_Models_Folder . '/Training';
    // Data/Models/User
    $Data_Models_User_Folder = $Data_Models_Folder . '/User';
    // Data/Models/ManikinLungs
    $Data_Models_ManikinLungs_Folder = $Data_Models_Folder . '/ManikinLungs';
    
    // End of location of all folders
    ///////////////////////////////////////






    $filePath = $appRootLocation;

    // find out what the class name is we are looking for
    switch($className)
    {
        // Classes

        case 'Authentication':
        case 'LoginAuthentication':
            $filePath .= $Classes_Authentication_Folder;
            break;

        case 'HelperFunctions' :
            $filePath .= $Classes_Folder;
            break;

        case 'Settings':
        case 'EnvReader':
            $filePath .= $Classes_Folder;
            break;


        // Data
        case 'SqLiteConnection':
            $filePath .= $Data_Database_Folder;
            break;

        case 'dbSQLiteAdmin':
            $filePath .= $Data_Database_Admin_Folder;
            break;
            
        case 'dbSQLiteTrainingRegister':
            $filePath .= $Data_Database_Training_Folder;
            break;

        case 'dbSQLiteUser':
        case 'dbSQLiteUsersInTrainingRegister':
            $filePath .= $Data_Database_User_Folder;
            break;

        case 'dbLungsUserGiven':
        case 'dbManikinLungType':
            $filePath .= $Data_Database_ManikinLungs_Folder;
            break;

        case 'TrainingRegister':
        case 'TrainingRegisterWithUsers':
            $filePath .= $Data_Models_Training_Folder;
            break;

        case 'User':
        case 'UsersInTrainingRegister':
            $filePath .= $Data_Models_User_Folder;
            break;

        case 'LungsUserGiven':
        case 'ManikinLungType':
            $filePath .= $Data_Models_ManikinLungs_Folder;
            break;

        
    }

    $filePath .= '/' . $className . '.php';

    // if we could not find the file, just return and do nothing
    if(!file_exists($filePath))
    {
        echo '<br />';
        
        echo $filePath;
        echo '<br />';
        return;
    }

    if(!is_readable($filePath))
    {
        echo '<br />';
        
        echo 'error';
        echo '<br />';
        return;
    }

    // include the file in the project
    require_once $filePath;
}