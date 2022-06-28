export class LungUserGiven
{


    /**
     * Id of the user who has a manikin lung
     */
    private _userID : number = 0;
    public get userID() : number 
    {
        return this._userID;
    }
    public set userID(v : number) 
    {
        this._userID = v;
    }



    /**
    * Id of the Manikin lung assgined to this user
    */
    private _manikinLungTypeID : number = 0;
    public get manikinLungTypeID() : number 
    {
        return this._manikinLungTypeID;
    }
    public set manikinLungTypeID(v : number) 
    {
        this._manikinLungTypeID = v;
    }
    



    /**
    * The date the user was given the lung.
    * Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
    */
    private _dateGivenManikinLung : number = 0;
    public get dateGivenManikinLung() : number 
    {
        return this._dateGivenManikinLung;
    }
    public set dateGivenManikinLung(v : number) 
    {
        this._dateGivenManikinLung = v;
    }
    

   
   

}