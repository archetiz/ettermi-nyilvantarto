<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 col-lg-2 d-none d-lg-block content-box"></div>
        <div class="col-12 col-lg-8 content-box">
          <h3>Asztalok</h3>
        </div>
        <div class="col-12 col-lg-2 d-none d-lg-block content-box"></div>
      </div>
      <div class="row">
        <div class="col-12 col-lg-2 d-none d-lg-block content-box"></div>
        <div class="col-12 col-lg-8 content-box">
          <nav class="navbar navbar-expand navbar-light bg-light">
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
              <ul class="navbar-nav mr-auto">
                <li class="nav-item form-check"></li>
              </ul>
              <ul class="navbar-nav">
                <li class="nav-item">
                  <button type="button" class="btn btn-outline-success btn-sm" @click="addNewTable">Hozzáad</button>
                </li>
              </ul>
            </div>
          </nav>
          <table id="tables-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Kód</th>
                <th scope="col">Méret</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <template v-for="category in tables">
              <tr :id="'table-category-' + category.categoryId" class="table-category-table-row">
                <td colspan="3" class="font-weight-bold">{{ category.categoryName }}</td>
              </tr>
              <tr v-for="item in category.items" :id="'table-'+item.id" class="tables-table-row" :key="item.id" @click="editTable(item.id)">
                <td class="font-weight-normal">{{ item.code }}</td>
                <td class="font-weight-normal">{{ item.size }} fő</td>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              </template>
              <tr v-if="tables.length==0">
                <td colspan="3">
                  <span class="font-weight-normal">Nincs egy asztal sem a rendszerben.</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="col-12 col-lg-2 d-none d-lg-block content-box"></div>
      </div>
    </div>

    <table-details-modal id="table-details-modal" :options="tableDetailsModalOptions" @confirm-callback="tableDetailsModalConfirmCallback" @dismiss-callback="tableDetailsModalDismissCallback" @delete-callback="tableDetailsModalDeleteCallback"></table-details-modal>
  </div>
</template>

<script>
  import TableDetailsModalComponent from './../TableDetailsModal.vue'

  export default {
    name: 'tables',

    components: {
      'table-details-modal': TableDetailsModalComponent
    },

    data() {
      return {
        formatMoney: global.formatMoney,

        tables: [],

        tableDetailsModalOptions: {
          isHidden: true,
          table: {},
          apiError: ''
        }
      }
    },

    mounted: function () {
      this.fetchMenuCategories();
    },

    methods: {
      fetchMenuCategories: function () {
        this.tables = [];
        
        fetch(global.App.baseURL + `api/table/categories`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            for (var i = 0; i < res.length; i++) {
              this.tables.push({ 
                categoryId: res[i].id, 
                categoryName: res[i].name,
                items: [] 
              });
            }
          })
          .catch(err => global.console.log(err));
      },

      fetchTables: function () {
        if (this.tables.length == 0) {
          return;
        }

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
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            for (var i = 0; i < res.length; i++) {
              for (var j = 0; j < this.tables.length; j++) {
                if (res[i].categoryId == this.tables[j].categoryId) {
                  this.tables[j].items.push(res[i]);

                  break;
                }
              }
            }
          })
          .catch(err => global.console.log(err));
      },

      addNewTable: function () {
        this.tableDetailsModalOptions.table = {};
        this.tableDetailsModalOptions.isHidden = false;
      },

      editTable: function (id) {
        const myID = id;
        const vm = this;
        this.tableDetailsModalOptions.table = {};

        this.tables.forEach(function (c) {
          c.items.forEach(function (e) {
            if (e.id == myID) {
              vm.tableDetailsModalOptions.table = e;
            }
          });
        })

        this.tableDetailsModalOptions.isHidden = false;
      },

      tableDetailsModalConfirmCallback: function (data) {
        let constData = data;
        fetch(global.App.baseURL + ((data.id) ? `api/table/${data.id}` : 'api/table'), {
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
                this.tableDetailsModalOptions.apiError = res.resultError;
                return;
              }

              // hide modal
              this.tableDetailsModalOptions.isHidden = true;
              this.tableDetailsModalOptions.table = {};

              // create notification
              global.jQuery.notify({
                message: 'Az asztal adatait sikeresen elmentettük.'
              }, {
                type: 'success',
              });

              this.fetchTables();
            });
          })
          .catch(err => console.log(err));
      },

      tableDetailsModalDismissCallback: function () {
        this.tableDetailsModalOptions.isHidden = true;
      },

      tableDetailsModalDeleteCallback: function (id) {
        fetch(global.App.baseURL + `api/table/${id}`, {
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
              this.tableDetailsModalOptions.apiError = "Ismeretlen hiba.";
            }

            // hide modal
            this.tableDetailsModalOptions.isHidden = true;
            this.tableDetailsModalOptions.table = {};

            // create notification
            global.jQuery.notify({
              message: 'Az asztal adatait sikeresen töröltük.'
            }, {
              type: 'success',
            });

            this.fetchTables();
          })
          .catch(err => console.log(err));
      }
    },

    watch: {
      "tables": function () {
        this.fetchTables();
      }
    }
  }
</script>

<style>
</style>