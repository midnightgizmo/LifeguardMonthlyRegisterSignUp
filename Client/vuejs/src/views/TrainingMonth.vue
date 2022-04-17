<template>
    <div id="training-month-container">
        <nav>
            <ul>
                <li>
                    <router-link :to="{name:'home'}">Home page</router-link>
                </li>
            </ul>
        </nav>
       <header>
            <div>
                <span>{{registerMonthData.monthName}} </span>
                <span>{{registerMonthData.year}}</span>
            </div>
        </header> 

        <section v-if="registerMonthData.areAllRegistersInMonthLocked == true" class="none-editable-dates-container">
            <div>These registers are currently locked and cannot be edited</div>

            <div>They may be locked due to one of the following reasons:</div>
            <div>
                <ul>
                    <li>You are trying to book onto them too early, in which case please try again at a later date.</li>
                    <li>It is too close to the start of the first training session in the month</li>
                    <li>The training dates are all in the past</li>
                </ul>
            </div>
        </section>
        
        <section v-else class="editable-dates-container">

            <!-- user can no longer add or remove them self from this months register
                 To close to the start of training or training date has passed -->
            <header v-if="registerMonthData.areAllRegistersNoLongerEditable" class="header-month-locked">
                <div>
                    This months register is now locked and can no longer be edited.
                </div>
            </header>

            <!-- User is booked onto one of the registers in the month -->
            <header v-else-if="registerMonthData.isUserBookedOntoMonth == true" class="header-booked-on">
                <div>
                    <div>
                        You are booked to attend the following training
                    </div>
                </div>

                <div>
                    <div v-if="registerMonthData.registerUserBookedOnto">
                        {{registerMonthData.registerUserBookedOnto.registerName}}
                    </div>

                    <button ref="cmdCancelBooking" v-on:click="cmdCancelBooking_click">Cancel Booking</button>
                </div>

            </header>

            <!-- User has not booked onto one of the registers in this month -->
            <header v-else class="header-not-booked-on">
                <div>
                    <div>Select and book the training you will be attending</div>
                </div>
                <div>
                    <select v-model="selectedRegister" class="theme-construction">
                        <option v-for="aRegister in registerMonthData.registersList" :key="aRegister.id" 
                            v-bind:value="aRegister"
                            :disabled="aRegister.isRegisterEditable ? false : true">
                            {{aRegister.registerName}}
                            </option>
                    </select>

                    <button ref="cmdConfirmBooking" v-on:click="cmdConfirmBooking_click">Confirm Booking</button>
                </div>
            </header>

            <div class="register-list-container">

                <div>
                    <div>People booked onto this month</div>
                </div>
                
                <div v-for="aRegister in registerMonthData.registersList" class="single-register-container" :key="aRegister.id">
                    <header>
                        <div>{{aRegister.registerName}}</div>
                        <span>Max allowed : {{aRegister.maxNoCandidatesAllowed}}</span>
                    </header>
                    <div v-if="aRegister.usersInRegister.length">
                        <div v-for="(eachUser, index) in aRegister.usersInRegister" :key="eachUser.id">
                            <div>{{eachUser.fullName}}</div>
                            <span>{{index + 1}}</span>
                        </div>
                        <span v-if="aRegister.usersInRegister.length >= aRegister.maxNoCandidatesAllowed">
                            This register is now full
                        </span>
                    </div>
                    <div v-else>
                        <span>no one currently booked in</span>
                    </div>
                </div>
            </div>

        </section>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';
import {Route} from 'vue-router';
import {ajaxRegister} from '@/models/server';
import { RegisterWithUsers } from '@/models/registerWithUsers';
import {WebSiteLocationData} from '@/data/settings';
import {user} from '@/models/user';
import { Register } from '@/models/register';


class RegisterMonth
{
    public year : string = "";
    public monthName : string = "";
    public registersList! : RegisterWithUsers[];

    //public areAllRegistersInPast : boolean = true;
    public areAllRegistersInMonthLocked : boolean = true;

    public isUserBookedOntoMonth : boolean = false;
    // if isUserBookedOntoMonth == true then registerUserBookedOnto will hold the register the user
    // is booked onto for this month, else undefined;
    public registerUserBookedOnto? : RegisterWithUsers = undefined;

    /** Date when user can no longer add or remove them selfs from registers in this onth */
    public dateWhenAllRegistersInMonthAreNoLongerEditable : Date = new Date();
    // this will be true when todays date is greater than the (first register in list, minus 5 days)
    public areAllRegistersNoLongerEditable = true;

    /** the unique id of the person whos logged in */
    public userID : number = -1;

    

    

    /** Call this once registerList has been populated*/
    public inishalize() : void
    {
        // make sure we have at least one register in the list
        if(this.registersList && this.registersList.length > 0)
        {
            // get the year and month name e.g. January
            this.year = this.registersList[0].startDate.getFullYear().toString();
            this.monthName = this.registersList[0].startDate.toLocaleString('default', { month: 'long' });

            let todaysDate = new Date();

            // set dateWhenAllRegistersInMonthAreNoLongerEditable to the first register in list start date.
            // this will allow us to subtract 5 days off the date
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setFullYear(this.registersList[0].startDate.getFullYear());
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setMonth(this.registersList[0].startDate.getMonth());
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setDate(this.registersList[0].startDate.getDate());
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setHours(this.registersList[0].startDate.getHours());
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setMinutes(this.registersList[0].startDate.getMinutes());
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setSeconds(this.registersList[0].startDate.getSeconds());
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setMinutes(this.registersList[0].startDate.getMilliseconds());

            // don't allow user to add or remove them self from this months register if todays date
            // is greater than the first register in the list, minus 5 days)
            this.dateWhenAllRegistersInMonthAreNoLongerEditable.setDate(this.registersList[0].startDate.getDate() - 2);
            if(todaysDate < this.dateWhenAllRegistersInMonthAreNoLongerEditable)
                this.areAllRegistersNoLongerEditable = false;
            
            this.registersList.forEach((eachRegister) =>
            {
                
                // if one of the registers has an editable start date thats less than or equal
                // to todays date we are set areAllRegistersInMonthLocked to false
                if(eachRegister.editableDate <= todaysDate)
                    this.areAllRegistersInMonthLocked = false;

                // We need to see if the user exists in this register, but only if
                // they were not found in a previouse register
                if(this.isUserBookedOntoMonth == false)
                {
                    // look through the users in the register and see if one of them is
                    // the current user logged in
                    for(let userCount = 0; userCount < eachRegister.usersInRegister.length; userCount++)
                    {
                        let aUser = eachRegister.usersInRegister[userCount];
                        if(aUser.id == this.userID)
                        {// user exists in this register
                            
                            // set the model up so we it knows current user is booked into one of the registers
                            this.isUserBookedOntoMonth = true;
                            // set the register the user is booked onto
                            this.registerUserBookedOnto = eachRegister;
                            break;
                        }
                    }
                }
    
            });
        }

    }

    /** Call this method when a user has clicked the Confirm booking button
     * and the sever has sent back a user model for the person who was booked ontothe register
     * @returns true if sucsefull, else false
     */
    public addLoggedInUserToRegister(userToAdd : user, registerID : number) : boolean
    {
        let register : RegisterWithUsers | undefined;
        // find the register we want to add the user too.
        register = this.registersList.find((r) => {return r.id == registerID});

        // if we could not find the register in the list of registers for this month
        if(register == undefined)
            // somthing has gone very wrong, this should not happen.
            return false;

        // set the booked in register to the register the user has just booked them self into
        this.registerUserBookedOnto = register;
        // let the model know the user has been booked onto one of the registers
        this.isUserBookedOntoMonth = true;
        // add the user to the register they just booked them self onto
        this.registerUserBookedOnto.usersInRegister.push(userToAdd);


        return true;
    }

    /**
     * Call this method when a user has clicked the Cancel booking button
     * and the server has sent back a user model for the person who was removed
     * from the passed in registerID
     * @returns true if sucsefull, else false
     */
    public removeLoggedInUserFromRegister(userToRemove : user, registerID : number) : boolean
    {
        let register : RegisterWithUsers | undefined;
        // find the register we want to remove the user from.
        register = this.registersList.find((r) => {return r.id == registerID});

        

        // if we could not find the register in the list of registers for this month
        if(register == undefined)
            // somthing has gone very wrong, this should not happen.
            return false;

        // find the index position of where the user is in the array
        let indexPositionOfUserInregister = register.usersInRegister.findIndex((aUser) => {return aUser.id == userToRemove.id})
        
        // if we could not find the user to be removed in the register
        if(indexPositionOfUserInregister == -1)
            // somthing has gone very wrong, this should not happen.
            return false;

        
        // remove the user from the register
        register.usersInRegister.splice(indexPositionOfUserInregister, 1);

        // set the booked in register to undefined because user is no longer booked into a register
        this.registerUserBookedOnto = undefined;
        // let the model know the user has been booked onto one of the registers
        this.isUserBookedOntoMonth = false;

        return true;

    }


}
// this is so we can access the router parameters that has been passed from
// TrainingSessionsList. The main TrainingMonth class below needs to implment this interface.
// (this.$route.params.year & this.$route.params.month)
interface WithRoute  {
	$route: Route
}
@Component({
  components: {

  }
})
export default class TrainingMonth extends Vue implements WithRoute
{
    @Ref('cmdConfirmBooking') readonly cmdConfirmBooking!: HTMLButtonElement;
    @Ref('cmdCancelBooking') readonly cmdCancelBooking!: HTMLButtonElement;

    // view model for this View
    public registerMonthData : RegisterMonth = new RegisterMonth();

    public selectedRegister? : RegisterWithUsers;

    async beforeMount()
    {
        let year:number = -1
        let month:number = -1;
        // try and get the router parameters that were passed to
        // us from TrainingSessionsList
        try
        {
            year = parseInt(this.$route.params.year)
            month = parseInt(this.$route.params.month);
        }
        catch(e)
        {}
        
        // see if we were able to get the parameter data
        if(year == -1 || isNaN(year) == true
            || month == -1 || isNaN(month) == true)
        {// data has not been set from a previouse page navigation

            // we need this data to be able to proceed.
            // As we do not have it, we cannot do anything.
            // so navigate the user back to the main page.
            this.$router.push({ name: 'home' });
        }

        // get the user id for client who logged in
        let userID = this.getUserIdFromJwtCookie();

        let ajax = new ajaxRegister();
        // let the register month data know the userID of the current logged in user
        this.registerMonthData.userID = userID;
        // go off to the server and get the registers for the given year and month
        this.registerMonthData.registersList = await ajax.getRegistesInMonth(year,month);
        this.registerMonthData.inishalize();

        
        
        

    
    }
    mounted()
    {
        
    }

    /*******************************
     * 
     *  P U B L I C    E V E N T S 
     *  
     *******************************/


    private isConfirmInSubmitState : boolean = false;
    /** When the user clicks the Confirm Booking button */
    public async cmdConfirmBooking_click()
    {
        // Ask the server to add the logged in user to the selected register (from the drop down list)


        // if we are allready in a submit state (button has allready been pressed)
        // then there is nothing we can do
        if(this.isConfirmInSubmitState == true)
            return;

        // has a register been selected from the drop down list
        if(this.selectedRegister == undefined)
            return;


        // put us into a submit state
        this.isConfirmInSubmitState = true;
        // set the button to a disabled state
        this.cmdConfirmBooking.disabled = true;

        // get the userID and registerID data needed to be passed to the server
        let userID = this.registerMonthData.userID;
        let registerID = this.selectedRegister.id;
        
        
        let userAddedToRegister;
        let ajax = new ajaxRegister();
        // send the uesrid and register id to the server and get back the user that
        // was added to the register. If user.id == -1, the user was not added
        // to the register
        userAddedToRegister = await ajax.addUserToRegister(userID, registerID)

        // if user id == -1, the user was not added to the register
        if(userAddedToRegister.id == -1)
        {
            // re enable the submit button but do nothing else
            this.isConfirmInSubmitState = false;
            this.cmdConfirmBooking.disabled = false;
            return;
        }

        // add the user to the register month view model and UI
        let wasSucsesfull = this.registerMonthData.addLoggedInUserToRegister(userAddedToRegister,registerID);

        // were we sucsefull in adding to the view model and UI
        if(wasSucsesfull == true)
        {
            // re enable the submit button (which will dispear because that section of code
            // will not be shown when the user exists in one of the reigsters for the given month)
            this.isConfirmInSubmitState = false;
            this.cmdConfirmBooking.disabled = false;
        }
        // somthing has gone wrong. 
        else
        {
            // this should not happen, so if we get hear somthing has gone very wrong.
            // redirect the user back to the home page.
            this.$router.push({ name: 'home' });
        }

    }

    private isCancelInSubmitState : boolean = false;
    /** When the user clicks the Cancel Booking button */
    public async cmdCancelBooking_click()
    {
        // Ask the server to remove the logged in user from the register they are booked in to attend this month


        // if we are allready in a submit state (button has allready been pressed)
        // then there is nothing we can do
        if(this.isCancelInSubmitState == true)
            return;

        // make sure the register the user is booked onto has a value. If it has no value, we can't do anything
        if(this.registerMonthData.registerUserBookedOnto == undefined || this.registerMonthData.registerUserBookedOnto == null)
            return;

        this.isCancelInSubmitState = true;
        this.cmdCancelBooking.disabled = true;

        // get the userID and registerID data needed to be passed to the server
        let userID = this.registerMonthData.userID;
        let registerID = this.registerMonthData.registerUserBookedOnto.id;

        // go off to the server and ask it to remove the user from the selected register
        let userRemovedFromRegister;
        let ajax = new ajaxRegister();
        // send the uesrid and register id to the server and get back the user that
        // was added to the register. If user.id == -1, the user was not added
        // to the register
        userRemovedFromRegister = await ajax.removeUserFromRegister(userID, registerID)

        // if user id == -1, the user was not removed to the register
        if(userRemovedFromRegister.id == -1)
        {
            // re enable the submit button but do nothing else
            this.isCancelInSubmitState = false;
            this.cmdCancelBooking.disabled = false;
            return;
        }

        // update the view model and remove the user from the selected register         
        let wasSucsesfull = this.registerMonthData.removeLoggedInUserFromRegister(userRemovedFromRegister,registerID);

        // were we sucsefull in removing the user from the registers view model and UI
        if(wasSucsesfull == true)
        {
            // re enable the cancel booking button (which will dispear because that section of code
            // will not be shown when the user no longer exists in one of the reigsters for the given month)
            this.isCancelInSubmitState = false;
            this.cmdCancelBooking.disabled = false;
        }
        // somthing has gone wrong. 
        else
        {
            // this should not happen, so if we get hear somthing has gone very wrong.
            // redirect the user back to the home page.
            this.$router.push({ name: 'home' });
        }



    }








    private getUserIdFromJwtCookie() : number
    {
        // get jwt cookie value
        let jwtCookieValue : string = this.$cookies.get(WebSiteLocationData.getCookieName);
        // were we able to get the cookies value, if not return -1
        if(jwtCookieValue == null)
            return -1;

        // split the jwt at the dot position, get the first position in the array and convert the base64 value to redable string
        let json = atob(<string>jwtCookieValue.split(".")[1])
        // convert the json data to a javascript object
        let userData = JSON.parse(json);
        // attempt to get the user id from the json object data
        let userID : number = userData.id == null ? -1 : parseInt(userData.id);

        // return the user id or -1 if not found
        return userID;
    }



    
}
</script>

<style scoped lang="less">
#training-month-container
{
    > nav
    {
        > ul
        {
            list-style-type: none;
            margin:0;
            padding:0;
            
            > li
            {
                margin:0;
                padding:0;
            }
        }
    }
    > header
    {
        > div
        {
            > span
            {
                color:red;
                font-size:2rem;
            }
        }
    }
}


.editable-dates-container
{
    margin-top: 30px;
    
}


    header.header-month-locked
    {

    }









    header.header-booked-on
    {
        // heading text container
        > div:nth-child(1)
        {
            // heading text
            > div
            {
                text-align: center;
                font-size: 1.3rem;
            }
        }

        > div:nth-child(2)
        {
            margin-top: 20px;

            > div
            {
                margin-left: auto;
                margin-right: auto;

                padding: 0.65em 2.5em 0.55em 0.75em;

                max-width:250px;
                background-color: #ABF4B9;

                font-size:1.1rem;
                font-weight: bold;
                
            }

            > button
            {
                margin-top: 10px;
                margin-left: auto;
                margin-right: auto;
                max-width: 300px;

                padding: 7px 20px;

                display:block;

                background-color: #C60000;
                color:#F0F0F0;
                font-size:1.3rem;
                text-align: center;
                border:none;
                outline:none;

                &:hover
                {
                    background-color:darken(#C60000,7%);
                    cursor: pointer;
                }

                &:active
                {
                    background-color:darken(#C60000,13%);
                }


                &:disabled
                {
                    background-color:lighten(#C60000,50%);
                    cursor:wait;
                }
            }
        }
    }










    header.header-not-booked-on
    {
        // heading text container
        > div:nth-child(1)
        {
            > div
            {
                text-align: center;
                font-size: 1.3rem;
            }
        }
        // select (drop down list) container
        > div:nth-child(2)
        {
            margin-top: 10px;

            .theme-construction 
            {
            --radius: 0;
            --baseFg: black;
            --baseBg: #F4F4F4;
            --accentFg: black;
            --accentBg: orange;
            --arrowFg:#F4F4F4;
            --arrowBg: #DDDDDD;
            }

            select 
            {
                //font: 400 12px/1.3 sans-serif;
                font-size:1rem;
                -webkit-appearance: none;
                appearance: none;
                color: var(--baseFg);
                //border: 1px solid var(--baseFg);
                border:none;
                line-height: 1;
                outline: 0;
                padding: 0.65em 2.5em 0.55em 0.75em;
                border-radius: var(--radius);
                background-color: var(--baseBg);
                background-image: linear-gradient(var(--arrowBg), var(--arrowBg)),
                    linear-gradient(-135deg, transparent 50%, var(--arrowBg) 50%),
                    linear-gradient(-225deg, transparent 50%, var(--arrowBg) 50%),
                    linear-gradient(var(--arrowBg) 42%, var(--arrowFg) 42%);
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
                linear-gradient(@arrow-bg-color 42%, var(--arrowFg) 42%);
            }

            select:active 
            {
                /*
                background-image: linear-gradient(var(--accentFg), var(--accentFg)),
                    linear-gradient(-135deg, transparent 50%, var(--accentFg) 50%),
                    linear-gradient(-225deg, transparent 50%, var(--accentFg) 50%),
                    linear-gradient(var(--accentFg) 42%, var(--accentBg) 42%);
                    */
                @arrow-bg-color: darken(#DDDDDD,10%);

                background-image: linear-gradient(var(--arrowBg), var(--arrowBg)),
                linear-gradient(-135deg, transparent 50%, @arrow-bg-color 50%),
                linear-gradient(-225deg, transparent 50%, @arrow-bg-color 50%),
                linear-gradient(@arrow-bg-color 42%, var(--arrowFg) 42%);

                //color: var(--accentBg);
                //border-color: var(--accentFg);
                //background-color: var(--accentFg);
            }


            > button
            {
                margin-top: 10px;
                margin-left: auto;
                margin-right: auto;
                max-width: 300px;

                padding: 7px 20px;

                display:block;

                background-color: #489ED8;
                color:#F0F0F0;
                font-size:1.3rem;
                text-align: center;
                border:none;
                outline:none;

                &:hover
                {
                    background-color:darken(#489ED8,7%);
                    cursor: pointer;
                }

                &:active
                {
                    background-color:darken(#489ED8,13%);
                }


                &:disabled
                {
                    background-color:lighten(#489ED8,30%);
                    cursor:wait;
                }
            }





        }
    }







    .register-list-container
    {
        margin-top: 50px;
        
        
        > div:nth-child(1)
        {
            margin-bottom: 20px;

            > div
            {
                font-size: 1.5rem;
                color:rgb(0, 162, 255);
                //font-weight: bold;
                
            }


        }




        // each training register container
        > div.single-register-container
        {
            &:nth-child(1)
            {
                margin-top:0;
            }

            margin-top:20px;

            > header
            {
                // register date and time.
                > div
                {
                    text-align: center;
                    font-size:1.4rem;
                    font-weight:bold;
                    //color:green;
                    color:black;
                }

                // max number of candidates allowed in register
                > span
                {
                    color: #048e42;
                    font-style: italic;
                    font-size: 0.8rem;
                    display: block;
                }
            }

            // container for list of users in register
            > div
            {
                //margin-top: 20px;
                max-width:250px;
                margin-left: auto;
                margin-right: auto;

                // each user container
                > div
                {
                    display: grid;
                    grid-template-columns: 1fr;

                    max-width: 250px;

                    @cell-border-style: 1px solid rgb(230, 230, 230);
                    border-bottom: @cell-border-style;
                    border-left: @cell-border-style;

                    // candidates name
                    > div
                    {
                        color:black;

                        border-top: @cell-border-style;
                        border-right: @cell-border-style;

                        width:100%;

                        padding-top: 7px;
                        padding-bottom: 7px;
                        
                        font-size:1rem;

                        grid-column-start: 1;
                        grid-row-start: 1;

                        // make each first letter of every word have a capital letter
                        // This is because its a persons name
                        text-transform: capitalize; 

                    }
                    // row number in register
                    > span
                    {
                        grid-column-start: 1;
                        grid-row-start: 1;
                        display: block;
                        justify-self: start;
                        align-self: center;
                        margin-left: 5px;
                        color: #e2e2e2;
                    }
                }
                > span
                {
                    color:Red;
                }

            }
        }
    }


</style>