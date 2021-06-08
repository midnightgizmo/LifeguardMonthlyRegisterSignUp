<template>
    <div class="each-user-row-container">
        <!-- if we are in edit mode, display the input controls -->
        <div v-if="isInEditMode" class="edit">
            <div v-if="userModel">
                <div><input type="text" v-model="userModel.firstName"></div>
                <div><input type="text" v-model="userModel.surname"></div>
                <div>
                    <select ref="ddlIsActive" name="ddlIsActive" v-model="userModel.isUserActive" class="theme-construction">
                        <option v-bind:value="true" selected>True</option>
                        <option v-bind:value="false">False</option>
                    </select>
                </div>
                <div><input type="date" v-model="userModel.password"></div>
                
                <div><button @click="cmdUpdate_click">Update</button></div>
                <div><button @click="cmdCancelUpdate_click">Cancel</button></div>
            </div>
        </div>

        <!-- if we are not in edit mode, display the read only data -->
        <div v-else class="read-only">
            <div v-if="userModel">
                <div><span>{{userModel.firstName}}</span></div>
                <div><span>{{userModel.surname}}</span></div>
                <div><span>{{userModel.isUserActive}}</span></div>
                <div></div>
                <div><button v-if="isInDeleteMode == false" @click="cmdEnableEdit_click">Edit</button></div>
                <div>
                    <button v-if="isInDeleteMode == false" @click="isInDeleteMode = true">Delete</button>
                    <div v-else>
                        <button @click="isInDeleteMode = false">Cancel</button>
                        <button @click="cmdConfirmDelete_Click">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>







<script lang="ts">

import { Component, Vue, Ref, Prop } from 'vue-property-decorator';
import { user } from '@/models/user';
import { ajaxUsers } from '@/models/server';


@Component
export default class UserRowComponent extends Vue
{
    @Prop(user) readonly userModel: user | undefined;

    public isInEditMode : boolean = false;
    public isInDeleteMode : boolean = false;

    public isInSubmitState : boolean = false;


    /**
     * When the user clicks on the confirm button to delete a user
     */
    public cmdConfirmDelete_Click()
    {
        // if this row is in some kind of submit state,
        // (this will because a, we allready pressed the delete button or
        // b, an edit update has been inishated)
        if(this.isInSubmitState) 
            return;

        // go off to the server and request the user be deleted
        this.askServerToRemoveUser().then((wasSucsefull : boolean) =>
        {
            if(wasSucsefull)
            {
                // fire an event for the parent of this new to intercept
                // so they can see the user has been deleted which means
                // this row will be removed from the UI;
                this.$emit('UserDeleted',this.userModel);
            }
        });

        this.isInDeleteMode = false;
        this.isInSubmitState = false;


    }
   
    /**
     * Asks the server to delete the user from the database
     */
    private async askServerToRemoveUser() : Promise<boolean>
    {
        let ajax = new ajaxUsers();
        let wasSucsefull : boolean = false;

        if(this.userModel == undefined)
            return false;
        try
        {
            wasSucsefull = await ajax.adminDeleteUser(this.userModel.id)
        }
        catch
        {
            wasSucsefull = false;
        }

        return wasSucsefull;
    }





    /**
     * When the user clicks the Edit button to put
     * the row into edit mode
     */
    public cmdEnableEdit_click()
    {
        if(this.userModel)
        {
            // set the model to edit mode
            this.userModel.BeginEdit();
            this.isInEditMode = true;
        }
    }
    

    /**
     * When in edit mode and the user clicks the update button
     */
    public async cmdUpdate_click()
    {
        // if we find some errors (e.g. blank first name), don't allow the user to submit the update
        if(this.areThereErrorsInEditMode() == true)
            return;

        if(this.userModel)
        {
            let ajax = new ajaxUsers();
            try
            {
                let updatedUserDetails = await ajax.adminUpdateUser(this.userModel);

                // copy the updated details from the server into the clientsuserModel.
                // they should be the same but best to take what the server is giving us
                // just to be safe.
                this.userModel.firstName = updatedUserDetails.firstName;
                this.userModel.surname = updatedUserDetails.surname;
                this.userModel.isUserActive = updatedUserDetails.isUserActive;

                // we are happy with the edits we have made
                this.userModel.EndEdit();
                // end edit mode.
                this.isInEditMode = false;
            }
            catch
            {
                // we need to let the use know somthing went wrong.

                // somthing went wrong so we can't change the details.
                // user will need to click the cancel button 
            }

            
        }


        
    }
    /**
     * Checks for changes the admin user has made and makes sure they are ok
     * @returns true if there are errors, else false
     */
    private areThereErrorsInEditMode() : boolean
    {
        // make sure we have an instance of usermodel
        if(this.userModel)
        {
            // check the first name has some text in it
            if(this.userModel.firstName.length < 1)
                return true;
            // check the surname has some text in it
            if(this.userModel.surname.length < 1)
                return true;
        }
        // if we get there far, there are no errors found
        return false;
    }
    /**
     * When in edit mode and the user clicks the cancel button
     */
    public cmdCancelUpdate_click()
    {
        if(this.userModel)
        {
            // revert back to the changes before we were in edit mode.
            this.userModel.CancelEdit();
        }

        this.isInEditMode = false;
    }
}
</script>





<style lang="less" scoped>
.each-user-row-container
{
    max-width: 800px;
    margin-left: auto;
    margin-right: auto;

    > .edit
    {
        > div
        {
            display:grid;
            grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
            column-gap: 10px;

            > div
            {
                input, select
                {
                    width:100%;

                    font-size:1.2rem;
                }


                > button
                {
                    border-radius: 5px;
                    border:none;
                    padding:5px 10px;
                    color:white;
                }

                &:nth-child(5)
                {
                    display:flex;
                    justify-content: flex-end;
                    > button
                    {
                        
                        background-color:green;
                        text-align: right;

                        &:hover
                        {
                            background-color: lighten(green,5%);
                        }
                        &:active
                        {
                            background-color: darken(green,5%);
                        }
                        
                    }
                }
                &:nth-child(6)
                {
                    display:flex;

                    > button
                    {
                        background-color: red;

                        &:hover
                        {
                            background-color: lighten(red,20%);
                        }
                        &:active
                        {
                            background-color: darken(red,5%);
                        }
                    }
                }
            }
        }
    }
    > .read-only
    {
        > div
        {
            display:grid;
            grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
            column-gap: 10px;

            > div
            {
                > span
                {
                    font-size: 1.2rem;
                }

                > button
                {
                    background: none;
                    border:none;
                    
                    color:#489ED8;
                    text-decoration: under;
                    font-size: 1.2rem;

                    cursor: pointer;

                    &:hover
                    {
                        color:lighten(#489ED8,10%);
                    }
                    &:active
                    {
                        color:darken(#489ED8,10%);
                    }
                }


                // delete confirmation container (confirm & cancel buttons container)
                > div
                {
                    display:grid;
                    grid-template-columns: 1fr 1fr;
                    column-gap: 3px;
                    > button
                    {
                        background:none;
                        border:none;
                        
                        font-size:1rem;
                        cursor: pointer;
                        padding:0;
                        margin:0;

                        &:nth-child(1)
                        {
                            color:red;

                            &:hover
                            {
                                color:lighten(rgb(255, 0, 0),5%);
                            }
                            &:active
                            {
                                color:darken(rgb(255, 0, 0),5%);
                            }
                        }
                        &:nth-child(2)
                        {
                            color:green;

                            &:hover
                            {
                                color:lighten(rgb(0, 128, 0),5%);
                            }
                            &:active
                            {
                                color:darken(rgb(0, 128, 0),5%);
                            }
                        }
                    }
                }
            }
        }
    }
}
</style>