<template>
    <div class="main-users-outer-container">
        <div>
            <router-link :to="{name:'AdminHome'}">Go back</router-link>
        </div>
        <div>
            <NewUserComponent v-on:NewUserAdded="NewUserComponent_NewUserAdded" />
        </div>
        <div class="users-list-table-container">
            <!-- heading rows go hear -->
            <div class="user-header-row">
                <div><span>First Name</span></div>
                <div><span>Surname</span></div>
                <div><span>Is Active</span></div>
                <div><span>New Password</span></div>
                <div></div>
                <div></div>
            </div>
            <!-- each row of user -->
            <div v-for="aUser in listOfUsers" :key="aUser.id" class="user-row-outer-container">
                <UserRowComponent v-bind:userModel="aUser" v-on:UserDeleted="UserRowComponent_UserDeleted" />
            </div>
        </div>
    </div>
</template>

<script lang="ts">


import { Component, Vue, Ref, Prop } from 'vue-property-decorator';
import NewUserComponent from '@/components/admin/NewUserComponent.vue'; // @ is an alias to /src
import UserRowComponent from '@/components/admin/UserRowComponent.vue'; // @ is an alias to /src
import {ajaxUsers} from '@/models/server'
import { user } from '@/models/user';

@Component({
  components: {
      NewUserComponent,
      UserRowComponent,
  }
})
export default class Users extends Vue
{
    public listOfUsers : user[] = [];


    public async created()
    {
        let ajax : ajaxUsers = new ajaxUsers();

        try
        {
            // get all the users from the server
            let users =  await ajax.getAllUsers();
            // add each user to the this.listOfUsers array
            users.forEach((eachUser : user) =>{ this.listOfUsers.push(eachUser)});
        }
        catch{}

        

    }


    public NewUserComponent_NewUserAdded(newUser : user)
    {
        this.listOfUsers.push(newUser);
    }

    public UserRowComponent_UserDeleted(deletedUser: user)
    {
        // find the index position in the array of users
        let indexPosition = this.listOfUsers.findIndex((aUser: user) => aUser.id == deletedUser.id);

        if(indexPosition != -1)
            // remove the user from the array, which will in turn remove the user from the UI
            this.listOfUsers.splice(indexPosition,1);
    }
}


</script>

<style lang="less" scoped>
    .main-users-outer-container
    {
        max-width: 800px;
        margin-left: auto;
        margin-right: auto;


        > div
        {
            max-width:
             800px;
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


        
    }


    .users-list-table-container
    {
        margin-top: 20px;
        > .user-header-row
        {
            display:grid;
            grid-template-columns: repeat(6, 1fr);

            > div
            {
                > span
                {
                    color:#FF2C2C;
                    font-size:1rem;
                    font-weight: bold;
                }
            }
        }
        > .user-row-outer-container
        {
            margin-top:10px;
            &:nth-child(1)
            {
                margin-top: 0;
            }
        }
    }
</style>