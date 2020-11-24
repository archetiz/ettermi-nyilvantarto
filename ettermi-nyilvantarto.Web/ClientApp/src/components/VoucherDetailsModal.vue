<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="voucher-details-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="voucher-details-modal-label">{{ addNew ? "Új kupon hozzáadása" : "Kupon módosítása" }}</h5>
          <button type="button" class="close" v-on:click="onDismiss" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <form>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="voucher-code" class="col-form-label">Kód:</label>
                <input type="text" :class="['form-control', {'is-invalid': error_code_length}]" id="voucher-code" v-model="voucher.code" required :disabled="!addNew">
                <small v-if="error_code_length" class="text-danger">
                A kód megadása kötelező!
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="voucher-discount-threshold" class="col-form-label">Minimum értékhatár:</label>
                <input type="number" :class="['form-control', {'is-invalid': error_discount_threshold_out_of_bounds}]" id="voucher-discount-threshold" v-model="voucher.discountThreshold" required min="0" :disabled="!addNew">
                <small v-if="error_discount_threshold_out_of_bounds" class="text-danger">
                A megadott érték nem lehet negatív!
                </small>
                <small class="text-info">
                Csak ezen értékhatár feletti fizetéseknél váltható be.
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label class="col-form-label">Típus:</label>
                  <div class="custom-control custom-radio">
                    <input type="radio" class="custom-control-input" aria-label="Százalékos kedvezmény" aria-describedby="voucher-type-percentage" v-model="voucherType" id="voucher-type-percentage" value="percentage" checked="checked" :disabled="!addNew">
                    <label class="custom-control-label" for="voucher-type-percentage">Százalékos kedvezmény</label>
                  </div>
                  <div class="custom-control custom-radio">
                    <input type="radio" class="custom-control-input" aria-label="Fix kedvezmény" aria-describedby="voucher-type-amount" v-model="voucherType" id="voucher-type-amount" value="amount" :disabled="!addNew">
                    <label class="custom-control-label" for="voucher-type-amount">Fix kedvezmény</label>
                  </div>
              </div>
            </div>
            <div v-if="voucherType == 'percentage'" class="form-row">
              <div class="form-group col-12">
                <label for="voucher-discount-percentage" class="col-form-label">Százalék:</label>
                <input type="number" :class="['form-control', {'is-invalid': error_discount_percentage_out_of_bounds}]" id="voucher-discount-percentage" v-model="voucher.discountPercentage" required min="1" max="100" :disabled="!addNew">
                <small v-if="error_discount_percentage_out_of_bounds" class="text-danger">
                A megadott értéknek 1 és 100 között kell lennie!
                </small>
                <small class="text-info">
                A megadott érték százalékban értendő (100 = 100%).
                </small>
              </div>
            </div>
            <div v-if="voucherType == 'amount'" class="form-row">
              <div class="form-group col-12">
                <label for="voucher-discount-amount" class="col-form-label">Összeg:</label>
                <input type="number" :class="['form-control', {'is-invalid': error_discount_amount_min}]" id="voucher-discount-amount" v-model="voucher.discountAmount" required min="1" :disabled="!addNew">
                <small v-if="error_discount_amount_min" class="text-danger">
                A megadott értéknek 0-nál nagyobbnak kell lennie!
                </small>
                <small class="text-info">
                A megadott érték forintban értendő (100 = 100 Ft).
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="voucher-start-date" class="col-form-label">Érvényesség kezdete:</label>
                <input type="text" class="form-control" :class="{'is-invalid': error_invalid_active_from}" id="voucher-start-date" maxlength="19" autocomplete="off" :disabled="!addNew">
                <small v-if="error_invalid_active_from" class="text-danger">
                A kezdődátum megadása kötelező.
                </small>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="voucher-end-date" class="col-form-label">Érvényesség vége:</label>
                <input type="text" class="form-control" :class="{'is-invalid': error_invalid_active_to || error_active_from_bigger_than_active_to}" id="voucher-end-date" maxlength="19" autocomplete="off" :disabled="!canEditActiveTo">
                <small v-if="error_invalid_active_to" class="text-danger">
                A befejezés dátumának megadása kötelező.
                </small>
                <small v-if="error_active_from_bigger_than_active_to" class="text-danger">
                A befejezés dátuma nem lehet előbbi a kezdődátumnál.
                </small>
              </div>
            </div>
            <div v-if="error_api" class="form-row">
              <div class="form-group col-12">
                <small class="text-danger">
                Nem sikerült rögzíteni a kupont a rendszerben. A hiba oka: {{ options.apiError }}
                </small>
              </div>
            </div>
          </form>
        </div>
        <div v-if="canEditActiveTo" class="modal-footer">
          <button type="button" class="btn btn-outline-secondary" v-on:click="onDismiss">Mégsem</button>
          <button type="button" class="btn btn-primary" v-on:click="onSubmit">OK</button>
        </div>
        <div v-else class="modal-footer">
          <button type="button" class="btn btn-primary" v-on:click="onDismiss">OK</button>
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
    name: 'voucher-details-modal',

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
            voucher: {},
            apiError: ''
          }
        }
      }
    },

    data() {
      return {
        voucher: {
          "activeFrom":null,
          "activeTo":null,
          "discountThreshold":0
        },
        voucherType: 'percentage',
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
        return (this.voucher.id === undefined);
      },
      canEditActiveTo: function () {
        return this.addNew || moment() < moment(this.voucher.activeTo);
      },
      error_api: function () {
        return this.options.apiError.length > 0;
      },
      error_code_length: function () {
        return this.errors.indexOf('code_length') > -1;
      },
      error_discount_threshold_out_of_bounds: function () {
        return this.errors.indexOf('discount_threshold_out_of_bounds') > -1;
      },
      error_discount_percentage_out_of_bounds: function () {
        return this.errors.indexOf('discount_percentage_out_of_bounds') > -1;
      },
      error_discount_amount_min: function () {
        return this.errors.indexOf('discount_amount_min') > -1;
      },
      error_invalid_active_from: function () {
        return this.errors.indexOf('invalid_active_from') > -1;
      },
      error_invalid_active_to: function () {
        return this.errors.indexOf('invalid_active_to') > -1;
      },
      error_active_from_bigger_than_active_to: function () {
        return this.errors.indexOf('active_from_bigger_than_active_to') > -1;
      }
    },

    mounted: function () {
      let vm = this;
      global.jQuery("#voucher-start-date").daterangepicker(
          Object.assign(this.dateTimePickerConfig, {startDate: moment()})
      );
        global.jQuery('#voucher-start-date').on('apply.daterangepicker', function(ev, picker) {
          vm.voucher.activeFrom = picker.startDate.format(picker.locale.format);
        });
        global.jQuery('#voucher-start-date').on('cancel.daterangepicker', function(ev, picker) {
          vm.voucher.activeFrom = '';
          global.jQuery("#voucher-start-date").val('');
        });

      global.jQuery("#voucher-end-date").daterangepicker(
          Object.assign(this.dateTimePickerConfig, {startDate: moment()}), 
          function(start, end) {
            vm.voucher.activeTo = start.format(vm.dateTimePickerConfig.locale.format);
          }
      );
        global.jQuery('#voucher-end-date').on('apply.daterangepicker', function(ev, picker) {
          vm.voucher.activeTo = picker.startDate.format(picker.locale.format);
        });
        global.jQuery('#voucher-end-date').on('cancel.daterangepicker', function(ev, picker) {
          vm.voucher.activeTo = '';
          global.jQuery("#voucher-end-date").val('');
        });
    },

    methods: {
      onSubmit: function () {
        this.options.apiError = '';
        this.errors = [];

        if (this.voucherType == 'percentage') {
          this.voucher.discountAmount = 0;
        } else {
          this.voucher.discountPercentage = 0;
        }

        if (this.voucher.code.length == 0) {
          this.errors.push('code_length');
        }
        if (this.voucher.discountThreshold < 0) {
          this.errors.push('discount_threshold_out_of_bounds');
        }
        if (this.voucherType == 'percentage' && (this.voucher.discountPercentage <= 0 || 100 < this.voucher.discountPercentage)) {
          this.errors.push('discount_percentage_out_of_bounds');
        }
        if (this.voucherType == 'amount' && this.voucher.discountAmount <= 0) {
          this.errors.push('discount_amount_min');
        }
        if (this.voucher.activeFrom === null) {
          this.errors.push('invalid_active_from');
        }
        if (this.voucher.activeTo === null) {
          this.errors.push('invalid_active_to');
        }
        if (moment(this.voucher.activeFrom) > moment(this.voucher.activeTo)) {
          this.errors.push('active_from_bigger_than_active_to');
        }

        if (this.errors.length > 0) {
          return;
        }

        // if everything is allright, convert dates to system format
        var data = Object.assign(this.voucher, {
          activeFrom: moment(this.voucher.activeFrom).format(),
          activeTo: moment(this.voucher.activeTo).format()
        });

        this.$emit('success-callback', data);
      },
      onDismiss: function () {
        this.voucher = Object.assign({}, this.options.voucher);
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('dismiss-callback');
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      },
      'options.voucher': function (val) {
        this.voucher = Object.assign({
          "activeFrom":null,
          "activeTo":null,
          "code":'',
          "discountThreshold":0
        }, val);

        if (this.voucher.discountPercentage > 0) {
          this.voucherType = 'percentage';
        } else {
          this.voucherType = 'amount';
        }

        var drp1 = global.jQuery("#voucher-start-date").data('daterangepicker');
        var startDate = (val.activeFrom) ? moment(val.activeFrom) : moment();
        drp1.setStartDate(startDate);
        drp1.setEndDate(startDate);

        var drp2 = global.jQuery("#voucher-end-date").data('daterangepicker');
        var endDate = (val.activeTo) ? moment(val.activeTo) : moment();
        drp2.setStartDate(endDate);
        drp2.setEndDate(endDate);
      },
      'voucher.activeFrom': function (val) {
        global.jQuery('#voucher-start-date').val((val) ? moment(val).format(this.dateTimePickerConfig.locale.format) : '');
      },
      'voucher.activeTo': function (val) {
        global.jQuery('#voucher-end-date').val((val) ? moment(val).format(this.dateTimePickerConfig.locale.format) : '');
      }
    }
  }
</script>

<style>
  .modal-dialog {
    max-width: 550px;
  }
</style>