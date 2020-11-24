import HomeComponent from './components/pages/Home.vue'
import LoginComponent from './components/pages/Login.vue'
import NewOrderSessionComponent from './components/pages/NewOrderSession.vue'
import VouchersComponent from './components/pages/Vouchers.vue'
import pageNotFoundPageComponent from './components/pages/PageNotFound.vue'

import VueRouter from 'vue-router';

let routes=[
  { path: '/', component: HomeComponent },
  { path: '/login', component: LoginComponent },
  { path: '/new-order-session', component: NewOrderSessionComponent },
  { path: '/vouchers', component: VouchersComponent },
  { path: '*', component: pageNotFoundPageComponent }
];

export default new VueRouter({
  linkExactActiveClass: 'is-active',
  routes
});