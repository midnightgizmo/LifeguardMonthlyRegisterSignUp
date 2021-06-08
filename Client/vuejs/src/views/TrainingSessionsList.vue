<template>
    <div class="main-container">

        <header>
            <h2>Training Sessions</h2>
            <div>(click a training session for more info)</div>
        </header>

        <section>
            <div class="training-month-container" v-for="aMonth in trainingMonths" :key="aMonth.monthNumber">
                <header>
                    <router-link 
                        :to="{name:'trainingMonth', params: { year:aMonth.year, month:aMonth.monthNumber }}">{{aMonth.monthName}}</router-link>
                </header>

                <div>
                    <router-link v-for="aRegister in aMonth.trainingRegistersInMonth" :key="aRegister.id"
                        :to="{ name: 'trainingMonth', params: { year : aMonth.year, month : aMonth.monthNumber }}">
                        <span>{{aRegister.startDate | formatDateToString}}</span>
                        <span> - </span>
                        <span>{{aRegister.startDate | formatTimeToString}}</span>
                    </router-link>
                </div>
                
            </div>
        </section>
        
    </div>
</template>

<script lang="ts">
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';
import {ajaxRegister} from '@/models/server';
import {Register} from '@/models/register';

/*
interface TrainingMonth
{
    monthNumber : number;
    monthName : string;
    trainingRegistersInMonth : Register[];

}
*/
class TrainingMonth
{
    public year : number = 2000;
    public monthNumber : number = -1;
    public monthName : string = '';
    public trainingRegistersInMonth : Register[] = [];
}


@Component({
  components: {

  },
  filters:{
      /** formats a date to a string e.g. "Saturday 12th" */
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
export default class TrainingSessionsList extends Vue 
{
    private trainingMonths :TrainingMonth[] = [];
    private registerList : Register[] = [];
    
    async beforeMount()
    {
        let ajax = new ajaxRegister();
        // get all the training registers from the server
        let registers = await ajax.getAvalableRegisters();
        // go through each training register and sort them into months
        registers.forEach((register :Register) =>
        {
            //Takes each a register that was sent from the server and adds it to trainingMonths
            this.addTrainingRegisterToTrainingMonthsList(register);
        })


    }

    /**
     * called from beforeMount. Takes each a register that was sent
     * from the server and adds it to trainingMonths
     */
    private addTrainingRegisterToTrainingMonthsList(register :Register) : void
    {
        let aTrainingMonth : TrainingMonth | undefined
        // see if we have allready found previouse training register
        // that match the same month as the current register in the loop
        aTrainingMonth = this.trainingMonths.find( t => t.monthNumber == register.startDate.getMonth() )

        if(aTrainingMonth == undefined)
        {// current register is the first one we have come across
            // in the month it falls in, so we need to create a new TrainingMonth to put it in
            aTrainingMonth = new TrainingMonth();
            // set the year number
            aTrainingMonth.year = register.startDate.getFullYear();
            // set the month number
            aTrainingMonth.monthNumber = register.startDate.getMonth();
            // set the month name e.g. January
            aTrainingMonth.monthName = register.startDate.toLocaleString('default', { month: 'long' });
            
            // add the Training month to the array or training months
            this.trainingMonths.push(aTrainingMonth);
            
            /*
            aTrainingMonth = 
            {
                monthNumber: register.startDate.getMonth(),
                monthName : register.startDate.toLocaleString('default', { month: 'long' }),
                trainingRegistersInMonth : []
            };
            */
        }
        // add the register to the training month we found or just created
        aTrainingMonth.trainingRegistersInMonth.push(register);
    }
}
</script>



<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
.main-container
{
    > header
    {
        > h2
        {
            font-weight:400;
            font-size: 2rem;
            color:#489ED8;
            text-align: center;
            margin:0;
            padding:0;
        }
        > div
        {
            font-size: 0.9rem;
            color:black;
            text-align: center;
        }
    }

    > section
    {
        margin-top:30px;
    }
}

.training-month-container
{
    margin-top:30px;
    &:nth-child(1)
    {
        margin-top:0;
    }
    > header
    {
        > a
        {
            display:block;
            text-decoration:none;
            color:#FF0000;
            font-size:1.2rem;

            &:visited, &:active
            {
                color:#FF0000;
            }
        }
    }

    > div
    {
        // each register in a month container
        > a, >a:visited, > a:active > a:link > a:hover
        {
            &:nth-child(1)
            {
                margin-top:5px;
            }
/*
            &:visited, &:active
            {
                >span
                {
                &:nth-child(1)
                {
                    color:black;
                }
                // the dash between date and time
                &:nth-child(2)
                {
                    color:red;
                }
                // time part "12:50pm"
                &:nth-chil(3)
                {
                    color:#005d2e;
                }
                }
            }*/

            text-decoration: none;
            display:block;
            > span
            {
                font-weight: bold;
                font-size:1rem;
                line-height: 1.4rem;
                // date part "Saturday 12th"
                &:nth-child(1)
                {
                    color:black;
                }
                // the dash between date and time
                &:nth-child(2)
                {
                    color:red;
                }
                // time part "12:50pm"
                &:nth-child(3)
                {
                    color:#005d2e;
                }
            }
        }
    }
}
</style>