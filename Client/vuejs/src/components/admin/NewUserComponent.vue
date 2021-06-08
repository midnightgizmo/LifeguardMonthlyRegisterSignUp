<template>
    <div>
        <form @submit.prevent="form_submit" class="new-user-outer-container" :disabled="_isInSubmitState">
            <!-- first row (labels) -->
            <div><label for="txtFirstName">First Name</label></div>
            <div><label for="txtSurname">Surname</label></div>
            <div><label for="ddlIsActive">Is Active</label></div>
            <div><label for="txtPassword">Password</label></div>
            <div></div>
            <div></div>


            <!-- second row, inputs -->
            <div><input ref="txtFirstName" type="text" name="txtFirstName" v-model="firstName" v-bind:class="{ 'error' : this.isErrorFirstName}"></div>
            <div><input ref="txtSurname" type="text" name="txtSurname" v-model="surname" v-bind:class="isErrorSurname ? 'error' : 'test'"></div>
            <div>
                <select ref="ddlIsActive" name="ddlIsActive" v-model="isActive" class="theme-construction">
                    <option value="1" selected>True</option>
                    <option value="0">False</option>
                </select>
            </div>
            <div><input ref="txtPassword" type="date" name="txtPassword" v-model="dateOfBirth"></div>
            <div><button ref="cmdAddNewUser" type="submit">Add New</button></div>
            <div></div>
        </form>

    </div>
</template>

<script lang="ts">
import { ajaxUsers } from '@/models/server';
import { user } from '@/models/user';
import { Component, Prop, Vue, Ref } from 'vue-property-decorator';

@Component
export default class NewUserComponent extends Vue
{
    
    @Ref('txtFirstName') readonly txtFirstName!: HTMLInputElement;
    @Ref('txtSurname') readonly txtSurname!: HTMLInputElement;
    @Ref('txtPassword') readonly txtPassword!: HTMLInputElement;
    @Ref('cmdAddNewUser') readonly cmdAddNewUser!: HTMLButtonElement;

    public firstName : string = '';
    public surname : string = '';
    public isActive : string = '1';
    public dateOfBirth : string = '';

    private _isInSubmitState : boolean = false;


    beforeMount()
    {
        // create a date that will be set to 16 years ago
        let aDate : Date = new Date();
        aDate.setFullYear(aDate.getFullYear() - 16);
        // convert the date to a string that an <input type="date"> will understand
        this.dateOfBirth = this.formateDateToString(aDate);
    }

    /** Converts a Date object to a string format
     * that the <input type=date /> will understand
     */
    private formateDateToString(date : Date) : string
    {
        return date.getFullYear().toString()
             + '-' + this.convertNumberToTwoDigitString(date.getMonth() + 1)
             + '-' + this.convertNumberToTwoDigitString(date.getDate());
    }
    /**
     * Converts the passed in number to a string.
     * If number passed in is below 10, the returned
     * string will have a leading zero e.g. 09
     */
    private convertNumberToTwoDigitString(num : number) : string
    {
        if(num < 10)
            return '0' + num.toString();
        else
            return num.toString();
    }


    /**
     * When user clicks the submit 'Add New' button to add a new candidate
     */
    public async form_submit()
    {

        // will hold the new users info when we get it back from the server
        let newUser : user;
        let ajax : ajaxUsers = new ajaxUsers();

        // make sure we are not allready in a submit state
        if(this._isInSubmitState == true)
            return;

        this._isInSubmitState = true;
        

        // check there are no errors on the page
        if(this.areThereErrorsOnForm() == true)
        {// we found errors on the page, which will be shown to the user by putting red box around anything that has an error

            this._isInSubmitState = false;
            return;
        }
        // send the user details off to the server to be added
        try
        {
            newUser = await ajax.adminCreateUser(this.firstName.trim(),
                                 this.surname.trim(),
                                 this.isActive == '1' ? true : false,
                                 this.dateOfBirth)
        }
        catch
        {// if somthing went wrong when trying to add the user on the server.

            this._isInSubmitState = false;
            return;
        }

        // if we make it this far, user added ok on the server
        this._isInSubmitState = false;

        // fire an event so that the parent view of this component can be 
        // told when a new user has been added.
        this.$emit('NewUserAdded',newUser);



    }

    
  
  
    
    public isErrorFirstName : boolean = false;
    public isErrorSurname : boolean = false;
    public isError_Password : boolean = false;
    private areThereErrorsOnForm() : boolean
    {
        
        let areThereErrors = false;

        if(this.firstName.trim().length < 1)
        {
            areThereErrors = true;
            this.isErrorFirstName = true;
        }
        else
            this.isErrorFirstName = false;



        if(this.surname.trim().length < 1)
        {
            areThereErrors = true;
            this.isErrorSurname = true;
        }
        else
            this.isErrorSurname = false;


        
        if(this.dateOfBirth.trim().length < 1)
        {
            areThereErrors = true;
            this.isError_Password = true;
        }
        else
            this.isError_Password = false;
        
        



        return areThereErrors;
    }



}
</script>

<style lang="less" scoped>

.new-user-outer-container
{
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
    column-gap: 10px;

    > div
    {
        
        > input
        {

            width:100%;
            background-color: #D9ECF9;
            border:none;
            font-size:1.2rem;
            padding:10px 10px;

            box-sizing: border-box;
        }
        > button
        {
            background-color:#489ED8;
            color:#F4F4F4;
            border:none;

            padding:10px 0;
            width: 100%;
            height:100%;
            font-size:1.2rem;

            &:hover
            {
                cursor: pointer;
                background-color: lighten(#489ED8,5%);
            }
            &:active
            {
                background-color: darken(#489ED8,5%);
            }
            &:disabled
            {
                background-color: darken(#fff,10%);
                cursor:wait;
            }
        }

        .theme-construction 
        {
        --radius: 0;
        --baseFg: black;
        --baseBg: #D9ECF9;
        --accentFg: black;
        --accentBg: orange;
        --arrowFg:#F4F4F4;
        --arrowBg: #DDDDDD;
        }

        select 
        {
            //font: 400 12px/1.3 sans-serif;
            font-size:1.2rem;
            -webkit-appearance: none;
            appearance: none;
            color: var(--baseFg);
            box-sizing: border-box;
            width: 100%;
            //border: 1px solid var(--baseFg);
            border:none;
            border-radius: 3px;
            line-height: 1;
            outline: 0;
            
            //padding: 0.65em 2.5em 0.55em 0.75em;
            padding:12px 10px;
            border-radius: var(--radius);
            background-color: var(--baseBg);
            background-image: linear-gradient(var(--arrowBg), var(--arrowBg)),
                linear-gradient(-135deg, transparent 50%, var(--arrowBg) 50%),
                linear-gradient(-225deg, transparent 50%, var(--arrowBg) 50%),
                linear-gradient(var(--arrowBg) 50%, var(--arrowFg) 50%);
            background-repeat: no-repeat, no-repeat, no-repeat, no-repeat;
            background-size: 1px 100%, 20px 22px, 20px 22px, 20px 100%;
            background-position: right 20px center, right bottom, right bottom, right bottom;   
        }

        select:hover 
        {
            @arrow-bg-color: darken(#DDDDDD,10%);

            background-image: linear-gradient(var(--arrowBg), var(--arrowBg)),
            linear-gradient(-135deg, transparent 50%, @arrow-bg-color 50%),
            linear-gradient(-225deg, transparent 50%, @arrow-bg-color 50%),
            linear-gradient(@arrow-bg-color 50%, var(--arrowFg) 50%);
        }

        select:active 
        {

            @arrow-bg-color: darken(#DDDDDD,10%);

            background-image: linear-gradient(var(--arrowBg), var(--arrowBg)),
            linear-gradient(-135deg, transparent 50%, @arrow-bg-color 50%),
            linear-gradient(-225deg, transparent 50%, @arrow-bg-color 50%),
            linear-gradient(@arrow-bg-color 50%, var(--arrowFg) 50%);

            
            //color: var(--accentBg);
            //border-color: var(--accentFg);
            //background-color: var(--accentFg);
        }
        select:focus
        {
            outline: 2px solid black;
        }

        // adds a border around an element to indicate its in an error state
        .error
        {
            border: 1px solid rgb(139, 1, 1);
        }
    }
}

</style>