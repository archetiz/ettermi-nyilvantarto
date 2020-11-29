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
        <div v-if="['Paid', 'Cancelled'].indexOf(orderSession.status) == -1" class="col-12 col-lg-10">
          <div class="d-none d-lg-block text-right">
            <button type="button" class="btn btn-danger" v-on:click="cancelOrderSession">Törlés</button>
          </div>
          <button type="button" class="btn btn-danger btn-block d-lg-none" v-on:click="cancelOrderSession">Törlés</button>
        </div>
      </div>

      <div v-if="!isLoaded" class="row content-box">
        <div class="col-12">
          <loading-spinner></loading-spinner>
        </div>
      </div>
      <div v-if="isLoaded">
        <div class="row content-box">
          <div class="col-12">
            <h4>Rendelési folyamat adatai</h4>
          </div>
        </div>
      </div>
      <div v-if="isLoaded && error_not_found">
        <div class="row">
          <div class="col-12 content-box">
            <div role="alert" class="alert alert-danger">A keresett rendelési folyamat nem szerepel a rendszerben. Kérlek válassz másikat!</div>
          </div>
        </div>
      </div>
      <div v-if="isLoaded && !error_not_found">
        <div class="row">
          <div class="col-12 col-lg-6 content-box">
            <div class="row">
              <div class="col-6">
                <h5 v-if="orderSession.tableId">{{ orderSession.tableCode }}</h5>
                <h5 v-if="!orderSession.tableId">Elvitelre</h5>
              </div>
            </div>
            <div class="row">
              <div class="col-6">
                <span class="font-weight-bold">Asztal</span>
              </div>
              <div class="col-6">
                <h5 v-if="orderSession.tableId">{{ orderSession.tableCode }}</h5>
                <h5 v-if="!orderSession.tableId">Elvitelre</h5>
              </div>
            </div>
            <div v-if="!orderSession.tableId">
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
        <div class="row content-box">
          <div class="col-12 col-lg-6">
            <div class="row">
              <div class="col-6">
                <span class="font-weight-bold">Folyamat állapota</span>
              </div>
              <div class="col-6">
                <span v-if="orderSession.status == 'Active'" class="badge badge-danger">Aktív</span>
                <span v-if="orderSession.status == 'Delivering'" class="badge badge-info">Kiszállítás alatt</span>
                <span v-if="orderSession.status == 'Paid'" class="badge badge-success">Kifizetve</span>
                <span v-if="orderSession.status == 'Cancelled'" class="badge badge-dark">Törölve</span>
              </div>
            </div>
          </div>
        </div>
        <div v-if="orderSession.status == 'Active' || orderSession.status == 'Delivering'" class="row content-box">
          <div class="col-12 col-lg-3">
            <span class="font-weight-bold">Állapot módosítása</span>
          </div>
          <div class="col-12 col-lg-9">
            <div class="btn-group btn-group-toggle order-session-status-btn-group" data-toggle="buttons">
              <label v-if="orderSession.status != 'Active'" class="btn btn-danger">
                <input @click="changeStatus('Active')" type="radio" name="order-session-status" id="order-session-status-Active" value="Active"> Aktív
              </label>
              <label v-if="orderSession.status != 'Delivering'" class="btn btn-info">
                <input @click="changeStatus('Delivering')" type="radio" name="order-session-status" id="order-session-status-Delivering" value="Delivering"> Kiszállítás alatt
              </label>
            </div>
          </div>
        </div>

        <div class="row content-box">
          <div class="col-12">
            <h4>Rendelések</h4>
          </div>
        </div>
        <div class="row content-box">
          <div class="col-12">
            <table id="orders-table" class="table table-hover table-clickable">
              <thead>
                <tr>
                  <th scope="col">Rendelés ideje</th>
                  <th scope="col">Pincér</th>
                  <th scope="col">Állapot</th>
                  <th scope="col">Összeg</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="order in orderSession.orders" :id="'order-'+order.id" class="orders-table-row" :key="order.id" @click="openOrder(order.id)">
                  <td class="font-weight-normal">{{ moment(order.openedAt).format(App.timeFormat) }}</td>
                  <td class="font-weight-normal">{{ order.waiterName }}</td>
                  <td class="font-weight-normal">
                    <span v-if="order.status == 'Ordering'" class="badge badge-danger">Rendelés folyamatban</span>
                    <span v-if="order.status == 'Ordered'" class="badge" :class="[{'badge-warning': isChef}, {'badge-light': isWaiter || isOwner}]">Megrendelve</span>
                    <span v-if="order.status == 'Preparing'" class="badge badge-info">Elkészítés alatt</span>
                    <span v-if="order.status == 'Prepared'" class="badge" :class="[{'badge-success': isChef}, {'badge-warning': isWaiter || isOwner}]">Elkészült</span>
                    <span v-if="order.status == 'Served'" class="badge badge-light">Felszolgálva</span>
                    <span v-if="order.status == 'Cancelled'" class="badge badge-dark">Törölve</span>
                  <td class="font-weight-normal">{{ formatMoney(order.price, 0, ',', '.') }} Ft</td>
                  <td class="text-right">
                    <ion-icon name="play"></ion-icon>
                  </td>
                </tr>
                <tr v-if="orderSession.orders.length==0">
                  <td colspan="5">
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
            <h5>{{ formatMoney(orderSession.fullPrice, 0, ',', '.') }} Ft</h5>
          </div>
          <div class="col-12 col-lg-4 d-lg-none">
            <h5>{{ formatMoney(orderSession.fullPrice, 0, ',', '.') }} Ft</h5>
          </div>
        </div>

        <div v-if="orderSession.voucherId" class="row content-box">
          <div class="col-12 col-lg-6">
            <div class="row">
              <div class="col-12">
                <h4>Kupon</h4>
              </div>
            </div>

            <div class="row">
              <div class="col-12">
                <div class="row">
                  <div class="col-6">
                    <span class="font-weight-bold">Kuponkód</span>
                  </div>
                  <div class="col-6">
                    {{ orderSession.voucherCode }}
                  </div>
                </div>
                <div class="row">
                  <div class="col-6">
                    <span class="font-weight-bold">Kedvezmény mértéke</span>
                  </div>
                  <div v-if="orderSession.voucherDiscountPercentage > 0" class="col-6">
                    {{ orderSession.voucherDiscountPercentage }}%
                  </div>
                  <div v-if="orderSession.voucherDiscountAmount > 0" class="col-6">
                    {{ formatMoney(orderSession.voucherDiscountAmount, 0, ',', '.') }} Ft
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div v-if="needToPay" class="row pb-3">
          <div class="col-12 content-box">
            <button type="button" class="btn btn-primary d-none d-lg-inline-block" v-on:click="payOrder">Fizetés</button>
            <button type="button" class="btn btn-primary btn-block d-lg-none" v-on:click="payOrder">Fizetés</button>
          </div>
        </div>

        <div v-if="orderSession.status == 'Paid'" class="row pb-3">
          <div class="col-12 text-right">
            <button type="button" class="btn btn-primary d-none d-lg-inline-block" v-on:click="addFeedback">Visszajelzés rögzítése</button>
            <button type="button" class="btn btn-primary btn-block d-lg-none" v-on:click="addFeedback">Visszajelzés rögzítése</button>
          </div>
        </div>

        <div v-if="orderSession.status == 'Paid' && orderSession.invoiceId" class="row pb-3">
          <div class="col-12 text-right">
            <button type="button" class="btn btn-primary d-none d-lg-inline-block" v-on:click="getInvoice">Számla letöltése</button>
            <button type="button" class="btn btn-primary btn-block d-lg-none" v-on:click="getInvoice">Számla letöltése</button>
          </div>
        </div>
      </div>
    </div>

    <feedback-modal id="feedback-modal" :options="feedbackModalOptions" @success-callback="feedbackModalSuccessCallback" @dismiss-callback="feedbackModalDismissCallback"></feedback-modal>

    <pay-order-modal id="pay-order-modal" :options="payOrderModalOptions" @success-callback="payOrderModalSuccessCallback" @dismiss-callback="payOrderModalDismissCallback"></pay-order-modal>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'
  import LoadingSpinnerComponent from './../LoadingSpinner.vue'
  import FeedbackModalComponent from './../FeedbackModal.vue'
  import PayOrderModalComponent from './../PayOrderModal.vue'

  var moment = require('moment');

  export default {
    name: 'order-session',

    components: {
      'pagination': PaginationComponent,
      'loading-spinner': LoadingSpinnerComponent,
      'feedback-modal': FeedbackModalComponent,
      'pay-order-modal': PayOrderModalComponent
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
        error_not_found: false,
        orderSession: {},

        voucherCode: '',
        loyaltyCardNumber: '',

        feedbackModalOptions: {
          isHidden: true,
          orderSessionId: 0
        },

        payOrderModalOptions: {
          isHidden: true,
          orderSessionId: 0
        }
      }
    },

    computed: {
      isLoaded: function () {
        return !this.loading && this.orderSession.id;
      },
      needToPay: function () {
        return ['Paid', 'Cancelled'].indexOf(this.orderSession.status) == -1;
      },
      isOwner: function () {
        return global.App.user.accountType == 'Owner';
      },
      isWaiter: function () {
        return global.App.user.accountType == 'Waiter';
      },
      isChef: function () {
        return global.App.user.accountType == 'Chef';
      }
    },

    mounted: function () {
      this.fetchOrderSession();
    },

    methods: {
      fetchOrderSession: function () {
        this.loading = true;
        this.error_not_found = false;

        this.feedbackModalOptions.orderSessionId = this.order_session_id * 1;
        this.payOrderModalOptions.orderSessionId = this.order_session_id * 1;

        fetch(global.App.baseURL + `api/orders/${this.order_session_id}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => {
            this.loading = false;

            if (res === undefined) { return; }

            if (res.status != 200) {
              this.error_not_found = true;

              return;
            }

            res.json().then(res => {
              this.orderSession = res;
            });
          })
          .catch(err => global.console.log(err));
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
          .then(res => global.handleNetworkError(res, this))
          .then(res => {
            if (res === undefined) { return; }

            if (res.status == 200) {
              this.fetchOrderSession();
              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Hiba lépett fel az állapot módosítása közben.'
            }, {
              type: 'danger',
            });
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
          .then(res => global.handleNetworkError(res, this))
          .then(res => {
            if (res.status != 200) {
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

      addFeedback: function () {
        this.feedbackModalOptions.isHidden = false;
      },

      feedbackModalSuccessCallback: function() {
        this.feedbackModalOptions.isHidden = true;
        this.fetchOrderSession();
      },
      feedbackModalDismissCallback: function() {
        this.feedbackModalOptions.isHidden = true;
      },

      payOrder: function () {
        this.payOrderModalOptions.isHidden = false;
      },

      payOrderModalSuccessCallback: function() {
        this.payOrderModalOptions.isHidden = true;
        this.fetchOrderSession();
      },
      payOrderModalDismissCallback: function() {
        this.payOrderModalOptions.isHidden = true;
      },

      getInvoice: function () {
        if (this.orderSession.invoiceId > 0) {
          window.open(global.App.baseURL + `api/invoice/${this.orderSession.invoiceId}`);
        }
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

<style>
  .order-session-status-btn-group .btn {
    font-size: 10pt;
  }
</style>