<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Megrendelők</h3>
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
                  <button type="button" class="btn btn-outline-success btn-sm" @click="addNewCustomer">Hozzáad</button>
                </li>
              </ul>
            </div>
          </nav>
          <table id="customers-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Név</th>
                <th scope="col">Telefonszám</th>
                <th scope="col">Cím</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="customer in customers" :id="'customer-'+customer.id" class="customers-table-row" :key="customer.id" @click="editCustomer(customer.id)">
                <td class="font-weight-normal">{{ customer.name }}</td>
                <td class="font-weight-normal">{{ customer.phoneNumber }}</td>
                <td class="font-weight-normal">{{ customer.address }}</td>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              <tr v-if="customers.length==0">
                <td colspan="4">
                  <span class="font-weight-normal">Nincs megrendelő a rendszerben.</span>
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

    <customer-details-modal id="customer-details-modal" :options="customerDetailsModalOptions" @confirm-callback="customerDetailsModalConfirmCallback" @dismiss-callback="customerDetailsModalDismissCallback" @delete-callback="customerDetailsModalDeleteCallback"></customer-details-modal>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'
  import CustomerDetailsModalComponent from './../CustomerDetailsModal.vue'

  export default {
    name: 'customers',

    components: {
      'pagination': PaginationComponent,
      'customer-details-modal': CustomerDetailsModalComponent
    },

    mounted: function () {
      this.fetchCustomers();
    },

    data() {
      return {
        customers: [],
        pagination: {
          currentPage: 1,
          data: {
            current_page: 1,
            last_page: 1,
            prev_page_url: null,
            next_page_url: null
          }
        },

        customerDetailsModalOptions: {
          isHidden: true,
          customer: {},
          apiError: ''
        }
      }
    },

    methods: {
      fetchCustomers: function () {
        this.customers = [];

        fetch(global.App.baseURL + `api/customer/page/${this.pagination.currentPage}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            this.customers = res.elements;

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
        this.fetchCustomers();
      },

      addNewCustomer: function () {
        this.customerDetailsModalOptions.customer = {};
        this.customerDetailsModalOptions.isHidden = false;
      },

      editCustomer: function (id) {
        const myID = id;
        const vm = this;
        this.customerDetailsModalOptions.customer = {};

        this.customers.forEach(function (e) {
          if (e.id == myID) {
            vm.customerDetailsModalOptions.customer = e;
          }
        });

        this.customerDetailsModalOptions.isHidden = false;
      },

      customerDetailsModalConfirmCallback: function (data) {
        let constData = data;
        fetch(global.App.baseURL + ((data.id) ? `api/customer/${data.id}` : 'api/customer'), {
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
                this.customerDetailsModalOptions.apiError = res.resultError;
                return;
              }

              // hide modal
              this.customerDetailsModalOptions.isHidden = true;
              this.customerDetailsModalOptions.voucher = {};

              // create notification
              global.jQuery.notify({
                message: 'A megrendelő adatait sikeresen elmentettük.'
              }, {
                type: 'success',
              });

              this.fetchCustomers();
            });
          })
          .catch(err => console.log(err));
      },

      customerDetailsModalDismissCallback: function () {
        this.customerDetailsModalOptions.isHidden = true;
      },

      customerDetailsModalDeleteCallback: function (id) {
        fetch(global.App.baseURL + `api/customer/${id}`, {
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
              this.customerDetailsModalOptions.apiError = "Ismeretlen hiba.";
            }

            // hide modal
            this.customerDetailsModalOptions.isHidden = true;
            this.customerDetailsModalOptions.voucher = {};

            // create notification
            global.jQuery.notify({
              message: 'A megrendelő adatait sikeresen töröltük.'
            }, {
              type: 'success',
            });

            this.pagination.currentPage = 1;
            this.fetchCustomers();
          })
          .catch(err => console.log(err));
      }
    }
  }
</script>

<style>
</style>