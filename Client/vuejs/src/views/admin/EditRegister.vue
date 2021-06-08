<template>
    <div class="edit-register-outer-container">
        <div>
            <router-link :to="{name:'AdminHome'}">Go back</router-link>
        </div>
        <header>
            <div>update register</div>
        </header>
        
        <fieldset :disabled="isFormDisable">

            <div>
                <div>
                    <label>Date/Time of training</label>
                </div>
                <div>
                    <input type="date" v-model="dateOfTraining" @change="dateOfTraining_ValueChanged">
                </div>
                <div>
                    <input type="time" v-model="timeOfTraining">
                </div>
            </div>

            <div>
                <div>
                    <label>Date/Time from when user can see register</label>
                </div>
                <div>
                    <input type="date" v-model="dateWhenCanSeeRegister">
                </div>
                <div>
                    <input type="time" v-model="timeWhenCanSeeRegister">
                </div>
            </div>

            <div>
                <div>
                    <label>Date/Time from when user join register</label>
                </div>
                <div>
                    <input type="date" v-model="dateFromWhenUserCanJoinRegister">
                </div>
                <div>
                    <input type="time" v-model="timeFromWhenUserCanJoinRegister">
                </div>
            </div>

            <div>
                <div>
                    <label>Max number of candidates allowed on register</label>
                </div>
                <div>
                    <input type="number" v-model="maxNumberOfCandidatesAllowedOnRegister">
                </div>
                <div>
                    
                </div>
            </div>

            <div>
                <div>
                    
                </div>
                <div>
                    <button ref="cmdUpdate" @click="cmdUpdate_click">Update</button>
                </div>
                <div>
                    <router-link :to="{name:'AdminHome'}" class="cancel-button">Cancel</router-link>
                </div>
            </div>

        </fieldset>
        


    </div>
</template>




<script lang="ts">

import { Register } from '@/models/register';
import { ajaxRegister } from '@/models/server';
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';

@Component({
  components: {

  }
})
export default class EditRegister extends Vue
{
    @Ref('cmdUpdate') readonly cmdUpdate! : HTMLButtonElement

    // form will stay disabled until all data base been loaded from the server
    public isFormDisable : boolean = true;

    public dateOfTraining : string = '';
    public timeOfTraining : string = '';

    public dateWhenCanSeeRegister : string = '';
    public timeWhenCanSeeRegister : string = '';

    public dateFromWhenUserCanJoinRegister : string = '';
    public timeFromWhenUserCanJoinRegister : string = '';

    public maxNumberOfCandidatesAllowedOnRegister : number = 6;

    private _registerID : number = -1;

    public async beforeMount()
    {
        let registerID : number = -1;
        let registerInfo : Register | null;

        // try and get the router parameter that were passed to
        // us from admin Home page
        try
        {
            registerID = parseInt(this.$route.params.registerID)
            
        }
        catch(e)
        {}

        // see if we were able to get the parameter data
        if(registerID == -1 || isNaN(registerID) == true)
        {// data has not been set from a previouse page navigation

            // we need this data to be able to proceed.
            // As we do not have it, we cannot do anything.
            // so navigate the user back to the main admin page.
            this.$router.push({ name: 'AdminHome' });
        }

        // get the registers details from the sever.
        registerInfo = await this.getRegisterFromServer(registerID);

        // if we were unable to get the registers details from the server
        // we will be unable let the user use this page, so redirect them
        // back to admin home
        if(registerInfo == null)
            this.$router.push({ name: 'AdminHome' });
        
        // add the register details the the verables the UI
        // binds too
        this.addRegisterDetailsToUI(<Register>registerInfo);

        // now that we have all the data, enable the form for the user to change values
        this.isFormDisable = false;

    }



    /** When the user changes the dateOfTraining, set some default 
     * values for this.dateWhenCanSeeRegister &
     * this.dateFromWhenUserCanJoinRegister
     */
    public dateOfTraining_ValueChanged()
    {
        // convert the string to a date object
        let dateCanSeeRegister = new Date(this.dateOfTraining);
        // set the date user can see the register to 6 months before
        // this.dateOfTraining is set
        dateCanSeeRegister.setMonth(dateCanSeeRegister.getMonth() - 6);
        this.dateWhenCanSeeRegister = this.formatRegisterDateToString(dateCanSeeRegister);

        // convert sthe string to a date object
        let dateCanJoinRegister = new Date(this.dateOfTraining);
        // set the date the user can join the register to the first day of
        // the previouse month for what this.dateOfTraining is set
        dateCanJoinRegister.setDate(1);
        dateCanJoinRegister.setMonth(dateCanJoinRegister.getMonth() - 1);
        this.dateFromWhenUserCanJoinRegister = this.formatRegisterDateToString(dateCanJoinRegister);
    }



    /** Keeps track on if we are in a submit state to the server */
    private isInSubmitState : boolean = false;
    /** When the user clicks the Update button */
    public async cmdUpdate_click()
    {
        // if we are allready pressed the button and waiting for a response from the sever
        if(this.isInSubmitState == true)
            return;

        this.isInSubmitState = true;
        this.cmdUpdate.disabled = true;

        // if one of the input boxes has not been filled in
        if(this.dateOfTraining.length == 0 ||
            this.timeOfTraining.length == 0 ||
            this.dateWhenCanSeeRegister.length == 0 ||
            this.timeWhenCanSeeRegister.length == 0 ||
            this.dateFromWhenUserCanJoinRegister.length == 0 ||
            this.timeFromWhenUserCanJoinRegister.length == 0)
        {
            this.isInSubmitState = false;
            this.cmdUpdate.disabled = false;
            return;
        }

        // combine the date and time string values into one string
        let trainingDate = this.combineDateAndTimeStrings(this.dateOfTraining, this.timeOfTraining);
        let dateWhenCanSeeRegister = this.combineDateAndTimeStrings(this.dateWhenCanSeeRegister, this.timeWhenCanSeeRegister);
        let dateFromWhenUserCanJoinRegister = this.combineDateAndTimeStrings(this.dateFromWhenUserCanJoinRegister, this.timeFromWhenUserCanJoinRegister);

        let ajax = new ajaxRegister();

        let wasSucsefull = false;
        try
        {
            // send the data back to the sever for it to update the register
            wasSucsefull = await ajax.updateRegister(this._registerID,trainingDate,dateWhenCanSeeRegister,dateFromWhenUserCanJoinRegister,this.maxNumberOfCandidatesAllowedOnRegister);
        }
        catch
        {
            wasSucsefull = false;
        }

        // if we were sucsefull in creating the register on the sever
        if(wasSucsefull == true)
        {
            this.$router.push({ name: 'AdminHome' });
        }
        // we were unsucsefull in creating the register
        else
        {

        }


        this.isInSubmitState = false;
        this.cmdUpdate.disabled = false;


        
    }







    private async getRegisterFromServer(registerID : number) : Promise<Register | null>
    {
        let ajax = new ajaxRegister();
        try
        {
            return await ajax.getRegister(registerID);
        }
        catch
        {
            return null;
        }
    }

    /**
     * Take the passed in register details and add them to the verables
     * the UI binds too.
     */
    private addRegisterDetailsToUI(registerDetails : Register) : void
    {
        this._registerID = registerDetails.id;

        this.dateOfTraining = this.formatRegisterDateToString(registerDetails.startDate);
        this.timeOfTraining = this.formatRegisterTimeToString(registerDetails.startDate);

        this.dateWhenCanSeeRegister = this.formatRegisterDateToString(registerDetails.visibleDate);
        this.timeWhenCanSeeRegister = this.formatRegisterTimeToString(registerDetails.visibleDate);

        this.dateFromWhenUserCanJoinRegister = this.formatRegisterDateToString(registerDetails.editableDate);
        this.timeFromWhenUserCanJoinRegister = this.formatRegisterTimeToString(registerDetails.editableDate);

        this.maxNumberOfCandidatesAllowedOnRegister = registerDetails.maxNoCandidatesAllowed;
    }

    /**
     * converts the passed in date to a formated string that will be accepted by
     * the <input type="date" />
     */
    private formatRegisterDateToString(dateToFormat : Date) : string
    {
        return dateToFormat.getFullYear().toString()
                    + '-' + this.convertNumberToTwoDigitString(dateToFormat.getMonth() + 1)
                    + '-' + this.convertNumberToTwoDigitString(dateToFormat.getDate());
    }
    /**
     * converts the passed in time to a formated string that will be accepted by
     * the <input type="time" />
     */
    private formatRegisterTimeToString(timeToFormat : Date) : string
    {
        return this.convertNumberToTwoDigitString(timeToFormat.getHours()) 
                + ':' 
                + this.convertNumberToTwoDigitString(timeToFormat.getMinutes());
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

    private combineDateAndTimeStrings(dateString : string, timeString : string) : string
    {
        //return new Date(dateString + ' ' + timeString);
        return dateString + ' ' + timeString;
    }

    

}
</script>




<style lang="less" scoped>
.edit-register-outer-container
{
    box-sizing: border-box;
    
    > div
    {
        max-width: 800px;
        margin-left: auto;
        margin-right: auto;
        text-align: left;

        > a
        {
            color:#489ED8;
            &:visited, &:active
            {
                color:#489ED8;
            }
        }
    }
    > header
    {
        font-size:2rem;
        color:#489ED8;
    }
    > fieldset
    {
        max-width: 800px;
        margin-left: auto;
        margin-right: auto;

        border:0;
        
        // each row
        > div
        {
            display:grid;
            grid-template-columns:1fr;

            margin-top:20px;
            
            // select the first 3 rows (that contain date and time inputs)
            &:nth-child(1), &:nth-child(2), &:nth-child(3), &:nth-child(4)
            {
                display:grid;
                grid-template-columns: 3fr 2fr 2fr;
                column-gap: 30px;

                > div
                {
                    &:nth-child(1)
                    {
                        align-self: center;
                        font-size:1.2rem;
                        text-align: left;

                        > label
                        {
                            color:black;
                            
                        }
                    }

                    &:nth-child(2), &:nth-child(3)
                    {
                        > input
                        {
                            width:100%;
                            background-color: #F4F4F4;
                            border:none;
                            font-size:1.2rem;
                            padding:10px 10px;

                            box-sizing: border-box;
                        }

                        
                    }
                }
            }

                  

            // submit button row
            &:nth-child(5)
            {
                display:grid;
                grid-template-columns: 3fr 2fr 2fr;
                column-gap: 30px;

                > div
                {
                    &:nth-child(2)
                    {
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
                                background-color:darken(#489ED8,7%);
                                cursor: pointer;
                            }
                            &:active
                            {
                                background-color:darken(#489ED8,10%);
                                cursor: pointer;
                            }
                            &:disabled
                            {
                                background-color: darken(#fff,10%);
                                cursor:wait;
                            }
                        }
                    }

                    &:nth-child(3)
                    {
                        > a
                        {
                            display:inline-block;

                            background-color:#C60000;;
                            color:#F4F4F4;
                            border:none;

                            padding:10px 0;
                            width: 100%;
                            //height:100%;
                            font-size:1.2rem;

                            text-decoration:none;

                            &:hover
                            {
                                background-color:darken(#C60000,7%);
                                cursor: pointer;
                            }
                            &:active
                            {
                                background-color:darken(#C60000,10%);
                                cursor: pointer;
                            }
                            
                        }
                        
                    }
                }
            }

        }
    }
}

</style>