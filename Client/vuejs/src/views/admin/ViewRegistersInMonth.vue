<template>
    <div id="viewRegisterInMonth-container">
        <div>
            <router-link :to="{name:'AdminHome'}">Go back</router-link>
        </div>
        <header>
            <div>{{registerMonthData.monthName}} {{registerMonthData.year}}</div>
        </header>

        <section>
            <div v-for="aRegisterInMonth in registerMonthData.registersList" class="single-register-container" :key="aRegisterInMonth.register.id">
                <header>
                    <div>{{aRegisterInMonth.register.registerName}}</div>
                </header>
                
                <div>
                    <div class="eachUser" v-for="eachUser in aRegisterInMonth.register.usersInRegister" :key="eachUser.id">
                        <div>{{eachUser.shortName}}</div>
                        <div>
                            <button @click="cmdRemoveUser_Click(aRegisterInMonth, eachUser)" :disabled="areWeInSubmitState">Remove</button>
                        </div>
                    </div>
                    <div>
                        <select v-model="aRegisterInMonth.comboBoxSelectedUser" class="theme-construction">
                            <option v-for="aUser in aRegisterInMonth.userNotInRegister" :key="aUser.id" 
                                v-bind:value="aUser">
                                {{aUser.firstName}}
                            </option>
                        </select>
                        <button @click="cmdAddUser_click(aRegisterInMonth)" :disabled="areWeInSubmitState">Add</button>
                    </div>
                </div>

                <!--
                <div v-else>
                    <span>no one currently booked in</span>
                </div>
                -->

            </div>
        </section>

        <!-- List all active users that have not booked onto this month -->
        <section class="users-not-booked-on-container">
            <header>
                <span>Users Not booked onto month</span>
            </header>
            <div v-for="aUser in registerMonthData.usersNotBookedOntoAnyRegister" :key="aUser.id">
                <span>{{aUser.fullName}}</span>
            </div>
        </section>
    </div>
</template>

<script lang="ts">
import { Register } from '@/models/register';
import { RegisterWithUsers } from '@/models/registerWithUsers';
import { ajaxRegister, ajaxUsers } from '@/models/server';
import { user } from '@/models/user';
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';


/**
 * Holds all registers in the month
 */
class RegisterMonth
{
    public year : string = "";
    public monthName : string = "";
    public registersList : RegisterInMonth[] = [];

    // list of active users that have not booked onto any register in the given month
    public usersNotBookedOntoAnyRegister : user[] = [];

    // a list of all active users (populated when this.inishalize is called)
    private allActiveusers : user[] = [];

    /** gets the class ready to be used by the view */
    inishalize(regisersWithUsers : RegisterWithUsers[], allActiveusers : user[])
    {
        // create a list of all active users (this will be needed when this.createListOfActiveUsersNotOnAnyRegister() is called)
        for(let i = 0; i < allActiveusers.length; i++)
            this.allActiveusers.push(allActiveusers[i]);

        // foreach reagister, create a RegisterInMonth
        // that wil hold the register and all active users
        regisersWithUsers.forEach((aRegister) =>
        {
            let aRegisterInMonth = new RegisterInMonth(aRegister,allActiveusers);
            // add to the registersList array
            this.registersList.push(aRegisterInMonth);
        })


        // make sure we have at least one register in the list
        if(this.registersList && this.registersList.length > 0)
        {
            // get the year and month name e.g. January
            this.year = this.registersList[0].register.startDate.getFullYear().toString();
            this.monthName = this.registersList[0].register.startDate.toLocaleString('default', { month: 'long' });
        }

        // populates the this.usersNotBookedOntoAnyRegister with a list of user that are not
        // currently booked onto any register in this month
        this.createListOfActiveUsersNotOnAnyRegister();
    }

    /**
     * populates the this.usersNotBookedOntoAnyRegister with a list of user that are not
     * currently booked onto any register in this month
     */
    public createListOfActiveUsersNotOnAnyRegister()
    {
        // make sure the array is empty
        this.usersNotBookedOntoAnyRegister = [];

        // start by adding every active user to the list (we will then remove the ones that have been added to the registers)
        for(let eachUserCount = 0; eachUserCount < this.allActiveusers.length; eachUserCount++)
            this.usersNotBookedOntoAnyRegister.push(this.allActiveusers[eachUserCount]);

        // now go through each register and remove users from this.usersNotBookedOntoAnyRegister if they are found in a register

        // go through each register
        for(let registerCount = 0; registerCount < this.registersList.length; registerCount++)
        {
            // get all user in this register
            let usersInThisRegister = this.registersList[registerCount].register.usersInRegister;

            // go through each user
            for(let userCount = 0; userCount < usersInThisRegister.length; userCount++)
            {
                let indexPosition = this.usersNotBookedOntoAnyRegister.findIndex((u) => u.id == usersInThisRegister[userCount].id);
                // if the user in the register was found in this.usersNotBookedOntoAnyRegister
                if(indexPosition != -1)
                {// remove the user from this.usersNotBookedOntoAnyRegister 
                    this.usersNotBookedOntoAnyRegister.splice(indexPosition,1);
                }
            }
        }
    }
}
/**
 * a single register within the month
 */
class RegisterInMonth
{
    public register! : RegisterWithUsers;

    // all user that are currenty active as lifeguards
    public activeUsersList : user[];
    // all active users that are not currently in this register
    public userNotInRegister : user[] = [];

    public comboBoxSelectedUser! : user;

    constructor(registerWithUsers : RegisterWithUsers, allActiveusers : user[]) 
    {
        this.register = registerWithUsers;
        this.activeUsersList = allActiveusers;

        this.workOutWhichUsersAreNotInRegister();
        
    }

    /**
     * Adds the user to the register view model which then updates the UI
     */
    public addUserToRegister(User : user) : boolean
    {
        // find the user we want to add to the register in the userNotInRegister array (because we will need to remove them from this array)
        let indexPositionOfFoundUser = this.userNotInRegister.findIndex( (aUser) => {return aUser.id == User.id});
        // if we could not find the user in the array
        if(indexPositionOfFoundUser == -1)
            return false;

        // remove the user from the array
        this.userNotInRegister.splice(indexPositionOfFoundUser,1);

        // add the user to the register
        this.register.usersInRegister.push(User);

        return true;
    }

    /**
     * Removes the user from the register view model which then updates the UI
     */
    public removeUserFromRegister(User : user) : boolean
    {
        // find the user we want to remove from the register;
        let indexPositionOfFoundUser = this.register.usersInRegister.findIndex((aUser) => {return aUser.id == User.id});
        // if we could not find the user in the array
        if(indexPositionOfFoundUser == -1)
            return false;

        // remove the user from the array
        this.register.usersInRegister.splice(indexPositionOfFoundUser,1);

        // add the user to the userNotInRegister
        this.userNotInRegister.push(User);

        return true;
    }

    /** Works out which users are not in the register
     * and adds them to the this.userNotInRegister list
     */
    private workOutWhichUsersAreNotInRegister()
    {
        // go through all users and see if any of them
        // are in the register
        this.activeUsersList.forEach((aUser) =>
        {
            let foundUser = this.register.usersInRegister.find((otherUser) =>
            {
                return otherUser.id == aUser.id;
            });

            // if the current aUser does not exist in the register
            // add them to the this.userNotInRegister list
            if(foundUser == undefined)
                this.userNotInRegister.push(aUser);
        })
    }

}

@Component({
  components: {

  }
})
export default class ViewRegistersInMonth extends Vue
{
    public registerMonthData : RegisterMonth = new RegisterMonth();

    public async beforeMount()
    {
        let registerID = this.getRegisterID();
        // if we were unable to get the register id,
        // navigate the user back to the admin home page
        if(registerID == -1)
        {
            this.$router.push({ name: 'AdminHome' });
            return;
        }

        // using the register id, get all the register that
        // that are in that month with the users that are booked onto each regieter
        let registerList = await this.getRegistersInMonth(registerID);

        // if we unable to get the register list
        // navigate the user back to the admin homepage
        if(registerList == null)
        {
            this.$router.push({ name: 'AdminHome' });
            return;
        }

        // get a list of all active users
        let activeUsers = await this.getAllActiveUser();

        // inishlize the registerMonthData so we can use
        // it to populate the view
        this.registerMonthData.inishalize(<RegisterWithUsers[]>registerList,activeUsers);
    }

    /** gets the register ID that should be passed to us
     * via the route params from the previouse page we just came from
     * @returns RegisterID or -1 if not found
     */
    private getRegisterID() : number
    {
        try
        {
            let id = parseInt(this.$route.params.registerID);
            if(id == NaN)
                return -1;
            else
                return id;
        }
        catch
        {
            return -1;
        }
    }

    private async getRegistersInMonth(registerID : number) :Promise<RegisterWithUsers[] | null>
    {
        let ajaxRegisters = new ajaxRegister();
        let aRegister : Register;
        let registerWithUsers : RegisterWithUsers[];
        
        try
        {
            aRegister = await ajaxRegisters.getRegister(registerID);
            let year = aRegister.startDate.getFullYear();
            let month = aRegister.startDate.getMonth();

            registerWithUsers = await ajaxRegisters.getRegistesInMonth(year,month);
        }
        // if somthing went wrong
        catch
        {
            return null
        }

        return registerWithUsers;
    }

    /**
     * Gets all the users that are current set to active
     */
    private async getAllActiveUser() : Promise<user[]>
    {
        let ajaxUser = new ajaxUsers();
        let activeUsers : user[];
        
        try
        {
            // get all the users from the server
            activeUsers = await ajaxUser.getAllActiveUsers();
        }
        catch
        {
            activeUsers = [];
        }
        // go through each user we got from the sever
        // and copy them into the activeUsers array (which is a reactive vue array)
        // activeUsers.forEach((eachUser) => 
        // {
        //     this.activeUsers.push(eachUser);
        // })

        return activeUsers;
    }

    /** keeps track of in any add or remove buttons are in a submit state to the server */
    public areWeInSubmitState : boolean = false;
    public async cmdAddUser_click(registerInMonth : RegisterInMonth)
    {
        // if we are waiting for a response back from the server from any buttons. 
        // Exit the function and do nothing.
        if(this.areWeInSubmitState)
            return;

        // get the user we want to add to the register
        let userToAddToRegister : user = registerInMonth.comboBoxSelectedUser
        let wasSucsefull = false;

        try
        {
            // go off to the server and ask for the selected user in the drop down box to be added to the register
            let ajax = new ajaxRegister();
            wasSucsefull = await ajax.adminAddUserToRegister(userToAddToRegister.id,registerInMonth.register.id);
            
        }
        catch
        {
            // somthing went wrong and user was not added to register
            this.areWeInSubmitState = false;
            return;
        }

        if(wasSucsefull ==  true)
        {
            // update the view model and ui (add the user to the register)
            registerInMonth.addUserToRegister(userToAddToRegister);

            // update the list of people we are not currently booked into this month
            this.registerMonthData.createListOfActiveUsersNotOnAnyRegister();
        }
  
        this.areWeInSubmitState = false;
    }
    public async cmdRemoveUser_Click(register : RegisterInMonth, userToRemove : user)
    {
        // if we are waiting for a response back from the server from any buttons. 
        // Exit the function and do nothing.
        if(this.areWeInSubmitState)
            return;

        let wasSucsefull = false;

        try
        {
            // go off to the server and ask for the user to be removed from the register
            let ajax = new ajaxRegister();
            wasSucsefull = await ajax.adminRemoveUserFromRegister(userToRemove.id,register.register.id);
        }
        catch
        {
            // somthing went wrong and user was not added to register
            this.areWeInSubmitState = false;
            return;
        }

        if(wasSucsefull == true)
        {
            // update the view model and ui (remove the user from the register)
            register.removeUserFromRegister(userToRemove);
            // update the list of people we are not currently booked into this month
            this.registerMonthData.createListOfActiveUsersNotOnAnyRegister();
        }

            
        this.areWeInSubmitState = false;
    }

}
</script>

<style lang="less" scoped>

#viewRegisterInMonth-container
{
    > div
    {
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
        div
        {
            font-size:3rem;
            line-height: 4rem;
            color:#489ED8;
        }
    }
}

// each training register container
div.single-register-container
{
    &:nth-child(1)
    {
        margin-top:0;
    }

    margin-top:40px;

    > header
    {
        > div
        {
            text-align: center;
            font-size:1.4rem;
            font-weight:bold;
            //color:green;
            color:black;
        }
    }

    // container for list of users in register
    > div
    {
        //margin-top: 20px;
        max-width:350px;
        margin-left: auto;
        margin-right: auto;

        margin-top:20px;

        // each user container
        > div.eachUser
        {
            display: grid;
            grid-template-columns: 2fr 1fr;

            max-width: 350px;

            @cell-border-style: 1px solid rgb(230, 230, 230);
            border-bottom: @cell-border-style;
            border-left: @cell-border-style;

            &:hover
            {
                
                button
                {
                    background-color: #C60000!important;;
                }
            }

            > div
            {
                color:black;

                border-top: @cell-border-style;
                

                width:100%;

                padding-top: 7px;
                padding-bottom: 7px;
                
                font-size:1rem;

                

                &:nth-child(1)
                {
                    text-align:left;
                    padding-left:7px;
                }
                &:nth-child(2)
                {
                    border-right: @cell-border-style;
                    padding-top: 0;
                    padding-bottom: 0;
                    display:flex;

                    > button
                    {
                        
                        width:100%;
                        height: 100%;
                        border:none;
                        padding:0;
                        margin:0;
                        background-color:#FFBBBB;
                        color:#F0F0F0;
                        
                        
                            &:hover
                            {
                                background-color:darken(#C60000,5%)!important;
                            }
                            &:active
                            {
                                background-color:darken(#C60000,10%)!important;
                            }
                        

                    }
                }

                
            }

            
        }
        // holds the drop down list box and add button under each register
        div:nth-last-child(1)
        {
            display: grid;
            grid-template-columns: 2fr 1fr;

            max-width: 350px;

            > button
            {
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
                    background-color:darken(#489ED8,7%)!important;
                    cursor: pointer;
                }

                &:active
                {
                    background-color:darken(#489ED8,13%)!important;
                }
            }
        }
    }
}

.users-not-booked-on-container
{
    margin-top: 30px;
    > header
    {
        span
        {
            text-align: center;
            font-size:1.4rem;
            font-weight:bold;
            //color:green;
            color:black;
        }
    }
    > div
    {
        > span
        {
            line-height: 1.5rem;
        }
    }
}

</style>