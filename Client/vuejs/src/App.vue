<template>
    
  <div id="app">
    <div class="logout-container" v-if="isLoggedIn == true">
        <button v-on:click="cmdLogOut">Log out - {{userName}}</button>
    </div>
    <router-view/>
  </div>
</template>

<script lang="ts">
import {WebSiteLocationData} from '@/data/settings';
import { Component, Vue, Ref, Prop, Watch } from 'vue-property-decorator';
@Component({
  components: {

  }
})
export default class App extends Vue
{
    public isLoggedIn : boolean = false;
    public userName : string = '';

    @Watch('$route', { immediate: true, deep: true })
    onUrlChange(newVal: any) 
    {
      // get the name of the route for the current page we are on
      //newVal.name = "AdminLogin"
        this.userName = '';
        
        // if we looking at one of the admin pages
        if((<string>newVal.path).indexOf("/admin") != -1)
        {
          // if the admin user is logged in, display the logout button, else hide the logout button
          if(document.cookie.indexOf(WebSiteLocationData.getAdminCookieName + '=') != -1)
              this.isLoggedIn = true;
          else
              this.isLoggedIn = false;
        }
        // we are looking at anything but admin pages (the client pages)
        else
        {
          // if the user is logged in, display the logout button, else hide the logout button
          if(document.cookie.indexOf(WebSiteLocationData.getCookieName + '=') != -1)
          {
              this.isLoggedIn = true;
              this.userName = this.getUserNameFromJwtCookie();
          }
          else
              this.isLoggedIn = false;
        }
        
    }

    private getUserNameFromJwtCookie() : string
    {
        // get jwt cookie value
        let jwtCookieValue : string = this.$cookies.get(WebSiteLocationData.getCookieName);
        // were we able to get the cookies value, if not return empty string
        if(jwtCookieValue == null)
            return '';

        // split the jwt at the dot position, get the first position in the array and convert the base64 value to redable string
        let json = atob(<string>jwtCookieValue.split(".")[1])
        // convert the json data to a javascript object
        let userData = JSON.parse(json);
        // attempt to get the user first name from the json object data
        let userName : string = userData.userFirstName == null ? '' : userData.userFirstName;

        // return the user first name or empty string if could not be found
        return userName;
    }

    public cmdLogOut()
    {
        // if its the admin uesr thats logged in
        if(this.$router.currentRoute.path.indexOf('/admin/') != -1)
        {
          this.logAdminOut();
        }
        // if its a lifeguard that is logged in
        else
        {
          this.logNoneAdminOut();
        }
    }

    
    /**
     * Logs the client (none admin) out and redirect them to the login page
     */
    private logNoneAdminOut()
    {
        // create a date that is 5 days in the past
        let expiredCookieDate = new Date();
        expiredCookieDate.setDate(expiredCookieDate.getDate() - 5);
 
        

        // set is logged in to false so the logout button will not be shown on the login page
        this.isLoggedIn = false;
        // set the jwt cookies expiry date to 5 days ago (this will make the browser remove the cookie)
        this.$cookies.set(WebSiteLocationData.getCookieName,'',expiredCookieDate);
        // navigate to the login page
        this.$router.push({ name: 'login' });
    }

    /**
     * Logs the admin user out and redirects them to the admin login page
     */
    private logAdminOut()
    {
        // create a date that is 5 days in the past
        let expiredCookieDate = new Date();
        expiredCookieDate.setDate(expiredCookieDate.getDate() - 5);
 
        

        // set is logged in to false so the logout button will not be shown on the login page
        this.isLoggedIn = false;
        // set the jwt cookies expiry date to 5 days ago (this will make the browser remove the cookie)
        this.$cookies.set(WebSiteLocationData.getAdminCookieName,'',expiredCookieDate);
        // navigate to the admin login page
        this.$router.push({ name: 'AdminLogin' });
    }
}
</script>


<style lang="less">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;

    > div.logout-container
    {
        display:flex;
        > button
        {
            outline: none;
            background-color: transparent;
            border:none;

            &:hover
            {
                text-decoration: underline;
            }
        }
    }
}

/*
#nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
*/
</style>
