<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Rendelések</h3>
        </div>
      </div>
      <div class="row">
        <div class="col-12 content-box">
          <table id="orders-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Rendelés azon.</th>
                <th scope="col">Rendelés ideje</th>
                <th scope="col">Pincér</th>
                <th scope="col">Állapot</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="order in orders" :id="'order-'+order.id" class="orders-table-row" :key="order.id" @click="openOrder(order.id)">
                <td class="font-weight-bold">#{{ order.id }}</td>
                <td class="font-weight-normal">{{ moment(order.openedAt).format(App.timeFormat) }}</td>
                <td class="font-weight-normal">{{ order.waiterName }}</td>
                <td class="font-weight-normal">
                  <span v-if="order.status == 'Ordering'" class="badge badge-danger">Rendelés folyamatban</span>
                  <span v-if="order.status == 'Ordered'" class="badge" :class="[{'badge-warning': isChef}, {'badge-light': isWaiter || isOwner}]">Megrendelve</span>
                  <span v-if="order.status == 'Preparing'" class="badge badge-info">Elkészítés alatt</span>
                  <span v-if="order.status == 'Prepared'" class="badge" :class="[{'badge-success': isChef}, {'badge-warning': isWaiter || isOwner}]">Elkészült</span>
                  <span v-if="order.status == 'Served'" class="badge badge-light">Felszolgálva</span>
                  <span v-if="order.status == 'Cancelled'" class="badge badge-dark">Törölve</span>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              <tr v-if="orders.length==0">
                <td colspan="5">
                  <span class="font-weight-normal">Nincs megjelenítendő rendelés a rendszerben.</span>
                </td>
              </tr>
            </tbody>
            <caption>
              <pagination :data="pagination.data" @callback="paginationCallback"></pagination>
            </caption>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'

  var moment = require('moment');

  export default {
    name: 'orders',

    components: {
      'pagination': PaginationComponent
    },

    mounted: function () {
      this.fetchOrders();
    },

    data() {
      return {
        App: global.App,
        moment: moment,

        orders: [],
        pagination: {
          currentPage: 1,
          data: {
            current_page: 1,
            last_page: 1,
            prev_page_url: null,
            next_page_url: null
          }
        }
      }
    },

    methods: {
      fetchOrders: function () {
        this.orders = [];

        fetch(global.App.baseURL + `api/order/page/${this.pagination.currentPage}` + ((global.App.user.accountType == 'Chef') ? '?statuses=Ordered&statuses=Preparing&statuses=Prepared&statuses=Cancelled' : ''), {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            this.orders = res.elements;

            this.pagination.currentPage = res.currentPage;
            this.pagination.data = {
              current_page: res.currentPage,
              last_page: res.totalPages,
              prev_page_url: (res.currentPage > 1) ? (res.currentPage - 1) : null,
              next_page_url: (res.currentPage < res.totalPages) ? (res.currentPage + 1) : null
            };
          })
          .catch(err => global.console.log(err));
      },

      paginationCallback: function (url) {
        this.pagination.currentPage = url;
        this.fetchOrders();
      },

      openOrder: function (id) {
        this.$router.push({ path: `/order/${id}` });
      }
    }
  }
</script>