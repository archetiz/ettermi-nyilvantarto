<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Rendelési folyamat részletező</h3>
        </div>
      </div>
      <div class="row content-box">
        <div class="col-12 col-lg-2 mb-2 mb-lg-0">
          <button type="button" class="btn btn-secondary d-none d-lg-block" v-on:click="goBack">Vissza</button>
          <button type="button" class="btn btn-secondary btn-block d-lg-none" v-on:click="goBack">Vissza</button>
        </div>
        <div v-if="orderSession.status != 'cancelled'" class="col-12 col-lg-10">
          <div class="d-none d-lg-block text-right">
            <button type="button" class="btn btn-danger" v-on:click="cancelOrderSession">Törlés</button>
          </div>
          <button type="button" class="btn btn-danger btn-block d-lg-none" v-on:click="cancelOrderSession">Törlés</button>
        </div>
      </div>

      <div v-if="!isLoaded" class="row">
        <div class="col-12 content-box">
          <loading-spinner></loading-spinner>
        </div>
      </div>
      <div v-if="isLoaded">
        <div class="row">
          <div class="col-12 content-box">
            <h4>Rendelési folyamat adatai</h4>
          </div>
        </div>
        <div class="row">
          <div class="col-12 col-lg-6 content-box">
            <div v-if="orderSession.tableId != 0" class="row">
              <div class="col-6">
                <span class="font-weight-bold">Asztal</span>
              </div>
              <div class="col-6">
                {{ orderSession.tableCode }}
              </div>
            </div>
            <div v-if="orderSession.tableId == 0">
              <div class="row">
                <div class="col-6">
                  <span class="font-weight-bold">Megrendelő neve</span>
                </div>
                <div class="col-6">
                  {{ orderSession.customerName }}
                </div>
              </div>
              <div class="row">
                <div class="col-6">
                  <span class="font-weight-bold">Telefonszám</span>
                </div>
                <div class="col-6">
                  {{ orderSession.customerPhoneNumber }}
                </div>
              </div>
              <div class="row">
                <div class="col-6">
                  <span class="font-weight-bold">Cím</span>
                </div>
                <div class="col-6">
                  {{ orderSession.customerAddress }}
                </div>
              </div>
            </div>
          </div>
          <div class="col-12 col-lg-6 content-box">
            <div class="row">
              <div class="col-6">
                <span class="font-weight-bold">Első rendelés időpontja</span>
              </div>
              <div class="col-6">
                {{ moment(orderSession.openedAt).format(App.timeFormat) }}
              </div>
            </div>
            <div class="row">
              <div class="col-6">
                <span class="font-weight-bold">Folyamat lezárásának időpontja</span>
              </div>
              <div v-if="orderSession.closedAt" class="col-6">
                {{ moment(orderSession.closedAt).format(App.timeFormat) }}
              </div>
              <div v-else class="col-6">-</div>
            </div>
          </div>
          </div>
        <div class="row">
          <div class="col-12">
            <div class="row">
              <div class="col-6 content-box">
                <span class="font-weight-bold">Folyamat állapota</span>
              </div>
              <div class="col-6 content-box">
                <span v-if="orderSession.status == 'active'" class="badge badge-danger">Aktív</span><span v-if="orderSession.status == 'delivering'" class="badge badge-info">Kiszállítás alatt</span><span v-if="orderSession.status == 'paid'" class="badge badge-success">Kifizetve</span><span v-if="orderSession.status == 'cancelled'" class="badge badge-dark">Törölve</span>
              </div>
            </div>
            <div v-if="orderSession.status == 'active' || orderSession.status == 'delivering'" class="row">
              <div class="col-6 content-box">
                <span class="font-weight-bold">Állapot módosítása</span>
              </div>
              <div class="col-6 content-box">
                <div class="btn-group btn-group-toggle" data-toggle="buttons">
                  <label v-if="orderSession.status != 'active'" class="btn btn-danger">
                    <input @click="changeStatus('active')" type="radio" name="order-session-status" id="order-session-status-active" value="active"> Aktív
                  </label>
                  <label v-if="orderSession.status != 'delivering'" class="btn btn-info">
                    <input @click="changeStatus('delivering')" type="radio" name="order-session-status" id="order-session-status-delivering" value="delivering"> Kiszállítás alatt
                  </label>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col-12 content-box">
            <h4>Rendelések</h4>
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
                  <th scope="col">Összeg</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="order in orderSession.orders" :id="'order-'+order.id" class="orders-table-row" :key="order.id" @click="openOrder(order.id)">
                  <td class="font-weight-normal">{{ moment(order.openedAt).format(App.timeFormat) }}</td>
                  <td class="font-weight-normal">{{ order.tableCode }}</td>
                  <td class="font-weight-normal">{{ order.waiterName }}</td>
                  <td class="font-weight-normal"><span v-if="order.status == 'ordered'" class="badge badge-info">Megrendelve</span><span v-if="order.status == 'preparing'" class="badge badge-light">Elkészítés alatt</span><span v-if="order.status == 'prepared'" class="badge badge-warning">Elkészült</span><span v-if="order.status == 'served'" class="badge badge-success">Felszolgálva</span><span v-if="order.status == 'cancelled'" class="badge badge-dark">Törölve</span></td>
                  <td class="font-weight-normal">{{ formatMoney(order.sum, 0, ',', '.') }} Ft</td>
                  <td class="text-right">
                    <ion-icon name="play"></ion-icon>
                  </td>
                </tr>
                <tr v-if="orderSession.orders.length==0">
                  <td colspan="4">
                    <span class="font-weight-normal">Nincs megjelenítendő rendelés.</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="row content-box">
          <div class="col-12 col-lg-8">
            <h5>Rendelések összesen</h5>
          </div>
          <div class="col-12 col-lg-4 d-none d-lg-block text-right">
            <h5>{{ formatMoney(orderSession.sum, 0, ',', '.') }} Ft</h5>
          </div>
          <div class="col-12 col-lg-4 d-lg-none">
            <h5>{{ formatMoney(orderSession.sum, 0, ',', '.') }} Ft</h5>
          </div>
        </div>

        <div class="row">
          <div class="col-12 content-box">
            <h4>Kupon</h4>
          </div>
        </div>
        <div v-if="!orderSession.voucherId" class="row content-box form-row">
          <div class="form-group col-12 col-lg-4">
            <input type="text" class="form-control" v-model="voucherCode" required>
          </div>
          <div class="form-group col-12 col-lg-2">
            <button type="button" class="btn btn-primary btn-block" v-on:click="applyVoucher">OK</button>
          </div>
        </div>
        <div v-if="orderSession.voucherId" class="row content-box">
          <div class="col-12">
            <span class="font-weight-bold">Kuponkód</span>
          </div>
          <div class="col-12">
            {{ orderSession.voucherCode }}
          </div>
          <div class="col-12">
            <span class="font-weight-bold">Kedvezmény mértéke</span>
          </div>
          <div v-if="orderSession.voucherDiscountPercentage > 0" class="col-12">
            {{ orderSession.voucherDiscountPercentage }}%
          </div>
          <div v-if="orderSession.voucherDiscountAmount > 0" class="col-12">
            {{ formatMoney(orderSession.voucherDiscountAmount, 0, ',', '.') }} Ft
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'
  import LoadingSpinnerComponent from './../LoadingSpinner.vue'

  var moment = require('moment');

  export default {
    name: 'order-session',

    components: {
      'pagination': PaginationComponent,
      'loading-spinner': LoadingSpinnerComponent
    },

    props: {
      order_session_id: {
        type: String,
        required: true
      },
    },

    data() {
      return {
        App: global.App,
        moment: moment,
        formatMoney: global.formatMoney,

        loading: true,
        orderSession: {}
      }
    },

    computed: {
      isLoaded: function () {
        return !this.loading && this.orderSession.id;
      }
    },

    mounted: function () {
      this.fetchOrderSession();
    },

    methods: {
      fetchOrderSession: function () {
            this.loading = false;

              this.orderSession = {
                "id": 1,
                "tableId": 2,
                "tableCode": "string",
                "customerId": 0,
                "customerName": "string",
                "customerPhoneNumber": "string",
                "customerAddress": "string",
                "voucherId": 10,
                "voucherCode": "string",
                "voucherDiscountPercentage": 0,
                "voucherDiscountAmount": 1000,
                "invoiceId": 0,
                "status": 'cancelled',
                "openedAt": "2020-11-26T11:39:39.823Z",
                "closedAt": "2020-11-26T11:39:39.823Z",
                "sum": 150000,
                "orders": [
                  {
                    "id": 0,
                    "tableCode": 'A',
                    "waiterId": 0,
                    "waiterName": 'asd',
                    "status": 'ordered',
                    "sum": 15000,
                    "openedAt": "2020-11-26T11:39:39.823Z",
                    "closedAt": "2020-11-26T11:39:39.823Z"
                  },
                  {
                    "id": 1,
                    "tableCode": 'A',
                    "waiterId": 0,
                    "waiterName": 'asd',
                    "status": 'preparing',
                    "sum": 15000,
                    "openedAt": "2020-11-26T11:39:39.823Z",
                    "closedAt": "2020-11-26T11:39:39.823Z"
                  },
                  {
                    "id": 3,
                    "tableCode": 'A',
                    "waiterId": 0,
                    "waiterName": 'asd',
                    "status": 'prepared',
                    "sum": 15000,
                    "openedAt": "2020-11-26T11:39:39.823Z",
                    "closedAt": "2020-11-26T11:39:39.823Z"
                  },
                  {
                    "id": 4,
                    "tableCode": 'A',
                    "waiterId": 0,
                    "waiterName": 'asd',
                    "status": 'served',
                    "sum": 15000,
                    "openedAt": "2020-11-26T11:39:39.823Z",
                    "closedAt": "2020-11-26T11:39:39.823Z"
                  },
                  {
                    "id": 5,
                    "tableCode": 'A',
                    "waiterId": 0,
                    "waiterName": 'asd',
                    "status": 'cancelled',
                    "sum": 15000,
                    "openedAt": "2020-11-26T11:39:39.823Z",
                    "closedAt": "2020-11-26T11:39:39.823Z"
                  }
                ]
              }; //   active, delivering, paid, cancelled 

        /*fetch(global.App.baseURL + `api/orders/${this.order_session_id}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            this.loading = false;
            
            if (res.resultError === undefined) {
              console.log(0);
              //this.orderSession = res;

              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült betölteni a rendelés adatait.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));*/
      },

      changeStatus: function (status) {
        fetch(global.App.baseURL + `api/orders/${this.orderSession.id}`, {
            method: 'put',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: `{"status": "${status}"}`
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              return;
            }

            this.fetchOrderSession();
          })
          .catch(err => console.log(err));
      },

      cancelOrderSession: function () {
        fetch(global.App.baseURL + `api/orders/${this.orderSession.id}`, {
            method: 'delete',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'A rendelési folyamatot sikeresen töröltük.'
            }, {
              type: 'success',
            });

            this.fetchOrderSession();
          })
          .catch(err => console.log(err));
      },

      openOrder: function (id) {
        this.$router.push({ path: `/order/${id}` });
      },

      goBack: function () {
        window.history.back();
      }
    },

    watch: {
      order_session_id: function () {
        this.loading = false;
        this.fetchOrderSession();
      }
    }
  }
</script>