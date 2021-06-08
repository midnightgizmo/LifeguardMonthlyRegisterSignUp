<?php
class LoginAuthentication
{

    public $_isLoggedIn = false;
    

    // if isLoggedIn is false, errorMessage will contain
    // information about why they are not logged in.
    public $_errorMessage= '';
  
    // the json web token used to authenticate the user
    public $_jwt = '';
    


}