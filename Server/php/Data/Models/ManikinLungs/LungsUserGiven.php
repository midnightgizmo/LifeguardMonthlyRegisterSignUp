<?php

class LungsUserGiven
{

    /**
     * Id of the user who has this lung type
     */
    public $userID = 0;

    /**
     * Id of the Manikin lung assgined to this user
     */
    public $manikinLungTypeID = 0;

    /**
     * The date the user was given the lung.
     * Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     */
    public $dateGivenManikinLung = 0;
}


?>