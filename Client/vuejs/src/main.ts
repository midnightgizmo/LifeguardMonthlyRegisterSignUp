import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import axios from 'axios';

import {WebSiteLocationData} from '@/data/settings';

import VueCookies from 'vue-cookies';
Vue.use(VueCookies);

//axios.defaults.withCredentials = true;
//axios.defaults.baseURL = WebSiteLocationData.postBackRootURL;// intercept all response axios recieves and check

axios.defaults.baseURL = process.env.VUE_APP_apiPostBackURL;// intercept all response axios recieves and check

// intercept all response axios recieves and check
// to see if the the server sent a 403 back saying user is not logged in.
// If they are not logged in, do a vue redirect to the login page
axios.interceptors.response.use(
(res) => 
{
  if(res.status == 403)
  {
    // user does not appear to be logged in
    // redirect them to the login page
    router.replace({
      name: 'login',
      query: { redirect: router.currentRoute.fullPath }
    });
    
    return Promise.reject(res);
  }
  else
    return Promise.resolve(res);
},
(err) =>
{
  return Promise.resolve(err.response);
  //return Promise.reject(err);
});
axios.interceptors.request.use(
  (req) =>
  {
    
    // This needs to be true so cookies can be saved to the client web browser when
    // the server is a different origin to that of the client. Cookie will also not be sent back to the server
    // unless this is set to true.
    // Because in testing mode the client uses a different port to the server
    // the CORS kicks in. To get around this the server must send the Access-Control-Allow-Origin header.
    // The server could set the Access-Control-Allow-Origin to * meaning anyone can access it from any domain(origin)
    // however if we want to set cookie on the client side we, the client has to set withCredentials to true
    // and the server has to set Access-Control-Allow-Origin to origin of the client (* can't be used)
    // req.withCredentials does not send back any special headers to the server, in fact the server
    // does not know if the withCredentials has been set, but the client won't save any cookies 
    // set from the server unless it is set.
    req.withCredentials = true;
    return req;
  },
  (err) =>
  {
    return Promise.reject(err);
  }
);



Vue.config.productionTip = false



// on each navigation check to see if the page requires authenticaion (user logged in).
// if it does require authentication but the user is not authenticated, redirect
// the user to the login page.
router.beforeEach((to, from, next) =>
{
  
  // check to see if this route has any meta data
  if(to.meta)
  {
    // does the meta data contain a requiresAuth veriable
    if (typeof to.meta.requiresAuth !== 'undefined')
    {
      // check to see if requiresAuth has been set to true, meansing this route can't be
      // disaplayed unless the use is logged in
      if(to.meta.requiresAuth === true)
      {
        // if its an admin page
        if(to.path.indexOf("/admin") != -1)
        {

            // check to see if the user is logged in
            if(document.cookie.indexOf(WebSiteLocationData.getAdminCookieName + '=') == -1)
            {
                

                // user does not appear to be logged in
                // redirect them to the admin login page
                next({
                    name: 'AdminLogin',
                    query: { redirect: to.fullPath }
            });
            }
            else
                next();

        }
        // must be a nonen admin page
        else
        {

            // check to see if the user is logged in
            if(document.cookie.indexOf(WebSiteLocationData.getCookieName + '=') == -1)
            {
                //router.push({name : 'login'})

                // user does not appear to be logged in
                // redirect them to the login page
                next({
                    name: 'login',
                    query: { redirect: to.fullPath }
            });
            }
            else
                next();

        }
        
        
      }
      else
        next();

    }
    else
      next();
  }
  else
    next();




});



new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
