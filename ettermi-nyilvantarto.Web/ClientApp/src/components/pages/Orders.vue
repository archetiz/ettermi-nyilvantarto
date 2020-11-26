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
                <th scope="col">Rendelés ideje</th>
                <th scope="col">Asztal</th>
                <th scope="col">Pincér</th>
                <th scope="col">Állapot</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="order in orders" :id="'order-'+order.id" class="orders-table-row" :key="order.id" @click="openOrder(order.id)">
                <td class="font-weight-normal">{{ moment(order.openedAt).format(App.timeFormat) }}</td>
                <td class="font-weight-normal">{{ order.tableCode }}</td>
                <td class="font-weight-normal">{{ order.waiterName }}</td>
                <td class="font-weight-normal"><span v-if="order.status == 'ordered'" class="badge badge-warning">Megrendelve</span><span v-if="order.status == 'preparing'" class="badge badge-info">Elkészítés alatt</span></td>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              <tr v-if="orders.length==0">
                <td colspan="4">
                  <span class="font-weight-normal">Nincs megjelenítendő rendelés a rendszerben.</span>
                </td>
              </tr>
            </tbody>
            <caption>
              <pagination :data="pagination" @callback="paginationCallback"></pagination>
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

        fetch(global.App.baseURL + `api/order/page/${this.pagination.currentPage}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError === undefined) {
              //this.orders = res;
              this.orders = [
                {
                  "id": 0,
                  "tableCode": 'A',
                  "waiterId": 0,
                  "waiterName": 'asd',
                  "status": 'ordered',
                  "openedAt": "2020-11-25T21:44:02.575Z",
                  "closedAt": "2020-11-25T21:44:02.575Z"
                },
                {
                  "id": 0,
                  "tableCode": 'A',
                  "waiterId": 0,
                  "waiterName": 'asd',
                  "status": 'preparing',
                  "openedAt": "2020-11-25T21:44:02.575Z",
                  "closedAt": "2020-11-25T21:44:02.575Z"
                }
              ]; //   ordered, preparing, prepared, served, cancelled

              /*let pagination = {
                current_page: data.current_page,
                last_page: data.last_page,
                prev_page_url: data.prev_page_url,
                next_page_url: data.next_page_url
              };

              this.pagination = pagination;*/

              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült betölteni a rendeléseket.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      },

      paginationCallback: function (url) {

      },

      openOrder: function (id) {
        this.$router.push({ path: `/order/${id}` });
      }
    }
  }
</script>