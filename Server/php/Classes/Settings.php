<?php
class Settings
{
    public static function isInDeveloperMode()
    {
        return true;
    }

    /**
     * Location of this project in relation to the base url of the website
     */
    public static function folderLocation()
    {
        return EnvReader::getEnvVarValue('ProjectFolderLocationOnWebSite');
    }

    public static function getSQLiteConnectionString()
    {
        return EnvReader::getEnvVarValue('sqlConectionStringLocation');
    }

    

    /**
     * gets the string represntation of the timezone we want to use
     */
    public static function getTimeZone()
    {
        return "Europe/London";
    }

    /** gets the <scheme> + <hostname> + <port> that WE THINK (its a guess) the client is running on e.g. http://somthing.com:8080 */
    public static function getClientOrigin()
    {
        $urlProtocol = "http://";
        if(Settings::isClientUsingHTTPS())
            $urlProtocol = "https://";

        // check if we need to add www to the url
        if(Settings::isSiteUrlUsingWWW())
            $urlProtocol .= "www.";
        
        if(Settings::isInDeveloperMode())
            return $urlProtocol . Settings::get_domain_name() . ":8080";
        else
            return $urlProtocol . Settings::get_domain_name();
        
        // $_SERVER['HTTP_ORIGIN'] may contain this information
        //return "http://localhost:8080";
    }






    ///////////////////////////////////////////
    // Private functions




    private static function isClientUsingHTTPS() 
    {
        return
          (!empty($_SERVER['HTTPS']) && $_SERVER['HTTPS'] !== 'off')
          || $_SERVER['SERVER_PORT'] == 443;
    }

     // does the site url start with www or does it just have http://somthing.com
     private static function isSiteUrlUsingWWW()
    {
        
        
        
        
        // we are checking the first array element for www.
        // $params = explode('.',$_SERVER['HTTP_HOST']);
        // if(sizeof($params) > 0 && $params[0] == 'www')
        //     return true;
        // else
        //     return false;

        $params = explode('.',$_SERVER['HTTP_ORIGIN']);
        if(sizeof($params) > 0)
        {
            if(stripos($params[0], 'www', 0 ) == false)
                return false;
            else
                return true;
        }
        else 
            return false;
             
    }

    public static function get_domain_name()
    {
        return EnvReader::getEnvVarValue('domainName');
    }


}
?>