import HomeComponent from './components/pages/Home.vue'
import LoginComponent from './components/pages/Login.vue'
import NewOrderSessionComponent from './components/pages/NewOrderSession.vue'
import VouchersComponent from './components/pages/Vouchers.vue'
import FeedbacksComponent from './components/pages/Feedbacks.vue'
import LoyaltyCardBalanceComponent from './components/pages/LoyaltyCardBalance.vue'
import CustomersComponent from './components/pages/Customers.vue'
import MenuComponent from './components/pages/Menu.vue'
import PageNotFoundPageComponent from './components/pages/PageNotFound.vue'

import VueRouter from 'vue-router';

let routes=[
  { path: '/', component: HomeComponent },
  { path: '/login', component: LoginComponent },
  { path: '/new-order-session', component: NewOrderSessionComponent },
  { path: '/vouchers', component: VouchersComponent },
  { path: '/feedbacks', component: FeedbacksComponent },
  { path: '/loyalty-card-balance', component: LoyaltyCardBalanceComponent },
  { path: '/customers', component: CustomersComponent },
  { path: '/menu', component: MenuComponent },
  { path: '*', component: PageNotFoundPageComponent }
];

export default new VueRouter({
  linkExactActiveClass: 'is-active',
  routes
});