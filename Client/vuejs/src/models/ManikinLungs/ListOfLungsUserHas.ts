export class ListOfLungsUserHas
{
    public ManikinLungsUserHas : ManikinLungdata[] = [];


    
    private _userID : number = 0;
    public get userID() : number {
        return this._userID;
    }
    public set userID(v : number) {
        this._userID = v;
    }

    
    private _userFullName : string = '';
    public get userFullName() : string {
        return this._userFullName;
    }
    public set userFullName(v : string) {
        this._userFullName = v;
    }

    
    private _DoesOneOrMoreLungsNeedReplacingForThisUser: boolean = false;
    public get DoesOneOrMoreLungsNeedReplacingForThisUser() : boolean {
        return this._DoesOneOrMoreLungsNeedReplacingForThisUser;
    }
    public set DoesOneOrMoreLungsNeedReplacingForThisUser(v : boolean) {
        this._DoesOneOrMoreLungsNeedReplacingForThisUser = v;
    }
    
    
    
    private _isVisable : boolean = false;
    /**
     * used in the view to determin if the list of manikin lungs
     * assiged to this user should be shown.
     */
    public get isVisable() : boolean {
        return this._isVisable;
    }
    public set isVisable(v : boolean) {
        this._isVisable = v;
    }
    
    
    
}


export class ManikinLungdata
{
    private static _monthNames = ["January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
];

    
   
    private _uniqueIDForGUI : number = 0;
    /**
     * A unique id which can be used by vuejs for a :key
     */
    public get uniqueIDForGUI() : number
    {

        return this._uniqueIDForGUI;
    }
    /**
     * A unique id to give this lung which can be used by vuejs for a :key
     */
    public set uniqueIDForGUI(val: number)
    {
        this._uniqueIDForGUI = val;
    }

    private _id : number = 0;
    public get id() : number {
        return this._id;
    }
    public set id(v : number) {
        this._id = v;
    }

    
    
    private _name : string = '';
    public get name() : string {
        return this._name;
    }
    public set name(v : string) {
        this._name = v;
    }
    
    
    private _dateGivenManikinLung : number = 0;
    public get dateGivenManikinLung() : number {
        return this._dateGivenManikinLung;
    }
    public set dateGivenManikinLung(v : number) 
    {
        this._dateGivenManikinLung = v;

        // convert the unix time stamp to a date object
        var date = new Date(v * 1000);
        // convert the date object to a string friendly name;
        this.dateGivenManikinLung_FormattedString = date.getDate().toString() + " " + ManikinLungdata._monthNames[date.getMonth()] + " "  + date.getFullYear().toString();

        let millisecondsPassed = new Date().getTime() - date.getTime();
        this.numberOfDaysSinceIssuedLung = Math.ceil(millisecondsPassed / (1000 * 3600 * 24));

    }

    
    private _dateGivenManikinLung_FormattedString : string = '';
    public get dateGivenManikinLung_FormattedString() : string {
        return this._dateGivenManikinLung_FormattedString;
    }
    public set dateGivenManikinLung_FormattedString(v : string) {
        this._dateGivenManikinLung_FormattedString = v;
    }
    


    
    private _numberOfDaysSinceIssuedLung : number = 0;
    public get numberOfDaysSinceIssuedLung() : number {
        return this._numberOfDaysSinceIssuedLung;
    }
    public set numberOfDaysSinceIssuedLung(v : number) {
        this._numberOfDaysSinceIssuedLung = v;
    }
    
/*
    public get hasManikinLungExpired() : boolean
    {
        // if the lungs have been held for lunger than 6 months
        if(this.numberOfDaysSinceIssuedLung > 180)
            return true;
        else
            return false;
    }
*/
     
    /**
     * If Manikin Lung has been held for >= process.env.VUE_APP_MaxDaysToHaveManikinLung,
     * return true, else false
     */
     public get DoesManikinLungNeedReplacing() : boolean 
     {
         // get the max number of days we should keep lungs
         let MaxDays : number = parseInt(process.env.VUE_APP_MaxDaysToHaveManikinLung);
         // how long have we had this lung for
         let daysHadLungsFor = Date.now() - this.dateGivenManikinLung;
         
         // if we have had the lungs for MaxDays or more return true, else return false
         if( this.numberOfDaysSinceIssuedLung >= MaxDays )
             return true;
         else
             return false;
         
     }


     
     private _isUiDisabled : boolean = false;
     /**
      * Used in the UI to determin if we want the button disabled
      */
     public get isUiDisabled() : boolean {
        return this._isUiDisabled;
     }
     public set isUiDisabled(v : boolean) {
        this._isUiDisabled = v;
     }
     
    
}