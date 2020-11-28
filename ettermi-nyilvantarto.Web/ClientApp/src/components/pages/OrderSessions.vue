<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Rendelési folyamatok</h3>
        </div>
      </div>
      <div class="row">
        <div class="col-12 content-box">
          <table id="order-sessions-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Első rendelés ideje</th>
                <th scope="col">Asztal / Megrendelő</th>
                <th scope="col">Állapot</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="orderSession in orderSessions" :id="'order-session-'+orderSession.id" class="order-sessions-table-row" :key="orderSession.id" @click="openOrderSession(orderSession.id)">
                <td class="font-weight-normal">{{ moment(orderSession.openedAt).format(App.timeFormat) }}</td>
                <td class="font-weight-normal">{{ (orderSession.customerId > 0) ? orderSession.customerName : orderSession.tableCode }}</td>
                <td class="font-weight-normal"><span v-if="orderSession.status == 'active'" class="badge badge-success">Folyamatban lévő</span><span v-if="orderSession.status == 'delivering'" class="badge badge-info">Kiszállítás alatt</span><span v-if="orderSession.status == 'paid'" class="badge badge-light">Kifizetett</span><span v-if="orderSession.status == 'cancelled'" class="badge badge-info">Törölt</span></td>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              <tr v-if="orderSessions.length==0">
                <td colspan="4">
                  <span class="font-weight-normal">Nincs megjelenítendő rendelési folyamat a rendszerben.</span>
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
    name: 'order-sessions',

    components: {
      'pagination': PaginationComponent
    },

    mounted: function () {
      this.fetchOrderSessions();
    },

    data() {
      return {
        App: global.App,
        moment: moment,

        orderSessions: [],
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
      fetchOrderSessions: function () {
        this.orderSessions = [];

        fetch(global.App.baseURL + `api/orders/page/${this.pagination.currentPage}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            this.orderSessions = res.elements;

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
        this.fetchOrderSessions();
      },

      openOrderSession: function (id) {
        this.$router.push({ path: `/order-session/${id}` });
      }
    }
  }
</script>