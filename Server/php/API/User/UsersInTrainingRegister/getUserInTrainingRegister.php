<?php

include_once(dirname(__FILE__).'../../../../Includes/AutoLoader.php');
//'2019-04-01 10:32:00';

// This page requires user to be logged in via a jwt cookie
$authentication = new Authentication();
// check to see if user is loggedin
if(!$authentication->isLoggedIn())
    // user not logged in so send back 401 response and stop any more of this page from executing
    $authentication->sendNotLoggedInResponseToClient();

