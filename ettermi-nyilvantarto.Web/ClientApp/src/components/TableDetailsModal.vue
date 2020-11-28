<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="table-details-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="table-details-modal-label">{{ addNew ? "Új asztal hozzáadása" : "Asztal adatainak módosítása" }}</h5>
          <button type="button" class="close" v-on:click="onDismiss" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <form>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="table-code" class="col-form-label">Kód:</label>
                <input type="text" :class="['form-control', {'is-invalid': error_code_length}]" id="table-code" v-model="table.code" required :disabled="!addNew">
                <small v-if="error_code_length" class="text-danger">
                A kód megadása kötelező!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="table-size" class="col-form-label">Méret:</label>
                <input type="number" :class="['form-control', {'is-invalid': error_size_out_of_bounds}]" id="table-size" v-model="table.size" required min="1" :disabled="!addNew">
                <small v-if="error_size_out_of_bounds" class="text-danger">
                Az asztal méretének 0-nál nagyobb számnak kell lennie!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="table-category-id" class="col-form-label">Kategória:</label>
                <select id="table-category-id" class="form-control custom-select" :class="{'is-invalid': error_invalid_category}" v-model="table.categoryId" :disabled="!addNew">
                  <option value="null" disabled>Kérlek válassz</option>
                  <option v-for="cat in tableCategories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
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
          </form>
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
    name: 'table-details-modal',

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
            table: {},
            apiError: ''
          }
        }
      }
    },

    data() {
      return {
        tableCategories: [],
        table: {},
        errors: []
      }
    },

    mounted: function () {
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
          this.tableCategories = res;
        })
        .catch(err => global.console.log(err));
    },

    computed: {
      addNew: function () {
        return (this.table.id === undefined);
      },
      error_api: function () {
        return this.options.apiError.length > 0;
      },
      error_code_length: function () {
        return this.errors.indexOf('code_length') > -1;
      },
      error_size_out_of_bounds: function () {
        return this.errors.indexOf('size_out_of_bounds') > -1;
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

        this.table.size = this.table.size*1;

        if (this.table.code.length == 0) {
          this.errors.push('code_length');
        }
        if (this.table.size <= 0 || this.table.size == NaN) {
          this.errors.push('size_out_of_bounds');
        }
        if (this.table.categoryId == null) {
          this.errors.push('invalid_category');
        } else {
          this.table.categoryId = this.table.categoryId*1;
        }

        if (this.errors.length > 0) {
          return;
        }

        this.$emit('confirm-callback', Object.assign({}, this.table));
      },
      onDismiss: function () {
        this.table = Object.assign({}, this.options.table);
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('dismiss-callback');
      },
      onDelete: function () {
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('delete-callback', this.table.id);
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      },
      'options.table': function (val) {
        this.table = Object.assign({
          code: '',
          size: '',
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