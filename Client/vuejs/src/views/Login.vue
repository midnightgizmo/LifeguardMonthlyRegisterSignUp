<template>
    <div class="main-container">
    
        <div>
            <div>Lifeguard Training <br> Register</div>
        </div>
        <form id="login-container" method="post"  @submit.prevent="form_submit">

            
            <div id="username-container">
                <div><span>Surname</span></div>
                <div>
                    <input id="txtSurname" name="txtSurname" type="text"   v-model="surname" />
                </div>
            </div>
<!--
            <div id="password-container">
                <div>
                    <input id="txtDateOfBirth" name="txtDateOfBirth" type="date" placeholder="Date of Birth"  v-model="dateOfBirth" />
                </div>
            </div>
-->
            <div id="dateofbirth-container">
                <div><span>Date of birth</span></div>

                <div>
                    <div>Day</div>
                    <div>Month</div>
                    <div>Year</div>
                    <select id="txtDateofBirth_Day" name="txtDateofBirth_Day" placeholder="Day"  v-model="dateOfBirth_Day" >
                        <option v-for="index in 31" :key="index" v-bind:value="index">
                            {{ index }}
                        </option>
                    </select>
                    <select id="txtDateofBirth_Month" name="txtDateofBirth_Month" placeholder="Month"  v-model="dateOfBirth_Month" >
                        <option v-for="(aMonth, index) in monthsOfTheYear" :key="index" v-bind:value="(index + 1)">
                            {{ (index+1) }} - {{aMonth}}
                        </option>
                    </select>
                    <select id="txtDateofBirth_Year" name="txtDateofBirth_Year" placeholder="Year"  v-model="dateOfBirth_Year" >
                        <option v-for="(aYear, index) in listOfYears" :key="index" v-bind:value="aYear">
                            {{aYear}}
                        </option>
                    </select>
                </div>
            </div>

            <div id="error-container" v-show="areErrorsOnForm">
                <div style="margin-bottom:10px;">
                    <span>
                    {{FormErrorText}}
                    </span>
                </div>
            </div>

            <div id="submit-container">
                <div>
                    <button type="submit" ref="cmdLoginSumit" >Login</button>
                </div>
            </div>

        </form>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';
import {serverAuthentication} from '@/models/server';
import {loginAuthentication} from '@/models/authentication';



@Component({
  components: {

  },
})
export default class Login extends Vue 
{
    @Ref('cmdLoginSumit') readonly cmdLoginSumit! : HTMLButtonElement
    @Ref('txtSurname') readonly txtFileName!: HTMLInputElement
    // the text the user inputed in the surname input form
    private surname: string = '';
    // the text the user inputed in the dateOfBirth input form
    //private dateOfBirth : string = '';

    private dateOfBirth_Day : string = '1'; // set default selected day to day 1 of month
    private dateOfBirth_Month : string = '1'; // select default selected month to Jan
    private dateOfBirth_Year : string = ((new Date().getFullYear()) - 16).toString(); // select default selected year to 16 years ago

    // lists all months from jan to dec (used to populate drop down combo box)
    private monthsOfTheYear : string[] = [];
    // contains a list of years. Years will get displayed in a drop down combo box
    private listOfYears : string[] = []

    // keeps track of if there are errors on the login form
    private areErrorsOnForm : boolean = false;
    // error text to show on the form is its incorrect in anyway
    private FormErrorText : string = '';

    // keeps track of when/if the form is in a submit state.
    private isInSubmitState = false;


    beforeMount()
    {
        // populate the Months of the year
        this.monthsOfTheYear.push('January');
        this.monthsOfTheYear.push('February');
        this.monthsOfTheYear.push('March');
        this.monthsOfTheYear.push('April');
        this.monthsOfTheYear.push('May');
        this.monthsOfTheYear.push('June');
        this.monthsOfTheYear.push('July');
        this.monthsOfTheYear.push('August');
        this.monthsOfTheYear.push('September');
        this.monthsOfTheYear.push('October');
        this.monthsOfTheYear.push('November');
        this.monthsOfTheYear.push('December');

        // get todays date
        let now = new Date();
        // get the current year
        let currentYear = new Date().getFullYear();
        // work out what the date was 15 years ago (because a lifeguard needs to be 16 or over)
        let fifthteenYearsAgo = currentYear - 15;
        // work out what the date was 80 years ago (assuming lifeguards won't be over 80 years old, although this is possible)
        let sixtyYearsAgo = currentYear - 80;

        // create a list of years for the user to choose from to say the year they were born
        for(let eachYear = fifthteenYearsAgo; eachYear >= sixtyYearsAgo; eachYear--)
        {
            this.listOfYears.push(eachYear.toString());
        }

    }

    async form_submit()
    {
        

        let surname = '';
        //let dateOfBirth = '';

        // if we are allready in a submit state, we dont' want to try and resubmit.
        if(this.isInSubmitState == true)
            return;

        // stop users from creating multiple submits (disable the submit button)
        if(this.cmdLoginSumit)
            this.cmdLoginSumit.disabled = true;
        
        this.startSubmitAnimation();

        // reset any error text that may have been present on the last submit
        this.FormErrorText = '';
        // reset errors to false
        this.areErrorsOnForm = false;

        // check form is in a state to be submitted.
        // if its not IsFormSubmittable will display errors to the user to let
        // them know what is wrong
        if(!this.IsFormSubmittable())
        {
            this.isInSubmitState = false;
            // reenable the submit button
            if(this.cmdLoginSumit)
                this.cmdLoginSumit.disabled = false;

            this.stopSubmitAnimation();
            return;
        }

        surname = this.surname.trim();
        //dateOfBirth = this.dateOfBirth.trim();

        let ajax = new serverAuthentication();

        try
        {
            let loginResponse : loginAuthentication;
            loginResponse = await ajax.logIn(surname,this.dateOfBirth_Day, this.dateOfBirth_Month, this.dateOfBirth_Year);

            if(loginResponse.isLoggedIn == true)
            {// do a redirect
                
                //Vue.$cookies.set("jwt", loginResponse.jwt, 60 * 24 * 60 * 60);
                //VueCookies.set("jwt", loginResponse.jwt, 60 * 24 * 60 * 60);
                this.$router.push({ name: 'home' });
                
            }
            else
            {// let user know why they were not logged in
                this.FormErrorText = loginResponse.errorMessage;
                // display the error message on the screen
                this.areErrorsOnForm = true;
            }
        }
        catch(e)
        {
            
            // unkonwn error, may be connection to the server was lost?
            this.FormErrorText = "Unable to log in. Might be a problem connecting to the server"
            this.areErrorsOnForm = true;
        }


        this.isInSubmitState = false;

        // reenable the submit button
            if(this.cmdLoginSumit)
                this.cmdLoginSumit.disabled = false;
        this.stopSubmitAnimation();
        
    }

    /**
     * Does basic checks on the forms input values. If they fail the checks
     * it displays a message to the user telling them surname and/or dateOfBirth
     * are incorrect
     * @returns true if submittable, else false
     */
    private IsFormSubmittable() : boolean
    {
        let areThereErrors = false;

        if(this.surname.trim().length < 3)
        {

            areThereErrors = true;
            this.FormErrorText += "Incorrect surname.";
        }

/*
        // check its a date the user has inputed
        let timestamp = Date.parse(this.dateOfBirth);
        if (isNaN(timestamp) == false) 
        {
            areThereErrors = true;
            this.FormErrorText += "Incorrect date format"
        }
        */

        // if areThereErros is true it will make the errors appear on the screen
        this.areErrorsOnForm = areThereErrors;

        return !areThereErrors;
    }

    // reference number to the timer event that gets created in startSubmitAnimation
    private submitTimerReferenceNumber : number = -1;
    // Starts a timer that animates the text in the submit button
    // so the user can see somthing is happening when they click
    // the submit button
    startSubmitAnimation()
    {
        // set the button text to a .
        if(this.cmdLoginSumit)
            this.cmdLoginSumit.innerText = ".";
        
        // start a timer function that gets called every
        // half a second
        this.submitTimerReferenceNumber = setInterval(() =>
        {
            if(this.cmdLoginSumit)
            {
                // keep adding a space and . to the button
                // text until it gets to 4 dot, at which
                // point set the button text to 1 dot
                // and it will keep then repeating.
                if(this.cmdLoginSumit.innerText.length > 6)
                    this.cmdLoginSumit.innerText = ".";
                else
                    this.cmdLoginSumit.innerText += " .";
            }
        },500);
    }

    // stops the animation on the submit button and puts its
    // text back to normal
    stopSubmitAnimation()
    {
        // stop the timer function called in startSubmitAnimation
        clearInterval(this.submitTimerReferenceNumber);
        this.submitTimerReferenceNumber = -1;

        // set the button text back to "Login"
        if(this.cmdLoginSumit)
            this.cmdLoginSumit.innerText = "Login"
    }
}
</script>
<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">

*
{
  box-sizing: border-box;
}

@DesktopBreakPoint: 600px;
@MobileBreakPoints: (@DesktopBreakPoint - 1);

.main-container
{
    > div:nth-child(1)
    {
        > div
        {
            font-size:2rem;
            color:red;
        }
        margin-bottom:20px;
    }
}

// Dekstop website styles
@media (min-width: @DesktopBreakPoint )
{
    html,body
    {
        height: 100%;// for background image
        padding: 0;
        margin: 0;

    }
    body
    {
        background-size:cover;
        background-position:center;
        

        > h1
        {
            color:white;
            font-family: Arial, Helvetica, sans-serif;
            text-align: center;
            
            margin: 0;      
            padding: 0;
            
            > div
            {
                padding-top:30px;
                padding-bottom:50px;
            }

        }
    }
    form#login-container
    {
        max-width: 380px;
        margin-left:auto;
        margin-right: auto;
        //margin-top: 75px;

        padding-top: 50px;
        padding-bottom: 1px;

        padding-left: 40px;
        padding-right: 40px;;
        
        //background-color:rgba(30, 39, 54, 0.7);
        border: 1px solid #e8e8e8;
        border-radius: 5px;

        > img
        {
            width:20%;
            margin-left:40%;
            margin-top: 60px;
            margin-bottom: 60px;
        }
        > div
        {
            width:100%;

        }

        > #username-container, #password-container
        {
            > div
            {

                &:nth-child(1)
                {
                    text-align: left;
                    > span
                    {
                        color:#1d4884;
                        font-weight: bold;
                        font-size:1.3rem;
                        margin-left:10px;
                        
                    }
                }


                > input
                {
                    color:black;
                    border:0;
                    border-bottom: 1px solid #bfe5ff;
                    background-color:rgb(189 213 255 / 10%);

                    width:100%;

                    margin-bottom:30px;
                    font-size:1.3rem;
                    padding-left:10px;

                    &::placeholder
                    {
                        color:#cacaca;
                    }
                }
            }
        }
        > #dateofbirth-container
        {
            margin-bottom:70px;

            // first row ()
            > div:nth-child(1)
            {
                text-align: left;
                > span
                {
                    color:#1d4884;
                    font-weight: bold;
                    font-size:1.3rem;
                    margin-left:10px;
                    
                }
            }
            > div:nth-child(2)
            {
                display:grid;
                grid-template-columns: 1fr 1fr 1fr;

                padding-left:10px;
                border-bottom: 1px solid #bfe5ff;

                > div
                {
                    margin-top:5px;
                    text-align: left;;
                }
                
                select
                {
                    width:100%;
                    padding:5px;

                    color:black;
                    font-size:1.3rem;

                    border:0;
                }

                
            }
        }
        #error-container
        {
            > div
            {
                > span
                {
                    color:red;
                    font-family: Arial, Helvetica, sans-serif;
                }
            }
        }
        #submit-container
        {
            margin-bottom: 30px;

            > div
            {
                > button
                {
                    border:none;
                    background-color:#1d4884;
                    color:white;
                    text-align: center;
                    width:100%;
                    font-size:1.3rem;
                    height:50px;

                    cursor: pointer;

                    &:disabled
                    {
                        
                        background-color:lighten(#1d4884, 20%);
                    }
                }
            }
        }
    }
}

// mobile site styles 
@media (max-width: @MobileBreakPoints)
{

    html,body
    {
        height: 100%;// for background image
        padding: 0;
        margin: 0;

    }
    body
    {
        background-size:cover;
        background-position:center;
        

        > h1
        {
            color:white;
            font-family: Arial, Helvetica, sans-serif;
            text-align: center;
            
            margin: 0;      
            padding: 0;
            
            > div
            {
                padding-top:30px;
                padding-bottom:50px;
            }

        }
    }
    form#login-container
    {
        margin-left:10px;
        margin-right: 10px;
        margin-top: 30px;

        padding-top: 30px;
        padding-bottom: 1px;

        padding-left: 40px;
        padding-right: 40px;;
        
        //background-color:rgba(30, 39, 54, 0.7);
        border: 1px solid #e8e8e8;
        border-radius: 5px;

        > img
        {
            width:20%;
            margin-left:40%;
            margin-top: 60px;
            margin-bottom: 60px;
        }
        > div
        {
            width:100%;

        }

        > #username-container, #password-container
        {
            > div
            {
                &:nth-child(1)
                {
                    text-align: left;
                    > span
                    {
                        color:#1d4884;
                        font-weight: normal;
                        font-size:1.1rem;
                        margin-left:10px;
                        
                    }
                }

                > input
                {
                    color:black;
                    border:0;
                    border-bottom: 1px solid #bfe5ff;
                    //background-color:rgb(189 213 255 / 10%);
                    background-color: #E8F0FE;
                    width:100%;

                    margin-bottom:30px;
                    font-size:1.3rem;
                    padding-left:10px;

                    &::placeholder
                    {
                        color:#cacaca;
                    }
                }
            }
        }
        > #dateofbirth-container
        {
            margin-bottom:30px;

            // first row ()
            > div:nth-child(1)
            {
                text-align: left;
                > span
                {
                    color:#1d4884;
                    font-weight: normal;
                    font-size:1.1rem;
                    margin-left:10px;
                    
                }
            }
            > div:nth-child(2)
            {
                display:grid;
                grid-template-columns: 1fr 1fr 1fr;

                padding-left:10px;
                border-bottom: 1px solid #bfe5ff;

                > div
                {
                    margin-top:5px;
                    text-align: left;;
                }
                
                select
                {
                    width:100%;
                    padding:5px;

                    color:black;
                    background-color: #E8F0FE;
                    font-size:1.3rem;

                    border:0;
                }

                
            }
        }
        #error-container
        {
            > div
            {
                > span
                {
                    color:red;
                    font-family: Arial, Helvetica, sans-serif;
                }
            }
        }
        #submit-container
        {
            margin-bottom: 60px;

            > div
            {
                > button
                {
                    border:none;
                    background-color:#1d4884;
                    color:white;
                    text-align: center;
                    width:100%;
                    font-size:1.3rem;
                    height:50px;

                    cursor: pointer;

                    &:disabled
                    {
                        background-color:lighten(#1d4884, 20%);
                    }
                }
            }
        }
    }

}



</style>