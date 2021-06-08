import { Register } from "./register"
import { user } from "./user"

export class RegisterWithUsers extends Register
{
    public usersInRegister : user[] = [];

    public isRegisterEditable : boolean = false;

    // e.g. Saturday 12th - 1:00pm
    //private _registerName : string = ""; // TODO: Need to compute this value
    public get registerName() : string
    {
        return this.formatDateToString(this.startDate) + " " +  this.formatTimeToString(this.startDate);
    }

    public inishalize() : void
    {
        super.inishalize();

        let todaysDate = new Date();
        // check to see if this register can be edited by the user (add or remove them self from the register)

        // if we have set the editable date to the future (past todays date) the user can't edit it
        if(this.editableDate > todaysDate)
            this.isRegisterEditable = false;
        // if the start date for the register has pased todays date, they can't edit it
        else if(this.startDate < todaysDate)
            this.isRegisterEditable = false;
        // if the register is full
        else if(this.usersInRegister.length >= this.maxNoCandidatesAllowed)
            this.isRegisterEditable = false;
        // register is ok to edit.
        else
            this.isRegisterEditable = true;
        
    }
    
    
}

