export class user
{
    
    private _id : number = -1;
    public get id() : number {
        return this._id;
    }
    public set id(v : number) {
        this._id = v;
    }
    
    
    private _firstName : string = "";
    public get firstName() : string {
        return this._firstName;
    }
    public set firstName(v : string) {
        this._firstName = v.trim();
    }

    
    private _surname : string = "";
    public get surname() : string {
        return this._surname;
    }
    public set surname(v : string) {
        this._surname = v.trim();
    }

    
    
    public get shortName() : string {
        return this.surname.length > 0 ? this.firstName + " " + this.surname.charAt(0) : this.firstName;
    }
    
    public get fullName() : string
    {
        return this.firstName + ' ' + this.surname;
    }



    
    private _isUserActive : boolean = false;
    /**
     * This property will only be set when client is logged into the admin portal
     */
    public get isUserActive() : boolean {
        return this._isUserActive;
    }
    public set isUserActive(v : boolean) {
        this._isUserActive = v;
    }


    
    private _password : string ='';
    /**
     * This property will only be set when the admin client is updating the user
     * to a new password
     */
    public get password() : string {
        return this._password;
    }
    public set password(v : string) {
        this._password = v.trim();
    }
    

    private _modelCopy : user | null = null;
    /** 
     * When the model is to be put in an edit state (which will then allow changs to be undone)
     */
    public BeginEdit()
    {
        // make a copy of the model so we can revert the changes, should be wish to,
        // by calling this.CancelEdit
        this._modelCopy = this.copy();
    }
    /**
     * When the model is in an edit state (from BeginEdit being called)
     * and when have decied to undo any changes made in the edit and 
     * restore the model to its state before BeginEdit was called
     */
    public CancelEdit()
    {
        // if we are not in an edit mode, there is nothing to undo
        if(this._modelCopy == null)
            return;

        // copy all the values from the model copy back into
        // this model to restore all values to the state
        // the model was in before BeginEdit was Called.
        this.id = this._modelCopy.id;
        this.firstName = this._modelCopy.firstName;
        this.surname = this._modelCopy.surname;
        this.isUserActive = this._modelCopy.isUserActive;
        this.password = this._modelCopy.password;

        this._modelCopy = null;
    }
    /**
     * When the model is in an edit state (from BeginEdit being called)
     * and we want to confirm the changes to the model and bring the 
     * model out of an edit state
     */
    public EndEdit()
    {
        // we are happy with the edits so keep the changes
        // and forget the backup values we had
        this._modelCopy = null;
    }

    /**
     * Makes a deep copy of the model
     */
    public copy() : user
    {
        let aUser = new user();
        aUser.id = this._id;
        aUser.firstName = this._firstName;
        aUser.surname = this._surname;
        aUser.isUserActive = this._isUserActive;
        aUser.password = this._password;

        return aUser;
    }
    
    
    
    
}