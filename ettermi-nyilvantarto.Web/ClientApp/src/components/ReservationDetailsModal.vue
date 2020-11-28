<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="reservation-details-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="reservation-details-modal-label">{{ addNew ? "Új foglalás hozzáadása" : "Foglalás módosítása" }}</h5>
          <button type="button" class="close" v-on:click="onDismiss" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <form>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="reservation-customer-name" class="col-form-label">Név:</label>
                <input type="text" :class="['form-control', {'is-invalid': error_customer_name_length}]" id="reservation-customer-name" v-model="reservation.customerName" required>
                <small v-if="error_customer_name_length" class="text-danger">
                A név megadása kötelező!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="reservation-customer-phone" class="col-form-label">Telefonszám:</label>
                <input type="text" :class="['form-control', {'is-invalid': error_customer_phone_wrong_format}]" id="reservation-customer-phone" v-model="reservation.customerPhone" required>
                <small v-if="error_customer_phone_wrong_format" class="text-danger">
                A megadott formátum nem megfelelő!
                </small>
                <small class="text-info">
                Formátum: +36201234567
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="reservation-start-date" class="col-form-label">Foglalás kezdete:</label>
                <input type="text" class="form-control" :class="{'is-invalid': error_invalid_time_from}" id="reservation-start-date" maxlength="19" autocomplete="off">
                <small v-if="error_invalid_time_from" class="text-danger">
                A kezdődátum megadása kötelező.
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="reservation-end-date" class="col-form-label">Foglalás vége:</label>
                <input type="text" class="form-control" :class="{'is-invalid': error_invalid_time_to || error_time_from_bigger_than_time_to}" id="reservation-end-date" maxlength="19" autocomplete="off">
                <small v-if="error_invalid_time_to" class="text-danger">
                A befejezés dátumának megadása kötelező.
                </small>
                <small v-if="error_time_from_bigger_than_time_to" class="text-danger">
                A befejezés dátuma nem lehet előbbi a kezdődátumnál.
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="reservation-table-code" class="col-form-label">Kiválasztott asztal:</label>
                <span class="form-control" id="reservation-table-code">{{ reservation.tableCode }}</span>
                <input type="hidden" id="reservation-table-id" v-model="reservation.tableId">
              </div>
            </div>

            <div class="form-row">
              <div class="form-group col-12 mt-3">
                <h5>Asztal kiválasztása</h5>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="reservation-table-size" class="col-form-label">Asztalméret:</label>
                <input type="number" :class="['form-control', {'is-invalid': error_table_size_out_of_bounds}]" id="reservation-table-size" v-model="tableSearch.minSize" required min="1">
                <small v-if="error_table_size_out_of_bounds" class="text-danger">
                A megadott értéknek 0-nál nagyobbnak kell lennie!
                </small>
                <small class="text-info">
                Minimum asztalméret.
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="search-table-id" class="col-form-label">Asztal kiválasztása:</label>
                <select id="search-table-id" class="form-control custom-select" :class="{'is-invalid': error_invalid_table}" v-model="tableSearch.id">
                  <option value="null" disabled>Kérlek válassz</option>
                  <optgroup v-for="t in tables" :label="t.categoryName">
                    <option v-for="table in t.items" :key="table.id" :value="table.id">{{ table.code }} ({{ table.size }} fő)</option>
                  </optgroup>
                </select>
                <small v-if="error_invalid_table" class="text-danger">
                Asztal választása kötelező!
                </small>
              </div>
            </div>
            <div v-if="error_api" class="form-row">
              <div class="form-group col-12">
                <small class="text-danger">
                Nem sikerült rögzíteni a foglalást a rendszerben. A hiba oka: {{ options.apiError }}
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
    name: 'reservation-details-modal',

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
            reservation: {},
            apiError: ''
          }
        }
      }
    },

    data() {
      return {
        reservation: {},
        reservation: {},
        tableSearch: {
          id: null,
          minSize: 0,
          loading: false
        },
        tables: [],
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
              format: "YYYY-MM-DD HH:mm:ss",
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
        return (this.reservation.id === undefined);
      },
      error_api: function () {
        return this.options.apiError.length > 0;
      },
      error_customer_name_length: function () {
        return this.errors.indexOf('customer_name_length') > -1;
      },
      error_customer_phone_wrong_format: function () {
        return this.errors.indexOf('customer_phone_wrong_format') > -1;
      },
      error_invalid_time_from: function () {
        return this.errors.indexOf('invalid_time_from') > -1;
      },
      error_invalid_time_to: function () {
        return this.errors.indexOf('invalid_time_to') > -1;
      },
      error_time_from_bigger_than_time_to: function () {
        return this.errors.indexOf('time_from_bigger_than_time_to') > -1;
      },
      error_table_size_out_of_bounds: function () {
        return this.errors.indexOf('table_size_out_of_bounds') > -1;
      },
      error_invalid_table: function () {
        return this.errors.indexOf('invalid_table') > -1;
      }
    },

    mounted: function () {
      this.fetchTableCategories();

      let vm = this;
      global.jQuery("#reservation-start-date").daterangepicker(
          Object.assign(this.dateTimePickerConfig, {
            startDate: moment(),
            minDate: moment()
          })
      );
        global.jQuery('#reservation-start-date').on('apply.daterangepicker', function(ev, picker) {
          vm.reservation.timeFrom = picker.startDate.format();
        });
        global.jQuery('#reservation-start-date').on('cancel.daterangepicker', function(ev, picker) {
          vm.reservation.timeFrom = null;
          global.jQuery("#reservation-start-date").val('');
        });

      global.jQuery("#reservation-end-date").daterangepicker(
          Object.assign(this.dateTimePickerConfig, {
            startDate: moment(),
            minDate: moment()
          }), 
          function(start, end) {
            vm.reservation.timeTo = start.format(vm.dateTimePickerConfig.locale.format);
          }
      );
        global.jQuery('#reservation-end-date').on('apply.daterangepicker', function(ev, picker) {
          vm.reservation.timeTo = picker.startDate.format();
        });
        global.jQuery('#reservation-end-date').on('cancel.daterangepicker', function(ev, picker) {
          vm.reservation.timeTo = null;
          global.jQuery("#reservation-end-date").val('');
        });
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

      fetchTableList: function (timeFrom, timeTo, minSize) {
        for (var i = 0; i < this.tables.length; i++) {
          this.tables[i].items = [];
        }

        if (this.tableSearch.loading || timeFrom == null || timeTo == null) {
          return;
        }

        this.tableSearch.loading = true;
        fetch(global.App.baseURL + `api/table/free?timeFrom=${encodeURIComponent(timeFrom)}&timeTo=${encodeURIComponent(timeTo)}&minSize=${minSize*1}`, {
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

            this.tableSearch.loading = false;
          })
          .catch(err => global.console.log(err));
      },

      onSubmit: function () {
        this.options.apiError = '';
        this.errors = [];

        this.reservation.tableId = this.reservation.tableId * 1;

        if (this.reservation.customerName.length == 0) {
          this.errors.push('customer_name_length');
        }
        if (!/^([+]?[0-9]{9,11})$/.test(this.reservation.customerPhone)) {
           this.errors.push('customer_phone_wrong_format');
        }
        if (this.reservation.timeFrom === null || this.reservation.timeFrom == '') {
          this.errors.push('invalid_time_from');
        }
        if (this.reservation.timeTo === null || this.reservation.timeTo == '') {
          this.errors.push('invalid_time_to');
        }
        if (moment(this.reservation.timeFrom) > moment(this.reservation.timeTo)) {
          this.errors.push('time_from_bigger_than_time_to');
        }
        if (this.reservation.tableId == null || this.reservation.tableId == NaN) {
          this.errors.push('invalid_table');
        }

        if (this.errors.length > 0) {
          return;
        }

        this.$emit('confirm-callback', Object.assign({}, this.reservation));
      },
      onDismiss: function () {
        this.reservation = Object.assign({}, this.options.reservation);
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('dismiss-callback');
      },
      onDelete: function () {
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('delete-callback', this.reservation.id);
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      },
      'options.reservation': function (val) {
        this.reservation = Object.assign({
          "tableId": null,
          "tableCode": "",
          "timeFrom": null,
          "timeTo": null,
          "customerName": "",
          "customerPhone": ""
        }, val);
        this.tableSearch = {
          "id": null,
          "minSize": 0,
          "loading": false
        };

        var drp1 = global.jQuery("#reservation-start-date").data('daterangepicker');
        var startDate = (val.timeFrom) ? moment(val.timeFrom) : moment();
        drp1.setStartDate(startDate);
        drp1.setEndDate(startDate);

        var drp2 = global.jQuery("#reservation-end-date").data('daterangepicker');
        var endDate = (val.timeTo) ? moment(val.timeTo) : moment();
        drp2.setStartDate(endDate);
        drp2.setEndDate(endDate);
        
        this.fetchTableList(this.reservation.timeFrom, this.reservation.timeTo, this.tableSearch.minSize);
      },
      'reservation.timeFrom': function (val) {
        global.jQuery('#reservation-start-date').val((val) ? moment(val).format(this.dateTimePickerConfig.locale.format) : '');

        this.fetchTableList(this.reservation.timeFrom, this.reservation.timeTo, this.tableSearch.minSize);
      },
      'reservation.timeTo': function (val) {
        global.jQuery('#reservation-end-date').val((val) ? moment(val).format(this.dateTimePickerConfig.locale.format) : '');

        this.fetchTableList(this.reservation.timeFrom, this.reservation.timeTo, this.tableSearch.minSize);
      },
      'tableSearch.minSize': function (val) {
        this.fetchTableList(this.reservation.timeFrom, this.reservation.timeTo, this.tableSearch.minSize);
      },
      'tables': function () {
        this.fetchTableList(this.reservation.timeFrom, this.reservation.timeTo, this.tableSearch.minSize);
      },
      'tableSearch.id': function (val) {
        var table = null;
        for (var i = 0; i < this.tables.length; i++) {
          for (var j = 0; j < this.tables[i].items.length; j++) {
            if (this.tables[i].items[j].id == val) {
              table = this.tables[i].items[j]

              break;
            }
          }
        }
        if (table != null) {
          this.reservation.tableId = table.id;
          this.reservation.tableCode = table.code;
        }
      }
    }
  }
</script>

<style>
  .modal-dialog {
    max-width: 550px;
  }
</style>