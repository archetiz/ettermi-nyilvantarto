import HomeComponent from './components/pages/Home.vue'
import LoginComponent from './components/pages/Login.vue'
import NewOrderSessionComponent from './components/pages/NewOrderSession.vue'
import VouchersComponent from './components/pages/Vouchers.vue'
import FeedbacksComponent from './components/pages/Feedbacks.vue'
import LoyaltyCardBalanceComponent from './components/pages/LoyaltyCardBalance.vue'
import CustomersComponent from './components/pages/Customers.vue'
import MenuComponent from './components/pages/Menu.vue'
import TablesComponent from './components/pages/Tables.vue'
import ReservationsComponent from './components/pages/Reservations.vue'
import PayOrderSessionComponent from './components/pages/PayOrderSession.vue'
import OrdersComponent from './components/pages/Orders.vue'
import OrderSessionsComponent from './components/pages/OrderSessions.vue'
import OrderSessionComponent from './components/pages/OrderSession.vue'
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
  { path: '/tables', component: TablesComponent },
  { path: '/reservations', component: ReservationsComponent },
  { path: '/pay-order-session', component: PayOrderSessionComponent },
  { path: '/orders', component: OrdersComponent },
  { path: '/order-sessions', component: OrderSessionsComponent },
  { path: '/order-session/:order_session_id', props: true, component: OrderSessionComponent },
  { path: '*', component: PageNotFoundPageComponent }
];

export default new VueRouter({
  linkExactActiveClass: 'is-active',
  routes
});