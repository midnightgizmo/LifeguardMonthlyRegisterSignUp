<template>
    <div id="admin-home-container">

        <div>
            <router-link :to="{name:'NewRegister'}">New register</router-link>
            <router-link :to="{name:'AdminUsers'}">Manage Users</router-link>
        </div>

        <header>
            <div>Filter results</div>
            
            <div>

                <div>
                    <span>All training between</span>
                </div>

                <div>
                    <input type="date"  v-model="searchDateStart">
                </div>

                <div><span>and</span></div>

                <div>
                    <input type="date" v-model="searchDateEnd">
                </div>

                <div>
                    <button @click="cmdSearch">Search</button>
                </div>
                
            </div>

        </header>

        <!-- if there are training register, we will list them bellow -->
        <section v-if="areThereTrainingRegisters == true" class="training-registers-outer-container">
            
            <div>
                <!-- header container -->
                <div>
                    <div>
                        <div></div>
                        <div>Date/Time of training</div>
                        <div></div>
                    </div>
                    
                </div>
                <!-- many body of data container -->
                <div>
                    <!-- each row -->
                    <div v-for="aRegister in registersList" :key="aRegister.id">
                        <div>
                            <router-link 
                                :to="{name:'EditMonth', 
                                params: { registerID:aRegister.id }}">Select</router-link>
                        </div>
                        <div>
                            <span>{{aRegister.startDate | formatDateToString}}</span>
                            <span>-</span>
                            <span>{{aRegister.startDate | formatTimeToString}}</span>
                        </div>
                        <div>
                            <router-link 
                                :to="{name:'EditRegister', 
                                params: { registerID:aRegister.id }}">Edit</router-link>
                        </div>
                        <div>
                            <button @click="cmdDelete_click($event)">Delete</button>
                            <button @click="cmdDelete_Cancel($event)" style="display:none;">Cancel</button>
                            <button @click="cmdConfirmDelete_click(aRegister)" style="display:none;">Confirm</button>
                        </div>

                    </div>
                </div>

            </div>
                

                
        </section>

        <!-- if there are no training registers -->
        <section v-else class="empty-Training-registers-outer-container">
            <div>
                <span>No registers within specified dates</span>
            </div>
        </section>
    </div>
</template>


<script lang="ts">

import { Register } from '@/models/register';
import { ajaxRegister } from '@/models/server';
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';

@Component({
  components: {

  },
  filters:{
      /** formats a date to a string e.g. "Saturday 12th" */
      formatDateToString(dateToFormat : Date)
      {
          const monthNames = ["January", "February", "March", "April", "May", "June",
                                "July", "August", "September", "October", "November", "December"];
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
        //return DayOfWeekAsString + " " + dayOfMonth + ordinal + " " + monthNames[dateToFormat.getMonth()] + " " +  dateToFormat.getFullYear().toString();
        return dayOfMonth + ordinal + " " + monthNames[dateToFormat.getMonth()] + " " +  dateToFormat.getFullYear().toString();
                 
      },
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
  }
})
export default class AdminHome extends Vue
{
    public searchDateStart: string = "2017-7-22";
    public searchDateEnd: string = "";

    public areThereTrainingRegisters : boolean = false;

    public registersList : Register[] = [];

    public created()
    {
        let currentDate = new Date();
        
        // create a start date set to the 1st of the current month
        // e.g. if today is 20 August 2020, set StartDate to
        // 1st August 2020
        let startDate = new Date();
        startDate.setMonth(currentDate.getMonth());
        startDate.setDate(1);
        // create the date as a string in the format year-month-day
        this.searchDateStart = startDate.getFullYear() + '-' + this.convertNumberToString(startDate.getMonth() + 1) + '-' + this.convertNumberToString(startDate.getDate());

        let endDate = new Date();
        endDate.setMonth(currentDate.getMonth() + 12);
        endDate.setDate(1);
        // create the date as a string in the format year-month-day
        this.searchDateEnd = endDate.getFullYear() + '-' + this.convertNumberToString(endDate.getMonth() + 1) + '-' + this.convertNumberToString(endDate.getDate());
    }

    /** Takes a number and converts to a string of a least 2 digits (adds a zero to any number below 10) */
    private convertNumberToString(num : number) :string
    {
        if(num < 10)
            return '0' + num.toString();
        else
            return num.toString();
    }

    mounted()
    {
        this.cmdSearch();
    }


    /** When the user clicks the Search button */
    public async cmdSearch()
    {// request from the server all training sessions that fall between the given start and end dates
        let startDate = new Date(this.searchDateStart);
        let endDate = new Date(this.searchDateEnd);

        let ajax = new ajaxRegister();

        // get all the registes for between the chosen selected dates from the server.
        let registersArray = await ajax.getRegistersBetween(startDate,endDate);

        // empty the array while still keeping it reactive to vuejs
        this.registersList.splice(0);

        // copy each register we got from the server into the registerList array
        registersArray.forEach( (aRegister : Register) =>
        {
            this.registersList.push(aRegister);
        });

        // if there are registers found betwen the start and end date
        // set areThereTrainingRegisters to true so the UI will display the registers
        if(this.registersList.length > 0)
            this.areThereTrainingRegisters = true;
        // if there are no registers found, set areThereTrainingRegisters to false
        // so the UI will display a message saying no register found between the 2 dates.
        else
            this.areThereTrainingRegisters = false;
    }

    public cmdDelete_click(evt : MouseEvent)
    {
        
        let cmdDelete = <HTMLElement>evt.target;
        let cmdDeleteCancelButton = <HTMLElement>cmdDelete.nextSibling;
        let cmdDeleteConfirmButton = <HTMLElement>cmdDeleteCancelButton.nextSibling;

        cmdDelete.style.display = "none";
        cmdDeleteCancelButton.style.display = "inline";
        cmdDeleteConfirmButton.style.display = "inline";
        
    }
    public cmdDelete_Cancel(evt : MouseEvent)
    {
        let cmdDeleteCancelButton = <HTMLElement>evt.target;
        let cmdDeleteConfirmButton = <HTMLElement>cmdDeleteCancelButton.nextSibling;
        let cmdDelete = <HTMLElement>cmdDeleteCancelButton.previousSibling;

        cmdDeleteCancelButton.style.display = "none";
        cmdDeleteConfirmButton.style.display = "none";
        cmdDelete.style.display = "inline";
    }
    private areWeInSubmitState : boolean = false;
    /**
     * When the user confirms they want to delete the register
     */
    public async cmdConfirmDelete_click(aRegister : Register)
    {
        // make sure we are not allready in a submit state
        if(this.areWeInSubmitState == true)
            return;

        this.areWeInSubmitState = true;

        // find the index position the register is in the list of registers
        let indexPoistion = this.registersList.findIndex((eachRegister : Register) => eachRegister.id == aRegister.id);
        
        // if we could not find the register in the array
        if(indexPoistion == -1)
        {
            this.areWeInSubmitState = false;
            return;
        }

        // ask the server to delete the register
        let ajax = new ajaxRegister();
        let wasSucsesfull = false;
        
        try
        {
            wasSucsesfull = await ajax.adminRemoveRegister(aRegister.id);
        }
        catch{}


        if(wasSucsesfull == true)
            // remove the register from the array
            this.registersList.splice(indexPoistion,1);

        this.areWeInSubmitState = false;
        return;
        
    }
}



</script>




<style lang="less" scoped>
#admin-home-container
{
    box-sizing:border-box;

    
    > div:nth-child(1)
    {
        display:grid;
        grid-template-columns: 1fr;
        row-gap: 10px;
        
        max-width: 900px;
        margin-left: auto;
        margin-right: auto;
        
        text-align: right;
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
        > div:nth-child(1)
        {
            
            font-size:2rem;
            color:#489ED8;
            
        }
        

        > div:nth-child(2)
        {
            display:grid;
            grid-template-columns: 10fr 10fr 4fr 10fr 6fr;

            max-width: 900px;
            margin-left: auto;
            margin-right: auto;
            margin-top: 25px;

            > div
            {
                align-self: center;
                // container for the button
                &:nth-child(5)
                {
                    padding-left:20px;
                    align-self: stretch;
                }

                > span
                {
                    font-size:1.5rem;
                    
                }

                > input
                {
                    width:100%;
                    background-color: #F4F4F4;
                    border:none;
                    font-size:1.5rem;
                    padding:10px 10px;

                    box-sizing: border-box;
                }

                > button
                {
                    width: 100%;
                    height:100%;

                    background-color: #489ED8;
                    border:none;
                    color:#F0F0F0;
                    font-size:1.2rem;

                    &:hover
                    {
                        background-color: darken(#489ED8,10%);
                    }
                    &:active
                    {
                        background-color: darken(#489ED8,15%);
                    }
                }
            }
        }
    }
}


.training-registers-outer-container
{
    margin-top: 30px;
    font-size:1.2rem;
    > div
    {
        display: table;
        max-width: 800px;
        margin-left: auto;
        margin-right: auto;

        border-spacing: 20px 0;

        > div:nth-child(1)
        {
            display:table-header-group;
            
            > div
            {
                display:table-row;

                > div
                {
                    display:table-cell;
                    padding-bottom: 10px;

                    color:black;
                    font-weight: bold;
                }
            }
        }

 

        > div:nth-child(2)
        {
            display:table-row-group;

            > div
            {
                display:table-row;
                
                

                > div
                {
                    display:table-cell;

                    padding-top: 10px;
                    padding-bottom:10px;

                    // holds the date and time
                    &:nth-child(2)
                    {
                        > span
                        {
                            // the dash (-) seperator between date and time
                            &:nth-child(2)
                            {
                                margin-left:5px;
                                margin-right: 5px;
                            }

                        }
                    }

                    > a
                    {
                        text-decoration:none;
                        color:#489ED8;
                        &:active, &:visited
                        {
                            color:#489ED8;
                        }
                        &:hover
                        {
                            text-decoration: underline;
                        }
                    }

                    > button
                    {
                        border:none;
                        font-size: 1.2rem;

                        &:hover
                        {
                            text-decoration:underline;
                        }
                        &:nth-child(1)
                        {
                            background-color: transparent;
                            color:#489ED8;
                        }
                        &:nth-child(2)
                        {
                            background-color: red;
                            color:white;
                            border-radius: 5px;
                        }
                        &:nth-child(3)
                        {
                            margin-left: 10px;
                            background-color: green;
                            color:white;
                            border-radius: 5px;
                        }
                    }
                }

            }
        }
    }

}

.empty-Training-registers-outer-container
{

}
</style>