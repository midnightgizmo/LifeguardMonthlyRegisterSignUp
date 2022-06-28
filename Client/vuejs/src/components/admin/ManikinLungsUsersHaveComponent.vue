<template>
    <div>
        <div class="manikinlungs-header-container">
            <span>Manikin Lungs Issuesd to staff</span>
            <span>(Click a persons name to see lungs issued to that person)</span>
        </div>
        <div v-for="aUser in usersLungData" :key="aUser.userID" class="each-user">
            <div class="user-name-container" 
                v-bind:class="(aUser.DoesOneOrMoreLungsNeedReplacingForThisUser)?'expired':'not-expired'"
                @click="aUser.isVisable = !aUser.isVisable">{{aUser.userFullName}}</div>
            <div class="manikin-lungs-container"
                 v-bind:class="(aUser.isVisable)?'selected':'not-selected'">
                <div v-for="manikinLung in aUser.ManikinLungsUserHas" :key="manikinLung.id" class="manikin-lung-container" v-bind:class="(manikinLung.DoesManikinLungNeedReplacing)?'expired':'not-expired'">
                    <div>{{manikinLung.name}}</div>
                    <div>{{manikinLung.dateGivenManikinLung_FormattedString}}</div>
                    <div>{{manikinLung.numberOfDaysSinceIssuedLung}}</div>
                    <div>
                        <button @click="cmdIssueNewLung_Click(aUser,manikinLung)" :disabled="manikinLung.isUiDisabled">Issue New lung</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script lang="ts">

import { Component, Prop, Vue, Ref } from 'vue-property-decorator';
import {ListOfLungsUserHas, ManikinLungdata} from '@/models/ManikinLungs/ListOfLungsUserHas';
import {ajaxManikinLungs,ajaxUsers} from '@/models/server';
import { LungType } from '@/models/ManikinLungs/LungType';
import { LungUserGiven } from '@/models/ManikinLungs/LungUserGiven';
import { user } from '@/models/user';
@Component
export default class ManikinLungsUsersHaveComponent extends Vue
{
    public usersLungData : ListOfLungsUserHas[] = [];


    public async created()
    {
        
        this.usersLungData = await this.createListOfUsersWithLatestManikinLungs();
       
    }

     private async createListOfUsersWithLatestManikinLungs() : Promise<ListOfLungsUserHas[]>
    {
        let AjaxManikinLungs = new ajaxManikinLungs();
        let AjaxUsers = new ajaxUsers();
        // get from the database, all the differnet type of lungs there are
        let listOfLungs : LungType[] = await AjaxManikinLungs.adminGetListOfManikinLungTypes();


        let LungsGivenToActiveUsers : LungUserGiven[] = await AjaxManikinLungs.adminGetLatestLungsGivenToActiveUsers();

        let ListOfActiveUsers : user[] =await  AjaxUsers.getAllActiveUsers();

        let lungsUserHasArray : ListOfLungsUserHas[] = [];

        // keeps track of the last user we were looking at in the loop (saves us having to keep searching through the loop)
        let currentUserLookingAt: ListOfLungsUserHas | undefined = undefined;
        LungsGivenToActiveUsers.forEach(element => 
        {
            // if this is the first time in the loop or the current element does not match the last current user we looked at
            if(currentUserLookingAt == undefined || (element.userID != currentUserLookingAt.userID))
            {
                // try and find the user we are currently looking at (element) within lungsUserHasArray.
                currentUserLookingAt = lungsUserHasArray.find( u => { return u.userID == element.userID});

                // if we could not find the current user in lungsUserHasArray
                if(currentUserLookingAt == undefined)
                {
                    // create a new ListOfLungsUserHas and populate it with the users details which we will find
                    // in the ListOfActiveUsers array
                    let aUser = ListOfActiveUsers.find(u => {return u.id == element.userID}) as user;
                    currentUserLookingAt = new ListOfLungsUserHas();
                    currentUserLookingAt.userID = aUser.id;
                    currentUserLookingAt.userFullName = aUser.fullName;

                    // add the currentUserLookingAt to the lungsUserHasArray array
                    lungsUserHasArray.push(currentUserLookingAt);
                }
            }

            // at this point in the code we should now have a currentUserLookingAt

            // create a new instance of ManikinLungdata and populate its properties
            let aManikinLung = new ManikinLungdata();
            aManikinLung.id = element.manikinLungTypeID;
            aManikinLung.dateGivenManikinLung = element.dateGivenManikinLung;
            aManikinLung.name = (listOfLungs.find(l => { return l.id==element.manikinLungTypeID;}) as LungType).name;
            
            // if the current lung needs replacing, set the DoesOneOrMoreLungsNeedReplacingForThisUser to true (will be set to false by default)
            if(aManikinLung.DoesManikinLungNeedReplacing == true)
                currentUserLookingAt.DoesOneOrMoreLungsNeedReplacingForThisUser = true;

            // add the manikin lung from (element) to the currentUserLookingAt
            currentUserLookingAt.ManikinLungsUserHas.push(aManikinLung);
            
        });

        return new Promise((sucsess,fail) =>
        {
            sucsess(lungsUserHasArray)
        });
    }

    public async cmdIssueNewLung_Click(userData: ListOfLungsUserHas, manikinLungData : ManikinLungdata)
    {
        let serverAjax = new ajaxManikinLungs();

        let currentTimeZoneDate = new Date();
        
        let utcNumber = Date.UTC(currentTimeZoneDate.getUTCFullYear(),
                                 currentTimeZoneDate.getUTCMonth(),
                                 currentTimeZoneDate.getUTCDate(),
                                 currentTimeZoneDate.getUTCHours(),
                                 currentTimeZoneDate.getUTCMinutes(),
                                 currentTimeZoneDate.getUTCSeconds(),0);

        
        let currentUtcDate = new Date(utcNumber);
        let UnixTimeStamp : number = Math.floor(currentUtcDate.getTime() / 1000);
        
        try
        {
            // disable the button that called this function
            manikinLungData.isUiDisabled = true;
            
            let newManikinLung = await serverAjax.adminAssignLungToUser(userData.userID,manikinLungData.id,UnixTimeStamp)
            manikinLungData.dateGivenManikinLung = newManikinLung.dateGivenManikinLung;
        }
        catch
        {
            // somthing went wrong and we could not update the manikin lung
        }
        finally
        {
            // reenable the button that called this function
            manikinLungData.isUiDisabled = false;
        }
    }
}
</script>

<style lang="less" scoped>

.manikinlungs-header-container
{
    margin-top: 30px;
    > span
    {
        
        color:#2a85c3;
        
        display:block;

        &:nth-child(1)
        {
            font-size:2rem;
        }
        &:nth-child(2)
        {
            font-size:1rem;
        }
    }
}
.each-user
{
    box-sizing: border-box;
    margin-top: 20px;

    > .user-name-container
    {
        font-size: 1.5rem;
        cursor:pointer;
    }

    > .manikin-lungs-container
    {
       


        // if it also has the selected class
        &.selected
        {
            display:block;
        }
        // if it also has the not-selected class
        &.not-selected
        {
            display:none;
        }


        > .manikin-lung-container
        {
            display:grid;
            grid-template-columns: 1fr 1fr 1fr 1fr;
            margin-top: 10px;
            padding-top: 5px;

            &:hover
            {
                color:green;
                background-color: rgb(227, 246, 255);
            }

            > div > button
            {
                border: none;
                background-color: #2a85c3;
                color:white;
                cursor: pointer;
                border-radius: 5px;

                &:hover
                {
                    background-color: lighten(#2a85c3,20%);
                    color:yellow;
                }
                &:active
                {
                    background-color: darken(#2a85c3,10%);
                }

                &:disabled
                {
                    background-color: lighten(#2a85c3,50%);
                    color:lighten(yellow, 50%);

                }
                
            }
            
        }
       
    }

    .expired
    {
        color:red;
    }

    .not-expired
    {
        color:black;
    }
}
</style>
