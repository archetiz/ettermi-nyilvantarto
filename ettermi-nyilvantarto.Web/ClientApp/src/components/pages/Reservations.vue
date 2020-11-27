<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Foglalások</h3>
        </div>
      </div>
      <div class="row">
        <div class="col-12 content-box">
          <nav class="navbar navbar-expand navbar-light bg-light">
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
              <ul class="navbar-nav mr-auto">
                <li class="nav-item form-check"></li>
              </ul>
              <ul class="navbar-nav">
                <li class="nav-item">
                  <button type="button" class="btn btn-outline-success btn-sm" @click="addNewReservation">Hozzáad</button>
                </li>
              </ul>
            </div>
          </nav>
          <table id="reservations-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Asztal</th>
                <th scope="col">Foglalás kezdete</th>
                <th scope="col">Foglalás vége</th>
                <th scope="col">Név</th>
                <th scope="col">Telefonszám</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="reservation in reservations" :id="'reservation-'+reservation.id" class="reservations-table-row" :key="reservation.id" @click="editReservation(reservation.id)">
                <td class="font-weight-normal">{{ reservation.tableCode }}</td>
                <td class="font-weight-normal">{{ moment(reservation.timeFrom).format(App.timeFormat) }}</td>
                <td class="font-weight-normal">{{ moment(reservation.timeTo).format(App.timeFormat) }}</td>
                <td class="font-weight-normal">{{ reservation.customerName }}</td>
                <td class="font-weight-normal">{{ reservation.customerPhone }}</td>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              <tr v-if="reservations.length==0">
                <td colspan="6">
                  <span class="font-weight-normal">Nincs foglalás a rendszerben.</span>
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

    <reservation-details-modal id="reservation-details-modal" :options="reservationDetailsModalOptions" @confirm-callback="reservationDetailsModalConfirmCallback" @dismiss-callback="reservationDetailsModalDismissCallback" @delete-callback="reservationDetailsModalDeleteCallback"></reservation-details-modal>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'
  import ReservationDetailsModalComponent from './../ReservationDetailsModal.vue'

  var moment = require('moment');

  export default {
    name: 'reservations',

    components: {
      'pagination': PaginationComponent,
      'reservation-details-modal': ReservationDetailsModalComponent
    },

    mounted: function () {
      this.fetchReservations();
    },

    data() {
      return {
        formatMoney: global.formatMoney,
        moment: moment,
        App: global.App,

        reservations: [],
        pagination: {
          currentPage: 1,
          data: {
            current_page: 1,
            last_page: 1,
            prev_page_url: null,
            next_page_url: null
          }
        },

        reservationDetailsModalOptions: {
          isHidden: true,
          reservation: {},
          apiError: ''
        }
      }
    },

    methods: {
      fetchReservations: function () {
        this.reservations = [];

        fetch(global.App.baseURL + `api/reservation/page/${this.pagination.currentPage}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            this.reservations = res.elements;

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
        this.fetchReservations();
      },

      addNewReservation: function () {
        this.reservationDetailsModalOptions.reservation = {};
        this.reservationDetailsModalOptions.isHidden = false;
      },

      editReservation: function (id) {
        const myID = id;
        const vm = this;
        this.reservationDetailsModalOptions.reservation = {};

        this.reservations.forEach(function (e) {
          if (e.id == myID) {
            vm.reservationDetailsModalOptions.reservation = e;
          }
        });

        this.reservationDetailsModalOptions.isHidden = false;
      },

      reservationDetailsModalConfirmCallback: function (data) {
        let constData = data;
        fetch(global.App.baseURL + ((data.id) ? `api/reservation/${data.id}` : 'api/reservation'), {
            method: (data.id) ? 'put' : 'post',
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

            res.json().then(res => {
              if (res.resultError !== undefined) {
                this.reservationDetailsModalOptions.apiError = res.resultError;
                return;
              }

              // hide modal
              this.reservationDetailsModalOptions.isHidden = true;
              this.reservationDetailsModalOptions.reservation = {};

              // create notification
              global.jQuery.notify({
                message: 'A foglalás adatait sikeresen elmentettük.'
              }, {
                type: 'success',
              });

              this.fetchReservations();
            });
          })
          .catch(err => console.log(err));
      },

      reservationDetailsModalDismissCallback: function () {
        this.reservationDetailsModalOptions.isHidden = true;
      },

      reservationDetailsModalDeleteCallback: function (id) {
        fetch(global.App.baseURL + `api/reservation/${id}`, {
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
              this.reservationDetailsModalOptions.apiError = "Ismeretlen hiba.";
            }

            // hide modal
            this.reservationDetailsModalOptions.isHidden = true;
            this.reservationDetailsModalOptions.reservation = {};

            // create notification
            global.jQuery.notify({
              message: 'A foglalást sikeresen deaktiváltuk.'
            }, {
              type: 'success',
            });

            this.pagination.currentPage = 1;
            this.fetchReservations();
          })
          .catch(err => console.log(err));
      }
    }
  }
</script>

<style>
</style>