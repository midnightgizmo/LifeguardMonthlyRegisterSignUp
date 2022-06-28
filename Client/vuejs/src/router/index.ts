import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../views/Login.vue'
import TrainingSessionsList from '@/views/TrainingSessionsList.vue';
import TrainingMonth from '@/views/TrainingMonth.vue';
Vue.use(VueRouter)

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'home',
    component: TrainingSessionsList,
    meta : 
      {
        requiresAuth : true,
      }
  },
  {
    path: '/TrainingMonth',
    name:'trainingMonth',
    component: TrainingMonth,
    meta : 
      {
        requiresAuth : true,
      }
  },
  {
    path : '/login',
    name : 'login',
    component: Login
  },
  {
    path: '/about',
    name: 'About',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
  },
  {
    path: '/admin/login',
    name: 'AdminLogin',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/admin/AdminLogin.vue')
  }
  ,
  {
    path: '/admin/',
    name: 'AdminHome',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('@/views/admin/AdminHome.vue'),
    meta : 
      {
        requiresAuth : true,
      }
  }
  ,
  {
    path: '/admin/newregister',
    name: 'NewRegister',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('@/views/admin/NewRegister.vue'),
    meta : 
      {
        requiresAuth : true,
      }
  }
  ,
  {
    path: '/admin/edit',
    name: 'EditRegister',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('@/views/admin/EditRegister.vue'),
    meta : 
      {
        requiresAuth : true,
      }
  }
  ,
  {
    path: '/admin/EditMonth',
    name: 'EditMonth',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('@/views/admin/ViewRegistersInMonth.vue'),
    meta : 
      {
        requiresAuth : true,
      }
  }
  ,
  {
    path: '/admin/users',
    name: 'AdminUsers',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('@/views/admin/Users.vue'),
    meta : 
      {
        requiresAuth : true,
      }
  }
  ,
  {
    path: '/admin/usermanikinlungs',
    name: 'UserManikinLungs',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('@/views/admin/ManageUserManikinLungs.vue'),
    meta : 
      {
        requiresAuth : true,
      }
  }
]

const router = new VueRouter({
  //mode: 'history',
  base:process.env.VUE_APP_RootAppLocation,
  routes
})

export default router
