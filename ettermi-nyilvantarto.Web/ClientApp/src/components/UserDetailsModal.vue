<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="user-details-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="user-details-modal-label">{{ addNew ? "Új felhasználó hozzáadása" : "Felhasználói fiók módosítása" }}</h5>
          <button type="button" class="close" v-on:click="onDismiss" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <form>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="user-name" class="col-form-label">Név:</label>
                <input type="text" :class="['form-control', {'is-invalid': error_name_length}]" id="user-name" v-model="user.name" required :disabled="!addNew">
                <small v-if="error_name_length" class="text-danger">
                A név megadása kötelező!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="user-email" class="col-form-label">E-mail cím:</label>
                <input type="text" :class="['form-control', {'is-invalid': error_email_length}]" id="user-email" v-model="user.email" required :disabled="!addNew">
                <small v-if="error_email_length" class="text-danger">
                Az e-mail cím megadása kötelező!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="user-user-name" class="col-form-label">Felhasználónév:</label>
                <input type="text" :class="['form-control', {'is-invalid': error_userName_length}]" id="user-user-name" v-model="user.userName" required :disabled="!addNew">
                <small v-if="error_userName_length" class="text-danger">
                A felhasználónév megadása kötelező!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="user-password" class="col-form-label">Jelszó:</label>
                <input type="password" :class="['form-control', {'is-invalid': error_password_length}]" id="user-password" v-model="user.password" required>
                <small v-if="error_password_length" class="text-danger">
                A jelszó megadása kötelező!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="user-account-type" class="col-form-label">Fióktípus:</label>
                <select id="user-account-type" class="form-control custom-select" :class="{'is-invalid': error_invalid_account_type}" v-model="user.accountType" :disabled="!addNew">
                  <option value="null" disabled>Kérlek válassz</option>
                  <option value="Owner">Tulajdonos</option>
                  <option value="Waiter">Pincér</option>
                  <option value="Chef">Séf</option>
                </select>
                <small v-if="error_invalid_account_type" class="text-danger">
                Fióktípus választása kötelező!
                </small>
              </div>
            </div>
            <div v-if="error_api" class="form-row">
              <div class="form-group col-12">
                <small class="text-danger">
                Nem sikerült rögzíteni a fiók adatait a rendszerben. A hiba oka: {{ options.apiError }}
                </small>
              </div>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button v-if="!addNew" type="button" class="btn btn-danger mr-auto" v-on:click="onDelete">Töröl</button>
          <button type="button" class="btn btn-outline-secondary" v-on:click="onDismiss">Mégsem</button>
          <button type="button" class="btn btn-primary" v-on:click="onSubmit">OK</button>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
  export default {
    name: 'user-details-modal',

    props: {
      id: {
        type: String,
        required: true
      },
      options: {
        type: Object,
        default: function () {
          return {
            isHidden: true,
            percentage: 'percentage',
            user: {},
            apiError: ''
          }
        }
      }
    },

    data() {
      return {
        user: {},
        userType: 'percentage',
        errors: [],

        dateTimePickerTimer: null,
        // Get more form http://www.daterangepicker.com/
        dateTimePickerConfig: {
          singleDatePicker: true,
          timePicker: true,
          timePicker24Hour: true,
          timePickerSeconds: true,
          autoUpdateInput: false,
          locale: {
              format: global.App.timeFormat,
              applyLabel: "OK",
              cancelLabel: "Mégsem",
              daysOfWeek: [
                  "V",
                  "H",
                  "K",
                  "Sze",
                  "Cs",
                  "P",
                  "Szo"
              ],
              monthNames: [
                  "Január",
                  "Február",
                  "Március",
                  "Április",
                  "Május",
                  "Június",
                  "Július",
                  "Augusztus",
                  "Szeptember",
                  "Október",
                  "November",
                  "December"
              ],
              firstDay: 1
          },
          alwaysShowCalendars: true,
          drops: "up",
          opens: "center"
        }
      }
    },

    computed: {
      addNew: function () {
        return (this.user.id === undefined);
      },
      error_api: function () {
        return this.options.apiError.length > 0;
      },
      error_name_length: function () {
        return this.errors.indexOf('name_length') > -1;
      },
      error_email_length: function () {
        return this.errors.indexOf('email_length') > -1;
      },
      error_userName_length: function () {
        return this.errors.indexOf('userName_length') > -1;
      },
      error_password_length: function () {
        return this.errors.indexOf('password_length') > -1;
      },
      error_invalid_account_type: function () {
        return this.errors.indexOf('invalid_account_type') > -1;
      }
    },

    methods: {
      onSubmit: function () {
        this.options.apiError = '';
        this.errors = [];

        if (this.user.name.length == 0) {
          this.errors.push('name_length');
        }
        if (this.user.email.length == 0) {
          this.errors.push('email_length');
        }
        if (this.user.userName.length == 0) {
          this.errors.push('userName_length');
        }
        if (this.user.password.length == 0) {
          this.errors.push('password_length');
        }
        if (this.user.accountType == null) {
          this.errors.push('invalid_account_type');
        }

        if (this.errors.length > 0) {
          return;
        }

        this.$emit('confirm-callback', Object.assign({}, this.user));
      },
      onDismiss: function () {
        this.user = Object.assign({}, this.options.user);
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('dismiss-callback');
      },
      onDelete: function () {
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('delete-callback', this.user.id);
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      },
      'options.user': function (val) {
        this.user = Object.assign({
          "name":'',
          "email":'',
          "userName":'',
          "password":'',
          "accountType":null,
        }, val);
      }
    }
  }
</script>

<style>
  .modal-dialog {
    max-width: 550px;
  }
</style>