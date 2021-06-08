<template>
    <div class="new-register-outer-container">
        <div>
            <router-link :to="{name:'AdminHome'}">Go back</router-link>
        </div>
        <header>
            <div>Create a new register</div>
        </header>
        
        <section>

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
                    <button ref="cmdInsertNew" @click="cmdInsertNew_click">Insert new</button>
                </div>
                <div>
                    
                </div>
            </div>

        </section>
        
        <div v-if="registersThatHaveBeenAddedWhileOnThisPage.length > 0" class="added-registers-container">
            <div v-for="(message, index) in registersThatHaveBeenAddedWhileOnThisPage" :key="index">
                {{message}}
            </div>
        </div>

    </div>
</template>

<script lang="ts">

import { ajaxRegister } from '@/models/server';
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';

@Component({
  components: {

  }
})
export default class NewRegister extends Vue
{
    @Ref('cmdInsertNew') readonly cmdInsertNew! : HTMLButtonElement

    public dateOfTraining : string = '';
    public timeOfTraining : string = '';

    public dateWhenCanSeeRegister : string = '';
    public timeWhenCanSeeRegister : string = '';

    public dateFromWhenUserCanJoinRegister : string = '';
    public timeFromWhenUserCanJoinRegister : string = '';

    public maxNumberOfCandidatesAllowedOnRegister : number = 6;

    // everytime a register has been added while we are on this page,
    // it will get added to this array (will add the date/time the register is meant to run)
    public registersThatHaveBeenAddedWhileOnThisPage : string[] = [];

    beforeMount()
    {
        // set some default values for dates and times on the form
        let todaysDate = new Date();
        this.dateOfTraining = this.formateDateToString(todaysDate);
        // set a default time in 24 hour clock format
        this.timeOfTraining = '12:00';

        this.timeWhenCanSeeRegister = "00:00";
        this.timeFromWhenUserCanJoinRegister = "00:00";
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
        this.dateWhenCanSeeRegister = this.formateDateToString(dateCanSeeRegister);

        // convert sthe string to a date object
        let dateCanJoinRegister = new Date(this.dateOfTraining);
        // set the date the user can join the register to the first day of
        // the previouse month for what this.dateOfTraining is set
        dateCanJoinRegister.setDate(1);
        dateCanJoinRegister.setMonth(dateCanJoinRegister.getMonth() - 1);
        this.dateFromWhenUserCanJoinRegister = this.formateDateToString(dateCanJoinRegister);
    }

    /** Keeps track on if we are in a submit state to the server */
    private isInSubmitState : boolean = false;
    /** When the user clicks the Insert New button */
    public async cmdInsertNew_click()
    {
        // if we are allready pressed the button and waiting for a response from the sever
        if(this.isInSubmitState == true)
            return;

        this.isInSubmitState = true;
        this.cmdInsertNew.disabled = true;

        // if one of the input boxes has not been filled in
        if(this.dateOfTraining.length == 0 ||
            this.timeOfTraining.length == 0 ||
            this.dateWhenCanSeeRegister.length == 0 ||
            this.timeWhenCanSeeRegister.length == 0 ||
            this.dateFromWhenUserCanJoinRegister.length == 0 ||
            this.timeFromWhenUserCanJoinRegister.length == 0)
        {
            this.isInSubmitState = false;
            this.cmdInsertNew.disabled = false;
            return;
        }

        let trainingDate = this.combineDateAndTimeStrings(this.dateOfTraining, this.timeOfTraining);
        let dateWhenCanSeeRegister = this.combineDateAndTimeStrings(this.dateWhenCanSeeRegister, this.timeWhenCanSeeRegister);
        let dateFromWhenUserCanJoinRegister = this.combineDateAndTimeStrings(this.dateFromWhenUserCanJoinRegister, this.timeFromWhenUserCanJoinRegister);

        let ajax = new ajaxRegister();

        let wasSucsefull = false;
        try
        {
            // send the data back to the sever for it to create the register
            wasSucsefull = await ajax.createRegister(trainingDate,dateWhenCanSeeRegister,dateFromWhenUserCanJoinRegister,this.maxNumberOfCandidatesAllowedOnRegister);
        }
        catch
        {
            wasSucsefull = false;
        }

        // if we were sucsefull in creating the register on the sever
        if(wasSucsefull == true)
        {
            let message = this.dateOfTraining + ' at ' + this.timeOfTraining + ' has been added';
            this.registersThatHaveBeenAddedWhileOnThisPage.unshift(message);
        }
        // we were unsucsefull in creating the register
        else
        {

        }


        this.isInSubmitState = false;
        this.cmdInsertNew.disabled = false;


        
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

    private combineDateAndTimeStrings(dateString : string, timeString : string) : string
    {
        //return new Date(dateString + ' ' + timeString);
        return dateString + ' ' + timeString;
    }
}


</script>


<style lang="less" scoped>
.new-register-outer-container
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
    > section
    {
        max-width: 800px;
        margin-left: auto;
        margin-right: auto;
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
                                cursor: pointer;
                            }
                            &:disabled
                            {
                                background-color: darken(#fff,10%);
                                cursor:wait;
                            }
                        }
                    }
                }
            }

        }
    }
}

.added-registers-container
{
    margin-top:30px;

    > div
    {
        font-size:1.2rem;
    }
}
</style>

