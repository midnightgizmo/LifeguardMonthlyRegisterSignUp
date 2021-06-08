<template>
    <div class="main-container">
    
        <div>
            <div>Lifeguard Training Register</div>
            <div>Admin Login</div>
        </div>
        <form id="login-container" method="post"  @submit.prevent="form_submit">

            
            <div id="username-container">
                <div><span>username</span></div>
                <div>
                    <input id="txtUsername" name="txtUsername" type="text"   v-model="username" />
                </div>
            </div>
<!--
            <div id="password-container">
                <div>
                    <input id="txtDateOfBirth" name="txtDateOfBirth" type="date" placeholder="Date of Birth"  v-model="dateOfBirth" />
                </div>
            </div>
-->
            <div id="password-container">
                <div><span>Password</span></div>

                <div>
                    <input v-model="password" />
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
import { loginAuthentication } from '@/models/authentication';
import { serverAuthentication } from '@/models/server';
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';




@Component({
  components: {

  },
})
export default class Login extends Vue 
{
    @Ref('cmdLoginSumit') readonly cmdLoginSumit! : HTMLButtonElement;

    public username : string = '';
    public password : string = '';
    public FormErrorText : string = '';

    public areErrorsOnForm : boolean = false;


    private isInSubmitState : boolean = false;
    public async form_submit()
    {
        // if we are allready in a submit state, we dont' want to try and resubmit.
        if(this.cmdLoginSumit)
            this.cmdLoginSumit.disabled = true;

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

            return;
        }

        let ajax = new serverAuthentication();

        try
        {
            let loginResponse : loginAuthentication;
            loginResponse = await ajax.adminLogin(this.username.trim(),this.password.trim());

            if(loginResponse.isLoggedIn == true)
            {// do a redirect
                
                //Vue.$cookies.set("jwt", loginResponse.jwt, 60 * 24 * 60 * 60);
                //VueCookies.set("jwt", loginResponse.jwt, 60 * 24 * 60 * 60);
                this.$router.push({ name: 'AdminHome' });
                
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
        


    }

/**
     * Does basic checks on the forms input values. If they fail the checks
     * it displays a message to the user telling them username and or password are incorrect
     * @returns true if submittable, else false
     */
    private IsFormSubmittable() : boolean
    {
        let areThereErrors = false;

        if(this.username.trim().length < 3)
        {

            areThereErrors = true;
            this.FormErrorText += "Incorrect surname.";
        }
        else if(this.password.trim().length < 3)
        {
            areThereErrors = true;
            this.FormErrorText += "Incorrect password.";
        }




        // if areThereErros is true it will make the errors appear on the screen
        this.areErrorsOnForm = areThereErrors;

        return !areThereErrors;
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
            font-size:1rem;
            
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
        /*
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
        }*/
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
        /*
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
        */
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
