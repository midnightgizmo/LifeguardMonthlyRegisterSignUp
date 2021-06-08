<?php
include_once(dirname(__FILE__).'/../../vendor/autoload.php');
include_once(dirname(__FILE__).'/../../Includes/AutoLoader.php');

use ReallySimpleJWT\Token;

use ReallySimpleJWT\Build;
use ReallySimpleJWT\Validate;
use ReallySimpleJWT\Encode;

use ReallySimpleJWT\Parse;
use ReallySimpleJWT\Jwt;


//require_once 'vendor/autoload.php';




class Authentication
{
    // The key used when encrypting the jwt (this is set in the constructor)
    private $jwtSecreteKey = '';
    
    // the time the cookie expires, in seconds
    private $cookieMaxAge;
    

    function __construct()
    {
        // set the default cookie time to expire 30 days from now
        $this->cookieMaxAge = time() + (1000       *  60      *      60       *   24    *    30);
        // get the secret key used when creating json web token
        $this->jwtSecreteKey = EnvReader::getEnvVarValue('jwtsecretKey');
    }

    public function getCookieMaxAge()
    {
        return $this->cookieMaxAge;
    }
    /**
     * Measured in seconds, the time when the cookie expires (unix time stamp)
     */
    public function setCookieMaxAge($expiration)
    {
        $this->cookieMaxAge = $expiration;
    }

    private $jwtCookieName = 'jwtCookie';
    public function getCookieName()
    {
        return $this->jwtCookieName;
    }

    public function getAdminCookieName()
    {
        return 'jwtAdminCookie';
    }


    /**
     * Checks the passed in credintional to see if they are correct
     * @param surname users surname
     * @param password users password in a SHA-2 hash
     * @return bool true if sucsefull, else false
     */
    public function authenticateUser($surname, $password)
    {
        // determins if the user has been authenticated, default as false
        $isAuthenticated = false;
        
        // create a database connection
        $con = new SqLiteConnection();
        // open the connection
        if($con->OpenConnection(Settings::getSQLiteConnectionString()) == true)
        {
            $dbUser = new dbSQLiteUser($con);

            $aUser = $dbUser->select_By_SurnameAndPassword($surname, $password);

            $con->CloseConnection();

            // if we found the user, they are authenticated
            if(is_null($aUser) == false)
                $isAuthenticated = true;
        }
        
        return $isAuthenticated;
        
    }

    /**
     * Converts the plain text password to a SHA-2 hash
     * @param password plain text password
     * @return string password converted to a SHA-2 hash
     */
    public function hashPassword($password)
    {
        return hash('sha256', $password);
    }

    /**
     * Creates and returns a jwt string with the data passed into this function.
     * @return string jwt string
     */
    public function createJWT($userID, $userFirstName, $userSurname)
    {
        //https://github.com/RobDWaller/ReallySimpleJWT

/*
        // Generate a token
        $userId = 12;
        
        $expiration = time() + 3600;
        $issuer = 'localhost';

        try
        {
            $token = Token::create($userId, $this->jwtSecreteKey, $expiration, $issuer);
            //var_dump($token);
        }
        catch(Exception $e)
        {
            var_dump($e);
        }

        // Validate the token
        //$result = Token::validate($token, 'secret');

        return $token;
        */

        $build = new Build('JWT', new Validate(), new Encode());
/*
        $token = $build->setContentType('JWT')
            ->setHeaderClaim('info', 'foo')
            ->setSecret('!secReT$123*')
            ->setIssuer('localhost')
            ->setSubject('admins')
            ->setAudience('https://google.com')
            ->setExpiration(time() + 30)
            ->setNotBefore(time() - 30)
            ->setIssuedAt(time())
            ->setJwtId('123ABC')
            ->setPayloadClaim('uid', 12)
            ->build();*/

            $token = $build->setContentType('JWT')
                            ->setSecret($this->jwtSecreteKey)
                            ->setExpiration($this->cookieMaxAge)
                            ->setNotBefore(time() - 30)
                            ->setIssuedAt(time())
                            ->setPayloadClaim('id', $userID)
                            ->setPayloadClaim('userFirstName', $userFirstName)
                            ->setPayloadClaim('userSurname',$userSurname)
                            ->build();

            return $token->getToken();
            

    }

    /**
     * Creates and returns a jwt string for admin login
     * @return string jwt string
     */
    public function createAdminJWT()
    {
        $build = new Build('JWT', new Validate(), new Encode());

        $token = $build->setContentType('JWT')
                            ->setSecret($this->jwtSecreteKey)
                            ->setExpiration($this->cookieMaxAge)
                            ->setNotBefore(time() - 30)
                            ->setIssuedAt(time())
                            ->setPayloadClaim('username', 'admin')
                            ->build();

        return $token->getToken();
    }

    /**
     * Checks a jwt string to see if its valid
     * @return bool true if valid, else false
     */
    public function isValidJWT($tokenAsString)
    {
        $isValid = false;

        $jwt = new Jwt($tokenAsString, $this->jwtSecreteKey);
        $parse = new Parse($jwt, new Validate(), new Encode());

        try
        {
            $parsed = $parse->validate()
                            ->validateExpiration()
                            ->parse();

            $isValid = true;
        }
        catch(Exception $e)
        {
            $isValid = false;
        }

        return $isValid;
        
    }


    /**
     * Checks to see if the jwt cookie is set and if it is,
     * checks to see if its a valid jwt
     * @return bool true if logged in, else false
     */
    public function isLoggedIn()
    {
        $isUserLoggedIn = false;

        if(!isset($_COOKIE[$this->jwtCookieName])) 
        {
            $isUserLoggedIn = false;
        } 
        else 
        {
            
            $tokenAsString = $_COOKIE[$this->jwtCookieName];
            

            $jwt = new Jwt($tokenAsString, $this->jwtSecreteKey);
            $parse = new Parse($jwt, new Validate(), new Encode());

            try
            {
                $parsed = $parse->validate()
                                ->validateExpiration()
                                ->parse();

                $isValid = true;
                $isUserLoggedIn = true;
            }
            catch(Exception $e)
            {
                $isValid = false;
            }
        }

        return $isUserLoggedIn;
    }

    /**
     * Checks to see if the jwt cookie is set and if it is,
     * checks to see if its a valid jwt
     * @return bool true if logged in, else false
     */
    public function isAdminLoggedIn()
    {
        $isUserLoggedIn = false;

        if(!isset($_COOKIE[$this->getAdminCookieName()])) 
        {
            $isUserLoggedIn = false;
        } 
        else 
        {
            
            $tokenAsString = $_COOKIE[$this->getAdminCookieName()];
            

            $jwt = new Jwt($tokenAsString, $this->jwtSecreteKey);
            $parse = new Parse($jwt, new Validate(), new Encode());

            try
            {
                $parsed = $parse->validate()
                                ->validateExpiration()
                                ->parse();

                $isValid = true;
                $isUserLoggedIn = true;
            }
            catch(Exception $e)
            {
                $isValid = false;
            }
        }

        return $isUserLoggedIn;
    }

    public function getLoggedInUsersIdFromJwtCookie()
    {
        $userID = -1;
        if(!isset($_COOKIE[$this->jwtCookieName])) 
        {
            $userID = -1;
        }
        else
        {
            $tokenAsString = $_COOKIE[$this->jwtCookieName];
            
            $jwt = new Jwt($tokenAsString, $this->jwtSecreteKey);
            $parse = new Parse($jwt, new Validate(), new Encode());

            try
            {
                $parsed = $parse->validate()
                                ->validateExpiration()
                                ->parse();
                $userID = $parsed->getPayload()['id'];
                
            }
            catch(Exception $e)
            {
                
            }

            
        }


        return $userID;
    }

    /**
     * Sends a 403 response to the client telling them they can't
     * do this action becuase they are not logged in.
     * No further code will be exicuted after calling this method
     */
    public function sendNotLoggedInResponseToClient()
    {
        header("HTTP/1.1 403 Unauthorized");
        exit;
    }
}

?>