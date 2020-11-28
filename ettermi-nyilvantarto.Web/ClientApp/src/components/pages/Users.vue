<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Felhasználói fiókok</h3>
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
                  <button type="button" class="btn btn-outline-success btn-sm" @click="addNewUser">Hozzáad</button>
                </li>
              </ul>
            </div>
          </nav>
          <table id="users-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col">Név</th>
                <th scope="col">E-mail cím</th>
                <th scope="col">Kedvezmény</th>
                <th scope="col">Felhasználónév</th>
                <th scope="col">Fióktípus</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in users" :id="'user-'+user.id" class="users-table-row" :key="user.id" @click="editUser(user.id)">
                <td class="font-weight-normal">{{ user.name }}</td>
                <td class="font-weight-normal">{{ user.email }}</td>
                <td class="font-weight-normal">{{ user.userName }}</td>
                <td class="font-weight-normal">{{ (user.accountType == 'Owner') ? 'Tulajdonos' : (user.accountType == 'Waiter') ? 'Pincér' : (user.accountType == 'Chef') ? 'Séf' : 'n/a' }}</td>
                <td class="text-right">
                  <ion-icon name="play"></ion-icon>
                </td>
              </tr>
              <tr v-if="users.length==0">
                <td colspan="6">
                  <span class="font-weight-normal">Nincs felhasználói fiók a rendszerben.</span>
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

    <user-details-modal id="user-details-modal" :options="userDetailsModalOptions" @confirm-callback="userDetailsModalConfirmCallback" @dismiss-callback="userDetailsModalDismissCallback" @delete-callback="userDetailsModalDeleteCallback"></user-details-modal>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'
  import UserDetailsModalComponent from './../UserDetailsModal.vue'

  export default {
    name: 'users',

    components: {
      'pagination': PaginationComponent,
      'user-details-modal': UserDetailsModalComponent
    },

    mounted: function () {
      this.fetchUsers();
    },

    data() {
      return {
        users: [],
        pagination: {
          currentPage: 1,
          data: {
            current_page: 1,
            last_page: 1,
            prev_page_url: null,
            next_page_url: null
          }
        },

        userDetailsModalOptions: {
          isHidden: true,
          user: {},
          apiError: ''
        }
      }
    },

    methods: {
      fetchUsers: function () {
        this.users = [];

        fetch(global.App.baseURL + `api/user/page/${this.pagination.currentPage}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            this.users = res.elements;

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
        this.fetchUsers();
      },

      addNewUser: function () {
        this.userDetailsModalOptions.user = {};
        this.userDetailsModalOptions.isHidden = false;
      },

      editUser: function (id) {
        const myID = id;
        const vm = this;
        this.userDetailsModalOptions.user = {};

        this.users.forEach(function (e) {
          if (e.id == myID) {
            vm.userDetailsModalOptions.user = e;
          }
        });

        this.userDetailsModalOptions.isHidden = false;
      },

      userDetailsModalConfirmCallback: function (data) {
        let constData = data;
        fetch(global.App.baseURL + ((data.id) ? `api/user/${data.id}/password` : 'api/user'), {
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
              this.userDetailsModalConfirmCallbackSuccess();
              return;
            }

            res.json().then(res => {
              if (res.resultError !== undefined) {
                this.userDetailsModalOptions.apiError = res.resultError;
                return;
              }

              this.userDetailsModalConfirmCallbackSuccess();
            });
          })
          .catch(err => console.log(err));
      },

      userDetailsModalConfirmCallbackSuccess: function () {
        // hide modal
        this.userDetailsModalOptions.isHidden = true;
        this.userDetailsModalOptions.user = {};

        // create notification
        global.jQuery.notify({
          message: 'A felhasználó jelszavát sikeresen elmentettük.'
        }, {
          type: 'success',
        });

        this.fetchUsers();
      },

      userDetailsModalDismissCallback: function () {
        this.userDetailsModalOptions.isHidden = true;
      },

      userDetailsModalDeleteCallback: function (id) {
        fetch(global.App.baseURL + `api/user/${id}`, {
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
              this.userDetailsModalOptions.apiError = "Ismeretlen hiba.";
            }

            // hide modal
            this.userDetailsModalOptions.isHidden = true;
            this.userDetailsModalOptions.user = {};

            // create notification
            global.jQuery.notify({
              message: 'A kupont sikeresen deaktiváltuk.'
            }, {
              type: 'success',
            });

            this.pagination.currentPage = 1;
            this.fetchUsers();
          })
          .catch(err => console.log(err));
      }
    }
  }
</script>

<style>
</style>