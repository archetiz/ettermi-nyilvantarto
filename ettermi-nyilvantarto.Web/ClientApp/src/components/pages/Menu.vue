<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 col-lg-2 d-none d-lg-block content-box"></div>
        <div class="col-12 col-lg-8 content-box">
          <h3>Étlap</h3>
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
                  <button type="button" class="btn btn-outline-success btn-sm" @click="addNewMenuItem">Hozzáad</button>
                </li>
              </ul>
            </div>
          </nav>
          <table id="menu-items-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Név</th>
                <th scope="col">Ár</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <template v-for="category in menuItems">
              <tr :id="'menu-item-category-' + category.categoryId" class="menu-item-category-table-row">
                <td colspan="3" class="font-weight-bold">{{ category.categoryName }}</td>
              </tr>
              <tr v-for="item in category.items" :id="'menu-item-'+item.id" class="menu-items-table-row" :key="item.id" @click="editMenuItem(item.id)">
                <td class="font-weight-normal">{{ item.name }}</td>
                <td class="font-weight-normal">{{ formatMoney(item.price, 0, ',', '.') }} Ft</td>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              </template>
              <tr v-if="menuItems.length==0">
                <td colspan="3">
                  <span class="font-weight-normal">Nincs semmi sem az étlapon.</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="col-12 col-lg-2 d-none d-lg-block content-box"></div>
      </div>
    </div>

    <menu-item-details-modal id="menu-item-details-modal" :options="menuItemDetailsModalOptions" @confirm-callback="menuItemDetailsModalConfirmCallback" @dismiss-callback="menuItemDetailsModalDismissCallback" @delete-callback="menuItemDetailsModalDeleteCallback"></menu-item-details-modal>
  </div>
</template>

<script>
  import MenuItemDetailsModalComponent from './../MenuItemDetailsModal.vue'

  export default {
    name: 'menu',

    components: {
      'menu-item-details-modal': MenuItemDetailsModalComponent
    },

    data() {
      return {
        formatMoney: global.formatMoney,

        menuItems: [],

        menuItemDetailsModalOptions: {
          isHidden: true,
          menuItem: {},
          apiError: ''
        }
      }
    },

    mounted: function () {
      this.fetchMenuCategories();
    },

    methods: {
      fetchMenuCategories: function () {
        this.menuItems = [];
        
        fetch(global.App.baseURL + `api/menu/categories`, {
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
              this.menuItems.push({ 
                categoryId: res[i].id, 
                categoryName: res[i].name,
                items: [] 
              });
            }
          })
          .catch(err => global.console.log(err));
      },

      fetchMenuItems: function () {
        if (this.menuItems.length == 0) {
          return;
        }

        for (var i = 0; i < this.menuItems.length; i++) {
          this.menuItems[i].items = [];
        }

        fetch(global.App.baseURL + `api/menu`, {
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
              for (var j = 0; j < this.menuItems.length; j++) {
                if (res[i].categoryId == this.menuItems[j].categoryId) {
                  this.menuItems[j].items.push(res[i]);

                  break;
                }
              }
            }
          })
          .catch(err => global.console.log(err));
      },

      addNewMenuItem: function () {
        this.menuItemDetailsModalOptions.menuItem = {};
        this.menuItemDetailsModalOptions.isHidden = false;
      },

      editMenuItem: function (id) {
        const myID = id;
        const vm = this;
        this.menuItemDetailsModalOptions.menuItem = {};

        this.menuItems.forEach(function (c) {
          c.items.forEach(function (e) {
            if (e.id == myID) {
              vm.menuItemDetailsModalOptions.menuItem = e;
            }
          });
        })

        this.menuItemDetailsModalOptions.isHidden = false;
      },

      menuItemDetailsModalConfirmCallback: function (data) {
        let constData = data;
        fetch(global.App.baseURL + ((data.id) ? `api/menu/${data.id}` : 'api/menu'), {
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

            if (constData.id && res.status == 200) {
              this.menuItemDetailsModalConfirmCallbackSuccess();
              return;
            }

            res.json().then(res => {
              if (res.resultError !== undefined) {
                this.menuItemDetailsModalOptions.apiError = res.resultError;
                return;
              }

              this.menuItemDetailsModalConfirmCallbackSuccess();
            });
          })
          .catch(err => console.log(err));
      },

      menuItemDetailsModalConfirmCallbackSuccess: function () {
        // hide modal
        this.menuItemDetailsModalOptions.isHidden = true;
        this.menuItemDetailsModalOptions.menuItem = {};

        // create notification
        global.jQuery.notify({
          message: 'Az étel/ital adatait sikeresen elmentettük.'
        }, {
          type: 'success',
        });

        this.fetchMenuItems();
      },

      menuItemDetailsModalDismissCallback: function () {
        this.menuItemDetailsModalOptions.isHidden = true;
      },

      menuItemDetailsModalDeleteCallback: function (id) {
        fetch(global.App.baseURL + `api/menu/${id}`, {
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
              this.menuItemDetailsModalOptions.apiError = "Ismeretlen hiba.";
            }

            // hide modal
            this.menuItemDetailsModalOptions.isHidden = true;
            this.menuItemDetailsModalOptions.menuItem = {};

            // create notification
            global.jQuery.notify({
              message: 'Az étel/ital adatait sikeresen töröltük.'
            }, {
              type: 'success',
            });

            this.fetchMenuItems();
          })
          .catch(err => console.log(err));
      }
    },

    watch: {
      "menuItems": function () {
        this.fetchMenuItems();
      }
    }
  }
</script>

<style>
</style>