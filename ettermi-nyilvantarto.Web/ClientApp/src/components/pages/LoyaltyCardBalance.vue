<template>
  <div>
    <div class="container">
      <div class="row loyalty-card-balance">
        <div class="d-none d-lg-block col-lg-3"></div>
        <div class="content-box col-12 col-lg-6">
          <div class="row">
            <div class="col-12">
              <h3 class="text-center">Hűségkártya egyenleg lekérdezése</h3>
            </div>
          </div>
          <div class="row content-box form-row">
            <div class="form-group col-12 col-lg-9">
              <input type="text" class="form-control" v-model="searchQuery" required>
            </div>
            <div class="form-group col-12 col-lg-3">
              <button type="button" class="btn btn-primary btn-block" v-on:click="getBalance">Keresés</button>
            </div>
          </div>
          <div v-if="error_api" class="row content-box form-row">
            <div class="form-group col-12">
              <small class="text-danger">
              Nem sikerült lekérdezni a hűségkártya egyenlegét. A hiba oka: {{ apiError }}
              </small>
            </div>
          </div>
          <div v-if="loyaltyCard.balance !== null" class="row content-box balance-box">
            <div class="col-12">
              <div class="row balance-box-row">
                <div class="col-12 col-lg-5">
                  Kártyaszám:
                </div>
                <div class="col-12 col-lg-7">
                  <span class="font-weight-bold">{{ loyaltyCard.number }}</span>
                </div>
              </div>
              <div class="row balance-box-row">
                <div class="col-12 col-lg-5">
                  Egyenleg:
                </div>
                <div class="col-12 col-lg-7">
                  <span class="font-weight-bold">{{ formatMoney(loyaltyCard.balance, 0, ',', '.') }} pont</span>
                </div>
              </div>
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
        searchQuery: '',
        loyaltyCard: {
          number: '',
          balance: null
        }
      }
    },

    computed: {
      error_api: function () {
        return this.apiError.length > 0;
      }
    },

    methods: {
      getBalance: function () {
        this.apiError = '';
        this.loyaltyCard.balance = null;

        fetch(global.App.baseURL + `api/loyalty/card/${this.searchQuery}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            if (res.resultError === undefined) {
              this.loyaltyCard.number = this.searchQuery;
              this.loyaltyCard.balance = res.balance;

              return;
            }

            this.apiError = res.resultError;
          })
          .catch(err => global.console.log(err));
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