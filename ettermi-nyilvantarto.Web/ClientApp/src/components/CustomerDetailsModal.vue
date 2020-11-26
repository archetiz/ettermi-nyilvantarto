<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="customer-details-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="customer-details-modal-label">{{ addNew ? "Új megrendelő hozzáadása" : "Megrendelő adatainak módosítása" }}</h5>
          <button type="button" class="close" v-on:click="onDismiss" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <form>
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
            </div>
            <div v-if="error_api" class="form-row">
              <div class="form-group col-12">
                <small class="text-danger">
                Nem sikerült rögzíteni a vásárlót a rendszerben. A hiba oka: {{ options.apiError }}
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
  var moment = require('moment');
  var daterangepicker = require('daterangepicker');
  require('daterangepicker/daterangepicker.css');


  export default {
    name: 'customer-details-modal',

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
            customer: {},
            apiError: ''
          }
        }
      }
    },

    data() {
      return {
        customer: {},
        errors: []
      }
    },

    computed: {
      addNew: function () {
        return (this.customer.id === undefined);
      },
      error_api: function () {
        return this.options.apiError.length > 0;
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

    methods: {
      onSubmit: function () {
        this.options.apiError = '';
        this.errors = [];

        if (this.customer.name.length == 0) {
          this.errors.push('name_length');
        }
        if (!/^([+]?[0-9]{9,11})$/.test(this.customer.phoneNumber)) {
           this.errors.push('phone_wrong_format');
        }
        if (this.customer.address.length == 0) {
          this.errors.push('address_length');
        }

        if (this.errors.length > 0) {
          return;
        }

        this.$emit('confirm-callback', Object.assign({}, this.customer));
      },
      onDismiss: function () {
        this.customer = Object.assign({}, this.options.customer);
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('dismiss-callback');
      },
      onDelete: function () {
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('delete-callback', this.customer.id);
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      },
      'options.customer': function (val) {
        this.customer = Object.assign({
          name: '',
          phoneNumber: '',
          address: ''
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