export class Register
{

    
    /** Unique ID of this register */
    private _id : number = -1;
    /**
     * get the id of this register
     */
    public get id() : number {
        return this._id;
    }
    /**
     * set the id of this register
     */
    public set id(v : number) {
        this._id = v;
    }

    
    /** maximum number of candidaets allowed on the register */
    private _maxNoCandidatesAllowed : number = -1;
    /**
     * gets the maximum number of candidates allowd in register
     */
    public get maxNoCandidatesAllowed() : number {
        return this._maxNoCandidatesAllowed;
    }
    /**
     * sets the maximum number of candidates allowed in register
     */
    public set maxNoCandidatesAllowed(v : number) {
        this._maxNoCandidatesAllowed = v;
    }
    
    



    /** Date/Time training sessions starts (unix time stamp) */
    private _dateTimeOfTraining_UnixTimeStamp : number = 0;
    /**
     * Get the date and time the training session starts 
     * (expressed in a unix time stamp)
     */
    public get dateTimeOfTraining_UnixTimeStamp() : number {
        return this._dateTimeOfTraining_UnixTimeStamp;
    }
    /**
     * Sets the date and time the training session starts
     * (expressed in a unix time stamp)
     */
    public set dateTimeOfTraining_UnixTimeStamp(v : number) {
        this._dateTimeOfTraining_UnixTimeStamp = v;

        // convert the unix time stamp to a javascript Date object
        this._registerStartDate = new Date(this.covertSecondsToMilliseconds(this._dateTimeOfTraining_UnixTimeStamp));
    }






    /** eariest point when the register becomes visable to the user (unix time stamp) */
    private _dateTimeOfWhenRegisterIsVisable : number = 0;
    /**
     * Gets the earilest date/time this register can be visable to the user
     * (expressed in unix time stamp)
     */
    public get dateTimeOfWhenRegisterIsVisable() : number {
        return this._dateTimeOfWhenRegisterIsVisable;
    }
    /**
     * Sets the earilest date/time this register can be visable to the user
     * (expressed in unix time stamp)
     */
    public set dateTimeOfWhenRegisterIsVisable(v : number) {
        this._dateTimeOfWhenRegisterIsVisable = v;

        this._registerVisibilityDate = new Date(this.covertSecondsToMilliseconds(this._dateTimeOfWhenRegisterIsVisable));
    }
    





    /** The earilyest point a user can book onto this register (unix time stamp) */
    private _dateTimeFromWhenCanBookOntoSession : number = 0;
    /** Gets the earilyest date/time a user can book onto this register
     * (expressed in a unix time stamp)
     */
    public get dateTimeFromWhenCanBookOntoSession() : number {
        return this._dateTimeFromWhenCanBookOntoSession;
    }
    /** Sets the earilest date/time a user can book onto this register
     * *(expressed in a unix time stamp)
     */
    public set dateTimeFromWhenCanBookOntoSession(v : number) {
        this._dateTimeFromWhenCanBookOntoSession = v;

        this._registerEditableDate = new Date(this.covertSecondsToMilliseconds(this._dateTimeFromWhenCanBookOntoSession));
    }
    
    












    private _registerStartDate : Date = new Date();
    /**
     * gets the date time the sessions starts
     * (expressed as a javascript date object)
     */
    public get startDate() : Date
    {
        return this._registerStartDate;
    }

    private _registerVisibilityDate : Date = new Date();
    /**
     * gets earilest date the register is visable to the user
     * (expressed as a javascript date object)
     */
    public get visibleDate() : Date
    {
        return this._registerVisibilityDate;
    }

    private _registerEditableDate : Date = new Date()
    /**
     * gets the earlyest date a user can join the register
     * (expressed in javascript date object)
     */
    public get editableDate() : Date
    {
        return this._registerEditableDate;
    }


    /**
     * This will need to be called when this class has been
     * populated from data from the server using the Object.assign method.
     * 
     */
    public inishalize()
    {
        this._registerStartDate = new Date(this.covertSecondsToMilliseconds(this._dateTimeOfTraining_UnixTimeStamp));
        this._registerVisibilityDate = new Date(this.covertSecondsToMilliseconds(this._dateTimeOfWhenRegisterIsVisable));
        this._registerEditableDate = new Date(this.covertSecondsToMilliseconds(this._dateTimeFromWhenCanBookOntoSession));
    }




    ////////////////////////
    // Public Methods


    /** 
     * formats a date to a string e.g. "Saturday 12th" 
     * */
    formatDateToString(dateToFormat : Date)
    {
      // day of the week, e.g. Monday
      let DayOfWeekAsString = "";
      // e.g. st, nd, rd, th
      let ordinal = "";
      let dayOfMonth = "";
      // am or pm time
      //let ampm = dateToFormat.getHours() >= 12 ? 'pm' : 'am';
      // hours part of time in 12 hour format
      //let hours = dateToFormat.getHours() >12 ? dateToFormat.getHours() - 12 : dateToFormat.getHours();
      switch(dateToFormat.getDay())
      {
          case 0: DayOfWeekAsString = "Sunday";break;
          case 1: DayOfWeekAsString = "Monday";break;
          case 2: DayOfWeekAsString = "Tuesday";break;
          case 3: DayOfWeekAsString = "Wednesday";break;
          case 4: DayOfWeekAsString = "Thursday";break;
          case 5: DayOfWeekAsString = "Friday";break;
          case 6: DayOfWeekAsString = "Saturday";break;
          
      }

      // if the date of month is below 10 we need to add a zero to the begining of it
      dayOfMonth = dateToFormat.getDate() < 10 ? "0" + dateToFormat.getDate().toString() : dateToFormat.getDate().toString();

      // work out if we need to put st,nd,rd, or th to the end of the date
      if (dateToFormat.getDate() > 3 && dateToFormat.getDate() < 21) 
          ordinal = 'th';
      switch (dateToFormat.getDate() % 10) 
      {
          case 1:  ordinal = "st";break;
          case 2:  ordinal = "nd";break;
          case 3:  ordinal = "rd";break;
          default: ordinal = "th";break;
      }
        
      // create the date string and return it
      return DayOfWeekAsString + " " + dayOfMonth + ordinal;
               
    }



    /**
    * Formats the time part of a date to a string e.g. 12:51am
    */
     formatTimeToString(timeToFormat : Date)
     {
         
       // am or pm time
       let ampm = timeToFormat.getHours() >= 12 ? 'pm' : 'am';
       // hours part of time in 12 hour format
       let hours = timeToFormat.getHours() >12 ? timeToFormat.getHours() - 12 : timeToFormat.getHours();
       let hoursAsString = "";

       let minutesAsString = timeToFormat.getMinutes() < 10 ? "0" + timeToFormat.getMinutes().toString() : timeToFormat.getMinutes().toString();
       // if the hours is below 10 we need to add a zero to the begining of it
       hoursAsString = hours < 10 ? "0" + hours.toString() : hours.toString();

       // create the time part of the date as a string and return it
       return hoursAsString + ":" + minutesAsString + ampm;
     }






    //////////////////////////
    // private methods
    private covertSecondsToMilliseconds(seconds  : number) : number
    {
        return seconds * 1000;
    }
    
}