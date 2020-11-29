<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Rendelésrészletező</h3>
        </div>
      </div>
      <div class="row content-box">
        <div class="col-12 col-lg-2 mb-2 mb-lg-0">
          <div v-if="!is_new">
            <button type="button" class="btn btn-secondary d-none d-lg-block" v-on:click="goBack">Vissza</button>
            <button type="button" class="btn btn-secondary btn-block d-lg-none" v-on:click="goBack">Vissza</button>
          </div>
        </div>
        <div v-if="!isChef && order.status != 'Cancelled'" class="col-12 col-lg-10">
          <div class="d-none d-lg-block text-right">
            <button type="button" class="btn btn-danger" v-on:click="cancelOrder">Törlés</button>
          </div>
          <button type="button" class="btn btn-danger btn-block d-lg-none" v-on:click="cancelOrder">Törlés</button>
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
            <h4>Rendelés adatai</h4>
          </div>
        </div>
      </div>
      <div v-if="isLoaded && error_not_found">
        <div class="row">
          <div class="col-12 content-box">
            <div role="alert" class="alert alert-danger">A keresett rendelés nem szerepel a rendszerben. Kérlek válassz másikat!</div>
          </div>
        </div>
      </div>
      <div v-if="isLoaded && !error_not_found">
        <div class="row">
          <div class="col-12 col-lg-6 content-box">
            <div class="row">
              <div class="col-6">
                <span class="font-weight-bold">Rendelés azonosítója</span>
              </div>
              <div class="col-6">
                <h5>#{{ order.id }}</h5>
              </div>
            </div>
            <div v-if="!isChef && order.tableId" class="row">
              <div class="col-6">
                <span class="font-weight-bold">Asztal</span>
              </div>
              <div class="col-6">
                {{ order.tableCode }}
              </div>
            </div>
            <div v-if="!isChef && !order.tableId">
              <div class="row">
                <div class="col-6">
                  <span class="font-weight-bold">Megrendelő neve</span>
                </div>
                <div class="col-6">
                  {{ order.customerName }}
                </div>
              </div>
              <div class="row">
                <div class="col-6">
                  <span class="font-weight-bold">Telefonszám</span>
                </div>
                <div class="col-6">
                  {{ order.customerPhoneNumber }}
                </div>
              </div>
              <div class="row">
                <div class="col-6">
                  <span class="font-weight-bold">Cím</span>
                </div>
                <div class="col-6">
                  {{ order.customerAddress }}
                </div>
              </div>
            </div>
          </div>
          <div class="col-12 col-lg-6 content-box">
            <div class="row">
              <div class="col-6">
                <span class="font-weight-bold">Rendelés megkezdésének időpontja</span>
              </div>
              <div class="col-6">
                {{ moment(order.openedAt).format(App.timeFormat) }}
              </div>
            </div>
            <div v-if="order.closedAt" class="row">
              <div class="col-6">
                <span class="font-weight-bold">Rendelés lezárásának időpontja</span>
              </div>
              <div v-if="order.closedAt" class="col-6">
                {{ moment(order.closedAt).format(App.timeFormat) }}
              </div>
              <div v-else class="col-6">-</div>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-12 col-lg-6 content-box">
            <div class="row content-box">
              <div class="col-6">
                <span class="font-weight-bold">Pincér neve</span>
              </div>
              <div class="col-6">
                {{ order.waiterName }}
              </div>
            </div>
            <div class="row content-box">
              <div class="col-6">
                <span class="font-weight-bold">Rendelés állapota</span>
              </div>
              <div class="col-6">
                <span v-if="order.status == 'Ordering'" class="badge badge-danger">Rendelés folyamatban</span>
                <span v-if="order.status == 'Ordered'" class="badge" :class="[{'badge-warning': isChef}, {'badge-light': isWaiter || isOwner}]">Megrendelve</span>
                <span v-if="order.status == 'Preparing'" class="badge badge-info">Elkészítés alatt</span>
                <span v-if="order.status == 'Prepared'" class="badge" :class="[{'badge-success': isChef}, {'badge-warning': isWaiter || isOwner}]">Elkészült</span>
                <span v-if="order.status == 'Served'" class="badge badge-light">Felszolgálva</span>
                <span v-if="order.status == 'Cancelled'" class="badge badge-dark">Törölve</span>
              </div>
            </div>
          </div>
        </div>
        <div v-if="(isChef && ['Ordered', 'Preparing', 'Prepared'].indexOf(order.status) != -1) || (!isChef && order.status != 'Cancelled')" class="row">
          <div class="col-12 col-lg-3 content-box">
            <span class="font-weight-bold">Állapot módosítása</span>
          </div>
          <div class="col-12 col-lg-9 content-box">
            <div class="btn-group btn-group-toggle order-status-btn-group" data-toggle="buttons">
              <label v-if="!isChef && order.status != 'Ordering'" class="btn btn-danger">
                <input @click="changeStatus('Ordering')" type="radio" name="order-status" id="order-status-Ordering" value="Ordering"> Rendelés folyamatban
              </label>
              <label v-if="order.status != 'Ordered'" class="btn" :class="[{'badge-warning': isChef}, {'badge-light': isWaiter || isOwner}]">
                <input @click="changeStatus('Ordered')" type="radio" name="order-status" id="order-status-Ordered" value="Ordered"> Megrendelve
              </label>
              <label v-if="order.status != 'Preparing'" class="btn btn-info">
                <input @click="changeStatus('Preparing')" type="radio" name="order-status" id="order-status-Preparing" value="Preparing"> Elkészítés alatt
              </label>
              <label v-if="order.status != 'Prepared'" class="btn" :class="[{'badge-success': isChef}, {'badge-warning': isWaiter || isOwner}]">
                <input @click="changeStatus('Prepared')" type="radio" name="order-status" id="order-status-Prepared" value="Prepared"> Elkészült
              </label>
              <label v-if="!isChef && order.status != 'Served'" class="btn btn-light">
                <input @click="changeStatus('Served')" type="radio" name="order-status" id="order-status-Served" value="Served"> Felszolgálva
              </label>
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
            <nav v-if="!isChef && ['Ordering', 'Ordered'].indexOf(order.status) != -1" class="navbar navbar-expand navbar-light bg-light">
              <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <ul class="navbar-nav mr-auto">
                  <li class="nav-item form-check"></li>
                </ul>
                <ul class="navbar-nav">
                  <li class="nav-item">
                    <button type="button" class="btn btn-outline-success btn-sm" @click="addNewOrderItem">Hozzáad</button>
                  </li>
                </ul>
              </div>
            </nav>
            <table id="order-items-table" class="table table-hover table-clickable">
              <thead>
                <tr>
                  <th scope="col">Megnevezés</th>
                  <th v-if="!isChef" scope="col">Ár</th>
                  <th scope="col">Mennyiség</th>
                  <th scope="col">Komment</th>
                  <th v-if="!isChef"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in order.items" :id="'order-item-'+item.orderItemId" class="order-items-table-row" :key="item.orderItemId" @click="editOrderItem(item.orderItemId)">
                  <td class="font-weight-normal">{{ item.name }}</td>
                  <td v-if="!isChef" class="font-weight-normal">{{ formatMoney(item.price, 0, ',', '.') }} Ft</td>
                  <td class="font-weight-normal">{{ item.quantity }}</td>
                  <td class="font-weight-normal">{{ item.comment }}</td>
                  <td v-if="!isChef" class="text-right">
                    <ion-icon name="play"></ion-icon>
                  </td>
                </tr>
                <tr v-if="order.items.length==0">
                  <td v-if="isChef" colspan="3">
                    <span class="font-weight-normal">Nincs megjelenítendő elem.</span>
                  </td>
                  <td v-else colspan="5">
                    <span class="font-weight-normal">Nincs megjelenítendő elem.</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div v-if="!isChef" class="row content-box">
          <div class="col-12 col-lg-8">
            <h5>Rendelések összesen</h5>
          </div>
          <div class="col-12 col-lg-4 d-none d-lg-block text-right">
            <h5>{{ formatMoney(order.price, 0, ',', '.') }} Ft</h5>
          </div>
          <div class="col-12 col-lg-4 d-lg-none">
            <h5>{{ formatMoney(order.price, 0, ',', '.') }} Ft</h5>
          </div>
        </div>
      </div>
    </div>

    <order-item-details-modal v-if="!isChef" id="order-item-details-modal" :options="orderItemDetailsModalOptions" @confirm-callback="orderItemDetailsModalConfirmCallback" @dismiss-callback="orderItemDetailsModalDismissCallback" @delete-callback="orderItemDetailsModalDeleteCallback"></order-item-details-modal>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'
  import LoadingSpinnerComponent from './../LoadingSpinner.vue'
  import OrderItemDetailsModalComponent from './../OrderItemDetailsModal.vue'

  var moment = require('moment');

  export default {
    name: 'order',

    components: {
      'pagination': PaginationComponent,
      'loading-spinner': LoadingSpinnerComponent,
      'order-item-details-modal': OrderItemDetailsModalComponent
    },

    props: {
      order_id: {
        type: String,
        required: true
      },
      is_new: {
        type: Boolean,
        default: false
      },
    },

    data() {
      return {
        App: global.App,
        moment: moment,
        formatMoney: global.formatMoney,

        loading: true,
        error_not_found: false,
        order: {},

        orderItemDetailsModalOptions: {
          isHidden: true,
          orderItem: {},
          apiError: ''
        }
      }
    },

    computed: {
      isLoaded: function () {
        return !this.loading && this.order.id;
      },
      isOwner: function () {
        return this.App.user.accountType == 'Owner';
      },
      isWaiter: function () {
        return this.App.user.accountType == 'Waiter';
      },
      isChef: function () {
        return this.App.user.accountType == 'Chef';
      }
    },

    mounted: function () {
      this.fetchOrder();
    },

    methods: {
      fetchOrder: function () {
            this.loading = true;
            this.error_not_found = false;

        fetch(global.App.baseURL + `api/order/${this.order_id}`, {
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
              this.order = res;
            });
          })
          .catch(err => global.console.log(err));
      },

      changeStatus: function (status) {
        fetch(global.App.baseURL + `api/order/${this.order.id}`, {
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
              this.fetchOrder();
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

      cancelOrder: function () {
        if (this.isChef) { return; }

        fetch(global.App.baseURL + `api/order/${this.order.id}`, {
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
              message: 'A rendelést sikeresen töröltük.'
            }, {
              type: 'success',
            });

            this.fetchOrder();
          })
          .catch(err => console.log(err));
      },

      goBack: function () {
        window.history.back();
      },

      addNewOrderItem: function () {
        if (this.isChef) { return; }

        this.orderItemDetailsModalOptions.orderItem = {"orderId": this.order.id};
        this.orderItemDetailsModalOptions.isHidden = false;
      },

      editOrderItem: function (id) {
        if (this.isChef || ['Ordering', 'Ordered'].indexOf(this.order.status) == -1) { return; }

        const myID = id;
        const vm = this;
        this.orderItemDetailsModalOptions.orderItem = {};

        this.order.items.forEach(function (e) {
          if (e.orderItemId == myID) {
            vm.orderItemDetailsModalOptions.orderItem = {
              "orderId": vm.order.id,
              "orderItemId": e.orderItemId,
              "menuItemId": e.menuItemId,
              "quantity": e.quantity,
              "comment": e.comment
            };
          }
        });

        this.orderItemDetailsModalOptions.isHidden = false;
      },

      orderItemDetailsModalConfirmCallback: function (data) {
        let constData = data;
        fetch(global.App.baseURL + ((data.orderItemId) ? `api/order/${this.order.id}/item/${data.orderItemId}` : `api/order/${this.order.id}/add`), {
            method: (data.orderItemId) ? 'put' : 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify(data)
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => {
            if (res === undefined) { return; }

            if (constData.orderItemId && res.status == 200) {
              this.orderItemDetailsModalConfirmCallbackSuccess();
              return;
            }

            res.json().then(res => {
              if (res.resultError !== undefined) {
                this.orderItemDetailsModalOptions.apiError = res.resultError;
                return;
              }

              this.orderItemDetailsModalConfirmCallbackSuccess();
            });
          })
          .catch(err => console.log(err));
      },

      orderItemDetailsModalConfirmCallbackSuccess: function () {
        // hide modal
        this.orderItemDetailsModalOptions.isHidden = true;
        this.orderItemDetailsModalOptions.orderItem = {};

        // create notification
        global.jQuery.notify({
          message: 'Az ételt/italt adatait sikeresen elmentettük.'
        }, {
          type: 'success',
        });

        this.fetchOrder();
      },

      orderItemDetailsModalDismissCallback: function () {
        this.orderItemDetailsModalOptions.isHidden = true;
      },

      orderItemDetailsModalDeleteCallback: function (id) {
        fetch(global.App.baseURL + `api/order/${this.order.id}/item/${id}`, {
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
              this.orderItemDetailsModalOptions.apiError = "Ismeretlen hiba.";
            }

            // hide modal
            this.orderItemDetailsModalOptions.isHidden = true;
            this.orderItemDetailsModalOptions.orderItem = {};

            // create notification
            global.jQuery.notify({
              message: 'Az ételt/italt sikeresen töröltük.'
            }, {
              type: 'success',
            });

            this.fetchOrder();
          })
          .catch(err => console.log(err));
      }
    },

    watch: {
      order_id: function () {
        this.loading = false;
        this.fetchOrder();
      }
    },
  }
</script>

<style>
  .order-status-btn-group .btn {
    font-size: 10pt;
  }
</style>