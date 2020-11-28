<template>
  <div>
    <div class="container">
      <div class="row loyalty-card-balance">
        <div class="d-none d-lg-block col-lg-3"></div>
        <div class="content-box col-12 col-lg-6">
          <div class="row">
            <div class="col-12">
              <h3 class="text-center">Jelszó módosítása</h3>
            </div>
          </div>
          <div class="row content-box form-row">
            <div class="form-group col-12">
              <label for="user-password" class="col-form-label">Jelenlegi jelszó:</label>
              <input type="password" :class="['form-control', {'is-invalid': error_current_password_length}]" id="user-password" required v-model="user.oldPassword">
              <small v-if="error_current_password_length" class="text-danger">
              A jelenlegi jelszó megadása kötelező!
              </small>
            </div>
          </div>
          <div class="row content-box form-row">
            <div class="form-group col-12">
              <label for="user-new-password" class="col-form-label">Új jelszó:</label>
              <input type="password" :class="['form-control', {'is-invalid': error_new_password_length}]" id="user-new-password" required v-model="user.newPassword">
              <small v-if="error_new_password_length" class="text-danger">
              Az új jelszó megadása kötelező!
              </small>
            </div>
          </div>
          <div class="row content-box form-row">
            <div class="form-group col-12">
              <label for="user-confirm-new-password" class="col-form-label">Új jelszó mégegyszer:</label>
              <input type="password" :class="['form-control', {'is-invalid': error_incorrect_confirm_new_password}]" id="user-confirm-new-password" required v-model="user.newPassword2">
              <small v-if="error_incorrect_confirm_new_password" class="text-danger">
              A két jelszó nem egyezik!
              </small>
            </div>
          </div>
          <div v-if="error_api" class="row content-box form-row">
            <div class="form-group col-12">
              <small class="text-danger">
              Nem sikerült rögzíteni a fiók adatait a rendszerben. A hiba oka: {{ apiError }}
              </small>
            </div>
          </div>
          <div class="row content-box form-row">
            <div class="form-group col-12 text-right">
              <button type="button" class="btn btn-primary" v-on:click="onSubmit">OK</button>
            </div>
          </div>
        </div>
        <div class="d-none d-lg-block col-lg-3"></div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'loyalty-card-balance',

    data() {
      return {
        formatMoney: global.formatMoney,

        apiError: '',
        errors: [],
        user: {
          oldPassword: '',
          newPassword: '',
          newPassword2: ''
        }
      }
    },

    computed: {
      error_api: function () {
        return this.apiError.length > 0;
      },
      error_current_password_length: function () {
        return this.errors.indexOf('current_password_length') > -1;
      },
      error_new_password_length: function () {
        return this.errors.indexOf('new_password_length') > -1;
      },
      error_incorrect_confirm_new_password: function () {
        return this.errors.indexOf('incorrect_confirm_new_password') > -1;
      }
    },

    methods: {
      onSubmit: function () {
        this.apiError = '';
        this.errors = [];

        if (this.user.oldPassword.length == 0) {
          this.errors.push('current_password_length');
        }
        if (this.user.newPassword.length == 0) {
          this.errors.push('new_password_length');
        }
        if (this.user.newPassword != this.user.newPassword2) {
          this.errors.push('incorrect_confirm_new_password');
        }

        if (this.errors.length > 0) {
          return;
        }

        fetch(global.App.baseURL + `api/user/password`, {
            method: 'put',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify(this.user)
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => {
            if (res === undefined) { return; }

            if (res.status == 200) {
              this.user = {oldPassword: '', newPassword: '', newPassword2: ''};

              // create notification
              global.jQuery.notify({
                message: 'Az új jelszót sikeresen elmentettük.'
              }, {
                type: 'success',
              });

              return;
            } else {
              res.json().then(res => {
                if (res.resultError !== undefined) {
                  this.apiError = res.resultError;
                  return;
                }
              });
            }
          })
          .catch(err => console.log(err));
      },
    }
  }
</script>

<style>
  .loyalty-card-balance .balance-box-row {
    background: #f8f9fa;
    margin: 0;
    padding: 20px;
  }
  .loyalty-card-balance .balance-box-row:first-child {
    padding-bottom: 5px;
  }
  .loyalty-card-balance .balance-box-row:last-child {
    padding-top: 0;
  }
</style>