<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="menu-item-details-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="menu-item-details-modal-label">{{ addNew ? "Új megrendelő hozzáadása" : "Megrendelő adatainak módosítása" }}</h5>
          <button type="button" class="close" v-on:click="onDismiss" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <div class="form-row">
            <div class="form-group col-12">
              <label for="menu-item-name" class="col-form-label">Elnevezés:</label>
              <input type="text" :class="['form-control', {'is-invalid': error_name_length}]" id="menu-item-name" v-model="menuItem.name" required :disabled="!addNew">
              <small v-if="error_name_length" class="text-danger">
              A név megadása kötelező!
              </small>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group col-12">
              <label for="menu-item-price" class="col-form-label">Ár:</label>
              <input type="number" :class="['form-control', {'is-invalid': error_price_out_of_bounds}]" id="menu-item-price" v-model="menuItem.price" required min="1" :disabled="!addNew">
              <small v-if="error_price_out_of_bounds" class="text-danger">
              Az étel/ital ára 0-nál nagyobb számnak kell lennie!
              </small>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group col-12">
              <label for="menu-item-category-id" class="col-form-label">Kategória:</label>
              <select id="menu-item-category-id" class="form-control custom-select" :class="{'is-invalid': error_invalid_category}" v-model="menuItem.categoryId" :disabled="!addNew">
                <option value="null" disabled>Kérlek válassz</option>
                <option v-for="cat in menuCategories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
              </select>
              <small v-if="error_invalid_category" class="text-danger">
              Kategória választása kötelező!
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
        </div>
        <div class="modal-footer">
          <button v-if="!addNew" type="button" class="btn btn-danger mr-auto" v-on:click="onDelete">Töröl</button>
          <button v-if="addNew" type="button" class="btn btn-outline-secondary" v-on:click="onDismiss">Mégsem</button>
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
    name: 'menu-item-details-modal',

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
            menuItem: {},
            apiError: ''
          }
        }
      }
    },

    data() {
      return {
        menuCategories: [],
        menuItem: {},
        errors: []
      }
    },

    mounted: function () {
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
          this.menuCategories = res;
        })
        .catch(err => global.console.log(err));
    },

    computed: {
      addNew: function () {
        return (this.menuItem.id === undefined);
      },
      error_api: function () {
        return this.options.apiError.length > 0;
      },
      error_name_length: function () {
        return this.errors.indexOf('name_length') > -1;
      },
      error_price_out_of_bounds: function () {
        return this.errors.indexOf('price_out_of_bounds') > -1;
      },
      error_invalid_category: function () {
        return this.errors.indexOf('invalid_category') > -1;
      }
    },

    methods: {
      onSubmit: function () {
        if (!this.addNew) {
          this.onDismiss();
          return;
        }

        this.options.apiError = '';
        this.errors = [];

        this.menuItem.price = this.menuItem.price*1;

        if (this.menuItem.name.length == 0) {
          this.errors.push('name_length');
        }
        if (this.menuItem.price <= 0 || this.menuItem.price == NaN) {
          this.errors.push('price_out_of_bounds');
        }
        if (this.menuItem.categoryId == null) {
          this.errors.push('invalid_category');
        } else {
          this.menuItem.categoryId = this.menuItem.categoryId*1;
        }

        if (this.errors.length > 0) {
          return;
        }

        this.$emit('confirm-callback', Object.assign({}, this.menuItem));
      },
      onDismiss: function () {
        this.menuItem = Object.assign({}, this.options.menuItem);
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('dismiss-callback');
      },
      onDelete: function () {
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('delete-callback', this.menuItem.id);
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      },
      'options.menuItem': function (val) {
        this.menuItem = Object.assign({
          name: '',
          price: '',
          categoryId: null
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