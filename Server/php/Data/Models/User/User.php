<?php
class User
{
    /**
     * Id of the training register in the database
     */
    public $id = 0;

    /**
     * users first name
     */
    public $firstName = '';
    /**
     * users surname
     */
    public $surname = '';
    /**
     * users password that is stored as a SHA-2 hash
     */
    public $password = '';
    /**
     * Indicates if user is active. An incative user may not be able to log in
     */
    public $isUserActive = false;
}