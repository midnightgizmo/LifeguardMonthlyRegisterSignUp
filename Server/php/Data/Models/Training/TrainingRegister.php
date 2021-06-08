<?php

class TrainingRegister
{
    /**
     * Id of the training register in the database
     */
    public $id = 0;
    /**
     * Idicates when this training should take place.
     * Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     */
    public $dateTimeOfTraining = 0;
    /**
     * Indicates the earilest date/time when this register appears to users.
     * Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     */
    public $dateTimeWhenRegisterIsActive = 0;
    /**
     * Indicatees the earlyist date/time users can start adding there name to this register
     * Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
     */
    public $dateTimeFromWhenRegisterCanBeUsed = 0;
    /**
     * The maxium number of people allowed on this register before it becomes full
     */
    public $maxNoCandidatesAllowed = 0;
}

?>