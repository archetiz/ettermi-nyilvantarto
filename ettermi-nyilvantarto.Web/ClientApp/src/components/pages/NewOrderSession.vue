<template>
  <div>
    <div class="container">
      <div class="row new-order-session">
        <div class="d-none d-lg-block col-lg-2"></div>
        <div class="content-box col-12 col-lg-8">
          <div class="row">
            <div class="col-12">
              <h3 class="text-center">Új rendelés felvétele</h3>
            </div>
          </div>
          <div class="row">
            <div :class="['content-box', 'col-12', 'mb-2', {'d-none': (orderType == '')}]">
              <button type="button" class="btn btn-secondary d-none d-lg-block" v-on:click="onBackButton">Vissza</button>
              <button type="button" class="btn btn-secondary btn-block d-lg-none" v-on:click="onBackButton">Vissza</button>
            </div>
          </div>
          <div :class="['row', {'d-none': (orderType != '')}]">
            <div class="content-box col-12 col-lg-6 mb-3">
              <span class="btn btn-light btn-block pt-4 pb-4 new-btn" @click="takeawaySelected" role="button">Elvitelre</span>
            </div>
            <div class="content-box col-12 col-lg-6 mb-3">
              <span class="btn btn-light btn-block pt-4 pb-4 new-btn" @click="onsiteSelected" role="button">Helyben fogyasztás</span>
            </div>
          </div>
          <div :class="['row', {'d-none': (orderType != 'takeaway')}]">
            <div class="content-box col-12">
              <h4 class="mb-2">Keresés a rendszerben</h4>
              <form>
                <div class="form-row">
                  <div class="form-group col-12 col-lg-10">
                    <input type="text" class="form-control" v-model="searchQuery" required>
                  </div>
                  <div class="form-group col-12 col-lg-2">
                    <button type="button" class="btn btn-primary btn-block" v-on:click="onSearchCustomer">Keresés</button>
                  </div>
                </div>
              </form>
              <table class="table table-hover table-clickable">
                <thead>
                  <tr>
                    <th scope="col">Név</th>
                    <th scope="col">Telefonszám</th>
                    <th scope="col">Cím</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="customer in searchResults" :key="customer.id" @click="onCustomerSelected(customer.id)">
                    <td class="font-weight-normal">{{ customer.name }}</td>
                    <td class="font-weight-normal">{{ customer.phoneNumber }}</td>
                    <td class="font-weight-normal">{{ customer.address }}</td>
                    <td class="text-right">
                      <ion-icon name="play"></ion-icon>
                    </td>
                  </tr>
                  <tr v-if="searchResults.length==0">
                    <td colspan="4">
                      <span class="font-weight-normal">Nincs a keresésnek megfelelő megrendelő a rendszerben.</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
          <div :class="['row', {'d-none': (orderType != 'takeaway')}]">
            <div class="content-box col-12">
              <h4 class="mb-2">Új megrendelő hozzáadása</h4>
              <div class="form-row">
                <div class="form-group col-12">
                  <label for="customer-name" class="col-form-label">Név:</label>
                  <input type="text" :class="['form-control', {'is-invalid': error_name_length}]" id="customer-name" v-model="customer.name" required>
                  <small v-if="error_name_length" class="text-danger">
                  A név megadása kötelező!
                  </small>
                </div>
              </div>
              <div class="form-row">
                <div class="form-group col-12">
                  <label for="customer-phone" class="col-form-label">Telefonszám:</label>
                  <input type="text" :class="['form-control', {'is-invalid': error_phone_wrong_format}]" id="customer-phone" v-model="customer.phoneNumber" required>
                  <small v-if="error_phone_wrong_format" class="text-danger">
                  A megadott formátum nem megfelelő!
                  </small>
                  <small class="text-info">
                  Formátum: +36201234567
                  </small>
                </div>
              </div>
              <div class="form-row">
                <div class="form-group col-12">
                  <label for="customer-address" class="col-form-label">Cím:</label>
                  <input type="text" :class="['form-control', {'is-invalid': error_address_length}]" id="customer-address" v-model="customer.address" required>
                  <small v-if="error_address_length" class="text-danger">
                  A cím megadása kötelező!
                  </small>
                </div>
                <div v-if="error_api" class="form-group col-12">
                  <small class="text-danger">
                  Nem sikerült rögzíteni a vásárlót a rendszerben. A hiba oka: {{ apiError }}
                  </small>
                </div>
              </div>
              <div class="form-row">
                <div class="form-group col-12">
                  <button type="button" class="btn btn-primary d-none d-lg-block" v-on:click="onSubmitNewCustomer">Hozzáad</button>
                  <button type="button" class="btn btn-primary btn-block d-lg-none" v-on:click="onSubmitNewCustomer">Hozzáad</button>
                </div>
              </div>
            </div>
          </div>
          <div :class="['row', {'d-none': (orderType != 'onsite')}]">
            <div class="content-box col-12">
              <h4 class="mb-2">Asztal kiválasztása</h4>
              <div v-for="tg in tables">
                <div class="row">
                  <div class="col-12">
                    <h5>{{ tg.categoryName }}</h5>
                  </div>
                </div>
                <div class="row">
                  <div v-for="table in tg.items" class="col-6 col-lg-3">
                    <span class="btn btn-light btn-block pt-4 pb-4 new-btn" @click="onTableSelected(table.id)" role="button">{{ table.code }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="content-box d-none d-lg-block col-lg-2"></div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'new-order-session',

    data() {
      return {
        orderType: '', // takeaway/onsite

        searchQuery: '',
        searchResults: [],

        customer: {
          name: '',
          phoneNumber: '',
          address: ''
        },
        errors: [],
        apiError: '',

        tables: []
      }
    },

    computed: {
      error_api: function () {
        return this.apiError.length > 0;
      },
      error_name_length: function () {
        return this.errors.indexOf('name_length') > -1;
      },
      error_phone_wrong_format: function () {
        return this.errors.indexOf('phone_wrong_format') > -1;
      },
      error_address_length: function () {
        return this.errors.indexOf('address_length') > -1;
      }
    },

    mounted: function () {
      this.fetchTableCategories();
    },

    methods: {
      fetchTableCategories: function () {
        fetch(global.App.baseURL + `api/table/categories`, {
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
              for (var i = 0; i < res.length; i++) {
                this.tables.push({ 
                  categoryId: res[i].id, 
                  categoryName: res[i].name,
                  items: [] 
                });
              }

              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült betölteni az asztalkategóriákat.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      },

      fetchTableList: function () {
        for (var i = 0; i < this.tables.length; i++) {
          this.tables[i].items = [];
        }

        fetch(global.App.baseURL + `api/table`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            res = [
              {
                "id": 1,
                "code": "A",
                "size": 2,
                "categoryId": 1,
                "categoryName": "CatA"
              },
              {
                "id": 2,
                "code": "AA",
                "size": 2,
                "categoryId": 1,
                "categoryName": "CatA"
              },
              {
                "id": 3,
                "code": "B",
                "size": 2,
                "categoryId": 2,
                "categoryName": "CatB"
              }
            ];

            if (res.resultError === undefined) {
              for (var i = 0; i < res.length; i++) {
                for (var j = 0; j < this.tables.length; j++) {
                  if (res[i].categoryId == this.tables[j].categoryId) {
                    this.tables[j].items.push(res[i]);

                    break;
                  }
                }
              }

              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült betölteni az asztallistát.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      },
      takeawaySelected: function (e) {
        e.preventDefault();

        this.orderType = 'takeaway';
      },

      onsiteSelected: function (e) {
        e.preventDefault();

        this.orderType = 'onsite';
      },

      onSearchCustomer: function () {
        this.searchResults = [];

        fetch(global.App.baseURL + `api/customer?query=${this.searchQuery}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              return;
            }

            //this.searchResults = res;
            this.searchResults = [{
              "id": 0,
              "name": "string",
              "phoneNumber": "string",
              "address": "string"
            }];
          })
          .catch(err => global.console.log(err));
      },

      onSubmitNewCustomer: function () {
        this.apiError = '';
        this.errors = [];

        if (this.customer.name !== undefined && this.customer.name.length == 0) {
          this.errors.push('name_length');
        }
        if (!/^([+]?[0-9]{9,11})$/.test(this.customer.phoneNumber)) {
           this.errors.push('phone_wrong_format');
        }
        if (this.customer.address !== undefined && this.customer.address.length == 0) {
          this.errors.push('address_length');
        }

        if (this.errors.length > 0) {
          return;
        }

        let vm = this;
        fetch(global.App.baseURL + `api/customer`, {
            method: 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify(this.customer)
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              this.apiError = res.resultError;
              return;
            }

            vm.onCustomerSelected(res);
          })
          .catch(err => console.log(err));
      },

      onCustomerSelected: function (id) {
        fetch(global.App.baseURL + `api/order`, {
            method: 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify({
              "customerId": id,
              "waiterId": global.App.user.id
            })
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              return;
            }

            this.$router.push({ path: `/order/${res}` });
          })
          .catch(err => console.log(err));
      },

      onTableSelected: function (id) {
        fetch(global.App.baseURL + `api/order`, {
            method: 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify({
              "tableId": id,
              "waiterId": global.App.user.id
            })
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              return;
            }

            this.$router.push({ path: `/order/${res}` });
          })
          .catch(err => console.log(err));
      },

      onBackButton: function () {
        this.orderType = '';
      }
    },

    watch: {
      'tables': function () {
        this.fetchTableList();
      }
    }
  }
</script>

<style>
  .new-order-session .new-btn {
    font-size: 16pt;
  }
</style>