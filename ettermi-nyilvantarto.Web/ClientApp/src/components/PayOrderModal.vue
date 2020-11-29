<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="pay-order-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="pay-order-modal-label">Fizetés</h5>
          <button type="button" class="close" v-on:click="onHide" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <div v-if="!summaryDisplayed">
            <div class="form-row">
              <div class="form-group col-12">
                <label for="voucher-code" class="col-form-label">Kuponkód:</label>
                <input type="text" class="form-control" id="voucher-code" v-model="voucherCode" required>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="loyalty-card-number" class="col-form-label">Hűségkártya:</label>
                <input type="text" class="form-control" id="loyalty-card-number" v-model="loyaltyCardNumber" required>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="should-reedem-points" class="col-form-label">Beszámítsuk az egyenleget?</label>&nbsp;&nbsp;&nbsp;
                <input type="checkbox" id="should-reedem-points" v-model="shouldReedemPoints">
              </div>
            </div>
          </div>
          <div v-if="summaryDisplayed">
            <div class="row mb-2">
              <div class="col-6">Rendelések összesen</div>
              <div class="col-6">{{ formatMoney(summary.fullPrice, 0, ',', '.') }} Ft</div>
            </div>
            <div v-if="summary.voucherCode" class="row mb-2">
              <div class="col-6">Megadott kupon kódja</div>
              <div class="col-6">{{ summary.voucherCode }}</div>
            </div>
            <div v-if="summary.loyaltyCardNumber">
              <div class="row mb-2">
                <div class="col-6">Megadott hűségkártya száma</div>
                <div class="col-6">{{ summary.loyaltyCardNumber }}</div>
              </div>
              <div class="row mb-2">
                <div class="col-6">Beváltott pontok összesen</div>
                <div class="col-6">{{ summary.usedLoyaltyPoints }} pont</div>
              </div>
            </div>
            <div class="row mb-2">
              <div class="col-6"><h5>Fizetendő végösszeg</h5></div>
              <div class="col-6"><h5>{{ formatMoney(summary.voucherTotalDiscountAmount, 0, ',', '.') }} Ft</h5></div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="billing-name" class="col-form-label">Számlázási név:</label>
                <input type="text" class="form-control" id="billing-name" v-model="billing.customerName" required>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="billing-tax" class="col-form-label">Adószám:</label>
                <input type="text" class="form-control" id="billing-tax" v-model="billing.customerTaxNumber" required>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="billing-address" class="col-form-label">Cím:</label>
                <input type="text" class="form-control" id="billing-address" v-model="billing.customerAddress" required>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="billing-phone" class="col-form-label">Telefonszám:</label>
                <input type="text" class="form-control" id="billing-phone" v-model="billing.customerPhoneNumber" required>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="billing-name" class="col-form-label">E-mail cím:</label>
                <input type="text" class="form-control" id="billing-name" v-model="billing.customerEmail" required>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <label for="billing-payment-method" class="col-form-label">Fizetési mód:</label>
                <select id="billing-payment-method" class="form-control custom-select" v-model="billing.paymentMethod">
                  <option value="0">Készpénz</option>
                  <option value="1">Bankkártya</option>
                </select>
              </div>
            </div>
          </div>
          <div v-if="apiError" class="form-row">
            <div class="form-group col-12">
              <small class="text-danger">
              Nem sikerült végrehajtani az összegzést. A hiba oka: {{ apiError }}
              </small>
            </div>
          </div>
          <div v-if="summaryDisplayed" class="form-row">
            <div class="col-12">
              <div class="alert alert-primary" role="alert">
                A fizetés lezárásához kattints a <b><i>Fizetés tényének rögzítése</i></b> gombra!
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button v-if="!summaryDisplayed" type="button" class="btn btn-primary" v-on:click="onSubmit">Összegző betöltése</button>
          <button v-else type="button" class="btn btn-primary" v-on:click="onSubmit">Fizetés tényének rögzítése</button>
        </div>
      </div>
    </div>
  </div>
</template>


<script>

  export default {
    name: 'pay-order-modal',

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
            orderSessionId: 0
          }
        }
      }
    },

    data() {
      return {
        formatMoney: global.formatMoney,

        summaryDisplayed: false,
        voucherCode: '',
        shouldReedemPoints: false,
        loyaltyCardNumber: '',
        summary: {
          fullPrice: 0,
          finalPrice: 0,
          loyaltyCardNumber: 0,
          usedLoyaltyPoints: 0,
          voucherCode: "",
          voucherTotalDiscountAmount: 0
        },
        billing: {
          customerName: "",
          customerTaxNumber: "",
          customerAddress: "",
          customerPhoneNumber: "",
          customerEmail: "",
          paymentMethod: 0
        },

        apiError: ''
      }
    },

    methods: {
      onSubmit: function () {
        if (!this.summaryDisplayed) {
          fetch(global.App.baseURL +  `api/orders/${this.options.orderSessionId}/summary?VoucherCode=${encodeURI(this.voucherCode)}&LoyaltyCardNumber=${this.loyaltyCardNumber}&ShouldRedeemPoints=${this.shouldReedemPoints}`, {
              headers: {
                'Accept': 'application/json',
                'content-type': 'application/json'
              },
              credentials: 'same-origin'
            })
            .then(res => global.handleNetworkError(res, this))
            .then(res => {
              res.json().then(res => {
                if (res.resultError !== undefined) {
                  this.apiError = res.resultError;
                  return;
                }

                this.summary = res;
                if (this.summary.voucherTotalDiscountAmount == 0) {
                  this.summary.voucherTotalDiscountAmount = this.summary.fullPrice;
                }
                this.summaryDisplayed = true;
              });
            })
            .catch(err => console.log(err));
        } else {
          var rqBody = {
                voucherCode: this.voucherCode,
                shouldRedeemPoints: this.shouldReedemPoints,
                customerName: this.billing.customerName,
                customerTaxNumber: this.billing.customerTaxNumber,
                customerAddress: this.billing.customerAddress,
                customerPhoneNumber: this.billing.customerPhoneNumber,
                customerEmail: this.billing.customerEmail,
                paymentMethod: this.billing.paymentMethod*1
              };
          if (this.loyaltyCardNumber.length > 0) {
            rqBody = Object.assign(rqBody, {loyaltyCardNumber: this.loyaltyCardNumber*1});
          }
          
          fetch(global.App.baseURL +  `api/orders/${this.options.orderSessionId}/pay`, {
              method: 'post',
              headers: {
                'Accept': 'application/json',
                'content-type': 'application/json'
              },
              credentials: 'same-origin',
              body: JSON.stringify(rqBody)
            })
            .then(res => global.handleNetworkError(res, this))
            .then(res => {
              if (res.status == 200) {
                this.$emit('success-callback');
                return;
              }

              res.json().then(res => {
                if (res.resultError !== undefined) {
                  this.apiError = res.resultError;
                  return;
                }
            })
            .catch(err => console.log(err));
          });
        }
      },

      onHide: function () {
        this.$emit('dismiss-callback');
      }
    },

    watch: {
      'options.isHidden': function (val) {
        this.summaryDisplayed = false;
        this.voucherCode = '';
        this.shouldReedemPoints = false;
        this.loyaltyCardNumber = '';
        this.summary = {
          fullPrice: 0,
          finalPrice: 0,
          loyaltyCardNumber: 0,
          usedLoyaltyPoints: 0,
          voucherCode: "",
          voucherTotalDiscountAmount: 0
        };
        this.apiError = '';

        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      }
    }
  }
</script>

<style>
  .modal-dialog {
    max-width: 550px;
  }
</style>