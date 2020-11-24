<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12">
          <h3 class="text-center mb-3">Kuponok</h3>
        </div>
      </div>
      <div class="row">
        <div class="col-12">
          <nav class="navbar navbar-expand navbar-light bg-light">
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
              <ul class="navbar-nav mr-auto">
                <li class="nav-item form-check"></li>
              </ul>
              <ul class="navbar-nav">
                <li class="nav-item">
                  <button type="button" class="btn btn-outline-success btn-sm" @click="addNewVoucher">Hozzáad</button>
                </li>
              </ul>
            </div>
          </nav>
          <table id="vouchers-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Kód</th>
                <th scope="col">Minimum értékhatár</th>
                <th scope="col">Kedvezmény</th>
                <th scope="col">Aktív időszak kezdete</th>
                <th scope="col">Aktív időszak vége</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="voucher in vouchers" :id="'voucher-'+voucher.id" class="vouchers-table-row" :key="voucher.id" @click="editVoucher(voucher.id)">
                <td class="font-weight-normal">{{ voucher.code }}</td>
                <td class="font-weight-normal">{{ formatMoney(voucher.discountThreshold, 0, ',', '.') }}</td>
                <td v-if="voucher.discountPercentage > 0" class="font-weight-normal">{{ voucher.discountPercentage }}%</td>
                <td v-else class="font-weight-normal">{{ formatMoney(voucher.discountAmount, 0, ',', '.') }} Ft</td>
                <td class="font-weight-normal">{{ moment(voucher.activeFrom).format('YYYY-MM-DD HH:mm:ss') }}</td>
                <td class="font-weight-normal"><span :class="{'badge badge-success': (moment() < moment(voucher.activeTo) )}">{{ moment(voucher.activeTo).format('YYYY-MM-DD HH:mm:ss') }}</span></td>
                <td>
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              <tr v-if="vouchers.length==0">
                <td colspan="6">
                  <span class="font-weight-normal">Nincs kupon a rendszerben.</span>
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

    <voucher-details-modal id="voucher-details-modal" :options="voucherDetailsModalOptions" @success-callback="voucherDetailsModalSuccessCallback" @dismiss-callback="voucherDetailsModalDismissCallback"></voucher-details-modal>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'
  import VoucherDetailsModalComponent from './../VoucherDetailsModal.vue'

  var moment = require('moment');

  export default {
    name: 'vouchers',

    components: {
      'pagination': PaginationComponent,
      'voucher-details-modal': VoucherDetailsModalComponent
    },

    mounted: function () {
      this.fetchVouchers();
    },

    data() {
      return {
        formatMoney: global.formatMoney,
        moment: moment,

        vouchers: [],
        pagination: {
          currentPage: 1,
          data: {
            current_page: 1,
            last_page: 1,
            prev_page_url: null,
            next_page_url: null
          }
        },

        voucherDetailsModalOptions: {
          isHidden: true,
          voucher: {},
          apiError: ''
        }
      }
    },

    computed: {
    },

    methods: {
      fetchVouchers: function () {
        this.vouchers = [];

        fetch(global.App.baseURL + `api/voucher/page/${this.pagination.currentPage}`, {
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
              this.vouchers = res;

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
              message: 'Nem sikerült betölteni a kuponokat.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      },

      paginationCallback: function (url) {

      },

      addNewVoucher: function () {
        this.voucherDetailsModalOptions.voucher = {};
        this.voucherDetailsModalOptions.isHidden = false;
      },

      editVoucher: function (id) {
        const myID = id;
        const vm = this;
        this.voucherDetailsModalOptions.voucher = {};

        this.vouchers.forEach(function () {
          if (e.id == myID) {
            vm.voucherDetailsModalOptions.voucher = e;
          }
        });

        this.voucherDetailsModalOptions.isHidden = false;
      },

      voucherDetailsModalSuccessCallback: function (data) {
        let constData = data;
        fetch(global.App.baseURL + ((data.id) ? `api/voucher/page/${data.id}` : 'api/voucher'), {
            method: (data.id) ? 'put' : 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify(data)
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              this.voucherDetailsModalOptions.apiError = res.resultError;
              return;
            }

            // hide modal
            this.voucherDetailsModalOptions.isHidden = true;
            this.voucherDetailsModalOptions.voucher = {};

            // create notification
            global.jQuery.notify({
              message: 'A kupon adatait sikeresen elmentettük.'
            }, {
              type: 'success',
            });

            this.fetchVouchers();
          })
          .catch(err => console.log(err));
      },

      voucherDetailsModalDismissCallback: function () {
        this.voucherDetailsModalOptions.isHidden = true;
      }
    }
  }
</script>

<style>
</style>