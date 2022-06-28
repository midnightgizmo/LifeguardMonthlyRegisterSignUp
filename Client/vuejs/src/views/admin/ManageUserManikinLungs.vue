<template>
  <div class="manage-user-manikin-lungs">
    <menu>
        <router-link :to="{name:'AdminUsers'}">Back to  Users</router-link>
    </menu>
    <header>
        <div>
            <select v-model="selectedUser" @change="selectedUser_changed($event)" class="theme-construction">
                <option v-for="aUser in ListOfActiveUsers" :key="aUser.id" v-bind:value="aUser">{{aUser.fullName}}</option>
            </select>
        </div>
    </header>
    <section class="assign-lung-container">
        <header>
            <span>
                Assign manikin lung to user
            </span>
        </header>
        <div v-show="isUserSelected">
            <select v-model="selectedNewLungType" @change="selectedNewLung_changed($event)" class="theme-construction">

                <option v-for="aLung in lungTypes" :key="aLung.id" v-bind:value="aLung">{{aLung.name}}</option>
            </select>
            <div>
                <button @click="AssingNewLungToUser_Click(selectedUser,selectedNewLungType)" :disabled="isAssignNewMankinLungButtonDisabled">Assign</button>
            </div>
        </div>
    </section>
    <section class="manage-lung-container">
        <header>
            <span>Lungs been given</span>
        </header>
        <div>
            <div v-for="aLung in LungsUserHas.ManikinLungsUserHas" :key="aLung.uniqueIDForGUI"
                 v-bind:class="(aLung.DoesManikinLungNeedReplacing)?'expired':'not-expired'">
                <div>{{aLung.name}}</div>
                <div>{{aLung.dateGivenManikinLung_FormattedString}}</div>
                <div>
                    <button @click="cmdRemoveLungFromUser_click(aLung)" :disabled="aLung.isUiDisabled">Remove</button>
                </div>
            </div>
        </div>
    </section>
  </div>
</template>

<script lang="ts">
import { IdGenerator } from '@/classes/IdGenerator';
import { ListOfLungsUserHas, ManikinLungdata } from '@/models/ManikinLungs/ListOfLungsUserHas';
import { LungType } from '@/models/ManikinLungs/LungType';
import { LungUserGiven } from '@/models/ManikinLungs/LungUserGiven';
import { ajaxManikinLungs, ajaxUsers } from '@/models/server';
import { user } from '@/models/user';
import { Component, Vue, Ref, Prop } from 'vue-property-decorator';
import Users from './Users.vue';

@Component({
  components: {
    
  }
})
export default class ManageUserManikinLungs extends Vue
{
    private idGenerator: IdGenerator = new IdGenerator();

    private passedInUserID : number = 0;
    public passedInUserName : string = "";
    public selectedUser : user = new user();
    public ListOfActiveUsers : user[] = [];
    public LungsUserHas : ListOfLungsUserHas = new ListOfLungsUserHas();
    public lungTypes : LungType[] = [];

    // the selected lung in the <select /> tag when created a new lung to assign to a user
    public selectedNewLungType : LungType = new LungType();
    public isAssignNewMankinLungButtonDisabled : boolean = true;

    public get isUserSelected() :  boolean
    {
        if(this.selectedUser.id == -1)
            return false;
        else
            return true;
    }



    private async created()
    {
        //await this.getAllActiveUsersFromServer();
        //await this.getManikinLungTypes();

        // Get all active user & get all manikin lung types
        await Promise.all([this.getAllActiveUsersFromServer(),this.getManikinLungTypes()]);

        // if we were passed in a userID & userName
        // if sucsefull, populates this.userID & this.userName with the value passed in
        if(this.getPassedInParamsOrFail() == true)
        {
            // we should now have a userId and userName.
            this.LungsUserHas.userID = this.passedInUserID;
            this.LungsUserHas.userFullName = this.passedInUserName;

            // find the users information
            let foundUser = this.ListOfActiveUsers.find(u => u.id== this.passedInUserID);
            if(foundUser != undefined)
            {
                // set the found user as the active user for this page
                this.selectedUser = foundUser;
                // get a list of all the lungs this user has
                // and adds them to the LungsUserHas
                this.getLungsAssignedToUser();
            }
        }


        
        
    }

    private async getAllActiveUsersFromServer()
    {
        let ajax = new ajaxUsers();

        try
        {
            this.ListOfActiveUsers = await ajax.getAllActiveUsers();
        }
        catch
        {

        }
    }

    /**
     * Gets the UserID from the params that was passed to this page.
     * if not there or not right, navigate back to admin page that lits all users
     * @return true if scusefull, else false
     */
    private getPassedInParamsOrFail(): boolean
    {
        // get the userID we should have been passed in from navigating to this page
        let parameter : string = this.$route.params.userId;
        this.passedInUserName = this.$route.params.userName;
        // have we been sent the right data (userID and User Name)
        if(parameter != undefined && this.passedInUserName != undefined)
        {
            // try and convert the userid from a string to a number
            this.passedInUserID = parseInt(parameter);
            // if we could not convert the number or the number equals zero
            if(this.passedInUserID == NaN || this.passedInUserID == 0 || this.passedInUserName.length < 1)
            {
                // go back to the List of users on the admin page
                this.$router.push({ name: 'AdminUsers' });
                return false;
            }
            // we have the userid and username
            else
                return true;
            
        }
        // we have not been sent the user id &/Or userName
        else
        {
            // go back to the List of users on the admin page
            //this.$router.push({ name: 'AdminUsers' });

            // we have navigated to this page without passed user info
            // This means the user will have to select a user from the <select /> tag before
            // any information can be loaded from the server
            return false;
        }

    }

    private async getManikinLungTypes() : Promise<boolean>
    {
        let ajax = new ajaxManikinLungs()

        try
        {
            // get a list of all the lung types there are
            this.lungTypes = await ajax.adminGetListOfManikinLungTypes();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async getLungsAssignedToUser() : Promise<boolean>
    {
        let wasSucsesfull : boolean = false;
        let ajax = new ajaxManikinLungs();

        // make sure the array is empty before we start
        this.LungsUserHas.ManikinLungsUserHas.splice(0);

        try
        {
            
            // get all the lungs the user has
            let lungsGivenToUserArray :LungUserGiven[] = await ajax.adminGetLungsAssignedToUser(this.selectedUser.id,30);

            // match up the lung type to the lungs the user has
            lungsGivenToUserArray.forEach(element => {
                
                let aLungModel : ManikinLungdata = new ManikinLungdata();
                aLungModel.id = element.manikinLungTypeID;
                aLungModel.dateGivenManikinLung = element.dateGivenManikinLung;
                let foundLung = this.lungTypes.find(l => l.id==element.manikinLungTypeID);
                // this should not happen, but if we were unable to find the lung in the list of lungs
                // we can't add it to the array of ungs the user has, so just stick it
                if(foundLung == undefined)
                    return;
                aLungModel.name = foundLung.name;
                aLungModel.uniqueIDForGUI = this.idGenerator.GenerateUniqueId();
                // add the lung to the array
                this.LungsUserHas.ManikinLungsUserHas.push(aLungModel);
            });


            wasSucsesfull = true;
        }
        catch(e)
        {
            wasSucsesfull = false;
        }

        return wasSucsesfull;
    }

    public async AssingNewLungToUser_Click(selectedUser: user,selectedNewLungType: LungType)
    {
        let serverAjax = new ajaxManikinLungs();

        let currentTimeZoneDate = new Date();
        
        // Create a UTC date number for todays date
        let utcNumber = Date.UTC(currentTimeZoneDate.getUTCFullYear(),
                                 currentTimeZoneDate.getUTCMonth(),
                                 currentTimeZoneDate.getUTCDate(),
                                 currentTimeZoneDate.getUTCHours(),
                                 currentTimeZoneDate.getUTCMinutes(),
                                 currentTimeZoneDate.getUTCSeconds(),0);
        
        // convert the utc number to a date object
        let currentUtcDate = new Date(utcNumber);
        // convert the utc date to a unix time stamp
        let UnixTimeStamp : number = Math.floor(currentUtcDate.getTime() / 1000);
        
        try
        {
            // disable the button that called this function
            this.isAssignNewMankinLungButtonDisabled = true;
            
            // go off to the database and add the new lung to the user.
            let newManikinLung = await serverAjax.adminAssignLungToUser(selectedUser.id,selectedNewLungType.id,UnixTimeStamp);
            // copy the data we haev been sent back into a ManikinLungData class
            let newManikinLungData = new ManikinLungdata();
            newManikinLungData.id = newManikinLung.manikinLungTypeID;
            newManikinLungData.dateGivenManikinLung = newManikinLung.dateGivenManikinLung;
            // search for the lung type we have added (we need it to get the lung types name)
            let foundLungType : LungType|undefined  = this.lungTypes.find(l => l.id==newManikinLung.manikinLungTypeID);
            
            // make sure we did find the lung type we are looking
            if(foundLungType != undefined)
            {
                // get its name
                newManikinLungData.name = foundLungType.name;
                newManikinLungData.uniqueIDForGUI = this.idGenerator.GenerateUniqueId();

                // add the new lung that has been assigned to the selected use to the array.
                // this will make it show up in the UI.
                this.LungsUserHas.ManikinLungsUserHas.unshift(newManikinLungData);
            }
            
        }
        catch
        {
            // somthing went wrong and we could not update the manikin lung
        }
        finally
        {
            // reenable the button that called this function
            this.isAssignNewMankinLungButtonDisabled = false;
        }
    }

    public async cmdRemoveLungFromUser_click(lung : ManikinLungdata)
    {
        let ajax = new ajaxManikinLungs();
        let wasLungRemovedFromUser = false;
        
        try
        {
            // ask the server to remove the lung assinged to the user at the given date
            wasLungRemovedFromUser = await ajax.removeLungFromUser(this.passedInUserID,lung.id,lung.dateGivenManikinLung);
        }
        catch(e)
        {

            // somehing went wrong so we assume the lung was not removed
            wasLungRemovedFromUser = false;
        }

        if(wasLungRemovedFromUser == true)
        {
            // remove the lung from the javascript array.
            let indexInArray = this.LungsUserHas.ManikinLungsUserHas.indexOf(lung);
            this.LungsUserHas.ManikinLungsUserHas.splice(indexInArray,1);

            //let foundLung : ManikinLungdata | undefined = this.LungsUserHas.ManikinLungsUserHas.find( l => l.id==lung.id);

            //if(foundLung != undefined)
            //{
            //    this.LungsUserHas.ManikinLungsUserHas.
            //}
        }
    }

    public selectedNewLung_changed(evt:any)
    {
        this.isAssignNewMankinLungButtonDisabled = false;
    }

    public selectedUser_changed(evt:any)
    {
        // the new lungs for the newly selected person
        this.getLungsAssignedToUser();
    }
}
</script>

<style lang="less" scoped>

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

.manage-user-manikin-lungs
{
    max-width: 800px;
    margin-left: auto;
    margin-right: auto;
    
    > menu
    {
        text-align: right;
        a
        {
            margin-right: 20px;
            
        }
    }
    > header
    {

        > div
        {
            select 
            {
                //font: 400 12px/1.3 sans-serif;
                font-size:2rem;
                -webkit-appearance: none;
                appearance: none;
                color: #489ED8;
                //border: 1px solid var(--baseFg);
                border:none;
                line-height: 1;
                outline: 0;
                padding: 0.5rem 3rem;
                border-radius: var(--radius);
                background-color: var(--baseBg);
                background-image: linear-gradient(var(--arrowBg), var(--arrowBg)),
                    linear-gradient(-135deg, transparent 50%, var(--arrowBg) 50%),
                    linear-gradient(-225deg, transparent 50%, var(--arrowBg) 50%),
                    linear-gradient(var(--arrowBg) 40%, var(--arrowFg) 42%);
                background-repeat: no-repeat, no-repeat, no-repeat, no-repeat;
                background-size: 1px 100%, 29px 36px, 20px 42px, 29px 100%;
                background-position: right 28px center, right bottom, right bottom, right bottom;   
            }

            select:hover 
            {
                @arrow-bg-color: darken(#DDDDDD,10%);

                background-image: linear-gradient(var(--arrowBg), var(--arrowBg)),
                linear-gradient(-135deg, transparent 50%, @arrow-bg-color 50%),
                linear-gradient(-225deg, transparent 50%, @arrow-bg-color 50%),
                linear-gradient(@arrow-bg-color 40%, var(--arrowFg) 42%);
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
        }
    }
    > .assign-lung-container
    {
        margin-top: 20px;
        > header
        {
            text-align: left;
            font-size: 1.5rem;
            color:black;
        }
        > div
        {
            display:grid;
            grid-template-columns: 1fr 1fr;

            

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
        
            > div
            {
                display:flex;
                > button
                {
                    margin-left: 20px;
                    width:200px;
                }
            }

        }
    }

    > .manage-lung-container
    {
        margin-top: 40px;
        
        > header
        {
            text-align: left;
            font-size: 1.5rem;
            color:black;
        }

        > div
        {
            > div
            {
                display:grid;
                grid-template-columns: 2fr 2fr 1fr;
                padding-top: 5px;
                padding-bottom: 5px;
                font-size:1.2rem;

                &:hover
                {
                    background-color: rgb(204, 222, 232);
                }

                &.expired
                {
                    color:red;
                }

                > div
                {
                    > button
                    {
                        &:disabled
                        {
                            background-color: rgb(215, 215, 215);
                        }
                    }
                }
            }
        }
    }
}
</style>